
namespace MG
{
	public static class ElementStates
	{
		public enum EState
		{
			NONE,
			Original,
			Growing,
			GrowingRand,
			GrowingClicked,
			GrowingBall,
			Collapsing,
			CollapsingRand,
			CollapsingClicked,
			CollapsingBall,
			Static,
			StaticRand,
			StaticClicked,
			StaticBall,
			Rand,
			Clicked, 
			Ball
		};

		static public EState GetGenericState(EState state)
		{
			EState result = EState.NONE;

			if (IsGrowing(state))
			{
				result = EState.Growing;
			}
			else if (IsCollapsing(state))
			{
				result = EState.Collapsing;
			}
			else if (IsStatic(state))
			{
				result = EState.Static;
			}
			return result;
		}

		static public bool IsGrowing(EState state)
		{
			return ( state == EState.Growing
				|| state == EState.GrowingBall
				|| state == EState.GrowingClicked
				|| state == EState.GrowingRand );
		}

		static public bool IsCollapsing(EState state)
		{
			return ( state == EState.Collapsing
			        || state == EState.CollapsingBall
			        || state == EState.CollapsingClicked
			        || state == EState.CollapsingRand );
		}
		
		static public bool IsStatic(EState state)
		{
			return ( state == EState.Static
			        || state == EState.StaticBall
			        || state == EState.StaticClicked
			        || state == EState.StaticRand );
		}
		
		public static EState ParseState(string s)
		{
			foreach (EState state in System.Enum.GetValues(typeof(EState)))
			{
				if (s.Equals(state.ToString()))
				{
					return state;
				}
			}
			return EState.NONE;
		}

	}
}