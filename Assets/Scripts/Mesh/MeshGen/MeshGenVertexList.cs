using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenVertexList // : MeshGenList < VertexListElement >  
	{
		private List< VertexListElement > vertices_ = new List< VertexListElement >();

		public int Count
		{
			get { return vertices_.Count; }
		}

		public void ConnectVertexToRect( int i, RectListElement t)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't connect vertex of index "+i+" from "+vertices_.Count);
				return;
			}
			vertices_[i].rects.Add (t);
		}

		public void DisconnectVertexFromRect( int i, RectListElement t)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't disconnect vertex of index "+i+" from "+vertices_.Count);
				return;
			}
			vertices_[i].rects.Remove (t);
			if ( vertices_[i].NumConnections <= 0 )
			{
				Debug.Log("Vertex now has no connections "+vertices_[i].DebugDescribe());
//				vertices_.RemoveAt(i);
			}
		}
		
		public void ConnectVertexToTriangle( int i, TriangleListElement t)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't connect vertex of index "+i+" from "+vertices_.Count);
				return;
			}
			vertices_[i].triangles.Add (t);
		}

		public void DisconnectVertexFromTriangle( int i, TriangleListElement t)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't disconnect vertex of index "+i+" from "+vertices_.Count);
				return;
			}
			vertices_[i].triangles.Remove (t);
			if ( vertices_[i].NumConnections <= 0 )
			{
				Debug.LogWarning("Vertex now has no connections "+vertices_[i].DebugDescribe());
//				vertices_.RemoveAt(i);
			}
		}
		
		public VertexListElement GetElement(int i)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't get element of index "+i+" from "+vertices_.Count);
				return null;
			}
			return vertices_[i];
		}

		public Vector3 GetVectorAtIndex(int i)
		{
			if ( i < 0 || i >= vertices_.Count )
			{
				Debug.LogError ("Can't get vertex of index "+i+" from "+vertices_.Count);
				return Vector3.zero;
			}
			return vertices_[i].GetVector();
		}

		public int AddVertex( VertexListElement v )
		{
			int i = IndexOf (v.GetVector());
			if (i != -1) 
			{
				Debug.LogWarning("Point "+v+" already in list");
			} 
			else 
			{
				i = vertices_.Count;
				vertices_.Add( v );
			}
			return i;
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
				vertices_.Add( new VertexListElement(v));
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
					if (vertices_[i].GetVector().EqualsApprox(v, MeshGenerator.POSITION_TELRANCE))
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

