using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC.Payload
{
	internal class ClosePayload : IPayload
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("message")]
		public string Reason { get; set; }

		[JsonConstructor]
		public ClosePayload()
		{
            Code = -1;
            Reason = "";
		}
	}
}
