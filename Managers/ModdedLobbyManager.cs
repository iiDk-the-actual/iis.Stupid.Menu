/*
 * ii's Stupid Menu  Managers/ModdedLobbyManager.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using ExitGames.Client.Photon;
using iiMenu.Notifications;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Managers
{
    public static class ModdedLobbyManager
    {
        private static bool menuEnabled = false;
        private static bool wasInModdedLobby = false;
        private static GameObject menuLoader;

        public static bool IsInModdedLobby()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties != null)
                {
                    return PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED");
                }
            }
            return false;
        }

        public static bool IsMenuEnabled()
        {
            return menuEnabled;
        }

        public static void Initialize(GameObject loader)
        {
            menuLoader = loader;
            menuLoader.SetActive(false);
            LogManager.Log("[ModdedLobbyManager] Initialized - Menu disabled by default");
        }

        public static void CheckLobbyAndUpdateMenuState()
        {
            bool isModded = IsInModdedLobby();

            if (isModded && !menuEnabled)
            {
                EnableMenu();
            }
            else if (!isModded && menuEnabled)
            {
                DisableMenu();
            }

            wasInModdedLobby = isModded;
        }

        private static void EnableMenu()
        {
            menuEnabled = true;
            LogManager.Log("[ModdedLobbyManager] Modded lobby detected - Menu enabled");
            NotifiLib.SendNotification("<color=green>Modded lobby detected!</color> Menu is now active.");

            if (menuLoader != null)
            {
                menuLoader.SetActive(true);
            }
        }

        private static void DisableMenu()
        {
            menuEnabled = false;
            LogManager.Log("[ModdedLobbyManager] Public lobby detected - Menu disabled");
            NotifiLib.SendNotification("<color=red>Public lobby detected!</color> Menu has been disabled for safety.");

            if (menuLoader != null)
            {
                menuLoader.SetActive(false);
            }
        }

        public static void OnJoinedRoom()
        {
            CheckLobbyAndUpdateMenuState();
        }

        public static void OnLeftRoom()
        {
            if (menuEnabled)
            {
                DisableMenu();
            }
        }

        public static void ForceModdedProperties()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
                if (properties == null)
                {
                    properties = new Hashtable();
                }

                properties["MODDED"] = "TRUE";
                PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

                LogManager.Log("[ModdedLobbyManager] Set room as MODDED");
                CheckLobbyAndUpdateMenuState();
            }
        }
    }
}