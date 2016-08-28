namespace LD36.Messaging
{
	internal enum MessageTypes
	{
		Artifact,
		Escape,
		Keyboard,
		Mouse,
		Start
	}

	internal abstract class GameMessage
	{
		protected GameMessage(MessageTypes type)
		{
			Type = type;
		}

		public MessageTypes Type { get; }
	}
}
