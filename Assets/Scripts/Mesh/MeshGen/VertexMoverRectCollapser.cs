using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class VertexMoverRectCollapser : VertexMover
	{
		public override void OnFinish()
		{
			Debug.LogWarning ( "Collapser finished. Removing Rect "+rect_.DebugDescribe() );
			rectList_.RemoveRect ( rect_ );
		}

		private VertexMoverTarget[] vertexMovers_ = new VertexMoverTarget[2]{null,null};

		private RectElement rect_;
		private VertexElement origin0;
		private VertexElement origin1;
		private VertexElement target0;
		private VertexElement target1;
		private MeshGenRectList rectList_;

//			private int originIndex0;
//			private int originIndex1;
//			private int targetIndex0;
//			private int targetIndex1;

		private VertexElement []vertexElementsToProtect = new VertexElement[0];

		private GridUVProviders.GridPosition movingPosition = MeshGenerator.greyRectGridPosition;

		public VertexMoverRectCollapser( MeshGenRectList rectList,
		                                RectElement rle, 
		                                VertexElement o0, VertexElement t0,
		                                VertexElement o1, VertexElement t1,
		                                VertexElement[] vtp,
		                                GridUVProviders.GridPosition mp,
		                                float t):base(t)
		{
			Debug.Log ("Creating RectCollapser: ");
			this.rectList_ = rectList;
//			this.originIndex0 = o0;
//			this.originIndex1 = o1;
//			this.targetIndex0 = t0;
//			this.targetIndex1 = t1;
			this.origin0 = o0;
			this.origin1 = o1;
			this.target0 = t0;
			this.target1 = t1;
			this.rect_ = rle;
			this.vertexElementsToProtect = vtp;
			if (mp!=null)
			{
				this.movingPosition = mp;
			}

			vertexMovers_[0]=new VertexMoverTarget( origin0, target0, timeTaken_);
			vertexMovers_[1]=new VertexMoverTarget( origin1, target1, timeTaken_);

			rect_.SetGridPosition(movingPosition);
		}


#region VertexMover

		public override bool MovesVertexElement(VertexElement el)
		{
			if ( vertexMovers_ [ 0 ].MovesVertexElement ( el ) || vertexMovers_ [ 1 ].MovesVertexElement ( el ) )
			{
				return true;
			}
			/*
			if ( target0 == el || target1 == el)
			{
				return true;
			}
			if (vertexElementsToProtect != null)
			{
				foreach (VertexListElement vli in vertexElementsToProtect)
				{
					if (vli == el)
					{
						return true;
					}
				}
			}
			*/
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
