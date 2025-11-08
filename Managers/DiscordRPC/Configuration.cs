using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	public class Configuration
	{
		[JsonProperty("api_endpoint")]
		public string ApiEndpoint { get; set; }

		[JsonProperty("cdn_host")]
		public string CdnHost { get; set; }

		[JsonProperty("environment")]
		public string Environment { get; set; }
	}
}
