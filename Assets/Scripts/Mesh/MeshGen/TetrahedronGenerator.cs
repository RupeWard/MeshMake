using UnityEngine;
using System.Collections;

namespace _MeshGen
{
	public class TetrahedronGenerator : MeshGenerator, IDebugDescribable
	{
		Vector3 centre_ = Vector3.zero;
		float size_ = 1f;

		public TetrahedronGenerator(Vector3 centre, float size) : base()
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

			double angleInTriangle = angleBetweenVertices/2;

			double heightOfTetCentre = System.Math.Sqrt ( squareDistCentreToVertex * squareDistCentreToVertex
			                                             - tetSideDistCentreToVertex * tetSideDistCentreToVertex);

			Vector3 base0 = centre_ + new Vector3 ( 0.5f * (float)tetSideLength, -1f * (float)(heightOfTetCentre) ,  (float)tetSideDistCentreToSide );
			Vector3 base1 = centre_ + new Vector3 ( -0.5f * (float)tetSideLength, -1f * (float)(heightOfTetCentre) ,  (float)tetSideDistCentreToSide );
			Vector3 base2 = centre_ + new Vector3 (0f, -1f * (float)(heightOfTetCentre), -1f * (float)tetSideDistCentreToVertex); 
			Vector3 apex = centre_ + new Vector3 ( 0f, (float)squareSideDistCentreToVertex, 0f  );

			int apexIndex = vertexList_.AddVertex( apex);
			int base0Index = vertexList_.AddVertex( base0);
			int base1Index = vertexList_.AddVertex( base1);
			int base2Index = vertexList_.AddVertex( base2);

			TriangleListElement baseTri = new TriangleListElement( base2Index, base0Index, base1Index);
			TriangleListElement side0Tri = new TriangleListElement ( apexIndex, base0Index, base2Index);
			TriangleListElement side1Tri = new TriangleListElement ( apexIndex, base1Index, base0Index);
			TriangleListElement side2Tri = new TriangleListElement ( apexIndex, base2Index, base1Index);

			triangleList_.AddTriangle(baseTri);
			triangleList_.AddTriangle(side0Tri);
			triangleList_.AddTriangle(side1Tri);
			triangleList_.AddTriangle(side2Tri);
		}

		#region IDebugDescribable
		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("TetGen: "+vertexList_.Count+" verts, "+triangleList_.Count+" tris");
		}
		#endregion IDebugDescribable
	}

}

