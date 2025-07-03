using GorillaLocomotion.Gameplay;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(SinglePool), "Initialize")]
    public class PoolPatch
    {
        public static void Prefix(SinglePool __instance, GameObject gameObject_) =>
            __instance.initAmountToPool *= 4;
    }
}
