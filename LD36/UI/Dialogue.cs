using System.Collections.Generic;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal abstract class Dialogue : UIElement
	{
		private const int CharacterRevealDelay = 25;

		private Timer revealTimer;
		private List<DialogueCharacter> characters;

		private string fontFilename;
		private string text;
		private int nextCharacterIndex;

		private SpriteFont font;
		private Vector2 basePosition;

		protected Dialogue(string fontFilename, string text, int offset)
		{
			this.fontFilename = fontFilename;
			this.text = text;

			font = ContentLoader.LoadFont(fontFilename);
			basePosition = new Vector2(Constants.ScreenWidth / 2, offset) - font.MeasureString(text) / 2;
			characters = new List<DialogueCharacter>();
			revealTimer = new Timer(CharacterRevealDelay, RevealNextCharacter, true, true);
		}

		private void RevealNextCharacter()
		{
			string revealedSoFar = text.Substring(0, nextCharacterIndex);

			Vector2 characterPosition = basePosition + new Vector2(font.MeasureString(revealedSoFar).X, 0);
			characters.Add(new DialogueCharacter(fontFilename, text[nextCharacterIndex], characterPosition));

			if (nextCharacterIndex == text.Length - 1)
			{
				revealTimer.Destroy();
				revealTimer = null;
			}
			else
			{
				nextCharacterIndex++;
			}
		}

		public override void Update(float dt)
		{
			characters.ForEach(c => c.Update(dt));
		}

		public override void Render(SpriteBatch sb)
		{
			characters.ForEach(c => c.Render(sb));
		}
	}
}
