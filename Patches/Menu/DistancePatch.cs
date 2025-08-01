﻿using HarmonyLib;
using iiMenu.Classes;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "IsPositionInRange")]
    public class DistancePatch
    {
        public static bool enabled;

        public static void Postfix(VRRig __instance, ref bool __result, Vector3 position, float range)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if ((enabled && __instance.isLocal) || (player != null && Menu.Main.ShouldBypassChecks(player)))
                __result = true;
        }
    }
}
