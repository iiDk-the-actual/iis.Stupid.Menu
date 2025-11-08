using System;

namespace iiMenu.Managers.DiscordRPC
{
	[Flags]
	public enum EventType
	{
		None = 0,
		Spectate = 1,
		Join = 2,
		JoinRequest = 4
	}
}
