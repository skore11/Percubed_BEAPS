using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVertices : MonoBehaviour
{
    public NeighborVertexHighlight neighborVertHighlighter;

    public MeshBonesSelect m_bonesSelect;

    // Update is called once per frame
    void Update()
    {
        if (m_bonesSelect.shapeIndexToVerticesDict != null && m_bonesSelect.doneSelecting)
        {
            foreach (var entry in m_bonesSelect.shapeIndexToVerticesDict)
            {
                foreach (int i in entry.Value)
                {
                    neighborVertHighlighter.AddIndex(i);
                }
            }
            m_bonesSelect.doneSelecting = false;
        }
    }
}
