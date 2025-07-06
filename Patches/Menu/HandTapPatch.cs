using HarmonyLib;
using iiMenu.Menu;
using Photon.Pun;
using UnityEngine;
using System.Reflection;

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

        private static bool Prefix(VRRig __instance, object effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
        {
            if (!enabled || __instance != VRRig.LocalRig)
                return true;

            var effectType = effectContext.GetType();

            if (doOverride)
            {
                FieldInfo speedField = effectType.GetField("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo volumeField = effectType.GetField("soundVolume", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (speedField != null) speedField.SetValue(effectContext, overrideVolume);
                if (volumeField != null) volumeField.SetValue(effectContext, overrideVolume);

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
                }
                else
                {
                    VRRig.LocalRig.PlayHandTapLocal(audioClipIndex, isLeftHand, overrideVolume);
                }

                return false;
            }

            if (!tapsEnabled)
            {
                FieldInfo speedField = effectType.GetField("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo volumeField = effectType.GetField("soundVolume", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (speedField != null) speedField.SetValue(effectContext, 0f);
                if (volumeField != null) volumeField.SetValue(effectContext, 0f);

                GorillaTagger.Instance.handTapVolume = 0f;
                GorillaTagger.Instance.handTapSpeed = 0f;

                FieldInfo clipIndexField = typeof(GorillaTagger).GetField("audioClipIndex", BindingFlags.NonPublic | BindingFlags.Instance);
                if (clipIndexField != null) clipIndexField.SetValue(GorillaTagger.Instance, -1);

                return false;
            }

            return true;
        }
    }
}
