using ExitGames.Client.Photon;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using System;
using UnityEngine;
using Valve.VR;
using UnityEngine.InputSystem;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Advantages
    {
        public static void TagSelf()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (!gorillaTagManager.currentInfected.Contains(PhotonNetwork.LocalPlayer))
                {
                    gorillaTagManager.AddInfectedPlayer(PhotonNetwork.LocalPlayer);
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You have been tagged!</color>");
                    GetIndex("Tag Self").enabled = false;
                }
            }
            else
            {
                GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (gorillaTagManager.currentInfected.Contains(PhotonNetwork.LocalPlayer))
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>You have been tagged!</color>");
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    GetIndex("Tag Self").enabled = false;
                }
                else
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (rig.mainSkin.material.name.Contains("fected"))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = rig.rightHandTransform.position;
                            GorillaTagger.Instance.myVRRig.transform.position = rig.rightHandTransform.position;
                        }
                    }
                }
            }
        }

        public static void UntagSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                if (tagman.currentInfected.Contains(PhotonNetwork.LocalPlayer))
                {
                    tagman.currentInfected.Remove(PhotonNetwork.LocalPlayer);
                }
            }
        }

        public static void UntagAll()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                {
                    if (tagman.currentInfected.Contains(v))
                    {
                        tagman.currentInfected.Remove(v);
                    }
                }
            }
        }

        public static float spamtagdelay = -1f;
        public static void SpamTagSelf()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                    if (tagman.currentInfected.Contains(PhotonNetwork.LocalPlayer))
                    {
                        tagman.currentInfected.Remove(PhotonNetwork.LocalPlayer);
                    }
                    else
                    {
                        tagman.AddInfectedPlayer(PhotonNetwork.LocalPlayer);
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
                        if (!GetIndex("Disable Auto Anti Ban").enabled)
                        {
                            Overpowered.FastMaster();
                        }
                    }
                    else
                    {
                        if (Time.time > spamtagdelay)
                        {
                            spamtagdelay = Time.time + 0.1f;
                            GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                            if (tagman.currentInfected.Contains(RigManager.GetPlayerFromVRRig(whoCopy)))
                            {
                                tagman.currentInfected.Remove(RigManager.GetPlayerFromVRRig(whoCopy));
                            }
                            else
                            {
                                tagman.AddInfectedPlayer(RigManager.GetPlayerFromVRRig(whoCopy));
                            }
                        }
                    }
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && !possibly.mainSkin.material.name.Contains("fected"))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                            if (!gorillaTagManager.currentInfected.Contains(RigManager.GetPlayerFromVRRig(possibly)))
                            {
                                gorillaTagManager.AddInfectedPlayer(RigManager.GetPlayerFromVRRig(possibly));
                            }
                        }
                        else
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
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                if (Time.time > spamtagdelay)
                {
                    spamtagdelay = Time.time + 0.1f;
                    GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                    foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                    {
                        if (tagman.currentInfected.Contains(v))
                        {
                            tagman.currentInfected.Remove(v);
                        }
                        else
                        {
                            tagman.AddInfectedPlayer(v);
                        }
                    }
                }
            }
        }

        public static void TagLag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>().tagCoolDown = 2147483647f; // lol
            }
        }

        public static void NahTagLag()
        {
            GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>().tagCoolDown = 5f;
        }

        public static void AntiTag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (!GetIndex("Disable Auto Anti Ban").enabled)
                {
                    Overpowered.FastMaster();
                }
            }
            else
            {
                if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected") && PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                    if (tagman.currentInfected.Contains(PhotonNetwork.LocalPlayer))
                    {
                        tagman.currentInfected.Remove(PhotonNetwork.LocalPlayer);
                        GorillaLocomotion.Player.Instance.disableMovement = false;
                    }
                }
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

        public static void PhysicalTagAura()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                Vector3 they = vrrig.headMesh.transform.position;
                Vector3 notthem = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                float distance = Vector3.Distance(they, notthem);

                if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected") && !vrrig.mainSkin.material.name.Contains("fected") && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
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

                    if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected") && !vrrig.mainSkin.material.name.Contains("fected") && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
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

                    if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected") && !vrrig.mainSkin.material.name.Contains("fected") && GorillaLocomotion.Player.Instance.disableMovement == false && distance < tagAuraDistance)
                    {
                        if (rightHand == true) { GorillaLocomotion.Player.Instance.rightControllerTransform.position = they; } else { GorillaLocomotion.Player.Instance.leftControllerTransform.position = they; }
                    }
                }
            }
        }

        /*public static void RPCTagAura()
        {
            if (GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
            {
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                {
                    VRRig rig = GorillaGameManager.instance.FindPlayerVRRig(player);
                    if (!rig.mainSkin.material.name.Contains("fected"))
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
                    if (!whoCopy.mainSkin.material.name.Contains("fected"))
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = false;

                        GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position - new Vector3(0f, 3f, 0f);
                        GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position - new Vector3(0f, 3f, 0f);

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
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && !possibly.mainSkin.material.name.Contains("fected"))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                            if (!gorillaTagManager.currentInfected.Contains(RigManager.GetPlayerFromVRRig(possibly)))
                            {
                                gorillaTagManager.AddInfectedPlayer(RigManager.GetPlayerFromVRRig(possibly));
                            }
                        }
                        else
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
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && possibly.mainSkin.material.name.Contains("fected"))
                    {
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                        {
                            GorillaTagManager gorillaTagManager = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                            if (gorillaTagManager.currentInfected.Contains(RigManager.GetPlayerFromVRRig(possibly)))
                            {
                                gorillaTagManager.currentInfected.Remove(RigManager.GetPlayerFromVRRig(possibly));
                            }
                        } else
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                Overpowered.FastMaster();
                            }
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
                GorillaTagManager tagman = GameObject.Find("GT Systems/GameModeSystem/Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                foreach (Photon.Realtime.Player v in PhotonNetwork.PlayerList)
                {
                    if (!tagman.currentInfected.Contains(v))
                    {
                        tagman.AddInfectedPlayer(v);
                    }
                }
                GetIndex("Tag All").enabled = false;
            }
            else
            {
                if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You must be tagged.</color>");
                    GetIndex("Tag All").enabled = false;
                }
                else
                {
                    bool isInfectedPlayers = false;
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected"))
                        {
                            isInfectedPlayers = true;
                            break;
                        }
                    }
                    if (isInfectedPlayers == true)
                    {
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (!vrrig.mainSkin.material.name.Contains("fected"))
                            {
                                GorillaTagger.Instance.offlineVRRig.enabled = false;

                                GorillaTagger.Instance.offlineVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                                GorillaTagger.Instance.myVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
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
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            if (!GorillaLocomotion.Player.Instance.disableMovement)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(target);
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
                GorillaTagger.Instance.myVRRig.transform.position = vrrig.transform.position - new Vector3(0f, -3f, 0f);
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
                if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                {
                    bool isInfectedPlayers = false;
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected"))
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
                        if (!vrrig.mainSkin.material.name.Contains("fected"))
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
            PlayerPrefs.SetString("tutorial", "true");
            Hashtable h = new Hashtable();
            h.Add("didTutorial", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(h, null, null);
            PlayerPrefs.Save();
        }

        public static void TagOnJoin()
        {
            PlayerPrefs.SetString("tutorial", "false");
            Hashtable h = new Hashtable();
            h.Add("didTutorial", false);
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
