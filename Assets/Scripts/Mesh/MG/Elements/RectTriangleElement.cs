using System;
using System.Collections.Generic;
using UnityEngine;

namespace MG
{
	public class RectTriangleElement :TriangleElement
	{
		private int triangleNumber_ = -1;

		public RectTriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, ElementStates.EState state, UV.I_RectUVProvider iup, int trNum)
				: base ( v0, v1, v2, state, iup)
		{
			triangleNumber_ = trNum;
		}

		public void ChangeTriangleNumber()
		{
			triangleNumber_ = 1 - triangleNumber_;
#if UNITY_EDITOR
			d_uvsDirtyReason+="TrNum.";
#endif
			uvsDirty_ = true;

		}

		public void AddToMeshGenLists( List < Vector3 > verts, List < Vector2 > uvs,  List < int > triVerts)
		{
			#if UNITY_EDITOR
			if (uvsDirty_)
			{
//				Debug.Log("UVsDirtyReason = "+d_uvsDirtyReason);
				d_uvsDirtyReason = string.Empty;
			}
			#endif

			int firstIndex = verts.Count;
			for (int v=0; v<3; v++)
			{
				verts.Add ( vertexElements_[v].GetVector() );
				triVerts.Add ( firstIndex + v);
				if (uvProvider_ != null)
				{
					if (uvsDirty_)
					{
						uvs_[v] = uvProvider_.GetUVForState(triangleNumber_, v, state_);
					}
					uvs.Add( uvs_[v] );
				}
			}
			uvsDirty_ = false;
		}

	}
}

