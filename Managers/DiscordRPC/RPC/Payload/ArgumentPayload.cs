using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace iiMenu.Managers.DiscordRPC.RPC.Payload
{
	internal class ArgumentPayload : IPayload
	{
		[JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Arguments { get; set; }

		public ArgumentPayload()
		{
            Arguments = null;
		}

		public ArgumentPayload(long nonce) : base(nonce)
		{
            Arguments = null;
		}

		public ArgumentPayload(object args, long nonce) : base(nonce)
		{
            SetObject(args);
		}

		public void SetObject(object obj)
		{
            Arguments = JObject.FromObject(obj);
		}

		public T GetObject<T>()
		{
			return Arguments.ToObject<T>();
		}

		public override string ToString()
		{
			return "Argument " + base.ToString();
		}
	}
}
