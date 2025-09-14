using ExitGames.Client.Photon;
using GorillaGameModes;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Advantages
    {
        public static void TagSelf()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddInfected(PhotonNetwork.LocalPlayer);
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You have been tagged.</color>");
                GetIndex("Tag Self").enabled = false;
            }
            else
            {
                if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You have been tagged.</color>");
                    VRRig.LocalRig.enabled = true;
                    GetIndex("Tag Self").enabled = false;
                }
                else
                {
                    VRRig rig = GorillaParent.instance.vrrigs.Where(rig => PlayerIsTagged(rig)).OrderBy(rig => rig.LatestVelocity().magnitude).FirstOrDefault();
                    if (PlayerIsTagged(rig))
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = rig.rightHandTransform.position;

                        if (GetIndex("Obnoxious Tag").enabled)
                        {
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
            
            GorillaLocomotion.GTPlayer.Instance.disableMovement = false;
        }

        public static void AntiTag()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (!Patches.ReportTagPatch.invinciblePlayers.Contains(NetworkSystem.Instance.LocalPlayer))
                        Patches.ReportTagPatch.invinciblePlayers.Add(NetworkSystem.Instance.LocalPlayer);
                } else
                {
                    if (PlayerIsTagged(VRRig.LocalRig))
                        UntagSelf();
                }
            }
            else
            {
                NoTagOnJoin();
                Patches.ReportTagPatch.invinciblePlayers.Clear();
            }
        }

        public static void UntagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                foreach (Player v in PhotonNetwork.PlayerList)
                    RemoveInfected(v);
            }
        }

        public static float spamtagdelay = -1f;
        public static void SpamTagSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                        RemoveInfected(PhotonNetwork.LocalPlayer);
                    else
                        AddInfected(PhotonNetwork.LocalPlayer);
                }
            }
        }

        public static void SpamTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (!NetworkSystem.Instance.IsMasterClient)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    else
                    {
                        if (Time.time > spamtagdelay)
                        {
                            spamtagdelay = Time.time + 0.1f;
                            if (InfectedList().Contains(RigManager.GetPlayerFromVRRig(lockTarget)))
                                RemoveInfected(RigManager.GetPlayerFromVRRig(lockTarget));
                            else
                                AddInfected(RigManager.GetPlayerFromVRRig(lockTarget));
                        }
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !PlayerIsTagged(gunTarget))
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

        public static void SpamTagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    foreach (Player v in PhotonNetwork.PlayerList)
                    {
                        if (InfectedList().Contains(v))
                            AddInfected(v);
                        else
                            RemoveInfected(v);
                    }
                }
            }
        }

        public static void TagLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !PlayerIsTagged(gunTarget))
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (lockTarget != null)
                                Patches.ReportTagPatch.blacklistedPlayers.Remove(RigManager.GetPlayerFromVRRig(lockTarget));

                            gunLocked = true;
                            lockTarget = gunTarget;

                            Patches.ReportTagPatch.blacklistedPlayers.Add(RigManager.GetPlayerFromVRRig(gunTarget));

                        } else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    Patches.ReportTagPatch.blacklistedPlayers.Remove(RigManager.GetPlayerFromVRRig(lockTarget));
                }
            }
        }

        public static void GiveTagLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !PlayerIsTagged(gunTarget))
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (lockTarget != null)
                                Patches.ReportTagPatch.invinciblePlayers.Remove(RigManager.GetPlayerFromVRRig(lockTarget));

                            gunLocked = true;
                            lockTarget = gunTarget;

                            Patches.ReportTagPatch.invinciblePlayers.Add(RigManager.GetPlayerFromVRRig(gunTarget));

                        }
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    Patches.ReportTagPatch.invinciblePlayers.Remove(RigManager.GetPlayerFromVRRig(lockTarget));
                }
            }
        }

        public static void SetTagCooldown(float value)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
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
            string[] names = new string[]
            {
                "Short",
                "Normal",
                "Far",
                "Maximum"
            };
            float[] distances = new float[]
            {
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
            GetIndex("ctaRange").overlapText = "Change Tag Aura Range <color=grey>[</color><color=green>"+names[tagAuraIndex]+"</color><color=grey>]</color>";
        }

        public static int tagRangeIndex;
        private static float tagReachDistance = 0.3f;
        public static void ChangeTagReachDistance(bool positive = true)
        {
            string[] names = new string[]
            {
                "Unnoticable",
                "Normal",
                "Far",
                "Maximum"
            };

            float[] distances = new float[]
            {
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
            GetIndex("ctrRange").overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>" + names[tagRangeIndex] + "</color><color=grey>]</color>";
        }

        public static void TagAura()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(VRRig.LocalRig) && !PlayerIsTagged(vrrig) && !GorillaLocomotion.GTPlayer.Instance.disableMovement && Vector3.Distance(vrrig.headMesh.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < tagAuraDistance)
                    ReportTag(vrrig);
            }
        }

        public static void TagAuraPlayer(VRRig giving)
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                float distance = Vector3.Distance(vrrig.headMesh.transform.position, giving.transform.position);

                if (PlayerIsTagged(giving) && !PlayerIsTagged(vrrig) && !GorillaLocomotion.GTPlayer.Instance.disableMovement && distance < tagAuraDistance && !PlayerIsLocal(vrrig) && PlayerIsTagged(VRRig.LocalRig))
                    TagPlayer(RigManager.GetPlayerFromVRRig(vrrig));
            }
        }

        public static void TagAuraGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                    TagAuraPlayer(lockTarget);

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

        public static void TagAuraAll()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                TagAuraPlayer(vrrig);
        }

        public static void TagReach()
        {
            if (PlayerIsTagged(VRRig.LocalRig))
            {
                GorillaTagger.Instance.maxTagDistance = float.MaxValue;

                GorillaTagger.Instance.tagRadiusOverride = tagReachDistance;
                GorillaTagger.Instance.tagRadiusOverrideFrame = Time.frameCount + 16;

                if (GetIndex("Visualize Tag Reach").enabled)
                {
                    VisualizeAura(GorillaTagger.Instance.leftHandTransform.position, tagReachDistance, backgroundColor.GetCurrentColor(), -149286);
                    VisualizeAura(GorillaTagger.Instance.rightHandTransform.position, tagReachDistance, backgroundColor.GetCurrentColor(), -149285);
                }
            }
        }

        public static bool ValidateTag(VRRig Rig) =>
            Vector3.Distance(ServerSyncPos, Rig.transform.position) < 6f;

        public static void TagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (!PlayerIsTagged(lockTarget))
                    {
                        VRRig.LocalRig.enabled = false;

                        if (!GetIndex("Obnoxious Tag").enabled)
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
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !PlayerIsTagged(gunTarget))
                    {
                        if (PhotonNetwork.IsMasterClient)
                            AddInfected(RigManager.GetPlayerFromVRRig(gunTarget));
                        else
                        {
                            if (PlayerIsTagged(VRRig.LocalRig))
                            {
                                gunLocked = true;
                                lockTarget = gunTarget;
                            }
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
                GameMode.ReportTag(RigManager.GetPlayerFromVRRig(rig));
            }
        }

        public static void TagPlayer(NetPlayer player)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddInfected(player);
                return;
            }

            if (!PlayerIsTagged(VRRig.LocalRig))
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be tagged.</color>");
                Toggle("Tag Player");
                return;
            }

            VRRig targetRig = RigManager.GetVRRigFromPlayer(player);
            if (!PlayerIsTagged(targetRig))
            {
                VRRig.LocalRig.enabled = false;

                if (!GetIndex("Obnoxious Tag").enabled)
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
                Toggle("Tag Player");
        }

        public static void UntagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && PlayerIsTagged(gunTarget))
                    {
                        if (PhotonNetwork.IsMasterClient)
                            RemoveInfected(RigManager.GetPlayerFromVRRig(gunTarget));
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void FlickTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun(GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = NewPointer.transform.position;

                    if (Vector3.Distance(GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position, GorillaTagger.Instance.bodyCollider.transform.position) > 4f)
                        GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position - GorillaTagger.Instance.bodyCollider.transform.position) * 4f;
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
                
                GetIndex("Tag All").enabled = false;
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
            }
            else
            {
                if (!PlayerIsTagged(VRRig.LocalRig))
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be tagged.</color>");
                    GetIndex("Tag All").enabled = false;
                }
                else
                {
                    bool isInfectedPlayers = false;
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig))
                        {
                            isInfectedPlayers = true;
                            break;
                        }
                    }
                    if (isInfectedPlayers)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!PlayerIsTagged(vrrig))
                            {
                                VRRig.LocalRig.enabled = false;

                                if (!GetIndex("Obnoxious Tag").enabled)
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
                    }
                    else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
                        VRRig.LocalRig.enabled = true;
                        GetIndex("Tag All").enabled = false;
                    }
                }
            }
        }

        public static void InstantTagPlayer(NetPlayer Target)
        {
            if (!PlayerIsTagged(VRRig.LocalRig) || PlayerIsTagged(RigManager.GetVRRigFromPlayer(Target)))
                return;

            Vector3 archiveRigPosition = VRRig.LocalRig.transform.position;
            VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(Target).transform.position;

            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });
            GameMode.ReportTag(Target);

            VRRig.LocalRig.transform.position = archiveRigPosition;

            RPCProtection();
        }

        private static float tagGunDelay = 0f;
        public static void InstantTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > tagGunDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        tagGunDelay = Time.time + 0.2f;
                        InstantTagPlayer(RigManager.NetPlayerToPlayer(RigManager.GetPlayerFromVRRig(gunTarget)));
                    }
                }
            }
        }

        public static void InstantTagAll()
        {
            if (!PlayerIsTagged(VRRig.LocalRig))
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be tagged.</color>");
                return;
            }

            Vector3 archiveRigPosition = VRRig.LocalRig.transform.position;

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!PlayerIsTagged(vrrig))
                {
                    VRRig.LocalRig.transform.position = vrrig.transform.position;
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });
                    GameMode.ReportTag(RigManager.GetPlayerFromVRRig(vrrig));
                }
            }

            VRRig.LocalRig.transform.position = archiveRigPosition;

            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });
            RPCProtection();
        }

        public static void HuntTagAll()
        {
            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            if (!GorillaLocomotion.GTPlayer.Instance.disableMovement)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(target);
                VRRig.LocalRig.enabled = false;

                if (!GetIndex("Obnoxious Tag").enabled)
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
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
                VRRig.LocalRig.enabled = true;
                GetIndex("Tag All").enabled = false;
                ReloadMenu();
            }
        }

        public static void TagBot()
        {
            if (PhotonNetwork.InRoom)
            {
                if (!PlayerIsTagged(VRRig.LocalRig))
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
            PlayerPrefs.SetString("tutorial", "nope");
            PlayerPrefs.SetString("didTutorial", "nope");
            Hashtable h = new Hashtable
            {
                { "didTutorial", false }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(h, null, null);
            PlayerPrefs.Save();
        }

        public static void TagOnJoin()
        {
            if (Patches.ReportTagPatch.invinciblePlayers.Contains(NetworkSystem.Instance.LocalPlayer))
                Patches.ReportTagPatch.invinciblePlayers.Remove(NetworkSystem.Instance.LocalPlayer);

            PlayerPrefs.SetString("tutorial", "done");
            PlayerPrefs.SetString("didTutorial", "done");
            Hashtable h = new Hashtable
            {
                { "didTutorial", true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(h, null, null);
            PlayerPrefs.Save();
        }

        public static void ReportAntiTag()
        {
            Patches.SerializePatch.OverrideSerialization = () =>
            {
                if (PlayerIsTagged(VRRig.LocalRig))
                    return true;

                MassSerialize(true, new PhotonView[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 positionArchive = VRRig.LocalRig.transform.position;
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = AllActorNumbersExcept(PhotonNetwork.MasterClient.ActorNumber) });

                VRRig.LocalRig.transform.position = new Vector3(99999f, 99999f, 99999f);
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber } });

                RPCProtection();
                VRRig.LocalRig.transform.position = positionArchive;

                return false;
            };
        }

        public static void PaintbrawlStartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.StartBattle();
            }
        }

        public static void PaintbrawlEndGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.BattleEnd();
            }
        }

        public static void PaintbrawlRestartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.BattleEnd();
                brawlManager.StartBattle();
            }
        }

        public static void PaintbrawlBalloonSpamSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
            }
        }

        public static void PaintbrawlBalloonSpam()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
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
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer owner = RigManager.GetPlayerFromVRRig(gunTarget);
                        if (!NetworkSystem.Instance.IsMasterClient)
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        else
                        {
                            GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                            brawlManager.playerLives[owner.ActorNumber] = 0;
                        }
                    }
                }
            }
        }

        public static void PaintbrawlKillSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
            }
        }

        public static void PaintbrawlKillAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                foreach (Player loln in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[loln.ActorNumber] = 0;
            }
        }

        public static void PaintbrawlReviveGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer owner = RigManager.GetPlayerFromVRRig(gunTarget);
                        if (!NetworkSystem.Instance.IsMasterClient)
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
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
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
            }
        }

        public static void PaintbrawlReviveAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                foreach (Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = 4;
            }
        }

        public static void PaintbrawlGodMode()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
                GorillaLocomotion.GTPlayer.Instance.disableMovement = false;
            }
        }
    }
}
