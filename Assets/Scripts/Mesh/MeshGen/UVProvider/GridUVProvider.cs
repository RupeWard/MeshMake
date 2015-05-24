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
	
	public virtual Vector2 GetUVForTriangleIndex ( int i  )
	{
		return uvs[i];
	}
}
