using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal class Edge
	{
		public Edge(Vector2 start, Vector2 end)
		{
			Start = start;
			End = end;
		}

		public Vector2 Start { get; }
		public Vector2 End { get; }
	}
}
