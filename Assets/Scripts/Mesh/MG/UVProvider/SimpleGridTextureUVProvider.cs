using UnityEngine;
using System.Collections;

namespace MG.UV
{

	public class SimpleGridTextureUVProvider : MonoBehaviour 
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
		public string[] subTextureNames;

		private MG.UV.GridPosition[] gridPositions_;

		private NewGridUVProvider gridUvProvider_ = null;

		[System.Serializable]
		public class StatePosition
		{
			public ElementStates.EState state = ElementStates.EState.NONE;
			public int column = 0;
			public int row = 0;
		}

		public StatePosition[] statePositions = new StatePosition[0];

		void Awake()
		{
			if ( subTextureNames.Length != numColumns * numRows )
			{
				Debug.LogError("UVProvider "+gameObject.name+" has wrong number of names"); 
			}
			/*
			gridPositions_ = new MG.UV.GridPosition[ numColumns * numRows];
			for ( int r=0; r<numRows; r++ )
			{
				for ( int c=0; c<numColumns; c++ )
				{
					gridPositions_[ c+r*numColumns] = new MG.UV.GridPosition(c,r);
				}
			}*/
			gridUvProvider_ = new NewGridUVProvider(numColumns, numRows, new GridPosition(0,0));
			foreach ( StatePosition statePos in statePositions )
			{
				gridUvProvider_.AddPositionForState( statePos.state, new GridPosition(statePos.column, statePos.row));
			}
/*
			gridUvProvider_.AddPositionForState(ElementStates.EState.Original, cyanRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.GrowingRand, redRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.GrowingClicked, greenRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.GrowingBall, mauveRectGridPosition);
			
			gridUvProvider_.AddPositionForState(ElementStates.EState.CollapsingRand, yellowRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.CollapsingClicked, yellowRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.CollapsingBall, yellowRectGridPosition);
			
			gridUvProvider_.AddPositionForState(ElementStates.EState.StaticRand, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.StaticClicked, blueRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.StaticBall, purpleRectGridPosition);
			
			gridUvProvider_.AddPositionForState(ElementStates.EState.Rand, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.Clicked, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.Ball, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.Collapsing, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.Growing, blackRectGridPosition);
			gridUvProvider_.AddPositionForState(ElementStates.EState.Static, blackRectGridPosition);

*/
		}

		void Start () 
		{
	
		}
	
		void Update () 
		{
	
		}
	}
}
