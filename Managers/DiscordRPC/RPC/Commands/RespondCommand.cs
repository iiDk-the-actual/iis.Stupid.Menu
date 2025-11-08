using iiMenu.Managers.DiscordRPC.RPC.Payload;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.RPC.Commands
{
	internal class RespondCommand : ICommand
	{
		[JsonProperty("user_id")]
		public string UserID { get; set; }

		[JsonIgnore]
		public bool Accept { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Accept ? Command.SendActivityJoinInvite : Command.CloseActivityJoinRequest
			};
		}
	}
}
