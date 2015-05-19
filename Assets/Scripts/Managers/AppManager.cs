﻿using UnityEngine;
using System.Collections;

public class AppManager : SingletonApplicationLifetime< AppManager > 
{
	public Material defaultMaterial;
	public float maxDist = 50f;

	public float camerabuffer = 2f;

	private _MeshGen.MeshGenerator currentGenerator_ = null;

	public GameObject ballPrefab;
	public Transform world;

	protected override void PostAwake()
	{
		Ball.maxDistFromOrigin = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
	}

	void Start () 
	{
		Debug.Log ( "AppManager.Start" );
	}
	
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

	public void OnBallButtonClicked()
	{
		MakeBall ( );
	}

	private int ballNum = 0;
	private void MakeBall()
	{
		GameObject go = Instantiate ( ballPrefab ) as GameObject;
		go.name = "Ball_"+ ballNum.ToString();
		Ball ball = go.AddComponent< Ball >();
		float dist = Ball.maxDistFromOrigin - go.transform.localScale.x;

		Debug.Log ("Making ball at dist "+dist);

		float xangle = Random.Range( 0, 2*Mathf.PI);
		float yangle = Random.Range( 0, 2*Mathf.PI);
		float zangle = Random.Range( 0, 2*Mathf.PI);

		Vector3 position = new Vector3(
			dist * Mathf.Cos( xangle ),
			dist * Mathf.Cos( yangle ),
			dist * Mathf.Cos( zangle )
			);
		Vector3 direction = -1f*position;
		direction = direction / direction.magnitude;

		float var =  20f;
		float speed = 40f;

		float xvar = var - Random.Range( 0, 2*var);
		float yvar = var - Random.Range( 0, 2*var);
		float zvar = var - Random.Range( 0, 2*var);
		
		direction = Quaternion.Euler( xvar, yvar, zvar ) * direction;
		direction.Normalize();
		ball.Init( position, speed * direction); 
		ballNum++;
	}

}
