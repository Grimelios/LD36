using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal class SpriteText : IRenderable
	{
		private SpriteFont font;

		private string text;

		public SpriteText(string fontFilename, string text, Vector2 position)
		{
			this.text = text;

			font = ContentLoader.LoadFont(fontFilename);
			Position = position;
			Scale = 0;
		}

		public Vector2 Position { get; set; }
		public Color Tint { get; set; }

		public float Rotation { get; set; }
		public float Scale { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.DrawString(font, text, Position, Tint, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
		}
	}
}
