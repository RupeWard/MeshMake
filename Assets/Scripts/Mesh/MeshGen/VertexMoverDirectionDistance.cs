using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class VertexMoverDirectionDistance : VertexMover
	{
		public override void OnFinish()
		{
			HashSet< RectElement > rles = vertex_.rects;
			foreach( RectElement rle in rles)
			{
				rle.SetGridPosition( finalGridPosition_ );
			}
		}

		private VertexElement originVertex_ = null;

		private VertexElement vertex_ = null;
		public VertexElement Vertex
		{
			get { return vertex_; }
		}

		private Vector3 initialPosition_;
		private Vector3 finalPosition_;
		
		private Vector3 direction_ = Vector3.zero;
		private float distance_;

		private GridUVProviders.GridPosition finalGridPosition_ = MG.MeshGenerator.redRectGridPosition;

		public VertexMoverDirectionDistance(VertexElement o, 
		                                    VertexElement v, 
		                                    Vector3 direction, 
		                                    float dist, float t, 
		                                    GridUVProviders.GridPosition fp): base(t)
		{
//			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+direction.ToString()+" "+dist+" "+t);
			this.originVertex_ = o;
			this.vertex_ = v;
			this.initialPosition_ = v.GetVector();
			this.direction_ = direction;
			this.direction_.Normalize();
			this.distance_ = dist;
			this.finalPosition_ = this.initialPosition_;
			if (fp != null)
			{
				finalGridPosition_ = fp;
			}
		}

		public VertexMoverDirectionDistance(VertexElement o,  VertexElement v, Vector3 final, float t, GridUVProviders.GridPosition fp):base(t)
		{
//			Debug.Log ("Creating VertexMover: "+v.GetVector ().ToString()+" "+final.ToString()+" "+t);
			this.originVertex_ = o;
			this.vertex_ = v;
			this.initialPosition_ = v.GetVector();
			this.finalPosition_ = final;
			this.direction_ = Vector3.zero;
			this.distance_ = (final - initialPosition_).magnitude;
			if (fp != null)
			{
				finalGridPosition_ = fp;
			}
		}

		protected Vector3 FinalPosition()
		{
			return (direction_.magnitude == 0f)?(finalPosition_):(initialPosition_+direction_*distance_); 
		}

#region VertexMover

		public override bool MovesVertexElement(VertexElement el)
		{
			return this.Vertex == el;
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
				float dist = distance_ * fraction;
				Vector3 newVector = initialPosition_ + (FinalPosition() - initialPosition_) * fraction;  

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
						Debug.Log ("Finished Direction move time "+timeSoFar_+" of "+timeTaken_+" from "+initialPosition_+" to "+vertex_.GetVector()+" towards "+FinalPosition()+" with dir = "+direction_+" and dist = "+distance_);
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

#endregion VertexMover

}
