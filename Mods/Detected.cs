/*
 * ii's Stupid Menu  Mods/Detected.cs
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

using GorillaGameModes;
using GorillaNetworking;
using iiMenu.Extensions;
using iiMenu.Managers;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public static class Detected
    {
        public static float del;
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > del)
                    {
                        PhotonNetwork.SetMasterClient(lockTarget.GetPlayer().GetPlayer());
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                        del = Time.time + 0.02f;
                    }
                    RPCProtection();
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        private static Coroutine disablePatchCoroutine;
        public static IEnumerator DisablePatch()
        {
            while (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
                yield return null;
            
            Patches.Menu.GameModePatch.enabled = false;
        }

        public static void ChangeGamemode(GameModeType gamemode)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (disablePatchCoroutine != null)
                disablePatchCoroutine = CoroutineManager.instance.StartCoroutine(DisablePatch());

            Patches.Menu.GameModePatch.enabled = true;
            NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.NetView.gameObject);

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
            {
                { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.networkZone + GorillaComputer.instance.currentQueue + gamemode.ToString() }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);

            GameMode.activeGameMode.StopPlaying();
            GameMode.activeGameMode = null;
            GameMode.activeNetworkHandler = null;

            GameMode.LoadGameMode(gamemode.ToString());
        }
    }
}
