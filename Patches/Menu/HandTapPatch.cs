using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "SetHandEffectData")]
    public class HandTapPatch
    {
        public static bool enabled;
        public static bool tapsEnabled = true;
        public static bool doOverride;
        public static float overrideVolume = 99999f;
        public static int tapMultiplier = 1;

        private static bool Prefix(VRRig __instance, HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
        {
            if (enabled)
            {
                if (__instance == VRRig.LocalRig)
                {
                    if (doOverride)
                    {
                        effectContext.speed = overrideVolume;
                        effectContext.soundVolume = overrideVolume;

                        if (PhotonNetwork.InRoom)
                        {
                            if (tapMultiplier > 1)
                            {
                                for (int i = 0; i < tapMultiplier; i++)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                                    {
                                        audioClipIndex,
                                        isLeftHand,
                                        handTapSpeed
                                    });
                                }
                                Main.RPCProtection();
                            }
                        } else
                            VRRig.LocalRig.PlayHandTapLocal(audioClipIndex, isLeftHand, overrideVolume);

                        return false;
                    }
                    if (!tapsEnabled)
                    {
                        effectContext.speed = 0f;
                        effectContext.soundVolume = 0f;

                        GorillaTagger.Instance.handTapVolume = 0f;
                        GorillaTagger.Instance.handTapSpeed = 0f;
                        GorillaTagger.Instance.audioClipIndex = -1;

                        return false;
                    }
                }
            }
            return true;
        }
    }
}
