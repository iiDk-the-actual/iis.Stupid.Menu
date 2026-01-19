/*
 * ii's Stupid Menu  Mods/CustomMaps/Maps/MonkeMagic.cs
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

using ExitGames.Client.Photon;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Extensions.VRRigExtensions;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RigUtilities;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public class MonkeMagic : CustomMap
    {
        public override long MapID => 5107228;
        public override ButtonInfo[] Buttons => new[]
        {
            new ButtonInfo { buttonText = "Lightning Strike Self", method = LightningStrikeSelf, toolTip = "Strikes yourself with lightning."},
            new ButtonInfo { buttonText = "Lightning Strike Gun", method = LightningStrikeGun, toolTip = "Strikes whoever your hand desires with lightning."},
            new ButtonInfo { buttonText = "Lightning Strike All", method = LightningStrikeAll, toolTip = "Strikes everyone in the room with lightning."},

            new ButtonInfo { buttonText = "Change Material Self", method = ChangeMaterialSelf, toolTip = "Changes your material."},
            new ButtonInfo { buttonText = "Change Material Gun", method = ChangeMaterialGun, toolTip = "Changes the material of whoever your hand desires."},
            new ButtonInfo { buttonText = "Change Material All", method = ChangeMaterialAll, toolTip = "Changes the material of everyone in the room."},

            new ButtonInfo { buttonText = "Spawn Lucy Self", isTogglable = false, method = SpawnLucySelf, toolTip = "Spawns lucy on yourself."},
            new ButtonInfo { buttonText = "Spawn Lucy Gun", method = SpawnLucyGun, toolTip = "Spawns lucy on whoever your hand desires."},
            new ButtonInfo { buttonText = "Spawn Lucy All", isTogglable = false, method = SpawnLucyAll, toolTip = "Spawns lucy on everyone in the room."},

            new ButtonInfo { buttonText = "Monke Magic Crash Gun", overlapText = "Crash Gun", method = CrashGun, toolTip = "Crashes whoever your hand desires in the custom map." },
            new ButtonInfo { buttonText = "Monke Magic Crash All", overlapText = "Crash All", method = CrashAll, isTogglable = false, toolTip = "Crashes everyone in the custom map." },
            new ButtonInfo { buttonText = "Monke Magic Anti Report", overlapText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method = AntiReportCrash, toolTip = "Crashes everyone who tries to report you." },
            new ButtonInfo { buttonText = "Monke Magic Crash Aura", overlapText = "Crash Aura", method = CrashAura, toolTip = "Crashes players nearby you in the custom map." },
            new ButtonInfo { buttonText = "Monke Magic Crash On Touch", overlapText = "Crash On Touch", method = CrashOnTouch, toolTip = "Crashes whoever you touch in the custom map." },
            new ButtonInfo { buttonText = "Monke Magic Crash When Touched", overlapText = "Crash When Touched", method = CrashWhenTouched, toolTip = "Crashes whoever touches you in the custom map." }
        };
        
        private static float lightningDelay;
        public static void LightningStrikeSelf()
        {
            if (!(Time.time > lightningDelay)) return;
            lightningDelay = Time.time + 0.2f;
            PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)PhotonNetwork.LocalPlayer.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            RPCProtection();
        }

        public static void LightningStrikeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null && Time.time > lightningDelay)
                {
                    lightningDelay = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)GetPlayerFromVRRig(lockTarget).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    RPCProtection();
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void LightningStrikeAll()
        {
            if (Time.time > lightningDelay)
            {
                lightningDelay = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)GetRandomPlayer(false).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        private static float materialDelay;
        public static void ChangeMaterialSelf()
        {
            if (Time.time > materialDelay)
            {
                materialDelay = Time.time + 0.1f;
                PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)PhotonNetwork.LocalPlayer.ActorNumber, (double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        public static void ChangeMaterialGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null && Time.time > materialDelay)
                {
                    materialDelay = Time.time + 0.1f;
                    PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)PhotonNetwork.LocalPlayer.ActorNumber, (double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    RPCProtection();
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void ChangeMaterialAll()
        {
            if (Time.time > materialDelay)
            {
                materialDelay = Time.time + 0.2f;
                PhotonNetwork.RaiseEvent(180, new object[] { "ChangingMaterial", (double)GetPlayerFromVRRig(lockTarget).ActorNumber, (double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length) }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        private static float lucyDelay;
        public static void SpawnLucySelf()
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)PhotonNetwork.LocalPlayer.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            RPCProtection();
        }

        public static void SpawnLucyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > lucyDelay)
                    {
                        lucyDelay = Time.time + 0.2f;
                        PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)GetPlayerFromVRRig(lockTarget).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        RPCProtection();
                    }
                }
            }
        }

        public static void SpawnLucyAll()
        {
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
            {
                PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)player.ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                RPCProtection();
            }
        }

        // I don't know who made this
        public static float crashDelay;
        public static void CrashPlayer(int ActorNumber)
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "leaveGame", (double)ActorNumber, false, (double)ActorNumber }, new RaiseEventOptions
            {
                TargetActors = new[]
                {
                    ActorNumber
                }
            }, SendOptions.SendReliable);
            RPCProtection();
        }
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null && Time.time > crashDelay)
                {
                    NetPlayer Player = GetPlayerFromVRRig(lockTarget);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = Time.time + 0.2f;
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }
        public static void CrashAura()
        {
            if (Time.time < crashDelay)
                return;

            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                {
                    CrashPlayer(nearbyPlayer.OwningNetPlayer.ActorNumber);
                    crashDelay = Time.time + 0.2f;
                }
            }
        }
        public static void CrashOnTouch()
        {
            if (Time.time < crashDelay)
                return;
            foreach (var Player in from rig in GorillaParent.instance.vrrigs where !rig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f) select GetPlayerFromVRRig(rig))
            {
                CrashPlayer(Player.ActorNumber);
                crashDelay = Time.time + 0.2f;
            }
        }
        public static void CrashWhenTouched()
        {
            if (Time.time < crashDelay)
                return;
            foreach (var Player in from vrrig in GorillaParent.instance.vrrigs where !vrrig.isMyPlayer && !vrrig.isOfflineVRRig && (Vector3.Distance(vrrig.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5 || Vector3.Distance(vrrig.leftHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5 || Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5) select GetPlayerFromVRRig(vrrig))
            {
                CrashPlayer(Player.ActorNumber);
                crashDelay = Time.time + 0.2f;
            }
        }
        public static void CrashAll()
        {
            if (Time.time > crashDelay)
            {
                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    CrashPlayer(Player.ActorNumber);
                }
                crashDelay = Time.time + 0.1f;
            }
        }
        public static void AntiReportCrash()
        {
            Safety.AntiReport((vrrig, position) =>
            {

                if (Time.time > crashDelay)
                {
                    NetPlayer Player = GetPlayerFromVRRig(vrrig);
                    CrashPlayer(Player.ActorNumber);
                    crashDelay = Time.time + 0.5f;
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been crashed.");
                }
            });
        }
    }
}
