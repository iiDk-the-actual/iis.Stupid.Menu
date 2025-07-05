﻿using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(RoomSystem), "SearchForNearby")]
    public class GroupPatch
    {
        public static bool enabled;
        public static bool Prefix() =>
            !enabled;
    }
}
