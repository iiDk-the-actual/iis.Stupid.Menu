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
using PlayFab;
using PlayFab.ClientModels;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
        }

        public static void Fullshade()
        {
            LightmapSettings.lightmaps = hell;
            GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
        }

        /*
        private static Volume Volume;
        public static void EnableShaders()
        {
            if (Volume == null)
                Volume = GameObject.Find("Main Camera").AddComponent<Volume>();

            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData = Camera.main.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            camData.renderPostProcessing = true;

            QualitySettings.antiAliasing = 4;

            Volume.isGlobal = true;
            VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
            Volume.profile = profile;

            UnityEngine.Rendering.Universal.Bloom bloom;
            if (!profile.TryGet(out bloom))
                bloom = profile.Add<UnityEngine.Rendering.Universal.Bloom>(true);
            
            bloom.intensity.value = 1f;
            bloom.threshold.value = 1f;
            bloom.active = true;

            UnityEngine.Rendering.Universal.Tonemapping tonemapping;
            if (!profile.TryGet(out tonemapping))
                tonemapping = profile.Add<UnityEngine.Rendering.Universal.Tonemapping>(true);
            
            tonemapping.mode.value = UnityEngine.Rendering.Universal.TonemappingMode.ACES;
            tonemapping.active = true;

            UnityEngine.Rendering.Universal.MotionBlur motionBlur;
            if (!profile.TryGet(out motionBlur))
                motionBlur = profile.Add<UnityEngine.Rendering.Universal.MotionBlur>(true);
            
            motionBlur.intensity.value = 0.4f;
            motionBlur.active = true;

            UnityEngine.Rendering.Universal.ColorAdjustments colorAdjust;
            if (!profile.TryGet(out colorAdjust))
                colorAdjust = profile.Add<UnityEngine.Rendering.Universal.ColorAdjustments>(true);
            
            colorAdjust.saturation.value = 20f;
            colorAdjust.contrast.value = 15f;
            colorAdjust.postExposure.value = 0.2f;
            colorAdjust.active = true;
        }

        public static void DisableShaders()
        {
            UnityEngine.Object.Destroy(Volume);
        }*/

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

                FriendshipGroupDetection.Instance.suppressPartyCreationUntilTimestamp = 0f;
                FriendshipGroupDetection.Instance.groupCreateAfterTimestamp = 0f;

                List<int> provisionalMembers = FriendshipGroupDetection.Instance.playersInProvisionalGroup;

                if (provisionalMembers.Count > 0)
                {
                    Color targetColor = GTColor.RandomHSV(FriendshipGroupDetection.Instance.braceletRandomColorHSVRanges);
                    FriendshipGroupDetection.Instance.myBraceletColor = targetColor;

                    List<int> members = new List<int> { PhotonNetwork.LocalPlayer.ActorNumber };
                    foreach (Player player in PhotonNetwork.PlayerListOthers)
                    {
                        if (FriendshipGroupDetection.Instance.IsInMyGroup(player.UserId) || provisionalMembers.Contains(player.ActorNumber))
                            members.Add(player.ActorNumber);
                    }
                    FriendshipGroupDetection.Instance.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(targetColor), members.ToArray(), false);
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
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(Important.RandomRoomName(), GorillaNetworking.JoinType.ForceJoinWithParty);
                partyTime = Time.time + 0.25f;
                phaseTwo = false;
                amountPartying = FriendshipGroupDetection.Instance.myPartyMemberIDs.Count - 1;
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
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("KKK", GorillaNetworking.JoinType.ForceJoinWithParty);
                partyTime = Time.time + 0.25f;
                phaseTwo = false;
                amountPartying = FriendshipGroupDetection.Instance.myPartyMemberIDs.Count - 1;
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
                    RaycastHit ray = GorillaLocomotion.GTPlayer.Instance.lastHitInfoHand;
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
                    RaycastHit ray = GorillaLocomotion.GTPlayer.Instance.lastHitInfoHand;
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
        public static void Boop(int sound = 84)
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
                        isBoopLeft = D1 < threshold;

                    if (!isBoopRight)
                        isBoopRight = D2 < threshold;
                }
            }
            if (isBoopLeft && !lastlhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        sound,
                        true,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(sound, true, 999999f);
                }
            }
            if (isBoopRight && !lastrhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]{
                        sound,
                        false,
                        999999f
                    });
                    RPCProtection();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(sound, false, 999999f);
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                        {
                            if (line.linePlayer == GetPlayerFromVRRig(gunTarget))
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
            foreach (RadioButtonGroupWearable djSet in GetAllType<RadioButtonGroupWearable>())
            {
                if (djSet.enabled)
                    djSet.enabled = false;
            }
        }

        public static void UnmuteDJSets()
        {
            foreach (RadioButtonGroupWearable djSet in GetAllType<RadioButtonGroupWearable>())
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

                if (gunLocked && lockTarget != null)
                {
                    AudioClip clippy = lockTarget.gameObject.GetComponent<GorillaSpeakerLoudness>().speaker.gameObject.GetComponent<AudioSource>().clip;
                    if (GorillaTagger.Instance.myRecorder.AudioClip != clippy)
                        GorillaTagger.Instance.myRecorder.AudioClip = clippy;
                }
                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                        GorillaTagger.Instance.myRecorder.AudioClip = lockTarget.gameObject.GetComponent<GorillaSpeakerLoudness>().speaker.gameObject.GetComponent<AudioSource>().clip;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
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

        private static float tapDelay = 0f;
        public static void TapAllBells()
        {
            if (rightGrab)
            {
                if (Time.time > tapDelay)
                {
                    foreach (TappableBell bell in GetAllType<TappableBell>())
                    {
                        bell.OnTap(1f);
                        RPCProtection();
                    }
                    tapDelay = Time.time + 0.1f;
                }
            }
        }

        public static void TapAllCrystals()
        {
            if (rightGrab)
            {
                if (Time.time > tapDelay)
                {
                    foreach (GorillaCaveCrystal bell in GetAllType<GorillaCaveCrystal>())
                    {
                        bell.OnTap(1f);
                        RPCProtection();
                    }
                    tapDelay = Time.time + 0.1f;
                }
            }
        }

        private static float buttonDelay;
        public static void ActivateAllDoors()
        {
            if (rightGrab)
            {
                if (Time.time > buttonDelay)
                {
                    foreach (GhostLabButton button in GetAllType<GhostLabButton>())
                    {
                        button.ButtonActivation();
                        RPCProtection();
                    }
                    buttonDelay = Time.time + 0.1f;
                }
            }
        }

        private static float hitDelay = 0f;
        public static void AutoHitMoles()
        {   
            foreach (Mole mole in GetAllType<Mole>())
            {
                int state = mole.randomMolePickedIndex;
                if (mole.CanTap() && mole.moleTypes[state].isHazard == false && Time.time > hitDelay)
                {
                    hitDelay = Time.time + 0.1f;

                    mole.OnTap(1f);
                    RPCProtection();
                }
            }
        }

        public static void AutoHitHazards()
        {
            foreach (Mole mole in GetAllType<Mole>())
            {
                int state = mole.randomMolePickedIndex;
                if (mole.CanTap() && mole.moleTypes[state].isHazard && Time.time > hitDelay)
                {
                    hitDelay = Time.time + 0.1f;
                    mole.OnTap(1f);
                    RPCProtection();
                }
            }
        }

        private static float moleMachineDelay;
        public static void SpazMoleMachines()
        {
            if (Time.time > moleMachineDelay)
            {
                moleMachineDelay = Time.time + 0.25f;
                foreach (WhackAMole moleMachine in GetAllType<WhackAMole>())
                {
                    moleMachine.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, new object[] { });
                    RPCProtection();
                }
            }
        }

        public static void AutoStartMoles()
        {
            if (Time.time > moleMachineDelay)
            {
                moleMachineDelay = Time.time + 0.1f;
                foreach (WhackAMole moleMachine in GetAllType<WhackAMole>())
                {
                    if (moleMachine.currentState == WhackAMole.GameState.Off || moleMachine.currentState == WhackAMole.GameState.TimesUp)
                    {
                        moleMachine.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, new object[] { });
                        RPCProtection();
                    }
                }
            }
        }

        public static void GetBracelet()
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

        private static bool braceletState;
        public static void BraceletSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                {
                    braceletState,
                    false
                });
                RPCProtection();
                braceletState = !braceletState;
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

        public static void FakeFPS()
        {
            Patches.FPSPatch.enabled = true;
            Patches.FPSPatch.spoofFPSValue = UnityEngine.Random.Range(0, 255);
        }

        public static void NoFakeFPS()
        {
            Patches.FPSPatch.enabled = false;
        }

        public static void EverythingGrabbable()
        {
            GamePlayerLocal.instance.gamePlayer.DisableGrabbing(false);
            foreach (GameEntity entity in GameEntityManager.instance.entities)
            {
                if (entity != null)
                {
                    try
                    {
                        entity.onlyGrabActorNumber = -1;
                        entity.pickupable = true;
                    } catch { }
                }
            }
        }

        public static void EntityReach()
        {
            Patches.EntityGrabPatch.enabled = true;
        }

        public static void NoEntityReach()
        {
            Patches.EntityGrabPatch.enabled = false;
        }

        public static void GrabIDCard()
        {
            if (rightGrab)
            {
                GamePlayer plr = GamePlayerLocal.instance.gamePlayer;

                if (!plr.IsHoldingEntity(false))
                {
                    foreach (GRBadge grBadge in GameObject.Find("GhostReactorRoot/GhostReactorZone/GhostReactorEmployeeBadges").GetComponent<GRUIStationEmployeeBadges>().registeredBadges)
                    {
                        GameEntity entity = grBadge.gameEntity;
                        if (entity.onlyGrabActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = entity.transform.position;

                            GameEntityManager.instance.RequestGrabEntity(entity.id, false, Vector3.zero, Quaternion.identity);
                        }
                    }
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        private static float crateDelay;
        public static void BreakAllCrates()
        {
            if (rightTrigger > 0.5f)
            {
                GamePlayer plr = GamePlayerLocal.instance.gamePlayer;

                if (!plr.IsHoldingEntity(false))
                {
                    foreach (GameEntity entity in GameEntityManager.instance.entities)
                    {
                        if (entity.gameObject.name.Contains("BreakableCrate"))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = entity.transform.position;

                            if (Time.time > crateDelay)
                            {
                                GhostReactorManager.instance.ReportBreakableBroken(entity);
                                crateDelay = Time.time + 0.1f;
                            }
                        }
                    }
                }
            }
            else
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        private static float purchaseDelay;
        public static void PurchaseAllToolStations()
        {
            if (Time.time > purchaseDelay)
            {
                GhostReactorManager.instance.ToolPurchaseStationRequest(UnityEngine.Random.Range(0, GhostReactorManager.instance.reactor.toolPurchasingStations.Count - 1), GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
                purchaseDelay = Time.time + 0.1f;
            }
        }

        public static void MaxCurrencySelf()
        {
            if (!PhotonNetwork.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).currency = int.MaxValue;
        }

        public static void MaxCurrencyGun()
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
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).currency = int.MaxValue;
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void MaxCurrencyAll()
        {
            if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.currency = int.MaxValue;
            }
        }

        public static void RemoveCurrencySelf()
        {
            if (!PhotonNetwork.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).currency = 0;
        }

        public static void RemoveCurrencyGun()
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
                        if (PhotonNetwork.LocalPlayer.IsMasterClient)
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).currency = 0;
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void RemoveCurrencyAll()
        {
            if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.currency = 0;
            }
        }

        public static void Invincibility()
        {
            if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            GRPlayer plr = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);

            if (plr.State == GRPlayer.GRPlayerState.Ghost)
                GhostReactorManager.instance.RequestPlayerStateChange(plr, GRPlayer.GRPlayerState.Alive);

            plr.hp = plr.maxHp;
        }

        public static void SetPlayerState(Player Target, GRPlayer.GRPlayerState State)
        {
            GRPlayer GRPlayer = GRPlayer.Get(Target.ActorNumber);

            if (GRPlayer.State == State)
                return;

            if ((Target == PhotonNetwork.LocalPlayer && State == GRPlayer.GRPlayerState.Ghost)
                    || (PhotonNetwork.IsMasterClient && State == GRPlayer.GRPlayerState.Alive)
                    )
            {
                GhostReactorManager.instance.RequestPlayerStateChange(GRPlayer, State);
                RPCProtection();
                return;
            }

            if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            if (State == GRPlayer.GRPlayerState.Ghost)
                CoroutineManager.instance.StartCoroutine(KillTarget(Target));
        }

        public static void SetPlayerState(NetPlayer Target, GRPlayer.GRPlayerState State)
        {
            SetPlayerState(NetPlayerToPlayer(Target), State);
        }

        public static void SetPlayerState(VRRig Target, GRPlayer.GRPlayerState State)
        {
            SetPlayerState(NetPlayerToPlayer(GetPlayerFromVRRig(Target)), State);
        }

        public static IEnumerator KillTarget(Player Target)
        {
            GRPlayer GRPlayer = GRPlayer.Get(Target.ActorNumber);
            VRRig Rig = RigManager.GetVRRigFromPlayer(Target);

            int netId = GameEntityManager.instance.CreateNetId();

            GameEntityManager.instance.photonView.RPC("CreateItemRPC", Target, new object[]
            {
                new int[] { netId },
                new int[] { (int)GTZone.ghostReactor },
                new int[] { 48354877 },
                new long[] { BitPackUtils.PackWorldPosForNetwork(Rig.transform.position) },
                new int[] { BitPackUtils.PackQuaternionForNetwork(Rig.transform.rotation) },
                new long[] { 0L }
            });

            GameAgentManager.instance.photonView.RPC("ApplyBehaviorRPC", Target, new object[]
            {
                new int[] { netId },
                new byte[] { 6 }
            });

            GRPlayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost);

            RPCProtection();

            yield return null;
            yield return null;
            yield return null;

            GameEntityManager.instance.photonView.RPC("DestroyItemRPC", Target, new object[]
            {
                new int[] { netId }
            });

            RPCProtection();
        }

        private static float killDelay;
        public static void KillSelf()
        {
            SetPlayerState(PhotonNetwork.LocalPlayer, GRPlayer.GRPlayerState.Ghost);
        }

        public static void KillGun()
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
                        SetPlayerState(gunTarget, GRPlayer.GRPlayerState.Ghost);
                }
            }
        }

        public static void KillAll()
        {
            foreach (Player target in PhotonNetwork.PlayerList)
                SetPlayerState(target, GRPlayer.GRPlayerState.Ghost);
        }

        public static void ReviveSelf()
        {
            SetPlayerState(PhotonNetwork.LocalPlayer, GRPlayer.GRPlayerState.Alive);
        }

        public static void ReviveGun()
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
                        SetPlayerState(gunTarget, GRPlayer.GRPlayerState.Alive);
                    }
                }
            }
        }

        public static void ReviveAll()
        {
            foreach (Player target in PhotonNetwork.PlayerList)
            {
                SetPlayerState(target, GRPlayer.GRPlayerState.Alive);
            }
        }

        public static void SpazKillSelf()
        {
            if (Time.time > killDelay)
            {
                killDelay = Time.time + 0.1f;

                GRPlayer plr = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
                SetPlayerState(PhotonNetwork.LocalPlayer, plr.State == GRPlayer.GRPlayerState.Alive ? GRPlayer.GRPlayerState.Ghost : GRPlayer.GRPlayerState.Alive);
            }
        }

        public static void SpazKillGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && Time.time > killDelay)
                    {
                        killDelay = Time.time + 0.1f;
                        GRPlayer plr = GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber);
                        SetPlayerState(gunTarget, plr.State == GRPlayer.GRPlayerState.Alive ? GRPlayer.GRPlayerState.Ghost : GRPlayer.GRPlayerState.Alive);
                    }
                }
            }
        }

        public static void SpazKillAll()
        {
            if (Time.time > killDelay)
            {
                foreach (Player target in PhotonNetwork.PlayerList)
                {
                    killDelay = Time.time + 0.1f;

                    GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                    SetPlayerState(target, plr.State == GRPlayer.GRPlayerState.Alive ? GRPlayer.GRPlayerState.Ghost : GRPlayer.GRPlayerState.Alive);
                }
            }
        }

        public static void SpazToolStations()
        {
            if (Time.time > purchaseDelay)
            {
                GhostReactorManager.instance.ToolPurchaseStationRequest(UnityEngine.Random.Range(0, GhostReactorManager.instance.reactor.toolPurchasingStations.Count - 1), (GhostReactorManager.ToolPurchaseStationAction)UnityEngine.Random.Range(0, 2));
                purchaseDelay = Time.time + 0.1f;
            }
        }

        public static void LowQualityMicrophone()
        {
            Photon.Voice.Unity.Recorder mic = GorillaTagger.Instance.myRecorder;
            mic.SamplingRate = SamplingRate.Sampling08000;
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
                    foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
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
                foreach (BuilderPiece piece in GetAllType<BuilderPiece>())
                {
                    if (piece.pieceType > 0)
                    {
                        archivepiecesfiltered.Add(piece);
                    }
                }
            }
            return archivepiecesfiltered.ToArray();
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
                    RequestCreatePiece(pieceIdSet, NewPointer.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
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
                    BuilderPiece gunTarget = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (gunTarget && Time.time > gbgd)
                    {
                        gbgd = Time.time + 0.1f;
                        pieceIdSet = gunTarget.pieceType;
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + gunTarget.name.Replace("(Clone)", "") + "!");
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

        public static void AntiNoclip()
        {
            Patches.CaughtPatch.enabled = true;
        }

        public static void AntiNoclipDisabled()
        {
            Patches.CaughtPatch.enabled = false;
        }

        public static void AntiKnockback()
        {
            Patches.KnockbackPatch.enabled = true;
        }

        public static void AntiKnockbackDisabled()
        {
            Patches.KnockbackPatch.enabled = false;
        }

        public static void AntiSting()
        {
            Patches.BeesPatch.enabled = true;
        }

        public static void AntiStingDisabled()
        {
            Patches.BeesPatch.enabled = false;
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

        public static void FastHoverboard()
        {
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMax = float.MaxValue;
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 5f;
            GorillaLocomotion.GTPlayer.Instance.hoverboardBoostGracePeriod = 0f;
            GorillaLocomotion.GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 1f;
        }

        public static void SlowHoverboard()
        {
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMax = 3.5f;
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.025f;
            GorillaLocomotion.GTPlayer.Instance.hoverboardBoostGracePeriod = 3f;
            GorillaLocomotion.GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.1f;
        }

        public static void FixHoverboard()
        {
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMax = 10f;
            GorillaLocomotion.GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.1f;
            GorillaLocomotion.GTPlayer.Instance.hoverboardBoostGracePeriod = 1f;
            GorillaLocomotion.GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.2f;
        }

        private static bool hasGrabbedHoverboard;
        public static void GlobalHoverboard()
        {
            if (!hasGrabbedHoverboard)
            {
                GorillaLocomotion.GTPlayer.Instance.GrabPersonalHoverboard(false, Vector3.zero, Quaternion.identity, Color.black);
                hasGrabbedHoverboard = true;
            }

            GorillaLocomotion.GTPlayer.Instance.SetHoverAllowed(true);
            GorillaLocomotion.GTPlayer.Instance.SetHoverActive(true);
            GorillaTagger.Instance.offlineVRRig.hoverboardVisual.gameObject.SetActive(true);
        }

        public static void DisableGlobalHoverboard()
        {
            hasGrabbedHoverboard = false;

            GorillaLocomotion.GTPlayer.Instance.SetHoverAllowed(false);
            GorillaLocomotion.GTPlayer.Instance.SetHoverActive(false);
            GorillaTagger.Instance.offlineVRRig.hoverboardVisual.gameObject.SetActive(false);
        }

        public static void RainbowHoverboard()
        {
            if (GorillaTagger.Instance.offlineVRRig.hoverboardVisual != null && GorillaTagger.Instance.offlineVRRig.hoverboardVisual.IsHeld)
            {
                float h = (Time.frameCount / 180f) % 1f;
                Color rgbColor = Color.HSVToRGB(h, 1f, 1f);
                GorillaTagger.Instance.offlineVRRig.hoverboardVisual.SetIsHeld(GorillaTagger.Instance.offlineVRRig.hoverboardVisual.IsLeftHanded, GorillaTagger.Instance.offlineVRRig.hoverboardVisual.NominalLocalPosition, GorillaTagger.Instance.offlineVRRig.hoverboardVisual.NominalLocalRotation, rgbColor);
            }
        }

        private static float hoverboardSpamDelay;
        public static void HoverboardSpam()
        {
            if (rightGrab && Time.time > hoverboardSpamDelay)
            {
                hoverboardSpamDelay = Time.time + 0.5f;

                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, GorillaTagger.Instance.rightHandTransform.forward * 15f, new Vector3(100f, 100f, 100f), new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255));
            }
        }

        public static void OrbitHoverboards()
        {
            if (Time.time > hoverboardSpamDelay)
            {
                hoverboardSpamDelay = Time.time + 0.2f;

                float offset = 0f;
                Vector3 position = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                offset = -25f;
                Vector3 position2 = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.headCollider.transform.position + position, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - position).normalized), (position2 - position).normalized * 6.5f, new Vector3(0f, 360f, 0f), new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255));

                offset = 180f;
                position = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                offset = 155f;
                position2 = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.headCollider.transform.position + position, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - position).normalized), (position2 - position).normalized * 6.5f, new Vector3(0f, 360f, 0f), new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255));
            }
        }

        public static void StartAllRaces()
        {
            foreach (RacingManager.Race race in RacingManager.instance.races)
            {
                if (race.racingState == RacingManager.RacingState.Inactive)
                    race.Button_StartRace(5);
            }
        }

        private static bool flashColor;
        private static float flashDelay;
        public static void StrobeHoverboard()
        {
            if (GorillaTagger.Instance.offlineVRRig.hoverboardVisual != null && GorillaTagger.Instance.offlineVRRig.hoverboardVisual.IsHeld)
            {
                if (Time.time > flashDelay)
                {
                    flashDelay = Time.time + 0.1f;
                    flashColor = !flashColor;
                }

                Color rgbColor = flashColor ? Color.white : Color.black;
                GorillaTagger.Instance.offlineVRRig.hoverboardVisual.SetIsHeld(GorillaTagger.Instance.offlineVRRig.hoverboardVisual.IsLeftHanded, GorillaTagger.Instance.offlineVRRig.hoverboardVisual.NominalLocalPosition, GorillaTagger.Instance.offlineVRRig.hoverboardVisual.NominalLocalRotation, rgbColor);
            }
        }

        public static void FastGliders()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                glider.pullUpLiftBonus = 0.5f;
                glider.dragVsSpeedDragFactor = 0.5f;
            }
        }

        public static void SlowGliders()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                glider.pullUpLiftBonus = 0.05f;
                glider.dragVsSpeedDragFactor = 0.05f;
            }
        }

        public static void FixGliderSpeed()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                glider.pullUpLiftBonus = 0.1f;
                glider.dragVsSpeedDragFactor = 0.2f;
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
                foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
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
                RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0);
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
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
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
                ClearType<BuilderPiece>();
                int count = 0;
                foreach (BuilderPiece piece in GetAllType<BuilderPiece>())
                {
                    if (count > 400)
                        break;
                    if (piece.gameObject.activeSelf)
                    {
                        count++;
                        GetBuilderTable().RequestRecyclePiece(piece, true, 2);
                    }
                }
            }
        }

        private static bool isFiring = false;
        public static IEnumerator FireShotgun()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                yield break;
            }

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

            GetBuilderTable().RequestGrabPiece(bullet, true, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestDropPiece(bullet, TrueRightHand().position + TrueRightHand().forward * 0.65f + TrueRightHand().right * 0.03f + TrueRightHand().up * 0.05f, TrueRightHand().rotation, TrueRightHand().forward * 19.9f, Vector3.zero);
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
                    BuilderPiece gunTarget = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (gunTarget)
                    {
                        GetBuilderTable().RequestRecyclePiece(gunTarget, true, 2);
                        RPCProtection();
                    }
                }
            }
            //RPCProtection();
        }

        public static void RequestCreatePiece(int pieceType, Vector3 position, Quaternion rotation, int materialType)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int pieceId = GetBuilderTable().CreatePieceId();

                GetBuilderTable().builderNetworking.photonView.RPC("PieceCreatedByShelfRPC", RpcTarget.All, new object[]
                {
                    pieceType,
                    pieceId,
                    BitPackUtils.PackWorldPosForNetwork(position),
                    BitPackUtils.PackQuaternionForNetwork(rotation),
                    materialType,
                    (byte)4,
                    1,
                    PhotonNetwork.LocalPlayer
                });
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
            }
        }

        public static void BuildingBlockAura()
        {
            RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.offlineVRRig.transform.position + Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))) * 2f, Quaternion.identity, 0);
            RPCProtection();
        }

        public static void RainBuildingBlocks()
        {
            RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 4f, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
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
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
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
            GliderHoldable[] them = GetAllType<GliderHoldable>();
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
            RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 0f, MathF.Sin((float)Time.frameCount / 30)), Quaternion.identity, 0);
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

            RequestCreatePiece(pieceType, GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
            RPCProtection();

            while (pieceId < 0)
            {
                yield return null;
            }
            yield return null;

            target = GetBuilderTable().GetPiece(pieceId);
            pieceId = -1;
            Patches.CreatePatch.enabled = false;
            Patches.CreatePatch.pieceTypeSearch = 0;

            onComplete?.Invoke(target); // so bad
        }

        public static IEnumerator CreateShotgun()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                yield break;
            }

            BuilderPiece basea = null;

            yield return CreateGetPiece(-1927069002, piece =>
            {
                basea = piece;
            });
            while (basea == null)
            {
                yield return null;
            }

            GetBuilderTable().RequestGrabPiece(basea, false, Vector3.zero, Quaternion.identity);
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

            GetBuilderTable().RequestGrabPiece(base2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(base2a, base2a, 0, 0, 0, basea, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(slopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(slopea, slopea, 0, 0, 2, base2a, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(trigger, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(trigger, trigger, -1, -2, 3, slopea, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(slopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(basea, trigger, 0, -2, 3, slopeb, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(base2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(slopeb, slopeb, 0, 0, 2, base2b, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(baseb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(base2b, base2b, 0, 0, 0, baseb, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(minislopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(baseb, slopea, 0, -3, 2, minislopeb, 2, 0);
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

            GetBuilderTable().RequestGrabPiece(minislopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(minislopeb, slopeb, 0, -3, 2, minislopea, 2, 0);
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

            GetBuilderTable().RequestGrabPiece(minislope2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(minislopea, minislopeb, 0, 0, 2, minislope2a, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(minislope2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(minislope2a, minislopea, 0, 0, 2, minislope2b, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(flatthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(minislope2b, minislope2b, 0, -1, 2, flatthinga, 2, 0);
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

            GetBuilderTable().RequestGrabPiece(flatthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(flatthinga, minislope2a, 0, -1, 2, flatthingb, 2, 0);
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

            GetBuilderTable().RequestGrabPiece(connectorthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(flatthingb, flatthinga, -1, 1, 3, connectorthinga, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(connectorthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(connectorthinga, connectorthinga, -1, 0, 1, connectorthingb, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(connectorthingc, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(connectorthingb, connectorthinga, 0, 0, 1, connectorthingc, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(barrela, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(connectorthingc, connectorthingb, 0, 0, 1, barrela, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(barrelb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(barrela, barrela, 0, 0, 2, barrelb, 1, 0);
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

            GetBuilderTable().RequestGrabPiece(scope, false, Vector3.zero, Quaternion.identity);
            yield return null;
            GetBuilderTable().RequestPlacePiece(barrelb, minislope2a, -2, 1, 3, scope, 1, 0);
            yield return null;
            GetBuilderTable().RequestDropPiece(scope, GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, Vector3.zero);
            yield return null;
            // pos is forward/back, left/right, up/down
            GetBuilderTable().RequestGrabPiece(basea, false, new Vector3(-0.2f, 0.01f, -0.3f), new Quaternion(0f, 0.1f, 0.75f, -0.6f));
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

            GetBuilderTable().RequestGrabPiece(stupid, false, Vector3.zero, Quaternion.identity);
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
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
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
                    foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
                    {
                        if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetAllType<MonkeyeAI>();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!PhotonNetwork.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                float offset = (360f / (float)them.Length) * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
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
                foreach (BuilderPiece piece in GetAllType<BuilderPiece>())
                {
                    if (Vector3.Distance(piece.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 2.5f)
                    {
                        if (!potentialgrabbedpieces.Contains(piece))
                        {
                            amnt++;
                            if (amnt < 8)
                            {
                                GetBuilderTable().RequestGrabPiece(piece, false, new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)), Quaternion.identity);
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
                    GetBuilderTable().RequestDropPiece(piece, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, new Vector3(UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f)), new Vector3(UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f), UnityEngine.Random.Range(-19f, 19f)));
                
                potentialgrabbedpieces.Clear();
                RPCProtection();
            }
        }

        public static void PopAllBalloons()
        {
            foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
                balloon.OwnerPopBalloon();
        }

        public static void GrabBalloons()
        {
            if (rightGrab)
            {
                foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
                {
                    if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
                        balloon.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    else
                        balloon.WorldShareableRequestOwnership();
                }
            }
        }

        public static void SpazBalloons()
        {
            foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
            {
                if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
                    balloon.gameObject.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                else
                    balloon.WorldShareableRequestOwnership();
            }
        }

        public static void OrbitBalloons()
        {
            BalloonHoldable[] them = GetAllType<BalloonHoldable>();
            int index = 0;
            foreach (BalloonHoldable balloon in them)
            {
                if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
                {
                    float offset = (360f / them.Length) * index;
                    balloon.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 5f, 2, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 5f);
                }
                else
                    balloon.WorldShareableRequestOwnership();
                
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
                    foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
                    {
                        if (balloon.ownerRig == GorillaTagger.Instance.offlineVRRig)
                            balloon.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                        else
                            balloon.WorldShareableRequestOwnership();
                    }
                }
            }
        }

        public static void DestroyBalloons()
        {
            foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
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
                foreach (BalloonHoldable Balloon in GetAllType<BalloonHoldable>())
                {
                    if (Balloon.ownerRig == GorillaTagger.Instance.offlineVRRig && Balloon.gameObject.name.Contains("LMAMI"))
                    {
                        FoundBalloon = true;

                        Balloon.maxDistanceFromOwner = float.MaxValue;
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

                    ClearType<BalloonHoldable>();
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
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
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
                ChangeColor(Color.HSVToRGB(h, 1f, 1f));
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
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        ChangeName(GetPlayerFromVRRig(gunTarget).NickName);
                        ChangeColor(gunTarget.playerColor);
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
                foreach (TransferrableObject cosmet in GetAllType<TransferrableObject>())
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
                foreach (TransferrableObject cosmet in GetAllType<TransferrableObject>())
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
            currentCategoryName = "Temporary Category";

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
            currentCategoryName = "Fun Mods";
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

        private static float lastTimeCosmeticsChecked;
        public static void AutoPurchaseCosmetics()
        {
            if (!GorillaComputer.instance.isConnectedToMaster)
                lastTimeCosmeticsChecked = Time.time + 60f;

            if (Time.time > lastTimeCosmeticsChecked)
            {
                lastTimeCosmeticsChecked = Time.time + 60f;
                foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
                {
                    if (hat.cost == 0 && hat.canTryOn && !GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains(hat.itemName))
                    {
                        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
                        {
                            ItemId = hat.itemName,
                            Price = hat.cost,
                            VirtualCurrency = CosmeticsController.instance.currencyName,
                            CatalogVersion = CosmeticsController.instance.catalog
                        }, delegate (PurchaseItemResult result)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Item \"" + ToTitleCase(hat.overrideDisplayName) + "\" has been purchased.");
                            CosmeticsController.instance.ProcessExternalUnlock(hat.itemName, false, false);
                        }, null, null, null);
                    }
                }
            }
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
