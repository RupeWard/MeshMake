using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float zoomSpeed = 10f;
	private float rotateSpeedDegrees = 10f;
	private float moveSpeedDegrees = 10f;

	public float tolerance = 0.001f;
	public float zoomStep = 0.1f;
	public float rotateStepDegrees = 10f;
	public float moveStepDegrees = 10f;

	private float maxDistFromOrigin_;

	private bool isZooming_ = false;
	private bool isRotating_ = false;
	private bool isMoving_ = false;

	private float destZoomDistance_ = 0f;
	private float distZoomedSoFar_ = 0f;

	private float angleRotatedSoFar_ = 0f;
	private float destRotationDegrees_ = 0f;
	

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
		maxDistFromOrigin_ = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
		destZoomDistance_  = 0f;
		distZoomedSoFar_ = 0f;
	 	destRotationDegrees_ = 0f;
		angleRotatedSoFar_ = 0f;
	}

	public void Stop()
	{
		destZoomDistance_  = 0f;
		destRotationDegrees_ = 0f;
		isZooming_ = false;
		isRotating_ = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if (isZooming_) 
		{
			bool bHasZoomed = false;
			if (Mathf.Abs(destZoomDistance_) > 0f && Mathf.Abs(distZoomedSoFar_) < Mathf.Abs (destZoomDistance_))
			{
				float sign = (destZoomDistance_ > 0f)?(1f):(-1f);
				float distToMove = sign * zoomSpeed * Time.deltaTime;
				
				if ( Mathf.Abs(distZoomedSoFar_ + distToMove) > Mathf.Abs (destZoomDistance_))
				{
					Debug.Log("Zoom ending when "+distZoomedSoFar_+" of "+destZoomDistance_+" with "+distToMove+" to add");
					distToMove = destZoomDistance_ - distZoomedSoFar_;
					distZoomedSoFar_ = destZoomDistance_;
					destZoomDistance_ = 0f;
				}
				else
				{
					distZoomedSoFar_ += distToMove;
				}
				
				transform.position = Vector3.MoveTowards( transform.position, Vector3.zero, -1f * distToMove);
				bHasZoomed = true;
			}
			if (!bHasZoomed)
			{
				isZooming_ = false;
				Debug.Log("Camera zoom finished with zoomDist = "+destZoomDistance_+", soFar ="+distZoomedSoFar_);
			}
		}
		if (isRotating_)
		{
			bool bHasRotated = false;
			if (Mathf.Abs(destRotationDegrees_) > 0f && Mathf.Abs(angleRotatedSoFar_) < Mathf.Abs (destRotationDegrees_))
			{
				float sign = (destRotationDegrees_ > 0f)?(1f):(-1f);
				float angleToRotate = sign * rotateSpeedDegrees * Time.deltaTime;

				if ( Mathf.Abs(angleRotatedSoFar_ + angleToRotate) > Mathf.Abs (destRotationDegrees_))
				{
					angleToRotate = destRotationDegrees_ - angleRotatedSoFar_;
					angleRotatedSoFar_ = destRotationDegrees_;
					destRotationDegrees_ = 0f;
				}
				else
				{
					angleRotatedSoFar_ += angleToRotate;
				}

				transform.RotateAround( transform.position, transform.position, -1f * angleToRotate);
				bHasRotated = true;
			}
			if (!bHasRotated)
			{
				isRotating_ = false;
				Debug.Log("Camera rotate finished");
			}
		}

	}

	void RotateLeftOrRight(bool left)
	{
		angleRotatedSoFar_ = 0f;

		float oldDest = destRotationDegrees_;
		destRotationDegrees_ = rotateStepDegrees;
		if ( left )
		{
			destRotationDegrees_ *= -1f;
		}
		Debug.Log("Rotate "+( (left)?("Left"):("Right")) +" changed destRotation from" + oldDest+" to " +destRotationDegrees_ );
		isRotating_ = true;
	}

	void ZoomInOrOut(bool moveIn)
	{
		distZoomedSoFar_ = 0f;
		float oldDest = destZoomDistance_;
		destZoomDistance_ = zoomStep;
		if ( moveIn )
		{
			destZoomDistance_ *= -1f;
		}
		Debug.Log("Zoom "+( (moveIn)?("In"):("Out")) +" changed destZoom from" + oldDest+" to " +destZoomDistance_ );

		isZooming_ = true;
	}

	public void RotateLeft()
	{
		RotateLeftOrRight ( true );
	}

	public void RotateRight()
	{
		RotateLeftOrRight ( false );
	}

	public void ZoomIn()
	{
		ZoomInOrOut ( true );
	}

	public void ZoomOut()
	{
		ZoomInOrOut ( false );
	}

	private bool IsValidDest(Vector3 v)
	{
		return ( v.magnitude < maxDistFromOrigin_ );
	}
}
