using ExitGames.Client.Photon;
using iiMenu.Notifications;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Experimental
    {
        public static void AntiBan()
        {
            antiBanEnabled = true;
            ExecuteCloudScriptRequest executeCloudScriptRequest = new ExecuteCloudScriptRequest();
            executeCloudScriptRequest.FunctionName = "RoomClosed";
            executeCloudScriptRequest.FunctionParameter = new
            {
                GameId = PhotonNetwork.CurrentRoom.Name,
                Region = Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
            };
            PlayFabClientAPI.ExecuteCloudScript(executeCloudScriptRequest, delegate (ExecuteCloudScriptResult result)
            {
            }, null, null, null);
            Hashtable hashtable = new Hashtable();
            hashtable.Add("gameMode", "forestDEFAULTMODDED_MODDED_INFECTION");
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);

            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI BAN</color><color=grey>]</color> <color=white>The anti ban has been enabled! I take ZERO responsibility for bans using this.</color>");
        }

        public static void SetMaster()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI BAN</color><color=grey>]</color> <color=white>You are now master client! This should ONLY be enabled with the anti ban or in modded lobbies.</color>");
        }
    }
}
