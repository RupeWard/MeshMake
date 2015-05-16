using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class TriangleListElement : IDebugDescribable
	{
		int[] vertexIndices_ = new int[3]{ -1, -1, -1};

		public TriangleListElement( int v0, int v1, int v2)
		{
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

		public void AddVertIndicesToList(ref List<int> list)
		{
			list.Add ( vertexIndices_[0]);
			list.Add ( vertexIndices_[1]);
			list.Add ( vertexIndices_[2]);
		}

		public static bool Equals(TriangleListElement t, TriangleListElement other)
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
				sb.Append(GetVertexIndex(i)).Append(",");
			}
		}
		#endregion IDebugDescribable


	}
}

