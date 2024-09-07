using ExitGames.Client.Photon;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using System;
using UnityEngine;
using Valve.VR;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;
using UnityEngine.Animations.Rigging;
using GorillaTagScripts;

namespace iiMenu.Mods
{
    internal class Advantages
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
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    GetIndex("Tag Self").enabled = false;
                }
                else
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(rig))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;

                            GorillaTagger.Instance.offlineVRRig.transform.position = rig.rightHandTransform.position;
                            GorillaTagger.Instance.myVRRig.transform.position = rig.rightHandTransform.position;
                            if (GetIndex("Obnoxious Tag").enabled)
                            {
                                Quaternion rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
                                GorillaTagger.Instance.offlineVRRig.transform.rotation = rotation;
                                GorillaTagger.Instance.myVRRig.transform.rotation = rotation;

                                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                                GorillaTagger.Instance.offlineVRRig.leftIndex.calcT = 0f;
                                GorillaTagger.Instance.offlineVRRig.leftMiddle.calcT = 0f;
                                GorillaTagger.Instance.offlineVRRig.leftThumb.calcT = 0f;

                                GorillaTagger.Instance.offlineVRRig.leftIndex.LerpFinger(1f, false);
                                GorillaTagger.Instance.offlineVRRig.leftMiddle.LerpFinger(1f, false);
                                GorillaTagger.Instance.offlineVRRig.leftThumb.LerpFinger(1f, false);

                                GorillaTagger.Instance.offlineVRRig.rightIndex.calcT = 0f;
                                GorillaTagger.Instance.offlineVRRig.rightMiddle.calcT = 0f;
                                GorillaTagger.Instance.offlineVRRig.rightThumb.calcT = 0f;

                                GorillaTagger.Instance.offlineVRRig.rightIndex.LerpFinger(1f, false);
                                GorillaTagger.Instance.offlineVRRig.rightMiddle.LerpFinger(1f, false);
                                GorillaTagger.Instance.offlineVRRig.rightThumb.LerpFinger(1f, false);
                            }
                        }
                    }
                }
            }
        }

        public static void UntagSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Important.Reconnect();
                NoTagOnJoin();
            }
            else
            {
                RemoveInfected(PhotonNetwork.LocalPlayer);
            }
            GorillaLocomotion.Player.Instance.disableMovement = false;
        }

        public static void AntiTag()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    UntagSelf();
                }
            } else
            {
                NoTagOnJoin();
            }
        }

        public static void UntagAll()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                {
                    RemoveInfected(v);
                }
            }
        }

        public static float spamtagdelay = -1f;
        public static void SpamTagSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                    {
                        RemoveInfected(PhotonNetwork.LocalPlayer);
                    } else
                    {
                        AddInfected(PhotonNetwork.LocalPlayer);
                    }
                }
            }
        }

        public static void SpamTagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                    else
                    {
                        if (Time.time > spamtagdelay)
                        {
                            spamtagdelay = Time.time + 0.1f;
                            if (InfectedList().Contains(RigManager.GetPlayerFromVRRig(whoCopy)))
                            {
                                RemoveInfected(RigManager.GetPlayerFromVRRig(whoCopy));
                            }
                            else
                            {
                                AddInfected(RigManager.GetPlayerFromVRRig(whoCopy));
                            }
                        }
                    }
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && !PlayerIsTagged(possibly))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            isCopying = true;
                            whoCopy = possibly;
                        }
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void SpamTagAll()
        {
            if (!PhotonNetwork.IsMasterClient)
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
                        {
                            AddInfected(v);
                        } else
                        {
                            RemoveInfected(v);
                        }
                    }
                }
            }
        }

        public static void TagLag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
            else
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                GorillaAmbushManager ambman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
                if (GorillaGameManager.instance.GameModeName().ToLower().Contains("ambush") || GorillaGameManager.instance.GameModeName().ToLower().Contains("stealth"))
                {
                    ambman.tagCoolDown = 2147483647f;
                }
                else
                {
                    tagman.tagCoolDown = 2147483647f;
                }
            }
        }

        public static void NahTagLag()
        {
            GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
            GorillaAmbushManager ambman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Stealth Manager").GetComponent<GorillaAmbushManager>();
            if (GorillaGameManager.instance.GameModeName().ToLower().Contains("ambush") || GorillaGameManager.instance.GameModeName().ToLower().Contains("stealth"))
            {
                ambman.tagCoolDown = 5f;
            }
            else
            {
                tagman.tagCoolDown = 5f;
            }
        }

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
                Vector3 notthem = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                float distance = Vector3.Distance(they, notthem);

                if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
                {
                    if (rightHand == true) { GorillaLocomotion.Player.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.Player.Instance.leftControllerTransform.position = they; }
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
                    Vector3 notthem = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                    float distance = Vector3.Distance(they, notthem);

                    if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
                    {
                        if (rightHand == true) { GorillaLocomotion.Player.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.Player.Instance.leftControllerTransform.position = they; }
                    }
                }
            }
        }

        public static bool lastj = false;
        public static bool jta = false;
        public static void JoystickTagAura()
        {
            bool l = SteamVR_Actions.gorillaTag_RightJoystickClick.state;
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
                    Vector3 notthem = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                    float distance = Vector3.Distance(they, notthem);

                    if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) && !PlayerIsTagged(vrrig) && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
                    {
                        if (rightHand == true) { GorillaLocomotion.Player.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.Player.Instance.leftControllerTransform.position = they; }
                    }
                }
            }
        }

        public static void TagReach()
        {
            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                Patches.SphereCastPatch.patchEnabled = true;
                Patches.SphereCastPatch.overrideRadius = tagReachDistance;
                if (GetIndex("Visualize Tag Reach").enabled)
                {
                    VisualizeAura(GorillaTagger.Instance.leftHandTransform.position, tagReachDistance, GetBGColor(0f));
                    VisualizeAura(GorillaTagger.Instance.rightHandTransform.position, tagReachDistance, GetBGColor(0f));
                }
            } else
            {
                Patches.SphereCastPatch.patchEnabled = false;
            }
        }

        public static void DisableTagReach()
        {
            Patches.SphereCastPatch.patchEnabled = false;
        }

        /*public static void RPCTagAura()
        {
            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                {
                    VRRig rig = GorillaGameManager.instance.FindPlayerVRRig(player);
                    if (!PlayerIsTagged(rig)})
                    {
                        if (Time.time > TagAuraDelay)
                        {
                            float distance = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, rig.transform.position);
                            if (distance < GorillaGameManager.instance.tagDistanceThreshold)
                            {
                                PhotonView.Get(GorillaGameManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[]
                                {
                                                player
                                });
                                RPCProtection();
                            }

                            TagAuraDelay = Time.time + 0.1f;
                        }
                    }
                }
            }
        }*/

        public static void TagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    if (!PlayerIsTagged(whoCopy))
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = false;

                        if (!GetIndex("Obnoxious Tag").enabled)
                        {
                            GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position - new Vector3(0f, 3f, 0f);
                            GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position - new Vector3(0f, 3f, 0f);
                        } else
                        {
                            Vector3 position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                            GorillaTagger.Instance.offlineVRRig.transform.position = position;
                            GorillaTagger.Instance.myVRRig.transform.position = position;

                            GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                            GorillaTagger.Instance.offlineVRRig.leftIndex.calcT = 0f;
                            GorillaTagger.Instance.offlineVRRig.leftMiddle.calcT = 0f;
                            GorillaTagger.Instance.offlineVRRig.leftThumb.calcT = 0f;

                            GorillaTagger.Instance.offlineVRRig.leftIndex.LerpFinger(1f, false);
                            GorillaTagger.Instance.offlineVRRig.leftMiddle.LerpFinger(1f, false);
                            GorillaTagger.Instance.offlineVRRig.leftThumb.LerpFinger(1f, false);

                            GorillaTagger.Instance.offlineVRRig.rightIndex.calcT = 0f;
                            GorillaTagger.Instance.offlineVRRig.rightMiddle.calcT = 0f;
                            GorillaTagger.Instance.offlineVRRig.rightThumb.calcT = 0f;

                            GorillaTagger.Instance.offlineVRRig.rightIndex.LerpFinger(1f, false);
                            GorillaTagger.Instance.offlineVRRig.rightMiddle.LerpFinger(1f, false);
                            GorillaTagger.Instance.offlineVRRig.rightThumb.LerpFinger(1f, false);
                        }

                        GorillaLocomotion.Player.Instance.rightControllerTransform.position = whoCopy.transform.position;
                        /*
                        GameObject l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

                        l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        l.transform.position = GorillaTagger.Instance.leftHandTransform.position;

                        GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
                        UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

                        r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        r.transform.position = GorillaTagger.Instance.rightHandTransform.position;

                        l.GetComponent<Renderer>().material.color = bgColorA;
                        r.GetComponent<Renderer>().material.color = bgColorA;

                        UnityEngine.Object.Destroy(l, Time.deltaTime);
                        UnityEngine.Object.Destroy(r, Time.deltaTime);
                        */
                    }
                    else
                    {
                        isCopying = false;
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                    }
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && !PlayerIsTagged(possibly))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            AddInfected(RigManager.GetPlayerFromVRRig(possibly));
                        }
                        else
                        {
                            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                            {
                                isCopying = true;
                                whoCopy = possibly;
                            }
                        }
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void UntagGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && PlayerIsTagged(possibly))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            RemoveInfected(RigManager.GetPlayerFromVRRig(possibly));
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position = Ray.point + new Vector3(0f, 0.3f, 0f);
                }
            }
        }

        public static void TagAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                {
                    AddInfected(v);
                }
                GetIndex("Tag All").enabled = false;
            }
            else
            {
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
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
                                GorillaTagger.Instance.offlineVRRig.enabled = false;

                                if (!GetIndex("Obnoxious Tag").enabled)
                                {
                                    GorillaTagger.Instance.offlineVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                                    GorillaTagger.Instance.myVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                                }
                                else
                                {
                                    Vector3 position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                                    
                                    GorillaTagger.Instance.offlineVRRig.transform.position = position;
                                    GorillaTagger.Instance.myVRRig.transform.position = position;

                                    Quaternion rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                                    GorillaTagger.Instance.offlineVRRig.transform.rotation = rotation;
                                    GorillaTagger.Instance.myVRRig.transform.rotation = rotation;

                                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                                    GorillaTagger.Instance.offlineVRRig.leftIndex.calcT = 0f;
                                    GorillaTagger.Instance.offlineVRRig.leftMiddle.calcT = 0f;
                                    GorillaTagger.Instance.offlineVRRig.leftThumb.calcT = 0f;

                                    GorillaTagger.Instance.offlineVRRig.leftIndex.LerpFinger(1f, false);
                                    GorillaTagger.Instance.offlineVRRig.leftMiddle.LerpFinger(1f, false);
                                    GorillaTagger.Instance.offlineVRRig.leftThumb.LerpFinger(1f, false);

                                    GorillaTagger.Instance.offlineVRRig.rightIndex.calcT = 0f;
                                    GorillaTagger.Instance.offlineVRRig.rightMiddle.calcT = 0f;
                                    GorillaTagger.Instance.offlineVRRig.rightThumb.calcT = 0f;

                                    GorillaTagger.Instance.offlineVRRig.rightIndex.LerpFinger(1f, false);
                                    GorillaTagger.Instance.offlineVRRig.rightMiddle.LerpFinger(1f, false);
                                    GorillaTagger.Instance.offlineVRRig.rightThumb.LerpFinger(1f, false);
                                }

                                GorillaLocomotion.Player.Instance.rightControllerTransform.position = vrrig.transform.position;
                            }
                        }
                    }
                    else
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                        GetIndex("Tag All").enabled = false;
                    }
                }
            }
        }

        public static void HuntTagAll()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            if (!GorillaLocomotion.Player.Instance.disableMovement)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(target);
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                if (!GetIndex("Obnoxious Tag").enabled)
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                    GorillaTagger.Instance.myVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                }
                else
                {
                    Vector3 position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                    GorillaTagger.Instance.offlineVRRig.transform.position = position;
                    GorillaTagger.Instance.myVRRig.transform.position = position;

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = vrrig.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                    GorillaTagger.Instance.offlineVRRig.leftIndex.calcT = 0f;
                    GorillaTagger.Instance.offlineVRRig.leftMiddle.calcT = 0f;
                    GorillaTagger.Instance.offlineVRRig.leftThumb.calcT = 0f;

                    GorillaTagger.Instance.offlineVRRig.leftIndex.LerpFinger(1f, false);
                    GorillaTagger.Instance.offlineVRRig.leftMiddle.LerpFinger(1f, false);
                    GorillaTagger.Instance.offlineVRRig.leftThumb.LerpFinger(1f, false);

                    GorillaTagger.Instance.offlineVRRig.rightIndex.calcT = 0f;
                    GorillaTagger.Instance.offlineVRRig.rightMiddle.calcT = 0f;
                    GorillaTagger.Instance.offlineVRRig.rightThumb.calcT = 0f;

                    GorillaTagger.Instance.offlineVRRig.rightIndex.LerpFinger(1f, false);
                    GorillaTagger.Instance.offlineVRRig.rightMiddle.LerpFinger(1f, false);
                    GorillaTagger.Instance.offlineVRRig.rightThumb.LerpFinger(1f, false);
                }

                GorillaLocomotion.Player.Instance.rightControllerTransform.position = vrrig.transform.position;
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Everyone is tagged!</color>");
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GetIndex("Hunt Tag All").enabled = false;
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
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
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
                GorillaTagger.Instance.offlineVRRig.enabled = true;
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
                if (!GorillaLocomotion.Player.Instance.disableMovement)
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

        /*
        public static void EnableRemoveChristmasLights()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("holidaylights"))
                {
                    g.SetActive(false);
                    lights.Add(g);
                }
            }
        }

        public static void DisableRemoveChristmasLights()
        {
            foreach (GameObject l in lights)
            {
                l.SetActive(true);
            }
            lights.Clear();
        }

        public static void EnableRemoveChristmasDecorations()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("WinterJan2024"))
                {
                    g.SetActive(false);
                    holidayobjects.Add(g);
                }
            }
        }

        public static void DisableRemoveChristmasDecorations()
        {
            foreach (GameObject h in holidayobjects)
            {
                h.SetActive(true);
            }
            holidayobjects.Clear();
        }
        */
    }
}
