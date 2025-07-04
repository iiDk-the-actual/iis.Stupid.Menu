using System;
using System.Collections.Generic;
using HarmonyLib;
using PlayFab;
using PlayFab.ClientModels;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(PlayFabClientAPI), "UpdateUserTitleDisplayName")] // Credits to Shiny for letting me use this
    public class DisplayNamePatch
    {
        public static void Prefix(ref UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null) =>
            request.DisplayName = GenerateRandomString(UnityEngine.Random.Range(3, 12));
    }
}
