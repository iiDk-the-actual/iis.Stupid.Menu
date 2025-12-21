/*
 * ii's Stupid Menu  Managers/PatreonManager.cs
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

using iiMenu.Extensions;
using iiMenu.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RigUtilities;

namespace iiMenu.Managers
{
    public class PatreonManager : MonoBehaviour
    {
        #region Patreon Manager Code
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

        public static KeyValuePair<NetPlayer, PatreonMembership>[] GetAllMembersInRoom()
        {
            if (!NetworkSystem.Instance.InRoom)
                return Array.Empty<KeyValuePair<NetPlayer, PatreonMembership>>();

            return NetworkSystem.Instance.PlayerListOthers
                .Where(player => instance.PatreonMembers.ContainsKey(player.UserId))
                .Select(player => new KeyValuePair<NetPlayer, PatreonMembership>(player, instance.PatreonMembers[player.UserId]))
                .ToArray();
        }

        public static bool IsPlayerPatreonMember(NetPlayer player) =>
            instance.PatreonMembers.ContainsKey(player.UserId);

        public static Color GetTierColor(string color)
        {
            return color switch
            {
                "Donor" => new Color32(196, 201, 200, 255),
                "Supporter" => new Color32(241, 196, 15, 255),
                "Basic Tracker" => new Color32(189, 221, 244, 255),
                "Ultimate Tracker" => new Color32(170, 184, 194, 255),
                _ => Color.white,
            };
        }

        public void Update()
        {
            List<VRRig> toRemoveRigs = new List<VRRig>();

            foreach (var indicator in iconPool.Where(indicator => !indicator.Key.Active() || !IsPlayerPatreonMember(GetPlayerFromVRRig(indicator.Key))))
            {
                toRemoveRigs.Add(indicator.Key);
                Destroy(indicator.Value);
            }

            foreach (VRRig rig in toRemoveRigs)
                iconPool.Remove(rig);

            if (NetworkSystem.Instance.InRoom)
            {
                var members = GetAllMembersInRoom();
                foreach (var member in members)
                {
                    VRRig playerRig = GetVRRigFromPlayer(member.Key);
                    if (playerRig != null)
                    {
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
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            textMesh.text = member.Value.TierName;
                            textMesh.color = GetTierColor(member.Value.TierName);
                            textMesh.fontStyle = FontStyle.Bold;
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
            }
        }
        #endregion
    }
}
