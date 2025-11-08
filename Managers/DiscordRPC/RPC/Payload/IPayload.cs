using iiMenu.Managers.DiscordRPC.Converters;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC.Payload
{
	internal abstract class IPayload
	{
		[JsonProperty("cmd")]
		[JsonConverter(typeof(EnumSnakeCaseConverter))]
		public Command Command { get; set; }

		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		protected IPayload()
		{
		}

		protected IPayload(long nonce)
		{
            Nonce = nonce.ToString();
		}

		public override string ToString()
		{
			return string.Format("Payload || Command: {0}, Nonce: {1}", Command, Nonce);
		}
	}
}
