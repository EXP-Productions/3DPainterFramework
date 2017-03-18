using UnityEngine;
using System.Collections;

public static class MeshExtensions 
{
	public static Mesh ScaleVerts( this Mesh mesh, float scaler )
	{
		for (int i = 0; i < mesh.vertexCount; i++) 
		{
			mesh.vertices[i] = mesh.vertices[i] * scaler;
		}

		return mesh;
	}

    //TODO: Calculate area by iterating thorugh tris
    public static float CalculateArea(Mesh mesh)
    {
        return 0;
    }

    public static void CombineMeshes(GameObject go, bool addMeshCollider, string tag)
    {
        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        go.transform.DestroyAllChildren();

        Debug.Log("Combining: " + meshFilters.Length);

        MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        go.AddComponent<MeshRenderer>().material = meshFilters[0].GetComponent<MeshRenderer>().material;
        go.GetComponent<MeshRenderer>().material.SetCol(Color.gray);
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        Vector3[] newVerts = new Vector3[meshFilter.mesh.vertices.Length];

        for (int v = 0; v < meshFilter.mesh.vertices.Length; v++)
        {
            newVerts[v] = meshFilter.mesh.vertices[v];
            newVerts[v] -= go.transform.position;
        }
        meshFilter.mesh.vertices = newVerts;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();

        go.transform.gameObject.SetActive( true );

        if(addMeshCollider)
            go.AddComponent<MeshCollider>();
        
        if(!string.IsNullOrEmpty(tag))
            go.tag = tag;
    }
}
