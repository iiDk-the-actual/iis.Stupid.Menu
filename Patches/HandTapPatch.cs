using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "SetHandEffectData")]
    public class HandTapPatch
    {
        public static bool doPatch = false;
        public static bool tapsEnabled = true;
        public static bool doOverride = false;
        public static float overrideVolume = 99999f;
        public static int tapMultiplier = 1;

        private static bool Prefix(VRRig __instance, HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
        {
            if (doPatch)
            {
                if (__instance == VRRig.LocalRig)
                {
                    if (doOverride)
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            GorillaTagger.Instance.myVRRig.SendRPC("OnHandTapRPC", RpcTarget.All, new object[]
                            {
                                audioClipIndex,
                                isDownTap,
                                isLeftHand,
                                handTapSpeed,
                                Utils.PackVector3ToLong(dirFromHitToHand)
                            });

                            if (tapMultiplier > 1)
                            {
                                for (int i = 0; i < tapMultiplier - 1; i++)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                                    {
                                        audioClipIndex,
                                        isLeftHand,
                                        handTapSpeed
                                    });
                                }
                            }
                            Main.RPCProtection();
                        } else
                            VRRig.LocalRig.PlayHandTapLocal(audioClipIndex, isLeftHand, overrideVolume);

                        return false;
                    }
                    if (!tapsEnabled)
                    {
                        handTapSpeed = 0f;
                        GorillaTagger.Instance.handTapVolume = 0f;
                        GorillaTagger.Instance.audioClipIndex = -1;
                        audioClipIndex = -1;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
