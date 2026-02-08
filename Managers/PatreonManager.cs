/*
 * ii's Stupid Menu  Managers/PatreonManager.cs
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
using iiMenu.Extensions;
using iiMenu.Menu;
using iiMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RigUtilities;

namespace iiMenu.Managers
{
    public class PatreonManager : MonoBehaviour
    {
        public static PatreonManager instance;

        public void Awake() =>
            instance = this;

        public readonly Dictionary<string, PatreonMembership> PatreonMembers = new Dictionary<string, PatreonMembership>();
        public readonly struct PatreonMembership
        {
            public readonly string TierName;
            public readonly string IconURL;

            public PatreonMembership(string tierName, string iconURL)
            {
                TierName = tierName;
                IconURL = iconURL;
            }
        }

        private Material iconMaterial;
        private readonly Dictionary<VRRig, GameObject> iconPool = new Dictionary<VRRig, GameObject>();
        private static readonly List<Player> excludedIndicators = new List<Player>();

        public static KeyValuePair<NetPlayer, PatreonMembership>[] GetAllMembersInRoom()
        {
            return !NetworkSystem.Instance.InRoom
                ? Array.Empty<KeyValuePair<NetPlayer, PatreonMembership>>()
                : NetworkSystem.Instance.PlayerListOthers
                .Where(player => instance.PatreonMembers.ContainsKey(player.UserId))
                .Select(player => new KeyValuePair<NetPlayer, PatreonMembership>(player, instance.PatreonMembers[player.UserId]))
                .ToArray();
        }

        public static bool IsPlayerPatreonMember(NetPlayer player) =>
            instance.PatreonMembers.ContainsKey(player.UserId);

        public static Color GetTierColor(string color) // Hard coded slop my beloved
        {
            return color switch
            {
                "Donor" => new Color32(196, 201, 200, 255),
                "Supporter" => new Color32(241, 196, 15, 255),
                "Basic Tracker" => new Color32(189, 221, 244, 255),
                "Ultimate Tracker" => new Color32(170, 184, 194, 255),

                "Owner" => new Color32(108, 190, 127, 255),
                "Co-Owner" => new Color32(73, 143, 214, 255),
                "Console Owner" => new Color32(189, 96, 231, 255),
                "Menu Developer" => new Color32(212, 132, 61, 255),
                "Admin" => new Color32(255, 110, 118, 255),
                "Staff Manager" => new Color32(102, 241, 180, 255),
                "Moderator" => new Color32(88, 101, 242, 255),
                "Community Helper" => new Color32(253, 215, 101, 255),

                "Boyfriend" => new Color32(244, 171, 186, 255),

                _ => Color.white,
            };
        }

        public static bool IndicatorsEnabled = true;
        public void Update()
        {
            List<VRRig> toRemoveRigs = new List<VRRig>();

            foreach (var indicator in iconPool.Where(indicator => !IndicatorsEnabled || !indicator.Key.Active() || !IsPlayerPatreonMember(GetPlayerFromVRRig(indicator.Key)) || excludedIndicators.Contains(indicator.Key.GetPhotonPlayer())))
            {
                toRemoveRigs.Add(indicator.Key);
                Destroy(indicator.Value);
            }

            foreach (VRRig rig in toRemoveRigs)
                iconPool.Remove(rig);

            if (!IndicatorsEnabled) return;

            if (!NetworkSystem.Instance.InRoom) return;
            var members = GetAllMembersInRoom();
            foreach (var member in members)
            {
                VRRig playerRig = GetVRRigFromPlayer(member.Key);
                if (playerRig == null) continue;
                if (excludedIndicators.Contains(member.Key.GetPlayer())) continue;

                if (!iconPool.TryGetValue(playerRig, out GameObject playerIndicator))
                {
                    playerIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(playerIndicator.GetComponent<Collider>());

                    if (iconMaterial == null)
                    {
                        iconMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                        iconMaterial.SetFloat("_Surface", 1);
                        iconMaterial.SetFloat("_Blend", 0);
                        iconMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        iconMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        iconMaterial.SetFloat("_ZWrite", 0);
                        iconMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        iconMaterial.renderQueue = (int)RenderQueue.Transparent;
                    }

                    playerIndicator.GetComponent<Renderer>().material = iconMaterial;
                    playerIndicator.GetComponent<Renderer>().material.mainTexture = LoadTextureFromURL(member.Value.IconURL, $"Images/Patreon/{member.Key.UserId}.{FileUtilities.GetFileExtension(member.Value.IconURL)}");
                    playerIndicator.GetComponent<Renderer>().material.color = Color.white;

                    GameObject go = new GameObject("iiMenu_Nametag");
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
                    textMesh.fontSize = 4.8f;
                    textMesh.alignment = TextAlignmentOptions.Center;

                    textMesh.SafeSetText(member.Value.TierName);
                    textMesh.SafeSetFontStyle(Main.activeFontStyle);
                    textMesh.SafeSetFont(Main.activeFont);
                    textMesh.color = GetTierColor(member.Value.TierName);
                    textMesh.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    textMesh.transform.SetParent(playerIndicator.transform, false);

                    iconPool.Add(playerRig, playerIndicator);
                }

                float distance = Classes.Menu.Console.GetIndicatorDistance(playerRig);
                playerIndicator.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * playerRig.scaleFactor;
                playerIndicator.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * (distance * playerRig.scaleFactor);
                playerIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);

                GameObject nameTag = playerIndicator.transform.Find("iiMenu_Nametag").gameObject;
                nameTag.transform.position = playerRig.headMesh.transform.position + playerRig.headMesh.transform.up * ((distance + 0.25f) * playerRig.scaleFactor);
                nameTag.transform.LookAt(Camera.main.transform.position);
                nameTag.transform.Rotate(0f, 180f, 0f);
            }
        }

        public const byte PatreonByte = 63;
        public static void EventReceived(EventData data)
        {
            try
            {
                NetPlayer sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);
                if (data.Code != PatreonByte || !IsPlayerPatreonMember(sender)) return;
                VRRig senderRig = GetVRRigFromPlayer(sender);
                object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                string command = args.Length > 0 ? (string)args[0] : "";

                switch (command)
                {
                    case "indicator":
                        {
                            if (args[0] is bool enabled)
                            {
                                if (enabled)
                                    if (!excludedIndicators.Contains(sender.GetPlayer()))
                                        excludedIndicators.Add(sender.GetPlayer());
                                else
                                    if (excludedIndicators.Contains(sender.GetPlayer()))
                                        excludedIndicators.Remove(sender.GetPlayer());
                            }
                            break;
                        }
                }
            }
            catch { }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!NetworkSystem.Instance.InRoom)
                return;

            PhotonNetwork.RaiseEvent(PatreonByte,
                new object[] { command }
                    .Concat(parameters)
                    .ToArray(),
            options, SendOptions.SendReliable);
        }

        public static void ExecuteCommand(string command, int[] targets, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = targets }, parameters);

        public static void ExecuteCommand(string command, int target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = new[] { target } }, parameters);

        public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { Receivers = target }, parameters);


        #region Patreon Mods
        public static void SetupPatreonMods(string patreonName)
        {
            NotificationManager.SendNotification($"<color=grey>[</color><color=purple>PATREON</color><color=grey>]</color> Welcome, {patreonName}! Patreon mods have been enabled.", 10000);

            List<ButtonInfo> buttons = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
            buttons.Add(new ButtonInfo { buttonText = "Patreon Mods", method = () => Buttons.CurrentCategoryName = "Patreon Mods", isTogglable = false, toolTip = "Opens the patreon mods." });
            Buttons.buttons[Buttons.GetCategory("Main")] = buttons.ToArray();

            if (Main.dynamicSounds)
                LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/patreon.ogg", "Audio/Menu/patreon.ogg").Play(Main.buttonClickVolume / 10f);
        }

        public static void ShowIndicator(bool enabled) =>
            ExecuteCommand("indicator", ReceiverGroup.All, enabled);

        private static int lastPlayerCount;
        public static void ConstantHideIndicator()
        {
            if (!PhotonNetwork.InRoom)
                lastPlayerCount = -1;

            if (PhotonNetwork.PlayerList.Length != lastPlayerCount && PhotonNetwork.InRoom)
            {
                ShowIndicator(false);
                lastPlayerCount = PhotonNetwork.PlayerList.Length;
            }
        }
        #endregion
    }
}
