using iiMenu.Managers.DiscordRPC.RPC.Payload;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC.Commands
{
	internal class CloseCommand : ICommand
	{
		[JsonProperty("pid")]
		public int PID { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload
			{
				Command = Command.Dispatch,
				Nonce = null,
				Arguments = null
			};
		}

		[JsonProperty("close_reason")]
		public string value = "Unity 5.5 doesn't handle thread aborts. Can you please close me discord?";
	}
}
