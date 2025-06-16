using HarmonyLib;
using System.Reflection;

namespace iiMenu.Patches
{
    public class Menu
    {
        public static bool IsPatched { get; private set; }

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                instance ??= new Harmony(PluginInfo.GUID);
                
                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                instance.UnpatchSelf();
                IsPatched = false;
                instance = null;
            }
        }

        private static Harmony instance;
        public const string InstanceId = PluginInfo.GUID;
    }
}
