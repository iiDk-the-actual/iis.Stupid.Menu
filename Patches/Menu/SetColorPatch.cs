/*
 * ii's Stupid Menu  Patches/Menu/SetColorPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using iiMenu.Managers;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "InitializeNoobMaterial")]
    public class InitializeNoobMaterial
    {
        public static bool Prefix(VRRig __instance, float red, float green, float blue, PhotonMessageInfoWrapped info)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if (player != null && iiMenu.Menu.Main.ShouldBypassChecks(player))
            {
                if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(__instance.rigSerializer.gameObject))
                    __instance.InitializeNoobMaterialLocal(red, green, blue);

                return false;
            }

            return true;
        }
    }
}
