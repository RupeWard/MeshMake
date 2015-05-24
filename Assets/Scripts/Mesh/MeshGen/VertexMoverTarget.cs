using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class VertexMoverTarget
	{
		private VertexListElement vertex_ = null;
		public VertexListElement Vertex
		{
			get { return vertex_; }
		}

		private VertexListElement target_ = null;

		private float timeTaken_;

		private float timeSoFar_ =0f;
		
		private Vector3 initialPosition_;

		private bool finished_ = false;
		public bool Finished
		{
			get { return finished_; }
		}

		public VertexMoverTarget( VertexListElement v, VertexListElement targ, float t)
		{
			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+targ.GetVector().ToString());
			this.vertex_ = v;
			this.target_ = targ;
			this.initialPosition_ = v.GetVector();
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

				Vector3 newVector = initialPosition_ + (target_.GetVector() - initialPosition_) * fraction;  

				//TODO stop if we hit another triangle
				vertex_.SetVector(newVector);
				changed = true;
				if (finished_)
				{
					Debug.Log ("Finished Target move time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+target_);
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
