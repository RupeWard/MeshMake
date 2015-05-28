using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenerator : MonoBehaviour, IDebugDescribable
	{
		static public readonly GridUVProviders gridUVProviders = new GridUVProviders ( 3,3);
		static public readonly GridUVProviders.GridPosition cyanRectGridPosition = new GridUVProviders.GridPosition ( 0,0 );
		static public readonly GridUVProviders.GridPosition mauveRectGridPosition = new GridUVProviders.GridPosition ( 0,1 );
		static public readonly GridUVProviders.GridPosition yellowRectGridPosition = new GridUVProviders.GridPosition ( 0,2 );
		static public readonly GridUVProviders.GridPosition greyRectGridPosition = new GridUVProviders.GridPosition( 1,0);// grey in color3x3
		static public readonly GridUVProviders.GridPosition purpleRectGridPosition = new GridUVProviders.GridPosition( 1,1);// purpkke in color3x3
		static public readonly GridUVProviders.GridPosition redRectGridPosition = new GridUVProviders.GridPosition( 1,2);// purpkke in color3x3
		static public readonly GridUVProviders.GridPosition greenRectGridPosition = new GridUVProviders.GridPosition( 2,0);// green in color3x3
		static public readonly GridUVProviders.GridPosition blackRectGridPosition = new GridUVProviders.GridPosition( 2,1);// black in color3x3
		static public readonly GridUVProviders.GridPosition blueRectGridPosition = new GridUVProviders.GridPosition( 2,2);// blue in color3x3

		public bool allowMultiExtend
		{
			get { return AppManager.Instance.allowMultiExtend; }
		}

		public static readonly float POSITION_TELRANCE = 0.001f;
		public static bool DEBUG_MESHMAKE = false;
		public static bool DEBUG_EXTENDRECT = false;

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
		private Rigidbody rigidBody_;
		private ReverseNormals reverseNormals_;

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
		public void SetDirty()
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
			meshCollider_.sharedMaterial = AppManager.Instance.defaultThingPhysicsMaterials;

			rigidBody_ = gameObject.GetComponent< Rigidbody > ( );
			if ( rigidBody_ == null )
			{
				rigidBody_ = gameObject.AddComponent< Rigidbody >();
			}
			rigidBody_.useGravity = false;
			rigidBody_.drag = 0.1f;
			rigidBody_.angularDrag = 0.1f;
			rigidBody_.velocity = Vector3.zero;
			rigidBody_.angularVelocity = Vector3.zero;
			rigidBody_.mass = 1f;
			rigidBody_.isKinematic = true;
			if ( reverseNormals_ == null )
			{
				reverseNormals_ = gameObject.AddComponent< ReverseNormals >();
			}
			reverseNormals_.SetState(ReverseNormals.EState.Outside);
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
			gameObject.tag = "Thing";

			Mesh mesh = meshFilter_.sharedMesh;
			if ( mesh == null )
			{
				meshFilter_.sharedMesh = new Mesh();
				mesh = meshFilter_.sharedMesh;
			}
			mesh.Clear ( );

			List < Vector3 > verts = new List< Vector3 >();
			List < Vector2 > uvs = new List< Vector2 >();
			List< int > triVerts = new List< int >();

			if (triangleList_ != null)
			{
				if ( DEBUG_MESHMAKE )
				{
					Debug.Log("Adding "+triangleList_.Count+" tris");
				}
				for (int i = 0; i < triangleList_.Count; i++)
				{
					TriangleElement t = triangleList_.GetTriAtIndex(i);
					t.AddToMeshGenLists( this, verts, uvs, triVerts);
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
					RectElement t = rectList_.GetRectAtIndex(i);
					t.AddToMeshGenLists( this, verts, uvs, triVerts);
				}
			}

			mesh.vertices = verts.ToArray();
			mesh.triangles = triVerts.ToArray();
			mesh.uv = uvs.ToArray();

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			meshCollider_.sharedMesh = null;
			meshCollider_.sharedMesh = meshFilter_.sharedMesh;

			reverseNormals_.SetState( (AppManager.Instance.Mode == AppManager.EMode.InternalCamera)?(ReverseNormals.EState.Inside):(ReverseNormals.EState.Outside) );
			if (DEBUG_MESHMAKE)
			{
				Debug.Log("Finish making "+this.DebugDescribe());
			}
//			rigidBody_.isKinematic = false;

		}
		
		void Update()
		{
			if ( isDirty_ )
			{
				isDirty_ = false;
				MakeMesh ( );
			}
			if ( AppManager.Instance.Mode != AppManager.EMode.InternalCamera && Input.GetMouseButtonDown (0 ) )
			{
				Debug.Log("Mouse");
				Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
				RaycastHit hit = new RaycastHit ( );
			
				if ( Physics.Raycast ( ray,  out hit ) )
				{
//					Debug.Log("Hit");

					if ( hit.collider == meshCollider_ )
					{
						OnClicked ( hit );
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
						mover.OnFinish();
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

		public bool IsSameRect(RectElement t, RectElement other)
		{
			int matches = 0;
			for (int tindex = 0; tindex < 4; tindex++)
			{
				Vector3 tpos = t.GetVector(tindex);
				for (int otherindex = 0; otherindex < 4; otherindex++)
				{
					Vector3 otherpos = other.GetVector(otherindex);

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
			List < RectElement[] > dupesSame = new List<RectElement [] > ( );
			List < RectElement > dupesOpposite = new List<RectElement> ( );
			List < RectElement > individuals = new List<RectElement> ( );
			for ( int i = 0; i < rectList_.Count; i++ )
			{
				RectElement rle = rectList_.GetRectAtIndex(i);

				RectElement matchingIndividual = null;
				foreach (RectElement ind in individuals)
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
					if ( rle.AngleFromNormalsRadians( matchingIndividual.GetNormal()) * Mathf.Rad2Deg < 1f )
					{
			//			dupesSame.Add (rle);
						dupesSame.Add (new RectElement[]{ matchingIndividual, rle });
					}
					else
					{
						dupesOpposite.Add(matchingIndividual);
						dupesOpposite.Add (rle);
					}
				}
				else
				{
					individuals.Add(rle);
				}
			}
			Debug.Log ("Found inds = "+individuals.Count+", dupesSame =  "+dupesSame.Count+" dupesOppos = "+dupesOpposite.Count+"  (should be even)");
			foreach ( RectElement rle in dupesOpposite )
			{
				rectList_.RemoveRect(rle);
			}

			foreach ( RectElement[] rles in dupesSame )
			{
//				rectList_.RemoveRectWithVertexReplace(rles[0], rles[1]);
				rectList_.RemoveRect(rles[0]);
//				rectList_.RemoveRect(rles[1]);
			}
		}

		public void OnClicked(RaycastHit hit)
		{
			Debug.Log ( "Clicked on the thing at "+hit.point );
			bool allow = true;
			if ( !allowMultiExtend && vertexMovers_.Count > 0 )
			{
				Debug.Log ("Not extending Rect because movers exist");
				allow = false;
			}
			if (allow)
			{
				RectElement rle = rectList_.GetClosestRect(hit.point);
				if (rle == null)
				{
					Debug.LogError("Failed to find closest rect to hit!");
				}
				else
				{
					
					Debug.Log ("Click-Extending "+rle.DebugDescribe());
					ExtendRect(rle, size_, yellowRectGridPosition, redRectGridPosition);
				}
			}
		}

		public void ExtendRandomRect()
		{
			bool allow = true;
			if ( !allowMultiExtend && vertexMovers_.Count > 0 )
			{
				Debug.Log ("Not extending Rect because movers exist");
				allow = false;
			}
			if (allow && rectList_.Count > 5 )
			{
				int i = UnityEngine.Random.Range( 0, rectList_.Count);
				RectElement t = rectList_.GetRectAtIndex(i);
				Debug.Log ("Rand-extending "+t.DebugDescribe());
				ExtendRect( t, size_, blueRectGridPosition, greyRectGridPosition);
			}

		}

		private System.Text.StringBuilder extendSB = new System.Text.StringBuilder();

		private bool AnyMoverMovesVertex(VertexElement el)
		{
			foreach ( VertexMover mover in vertexMovers_ )
			{
				if (mover.MovesVertexElement(el))
				{
					return true;
				}
			}
			return false;
		}

		public void ExtendRect(RectElement originRect, float height, GridUVProviders.GridPosition movingGridPosition, GridUVProviders.GridPosition finalGridPosition)
		{
			if ( !AppManager.Instance.allowCloseMultiExtend )
			{
				bool alreadyExtending = false;
				for ( int i = 0; i < 4; i++ )
				{
					if ( AnyMoverMovesVertex ( originRect.GetVertexElement ( i ) ) )
					{
						alreadyExtending = true;
					}
				}
				if ( alreadyExtending )
				{
					if ( DEBUG_EXTENDRECT )
					{
						Debug.LogWarning ( "Not extending " + originRect.DebugDescribe ( ) + " because one of its vertices is in motion" );
					}
					return;
				}
			}
			if ( DEBUG_EXTENDRECT )
			{
				extendSB.Length = 0;
				Debug.Log ( "ExtendRect " + originRect.DebugDescribe() );
			}
			if ( movingGridPosition == null )
			{
				movingGridPosition = greyRectGridPosition;
			}

//			int[] originVertexIndices = new int[4];
			Vector3[] originVectors = new Vector3[4];

			for ( int i=0; i<4; i++ )
			{
//				originVertexIndices[i] = originRect.GetVertexIndex ( i );
				originVectors[i] = originRect.GetVector ( i );
			}

			Vector3 originCentre = originRect.GetCentre ( );
	
			Vector3 direction = originRect.GetNormal();
//			Vector3 direction = Vector3.Cross( originVertices[0]-originVertices[2], originVertices[1]-originVertices[3]);
			direction.Normalize();

			VertexElement[] newVertexElements = new VertexElement[4]{ null, null, null, null };
			Vector3[] newVertices = new Vector3[4];


			if (AppManager.Instance.denyFacing)
			{
				int found = 0;
				for ( int i=0; i<4; i++ )
				{
					Vector3 newTarget = originVectors[i] + direction * size_;
					float closest;
					VertexElement vle = vertexList_.GetClosestElement(newTarget, size_/100f, out closest); 
					
					if (vle != null)
					{
						found++;
						//					Debug.LogWarning("Found close vertex to "+i+": "+newTarget+" "+vertexList_.GetElement(i).DebugDescribe()+"S="+size_+" D = "+closest);
						//					if (AnyMoverMovesVertex( vertexList_.GetElement(vleIndex)))
						//				    {
						//						found++;
						//					}
					}
				}
				if (found > 3)
				{
					Debug.LogError("Not extending rect "+originRect.DebugDescribe()+" because all 4 target vertices exist");
					return;
				}
			}
		
			for ( int i=0; i<4; i++ )
			{
					newVertices[i] = originVectors[i] + direction * POSITION_TELRANCE * 2f;
					newVertexElements[i] = vertexList_.AddVertexElement(newVertices[i]);
			}

			rectList_.RemoveRect( originRect);

			// Don't add new ones till analysed
			List < MeshGenRectList.RectsSharingEdgeInfo >[] rectsSharingEdges = new List < MeshGenRectList.RectsSharingEdgeInfo >[4];
			int totalRectsSharingEdges = 0;
			for (int i = 0; i<4; i++)
			{
				rectsSharingEdges[i] = rectList_.GetRectsSharingEdge
					(
						originRect.GetVertexElement(RectElement.EdgeDefs.EdgeDef(i).GetIndex(0)),
						originRect.GetVertexElement(RectElement.EdgeDefs.EdgeDef(i).GetIndex(1)),
						originRect
					);
				totalRectsSharingEdges += rectsSharingEdges[i].Count; 
			}

			Vector3 originRectNormalNormed =  originRect.GetNormal();
			originRectNormalNormed.Normalize();

			bool pruned = false;

			if (DEBUG_EXTENDRECT)
			{
				extendSB.Append ("ANALYSIS: origin = ").Append (originRect.DebugDescribe())
					.Append (" Norm = ").Append(originRectNormalNormed)
						.Append(" dirn = ").Append (direction);
			}
			if (totalRectsSharingEdges > 0)
			{
				if (DEBUG_EXTENDRECT)
				{
					extendSB.Append ("\n "+totalRectsSharingEdges+" RectSharing edges:");
				}
				for (int i = 0; i<4; i++)
				{
					if (DEBUG_EXTENDRECT)
					{
						extendSB.Append ("\n  Edge ").Append (i)
							.Append(" (" ).Append(originRect.GetVector( RectElement.EdgeDefs.EdgeDef(i).GetIndex(0) ))
								.Append (", ").Append(originRect.GetVector( RectElement.EdgeDefs.EdgeDef(i).GetIndex(1) ))
								.Append (" ): ").Append (rectsSharingEdges[i].Count);
					}
					List< MeshGenRectList.RectsSharingEdgeInfo > toRemove = new List<MeshGenRectList.RectsSharingEdgeInfo>();
					foreach (MeshGenRectList.RectsSharingEdgeInfo sharingEdgeInfo in rectsSharingEdges[i])
					{
						if (sharingEdgeInfo.shareOrder * sharingEdgeInfo.shareOrder != 1)
						{
							string error = "This rect shouldn't have shares = "+sharingEdgeInfo.shareOrder+": "+sharingEdgeInfo.rle.DebugDescribe();
							Debug.LogError(error);
							if (DEBUG_EXTENDRECT)
							{
								extendSB.Append(error);
							}
						}
						else
						{
							Vector3 rleNormalNormed = sharingEdgeInfo.rle.GetNormal();
							rleNormalNormed.Normalize();
							sharingEdgeInfo.dirnAway0.Normalize();
							sharingEdgeInfo.dirnAway1.Normalize();
							if (DEBUG_EXTENDRECT)
							{
								extendSB.Append ("\n   ").Append(sharingEdgeInfo.rle.DebugDescribe());
								extendSB.Append( " angle = ").Append(sharingEdgeInfo.shareOrder * RectElement.AngleBetweenNormalsDegrees(sharingEdgeInfo.rle, originRect))
									.Append ( " = ").Append(sharingEdgeInfo.shareOrder * RectElement.AngleBetweenNormalsDegrees(originRect, sharingEdgeInfo.rle))
										.Append (" (").Append (sharingEdgeInfo.shareOrder).Append (") norm = ")
										.Append (rleNormalNormed);
								extendSB.Append(" Dirns Away: ").Append(sharingEdgeInfo.dirnAway0);
								if (sharingEdgeInfo.dirnAway0 == direction)
								{
									extendSB.Append(" !!!! ");
								}
							}
							if (sharingEdgeInfo.dirnAway0 != direction)
							{
								toRemove.Add(sharingEdgeInfo);
							}
							if (DEBUG_EXTENDRECT)
							{
								extendSB.Append (" ").Append(sharingEdgeInfo.dirnAway1);
								if (sharingEdgeInfo.dirnAway1 == direction)
								{
									extendSB.Append(" !!!! ");
								}
							}
						}
					}
					foreach( MeshGenRectList.RectsSharingEdgeInfo rsi in toRemove)
					{
						pruned = true;
						rectsSharingEdges[i].Remove(rsi);
					}
				}
				if (DEBUG_EXTENDRECT)
				{
					if (pruned)
					{
						if (totalRectsSharingEdges > 0)
						{
							extendSB.Append ("\nAfterPruning...");
							extendSB.Append ("\n ").Append (totalRectsSharingEdges).Append (" RectSharing edges:");
							for (int i = 0; i<4; i++)
							{
								extendSB.Append ("\n  Edge ").Append (i)
									.Append(" (" ).Append(originRect.GetVector( RectElement.EdgeDefs.EdgeDef(i).GetIndex(0) ))
										.Append (", ").Append(originRect.GetVector( RectElement.EdgeDefs.EdgeDef(i).GetIndex(1) ))
										.Append (" ): ").Append (rectsSharingEdges[i].Count);
								foreach (MeshGenRectList.RectsSharingEdgeInfo sharingEdgeInfo in rectsSharingEdges[i])
								{
									if (sharingEdgeInfo.shareOrder * sharingEdgeInfo.shareOrder != 1)
									{
										string error = "This rect shouldn't have shares = "+sharingEdgeInfo.shareOrder+": "+sharingEdgeInfo.rle.DebugDescribe();
										Debug.LogError(error);
										extendSB.Append(error);
									}
									else
									{
										Vector3 rleNormalNormed = sharingEdgeInfo.rle.GetNormal();
										rleNormalNormed.Normalize();
										extendSB.Append ("\n   ").Append(sharingEdgeInfo.rle.DebugDescribe());
										extendSB.Append( " angle = ").Append(sharingEdgeInfo.shareOrder * RectElement.AngleBetweenNormalsDegrees(sharingEdgeInfo.rle, originRect))
											.Append ( " = ").Append(sharingEdgeInfo.shareOrder * RectElement.AngleBetweenNormalsDegrees(originRect, sharingEdgeInfo.rle))
												.Append (" (").Append (sharingEdgeInfo.shareOrder).Append (") norm = ")
												.Append (rleNormalNormed);
										sharingEdgeInfo.dirnAway0.Normalize();
										sharingEdgeInfo.dirnAway1.Normalize();
										extendSB.Append(" Dirns Away: ").Append(sharingEdgeInfo.dirnAway0);
										if (sharingEdgeInfo.dirnAway0 == direction)
										{
											extendSB.Append(" !!!! ");
										}
										extendSB.Append (" ").Append(sharingEdgeInfo.dirnAway1);
										if (sharingEdgeInfo.dirnAway1 == direction)
										{
											extendSB.Append(" !!!! ");
										}
										
									}
								}
							}
						}
					}
				}


			}
			else
			{
				extendSB.Append ("\n NO RectSharing edges:");
			}


			// identify which rects sharing edges need modifying
			// angle between rects? is direction same as normal?
			// don't create a side rect, just connect to the shared one
			// make the new top rect's shared one's top vertices
			// new type of mover - takes vertex to another vertex
			// 0123 faces out


			// go through the 4 edges
			// if there's no shared edge, do as before...

			// create all movers?
			RectElement newTopRect = new RectElement(rectList_, newVertexElements[0], newVertexElements[1], newVertexElements[2], newVertexElements[3], MeshGenerator.gridUVProviders, movingGridPosition);
			rectList_.AddRect( newTopRect);

			bool[] moverMadeForVertex = new bool[4]
			{
				false, false, false, false
			};

			for (int edgeIndex = 0; edgeIndex < 4; edgeIndex ++)
			{
				RectElement.EdgeDef edgeDef = RectElement.EdgeDefs.EdgeDef(edgeIndex);

				List < MeshGenRectList.RectsSharingEdgeInfo > rectsSharingEdge = rectsSharingEdges[edgeIndex];
				if (rectsSharingEdge != null && rectsSharingEdge.Count > 0)
				{
					//FIXME check length & log;

					MeshGenRectList.RectsSharingEdgeInfo edgeInfo = rectsSharingEdge[0];
					if (edgeInfo != null)
					{
						VertexElement newVertexElement0 = null;
						VertexElement newVertexElement1 = null;
						for (int i =0; i<4; i++)
						{
							if (originRect.GetVertexElement(i) == edgeInfo.vle0)
							{
								newVertexElement0 = newVertexElements[i];
							}
							if (originRect.GetVertexElement(i) == edgeInfo.vle1)
							{
								newVertexElement1 = newVertexElements[i];
							}
						}
						if (newVertexElement0 == null || newVertexElement1 == null)
						{
							Debug.LogError("Couldn't find matchign old/new vertices");
						}

						edgeInfo.rle.ReplaceVertex(edgeInfo.vle0, newVertexElement0);
						edgeInfo.rle.ReplaceVertex(edgeInfo.vle1, newVertexElement1);

						VertexMoverRectCollapser newMover = 
							new VertexMoverRectCollapser( rectList_,
							                             edgeInfo.rle,
							                             newVertexElement0,
							                             edgeInfo.neighbourVle0,
							                             newVertexElement1,
							                             edgeInfo.neighbourVle1,
							                             null,
							                             greenRectGridPosition,
							                             AppManager.Instance.moveDuration);

						vertexMovers_.Add(newMover);
						moverMadeForVertex[ edgeDef.GetIndex(0) ] = true;
						moverMadeForVertex[ edgeDef.GetIndex(1) ] = true;
						// FIXME log;

					}
				}
				else
				{
					RectElement newRect = 
						new RectElement(rectList_, 
						                    originRect.GetVertexElement( edgeDef.GetIndex(0)), 
						                    originRect.GetVertexElement( edgeDef.GetIndex(1)),
						                            newVertexElements[ edgeDef.GetIndex(1)],
						                    		newVertexElements [edgeDef.GetIndex(0)], 
						                    MeshGenerator.gridUVProviders, movingGridPosition );
					rectList_.AddRect( newRect);
					//FIXME log;

				}
			}

			for (int i =0; i<4; i++)
			{
				VertexMover newMover = 
					new VertexMoverDirectionDistance(originRect.GetVertexElement(i),

					                                 newVertexElements[i], 
					                                 direction, height, 
					                                 AppManager.Instance.moveDuration, 
					                                 finalGridPosition);
				vertexMovers_.Add(newMover);
				//FIXME log;

			}
			rigidBody_.mass = rigidBody_.mass +1f;
			if (DEBUG_EXTENDRECT)
			{
				Debug.Log(extendSB.ToString());
			}
		}

		/*
		public void ExtendRectOriginal(RectListElement originRect, float height)
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
				VertexMoverDirectionDistance newMover = new VertexMoverDirectionDistance( vertexList_.GetElement(newVertexIndices[i]), direction, height, 2f);
				vertexMovers_.Add(newMover);
			}
		}
		*/

		public void SplitRandomTriangle()
		{
			bool allow = true;
			if ( !allowMultiExtend && vertexMovers_.Count > 0 )
			{
				Debug.Log ("Not splitting triangle because movers exist");
				allow = false;
			}

			if ( allow && triangleList_.Count > 3 )
			{
				int i = UnityEngine.Random.Range( 0, triangleList_.Count);
				TriangleElement t = triangleList_.GetTriAtIndex(i);

				// TODO Check distance from centre of triangle to obstacle
				// this is lower bound for mover distance
				VertexElement newVertex = SplitTriangle( t);

				Vector3 v0 = t.GetVertex(0).GetVector();
				Vector3 v1 = t.GetVertex(1).GetVector();
				Vector3 v2 = t.GetVertex(2).GetVector();
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
				VertexMoverDirectionDistance newMover = new VertexMoverDirectionDistance(null, newVertex, direction, height, AppManager.Instance.moveDuration, null);
				vertexMovers_.Add(newMover);
			}
		}

		VertexElement SplitTriangle(TriangleElement t)
		{
			VertexElement newVertex = vertexList_.AddVertexElement(triangleList_.GetCentre ( t ));

			Vector3 v0 = t.GetVertex(0).GetVector();
			Vector3 v1 = t.GetVertex(1).GetVector();
			Vector3 v2 = t.GetVertex(2).GetVector();
			
			TriangleElement t0 = new TriangleElement( t.GetVertex(0), t.GetVertex(1), newVertex);
			TriangleElement t1 = new TriangleElement( t.GetVertex(1), t.GetVertex(2), newVertex);
			TriangleElement t2 = new TriangleElement( newVertex, t.GetVertex(2), t.GetVertex(0));
			
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
			
			return newVertex;
		}
		

#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("MeshGen: "+vertexList_.Count+" verts, "+triangleList_.Count+" tris, "+rectList_.Count+" rects");
		}
#endregion IDebugDescribable

		public void OnCollisionEnter(Collision collision)
		{
			PhysBall ball = collision.gameObject.GetComponent< PhysBall > ( );
			if ( ball != null )
			{
				bool allow = true;
				if ( !allowMultiExtend && vertexMovers_.Count > 0 )
				{
					Debug.Log ("Not extending Rect because movers exist");
					allow = false;
				}
				if (allow)
				{
					RectElement hitRect = rectList_.GetClosestRect ( collision.contacts[0].point);
					Debug.Log ( "Collision-extending "+hitRect.DebugDescribe()+" as Ball "+collision.gameObject.name+" hit "+gameObject.name );
					ExtendRect(hitRect, size_, purpleRectGridPosition, mauveRectGridPosition);
				}
			}
			else
			{
				Debug.Log ( "Something '"+collision.gameObject.name+"' hit "+gameObject.name );
			}
		}
	}

}

