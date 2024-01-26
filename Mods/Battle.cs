using iiMenu.Notifications;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace iiMenu.Mods
{
    internal class Battle
    {
        public static void BattleStartGame()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                foreach (GorillaBattleManager battle in UnityEngine.Object.FindObjectsOfType<GorillaBattleManager>())
                {
                    if (!PhotonView.Get(battle).IsMine)
                    {
                        PhotonView.Get(battle).RequestOwnership();
                    }
                    if (PhotonView.Get(battle).IsMine)
                    {
                        battle.StartBattle();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleEndGame()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                foreach (GorillaBattleManager battle in UnityEngine.Object.FindObjectsOfType<GorillaBattleManager>())
                {
                    if (!PhotonView.Get(battle).IsMine)
                    {
                        PhotonView.Get(battle).RequestOwnership();
                    }
                    if (PhotonView.Get(battle).IsMine)
                    {
                        battle.BattleEnd();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleRestartGame()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                foreach (GorillaBattleManager battle in UnityEngine.Object.FindObjectsOfType<GorillaBattleManager>())
                {
                    if (!PhotonView.Get(battle).IsMine)
                    {
                        PhotonView.Get(battle).RequestOwnership();
                    }
                    if (PhotonView.Get(battle).IsMine)
                    {
                        battle.BattleEnd();
                        battle.StartBattle();
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }

        public static void BattleBalloonSpam()
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
            {
                foreach (GorillaBattleManager battle in UnityEngine.Object.FindObjectsOfType<GorillaBattleManager>())
                {
                    if (!PhotonView.Get(battle).IsMine)
                    {
                        PhotonView.Get(battle).RequestOwnership();
                    }
                    if (PhotonView.Get(battle).IsMine)
                    {
                        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                        {
                            if (new System.Random().Next(0, 2) == 1)
                            {
                                battle.playerLives[player.ActorNumber] = 0;
                            }
                            else
                            {
                                if (battle.playerLives[player.ActorNumber] == 0)
                                {
                                    battle.playerLives[player.ActorNumber] = 3;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master.</color>");
            }
        }
    }
}
