namespace iiMenu.Managers.DiscordRPC.Logging
{
	public class FileLogger : ILogger
	{
		public LogLevel Level { get; set; }

		public string File { get; set; }

		public FileLogger(string path) : this(path, LogLevel.Info)
		{
		}

		public FileLogger(string path, LogLevel level)
		{
            Level = level;
            File = path;
            filelock = new object();
		}

		public void Trace(string message, params object[] args)
		{
			if (Level > LogLevel.Trace)
			{
				return;
			}
			object obj = filelock;
			lock (obj)
			{
				System.IO.File.AppendAllText(File, "\r\nTRCE: " + (args.Length != 0 ? string.Format(message, args) : message));
			}
		}

		public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info)
			{
				return;
			}
			object obj = filelock;
			lock (obj)
			{
				System.IO.File.AppendAllText(File, "\r\nINFO: " + (args.Length != 0 ? string.Format(message, args) : message));
			}
		}

		public void Warning(string message, params object[] args)
		{
			if (Level > LogLevel.Warning)
			{
				return;
			}
			object obj = filelock;
			lock (obj)
			{
				System.IO.File.AppendAllText(File, "\r\nWARN: " + (args.Length != 0 ? string.Format(message, args) : message));
			}
		}

		public void Error(string message, params object[] args)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			object obj = filelock;
			lock (obj)
			{
				System.IO.File.AppendAllText(File, "\r\nERR : " + (args.Length != 0 ? string.Format(message, args) : message));
			}
		}

		private object filelock;
	}
}
