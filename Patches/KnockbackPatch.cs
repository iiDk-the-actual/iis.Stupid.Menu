using HarmonyLib;
using UnityEngine;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "ApplyKnockback")]
    public class KnockbackPatch
    {
        public static bool enabled = false;

        public static bool Prefix(Vector3 direction, float speed)
        {
            if (enabled)
            {
                return false;
            }
            return true;
        }
    }
}
