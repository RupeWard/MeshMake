using UnityEngine;
using System.Collections;

public class GridUVProvider : UVProvider_Base
{
	private GridUVProviders.GridPosition position;

	public static int numRows = 0;
	public static int numColumns = 0;

	public GridUVProvider( Vector2[] baseUVs, GridUVProviders.GridPosition pos ): base(baseUVs)
	{
		position=pos;
	}

	public void SetGridPosition(GridUVProviders.GridPosition pos)
	{
		position = pos;
	}

	public override Vector2 GetUVForTriangleIndex ( int i )
	{
		Vector2 baseV = base.GetUVForTriangleIndex(i);

		float left =  (float)position.row / (float)numRows;
		float right = (float)( position.row + 1 )/ (float)numRows;
		float bottom = (float)position.column / (float)numColumns;
		float top = (float)( position.column + 1 ) / (float)numColumns;

		baseV.x = left + baseV.x * (right -left);
		baseV.y = bottom + baseV.y * (top -bottom);

		return baseV;
	}
	/*
	 * 		

	 */
}
