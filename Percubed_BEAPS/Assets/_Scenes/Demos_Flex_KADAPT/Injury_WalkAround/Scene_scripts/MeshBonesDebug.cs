using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBonesDebug : MonoBehaviour
{
    private Transform[] mesh_bones;

    public SkinnedMeshRenderer myskinmesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh_bones = myskinmesh.bones;
        print(this.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mesh_bones.Length; i++)
        {
            Debug.Log(this.gameObject.name + " bone index: " + i + " x: " + mesh_bones[i].transform.position.x + "y: " + mesh_bones[i].transform.position.y + "z: " + mesh_bones[i].transform.position.z);           
        }
    }

    private void XXOnDrawGizmos()
    {
        for (int i = 0; i < mesh_bones.Length; i++)
        {
            Debug.Log("x: " + mesh_bones[i].transform.position.x + "y: " + mesh_bones[i].transform.position.y + "z: " + mesh_bones[i].transform.position.z);
            Gizmos.DrawCube(new Vector3(mesh_bones[i].transform.position.x, mesh_bones[i].transform.position.y, mesh_bones[i].transform.position.z), new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}
