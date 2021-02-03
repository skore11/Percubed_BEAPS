using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This class can handle morph targets (blend shapes) for skinned meshes
/// with many submeshes
/// </summary>
public class SkinnedMorphTargets : MonoBehaviour 
{
    /// <summary>
    /// The root of the hierarchy of source meshes that will be morphed "from".
    /// </summary>
    public GameObject originalMeshObjects;

    /// <summary>
    /// A simple array of meshes.
    /// This class is created because unity inspector can't handle Mesh[][] or Mesh[,]
    /// </summary>
    [Serializable]
    public class MeshArray
    {
        public string name;
        public Mesh[] submeshes;
        public Mesh this[int index]
        {
            get { return submeshes[index]; }
            set { submeshes[index] = value; }
        }
        public int Length { get { return submeshes.Length; } }
    }

    /// <summary>
    /// The meshes that will be used to alter the original mesh
    /// Each mesh needs to consist of the same amount of submeshes as the modified mesh
    /// </summary>
    public MeshArray[] morphTargets;
    
    /// <summary>
    /// The weights of each morph target weight.
    /// The first weight is that of the original mesh.
    /// </summary>
    public float[] blendWeights;

    /// <summary>
    /// If true, recalculate morph for every index as often as possible.
    /// Check this if the target or source mesh is dynamic/animated.
    /// </summary>
    public bool dynamicMorph;

    /// <summary>
    /// The normalized weights. Public for debugging
    /// </summary>
    public float[] normalizedBlendWeights;

    /// <summary>
    /// The weight of the neutral pose. Public for debugging.
    /// </summary>
    public float neutralWeight;

    /// <summary>
    /// Was the script loaded successfully? (Public for debugging use)
    /// </summary>
    public bool loadedSuccessfully;

    /// <summary>
    /// A simple array of indices.
    /// IndexArray[] is easier to manage than int[][], so we created this.
    /// </summary>
    internal class IndexArray
    {
        public int[] indices;
        public int this[int index]
        {
            get { return indices[index]; }
            set { indices[index] = value; }
        }
        public int Length { get { return indices.Length; } }
    }

    /// <summary>
    /// A per submesh list of the indices that might be affected
    /// </summary>
    private IndexArray[] morphVertexIndices;

    /// <summary>
    /// The meshes that get modified every frame (and rendered)
    /// </summary>
    private Mesh[] workingMeshes;

    /// <summary>
    /// The original mesh before the blending modifications
    /// </summary>
    private Mesh[] sourceMeshes;

