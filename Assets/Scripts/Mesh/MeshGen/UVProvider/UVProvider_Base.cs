using UnityEngine;
using System.Collections;

public class UVProvider_Base : I_UVProvider 
{
	protected Vector2 [] uvs = null;

	public UVProvider_Base( Vector2[] baseUVs )
	{
		uvs = baseUVs;
	}

	protected UVProvider_Base(){}


#region I_UVProvider

	public Vector2 GetUVForTriangleIndex ( int i )
	{
		return uvs[i];
	}

	#endregion I_UVProvider

}
