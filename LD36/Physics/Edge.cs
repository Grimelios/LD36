using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal class Edge
	{
		public Edge(Vector2 start, Vector2 end, bool isGround)
		{
			Start = start;
			End = end;
			IsGround = isGround;
		}

		public Vector2 Start { get; }
		public Vector2 End { get; }

		public bool IsGround { get; }
	}
}
