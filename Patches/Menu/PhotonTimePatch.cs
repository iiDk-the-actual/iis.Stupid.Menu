using HarmonyLib;
using Photon.Pun;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "get_ServerTimestamp")]
    public class PhotonTimePatch
    {
        public static bool enabled;
        public static int distTime = 0;

        public static void Postfix(ref int __result)
        {
            if (enabled)
                __result += distTime;
        }
    }
}
