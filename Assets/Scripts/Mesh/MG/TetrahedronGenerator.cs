using UnityEngine;
using System.Collections;

namespace MG
{
	public class TetrahedronGenerator : MeshGenerator
	{
		Vector3 centre_ = Vector3.zero;

		static public TetrahedronGenerator Create (string name, Vector3 centre, float size)
		{
			GameObject go = new GameObject ( name );
			TetrahedronGenerator tg = go.AddComponent< TetrahedronGenerator >();
			tg.Init( centre, size);
			go.transform.localPosition = centre;
			return tg;
		}

		private void Init(Vector3 centre, float size) 
		{
			Debug.Log ( "TetGen: CTOR Start" );
			this.centre_ = centre;
			this.size_ = size;

			Create ( );

			Debug.Log ( "Created " + this.DebugDescribe ( ));
		}

		private void Create()
		{
			double angleBetweenVertices = System.Math.Acos ( 1/3 );

			double squareSide = ( double )size_;
			double halfSquareSide = 0.5 * squareSide;

			double squareSideDistCentreToVertex = squareSide / System.Math.Sqrt ( 2 );
			double squareDistCentreToVertex = System.Math.Sqrt( squareSideDistCentreToVertex * squareSideDistCentreToVertex
			                                             + halfSquareSide * halfSquareSide);

			double tetSideLength = squareSide * System.Math.Sqrt(2); // same as daiagonal of square's side
			double halfTetSideLength = 0.5 * tetSideLength;

			// using 30-60 triangle on side of tet
			double tetSideDistCentreToVertex = 2 * halfTetSideLength / System.Math.Sqrt( 3);
			double tetSideDistCentreToSide = 0.5 * tetSideDistCentreToVertex;

			//double angleInTriangle = angleBetweenVertices/2;

			double heightOfTetCentre = System.Math.Sqrt ( squareDistCentreToVertex * squareDistCentreToVertex
			                                             - tetSideDistCentreToVertex * tetSideDistCentreToVertex);

			Vector3 base0 = new Vector3 ( 0.5f * (float)tetSideLength, -1f * (float)(heightOfTetCentre) ,  (float)tetSideDistCentreToSide );
			Vector3 base1 = new Vector3 ( -0.5f * (float)tetSideLength, -1f * (float)(heightOfTetCentre) ,  (float)tetSideDistCentreToSide );
			Vector3 base2 = new Vector3 (0f, -1f * (float)(heightOfTetCentre), -1f * (float)tetSideDistCentreToVertex); 
			Vector3 apex = new Vector3 ( 0f, (float)squareSideDistCentreToVertex, 0f  );

			VertexElement apexElement = vertexList_.AddElement( apex);
			VertexElement base0Element = vertexList_.AddElement( base0);
			VertexElement base1Element = vertexList_.AddElement( base1);
			VertexElement base2Element = vertexList_.AddElement( base2);

			TriangleElement baseTri = new TriangleElement( base2Element, base0Element, base1Element, ElementStates.EState.Original);
			TriangleElement side0Tri = new TriangleElement ( apexElement, base0Element, base2Element, ElementStates.EState.Original);
			TriangleElement side1Tri = new TriangleElement ( apexElement, base1Element, base0Element, ElementStates.EState.Original);
			TriangleElement side2Tri = new TriangleElement ( apexElement, base2Element, base1Element, ElementStates.EState.Original);

			triangleList_.AddElement(baseTri);
			triangleList_.AddElement(side0Tri);
			triangleList_.AddElement(side1Tri);
			triangleList_.AddElement(side2Tri);

			SetDirty();
		}
	}

}

