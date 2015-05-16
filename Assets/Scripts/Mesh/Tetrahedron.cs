using UnityEngine;
using System.Collections;

public class Tetrahedron : MonoBehaviour 
{
	/*
	private MeshFilter meshFilter_ = null;

	_MeshGen.Triangle[] triangles_ = new _MeshGen.Triangle[4];

	void Init(Vector3 centre, float size)
	{
		gameObject.transform.localPosition = centre;

		double d = (double)size;

		double tetSideLength = d * System.Math.Sqrt (2);
		double tetDistCentreToVertex = d * System.Math.Sqrt (3) / 2;
		double tetVertDistCentreToBase = d - tetDistCentreToVertex;
		double tetSideHeight = d * System.Math.Sqrt (3 / 2);
		double tetSideDistCentreToSide = d / Mathf.Sqrt (6);
		double tetSideDistCentreToVertez = tetSideHeight - tetSideDistCentreToSide;

		Vector3 apex = new Vector3 ( 0f, (float)tetDistCentreToVertex, 0f  );
		Vector3 base0 = new Vector3 ( 0.5f * (float)tetSideLength, -1f * (float)(tetVertDistCentreToBase) ,  (float)tetSideDistCentreToSide );
		Vector3 base1 = new Vector3 ( -0.5f * (float)tetSideLength, -1f * (float)(tetVertDistCentreToBase) ,  (float)tetSideDistCentreToSide );
		Vector3 base2 = new Vector3 (0f, -1f * (float)(tetVertDistCentreToBase), -1f * (float)tetSideDistCentreToVertez); 

		triangles_ [0] = new _MeshGen.Triangle ( base0, apex, base2);
		triangles_ [1] = new _MeshGen.Triangle (base1, apex, base0);
		triangles_[2] = new _MeshGen.Triangle(base2, apex, base1);
		triangles_[3] = new _MeshGen.Triangle(base0, base2, base1);


	}
	
	void Awake()
	{
		meshFilter_ = GetComponent< MeshFilter > ();
		if (meshFilter_ == null) 
		{
			Debug.Log ("MeshFilter null in polyhedron Awaek()");
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void CreateMesh()
	{
		Mesh mesh = meshFilter_.sharedMesh;
		if (mesh == null) 
		{
			meshFilter_.mesh = new Mesh();
			mesh = meshFilter_.sharedMesh;
		}
		mesh.Clear ();

	}
	*/
}
