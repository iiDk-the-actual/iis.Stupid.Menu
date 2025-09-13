using ExitGames.Client.Photon;
using GorillaLocomotion;
using HarmonyLib;
using Photon.Realtime;
using System.Linq;

namespace iiMenu.Patches
{
    // Gizmo I didn't steal this idea from you
    [HarmonyPatch(typeof(Player), "SetCustomProperties")]
    public class PropertiesPatch
    {
        public static bool enabled;

        public static bool Prefix(Player __instance, ref Hashtable propertiesToSet)
        {
            if (__instance.IsLocal && enabled)
            {
                if (propertiesToSet.Any(prop => prop.Key.ToString() != "didTutorial"))
                    return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Player), "set_CustomProperties")]
    public class PropertiesPatch2
    {
        public static bool Prefix(Player __instance, ref Hashtable value)
        {
            if (__instance.IsLocal && PropertiesPatch.enabled)
            {
                if (value.Any(prop => prop.Key.ToString() != "didTutorial"))
                    return false;
            }

            return true;
        }
    }
}