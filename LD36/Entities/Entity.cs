using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.Entities
{
	internal abstract class Entity : IDynamic, IRenderable
	{
		protected Entity(Vector2 position)
		{
			Position = position;
		}

		public Vector2 Position { get; set; }

		public abstract void Update(float dt);
		public abstract void Render(SpriteBatch sb);
	}
}
