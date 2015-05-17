using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour 
{

	void Awake()
	{
		/*
		MeshFilter meshFilter = GetComponent< MeshFilter > ( );

		int[] triangles = meshFilter.sharedMesh.triangles;

		for ( int i = 0; i < triangles.Length; i += 3 )
		{
			int tmp = triangles[i];
			triangles[i] = triangles[ i+1 ];
			triangles[i] = tmp;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = meshFilter.sharedMesh.vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		meshFilter.sharedMesh = mesh;
		*/
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
