using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenRectList // : MeshGenList < TriangleListElement >  
	{
		private MeshGenVertexList vertexList_;
		private List < RectListElement > rects_ = null;

		public int Count
		{
			get { return rects_.Count; }
		}

		public MeshGenRectList(  MeshGenVertexList vl)
		{
			vertexList_ = vl;
			rects_ = new List< RectListElement >();
		}

		public void TurnInsideOut()
		{
			foreach ( RectListElement t in rects_ )
			{
				t.flipOrientation();
			}
		}

		public int AddRect(RectListElement t)
		{
			int result = -1;
			result = rects_.Count;
			rects_.Add ( t );
			for ( int i = 0; i <4; i++)
			{
				vertexList_.ConnectVertexToRect( t.GetVertexIndex(i), t );
			}
			return result;
		}

		public void RemoveRect(RectListElement t)
		{
			for ( int i = 0; i <4; i++)
			{
				vertexList_.DisconnectVertexFromRect( t.GetVertexIndex(i), t );
			}
			rects_.Remove ( t );
		}

		public RectListElement GetRectAtIndex(int i)
		{
			if ( i < 0 || i >= rects_.Count )
			{
				Debug.LogError ("Can't get rect at index "+i+" from "+rects_.Count);
				return null;
			}
			return rects_ [ i ];
		}

		public Vector3 GetCentre(RectListElement t)
		{
			Vector3 result = Vector3.zero;
			for (int i = 0; i<4; i++)
			{
				result = result + vertexList_.GetVectorAtIndex( t.GetVertexIndex(i) );
			}
			result = result /4f;
			return result;
		}

		public List< RectListElement > GetRectsSharingEdge( int index0, int index1 )
		{
			List< RectListElement > result = new List< RectListElement > ();
			foreach (RectListElement rle in rects_)
			{
				if (rle.SharesEdge( index0, index1 ))
				{
					result.Add(rle);
				}
			}
			return result;
		}
	}
}

