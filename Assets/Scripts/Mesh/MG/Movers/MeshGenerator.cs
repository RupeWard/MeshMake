using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class MeshGenerator : MonoBehaviour, IDebugDescribable
	{
		static public int gridWidth=3;
		static public int gridHeight=3;

		protected UV.GridUVProvider gridUvProvider_=null;

		public bool allowMultiExtend
		{
			get { return AppManager.Instance.allowMultiExtend; }
		}

		public static readonly float POSITION_TELRANCE = 0.001f;
		public static bool DEBUG_MESHMAKE = false;
		public static bool DEBUG_EXTENDRECT = false;

		protected VertexList vertexList_ = null;
		public VertexList VertexList
		{
			get { return vertexList_; }
		}
		protected TriangleList triangleList_ = null;
		protected RectList rectList_ = null;

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
			gridUvProvider_ = new MG.UV.GridUVProvider (gridHeight, gridWidth );

			vertexList_ = new VertexList ( );
			triangleList_ = new TriangleList ( );
			rectList_ = new RectList (  );

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
				foreach (TriangleElement t in triangleList_.Elements)
				{
					t.AddToMeshGenLists( this, verts, uvs, triVerts, 0);
				}
			}

			if (rectList_ != null)
			{
				if ( DEBUG_MESHMAKE )
				{
					Debug.Log("Adding "+rectList_.Count+" rects");
				}
				foreach (RectElement r in rectList_.Elements)
				{
					r.AddToMeshGenLists( this, verts, uvs, triVerts);
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
//				Debug.Log("Mouse");
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
			foreach (RectElement rle in rectList_.Elements )
			{
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
				rectList_.RemoveElement(rle);
			}

			foreach ( RectElement[] rles in dupesSame )
			{
				rectList_.RemoveElement(rles[0]);
			}
		}

		public void OnClicked(RaycastHit hit)
		{
			if (!HudManager.Instance.eventSystem.IsPointerOverGameObject() )
			{
				//			Debug.Log ( "Clicked on the thing at "+hit.point );
				bool allow = true;
				if ( !allowMultiExtend && vertexMovers_.Count > 0 )
				{
					Debug.Log ("Not extending Rect because movers exist");
					allow = false;
				}
				if (allow)
				{
					RectElement rle = rectList_.GetClosestElement(hit.point);
					if (rle == null)
					{
						Debug.LogError("Failed to find closest rect to hit!");
					}
					else
					{
						
						//					Debug.Log ("Click-Extending "+rle.DebugDescribe());
						ExtendRect(rle, size_, ElementStates.EState.GrowingClicked, ElementStates.EState.StaticClicked, ElementStates.EState.CollapsingClicked);
					}
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
				RectElement t = rectList_.GetRandomElement();
//				Debug.Log ("Rand-extending "+t.DebugDescribe());
				ExtendRect( t, size_, ElementStates.EState.GrowingRand, ElementStates.EState.StaticRand, ElementStates.EState.CollapsingRand);
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

		private bool AnyMoverProtectsEdge(VertexElement vle0, VertexElement vle1)
		{
			foreach ( VertexMover mover in vertexMovers_ )
			{
				if (mover.IsProtectedEdge(vle0, vle1))
				{
					return true;
				}
			}
			return false;
		}

		public void ExtendRect(RectElement originRect, float height, ElementStates.EState growingState, ElementStates.EState finalState, ElementStates.EState collapsingState)
		{
			if ( !AppManager.Instance.allowSameVertexMultiExtend )
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

			Vector3[] originVectors = new Vector3[4];

			for ( int i=0; i<4; i++ )
			{
				originVectors[i] = originRect.GetVector ( i );
			}

			Vector3 originCentre = originRect.GetCentre ( );
	
			Vector3 direction = originRect.GetNormal();
			direction.Normalize();

			VertexElement[] newVertexElements = new VertexElement[4]{ null, null, null, null };
			Vector3[] newVertices = new Vector3[4];


			VertexElement[] existingTargetElements = new VertexElement[4]{ null, null, null, null };

			if (AppManager.Instance.denyFacing)
			{
				int numFoundFacingVertex = 0;
				int numProtectedEdges = 0;
				for ( int i=0; i<4; i++ )
				{
					Vector3 newTarget = originVectors[i] + direction * size_;
					float closest;
					VertexElement vle = vertexList_.GetClosestElement(newTarget, size_/100f, out closest); 
					
					if (vle != null)
					{
						existingTargetElements[i] = vle;
						numFoundFacingVertex++;
						//					Debug.LogWarning("Found close vertex to "+i+": "+newTarget+" "+vertexList_.GetElement(i).DebugDescribe()+"S="+size_+" D = "+closest);
						//					if (AnyMoverMovesVertex( vertexList_.GetElement(vleIndex)))
						//				    {
						//						found++;
						//					}
						if (AnyMoverProtectsEdge( originRect.GetVertexElement(i), vle))
						{
							numProtectedEdges++;
						}
						else
						{
//							Debug.Log("Found facing");
						}
					}
				}
				if (numProtectedEdges > 0)
				{
					Debug.LogError("NOT extending rect, "+numFoundFacingVertex+ " target vertices exist, with "
					               +numProtectedEdges+" protected edges"+"\n"+originRect.DebugDescribe());
					return;
				}
				if (numFoundFacingVertex > 3)
				{
					Debug.LogWarning("On extending rect, all 4 target vertices exist, with "
					               +numProtectedEdges+" protected edges"+"\n"+originRect.DebugDescribe());
//					return;
				}
			}
		
			for ( int i=0; i<4; i++ )
			{
					newVertices[i] = originVectors[i] + direction * POSITION_TELRANCE * 2f;
					newVertexElements[i] = vertexList_.AddElement(newVertices[i]);
			}

			rectList_.RemoveElement( originRect);

			// Don't add new ones till analysed
			List < RectList.RectsSharingEdgeInfo >[] rectsSharingEdges = new List < RectList.RectsSharingEdgeInfo >[4];
			int totalRectsSharingEdges = 0;
			for (int i = 0; i<4; i++)
			{
				rectsSharingEdges[i] = rectList_.GetRectsSharingEdge
					(
						originRect.GetVertexElement(RectEdgeDef.EdgeDefForEdge(i).GetIndex(0)),
						originRect.GetVertexElement(RectEdgeDef.EdgeDefForEdge(i).GetIndex(1)),
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
							.Append(" (" ).Append(originRect.GetVector( RectEdgeDef.EdgeDefForEdge(i).GetIndex(0) ))
								.Append (", ").Append(originRect.GetVector( RectEdgeDef.EdgeDefForEdge(i).GetIndex(1) ))
								.Append (" ): ").Append (rectsSharingEdges[i].Count);
					}
					List< RectList.RectsSharingEdgeInfo > toRemove = new List<RectList.RectsSharingEdgeInfo>();
					foreach (RectList.RectsSharingEdgeInfo sharingEdgeInfo in rectsSharingEdges[i])
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
					foreach( RectList.RectsSharingEdgeInfo rsi in toRemove)
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
									.Append(" (" ).Append(originRect.GetVector( RectEdgeDef.EdgeDefForEdge(i).GetIndex(0) ))
										.Append (", ").Append(originRect.GetVector( RectEdgeDef.EdgeDefForEdge(i).GetIndex(1) ))
										.Append (" ): ").Append (rectsSharingEdges[i].Count);
								foreach (RectList.RectsSharingEdgeInfo sharingEdgeInfo in rectsSharingEdges[i])
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
			RectElement newTopRect = new RectElement(rectList_, 
			                                         newVertexElements[0], 
			                                         newVertexElements[1], 
			                                         newVertexElements[2], 
			                                         newVertexElements[3], 
			                                         growingState,
			                                         gridUvProvider_);
			rectList_.AddElement( newTopRect);

			bool[] moverMadeForVertex = new bool[4]
			{
				false, false, false, false
			};

			for (int edgeIndex = 0; edgeIndex < 4; edgeIndex ++)
			{
				RectEdgeDef edgeDef = RectEdgeDef.EdgeDefForEdge(edgeIndex);

				List < RectList.RectsSharingEdgeInfo > rectsSharingEdge = rectsSharingEdges[edgeIndex];
				if (rectsSharingEdge != null && rectsSharingEdge.Count > 0)
				{
					//FIXME check length & log;

					RectList.RectsSharingEdgeInfo edgeInfo = rectsSharingEdge[0];
					if (edgeInfo != null)
					{
						VertexElement newVertexElement0 = null;
						VertexElement newVertexElement1 = null;
						VertexElement originVertexElement0 = null;
						VertexElement originVertexElement1 = null;
						for (int i =0; i<4; i++)
						{
							if (originRect.GetVertexElement(i) == edgeInfo.vle0)
							{
								newVertexElement0 = newVertexElements[i];
								originVertexElement0 = originRect.GetVertexElement(i);
							}
							if (originRect.GetVertexElement(i) == edgeInfo.vle1)
							{
								newVertexElement1 = newVertexElements[i];
								originVertexElement1 = originRect.GetVertexElement(i);
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
							                             originVertexElement0,
							                             originVertexElement1,
							                             collapsingState,
							                             AppManager.Instance.moveDuration);

						vertexMovers_.Add(newMover);
//						moverMadeForVertex[ edgeDef.GetIndex(0) ] = true;
//						moverMadeForVertex[ edgeDef.GetIndex(1) ] = true;
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
						                    		growingState,
						                gridUvProvider_);
					rectList_.AddElement( newRect);
					//FIXME log;
//					moverMadeForVertex[ edgeDef.GetIndex(0) ] = true;
//					moverMadeForVertex[ edgeDef.GetIndex(1) ] = true;

				}
			}

			for (int i =0; i<4; i++)
			{				
				if (!moverMadeForVertex[i])
				{
					if (existingTargetElements[i] == null)
					{
						VertexMover newMover = 
							new VertexMoverDirectionDistance(originRect.GetVertexElement(i),
							                                 
							                                 newVertexElements[i], 
							                                 direction, height, 
							                                 AppManager.Instance.moveDuration, 
							                                 finalState);
						vertexMovers_.Add(newMover);
					}
					else
					{
						VertexMoverTarget newMover=new VertexMoverTarget( newVertexElements[i], 
						                                                 existingTargetElements[i], 
						                                                 originRect.GetVertexElement(i), 
						                                                 AppManager.Instance.moveDuration);
						vertexMovers_.Add(newMover);
					}
				}

				//FIXME log;
			}
			rigidBody_.mass = rigidBody_.mass +1f;
			if (DEBUG_EXTENDRECT)
			{
				Debug.Log(extendSB.ToString());
			}
		}

#region Old Triangle Stuff		

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
				TriangleElement t = triangleList_.GetRandomElement();

				// TODO Check distance from centre of triangle to obstacle
				// this is lower bound for mover distance
				VertexElement newVertex = SplitTriangle( t, ElementStates.EState.GrowingRand);

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
				VertexMoverDirectionDistance newMover = new VertexMoverDirectionDistance(null, newVertex, direction, height, AppManager.Instance.moveDuration, ElementStates.EState.NONE);
				vertexMovers_.Add(newMover);
			}
		}

		VertexElement SplitTriangle(TriangleElement t, ElementStates.EState state)
		{
			VertexElement newVertex = vertexList_.AddElement(t.GetCentre ());

			Vector3 v0 = t.GetVertex(0).GetVector();
			Vector3 v1 = t.GetVertex(1).GetVector();
			Vector3 v2 = t.GetVertex(2).GetVector();
			
			TriangleElement t0 = new TriangleElement( t.GetVertex(0), t.GetVertex(1), newVertex, state);
			TriangleElement t1 = new TriangleElement( t.GetVertex(1), t.GetVertex(2), newVertex, state);
			TriangleElement t2 = new TriangleElement( newVertex, t.GetVertex(2), t.GetVertex(0), state);
			
			triangleList_.AddElement(t0);
			triangleList_.AddElement(t1);
			triangleList_.AddElement(t2);
			
			triangleList_.RemoveElement(t);
			
			Debug.Log ("Split triangle: Lost "+t.DebugDescribe()
			           +"\nGained "+t0.DebugDescribe()
			           +"\n       "+t1.DebugDescribe()
			           +"\n       "+t2.DebugDescribe()
			           );
			
			SetDirty();
			
			return newVertex;
		}
#endregion Old Triangle Stuff		

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
					RectElement hitRect = rectList_.GetClosestElement ( collision.contacts[0].point);
					Debug.Log ( "Collision-extending "+hitRect.DebugDescribe()+" as Ball "+collision.gameObject.name+" hit "+gameObject.name );
					ExtendRect(hitRect, size_, ElementStates.EState.GrowingBall, ElementStates.EState.StaticBall, ElementStates.EState.CollapsingBall);
				}
			}
			else
			{
				Debug.Log ( "Something '"+collision.gameObject.name+"' hit "+gameObject.name );
			}
		}
	}

}

