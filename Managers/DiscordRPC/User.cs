using System;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	public class User
	{
		[JsonProperty("id")]
		public ulong ID { get; private set; }

		[JsonProperty("username")]
		public string Username { get; private set; }

		[JsonProperty("discriminator")]
		[Obsolete("Discord no longer uses discriminators.")]
		public int Discriminator { get; private set; }

		[JsonProperty("global_name")]
		public string DisplayName { get; private set; }

		[JsonProperty("avatar")]
		public string Avatar { get; private set; }

		[JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
		public Flag Flags { get; private set; }

		[JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
		public PremiumType Premium { get; private set; }

		public string CdnEndpoint { get; private set; }

		internal User()
		{
            CdnEndpoint = "cdn.discordapp.com";
		}

		internal void SetConfiguration(Configuration configuration)
		{
            CdnEndpoint = configuration.CdnHost;
		}

        [Obsolete]
        public string GetAvatarURL(AvatarFormat format)
		{
			return GetAvatarURL(format, AvatarSize.x128);
		}

        [Obsolete]
        public string GetAvatarURL(AvatarFormat format, AvatarSize size)
		{
			string text = string.Format("/avatars/{0}/{1}", ID, Avatar);
			if (string.IsNullOrEmpty(Avatar))
			{
				if (format != AvatarFormat.PNG)
				{
					throw new BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");
				}
				int num = (int)((ID >> 22) % 6UL);
				if (Discriminator > 0)
				{
					num = Discriminator % 5;
				}
				text = string.Format("/embed/avatars/{0}", num);
			}
			return string.Format("https://{0}{1}{2}?size={3}", new object[]
			{
                CdnEndpoint,
				text,
                GetAvatarExtension(format),
				(int)size
			});
		}

		public string GetAvatarExtension(AvatarFormat format)
		{
			return "." + format.ToString().ToLowerInvariant();
		}

        public override string ToString()
		{
			if (!string.IsNullOrEmpty(DisplayName))
			{
				return DisplayName;
			}
			return Username;
		}

		public enum AvatarFormat
		{
			PNG,
			JPEG,
			WebP,
			GIF
		}

		public enum AvatarSize
		{
			x16 = 16,
			x32 = 32,
			x64 = 64,
			x128 = 128,
			x256 = 256,
			x512 = 512,
			x1024 = 1024,
			x2048 = 2048
		}

		[Flags]
		public enum Flag
		{
			None = 0,
			Employee = 1,
			Partner = 2,
			HypeSquad = 4,
			BugHunter = 8,
			HouseBravery = 64,
			HouseBrilliance = 128,
			HouseBalance = 256,
			EarlySupporter = 512,
			TeamUser = 1024
		}

		public enum PremiumType
		{
			None,
			NitroClassic,
			Nitro
		}
	}
}
