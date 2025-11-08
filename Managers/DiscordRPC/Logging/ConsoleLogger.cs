using System;

namespace iiMenu.Managers.DiscordRPC.Logging
{
	public class ConsoleLogger : ILogger
	{
		public LogLevel Level { get; set; }

		public bool Coloured { get; set; }

		[Obsolete("Use Coloured")]
		public bool Colored
		{
			get
			{
				return Coloured;
			}
			set
			{
                Coloured = value;
			}
		}

		public ConsoleLogger()
		{
            Level = LogLevel.Info;
            Coloured = false;
		}

		public ConsoleLogger(LogLevel level) : this()
		{
            Level = level;
		}

		public ConsoleLogger(LogLevel level, bool coloured)
		{
            Level = level;
            Coloured = coloured;
		}

		public void Trace(string message, params object[] args)
		{
			if (Level > LogLevel.Trace)
			{
				return;
			}
			if (Coloured)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			string text = "TRACE: " + message;
			if (args.Length != 0)
			{
				Console.WriteLine(text, args);
				return;
			}
			Console.WriteLine(text);
		}

		public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info)
			{
				return;
			}
			if (Coloured)
			{
				Console.ForegroundColor = ConsoleColor.White;
			}
			string text = "INFO: " + message;
			if (args.Length != 0)
			{
				Console.WriteLine(text, args);
				return;
			}
			Console.WriteLine(text);
		}

		public void Warning(string message, params object[] args)
		{
			if (Level > LogLevel.Warning)
			{
				return;
			}
			if (Coloured)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
			}
			string text = "WARN: " + message;
			if (args.Length != 0)
			{
				Console.WriteLine(text, args);
				return;
			}
			Console.WriteLine(text);
		}

		public void Error(string message, params object[] args)
		{
			if (Level > LogLevel.Error)
			{
				return;
			}
			if (Coloured)
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}
			string text = "ERR : " + message;
			if (args.Length != 0)
			{
				Console.WriteLine(text, args);
				return;
			}
			Console.WriteLine(text);
		}
	}
}
