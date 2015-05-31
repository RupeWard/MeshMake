using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
	public class TriangleElement : IDebugDescribable
	{
		protected UV.I_RectUVProvider uvProvider_ = null;

		protected VertexElement[] vertexElements_ = new VertexElement[3]{ null, null, null };

		protected ElementStates.EState state_ =  ElementStates.EState.NONE;

		protected Vector2 [] uvs_ = new Vector2[3] { Vector2.zero, Vector2.zero, Vector2.zero };

		protected bool uvsDirty_ = false;

#if UNITY_EDITOR
		protected string d_uvsDirtyReason = string.Empty;
#endif
		public TriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, ElementStates.EState state)
		{
			state_ = state;
			vertexElements_[0] = v0;
			vertexElements_[1] = v1;
			vertexElements_[2] = v2;
			uvsDirty_ = true;
			#if UNITY_EDITOR
			d_uvsDirtyReason+="Ctor.";
			#endif

		}

		public TriangleElement( VertexElement v0, VertexElement v1, VertexElement v2, ElementStates.EState state, UV.I_RectUVProvider iup)
		{
			state_ = state;
			uvProvider_ = iup;
			vertexElements_[0] = v0;
			vertexElements_[1] = v1;
			vertexElements_[2] = v2;
			uvsDirty_ = true;
			#if UNITY_EDITOR
			d_uvsDirtyReason+="Ctor.";
			#endif
		}

		public void SetState(ElementStates.EState s)
		{
			if ( state_ != s )
			{
				state_ = s;
				uvsDirty_ = true;
				#if UNITY_EDITOR
				d_uvsDirtyReason+="State.";
				#endif
			}
		}

		public virtual void flipOrientation()
		{
			VertexElement tmp = vertexElements_ [ 0 ];
			vertexElements_[0] = vertexElements_[1];
			vertexElements_[1] = tmp;
		}

		public Vector3 GetCentre()
		{
			Vector3 result = Vector3.zero;
			for (int i = 0; i<3; i++)
			{
				result = result + vertexElements_[ i].GetVector();
			}
			result = result /4f;
			return result;
		}


		protected TriangleElement(){}

		public VertexElement GetVertex(int i)
		{
			return vertexElements_[i];
		}

		public bool ReplaceVertex( VertexElement oldVle, VertexElement newVle)
		{
			for (int i = 0; i< 3; i++)
			{
				if (vertexElements_[i] == oldVle)
				{
					vertexElements_[i] = newVle;
					return true;
				}
			}
			return false;
		}

		public virtual void AddToMeshGenLists( MeshGenerator gen, List < Vector3 > verts, List < Vector2 > uvs,  List < int > triVerts, int triangleNumber )
		{
			#if UNITY_EDITOR
			if (uvsDirty_)
			{
				Debug.Log("UVsDirtyReason = "+d_uvsDirtyReason);
				d_uvsDirtyReason = string.Empty;
			}
			#endif

			int firstIndex = verts.Count;
			for (int v=0; v<3; v++)
			{
				verts.Add ( vertexElements_[v].GetVector() );
				triVerts.Add ( firstIndex + v);
				if (uvProvider_ != null)
				{
					if (uvsDirty_)
					{  
						uvs_[v] = uvProvider_.GetUVForState(triangleNumber, v, state_);
					}
					uvs.Add( uvs_[v] );
				}
			}
			uvsDirty_ = false;
		}

		public static bool HasSameVertices(TriangleElement t, TriangleElement other)
		{
			int matches = 0;

			for (int tindex = 0; tindex < 3; tindex++)
			{
				for (int otherindex = 0; otherindex < 3; otherindex++)
				{
					if (t.GetVertex(tindex) == other.GetVertex(otherindex))
					{
						matches++;
						break;
					}
				}
			}
			return ( matches == 3 );
		}

		#region IDebugDescribable
		public virtual void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ("Tr: ");
			for ( int i =0; i<3; i++ )
			{
				if (i >0 ) sb.Append(", ");
				sb.Append(GetVertex(i));
			}
		}
		#endregion IDebugDescribable


	}
}

