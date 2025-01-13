﻿using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Mods;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
    public class RequestPatch
    {
        public static bool enabled = false;
        public static Coroutine currentCoroutine = null;

        public static bool Prefix(VRRig __instance, PhotonMessageInfoWrapped info)
        {
            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                if (enabled)
                {
                    if (CosmeticsController.hasInstance)
                    {
                        if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
                        else
                            if (currentCoroutine == null)
                                currentCoroutine = CoroutineManager.RunCoroutine(LoadCosmetics());
                    }
                    return false;
                }
            }
            return true;
        }

        private static string[] archiveCosmetics = null;
        public static IEnumerator LoadCosmetics()
        {
            if (PhotonNetwork.InRoom)
            {
                Vector3 target = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                string[] spamarray = new string[] { "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU." };

                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = target;

                archiveCosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(spamarray, CosmeticsController.instance);

                Vector3 point = GorillaTagger.Instance.bodyCollider.transform.position;
                while (Vector3.Distance(point, target) > 0.2f)
                {
                    point = Vector3.Lerp(point, target, GorillaTagger.Instance.offlineVRRig.lerpValueBody * 0.3f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.1f);

                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[] { Fun.PackCosmetics(spamarray), CosmeticsController.instance.currentWornSet.ToPackedIDArray() });
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                yield return new WaitForSeconds(0.5f);

                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
                GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.tryOnSet);

                float delay = Time.time + 30f;
                while (Time.time < delay || PhotonNetwork.InRoom)
                {
                    yield return null;
                }
                currentCoroutine = null;
            }
        }
    }
}
