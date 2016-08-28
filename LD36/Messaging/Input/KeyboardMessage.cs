using LD36.Input;

namespace LD36.Messaging.Input
{
	internal class KeyboardMessage : GameMessage
	{
		public KeyboardMessage(KeyboardData data) : base(MessageTypes.Keyboard)
		{
			Data = data;
		}

		public KeyboardData Data { get; }
	}
}
