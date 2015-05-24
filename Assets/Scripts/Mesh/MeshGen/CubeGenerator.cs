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
			go.transform.parent = AppManager.Instance.world;
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


/*
			GridUVProviders.GridPosition gp0 = new GridUVProviders.GridPosition( 0,0); // cyan in color3x3
			GridUVProviders.GridPosition gp1 = new GridUVProviders.GridPosition( 0,1);// mauve in color3x3
			GridUVProviders.GridPosition gp2 = new GridUVProviders.GridPosition( 0,2 );// yellow in color3x3
			GridUVProviders.GridPosition gp3 = new GridUVProviders.GridPosition( 1,1);// grey in color3x3
			GridUVProviders.GridPosition gp4 = new GridUVProviders.GridPosition( 1,2);// purpkke in color3x3
			GridUVProviders.GridPosition gp5 = new GridUVProviders.GridPosition( 1,2);// red in color3x3
//			GridUVProviders.GridPosition gp6 = new GridUVProviders.GridPosition( 2,0);// green in color3x3
//			GridUVProviders.GridPosition gp7 = new GridUVProviders.GridPosition( 2,1);// black in color3x3
//			GridUVProviders.GridPosition gp8 = new GridUVProviders.GridPosition( 2,2);// blue in color3x3
*/

			GridUVProviders.GridPosition gp = MeshGenerator.cyanRectGridPosition;

//			GridUVProviders.GridPosition gp1 = gp0;
//			GridUVProviders.GridPosition gp2 = gp0;
//			GridUVProviders.GridPosition gp3 = gp0;
//			GridUVProviders.GridPosition gp4 = gp0;
//			GridUVProviders.GridPosition gp5 = gp0;

			RectListElement baseRect = new RectListElement(rectList_,  b3, b2, b1, b0, MeshGenerator.gridUVProviders, gp );
			RectListElement topRect = new RectListElement(rectList_,  t1, t2, t3, t0, MeshGenerator.gridUVProviders, gp );
			RectListElement frontRect = new RectListElement(rectList_,  b0, b1, t1, t0, MeshGenerator.gridUVProviders, gp );
			RectListElement arseRect = new RectListElement(rectList_,  b2, b3, t3, t2, MeshGenerator.gridUVProviders, gp );
			RectListElement leftRect = new RectListElement(rectList_,  b3, b0, t0, t3, MeshGenerator.gridUVProviders, gp);
			RectListElement rightRect = new RectListElement(rectList_,  b1, b2, t2, t1, MeshGenerator.gridUVProviders, gp);

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

