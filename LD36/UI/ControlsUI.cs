using System.Collections.Generic;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36.UI
{
	internal class ControlsUI : UIElement
	{
		private const int Lifetime = 8000;
		private const int FadeInDuration = 500;
		private const int FadeOutDuration = 1500;
		private const int EdgeOffset = 75;
		private const int Spacing = 105;
		private const float Opacity = 0.5f;

		private Timer timer;
		private List<Sprite> sprites;

		private bool fading;
		private bool fadingIn;

		public ControlsUI(UserInterface userInterface)
		{
			fading = true;
			fadingIn = true;
			timer = new Timer(FadeInDuration, () =>
			{
				fading = false;
				timer = new Timer(Lifetime, () =>
				{
					fading = true;
					fadingIn = false;
					timer = new Timer(FadeOutDuration, () =>
					{
						userInterface.Elements.Remove(this);
					}, false);
				}, false);
			}, false);

			Vector2 bottomLeftCorner = new Vector2(0, Constants.ScreenHeight);
			Vector2 bottomRightCorner = new Vector2(Constants.ScreenWidth, Constants.ScreenHeight);
			Vector2 spacing = new Vector2(Spacing, 0);
			Vector2 basePosition = bottomLeftCorner + new Vector2(EdgeOffset, -EdgeOffset);

			sprites = new List<Sprite>
			{
				new Sprite("UI/Movement", basePosition),
                new Sprite("UI/Collect", basePosition + spacing),
                new Sprite("UI/Jump", basePosition + spacing * 2),
				new Sprite("UI/Grapple", bottomRightCorner - new Vector2(EdgeOffset))
			};
		}

		public override void Update(float dt)
		{
			if (fading)
			{
				Color startColor = fadingIn ? Color.Transparent : Color.White * Opacity;
				Color endColor = fadingIn ? Color.White * Opacity : Color.Transparent;
				Color tint = Color.Lerp(startColor, endColor, timer.Progress);

				sprites.ForEach(s => s.Tint = tint);
			}
		}

		public override void Render(SpriteBatch sb)
		{
			sprites.ForEach(s => s.Render(sb));
		}
	}
}
