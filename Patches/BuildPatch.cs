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
        private static void Prefix()
        {
            if (isEnabled)
            {
                previous = GorillaTagger.Instance.offlineVRRig.scaleFactor;
                GorillaTagger.Instance.offlineVRRig.scaleFactor = 1f;
            }
        }

        private static void Postfix()
        {
            if (isEnabled)
            {
                GorillaTagger.Instance.offlineVRRig.scaleFactor = previous;
            }
        }
    }
}
