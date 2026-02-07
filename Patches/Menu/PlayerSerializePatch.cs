/*
 * ii's Stupid Menu  Patches/Menu/PlayerSerializePatch.cs
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

using HarmonyLib;
using iiMenu.Managers;
using System;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.SerializeReadShared))]
    public class PlayerSerializePatch
    {
        public static bool stopSerialization;
        public static float? delay;

        public static event Action<VRRig> OnPlayerSerialize;
        public static bool Prefix(VRRig __instance, InputStruct data)
        {
            if (stopSerialization)
                return false;

            if (delay != null)
            {
                CoroutineManager.instance.StartCoroutine(
                    iiMenu.Menu.Main.SerializationDelay(() =>
                    {
                        float oldDelay = delay.Value;
                        delay = null;
                        try
                        {
                            __instance.SerializeReadShared(data);
                        } catch { }
                        delay = oldDelay;
                    }, delay.Value)
                );

                return false;
            }

            return true;
        }

        public static void Postfix(VRRig __instance, InputStruct data) =>
            OnPlayerSerialize?.Invoke(__instance);
    }
}
