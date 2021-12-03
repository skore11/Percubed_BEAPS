using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugCheckForDuplicateVertices : MonoBehaviour
{
    public Mesh meshToCheck;
    Vector3[] verts;

    void Start()
    {
        verts = meshToCheck.vertices;
        print(string.Format("Checking {0} Vertices for duplicates", verts.Length));
        var duplicates = verts
                .Select((vert, index) => new { index, vert })
                .GroupBy(x => x.vert, x => x.index)
                .Where(g => g.Count() > 1)
                .ToDictionary(x => x.Key, y => y.ToArray())
                ;
        print(string.Format("{0} duplicate vertices: {1}",
                duplicates.Count,
                string.Join(", ", duplicates.Select(entry => entry.Key + " at " + string.Join(",", entry.Value)))
                ));
    }

}
