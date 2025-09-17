using HarmonyLib;
using iiMenu.Managers;
using Photon.Pun;
using System;

namespace iiMenu.Patches.Menu
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

            try
            {
                OnSerialize?.Invoke();
            } catch (Exception e)
            {
                LogManager.LogError($"Error in SerializePatch.OnSerialize: ${e}");
            }

            if (OverrideSerialization == null)
                return true;
            else
                return OverrideSerialization();
        }
    }
}
