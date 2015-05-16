using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenerator : MonoBehaviour
	{
		public static readonly float POSITION_TELRANCE = 0.001f;

		protected MeshGenVertexList vertexList_ = null;
		protected MeshGenTriangleList triangleList_ = null;

		private MeshFilter meshFilter_;
		private MeshRenderer meshRenderer_;

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
			triangleList_ = new MeshGenTriangleList ( vertexList_);

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
		}

		void Start()
		{
			isDirty_ = false;
			vertexList_ = new MeshGenVertexList();
			triangleList_ = new MeshGenTriangleList(vertexList_);
		}

		public void MakeMesh()
		{
			Debug.Log("Making mesh");

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

			Debug.Log("Finish making mesh");
		}

		void Update()
		{
			if ( isDirty_ )
			{
				MakeMesh();
				isDirty_ = false;
			}
		}



	}

}

