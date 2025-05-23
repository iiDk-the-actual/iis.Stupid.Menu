using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using System.Reflection;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnHandTap")]
    public class HandTapPatch
    {
        public static bool doPatch = false;
        public static bool tapsEnabled = true;
        public static bool doOverride = false;
        public static float overrideVolume = 99999f;
        public static int tapMultiplier = 1;

        private static bool Prefix(VRRig __instance, int soundIndex, bool isLeftHand, float handSpeed, Vector3 tapDir)
        {
            if (doPatch)
            {
                if (__instance == GorillaTagger.Instance.offlineVRRig)
                {
                    if (doOverride)
                    {
                        handSpeed = overrideVolume;
                        tapDir = new Vector3(overrideVolume, overrideVolume, overrideVolume);
                        GorillaTagger.Instance.handTapVolume = overrideVolume;
                        typeof(GorillaTagger).GetField("tempHitDir", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance, new Vector3(overrideVolume, overrideVolume, overrideVolume));
                        if (PhotonNetwork.InRoom)
                        {
                            GorillaTagger.Instance.myVRRig.SendRPC("OnHandTapRPC", RpcTarget.All, new object[]
                            {
                                soundIndex,
                                isLeftHand,
                                handSpeed,
                                Utils.PackVector3ToLong(tapDir)
                            });

                            if (tapMultiplier > 1)
                            {
                                for (int i = 0; i < tapMultiplier - 1; i++)
                                {
                                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                                    {
                                        soundIndex,
                                        isLeftHand,
                                        handSpeed
                                    });
                                }
                                Main.RPCProtection();
                            }
                        } else
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundIndex, isLeftHand, handSpeed);

                        return false;
                    }
                    if (!tapsEnabled)
                    {
                        handSpeed = 0f;
                        GorillaTagger.Instance.handTapVolume = 0f;
                        typeof(GorillaTagger).GetField("tempInt", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(GorillaTagger.Instance, (int)-1);
                        soundIndex = -1;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
