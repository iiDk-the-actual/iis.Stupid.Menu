using System;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	[Serializable]
	public class Timestamps
	{
		public static Timestamps Now
		{
			get
			{
				return new Timestamps(DateTime.UtcNow);
			}
		}

		public static Timestamps FromTimeSpan(double seconds)
		{
			return FromTimeSpan(TimeSpan.FromSeconds(seconds));
		}

		public static Timestamps FromTimeSpan(TimeSpan timespan)
		{
			return new Timestamps
			{
				Start = new DateTime?(DateTime.UtcNow),
				End = new DateTime?(DateTime.UtcNow + timespan)
			};
		}

		[JsonIgnore]
		public DateTime? Start { get; set; }

		[JsonIgnore]
		public DateTime? End { get; set; }

		public Timestamps()
		{
            Start = null;
            End = null;
		}

		public Timestamps(DateTime start)
		{
            Start = new DateTime?(start);
            End = null;
		}

		public Timestamps(DateTime start, DateTime end)
		{
            Start = new DateTime?(start);
            End = new DateTime?(end);
		}

		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		public ulong? StartUnixMilliseconds
		{
			get
			{
				if (Start == null)
				{
					return null;
				}
				return new ulong?(ToUnixMilliseconds(Start.Value));
			}
			set
			{
                Start = value != null ? new DateTime?(FromUnixMilliseconds(value.Value)) : null;
			}
		}

		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		public ulong? EndUnixMilliseconds
		{
			get
			{
				if (End == null)
				{
					return null;
				}
				return new ulong?(ToUnixMilliseconds(End.Value));
			}
			set
			{
                End = value != null ? new DateTime?(FromUnixMilliseconds(value.Value)) : null;
			}
		}

		public static DateTime FromUnixMilliseconds(ulong unixTime)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddMilliseconds(Convert.ToDouble(unixTime));
		}

		public static ulong ToUnixMilliseconds(DateTime date)
		{
			DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToUInt64((date - d).TotalMilliseconds);
		}
	}
}
