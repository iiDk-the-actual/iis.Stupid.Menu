using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC
{
	public sealed class RichPresence : BaseRichPresence
	{
		[JsonProperty("buttons", NullValueHandling = NullValueHandling.Ignore)]
		public Button[] Buttons { get; set; }

		public bool HasButtons()
		{
			return Buttons != null && Buttons.Length != 0;
		}

		public RichPresence WithState(string state)
		{
            State = state;
			return this;
		}

		public RichPresence WithDetails(string details)
		{
            Details = details;
			return this;
		}

		public RichPresence WithTimestamps(Timestamps timestamps)
		{
            Timestamps = timestamps;
			return this;
		}

		public RichPresence WithAssets(Assets assets)
		{
            Assets = assets;
			return this;
		}

		public RichPresence WithParty(Party party)
		{
            Party = party;
			return this;
		}

		public RichPresence WithSecrets(Secrets secrets)
		{
            Secrets = secrets;
			return this;
		}

		public RichPresence Clone()
		{
			RichPresence richPresence = new RichPresence();
			richPresence.State = _state != null ? _state.Clone() as string : null;
			richPresence.Details = _details != null ? _details.Clone() as string : null;
			richPresence.Buttons = !HasButtons() ? null : Buttons.Clone() as Button[];
			Secrets secrets2;
			if (HasSecrets())
			{
				Secrets secrets = new Secrets();
				secrets.JoinSecret = Secrets.JoinSecret != null ? Secrets.JoinSecret.Clone() as string : null;
				secrets2 = secrets;
				secrets.SpectateSecret = Secrets.SpectateSecret != null ? Secrets.SpectateSecret.Clone() as string : null;
			}
			else
			{
				secrets2 = null;
			}
			richPresence.Secrets = secrets2;
			Timestamps timestamps2;
			if (HasTimestamps())
			{
				Timestamps timestamps = new Timestamps();
				timestamps.Start = Timestamps.Start;
				timestamps2 = timestamps;
				timestamps.End = Timestamps.End;
			}
			else
			{
				timestamps2 = null;
			}
			richPresence.Timestamps = timestamps2;
			Assets assets2;
			if (HasAssets())
			{
				Assets assets = new Assets();
				assets.LargeImageKey = Assets.LargeImageKey != null ? Assets.LargeImageKey.Clone() as string : null;
				assets.LargeImageText = Assets.LargeImageText != null ? Assets.LargeImageText.Clone() as string : null;
				assets.SmallImageKey = Assets.SmallImageKey != null ? Assets.SmallImageKey.Clone() as string : null;
				assets2 = assets;
				assets.SmallImageText = Assets.SmallImageText != null ? Assets.SmallImageText.Clone() as string : null;
			}
			else
			{
				assets2 = null;
			}
			richPresence.Assets = assets2;
			Party party2;
			if (HasParty())
			{
				Party party = new Party();
				party.ID = Party.ID;
				party.Size = Party.Size;
				party.Max = Party.Max;
				party2 = party;
				party.Privacy = Party.Privacy;
			}
			else
			{
				party2 = null;
			}
			richPresence.Party = party2;
			return richPresence;
		}

		internal RichPresence Merge(BaseRichPresence presence)
		{
            _state = presence.State;
            _details = presence.Details;
            Party = presence.Party;
            Timestamps = presence.Timestamps;
            Secrets = presence.Secrets;
			if (presence.HasAssets())
			{
				if (!HasAssets())
				{
                    Assets = presence.Assets;
				}
				else
				{
                    Assets.Merge(presence.Assets);
				}
			}
			else
			{
                Assets = null;
			}
			return this;
		}

		internal override bool Matches(RichPresence other)
		{
			if (!base.Matches(other))
			{
				return false;
			}
			if (Buttons == null ^ other.Buttons == null)
			{
				return false;
			}
			if (Buttons != null)
			{
				if (Buttons.Length != other.Buttons.Length)
				{
					return false;
				}
				for (int i = 0; i < Buttons.Length; i++)
				{
					Button button = Buttons[i];
					Button button2 = other.Buttons[i];
					if (button.Label != button2.Label || button.Url != button2.Url)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static implicit operator bool(RichPresence presesnce)
		{
			return presesnce != null;
		}
	}
}
