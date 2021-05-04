using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    [System.Serializable]
    public class ParticleBoneWeight
    {
        public int particleIndex;
        public Vector3 localPosition;
        public float weight;

        public ParticleBoneWeight(int i, Vector3 p, float w)
        {
            particleIndex = i;
            localPosition = p;
            weight = w;
        }
    }

    [System.Serializable]
    public class WeightList
    {
        public int boneIndex; // for transform
        public List<ParticleBoneWeight> weights = new List<ParticleBoneWeight>();
    }

    /**
     * Animate a Flex object by trying to mimic an animation as it plays.
     * Adjust the velocities of particles to nudge them into the positions they should have
     * based on the current state of the mesh in the animation. But rather than re-rasterize
     * the mesh every frame, create a mapping of particles to vertices 
     */
    [RequireComponent(typeof(FlexSoftActor))]
    public class FlexAnimSoftSkin : MonoBehaviour
    {
        public SkinnedMeshRenderer referenceAnimSkin; // the original animation,
        // i.e. the original model playing the animation (potentially offscreen)

        private FlexSoftActor m_actor;
        /// caches for values copied in and out of the flex subsystem:
        private Vector4[] m_particles;
        private Vector3[] m_velocities;

        /// Particle to Vertex / Bones relations that are calculated once on first run:

        // The offset from the nearest Vertex to "its" particle
        // (Note: we might not need that, if we change ParticleBoneWeight to just store the offset to the
        // particle rather than to the vertex? Also could be an array instead of a list for small performance boost)
        private List<Vector3> vertOffsetVectors = new List<Vector3>();
        // for each bone of the original mesh, store a list of weights for the particles it should influence:
        public WeightList[] particleBoneWeights; // (only public for DEBUG)

        /// Used when updating particle positions (in local space) and calculating velocities:
        private Vector3[] particlePositions;
        private Vector3[] particleDisplacements;

        private bool firstRun = true;

        /// DEBUG:
        private bool debugCollect = false;
        private string debugStr = "";

        private void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
            if (referenceAnimSkin == null)
            {
                Debug.LogError("FlexAnimSoftSkin cannot work without a reference animation (Note: NOT the Skin of the Flex Object itself!)");
            }
        }

        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            // local caches used during the update of particle positions based on the reference animation:
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
            particleDisplacements = new Vector3[m_particles.Length];
            particlePositions = new Vector3[m_particles.Length];
            string debugOutput = "Animating " + m_actor.name + " based on Skin " + referenceAnimSkin.name;
            debugOutput += "\n Indices: " + m_actor.indexCount + " from " + m_actor.indices[0] + " to " + m_actor.indices[m_actor.indexCount - 1];
            debugOutput += "\n mass Scale: " + m_actor.massScale;
            debugOutput += "\n particle Group: " + m_actor.particleGroup;
            debugOutput += "\n reference Shape: " + m_actor.asset.referenceShape;
            debugOutput += "\n shapeindices length: " + m_actor.asset.shapeCenters.Length;
            debugOutput += "\n fixed particles: " + m_actor.asset.fixedParticles.Length;
            Debug.Log(debugOutput);
            if (m_actor.asset.referenceShape != -1)
            {
                Debug.Log("Unsetting reference shape for " + m_actor.name + " to avoid FlexSoftActor automatically moving the transform" +
                        " as this would interfere with tracking the animation.");
                this.m_actor.asset.referenceShape = -1;
            }
            StartCoroutine(DebugDump());
        }

        IEnumerator DebugDump()
        {
            yield return null;
            while (Application.isPlaying)
            {
                debugStr = "";
                debugCollect = true;
                while (debugStr == "")
                {
                    yield return null;
                }
                debugCollect = false;
                Debug.Log("DebugDump for FlexAnimSoftSkin of " + m_actor.name + debugStr);
                yield return new WaitForSeconds(0.5f);
            }
        }

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            // Fill a local copy of the particles: NOTE: the particle coordinates are in world space
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            if (firstRun)
            {
                firstRun = false; // only run this once:
                // The mesh vertices are in local space:
                Vector3[] cachedMeshVertices = referenceAnimSkin.sharedMesh.vertices;
                // list of the index of the mesh vertex that is closest to each Flex particle in the rest state
                // the bone weights of that vertex will be used to animate that particle:
                List<int> nearestVertexIndexForParticle = new List<int>();
                // Use the particle position transformed to local space to setup the needed mappings:
                foreach (var i in m_particles)
                {
                    Vector3 localPos = this.transform.InverseTransformPoint(i);
                    int nearestIndexforParticle = GetNearestVertIndex(localPos, cachedMeshVertices);
                    Vector3 VertOffset = localPos - cachedMeshVertices[nearestIndexforParticle];
                    vertOffsetVectors.Add(VertOffset);
                    nearestVertexIndexForParticle.Add(nearestIndexforParticle);
                }
                SetBoneWeights(cachedMeshVertices, nearestVertexIndexForParticle);
            }
            else // no update on firstRun! (that assumes that the animation starts from the rest position)
            {
                UpdateParticlePositions(_particleData);
            }
        }

        public void UpdateParticlePositions(FlexContainer.ParticleData _particleData)
        {
            // In the following, m_particles and m_velocities always work in world space,
            // while the calculation for particlePositions are done based on the bindposes and the bones of
            // the reference animation skin (which is in a different location in the world)
            // so they need to then be moved to the world position of this gameobject
            // before calculating the particleDisplacements in world space again so it can be applied to the particles!

            // "Zero out" all particle postions first
            for (int i = 0; i < particlePositions.Length; i++)
            {
                particlePositions[i] = Vector3.zero;
            }
            // For each bone, check all particles it should affect and add the effect to that particlePosition:
            foreach (WeightList wList in particleBoneWeights)
            {
                foreach (ParticleBoneWeight pbw in wList.weights)
                {
                    // Use the current transform of the bone to add its contribution to the new particle position:
                    Transform t = referenceAnimSkin.bones[wList.boneIndex];
                    // It contributes its full hierarchy transform, i.e. what it considers local space to world space,
                    // moving the vertex of the mesh to where it should be in world space for the reference animation right now.
                    // Adding the VertOffsetVector gives us the location of the particle according to this bone right now
                    particlePositions[pbw.particleIndex] += pbw.weight * (t.TransformPoint(pbw.localPosition) + vertOffsetVectors[pbw.particleIndex]);

                }
            }
            // We now have the particlePositions as they should be based on the animation, in world space,
            // BUT at the position of the reference animation's root bone in world space.
            Vector3 _tempVector3Local;
            Vector3 _tempVector3Global; // (split into three different temps for DEBUG)
            Vector3 _tempVector3Diff;
            Matrix4x4 localRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.localRotation));
            for (int i = 0; i < particlePositions.Length; i++)
            {
                // Now: First move it back to local space relative where its root bone is (i.e. to its parent).
                _tempVector3Local = referenceAnimSkin.rootBone.parent.InverseTransformPoint(particlePositions[i]);
                // HACK: undo rotation of the FlexObject's GameObject needed to align flex model created from mesh
                // with mesh as it is oriented after import (Ideally they should align of course)
                // There might be a cleaner way to do that.
                _tempVector3Local = localRotation.MultiplyPoint3x4(_tempVector3Local);
                // And then move it back to world space relative to this GameObject
                // and calculate how far it is from its intended (animation-)position:
                // (casting a Vector4 to a Vector3 automatically discards the w component)
                _tempVector3Global = this.transform.TransformPoint(_tempVector3Local);
                _tempVector3Diff = _tempVector3Global - ((Vector3) m_particles[i]);
                particleDisplacements[i].x = _tempVector3Diff.x;
                particleDisplacements[i].y = _tempVector3Diff.y;
                particleDisplacements[i].z = _tempVector3Diff.z;
                // DEBUG:
                if (debugCollect)
                {
                    if (i % 1000 == 0)
                    {
                        debugStr += string.Format("\nparticle {0}  \treferenceLocalPos {1}  \tglobalPos {2}  \tparticlePos {3}  \tdiff {4}",
                                i, _tempVector3Local, _tempVector3Global, m_particles[i], _tempVector3Diff);
                    }
                }
            }
            // Now apply an appropriate velocity change to nudge the particle into the right direction:
            // Get/Set all velocities at the same time, as this is the most performant method:
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            for (int i = 0; i < particleDisplacements.Length; i++)
            {
                // Note: we replicate roughly what ApplyImpulses in Actor would do, i.e. scale by weight:
                // impulse divided by particle mass (which is 1/w):
                //m_actor.ApplyImpulse(particleDisplacementVector[i], i);
                m_velocities[i] += particleDisplacements[i] * m_particles[i].w;// * 0.1f ; // DEBUG: temp. small factor for clarity
                // TODO: find physically appropriate factor!
            }
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
        }

        private void SetBoneWeights(Vector3[] cachedMeshVertices, List<int> nearestVertexIndexForParticle)
        {
            Matrix4x4[] cachedBindposes = referenceAnimSkin.sharedMesh.bindposes;
            BoneWeight[] cachedBoneWeights = referenceAnimSkin.sharedMesh.boneWeights;
            // Make a WeightList-list, one for each bone in the skinned mesh
            WeightList[] boneWeights = new WeightList[referenceAnimSkin.bones.Length];
            for (int i = 0; i < referenceAnimSkin.bones.Length; i++)
            {
                boneWeights[i] = new WeightList { boneIndex = i };
            }
            // for every particle, add up to 4 appropriate VertexWeights based on the bones affecting the nearest vertex
            for (int particleIndex = 0; particleIndex < nearestVertexIndexForParticle.Count; particleIndex++)
            {
                int nearestVertexIndex = nearestVertexIndexForParticle[particleIndex];
                Vector3 vertexPos = cachedMeshVertices[nearestVertexIndex];
                // for each non-zero BoneWeight of that vertex, add a corresponding VertexWeight,
                // remembering the offset from the bone to the vertex in bone-local space
                // i.e. after multiplying with the corresponding bindpose (so it can be used on the particle later)
                BoneWeight bw = cachedBoneWeights[nearestVertexIndex];
                if (bw.weight0 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex0].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex0].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight0));
                }
                if (bw.weight1 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex1].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex1].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight1));
                }
                if (bw.weight2 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex2].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex2].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight2));
                }
                if (bw.weight3 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex3].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex3].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight3));
                }
                // DEBUG:
                if (particleIndex % 1000 == 0)
                {
                    var debugPBW = "";
                    for (int i = 0; i < referenceAnimSkin.bones.Length; i++)
                    {
                        foreach (ParticleBoneWeight pbw in boneWeights[i].weights) {
                            if (pbw.particleIndex == particleIndex)
                            {
                                debugPBW += string.Format("bone {0} localPos {1} weight {2} ",
                                        i, pbw.localPosition, pbw.weight);
                            }
                        }
                    }
                    Debug.Log(string.Format("particle {0} nearest vertex {1} at {2}" +
                            "\nparticle world pos {3} local pos {4}" +
                            "\nanimation bone weight {5}\nparticle bone weights {6}" +
                            "\nreference anim skin bones count {7} bindposes {8} boneweights {9}",
                            particleIndex, nearestVertexIndex, vertexPos,
                            m_particles[particleIndex], this.transform.InverseTransformPoint(m_particles[particleIndex]),
                            JsonUtility.ToJson(bw), debugPBW,
                            referenceAnimSkin.bones.Length, cachedBindposes.Length, cachedBoneWeights.Length));
                }
            }
            particleBoneWeights = boneWeights;
        }

        private int GetNearestVertIndex(Vector3 particlePos, Vector3[] cachedVertices)
        {
            float nearestDist = float.MaxValue;
            int nearestIndex = -1;
            for (int i = 0; i < cachedVertices.Length; i++)
            {
                float dist = Vector3.Distance(particlePos, cachedVertices[i]);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestIndex = i;
                }
            }
            return nearestIndex;
        }
    }
}
