namespace iiMenu
{
    public class PluginInfo
    {
        public const string GUID = "org.iidk.gorillatag.iimenu";
        public const string Name = "ii's Stupid Menu";
        public const string Description = "Created by @goldentrophy with love <3";
<<<<<<< HEAD
        public const string BuildTimestamp = "2025-07-16T01:40:38Z";
=======
        public const string BuildTimestamp = "2025-07-15T23:36:07Z";
>>>>>>> b8708785a1ac47f65b09efb4869718ebb0650b7e
        public const string Version = "6.7.0";

        public const string BaseDirectory = "iisStupidMenu";

#if DEBUG
        public static bool BetaBuild = true;
#else
        public static bool BetaBuild = false;
#endif
    }
}
