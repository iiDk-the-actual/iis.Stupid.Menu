/*
 * ii's Stupid Menu  Patches/Menu/CreatePatch.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(BuilderTableNetworking), "PieceCreatedByShelfRPC")]
    public class CreatePatch
    {
        public static bool enabled;
        public static int pieceTypeSearch;

        private static void Postfix(int pieceType, int pieceId)
        {
            if (enabled)
            {
                if (pieceTypeSearch == pieceType)
                {
                    Mods.Fun.pieceId = pieceId;
                    enabled = false;
                }
            }
        }
    }
}
