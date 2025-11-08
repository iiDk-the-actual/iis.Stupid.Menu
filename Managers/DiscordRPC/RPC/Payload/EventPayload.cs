using iiMenu.Managers.DiscordRPC.Converters;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace iiMenu.Managers.DiscordRPC.RPC.Payload
{
	internal class EventPayload : IPayload
	{
		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Data { get; set; }

		[JsonProperty("evt")]
		[JsonConverter(typeof(EnumSnakeCaseConverter))]
		public ServerEvent? Event { get; set; }

		public EventPayload()
		{
            Data = null;
		}

		public EventPayload(long nonce) : base(nonce)
		{
            Data = null;
		}

		public T GetObject<T>()
		{
			if (Data == null)
			{
				return default;
			}
			return Data.ToObject<T>();
		}

		public override string ToString()
		{
			return "Event " + base.ToString() + ", Event: " + (Event != null ? Event.ToString() : "N/A");
		}
	}
}
