/*
 * ii's Stupid Menu  Patches/Menu/RequestPatch.cs
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

using GorillaNetworking;
using HarmonyLib;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Mods;
using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.RequestCosmetics))]
    public class RequestPatch
    {
        public static bool enabled;
        public static bool bypassCosmeticCheck;
        public static Coroutine currentCoroutine;

        public static bool Prefix(VRRig __instance, PhotonMessageInfoWrapped info)
        {
            if (__instance.netView.IsMine && __instance.isLocal)
            {
                if (CosmeticsController.hasInstance)
                {
                    if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", info.Sender);
                        return false;
                    }

                    if (enabled)
                    {
                        currentCoroutine ??= CoroutineManager.instance.StartCoroutine(LoadCosmetics());
                        return false;
                    }

                    if (bypassCosmeticCheck)
                    {
                        CosmeticsController.CosmeticSet items = new CosmeticsController.CosmeticSet
                        (
                            CosmeticsController.instance.currentWornSet.ToDisplayNameArray().Select(
                                cosmetic => Main.CosmeticsOwned.Contains(cosmetic)
                                    ? cosmetic
                                    : "null"
                                )
                            .ToArray(),
                            CosmeticsController.instance
                        );

                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", NetworkSystem.Instance.GetPlayer(info.senderID), items.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
                        return false;
                    }
                }
            }
            return true;
        }

        private static string[] archiveCosmetics;
        public static IEnumerator LoadCosmetics()
        {
            if (PhotonNetwork.InRoom)
            {
                Vector3 target = Main.TryOnRoom.transform.position;

                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = target;

                string[] cosmeticArray = { "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU." };

                archiveCosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(cosmeticArray, CosmeticsController.instance);

                while (Vector3.Distance(Main.ServerPos, target) > 0.2f)
                    yield return null;
                
                yield return new WaitForSeconds(0.1f);

                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, Fun.PackCosmetics(cosmeticArray), CosmeticsController.instance.currentWornSet.ToPackedIDArray(), false);
                VRRig.LocalRig.enabled = true;
                yield return new WaitForSeconds(0.5f);

                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
                VRRig.LocalRig.LocalUpdateCosmeticsWithTryon(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.tryOnSet, false);

                float delay = Time.time + 30f;
                while (Time.time < delay || PhotonNetwork.InRoom)
                    yield return null;
                
                currentCoroutine = null;
            }
        }
    }
}
