using UnityEngine;
using System.Collections;

public class InternalCameraMover : MonoBehaviour 
{
//	private static readonly bool DEBUG_CAMERAMOVER = false; 

//	public float initialZoomSpeed = 10f;
//	public float maxZoomSpeed = 50f;
//	public float zoomAcceleration = 0.1f;

	public float initialSpinSpeedDegrees = 10f;
	public float maxSpinSpeedDegrees = 50f;
	public float spinAcceleration = 0.1f;
	
	public float initialRotateSpeedDegrees = 10f;
	public float maxRotateSpeedDegrees = 50f;
	public float rotateAcceleration = 0.1f;

	public float fieldOfViewAcceleration = 0.1f;
	public float initialFieldOfViewSpeed = 5f;
	public float maxFieldOfViewSpeed = 20f;

	private float rotateUpDownSpeed_ = 0f;
	private float rotateLeftRightSpeed_ =  0f;
	private float fieldOfViewSpeed_ = 0f;
	private float spinSpeed_ = 0f;
	

//	public float initialMoveSpeedDegrees = 10f;
//	public float maxMoveSpeedDegrees = 50f;
//	public float moveAcceleration = 0.1f;

//	private float currentZoomSpeed_ = 0f;
//	private float currentMoveUpDownSpeed_ = 0f;
//	private float currentMoveLeftRightSpeed_ = 0f;
//	private float currentRotateSpeed_ = 0f;

	public float tolerance = 0.001f;

	public Camera myCamera;
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () 
	{
	}

	public void SpinLeft()
	{
		spinSpeed_ = -1f * initialSpinSpeedDegrees;
	}

	public void SpinRight()
	{
		spinSpeed_ = initialSpinSpeedDegrees;
	}

	public void SpinStop()
	{
		spinSpeed_ = 0f;
	}
	
	public void FieldOfViewIn()
	{
		fieldOfViewSpeed_ = -1f * initialFieldOfViewSpeed;
	}

	public void FieldOfViewOut()
	{
		fieldOfViewSpeed_ = initialFieldOfViewSpeed;
	}

	public void FieldOfViewStop()
	{
		fieldOfViewSpeed_ = 0f;
	}

	public void RotateUp()
	{
		rotateUpDownSpeed_ = initialRotateSpeedDegrees;
	}
	
	public void RotateDown()
	{
		rotateUpDownSpeed_ = -1f * initialRotateSpeedDegrees;
	}

	public void RotateLeft()
	{
		rotateLeftRightSpeed_ = -1f * initialRotateSpeedDegrees;
	}
	
	public void RotateRight()
	{
		rotateLeftRightSpeed_ = initialRotateSpeedDegrees;
	}
	

	public void RotateUpDownStop ( )
	{
		rotateUpDownSpeed_ = 0f;
	}

	public void RotateLeftRightStop ( )
	{
		rotateLeftRightSpeed_ = 0f;
	}



	public void Stop()
	{
		RotateUpDownStop ( );
		RotateLeftRightStop ( );
		FieldOfViewStop ( );
		SpinStop ( );
	}

