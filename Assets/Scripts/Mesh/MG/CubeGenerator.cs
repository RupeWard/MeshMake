using UnityEngine;
using System.Collections;

namespace MG
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

			Vector3 base0 = new Vector3 ( -1f * halfSide, -1f * halfSide, -1f * halfSide );
			Vector3 base1 = new Vector3 ( -1f * halfSide, -1f * halfSide, halfSide );
			Vector3 base2 = new Vector3 ( halfSide, -1f * halfSide, halfSide );
			Vector3 base3 = new Vector3 ( halfSide, -1f * halfSide, -1f * halfSide );

			Vector3 top0 = new Vector3 ( -1f * halfSide, halfSide, -1f * halfSide );
			Vector3 top1 = new Vector3 ( -1f * halfSide, halfSide, halfSide );
			Vector3 top2 = new Vector3 ( halfSide, halfSide, halfSide );
			Vector3 top3 = new Vector3 ( halfSide, halfSide, -1f * halfSide );
			

			VertexElement b0 = vertexList_.AddElement( base0);
			VertexElement b1 = vertexList_.AddElement( base1);
			VertexElement b2 = vertexList_.AddElement( base2);
			VertexElement b3 = vertexList_.AddElement( base3);

			VertexElement t0 = vertexList_.AddElement( top0);
			VertexElement t1 = vertexList_.AddElement( top1);
			VertexElement t2 = vertexList_.AddElement( top2);
			VertexElement t3 = vertexList_.AddElement( top3);


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

			RectElement baseRect = new RectElement(rectList_,  b3, b2, b1, b0, MeshGenerator.gridUVProviders, gp );
			RectElement topRect = new RectElement(rectList_,  t1, t2, t3, t0, MeshGenerator.gridUVProviders, gp );
			RectElement frontRect = new RectElement(rectList_,  b0, b1, t1, t0, MeshGenerator.gridUVProviders, gp );
			RectElement arseRect = new RectElement(rectList_,  b2, b3, t3, t2, MeshGenerator.gridUVProviders, gp );
			RectElement leftRect = new RectElement(rectList_,  b3, b0, t0, t3, MeshGenerator.gridUVProviders, gp);
			RectElement rightRect = new RectElement(rectList_,  b1, b2, t2, t1, MeshGenerator.gridUVProviders, gp);

			rectList_.AddElement(baseRect);
			rectList_.AddElement(topRect);
			rectList_.AddElement(frontRect);
			rectList_.AddElement(arseRect);
			rectList_.AddElement(leftRect);
			rectList_.AddElement(rightRect);

			SetDirty();
		}
	}

}

