/*
 * ii's Stupid Menu  Mods/Advantages.cs
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
using GorillaLocomotion;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.GameModeUtilities;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;

namespace iiMenu.Mods
{
    public static class Advantages
    {
        public static bool instantTag;

        public static void TagSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddInfected(PhotonNetwork.LocalPlayer);
                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> You have been tagged.");
                Buttons.GetIndex("Tag Self").enabled = false;
            }
            else
            {
                if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> You have been tagged.");
                    VRRig.LocalRig.enabled = true;
                    Buttons.GetIndex("Tag Self").enabled = false;

                    if (instantTag)
                        SerializePatch.OverrideSerialization = null;
                }
                else
                {
                    VRRig rig = GorillaParent.instance.vrrigs
                        .Where(r => !r.IsLocal() && r.IsTagged())
                        .OrderBy(r => Vector3.Distance(
                                        r.transform.position,
                                        GorillaTagger.Instance.headCollider.transform.position)
                                    + r.LatestVelocity().magnitude)
                        .FirstOrDefault();

                    if (instantTag)
                    {
                        SerializePatch.OverrideSerialization = () =>
                        {
                            if (VRRig.LocalRig.IsTagged())
                                return true;

                            MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                            Vector3 positionArchive = VRRig.LocalRig.transform.position;
                            VRRig.LocalRig.transform.position = rig.rightHandTransform.transform.position;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber, rig.GetPlayer().ActorNumber } });

                            RPCProtection();
                            VRRig.LocalRig.transform.position = positionArchive;

                            return false;
                        };
                    } else
                    {
                        if (!rig.IsTagged()) return;
                        VRRig.LocalRig.enabled = false;
                        if (rig != null) VRRig.LocalRig.transform.position = rig.rightHandTransform.position;

                        if (!Buttons.GetIndex("Obnoxious Tag").enabled) return;
                        Quaternion rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                        VRRig.LocalRig.transform.rotation = rotation;

                        VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                        VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + RandomVector3();
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + RandomVector3();

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                        VRRig.LocalRig.leftIndex.calcT = 0f;
                        VRRig.LocalRig.leftMiddle.calcT = 0f;
                        VRRig.LocalRig.leftThumb.calcT = 0f;

                        VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                        VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                        VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                        VRRig.LocalRig.rightIndex.calcT = 0f;
                        VRRig.LocalRig.rightMiddle.calcT = 0f;
                        VRRig.LocalRig.rightThumb.calcT = 0f;

                        VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                        VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                        VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                    }
                }
            }
        }

        public static void UntagSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                Important.Reconnect();
                NoTagOnJoin();
            }
            else
                RemoveInfected(PhotonNetwork.LocalPlayer);
            
            GTPlayer.Instance.disableMovement = false;
        }

        public static void AntiTag()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (!ReportTagPatch.invinciblePlayers.Contains(NetworkSystem.Instance.LocalPlayer))
                        ReportTagPatch.invinciblePlayers.Add(NetworkSystem.Instance.LocalPlayer);
                } else
                {
                    if (VRRig.LocalRig.IsTagged())
                        UntagSelf();
                }
            }
            else
            {
                NoTagOnJoin();
                ReportTagPatch.invinciblePlayers.Clear();
            }
        }

        public static void UntagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                foreach (Player v in PhotonNetwork.PlayerList)
                    RemoveInfected(v);
            }
        }

        public static float spamTagDelay;
        public static void SpamTagSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                if (!(Time.time > spamTagDelay)) return;
                spamTagDelay = Time.time + 0.1f;
                if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                    RemoveInfected(PhotonNetwork.LocalPlayer);
                else
                    AddInfected(PhotonNetwork.LocalPlayer);
            }
        }

        public static void SpamTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (!NetworkSystem.Instance.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                    {
                        if (Time.time > spamTagDelay)
                        {
                            spamTagDelay = Time.time + 0.1f;
                            if (InfectedList().Contains(GetPlayerFromVRRig(lockTarget)))
                                RemoveInfected(GetPlayerFromVRRig(lockTarget));
                            else
                                AddInfected(GetPlayerFromVRRig(lockTarget));
                        }
                    }
                }

                if (!GetGunInput(true)) return;
                VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (!gunTarget || gunTarget.IsLocal()) return;
                if (!PhotonNetwork.IsMasterClient) return;
                gunLocked = true;
                lockTarget = gunTarget;
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void SpamTagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                if (!(Time.time > spamTagDelay)) return;
                spamTagDelay = Time.time + 0.1f;
                foreach (Player v in PhotonNetwork.PlayerList)
                {
                    if (InfectedList().Contains(v))
                        AddInfected(v);
                    else
                        RemoveInfected(v);
                }
            }
        }

        public static void TagLagGun()
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
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (lockTarget != null)
                                ReportTagPatch.blacklistedPlayers.Remove(GetPlayerFromVRRig(lockTarget));

                            gunLocked = true;
                            lockTarget = gunTarget;

                            ReportTagPatch.blacklistedPlayers.Add(GetPlayerFromVRRig(gunTarget));

                        } else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    ReportTagPatch.blacklistedPlayers.Remove(GetPlayerFromVRRig(lockTarget));
                }
            }
        }

        public static void GiveTagLagGun()
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
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (lockTarget != null)
                                ReportTagPatch.invinciblePlayers.Remove(GetPlayerFromVRRig(lockTarget));

                            gunLocked = true;
                            lockTarget = gunTarget;

                            ReportTagPatch.invinciblePlayers.Add(GetPlayerFromVRRig(gunTarget));

                        }
                        else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    ReportTagPatch.invinciblePlayers.Remove(GetPlayerFromVRRig(lockTarget));
                }
            }
        }

        public static void SetTagCooldown(float value)
        {
            if (!NetworkSystem.Instance.IsMasterClient || !NetworkSystem.Instance.InRoom)
                Overpowered.LagMasterClient();
            else
            {
                GorillaTagManager tagman = (GorillaTagManager)GorillaGameManager.instance;
                tagman.tagCoolDown = value;
            }
        }

        public static float tagAuraDistance = 1.666f;
        public static int tagAuraIndex = 1;

        public static void ChangeTagAuraRange(bool positive = true)
        {
            string[] names = {
                "Short",
                "Normal",
                "Far",
                "Maximum"
            };
            float[] distances = {
                0.777f,
                1.666f,
                3f,
                5.5f
            };

            if (positive)
                tagAuraIndex++;
            else
                tagAuraIndex--;

            tagAuraIndex %= names.Length;
            if (tagAuraIndex < 0)
                tagAuraIndex = names.Length - 1;

            tagAuraDistance = distances[tagAuraIndex];
            Buttons.GetIndex("ctaRange").overlapText = "Change Tag Aura Range <color=grey>[</color><color=green>"+names[tagAuraIndex]+"</color><color=grey>]</color>";
        }

        public static int tagRangeIndex;
        private static float tagReachDistance = 0.3f;
        public static void ChangeTagReachDistance(bool positive = true)
        {
            string[] names = {
                "Unnoticable",
                "Normal",
                "Far",
                "Maximum"
            };

            float[] distances = {
                0.3f,
                0.5f,
                1f,
                3f
            };

            if (positive)
                tagRangeIndex++;
            else
                tagRangeIndex--;

            tagRangeIndex %= names.Length;
            if (tagRangeIndex < 0)
                tagRangeIndex = names.Length - 1;

            tagReachDistance = distances[tagRangeIndex];
            Buttons.GetIndex("ctrRange").overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>" + names[tagRangeIndex] + "</color><color=grey>]</color>";
        }

        public static void TagAura()
        {
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => VRRig.LocalRig.IsTagged() && !vrrig.IsTagged() && !GTPlayer.Instance.disableMovement && Vector3.Distance(vrrig.headMesh.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < tagAuraDistance))
                ReportTag(vrrig);
        }

        public static void GripTagAura()
        {
            if (rightGrab)
                TagAura();
        }

        public static void TagAuraPlayer(VRRig giving)
        {
            foreach (var vrrig in from vrrig in GorillaParent.instance.vrrigs let distance = Vector3.Distance(vrrig.headMesh.transform.position, giving.transform.position) where giving.IsTagged() && !vrrig.IsTagged() && !GTPlayer.Instance.disableMovement && distance < tagAuraDistance && !VRRig.LocalRig.IsLocal() && VRRig.LocalRig.IsTagged() select vrrig)
                TagPlayer(GetPlayerFromVRRig(vrrig));
        }

        public static void TagAuraGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    TagAuraPlayer(lockTarget);

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

        public static void TagAuraAll()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                TagAuraPlayer(vrrig);
        }

        public static void TagReach()
        {
            if (!VRRig.LocalRig.IsTagged()) return;
            GorillaTagger.Instance.maxTagDistance = float.MaxValue;

            GorillaTagger.Instance.tagRadiusOverride = tagReachDistance;
            GorillaTagger.Instance.tagRadiusOverrideFrame = Time.frameCount + 16;

            if (!Buttons.GetIndex("Visualize Tag Reach").enabled) return;
            Visuals.VisualizeAura(GorillaTagger.Instance.leftHandTransform.position, tagReachDistance, backgroundColor.GetCurrentColor(), -149286);
            Visuals.VisualizeAura(GorillaTagger.Instance.rightHandTransform.position, tagReachDistance, backgroundColor.GetCurrentColor(), -149285);
        }

        public static bool ValidateTag(VRRig Rig) =>
            Vector3.Distance(ServerSyncPos, Rig.transform.position) < 6f;

        public static void TagGun()
        {
            if (instantTag)
            {
                InstantTagGun();
                return;
            }

            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (!lockTarget.IsTagged())
                    {
                        VRRig.LocalRig.enabled = false;

                        if (!Buttons.GetIndex("Obnoxious Tag").enabled)
                            VRRig.LocalRig.transform.position = lockTarget.transform.position - new Vector3(0f, 3f, 0f);
                        else
                        {
                            Vector3 position = lockTarget.transform.position + RandomVector3();

                            VRRig.LocalRig.transform.position = position;

                            VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                            VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();

                            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                            VRRig.LocalRig.leftIndex.calcT = 0f;
                            VRRig.LocalRig.leftMiddle.calcT = 0f;
                            VRRig.LocalRig.leftThumb.calcT = 0f;

                            VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                            VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                            VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                            VRRig.LocalRig.rightIndex.calcT = 0f;
                            VRRig.LocalRig.rightMiddle.calcT = 0f;
                            VRRig.LocalRig.rightThumb.calcT = 0f;

                            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                            VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                        }

                        if (ValidateTag(lockTarget))
                            ReportTag(lockTarget);
                    }
                    else
                    {
                        gunLocked = false;
                        VRRig.LocalRig.enabled = true;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (PhotonNetwork.IsMasterClient)
                            AddInfected(GetPlayerFromVRRig(gunTarget));
                        else
                        {
                            if (!VRRig.LocalRig.IsTagged()) return;
                            gunLocked = true;
                            lockTarget = gunTarget;
                        }
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        private static float reportTagDelay;
        public static void ReportTag(VRRig rig)
        {
            if (Time.time > reportTagDelay)
            {
                reportTagDelay = Time.time + 0.1f;
                GameMode.ReportTag(GetPlayerFromVRRig(rig));
            }
        }

        public static void TagPlayer(NetPlayer player)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddInfected(player);
                return;
            }

            if (!VRRig.LocalRig.IsTagged())
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
                Buttons.GetIndex("Tag Player").enabled = false;
                return;
            }

            if (instantTag)
            {
                InstantTagPlayer(player);
                Buttons.GetIndex("Tag Player").enabled = false;
                return;
            }

            VRRig targetRig = GetVRRigFromPlayer(player);
            if (!targetRig.IsTagged())
            {
                VRRig.LocalRig.enabled = false;

                if (!Buttons.GetIndex("Obnoxious Tag").enabled)
                    VRRig.LocalRig.transform.position = targetRig.transform.position - new Vector3(0f, 3f, 0f);
                else
                {
                    Vector3 position = targetRig.transform.position + RandomVector3();

                    VRRig.LocalRig.transform.position = position;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                    VRRig.LocalRig.leftIndex.calcT = 0f;
                    VRRig.LocalRig.leftMiddle.calcT = 0f;
                    VRRig.LocalRig.leftThumb.calcT = 0f;

                    VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                    VRRig.LocalRig.rightIndex.calcT = 0f;
                    VRRig.LocalRig.rightMiddle.calcT = 0f;
                    VRRig.LocalRig.rightThumb.calcT = 0f;

                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                }

                if (ValidateTag(targetRig))
                    ReportTag(targetRig);
            }
            else
                Buttons.GetIndex("Tag Player").enabled = false;
        }

        public static void UntagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && gunTarget.IsTagged())
                    {
                        if (PhotonNetwork.IsMasterClient)
                            RemoveInfected(GetPlayerFromVRRig(gunTarget));
                        else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
        }

        public static void FlickTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun(GTPlayer.Instance.locomotionEnabledLayers);
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GTPlayer.Instance.GetControllerTransform(false).position = NewPointer.transform.position;

                    if (Vector3.Distance(GTPlayer.Instance.GetControllerTransform(false).position, GorillaTagger.Instance.bodyCollider.transform.position) > 4f)
                        GTPlayer.Instance.GetControllerTransform(false).position = GorillaTagger.Instance.bodyCollider.transform.position + (GTPlayer.Instance.GetControllerTransform(false).position - GorillaTagger.Instance.bodyCollider.transform.position) * 4f;
                }
            }
        }

        public static void TagAll()
        {
            if (GorillaGameManager.instance.GameType() == GameModeType.HuntDown)
            {
                HuntTagAll();
                return;
            }

            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (Player v in PhotonNetwork.PlayerList)
                    AddInfected(v);
                
                Buttons.GetIndex("Tag All").enabled = false;
                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
            }
            else
            {
                if (instantTag)
                {
                    InstantTagAll();
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
                    Buttons.GetIndex("Tag All").enabled = false;
                    return;
                }

                if (!VRRig.LocalRig.IsTagged())
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
                    Buttons.GetIndex("Tag All").enabled = false;
                }
                else
                {
                    bool isInfectedPlayers = GorillaParent.instance.vrrigs.Any(vrrig => !vrrig.IsTagged());
                    if (isInfectedPlayers)
                    {
                        foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.IsTagged()))
                        {
                            VRRig.LocalRig.enabled = false;

                            if (!Buttons.GetIndex("Obnoxious Tag").enabled)
                                VRRig.LocalRig.transform.position = vrrig.transform.position - new Vector3(0f, 3f, 0f);
                            else
                            {
                                Vector3 position = vrrig.transform.position + RandomVector3();
                                    
                                VRRig.LocalRig.transform.position = position;
                                VRRig.LocalRig.transform.rotation = RandomQuaternion();

                                VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                                VRRig.LocalRig.leftHand.rigTarget.transform.position = vrrig.transform.position + RandomVector3();
                                VRRig.LocalRig.rightHand.rigTarget.transform.position = vrrig.transform.position + RandomVector3();

                                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                                VRRig.LocalRig.leftIndex.calcT = 0f;
                                VRRig.LocalRig.leftMiddle.calcT = 0f;
                                VRRig.LocalRig.leftThumb.calcT = 0f;

                                VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                                VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                                VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                                VRRig.LocalRig.rightIndex.calcT = 0f;
                                VRRig.LocalRig.rightMiddle.calcT = 0f;
                                VRRig.LocalRig.rightThumb.calcT = 0f;

                                VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                                VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                                VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                            }

                            if (ValidateTag(vrrig))
                                ReportTag(vrrig);
                        }
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
                        VRRig.LocalRig.enabled = true;
                        Buttons.GetIndex("Tag All").enabled = false;
                    }
                }
            }
        }

        public static void InstantTagPlayer(NetPlayer Target)
        {
            if (!VRRig.LocalRig.IsTagged() || Target.VRRig().IsTagged())
                return;

            Vector3 archiveRigPosition = VRRig.LocalRig.transform.position;
            VRRig.LocalRig.transform.position = GetVRRigFromPlayer(Target).transform.position;

            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber } });
            GameMode.ReportTag(Target);

            VRRig.LocalRig.transform.position = archiveRigPosition;

            RPCProtection();
        }

        private static float tagGunDelay;
        public static void InstantTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > tagGunDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        tagGunDelay = Time.time + 0.2f;
                        InstantTagPlayer(NetPlayerToPlayer(GetPlayerFromVRRig(gunTarget)));
                    }
                }
            }
        }

        public static void InstantTagAll()
        {
            if (!VRRig.LocalRig.IsTagged())
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
                return;
            }

            Vector3 archiveRigPosition = VRRig.LocalRig.transform.position;

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.IsTagged()))
            {
                VRRig.LocalRig.transform.position = vrrig.transform.position;
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber } });
                GameMode.ReportTag(GetPlayerFromVRRig(vrrig));
            }

            VRRig.LocalRig.transform.position = archiveRigPosition;

            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber } });
            RPCProtection();
        }

        public static void HuntTagAll()
        {
            GorillaHuntManager huntComputer = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = huntComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            if (!GTPlayer.Instance.disableMovement)
            {
                VRRig vrrig = GetVRRigFromPlayer(target);
                VRRig.LocalRig.enabled = false;

                if (!Buttons.GetIndex("Obnoxious Tag").enabled)
                    VRRig.LocalRig.transform.position = vrrig.transform.position - new Vector3(0f, 3f, 0f);
                else
                {
                    Vector3 position = vrrig.transform.position + RandomVector3();

                    VRRig.LocalRig.transform.position = position;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = vrrig.transform.position + RandomVector3();
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = vrrig.transform.position + RandomVector3();

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                    VRRig.LocalRig.leftIndex.calcT = 0f;
                    VRRig.LocalRig.leftMiddle.calcT = 0f;
                    VRRig.LocalRig.leftThumb.calcT = 0f;

                    VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                    VRRig.LocalRig.rightIndex.calcT = 0f;
                    VRRig.LocalRig.rightMiddle.calcT = 0f;
                    VRRig.LocalRig.rightThumb.calcT = 0f;

                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                }

                if (ValidateTag(vrrig))
                    ReportTag(vrrig);
            }
            else
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
                VRRig.LocalRig.enabled = true;
                Buttons.GetIndex("Tag All").enabled = false;
                ReloadMenu();
            }
        }

        public static void TagBot()
        {
            if (PhotonNetwork.InRoom)
            {
                if (!VRRig.LocalRig.IsTagged())
                {
                    if (InfectedList().Count > 0)
                        TagSelf();
                }
                else
                {
                    if (InfectedList().Count != PhotonNetwork.PlayerList.Length)
                        TagAll();
                }
            } else
                VRRig.LocalRig.enabled = true;
        }

        public static void NoTagOnJoin()
        {
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("didTutorial", out object obj);

            if (obj == null || (obj is bool @bool && @bool))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    { "didTutorial", false }
                });
            }
        }

        public static void TagOnJoin() =>
            NetworkSystem.Instance.SetMyTutorialComplete();

        public static void ReportAntiTag()
        {
            SerializePatch.OverrideSerialization = () =>
            {
                if (VRRig.LocalRig.IsTagged())
                    return true;

                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 positionArchive = VRRig.LocalRig.transform.position;
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = PhotonNetwork.PlayerList.Where(plr => plr.ActorNumber != PhotonNetwork.MasterClient.ActorNumber).Select(plr => plr.ActorNumber).ToArray() });

                VRRig.LocalRig.transform.position = new Vector3(99999f, 99999f, 99999f);
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber } });

                RPCProtection();
                VRRig.LocalRig.transform.position = positionArchive;

                return false;
            };
        }

        public static void PaintbrawlStartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.StartBattle();
            }
        }

        public static void PaintbrawlEndGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.BattleEnd();
            }
        }

        public static void PaintbrawlRestartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.BattleEnd();
                brawlManager.StartBattle();
            }
        }

        public static float paintbrawlSpamDelay;
        public static void PaintbrawlBalloonSpamSelf()
        {
            if (Time.time < paintbrawlSpamDelay)
                return;

            paintbrawlSpamDelay = Time.time + 0.1f;

            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
            }
        }

        public static void PaintbrawlBalloonSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (!NetworkSystem.Instance.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                    {
                        if (Time.time > paintbrawlSpamDelay)
                        {
                            paintbrawlSpamDelay = Time.time + 0.1f;

                            if (!NetworkSystem.Instance.IsMasterClient)
                                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                            else
                            {
                                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
                            }
                        }
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            gunLocked = true;
                            lockTarget = gunTarget;
                        }
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static void PaintbrawlBalloonSpam()
        {
            if (Time.time < paintbrawlSpamDelay)
                return;

            paintbrawlSpamDelay = Time.time + 0.1f;

            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                foreach (Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = Random.Range(0, 4);
            }
        }

        public static void PaintbrawlKillGun()
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
                        NetPlayer owner = GetPlayerFromVRRig(gunTarget);
                        if (!NetworkSystem.Instance.IsMasterClient)
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                        else
                        {
                            GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                            brawlManager.playerLives[owner.ActorNumber] = 0;
                        }
                    }
                }
            }
        }

        public static int paintbrawlKillIndex;
        public static readonly Dictionary<int, float> paintbrawlKillDelays = new Dictionary<int, float>();
        public static void PaintbrawlKillPlayer(NetPlayer Target)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                if (paintbrawlKillDelays.TryGetValue(Target.ActorNumber, out float lastTime))
                {
                    if (Time.time > lastTime)
                        return;
                }

                paintbrawlKillDelays[Target.ActorNumber] = Time.time + 3.1f;

                VRRig rig = GetVRRigFromPlayer(Target);
                GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, NetPlayerToPlayer(Target), rig.transform.position, paintbrawlKillIndex);
                RPCProtection();

                paintbrawlKillIndex++;
            }
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[Target.ActorNumber] = 0;
            }
        }

        public static void PaintbrawlKillSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                PaintbrawlKillPlayer(NetworkSystem.Instance.LocalPlayer);
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
            }
        }

        public static void PaintbrawlKillAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                PaintbrawlKillPlayer(GetRandomPlayer(false));
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                foreach (Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = 0;
            }
        }

        public static void PaintbrawlReviveGun()
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
                        NetPlayer owner = GetPlayerFromVRRig(gunTarget);
                        if (!NetworkSystem.Instance.IsMasterClient)
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                        else
                        {
                            GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                            brawlManager.playerLives[owner.ActorNumber] = 4;
                        }
                    }
                }
            }
        }

        public static void PaintbrawlReviveSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
            }
        }

        public static void PaintbrawlReviveAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                foreach (Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = 4;
            }
        }

        // Credits to Malachi for the idea
        public static void PaintbrawlNoDelay()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.hitCooldown = 0f;
                brawlManager.tagCoolDown = 0f;
                brawlManager.stunGracePeriod = 0f;
            }
        }

        public static void DisablePaintbrawlNoDelay()
        {
            GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
            brawlManager.hitCooldown = 3f;
            brawlManager.tagCoolDown = 5f;
            brawlManager.stunGracePeriod = 2f;
        }

        public static void PaintbrawlGodMode()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
                GTPlayer.Instance.disableMovement = false;
            }
        }
    }
}
