using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class RectListElement : IDebugDescribable
	{
		static public readonly int[][] edgeIndices = new int[][] 
		{
			new int[]{ 0, 1 },
			new int[]{ 1, 2 },
			new int[]{ 2, 3 },
			new int[]{ 3, 0 }
		};
		
		int[] vertexIndices_ = new int[4]{ -1, -1, -1, -1};
		TriangleListElement[] triangles = new TriangleListElement[2] { null, null };

		public RectListElement( int v0, int v1, int v2, int v3)
		{
			vertexIndices_[0] = v0;
			vertexIndices_[1] = v1;
			vertexIndices_[2] = v2;
			vertexIndices_[3] = v3;

			triangles[0] = new TriangleListElement( v0, v1, v3);
			triangles[1] = new TriangleListElement( v1, v2, v3);

		}

		public bool SharesEdge( int index0, int index1 )
		{
			bool shares = false;
			for ( int edge = 0; edge < 4; edge++)
			{
				if ((vertexIndices_[ edgeIndices[edge][0]] == index0 &&  vertexIndices_[ edgeIndices[edge][1]] == index1)
				    || (vertexIndices_[ edgeIndices[edge][1]] == index0 &&  vertexIndices_[ edgeIndices[edge][0]] == index1))
				{
					shares = true;
					break;
				}
			}
			return shares;
		}

		public void flipOrientation()
		{
			triangles [ 0 ].flipOrientation ( );
			triangles[1].flipOrientation();
		}

		protected RectListElement(){}

		public int GetVertexIndex(int i)
		{
			return vertexIndices_[i];
		}

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < int > triVerts )
		{
			triangles[0].AddToMeshGenLists( gen, verts, triVerts );
			triangles[1].AddToMeshGenLists( gen, verts, triVerts );
		}

		public static bool IsSameRect(RectListElement t, RectListElement other)
		{
			int matches = 0;

			for (int tindex = 0; tindex < 4; tindex++)
			{
				for (int otherindex = 4; otherindex < 4; otherindex++)
				{
					if (t.GetVertexIndex(tindex) == other.GetVertexIndex(otherindex))
					{
						matches++;
						break;
					}
				}
			}
			return ( matches == 4 );
		}

		#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("Rect: ");
			for ( int i =0; i<4; i++ )
			{
				if (i >0 ) sb.Append(", ");
				sb.Append(GetVertexIndex(i));
			}
		}
		#endregion IDebugDescribable


	}
}

