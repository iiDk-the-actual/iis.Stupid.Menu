using iiMenu.Notifications;
using Photon.Pun;
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

        public static void ProtectionEn() // somehow people accidentally click set master so i have  to make this shit now
        {
            protsavetimekys = Time.time + 1f;
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>PROTECTION</color><color=grey>]</color> <color=white>To prevent against accidentally clicking this, you need to press the button twice.</color>");
        }

        public static void Protection()
        {
            if (Time.time > protsavetimekys)
            {
                GetIndex("Set Master <color=grey>[</color><color=red>Detected</color><color=grey>]</color>").enabled = false;
                NotifiLib.SendNotification("<color=grey>[</color><color=red>PROTECTION</color><color=grey>]</color> <color=white>To prevent against accidentally clicking this, you need to press the button twice.</color>");
            }
        }

        public static void SetMaster()
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI BAN</color><color=grey>]</color> <color=white>You are now master client! This should ONLY be enabled with the anti ban or in modded lobbies.</color>");
        }
    }
}
