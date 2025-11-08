using System;

namespace iiMenu.Managers.DiscordRPC.Message
{
	public abstract class IMessage
	{
		public abstract MessageType Type { get; }

		public DateTime TimeCreated
		{
			get
			{
				return _timecreated;
			}
		}

		public IMessage()
		{
            _timecreated = DateTime.Now;
		}

		private DateTime _timecreated;
	}
}
