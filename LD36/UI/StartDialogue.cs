using LD36.Timing;

namespace LD36.UI
{
	internal class StartDialogue : Dialogue
	{
		private const int Offset = 30;
		private const int Lifetime = 5000;
		private const int FadeTime = 500;

		private Timer timer;
		private Timer fadeTimer;

		public StartDialogue(UserInterface userInterface) : base("Dialogue", "Find the three pieces of the ancient artifact.", Offset)
		{
			timer = new Timer(Lifetime, () =>
			{
				fadeTimer = new Timer(FadeTime, () =>
				{
					userInterface.Elements.Remove(this);
				}, false);
			}, false);
		}

		public override void Update(float dt)
		{
			if (fadeTimer != null)
			{
				FadeCharacters(fadeTimer.Progress);
			}

			base.Update(dt);
		}
	}
}
