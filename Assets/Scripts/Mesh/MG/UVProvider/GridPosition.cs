namespace MG.UV
{
	public class GridPosition : IDebugDescribable
	{
		public int column;
		public int row;
		public GridPosition(int c, int r)
		{
			column=c;
			row = r;
		}

#region IDebugDescribable
		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append ( "[" ).Append ( column ).Append ( "," ).Append ( row ).Append ( "]" );
		}
#endregion IDebugDescribable

	}
	

}

