using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class TriangleListElement : IDebugDescribable
	{
		I_UVProvider uvProvider = null;

		int[] vertexIndices_ = new int[3]{ -1, -1, -1};

		public TriangleListElement( int v0, int v1, int v2)
		{
			vertexIndices_[0] = v0;
			vertexIndices_[1] = v1;
			vertexIndices_[2] = v2;
		}

		public TriangleListElement( int v0, int v1, int v2, I_UVProvider iup)
		{
			uvProvider = iup;
			vertexIndices_[0] = v0;
			vertexIndices_[1] = v1;
			vertexIndices_[2] = v2;
		}
		
		public void flipOrientation()
		{
			int tmp = vertexIndices_ [ 0 ];
			vertexIndices_[0] = vertexIndices_[1];
			vertexIndices_[1] = tmp;
		}

		protected TriangleListElement(){}

		public int GetVertexIndex(int i)
		{
			return vertexIndices_[i];
		}

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs,  List < int > triVerts )
		{
			int firstIndex = verts.Count;
			for (int v=0; v<3; v++)
			{
				verts.Add ( gen.VertexList.GetVectorAtIndex( GetVertexIndex(v) ) );
				triVerts.Add ( firstIndex + v);
				if (uvProvider != null)
				{
					uvs.Add( uvProvider.GetUVForTriangleIndex(v) );
				}
			}
		}

		public static bool HasSameIndices(TriangleListElement t, TriangleListElement other)
		{
			int matches = 0;

			for (int tindex = 0; tindex < 3; tindex++)
			{
				for (int otherindex = 0; otherindex < 3; otherindex++)
				{
					if (t.GetVertexIndex(tindex) == other.GetVertexIndex(otherindex))
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
				sb.Append(GetVertexIndex(i));
			}
		}
		#endregion IDebugDescribable


	}
}

