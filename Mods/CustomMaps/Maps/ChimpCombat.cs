/*
 * ii's Stupid Menu  Mods/CustomMaps/Maps/ChimpCombat.cs
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

using ExitGames.Client.Photon;
using iiMenu.Classes.Menu;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Managers.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.CustomMaps.Manager;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public class ChimpCombat : CustomMap
    {
        public override long MapID => 5135423;
        public override ButtonInfo[] Buttons => new[]
        {
            new ButtonInfo { buttonText = "Kill All Players", method = KillAll, toolTip = "Kills everyone in the room."},
            new ButtonInfo { buttonText = "God Mode_", overlapText = "God Mode", enableMethod = GodMode, disableMethod = DisableGodMode, toolTip = "Prevents you from getting killed."},
            new ButtonInfo { buttonText = "No Grenade Cooldown", enableMethod = NoGrenadeCooldown, disableMethod = DisableNoGrenadeCooldown, toolTip = "Disables the cooldown on spawning grenades."},
            new ButtonInfo { buttonText = "No Shoot Cooldown", enableMethod = NoShootCooldown, disableMethod = DisableNoShootCooldown, toolTip = "Disables the cooldown on shooting."},
            new ButtonInfo { buttonText = "Rapid Fire", enableMethod = RapidFire, disableMethod = DisableRapidFire, toolTip = "Automatically shoots when holding down right trigger."},
            new ButtonInfo { buttonText = "Instant Kill", enableMethod = InstantKill, disableMethod = DisableInstantKill, toolTip = "Makes your gun instant kill players."},
            new ButtonInfo { buttonText = "Infinite Points", enableMethod = InfinitePoints, disableMethod = DisableInfinitePoints, toolTip = "Gives you an infinite amount of points."},
            new ButtonInfo { buttonText = "Infinite Ammo", enableMethod = InfiniteAmmo, disableMethod = DisableInfiniteAmmo, toolTip = "Gives you an infinite amount of ammo."},
            new ButtonInfo { buttonText = "Crash Gun <color=grey>[</color><color=purple>Crash</color><color=grey>]</color>", method = VSCrashGun, toolTip = "Crashes whoever your hand desires in the custom map." },
            new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=purple>Crash</color><color=grey>]</color>", method = VSCrashAll, isTogglable = false, toolTip = "Crashes everyone in the custom map." },
            new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method = AntiReportCrash, toolTip = "Crashes everyone who tries to report you." },
            new ButtonInfo { buttonText = "Crash On Touch <color=grey>[</color><color=purple>Crash</color><color=grey>]</color>", method = VSCrashOnTouch, toolTip = "Crashes whoever you touch in the custom map." }
        };

        public static void KillAll()
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "HitPlayer", (double)GetRandomPlayer(false).ActorNumber, (double)99999, (double)GetRandomPlayer(false).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            RPCProtection();
        }

        public static void GodMode()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 957, "if not IsMe then PlayerData[Player.playerID].Health -= Modules.roundToQuarter(dmg) end" }
            });
        }

        public static void DisableGodMode() =>
            RevertCustomScript(957);

        public static void NoGrenadeCooldown()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1296, "grenadeCooldown = 0" }
            });
        }

        public static void DisableNoGrenadeCooldown() =>
            RevertCustomScript(1296);

        public static void NoShootCooldown()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1243, "shootCooldown = 0" }
            });
        }

        public static void DisableNoShootCooldown() =>
            RevertCustomScript(1243);

        public static void InfiniteAmmo()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1244, "" }
            });
        }

        public static void DisableInfiniteAmmo() =>
            RevertCustomScript(1244);

        public static void InstantKill()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1278, "emitAndOnEvent(\"HitPlayer\", {found.playerID, 99999.0, LocalPlayer.playerID})" }
            });
        }

        public static void DisableInstantKill() =>
            RevertCustomScript(1278);

        public static void InfinitePoints()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 496, "saveData[\"Points\"] = 999999" }
            });
        }

        public static void DisableInfinitePoints() =>
            RevertCustomScript(496);

        public static void RapidFire()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 2041, "needsLetGoR = false" }
            });
        }

        public static void DisableRapidFire() =>
            RevertCustomScript(2041);

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
        public static void VSCrashGun()
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
                    if (gunTarget && !PlayerIsLocal(gunTarget))
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
        public static void VSCrashOnTouch()
        {
            if (Time.time < crashDelay)
                return;
            foreach (var netPlayer in from rig in GorillaParent.instance.vrrigs where !rig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f) select GetPlayerFromVRRig(rig))
            {
                CrashPlayer(netPlayer.ActorNumber);
                crashDelay = Time.time + 0.2f;
            }
        }
        public static void VSCrashAll()
        {
            if (!(Time.time > crashDelay)) return;
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
                CrashPlayer(player.ActorNumber);
                
            crashDelay = Time.time + 0.1f;
        }
        public static void AntiReportCrash()
        {
            Safety.AntiReport((vrrig, position) =>
            {

                if (Time.time > crashDelay)
                {
                    NetPlayer netPlayer = GetPlayerFromVRRig(vrrig);
                    CrashPlayer(netPlayer.ActorNumber);
                    crashDelay = Time.time + 0.5f;
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been crashed.");
                }
            });
        }
    }
}
