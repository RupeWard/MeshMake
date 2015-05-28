using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace _MeshGen
{
	public class TriangleList // : MeshGenList < TriangleListElement >  
	{
		private List < TriangleElement > elements_ = new List< TriangleElement >();

		public List < TriangleElement > Elements
		{
			get { return elements_; }
		}

		public int Count
		{
			get { return elements_.Count; }
		}

		public TriangleList(  )
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

		public TriangleElement GetElement(int i)
		{
			if ( i < 0 || i >= elements_.Count )
			{
				Debug.LogError ("Can't get triangle at index "+i+" from "+elements_.Count);
				return null;
			}
			return elements_ [ i ];
		}
		
	}
}

