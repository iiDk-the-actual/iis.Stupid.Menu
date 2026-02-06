/*
 * ii's Stupid Menu  Extensions/VRRigExtensions.cs
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

using GorillaGameModes;
using iiMenu.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.GameModeUtilities;

namespace iiMenu.Extensions
{
    public static class VRRigExtensions
    {
        public static bool IsLocal(this VRRig rig) =>
            rig != null && (rig.isLocal || (GhostRig != null && rig == GhostRig));

        public static bool IsTagged(this VRRig rig)
        {
            if (rig == null) return false;
            List<NetPlayer> infectedPlayers = InfectedList();
            NetPlayer targetPlayer = rig.GetPlayer();

            return infectedPlayers.Contains(targetPlayer);
        }

        public static bool IsSteam(this VRRig rig) =>
            rig.GetPlatform() != "Standalone";

        public static bool IsKIDRestricted(this VRRig rig) =>
            !rig.IsMicEnabled && rig.GetName().ToLower().StartsWith("gorilla");

        public static string GetPlatform(this VRRig rig)
        {
            string concatStringOfCosmeticsAllowed = rig.rawCosmeticString;

            if (concatStringOfCosmeticsAllowed.Contains("S. FIRST LOGIN"))
                return "Steam";
            else if (concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") || rig.Creator.GetPlayerRef().CustomProperties.Count >= 2)
                return "PC";

            return "Standalone";
        }

        public static string GetCreationDate(this VRRig rig, Action<string> onTranslated = null, string format = "MMMM dd, yyyy h:mm tt") =>
            RigUtilities.GetCreationDate(rig.Creator.UserId, onTranslated, format);

        public static Color GetColor(this VRRig rig)
        {
            if (Buttons.GetIndex("Follow Player Colors").enabled)
                return rig.playerColor;

            if (rig.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
                return Color.green;

            switch (rig.setMatIndex)
            {
                case 1:
                    return Color.red;
                case 2:
                case 11:
                    return new Color32(255, 128, 0, 255);
                case 3:
                case 7:
                    return Color.blue;
                case 12:
                    return Color.green;
                default:
                    return rig.playerColor;
            }
        }

        public static bool Active(this VRRig rig) =>
            rig != null && GorillaParent.instance.vrrigs.Contains(rig);

        public static float Distance(this VRRig rig, Vector3 position) =>
            Vector3.Distance(rig.transform.position, position);

        public static float Distance(this VRRig rig, VRRig otherRig) =>
            rig.Distance(otherRig.transform.position);
        
        public static float Distance(this VRRig rig) =>
            rig.Distance(GorillaTagger.Instance.bodyCollider.transform.position);

        public static VRRig GetClosest(this VRRig rig) =>
            GorillaParent.instance.vrrigs.Where(targetRig => targetRig != null && targetRig != rig)
                                         .OrderBy(rig.Distance)
                                         .FirstOrDefault();

        public static int GetPing(this VRRig rig)
        {
            return playerPing.TryGetValue(rig, out int ping) ? ping : PhotonNetwork.GetPing();
        }

        public static int GetTruePing(this VRRig rig)
        {
            double ping = Math.Abs((rig.velocityHistoryList[0].time - PhotonNetwork.Time) * 1000);
            int safePing = (int)Math.Clamp(Math.Round(ping), 0, int.MaxValue);

            return safePing;
        }

        public static string GetName(this VRRig rig) =>
            RigUtilities.GetPlayerFromVRRig(rig)?.NickName ?? "null";

        public static NetPlayer GetPlayer(this VRRig rig) =>
            RigUtilities.GetPlayerFromVRRig(rig);

        public static Photon.Realtime.Player GetPhotonPlayer(this VRRig rig) =>
            RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(rig));

        public static ProjectileWeapon GetSlingshot(this VRRig rig) =>
            rig.projectileWeapon;

        public static float[] GetSpeed(this VRRig rig)
        {
            NetPlayer player = rig.GetPlayer();
            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    return tagManager.isCurrentlyTag
                        ? player == tagManager.currentIt
                            ? (new[]
                            {
                                tagManager.fastJumpLimit,
                                tagManager.fastJumpMultiplier
                            })
                            : (new[]
                        {
                            tagManager.slowJumpLimit,
                            tagManager.slowJumpMultiplier
                        })
                        : tagManager.currentInfected.Contains(player)
                            ? (new[]
                            {
                                tagManager.InterpolatedInfectedJumpSpeed(tagManager.currentInfected.Count),
                                tagManager.InterpolatedInfectedJumpMultiplier(tagManager.currentInfected.Count)
                            })
                            : (new[]
                        {
                            tagManager.InterpolatedNoobJumpSpeed(tagManager.currentInfected.Count),
                            tagManager.InterpolatedNoobJumpMultiplier(tagManager.currentInfected.Count)
                        });
                default:
                    return new[] { 6.5f, 1.1f };
            }
        }

        public static float GetMaxSpeed(this VRRig rig) =>
            rig.GetSpeed()[0];

        public static float GetSpeedMultiplier(this VRRig rig) =>
            rig.GetSpeed()[1];
    }
}
