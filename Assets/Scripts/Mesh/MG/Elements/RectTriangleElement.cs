using System;
using System.Collections.Generic;
using UnityEngine;

namespace MG
{
	public class RectTriangleElement :TriangleElement
	{
		private readonly int triangleNumber_ = -1;

		public RectTriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, ElementStates.EState state, int trNum)
				: base( v0, v1, v2, state)
		{
			triangleNumber_ = trNum;
		}
		
		public RectTriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, ElementStates.EState state, UV.I_UVProvider iup, int trNum)
				: base ( v0, v1, v2, state, iup)
		{
			triangleNumber_ = trNum;
		}

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs,  List < int > triVerts)
		{
			int firstIndex = verts.Count;
			for (int v=0; v<3; v++)
			{
				verts.Add ( vertexElements_[v].GetVector() );
				triVerts.Add ( firstIndex + v);
				if (uvProvider_ != null)
				{
					uvs.Add( uvProvider_.GetUVForState(triangleNumber_, v, state_) );
				}
			}
		}

	}
}

