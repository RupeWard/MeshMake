using UnityEngine;
using System.Collections;

public class PhysBall : MonoBehaviour 
{
	private Rigidbody rigidBody_; 

	void Awake()
	{
		rigidBody_ = GetComponent< Rigidbody > ( );
	}

	public void Init(Vector3 pos, Vector3 vel)
	{
		transform.localPosition = pos;
		//rigidBody_.position = pos;
		//rigidBody_.AddForce(vel);
		//rigidBody_.velocity = Vector3.zero;
		rigidBody_.velocity = vel;
		Debug.Log ( "Created PhysBall with pos = " + transform.localPosition + " v = " + vel + " or " +rigidBody_.velocity);
	}
	private bool dead=false;
	void FixedUpdate () 
	{
		if ( dead )
			return;
		else if ( transform.localPosition.magnitude > 1.5f * Ball.maxDistFromOrigin )
		{
			Debug.LogWarning ( "Destroying escaped ball "+gameObject.name+" at "+transform.localPosition+" dist "+transform.localPosition.magnitude+" max "+ Ball.maxDistFromOrigin+" vel = "+rigidBody_.velocity);
//			GameObject.Destroy ( this.gameObject );
			rigidBody_.velocity = Vector3.zero;
			dead = true;
		}
		/*
		else
			if (transform.localPosition.magnitude >= Ball.maxDistFromOrigin )
		{
			float speed = rigidBody_.velocity.magnitude;
			Vector3 newVelocity = Vector3.Reflect ( rigidBody_.velocity, transform.localPosition);
			newVelocity = newVelocity * speed / newVelocity.magnitude;
			Debug.LogWarning("Ball "+gameObject.name+" reflected at " +transform.localPosition+" v from  "+rigidBody_.velocity+" to "+newVelocity+" because mag = "+transform.localPosition.magnitude);
			transform.localPosition = transform.localPosition - rigidBody_.velocity * Time.deltaTime;
			rigidBody_.velocity = newVelocity;

		}
		else
		{
			Debug.Log("Ball moved to "+transform.localPosition+" v = "+rigidBody_.velocity);
		}*/
		
	}

}
