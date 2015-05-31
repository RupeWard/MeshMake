using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG.UV
{
	public class NewGridUVProvider : I_RectUVProvider
	{
		private ElementStates.EState state_ = ElementStates.EState.NONE;
		private GridPosition position_;
		private float bottom = 0f;
		private float top = 0f;
		private float left = 0f;
		private float right  = 0f;
		private Vector2 [][] baseUVset_ = null;
		
		protected Vector2 [] baseUVs_ = null;
		
		private int numRows_ = 0;
		private int numColumns_ = 0;
		
		UV.GridPosition defaultRectGridPosition_;


		public void SetState(ElementStates.EState state)
		{
			if ( state_ != state )
			{
				ElementStates.EState oldState = state_;
				state_ = state;

				/*
				GridPosition gp = GetPositionForState(state_);
				SetPosition(gp);
				Debug.LogWarning ("State changed from "+oldState+" to "+state_+", pos = "+gp.DebugDescribe());
				*/
				SetPosition(GetPositionForState(state_));
			}
		}

		public void AddPositionForState(ElementStates.EState state, UV.GridPosition pos)
		{
			if ( positionsByState.ContainsKey ( state ) )
			{
				positionsByState[state] = pos;
			}
			else
			{
				positionsByState.Add( state, pos);
			}
		}

		private Dictionary< ElementStates.EState, UV.GridPosition> positionsByState = new Dictionary<ElementStates.EState, GridPosition> ( )
		{
		};

		public UV.GridPosition GetPositionForState(ElementStates.EState state)
		{
			UV.GridPosition result = defaultRectGridPosition_;
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

		public NewGridUVProvider(int c, int r, GridPosition d)
		{
			defaultRectGridPosition_ = d;
			position_ = defaultRectGridPosition_;
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

		private NewGridUVProvider()
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

		public Vector2 GetUVForState (int triangleNumber, int vertexNumber, ElementStates.EState state )
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
