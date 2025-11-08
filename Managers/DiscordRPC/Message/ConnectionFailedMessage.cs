namespace iiMenu.Managers.DiscordRPC.Message
{
	public class ConnectionFailedMessage : IMessage
	{
		public override MessageType Type
		{
			get
			{
				return MessageType.ConnectionFailed;
			}
		}

		public int FailedPipe { get; internal set; }
	}
}
