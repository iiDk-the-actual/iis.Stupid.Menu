/*
 * ii's Stupid Menu  Patches/Menu/LimitPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

﻿using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "SnowballThrowEventReceiver")]
    public class LimitPatch
    {
        public static bool Prefix(GrowingSnowballThrowable __instance, int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
        {
            NetPlayer player = info.Sender;
            if (player != null && iiMenu.Menu.Main.ShouldBypassChecks(player))
            {
                object obj = args[0];
                if (obj is Vector3)
                {
                    Vector3 vector = (Vector3)obj;
                    obj = args[1];
                    if (obj is Vector3)
                    {
                        Vector3 vector2 = (Vector3)obj;
                        obj = args[2];
                        if (obj is int)
                            __instance.LaunchSnowballRemote(vector, vector2, __instance.snowballModelTransform.lossyScale.x, (int)obj, info);
                    }
                }
                return false;
            }

            return true;
        }
    }
}
