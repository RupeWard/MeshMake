using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenVertexList // : MeshGenList < VertexListElement >  
	{
		private List< Vector3 > vertices_ = new List< Vector3 >();

		public int Count
		{
			get { return vertices_.Count; }
		}

		public Vector3 GetVectorAtIndex(int i)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't get vertex of index "+i+" from "+vertices_.Count);
				return Vector3.zero;
			}
			return vertices_ [ i ];
		}

		public int AddVertex( Vector3 v )
		{
			int i = IndexOf (v);
			if (i != -1) 
			{
				Debug.LogWarning("Point "+v+" already in list");
			} 
			else 
			{
				i = vertices_.Count;
				vertices_.Add(v);
			}
			return i;
		}

		public int IndexOf( Vector3 v)
		{
			int result = -1;
			if ( vertices_.Count > 0)
			{
				for (int i = 0; i < vertices_.Count; i++)
				{
					if (vertices_[i].EqualsApprox(v, MeshGenerator.POSITION_TELRANCE))
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public bool Contains( Vector3 v)
		{
			return (IndexOf (v) != -1);
		}
	}
}

