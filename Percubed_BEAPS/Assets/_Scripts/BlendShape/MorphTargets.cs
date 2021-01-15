using System;
using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]

public class MorphTargets : MonoBehaviour
{
    internal class BlendShapeVertex
    {
        public int originalIndex;
        public Vector3 position;
        public Vector3 normal;
    }

    internal class BlendShape
    {
        public BlendShapeVertex[] vertices;// = new Array();
    }

    public String[] attributes; //Names for the attributes to be morphed
    public Mesh sourceMesh; //The original mesh
    public Mesh[] attributeMeshes; //The destination meshes for each attribute.
    public float[] attributeProgress;

    private BlendShape[] blendShapes;
    private Mesh workingMesh;

    void Awake()
    {
        for (int i = 0; i < attributeMeshes.Length; i++)
        {
            if (attributeMeshes[i] == null)
            {
                Debug.Log("Attribute " + i + " has not been assigned.");
                return;
            }
        }

        //Populate the working mesh
        //MeshFilter filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
        SkinnedMeshRenderer filter = gameObject.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
        //filter.sharedMesh = sourceMesh;
        filter.BakeMesh(sourceMesh);
        //workingMesh = filter.mesh;
        workingMesh = filter.sharedMesh;

        //Check attribute meshes to be sure vertex count is the same.
        int vertexCount = sourceMesh.vertexCount;
        Debug.Log("Vertex Count Source:" + vertexCount);
        for (int i = 0; i < attributeMeshes.Length; i++)
        {
            Debug.Log("Vertex Mesh " + i + ":" + attributeMeshes[i].vertexCount);
            if (attributeMeshes[i].vertexCount != vertexCount)
            {

                Debug.Log("Mesh " + i + " doesn't have the same number of vertices as the first mesh");
                return;
            }
        }

        //Build blend shapes
        BuildBlendShapes();
    }

    void BuildBlendShapes()
    {

        blendShapes = new BlendShape[attributes.Length];

        //For each attribute figure out which vertices are affected, then store their info in the blend shape object.
        for (int i = 0; i < attributes.Length; i++)
        {
            //Populate blendShapes array with new blend shapes
            blendShapes[i] = new BlendShape();

            /** TODO: Make this a little more stylish!
             *  UGLY hack to compensate the lack of dynamic arrays in C#. Feel free to improve!
             */
            int blendShapeCounter = 0;
            print("working mesh vertex count:" + workingMesh.vertexCount);
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {

                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    blendShapeCounter++;
                }
            }

            blendShapes[i].vertices = new BlendShapeVertex[blendShapeCounter];
            blendShapeCounter = 0;
            for (int j = 0; j < workingMesh.vertexCount; j++)
            {
                //If the vertex is affected, populate a blend shape vertex with that info
                if (workingMesh.vertices[j] != attributeMeshes[i].vertices[j])
                {
                    //Create a blend shape vertex and populate its data.

                    BlendShapeVertex blendShapeVertex = new BlendShapeVertex();
                    blendShapeVertex.originalIndex = j;
                    blendShapeVertex.position = attributeMeshes[i].vertices[j] - workingMesh.vertices[j];
                    blendShapeVertex.normal = attributeMeshes[i].normals[j] - workingMesh.normals[j];

                    //Add new blend shape vertex to blendShape object.
                    blendShapes[i].vertices[blendShapeCounter] = blendShapeVertex;
                    blendShapeCounter++;
                }
            }

            //Convert blendShapes.vertices to builtin array
            //blendShapes[i].vertices = blendShapes[i].vertices.ToBuiltin(BlendShapeVertex);
        }
    }


    public void SetMorph()
    {

        //Set up working data to store mesh offset information.
        Vector3[] morphedVertices = sourceMesh.vertices;
        Vector3[] morphedNormals = sourceMesh.normals;

        //For each attribute...
        for (int j = 0; j < attributes.Length; j++)
        {
            //If the weight of this attribute isn't 0	
            if (!Mathf.Approximately(attributeProgress[j], 0))
            {
                //For each vertex in this attribute's blend shape...
                for (int i = 0; i < blendShapes[j].vertices.Length; i++)
                {
                    //...adjust the mesh according to the offset value and weight
                    morphedVertices[blendShapes[j].vertices[i].originalIndex] += blendShapes[j].vertices[i].position * attributeProgress[j];
                    //Adjust normals as well
                    morphedNormals[blendShapes[j].vertices[i].originalIndex] += blendShapes[j].vertices[i].normal * attributeProgress[j];
                }
            }
        }

        //Update the actual mesh with new vertex and normal information, then recalculate the mesh bounds.		
        workingMesh.vertices = morphedVertices;
        workingMesh.normals = morphedNormals;
        workingMesh.RecalculateBounds();
    }



}