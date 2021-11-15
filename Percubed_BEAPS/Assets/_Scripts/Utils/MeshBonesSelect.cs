using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;
using Percubed.Flex;

// Raycast into the particles lattice, find the nearest shape center
// Highlight it on the screen
// (Maybe later: Find all vertices nearest to each of the shapes selected using the neighborindex highlighter)
public class MeshBonesSelect : MonoBehaviour
{
    public Vector3 selectedBone; // only for debugging
    //public FlexAnimSoftSkin flexAnim;
    public FlexSoftActor m_softActor;
    private Vector4[] m_particles; // local cache of positions
    Vector3[] shapeBones;
    public List<int> selectedBonesIndices;
    float particleRadius;
    BoneWeight[] m_boneWeights = new BoneWeight[0];
    public Dictionary<Transform, List<int>> boneToVerticesDict;
    public bool doneSelecting = false;
    void Awake()
    {
        if (m_softActor == null) {
            m_softActor = GetComponent<FlexSoftActor>();
        }
        boneToVerticesDict = new Dictionary<Transform, List<int>>();
    }

    void Start()
    {
        m_boneWeights = new BoneWeight[m_softActor.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights.Length];
        for (int i = 0; i < m_boneWeights.Length; i++)
        {
            m_boneWeights[i] = m_softActor.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights[i];
        }
        m_particles = new Vector4[m_softActor.indexCount];
        shapeBones = m_softActor.asset.shapeCenters;
        string shapeDebug = "";
        foreach (Vector3 sc in shapeBones) {
            shapeDebug += sc.ToString();
        }
        print(shapeDebug);
        particleRadius = m_softActor.asset.particleSpacing;
        m_softActor.onFlexUpdate += OnFlexUpdate;
    }

    void OnFlexUpdate(FlexContainer.ParticleData _particleData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("Mousedown: Find a shape center close to click");
            _particleData.GetParticles(m_softActor.indices[0], m_softActor.indexCount, m_particles);
            Vector3? foundParticle = FindParticle(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (foundParticle != null) {
                Vector3 localParticle = this.transform.InverseTransformPoint((Vector3) foundParticle);
                print("foundParticle's world position: " + foundParticle + " local: " + localParticle);
                int shapeCenterIndex = PickShapeCenter(localParticle);
                findVerticesBoneWeight(shapeCenterIndex);
                //Important: Will need to store this dictionary in XML using serializable formats, for future use
                doneSelecting = true;
                print("shapeCenter's index " + shapeCenterIndex + " position: " + selectedBone);
                GameObject sBone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sBone.transform.parent = this.gameObject.transform;
                sBone.transform.localPosition = selectedBone;
                sBone.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                if (boneToVerticesDict != null)
                {
                    print("dictionary is not empty");
                    foreach (var x in boneToVerticesDict)
                    {
                        print("Bone: " + x.Key + " List of associated vertices: ");
                        foreach (int i in x.Value)
                        {
                            print("vert index: " + i);
                        }
                    }
                }
            }
        }
    }
    Vector3? FindParticle(Ray ray)
    {
        print("ray origin" + ray.origin);
        print("ray direction" + ray.direction);
        int pickedParticleIndex = PickedParticle(ray.origin, ray.direction, m_particles, particleRadius);
        print("index of picked particle: " + pickedParticleIndex);
        if (pickedParticleIndex == -1) {
            return null;
        }
        return (Vector3) m_particles[pickedParticleIndex];
    }

    int PickedParticle(Vector3 origin, Vector3 dir, Vector4[] particles, float radius) {
        float maxDistSq = radius;
        float minDot = float.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < particles.Length; ++i) {
            Vector3 particleDir = (Vector3)particles[i] - origin;
            float dotProduct = Vector3.Dot(particleDir, dir);
            if (dotProduct > 0.0f) {
                Vector3 perp = particleDir - dotProduct * dir;
                float dSq = perp.sqrMagnitude;
                if (dSq < maxDistSq && dotProduct < minDot) {
                    minDot = dotProduct;
                    minIndex = i;
                }
            }
        }
        return minIndex;
    }

    int PickShapeCenter(Vector3 particle)
    {
        float minDist = Mathf.Infinity;
        int minIndex = -1;
        for (int i = 0; i < shapeBones.Length; ++i)
        {
            float dist = Vector3.Distance(particle, shapeBones[i]); 
            if (dist < minDist)
            {
                minDist = dist;
                minIndex = i;
            }
        }
        selectedBone = shapeBones[minIndex];
        return minIndex;
    }

    void findVerticesBoneWeight(int selectedBone)
    {
        if (selectedBone != -1)
        {
            for (int j = 0; j < m_boneWeights.Length; j++)
            {
                if (selectedBone == m_boneWeights[j].boneIndex0 && m_boneWeights[j].weight0 > 0)
                {
                    addBoneVert(selectedBone, j);
                }
                else if (selectedBone == m_boneWeights[j].boneIndex1 && m_boneWeights[j].weight1 > 0)
                {
                    addBoneVert(selectedBone, j);
                }
                else if (selectedBone == m_boneWeights[j].boneIndex2 && m_boneWeights[j].weight2 > 0)
                {
                    addBoneVert(selectedBone, j);
                }
                else if (selectedBone == m_boneWeights[j].boneIndex3 && m_boneWeights[j].weight3 > 0)
                {
                    addBoneVert(selectedBone, j);
                }
            }
        }

    }

    void addBoneVert(int boneIndex, int vertIndex)
    {
        if (boneToVerticesDict.ContainsKey(m_softActor.GetComponent<SkinnedMeshRenderer>().bones[boneIndex]))
        {
            boneToVerticesDict[m_softActor.GetComponent<SkinnedMeshRenderer>().bones[boneIndex]].Add(vertIndex);
        }
        else if (!boneToVerticesDict.ContainsKey(m_softActor.GetComponent<SkinnedMeshRenderer>().bones[boneIndex]))
        {
            boneToVerticesDict.Add(m_softActor.GetComponent<SkinnedMeshRenderer>().bones[boneIndex], new List<int>());
            boneToVerticesDict[m_softActor.GetComponent<SkinnedMeshRenderer>().bones[boneIndex]].Add(vertIndex);
        }
        //return boneToVerticesDict;
    }
}

