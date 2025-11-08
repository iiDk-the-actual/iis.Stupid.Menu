namespace iiMenu.Managers.DiscordRPC.Message
{
	public enum ErrorCode
	{
		Success,
		PipeException,
		ReadCorrupt,
		NotImplemented = 10,
		UnkownError = 1000,
		InvalidPayload = 4000,
		InvalidCommand = 4002,
		InvalidEvent = 4004
	}
}
