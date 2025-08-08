namespace iiMenu
{
    public class PluginInfo
    {
        public const string GUID = "org.iidk.gorillatag.iimenu";
        public const string Name = "ii's Stupid Menu";
        public const string Description = "Created by @goldentrophy with love <3";
        public const string BuildTimestamp = "2025-08-08T03:50:50Z";
        public const string Version = "6.8.1";

        public const string BaseDirectory = "iisStupidMenu";

#if DEBUG
        public static bool BetaBuild = true;
#else
        public static bool BetaBuild = false;
#endif
    }
}
