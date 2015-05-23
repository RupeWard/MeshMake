using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenRectList // : MeshGenList < TriangleListElement >  
	{
		private MeshGenVertexList vertexList_;
		private List < RectListElement > rects_ = null;

		public Vector3 GetVertex(int i)
		{
			return vertexList_.GetVectorAtIndex(i);
		}

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
				result = result + t.GetVertex(i);
			}
			result = result /4f;
			return result;
		}

		public Vector3 GetNormal(RectListElement t)
		{
			return Vector3.Cross(	
					t.GetVertex(0) - t.GetVertex(2),
					t.GetVertex(1) - t.GetVertex(3));
		}

		public class RectsSharingEdgeInfo
		{
			public int index0;
			public int index1;

			public RectListElement rle;
			public int shares;
			public Vector3 dirnAway0;
			public Vector3 dirnAway1;
		}

		public List< RectsSharingEdgeInfo > GetRectsSharingEdge( int index0, int index1 )
		{
			List< RectsSharingEdgeInfo > result = new List< RectsSharingEdgeInfo >  ();
			foreach (RectListElement rle in rects_)
			{
				Vector3 dirnAway0 = Vector3.zero;
				Vector3 dirnAway1 = Vector3.zero;

				int shares = rle.SharesEdge( index0, index1, ref dirnAway0, ref dirnAway1 );
				if (shares != 0)
				{
					RectsSharingEdgeInfo info = new RectsSharingEdgeInfo();
					info.index0 = index0;
					info.index1 = index1;
					info.rle = rle;
					info.shares = shares;
					info.dirnAway0 = dirnAway0;
					info.dirnAway1 = dirnAway1;
					result.Add(info);
				}
			}
			return result;
		}
	}
}

