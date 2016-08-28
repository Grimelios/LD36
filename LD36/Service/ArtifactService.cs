using LD36.Interfaces;
using LD36.Messaging;

namespace LD36.Service
{
	internal class ArtifactService : IMessageReceiver
	{
		private const int TotalArtifacts = 1;

		private int artifactsCollected;

		private MessageSystem messageSystem;

		public ArtifactService(MessageSystem messageSystem)
		{
			this.messageSystem = messageSystem;

			messageSystem.Subscribe(MessageTypes.Artifact, this);
		}

		public void Receive(GameMessage message)
		{
			artifactsCollected++;

			if (artifactsCollected == TotalArtifacts)
			{
				messageSystem.Send(new EscapeMessage());
			}
		}
	}
}
