using iiMenu.Managers.DiscordRPC.RPC.Payload;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC.Commands
{
	internal class PresenceCommand : ICommand
	{
		[JsonProperty("pid")]
		public int PID { get; set; }

		[JsonProperty("activity")]
		public RichPresence Presence { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Command.SetActivity
			};
		}
	}
}
