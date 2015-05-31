using UnityEngine;
using System.Collections;

//  NOT YET USED
public class SimpleGridTextureUVProvider : MonoBehaviour 
{
	public Texture2D texture;
	public int numColumns;
	public int numRows;
	public string[] subTextureNames;

	private MG.UV.GridPosition[] gridPositions_;

	void Awake()
	{
		if ( subTextureNames.Length != numColumns * numRows )
		{
			Debug.LogError("UVProvider "+gameObject.name+" has wrong number of names"); 
		}
		gridPositions_ = new MG.UV.GridPosition[ numColumns * numRows];
		for ( int r=0; r<numRows; r++ )
		{
			for ( int c=0; c<numColumns; c++ )
			{
				gridPositions_[ c+r*numColumns] = new MG.UV.GridPosition(c,r);
			}
		}
	}

	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}
}
