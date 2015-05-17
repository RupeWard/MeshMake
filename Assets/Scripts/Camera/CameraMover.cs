using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public bool allowThroughOrigin = false;

	public float zoomSpeed = 10f;
	private float rotateSpeedDegrees = 10f;
	private float moveSpeedDegrees = 10f;

	public float tolerance = 0.001f;
	public float zoomStep = 0.1f;
	public float rotateStepDegrees = 10f;
	public float moveStepDegrees = 10f;

	private float maxDistFromOrigin_;
	private float minDistFromOrigin_;

	private bool isZooming_ = false;
	private bool isRotating_ = false;
	private bool isMoving_ = false;

	private float destZoomDistance_ = 0f;
	private float distZoomedSoFar_ = 0f;

	private float angleRotatedSoFar_ = 0f;
	private float destRotationDegrees_ = 0f;
	
	private float angleMovedSoFar_ = 0f;
	private float destMoveDegrees_ = 0f;

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
		Stop ( );
	}

	public void Stop()
	{
		destZoomDistance_  = 0f;
		distZoomedSoFar_ = 0f;
		destRotationDegrees_ = 0f;
		angleRotatedSoFar_ = 0f;
		destMoveDegrees_ = 0f;
		angleMovedSoFar_ = 0f;		
		isZooming_ = false;
		isRotating_ = false;
		isMoving_ = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if ( isMoving_ )
		{
			bool bHasMoved = false;
			if (Mathf.Abs(destMoveDegrees_) > 0f && Mathf.Abs(angleMovedSoFar_) < Mathf.Abs (destMoveDegrees_))
			{
				float sign = (destMoveDegrees_ > 0f)?(1f):(-1f);
				float angleToMove = sign * moveSpeedDegrees * Time.deltaTime;
				
				if ( Mathf.Abs(angleMovedSoFar_ + angleToMove) > Mathf.Abs (destMoveDegrees_))
				{
					angleToMove = destMoveDegrees_ - angleMovedSoFar_;
					angleMovedSoFar_ = 0f;
					destMoveDegrees_ = 0f;
				}
				else
				{
					angleMovedSoFar_ += angleToMove;
				}

				Vector3 v0 = camera_.ScreenToWorldPoint( new Vector3( 0f, -100f, camera_.nearClipPlane ) );
				Vector3 v1 = camera_.ScreenToWorldPoint( new Vector3( 0f, 100f, camera_.nearClipPlane ) );
				Vector3 line1 = v1-v0; // horiz line in screen

				Debug.Log ("line is "+line1);

				Vector3 axis = Vector3.Cross(line1,transform.position);
				Vector3 point = Vector3.zero;

				Debug.Log ( "Move "+(sign*angleToMove)+" degrees  about pt "+point+" axis"+axis);
				transform.RotateAround( point, axis, -1f * angleToMove);
				bHasMoved = true;
			}
			if (!bHasMoved)
			{
				isMoving_ = false;
				Debug.Log("Camera move finished");
			}

		}

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
				if (sign > 0 && transform.position.magnitude >= maxDistFromOrigin_)
				{
					Debug.Log ("Zoom ending when reached maxDist");
					destZoomDistance_ = 0f;
					distZoomedSoFar_ = 0f;
				}
				else if (sign < 0 && transform.position.magnitude <= minDistFromOrigin_)
				{
					Debug.Log ("Zoom ending when reached minDist");
					destZoomDistance_ = 0f;
					distZoomedSoFar_ = 0f;
				}
				bHasZoomed = true;
			}
			if (!bHasZoomed)
			{
				isZooming_ = false;
				Debug.Log("Camera zoom finished");
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

	void MoveUpOrDown(bool up)
	{
		angleMovedSoFar_ = 0f;

		float oldDest = destMoveDegrees_;
		destMoveDegrees_ = moveStepDegrees;
		if ( !up )
		{
			destMoveDegrees_ *= -1f;
		}
		Debug.Log("Rotate "+( (up)?("Up"):("Down")) +" changed destRotation from" + oldDest+" to " +destMoveDegrees_ );
		isMoving_ = true;
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

	public void MoveUp()
	{
		MoveUpOrDown ( true );
	}

	public void MoveDown()
	{
		MoveUpOrDown ( false );
	}

	private bool IsValidDest(Vector3 v)
	{
		return ( v.magnitude < maxDistFromOrigin_ );
	}
}
