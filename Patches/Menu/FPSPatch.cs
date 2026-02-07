/*
 * ii's Stupid Menu  Patches/Menu/FPSPatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
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
using UnityEngine.XR.Interaction.Toolkit;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.PackCompetitiveData))]
    public class FPSPatch
    {
        public static bool enabled;
        public static int spoofFPSValue;

        public static void Postfix(ref short __result)
        {
            if (enabled)
            {
                GorillaSnapTurn GorillaSnapTurningComp = VRRig.LocalRig.GorillaSnapTurningComp;

                int turnTypeInt = 0;
                if (GorillaSnapTurningComp != null)
                {
                    VRRig.LocalRig.turnFactor = GorillaSnapTurningComp.turnFactor;
                    VRRig.LocalRig.turnType = GorillaSnapTurningComp.turnType;

                    if (!(GorillaSnapTurningComp.turnType == "SNAP"))
                    {
                        if (GorillaSnapTurningComp.turnType == "SMOOTH")
                            turnTypeInt = 2;
                    }
                    else
                        turnTypeInt = 1;
                    
                    turnTypeInt *= 10;
                    turnTypeInt += GorillaSnapTurningComp.turnFactor;
                }
                __result = (short)(spoofFPSValue + (turnTypeInt << 8));
            }
        }
    }
}
