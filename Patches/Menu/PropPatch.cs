using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(PropHuntHandFollower), "GeoCollisionPoint")]
    public class PropPatch
    {
        public static bool enabled;

        public static void Postfix(ref Vector3 __result, Vector3 sourcePos, Vector3 targetPos)
        {
            if (enabled)
                __result = targetPos;
        }
    }
}
