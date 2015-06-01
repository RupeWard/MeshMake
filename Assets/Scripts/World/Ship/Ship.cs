using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour 
{
	public Transform shipFollowingCameraIdeal;
	public Camera shipFollowingCamera;

	public GameObject nose;
	public GameObject rear;
	public GameObject fin;

	public Rigidbody rigidBody;

	public float accelerationRate = 50f;
	public float decelerationRate = 50f;

	private bool accelerating_ = false;
	private bool decelerating_ = false;

	public float rotateAccelerationRate = 10f;
	private float rotateUpDownRate = 0f;
	private float rotateLeftRightRate = 0f;
	private float spinRate = 0f;

	public float followingCameraMoveSpeed = 10f;
	public float followingCameraRotateSpeed = 10f;

	public Vector3 DirectionPointing
	{
		get { Vector3 v = (nose.transform.position - rear.transform.position); v.Normalize(); return v; }
	}

	public void OnSpinLeftPressed()
	{
		spinRate = -1f * rotateAccelerationRate;
	}

	public void OnSpinRightPressed()
	{
		spinRate = rotateAccelerationRate;
	}

	public void OnSpinReleased()
	{
		spinRate = 0f;
	}
	
	public void OnRotateUpPressed()
	{
		rotateUpDownRate = rotateAccelerationRate;
	}

	public void OnRotateDownPressed()
	{
		rotateUpDownRate = -1f * rotateAccelerationRate;
	}

	public void OnRotateLeftPressed()
	{
		rotateLeftRightRate = -1f * rotateAccelerationRate;
	}

	public void OnRotateRightPressed()
	{
		rotateLeftRightRate = rotateAccelerationRate;
	}

	public void OnRotateUpDownReleased()
	{
		rotateUpDownRate = 0f;
	}

	public void OnRotateLeftRightReleased()
	{
		rotateLeftRightRate = 0f;
	}

	public void OnAcceleratePressed()
	{
		accelerating_ = true;
		decelerating_ = false;
	}

	public void OnAccelerateReleased()
	{
		accelerating_ = false;
	}

	public void OnBrakePressed()
	{
		decelerating_ = true;
		accelerating_ = false;
	}

	public void OnBrakeReleased()
	{
		decelerating_ = false;		
	}
	
	void Awake()
	{
		shipFollowingCamera.transform.position = shipFollowingCameraIdeal.transform.position;
		shipFollowingCamera.transform.rotation = shipFollowingCameraIdeal.transform.rotation;
		rigidBody.velocity = Vector3.zero;
		rigidBody.rotation = Quaternion.Euler( new Vector3(0f,0f,0f) );
	}

	void Start () 
	{
	
	}


	void FixedUpdate()
	{
		if ( shipFollowingCamera.transform.position != shipFollowingCameraIdeal.transform.position )
		{
			shipFollowingCamera.transform.position = Vector3.Lerp ( shipFollowingCamera.transform.position, shipFollowingCameraIdeal.transform.position, followingCameraMoveSpeed * Time.fixedDeltaTime); 
		}
		if (shipFollowingCamera.transform.rotation != shipFollowingCameraIdeal.transform.rotation)
		{
			shipFollowingCamera.transform.rotation = Quaternion.Lerp ( shipFollowingCamera.transform.rotation, shipFollowingCameraIdeal.transform.rotation, followingCameraRotateSpeed * Time.fixedDeltaTime);
		}

		if (accelerating_ )
		{
			rigidBody.AddForce( DirectionPointing * accelerationRate, ForceMode.Force);
//			rigidBody.AddForce( DirectionPointing * accelerationRate * Time.fixedDeltaTime);
		}
		else if (decelerating_)
		{
			rigidBody.AddForce( -1f * DirectionPointing * accelerationRate, ForceMode.Force);
//			rigidBody.AddForce( -1f * DirectionPointing * accelerationRate * Time.fixedDeltaTime);
		}	
		if (rotateUpDownRate != 0f )
		{
			Vector3 direction = DirectionPointing;
			Vector3 v0 = shipFollowingCamera.ScreenToWorldPoint( new Vector3( 0f, -100f, shipFollowingCamera.nearClipPlane ) );
			Vector3 v1 = shipFollowingCamera.ScreenToWorldPoint( new Vector3( 0f, 100f, shipFollowingCamera.nearClipPlane ) );
			Vector3 line1 = v1-v0; // vert line in screen
			Vector3 axis = Vector3.Cross(line1, direction);
			axis.Normalize();
			rigidBody.AddTorque( -1f * axis * rotateUpDownRate * Time.fixedDeltaTime);
		}
		if (rotateLeftRightRate != 0f)
		{
			Vector3 direction = DirectionPointing;
			Vector3 v0 = shipFollowingCamera.ScreenToWorldPoint( new Vector3( -100f, 0f, shipFollowingCamera.nearClipPlane ) );
			Vector3 v1 = shipFollowingCamera.ScreenToWorldPoint( new Vector3( 100f, 0f, shipFollowingCamera.nearClipPlane ) );
			Vector3 line1 = v1-v0; // vert line in screen
			Vector3 axis = Vector3.Cross(line1, direction);
			axis.Normalize();
			rigidBody.AddTorque( -1f * axis * rotateLeftRightRate * Time.fixedDeltaTime);
		}
		if (spinRate != 0f)
		{
			Vector3 axis = DirectionPointing;
			axis.Normalize();
			rigidBody.AddTorque( -1f * axis * spinRate * Time.fixedDeltaTime);
		}

	}
}
