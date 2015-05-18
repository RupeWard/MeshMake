using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class VertexListElement
	{
		Vector3 vector_ = Vector3.zero;

		public HashSet< TriangleListElement > triangles = new HashSet<TriangleListElement> ( );
		public HashSet< RectListElement > rects = new HashSet<RectListElement> ( );

		public VertexListElement( Vector3 v)
		{
			vector_= v;
		}

		protected VertexListElement(){}

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

		public static bool Equals(VertexListElement t, VertexListElement other)
		{
			return (Vector3.Distance(t.GetVector(), other.GetVector()) < MeshGenerator.POSITION_TELRANCE);
		}
	}
}

