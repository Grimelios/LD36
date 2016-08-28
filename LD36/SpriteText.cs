using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal class SpriteText : IRenderable
	{
		private SpriteFont font;
		private Vector2 origin;

		private string text;

		public SpriteText(string fontFilename, string text, Vector2 position)
		{
			this.text = text;

			font = ContentLoader.LoadFont(fontFilename);
			origin = font.MeasureString(text) / 2;
			Position = position;
		}

		public Vector2 Position { get; set; }

		public float Rotation { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.DrawString(font, text, Position, Color.White, Rotation, origin, 1, SpriteEffects.None, 0);
		}
	}
}
