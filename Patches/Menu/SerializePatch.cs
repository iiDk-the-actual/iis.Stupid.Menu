using HarmonyLib;
using Photon.Pun;
using System;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
    public class SerializePatch
    {
        public static event Action OnSerialize;
        public static Func<bool> OverrideSerialization;

        public static bool Prefix()
        {
            if (!PhotonNetwork.InRoom)
                return true;

            OnSerialize?.Invoke();

            if (OverrideSerialization == null)
                return true;
            else
                return OverrideSerialization();
        }
    }
}
