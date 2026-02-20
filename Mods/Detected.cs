/*
 * ii's Stupid Menu  Mods/Detected.cs
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

using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
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
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.RigUtilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace iiMenu.Mods
{
    public static class Detected
    {
        public static void EnterDetectedTab()
        {
            if (!allowDetected) { 
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/danger.ogg", "Audio/Menu/danger.ogg"), buttonClickVolume / 10f); 
                Prompt("The mods in this category are detected. <b>Unless you know what you're doing, you will get banned.</b> Are you sure you would like to continue?", 
                    () => { 
                        allowDetected = true; Buttons.CurrentCategoryName = "Detected Mods";

                        AchievementManager.UnlockAchievement(new AchievementManager.Achievement
                        {
                            name = "Sinister",
                            description = "Open the \"Detected Mods\" category.",
                            icon = "Images/Achievements/sinister.png"

                        });
                    }); 
            } else Buttons.CurrentCategoryName = "Detected Mods";
        }

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

        public static void SetMasterClientAll()
        {
            if (Time.time > masterDelay)
            {
                PhotonNetwork.SetMasterClient(GetTargetPlayer().GetPhotonPlayer());
                masterDelay = Time.time + 0.02f;
            }
        }

        public static void SetMasterClientAura()
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

            if (nearbyPlayers.Count <= 0) return;
            foreach (var nearbyPlayer in nearbyPlayers.Where(nearbyPlayer => Time.time > masterDelay))
            {
                PhotonNetwork.SetMasterClient(nearbyPlayer.GetPhotonPlayer());
                masterDelay = Time.time + 0.02f;
            }
        }

        public static void SetMasterClientOnTouch()
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

            if (touchedPlayers.Count <= 0) return;
            {
                foreach (var rig in touchedPlayers.Where(rig => Time.time > masterDelay))
                {
                    PhotonNetwork.SetMasterClient(rig.GetPhotonPlayer());
                    masterDelay = Time.time + 0.02f;
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
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new[] { rig.GetPlayer().ActorNumber } };
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(204, hashtable, raiseEventOptions, SendOptions.SendReliable);
                    break;
                case Player player:
                    hashtable ??= new Hashtable { { 0, player.ActorNumber } };
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } };
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

        public static void CrashWhenTouched()
        {
            foreach (var playerFromVRRig in from vrrig in GorillaParent.instance.vrrigs where !vrrig.isMyPlayer && !vrrig.isOfflineVRRig && (Vector3.Distance(vrrig.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5 || Vector3.Distance(vrrig.leftHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5 || Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position) <= 0.5) select GetPlayerFromVRRig(vrrig))
            {
                PhotonNetwork.SetMasterClient(playerFromVRRig.GetPlayer());
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
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
                foreach (VRRig rig in nearbyPlayers.ToList())
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
            foreach (var scoreboard in GorillaScoreboardTotalUpdater.allScoreboards.Where(scoreboard => scoreboard.buttonText.text.Contains("REPORT")))
                scoreboard.buttonText.text = scoreboard.buttonText.text.Replace("REPORT", "GHOST");

            foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.linePlayer != NetworkSystem.Instance.LocalPlayer))
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

                if (!line.reportButton.isOn || !line.reportInProgress) continue;
                line.SetReportState(false, GorillaPlayerLineButton.ButtonType.Cancel);
                line.reportButton.isOn = false;
                line.reportButton.UpdateColor();
                int viewID = viewIdArchive[line.linePlayer.VRRig()];
                Destroy(line.linePlayer.VRRig(), null, null, viewID);
            }
        }

        public static void DisableLeaderboardGhost()
        {
            foreach (var scoreboard in GorillaScoreboardTotalUpdater.allScoreboards.Where(scoreboard => scoreboard.buttonText.text.Contains("GHOST")))
                scoreboard.buttonText.text = scoreboard.buttonText.text.Replace("GHOST", "REPORT");

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
                                    TargetActors = new[] { gunTarget.GetPlayer().ActorNumber }
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
                            TargetActors = new[] { view.Owner.ActorNumber }
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
                            TargetActors = new[] { touchedRig.GetPlayer().ActorNumber }
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
            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal()))
                Destroy(rig.GetPhotonPlayer());
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
            if (!(Time.time > muteDelay)) return;
            foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal()))
                Destroy(rig.GetPhotonPlayer());

            muteDelay = Time.time + 0.15f;
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

        public static void ChangeNameAura()
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
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = name
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(nearbyPlayer.GetPlayer().ActorNumber, hashtable);
                }
            }
        }

        public static void ChangeNameOnTouch()
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
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = name
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(rig.GetPlayer().ActorNumber, hashtable);
                }
            }
        }

        public static void BanGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = GorillaComputer.instance.anywhereTwoWeek[Random.Range(0, GorillaComputer.instance.anywhereTwoWeek.Length)]
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(lockTarget.GetPlayer().ActorNumber, hashtable);
                    MonkeAgent.instance.SendReport("evading the name ban", lockTarget.GetPlayer().UserId, lockTarget.GetPlayer().NickName);
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

        public static void BanAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                Hashtable hashtable = new Hashtable
                {
                    [byte.MaxValue] = GorillaComputer.instance.anywhereTwoWeek[Random.Range(0, GorillaComputer.instance.anywhereTwoWeek.Length)]
                };
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(player.ActorNumber, hashtable);
                MonkeAgent.instance.SendReport("evading the name ban", player.UserId, player.NickName);
            }
        }

        public static void BanAura()
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
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = GorillaComputer.instance.anywhereTwoWeek[Random.Range(0, GorillaComputer.instance.anywhereTwoWeek.Length)]
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(nearbyPlayer.GetPlayer().ActorNumber, hashtable);
                    MonkeAgent.instance.SendReport("evading the name ban", nearbyPlayer.GetPlayer().UserId, nearbyPlayer.GetPlayer().NickName);
                }
            }
        }

        public static void BanOnTouch()
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
                    Hashtable hashtable = new Hashtable
                    {
                        [byte.MaxValue] = GorillaComputer.instance.anywhereTwoWeek[Random.Range(0, GorillaComputer.instance.anywhereTwoWeek.Length)]
                    };
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(rig.GetPlayer().ActorNumber, hashtable);
                    MonkeAgent.instance.SendReport("evading the name ban", rig.GetPlayer().UserId, rig.GetPlayer().NickName);
                }
            }
        }

        private static float customPropertyDelay;
        public static void BypassModCheckersGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > customPropertyDelay)
                    {
                        customPropertyDelay = Time.time + 0.25f;

                        var player = gunTarget.GetPhotonPlayer();
                        if (player == null) return;

                        if (player.CustomProperties == null || player.CustomProperties.Count == 0) return;

                        Hashtable toRemove = new Hashtable();

                        foreach (var key in from keyObj in player.CustomProperties.Keys.ToList() select keyObj?.ToString() into key where key != null where !key.Equals("didTutorial") select key)
                            toRemove[key] = null;

                        if (toRemove.Count > 0)
                            player.SetCustomProperties(toRemove);
                    }
                }
            }

        }

        public static void BypassModCheckersAll()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player == null) continue;

                if (player.CustomProperties == null || player.CustomProperties.Count == 0) return;

                Hashtable toRemove = new Hashtable();

                foreach (var key in from keyObj in player.CustomProperties.Keys.ToList() select keyObj?.ToString() into key where key != null where !key.Equals("didTutorial") select key)
                    toRemove[key] = null;

                if (toRemove.Count > 0)
                    player.SetCustomProperties(toRemove);
            }
        }

        public static void BypassModCheckersAura()
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

            if (nearbyPlayers.Count <= 0) return;
            foreach (var player in nearbyPlayers.Select(nearbyPlayer => nearbyPlayer.GetPhotonPlayer()).Where(player => player != null))
            {
                if (player.CustomProperties == null || player.CustomProperties.Count == 0) return;

                Hashtable toRemove = new Hashtable();

                foreach (var key in from keyObj in player.CustomProperties.Keys.ToList() select keyObj?.ToString() into key where key != null where !key.Equals("didTutorial") select key)
                    toRemove[key] = null;

                if (toRemove.Count > 0)
                    player.SetCustomProperties(toRemove);
            }
        }

        public static void BypassModCheckersOnTouch()
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
                foreach (var player in touchedPlayers.Select(rig => rig.GetPhotonPlayer()).Where(player => player != null))
                {
                    if (player.CustomProperties == null || player.CustomProperties.Count == 0) return;

                    Hashtable toRemove = new Hashtable();

                    foreach (var key in from keyObj in player.CustomProperties.Keys.ToList() select keyObj?.ToString() into key where key != null where !key.Equals("didTutorial") select key)
                        toRemove[key] = null;

                    if (toRemove.Count > 0)
                        player.SetCustomProperties(toRemove);
                }
            }
        }

        public static void BreakModCheckersGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > customPropertyDelay)
                    {
                        customPropertyDelay = Time.time + 0.25f;

                        Hashtable props = new Hashtable();
                        foreach (string mod in Visuals.modDictionary.Keys)
                            props[mod] = true;

                        gunTarget.GetPhotonPlayer().SetCustomProperties(props);
                    }
                }
            }
        }

        public static void BreakModCheckersAll()
        {
            Hashtable props = new Hashtable();
            foreach (string mod in Visuals.modDictionary.Keys)
                props[mod] = true;

            foreach (Player player in PhotonNetwork.PlayerList)
                player.SetCustomProperties(props);
        }

        public static void BreakModCheckersAura()
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
                    Hashtable props = new Hashtable();
                    foreach (string mod in Visuals.modDictionary.Keys)
                        props[mod] = true;

                    nearbyPlayer.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void BreakModCheckersOnTouch()
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
                    Hashtable props = new Hashtable();
                    foreach (string mod in Visuals.modDictionary.Keys)
                        props[mod] = true;

                    rig.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void GamemodeIncludeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > customPropertyDelay)
                    {
                        customPropertyDelay = Time.time + 0.25f;

                        Hashtable props = new Hashtable { { "didTutorial", true } };
                        gunTarget.GetPhotonPlayer().SetCustomProperties(props);
                    }
                }
            }
        }

        public static void GamemodeIncludeAll()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Hashtable props = new Hashtable { { "didTutorial", true } };
                player.SetCustomProperties(props);
            }
        }

        public static void GamemodeIncludeAura()
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
                    Hashtable props = new Hashtable { { "didTutorial", true } };
                    nearbyPlayer.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void GamemodeIncludeOnTouch()
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
                    Hashtable props = new Hashtable { { "didTutorial", true } };
                    rig.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void GamemodeExcludeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > customPropertyDelay)
                    {
                        customPropertyDelay = Time.time + 0.25f;

                        Hashtable props = new Hashtable { { "didTutorial", false } };
                        gunTarget.GetPhotonPlayer().SetCustomProperties(props);
                    }
                }
            }
        }

        public static void GamemodeExcludeAll()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Hashtable props = new Hashtable { { "didTutorial", false } };
                player.SetCustomProperties(props);
            }
        }

        public static void GamemodeExcludeAura()
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
                    Hashtable props = new Hashtable { { "didTutorial", false } };
                    nearbyPlayer.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void GamemodeExcludeOnTouch()
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
                    Hashtable props = new Hashtable { { "didTutorial", false } };
                    rig.GetPhotonPlayer().SetCustomProperties(props);
                }
            }
        }

        public static void BreakGamemode(bool breaking)
        {
            Hashtable props = new Hashtable { { "didTutorial", !breaking } };

            foreach (Player player in PhotonNetwork.PlayerList)
                player.SetCustomProperties(props);
        }

        public static void BreakNetworkTriggers()
        {
            string queue = Buttons.GetIndex("Switch to Modded Gamemode").enabled ? GorillaComputer.instance.currentQueue + "MODDED_" : GorillaComputer.instance.currentQueue;
            Hashtable hash = new Hashtable
            {
                { "gameMode", string.Join("", GorillaComputer.instance.allowedMapsToJoin) + queue + GorillaComputer.instance.currentGameMode.Value }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        public static void KickNetworkTriggers()
        {
            if (NetworkSystem.Instance.SessionIsPrivate)
                Overpowered.SetRoomStatus(false);

            Hashtable hash = new Hashtable
            {
                { "gameMode", GorillaComputer.instance.currentGameMode.Value }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
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
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                return;
            }

            if (disablePatchCoroutine != null)
                disablePatchCoroutine = CoroutineManager.instance.StartCoroutine(DisablePatch());

            GameModePatch.enabled = true;
            NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.NetView.gameObject);

            string queue = moddedGamemode ? GorillaComputer.instance.currentQueue + "MODDED_" : GorillaComputer.instance.currentQueue;

            Hashtable hash = new Hashtable
            {
                { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.networkZone + queue + gamemode }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

            GameMode.activeGameMode.StopPlaying();
            GameMode.activeGameMode = null;
            GameMode.activeNetworkHandler = null;

            GameMode.LoadGameMode(gamemode.ToString());
        }
    }
}
