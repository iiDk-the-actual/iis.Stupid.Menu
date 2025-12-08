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

using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RigUtilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace iiMenu.Mods
{
    public static class Detected
    {
        public static float masterDelay;
        public static Dictionary<Player, int> viewIdArchive = new Dictionary<Player, int>();
        public static void SetMasterClientGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        if (Time.time > masterDelay)
                        {
                            PhotonNetwork.SetMasterClient(gunTarget.GetPlayer().GetPlayer());
                            masterDelay = Time.time + 0.02f;
                        }
    
                    }
                }
            }
        }

        public static void AutoSetMasterClient()
        {
            if (!PhotonNetwork.InRoom) return;
            if (!PhotonNetwork.IsMasterClient)
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        public static float crashDelay;
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > crashDelay)
                    {
                        PhotonNetwork.SetMasterClient(lockTarget.GetPlayer().GetPlayer());
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                        crashDelay = Time.time + 0.02f;
                    }

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

        public static void CrashAll()
        {
            if (Time.time > crashDelay)
            {
                PhotonNetwork.SetMasterClient(GetCurrentTargetRig().GetPlayer().GetPlayer());
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                crashDelay = Time.time + 0.02f;
            }
        }

        public static void KickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    PhotonView view = GetPhotonViewFromVRRig(lockTarget);
                    if (!Movement.isBlinking)
                        Movement.Blink();
                    for (int i = 0; i < 3950; i++)
                    {
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                        {
                            { 0, view.ViewID }
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { view.Owner.ActorNumber },
                        }, SendOptions.SendUnreliable);
                    }
                        
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
                if (Movement.isBlinking)
                    Movement.DisableBlink();
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void GhostGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        PhotonView view = GetPhotonViewFromVRRig(gunTarget);
                        if (view != null)
                        {
                            viewIdArchive[view.Owner] = view.ViewID;
                            PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                            {
                                { 0, view.ViewID }
                            }, new RaiseEventOptions
                            {
                                TargetActors = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray()
                            }, SendOptions.SendReliable);
                        }
                    }
                }
            }
        }

        public static void GhostAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                PhotonView view = GetPhotonViewFromVRRig(rig);

                if (view != null)
                {
                    viewIdArchive[view.Owner] = view.ViewID;
                    int[] targets = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray();

                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                    {
                        { 0, view.ViewID }
                    },
                    new RaiseEventOptions
                    {
                        TargetActors = targets
                    }, SendOptions.SendReliable);
                }
            }
        }

        public static void GhostAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !PlayerIsLocal(vrrig))
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig rig in nearbyPlayers)
                {
                    PhotonView view = GetPhotonViewFromVRRig(rig);

                    if (view != null)
                    {
                        viewIdArchive[view.Owner] = view.ViewID;
                        int[] targets = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray();

                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                        {
                            { 0, view.ViewID }
                        },
                        new RaiseEventOptions
                        {
                            TargetActors = targets
                        }, SendOptions.SendReliable);
                        nearbyPlayers.Remove(rig);
                    }
                }
            }
        }

        public static void UnghostGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        Player target = gunTarget.GetPlayer().GetPlayer();
                        int viewID = viewIdArchive[target];
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                        {
                            { 0, viewID }
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { target.ActorNumber }
                        }, SendOptions.SendReliable);
                    }
                }
            }
        }

        public static void UnghostAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                Player target = rig.GetPlayer().GetPlayer();
                int viewID = viewIdArchive[target];

                PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                {
                    { 0, viewID }
                },
                new RaiseEventOptions
                {
                    TargetActors = new int[] { target.ActorNumber },
                }, SendOptions.SendReliable);
            }
        }

        public static void UnghostAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !PlayerIsLocal(vrrig))
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig rig in nearbyPlayers)
                {
                    Player target = rig.GetPlayer().GetPlayer();
                    int viewID = viewIdArchive[target];

                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                    {
                        { 0, viewID }
                    },
                    new RaiseEventOptions
                    {
                        TargetActors = new int[] { target.ActorNumber },
                    }, SendOptions.SendReliable);
                }
            }
        }

        public static void IsolateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !PlayerIsLocal(rig);
                            PhotonView view = GetPhotonViewFromVRRig(rig);
                            if (includeLocal && rig != gunTarget)
                            {
                                PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                                {
                                    { 0, view.ViewID }
                                }, new RaiseEventOptions
                                {
                                    TargetActors = new int[] { gunTarget.GetPlayer().ActorNumber }
                                }, SendOptions.SendReliable);
                            }
                        }
                    }
                }
            }
        }

        public static void IsolateAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !PlayerIsLocal(rig);
                if (includeLocal)
                {
                    PhotonView view = GetPhotonViewFromVRRig(rig);
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                    {
                        { 0, view.ViewID }
                    }, new RaiseEventOptions
                    {
                        TargetActors = AllActorNumbersExcept(view.Owner.ActorNumber)
                    }, SendOptions.SendReliable);
                }
            }
        }

        public static void IsolateAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !PlayerIsLocal(vrrig))
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig rig in nearbyPlayers)
                {
                    bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !PlayerIsLocal(rig);
                    if (includeLocal)
                    {
                        PhotonView view = GetPhotonViewFromVRRig(rig);
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(204, new Hashtable
                        {
                            { 0, view.ViewID }
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { view.Owner.ActorNumber }
                        }, SendOptions.SendReliable);
                    }
                    
                }
            }
        }

        public static void LagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    PhotonNetwork.Destroy(GetPhotonViewFromVRRig(lockTarget));

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
        
        public static void LagAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
                PhotonNetwork.Destroy(GetPhotonViewFromVRRig(rig));
        }

        public static void LagAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !PlayerIsLocal(vrrig))
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                    PhotonNetwork.Destroy(GetPhotonViewFromVRRig(nearbyPlayer));
            }
        }

        public static float muteDelay;
        public static void MuteGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > muteDelay)
                    {
                        PhotonNetwork.Destroy(GetPhotonViewFromVRRig(lockTarget));
                        muteDelay = Time.time + 0.15f;
                    }
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

        public static void MuteAll()
        {
            if (Time.time > muteDelay)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    PhotonNetwork.Destroy(GetPhotonViewFromVRRig(rig));

                muteDelay = Time.time + 0.15f;
            }
        }

        public static void MuteAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !PlayerIsLocal(vrrig))
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0 && Time.time > muteDelay)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                {
                    PhotonNetwork.Destroy(GetPhotonViewFromVRRig(nearbyPlayer));
                    muteDelay = Time.time + 0.15f;
                }
            }
        }

        private static Coroutine disablePatchCoroutine;
        public static IEnumerator DisablePatch()
        {
            while (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
                yield return null;
            
            Patches.Menu.GameModePatch.enabled = false;
        }

        public static void BreakNetworkTriggers()
        {
            string queue = Buttons.GetIndex("Switch to Modded Gamemode").enabled ? GorillaComputer.instance.currentQueue + "MODDED_" : GorillaComputer.instance.currentQueue;
            Hashtable hash = new Hashtable
            {
                { "gameMode", string.Join("", GorillaComputer.instance.allowedMapsToJoin) + queue + GorillaComputer.instance.currentGameMode.Value }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);
        }

        public static void KickNetworkTriggers()
        {
            if (NetworkSystem.Instance.SessionIsPrivate)
                Overpowered.SetRoomStatus(false);

            Hashtable hash = new Hashtable
            {
                { "gameMode", GorillaComputer.instance.currentGameMode.Value }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);
        }

        private static float spazGamemodeDelay;
        public static void SpazGamemode()
        {
            if (Time.time > spazGamemodeDelay)
            {
                ChangeGamemode((GameModeType)Random.Range(0, (int)GameModeType.Count));
                spazGamemodeDelay = Time.time + 0.1f;
            }
        }

        public static void ChangeGamemode(GameModeType gamemode)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                CoroutineManager.instance.StartCoroutine(ChangeGamemodeMasterDelay(gamemode));
                return;
            }

            if (disablePatchCoroutine != null)
                disablePatchCoroutine = CoroutineManager.instance.StartCoroutine(DisablePatch());

            Patches.Menu.GameModePatch.enabled = true;
            NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.NetView.gameObject);

            string queue = Buttons.GetIndex("Switch to Modded Gamemode").enabled ? GorillaComputer.instance.currentQueue + "MODDED_" : GorillaComputer.instance.currentQueue;

            Hashtable hash = new Hashtable
            {
                { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.networkZone + queue + gamemode.ToString() }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);

            GameMode.activeGameMode.StopPlaying();
            GameMode.activeGameMode = null;
            GameMode.activeNetworkHandler = null;

            GameMode.LoadGameMode(gamemode.ToString());
        }

        public static IEnumerator ChangeGamemodeMasterDelay(GameModeType gamemode)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);

            while (PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
                yield return null;

            if (PhotonNetwork.InRoom)
                ChangeGamemode(gamemode);
        }
    }
}
