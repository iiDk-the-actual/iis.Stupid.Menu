using ExitGames.Client.Photon;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using HarmonyLib;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using PlayFab.ClientModels;
using PlayFab;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static GorillaNetworking.CosmeticsController;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static UnityEngine.UI.GridLayoutGroup;

namespace iiMenu.Mods
{
    internal class Fun
    {
        public static void NightTime()
        {
            BetterDayNightManager.instance.SetTimeOfDay(0);
        }

        public static void EveningTime()
        {
            BetterDayNightManager.instance.SetTimeOfDay(7);
        }

        public static void MorningTime()
        {
            BetterDayNightManager.instance.SetTimeOfDay(1);
        }

        public static void DayTime()
        {
            BetterDayNightManager.instance.SetTimeOfDay(3);
        }

        public static void Rain()
        {
            for (int i = 1; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
            {
                BetterDayNightManager.instance.weatherCycle[i] = BetterDayNightManager.WeatherType.Raining;
            }
        }

        public static void NoRain()
        {
            for (int i = 1; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
            {
                BetterDayNightManager.instance.weatherCycle[i] = BetterDayNightManager.WeatherType.None;
            }
        }

        public static void FixHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }

        public static void UpsideDownHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 180f;
        }

        public static void BrokenNeck()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 90f;
        }

