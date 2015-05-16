using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenTriangleList // : MeshGenList < TriangleListElement >  
	{
		private MeshGenVertexList vertexList_;
		private List < TriangleListElement > triangles_ = null;

		public int Count
		{
			get { return triangles_.Count; }
		}

		public MeshGenTriangleList(  MeshGenVertexList vl)
		{
			vertexList_ = vl;
			triangles_ = new List< TriangleListElement >();
		}

		public void TurnInsideOut()
		{
			foreach ( TriangleListElement t in triangles_ )
			{
				t.flipOrientation();
			}
		}

		public int AddTriangle(TriangleListElement t)
		{
			int result = -1;
			result = triangles_.Count;
			triangles_.Add ( t );
			for ( int i = 0; i <3; i++)
			{
				vertexList_.ConnectVertexToTriangle( t.GetVertexIndex(i), t );
			}
			return result;
		}

		public void RemoveTriangle(TriangleListElement t)
		{
			for ( int i = 0; i <3; i++)
			{
				vertexList_.DisconnectVertexFromTriangle( t.GetVertexIndex(i), t );
			}
			triangles_.Remove ( t );
		}

		public TriangleListElement GetTriAtIndex(int i)
		{
			if ( i < 0 || i >= triangles_.Count )
			{
				Debug.LogError ("Can't get triangle at index "+i+" from "+triangles_.Count);
				return null;
			}
			return triangles_ [ i ];
		}

		public Vector3 GetCentre(TriangleListElement t)
		{
			Vector3 result = Vector3.zero;
			for (int i = 0; i<3; i++)
			{
				result = result + vertexList_.GetVectorAtIndex( t.GetVertexIndex(i) );
			}
			result = result /3f;
			return result;
		}

	}
}

