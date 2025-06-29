using HarmonyLib;
using Photon.Pun;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
    public class SerializePatch
    {
        public static event System.Action OnSerialize;

        public static void Postfix() =>
            OnSerialize?.Invoke();
    }
}
