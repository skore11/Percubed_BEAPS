using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ExposeBakedMesh : MonoBehaviour
{
    private SkinnedMeshRenderer skin;
    [HideInInspector]
    public Mesh bakedMesh;

    void Awake()
    {
        skin = GetComponent<SkinnedMeshRenderer>();
    }

    void Start()
    {
        bakedMesh = new Mesh();
    }

    void LateUpdate()
    {
        //Pass baked mesh to vertex selector
        skin.BakeMesh(bakedMesh);
    }
}
