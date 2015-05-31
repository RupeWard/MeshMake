using System;
using UnityEngine;

namespace MG
{
	
	public class RectEdgeDef :IDebugDescribable
	{
		#region static
		
		static private readonly RectEdgeDef[] s_edgeDefsForEdges = new RectEdgeDef[] 
		{
			new RectEdgeDef( 0, 1 ),
			new RectEdgeDef( 1, 2 ),
			new RectEdgeDef( 2, 3 ),
			new RectEdgeDef( 3, 0 )
		};
		
		static public RectEdgeDef EdgeDefForEdge(int i)
		{
			return s_edgeDefsForEdges [ i ];
		}
		
		static public int GetIndexOfNeighbouringPointFromEdge(RectEdgeDef edgeDef, int index)
		{
			int result = -1;
			foreach ( RectEdgeDef ed in s_edgeDefsForEdges )
			{
				if (!ed.SameEdge(edgeDef))
				{
					if (ed.GetIndex(0) == index)
					{
						if (result != -1)
						{
							Debug.LogError("Found secoind neighbour of "+index+" not in "+edgeDef+" which is "+ed.GetIndex(1));
						}
						result = ed.GetIndex(1);
					}
					else if (ed.GetIndex(1) == index)
					{
						if (result != -1)
						{
							Debug.LogError("Found secoind neighbour of "+index+" not in "+edgeDef+" which is "+ed.GetIndex(0));
						}
						result = ed.GetIndex(0);
					}
				}
			}
			if ( result == -1 )
			{
				Debug.LogError ( "Couldn't fidn nehbouring index" );
			}
			return result;
		}
		
		
		#endregion static
		private readonly int [] indices_ = new int[2];
		
		public int GetIndex(int i)
		{
			return indices_ [ i ];
		}
		
		private RectEdgeDef(int i0, int i1)
		{
			indices_[0]=i0;
			indices_[1]=i1;
		}
		
		private RectEdgeDef(){}
		
		public bool SameEdge (RectEdgeDef ed)
		{
			bool result = false;
			if (ed.indices_[0] == this.indices_[0] && ed.indices_[1] == this.indices_[1])
			{
				result = true;
			}
			#if UNITY_EDITOR
			if (ed.indices_[1] == this.indices_[0] && ed.indices_[0] == this.indices_[1])
			{
				Debug.LogWarning("Same edges but other way round");
			}
			#endif
			return result;
		}
		
		#region IDebugDescribable
		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ( "(" ).Append ( indices_ [ 0 ] ).Append ( "," ).Append ( indices_ [ 1 ] ).Append ( ")" );
		}
		#endregion IDebugDescribable
		
		static System.Text.StringBuilder s_sb = new System.Text.StringBuilder ( );
		public override string ToString ()
		{
			s_sb.Length = 0;
			DebugDescribe ( s_sb );
			return s_sb.ToString();
		}
		
	}

}
