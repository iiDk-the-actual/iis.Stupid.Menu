/*
 * ii's Stupid Menu  Patches/Menu/AntiCrashPatches.cs
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

﻿using ExitGames.Client.Photon;
using GorillaExtensions;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class AntiCrashPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
        {
            if (enabled && __instance.isLocal && !GTExt.IsValid(throwVelocity))
                return false;
            
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
    public class AntiCrashPatch2
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashPatch.enabled && __instance.isLocal)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestMaterialColor")]
    public class AntiCrashPatch3
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashPatch.enabled && __instance.isLocal)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DeployedChild), "Deploy")]
    public class AntiCrashPatch4
    {
        public static void Postfix(DeployedChild __instance, DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
        {
            if (AntiCrashPatch.enabled)
                __instance._rigidbody.linearVelocity = __instance._rigidbody.linearVelocity.ClampMagnitudeSafe(100f);
        }
    }

    [HarmonyPatch(typeof(LuauVm), "OnEvent")]
    public class AntiCrashPatch5
    {
        public static bool Prefix(EventData eventData)
        {
            if (AntiCrashPatch.enabled)
            {
                if (eventData.Code != 180) return false;

                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(eventData.Sender, false);

                object[] args = eventData.CustomData == null ? new object[] { } : (object[])eventData.CustomData;
                string command = args.Length > 0 ? (string)args[0] : "";

                if (sender != PhotonNetwork.LocalPlayer && args[1] is double v && v == PhotonNetwork.LocalPlayer.ActorNumber && command == "leaveGame")
                    return false;
            }

            return true;
        }
    }
}
