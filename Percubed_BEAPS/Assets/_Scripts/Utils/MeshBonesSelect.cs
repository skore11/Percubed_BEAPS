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
    int shapeBonesOffset;
    public List<int> selectedBonesIndices;
    float particleRadius;
    BoneWeight[] m_boneWeights;
    public Dictionary<int, List<int>> shapeIndexToVerticesDict;
    public bool doneSelecting = false;
    public bool selectBones = false;
    public GameObject shapePrefab;
    
    void Awake()
    {
        if (m_softActor == null) {
            m_softActor = GetComponent<FlexSoftActor>();
        }
        shapeIndexToVerticesDict = new Dictionary<int, List<int>>();
    }

    void Start()
    {
        m_boneWeights = m_softActor.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights;
        m_particles = new Vector4[m_softActor.indexCount];
        shapeBones = m_softActor.asset.shapeCenters;
        string shapeDebug = "Shape Centers: ";
        foreach (Vector3 sc in shapeBones) {
            shapeDebug += sc.ToString();
        }
        print(shapeDebug);
        int numBones = m_softActor.GetComponent<SkinnedMeshRenderer>().bones.Length;
        shapeBonesOffset = numBones - shapeBones.Length;
        print(string.Format("No of shapes {0}, no of bones on SMeshRenderer {1}, offset {2}",
                shapeBones.Length, numBones, shapeBonesOffset));
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
            if (foundParticle != null)
            {
                Vector3 localParticle = m_softActor.transform.InverseTransformPoint((Vector3)foundParticle);
                print("foundParticle's world position: " + foundParticle + " local: " + localParticle);
                int shapeCenterIndex = PickShapeCenter(localParticle);
                findVerticesfromBoneWeight(shapeCenterIndex);
                //Important: Will need to store this dictionary in XML using serializable formats, for future use
                doneSelecting = true;
                print("shapeCenter's index " + shapeCenterIndex + " position: " + selectedBone);
                GameObject sBone = Instantiate(shapePrefab, m_softActor.transform);
                sBone.transform.localPosition = selectedBone;
                sBone.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                if (shapeIndexToVerticesDict.Count > 0)
                {
                    string debugMsg = string.Format("shapeIndexToVerticesDict with {0} elements:", shapeIndexToVerticesDict.Count);
                    foreach (var x in shapeIndexToVerticesDict)
                    {
                        debugMsg += string.Format("\nBone: {0} List of associated vertex indices:\n", x.Key);
                        foreach (int i in x.Value)
                        {
                            debugMsg += i + " ";
                        }
                    }
                    print(debugMsg);
                }
                else
                {
                    print("Found a shape center but shapeIndexToVerticesDict is empty!");
                }
            }
        }
        
        
    }
    Vector3? FindParticle(Ray ray)
    {
        int pickedParticleIndex = PickedParticle(ray.origin, ray.direction, m_particles, particleRadius);
        print(string.Format("Ray for picking particle, origin {0} direction {1}. Index of picked particle: {2}",
                ray.origin, ray.direction, pickedParticleIndex));
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

    void findVerticesfromBoneWeight(int shapeIndex)
    {
        if (shapeIndex != -1)
        {
            int correspondingBoneIndex = shapeIndex + shapeBonesOffset;
            for (int j = 0; j < m_boneWeights.Length; j++)
            {
                BoneWeight bw = m_boneWeights[j];
                if (correspondingBoneIndex == bw.boneIndex0 && bw.weight0 > 0)
                {
                    addShapeVert(shapeIndex, j);
                }
                else if (correspondingBoneIndex == bw.boneIndex1 && bw.weight1 > 0)
                {
                    addShapeVert(shapeIndex, j);
                }
                else if (correspondingBoneIndex == bw.boneIndex2 && bw.weight2 > 0)
                {
                    addShapeVert(shapeIndex, j);
                }
                else if (correspondingBoneIndex == bw.boneIndex3 && bw.weight3 > 0)
                {
                    addShapeVert(shapeIndex, j);
                }
            }
        }
    }

    void addShapeVert(int shapeIndex, int vertIndex)
    {
        if (shapeIndexToVerticesDict.ContainsKey(shapeIndex))
        {
            shapeIndexToVerticesDict[shapeIndex].Add(vertIndex);
        }
        else if (!shapeIndexToVerticesDict.ContainsKey(shapeIndex))
        {
            shapeIndexToVerticesDict.Add(shapeIndex, new List<int>());
            shapeIndexToVerticesDict[shapeIndex].Add(vertIndex);
        }
    }
}

