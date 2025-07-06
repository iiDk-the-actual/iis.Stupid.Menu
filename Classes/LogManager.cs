namespace iiMenu.Classes
{
    public class LogManager
    {
        public static void Log(object log) =>
            Plugin.PluginLogger.LogInfo(log);

        public static void LogError(object log) =>
            Plugin.PluginLogger.LogError(log);

        public static void LogWarning(object log) =>
            Plugin.PluginLogger.LogDebug(log);
    }
}
