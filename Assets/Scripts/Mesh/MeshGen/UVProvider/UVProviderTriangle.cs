using UnityEngine;
using System.Collections;

public class GridUVProvider : I_UVProvider 
{
	protected Vector2[] uvs = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(0.5f, 1f),
		new Vector2(1f, 0f)
	};

	#region I_UVProvider

	public Vector2 GetUVForTriangleIndex ( int i )
	{
		return uvs[i];
	}

	#endregion I_UVProvider

}
