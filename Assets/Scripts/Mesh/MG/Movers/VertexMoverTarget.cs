using UnityEngine;
using System.Collections;

namespace MG
{
	public class VertexMoverTarget : VertexMover
	{
		private VertexElement vertex_ = null;
		public VertexElement Vertex
		{
			get { return vertex_; }
		}

		private VertexElement target_ = null;

		protected Vector3 FinalPosition()
		{
			return target_.GetVector(); 
		}


		private Vector3 initialPosition_;

		private readonly VertexElement origin_;

		public VertexMoverTarget( VertexElement v, VertexElement targ, VertexElement o, float t):base(t)
		{
//			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+targ.GetVector().ToString());
			this.vertex_ = v;
			this.target_ = targ;
			this.origin_ = o;
			this.initialPosition_ = v.GetVector();
			protectedEdges_.Add(
				new VertexElement[]{ origin_, target_ } );
		}

#region VertexMover

		public override bool MovesVertexElement(VertexElement el)
		{
			return vertex_ == el || target_ == el;
		}

		public override bool update(float elapsed)
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
//					Debug.Log ("Finished Target move time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+target_);
				}
				else
				{
//					Debug.Log ("Moved time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+finalPosition_);
				}
			}
			return changed;
		}
	}

#endregion VertexMover

}
