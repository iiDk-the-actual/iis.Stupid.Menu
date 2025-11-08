namespace iiMenu.Managers.DiscordRPC.IO
{
	public enum Opcode : uint
	{
		Handshake,
		Frame,
		Close,
		Ping,
		Pong
	}
}
