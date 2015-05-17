using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float linearSpeed = 10f;
	private float angularSpeed = 10f;

	public float tolerance = 0.001f;
	public float inOutStep = 0.1f;
	public float rotateStepDegrees = 10f;

	private float maxDist_;

	private bool isMoving_ = false;
	private bool isRotating_ = false;

	private Vector3 destPosition_;
//	private Quaternion destRotation_;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
		maxDist_ = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
		destPosition_ = transform.position;
	 	destRotationDegrees_ = 0f;
	}

	public void Stop()
	{
		destPosition_ = transform.position;
		destRotationDegrees_ = 0f;
		isMoving_ = false;
		isRotating_ = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if (isMoving_) 
		{
			bool bHasMoved = false;
			if (Vector3.Distance ( transform.position, destPosition_) > tolerance)
			{				
				transform.position = Vector3.MoveTowards( transform.position, destPosition_, linearSpeed * Time.deltaTime);
				bHasMoved = true;
			}
			if (!bHasMoved)
			{
				isMoving_ = false;
				Debug.Log("Camera mover finished");
			}
		}
		if (isRotating_)
		{
			bool bHasRotated = false;
			if (Mathf.Abs(destRotationDegrees_) > 0f && Mathf.Abs(angleRotatedSoFar_) < Mathf.Abs (destRotationDegrees_))
			{
				float sign = (destRotationDegrees_ > 0f)?(1f):(-1f);
				float angleToRotate = sign * angularSpeed * Time.deltaTime;

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

	float angleRotatedSoFar_ = 0f;
	float destRotationDegrees_ = 0f;

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

	void MoveInOrOut(bool moveIn)
	{
		Vector3 newDest = transform.position;
		if ( moveIn )
		{
			newDest = newDest * ( 1f - inOutStep );
		}
		else
		{
			newDest = newDest * ( 1f + inOutStep );
		}

		newDest = 0.5f * ( newDest + destPosition_ );

		/*
		if ( isMoving_ )
		{
			Vector3 normedDest = dest_;
			normedDest .Normalize();
			newDest = 0.5f * (newDest + normedDest);
			newDest.Normalize();
			Debug.Log("Combined movements");
		}*/

//		newDest = newDest * step;
		if ( IsValidDest ( newDest ) )
		{
			Debug.Log("Move "+( (moveIn)?("In"):("Out") +" changed dest from" + destPosition_ +" to "+newDest ));
			destPosition_ = newDest;
			isMoving_ = true;
		}
		else
		{
			Debug.Log ("Move invalid");
		}
	}

	public void RotateLeft()
	{
		RotateLeftOrRight ( true );
	}

	public void RotateRight()
	{
		RotateLeftOrRight ( false );
	}

	public void MoveIn()
	{
		MoveInOrOut ( true );
	}

	public void MoveOut()
	{
		MoveInOrOut ( false );
	}

	private bool IsValidDest(Vector3 v)
	{
		return ( v.magnitude < maxDist_ );
	}
}
