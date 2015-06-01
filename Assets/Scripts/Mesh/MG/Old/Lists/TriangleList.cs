using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class TriangleList : ElementList < TriangleElement >  
	{
		public TriangleList(  ): base()
		{
		}

		public void TurnInsideOut()
		{
			foreach ( TriangleElement t in elements_ )
			{
				t.flipOrientation();
			}
		}

		public int AddElement(TriangleElement t)
		{
			int result = -1;
			result = elements_.Count;
			elements_.Add ( t );
			for ( int i = 0; i <3; i++)
			{
				t.GetVertex(i).ConnectToTriangle( t );
			}
			return result;
		}

		public void RemoveElement(TriangleElement t)
		{
			for ( int i = 0; i <3; i++)
			{
				t.GetVertex(i).DisconnectFromTriangle( t );
			}
			elements_.Remove ( t );
		}

	}
}

