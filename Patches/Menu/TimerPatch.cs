/*
 * ii's Stupid Menu  Patches/Menu/TimerPatch.cs
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

using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class TimerPatch
    {
        public static bool enabled;

        // hello - kingofnetflix
        // this is basically a brainf*ck of code that "replaces" code in a method in real time
        // uses harmony's transpiler feature
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!enabled)
            {
                foreach (var instruction in instructions)
                    yield return instruction;
                yield break;
            }
            var codes = new List<CodeInstruction>(instructions);
            var abs = typeof(Mathf).GetMethod("Abs", new[] { typeof(float) });
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].Calls(abs))
                {
                    for (int j = i + 1; j < System.Math.Min(i + 10, codes.Count); j++)
                    {
                        // all of these are basically checking for any "comparisons" (greather than, less than, if true/false, etc)
                        // yes i know it's a lot
                        if (codes[j].opcode == OpCodes.Bgt ||
                            codes[j].opcode == OpCodes.Bgt_S ||
                            codes[j].opcode == OpCodes.Bge ||
                            codes[j].opcode == OpCodes.Bge_S ||
                            codes[j].opcode == OpCodes.Blt ||
                            codes[j].opcode == OpCodes.Blt_S ||
                            codes[j].opcode == OpCodes.Ble ||
                            codes[j].opcode == OpCodes.Ble_S ||
                            codes[j].opcode == OpCodes.Brtrue ||
                            codes[j].opcode == OpCodes.Brtrue_S ||
                            codes[j].opcode == OpCodes.Brfalse ||
                            codes[j].opcode == OpCodes.Brfalse_S) 
                        {
                            codes[j].opcode = OpCodes.Br; // basically changes the if statement to always skip that block of code it holds
                            break;
                        }
                    }
                }
                yield return codes[i];
            }  
        }
    }
}
