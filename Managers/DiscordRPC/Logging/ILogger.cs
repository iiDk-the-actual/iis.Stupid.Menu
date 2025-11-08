namespace iiMenu.Managers.DiscordRPC.Logging
{
	public interface ILogger
	{
		LogLevel Level { get; set; }

		void Trace(string message, params object[] args);

		void Info(string message, params object[] args);

		void Warning(string message, params object[] args);

		void Error(string message, params object[] args);
	}
}
