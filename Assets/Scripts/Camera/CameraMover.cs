using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float speed = 10f;
	public float tolerance = 0.001f;
	public float step = 0.1f;

	private float maxDist_;

	private bool isMoving_ = false;

	void Awake()
	{
	}

	private Vector3 dest_;

	// Use this for initialization
	void Start () 
	{
		maxDist_ = AppManager.Instance.maxDist - AppManager.Instance.camerabuffer;
		dest_ = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isMoving_) 
		{
			if (Vector3.Distance ( transform.position, dest_) > tolerance)
			{				
				transform.position = Vector3.MoveTowards( transform.position, dest_, speed * Time.deltaTime);
			}
			else
			{
				Debug.Log("Camera mover finished");
				isMoving_ = false;
			}
		}
	}

	void MoveInOrOut(bool moveIn)
	{
		Vector3 newDest = transform.position;
		if ( moveIn )
		{
			newDest = newDest * ( 1f - step );
		}
		else
		{
			newDest = newDest * ( 1f + step );
		}

		newDest = 0.5f * ( newDest + dest_ );

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
			Debug.Log("Move "+( (moveIn)?("In"):("Out") +" changed dest from" + dest_ +" to "+newDest ));
			dest_ = newDest;
			isMoving_ = true;
		}
		else
		{
			Debug.Log ("Move invalid");
		}
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
