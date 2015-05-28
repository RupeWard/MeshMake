using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class VertexElement : IDebugDescribable
	{
		Vector3 vector_ = Vector3.zero;

		public HashSet< TriangleElement > triangles = new HashSet<TriangleElement> ( );
		public HashSet< RectElement > rects = new HashSet<RectElement> ( );

		public float Distance(Vector3 other)
		{
			return Vector3.Distance ( vector_, other );
		}

		public void ConnectToRect(RectElement e)
		{
			rects.Add ( e );
		}

		public int DisconnectFromRect( RectElement t)
		{
			rects.Remove (t);
			if ( NumConnections <= 0 )
			{
				Debug.Log("Vertex now has no connections "+this.DebugDescribe());
				//				vertices_.RemoveAt(i);
			}
			return NumConnections;
		}

		public void ConnectToTriangle( TriangleElement t)
		{
			triangles.Add (t);
		}
		
		public void DisconnectFromTriangle( TriangleElement t)
		{
			triangles.Remove (t);
			if ( NumConnections <= 0 )
			{
				Debug.LogWarning("Vertex now has no connections "+this.DebugDescribe());
				//				vertices_.RemoveAt(i);
			}
		}



		public int NumConnections
		{
			get { return triangles.Count + rects.Count; }
		}

		public VertexElement( Vector3 v)
		{
			vector_= v;
		}

		protected VertexElement(){}

		public Vector3 GetVector()
		{
			return vector_;
		}

		public void SetVector(Vector3 v)
		{
			vector_.Set ( v.x, v.y, v.z);
		}

		public void AddVector(ref List< float > list)
		{
			list.Add ( vector_.x );
			list.Add (vector_.y);
			list.Add (vector_.z);
		}

		public static bool Equals(VertexElement t, VertexElement other)
		{
			return (Vector3.Distance(t.GetVector(), other.GetVector()) < MeshGenerator.POSITION_TELRANCE);
		}

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ( "[Vertex " ).Append ( vector_ )
				.Append (triangles.Count).Append ("/").Append (rects.Count)
				.Append ( "]" );
		}
	}
}

