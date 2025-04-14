using HarmonyLib;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BuilderPieceInteractor), "UpdateHandState")]
    public class BuildPatch
    {
        public static bool isEnabled = false;
        public static float previous = 0f;
        public static float previous2 = 0f;
        private static void Prefix()
        {
            if (isEnabled)
            {
                previous = GorillaTagger.Instance.offlineVRRig.NativeScale;
                previous2 = GorillaTagger.Instance.offlineVRRig.ScaleMultiplier;
                GorillaTagger.Instance.offlineVRRig.NativeScale = 1f;
                GorillaTagger.Instance.offlineVRRig.ScaleMultiplier = 1f;
            }
        }

        private static void Postfix()
        {
            if (isEnabled)
            {
                GorillaTagger.Instance.offlineVRRig.NativeScale = previous;
                GorillaTagger.Instance.offlineVRRig.ScaleMultiplier = previous2;
            }
        }
    }
}
