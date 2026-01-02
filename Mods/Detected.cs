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
using GorillaTagScripts.VirtualStumpCustomMaps;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
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
        public static Dictionary<VRRig, int> viewIdArchive = new Dictionary<VRRig, int>();
        public static void SetMasterClientGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (Time.time > masterDelay)
                        {
                            PhotonNetwork.SetMasterClient(gunTarget.GetPhotonPlayer());
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

        public static void Destroy(object target, Hashtable hashtable = null, RaiseEventOptions raiseEventOptions = null, int viewID = -1)
        {
            switch (target)
            {
                case VRRig rig:
                    if (hashtable == null)
                    {
                        PhotonView view = GetPhotonViewFromVRRig(rig);
                        hashtable = new Hashtable { { 0, viewID == -1 ? view.ViewID : viewID } };
                    }
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new int[] { rig.GetPlayer().ActorNumber } };
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, hashtable, raiseEventOptions, SendOptions.SendReliable);
                    break;
                case Player player:
                    hashtable ??= new Hashtable { { 0, player.ActorNumber } };
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new int[] { player.ActorNumber } };
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(207, hashtable, raiseEventOptions, SendOptions.SendReliable);
                    break;
                case GameObject _:
                    break;
            }
        }
        
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    PhotonNetwork.SetMasterClient(lockTarget.GetPhotonPlayer());
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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
            PhotonNetwork.SetMasterClient(GetTargetPlayer().GetPhotonPlayer());
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        public static void CrashAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                {  
                    PhotonNetwork.SetMasterClient(nearbyPlayer.GetPhotonPlayer());
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }
            }
        }

        public static void CrashOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0)
            {
                foreach (VRRig rig in touchedPlayers)
                {
                    PhotonNetwork.SetMasterClient(rig.GetPhotonPlayer());
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }
            }
        }

        public static void KickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    SerializePatch.OverrideSerialization ??= () => false;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (lockTarget == null)
                        {
                            for (int i = 0; i < 3950; i++)
                                Destroy(gunTarget);

                            RPCProtection();
                        }

                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (SerializePatch.OverrideSerialization != null)
                    SerializePatch.OverrideSerialization = null;
                if (gunLocked)
                    gunLocked = false;
            }
        }

        private static float kickDelay;
        public static void KickAll()
        {
            SerializePatch.OverrideSerialization ??= () => false;

            if (Time.time > kickDelay)
            {
                kickDelay = Time.time + 10f;
                for (int i = 0; i < 3950; i++)
                    Destroy(GetTargetPlayer().GetPhotonPlayer());

                RPCProtection();
            }
        }

        public static void KickAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                SerializePatch.OverrideSerialization ??= () => false;

                if (Time.time > kickDelay)
                {
                    foreach (VRRig nearbyPlayer in nearbyPlayers)
                    {
                        for (int i = 0; i < 3950; i++)
                            Destroy(nearbyPlayer.GetPhotonPlayer());
                    }
                }
            }
            else
                SerializePatch.OverrideSerialization = null;
        }
        
        public static void KickOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0)
            {
                SerializePatch.OverrideSerialization ??= () => false;

                if (Time.time > kickDelay)
                {
                    foreach (VRRig rig in touchedPlayers)
                    {
                        for (int i = 0; i < 3950; i++)
                            Destroy(GetTargetPlayer().GetPhotonPlayer());
                    }
                }
            }
            else
                SerializePatch.OverrideSerialization = null;
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
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        PhotonView view = GetPhotonViewFromVRRig(gunTarget);
                        if (view != null)
                        {
                            viewIdArchive[gunTarget] = view.ViewID;
                            Destroy(gunTarget, new Hashtable
                            {
                                { 0, view.ViewID }
                            }, new RaiseEventOptions
                            {
                                TargetActors = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray()
                            });
                        }
                    }
                }
            }
        }

        public static void GhostAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!rig.IsLocal())
                    {
                        PhotonView view = GetPhotonViewFromVRRig(rig);

                        if (view != null)
                        {
                            viewIdArchive[rig] = view.ViewID;
                            int[] targets = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray();

                            Destroy(rig, new Hashtable
                            {
                                { 0, view.ViewID }
                            },
                            new RaiseEventOptions
                            {
                                TargetActors = targets
                            });
                        }
                    }
                }
                catch { }
            }
        }

        public static void GhostAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
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
                        viewIdArchive[rig] = view.ViewID;
                        int[] targets = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray();

                        Destroy(rig, new Hashtable
                        {
                            { 0, view.ViewID }
                        },
                        new RaiseEventOptions
                        {
                            TargetActors = targets
                        });
                        nearbyPlayers.Remove(rig);
                    }
                }
            }
        }

        public static void GhostOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedRigs = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                        touchedRigs.Add(rig);
                }
            }

            foreach (VRRig rig in touchedRigs)
            {
                PhotonView view = GetPhotonViewFromVRRig(rig);
                if (view != null)
                {
                    viewIdArchive[rig] = view.ViewID;
                    Destroy(rig, new Hashtable
                    {
                        { 0, view.ViewID }
                    }, new RaiseEventOptions
                    {
                        TargetActors = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray()
                    });
                }
            }
        }

        private static readonly Dictionary<GorillaPlayerScoreboardLine, VRRig> linerig = new Dictionary<GorillaPlayerScoreboardLine, VRRig>();
        public static void LeaderboardGhost()
        {
            foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
            {
                if (scoreboard.buttonText.text.Contains("REPORT"))
                    scoreboard.buttonText.text = scoreboard.buttonText.text.Replace("REPORT", "GHOST");
            }

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer != NetworkSystem.Instance.LocalPlayer)
                {
                    if (line.reportInProgress)
                    {
                        line.SetReportState(false, GorillaPlayerLineButton.ButtonType.Cancel);
                        line.reportButton.isOn = true;
                        line.reportButton.UpdateColor();
                        PhotonView view = GetPhotonViewFromVRRig(line.linePlayer.VRRig());
                        if (view != null)
                        {
                            viewIdArchive[line.linePlayer.VRRig()] = view.ViewID;
                            linerig.Add(line, line.linePlayer.VRRig());
                            Destroy(line.linePlayer.VRRig(), new Hashtable
                            {
                                { 0, view.ViewID }
                            }, new RaiseEventOptions
                            {
                                TargetActors = PhotonNetwork.PlayerList.Where(p => p != view.Owner).Select(p => p.ActorNumber).ToArray()
                            });
                        }
                    }
                    if (line.reportButton.isOn && line.reportInProgress)
                    {
                        line.SetReportState(false, GorillaPlayerLineButton.ButtonType.Cancel);
                        line.reportButton.isOn = false;
                        line.reportButton.UpdateColor();
                        int ViewID = viewIdArchive[line.linePlayer.VRRig()];
                        Destroy(line.linePlayer.VRRig(), null, null, ViewID);
                    }
                }
            }
        }

        public static void DisableLeaderboardGhost()
        {
            foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
            {
                if (scoreboard.buttonText.text.Contains("GHOST"))
                    scoreboard.buttonText.text = scoreboard.buttonText.text.Replace("GHOST", "REPORT");
            }

            foreach(GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                line.SetReportState(false, GorillaPlayerLineButton.ButtonType.Cancel);
                line.reportButton.isOn = false;
                line.reportButton.UpdateColor();
            }
        }

        public static void LeaderboardMute()
        {
            if (Time.time > muteDelay)
            {
                muteDelay = Time.time + 0.15f;
                foreach (VRRig rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal() && rig.muted))
                {
                    try
                    {
                        Destroy(rig);
                    } catch { }
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
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        int viewID = viewIdArchive[gunTarget];
                        Destroy(gunTarget, null, null, viewID);
                    }
                }
            }
        }

        public static void UnghostAll()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (viewIdArchive.TryGetValue(rig, out int viewID))
                    Destroy(rig, null, null, viewID);
            }
        }
        
        public static void UnghostAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig rig in nearbyPlayers)
                {
                    int viewID = viewIdArchive[rig];
                    Destroy(rig, null, null, viewID);
                }
            }
        }

        public static void UnghostOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedRigs = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedRigs.Add(rig);
                    }
                }
            }

            foreach (VRRig rig in touchedRigs)
            {
                int viewID = viewIdArchive[rig];
                Destroy(rig, null, null,viewID);
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
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !rig.IsLocal();
                            PhotonView view = GetPhotonViewFromVRRig(rig);
                            if (includeLocal && rig != gunTarget)
                            {
                                Destroy(rig, new Hashtable
                                {
                                    { 0, view.ViewID }
                                }, new RaiseEventOptions
                                {
                                    TargetActors = new int[] { gunTarget.GetPlayer().ActorNumber }
                                });
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
                bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !rig.IsLocal();
                if (includeLocal)
                {
                    PhotonView view = GetPhotonViewFromVRRig(rig);
                    Destroy(rig, new Hashtable
                    {
                        { 0, view.ViewID }
                    }, new RaiseEventOptions
                    {
                        TargetActors = PhotonNetwork.PlayerList.Where(plr => plr.ActorNumber != view.Owner.ActorNumber).Select(plr => plr.ActorNumber).ToArray()
                    });
                }
            }
        }



        public static void IsolateAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig rig in nearbyPlayers)
                {
                    bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !rig.IsLocal();
                    if (includeLocal)
                    {
                        PhotonView view = GetPhotonViewFromVRRig(rig);
                        Destroy(rig, new Hashtable
                        {
                            { 0, view.ViewID }
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { view.Owner.ActorNumber }
                        });
                    }
                }
            }
        }

        public static void IsolateOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedRigs = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedRigs.Add(rig);
                    }
                }
            }

            foreach (VRRig touchedRig in touchedRigs)
            {
                foreach (VRRig otherRig in GorillaParent.instance.vrrigs)
                {
                    bool includeLocal = !Buttons.GetIndex("Isolate Others").enabled || !otherRig.IsLocal();
                    PhotonView view = GetPhotonViewFromVRRig(otherRig);
                    if (includeLocal && otherRig != touchedRig)
                    {
                        Destroy(otherRig, new Hashtable
                        {
                            { 0, view.ViewID }
                        }, new RaiseEventOptions
                        {
                            TargetActors = new int[] { touchedRig.GetPlayer().ActorNumber }
                        });
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
                    Destroy(lockTarget.GetPhotonPlayer());

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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
            {
                if (!rig.IsLocal())
                    Destroy(rig.GetPhotonPlayer());
            }
        }

        public static void LagAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                    Destroy(nearbyPlayer.GetPhotonPlayer());
            }
        }

        public static void LagOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0)
            {
                foreach (VRRig rig in touchedPlayers)
                    Destroy(rig.GetPhotonPlayer());
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
                        Destroy(lockTarget.GetPhotonPlayer());
                        muteDelay = Time.time + 0.15f;
                    }
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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
                {
                    if (!rig.IsLocal())
                        Destroy(rig.GetPhotonPlayer());
                }

                muteDelay = Time.time + 0.15f;
            }
        }

        public static void MuteAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0 && Time.time > muteDelay)
            {
                foreach (VRRig nearbyPlayer in nearbyPlayers)
                {
                    Destroy(nearbyPlayer.GetPhotonPlayer());
                    muteDelay = Time.time + 0.15f;
                }
            }
        }

        public static void MuteOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedRigs = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, GorillaTagger.Instance.offlineVRRig.leftHandTransform.position) <= 0.35f)
                        touchedRigs.Add(rig);
                }
            }

            if (touchedRigs.Count > 0 && Time.time > muteDelay)
            {
                foreach (VRRig rig in touchedRigs)
                    Destroy(rig.GetPhotonPlayer());

                muteDelay = Time.time + 0.15f;
            }
        }

        private static Coroutine disablePatchCoroutine;
        public static IEnumerator DisablePatch()
        {
            while (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
                yield return null;
            
            GameModePatch.enabled = false;
        }

        public static string name = "GOLDENTROPHY";
        
        public static void PromptNameChange() =>
            Prompt("Would you like to set a name?", () => PromptSingleText("Please enter the name you'd like to use:", () => name = keyboardInput));

        public static void ChangeNameGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = name
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(lockTarget.GetPlayer().ActorNumber, hashtable);
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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

        public static void ChangeNameAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                Hashtable hashtable = new Hashtable
                {
                    [byte.MaxValue] = name
                };
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(player.ActorNumber, hashtable);
            }
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

        public static bool moddedGamemode;
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

            string queue = moddedGamemode ? GorillaComputer.instance.currentQueue + "MODDED_" : GorillaComputer.instance.currentQueue;

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

            float timeUntil = Time.time + 1f;
            while (!(PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) || Time.time < timeUntil)
                yield return null;

            if (PhotonNetwork.InRoom)
                ChangeGamemode(gamemode);
        }

        public static void DriverStatus(bool locked)
        {
            if (PhotonNetwork.IsMasterClient)
                CustomMapsTerminal.instance.mapTerminalNetworkObject.SendRPC("SetTerminalControlStatus_RPC", true, locked, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public static void BecomeDriver()
        {
            if (PhotonNetwork.IsMasterClient)
                CustomMapsTerminal.instance.mapTerminalNetworkObject.SendRPC("SetTerminalControlStatus_RPC", true, true, PhotonNetwork.LocalPlayer.ActorNumber);
            CustomMapsTerminal.instance.mapTerminalNetworkObject.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
        }

        private static long? id;
        private static float setMapDelay;
        public static void VirtualStumpKickGun()
        {
            if (!PhotonNetwork.InRoom)
            {
                id = null;
                return;
            }

            if (id == null && Time.time > setMapDelay)
            {
                setMapDelay = Time.time + 1f;

                if (CustomMapsTerminal.GetDriverID() != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>VSTUMP</color><color=grey>]</color> Gaining control of the terminal, please wait...");
                    BecomeDriver();
                    return;
                }

                if (CustomMapManager.IsRemotePlayerInVirtualStump(NetworkSystem.Instance.LocalPlayer.UserId))
                {
                    id = CustomMaps.Manager.currentMapId == 4977315 ? 5024157 : 4977315;

                    CustomMapsTerminal.instance.mapTerminalNetworkObject.photonView.RPC("UpdateScreen_RPC", lockTarget.GetPhotonPlayer(), new object[]
                    {
                        6,
                        id,
                        CustomMapsTerminal.GetDriverID()
                    });

                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully assigned ID. You may now use the kick gun.");
                } else
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please temporarily enter the Virtual Stump.");
            }

            if (GetGunInput(false) && id != null)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                        CustomMapsTerminal.instance.mapTerminalNetworkObject.photonView.RPC("SetRoomMap_RPC", lockTarget.GetPhotonPlayer(), id.Value);
                }
            }
        }

        public static void VirtualStumpKickAll()
        {
            if (!PhotonNetwork.InRoom)
            {
                id = null;
                return;
            }

            if (id == null && Time.time > setMapDelay)
            {
                setMapDelay = Time.time + 1f;

                if (CustomMapsTerminal.GetDriverID() != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>VSTUMP</color><color=grey>]</color> Gaining control of the terminal, please wait...");
                    BecomeDriver();
                    return;
                }

                if (CustomMapManager.IsRemotePlayerInVirtualStump(NetworkSystem.Instance.LocalPlayer.UserId))
                {
                    id = CustomMaps.Manager.currentMapId == 4977315 ? 5024157 : 4977315;

                    CustomMapsTerminal.instance.mapTerminalNetworkObject.photonView.RPC("UpdateScreen_RPC", lockTarget.GetPhotonPlayer(), new object[]
                    {
                        6,
                        id,
                        CustomMapsTerminal.GetDriverID()
                    });

                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully assigned ID. You may now use the kick gun.");
                }
                else
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please temporarily enter the Virtual Stump.");
            }

            CustomMapsTerminal.instance.mapTerminalNetworkObject.photonView.RPC("SetRoomMap_RPC", RpcTarget.Others, id.Value);

            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully kicked others.");
            Toggle("Virtual Stump Kick All");
        }
    }
}
