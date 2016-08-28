using LD36.Messaging;

namespace LD36.Interfaces
{
	internal interface IMessageReceiver
	{
		void Receive(GameMessage message);
	}
}
