/*
 * ii's Stupid Menu  Patches/Menu/FXPatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using HarmonyLib;
using System;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(FXSystem), "PlayFXForRig", new Type[] { typeof(FXType), typeof(IFXContext), typeof(PhotonMessageInfoWrapped) })]
    public class FXPatch
    {
        public static bool Prefix(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
        {
            NetPlayer player = info.Sender;
            if (player != null && iiMenu.Menu.Main.ShouldBypassChecks(player))
            {
                context.OnPlayFX();
                return false;
            }

            return true;
        }
    }
}
