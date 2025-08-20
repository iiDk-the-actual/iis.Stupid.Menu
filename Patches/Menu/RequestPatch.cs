using GorillaNetworking;
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
        public static bool enabled;
        public static Coroutine currentCoroutine = null;

        public static bool Prefix(VRRig __instance, PhotonMessageInfoWrapped info)
        {
            if (__instance.isLocal)
            {
                if (enabled)
                {
                    if (CosmeticsController.hasInstance)
                    {
                        if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
                        else
                            currentCoroutine ??= CoroutineManager.RunCoroutine(LoadCosmetics());
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
                Vector3 target = Menu.Main.TryOnRoom.transform.position;

                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = target;

                archiveCosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(Array.Empty<string>(), CosmeticsController.instance);

                while (Vector3.Distance(Menu.Main.ServerPos, target) > 0.2f)
                    yield return null;
                
                yield return new WaitForSeconds(0.1f);

                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[] { Fun.PackCosmetics(Array.Empty<string>()), CosmeticsController.instance.currentWornSet.ToPackedIDArray() });
                VRRig.LocalRig.enabled = true;
                yield return new WaitForSeconds(0.5f);

                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
                VRRig.LocalRig.LocalUpdateCosmeticsWithTryon(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.tryOnSet);

                float delay = Time.time + 30f;
                while (Time.time < delay || PhotonNetwork.InRoom)
                    yield return null;
                
                currentCoroutine = null;
            }
        }
    }
}
