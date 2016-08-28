using LD36.Interfaces;
using LD36.Messaging;
using LD36.UI;

namespace LD36.Service
{
	internal class UserInterfaceService : IMessageReceiver
	{
		private UserInterface userInterface;

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
					userInterface.Elements.Add(DIKernel.Get<EscapeDialogue>());
					break;

				case MessageTypes.Start:
					userInterface.Elements.Add(DIKernel.Get<StartDialogue>());
					break;
			}
		}
	}
}
