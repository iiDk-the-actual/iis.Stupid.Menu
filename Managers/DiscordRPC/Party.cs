using iiMenu.Managers.DiscordRPC.Helper;
using System;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	[Serializable]
	public class Party
	{
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string ID
		{
			get
			{
				return _partyid;
			}
			set
			{
                _partyid = value.GetNullOrString();
			}
		}

		[JsonIgnore]
		public int Size { get; set; }

		[JsonIgnore]
		public int Max { get; set; }

		[JsonProperty("privacy", NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Include)]
		public PrivacySetting Privacy { get; set; }

		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		private int[] _size
		{
			get
			{
				int num = Math.Max(1, Size);
				return new int[]
				{
					num,
					Math.Max(num, Max)
				};
			}
			set
			{
				if (value.Length != 2)
				{
                    Size = 0;
                    Max = 0;
					return;
				}
                Size = value[0];
                Max = value[1];
			}
		}

		private string _partyid;

		public enum PrivacySetting
		{
			Private,
			Public
		}
	}
}
