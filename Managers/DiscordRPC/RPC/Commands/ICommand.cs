using iiMenu.Managers.DiscordRPC.RPC.Payload;

namespace iiMenu.Managers.DiscordRPC.RPC.Commands
{
    internal interface ICommand
	{
		IPayload PreparePayload(long nonce);
	}
}
