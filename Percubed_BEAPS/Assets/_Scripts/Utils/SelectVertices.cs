using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectVertices : MonoBehaviour
{
    public SkinnedMeshRenderer skm;
    Vector3[] verts;

    // Start is called before the first frame update
    void Start()
    {
        skm = GetComponent<SkinnedMeshRenderer>();
        verts = skm.sharedMesh.vertices;
        Debug.Log("Vertices" + verts.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300))
            {
                Vector3 hitVertex= PickNearestPoint(hit.point);
                Debug.Log("hit Vertices " + hitVertex);
            }
        }
    }

    private Vector3 PickNearestPoint(Vector3 point)
    {
        Vector3 nearesPoint = new Vector3();
        float lastDistance = 999999999f;

        for (int i = 0; i < verts.Length; i++)
        {
            float distance = GetDistance(point, verts[i]);
            if (distance < lastDistance)
            {
                lastDistance = distance;
                nearesPoint = verts[i];
            }
        }

        return nearesPoint;
    }

    private float GetDistance(Vector3 start, Vector3 end)
    {
        return Mathf.Sqrt(Mathf.Pow((start.x - end.x), 2) + Mathf.Pow((start.y - end.y), 2) + Mathf.Pow((start.z - end.z), 2));
    }
}
