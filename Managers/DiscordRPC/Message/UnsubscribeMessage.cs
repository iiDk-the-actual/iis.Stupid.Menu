using iiMenu.Managers.DiscordRPC.RPC.Payload;

namespace iiMenu.Managers.DiscordRPC.Message
{
	public class UnsubscribeMessage : IMessage
	{
		public override MessageType Type
		{
			get
			{
				return MessageType.Unsubscribe;
			}
		}

		public EventType Event { get; internal set; }

		internal UnsubscribeMessage(ServerEvent evt)
		{
			switch (evt)
			{
			default:
                    Event = EventType.Join;
				return;
			case ServerEvent.ActivitySpectate:
                    Event = EventType.Spectate;
				return;
			case ServerEvent.ActivityJoinRequest:
                    Event = EventType.JoinRequest;
				return;
			}
		}
	}
}
