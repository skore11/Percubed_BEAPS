using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

//Find the set of vertices for a bone index from the boneweights array of a rendered mesh
public class FindVerticesBoneWeights : MonoBehaviour
{
    public GameObject m_object;
    SkinnedMeshRenderer flexCloneMesh;
    public MeshBonesSelect m_selectedBones;
    public int testBoneIndex = -1;
    BoneWeight[] m_boneWeights = new BoneWeight[0];
    Dictionary<Transform, List<int>> boneToVerticesDict;
    // Start is called before the first frame update
    void Start()
    {
        flexCloneMesh = m_object.GetComponent<SkinnedMeshRenderer>();
        m_boneWeights = new BoneWeight[flexCloneMesh.sharedMesh.boneWeights.Length];
        for (int i = 0; i < m_boneWeights.Length; i++)
        {
            m_boneWeights[i] = flexCloneMesh.sharedMesh.boneWeights[i];
        }

       

        // Update is called once per frame
        void Update()
        {
            if (m_selectedBones.doneSelect)
            {
                foreach (int boneIndex in m_selectedBones.selectedBonesIndices)
                {
                    for (int i = 0; i < flexCloneMesh.sharedMesh.vertices.Length; i++)
                    {
                        if (boneIndex != -1)
                        {
                            for (int j = 0; j < m_boneWeights.Length; j++)
                            {
                                if (boneIndex == m_boneWeights[j].boneIndex0 && m_boneWeights[j].weight0 > 0)
                                {
                                    if (boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                    else if (!boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict.Add(flexCloneMesh.bones[boneIndex], new List<int>());
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                }
                                else if (boneIndex == m_boneWeights[j].boneIndex1 && m_boneWeights[j].weight1 > 0)
                                {
                                    if (boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                    else if (!boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict.Add(flexCloneMesh.bones[boneIndex], new List<int>());
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                }
                                else if (boneIndex == m_boneWeights[j].boneIndex2 && m_boneWeights[j].weight2 > 0)
                                {
                                    if (boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                    else if (!boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict.Add(flexCloneMesh.bones[boneIndex], new List<int>());
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                }
                                else if (boneIndex == m_boneWeights[j].boneIndex3 && m_boneWeights[j].weight3 > 0)
                                {
                                    if (boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                    else if (!boneToVerticesDict.ContainsKey(flexCloneMesh.bones[boneIndex]))
                                    {
                                        boneToVerticesDict.Add(flexCloneMesh.bones[boneIndex], new List<int>());
                                        boneToVerticesDict[flexCloneMesh.bones[boneIndex]].Add(i);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
