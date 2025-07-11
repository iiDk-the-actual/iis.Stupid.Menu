using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BuilderPieceInteractor), "UpdateHandState")]
    public class BuildPatch
    {
        public static bool enabled;
        public static float previous;
        public static float previous2;

        private static void Prefix()
        {
            if (enabled)
            {
                previous = RigManager.LocalRig.NativeScale;
                previous2 = RigManager.LocalRig.ScaleMultiplier;
                RigManager.LocalRig.NativeScale = 1f;
                RigManager.LocalRig.ScaleMultiplier = 1f;
            }
        }

        private static void Postfix()
        {
            if (enabled)
            {
                RigManager.LocalRig.NativeScale = previous;
                RigManager.LocalRig.ScaleMultiplier = previous2;
            }
        }
    }
}
