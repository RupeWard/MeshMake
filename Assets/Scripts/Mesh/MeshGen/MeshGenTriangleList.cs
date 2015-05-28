using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenTriangleList // : MeshGenList < TriangleListElement >  
	{
		private MeshGenVertexList vertexList_;
		private List < TriangleElement > triangles_ = null;

		public int Count
		{
			get { return triangles_.Count; }
		}

		public MeshGenTriangleList(  MeshGenVertexList vl)
		{
			vertexList_ = vl;
			triangles_ = new List< TriangleElement >();
		}

		public void TurnInsideOut()
		{
			foreach ( TriangleElement t in triangles_ )
			{
				t.flipOrientation();
			}
		}

		public int AddTriangle(TriangleElement t)
		{
			int result = -1;
			result = triangles_.Count;
			triangles_.Add ( t );
			for ( int i = 0; i <3; i++)
			{
				t.GetVertex(i).ConnectToTriangle( t );
			}
			return result;
		}

		public void RemoveTriangle(TriangleElement t)
		{
			for ( int i = 0; i <3; i++)
			{
				t.GetVertex(i).DisconnectFromTriangle( t );
			}
			triangles_.Remove ( t );
		}

		public TriangleElement GetTriAtIndex(int i)
		{
			if ( i < 0 || i >= triangles_.Count )
			{
				Debug.LogError ("Can't get triangle at index "+i+" from "+triangles_.Count);
				return null;
			}
			return triangles_ [ i ];
		}

		public Vector3 GetCentre(TriangleElement t)
		{
			Vector3 result = Vector3.zero;
			for (int i = 0; i<3; i++)
			{
				result = result + t.GetVertex(i).GetVector();
			}
			result = result /3f;
			return result;
		}

	}
}

