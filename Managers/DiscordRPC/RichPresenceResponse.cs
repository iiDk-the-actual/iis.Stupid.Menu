using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	internal sealed class RichPresenceResponse : BaseRichPresence
	{
		[JsonProperty("application_id")]
		public string ClientID { get; private set; }

		[JsonProperty("name")]
		public string Name { get; private set; }
	}
}
