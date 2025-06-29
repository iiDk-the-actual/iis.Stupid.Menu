using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(VRRig), "PlayHandTapLocal")]
    public class AntiSoundPatch
    {
        public static bool Prefix(int audioClipIndex, bool isLeftHand, float tapVolume) =>
            !AntiSoundToggle;
    }
}
