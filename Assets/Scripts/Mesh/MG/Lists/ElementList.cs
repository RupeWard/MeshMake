﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class ElementList < TElement >// : MeshGenList < VertexListElement >  
	{
		private string ElementTypeString 
		{
			get { return typeof( TElement ).ToString()+"List"; }
		}

		protected List< TElement > elements_ = new List< TElement >();
		public List< TElement > Elements
		{
			get { return elements_; }
		}

		public virtual TElement GetClosestElement(Vector3 pos, float max, out float closestDistance)
		{
			Debug.LogError("Shouldn't be calling this on a "+ElementTypeString+"!");
			closestDistance = float.MaxValue;
			return default(TElement);
		}

		public virtual TElement GetClosestElement(Vector3 pos, float max)
		{
			Debug.LogError ( "Shouldn't be calling this on a " + ElementTypeString + "!" );
			return default(TElement);
		}	
	
		public virtual TElement FindElement(Vector3 pos)
		{
			return GetClosestElement ( pos, MG.MeshGenerator.POSITION_TELRANCE );
		}

		public int Count
		{
			get { return elements_.Count; }
		}

		public TElement GetRandomElement()
		{
			if ( elements_ != null && elements_.Count > 0 )
			{
				int i = UnityEngine.Random.Range( 0, Count);
				return elements_[i];
			}
			return  default(TElement);
		}
		

		/*
		public VertexElement GetElement(int i)
		{
			if ( i < 0 || i >= elements_.Count )
			{
				Debug.LogError ("Can't get element of index "+i+" from "+vertexElements_.Count);
				return null;
			}
			return vertexElements_[i];
		}*/
	}
}
