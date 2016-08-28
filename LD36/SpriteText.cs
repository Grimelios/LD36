using LD36.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal class SpriteText : IRenderable
	{
		// This color is papyrus (at least according to google).
		private static readonly Color papyrusTint = new Color(238, 223, 166);

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

		public float Rotation { get; set; }
		public float Scale { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.DrawString(font, text, Position, papyrusTint, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
		}
	}
}
