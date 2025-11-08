using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.Message
{
	public class JoinRequestMessage : IMessage
	{
		public override MessageType Type
		{
			get
			{
				return MessageType.JoinRequest;
			}
		}

		[JsonProperty("user")]
		public User User { get; internal set; }
	}
}
