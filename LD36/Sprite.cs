using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal class Sprite : IRenderable
	{
		private Texture2D texture;
		private Vector2 origin;

		public Sprite(string textureFilename, Vector2 position)
		{
			texture = ContentLoader.LoadTexture(textureFilename);
			origin = new Vector2(texture.Width, texture.Height) / 2;
			Position = position;
			Scale = 1;
		}

		public Vector2 Position { get; set; }

		public float Rotation { get; set; }
		public float Scale { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.Draw(texture, Position, null, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0);
		}
	}
}
