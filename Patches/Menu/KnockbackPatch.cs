using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "ApplyKnockback")]
    public class KnockbackPatch
    {
        public static bool enabled;

        public static bool Prefix(Vector3 direction, float speed) =>
            !enabled;
    }
}
