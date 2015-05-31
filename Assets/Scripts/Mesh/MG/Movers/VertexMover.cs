using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	abstract public class VertexMover
	{
		protected List< VertexElement[] > protectedEdges_ = new List<VertexElement[]>();
		public bool IsProtectedEdge( VertexElement vle0, VertexElement vle1)
		{
			foreach (VertexElement[] vle in protectedEdges_)
			{
				if ((vle[0] == vle0 && vle[1] == vle1) 
				    || (vle[1] ==vle0 && vle[0] ==vle1))
				{
					return true;
				}
			}
			return false;
		}

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
