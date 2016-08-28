using LD36.Interfaces;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal class DialogueCharacter : IDynamic, IRenderable
	{
		// This color is papyrus (at least according to google).
		private static readonly Color papyrusTint = new Color(238, 223, 166);

		private const int RevealTime = 200;

		private Timer timer;
		private SpriteText spriteText;

		public DialogueCharacter(string fontFilename, char character, Vector2 position)
		{
			spriteText = new SpriteText(fontFilename, character.ToString(), position)
			{
				Tint = papyrusTint
			};

			timer = new Timer(RevealTime, () =>
			{
				spriteText.Scale = 1;
			}, false);
		}

		public void Fade(float amount)
		{
			spriteText.Tint = Color.Lerp(papyrusTint, Color.Transparent, amount);
		}

		public void Update(float dt)
		{
			if (timer != null)
			{
				float amount = -timer.Progress * (timer.Progress - 2);
				spriteText.Scale = MathHelper.Lerp(0, 1, amount);
			}
		}

		public void Render(SpriteBatch sb)
		{
			spriteText.Render(sb);
		}
	}
}
