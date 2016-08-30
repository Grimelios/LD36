using System.Collections.Generic;
using LD36.Input;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Messaging.Input;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD36.UI
{
	internal class ControlsUI : UIElement, IMessageReceiver
	{
		private const int Lifetime = 8000;
		private const int FadeInDuration = 500;
		private const int FadeOutDuration = 1500;
		private const int QuickFadeTime = 100;
		private const int EdgeOffset = 75;
		private const int Spacing = 105;
		private const float Opacity = 0.5f;

		private static Color visibleTint = Color.White * Opacity;

		private Timer timer;
		private List<Sprite> sprites;
		private Color targetColor;

		private bool fading;
		private bool fadingIn;
		private bool initialFadeComplete;

		public ControlsUI(MessageSystem messageSystem)
		{
			fading = true;
			fadingIn = true;
			timer = new Timer(FadeInDuration, () =>
			{
				targetColor = visibleTint;
				EndFade();

				timer = new Timer(Lifetime, () =>
				{
					fading = true;
					fadingIn = false;
					timer = new Timer(FadeOutDuration, () =>
					{
						initialFadeComplete = true;
						targetColor = Color.Transparent;

						EndFade();
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

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
		}

		public void Receive(GameMessage message)
		{
			if (!initialFadeComplete)
			{
				return;
			}

			KeyboardData data = ((KeyboardMessage)message).Data;

			bool tabPressedThisFrame = data.KeysPressedThisFrame.Contains(Keys.Tab);
			bool tabReleasedThisFrame = data.KeysReleasedThisFrame.Contains(Keys.Tab);

			if (tabPressedThisFrame || tabReleasedThisFrame)
			{
				float initialElapsed = 0;

				if (fading)
				{
					initialElapsed = timer.Duration - timer.Elapsed;
					timer.Destroy();
				}
				else
				{
					fading = true;
				}

				if (tabPressedThisFrame)
				{
					fadingIn = true;
					targetColor = visibleTint;
					timer = new Timer(QuickFadeTime, EndFade, false);
				}
				else
				{
					fadingIn = false;
					targetColor = Color.Transparent;
					timer = new Timer(QuickFadeTime, EndFade, false);
				}

				timer.Elapsed = initialElapsed;
			}
		}

		private void EndFade()
		{
			fading = false;
			sprites.ForEach(s => s.Tint = targetColor);
		}

		public override void Update(float dt)
		{
			if (fading)
			{
				Color startColor = fadingIn ? Color.Transparent : visibleTint;
				Color endColor = fadingIn ? visibleTint : Color.Transparent;
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
