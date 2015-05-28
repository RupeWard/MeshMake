using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenVertexList // : MeshGenList < VertexListElement >  
	{
		private List< VertexElement > vertices_ = new List< VertexElement >();

		public VertexElement GetClosestElement(Vector3 pos, float max, out float closestDistance)
		{
			VertexElement result = null;
			closestDistance = float.MaxValue;
			foreach ( VertexElement v in vertices_ )
			{
				float d = v.Distance(pos);
				if (d < max && d < closestDistance)
				{
					closestDistance = d;
					result = v;
				}
			}
			return result;
		}

		public VertexElement GetClosestElement(Vector3 pos, float max)
		{
			float closestDistance = float.MaxValue;
			VertexElement result = null;
			foreach ( VertexElement v in vertices_ )
			{
				float d = v.Distance(pos);
				if (d < max && d < closestDistance)
				{
					closestDistance = d;
					result = v;
				}
			}
			return result;
		}

		public VertexElement FindElement(Vector3 pos)
		{
			return GetClosestElement ( pos, _MeshGen.MeshGenerator.POSITION_TELRANCE );
		}

		public VertexElement AddVertexElement(Vector3 pos)
		{
			VertexElement result = FindElement ( pos );
			if (result == null)
			{
				result = new VertexElement(pos);
				vertices_.Add(result);
			}
			else
			{
				Debug.LogWarning("Alrready have an element at "+pos);
			}
			return result;
		}
		/*
		public int GetIndexOfClosestElement(Vector3 pos, float max, out float closestDistance)
		{
			int result = -1;
			closestDistance = float.MaxValue;

			for ( int i = 0; i < vertices_.Count; i++ )
			{
				float d = vertices_[i].Distance(pos);
				if (d < max && d < closestDistance)
				{
					closestDistance = d;
					result = i;
				}
			}
			return result;
		}*/

		public int Count
		{
			get { return vertices_.Count; }
		}



		/*
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
		*/

		/*
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
		*/
		
		public VertexElement GetElement(int i)
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

		/*
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

		public VertexListElement AddVertexElement( Vector3 v )
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
		


		public bool Contains( Vector3 v)
		{
			return (IndexOf (v) != -1);
		}*/
	}
}