        public static void BackwardsHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 180f;
        }

        public static int BPM = 159;
        public static void HeadBang()
        {
            if (Time.time > lastBangTime)
            {
                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 50f;
                lastBangTime = Time.time + (60f/(float)BPM);
            } else
            {
                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = Mathf.Lerp(GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x,0f,0.1f);
            }
        }

        public static void SpinHeadX()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += 10f;
        }

        public static void SpinHeadY()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += 10f;
        }

        public static void SpinHeadZ()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += 10f;
        }

        public static void FlipHands()
        {
            Vector3 lh = GorillaTagger.Instance.leftHandTransform.position;
            Vector3 rh = GorillaTagger.Instance.rightHandTransform.position;
            Quaternion lhr = GorillaTagger.Instance.leftHandTransform.rotation;
            Quaternion rhr = GorillaTagger.Instance.rightHandTransform.rotation;

            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = lh;
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = rh;

            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.rotation = lhr;
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.rotation = rhr;
        }

        public static void FixHandTaps()
        {
            GorillaTagger.Instance.handTapVolume = 0.1f;
        }

        public static void LoudHandTaps()
        {
            GorillaTagger.Instance.handTapVolume = int.MaxValue;
        }

        public static void SilentHandTaps()
        {
            GorillaTagger.Instance.handTapVolume = 0;
        }

        public static void EnableInstantHandTaps()
        {
            GorillaTagger.Instance.tapCoolDown = 0f;
        }

        public static void DisableInstantHandTaps()
        {
            GorillaTagger.Instance.tapCoolDown = 0.33f;
        }

        /*
        public static void InstantParty()
        {
            typeof(FriendshipGroupDetection).GetField("groupTime", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(FriendshipGroupDetection.Instance, 0f);
            typeof(FriendshipGroupDetection).GetField("groupCreateAfterTimestamp", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(FriendshipGroupDetection.Instance, -1f);
            typeof(FriendshipGroupDetection).GetField("amFirstProvisionalPlayer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(FriendshipGroupDetection.Instance, true);
        }

        public static void PartyAll()
        {
            List<string> people = new List<string> { };
            foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerListOthers)
            {
                people.Add(plr.UserId);
            }
            FriendshipGroupDetection.Instance.photonView.RPC("AddPartyMembers", RpcTarget.Others, new object[]
            {
                FriendshipGroupDetection.PackColor(new Color(0f, 0f, 0f)),
                people.ToArray()
            });
        }*/

        public static void LeaveParty()
        {
            FriendshipGroupDetection.Instance.LeaveParty();
        }

        public static void KickAllInParty()
        {
            if (FriendshipGroupDetection.Instance.IsInParty)
            {
                partyLastCode = PhotonNetwork.CurrentRoom.Name;
                waitForPlayerJoin = false;
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(Important.RandomRoomName(), JoinType.ForceJoinWithParty);
                partyTime = Time.time + 0.25f;
                phaseTwo = false;
                amountPartying = ((List<string>)typeof(FriendshipGroupDetection).GetField("myPartyMemberIDs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(FriendshipGroupDetection.Instance)).Count - 1;
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> <color=white>Kicking " + amountPartying.ToString() + " party members, this may take a second...</color>");
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not in a party!</color>");
            }
        }

        public static void BanAllInParty()
        {
            if (FriendshipGroupDetection.Instance.IsInParty)
            {
                partyLastCode = PhotonNetwork.CurrentRoom.Name;
                waitForPlayerJoin = true;
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("KKK", JoinType.ForceJoinWithParty);
                partyTime = Time.time + 0.25f;
                phaseTwo = false;
                amountPartying = ((List<string>)typeof(FriendshipGroupDetection).GetField("myPartyMemberIDs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(FriendshipGroupDetection.Instance)).Count - 1;
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> <color=white>Banning " + amountPartying.ToString() + " party members, this may take a second...</color>");
            }
            else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not in a party!</color>");
            }
        }

        public static bool lastPartyKickThingy = false;
        public static void AutoPartyKick()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && !lastPartyKickThingy)
            {
                KickAllInParty();
            }
            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static void AutoPartyBan()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && !lastPartyKickThingy)
            {
                BanAllInParty();
            }
            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static void WaterSplashHands()
        {
            if (rightGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.rightHandTransform.position,
                        GorillaTagger.Instance.rightHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    RPCProtection();
                    splashDel = Time.time + 0.1f;
                }
            }
            if (leftGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.leftHandTransform.position,
                        GorillaTagger.Instance.leftHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    RPCProtection();
                    splashDel = Time.time + 0.1f;
                }
            }
        }

        public static void WaterSplashAura()
        {
            if (Time.time > splashDel)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f)),
                    GorillaTagger.Instance.offlineVRRig.transform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RPCProtection();
                splashDel = Time.time + 0.1f;
            }
        }

        public static void OrbitWaterSplash()
        {
            if (Time.time > splashDel)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)),
                    Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360))),
                    4f,
                    100f,
                    true,
                    false
                });
                RPCProtection();
                splashDel = Time.time + 0.1f;
            }
        }

        public static void WaterSplashGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    GorillaTagger.Instance.myVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    if (Time.time > splashDel)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                        {
                            NewPointer.transform.position,
                            Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360))),
                            4f,
                            100f,
                            true,
                            false
                        });
                        RPCProtection();
                        splashDel = Time.time + 0.1f;
                    }
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }
        
        private static bool lastlhboop = false;
        private static bool lastrhboop = false;
        public static void Boop()
        {
            bool isBoopLeft = false;
            bool isBoopRight = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float D1 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.headMesh.transform.position);
                    float D2 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.headMesh.transform.position);

                    float threshold = 0.275f;

                    if (!isBoopLeft)
                    {
                        isBoopLeft = D1 < threshold;
                    }
                    if (!isBoopRight)
                    {
                        isBoopRight = D2 < threshold;
                    }
                }
            }
            if (isBoopLeft && !lastlhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        84,
                        true,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(84, true, 999999f);
                }
            }
            if (isBoopRight && !lastrhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        84,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(84, false, 999999f);
                }
            }
            lastlhboop = isBoopLeft;
            lastrhboop = isBoopRight;
        }

        public static void Slap()
        {
            bool isBoopLeft = false;
            bool isBoopRight = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float D1 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.headMesh.transform.position);
                    float D2 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.headMesh.transform.position);

                    float threshold = 0.275f;

                    if (!isBoopLeft)
                    {
                        isBoopLeft = D1 < threshold;
                    }
                    if (!isBoopRight)
                    {
                        isBoopRight = D2 < threshold;
                    }
                }
            }
            if (isBoopLeft && !lastlhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        248,
                        true,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(248, true, 999999f);
                }
            }
            if (isBoopRight && !lastrhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                        248,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(248, false, 999999f);
                }
            }
            lastlhboop = isBoopLeft;
            lastrhboop = isBoopRight;
        }

        /*
        public static void GorillaVoice()
        {
            GorillaTagger.Instance.offlineVRRig.remoteUseReplacementVoice = rightPrimary;
            GorillaTagger.Instance.offlineVRRig.localUseReplacementVoice = rightPrimary;
        }*/

        public static void GetHoneyComb()
        {
            if (leftGrab)
            {
                GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    false,
                    false
                });
                GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    true,
                    true
                });
                RPCProtection();
            }
            if (rightGrab)
            {
                GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    false,
                    true
                });
                GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    true,
                    false
                });
                RPCProtection();
            }
        }

        private static bool shouldThingIdk = true;
        public static void HoneycombSpam()
        {
            if (rightGrab)
            {
                GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    shouldThingIdk,
                    false
                });
                RPCProtection();
                shouldThingIdk = !shouldThingIdk;
            }
        }

        public static void RemoveBracelet()
        {
            GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                false,
                true
            });
            GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                false,
                false
            });
            RPCProtection();
        }

        /*
        public static void KillBees()
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
                AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 goldentrophyy = new Vector3(99999f, 99999f, 99999f);
                goldentrophy.Emerge(goldentrophyy, new Vector3(999f, 999f, 999f));
            }
        }

        public static void AngerBees()
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
                AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 goldentrophyy = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 3f, 0f);
                goldentrophy.Emerge(goldentrophyy, goldentrophyy);
            }
        }

        public static void AngerBeesGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
                        AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                        goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                        Vector3 goldentrophyy = Ray.point;
                        goldentrophy.Emerge(goldentrophyy, goldentrophyy);
                    }
                }
            }
        }

        public static float beedelay = -1f;
        public static void AngerBeesAll()
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
                if (Time.time > beedelay)
                {
                    beedelay = Time.time + 0.1f;
                    AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                    goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    Vector3 goldentrophyy = GetRandomVRRig(true).headConstraint.transform.position;
                    goldentrophy.Emerge(goldentrophyy, goldentrophyy);
                }
            }
        }

        public static void StingSelf()
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
                AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

                System.Type goldentrophyy = goldentrophy.GetType();
                FieldInfo goldentrophyyy = goldentrophyy.GetField("grabTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                goldentrophyyy.SetValue(goldentrophy, Time.time);
                goldentrophyyy = goldentrophyy.GetField("emergeStartedTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                goldentrophyyy.SetValue(goldentrophy, Time.time);

                goldentrophy.targetPlayer = PhotonNetwork.LocalPlayer;
                goldentrophy.grabbedPlayer = PhotonNetwork.LocalPlayer;

                goldentrophy.lastState = AngryBeeSwarm.ChaseState.Chasing;
                goldentrophy.currentState = AngryBeeSwarm.ChaseState.Grabbing;
            }
        }

        public static void StingGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!GetIndex("Disable Auto Anti Ban").enabled)
                            {
                                Overpowered.FastMaster();
                            }
                        }
                        else
                        {
                            AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                            goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

                            System.Type goldentrophyy = goldentrophy.GetType();
                            FieldInfo goldentrophyyy = goldentrophyy.GetField("grabTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                            goldentrophyyy.SetValue(goldentrophy, Time.time);
                            goldentrophyyy = goldentrophyy.GetField("emergeStartedTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                            goldentrophyyy.SetValue(goldentrophy, Time.time);

                            goldentrophy.targetPlayer = player;
                            goldentrophy.grabbedPlayer = player;

                            goldentrophy.currentState = AngryBeeSwarm.ChaseState.Grabbing;
                        }
                    }
                }
            }
        }

        public static void StingAll()
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
                if (Time.time > beedelay)
                {
                    beedelay = Time.time + 0.1f;
                    Photon.Realtime.Player player = GetRandomPlayer(true);
                    AngryBeeSwarm goldentrophy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Nowruz2024_PersistentObjects/AngryBeeSwarm/FloatingChaseBeeSwarm").GetComponent<AngryBeeSwarm>();
                    goldentrophy.photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

                    System.Type goldentrophyy = goldentrophy.GetType();
                    FieldInfo goldentrophyyy = goldentrophyy.GetField("grabTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                    goldentrophyyy.SetValue(goldentrophy, Time.time);
                    goldentrophyyy = goldentrophyy.GetField("emergeStartedTimestamp", BindingFlags.NonPublic | BindingFlags.Instance);
                    goldentrophyyy.SetValue(goldentrophy, Time.time);

                    goldentrophy.targetPlayer = player;
                    goldentrophy.grabbedPlayer = player;

                    goldentrophy.currentState = AngryBeeSwarm.ChaseState.Grabbing;
                }
            }
        }

        public static void LavaSplashHands()
        {
            GorillaLocomotion.Player.Instance.OnEnterWaterVolume(GorillaLocomotion.Player.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
            if (rightGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.rightHandTransform.position,
                        GorillaTagger.Instance.rightHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    RPCProtection();
                    splashDel = Time.time + 0.1f;
                }
            }
            if (leftGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.leftHandTransform.position,
                        GorillaTagger.Instance.leftHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    RPCProtection();
                    splashDel = Time.time + 0.1f;
                }
            }
        }

        public static void LavaSplashAura()
        {
            GorillaLocomotion.Player.Instance.OnEnterWaterVolume(GorillaLocomotion.Player.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
            if (Time.time > splashDel)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f)),
                    GorillaTagger.Instance.offlineVRRig.transform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RPCProtection();
                splashDel = Time.time + 0.1f;
            }
        }

        public static void LavaSplashGun()
        {
            GorillaLocomotion.Player.Instance.OnEnterWaterVolume(GorillaLocomotion.Player.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    GorillaTagger.Instance.myVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    if (Time.time > splashDel)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                        {
                            NewPointer.transform.position,
                            Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0,360))),
                            4f,
                            100f,
                            true,
                            false
                        });
                        RPCProtection();
                        splashDel = Time.time + 0.1f;
                    }
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }*/

        public static void LowQualityMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GameObject.Find("Photon Manager").GetComponent<Photon.Voice.Unity.Recorder>();
            mic.SamplingRate = SamplingRate.Sampling08000;
            //oldBitrate = mic.Bitrate;
            mic.Bitrate = 5;

            mic.RestartRecording(true);
        }

        public static void HighQualityMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GameObject.Find("Photon Manager").GetComponent<Photon.Voice.Unity.Recorder>();
            mic.SamplingRate = SamplingRate.Sampling16000;
            mic.Bitrate = 30000;

            mic.RestartRecording(true);
        }

        public static void LoudMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GameObject.Find("Photon Manager").GetComponent<Photon.Voice.Unity.Recorder>();

            if (!mic.gameObject.GetComponent<MicAmplifier>())
            {
                mic.gameObject.AddComponent<MicAmplifier>();
            }

            MicAmplifier loudman = mic.gameObject.GetComponent<MicAmplifier>();
            loudman.AmplificationFactor = 16;
            loudman.BoostValue = 16;

            mic.RestartRecording(true);
        }

        public static void NotLoudMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GameObject.Find("Photon Manager").GetComponent<Photon.Voice.Unity.Recorder>();

            if (mic.gameObject.GetComponent<MicAmplifier>())
            {
                UnityEngine.Object.Destroy(mic.gameObject.GetComponent<MicAmplifier>());
            }

            mic.RestartRecording(true);
        }

        public static void ReloadMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GameObject.Find("Photon Manager").GetComponent<Photon.Voice.Unity.Recorder>();
            mic.RestartRecording(true);
        }

        public static void BugGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                        GameObject.Find("Floating Bug Holdable").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void BatGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GameObject.Find("Cave Bat Holdable").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void BeachBallGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GameObject.Find("BeachBall").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void GliderGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    foreach (GliderHoldable glider in GetGliders())
                    {
                        if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            glider.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                        } else
                        {
                            glider.OnHover(null, null);
                        }
                    }
                }
            }
        }

        public static void NoRespawnBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = float.MaxValue;
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = float.MaxValue;
        }

        public static void PleaseRespawnBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = 50f;
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = 50f;
        }

        public static void NoRespawnBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = float.MaxValue;
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = float.MaxValue;
        }

        public static void PleaseRespawnBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = 50f;
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = 50f;
        }

        public static void NoRespawnGliders()
        {
            NoGliderRespawn = true;
        }

        public static void PleaseRespawnGliders()
        {
            NoGliderRespawn = false;
        }

        public static void FastGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                glider.pullUpLiftBonus = 0.5f;
                glider.dragVsSpeedDragFactor = 0.5f;
            }
        }

        public static void SlowGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                glider.pullUpLiftBonus = 0.05f;
                glider.dragVsSpeedDragFactor = 0.05f;
            }
        }

        public static void FixGliderSpeed()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                glider.pullUpLiftBonus = 0.1f;
                glider.dragVsSpeedDragFactor = 0.2f;
            }
        }

        public static float lastTime = 0f;
        public static void SpazGliderMaterial()
        {
            if (Time.time > lastTime)
            {
                lastTime = Time.time + 0.1f;
                foreach (GliderHoldable glider in GetGliders())
                {
                    if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                    {
                        FieldInfo SyncedStateField = typeof(GliderHoldable).GetField("syncedState", BindingFlags.NonPublic | BindingFlags.Instance);
                        object SyncedStateValue = SyncedStateField.GetValue(glider);

                        FieldInfo RiderIdField = SyncedStateValue.GetType().GetField("materialIndex", BindingFlags.Public | BindingFlags.Instance);
                        RiderIdField.SetValue(SyncedStateValue, (byte)UnityEngine.Random.Range(0, 3));

                        SyncedStateField.SetValue(glider, SyncedStateValue);
                    }
                    else
                    {
                        glider.OnHover(null, null);
                    }
                }
            }
        }

        public static void GrabBug()
        {
            if (rightGrab)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void GrabBat()
        {
            if (rightGrab)
            {
                GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void GrabBeachBall()
        {
            if (rightGrab)
            {
                GameObject.Find("BeachBall").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void GrabGliders()
        {
            if (rightGrab)
            {
                foreach (GliderHoldable glider in GetGliders())
                {
                    if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                    {
                        glider.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    }
                    else
                    {
                        glider.OnHover(null, null);
                    }
                }
            }
        }

        public static void DestroyBug()
        {
            GameObject.Find("Floating Bug Holdable").transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void DestroyBat()
        {
            GameObject.Find("Cave Bat Holdable").transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void DestroyBeachBall()
        {
            GameObject.Find("BeachBall").transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void RespawnGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.Respawn();
                }
                else
                {
                    glider.OnHover(null, null);
                }
            }
        }

        public static void SpazBug()
        {
             GameObject.Find("Floating Bug Holdable").transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        }

        public static void SpazBat()
        {
            GameObject.Find("Cave Bat Holdable").transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        }

        public static void SpazBeachBall()
        {
            GameObject.Find("BeachBall").transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        }

        public static void SpazGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                }
                else
                {
                    glider.OnHover(null, null);
                }
            }
        }

        public static void BugHalo()
        {
            float offset = 0;
            GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)), 2, MathF.Sin(offset + ((float)Time.frameCount / 30)));
        }

        public static void BatHalo()
        {
            float offset = 360f / 3f;
            GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)), 2, MathF.Sin(offset + ((float)Time.frameCount / 30)));
        }

        public static void BeachBallHalo()
        {
            float offset = (360f / 3f) * 2f;
            GameObject.Find("BeachBall").transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)), 2, MathF.Sin(offset + ((float)Time.frameCount / 30)));
        }

        public static void OrbitGliders()
        {
            GliderHoldable[] them = GetGliders();
            int index = 0;
            foreach (GliderHoldable glider in them)
            {
                if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    float offset = (360f / (float)them.Length) * index;
                    glider.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 5f, 2, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 5f);
                }
                else
                {
                    glider.OnHover(null, null);
                }
                index++;
            }
        }

        public static void RideBug()
        {
            GorillaTagger.Instance.rigidbody.transform.position = GameObject.Find("Floating Bug Holdable").transform.position;
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void RideBat()
        {
            GorillaTagger.Instance.rigidbody.transform.position = GameObject.Find("Cave Bat Holdable").transform.position;
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void RideBeachBall()
        {
            GorillaTagger.Instance.rigidbody.transform.position = GameObject.Find("BeachBall").transform.position;
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void BreakBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().allowPlayerStealing = false;
        }

        public static void BreakBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().allowPlayerStealing = false;
        }

        public static void FixBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().allowPlayerStealing = false;
        }

        public static void FixBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().allowPlayerStealing = false;
        }

        public static void StealBug()
        {
            if (rightGrab)
            {
                GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().WorldShareableRequestOwnership();
                ThrowableBug bug = GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>();
                bug.currentState = TransferrableObject.PositionState.Dropped;
                System.Type type = bug.GetType();
                FieldInfo fieldInfo = type.GetField("locked", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo.SetValue(bug, false);
                GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void StealBat()
        {
            if (rightGrab)
            {
                GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().WorldShareableRequestOwnership();
                ThrowableBug bug = GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>();
                bug.currentState = TransferrableObject.PositionState.Dropped;
                System.Type type = bug.GetType();
                FieldInfo fieldInfo = type.GetField("locked", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo.SetValue(bug, false);
                GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void PopAllBalloons()
        {
            foreach (BalloonHoldable balloon in GetBalloons())
            {
                Vector3 startpos = balloon.gameObject.transform.position;
                Vector3 charvel = Vector3.zero;

                Mods.Spammers.Projectiles.BetaFireProjectile("SlingshotProjectile", startpos, charvel, Color.white, true);
            }
        }

        public static void GrabBalloons()
        {
            if (rightGrab)
            {
                foreach (BalloonHoldable balloon in GetBalloons())
                {
                    if (balloon.photonView.Owner == PhotonNetwork.LocalPlayer)
                    {
                        balloon.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    }
                    else
                    {
                        balloon.WorldShareableRequestOwnership();
                    }
                }
            }
        }

        public static void SpazBalloons()
        {
            foreach (BalloonHoldable balloon in GetBalloons())
            {
                if (balloon.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    balloon.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360))); ;
                }
                else
                {
                    balloon.WorldShareableRequestOwnership();
                }
            }
        }

        public static void OrbitBalloons()
        {
            BalloonHoldable[] them = GetBalloons();
            int index = 0;
            foreach (BalloonHoldable balloon in them)
            {
                if (balloon.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    float offset = (360f / (float)them.Length) * index;
                    balloon.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 5f, 2, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 5f);
                }
                else
                {
                    balloon.WorldShareableRequestOwnership();
                }
                index++;
            }
        }

        public static void BalloonGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    foreach (BalloonHoldable balloon in GetBalloons())
                    {
                        if (balloon.photonView.Owner == PhotonNetwork.LocalPlayer)
                        {
                            balloon.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                        } else
                        {
                            balloon.WorldShareableRequestOwnership();
                        }
                    }
                }
            }
        }

        public static void DestroyBalloons()
        {
            foreach (BalloonHoldable balloon in GetBalloons())
            {
                if (balloon.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    balloon.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                }
                else
                {
                    balloon.WorldShareableRequestOwnership();
                }
            }
        }

        public static void DestroyGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                if (glider.photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                }
                else
                {
                    glider.OnHover(null, null);
                }
            }
        }

        public static void UnacidSelf()
        {
            ScienceExperimentManager.instance.photonView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.All, new object[]
            {
                PhotonNetwork.LocalPlayer.ActorNumber
            });
        }

        public static void UnacidGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > kgDebounce)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        Photon.Realtime.Player player = GetPlayerFromVRRig(possibly);
                        // Not created by me, leaked by REV
                        ScienceExperimentManager.instance.photonView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.All, new object[]
                        {
                            player.ActorNumber
                        });
                        RPCProtection();
                        kgDebounce = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void UnacidAll()
        {
            foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerList)
            {
                ScienceExperimentManager.instance.photonView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.All, new object[]
                {
                    plr.ActorNumber
                });
            };
        }

        public static void SpazVoice()
        {
            GorillaTagger.Instance.offlineVRRig.shouldSendSpeakingLoudness = true;
            GorillaTagger.Instance.offlineVRRig.replacementVoiceLoudnessThreshold = 0f;
            GorillaTagger.Instance.offlineVRRig.replacementVoiceDetectionDelay = 0;

            System.Type type = GorillaTagger.Instance.offlineVRRig.GetType();
            FieldInfo fieldInfo = type.GetField("speakingLoudness", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(GorillaTagger.Instance.offlineVRRig, (float)UnityEngine.Random.Range(0, 1));
        }

        public static void UnspazVoice()
        {
            GorillaTagger.Instance.offlineVRRig.shouldSendSpeakingLoudness = false;
            GorillaTagger.Instance.offlineVRRig.replacementVoiceLoudnessThreshold = 0.05f;
            GorillaTagger.Instance.offlineVRRig.replacementVoiceDetectionDelay = 128;
        }

        /*
        public static void GrabTrain()
        {
            if (rightGrab)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab").transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
        }

        public static void TrainGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void DestroyTrain()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab").transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void SlowTrain()
        {
            GameObject train = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab/NCTrainEngine_Prefab");
            train.GetComponent<PhotonView>().ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<PhotonView>().OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<TraverseSpline>().duration = 60f;
        }

        public static void FastTrain()
        {
            GameObject train = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab/NCTrainEngine_Prefab");
            train.GetComponent<PhotonView>().ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<PhotonView>().OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<TraverseSpline>().duration = 10f;
        }

        public static void FixTrain()
        {
            GameObject train = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Holiday2023Forest/Holiday2023Forest_Gameplay/NCTrain_Kit_Prefab/NCTrainEngine_Prefab");
            train.GetComponent<PhotonView>().ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<PhotonView>().OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            train.GetComponent<TraverseSpline>().duration = 30f;
        }*/

        public static void RemoveName()
        {
            ChangeName("________");
        }

        public static void SetNameToSTATUE()
        {
            ChangeName("STATUE");
        }

        public static void SetNameToRUN()
        {
            ChangeName("RUN");
        }

        public static void SetNameToiiOnTop()
        {
            ChangeName("iiOnTop");
        }

        public static void SetNameToBEHINDYOU()
        {
            ChangeName("BEHINDYOU");
        }

        public static void PBBVNameCycle()
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > 3)
                {
                    nameCycleIndex = 1;
                }

                if (nameCycleIndex == 1)
                {
                    ChangeName("PBBV");
                }
                if (nameCycleIndex == 2)
                {
                    ChangeName("IS");
                }
                if (nameCycleIndex == 3)
                {
                    ChangeName("HERE");
                }

                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void J3VUNameCycle()
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > 4)
                {
                    nameCycleIndex = 1;
                }

                if (nameCycleIndex == 1)
                {
                    ChangeName("J3VU");
                }
                if (nameCycleIndex == 2)
                {
                    ChangeName("HAS");
                }
                if (nameCycleIndex == 3)
                {
                    ChangeName("BECOME");
                }
                if (nameCycleIndex == 4)
                {
                    ChangeName("HOSTILE");
                }

                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void RunRabbitNameCycle()
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > 2)
                {
                    nameCycleIndex = 1;
                }

                if (nameCycleIndex == 1)
                {
                    ChangeName("RUN");
                }
                if (nameCycleIndex == 2)
                {
                    ChangeName("RABBIT");
                }

                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void RandomNameCycle()
        {
            if (Time.time > nameCycleDelay)
            {
                string random = "";
                for (int i = 0; i < 8; i++)
                {
                    random += letters[UnityEngine.Random.Range(0,letters.Length - 1)];
                }
                ChangeName(random);

                nameCycleDelay = Time.time + 1f;
            }
        }

        public static string[] names = new string[] { };
        public static void EnableCustomNameCycle()
        {
            if (File.Exists("iisStupidMenu/iiMenu_CustomNameCycle.txt"))
            {
                names = File.ReadAllText("iisStupidMenu/iiMenu_CustomNameCycle.txt").Split('\n');
            }
            else
            {
                File.WriteAllText("iisStupidMenu/iiMenu_CustomNameCycle.txt","YOUR\nTEXT\nHERE");
            }
        }

        public static void CustomNameCycle()
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > names.Length)
                {
                    nameCycleIndex = 1;
                }

                ChangeName(names[nameCycleIndex-1]);

                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void StrobeColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.1f;
                strobeColor = !strobeColor;
                ChangeColor(new Color(strobeColor ? 1 : 0, strobeColor ? 1 : 0, strobeColor ? 1 : 0));
            }
        }

        public static void RainbowColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.1f;
                float h = (Time.frameCount / 180f) % 1f;
                ChangeColor(UnityEngine.Color.HSVToRGB(h, 1f, 1f));
            }
        }

        public static void HardRainbowColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 1f;
                colorChangeType++;
                if (colorChangeType > 3)
                {
                    colorChangeType = 0;
                }
                Color[] colors = new Color[]
                {
                    Color.red,
                    Color.green,
                    Color.blue,
                    Color.magenta
                };

                ChangeColor(colors[colorChangeType]);
            }
        }


        public static void NegativeColor()
        {
            PlayerPrefs.SetFloat("redValue", -2147483648);
            PlayerPrefs.SetFloat("greenValue", -2147483648);
            PlayerPrefs.SetFloat("blueValue", -2147483648);

            GorillaTagger.Instance.UpdateColor(-2147483648, -2147483648, -2147483648);
            PlayerPrefs.Save();
            GorillaTagger.Instance.myVRRig.RPC("InitializeNoobMaterial", RpcTarget.All, new object[] { -2147483648, -2147483648, -2147483648, false });
            RPCProtection();
        }

        public static void BecomeGoldentrophy()
        {
            ChangeName("goldentrophy");
            ChangeColor(new Color32(255, 128, 0, 255));
        }

        public static void BecomePBBV()
        {
            ChangeName("PBBV");
            ChangeColor(new Color32(230, 127, 102, 255));
        }

        public static void BecomeJ3VU()
        {
            ChangeName("J3VU");
            ChangeColor(Color.green);
        }

        public static void BecomeECHO()
        {
            ChangeName("ECHO");
            ChangeColor(new Color32(0, 150, 255, 255));
        }

        public static void BecomeDAISY09()
        {
            ChangeName("DAISY09");
            ChangeColor(new Color32(255, 81, 231, 255));
        }

        public static void BecomeHiddenOnLeaderboard() {
            ChangeName("________");
            ChangeColor(new Color32(0, 53, 2, 255));
        }

        public static void CopyIdentityGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > stealIdentityDelay)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        ChangeName(GetPlayerFromVRRig(possibly).NickName);
                        ChangeColor(possibly.playerColor);
                        stealIdentityDelay = Time.time + 0.5f;
                    }
                }
            }
        }

        public static void ChangeAccessories()
        {
            if (leftGrab && !lastHitL)
            {
                hat--;
                if (hat < 1)
                {
                    hat = 3;
                }

                if (hat == 1)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 2)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 3)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
            }
            lastHitL = leftGrab;
            if (rightGrab && !lastHitR)
            {
                hat++;
                if (hat > 3)
                {
                    hat = 1;
                }

                if (hat == 1)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 2)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 3)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
            }
            lastHitR = rightGrab;
            if (leftPrimary && !lastHitLP)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeLeftButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                if (hat == 1)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 2)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 3)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
            }
            lastHitLP = leftPrimary;

            if (rightPrimary && !lastHitRP)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeRightItem").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                if (hat == 1)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 2)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
                if (hat == 3)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                }
            }
            lastHitRP = rightPrimary;

            if (rightSecondary && !lastHitRS)
            {
                accessoryType++;
                if (accessoryType > 4)
                {
                    accessoryType = 1;
                }
                if (accessoryType == 1)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardobeHatButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                }
                if (accessoryType == 2)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeFaceButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                }
                if (accessoryType == 3)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeBadgeButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                }
                if (accessoryType == 4)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeHoldableButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                }
            }
            lastHitRS = rightSecondary;
        }

        public static void SpazAccessories()
        {
            int rando = UnityEngine.Random.Range(1,9);

            if (rando == 1)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
            }
            if (rando == 2)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
            }
            if (rando == 3)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
            }

            if (rando == 4)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeLeftButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }
            if (rando == 5)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeRightItem").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }

            if (rando == 6)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardobeHatButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }
            if (rando == 7)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeFaceButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }
            if (rando == 8)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeBadgeButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }
            if (rando == 9)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeHoldableButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
            }
        }

        public static void EnableCustomSoundOnJoin()
        {
            customSoundOnJoin = true;
        }

        public static void DisableCustomSoundOnJoin()
        {
            customSoundOnJoin = false;
        }
    }
}
