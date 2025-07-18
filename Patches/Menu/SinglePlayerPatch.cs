using HarmonyLib;
using iiMenu.Classes;
using Photon.Pun;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(NetworkSystemPUN), "InternalDisconnect")]
    public class SinglePlayerPatch
    {
        public static bool enabled;

        private static bool Prefix(NetworkSystemPUN __instance, ref Task __result)
        {
            if (!enabled)
                return true;

            __instance.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
            PhotonNetwork.Disconnect();

            Object.Destroy(__instance.VoiceNetworkObject);
            __instance.UpdatePlayers();
            __instance.SinglePlayerStarted();

            return false;
        }
    }
}
