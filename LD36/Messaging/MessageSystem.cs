using System.Collections.Generic;
using LD36.Interfaces;

namespace LD36.Messaging
{
	using ReceiverMap = Dictionary<MessageTypes, List<IMessageReceiver>>;

	internal class MessageSystem
	{
		private ReceiverMap receiverMap;

		public MessageSystem()
		{
			receiverMap = new ReceiverMap();
		}

		public void Subscribe(MessageTypes messageType, IMessageReceiver receiver)
		{
			CheckType(messageType);
			receiverMap[messageType].Add(receiver);
		}

		public void Send(GameMessage message)
		{
			CheckType(message.Type);
			receiverMap[message.Type].ForEach(receiver => receiver.Receive(message));
		}

		private void CheckType(MessageTypes messageType)
		{
			if (!receiverMap.ContainsKey(messageType))
			{
				receiverMap.Add(messageType, new List<IMessageReceiver>());
			}
		}
	}
}
