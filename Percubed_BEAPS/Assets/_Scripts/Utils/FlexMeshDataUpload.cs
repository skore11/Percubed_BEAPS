using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexMeshDataUpload : MonoBehaviour
{
    // Start is called before the first frame update
    SkinnedMeshRenderer m_meshInstance;
    void Awake()
    {
        m_meshInstance = GetComponent<SkinnedMeshRenderer>();
        //Debug.Log("mesh name:" + m_meshInstance.sharedMesh.name);
        m_meshInstance.sharedMesh.UploadMeshData(false);
        Debug.Log(m_meshInstance.sharedMesh.isReadable);
    }

    // Update is called once per frame
    void Update()
    {
        m_meshInstance.sharedMesh.UploadMeshData(false);
        Debug.Log(m_meshInstance.sharedMesh.isReadable);
    }
}
