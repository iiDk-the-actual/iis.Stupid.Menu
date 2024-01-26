using GorillaNetworking;
using iiMenu.Notifications;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Experimental
    {
        /*public static void AntiBan()
        { this shit was skidded and wont come back
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
        }*/
        
        public static void SetMaster()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString().ToLower().Contains("modded"))
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> <color=white>You are now master client! This should ONLY be enabled in modded lobbies.</color>");
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are either not in a lobby, or your lobby is not modded.</color>");
            }
        }
    }
}
