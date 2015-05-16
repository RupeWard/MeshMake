using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class MeshGenerator
	{
		public static readonly float POSITION_TELRANCE = 0.001f;

		protected MeshGenVertexList vertexList_ = null;
		protected MeshGenTriangleList triangleList_ = null;

		public MeshGenerator()
		{
			vertexList_ = new MeshGenVertexList();
			triangleList_ = new MeshGenTriangleList(vertexList_);

		}

		public void MakeMesh(MeshFilter meshFilter)
		{
			if (vertexList_.Count == 0 || triangleList_.Count == 0)
			{
				Debug.LogError( "Can't make mesh with "+vertexList_.Count+" verts and "+triangleList_.Count+" tris");
				return;
			}

			Mesh mesh = meshFilter.sharedMesh;
			if ( mesh == null )
			{
				meshFilter.sharedMesh = new Mesh();
				mesh = meshFilter.sharedMesh;
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
		}

	}

}

