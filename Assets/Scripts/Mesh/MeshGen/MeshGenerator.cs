using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenerator : MonoBehaviour, IDebugDescribable
	{
		public static bool allowMultiMovement = false;

		public static readonly float POSITION_TELRANCE = 0.001f;
		public static bool DEBUG_MESHMAKE = false;

		protected MeshGenVertexList vertexList_ = null;
		public MeshGenVertexList VertexList
		{
			get { return vertexList_; }
		}
		protected MeshGenTriangleList triangleList_ = null;
		protected MeshGenRectList rectList_ = null;

		private MeshFilter meshFilter_;
		private MeshRenderer meshRenderer_;
		private MeshCollider meshCollider_;

		private List < VertexMover > vertexMovers_ = new List< VertexMover > ( );

		protected float size_ = 1f;
		public float Size
		{
			get { return size_; }
		}

		public void SetMaterial(Material m)
		{
			meshRenderer_.material = m;
		}

		private bool isDirty_ = false;
		protected void SetDirty()
		{
			isDirty_ = true;
		}

		void Awake()
		{
			vertexList_ = new MeshGenVertexList ( );
			triangleList_ = new MeshGenTriangleList ( vertexList_ );
			rectList_ = new MeshGenRectList ( vertexList_ );

			meshFilter_ = gameObject.GetComponent< MeshFilter > ( );
			if ( meshFilter_ == null )
			{
				meshFilter_ = gameObject.AddComponent< MeshFilter >();
			}

			meshRenderer_ = gameObject.GetComponent< MeshRenderer > ( );
			if ( meshRenderer_ == null )
			{
				meshRenderer_ = gameObject.AddComponent< MeshRenderer >();
			}

			meshCollider_ = gameObject.GetComponent< MeshCollider > ( );
			if ( meshCollider_ == null )
			{
				meshCollider_ = gameObject.AddComponent< MeshCollider >();
			}
			meshCollider_.sharedMesh = meshFilter_.sharedMesh;
		}

		void Start()
		{
			isDirty_ = false;
		}

		public void MakeMesh()
		{
			if ( DEBUG_MESHMAKE )
			{
				Debug.Log("Making mesh");
			}

			if (vertexList_.Count == 0 || (triangleList_.Count == 0 && rectList_.Count == 0))
			{
				Debug.LogError( "Can't make mesh with "+vertexList_.Count+" verts, "+rectList_.Count+"rects and "+triangleList_.Count+" tris");
				return;
			}

			Mesh mesh = meshFilter_.sharedMesh;
			if ( mesh == null )
			{
				meshFilter_.sharedMesh = new Mesh();
				mesh = meshFilter_.sharedMesh;
			}
			mesh.Clear ( );

			List < Vector3 > verts = new List< Vector3 >();
			List< int > triVerts = new List< int >();

			if (triangleList_ != null)
			{
				if ( DEBUG_MESHMAKE )
				{
					Debug.Log("Adding "+triangleList_.Count+" tris");
				}
				for (int i = 0; i < triangleList_.Count; i++)
				{
					TriangleListElement t = triangleList_.GetTriAtIndex(i);
					t.AddToMeshGenLists( this, verts, triVerts);
				}
			}

			if (rectList_ != null)
			{
				if ( DEBUG_MESHMAKE )
				{
					Debug.Log("Adding "+rectList_.Count+" rects");
				}
				for (int i = 0; i < rectList_.Count; i++)
				{
					RectListElement t = rectList_.GetRectAtIndex(i);
					t.AddToMeshGenLists( this, verts, triVerts);
				}
			}

			mesh.vertices = verts.ToArray();
			mesh.triangles = triVerts.ToArray();

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			meshCollider_.sharedMesh = null;
			meshCollider_.sharedMesh = meshFilter_.sharedMesh;
			if (DEBUG_MESHMAKE)
			{
				Debug.Log("Finish making "+this.DebugDescribe());
			}
		}

		void Update()
		{
			if ( isDirty_ )
			{
				isDirty_ = false;
				MakeMesh ( );
			}
			if ( Input.GetMouseButtonDown (0 ) )
			{
				Debug.Log("Mouse");
				Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
				RaycastHit hit = new RaycastHit ( );
			
				if ( Physics.Raycast ( ray,  out hit ) )
				{
					Debug.Log("Hit");

					if ( hit.collider == meshCollider_ )
					{
						OnClicked ( );
					}
				}
			}
			if (vertexMovers_.Count > 0)
			{
				List < VertexMover > toRemove = new List<VertexMover>();

				foreach (VertexMover mover in vertexMovers_)
				{
					bool changed = mover.update( Time.deltaTime );
					if (changed)
					{
						SetDirty();
					}
					if (mover.Finished)
					{
						toRemove.Add(mover);
					}
				}
				if (toRemove.Count > 0)
				{
					foreach (VertexMover mover in toRemove)
					{
						vertexMovers_.Remove(mover);
					}
					RemoveDuplicateRects();
				}
			}
		}

		public bool IsSameRect(RectListElement t, RectListElement other)
		{
			int matches = 0;
			for (int tindex = 0; tindex < 4; tindex++)
			{
				Vector3 tpos = t.GetVertex(tindex);
				for (int otherindex = 0; otherindex < 4; otherindex++)
				{
					Vector3 otherpos = other.GetVertex(otherindex);

					if (Vector3.Distance( tpos, otherpos ) <= POSITION_TELRANCE*2f)
					{
						matches++;
						break;
					}
				}
			}
			if (matches > 0)
			{
//				Debug.Log(matches+" matches");
			}
			if (matches == 4)
			{
				Debug.Log ("Same Rect : "+t.DebugDescribe()+" "+other.DebugDescribe());
			}
			return ( matches == 4 );
		}
	
		private void RemoveDuplicateRects()
		{
			List < RectListElement > dupes = new List<RectListElement> ( );
			List < RectListElement > individuals = new List<RectListElement> ( );
			for ( int i = 0; i < rectList_.Count; i++ )
			{
				RectListElement rle = rectList_.GetRectAtIndex(i);

				RectListElement matchingIndividual = null;
				foreach (RectListElement ind in individuals)
				{
					if (IsSameRect(ind,rle))
					{
						matchingIndividual = ind;
						break;
					}
				}
				if (matchingIndividual != null)
				{
					individuals.Remove(matchingIndividual);
					dupes.Add(matchingIndividual);
					dupes.Add (rle);
				}
				else
				{
					individuals.Add(rle);
				}
			}
			Debug.Log ("Found "+individuals.Count+" individual and "+dupes.Count+" dupe rects (should be even)");
			foreach ( RectListElement rle in dupes )
			{
				rectList_.RemoveRect(rle);
			}
		}

		public void OnClicked()
		{
			Debug.Log ( "Clicked on the thing" );
//			SplitRandomTriangle();
		}

		public void ExtendRandomRect()
		{
			bool allow = true;
			if ( !allowMultiMovement && vertexMovers_.Count > 0 )
			{
				Debug.Log ("Not extending Rect because movers exist");
				allow = false;
			}
			if (allow && rectList_.Count > 5 )
			{
				int i = UnityEngine.Random.Range( 0, rectList_.Count);
				RectListElement t = rectList_.GetRectAtIndex(i);
				ExtendRect2( t, size_);
			}

		}

		/*
		public void ExtendRect(RectListElement originRect, float height)
		{
			int o0_i = originRect.GetVertexIndex ( 0 );
			Vector3 o0 = vertexList_.GetVectorAtIndex(o0_i);

			int o1_i = originRect.GetVertexIndex ( 1 );
			Vector3 o1 = vertexList_.GetVectorAtIndex(o1_i);

			int o2_i =  originRect.GetVertexIndex( 2 );
			Vector3 o2 = vertexList_.GetVectorAtIndex(o2_i);

			int o3_i = originRect.GetVertexIndex( 3 );
			Vector3 o3 = vertexList_.GetVectorAtIndex(o3_i);

			Vector3 originCentre = rectList_.GetCentre ( originRect);

			Vector3 direction = Vector3.Cross( o0-o2, o1-o3 );

			Vector3 n0 = o0 + direction * POSITION_TELRANCE * 2f;
			Vector3 n1 = o1 + direction * POSITION_TELRANCE * 2f;
			Vector3 n2 = o2 + direction * POSITION_TELRANCE * 2f;
			Vector3 n3 = o3 + direction * POSITION_TELRANCE * 2f;

			int n0_i = vertexList_.AddVertex(n0);
			int n1_i = vertexList_.AddVertex(n1);
			int n2_i = vertexList_.AddVertex(n2);
			int n3_i = vertexList_.AddVertex(n3);

			RectListElement newTopRect = new RectListElement(n0_i,n1_i,n2_i,n3_i);
			RectListElement newFrontRect = new RectListElement( o0_i, o1_i, n1_i, n0_i );
			RectListElement newArseRect = new RectListElement( o2_i, o3_i, n3_i, n2_i );
			RectListElement newLeftRect = new RectListElement( o3_i, o0_i, n0_i, n3_i);
			RectListElement newRightRect = new RectListElement( o1_i, o2_i, n2_i, n1_i);

			rectList_.AddRect( newTopRect);
			rectList_.AddRect( newFrontRect);
			rectList_.AddRect( newArseRect);
			rectList_.AddRect( newLeftRect);
			rectList_.AddRect( newRightRect);

			rectList_.RemoveRect( originRect);

			VertexMover newMover0 = new VertexMover( vertexList_.GetElement(n0_i), direction, height, 2f);
			VertexMover newMover1 = new VertexMover( vertexList_.GetElement(n1_i), direction, height, 2f);
			VertexMover newMover2 = new VertexMover( vertexList_.GetElement(n2_i), direction, height, 2f);
			VertexMover newMover3 = new VertexMover( vertexList_.GetElement(n3_i), direction, height, 2f);

			vertexMovers_.Add(newMover0);
			vertexMovers_.Add(newMover1);
			vertexMovers_.Add(newMover2);
			vertexMovers_.Add(newMover3);

		}*/

		public void ExtendRect2(RectListElement originRect, float height)
		{
			Debug.Log ( "ExtendRect2" );
			int[] originVertexIndices = new int[4];
			Vector3[] originVertices = new Vector3[4];

			for ( int i=0; i<4; i++ )
			{
				originVertexIndices[i] = originRect.GetVertexIndex ( i );
				originVertices[i] = originRect.GetVertex ( i );
			}
			Vector3 originCentre = originRect.GetCentre ( );
			
			Vector3 direction = Vector3.Cross( originVertices[0]-originVertices[2], originVertices[1]-originVertices[3]);
			direction.Normalize();

			int[] newVertexIndices = new int[4];
			Vector3[] newVertices = new Vector3[4];

			for ( int i=0; i<4; i++ )
			{
				newVertices[i] = originVertices[i] + direction * POSITION_TELRANCE * 2f;
				newVertexIndices[i] = vertexList_.AddVertex(newVertices[i]);
			}

			rectList_.RemoveRect( originRect);

			// Don't add new ones till analysed
			List < MeshGenRectList.RectsSharingEdgeInfo >[] rectsSharingEdges = new List < MeshGenRectList.RectsSharingEdgeInfo >[4];
			int totalRectsSharingEdges = 0;
			for (int i = 0; i<4; i++)
			{
				rectsSharingEdges[i] = rectList_.GetRectsSharingEdge
					(
						originVertexIndices[ RectListElement.EdgeDefs.EdgeDef(i).GetIndex(0) ] ,
						originVertexIndices[ RectListElement.EdgeDefs.EdgeDef(i).GetIndex(1)]
					);
				totalRectsSharingEdges += rectsSharingEdges[i].Count; 
			}

			Vector3 originRectNormalNormed =  originRect.GetNormal();
			originRectNormalNormed.Normalize();

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append ("ANALYSIS: origin = ").Append (originRect.DebugDescribe())
				.Append (" Norm = ").Append(originRectNormalNormed)
					.Append(" dirn = ").Append (direction);
			if (totalRectsSharingEdges > 0)
			{
				sb.Append ("\n "+totalRectsSharingEdges+" RectSharing edges:");
				for (int i = 0; i<4; i++)
				{
					sb.Append ("\n  Edge ").Append (i)
						.Append(" (" ).Append(originRect.GetVertexIndex( RectListElement.EdgeDefs.EdgeDef(i).GetIndex(0) ))
							.Append (", ").Append(originRect.GetVertexIndex( RectListElement.EdgeDefs.EdgeDef(i).GetIndex(1) ))
							.Append (" ): ").Append (rectsSharingEdges[i].Count);
					foreach (MeshGenRectList.RectsSharingEdgeInfo sharingEdgeInfo in rectsSharingEdges[i])
					{
						if (sharingEdgeInfo.shares * sharingEdgeInfo.shares != 1)
						{
							string error = "This rect shouldn't have shares = "+sharingEdgeInfo.shares+": "+sharingEdgeInfo.rle.DebugDescribe();
							Debug.LogError(error);
							sb.Append(error);
						}
						else
						{
							Vector3 rleNormalNormed = sharingEdgeInfo.rle.GetNormal();
							rleNormalNormed.Normalize();
							sb.Append ("\n   ").Append(sharingEdgeInfo.rle.DebugDescribe());
							sb.Append( " angle = ").Append(sharingEdgeInfo.shares * RectListElement.AngleBetweenNormalsDegrees(sharingEdgeInfo.rle, originRect))
								.Append ( " = ").Append(sharingEdgeInfo.shares * RectListElement.AngleBetweenNormalsDegrees(originRect, sharingEdgeInfo.rle))
									.Append (" (").Append (sharingEdgeInfo.shares).Append (") norm = ")
									.Append (rleNormalNormed);
							sharingEdgeInfo.dirnAway0.Normalize();
							sharingEdgeInfo.dirnAway1.Normalize();
							sb.Append(" Dirns Away: ").Append(sharingEdgeInfo.dirnAway0);
							if (sharingEdgeInfo.dirnAway0 == direction)
							{
								sb.Append(" !!!! ");
							}
							sb.Append (" ").Append(sharingEdgeInfo.dirnAway1);
							if (sharingEdgeInfo.dirnAway1 == direction)
							{
								sb.Append(" !!!! ");
							}

						}
					}
				}


			}
			else
			{
				sb.Append ("\n NO RectSharing edges:");
			}

			Debug.Log(sb.ToString());

			// identify which rects sharing edges need modifying
			// angle between rects? is direction same as normal?
			// don't create a side rect, just connect to the shared one
			// make the new top rect's shared one's top vertices
			// new type of mover - takes vertex to another vertex
			// 0123 faces out


			RectListElement newTopRect = new RectListElement(rectList_, newVertexIndices[0], newVertexIndices[1], newVertexIndices[2], newVertexIndices[3]);
			RectListElement newFrontRect = new RectListElement(rectList_,  originVertexIndices[0], originVertexIndices[1], newVertexIndices[1], newVertexIndices[0] );
			RectListElement newArseRect = new RectListElement(rectList_,  originVertexIndices[2], originVertexIndices[3], newVertexIndices[3], newVertexIndices[2] );
			RectListElement newLeftRect = new RectListElement(rectList_,  originVertexIndices[3], originVertexIndices[0], newVertexIndices[0], newVertexIndices[3]);
			RectListElement newRightRect = new RectListElement(rectList_,  originVertexIndices[1], originVertexIndices[2], newVertexIndices[2], newVertexIndices[1]);

			// Don't add new ones till analysed
			rectList_.AddRect( newTopRect);
			rectList_.AddRect( newFrontRect);
			rectList_.AddRect( newArseRect);
			rectList_.AddRect( newLeftRect);
			rectList_.AddRect( newRightRect);



			for (int i =0; i<4; i++)
			{
				VertexMover newMover = new VertexMover( vertexList_.GetElement(newVertexIndices[i]), direction, height, 2f);
				vertexMovers_.Add(newMover);
			}
		}

		public void ExtendRect(RectListElement originRect, float height)
		{
			int[] originVertexIndices = new int[4];
			Vector3[] originVertices = new Vector3[4];
			
			for ( int i=0; i<4; i++ )
			{
				originVertexIndices[i] = originRect.GetVertexIndex ( i );
				originVertices[i] = vertexList_.GetVectorAtIndex( originVertexIndices[i]);
			}
			Vector3 originCentre = originRect.GetCentre ( );
			
			Vector3 direction = Vector3.Cross( originVertices[0]-originVertices[2], originVertices[1]-originVertices[3]);
			
			int[] newVertexIndices = new int[4];
			Vector3[] newVertices = new Vector3[4];
			
			for ( int i=0; i<4; i++ )
			{
				newVertices[i] = originVertices[i] + direction * POSITION_TELRANCE * 10f;
				newVertexIndices[i] = vertexList_.AddVertex(newVertices[i]);
			}
			
			RectListElement newTopRect = new RectListElement(rectList_, newVertexIndices[0], newVertexIndices[1], newVertexIndices[2], newVertexIndices[3]);
			RectListElement newFrontRect = new RectListElement(rectList_,  originVertexIndices[0], originVertexIndices[1], newVertexIndices[1], newVertexIndices[0] );
			RectListElement newArseRect = new RectListElement(rectList_,  originVertexIndices[2], originVertexIndices[3], newVertexIndices[3], newVertexIndices[2] );
			RectListElement newLeftRect = new RectListElement(rectList_,  originVertexIndices[3], originVertexIndices[0], newVertexIndices[0], newVertexIndices[3]);
			RectListElement newRightRect = new RectListElement(rectList_,  originVertexIndices[1], originVertexIndices[2], newVertexIndices[2], newVertexIndices[1]);

			rectList_.AddRect( newTopRect);
			rectList_.AddRect( newFrontRect);
			rectList_.AddRect( newArseRect);
			rectList_.AddRect( newLeftRect);
			rectList_.AddRect( newRightRect);
			
			rectList_.RemoveRect( originRect);
			
			for (int i =0; i<4; i++)
			{
				VertexMover newMover = new VertexMover( vertexList_.GetElement(newVertexIndices[i]), direction, height, 2f);
				vertexMovers_.Add(newMover);
			}
		}

		public void SplitRandomTriangle()
		{
			bool allow = true;
			if ( !allowMultiMovement && vertexMovers_.Count > 0 )
			{
				Debug.Log ("Not splitting triangle because movers exist");
				allow = false;
			}

			if ( allow && triangleList_.Count > 3 )
			{
				int i = UnityEngine.Random.Range( 0, triangleList_.Count);
				TriangleListElement t = triangleList_.GetTriAtIndex(i);

				// TODO Check distance from centre of triangle to obstacle
				// this is lower bound for mover distance
				VertexListElement newVertex = SplitTriangle( t);

				Vector3 v0 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(0));
				Vector3 v1 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(1));
				Vector3 v2 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(2));
				// get dist as mean of the 3 edges
				float tetEdge = ( 
				              Vector3.Distance( v0,v1)
				              + Vector3.Distance( v1,v2)
				              + Vector3.Distance( v2,v0)
				              ) / 3f;
				float height = tetEdge * Mathf.Sqrt (2f/3f);
				Vector3 direction = Vector3.Cross( v0-v1, v2-v0 );
				direction.Normalize();
				direction = -1f * direction;
				VertexMover newMover = new VertexMover( newVertex, direction, height, 2f);
				vertexMovers_.Add(newMover);
			}
		}

		VertexListElement SplitTriangle(TriangleListElement t)
		{
			Vector3 newVector = triangleList_.GetCentre ( t );
			int newVectorIndex = vertexList_.AddVertex( newVector );
			
			Vector3 v0 = vertexList_.GetVectorAtIndex( t.GetVertexIndex(0) );
			Vector3 v1 = vertexList_.GetVectorAtIndex( t.GetVertexIndex(1) );
			Vector3 v2 = vertexList_.GetVectorAtIndex( t.GetVertexIndex(2) );
			
			TriangleListElement t0 = new TriangleListElement( t.GetVertexIndex(0), t.GetVertexIndex(1), newVectorIndex);
			TriangleListElement t1 = new TriangleListElement( t.GetVertexIndex(1), t.GetVertexIndex(2), newVectorIndex);
			TriangleListElement t2 = new TriangleListElement( newVectorIndex, t.GetVertexIndex(2), t.GetVertexIndex(0));
			
			triangleList_.AddTriangle(t0);
			triangleList_.AddTriangle(t1);
			triangleList_.AddTriangle(t2);
			
			triangleList_.RemoveTriangle(t);
			
			Debug.Log ("Split triangle: Lost "+t.DebugDescribe()
			           +"\nGained "+t0.DebugDescribe()
			           +"\n       "+t1.DebugDescribe()
			           +"\n       "+t2.DebugDescribe()
			           );
			
			SetDirty();
			
			return vertexList_.GetElement( newVectorIndex);
		}
		

#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("MeshGen: "+vertexList_.Count+" verts, "+triangleList_.Count+" tris, "+rectList_.Count+" rects");
		}
#endregion IDebugDescribable


	}

}