	// Update is called once per frame
	void Update () 
	{
		if (rotateLeftRightSpeed_ != 0f)
		{
			transform.Rotate( Vector3.up * rotateLeftRightSpeed_ * Time.deltaTime );
			if (rotateLeftRightSpeed_ < 0f) 
			{
				rotateLeftRightSpeed_ -= rotateAcceleration;
				rotateLeftRightSpeed_ = Mathf.Max ( rotateLeftRightSpeed_, -1f * maxRotateSpeedDegrees);
			}
			else if (rotateLeftRightSpeed_ > 0f) 
			{
				rotateLeftRightSpeed_ += rotateAcceleration;
				rotateLeftRightSpeed_ = Mathf.Min ( rotateLeftRightSpeed_, maxRotateSpeedDegrees);
			}
		}
		if (rotateUpDownSpeed_ != 0f)
		{
			transform.Rotate( Vector3.left * rotateUpDownSpeed_ * Time.deltaTime );
			if (rotateUpDownSpeed_ < 0f) 
			{
				rotateUpDownSpeed_ -= rotateAcceleration;
				rotateUpDownSpeed_ = Mathf.Max ( rotateUpDownSpeed_, -1f * maxRotateSpeedDegrees);
			}
			else if (rotateUpDownSpeed_ > 0f) 
			{
				rotateUpDownSpeed_ += rotateAcceleration;
				rotateUpDownSpeed_ = Mathf.Min ( rotateUpDownSpeed_, maxRotateSpeedDegrees);
			}
		}

		if (spinSpeed_ != 0f)
		{
			float angleToRotate = spinSpeed_ * Time.deltaTime;
			transform.Rotate( Vector3.forward * angleToRotate);
			
			if (spinSpeed_ < 0f) 
			{
				spinSpeed_ -= spinAcceleration;
				spinSpeed_ = Mathf.Max ( spinSpeed_, -1f * maxSpinSpeedDegrees);
			}
			else if (spinSpeed_ > 0f) 
			{
				spinSpeed_ += spinAcceleration;
				spinSpeed_ = Mathf.Min ( spinSpeed_, maxSpinSpeedDegrees);
			}
			
		}
		/*
		if (currentMoveUpDownSpeed_ != 0f)
		{
			Vector3 v0 = camera_.ScreenToWorldPoint( new Vector3( 0f, -100f, camera_.nearClipPlane ) );
			Vector3 v1 = camera_.ScreenToWorldPoint( new Vector3( 0f, 100f, camera_.nearClipPlane ) );
			Vector3 line1 = v1-v0; // vert line in screen

			if (DEBUG_CAMERAMOVER)
			{
				Debug.Log ("line is "+line1);
			}

			Vector3 axis = Vector3.Cross(line1,transform.position);
			Vector3 point = Vector3.zero;

			float angleToMove = currentMoveUpDownSpeed_ * Time.deltaTime;

			if (DEBUG_CAMERAMOVER)
			{
				Debug.Log ( "Move "+(angleToMove)+" degrees  about pt "+point+" axis"+axis);
			}
			transform.RotateAround( point, axis, -1f * angleToMove);

			if (currentMoveUpDownSpeed_ < 0f) 
			{
				currentMoveUpDownSpeed_ -= moveAcceleration;
				currentMoveUpDownSpeed_ = Mathf.Max ( currentMoveUpDownSpeed_, -1f * maxMoveSpeedDegrees);
			}
			else if (currentMoveUpDownSpeed_ > 0f) 
			{
				currentMoveUpDownSpeed_ += moveAcceleration;
				currentMoveUpDownSpeed_ = Mathf.Min ( currentMoveUpDownSpeed_, maxMoveSpeedDegrees);
			}
		}

		if (currentMoveLeftRightSpeed_ != 0f)
		{
			Vector3 v0 = camera_.ScreenToWorldPoint( new Vector3( -100f, 0f, camera_.nearClipPlane ) );
			Vector3 v1 = camera_.ScreenToWorldPoint( new Vector3( 100f, 0f, camera_.nearClipPlane ) );
			Vector3 line1 = v1-v0; // horiz line in screen
			
			if (DEBUG_CAMERAMOVER)
			{
				Debug.Log ("line is "+line1);
			}
			
			Vector3 axis = Vector3.Cross(line1,transform.position);
			Vector3 point = Vector3.zero;
			
			float angleToMove = currentMoveLeftRightSpeed_ * Time.deltaTime;
			
			if (DEBUG_CAMERAMOVER)
			{
				Debug.Log ( "Move "+(angleToMove)+" degrees  about pt "+point+" axis"+axis);
			}
			transform.RotateAround( point, axis, -1f * angleToMove);
			
			if (currentMoveLeftRightSpeed_ < 0f) 
			{
				currentMoveLeftRightSpeed_ -= moveAcceleration;
				currentMoveLeftRightSpeed_ = Mathf.Max ( currentMoveLeftRightSpeed_, -1f * maxMoveSpeedDegrees);
			}
			else if (currentMoveLeftRightSpeed_ > 0f) 
			{
				currentMoveLeftRightSpeed_ += moveAcceleration;
				currentMoveLeftRightSpeed_ = Mathf.Min ( currentMoveLeftRightSpeed_, maxMoveSpeedDegrees);
			}
		}
		

		if (currentZoomSpeed_ != 0f)
		{
			float newMagnitude = transform.position.magnitude + currentZoomSpeed_ * Time.deltaTime;
			Vector3 newPosition = transform.position * newMagnitude/transform.position.magnitude;
			if ( newPosition.magnitude >= minDistFromOrigin_  && newPosition.magnitude <= maxDistFromOrigin_ )
			{
				transform.position = newPosition;
				if (currentZoomSpeed_ < 0f)
				{
					currentZoomSpeed_ -= zoomAcceleration;
					currentZoomSpeed_ = Mathf.Max ( currentZoomSpeed_, -1f * maxZoomSpeed);
				}
				else if (currentZoomSpeed_ > 0f)
				{
					currentZoomSpeed_ += zoomAcceleration;
					currentZoomSpeed_ = Mathf.Min ( currentZoomSpeed_, maxZoomSpeed);
				}
			}
			else
			{
				Debug.Log ("Reached zoom end");
				currentZoomSpeed_ = 0f;
			}
		}

		if (currentRotateSpeed_ != 0f)
		{
			float angleToRotate = currentRotateSpeed_ * Time.deltaTime;

			transform.RotateAround( transform.position, transform.position, -1f * angleToRotate);

			if (currentRotateSpeed_ < 0f) 
			{
				currentRotateSpeed_ -= rotateAcceleration;
				currentRotateSpeed_ = Mathf.Max ( currentRotateSpeed_, -1f * maxRotateSpeedDegrees);
			}
			else if (currentRotateSpeed_ > 0f) 
			{
				currentRotateSpeed_ += rotateAcceleration;
				currentRotateSpeed_ = Mathf.Min ( currentRotateSpeed_, maxRotateSpeedDegrees);
			}

		}*/

	}
}
