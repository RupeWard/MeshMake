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

		public MeshGenTriangleList( MeshGenVertexList vl)
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
			return result;
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
	}
}