	/// <summary>
	/// Start the script - attach the targets 
	/// </summary>
    void Start()
    {
		loadedSuccessfully = false;
		
        //Get the neutral pose meshes
        SkinnedMeshRenderer[] filters = originalMeshObjects.GetComponentsInChildren<SkinnedMeshRenderer>();
        //Debug.Log(filters[0].name);
        sourceMeshes = new Mesh[filters.Length];
        for (int i = 0; i < filters.Length; i++)
        {
            //Save a reference to the mesh being edited
            // IF it has a baked mesh exposed, use that instead! --strank
            ExposeBakedMesh bakedMeshExposer = filters[i].gameObject.GetComponent<ExposeBakedMesh>();
            if (bakedMeshExposer != null) {
                sourceMeshes[i] = bakedMeshExposer.bakedMesh;
            } else {
                sourceMeshes[i] = filters[i].sharedMesh;
            }
            print("Morphing from: " + sourceMeshes[i].name);
        }

        //Check that attribute meshes have been assigned and extract their meshes
        for (int i = 0; i < morphTargets.Length; i++)
        {
            
            if (morphTargets[i] == null)
            {
                Debug.Log("Attribute " + i + " has not been assigned.");
                return;
            }
            if (morphTargets[i].Length != sourceMeshes.Length)
            {
                Debug.Log("Attribute " + i + " has wrong number of meshes");
                Debug.Log(morphTargets[i].Length + "," + sourceMeshes.Length);
            }
        }

        //Check attribute meshes to be sure vertex count is the same.
        for (int submeshNum = 0; submeshNum < sourceMeshes.Length; submeshNum++)
        {   
            int vertexCount = sourceMeshes[submeshNum].vertexCount;
			string submeshName = sourceMeshes[submeshNum].name;
			
            for (int i = 0; i < morphTargets.Length; i++)
            {
                Debug.Log(string.Format(
                    "Morph target {0} for Submesh {1} num {2} with {3} vertices.",
                    morphTargets[i].name, submeshName, submeshNum, vertexCount));
                if (morphTargets[i][submeshNum].name != submeshName)
				{
					Debug.Log(string.Format(
                        "Morph Target {2} : Submesh name mismatch - this might be a problem - {0} is a pose for {1}",
                        morphTargets[i][submeshNum].name, submeshName, morphTargets[i].name));
				}
                if (morphTargets[i][submeshNum].vertexCount != vertexCount)
                {

                    Debug.Log(string.Format(
                        "Morph target {0} : Submesh {1} doesn't have the same number of vertices as the source mesh",
                        morphTargets[i].name, sourceMeshes[submeshNum].name));
                    return;
                }
            }
        }
        
        //Check which vertices get modified by the morphing process
        CalculateMorphedVertices();

        //Create working buffers for the submeshes that will get morphed
        //and make the skinned mesh use these buffers
        SkinnedMeshRenderer[] wrkFilters = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //Debug.Log(filters[0].name);
        workingMeshes = new Mesh[wrkFilters.Length];
        for (int i = 0; i < sourceMeshes.Length; i++)
        {
            if (morphVertexIndices[i].Length > 0)
            {
                workingMeshes[i] = CloneMesh(sourceMeshes[i]);
                wrkFilters[i].sharedMesh = workingMeshes[i];
            }
        }

        //Check that working meshes correspond to source meshes:
        if (workingMeshes.Length != sourceMeshes.Length)
        {
            Debug.Log("Different number of local working meshes than source meshes");
            Debug.Log(workingMeshes.Length + "," + sourceMeshes.Length);
        }

        //Initialize the blending weights. Start off in neutral pose if not specified.
        if (blendWeights == null || blendWeights.Length != morphTargets.Length)
        {
            blendWeights = new float[morphTargets.Length];
        }
        normalizedBlendWeights = new float[blendWeights.Length];
		
		Debug.Log("Successfully set up morph targets for " + gameObject.name);
		loadedSuccessfully = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Early out if loading failed.
        if (!loadedSuccessfully)
        {
            return;
        }

        if (CheckWeights() || this.dynamicMorph)
        {
            GenerateBlendedMesh();
        }
    }

    /// <summary>
    /// Check which submeshes might take place in the operation
    /// </summary>
    void CalculateMorphedVertices()
    {
        morphVertexIndices = new IndexArray[sourceMeshes.Length];
        for (int submeshNum = 0; submeshNum < sourceMeshes.Length; submeshNum++)
        {
            List<int> morphedVertices = new List<int>();

            Vector3[] originalVertices = sourceMeshes[submeshNum].vertices;
            Vector3[] originalNormals = sourceMeshes[submeshNum].normals;

            //The fetch seems to copy vertices, so prefetch here
            List<Vector3[]> poseVertices = new List<Vector3[]>();
            List<Vector3[]> poseNormals = new List<Vector3[]>();
            for (int i = 0; i < morphTargets.Length; i++)
            {
                poseVertices.Add(morphTargets[i][submeshNum].vertices);
                poseNormals.Add(morphTargets[i][submeshNum].normals);
            }

            for (int j = 0; j < originalVertices.Length; j++)
            {
                for (int i = 0; i < morphTargets.Length; i++)
                {
                    Debug.Log("check vertex " + poseVertices[i][j] + originalVertices[j]);
                    if (this.dynamicMorph ||
                        poseVertices[i][j] != originalVertices[j] || 
                        poseNormals[i][j] != originalNormals[j])
                    {
                        morphedVertices.Add(j);
                        break;
                    }
                }
            }
            IndexArray idxArray = new IndexArray();
            idxArray.indices = new int[morphedVertices.Count];
            morphedVertices.CopyTo(idxArray.indices);
            morphVertexIndices[submeshNum] = idxArray;
            Debug.Log(string.Format(
                "For submesh {0}: morphVertexIndices.length {1}.",
                submeshNum, morphVertexIndices[submeshNum].Length));

        }
    }

