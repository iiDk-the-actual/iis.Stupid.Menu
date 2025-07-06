using HarmonyLib;
using System.Reflection;

namespace iiMenu.Patches
{
    [HarmonyPatch]
    public class GroupPatch
    {
        public static bool enabled;

        static MethodBase TargetMethod()
        {
            var type = typeof(VRRig).Assembly.GetType("RoomSystem");
            return type?.GetMethod("SearchForNearby", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        }

        public static bool Prefix() =>
            !enabled;
    }
}
