using LD36.Entities;
using Microsoft.Xna.Framework;

namespace LD36.Physics
{
	internal class RayCastResults
	{
		public RayCastResults(Vector2 position, Entity entity)
		{
			Position = position;
			Entity = entity;
		}

		public Vector2 Position { get; }
		public Entity Entity { get; }
	}
}
