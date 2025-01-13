using HarmonyLib;
using iiMenu.Mods;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "LateUpdate")]
    public class TorsoPatch
    {
        public static bool enabled = false;
        public static int mode = 0;

        public static void Postfix(VRRig __instance)
        {
            if (__instance == GorillaTagger.Instance.offlineVRRig && enabled)
            {
                Quaternion rotation = Quaternion.identity;
                switch (mode)
                {
                    case 0:
                        rotation = Quaternion.Euler(0f, (Time.time * 90f) % 360, 0f);
                        break;
                    case 1:
                        rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                        break;
                    case 2:
                        rotation = Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y + 180f, 0f);
                        break;
                    case 3:
                        rotation = Quaternion.Euler(0f, Movement.recBodyRotary.transform.rotation.eulerAngles.y, 0f);
                        break;
                }

                __instance.transform.rotation = rotation;
                __instance.head.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
                __instance.leftHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
                __instance.rightHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
            }
        }
    }
}
