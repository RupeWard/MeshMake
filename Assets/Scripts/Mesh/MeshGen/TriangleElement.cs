using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class TriangleElement : IDebugDescribable
	{
		GridUVProvider uvProvider = null;

		VertexElement[] vertices_ = new VertexElement[3]{ null, null, null };

		public TriangleElement( VertexElement v0, VertexElement v1, VertexElement v2)
		{
			vertices_[0] = v0;
			vertices_[1] = v1;
			vertices_[2] = v2;
		}

		public TriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, GridUVProvider iup)
		{
			uvProvider = iup;
			vertices_[0] = v0;
			vertices_[1] = v1;
			vertices_[2] = v2;
		}

		public void SetGridPosition(GridUVProviders.GridPosition pos)
		{
			if ( uvProvider != null )
			{
				uvProvider.SetGridPosition(pos);
			}
		}

		public void flipOrientation()
		{
			VertexElement tmp = vertices_ [ 0 ];
			vertices_[0] = vertices_[1];
			vertices_[1] = tmp;
		}

		protected TriangleElement(){}

		public VertexElement GetVertex(int i)
		{
			return vertices_[i];
		}
		/*
		public bool ReplaceVertexIndex( int oldIndex, int newIndex)
		{
			for (int i = 0; i< 3; i++)
			{
				if (vertexIndices_[i] == oldIndex)
				{
					vertexIndices_[i] = newIndex;
					return true;
				}
			}
			return false;
		}
		*/

		public bool ReplaceVertex( VertexElement oldVle, VertexElement newVle)
		{
			for (int i = 0; i< 3; i++)
			{
				if (vertices_[i] == oldVle)
				{
					vertices_[i] = newVle;
					return true;
				}
			}
			return false;
		}

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs,  List < int > triVerts )
		{
			int firstIndex = verts.Count;
			for (int v=0; v<3; v++)
			{
				verts.Add ( vertices_[v].GetVector() );
				triVerts.Add ( firstIndex + v);
				if (uvProvider != null)
				{
					uvs.Add( uvProvider.GetUVForTriangleIndex(v) );
				}
			}
		}

		public static bool HasSameVertices(TriangleElement t, TriangleElement other)
		{
			int matches = 0;

			for (int tindex = 0; tindex < 3; tindex++)
			{
				for (int otherindex = 0; otherindex < 3; otherindex++)
				{
					if (t.GetVertex(tindex) == other.GetVertex(otherindex))
					{
						matches++;
						break;
					}
				}
			}
			return ( matches == 3 );
		}

		#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("Tr: ");
			for ( int i =0; i<3; i++ )
			{
				if (i >0 ) sb.Append(", ");
				sb.Append(GetVertex(i));
			}
		}
		#endregion IDebugDescribable


	}
}

