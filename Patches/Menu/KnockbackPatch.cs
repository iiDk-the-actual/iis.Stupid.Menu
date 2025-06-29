using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "ApplyKnockback")]
    public class KnockbackPatch
    {
        public static bool enabled = false;

        public static bool Prefix(Vector3 direction, float speed) =>
            !enabled;
    }
}
