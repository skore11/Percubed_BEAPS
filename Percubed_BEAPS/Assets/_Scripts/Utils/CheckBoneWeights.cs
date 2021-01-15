using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Percubed.Flex;

//Show what vertices and what particles are influenced by the same bone weights//
public class CheckBoneWeights : MonoBehaviour
{
    public FlexAnimSoftSkin m_flexAnim;

    public SkinnedMeshRenderer refAnimMesh;

    private WeightList[] boneWeightsFlex;

    private BoneWeight[] boneWeightsRef;
    // Start is called before the first frame update
    void Start()
    {
        boneWeightsFlex = m_flexAnim.particleBoneWeights;
        boneWeightsRef = refAnimMesh.sharedMesh.boneWeights;
        print("ref vertex count:" + refAnimMesh.sharedMesh.vertexCount);
        print("ref bone weights:" + boneWeightsRef.Length);
        print(m_flexAnim.referenceAnimSkin.sharedMesh.name);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_flexAnim.referenceAnimSkin.sharedMesh.vertices.Length; i++)
        {
            print("flex vertex pos:" + m_flexAnim.referenceAnimSkin.sharedMesh.vertices[i]);
            print("ref vertex pos:" + refAnimMesh.sharedMesh.vertices[i]);
        }
        
    }
}
