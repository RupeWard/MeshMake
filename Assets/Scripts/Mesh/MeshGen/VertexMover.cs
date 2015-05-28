using UnityEngine;
using System.Collections;

namespace MG
{
	abstract public class VertexMover
	{
		public virtual void OnFinish()
		{
		}

		protected float timeTaken_;

		protected float timeSoFar_ =0f;

		protected bool finished_ = false;
		public bool Finished
		{
			get { return finished_; }
		}

		public VertexMover( float t)
		{
			this.timeTaken_ = t;
			this.timeSoFar_ = 0f;
		}


//		protected abstract Vector3 FinalPosition ( );
		public abstract bool update(float elapsed);
		public abstract bool MovesVertexElement(VertexElement el);
	}
}
