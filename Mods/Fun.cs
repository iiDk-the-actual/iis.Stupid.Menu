using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
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
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Fun
    {
        public static void FixHead()
        {
            VRRig.LocalRig.head.trackingRotationOffset.x = 0f;
            VRRig.LocalRig.head.trackingRotationOffset.y = 0f;
            VRRig.LocalRig.head.trackingRotationOffset.z = 0f;
        }

        public static void UpsideDownHead() =>
            VRRig.LocalRig.head.trackingRotationOffset.z = 180f;

        public static void BrokenNeck() =>
            VRRig.LocalRig.head.trackingRotationOffset.z = 90f;

        public static void BackwardsHead() =>
            VRRig.LocalRig.head.trackingRotationOffset.y = 180f;

        public static void SidewaysHead() =>
            VRRig.LocalRig.head.trackingRotationOffset.y = 90f;

        public static void RandomYHeadSpaz() =>
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = UnityEngine.Random.Range(20, 340);
        
        public static void RandomYHead() =>
            VRRig.LocalRig.head.trackingRotationOffset.y = UnityEngine.Random.Range(20, 340);

        public static int BPM = 159;
        public static void HeadBang()
        {
            if (Time.time > lastBangTime)
            {
                VRRig.LocalRig.head.trackingRotationOffset.x = 50f;
                lastBangTime = Time.time + (60f/(float)BPM);
            } 
            else
                VRRig.LocalRig.head.trackingRotationOffset.x = Mathf.Lerp(VRRig.LocalRig.head.trackingRotationOffset.x,0f,0.1f);
        }

        public static void SpinHeadX()
        {
            if (VRRig.LocalRig.enabled)
                VRRig.LocalRig.head.trackingRotationOffset.x += 10f;
            else
                VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(10f, 0f, 0f));
        }

        public static void SpinHeadY()
        {
            if (VRRig.LocalRig.enabled)
                VRRig.LocalRig.head.trackingRotationOffset.y += 10f;
            else
                VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
        }

        public static void SpinHeadZ()
        {
            if (VRRig.LocalRig.enabled)
                VRRig.LocalRig.head.trackingRotationOffset.z += 10f;
            else
                VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 0f, 10f));
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
            Patches.HandTapPatch.enabled = false;
            Patches.HandTapPatch.tapsEnabled = true;
            Patches.HandTapPatch.doOverride = false;
            Patches.HandTapPatch.overrideVolume = 0.1f;
            Patches.HandTapPatch.tapMultiplier = 1;
            GorillaTagger.Instance.handTapVolume = 0.1f;
        }

        public static void LoudHandTaps()
        {
            Patches.HandTapPatch.enabled = true;
            Patches.HandTapPatch.tapsEnabled = true;
            Patches.HandTapPatch.doOverride = true;
            Patches.HandTapPatch.overrideVolume = 99999f;
            Patches.HandTapPatch.tapMultiplier = 10;
            GorillaTagger.Instance.handTapVolume = 99999f;
        }

        public static void SilentHandTaps()
        {
            Patches.HandTapPatch.enabled = true;
            Patches.HandTapPatch.tapsEnabled = false;
            Patches.HandTapPatch.doOverride = false;
            Patches.HandTapPatch.overrideVolume = 0f;
            Patches.HandTapPatch.tapMultiplier = 0;
            GorillaTagger.Instance.handTapVolume = 0;
        }

        public static void SilentHandTapsOnTag()
        {
            if (PlayerIsTagged(VRRig.LocalRig))
                SilentHandTaps();
            else
                FixHandTaps();
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
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not in a party!</color>");
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
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not in a party!</color>");
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
                partyKickDelayCoroutine ??= CoroutineManager.RunCoroutine(PartyKickDelay(false));
            
            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static void AutoPartyBan()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && !lastPartyKickThingy)
                partyKickDelayCoroutine ??= CoroutineManager.RunCoroutine(PartyKickDelay(true));

            lastPartyKickThingy = FriendshipGroupDetection.Instance.IsInParty;
        }

        public static float splashDel;
        public static void WaterSplashHands()
        {
            if (Time.time > splashDel && (rightGrab || leftGrab))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                {
                    rightGrab ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position,
                    rightGrab ? GorillaTagger.Instance.rightHandTransform.rotation : GorillaTagger.Instance.leftHandTransform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RPCProtection();
                splashDel = Time.time + 0.1f;
            }
        }

        public static void WaterSplashAura()
        {
            if (Time.time > splashDel)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                {
                    VRRig.LocalRig.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f),UnityEngine.Random.Range(-0.5f, 0.5f),UnityEngine.Random.Range(-0.5f, 0.5f)),
                    RandomQuaternion(),
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
                    RandomQuaternion(),
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
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position - new Vector3(0, 1, 0);
                    if (Time.time > splashDel)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                        {
                            NewPointer.transform.position,
                            RandomQuaternion(),
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
                    VRRig.LocalRig.enabled = true;
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
                if (vrrig != VRRig.LocalRig)
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
                    VRRig.LocalRig.PlayHandTapLocal(sound, true, 999999f);
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
                    VRRig.LocalRig.PlayHandTapLocal(sound, false, 999999f);
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

                VRRig.LocalRig.leftHand.calcT = autoclickstate ? 1f : 0f;
                VRRig.LocalRig.leftHand.MapMyFinger(1f);
            }
            if (rightTrigger > 0.5f)
            {
                ControllerInputPoller.instance.rightControllerIndexFloat = autoclickstate ? 1f : 0f;

                VRRig.LocalRig.rightHand.calcT = autoclickstate ? 1f : 0f;
                VRRig.LocalRig.rightHand.MapMyFinger(1f);
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

        public static void SetDebugEchoMode(bool value)
        {
            if (!GorillaTagger.Instance.myRecorder.DebugEchoMode)
                GorillaTagger.Instance.myRecorder.DebugEchoMode = value;
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

        private static float tapDelay = 0f;
        public static void TapAllClass<T>() where T : Tappable
        {
            if (rightGrab)
            {
                if (Time.time > tapDelay)
                {
                    foreach (Tappable TappableObject in GetAllType<T>())
                        TappableObject.OnTap(1f);

                    RPCProtection();
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
        public static void AutoHitMoleType(bool isHazard)
        {   
            foreach (Mole mole in GetAllType<Mole>())
            {
                int state = mole.randomMolePickedIndex;
                if (mole.CanTap() && mole.moleTypes[state].isHazard == isHazard && Time.time > hitDelay)
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

        public static void SetBraceletState(bool enable, bool isLeftHand) => 
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                enable,
                isLeftHand
            });

        public static void GetBracelet(bool state)
        {
            if (leftGrab)
            {
                SetBraceletState(false, false);
                SetBraceletState(state, true);
            }

            if (rightGrab)
            {
                SetBraceletState(state, false);
                SetBraceletState(false, true);
            }

            if (leftGrab || rightGrab)
                RPCProtection();
        }

        public static void BraceletSpam() => GetBracelet(Time.frameCount % 2 == 0);

        public static void RemoveBracelet()
        {
            SetBraceletState(false, true);
            SetBraceletState(false, false);
            RPCProtection();
        }

        public static void RainbowBracelet()
        {
            Patches.BraceletPatch.enabled = true;
            if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
            {
                SetBraceletState(true, false);
                RPCProtection();

                VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(true);
            }
            List<Color> rgbColors = new List<Color> { };
            for (int i=0; i<10; i++)
                rgbColors.Add(Color.HSVToRGB(((Time.frameCount / 180f) + (i / 10f)) % 1f, 1f, 1f));
            
            VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
            VRRig.LocalRig.reliableState.braceletSelfIndex = 99;
            VRRig.LocalRig.reliableState.braceletBeadColors = rgbColors;
            VRRig.LocalRig.friendshipBraceletRightHand.UpdateBeads(rgbColors, 99);
        }

        public static void RemoveRainbowBracelet()
        {
            Patches.BraceletPatch.enabled = false;
            if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
            {
                SetBraceletState(false, false);
                RPCProtection();

                VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(false);
            }

            VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
            VRRig.LocalRig.reliableState.braceletSelfIndex = 0;
            VRRig.LocalRig.reliableState.braceletBeadColors.Clear();
            VRRig.LocalRig.UpdateFriendshipBracelet();
        }

        public static void GiveBuilderWatch()
        {
            VRRig.LocalRig.EnableBuilderResizeWatch(true);
            RPCProtection();
        }

        public static void RemoveBuilderWatch()
        {
            VRRig.LocalRig.EnableBuilderResizeWatch(false);
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
                VRRig.LocalRig.SetQuestScore(int.MaxValue);
            }
        }

        public static void FakeFPS()
        {
            Patches.FPSPatch.enabled = true;
            Patches.FPSPatch.spoofFPSValue = UnityEngine.Random.Range(0, 255);
        }

        public static void EverythingGrabbable()
        {
            GamePlayerLocal.instance.gamePlayer.DisableGrabbing(false);
            foreach (GameEntity entity in GhostReactorManager.instance.gameEntityManager.entities)
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

        public static void GrabIDCard()
        {
            if (rightGrab)
            {
                GamePlayer plr = GamePlayerLocal.instance.gamePlayer;

                if (plr.GetGameEntityId(GamePlayer.GetHandIndex(false)) == null)
                {
                    foreach (GRBadge grBadge in GameObject.Find("GhostReactorRoot/GhostReactorZone/GhostReactorEmployeeBadges").GetComponent<GRUIStationEmployeeBadges>().registeredBadges)
                    {
                        GameEntity entity = grBadge.gameEntity;
                        if (entity.onlyGrabActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            VRRig.LocalRig.enabled = false;
                            VRRig.LocalRig.transform.position = entity.transform.position;

                            GhostReactorManager.instance.gameEntityManager.RequestGrabEntity(entity.id, false, Vector3.zero, Quaternion.identity);
                        }
                    }
                }
            }
            else
            {
                VRRig.LocalRig.enabled = true;
            }
        }

        private static float crateDelay;
        public static void BreakAllCrates()
        {
            if (rightTrigger > 0.5f)
            {
                GamePlayer plr = GamePlayerLocal.instance.gamePlayer;

                if (plr.GetGameEntityId(GamePlayer.GetHandIndex(false)) == null)
                {
                    foreach (GameEntity entity in GhostReactorManager.instance.gameEntityManager.entities)
                    {
                        if (entity.gameObject.name.Contains("BreakableCrate"))
                        {
                            VRRig.LocalRig.enabled = false;
                            VRRig.LocalRig.transform.position = entity.transform.position;

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
                VRRig.LocalRig.enabled = true;
        }

        public static void SetPropDistanceLimit(float distance)
        {
            if (PhotonNetwork.InRoom && GorillaGameManager.instance.GameType() == GameModeType.PropHaunt)
            {
                GorillaPropHauntGameManager hauntManager = (GorillaPropHauntGameManager)GorillaGameManager.instance;
                hauntManager.m_ph_hand_follow_distance = distance;
            }
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

        public static void SetCurrencySelf(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).currency = currency;
        }

        public static void SetCurrencyGun(int currency = 0)
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
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).currency = currency;
                        else
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    }
                }
            }
        }

        public static void SetCurrencyAll(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.currency = currency;
            }
        }

        public static void RemoveCurrencySelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { return; }
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
            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.currency = 0;
            }
        }

        public static void Invincibility()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

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
                    || (NetworkSystem.Instance.IsMasterClient && State == GRPlayer.GRPlayerState.Alive)
                    )
            {
                GhostReactorManager.instance.RequestPlayerStateChange(GRPlayer, State);
                RPCProtection();
                return;
            }

            if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }

            if (State == GRPlayer.GRPlayerState.Ghost)
                CoroutineManager.instance.StartCoroutine(KillTarget(Target));
        }

        public static void SetPlayerState(NetPlayer Target, GRPlayer.GRPlayerState State) =>
            SetPlayerState(NetPlayerToPlayer(Target), State);

        public static void SetPlayerState(VRRig Target, GRPlayer.GRPlayerState State) =>
            SetPlayerState(NetPlayerToPlayer(GetPlayerFromVRRig(Target)), State);

        public static IEnumerator KillTarget(Player Target)
        {
            GRPlayer GRPlayer = GRPlayer.Get(Target.ActorNumber);
            VRRig Rig = GetVRRigFromPlayer(Target);

            int netId = GhostReactorManager.instance.gameEntityManager.CreateNetId();

            GhostReactorManager.instance.gameEntityManager.photonView.RPC("CreateItemRPC", Target, new object[]
            {
                new int[] { netId },
                new int[] { (int)GTZone.ghostReactor },
                new int[] { 48354877 },
                new long[] { BitPackUtils.PackWorldPosForNetwork(Rig.transform.position) },
                new int[] { BitPackUtils.PackQuaternionForNetwork(Rig.transform.rotation) },
                new long[] { 0L }
            });

            GhostReactorManager.instance.gameAgentManager.photonView.RPC("ApplyBehaviorRPC", Target, new object[]
            {
                new int[] { netId },
                new byte[] { 6 }
            });

            GRPlayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, GhostReactorManager.instance);

            RPCProtection();

            yield return null;
            yield return null;
            yield return null;

            GhostReactorManager.instance.gameEntityManager.photonView.RPC("DestroyItemRPC", Target, new object[]
            {
                new int[] { netId }
            });

            RPCProtection();
        }

        private static float killDelay;
        public static void SetStateSelf(int state) =>
            SetPlayerState(PhotonNetwork.LocalPlayer, (GRPlayer.GRPlayerState)state);

        public static void SetStateAll(int state)
        {
            foreach (Player target in PhotonNetwork.PlayerList)
                SetPlayerState(target, (GRPlayer.GRPlayerState)state);
        }

        public static void SetStateGun(int state)
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
                        SetPlayerState(gunTarget, (GRPlayer.GRPlayerState)state);
                }
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

        public static void SetMicrophoneQuality(int bitrate, int samplingRate)
        {
            Recorder mic = GorillaTagger.Instance.myRecorder;
            mic.SamplingRate = (SamplingRate)samplingRate;
            mic.Bitrate = bitrate;

            mic.RestartRecording(true);
        }

        public static void SetMicrophoneAmplification(bool amplify)
        {
            Recorder mic = GorillaTagger.Instance.myRecorder;

            if (amplify)
            {
                MicAmplifier microphoneAmplifier = mic.gameObject.GetOrAddComponent<MicAmplifier>();
                microphoneAmplifier.AmplificationFactor = 16;
                microphoneAmplifier.BoostValue = 16;
            } else
            {
                if (mic.gameObject.GetComponent<MicAmplifier>())
                    UnityEngine.Object.Destroy(mic.gameObject.GetComponent<MicAmplifier>());
            }

            mic.RestartRecording(true);
        }

        public static void ReloadMicrophone() =>
            GameObject.Find("Photon Manager").GetComponent<Recorder>().RestartRecording(true);

        public static void ObjectToPointGun(string objectName)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GameObject.Find(objectName).transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
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
                        archivepiecesfiltered.Add(piece);
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

        public static void NoRespawnBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = float.MaxValue;
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = float.MaxValue;
        }

        public static void DisableNoRespawnBug()
        {
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = 50f;
            GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = 50f;
        }

        public static void NoRespawnBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = float.MaxValue;
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = float.MaxValue;
        }

        public static void DisableNoRespawnBat()
        {
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = 50f;
            GameObject.Find("Cave Bat Holdable").GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = 50f;
        }

        public static void FastSnowballs()
        {
            foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
            {
                foreach (SnowballThrowable Throwable in Maker.snowballs)
                {
                    Throwable.linSpeedMultiplier = 10f;
                    Throwable.maxLinSpeed = 99999f;
                    Throwable.maxWristSpeed = 99999f;
                }
            }
        }

        public static void SlowSnowballs()
        {
            foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
            {
                foreach (SnowballThrowable Throwable in Maker.snowballs)
                {
                    Throwable.linSpeedMultiplier = 0.2f;
                    Throwable.maxLinSpeed = 6f;
                    Throwable.maxWristSpeed = 2f;
                }
            }
        }

        public static void FixSnowballs()
        {
            foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
            {
                foreach (SnowballThrowable Throwable in Maker.snowballs)
                {
                    Throwable.linSpeedMultiplier = 1f;
                    Throwable.maxLinSpeed = 12f;
                    Throwable.maxWristSpeed = 4f;
                }
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
            VRRig.LocalRig.hoverboardVisual.gameObject.SetActive(true);
        }

        public static void DisableGlobalHoverboard()
        {
            hasGrabbedHoverboard = false;

            GorillaLocomotion.GTPlayer.Instance.SetHoverAllowed(false);
            GorillaLocomotion.GTPlayer.Instance.SetHoverActive(false);
            VRRig.LocalRig.hoverboardVisual.gameObject.SetActive(false);
        }

        public static void SpawnHoverboard()
        {
            FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.offlineVRRig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.rotation, Vector3.zero, Vector3.zero, new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255));
            GorillaLocomotion.GTPlayer.Instance.SetHoverAllowed(true);
        }

        public static void RainbowHoverboard()
        {
            if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
            {
                float h = (Time.frameCount / 180f) % 1f;
                Color rgbColor = Color.HSVToRGB(h, 1f, 1f);
                VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, rgbColor);
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
            if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
            {
                if (Time.time > flashDelay)
                {
                    flashDelay = Time.time + 0.1f;
                    flashColor = !flashColor;
                }

                Color rgbColor = flashColor ? Color.white : Color.black;
                VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, rgbColor);
            }
        }

        public static void ModifyGliderSpeed(float pullUpLiftBonus, float dragVsSpeedDragFactor)
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                glider.pullUpLiftBonus = pullUpLiftBonus;
                glider.dragVsSpeedDragFactor = dragVsSpeedDragFactor;
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

        public static void ObjectToHand(string objectName)
        {
            if (rightGrab)
                GameObject.Find(objectName).transform.position = GorillaTagger.Instance.rightHandTransform.position;
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

        public static void DestroyObject(string objectName) =>
            GameObject.Find(objectName).transform.position = new Vector3(99999f, 99999f, 99999f);

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
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                yield break;
            }

            isFiring = true;

            if (!File.Exists("iisStupidMenu/shotgun.wav"))
                LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/refs/heads/main/shotgun.wav", "shotgun.wav");

            Sound.PlayAudio("shotgun.wav");

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
            if (NetworkSystem.Instance.IsMasterClient)
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
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
        }

        public static void BuildingBlockAura()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + Vector3.Normalize(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))) * 2f, Quaternion.identity, 0);
            RPCProtection();
        }

        public static void RainBuildingBlocks()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 4f, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
            RPCProtection();
        }

        public static void SpazObject(string objectName) =>
             GameObject.Find(objectName).transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

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

        public static void OrbitObject(string objectName, float offset = 0) =>
            GameObject.Find(objectName).transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)), 2, MathF.Sin(offset + ((float)Time.frameCount / 30)));

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

        public static void RideObject(string objectName)
        {
            TeleportPlayer(GameObject.Find(objectName).transform.position);
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        public static void AllowStealingThrowableBug(string objectName, bool allowPlayerStealing) =>
            GameObject.Find(objectName).GetComponent<ThrowableBug>().allowPlayerStealing = allowPlayerStealing;

        public static void MultiGrab()
        {
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
        }

        public static int pieceId = -1;
        public static IEnumerator CreateGetPiece(int pieceType, Action<BuilderPiece> onComplete)
        {
            BuilderPiece target = null;

            Patches.CreatePatch.enabled = true;
            Patches.CreatePatch.pieceTypeSearch = pieceType;

            yield return null;

            RequestCreatePiece(pieceType, VRRig.LocalRig.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
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
            if (!NetworkSystem.Instance.IsMasterClient)
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

        private static bool lastgripcrap;
        private static bool lasttrigcrap;
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
            VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
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

            VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
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
                VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;

            if (rightGrab)
                VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
        }

        public static void SlowMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
                {
                    if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
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
                        if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetAllType<MonkeyeAI>();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                float offset = (360f / (float)them.Length) * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; } // GetOwnership(monkeyeAI.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
            }
        }

        public static List<BuilderPiece> GetBlocks(string blockname)
        {
            List<BuilderPiece> blocks = new List<BuilderPiece> { };

            foreach (BuilderPiece filteredBlock in GetPiecesFiltered())
            {
                if (filteredBlock.name.ToLower().Contains(blockname))
                    blocks.Add(filteredBlock);
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
                    if (balloon.ownerRig == VRRig.LocalRig)
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
                if (balloon.ownerRig == VRRig.LocalRig)
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
                if (balloon.ownerRig == VRRig.LocalRig)
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
                        if (balloon.ownerRig == VRRig.LocalRig)
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
                if (balloon.ownerRig == VRRig.LocalRig)
                    balloon.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                else
                    balloon.WorldShareableRequestOwnership();
            }
        }

        // I would like to apologize to anyone who had this beforehand
        // "Tubski" will be long loved forever because that was the original name of this mod I don't know what that means
        public static void BecomeBalloon()
        {
            if (rightTrigger > 0.5f)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.inTryOnRoom = true;
                VRRig.LocalRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                bool FoundBalloon = false;
                foreach (BalloonHoldable Balloon in GetAllType<BalloonHoldable>())
                {
                    if (Balloon.ownerRig == VRRig.LocalRig && Balloon.gameObject.name.Contains("LMAMI"))
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
                    CosmeticsController.instance.ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMAAP."), true, false);
                    CosmeticsController.instance.UpdateWornCosmetics(true);
                    RPCProtection();

                    ClearType<BalloonHoldable>();
                }
            }
            else
            {
                if (!VRRig.LocalRig.enabled)
                    VRRig.LocalRig.enabled = true;
            }
        }

        public static void DestroyGliders()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                    glider.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                else
                    glider.OnHover(null, null);
            }
        }

        public static float nameCycleDelay;
        public static int nameCycleIndex;
        public static void NameCycle(string[] names)
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > names.Length - 1)
                    nameCycleIndex = 0;

                ChangeName(names[nameCycleIndex]);
                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void RandomNameCycle() =>
            NameCycle(new string[] { GenerateRandomString(8) });

        public static string[] names = new string[] { };
        public static void EnableCustomNameCycle()
        {
            if (File.Exists("iisStupidMenu/iiMenu_CustomNameCycle.txt"))
                names = File.ReadAllText("iisStupidMenu/iiMenu_CustomNameCycle.txt").Split('\n');
            else
                File.WriteAllText("iisStupidMenu/iiMenu_CustomNameCycle.txt","YOUR\nTEXT\nHERE");
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
                    colorChangeType = 0;
                
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

        public static void BecomePlayer(string name, Color color)
        {
            ChangeName(name);
            ChangeColor(color);
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

            BecomePlayer(names[UnityEngine.Random.Range(0, names.Length - 1)], colors[UnityEngine.Random.Range(0, colors.Length - 1)]);
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

        public static int accessoryType;
        public static int hat;
        public static void ChangeAccessories()
        {
            if (leftGrab && !lastHitL)
            {
                hat--;
                if (hat < 1)
                    hat = 3;

                switch (hat)
                {
                    case 1:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton")
                            .GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)")
                            .GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)")
                            .GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }
            }
            lastHitL = leftGrab;
            if (rightGrab && !lastHitR)
            {
                hat++;
                if (hat > 3)
                    hat = 1;

                switch (hat)
                {
                    case 1:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }

            }
            lastHitR = rightGrab;
            if (leftPrimary && !lastHitLP)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeLeftButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                switch (hat)
                {
                    case 1:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }
            }
            lastHitLP = leftPrimary;

            if (rightPrimary && !lastHitRP)
            {
                GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeRightItem").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                switch (hat)
                {
                    case 1:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }
            }
            lastHitRP = rightPrimary;

            if (rightSecondary && !lastHitRS)
            {
                accessoryType++;
                if (accessoryType > 4)
                    accessoryType = 1;
                
                switch (accessoryType)
                {
                    case 1:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardobeHatButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 2:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeFaceButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 3:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeBadgeButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 4:
                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeHoldableButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
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
                    if (VRRig.LocalRig.concatStringOfCosmeticsAllowed.Contains(dearlord.itemName))
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
                        tryonarchive.Add(dearlord.itemName);
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
                string[] owned = VRRig.LocalRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> holyshit = new List<string> { };
                    for (int i = 0; i <= amnt; i++)
                        holyshit.Add(owned[UnityEngine.Random.Range(0, owned.Length - 1)]);
                    
                    if (VRRig.LocalRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
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
                string[] owned = VRRig.LocalRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> holyshit = new List<string> { };
                    for (int i = 0; i <= amnt; i++)
                        holyshit.Add(owned[UnityEngine.Random.Range(0, owned.Length - 1)]);
                    if (VRRig.LocalRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.tryOnSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(holyshit.ToArray(), CosmeticsController.instance);
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
                            cosmet.currentState = TransferrableObject.PositionState.InLeftHand;
                        if (cosmet.currentState == TransferrableObject.PositionState.OnRightArm || cosmet.currentState == TransferrableObject.PositionState.OnRightShoulder || cosmet.currentState == TransferrableObject.PositionState.OnChest)
                            cosmet.currentState = TransferrableObject.PositionState.InRightHand;
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
                            cosmet.currentState = TransferrableObject.PositionState.OnLeftArm;
                    }
                }
            }
        }

        private static int[] archiveCosmetics = null;
        public static void TryOnAnywhere()
        {
            archiveCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(Array.Empty<string>(), CosmeticsController.instance);
            VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(Array.Empty<string>(), CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, new object[] { PackCosmetics(Array.Empty<string>()), CosmeticsController.instance.tryOnSet.ToPackedIDArray() });
            RPCProtection();
        }

        public static void TryOffAnywhere()
        {
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
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
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                if (hat.canTryOn)
                    cosmeticbuttons.Add(new ButtonInfo { buttonText = ToTitleCase(hat.overrideDisplayName), method = () => AddCosmeticToCart(hat.itemName), isTogglable = false, toolTip = "Adds the " + hat.overrideDisplayName.ToLower() + "to your cart." });
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
            Patches.RequestPatch.currentCoroutine ??= CoroutineManager.RunCoroutine(Patches.RequestPatch.LoadCosmetics());
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
                    if (hat.cost == 0 && hat.canTryOn && !VRRig.LocalRig.concatStringOfCosmeticsAllowed.Contains(hat.itemName))
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
            if (!lasttagged && PlayerIsTagged(VRRig.LocalRig))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.Others, new object[] { new string[] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" }, new string[] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" } });
                RPCProtection();
            }
            if (lasttagged && !PlayerIsTagged(VRRig.LocalRig))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.Others, new object[] { CosmeticsController.instance.currentWornSet.ToDisplayNameArray(), CosmeticsController.instance.tryOnSet.ToDisplayNameArray() });
                RPCProtection();
            }
            lasttagged = PlayerIsTagged(VRRig.LocalRig);
        }
    }
}
