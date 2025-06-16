using ExitGames.Client.Photon;
using GorillaTagScripts;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Advantages
    {
        public static void TagSelf()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
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
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
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
                if (PlayerIsTagged(VRRig.LocalRig))
                    UntagSelf();
            } else
                NoTagOnJoin();
        }

        public static void UntagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
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
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void SpamTagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                    {
                        if (InfectedList().Contains(v))
                            AddInfected(v);
                        else
                            RemoveInfected(v);
                    }
                }
            }
        }

        public static void SetTagCooldown(float value)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                GorillaAmbushManager ambman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();

                if (GorillaGameManager.instance.GameType() == GorillaGameModes.GameModeType.Ambush || GorillaGameManager.instance.GameType() == GorillaGameModes.GameModeType.Ghost)
                    ambman.tagCoolDown = value;
                else
                    tagman.tagCoolDown = value;
            }
        }

        public static float tagAuraDistance = 1.666f;
        public static int tagAuraIndex = 1;

        public static void ChangeTagAuraRange()
        {
            tagAuraIndex++;
            if (tagAuraIndex > 3)
            {
                tagAuraIndex = 0;
            }
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
                5f
            };

            tagAuraDistance = distances[tagAuraIndex];
            GetIndex("ctaRange").overlapText = "Change Tag Aura Distance <color=grey>[</color><color=green>"+names[tagAuraIndex]+"</color><color=grey>]</color>";
        }

        public static int tagRangeIndex = 0;
        private static float tagReachDistance = 0.3f;
        public static void ChangeTagReachDistance()
        {
            tagRangeIndex++;
            if (tagRangeIndex > 3)
            {
                tagRangeIndex = 0;
            }
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

            tagReachDistance = distances[tagRangeIndex];
            GetIndex("ctrRange").overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>" + names[tagRangeIndex] + "</color><color=grey>]</color>";
        }

        public static void PhysicalTagAura()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                Vector3 they = vrrig.headMesh.transform.position;
                Vector3 notthem = VRRig.LocalRig.head.rigTarget.position;
                float distance = Vector3.Distance(they, notthem);

                if (PlayerIsTagged(VRRig.LocalRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.GTPlayer.Instance.disableMovement == false && distance < tagAuraDistance)
                {
                    if (rightHand == true) { GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position = they; }
                }
            }
        }

        public static void GripTagAura()
        {
            if (rightGrab)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    Vector3 they = vrrig.headMesh.transform.position;
                    Vector3 notthem = VRRig.LocalRig.head.rigTarget.position;
                    float distance = Vector3.Distance(they, notthem);

                    if (PlayerIsTagged(VRRig.LocalRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.GTPlayer.Instance.disableMovement == false && distance < tagAuraDistance)
                    {
                        if (rightHand == true) { GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position = they; }
                    }
                }
            }
        }

        public static bool lastj = false;
        public static bool jta = false;
        public static void JoystickTagAura()
        {
            bool l = rightJoystickClick;
            if (l && !lastj)
            {
                jta = !jta;
            }
            lastj = l;
            if (jta)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    Vector3 they = vrrig.headMesh.transform.position;
                    Vector3 notthem = VRRig.LocalRig.head.rigTarget.position;
                    float distance = Vector3.Distance(they, notthem);

                    if (PlayerIsTagged(VRRig.LocalRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.GTPlayer.Instance.disableMovement == false && distance < tagAuraDistance)
                    {
                        if (rightHand == true) { GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position = they; }
                    }
                }
            }
        }

        public static void TagReach()
        {
            if (PlayerIsTagged(VRRig.LocalRig))
            {
                Patches.SphereCastPatch.enabled = true;
                Patches.SphereCastPatch.overrideRadius = tagReachDistance;
                if (GetIndex("Visualize Tag Reach").enabled)
                {
                    VisualizeAura(GorillaTagger.Instance.leftHandTransform.position, tagReachDistance, GetBGColor(0f));
                    VisualizeAura(GorillaTagger.Instance.rightHandTransform.position, tagReachDistance, GetBGColor(0f));
                }
            } else
                Patches.SphereCastPatch.enabled = false;
        }

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

                        GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = lockTarget.transform.position;
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
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            AddInfected(RigManager.GetPlayerFromVRRig(gunTarget));
                        }
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
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            RemoveInfected(RigManager.GetPlayerFromVRRig(gunTarget));
                        } else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                        }
                    }
                }
            }
        }

        public static void FlickTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = NewPointer.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void TagAll()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                {
                    AddInfected(v);
                }
                GetIndex("Tag All").enabled = false;
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
                    if (isInfectedPlayers == true)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!PlayerIsTagged(vrrig))
                            {
                                VRRig.LocalRig.enabled = false;

                                if (!GetIndex("Obnoxious Tag").enabled)
                                    VRRig.LocalRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
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

                                GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = vrrig.transform.position;
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

        public static void HuntTagAll()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            if (!GorillaLocomotion.GTPlayer.Instance.disableMovement)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(target);
                VRRig.LocalRig.enabled = false;

                if (!GetIndex("Obnoxious Tag").enabled)
                    VRRig.LocalRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
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

                GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position = vrrig.transform.position;
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
                VRRig.LocalRig.enabled = true;
                GetIndex("Hunt Tag All").enabled = false;
                ReloadMenu();
            }
        }

        public static void TagBot()
        {
            if (rightSecondary)
            {
                GetIndex("Tag Bot").enabled = false;
            }
            if (PhotonNetwork.InRoom)
            {
                if (!PlayerIsTagged(VRRig.LocalRig))
                {
                    bool isInfectedPlayers = false;
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig))
                        {
                            isInfectedPlayers = true;
                            break;
                        }
                    }
                    if (isInfectedPlayers)
                    {
                        GetIndex("Tag Self").method.Invoke();
                        GetIndex("Tag All").enabled = false;
                    }
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
                        GetIndex("Tag All").enabled = true;
                    }
                }
            } else
            {
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void HuntTagBot()
        {
            if (rightSecondary)
            {
                GetIndex("Hunt Tag Bot").enabled = false;
            }
            if (PhotonNetwork.InRoom)
            {
                if (!GorillaLocomotion.GTPlayer.Instance.disableMovement)
                {
                    GetIndex("Hunt Tag All").enabled = true;
                }
            }
        }

        public static void NoTagOnJoin()
        {
            PlayerPrefs.SetString("tutorial", "nope");
            PlayerPrefs.SetString("didTutorial", "nope");
            Hashtable h = new Hashtable();
            h.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(h, null, null);
            PlayerPrefs.Save();
        }

        public static void TagOnJoin()
        {
            PlayerPrefs.SetString("tutorial", "done");
            PlayerPrefs.SetString("didTutorial", "done");
            Hashtable h = new Hashtable();
            h.Add("didTutorial", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(h, null, null);
            PlayerPrefs.Save();
        }

        public static void BattleStartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.StartBattle();
            }
        }

        public static void BattleEndGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.BattleEnd();
            }
        }

        public static void BattleRestartGame()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.BattleEnd();
                brawlManager.StartBattle();
            }
        }

        public static void BattleBalloonSpamSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
            }
        }

        public static void BattleBalloonSpam()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = Random.Range(0, 4);
            }
        }

        public static void BattleKillGun()
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
                            GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                            brawlManager.playerLives[owner.ActorNumber] = 0;
                        }
                    }
                }
            }
        }

        public static void BattleKillSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
            }
        }

        public static void BattleKillAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                foreach (Photon.Realtime.Player loln in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[loln.ActorNumber] = 0;
            }
        }

        public static void BattleReviveGun()
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
                            GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                            brawlManager.playerLives[owner.ActorNumber] = 4;
                        }
                    }
                }
            }
        }

        public static void BattleReviveSelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
            }
        }

        public static void BattleReviveAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                    brawlManager.playerLives[player.ActorNumber] = 4;
            }
        }

        public static void BattleGodMode()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            else
            {
                GorillaPaintbrawlManager brawlManager = GameObject.Find("Gorilla Battle Manager").GetComponent<GorillaPaintbrawlManager>();
                brawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
                GorillaLocomotion.GTPlayer.Instance.disableMovement = false;
            }
        }
    }
}
