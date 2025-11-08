using System;

namespace iiMenu.Managers.DiscordRPC.Exceptions
{
	public class BadPresenceException : Exception
	{
		internal BadPresenceException(string message) : base(message)
		{
		}
	}
}
