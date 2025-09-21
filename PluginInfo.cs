namespace iiMenu
{
    public class PluginInfo
    {
        public const string GUID = "org.iidk.gorillatag.iimenu";
        public const string Name = "ii's Stupid Menu";
        public const string Description = "Created by @crimsoncauldron with love <3";
        public const string BuildTimestamp = "2025-09-21T15:59:38Z";
        public const string Version = "7.1.0";

        public const string BaseDirectory = "iisStupidMenu";
        public const string ResourceURL = "https://github.com/iiDk-the-actual/ModInfo/raw/main";

#if DEBUG
        public static bool BetaBuild = true;
#else
        public static bool BetaBuild = false;
#endif
    }
}
