namespace iiMenu.Managers.DiscordRPC.Logging
{
	public class NullLogger : ILogger
	{
		public LogLevel Level { get; set; }

		public void Trace(string message, params object[] args)
		{
		}

		public void Info(string message, params object[] args)
		{
		}

		public void Warning(string message, params object[] args)
		{
		}

		public void Error(string message, params object[] args)
		{
		}
	}
}
