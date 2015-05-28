using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class RectMover
	{
		private VertexElement vertex_ = null;
		private Vector3 initialPosition_;
		private Vector3 direction_;
		private float timeTaken_;
		private float distance_;

		private float timeSoFar_ =0f;

		private Vector3 finalPosition_;

		private bool finished_ = false;
		public bool Finished
		{
			get { return finished_; }
		}

		public RectMover( VertexElement v, Vector3 direction, float dist, float t)
		{
			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+direction.ToString()+" "+dist+" "+t);
			this.vertex_ = v;
			this.initialPosition_ = v.GetVector();
			this.direction_ = direction;
			this.direction_.Normalize();
			this.distance_ = dist;
			this.finalPosition_ = this.initialPosition_ + this.direction_ * dist;
			this.timeTaken_ = t;
			this.timeSoFar_ = 0f;
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
				Vector3 newVector = initialPosition_ + (finalPosition_ - initialPosition_) * fraction;  

				//TODO stop if we hit another triangle
				vertex_.SetVector(newVector);
				changed = true;
				Debug.Log ("Moved time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+finalPosition_);
			}
			return changed;
		}
	}
}
