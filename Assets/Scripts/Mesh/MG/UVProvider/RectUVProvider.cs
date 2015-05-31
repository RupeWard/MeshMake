using UnityEngine;
using System.Collections;

namespace MG.UV
{
	public abstract class RectUVProvider : MonoBehaviour, I_RectUVProvider
	{
		#region I_RectUVProvider 
		abstract public Vector2 GetUVForState ( int triangleNumber, int vertexNumber, ElementStates.EState state );
		#endregion I_RectUVProvider 

	}
}
