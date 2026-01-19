/*
 * ii's Stupid Menu  Utilities/GameModeUtilities.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GorillaGameModes;
using GorillaTagScripts;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

namespace iiMenu.Utilities
{
    public class GameModeUtilities
    {
        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer>();

            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return infected;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        infected.Add(tagManager.currentIt);
                    else
                        infected.AddRange(tagManager.currentInfected);
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        infected.Add(ghostManager.currentIt);
                    else
                        infected.AddRange(ghostManager.currentInfected);
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;

                    infected.AddRange(paintbrawlManager.playerLives.Where(element => element.Value <= 0).Select(element => element.Key).ToArray().Select(deadPlayer => PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(deadPlayer)).Select(dummy => (NetPlayer)dummy));

                    if (!infected.Contains(NetworkSystem.Instance.LocalPlayer))
                        infected.Add(NetworkSystem.Instance.LocalPlayer);

                    break;
            }

            return infected;
        }

        public static void AddInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        tagManager.ChangeCurrentIt(plr);
                    else if (!tagManager.currentInfected.Contains(plr))
                        tagManager.AddInfectedPlayer(plr);
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        ghostManager.ChangeCurrentIt(plr);
                    else if (!ghostManager.currentInfected.Contains(plr))
                        ghostManager.AddInfectedPlayer(plr);
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 0;

                    break;
            }
        }

        public static void RemoveInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    switch (tagManager.isCurrentlyTag)
                    {
                        case true when tagManager.currentIt == plr:
                            tagManager.currentIt = null;
                            break;
                        case false when tagManager.currentInfected.Contains(plr):
                            tagManager.currentInfected.Remove(plr);
                            break;
                    }
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    switch (ghostManager.isCurrentlyTag)
                    {
                        case true when ghostManager.currentIt == plr:
                            ghostManager.currentIt = null;
                            break;
                        case false when ghostManager.currentInfected.Contains(plr):
                            ghostManager.currentInfected.Remove(plr);
                            break;
                    }
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 3;

                    break;
            }
        }

        public static void AddRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    tagManager.ChangeCurrentIt(plr);

                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    ghostManager.ChangeCurrentIt(plr);

                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 0;

                    break;
            }
        }

        public static void RemoveRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.currentIt == plr)
                        tagManager.ChangeCurrentIt(null);

                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.currentIt == plr)
                        ghostManager.ChangeCurrentIt(null);

                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 3;

                    break;
            }
        }
    }
}
