using LD36.Input;

namespace LD36.Messaging.Input
{
	internal class MouseMessage : GameMessage
	{
		public MouseMessage(MouseData data) : base(MessageTypes.Mouse)
		{
			Data = data;
		}

		public MouseData Data { get; }
	}
}
