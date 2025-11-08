namespace iiMenu.Managers.DiscordRPC.Message
{
	public class ConnectionEstablishedMessage : IMessage
	{
		public override MessageType Type
		{
			get
			{
				return MessageType.ConnectionEstablished;
			}
		}

		public int ConnectedPipe { get; internal set; }
	}
}
