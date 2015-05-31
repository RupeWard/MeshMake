using UnityEngine;
using System.Collections;

namespace MG.UV
{

	public class SimpleGridTextureUVProvider : RectUVProvider 
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
		

		public Material material;
		public int numColumns;
		public int numRows;
		public int buffer = 0;

		private MG.UV.GridPosition[] gridPositions_;

		private NewGridUVProvider gridUvProvider_ = null;

		[System.Serializable]
		public class StatePosition
		{
			public ElementStates.EState state = ElementStates.EState.NONE;
			public string name = string.Empty;
		}

		public StatePosition[] statePositions = new StatePosition[0];

		[System.Serializable]
		public class PositionDefinition
		{
			public string name = string.Empty;
			public int column =0;
			public int row = 0;
		}
		public PositionDefinition[] positionDefinitions = new PositionDefinition[0];

		private PositionDefinition positionForStatePosition( StatePosition statePos)
		{
			foreach ( PositionDefinition posdef in positionDefinitions)
			{
				if (posdef.name.Equals(statePos.name))
				{
					return posdef;
				}
			}
			return positionDefinitions[0];
		}

		void Awake()
		{
			float uvBuffer = 0f;
			if ( material.mainTexture != null )
			{
				uvBuffer = Mathf.Max( (float)buffer / (float)material.mainTexture.width, 
				                     (float)buffer/(float)material.mainTexture.height);
			}
			gridUvProvider_ = new NewGridUVProvider(numColumns, numRows, new GridPosition(0,0), uvBuffer);
			foreach ( StatePosition statePos in statePositions )
			{
				PositionDefinition pd = positionForStatePosition(statePos);

				gridUvProvider_.AddPositionForState( statePos.state, new GridPosition(pd.column, pd.row));
			}
		}

		#region  RectUVProvider
		public override Vector2 GetUVForState ( int triangleNumber, int vertexNumber, ElementStates.EState state )
		{
			return gridUvProvider_.GetUVForState(triangleNumber, vertexNumber, state);
		}
		#endregion  RectUVProvider

	}
}
