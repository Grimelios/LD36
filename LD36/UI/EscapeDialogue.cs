using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal class EscapeDialogue : Dialogue
	{
		private const int Offset = 40;
		private const int SubTextOffset = 80;

		private SpriteText subText;

		public EscapeDialogue() : base("DialogueEscape", "Escape", Offset)
		{
			subText = new SpriteText("DialogueSmall", "Climb to the top.", new Vector2(Constants.ScreenWidth / 2, SubTextOffset));
		}

		public override void Update(float dt)
		{
		}

		public override void Render(SpriteBatch sb)
		{
			subText.Render(sb);

			base.Render(sb);
		}
	}
}
