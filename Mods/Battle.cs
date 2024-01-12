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

        public static void BattleEndGame()
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

        public static void BattleRestartGame()
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

        public static void BattleBalloonSpam()
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
    }
}
