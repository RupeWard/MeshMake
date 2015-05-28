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

		private List < RectElement > rects_ = null;

		public Vector3 GetVector(int i)
		{
			return vertexList_.GetVectorAtIndex(i);
		}

		public VertexElement GetVertexElement(int i)
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
			rects_ = new List< RectElement >();
		}

		public void TurnInsideOut()
		{
			foreach ( RectElement t in rects_ )
			{
				t.flipOrientation();
			}
		}
		/*
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
		*/

		public int ReplaceVertex( VertexElement vle0, VertexElement vle1)
		{
			int numReplaced = 0;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (RectElement rle in rects_)
			{
				if (rle.ReplaceVertex(vle0, vle1))
				{
					sb.Append("Replaced ").Append (vle0.DebugDescribe()).Append (" with ").Append(vle1.DebugDescribe()).Append (" in ").Append (rle.DebugDescribe()+"\n");
					vle0.DisconnectFromRect(rle);
					numReplaced++;
				}
			}
			if (sb.Length > 0)
			{
				Debug.Log (sb.ToString());
			}
			return numReplaced;
		}

		public int AddRect(RectElement t)
		{
			int result = -1;
			result = rects_.Count;
			rects_.Add ( t );
			for ( int i = 0; i <4; i++)
			{
				t.GetVertexElement(i).ConnectToRect( t );
			}
			return result;
		}

		public RectElement GetClosestRect(Vector3 position)
		{
			RectElement result = null;
			float closestDistance = float.MaxValue;
			for ( int i = 0; i< Count; i++ )
			{
				float d = GetRectAtIndex(i).DistanceFromCentre(position);
				if (d < closestDistance)
				{
					closestDistance = d;
					result = GetRectAtIndex(i);
				}
			}
			return result;
		}
		

		public void RemoveRectWithVertexReplace( RectElement toReplace, RectElement match)
		{
			for (int i = 0; i<4; i++)
			{
				VertexElement vleToReplace = toReplace.GetVertexElement(i);
				VertexElement newVle = match.GetClosestVertex( toReplace.GetVertexElement(i).GetVector(), MeshGenerator.POSITION_TELRANCE * 2f );
				if (newVle != null)
				{
					ReplaceVertex( vleToReplace, newVle);
				}
				else
				{
					Debug.LogError ("newVle = null");
			    }
			}
		}

		public void RemoveRect(RectElement t)
		{
			Debug.Log ( "Removing rect: " + t.DebugDescribe ( ) );
			for ( int i = 0; i <4; i++)
			{
				t.GetVertexElement(i).DisconnectFromRect(t );
			}
			rects_.Remove ( t );
		}

		public RectElement GetRectAtIndex(int i)
		{
			if ( i < 0 || i >= rects_.Count )
			{
				Debug.LogError ("Can't get rect at index "+i+" from "+rects_.Count);
				return null;
			}
			return rects_ [ i ];
		}

		public class RectsSharingEdgeInfo
		{
			public VertexElement vle0;
			public VertexElement vle1;

			public RectElement originRle;
			public RectElement rle;
			public int shareOrder;
			public Vector3 dirnAway0;
			public Vector3 dirnAway1;

			public VertexElement neighbourVle0;
			public VertexElement neighbourVle1;
		}


		public List< RectsSharingEdgeInfo > GetRectsSharingEdge( VertexElement v0, VertexElement v1, RectElement o )
		{
			List< RectsSharingEdgeInfo > result = new List< RectsSharingEdgeInfo >  ();
			foreach (RectElement rle in rects_)
			{
				Vector3 dirnAway0 = Vector3.zero;
				Vector3 dirnAway1 = Vector3.zero;
				VertexElement otherNeighbourVle0 = null;
				VertexElement otherNeighbourVle1 = null;
				int shareOrder = rle.SharesEdge( v0, v1, ref dirnAway0, ref dirnAway1, ref otherNeighbourVle0, ref otherNeighbourVle1 );
				if (shareOrder != 0)
				{
					RectsSharingEdgeInfo info = new RectsSharingEdgeInfo();
					info.vle0 = v0;
					info.vle1 = v1;
					info.rle = rle;
					info.shareOrder = shareOrder;
					info.dirnAway0 = dirnAway0;
					info.dirnAway1 = dirnAway1;
					info.originRle = o;
					info.neighbourVle0 = otherNeighbourVle0;
					info.neighbourVle1 = otherNeighbourVle1;
					result.Add(info);
				}
			}
			return result;
		}
	}
}

