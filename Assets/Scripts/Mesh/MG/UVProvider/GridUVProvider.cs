using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG.UV
{
	public class GridUVProvider : RectUVProvider
	{
		static public readonly UV.GridPosition cyanRectGridPosition = new UV.GridPosition ( 0,0 );
		static public readonly UV.GridPosition greyRectGridPosition = new UV.GridPosition( 1,0);// grey in color3x3
		static public readonly UV.GridPosition greenRectGridPosition = new UV.GridPosition( 2,0);// green in color3x3

		static public readonly UV.GridPosition mauveRectGridPosition = new UV.GridPosition ( 0,1 );
		static public readonly UV.GridPosition purpleRectGridPosition = new UV.GridPosition( 1,1);// purpkke in color3x3
		static public readonly UV.GridPosition blackRectGridPosition = new UV.GridPosition( 2,1);// black in color3x3

		static public readonly UV.GridPosition yellowRectGridPosition = new UV.GridPosition ( 0,2 );
		static public readonly UV.GridPosition redRectGridPosition = new UV.GridPosition( 1,2);// purpkke in color3x3
		static public readonly UV.GridPosition blueRectGridPosition = new UV.GridPosition( 2,2);// blue in color3x3

		private ElementStates.EState state_ = ElementStates.EState.NONE;
		private GridPosition position_ = blackRectGridPosition;
		private float bottom = 0f;
		private float top = 0f;
		private float left = 0f;
		private float right  = 0f;
		private Vector2 [][] baseUVset_ = null;
		
		protected Vector2 [] baseUVs_ = null;
		
		private int numRows_ = 0;
		private int numColumns_ = 0;
		
		

		public void SetState(ElementStates.EState state)
		{
			if ( state_ != state )
			{
				ElementStates.EState oldState = state_;
				state_ = state;

				SetPosition(GetPositionForState(state_));
			}
		}

		private Dictionary< ElementStates.EState, UV.GridPosition> positionsByState = new Dictionary<ElementStates.EState, GridPosition> ( )
		{
			{ ElementStates.EState.NONE, blackRectGridPosition },
			{ ElementStates.EState.Original, cyanRectGridPosition },

			{ ElementStates.EState.GrowingRand, redRectGridPosition },
			{ ElementStates.EState.GrowingClicked, greenRectGridPosition },
			{ ElementStates.EState.GrowingBall, mauveRectGridPosition },

			{ ElementStates.EState.CollapsingRand, yellowRectGridPosition },
			{ ElementStates.EState.CollapsingClicked, yellowRectGridPosition },
			{ ElementStates.EState.CollapsingBall, yellowRectGridPosition },

			{ ElementStates.EState.StaticRand, blackRectGridPosition },
			{ ElementStates.EState.StaticClicked, blueRectGridPosition },
			{ ElementStates.EState.StaticBall, purpleRectGridPosition },

			{ ElementStates.EState.Rand, blackRectGridPosition },
			{ ElementStates.EState.Clicked, blackRectGridPosition },
			{ ElementStates.EState.Ball, blackRectGridPosition },
			{ ElementStates.EState.Collapsing, blackRectGridPosition },
			{ ElementStates.EState.Growing, blackRectGridPosition },
			{ ElementStates.EState.Static, blackRectGridPosition }
		};

		public UV.GridPosition GetPositionForState(ElementStates.EState state)
		{
			UV.GridPosition result = cyanRectGridPosition;
			if ( positionsByState.ContainsKey ( state ) )
			{
				result = positionsByState [ state ];
//				Debug.LogWarning("Found position "+result.DebugDescribe()+"for state "+state);
			}
			else
			{
				Debug.LogError("No position for state "+state);
			}
			return result;
		}

		public GridUVProvider(int r, int c)
		{
			numRows_ = r;
			numColumns_ = c;
			baseUVset_ = new Vector2[][]
			{
				new Vector2[]{
					new Vector2 ( 0f, 0f ),
					new Vector2 ( 0f, 1f ),
					new Vector2 ( 1f, 1f )
				},
				new Vector2[]{
					new Vector2 ( 1f, 1f ),
					new Vector2 ( 1f, 0f ),
					new Vector2 ( 0f, 0f )
				},
			};
		}

		private GridUVProvider()
		{
		}

		int currentTriangleNumber_ = -1;

		private Vector2 GetUVForTriangleIndex ( int triangleNumber, int vertexNumber )
		{
			if (triangleNumber != currentTriangleNumber_)
			{
				currentTriangleNumber_ = triangleNumber;
				baseUVs_ = baseUVset_[triangleNumber];// baseUVset_[triangleNumber];_
//				Debug.Log ("UVs changed for triangle "+triangleNumber+" = "+uvs.Length);
			}
			Vector2 baseV = baseUVs_[vertexNumber];

			baseV.x = left + baseV.x * (right -left);
			baseV.y = bottom + baseV.y * (top -bottom);

//			Debug.LogWarning ("UV "+vertexNumber+" (tri "+triangleNumber+") for "+state_+" "+position_.DebugDescribe()+" = "+baseV);
			return baseV;
		}

		public void GetUVsForState( int triangleNumber, ElementStates.EState state, ref Vector2[] uvsOut)
		{
			for(int i=0; i<3; i++)
			{
				uvsOut[i] = GetUVForState(triangleNumber, i, state); 
			}
		}

		public override Vector2 GetUVForState (int triangleNumber, int vertexNumber, ElementStates.EState state )
		{
			Vector2 uv = Vector2.zero;
			SetState(state);
			uv = GetUVForTriangleIndex ( triangleNumber, vertexNumber );
			return uv;
		}

		private void SetPosition(GridPosition pos)
		{

			bottom = (float)pos.row/(float) numRows_;
			top = (float)( pos.row + 1 ) / (float)numRows_;
			left = (float)pos.column / (float)numColumns_;
			right = (float)( pos.column + 1 ) / (float)numColumns_;

			if ( float.IsNaN ( bottom ) || float.IsNaN ( top ) || float.IsNaN ( left ) || float.IsNaN ( right ) )
			{
				Debug.LogError("NAN!!"
				               +"\nnumRows = "+numRows_
				               +"\nnumColumns = "+numColumns_
				               );
			}
		}

	}

}
