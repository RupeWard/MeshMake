using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenerator : MonoBehaviour, IDebugDescribable
	{
		public static readonly float POSITION_TELRANCE = 0.001f;

		protected MeshGenVertexList vertexList_ = null;
		protected MeshGenTriangleList triangleList_ = null;

		private MeshFilter meshFilter_;
		private MeshRenderer meshRenderer_;
		private MeshCollider meshCollider_;

		private List < VertexMover > vertexMovers_ = new List< VertexMover > ( );

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
//			Debug.Log("Making mesh");

			if (vertexList_.Count == 0 || triangleList_.Count == 0)
			{
				Debug.LogError( "Can't make mesh with "+vertexList_.Count+" verts and "+triangleList_.Count+" tris");
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
			for (int i = 0; i < triangleList_.Count; i++)
			{
				TriangleListElement t = triangleList_.GetTriAtIndex(i);

				for (int tmp=0; tmp<3; tmp++)
				{
					verts.Add ( vertexList_.GetVectorAtIndex( t.GetVertexIndex(tmp) ) );
					triVerts.Add ( i*3 + tmp);
				}
			}
			mesh.vertices = verts.ToArray();
			mesh.triangles = triVerts.ToArray();

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			meshCollider_.sharedMesh = meshFilter_.sharedMesh;

			Debug.Log("Finish making "+this.DebugDescribe());
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

		void Update()
		{
			if ( isDirty_ )
			{
				MakeMesh ( );
				isDirty_ = false;
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
				foreach (VertexMover mover in toRemove)
				{
					vertexMovers_.Remove(mover);
				}
			}
		}

		public void OnClicked()
		{
			Debug.Log ( "Clicked on the thing" );
			SplitRandomTriangle();
		}

		public void SplitRandomTriangle()
		{
			if ( triangleList_.Count > 3 )
			{
				int i = UnityEngine.Random.Range( 0, triangleList_.Count);
				TriangleListElement t = triangleList_.GetTriAtIndex(i);
				VertexListElement newVertex = SplitTriangle( t);

				Vector3 v0 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(0));
				Vector3 v1 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(1));
				Vector3 v2 = vertexList_.GetVectorAtIndex(t.GetVertexIndex(2));
				// get dist as mean of the 3 edges
				float dist = ( 
				              Vector3.Distance( v0,v1)
				              + Vector3.Distance( v1,v2)
				              + Vector3.Distance( v2,v0)
				              ) / 3f;
				Vector3 direction = Vector3.Cross( v0-v1, v2-v0 );
				direction.Normalize();
				direction = -1f * direction;
				VertexMover newMover = new VertexMover( newVertex, direction, dist, 2f);
				vertexMovers_.Add(newMover);
			}
		}

#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("MeshGen: "+vertexList_.Count+" verts, "+triangleList_.Count+" tris");
		}
#endregion IDebugDescribable


	}

}

