using UnityEngine;
using System.Collections;

public class AppManager : SingletonApplicationLifetime< AppManager > 
{
	public MG.UV.RectUVProvider rectUVProvider;

	public Material defaultMaterial;
	public PhysicMaterial defaultThingPhysicsMaterials;

	public float maxDist = 50f;

	public float camerabuffer = 2f;

	public float minMeshUpdateWait = 0.05f;

	public bool allowMultiExtend = false;
	public bool allowSameVertexMultiExtend = false;
	public bool denyFacing =true;

	private MG.CubeMeshGenerator currentCubeGenerator_ = null;
	private MG.TetrahedronGenerator currentTetGenerator_ = null;

	public float moveDuration = 2f;

	public GameObject ballPrefab;
	public GameObject physBallPrefab;
	public Transform world;

	public Camera tetheredCamera;
	public Camera shipCamera;
	public Camera internalCamera;

	public enum EMode
	{
		TetheredCamera,
		ShipCamera,
		InternalCamera,
		NONE
	}

	private EMode mode_ = EMode.NONE;
	public EMode Mode
	{
		get { return mode_; }
	}

	protected override void PostAwake()
	{
		SetMode(EMode.TetheredCamera);
		Ball.maxDistFromOrigin = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
	}

	void Start () 
	{
		Debug.Log ( "AppManager.Start" );
	}
	
	void Update () 
	{
	
	}

	private void SetMode( EMode m)
	{
		if (mode_ != m)
		{
			mode_ = m;
			switch(mode_)
			{
				case EMode.TetheredCamera:
				{
					tetheredCamera.enabled = true;
					shipCamera.enabled = false;
					internalCamera.enabled = false;
					SetThingMode(ReverseNormals.EState.Outside);
					break;
				}
				case EMode.ShipCamera:
				{
					shipCamera.enabled = true;
					tetheredCamera.enabled = false;
					internalCamera.enabled = false;
					SetThingMode(ReverseNormals.EState.Outside);
					break;
				}
				case EMode.InternalCamera:
				{
					shipCamera.enabled = false;
					tetheredCamera.enabled = false;
					internalCamera.enabled = true;
					SetThingMode(ReverseNormals.EState.Inside);
					break;
				}
			}
			HudManager.Instance.HandleModeChange(mode_);
		}
	}

	private void SetThingMode(ReverseNormals.EState state)
	{
		GameObject[] gos = GameObject.FindGameObjectsWithTag ( "Thing" );
//		Debug.LogWarning("Found "+gos.Length+" Things");
		foreach( GameObject go in gos)
		{
			ReverseNormals rn = go.GetComponent< ReverseNormals >();
			if (rn != null)
			{
//				Debug.LogWarning("Thing '"+go.name+"' SetState "+state);
				if (rn.SetState(state))
				{
					rn.GetComponent< MG.CubeMeshGenerator >().SetDirty();
				}
			}
			else
			{
				Debug.LogError("Thing '"+go.name+"' has no ReverseNormals");
			}
		}
	}

	public void OnCameraButtonClicked()
	{
		if ( mode_ == EMode.InternalCamera )
		{
			SetMode(EMode.TetheredCamera);
		}
		else if ( mode_ == EMode.TetheredCamera )
		{
			SetMode(EMode.ShipCamera);
		}
		else if ( mode_ == EMode.ShipCamera )
		{
			SetMode(EMode.InternalCamera);
		}
	}

	public void OnTetButtonClicked()
	{
		if ( currentTetGenerator_ != null )
		{
			MG.TetrahedronGenerator tetGenerator = currentTetGenerator_ as MG.TetrahedronGenerator;
			if ( tetGenerator == null )
			{
				GameObject.Destroy ( currentTetGenerator_.gameObject );
			}
			else
			{
				tetGenerator.SplitRandomTriangle ( );
				return;
			}
		}
		else
		{
			MG.TetrahedronGenerator tetGenerator = MG.TetrahedronGenerator.Create ("Tet", Vector3.zero, 10f);
			tetGenerator.SetMaterial(defaultMaterial);
			tetGenerator.MakeMesh();
			
			currentTetGenerator_ = tetGenerator;
		}
	}

	public void OnCubeButtonClicked()
	{
		if ( currentCubeGenerator_ != null )
		{
			MG.CubeMeshGenerator cubeGenerator = currentCubeGenerator_ as MG.CubeMeshGenerator;
			if (cubeGenerator == null)
			{
				GameObject.Destroy (currentCubeGenerator_.gameObject);
			}
			else
			{
				cubeGenerator.ExtendRandomRect();
				return;
			}
		}
		else
		{
			MG.UV.I_RectUVProvider iUVp= rectUVProvider;
			if (iUVp == null)
			{
				Debug.LogWarning("Having to create the UVprovider");
				iUVp = new MG.UV.GridUVProvider(3,3);
			}
			MG.CubeMeshGenerator cubeGenerator 
				= MG.CubeMeshGenerator.Create ("Cub", Vector3.zero, 10f,
				                               defaultMaterial,
				                               iUVp);
			cubeGenerator.SetDirty();
			currentCubeGenerator_ = cubeGenerator;
		}
	}

	public void OnBallButtonClicked()
	{
//		MakeBall ( );
		MakePhysBall ( );
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

	private void MakePhysBall()
	{
		GameObject go = Instantiate ( physBallPrefab ) as GameObject;
		go.name = "PhysBall_"+ ballNum.ToString();
		PhysBall ball = go.AddComponent< PhysBall >();
		go.transform.parent = AppManager.Instance.world;

		float dist = Ball.maxDistFromOrigin - go.transform.localScale.x;
		
//		Debug.Log ("Making ball at dist "+dist);
		
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
