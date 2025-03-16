using HarmonyLib;
using hykmMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine;
using static hykmMenu.Menu.Main;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
    public class JoinPatch
    {
        private static void Prefix(Player newPlayer)
        {
            if (newPlayer != oldnewplayer)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>JOIN</color><color=grey>] </color><color=white>Name: " + newPlayer.NickName + "</color>");
                if (customSoundOnJoin)
                {
                    if (!Directory.Exists("iisStupidMenu"))
                    {
                        Directory.CreateDirectory("iisStupidMenu");
                    }
                    File.WriteAllText("iisStupidMenu/hykmMenu_CustomSoundOnJoin.txt", "PlayerJoin");
                }
                oldnewplayer = newPlayer;
            }
        }

        private static Player oldnewplayer;
    }
}