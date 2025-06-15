using HarmonyLib;

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
                previous = VRRig.LocalRig.NativeScale;
                previous2 = VRRig.LocalRig.ScaleMultiplier;
                VRRig.LocalRig.NativeScale = 1f;
                VRRig.LocalRig.ScaleMultiplier = 1f;
            }
        }

        private static void Postfix()
        {
            if (isEnabled)
            {
                VRRig.LocalRig.NativeScale = previous;
                VRRig.LocalRig.ScaleMultiplier = previous2;
            }
        }
    }
}
