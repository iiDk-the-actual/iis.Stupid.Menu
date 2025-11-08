using System;

namespace iiMenu.Managers.DiscordRPC.Helper
{
	internal class BackoffDelay
	{
		public int Maximum { get; private set; }

		public int Minimum { get; private set; }

		public int Current
		{
			get
			{
				return _current;
			}
		}

		public int Fails
		{
			get
			{
				return _fails;
			}
		}

		public Random Random { get; set; }

		private BackoffDelay()
		{
		}

		public BackoffDelay(int min, int max) : this(min, max, new Random())
		{
		}

		public BackoffDelay(int min, int max, Random random)
		{
            Minimum = min;
            Maximum = max;
            _current = min;
            _fails = 0;
            Random = random;
		}

		public void Reset()
		{
            _fails = 0;
            _current = Minimum;
		}

		public int NextDelay()
		{
            _fails++;
			double num = (double)((Maximum - Minimum) / 100f);
            _current = (int)Math.Floor(num * _fails) + Minimum;
			return Math.Min(Math.Max(_current, Minimum), Maximum);
		}

		private int _current;

		private int _fails;
	}
}
