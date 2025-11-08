using iiMenu.Managers.DiscordRPC.Converters;
using System;

namespace iiMenu.Managers.DiscordRPC.RPC.Payload
{
	internal enum Command
	{
		[EnumValue("DISPATCH")]
		Dispatch,
		[EnumValue("SET_ACTIVITY")]
		SetActivity,
		[EnumValue("SUBSCRIBE")]
		Subscribe,
		[EnumValue("UNSUBSCRIBE")]
		Unsubscribe,
		[EnumValue("SEND_ACTIVITY_JOIN_INVITE")]
		SendActivityJoinInvite,
		[EnumValue("CLOSE_ACTIVITY_JOIN_REQUEST")]
		CloseActivityJoinRequest,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		Authorize,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		Authenticate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetGuild,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetGuilds,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetChannel,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetChannels,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SetUserVoiceSettings,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SelectVoiceChannel,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetSelectedVoiceChannel,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SelectTextChannel,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GetVoiceSettings,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SetVoiceSettings,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		CaptureShortcut
	}
}
