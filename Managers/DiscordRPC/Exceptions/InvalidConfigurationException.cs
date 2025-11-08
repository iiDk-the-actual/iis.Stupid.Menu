using System;

namespace iiMenu.Managers.DiscordRPC.Exceptions
{
	public class InvalidConfigurationException : Exception
	{
		internal InvalidConfigurationException(string message) : base(message)
		{
		}
	}
}
