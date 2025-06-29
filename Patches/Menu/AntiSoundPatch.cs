using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "PlayHandTapLocal")]
    public class AntiSoundPatch
    {
        public static bool enabled;
        public static bool Prefix(int audioClipIndex, bool isLeftHand, float tapVolume) =>
            !enabled;
    }
}
