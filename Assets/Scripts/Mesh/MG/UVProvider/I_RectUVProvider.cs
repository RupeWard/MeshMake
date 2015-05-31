using UnityEngine;
using System.Collections;

namespace MG.UV
{
	public interface I_RectUVProvider 
	{
		Vector2 GetUVForState ( int triangleNumber, int vertexNumber, ElementStates.EState state );
	}
}
		                          