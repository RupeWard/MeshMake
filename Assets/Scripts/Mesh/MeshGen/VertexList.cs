using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class VertexList // : MeshGenList < VertexListElement >  
	{
		private List< VertexElement > vertexElements_ = new List< VertexElement >();
		public List< VertexElement > Elements
		{
			get { return vertexElements_; }
		}

		public VertexElement GetClosestElement(Vector3 pos, float max, out float closestDistance)
		{
			VertexElement result = null;
			closestDistance = float.MaxValue;
			foreach ( VertexElement v in vertexElements_ )
			{
				float d = v.Distance(pos);
				if (d < max && d < closestDistance)
				{
					closestDistance = d;
					result = v;
				}
			}
			return result;
		}

		public VertexElement GetClosestElement(Vector3 pos, float max)
		{
			float closestDistance = float.MaxValue;
			VertexElement result = null;
			foreach ( VertexElement v in vertexElements_ )
			{
				float d = v.Distance(pos);
				if (d < max && d < closestDistance)
				{
					closestDistance = d;
					result = v;
				}
			}
			return result;
		}

		public VertexElement FindElement(Vector3 pos)
		{
			return GetClosestElement ( pos, _MeshGen.MeshGenerator.POSITION_TELRANCE );
		}

		public VertexElement AddElement(Vector3 pos)
		{
			VertexElement result = FindElement ( pos );
			if (result == null)
			{
				result = new VertexElement(pos);
				vertexElements_.Add(result);
			}
			else
			{
				Debug.LogWarning("Alrready have an element at "+pos);
			}
			return result;
		}

		public int Count
		{
			get { return vertexElements_.Count; }
		}

		public VertexElement GetElement(int i)
		{
			if ( i < 0 || i >= vertexElements_.Count )
			{
				Debug.LogError ("Can't get element of index "+i+" from "+vertexElements_.Count);
				return null;
			}
			return vertexElements_[i];
		}
	}
}

