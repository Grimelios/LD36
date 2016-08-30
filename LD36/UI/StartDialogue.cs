using LD36.Input;
using LD36.Interfaces;
using LD36.Messaging;
using LD36.Messaging.Input;
using LD36.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD36.UI
{
	internal class StartDialogue : Dialogue, IMessageReceiver
	{
		private const int Offset = 30;
		private const int Lifetime = 8500;
		private const int FadeTime = 1500;
		private const int QuickFadeTime = 100;

		private Timer timer;
		private Timer fadeTimer;

		private float targetAmount;
		private bool fading;
		private bool fadingIn;
		private bool initialFadeComplete;

		public StartDialogue(MessageSystem messageSystem) :
			base("Dialogue", "Find the three pieces of the ancient artifact.", Offset)
		{
			timer = new Timer(Lifetime, () =>
			{
				fading = true;

				fadeTimer = new Timer(FadeTime, () =>
				{
					initialFadeComplete = true;
					targetAmount = 0;

					EndFade();
				}, false);
			}, false);

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
					targetAmount = 1;
					timer = new Timer(QuickFadeTime, EndFade, false);
				}
				else
				{
					fadingIn = false;
					targetAmount = 0;
					timer = new Timer(QuickFadeTime, EndFade, false);
				}

				timer.Elapsed = initialElapsed;
			}
		}

		private void EndFade()
		{
			FadeCharactersIn(targetAmount);
		}

		public override void Update(float dt)
		{
			if (fading)
			{
				if (fadingIn)
				{
					FadeCharactersIn(fadeTimer.Progress);
				}
				else
				{
					FadeCharactersOut(fadeTimer.Progress);
				}
			}

			base.Update(dt);
		}
	}
}
