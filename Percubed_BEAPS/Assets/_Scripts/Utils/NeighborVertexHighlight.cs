using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborVertexHighlight : MonoBehaviour
{
    private Mesh highlightMesh;
    private Mesh parentMesh; //refers to the mesh we are getting the indices to highlight

    private List<int>[] neighborGraph;//precomputed neighbor graph

    public Dictionary<Vector3, VertexNeighbor> indexNeighbors;//contains the current indices that we want to highlight

    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    Dictionary<int, int> indexRemap = new Dictionary<int, int>();

    private bool finishedProcessingNeighborGraph = false;
    void Start()
    {
        indexNeighbors = new Dictionary<Vector3, VertexNeighbor>();
        if (transform.parent == null)
            Debug.LogError("NeighboorHighlight needs to be attached to a child of the mesh one wants to highlight");
        //parentMesh = transform.parent.GetComponent<MeshFilter>().mesh;
        parentMesh = transform.parent.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        if (parentMesh == null)
            Debug.LogError("Parent GameObject doesnt have a mesh");

        Material meshMat = new Material(Shader.Find("Unlit/Color"));
        meshMat.color = Color.red;

        gameObject.AddComponent<MeshRenderer>().material = meshMat;
        gameObject.AddComponent<MeshFilter>();

        /* Neighbor Graph generation */
        ProcessNeighborGraph();

    }

    private void ThreadedNeighborGraphProcess(Vector3[] parentVerts, int[] parentTriangs)
    {
        finishedProcessingNeighborGraph = false;
        Debug.Log("Generating neighbor hierarchy in other thread...");
        neighborGraph = GenerateNeighborGraph(parentVerts, parentTriangs);
        Debug.Log("Finished generating hierarchy!");
        finishedProcessingNeighborGraph = true;
    }

    private void ProcessNeighborGraph()
    {
        finishedProcessingNeighborGraph = false;
        Vector3[] parentVerts = parentMesh.vertices;
        int[] parentTriangs = parentMesh.triangles;
        Thread graphWorker = new Thread(new ThreadStart(() => ThreadedNeighborGraphProcess(parentVerts, parentTriangs)));
        graphWorker.Start();
    }

    public void AddIndex(int i)
    {
        if (!finishedProcessingNeighborGraph)
        {
            Debug.LogError("Cant add index as neighbor graph is being processed!");
            return;
        }
        if (!indexNeighbors.ContainsKey(parentMesh.vertices[i]))
        {
            indexNeighbors.Add(parentMesh.vertices[i], new VertexNeighbor(i, neighborGraph[i]));
            AddVertexToMesh(parentMesh.vertices[i], i);
        }
    }
    public void RemoveIndex(int i)
    {
        if (!finishedProcessingNeighborGraph)
        {
            Debug.LogError("Cant remove index as neighbor graph is being processed!");
            return;
        }
        if (indexNeighbors.ContainsKey(parentMesh.vertices[i]))
        {
            indexNeighbors.Remove(parentMesh.vertices[i]);
            RemoveVertexFromMesh(parentMesh.vertices[i], i);
        }
    }

    private void AddVertexToMesh(Vector3 vert, int index)
    {
        highlightMesh = new Mesh();
        List<int> neighborsOfIndex = neighborGraph[index];

        if (!indexRemap.ContainsKey(index))
        {
            vertices.Add(vert);
            indexRemap.Add(index, vertices.Count - 1);
        }
        for (int i = 0; i < neighborsOfIndex.Count; i++)
        {
            indices.Add(indexRemap[index]);

            if (indexRemap.ContainsKey(neighborsOfIndex[i]))
            {
                indices.Add(indexRemap[neighborsOfIndex[i]]);
            }
            else
            {
                vertices.Add(parentMesh.vertices[neighborsOfIndex[i]]);
                indexRemap.Add(neighborsOfIndex[i], vertices.Count - 1);
                indices.Add(indexRemap[neighborsOfIndex[i]]);
            }
        }
        highlightMesh.SetVertices(vertices);
        highlightMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().sharedMesh = highlightMesh;
    }

    private void RemoveVertexFromMesh(Vector3 vert, int index)
    {
        highlightMesh = new Mesh();
        if (indexRemap.ContainsKey(index))
        {
            for (int i = 0; i < indices.Count - 1; i++)
            {
                if (indices[i] == indexRemap[index])
                {
                    indices[i] = -1;
                    indices[i + 1] = -1;
                }
            }
            indices.RemoveAll(x => x == -1);
        }

        highlightMesh.SetVertices(vertices);
        highlightMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().sharedMesh = highlightMesh;
    }

    private List<int>[] GenerateNeighborGraph(Vector3[] _vertices, int[] _triangles)
    {
        List<int>[] neighbor = new List<int>[_vertices.Length];

        Dictionary<Vector3, int> currentConnections = new Dictionary<Vector3, int>();
        for (int j = 0; j < _triangles.Length; j += 3)
        {
            int j1 = _triangles[j + 0];
            int j2 = _triangles[j + 1];
            int j3 = _triangles[j + 2];
            if (neighbor[j1] == null) neighbor[j1] = new List<int>();
            if (neighbor[j2] == null) neighbor[j2] = new List<int>();
            if (neighbor[j3] == null) neighbor[j3] = new List<int>();

            // TODO: the currentConnections dictionary seems completely unnecessary!
            // it just maps the vector to its index, but we have the index right here,
            // and it is ever read from with _vertices[index], i.e. with the index right there!
            // it might be useful though if it was a list of indices for co-located vertices,
            // if we wanted to deal with co-located vertices here as well!

            if (!currentConnections.ContainsKey(_vertices[j1]))
                currentConnections.Add(_vertices[j1], j1);
            if (!currentConnections.ContainsKey(_vertices[j2]))
                currentConnections.Add(_vertices[j2], j2);
            if (!currentConnections.ContainsKey(_vertices[j3]))
                currentConnections.Add(_vertices[j3], j3);

            neighbor[currentConnections[_vertices[j1]]].Add(j2);
            neighbor[currentConnections[_vertices[j1]]].Add(j3);

            neighbor[currentConnections[_vertices[j2]]].Add(j1);
            neighbor[currentConnections[_vertices[j2]]].Add(j3);

            neighbor[currentConnections[_vertices[j3]]].Add(j1);
            neighbor[currentConnections[_vertices[j3]]].Add(j2);
        }
        return neighbor;
    }

    //contains index of a vertex and its neighbor indexes
    public class VertexNeighbor
    {
        private int index; public int Index { get { return index; } }
        private List<int> neighborIndexes; public List<int> NeighborIndexes { get { return neighborIndexes; } }
        public VertexNeighbor(int i, List<int> indexes)
        {
            index = i;
            neighborIndexes = indexes;
        }
    }
}