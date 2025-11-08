using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.Message
{
	public class ReadyMessage : IMessage
	{
		public override MessageType Type
		{
			get
			{
				return MessageType.Ready;
			}
		}

		[JsonProperty("config")]
		public Configuration Configuration { get; set; }

		[JsonProperty("user")]
		public User User { get; set; }

		[JsonProperty("v")]
		public int Version { get; set; }
	}
}
