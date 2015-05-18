using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class CubeGenerator : MeshGenerator
	{
		Vector3 centre_ = Vector3.zero;

		static public CubeGenerator Create (string name, Vector3 centre, float size)
		{
			GameObject go = new GameObject ( name );
			CubeGenerator tg = go.AddComponent< CubeGenerator >();
			tg.Init( centre, size);
			go.transform.localPosition = centre;
			return tg;
		}

		private void Init(Vector3 centre, float size) 
		{
			Debug.Log ( "CubeGen: CTOR Start" );
			this.centre_ = centre;
			this.size_ = size;

			Create ( );

			Debug.Log ( "Created " + this.DebugDescribe ( ));
		}

		private void Create()
		{
			float halfSide = 0.5f * size_;

			VertexListElement base0 = new VertexListElement( new Vector3 ( -1f * halfSide, -1f * halfSide, -1f * halfSide ));
			VertexListElement base1 = new VertexListElement( new Vector3 ( -1f * halfSide, -1f * halfSide, halfSide ));
			VertexListElement base2 = new VertexListElement( new Vector3 ( halfSide, -1f * halfSide, halfSide ));
			VertexListElement base3 = new VertexListElement( new Vector3 ( halfSide, -1f * halfSide, -1f * halfSide ));

			VertexListElement top0 = new VertexListElement( new Vector3 ( -1f * halfSide, halfSide, -1f * halfSide ));
			VertexListElement top1 = new VertexListElement( new Vector3 ( -1f * halfSide, halfSide, halfSide ));
			VertexListElement top2 = new VertexListElement( new Vector3 ( halfSide, halfSide, halfSide ));
			VertexListElement top3 = new VertexListElement( new Vector3 ( halfSide, halfSide, -1f * halfSide ));
			

			int b0 = vertexList_.AddVertex( base0);
			int b1 = vertexList_.AddVertex( base1);
			int b2 = vertexList_.AddVertex( base2);
			int b3 = vertexList_.AddVertex( base3);

			int t0 = vertexList_.AddVertex( top0);
			int t1 = vertexList_.AddVertex( top1);
			int t2 = vertexList_.AddVertex( top2);
			int t3 = vertexList_.AddVertex( top3);

			RectListElement baseRect = new RectListElement( b3, b2, b1, b0 );
			RectListElement topRect = new RectListElement( t1, t2, t3, t0 );
			RectListElement frontRect = new RectListElement( b0, b1, t1, t0 );
			RectListElement arseRect = new RectListElement( b2, b3, t3, t2 );
			RectListElement leftRect = new RectListElement( b3, b0, t0, t3);
			RectListElement rightRect = new RectListElement( b1, b2, t2, t1);

			rectList_.AddRect(baseRect);
			rectList_.AddRect(topRect);
			rectList_.AddRect(frontRect);
			rectList_.AddRect(arseRect);
			rectList_.AddRect(leftRect);
			rectList_.AddRect(rightRect);

			SetDirty();
		}
	}

}

