using UnityEngine;
using System.Collections;

namespace MG
{
	public class VertexMoverRectCollapser : VertexMover
	{
		public override void OnFinish()
		{
			Debug.LogWarning ( "Collapser finished. Removing Rect "+rect_.DebugDescribe() );
			rectList_.RemoveElement ( rect_ );
		}

		private VertexMoverTarget[] vertexMovers_ = new VertexMoverTarget[2]{null,null};

		private RectElement rect_;
		private VertexElement origin0;
		private VertexElement origin1;
		private VertexElement target0;
		private VertexElement target1;
		private RectList rectList_;

		private VertexElement []vertexElementsToProtect = new VertexElement[0];

		private ElementStates.EState movingState = ElementStates.EState.NONE;

		public VertexMoverRectCollapser( RectList rectList,
		                                RectElement rle, 
		                                VertexElement o0, VertexElement t0,
		                                VertexElement o1, VertexElement t1,
		                                VertexElement[] vtp,
		                                ElementStates.EState s,
		                                float t):base(t)
		{
			Debug.Log ("Creating RectCollapser: ");
			this.rectList_ = rectList;
			this.origin0 = o0;
			this.origin1 = o1;
			this.target0 = t0;
			this.target1 = t1;
			this.rect_ = rle;
			this.vertexElementsToProtect = vtp;
			this.movingState  = s;
			vertexMovers_[0]=new VertexMoverTarget( origin0, target0, timeTaken_);
			vertexMovers_[1]=new VertexMoverTarget( origin1, target1, timeTaken_);

			rect_.SetState(movingState);
		}


#region VertexMover

		public override bool MovesVertexElement(VertexElement el)
		{
			if ( vertexMovers_ [ 0 ].MovesVertexElement ( el ) || vertexMovers_ [ 1 ].MovesVertexElement ( el ) )
			{
				return true;
			}
			return false;
		}

		public override bool update(float elapsed)
		{
			bool changed = false;
			if ( !finished_ )
			{
				if (!vertexMovers_[0].Finished)
				{
					changed |= vertexMovers_[0].update(elapsed);
				}

				if (!vertexMovers_[1].Finished)
				{
					changed |= vertexMovers_[1].update(elapsed);
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
					Debug.Log ("Finished RectCollapser "+timeSoFar_+" of "+timeTaken_);
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
