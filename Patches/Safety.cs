using GorillaExtensions;
using GorillaTag;
using HarmonyLib;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
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
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>] </color><color=white>" + susNick + " was reported for " + susReason + ".</color>");
                }
            }
            if (AntiACReport)
            {
                if (susReason.ToLower() == "empty rig")
                    return false;

                Mods.Safety.AntiReportFRT(null, false);
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>The anti cheat attempted to report you, you have been disconnected.</color>");
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
    public class NoLogErrorCount
    {
        private static bool Prefix(string logString, string stackTrace, LogType type)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
    public class NoCheckReports
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    public class NoQuitOnBan
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    public class NoIncrementRPCCallLocal
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
        {
            // LogManager.Log(info.Sender.NickName + " sent rpc: " + rpcFunction);
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class NoIncrementRPCCall
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "")
        {
            return false;
        }
    }

    // Thanks DrPerky
    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class NoIncrementRPC
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    internal class PlayfabUtil01
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    internal class PlayfabUtil02
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
    public class PlayfabUtil03
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
    public class PlayfabUtil04
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
    public class PlayfabUtil05
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    public class PlayfabUtil06
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
    public class NoDispatchReport
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
    public class GNPTJ1
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
    public class GNPTJ2
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
    public class NoShouldDisconnectFromRoom
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
    public class AntiCrashPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
        {
            if (AntiCrashToggle && __instance == GorillaTagger.Instance.offlineVRRig && !GTExt.IsValid(throwVelocity))
                return false;
            
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
    public class AntiCrashPatch2
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashToggle && __instance == GorillaTagger.Instance.offlineVRRig)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(VRRig), "RequestMaterialColor")]
    public class AntiCrashPatch3
    {
        private static List<float> callTimestamps = new List<float>();
        public static bool Prefix(VRRig __instance)
        {
            if (AntiCrashToggle && __instance == GorillaTagger.Instance.offlineVRRig)
            {
                callTimestamps.Add(Time.time);
                callTimestamps.RemoveAll(t => (Time.time - t) > 1);

                return callTimestamps.Count < 15;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ModIOManager), "OnJoinedRoom")]
    public class ModIOPatch
    {
        public static bool Prefix(VRRig __instance)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientAPI), "UpdateUserTitleDisplayName")] // Credits to Shiny for letting me use this
    public class DisplayNamePatch
    {
        public static void Prefix(ref UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
        {
            request.DisplayName = GenerateRandomString(8);
        }
    }

    [HarmonyPatch(typeof(VRRig), "PlayHandTapLocal")]
    public class AntiSoundPatch
    {
        public static bool Prefix(int soundIndex, bool isLeftHand, float tapVolume)
        {
            if (AntiSoundToggle)
                return false;
            
            return true;
        }
    }

    [HarmonyPatch(typeof(GliderHoldable), "Respawn")]
    public class AntiGliderRespawn
    {
        public static bool Prefix()
        {
            if (NoGliderRespawn)
                return false;
            
            return true;
        }
    }
}
