﻿using UnityEngine;
using System.Collections;

public class TetheredCameraMover : MonoBehaviour 
{
	private static readonly bool DEBUG_CAMERAMOVER = false; 

	public bool allowThroughOrigin = false;

	public float initialZoomSpeed = 10f;
	public float maxZoomSpeed = 50f;
	public float zoomAcceleration = 0.1f;

	public float initialRotateSpeedDegrees = 10f;
	public float maxRotateSpeedDegrees = 50f;
	public float rotateAcceleration = 0.1f;

	public float initialMoveSpeedDegrees = 10f;
	public float maxMoveSpeedDegrees = 50f;
	public float moveAcceleration = 0.1f;

	private float currentZoomSpeed_ = 0f;
	private float currentMoveUpDownSpeed_ = 0f;
	private float currentMoveLeftRightSpeed_ = 0f;
	private float currentRotateSpeed_ = 0f;

	public float tolerance = 0.001f;

	private float maxDistFromOrigin_;
	private float minDistFromOrigin_;

	private Camera camera_;
	
	void Awake()
	{
		camera_ = GetComponent< Camera > ( );
	}
	
	// Use this for initialization
	void Start () 
	{
		maxDistFromOrigin_ = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
		minDistFromOrigin_ = AppManager.Instance.camerabuffer;
	}
	
	public void ZoomIn()
	{
		currentZoomSpeed_ = -1f * initialZoomSpeed;
	}
	
	public void ZoomOut()
	{
		currentZoomSpeed_ = initialZoomSpeed;
	}

	public void ZoomStop ( )
	{
		currentZoomSpeed_ = 0f;
	}

	public void MoveUp()
	{
		currentMoveUpDownSpeed_ = initialMoveSpeedDegrees;
	}
	
	public void MoveDown()
	{
		currentMoveUpDownSpeed_ = -1f * initialMoveSpeedDegrees;
	}

	public void MoveLeft()
	{
		currentMoveLeftRightSpeed_ = -1f * initialMoveSpeedDegrees;
	}
	
	public void MoveRight()
	{
		currentMoveLeftRightSpeed_ = initialMoveSpeedDegrees;
	}
	

	public void MoveUpDownStop ( )
	{
		currentMoveUpDownSpeed_ = 0f;
	}

	public void MoveLeftRightStop ( )
	{
		currentMoveLeftRightSpeed_ = 0f;
	}

	public void RotateLeft()
	{
		currentRotateSpeed_ = -1f * initialRotateSpeedDegrees;
	}
	
	public void RotateRight()
	{
		currentRotateSpeed_ = initialRotateSpeedDegrees;
	}
	
	public void RotateStop ( )
	{
		currentRotateSpeed_ = 0f;
	}
	

	public void Stop()
	{
		ZoomStop ( );
		RotateStop ( );
		MoveUpDownStop ( );
		MoveLeftRightStop ( );
	}

	// Update is called once per frame
	void Update () 
	{
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

		}

	}
}
