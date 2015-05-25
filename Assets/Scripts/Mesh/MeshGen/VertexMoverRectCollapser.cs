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

		private RectListElement rect_;
		private VertexListElement origin0;
		private VertexListElement origin1;
		private VertexListElement target0;
		private VertexListElement target1;
		private MeshGenRectList rectList_;

			private int originIndex0;
			private int originIndex1;
			private int targetIndex0;
			private int targetIndex1;

		private GridUVProviders.GridPosition movingPosition = MeshGenerator.greyRectGridPosition;

		public VertexMoverRectCollapser( MeshGenRectList rectList,
		                                RectListElement rle, 
			                                int o0, int t0,
			                                int o1, int t1,
		                                GridUVProviders.GridPosition mp,
		                                float t):base(t)
		{
			Debug.Log ("Creating RectCollapser: ");
			this.rectList_ = rectList;
			this.originIndex0 = o0;
			this.originIndex1 = o1;
			this.targetIndex0 = t0;
			this.targetIndex1 = t1;
			this.origin0 = rectList_.vertexList.GetElement(o0);
			this.origin1 = rectList_.vertexList.GetElement(o1);
			this.target0 = rectList_.vertexList.GetElement(t0);
			this.target1 = rectList_.vertexList.GetElement(t1);
			this.rect_ = rle;
			if (mp!=null)
			{
				this.movingPosition = mp;
			}

			vertexMovers_[0]=new VertexMoverTarget( origin0, target0, timeTaken_);
			vertexMovers_[1]=new VertexMoverTarget( origin1, target1, timeTaken_);

			rect_.SetGridPosition(movingPosition);
		}


#region VertexMover

		public override bool MovesVertexIndex(VertexListElement el)
		{
			return vertexMovers_[0].MovesVertexIndex(el) || vertexMovers_[1].MovesVertexIndex(el);
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
