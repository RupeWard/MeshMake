using UnityEngine;
using System.Collections;

public class AppManager : SingletonApplicationLifetime< AppManager > 
{
	public MeshFilter mesh;

	// Use this for initialization
	void Start () 
	{
		mesh.transform.position = Vector3.zero;

		Debug.Log ( "AppManager.Start" );
		_MeshGen.TetrahedronGenerator tetGenerator = new _MeshGen.TetrahedronGenerator ( Vector3.zero, 10f);
		tetGenerator.MakeMesh(mesh);
		mesh.transform.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
