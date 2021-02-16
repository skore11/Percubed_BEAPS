using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintRemoveVerticesExample : MonoBehaviour
{

    public float speed = 40;

    void Update()
    {
        RaycastHit rayHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit))
        {
            
            MeshCollider hitObjCollider = rayHit.collider as MeshCollider;
            NeighborVertexHighlight vertHighlighter = rayHit.transform.GetComponentInChildren<NeighborVertexHighlight>();

            if (hitObjCollider != null && vertHighlighter != null)
            {
                if (Input.GetMouseButton(0))//left mouse pressed - add vertices to highlight
                    vertHighlighter.AddIndex(hitObjCollider.sharedMesh.triangles[rayHit.triangleIndex * 3]);
                if (Input.GetMouseButton(1))//right mouse pressed - remove highlighted vertices
                    vertHighlighter.RemoveIndex(hitObjCollider.sharedMesh.triangles[rayHit.triangleIndex * 3]);
            }
            else
                Debug.LogError("Hit object: " + rayHit.transform.name + " doesnt have a NeighborVertexHighlight behaviour OR doesnt have a mesh collider");
        }

        //rotate mesh
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.RotateAround(transform.position, Vector3.up, -speed * Time.deltaTime);
    }

    void OnGUI()
    {
        GUILayout.Label("Press left click over mesh to paint vertices");
        GUILayout.Label("Press right click over mesh to remove vertices");
        GUILayout.Label("Left/right arrow to rotate sphere");
    }
}