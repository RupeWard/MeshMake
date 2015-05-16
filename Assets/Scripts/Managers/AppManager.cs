using UnityEngine;
using System.Collections;

public class AppManager : SingletonApplicationLifetime< AppManager > 
{
	public Material defaultMaterial;
	public float maxDist = 50f;
	public float camerabuffer = 2f;

	// Use this for initialization
	void Start () 
	{
		Debug.Log ( "AppManager.Start" );
		_MeshGen.TetrahedronGenerator tetGenerator = _MeshGen.TetrahedronGenerator.Create ("Tet", Vector3.zero, 10f);
		tetGenerator.SetMaterial(defaultMaterial);
		tetGenerator.MakeMesh();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
