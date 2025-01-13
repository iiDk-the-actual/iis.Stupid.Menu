﻿using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "UpdateFriendshipBracelet")]
    public class BraceletPatch
    {
        public static bool enabled = false;

        public static bool Prefix(VRRig __instance)
        {
            return !enabled;
        }
    }
}
