using HarmonyLib;
using Photon.Pun;
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

            __result = InternalDisconnect(__instance);

            return false;
        }

        private static async Task InternalDisconnect(NetworkSystemPUN instance)
        {
            await instance.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Internal_Disconnected });
            instance.internalState = NetworkSystemPUN.InternalState.Idle;
        }
    }
}
