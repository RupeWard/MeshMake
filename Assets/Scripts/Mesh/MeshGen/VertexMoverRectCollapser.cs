using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class VertexMoverRectCollapser : VertexMover
	{
		private VertexMoverTarget[] vertexMovers_ = new VertexMoverTarget[2]{null,null};

		private RectListElement rect_;
		private VertexListElement origin0;
		private VertexListElement origin1;
		private VertexListElement target0;
		private VertexListElement target1;

		public VertexMoverRectCollapser( RectListElement rle, VertexListElement o0, VertexListElement t0, VertexListElement o1, VertexListElement t1, float t):base(t)
		{
			Debug.Log ("Creating RectCollapser: ");
			this.origin0 = o0;
			this.origin0 = o1;
			this.target0 = t0;
			this.target1= t1;
			this.rect_ = rle;

			vertexMovers_[0]=new VertexMoverTarget( o0, t0, timeTaken_);
			vertexMovers_[1]=new VertexMoverTarget( o1, t1, timeTaken_);
		}

		public override bool update(float elapsed)
		{
			bool changed = false;
			if ( !finished_ )
			{
				if (!vertexMovers_[0].Finished)
				{
					vertexMovers_[0].update(elapsed);
					changed = true;
				}

				if (!vertexMovers_[1].Finished)
				{
					vertexMovers_[1].update(elapsed);
					changed = true;
				}

				timeSoFar_ += elapsed;
				if (!changed || timeSoFar_ > timeTaken_)
				{
					timeSoFar_ = timeTaken_;
					finished_ = true;
				}
				//TODO stop if we hit another triangle

				if (finished_)
				{
					Debug.Log ("Finished Target move time "+timeSoFar_+" of "+timeTaken_);
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
