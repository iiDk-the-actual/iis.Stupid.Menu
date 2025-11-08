using iiMenu.Managers.DiscordRPC.Exceptions;
using System;
using System.Text;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	[Serializable]
	public class Assets
	{
		[JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageKey
		{
			get
			{
				return _largeimagekey;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _largeimagekey, 256, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(256);
				}
				string largeimagekey = _largeimagekey;
                _islargeimagekeyexternal = largeimagekey != null && largeimagekey.StartsWith("mp:external/");
                _largeimageID = null;
			}
		}

		[JsonIgnore]
		public bool IsLargeImageKeyExternal
		{
			get
			{
				return _islargeimagekeyexternal;
			}
		}

		[JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageText
		{
			get
			{
				return _largeimagetext;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _largeimagetext, 128, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(128);
				}
			}
		}

		[JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageKey
		{
			get
			{
				return _smallimagekey;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _smallimagekey, 256, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(256);
				}
				string smallimagekey = _smallimagekey;
                _issmallimagekeyexternal = smallimagekey != null && smallimagekey.StartsWith("mp:external/");
                _smallimageID = null;
			}
		}

		[JsonIgnore]
		public bool IsSmallImageKeyExternal
		{
			get
			{
				return _issmallimagekeyexternal;
			}
		}

		[JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageText
		{
			get
			{
				return _smallimagetext;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _smallimagetext, 128, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(128);
				}
			}
		}

		[JsonIgnore]
		public ulong? LargeImageID
		{
			get
			{
				return _largeimageID;
			}
		}

		[JsonIgnore]
		public ulong? SmallImageID
		{
			get
			{
				return _smallimageID;
			}
		}

		internal void Merge(Assets other)
		{
            _smallimagetext = other._smallimagetext;
            _largeimagetext = other._largeimagetext;
			ulong value;
			if (ulong.TryParse(other._largeimagekey, out value))
			{
                _largeimageID = new ulong?(value);
			}
			else
			{
                _largeimagekey = other._largeimagekey;
                _largeimageID = null;
			}
			ulong value2;
			if (ulong.TryParse(other._smallimagekey, out value2))
			{
                _smallimageID = new ulong?(value2);
				return;
			}
            _smallimagekey = other._smallimagekey;
            _smallimageID = null;
		}

		private string _largeimagekey;

		private bool _islargeimagekeyexternal;

		private string _largeimagetext;

		private string _smallimagekey;

		private bool _issmallimagekeyexternal;

		private string _smallimagetext;

		private ulong? _largeimageID;

		private ulong? _smallimageID;
	}
}
