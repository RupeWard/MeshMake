using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class RectElement : IDebugDescribable
	{

		private RectList rectList_ = null;

		VertexElement[] vertices = new VertexElement[4]{ null, null, null, null };
		RectTriangleElement[] triangles = new RectTriangleElement[2] { null, null };

		private ElementStates.EState state_ =  ElementStates.EState.NONE;
		private UV.I_UVProvider uvProvider_;

		public void SetState(ElementStates.EState state)
		{
			if ( state_ != state )
			{
				state_ = state;
				triangles [ 0 ].SetState(state);
				triangles [ 1 ].SetState(state);
			}
		}

		public VertexElement GetClosestVertex(Vector3 position, float maxDistance)
		{
			VertexElement vle = null;
			float closestDistance = float.MaxValue;
			for ( int i = 0; i< 4; i++ )
			{
				float d = Vector3.Distance ( GetVector(i), position);
				if (d < closestDistance && d < maxDistance)
				{
					closestDistance = d;
					vle = GetVertexElement(i);
				}
			}
			return vle;
		}

		public float DistanceFromCentre(Vector3 position)
		{
			Vector3 centre = GetCentre ( );
			return ( position - centre ).magnitude;
		}

		public RectElement( RectList rectList, VertexElement v0, VertexElement v1, VertexElement v2, VertexElement v3, ElementStates.EState state, UV.I_UVProvider uvp)
		{
			rectList_ = rectList;
			uvProvider_ = uvp;

			vertices[0] = v0;
			vertices[1] = v1;
			vertices[2] = v2;
			vertices[3] = v3;
			
			triangles[0] = new RectTriangleElement( v0, v1, v3, state, uvp, 0);
			triangles[1] = new RectTriangleElement( v1, v2, v3, state, uvp, 1);
			SetState(state);

		}
		
		public Vector3 GetCentre()
		{
			Vector3 result = Vector3.zero;
			for (int i = 0; i<4; i++)
			{
				result = result + GetVector(i);
			}
			result = result /4f;
			return result;
		}

		public Vector3 GetNormal()
		{
			return Vector3.Cross(	
			                     GetVector(0) - GetVector(2),
			                     GetVector(1) - GetVector(3));
		}

		public float AngleFromNormalsRadians(Vector3 v)
		{
			Vector3 n0 = GetNormal ();
			return Mathf.Acos( Vector3.Dot (n0,v) / ( n0.magnitude * v.magnitude ) );
		}

		public static float AngleBetweenNormalsDegrees(RectElement r0, RectElement r1)
		{
			return Mathf.Rad2Deg * AngleBetweenNormalsRadians(r0, r1);
		}
		

		static public float AngleBetweenNormalsRadians(RectElement r0, RectElement r1)
		{
			return r0.AngleFromNormalsRadians(r1.GetNormal());
		}

		public bool ReplaceVertex( VertexElement oldVle, VertexElement newVle)
		{
			bool changed = false;
			if (triangles[0].ReplaceVertex(oldVle, newVle))
			{
				changed = true;
			}
			if (triangles[1].ReplaceVertex(oldVle, newVle))
			{
				changed = true;
			}
			if (changed)
			{
				oldVle.DisconnectFromRect(this);
			}
			return changed;
		}
		/*
		public int SharesEdgeOld( VertexElement v0, VertexElement v1 )
		{
			int shares = 0;
			for ( int edge = 0; edge < 4 && shares == 0; edge++)
			{
				if (vertices[ EdgeDefs.EdgeDef(edge).GetIndex(0)] == v0 &&  vertices[ EdgeDefs.EdgeDef(edge).GetIndex(1)] == v1)
				{
					shares = 1;
				}
				else if (vertices[ EdgeDefs.EdgeDef(edge).GetIndex(1)] == v0 &&  vertices[ EdgeDefs.EdgeDef(edge).GetIndex(0)] == v1)
				{
					shares = -1;
				}
			}
			return shares;
		}
		*/

		public int SharesEdge( VertexElement v0, VertexElement v1, ref Vector3 directionaway0, ref Vector3 directionaway1, ref VertexElement otherNeighbour0, ref VertexElement otherNeighbour1 )
		{
			int shareOrder = 0;
			for ( int edge = 0; edge < 4 && shareOrder == 0; edge++)
			{
				int edgeIndex0 = RectEdgeDef.EdgeDefForEdge(edge).GetIndex(0);
				int edgeIndex1 = RectEdgeDef.EdgeDefForEdge(edge).GetIndex(1);
				int indexOfNextToEdgeIndex0 = RectEdgeDef.GetIndexOfNeighbouringPointFromEdge(RectEdgeDef.EdgeDefForEdge(edge), edgeIndex0);
				int indexOfNextToEdgeIndex1 = RectEdgeDef.GetIndexOfNeighbouringPointFromEdge(RectEdgeDef.EdgeDefForEdge(edge), edgeIndex1);

				otherNeighbour0 = GetVertexElement( indexOfNextToEdgeIndex0);
				otherNeighbour1 = GetVertexElement( indexOfNextToEdgeIndex1);

				if (vertices[ edgeIndex0 ] == v0 &&  vertices[ edgeIndex1 ] == v1)
				{
					shareOrder = 1;
				}
				else if (vertices[ edgeIndex1] == v0 &&  vertices[ edgeIndex0] == v1)
				{
					shareOrder = -1;
				}
				if (shareOrder != 0)
				{
					directionaway0 = GetVector( indexOfNextToEdgeIndex0 ) - GetVector( edgeIndex0 );
					directionaway1 = GetVector( indexOfNextToEdgeIndex1 ) - GetVector( edgeIndex1 );
//					Debug.LogWarning("shares edge: "+this.DebugDescribe()+" "+index0+", "+index1+" "+shareOrder+" neighbours = "+otherNeighbour0+", "+otherNeighbour1);
				}
			}
			return shareOrder;
		}
		

		public void flipOrientation()
		{
			triangles [ 0 ].flipOrientation ( );
			triangles[1].flipOrientation();
		}

		protected RectElement(UV.I_UVProvider uvp)
		{
			uvProvider_ = uvp;
		}

		public Vector3 GetVector(int i)
		{
			return vertices[i].GetVector();
		}
		
		public VertexElement GetVertexElement(int i)
		{
			return vertices[i];
		}
		

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs, List < int > triVerts )
		{
			triangles[0].AddToMeshGenLists( gen, verts, uvs, triVerts);
			triangles[1].AddToMeshGenLists( gen, verts, uvs, triVerts);
		}

		public static bool IsSameRect(RectElement t, RectElement other)
		{
			int matches = 0;

			for (int tindex = 0; tindex < 4; tindex++)
			{
				for (int otherindex = 4; otherindex < 4; otherindex++)
				{
					if (t.GetVertexElement(tindex) == other.GetVertexElement(otherindex))
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
				sb.Append(GetVertexElement(i).DebugDescribe());
			}
		}
		#endregion IDebugDescribable


	}
}

