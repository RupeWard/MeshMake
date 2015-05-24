using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class RectListElement : IDebugDescribable
	{
		private MeshGenRectList rectList_ = null;
		private GridUVProviders uvProviders_ = null;
		private GridUVProviders.GridPosition gridPosition;

		public void SetGridPosition(GridUVProviders.GridPosition gp)
		{
			gridPosition = gp;

			triangles [ 0 ].SetGridPosition ( gp );
			triangles [ 1 ].SetGridPosition ( gp );
		}

		public class EdgeDef :IDebugDescribable
		{
			private readonly int [] indices = new int[2];

			public int GetIndex(int i)
			{
				return indices [ i ];
			}

			public EdgeDef(int i0, int i1)
			{
				indices[0]=i0;
				indices[1]=i1;
			}

			private EdgeDef(){}

			public bool SameEdge (EdgeDef ed)
			{
				bool result = false;
				if (ed.indices[0] == this.indices[0] && ed.indices[1] == this.indices[1])
				{
					result = true;
				}
				return result;
			}

			public void DebugDescribe(System.Text.StringBuilder sb)
			{
				sb.Append ( "(" ).Append ( indices [ 0 ] ).Append ( "," ).Append ( indices [ 1 ] ).Append ( ")" );
			}

			static System.Text.StringBuilder s_sb = new System.Text.StringBuilder ( );
			public override string ToString ()
			{
				s_sb.Length = 0;
				DebugDescribe ( s_sb );
				return s_sb.ToString();
			}
		}

		public class EdgeDefs
		{
			static private readonly EdgeDef[] edgeDefs = new EdgeDef[] 
			{
				new EdgeDef( 0, 1 ),
				new EdgeDef( 1, 2 ),
				new EdgeDef( 2, 3 ),
				new EdgeDef( 3, 0 )
			};

			static public EdgeDef EdgeDef(int i)
			{
				return edgeDefs [ i ];
			}

			static public int GetIndexOfNeighbouringPointFromEdge(EdgeDef edgeDef, int index)
			{
				int result = -1;
				foreach ( EdgeDef ed in edgeDefs )
				{
					if (!ed.SameEdge(edgeDef))
					{
						if (ed.GetIndex(0) == index)
						{
							if (result != -1)
							{
								Debug.LogError("Found secoind neighbour of "+index+" not in "+edgeDef+" which is "+ed.GetIndex(1));
							}
							result = ed.GetIndex(1);
						}
						else if (ed.GetIndex(1) == index)
						{
							if (result != -1)
							{
								Debug.LogError("Found secoind neighbour of "+index+" not in "+edgeDef+" which is "+ed.GetIndex(0));
							}
							result = ed.GetIndex(0);
						}
					}
				}
				if ( result == -1 )
				{
					Debug.LogError ( "Couldn't fidn nehbouring index" );
				}
				return result;
			}
		}

		int[] vertexIndices_ = new int[4]{ -1, -1, -1, -1};
		TriangleListElement[] triangles = new TriangleListElement[2] { null, null };


		public RectListElement( MeshGenRectList rectList, int v0, int v1, int v2, int v3)
		{
			rectList_ = rectList;

			vertexIndices_[0] = v0;
			vertexIndices_[1] = v1;
			vertexIndices_[2] = v2;
			vertexIndices_[3] = v3;

			triangles[0] = new TriangleListElement( v0, v1, v3);
			triangles[1] = new TriangleListElement( v1, v2, v3);

		}

		public RectListElement( MeshGenRectList rectList, int v0, int v1, int v2, int v3, GridUVProviders gup, GridUVProviders.GridPosition gp)
		{
			uvProviders_ = gup;

			rectList_ = rectList;
			
			vertexIndices_[0] = v0;
			vertexIndices_[1] = v1;
			vertexIndices_[2] = v2;
			vertexIndices_[3] = v3;
			
			triangles[0] = new TriangleListElement( v0, v1, v3, uvProviders_.GetTriangleProviderForRect(gp, 0));
			triangles[1] = new TriangleListElement( v1, v2, v3, uvProviders_.GetTriangleProviderForRect(gp, 1));
			SetGridPosition(gp);

		}

		public void SetUVProvider(GridUVProviders g)
		{
			uvProviders_ = g;
		}

		public Vector3 GetCentre()
		{
			return rectList_.GetCentre ( this );
		}

		public Vector3 GetNormal()
		{
			return rectList_.GetNormal ( this );
		}

		public float AngleFromNormalsRadians(Vector3 v)
		{
			Vector3 n0 = GetNormal ();
			return Mathf.Acos( Vector3.Dot (n0,v) / ( n0.magnitude * v.magnitude ) );
		}

		public static float AngleBetweenNormalsDegrees(RectListElement r0, RectListElement r1)
		{
			return Mathf.Rad2Deg * AngleBetweenNormalsRadians(r0, r1);
		}
		

		static public float AngleBetweenNormalsRadians(RectListElement r0, RectListElement r1)
		{
			return r0.AngleFromNormalsRadians(r1.GetNormal());
		}

		public int SharesEdgeOld( int index0, int index1 )
		{
			int shares = 0;
			for ( int edge = 0; edge < 4 && shares == 0; edge++)
			{
				if (vertexIndices_[ EdgeDefs.EdgeDef(edge).GetIndex(0)] == index0 &&  vertexIndices_[ EdgeDefs.EdgeDef(edge).GetIndex(1)] == index1)
				{
					shares = 1;
				}
				else if (vertexIndices_[ EdgeDefs.EdgeDef(edge).GetIndex(1)] == index0 &&  vertexIndices_[ EdgeDefs.EdgeDef(edge).GetIndex(0)] == index1)
				{
					shares = -1;
				}
			}
			return shares;
		}

		public int SharesEdge( int index0, int index1, ref Vector3 directionaway0, ref Vector3 directionaway1 )
		{
			int shares = 0;
			for ( int edge = 0; edge < 4 && shares == 0; edge++)
			{
				int edgeIndex0 = EdgeDefs.EdgeDef(edge).GetIndex(0);
				int edgeIndex1 = EdgeDefs.EdgeDef(edge).GetIndex(1);
				int indexOfNextToEdgeIndex0 = EdgeDefs.GetIndexOfNeighbouringPointFromEdge(EdgeDefs.EdgeDef(edge), edgeIndex0);
				int indexOfNextToEdgeIndex1 = EdgeDefs.GetIndexOfNeighbouringPointFromEdge(EdgeDefs.EdgeDef(edge), edgeIndex1);

				if (vertexIndices_[ edgeIndex0 ] == index0 &&  vertexIndices_[ edgeIndex1 ] == index1)
				{
					shares = 1;
				}
				else if (vertexIndices_[ edgeIndex1] == index0 &&  vertexIndices_[ edgeIndex0] == index1)
				{
					shares = -1;
				}
				if (shares != 0)
				{
					directionaway0 = GetVertex( indexOfNextToEdgeIndex0 ) - GetVertex( edgeIndex0 );
					directionaway1 = GetVertex( indexOfNextToEdgeIndex1 ) - GetVertex( edgeIndex1 );
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

		public Vector3 GetVertex(int i)
		{
			return rectList_.GetVertex ( vertexIndices_[i] );
		}

		public void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs, List < int > triVerts )
		{
			triangles[0].AddToMeshGenLists( gen, verts, uvs, triVerts );
			triangles[1].AddToMeshGenLists( gen, verts, uvs, triVerts );
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

