using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenRectList // : MeshGenList < TriangleListElement >  
	{
		private MeshGenVertexList vertexList_;
		public MeshGenVertexList vertexList
		{
			get { return vertexList_; }
		}

		private List < RectListElement > rects_ = null;

		public Vector3 GetVertex(int i)
		{
			return vertexList_.GetVectorAtIndex(i);
		}

		public VertexListElement GetVertexElement(int i)
		{
			return vertexList_.GetElement(i);
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

		public int ReplaceVertexIndex( int oldIndex, int newIndex)
		{
			int numReplaced = 0;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (RectListElement rle in rects_)
			{
				if (rle.ReplaceVertexIndex(oldIndex, newIndex))
				{
					sb.Append("Replaced ").Append (oldIndex).Append (" with ").Append(newIndex).Append (" in ").Append (rle.DebugDescribe()+"\n");
					vertexList_.DisconnectVertexFromRect(oldIndex, rle);
					numReplaced++;
				}
			}
			if (sb.Length > 0)
			{
				Debug.Log (sb.ToString());
			}
			return numReplaced;
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
			Debug.Log ( "Removing rect: " + t.DebugDescribe ( ) );
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

			public RectListElement originRle;
			public RectListElement rle;
			public int shareOrder;
			public Vector3 dirnAway0;
			public Vector3 dirnAway1;
			public int neighbourIndex0;
			public int neighbourIndex1;
		}


		public List< RectsSharingEdgeInfo > GetRectsSharingEdge( int index0, int index1, RectListElement o )
		{
			List< RectsSharingEdgeInfo > result = new List< RectsSharingEdgeInfo >  ();
			foreach (RectListElement rle in rects_)
			{
				Vector3 dirnAway0 = Vector3.zero;
				Vector3 dirnAway1 = Vector3.zero;
				int otherNeighbourIndex0 = -1;
				int otherNeighbourIndex1 = -1;
				int shareOrder = rle.SharesEdge( index0, index1, ref dirnAway0, ref dirnAway1, ref otherNeighbourIndex0, ref otherNeighbourIndex1 );
				if (shareOrder != 0)
				{
					RectsSharingEdgeInfo info = new RectsSharingEdgeInfo();
					info.index0 = index0;
					info.index1 = index1;
					info.rle = rle;
					info.shareOrder = shareOrder;
					info.dirnAway0 = dirnAway0;
					info.dirnAway1 = dirnAway1;
					info.originRle = o;
					info.neighbourIndex0 = otherNeighbourIndex0;
					info.neighbourIndex1 = otherNeighbourIndex1;
					result.Add(info);
				}
			}
			return result;
		}
	}
}

