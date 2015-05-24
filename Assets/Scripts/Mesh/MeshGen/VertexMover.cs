using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class VertexMover
	{
		private VertexListElement vertex_ = null;
		public VertexListElement Vertex
		{
			get { return vertex_; }
		}

		private float timeTaken_;

		private float timeSoFar_ =0f;
		
		private Vector3 initialPosition_;
		private Vector3 finalPosition_;
		
		private Vector3 direction_ = Vector3.zero;
		private float distance_;

		private bool finished_ = false;
		public bool Finished
		{
			get { return finished_; }
		}

		public VertexMover( VertexListElement v, Vector3 direction, float dist, float t)
		{
			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+direction.ToString()+" "+dist+" "+t);
			this.vertex_ = v;
			this.initialPosition_ = v.GetVector();
			this.direction_ = direction;
			this.direction_.Normalize();
			this.distance_ = dist;
			this.finalPosition_ = this.initialPosition_;
			this.timeTaken_ = t;
			this.timeSoFar_ = 0f;
		}

		public VertexMover( VertexListElement v, Vector3 final, float t)
		{
			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+final.ToString()+" "+t);
			this.vertex_ = v;
			this.initialPosition_ = v.GetVector();
			this.finalPosition_ = final;
			this.direction_ = Vector3.zero;
			this.distance_ = (final - initialPosition_).magnitude;
			this.timeTaken_ = t;
			this.timeSoFar_ = 0f;
		}

		private Vector3 FinalPosition
		{
			get { return (direction_.magnitude == 0f)?(finalPosition_):(initialPosition_+direction_*distance_); }
		}

		public bool update(float elapsed)
		{
			bool changed = false;
			if ( !finished_ )
			{
				Vector3 oldVector = vertex_.GetVector();

				timeSoFar_ += elapsed;
				if (timeSoFar_ > timeTaken_)
				{
					timeSoFar_ = timeTaken_;
					finished_ = true;
				}
				float fraction = timeSoFar_/timeTaken_;
				float dist = distance_ * fraction;
				Vector3 newVector = initialPosition_ + (FinalPosition - initialPosition_) * fraction;  

				//TODO stop if we hit another triangle
				vertex_.SetVector(newVector);
				changed = true;
				if (finished_)
				{
					if (direction_.magnitude==0f)
					{
						Debug.Log ("Finished Target move time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+finalPosition_);
					}
					else
					{
						Debug.Log ("Finished Direction move time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+FinalPosition+" with dir = "+direction_+" and dist = "+distance_);
					}
				}
				else
				{
//					Debug.Log ("Moved time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+finalPosition_);
				}
			}
			return changed;
		}
	}
}
