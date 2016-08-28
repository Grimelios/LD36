using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal abstract class Dialogue : UIElement
	{
		private const int TopOffset = 30;

		private SpriteText spriteText;

		protected Dialogue(string fontFilename, string text, int offset)
		{
			spriteText = new SpriteText(fontFilename, text, new Vector2(Constants.ScreenWidth / 2, offset));
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			spriteText.Render(sb);
		}
	}
}
