using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

//Find the set of vertices for a bone index from the boneweights array of a rendered mesh
public class FindVerticesBoneWeights : MonoBehaviour
{
    public GameObject m_object;
    SkinnedMeshRenderer flexCLoneMesh;
    public int testBoneIndex = -1;
    BoneWeight[] m_boneWeights = new BoneWeight[0];
    Dictionary<Transform, List<int>> boneToVerticesDict;
    // Start is called before the first frame update
    void Start()
    {
        flexCLoneMesh = m_object.GetComponent<SkinnedMeshRenderer>();
        m_boneWeights = new BoneWeight[flexCLoneMesh.sharedMesh.boneWeights.Length];
        for (int i = 0; i < m_boneWeights.Length; i++)
        {
            m_boneWeights[i] = flexCLoneMesh.sharedMesh.boneWeights[i];
        }

        if (testBoneIndex != -1)
        {

            for (int i = 0; i < flexCLoneMesh.bones.Length; i++)
            {
                for (int j = 0; j < m_boneWeights.Length; i++)
                {
                    if (i == m_boneWeights[j].boneIndex0) 
                    {
                        if (m_boneWeights[j].weight0 > 0)
                        {
                            if (boneToVerticesDict.ContainsKey(flexCLoneMesh.bones[i]))
                            {
                                boneToVerticesDict[flexCLoneMesh.bones[i]].Add(j);
                            }
                            else if (!boneToVerticesDict.ContainsKey(flexCLoneMesh.bones[i]))
                            {
                                boneToVerticesDict.Add(flexCLoneMesh.bones[i], new List<int>());
                                boneToVerticesDict[flexCLoneMesh.bones[i]].Add(j);
                            }
                        }
                    }
                    else if (i == m_boneWeights[j].boneIndex1)
                    {
                        if (m_boneWeights[j].weight0 > 1)
                        {

                        }
                    }
                    else if (i == m_boneWeights[j].boneIndex2)
                    {
                        if (m_boneWeights[j].weight0 > 2)
                        {

                        }
                    }
                    else if (i == m_boneWeights[j].boneIndex3)
                    {
                        if (m_boneWeights[j].weight0 > 3)
                        {

                        }
                    }
                }
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        print(flexCLoneMesh.name);
    }
}