    /// <summary>
    /// Check if the weights have been changed. Also normalizes them.
    /// </summary>
    /// <returns>True if changed, false otherwise</returns>
    bool CheckWeights()
    {
        bool isDirty = false;
        float weightSum = 0;
        foreach (float weight in blendWeights)
        {
            weightSum += weight;
        }

        //If the sum of the weights is bigger than one, normalize
        float normalizeFactor = Mathf.Clamp(weightSum, 1, float.MaxValue);
        for (int i = 0; i < blendWeights.Length; i++)
        {
            float normalizedWeight = blendWeights[i] / normalizeFactor;
            if (normalizedBlendWeights[i] != normalizedWeight)
            {
                normalizedBlendWeights[i] = normalizedWeight;
                isDirty = true;
            }
        }
        
        
        //The neutral weight will be the remainder of the weights from 1, if any.
        neutralWeight = Mathf.Clamp01(1 - weightSum);
        return isDirty;
    }

    /// <summary>
    /// Generate the blended mesh
    /// </summary>
    private void GenerateBlendedMesh()
    {
        for (int submeshNum = 0; submeshNum < workingMeshes.Length; submeshNum++)
        {
            //Early out if not morphed
            if (morphVertexIndices[submeshNum].Length == 0)
            {
                continue;
            }
            Vector3[] vertices = sourceMeshes[submeshNum].vertices;
            Vector3[] normals = sourceMeshes[submeshNum].normals;
            int[] morphIndices = morphVertexIndices[submeshNum].indices;
            
            //First, put in the source mesh in the right vertices
            foreach (int morphIndex in morphIndices)
            {
                vertices[morphIndex] = vertices[morphIndex] * neutralWeight;
                normals[morphIndex] = normals[morphIndex] * neutralWeight;
            }

            //Next, factor in the pose meshes
            for (int j = 0; j < morphTargets.Length; j++)
            {
                
                //Early out to avoid unneccesary empty loops
                if (normalizedBlendWeights[j] < 0.001f)
                {
                    continue;
                }
                Vector3[] poseVertices = morphTargets[j][submeshNum].vertices;
                Vector3[] poseNormals = morphTargets[j][submeshNum].normals;
                foreach (int morphIndex in morphIndices)
                {
                    vertices[morphIndex] += poseVertices[morphIndex] * normalizedBlendWeights[j];
                    //TODO : Is this correct? Not sure. Might be good enough though.
                    normals[morphIndex] += poseNormals[morphIndex] * normalizedBlendWeights[j];
                }
                foreach (int morphIndex in morphIndices)
                {
                    //Is this avoidable?
                    normals[morphIndex].Normalize();
                }
            }
            workingMeshes[submeshNum].vertices = vertices;
            workingMeshes[submeshNum].normals = normals;

            workingMeshes[submeshNum].RecalculateBounds();
        }
    }


    /// <summary>
    /// Clone a mesh
    /// </summary>
    /// <param name="mesh">The mesh to clone</param>
    /// <returns>A clone of the mesh - same data, new buffers</returns>
    private static Mesh CloneMesh(Mesh mesh)
    {
        Mesh clone = new Mesh();
        clone.vertices = mesh.vertices;
        clone.normals = mesh.normals;
        clone.tangents = mesh.tangents;
        clone.triangles = mesh.triangles;
        clone.uv = mesh.uv;
        clone.uv2 = mesh.uv2;
        clone.uv2 = mesh.uv2;
        clone.bindposes = mesh.bindposes;
        clone.boneWeights = mesh.boneWeights;
        clone.bounds = mesh.bounds;
        clone.colors = mesh.colors;
        clone.name = mesh.name;
        //TODO : Are we missing anything?
        return clone;
    }
}
