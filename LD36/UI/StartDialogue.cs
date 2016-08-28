using LD36.Timing;

namespace LD36.UI
{
	internal class StartDialogue : Dialogue
	{
		private const int Offset = 30;
		private const int Lifetime = 1000;

		private Timer timer;

		public StartDialogue(UserInterface userInterface) : base("Dialogue", "Find the three pieces of the ancient artifact.", Offset)
		{
			timer = new Timer(Lifetime, () =>
			{
				userInterface.Elements.Remove(this);
			}, false);
		}
	}
}
