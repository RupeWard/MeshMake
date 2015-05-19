using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
	public Vector3 velocity;
	static public float maxDistFromOrigin = 50f;

	void Awake()
	{
	}

	public void Init(Vector3 pos, Vector3 vel)
	{
		this.transform.localPosition = pos;
		this.velocity = vel;
		Debug.Log ( "Made ball " + gameObject.name + " at " + pos + " with v = " + velocity );
	}

	void Start () 
	{
	}
	
	void Update () 
	{
		Vector3 startingPosition = transform.localPosition;
		transform.localPosition = transform.localPosition + velocity * Time.deltaTime;
		if ( transform.localPosition.magnitude > 1.5f * maxDistFromOrigin )
		{
			Debug.LogWarning ( "Destroying escaped ball "+gameObject.name+" at "+transform.localPosition+" dist "+transform.localPosition.magnitude+" max "+ maxDistFromOrigin);
			GameObject.Destroy ( this.gameObject );
		}
		else
			if ( transform.localPosition.magnitude >= maxDistFromOrigin )
		{
			Vector3 outsidePosition = transform.localPosition;
			transform.localPosition = startingPosition;

			float speed = velocity.magnitude;
			Vector3 newVelocity = Vector3.Reflect ( velocity, transform.localPosition);
			newVelocity = newVelocity * speed / newVelocity.magnitude;
			Debug.LogWarning("Ball "+gameObject.name+" reflected at " +transform.localPosition+" v from  "+velocity+" to "+newVelocity+" because mag = "+outsidePosition.magnitude);
			velocity = newVelocity;
		}
		else
		{
//			Debug.Log("Ball moved to "+this.transform.localPosition);
		}
	
	}
}
