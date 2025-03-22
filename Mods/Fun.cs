using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using GorillaTagScripts.ObstacleCourse;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Fun
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

        private static LightmapData[] hell = null;
        public static void Fullbright()
        {
            hell = LightmapSettings.lightmaps;
            LightmapSettings.lightmaps = null;
        }

        public static void Fullshade()
        {
            LightmapSettings.lightmaps = hell;
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
            if (GorillaTagger.Instance.offlineVRRig.enabled)
            {
                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x += 10f;
            } else
            {
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(10f, 0f, 0f));
            }
        }

        public static void SpinHeadY()
        {
            if (GorillaTagger.Instance.offlineVRRig.enabled)
            {
                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += 10f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
            }
        }

        public static void SpinHeadZ()
        {
            if (GorillaTagger.Instance.offlineVRRig.enabled)
            {
                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z += 10f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 0f, 10f));
            }
        }

        public static void FlipHands()
        {
            Vector3 lh = GorillaTagger.Instance.leftHandTransform.position;
            Vector3 rh = GorillaTagger.Instance.rightHandTransform.position;
            Quaternion lhr = GorillaTagger.Instance.leftHandTransform.rotation;
            Quaternion rhr = GorillaTagger.Instance.rightHandTransform.rotation;

            GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.transform.position = lh;
            GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.transform.position = rh;

            GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.transform.rotation = lhr;
            GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.transform.rotation = rhr;
        }

        public static void FixHandTaps()
        {
            GorillaTagger.Instance.handTapVolume = 0.1f;
            Patches.HandTapPatch.doPatch = false;
            Patches.HandTapPatch.tapsEnabled = true;
            Patches.HandTapPatch.doOverride = false;
            Patches.HandTapPatch.overrideVolume = 0.1f;
            Patches.HandTapPatch.tapMultiplier = 1;
        }

        public static void LoudHandTaps()
        {
            Patches.HandTapPatch.doPatch = true;
            Patches.HandTapPatch.tapsEnabled = true;
            Patches.HandTapPatch.doOverride = true;
            Patches.HandTapPatch.overrideVolume = 99999f;
            Patches.HandTapPatch.tapMultiplier = 10;
            GorillaTagger.Instance.handTapVolume = 99999f;
        }

        public static void SilentHandTaps()
        {
            Patches.HandTapPatch.doPatch = true;
            Patches.HandTapPatch.tapsEnabled = false;
            Patches.HandTapPatch.doOverride = false;
            Patches.HandTapPatch.overrideVolume = 0f;
            Patches.HandTapPatch.tapMultiplier = 0;
            GorillaTagger.Instance.handTapVolume = 0;
        }

        public static void SilentHandTapsOnTag()
        {
            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                SilentHandTaps();
            } else
            {
                FixHandTaps();
            }
            
        }

        public static void EnableInstantHandTaps()
        {
            GorillaTagger.Instance.tapCoolDown = 0f;
        }

        public static void DisableInstantHandTaps()
        {
            GorillaTagger.Instance.tapCoolDown = 0.33f;
        }

        private static float instantPartyDelay = 0f;
        public static void InstantParty()
        {
            if (Time.time > instantPartyDelay)
            {
                instantPartyDelay = Time.time + 0.1f;

                Traverse.Create(FriendshipGroupDetection.Instance).Field("suppressPartyCreationUntilTimestamp").SetValue(0f);
                Traverse.Create(FriendshipGroupDetection.Instance).Field("groupCreateAfterTimestamp").SetValue(0f);

                List<int> provisionalMembers = (List<int>)Traverse.Create(FriendshipGroupDetection.Instance).Field("playersInProvisionalGroup").GetValue();

                if (provisionalMembers.Count > 0)
                {
                    Color targetColor = GTColor.RandomHSV((GTColor.HSVRanges)Traverse.Create(FriendshipGroupDetection.Instance).Field("braceletRandomColorHSVRanges").GetValue());
                    Traverse.Create(FriendshipGroupDetection.Instance).Field("myBraceletColor").SetValue(targetColor);

                    List<int> members = new List<int> { PhotonNetwork.LocalPlayer.ActorNumber };
                    foreach (Player player in PhotonNetwork.PlayerListOthers)
                    {
                        if (FriendshipGroupDetection.Instance.IsInMyGroup(player.UserId) || provisionalMembers.Contains(player.ActorNumber))
                            members.Add(player.ActorNumber);
                    }
                    typeof(FriendshipGroupDetection).GetMethod("SendPartyFormedRPC", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(FriendshipGroupDetection.Instance, new object[] { FriendshipGroupDetection.PackColor(targetColor), members.ToArray(), false });
                    RPCProtection();
                }
            }
        }

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

        public static Coroutine partyKickDelayCoroutine;
        public static IEnumerator PartyKickDelay(bool ban)
        {
            yield return new WaitForSeconds(0.25f);

            if (ban)
                BanAllInParty();
            else
                KickAllInParty();

            Coroutine thisCoroutine = partyKickDelayCoroutine;
            partyKickDelayCoroutine = null;

            CoroutineManager.EndCoroutine(thisCoroutine);
        }

        public static bool lastPartyKickThingy = false;
        public static void AutoPartyKick()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && !lastPartyKickThingy)
            {
                if (partyKickDelayCoroutine == null)
                    partyKickDelayCoroutine = CoroutineManager.RunCoroutine(PartyKickDelay(false));
            }
            
            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static void AutoPartyBan()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && !lastPartyKickThingy)
            {
                if (partyKickDelayCoroutine == null)
                    partyKickDelayCoroutine = CoroutineManager.RunCoroutine(PartyKickDelay(true));
            }

            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static void WaterSplashHands()
        {
            if (rightGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
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
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f),UnityEngine.Random.Range(-0.5f, 0.5f),UnityEngine.Random.Range(-0.5f, 0.5f)),
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

        public static void OrbitWaterSplash()
        {
            if (Time.time > splashDel)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
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
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    GorillaTagger.Instance.myVRRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    if (Time.time > splashDel)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
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

        public static void WaterSplashWalk()
        {
            if (Time.time > splashDel)
            {
                if (GorillaLocomotion.GTPlayer.Instance.IsHandTouching(true))
                {
                    FieldInfo fieldInfo = typeof(GorillaLocomotion.GTPlayer).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                    RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.GTPlayer.Instance);
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.leftHandTransform.position,
                        Quaternion.Euler(ray.normal),
                        4f,
                        100f,
                        true,
                        false
                    });
                    RPCProtection();
                    splashDel = Time.time + 0.1f;
                }
                if (GorillaLocomotion.GTPlayer.Instance.IsHandTouching(false))
                {
                    FieldInfo fieldInfo = typeof(GorillaLocomotion.GTPlayer).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                    RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.GTPlayer.Instance);
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.rightHandTransform.position,
                        Quaternion.Euler(ray.normal),
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
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

        private static bool autoclickstate = false;
        public static void AutoClicker()
        {
            autoclickstate = !autoclickstate;
            if (leftTrigger > 0.5f)
            {
                ControllerInputPoller.instance.leftControllerIndexFloat = autoclickstate ? 1f : 0f;

                GorillaTagger.Instance.offlineVRRig.leftHand.calcT = autoclickstate ? 1f : 0f;
                GorillaTagger.Instance.offlineVRRig.leftHand.LerpFinger(1f, false);
            }
            if (rightTrigger > 0.5f)
            {
                ControllerInputPoller.instance.rightControllerIndexFloat = autoclickstate ? 1f : 0f;

                GorillaTagger.Instance.offlineVRRig.rightHand.calcT = autoclickstate ? 1f : 0f;
                GorillaTagger.Instance.offlineVRRig.rightHand.LerpFinger(1f, false);
            }
        }

        public static List<object[]> keyLogs = new List<object[]> { };
        public static bool keyboardTrackerEnabled = false;
        public static void KeyboardTracker()
        {
            keyboardTrackerEnabled = true;
            if (keyLogs.Count > 0)
            {
                foreach (object[] keylog in keyLogs)
                {
                    if (Time.time > (float)keylog[2])
                    {
                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>KEYLOGS</color><color=grey>]</color> " + (string)keylog[1], 5000);
                        keyLogs.Remove(keylog);
                    }
                }
            }
        }

        public static void DisableKeyboardTracker()
        {
            keyboardTrackerEnabled = false;
        }

        private static float muteDelay = 0f;
        public static void MuteGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > muteDelay)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (line.linePlayer == GetPlayerFromVRRig(possibly))
                            {
                                muteDelay = Time.time + 0.5f;
                                line.PressButton(!line.muteButton.isOn, GorillaPlayerLineButton.ButtonType.Mute);
                            }
                        }
                    }
                }
            }
        }

        public static void MuteAll()
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (!line.muteButton.isAutoOn)
                    line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);
            }
        }

        public static void UnmuteAll()
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.muteButton.isAutoOn)
                    line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);
            }
        }

        public static void MuteDJSets()
        {
            foreach (RadioButtonGroupWearable djSet in GetRadios())
            {
                if (djSet.enabled)
                    djSet.enabled = false;
            }
        }

        public static void UnmuteDJSets()
        {
            foreach (RadioButtonGroupWearable djSet in GetRadios())
            {
                if (!djSet.enabled)
                    djSet.enabled = true;
            }
        }

        public static void MicrophoneFeedback()
        {
            if (!GorillaTagger.Instance.myRecorder.DebugEchoMode)
            {
                GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
            }
        }

        public static void DisableMicrophoneFeedback()
        {
            if (GorillaTagger.Instance.myRecorder.DebugEchoMode)
            {
                GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
            }
        }

        public static void CopyVoiceGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    AudioClip clippy = ((Photon.Voice.Unity.Speaker)Traverse.Create(whoCopy.gameObject.GetComponent<GorillaSpeakerLoudness>()).Field("speaker").GetValue()).gameObject.GetComponent<AudioSource>().clip;
                    if (GorillaTagger.Instance.myRecorder.AudioClip != clippy)
                    {
                        GorillaTagger.Instance.myRecorder.AudioClip = clippy;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                        GorillaTagger.Instance.myRecorder.AudioClip = ((Photon.Voice.Unity.Speaker)Traverse.Create(possibly.gameObject.GetComponent<GorillaSpeakerLoudness>()).Field("speaker").GetValue()).gameObject.GetComponent<AudioSource>().clip;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    Sound.FixMicrophone();
                }
            }
        }

        /*
        public static void GorillaVoice()
        {
            GorillaTagger.Instance.offlineVRRig.remoteUseReplacementVoice = rightPrimary;
            GorillaTagger.Instance.offlineVRRig.localUseReplacementVoice = rightPrimary;
        }*/

        private static float stupiddelay = 0f;
        public static void TapAllBells()
        {
            if (rightGrab)
            {
                if (Time.time > stupiddelay)
                {
                    foreach (TappableBell stupid in GetBells())
                    {
                        stupid.OnTap(1f);
                        RPCProtection();
                    }
                    stupiddelay = Time.time + 0.1f;
                }
            }
        }

        public static void TapAllCrystals()
        {
            if (rightGrab)
            {
                if (Time.time > stupiddelay)
                {
                    foreach (GorillaCaveCrystal stupid in GetCrystals())
                    {
                        stupid.OnTap(1f);
                        RPCProtection();
                    }
                    stupiddelay = Time.time + 0.1f;
                }
            }
        }

        public static void ActivateAllDoors()
        {
            if (rightGrab)
            {
                if (Time.time > stupiddelay)
                {
                    foreach (GhostLabButton stupid in GetLabButtons())
                    {
                        stupid.ButtonActivation();
                        RPCProtection();
                    }
                    stupiddelay = Time.time + 0.1f;
                }
            }
        }

        private static float hitDelay = 0f;
        public static void AutoHitMoles()
        {   
            foreach (Mole stupid in GetMoles())
            {
                int state = (int)Traverse.Create(stupid).Field("randomMolePickedIndex").GetValue();
                if (stupid.CanTap() && stupid.moleTypes[state].isHazard == false && Time.time > hitDelay)
                {
                    hitDelay = Time.time + 0.1f;
                    stupid.OnTap(1f);
                    RPCProtection();
                }
            }
        }

        public static void AutoHitHazards()
        {
            foreach (Mole stupid in GetMoles())
            {
                int state = (int)Traverse.Create(stupid).Field("randomMolePickedIndex").GetValue();
                if (stupid.CanTap() && stupid.moleTypes[state].isHazard && Time.time > hitDelay)
                {
                    hitDelay = Time.time + 0.1f;
                    stupid.OnTap(1f);
                    RPCProtection();
                }
            }
        }

        public static void SpazMoleMachines()
        {
            if (Time.time > stupiddelay)
            {
                stupiddelay = Time.time + 0.25f;
                foreach (WhackAMole stupid in GetWAMoles())
                {
                    stupid.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, new object[] { });
                    RPCProtection();
                }
            }
        }

        public static void AutoStartMoles()
        {
            if (Time.time > stupiddelay)
            {
                stupiddelay = Time.time + 0.1f;
                foreach (WhackAMole stupid in GetWAMoles())
                {
                    int state = (int)Traverse.Create(stupid).Field("currentState").GetValue();
                    if (state == 0 || state == 4)
                    {
                        stupid.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, new object[] { });
                        RPCProtection();
                    }
                }
            }
        }

        public static void GetHoneyComb()
        {
            if (leftGrab)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    false,
                    false
                });
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    true,
                    true
                });
                RPCProtection();
            }
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    false,
                    true
                });
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
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
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
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
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                false,
                true
            });
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                false,
                false
            });
            RPCProtection();
        }

        public static void RainbowBracelet()
        {
            Patches.BraceletPatch.enabled = true;
            if (!GorillaTagger.Instance.offlineVRRig.nonCosmeticRightHandItem.IsEnabled)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.Others, new object[]
                {
                    true,
                    false
                });
                RPCProtection();

                GorillaTagger.Instance.offlineVRRig.nonCosmeticRightHandItem.EnableItem(true);
            }
            List<Color> rgbColors = new List<Color> { };
            for (int i=0; i<10; i++)
                rgbColors.Add(Color.HSVToRGB(((Time.frameCount / 180f) + (i / 10f)) % 1f, 1f, 1f));
            
            GorillaTagger.Instance.offlineVRRig.reliableState.isBraceletLeftHanded = false;
            GorillaTagger.Instance.offlineVRRig.reliableState.braceletSelfIndex = 99;
            GorillaTagger.Instance.offlineVRRig.reliableState.braceletBeadColors = rgbColors;
            GorillaTagger.Instance.offlineVRRig.friendshipBraceletRightHand.UpdateBeads(rgbColors, 99);
        }

        public static void RemoveRainbowBracelet()
        {
            Patches.BraceletPatch.enabled = false;
            if (!GorillaTagger.Instance.offlineVRRig.nonCosmeticRightHandItem.IsEnabled)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.Others, new object[]
                {
                    false,
                    false
                });
                RPCProtection();

                GorillaTagger.Instance.offlineVRRig.nonCosmeticRightHandItem.EnableItem(false);
            }

            GorillaTagger.Instance.offlineVRRig.reliableState.isBraceletLeftHanded = false;
            GorillaTagger.Instance.offlineVRRig.reliableState.braceletSelfIndex = 0;
            GorillaTagger.Instance.offlineVRRig.reliableState.braceletBeadColors.Clear();
            GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
        }

        public static void GiveBuilderWatch()
        {
            GorillaTagger.Instance.offlineVRRig.EnableBuilderResizeWatch(true);
            RPCProtection();
        }

        public static void RemoveBuilderWatch()
        {
            GorillaTagger.Instance.offlineVRRig.EnableBuilderResizeWatch(false);
            RPCProtection();
        }

        private static float lastTimeDingied = 0f;
        public static void QuestNoises()
        {
            if (rightTrigger > 0.5f && Time.time > lastTimeDingied)
            {
                lastTimeDingied = Time.time + 0.12f;
                GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/CosmeticsRoomAnchor/outsidestores_prefab/Bottom Layer/OutsideBuildings/Wardrobe Hut/MonkeBusinessStation").GetComponent<PhotonView>().RPC("BroadcastRedeemQuestPoints", RpcTarget.All, UnityEngine.Random.Range(0, 50));
            }
        }

        private static float delaybetweenscore = 0f;
        public static void MaxQuestScore()
        {
            if (Time.time > delaybetweenscore)
            {
                delaybetweenscore = Time.time + 1f;
                GorillaTagger.Instance.offlineVRRig.SetQuestScore(int.MaxValue);
            }
        }

        public static void GrabIDCard()
        {
            if (rightGrab)
            {
                GameObject.Find("Environment Objects/05Maze_PersistentObjects/HiddenIDCard/ID Card Anchor/ID Card Holdable").GetComponent<ScannableIDCard>().enabled = true;
                GameObject.Find("Environment Objects/05Maze_PersistentObjects/HiddenIDCard/ID Card Anchor/ID Card Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position; ;
            }
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
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
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
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
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
            GorillaLocomotion.GTPlayer.Instance.OnEnterWaterVolume(GorillaLocomotion.GTPlayer.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
            if (rightGrab)
            {
                if (Time.time > splashDel)
                {
                    GorillaTagger.Instance.myVRRig.RPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
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
            GorillaLocomotion.GTPlayer.Instance.OnEnterWaterVolume(GorillaLocomotion.GTPlayer.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
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
            GorillaLocomotion.GTPlayer.Instance.OnEnterWaterVolume(GorillaLocomotion.GTPlayer.Instance.bodyCollider, GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ILavaYou_PrefabV/Lava/ForestLavaWaterVolume").GetComponent<GorillaLocomotion.Swimming.WaterVolume>());
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
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
            Photon.Voice.Unity.Recorder mic = GorillaTagger.Instance.myRecorder;
            mic.SamplingRate = SamplingRate.Sampling08000;
            //oldBitrate = mic.Bitrate;
            mic.Bitrate = 5;

            mic.RestartRecording(true);
        }

        public static void HighQualityMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GorillaTagger.Instance.myRecorder;
            mic.SamplingRate = SamplingRate.Sampling16000;
            mic.Bitrate = 20000;

            mic.RestartRecording(true);
        }

        public static void LoudMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GorillaTagger.Instance.myRecorder;

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
            Photon.Voice.Unity.Recorder mic = GorillaTagger.Instance.myRecorder;

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
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GameObject.Find("Floating Bug Holdable").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void BatGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GameObject.Find("Cave Bat Holdable").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void BeachBallGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GameObject.Find("BeachBall").transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                }
            }
        }

        public static void GliderGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (GliderHoldable glider in GetGliders())
                    {
                        if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
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

        private static float lastReceivedTime = 0f;
        private static List<BuilderPiece> archivepiecesfiltered = new List<BuilderPiece>() { };
        public static BuilderPiece[] GetPiecesFiltered()
        {
            if (Time.time > lastReceivedTime)
            {
                archivepiecesfiltered = null;
                lastReceivedTime = Time.time + 5f;
            }
            if (archivepiecesfiltered == null)
            {
                archivepiecesfiltered = new List<BuilderPiece>() { };
                foreach (BuilderPiece piece in GetPieces())
                {
                    if (piece.pieceType > 0)
                    {
                        archivepiecesfiltered.Add(piece);
                    }
                }
            }
            return archivepieces.ToArray();
        }

        private static int pieceIdSet = -566818631;
        private static float blockDelay = 0f;
        public static void BlocksGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    BuilderTable.instance.RequestCreatePiece(pieceIdSet, NewPointer.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
                    RPCProtection();
                }
            }
        }

        private static float gbgd = 0f;
        public static void SelectBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    BuilderPiece possibly = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (possibly && Time.time > gbgd)
                    {
                        gbgd = Time.time + 0.1f;
                        pieceIdSet = possibly.pieceType;
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + possibly.name.Replace("(Clone)", "") + "!");
                        RPCProtection();
                    }
                }
            }
            //RPCProtection();
        }

        public static void EnableFastRopes()
        {
            Patches.RopePatch.enabled = true;
        }

        public static void DisableFastRopes()
        {
            Patches.RopePatch.enabled = false;
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

        public static void AntiGrab()
        {
            Patches.GrabPatch.enabled = true;
        }

        public static void AntiGrabDisabled()
        {
            Patches.GrabPatch.enabled = false;
        }

        public static void AntiKnockback()
        {
            Patches.KnockbackPatch.enabled = true;
        }

        public static void AntiKnockbackDisabled()
        {
            Patches.KnockbackPatch.enabled = false;
        }

        public static void LargeSnowballs()
        {
            Patches.EnablePatch.enabled = true;
        }

        public static void LargeSnowballsDisabled()
        {
            Patches.EnablePatch.enabled = false;
        }

        public static void FastSnowballs()
        {
            GetProjectile("LMACE. LEFT.");
            foreach (SnowballThrowable snowball in snowballs)
            {
                snowball.linSpeedMultiplier = 10f;
                snowball.maxLinSpeed = 99999f;
                snowball.maxWristSpeed = 99999f;
            }
        }

        public static void SlowSnowballs()
        {
            GetProjectile("LMACE. LEFT.");
            foreach (SnowballThrowable snowball in snowballs)
            {
                snowball.linSpeedMultiplier = 0.2f;
                snowball.maxLinSpeed = 6f;
                snowball.maxWristSpeed = 2f;
            }
        }

        public static void FixSnowballs()
        {
            GetProjectile("LMACE. LEFT.");
            foreach (SnowballThrowable snowball in snowballs)
            {
                snowball.linSpeedMultiplier = 1f;
                snowball.maxLinSpeed = 12f;
                snowball.maxWristSpeed = 4f;
            }
        }



        // You Need to add position And Colour To Fix

        /*public static void GlobalHoverboard()
        {
            GorillaLocomotion.GTPlayer.Instance.GrabPersonalHoverboard(true);
=======
            GorillaLocomotion.Player.Instance.SetEnableHoverboard(true);
            GorillaTagger.Instance.offlineVRRig.hoverboardVisual.SetActive(true);
        }

        public static void DisableGlobalHoverboard()
        {
            /GorillaLocomotion.GTPlayer.Instance.GrabPersonalHoverboard(false);
            GorillaTagger.Instance.offlineVRRig.hoverboardVisual.SetActive(false);
        }*/




            GorillaLocomotion.Player.Instance.SetEnableHoverboard(false);
            GorillaTagger.Instance.offlineVRRig.hoverboardVisual.SetActive(false);
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
                    if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
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
                    if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
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

        public static void SpamGrabBlocks()
        {
            if (rightGrab)
            {
                BuilderTable.instance.RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0);
                RPCProtection();
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
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.Respawn();
                }
                else
                {
                    glider.OnHover(null, null);
                }
            }
        }

        private static float delayer = -1f;
        public static void DestroyBlocks()
        {
            if (Time.time > delayer)
            {
                delayer = Time.time + 1f;
                archivepieces = null;
                int count = 0;
                foreach (BuilderPiece piece in GetPieces())
                {
                    if (count > 400)
                        break;
                    if (piece.gameObject.activeSelf)
                    {
                        count++;
                        BuilderTable.instance.RequestRecyclePiece(piece, true, 2);
                    }
                }
            }
        }

        private static bool isFiring = false;
        public static IEnumerator FireShotgun()
        {
            isFiring = true;

            if (!File.Exists("iisStupidMenu/shotgun.wav"))
                LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/refs/heads/main/shotgun.wav", "shotgun.wav");

            iiMenu.Mods.Spammers.Sound.PlayAudio("shotgun.wav");

            BuilderPiece bullet = null;

            yield return CreateGetPiece(1925587737, piece =>
            {
                bullet = piece;
            });
            while (bullet == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(bullet, true, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestDropPiece(bullet, TrueRightHand().position + TrueRightHand().forward * 0.65f + TrueRightHand().right * 0.03f + TrueRightHand().up * 0.05f, TrueRightHand().rotation, TrueRightHand().forward * 19.9f, Vector3.zero);
            yield return null;
        }

        public static void UnlimitedBuilding()
        {
            BuilderPieceInteractor.instance.maxHoldablePieceStackCount = int.MaxValue;
            Patches.UnlimitPatches.enabled = true;
        }

        public static void DisableUnlimitedBuilding()
        {
            BuilderPieceInteractor.instance.maxHoldablePieceStackCount = 50;
            Patches.UnlimitPatches.enabled = false;
        }

        public static void DestroyBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    BuilderPiece possibly = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (possibly)
                    {
                        BuilderTable.instance.RequestRecyclePiece(possibly, true, 2);
                        RPCProtection();
                    }
                }
            }
            //RPCProtection();
        }

        public static void BuildingBlockAura()
        {
            BuilderTable.instance.RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.offlineVRRig.transform.position + Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))) * 2f, Quaternion.identity, 0);
            RPCProtection();
        }

        public static void RainBuildingBlocks()
        {
            BuilderTable.instance.RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 4f, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
            RPCProtection();
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
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
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
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
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

        public static void OrbitBlocks()
        {
            BuilderTable.instance.RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 0f, MathF.Sin((float)Time.frameCount / 30)), Quaternion.identity, 0);
            RPCProtection();
        }

        public static void RideBug()
        {
            TeleportPlayer(GameObject.Find("Floating Bug Holdable").transform.position);
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void RideBat()
        {
            TeleportPlayer(GameObject.Find("Cave Bat Holdable").transform.position);
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void RideBeachBall()
        {
            TeleportPlayer(GameObject.Find("BeachBall").transform.position);
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

        public static void SmallBuilding()
        {
            Patches.BuildPatch.isEnabled = true;
        }

        public static void BigBuilding()
        {
            Patches.BuildPatch.isEnabled = false;
        }

        public static void MultiGrab()
        {
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
        }

        /*
        public static void PieceNameHelper()
        {
            if (BuilderPieceInteractor.instance.handState[1] == BuilderPieceInteractor.HandState.Grabbed)
                NotifiLib.SendNotification(BuilderPieceInteractor.instance.heldPiece[1].name + " type " + BuilderPieceInteractor.instance.heldPiece[1].pieceType);
        }*/

        public static int pieceId = -1;
        public static IEnumerator CreateGetPiece(int pieceType, Action<BuilderPiece> onComplete)
        {
            BuilderPiece target = null;

            Patches.CreatePatch.enabled = true;
            Patches.CreatePatch.pieceTypeSearch = pieceType;

            yield return null;

            BuilderTable.instance.RequestCreatePiece(pieceType, GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
            RPCProtection();

            while (pieceId < 0)
            {
                yield return null;
            }
            yield return null;

            target = BuilderTable.instance.GetPiece(pieceId);
            pieceId = -1;
            Patches.CreatePatch.enabled = false;
            Patches.CreatePatch.pieceTypeSearch = 0;

            onComplete?.Invoke(target); // so bad
        }

        public static IEnumerator CreateShotgun()
        {
            BuilderPiece basea = null;

            yield return CreateGetPiece(-1927069002, piece =>
            {
                basea = piece;
            });
            while (basea == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(basea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
            yield return null;

            BuilderPiece base2a = null;

            yield return CreateGetPiece(-1621444201, piece =>
            {
                base2a = piece;

            });
            while (base2a == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(base2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(base2a, base2a, 0, 0, 0, basea, 1, 0);
            yield return null;

            BuilderPiece slopea = null;

            yield return CreateGetPiece(-993249117, piece =>
            {
                slopea = piece;

            });
            while (slopea == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(slopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(slopea, slopea, 0, 0, 2, base2a, 1, 0);
            yield return null;

            BuilderPiece trigger = null;

            yield return CreateGetPiece(251444537, piece =>
            {
                trigger = piece;
            });
            while (trigger == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(trigger, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(trigger, trigger, -1, -2, 3, slopea, 1, 0);
            yield return null;

            BuilderPiece slopeb = null;

            yield return CreateGetPiece(-993249117, piece =>
            {
                slopeb = piece;
            });
            while (slopeb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(slopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(basea, trigger, 0, -2, 3, slopeb, 1, 0);
            yield return null;

            BuilderPiece base2b = null;

            yield return CreateGetPiece(-1621444201, piece =>
            {
                base2b = piece;
            });
            while (base2b == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(base2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(slopeb, slopeb, 0, 0, 2, base2b, 1, 0);
            yield return null;

            BuilderPiece baseb = null;

            yield return CreateGetPiece(-1927069002, piece =>
            {
                baseb = piece;
            });
            while (baseb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(baseb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(base2b, base2b, 0, 0, 0, baseb, 1, 0);
            yield return null;

            BuilderPiece minislopeb = null;

            yield return CreateGetPiece(1700655257, piece =>
            {
                minislopeb = piece;
            });
            while (minislopeb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(minislopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(baseb, slopea, 0, -3, 2, minislopeb, 2, 0);
            yield return null;

            BuilderPiece minislopea = null;

            yield return CreateGetPiece(1700655257, piece =>
            {
                minislopea = piece;
            });
            while (minislopea == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(minislopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(minislopeb, slopeb, 0, -3, 2, minislopea, 2, 0);
            yield return null;

            BuilderPiece minislope2a = null;

            yield return CreateGetPiece(1700655257, piece =>
            {
                minislope2a = piece;
            });
            while (minislope2a == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(minislope2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(minislopea, minislopeb, 0, 0, 2, minislope2a, 1, 0);
            yield return null;

            BuilderPiece minislope2b = null;

            yield return CreateGetPiece(1700655257, piece =>
            {
                minislope2b = piece;
            });
            while (minislope2b == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(minislope2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(minislope2a, minislopea, 0, 0, 2, minislope2b, 1, 0);
            yield return null;

            BuilderPiece flatthinga = null;

            yield return CreateGetPiece(477262573, piece =>
            {
                flatthinga = piece;
            });
            while (flatthinga == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(flatthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(minislope2b, minislope2b, 0, -1, 2, flatthinga, 2, 0);
            yield return null;

            BuilderPiece flatthingb = null;

            yield return CreateGetPiece(477262573, piece =>
            {
                flatthingb = piece;
            });
            while (flatthingb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(flatthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(flatthinga, minislope2a, 0, -1, 2, flatthingb, 2, 0);
            yield return null;

            BuilderPiece connectorthinga = null;

            yield return CreateGetPiece(251444537, piece =>
            {
                connectorthinga = piece;
            });
            while (connectorthinga == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(connectorthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(flatthingb, flatthinga, -1, 1, 3, connectorthinga, 1, 0);
            yield return null;

            BuilderPiece connectorthingb = null;

            yield return CreateGetPiece(661312857, piece =>
            {
                connectorthingb = piece;
            });
            while (connectorthingb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(connectorthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(connectorthinga, connectorthinga, -1, 0, 1, connectorthingb, 1, 0);
            yield return null;

            BuilderPiece connectorthingc = null;

            yield return CreateGetPiece(661312857, piece =>
            {
                connectorthingc = piece;
            });
            while (connectorthingc == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(connectorthingc, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(connectorthingb, connectorthinga, 0, 0, 1, connectorthingc, 1, 0);
            yield return null;

            BuilderPiece barrela = null;

            yield return CreateGetPiece(661312857, piece =>
            {
                barrela = piece;
            });
            while (barrela == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(barrela, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(connectorthingc, connectorthingb, 0, 0, 1, barrela, 1, 0);
            yield return null;

            BuilderPiece barrelb = null;

            yield return CreateGetPiece(661312857, piece =>
            {
                barrelb = piece;
            });
            while (barrelb == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(barrelb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(barrela, barrela, 0, 0, 2, barrelb, 1, 0);
            yield return null;

            BuilderPiece scope = null;

            yield return CreateGetPiece(-648273975, piece =>
            {
                scope = piece;
            });
            while (scope == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(scope, false, Vector3.zero, Quaternion.identity);
            yield return null;
            BuilderTable.instance.RequestPlacePiece(barrelb, minislope2a, -2, 1, 3, scope, 1, 0);
            yield return null;
            BuilderTable.instance.RequestDropPiece(scope, GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, Vector3.zero);
            yield return null;
            // pos is forward/back, left/right, up/down
            BuilderTable.instance.RequestGrabPiece(basea, false, new Vector3(-0.2f, 0.01f, -0.3f), new Quaternion(0f, 0.1f, 0.75f, -0.6f));
            yield return null;
        }

        private static bool lastgripcrap = false;
        private static bool lasttrigcrap = false;
        public static void Shotgun()
        {
            if (isFiring)
                ControllerInputPoller.instance.leftControllerGripFloat = 1f;

            if (rightGrab && !lastgripcrap)
                CoroutineManager.RunCoroutine(CreateShotgun());

            if (rightGrab && (rightTrigger > 0.5f && !lasttrigcrap))
                CoroutineManager.RunCoroutine(FireShotgun());

            lastgripcrap = rightGrab;
            lasttrigcrap = rightTrigger > 0.5f;
        }

        public static IEnumerator CreateMassiveBlock()
        {
            GorillaTagger.Instance.offlineVRRig.sizeManager.currentSizeLayerMaskValue = 2;
            yield return new WaitForSeconds(0.55f);

            BuilderPiece stupid = null;

            yield return CreateGetPiece(pieceIdSet, piece =>
            {
                stupid = piece;
            });
            while (stupid == null)
            {
                yield return null;
            }

            BuilderTable.instance.RequestGrabPiece(stupid, false, Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(0.7f);

            GorillaTagger.Instance.offlineVRRig.sizeManager.currentSizeLayerMaskValue = 13;
        }

        public static void MassiveBlock()
        {
            if (rightGrab && !lastgripcrap)
                CoroutineManager.RunCoroutine(CreateMassiveBlock());

            lastgripcrap = rightGrab;
        }

        public static void AtticSizeToggle()
        {
            if (rightTrigger > 0.5f)
                GorillaTagger.Instance.offlineVRRig.sizeManager.currentSizeLayerMaskValue = 13;

            if (rightGrab)
                GorillaTagger.Instance.offlineVRRig.sizeManager.currentSizeLayerMaskValue = 2;
        }

        public static void SlowMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetMonsters())
                {
                    if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                    monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                }
            }
        }

        public static void MonsterGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (MonkeyeAI monkeyeAI in GetMonsters())
                    {
                        if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetMonsters();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                float offset = (360f / (float)them.Length) * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetMonsters())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
            }
        }

        public static List<BuilderPiece> GetBlocks(string blockname)
        {
            List<BuilderPiece> blocks = new List<BuilderPiece> { };

            foreach (BuilderPiece lol in GetPiecesFiltered())
            {
                if (lol.name.ToLower().Contains(blockname))
                {
                    blocks.Add(lol);
                }
            }

            return blocks;
        }

        private static List<BuilderPiece> potentialgrabbedpieces = new List<BuilderPiece> { };
        public static void GrabAllBlocksNearby()
        {
            if (rightGrab && Time.time > blockDelay)
            {
                blockDelay = Time.time + 0.25f;
                int amnt = 0;
                foreach (BuilderPiece piece in GetPieces())
                {
                    if (Vector3.Distance(piece.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 2.5f)
                    {
                        if (!potentialgrabbedpieces.Contains(piece))
                        {
                            amnt++;
                            if (amnt < 8)
                            {
                                BuilderTable.instance.RequestGrabPiece(piece, false, new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)), Quaternion.identity);
                                potentialgrabbedpieces.Add(piece);
                            }
                        }
                    }
                }
                RPCProtection();
            }
            if (rightTrigger > 0.5f && Time.time > blockDelay)
            {
                blockDelay = Time.time + 0.25f;
                foreach (BuilderPiece piece in potentialgrabbedpieces)
                {
                    BuilderTable.instance.RequestDropPiece(piece, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, new Vector3(UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f)), new Vector3(UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f)));
                }
                potentialgrabbedpieces.Clear();
                RPCProtection();
            }
        }

        /*
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
        */

        public static void PopAllBalloons()
        {
            foreach (BalloonHoldable balloon in GetBalloons())
            {
                typeof(BalloonHoldable).GetMethod("OwnerPopBalloon", BindingFlags.NonPublic | BindingFlags.Static).Invoke(balloon, new object[] { });
                balloon.PopBalloonRemote();
            }
        }

        public static void GrabBalloons()
        {
            if (rightGrab)
            {
                foreach (BalloonHoldable balloon in GetBalloons())
                {
                    if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
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
                if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
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
                if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
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
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (BalloonHoldable balloon in GetBalloons())
                    {
                        if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
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
                if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
                {
                    balloon.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                }
                else
                {
                    balloon.WorldShareableRequestOwnership();
                }
            }
        }

        // I would like to apologize to anyone who had this beforehand
        // "Tubski" will be long loved forever because that was the original name of this mod I don't know what that means
        public static void BecomeBalloon()
        {
            if (rightTrigger > 0.5f)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.inTryOnRoom = true;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                bool FoundBalloon = false;
                foreach (BalloonHoldable Balloon in GetBalloons())
                {
                    if (Balloon.ownerRig == GorillaTagger.Instance.offlineVRRig && Balloon.gameObject.name.Contains("LMAMI"))
                    {
                        FoundBalloon = true;

                        Traverse.Create(Balloon).Field("maxDistanceFromOwner").SetValue(float.MaxValue);
                        Balloon.currentState = TransferrableObject.PositionState.Dropped;

                        Balloon.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.up * - 1f);
                        Balloon.gameObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
                    }
                }

                if (!FoundBalloon)
                {
                    CosmeticsController.instance.ApplyCosmeticItemToSet(GorillaTagger.Instance.offlineVRRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMAAP."), true, false);
                    CosmeticsController.instance.UpdateWornCosmetics(true);
                    RPCProtection();

                    archiveballoons = null;
                }
            }
            else
            {
                if (!GorillaTagger.Instance.offlineVRRig.enabled)
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void DestroyGliders()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                }
                else
                {
                    glider.OnHover(null, null);
                }
            }
        }

        public static void LowercaseName()
        {
            string name = PhotonNetwork.NickName.ToLower();
            string outputname = "";
            foreach (char ch in name)
            {
                if (superscript.ContainsKey(ch))
                {
                    outputname += superscript[ch];
                } else
                {
                    outputname += ch;
                }
            }
            ChangeName(outputname);
        }

        public static void LongName()
        {
            ChangeName("ǄǄǄǄǄǄǄǄǄǄǄǄ");
        }

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
                for (int i = 0; i < 12; i++)
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

        public static void BecomeMinigamesKid()
        {
            string[] names = new string[]
            {
                "MINIGAMES",
                "MINIGAMESKID",
                "LITTLETIMMY",
                "TIMMY",
                "SILLYBILLY"
            };
            Color[] colors = new Color[]
            {
                Color.cyan,
                Color.green,
                Color.red,
                Color.magenta
            };

            ChangeName(names[UnityEngine.Random.Range(0, names.Length - 1)]);
            ChangeColor(colors[UnityEngine.Random.Range(0, colors.Length - 1)]);
        }

        public static void BecomeHiddenOnLeaderboard() {
            ChangeName("________");
            ChangeColor(new Color32(0, 53, 2, 255));
        }

        public static void CopyIdentityGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > stealIdentityDelay)
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

        private static Dictionary<string[], int[]> cachePacked = new Dictionary<string[], int[]> { };
        public static int[] PackCosmetics(string[] Cosmetics)
        {
            if (cachePacked.ContainsKey(Cosmetics))
                return cachePacked[Cosmetics];

            CosmeticsController.CosmeticSet Set = new CosmeticsController.CosmeticSet(Cosmetics, CosmeticsController.instance);
            int[] PackedIDs = Set.ToPackedIDArray();
            cachePacked.Add(Cosmetics, PackedIDs);
            return PackedIDs;
        }

        private static List<string> ownedarchive = null;
        private static string[] GetOwnedCosmetics()
        {
            if (ownedarchive == null)
            {
                ownedarchive = new List<string> { };
                foreach (CosmeticsController.CosmeticItem dearlord in CosmeticsController.instance.allCosmetics)
                {
                    if (GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains(dearlord.itemName))
                    {
                        ownedarchive.Add(dearlord.itemName);
                    }
                }
            }
            return ownedarchive.ToArray();
        }
        private static List<string> tryonarchive = null;
        private static string[] GetTryOnCosmetics()
        {
            if (tryonarchive == null)
            {
                tryonarchive = new List<string> { };
                foreach (CosmeticsController.CosmeticItem dearlord in CosmeticsController.instance.allCosmetics)
                {
                    if (dearlord.canTryOn)
                    {
                        tryonarchive.Add(dearlord.itemName);
                    }
                }
            }
            return tryonarchive.ToArray();
        }

        private static float delay = 0f;
        public static void SpazAccessories()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                delay = Time.time + 0.05f;
                string[] owned = GorillaTagger.Instance.offlineVRRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> holyshit = new List<string> { };
                    for (int i = 0; i <= amnt; i++)
                    {
                        holyshit.Add(owned[UnityEngine.Random.Range(0, owned.Length - 1)]);
                    }
                    if (GorillaTagger.Instance.offlineVRRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        GorillaTagger.Instance.offlineVRRig.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        GorillaTagger.Instance.offlineVRRig.cosmeticSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, new object[] { PackCosmetics(holyshit.ToArray()), PackCosmetics(holyshit.ToArray()) });
                    RPCProtection();
                }
            }
        }

        public static void SpazAccessoriesOthers()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                delay = Time.time + 0.05f;
                string[] owned = GorillaTagger.Instance.offlineVRRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> holyshit = new List<string> { };
                    for (int i = 0; i <= amnt; i++)
                    {
                        holyshit.Add(owned[UnityEngine.Random.Range(0, owned.Length - 1)]);
                    }
                    if (GorillaTagger.Instance.offlineVRRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        GorillaTagger.Instance.offlineVRRig.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        GorillaTagger.Instance.offlineVRRig.cosmeticSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, new object[] { PackCosmetics(holyshit.ToArray()), PackCosmetics(holyshit.ToArray()) });
                    RPCProtection();
                }
            }
        }

        private static float delayonhold = 0f;
        public static void StickyHoldables()
        {
            if (Time.time > delayonhold)
            {
                delayonhold = Time.time + 0.1f;
                foreach (TransferrableObject cosmet in GetTransferrableObjects())
                {
                    if (cosmet.IsMyItem())
                    {
                        if (cosmet.currentState == TransferrableObject.PositionState.OnLeftArm || cosmet.currentState == TransferrableObject.PositionState.OnLeftShoulder)
                        {
                            cosmet.currentState = TransferrableObject.PositionState.InLeftHand;
                        }
                        if (cosmet.currentState == TransferrableObject.PositionState.OnRightArm || cosmet.currentState == TransferrableObject.PositionState.OnRightShoulder || cosmet.currentState == TransferrableObject.PositionState.OnChest)
                        {
                            cosmet.currentState = TransferrableObject.PositionState.InRightHand;
                        }
                    }
                }
            }
        }

        public static void SpazHoldables()
        {
            if (Time.time > delayonhold)
            {
                delayonhold = Time.time + 0.1f;
                foreach (TransferrableObject cosmet in GetTransferrableObjects())
                {
                    if (cosmet.IsMyItem())
                    {
                        cosmet.currentState = (TransferrableObject.PositionState)((int)cosmet.currentState * 2);
                        if ((int)cosmet.currentState > 128)
                        {
                            cosmet.currentState = TransferrableObject.PositionState.OnLeftArm;
                        }
                    }
                }
            }
        }

        private static int[] archiveCosmetics = null;
        public static void TryOnAnywhere()
        {
            archiveCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
            string[] itjustworks = new string[] { "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU." };
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(itjustworks, CosmeticsController.instance);
            GorillaTagger.Instance.offlineVRRig.cosmeticSet = new CosmeticsController.CosmeticSet(itjustworks, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, new object[] { PackCosmetics(itjustworks), CosmeticsController.instance.tryOnSet.ToPackedIDArray() });
            RPCProtection();
        }

        public static void TryOffAnywhere()
        {
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            GorillaTagger.Instance.offlineVRRig.cosmeticSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.All, new object[] { archiveCosmetics, CosmeticsController.instance.tryOnSet.ToPackedIDArray() });
            RPCProtection();
        }

        public static void AddCosmeticToCart(string cosmetic)
        {
            CosmeticsController.instance.currentCart.Insert(0, CosmeticsController.instance.GetItemFromDict(cosmetic));
            CosmeticsController.instance.UpdateShoppingCart();
        }

        private static int rememberdirectory = 0;
        public static void CosmeticBrowser()
        {
            rememberdirectory = pageNumber;
            buttonsType = 29;
            pageNumber = 0;
            List<ButtonInfo> cosmeticbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Cosmetic Browser", method = () => RemoveCosmeticBrowser(), isTogglable = false, toolTip = "Returns you back to the fun mods." } };
            foreach (GorillaNetworking.CosmeticsController.CosmeticItem hat in GorillaNetworking.CosmeticsController.instance.allCosmetics)
            {
                if (hat.canTryOn)
                {
                    cosmeticbuttons.Add(new ButtonInfo { buttonText = ToTitleCase(hat.overrideDisplayName), method = () => Fun.AddCosmeticToCart(hat.itemName), isTogglable = false, toolTip = "Adds the " + hat.overrideDisplayName.ToLower() + "to your cart." });
                }
            }
            Buttons.buttons[29] = cosmeticbuttons.ToArray();
        }

        public static void RemoveCosmeticBrowser()
        {
            Settings.EnableFun();
            pageNumber = rememberdirectory;
        }

        public static void AutoLoadCosmetics()
        {
            Patches.RequestPatch.enabled = true;
            if (Patches.RequestPatch.currentCoroutine == null)
                Patches.RequestPatch.currentCoroutine = CoroutineManager.RunCoroutine(Patches.RequestPatch.LoadCosmetics());
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").GetComponent<CosmeticBoundaryTrigger>().enabled = false;
        }

        public static void NoAutoLoadCosmetics()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").GetComponent<CosmeticBoundaryTrigger>().enabled = true;
            Patches.RequestPatch.enabled = false;
        }

        private static bool lasttagged = false;
        public static void DisableCosmeticsOnTag()
        {
            if (!lasttagged && PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.Others, new object[] { new string[] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" }, new string[] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" } });
                RPCProtection();
            }
            if (lasttagged && !PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.Others, new object[] { CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray() });
                RPCProtection();
            }
            lasttagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
        }
    }
}
