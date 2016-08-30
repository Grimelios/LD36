using LD36.Interfaces;
using LD36.Messaging;
using LD36.Timing;
using LD36.UI;

namespace LD36.Service
{
	internal class UserInterfaceService : IMessageReceiver
	{
		private const int SubtextDelay = 1750;

		private UserInterface userInterface;
		private Timer timer;

		public UserInterfaceService(MessageSystem messageSystem, UserInterface userInterface)
		{
			this.userInterface = userInterface;

			messageSystem.Subscribe(MessageTypes.Escape, this);
			messageSystem.Subscribe(MessageTypes.Start, this);
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Escape:
					userInterface.Elements.Add(new EscapeDialogue());
					timer = new Timer(SubtextDelay, () =>
					{
						userInterface.Elements.Add(new ContinueDialogue());
					}, false);

					break;

				case MessageTypes.Start:
					userInterface.Elements.Add(DIKernel.Get<StartDialogue>());
					userInterface.Elements.Add(DIKernel.Get<ControlsUI>());
					break;
			}
		}
	}
}
