using UnityEngine;
using System.Collections;

public class AppManager : SingletonApplicationLifetime< AppManager > 
{
	public Material defaultMaterial;
	public float maxDist = 50f;

	public float camerabuffer = 2f;

	private _MeshGen.MeshGenerator currentGenerator_ = null;

	// Use this for initialization
	void Start () 
	{
		Debug.Log ( "AppManager.Start" );
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnTetButtonClicked()
	{
		if ( currentGenerator_ != null )
		{
			_MeshGen.TetrahedronGenerator tetGenerator = currentGenerator_ as _MeshGen.TetrahedronGenerator;
			if ( tetGenerator == null )
			{
				GameObject.Destroy ( currentGenerator_.gameObject );
			}
			else
			{
				tetGenerator.SplitRandomTriangle ( );
				return;
			}
		}

		{
			_MeshGen.TetrahedronGenerator tetGenerator = _MeshGen.TetrahedronGenerator.Create ("Tet", Vector3.zero, 10f);
			tetGenerator.SetMaterial(defaultMaterial);
			tetGenerator.MakeMesh();
			
			currentGenerator_ = tetGenerator;
		}
	}

	public void OnCubeButtonClicked()
	{
		if ( currentGenerator_ != null )
		{
			_MeshGen.CubeGenerator cubeGenerator = currentGenerator_ as _MeshGen.CubeGenerator;
			if (cubeGenerator == null)
			{
				GameObject.Destroy (currentGenerator_.gameObject);
			}
			else
			{
				cubeGenerator.ExtendRandomRect();
				return;
			}
		}
		{
			_MeshGen.CubeGenerator cubeGenerator = _MeshGen.CubeGenerator.Create ("Cub", Vector3.zero, 10f);
			cubeGenerator.SetMaterial(defaultMaterial);
			cubeGenerator.MakeMesh();
			
			currentGenerator_ = cubeGenerator;
		}
	}

}
