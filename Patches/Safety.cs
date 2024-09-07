using GorillaNetworking;
using GorillaTag;
using HarmonyLib;
using iiMenu.Notifications;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    public class AntiCheat
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (AntiCheatSelf || AntiCheatAll)
            {
                if (susId == PhotonNetwork.LocalPlayer.UserId)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>] </color><color=white>You have been reported for " + susReason + ".</color>");
                    susNick.Remove(PhotonNetwork.LocalPlayer.NickName.Length);
                    susId.Remove(PhotonNetwork.LocalPlayer.UserId.Length);
                    RPCProtection();
                }
                else
                {
                    if (AntiCheatAll)
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>] </color><color=white>" + susNick + " was reported for " + susReason + ".</color>");
                    }
                }
            }
            if (AntiACReport)
            {
                Mods.Safety.AntiReportFRT(null, false);
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>The anti cheat attempted to report you, you have been disconnected.</color>");
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
    public class NoLogErrorCount : MonoBehaviour
    {
        private static bool Prefix(string logString, string stackTrace, LogType type)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
    public class NoCheckReports : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    public class NoQuitOnBan : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    /*[HarmonyPatch(typeof(GorillaNot), "IncrementRPCTracker", new Type[] { typeof(string), typeof(string), typeof(int) })]
    public class NoIncrementRPCTracker : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }*/

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    public class NoIncrementRPCCallLocal : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
        {
            // Debug.Log(info.Sender.NickName + " sent rpc: " + rpcFunction);
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class NoIncrementRPCCall : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "")
        {
            return false;
        }
    }

    // Thanks DrPerky
    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class NoIncrementRPC : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    internal class PlayfabUtil01 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    internal class PlayfabUtil02 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    internal class PlayfabUtil03 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
    internal class NoDispatchReport : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
    internal class NoShouldDisconnectFromRoom : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(ScienceExperimentPlatformGenerator), "SpawnSodaBubbleRPC")]
    public class AntiCrashPatch
    {
        public static bool Prefix(Vector2 surfacePosLocal, float spawnSize, float lifetime, double spawnTime)
        {
            if (AntiCrashToggle)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "UpdateUserTitleDisplayName")] // Credits to Shiny for letting me use this
    internal class NamePatch
    {
        public static void Prefix(UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            string random = "";
            for (int i = 0; i < 8; i++)
            {
                random += letters[UnityEngine.Random.Range(0, letters.Length - 1)];
            }
            new UpdateUserTitleDisplayNameRequest().DisplayName = random;
        }
    }

    [HarmonyPatch(typeof(VRRig), "PlayHandTapLocal")]
    public class AntiSoundPatch
    {
        public static bool Prefix(int soundIndex, bool isLeftHand, float tapVolume)
        {
            if (AntiSoundToggle)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GliderHoldable), "Respawn")]
    public class AntiGliderRespawn
    {
        public static bool Prefix()
        {
            if (NoGliderRespawn)
            {
                return false;
            }
            return true;
        }
    }
}
