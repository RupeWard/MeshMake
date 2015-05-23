﻿using UnityEngine;
using System.Collections;

public class GridUVProviders 
{
	int numRows = 0;
	int numColumns = 0;

	public class GridPosition
	{
		public int row;
		public int column;
		public GridPosition(int r, int c)
		{
			row=r;
			column = c;
		}
	}

	public GridUVProviders( int r, int c)
	{
		numRows = r;
		numColumns = c;
	}

	private GridUVProviders ( ){}

	public I_UVProvider GetTriangleProvider(GridPosition pos)
	{
		float left = (float)pos.row/(float) numRows;
		float right = (float)( pos.row + 1 ) / (float)numRows;
		float bottom = (float)pos.column / (float)numColumns;
		float top = (float)( pos.column + 1 ) / (float)numColumns;

		I_UVProvider result = null;
		result = new UVProvider_Base (
			new Vector2[]{
				new Vector2(left,bottom),
				new Vector2(0.5f*(left+right),top),
				new Vector2(right,bottom)
			}
		 );
		return result;
	}

	public I_UVProvider GetTriangleProviderForRect(GridPosition pos, int triangleNumber)
	{
		float left =  (float)pos.row / (float)numRows;
		float right = (float)( pos.row + 1 )/ (float)numRows;
		float bottom = (float)pos.column / (float)numColumns;
		float top = (float)( pos.column + 1 ) / (float)numColumns;

		I_UVProvider result = null;
		if ( triangleNumber == 0 )
		{
			result = new UVProvider_Base (
				new Vector2[]{
				new Vector2(left,bottom),
				new Vector2(left,top),
				new Vector2(right,top)
			});
		}
		else
		{
			result = new UVProvider_Base (
				new Vector2[]{
				new Vector2(right,top),
				new Vector2(right,bottom),
				new Vector2(left,bottom)
			});
		}
		return result;
	}
}
