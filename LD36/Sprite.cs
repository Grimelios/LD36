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
			SetCommonValues(textureFilename, position);

			origin = new Vector2(texture.Width, texture.Height) / 2;
		}

		public Sprite(string textureFilename, Vector2 position, Vector2 origin)
		{
			this.origin = origin;
			
			SetCommonValues(textureFilename, position);
		}

		private void SetCommonValues(string textureFilename, Vector2 position)
		{
			texture = ContentLoader.LoadTexture(textureFilename);
			Position = position;
			Scale = Vector2.One;
			Tint = Color.White;
		}

		public Vector2 Position { get; set; }
		public Vector2 Scale { get; set; }
		public Color Tint { get; set; }

		public float Rotation { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.Draw(texture, Position, null, Tint, Rotation, origin, Scale, SpriteEffects.None, 0);
		}
	}
}
