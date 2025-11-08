using iiMenu.Managers.DiscordRPC.Exceptions;
using System;
using System.Text;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	[Serializable]
	public class Secrets
	{
		[Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.")]
		[JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
		public string MatchSecret
		{
			get
			{
				return _matchSecret;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _matchSecret, 128, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(128);
				}
			}
		}

		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		public string JoinSecret
		{
			get
			{
				return _joinSecret;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _joinSecret, 128, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(128);
				}
			}
		}

		[JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
		public string SpectateSecret
		{
			get
			{
				return _spectateSecret;
			}
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _spectateSecret, 128, Encoding.UTF8))
				{
					throw new StringOutOfRangeException(128);
				}
			}
		}

		public static Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public static int SecretLength
		{
			get
			{
				return 128;
			}
		}

		public static string CreateSecret(Random random)
		{
			byte[] array = new byte[SecretLength];
			random.NextBytes(array);
			return Encoding.GetString(array);
		}

		public static string CreateFriendlySecret(Random random)
		{
			string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < SecretLength; i++)
			{
				stringBuilder.Append(text[random.Next(text.Length)]);
			}
			return stringBuilder.ToString();
		}

		private string _matchSecret;

		private string _joinSecret;

		private string _spectateSecret;
	}
}
