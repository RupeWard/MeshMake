using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class VertexList :ElementList< VertexElement > // : MeshGenList < VertexListElement >  
	{
		public VertexList() : base()
		{
		}

		public override VertexElement GetClosestElement(Vector3 pos, float max, out float closestDistance)
		{
			VertexElement result = null;
			closestDistance = float.MaxValue;
			foreach ( VertexElement v in elements_ )
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

		public override VertexElement GetClosestElement(Vector3 pos, float max)
		{
			float closestDistance = float.MaxValue;
			VertexElement result = null;
			foreach ( VertexElement v in elements_ )
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

		public override VertexElement FindElement(Vector3 pos)
		{
			return GetClosestElement ( pos, MG.MGSettings.POSITION_TOLERANCE );
		}

		public VertexElement AddElement(Vector3 pos)
		{
			VertexElement result = FindElement ( pos );
			if (result == null)
			{
				result = new VertexElement(pos);
				elements_.Add(result);
			}
			else
			{
				Debug.LogWarning("Alrready have an element at "+pos);
			}
			return result;
		}

		public void DisconnectVertexFromRect(VertexElement vle, RectElement rle)
		{
			if (vle.DisconnectFromRect(rle) == 0)
			{
//				Debug.LogWarning("Removing unconnected vertex "+vle);
				elements_.Remove(vle);
			}
		}
	}
}

