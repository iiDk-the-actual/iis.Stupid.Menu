using iiMenu.Notifications;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace iiMenu.Mods
{
    internal class Battle
    {
        public static void BattleStartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.StartBattle();
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleEndGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.BattleEnd();
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleRestartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                lol.BattleEnd();
                lol.StartBattle();
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleBalloonSpam()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaBattleManager lol = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaBattleManager>();
                foreach (Photon.Realtime.Player loln in PhotonNetwork.PlayerListOthers)
                {
                    lol.playerLives[loln.ActorNumber] = UnityEngine.Random.Range(0, 4);
                }
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }
    }
}
