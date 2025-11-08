using System;

namespace iiMenu.Managers.DiscordRPC.Exceptions
{
	public class UninitializedException : Exception
	{
		internal UninitializedException(string message) : base(message)
		{
		}

		internal UninitializedException() : this("Cannot perform action because the client has not been initialized yet or has been deinitialized.")
		{
		}
	}
}
