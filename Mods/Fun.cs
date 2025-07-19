using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using PlayFab;
using PlayFab.ClientModels;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static Mono.Security.X509.X520;

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
 

        public static float lastBangTime;
        public static float BPM = 159f;
        public static void HeadBang()
        {
            if (Time.time > lastBangTime)
            {
                VRRig.LocalRig.head.trackingRotationOffset.x = 50f;
                lastBangTime = Time.time + (60f/BPM);
            } 
            else
                VRRig.LocalRig.head.trackingRotationOffset.x = Mathf.Lerp(VRRig.LocalRig.head.trackingRotationOffset.x, 0f, 0.1f);
        }

        public static void SpinHead(string axis)
        {
            if (VRRig.LocalRig.enabled)
            {
                switch (axis.ToLower())
                {
                    case "x":
                        VRRig.LocalRig.head.trackingRotationOffset.x += 10f;
                        break;
                    case "y":
                        VRRig.LocalRig.head.trackingRotationOffset.y += 10f;
                        break;
                    case "z":
                        VRRig.LocalRig.head.trackingRotationOffset.z += 10f;
                        break;
                    default:
                        return;
                }
            }
            else
                VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
        }

        public static void SpazHead(string axis)
        {
            int offset = UnityEngine.Random.Range(0, 360);
            switch (axis.ToLower())
            {
                case "x":
                    VRRig.LocalRig.head.trackingRotationOffset.x = offset;
                    break;
                case "y":
                    VRRig.LocalRig.head.trackingRotationOffset.y = offset;
                    break;
                case "z":
                    VRRig.LocalRig.head.trackingRotationOffset.z = offset;
                    break;
                default:
                    return;
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
            Patches.EffectDataPatch.enabled = false;
            Patches.EffectDataPatch.tapsEnabled = true;
            Patches.EffectDataPatch.doOverride = false;
            Patches.EffectDataPatch.overrideVolume = 0.1f;
            Patches.EffectDataPatch.tapMultiplier = 1;
            GorillaTagger.Instance.handTapVolume = 0.1f;
        }

        public static void LoudHandTaps()
        {
            Patches.EffectDataPatch.enabled = true;
            Patches.EffectDataPatch.tapsEnabled = true;
            Patches.EffectDataPatch.doOverride = true;
            Patches.EffectDataPatch.overrideVolume = 99999f;
            Patches.EffectDataPatch.tapMultiplier = 10;
            GorillaTagger.Instance.handTapVolume = 99999f;
        }

        public static void SilentHandTaps()
        {
            Patches.EffectDataPatch.enabled = true;
            Patches.EffectDataPatch.tapsEnabled = false;
            Patches.EffectDataPatch.doOverride = false;
            Patches.EffectDataPatch.overrideVolume = 0f;
            Patches.EffectDataPatch.tapMultiplier = 0;
            GorillaTagger.Instance.handTapVolume = 0;
        }

        public static void SilentHandTapsOnTag()
        {
            if (PlayerIsTagged(VRRig.LocalRig))
                SilentHandTaps();
            else
                FixHandTaps();
        }

        public static void BetaNearbyFollowCommand(GorillaFriendCollider friendCollider, Player player) 
        {
            PhotonNetworkController.Instance.FriendIDList.Add(player.UserId);

            object[] groupJoinSendData = new object[2];
            groupJoinSendData[0] = PhotonNetworkController.Instance.shuffler;
            groupJoinSendData[1] = PhotonNetworkController.Instance.keyStr;
            NetEventOptions netEventOptions = new NetEventOptions { TargetActors = new int[] { player.ActorNumber } };

            if (friendCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) && friendCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
                RoomSystem.SendEvent(4, groupJoinSendData, netEventOptions, false);
            else if (!friendCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in stump.");
        }

        public static IEnumerator StumpKickDelay(Action action, Action action2)
        {
            yield return new WaitForSeconds(0.2f);
            action?.Invoke();
            yield return new WaitForSeconds(0.2f);
            action2?.Invoke();
        }

        public static void StumpKickGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > kgDebounce)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        NetPlayer player = GetPlayerFromVRRig(gunTarget);
                        kgDebounce = Time.time + 0.5f;

                        if (!NetworkSystem.Instance.SessionIsPrivate)
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be in a private room.");
                            return;
                        }

                        CoroutineManager.instance.StartCoroutine(StumpKickDelay(() =>
                        {
                            PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99).ToString().PadLeft(2, '0') + UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                            PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');

                            BetaNearbyFollowCommand(GorillaComputer.instance.friendJoinCollider, NetPlayerToPlayer(player));
                            RPCProtection();
                        }, () =>
                        {
                            GorillaComputer.instance.primaryTriggersByZone.TryGetValue(GorillaComputer.instance.allowedMapsToJoin[0], out GorillaNetworkJoinTrigger trigger);
                            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger, GorillaNetworking.JoinType.JoinWithNearby, null);
                        }));
                    }
                }
            }
        }

        public static void StumpKickAll()
        {
            if (PhotonNetwork.InRoom)
            {
                if (!NetworkSystem.Instance.SessionIsPrivate)
                {
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be in a private room.");
                    return;
                }

                CoroutineManager.instance.StartCoroutine(StumpKickDelay(() =>
                {
                    PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99).ToString().PadLeft(2, '0') + UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                    PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');

                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        BetaNearbyFollowCommand(GorillaComputer.instance.friendJoinCollider, NetPlayerToPlayer(GetPlayerFromVRRig(rig)));
                    RPCProtection();
                }, () =>
                {
                    GorillaComputer.instance.primaryTriggersByZone.TryGetValue(GorillaComputer.instance.allowedMapsToJoin[0], out GorillaNetworkJoinTrigger trigger);
                    PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger, GorillaNetworking.JoinType.JoinWithNearby, null);
                }));
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.");
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
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> Kicking " + amountPartying.ToString() + " party members, please be patient..");
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a party.");
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
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> Banning " + amountPartying.ToString() + " party members, please be patient..");
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a party.");
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

        public static Coroutine waterSplashCoroutine;
        public static IEnumerator EnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        public static void BetaWaterSplash(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, object general = null)
        {
            if (general == null)
                general = RpcTarget.All;

            splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
            boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);

            if ((GorillaTagger.Instance.bodyCollider.transform.position - splashPosition).sqrMagnitude >= 8.5f)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = splashPosition + Vector3.down * 2f;

                if (waterSplashCoroutine != null)
                    CoroutineManager.instance.StopCoroutine(waterSplashCoroutine);

                waterSplashCoroutine = CoroutineManager.instance.StartCoroutine(EnableRig());
            }

            object[] parameters = new object[]
            {
                splashPosition,
                splashRotation,
                splashScale,
                boundingRadius,
                bigSplash,
                enteringWater
            };

            try
            {
                if (general is NetPlayer player)
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", NetPlayerToPlayer(player), parameters);
                else if (general is RpcTarget target)
                {
                    if (target == RpcTarget.All)
                    {
                        ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
                        ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);

                        target = RpcTarget.Others;
                    }

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", target, parameters);
                }
                else if (general is int[] targets)
                {
                    if (targets.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber))
                        ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);

                    Overpowered.SpecialTargetRPC(GorillaTagger.Instance.myVRRig.GetView, "RPC_PlaySplashEffect", targets, parameters);
                }
            } catch { }

            RPCProtection();
        }

        public static float splashDel;
        public static void WaterSplashHands()
        {
            if (Time.time > splashDel && (rightGrab || leftGrab))
            {
                BetaWaterSplash(rightGrab ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position, rightGrab ? GorillaTagger.Instance.rightHandTransform.rotation : GorillaTagger.Instance.leftHandTransform.rotation, 4f, 100f, true, false);
                splashDel = Time.time + 0.1f;
            }
        }

        public static void GiveWaterSplashHandsGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (lockTarget.rightMiddle.calcT > 0.5f || lockTarget.leftMiddle.calcT > 0.5f)
                    {
                        if (Time.time > splashDel)
                        {
                            Vector3 position = lockTarget.rightMiddle.calcT > 0.5f ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position;
                            Quaternion rotation = lockTarget.rightMiddle.calcT > 0.5f ? GorillaTagger.Instance.rightHandTransform.rotation : GorillaTagger.Instance.leftHandTransform.rotation;

                            BetaWaterSplash(position, rotation, 4f, 100f, true, false);
                            splashDel = Time.time + 0.1f;
                        }
                    }
                }
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

        public static void WaterSplashAura()
        {
            if (Time.time > splashDel)
            {
                BetaWaterSplash(VRRig.LocalRig.transform.position + RandomVector3(2f), RandomQuaternion(), 4f, 100f, true, false);
                splashDel = Time.time + 0.1f;
            }
        }

        public static void OrbitWaterSplash()
        {
            if (Time.time > splashDel)
            {
                BetaWaterSplash(GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30), 1f, MathF.Sin((float)Time.frameCount / 30)), RandomQuaternion(), 4f, 100f, true, false);
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

                if (GetGunInput(true) && Time.time > splashDel)
                {
                    splashDel = Time.time + 0.1f;
                    BetaWaterSplash(NewPointer.transform.position, RandomQuaternion(), 4f, 100f, true, false);
                }
            }
        }

        public static void WaterSplashWalk()
        {
            if (Time.time > splashDel)
            {
                if (GTPlayer.Instance.IsHandTouching(true))
                {
                    RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;
                    BetaWaterSplash(GorillaTagger.Instance.leftHandTransform.position, Quaternion.Euler(ray.normal), 4f, 100f, true, false);
                    splashDel = Time.time + 0.1f;
                } else if (GTPlayer.Instance.IsHandTouching(false))
                {
                    RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;
                    BetaWaterSplash(GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(ray.normal), 4f, 100f, true, false);
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
                if (!vrrig.isLocal)
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

        public static void PreloadTagSounds()
        {
            string[] sounds = new string[]
            {
                "firstblood",
                "doublekill",
                "triplekill",
                "killingspree",
                "wickedsick",
                "monsterkill",
                "rampage"
            };

            foreach (string sound in sounds)
                LoadSoundFromURL($"https://github.com/iiDk-the-actual/ModInfo/raw/main/killsounds/{sound}.wav", $"{sound}.wav");
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

        public static void ReportGun()
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
                        NetPlayer player = GetPlayerFromVRRig(gunTarget);

                        GorillaPlayerScoreboardLine.ReportPlayer(player.UserId, GorillaPlayerLineButton.ButtonType.Cheating, player.NickName);
                        muteDelay = Time.time + 0.2f;
                    }
                }
            }
        }

        public static void ReportAll()
        {
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
                GorillaPlayerScoreboardLine.ReportPlayer(player.UserId, GorillaPlayerLineButton.ButtonType.Cheating, player.NickName);
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
                GetObject("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/CosmeticsRoomAnchor/outsidestores_prefab/Bottom Layer/OutsideBuildings/Wardrobe Hut/MonkeBusinessStation").GetComponent<PhotonView>().RPC("BroadcastRedeemQuestPoints", RpcTarget.All, UnityEngine.Random.Range(0, 50));
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
                    foreach (GRBadge grBadge in GetObject("GhostReactorRoot/GhostReactorZone/GhostReactorEmployeeBadges").GetComponent<GRUIStationEmployeeBadges>().registeredBadges)
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
                VRRig.LocalRig.enabled = true;
        }

        public static void SetPropDistanceLimit(float distance)
        {
            if (PhotonNetwork.InRoom && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
            {
                GorillaPropHuntGameManager hauntManager = (GorillaPropHuntGameManager)GorillaGameManager.instance;
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

        public class MicPitchShifter : VoiceComponent
        {
            public float PitchFactor = 1.5f;
            public PitchProcessor floatProcessor;

            public void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
            {
                if (p.Voice is LocalVoiceAudioFloat floatVoice)
                {
                    floatProcessor = new PitchProcessor(PitchFactor);
                    floatVoice.AddPostProcessor(new IProcessor<float>[] { floatProcessor });
                }
            }

            public class PitchProcessor : IProcessor<float>
            {
                private float pitch;

                public PitchProcessor(float pitchFactor) =>
                    pitch = Mathf.Clamp(pitchFactor, 0.5f, 2f);

                public float[] Process(float[] buf)
                {
                    int inputLength = buf.Length;
                    float[] output = new float[inputLength];

                    float sampleIndex = 0f;
                    for (int i = 0; i < inputLength; i++)
                    {
                        int indexFloor = Mathf.FloorToInt(sampleIndex);
                        int indexCeil = Mathf.Min(indexFloor + 1, inputLength - 1);
                        float t = sampleIndex - indexFloor;

                        float interpolated = Mathf.Lerp(buf[indexFloor], buf[indexCeil], t);
                        output[i] = interpolated;

                        sampleIndex += pitch;
                        if (sampleIndex >= inputLength - 1)
                            break;
                    }

                    return output;
                }

                public void Dispose() { }
            }
        }

        public class LoopbackFactory : IAudioReader<float>
        {
            private Queue<float> buffer = new Queue<float>();

            public void Feed(float[] data)
            {
                foreach (var sample in data)
                    buffer.Enqueue(sample);
            }

            public bool Read(float[] bufferOut)
            {
                if (buffer.Count < bufferOut.Length)
                    return false;

                for (int i = 0; i < bufferOut.Length; i++)
                    bufferOut[i] = buffer.Dequeue();

                return true;
            }

            public int SamplingRate => 16000;
            public int Channels => 1;
            public string Error => null;

            public void Dispose() =>
                buffer.Clear();
        }

        public static void SetMicrophoneQuality(int bitrate, int samplingRate)
        {
            if (!PhotonNetwork.InRoom)
                return;

            Recorder mic = GorillaTagger.Instance.myRecorder;

            if (mic.SamplingRate == (SamplingRate)samplingRate && mic.Bitrate == bitrate)
                return;

            mic.SamplingRate = (SamplingRate)samplingRate;
            mic.Bitrate = bitrate;

            CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
        }

        public static void SetMicrophoneAmplification(bool amplify)
        {
            if (!PhotonNetwork.InRoom)
                return;

            Recorder mic = GorillaTagger.Instance.myRecorder;

            if (amplify)
            {
                if (mic.gameObject.GetComponent<MicAmplifier>() != null)
                    return;

                MicAmplifier microphoneAmplifier = mic.gameObject.GetOrAddComponent<MicAmplifier>();
                microphoneAmplifier.AmplificationFactor = 16;
                microphoneAmplifier.BoostValue = 16;
            } else
            {
                if (mic.gameObject.GetComponent<MicAmplifier>())
                {
                    if (mic.gameObject.GetComponent<MicAmplifier>() == null)
                        return;

                    MicAmplifier microphoneAmplifier = mic.gameObject.GetComponent<MicAmplifier>();
                    microphoneAmplifier.enabled = false;
                    UnityEngine.Object.Destroy(mic.gameObject.GetComponent<MicAmplifier>());
                }
            }

            CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
        }

        public static void MuteMicrophone(bool mute)
        {
            if (!PhotonNetwork.InRoom)
                return;
            Recorder mic = GorillaTagger.Instance.myRecorder;
            if (mic.IsRecording != mute)
                return;
            mic.IsRecording = !mute;
        }

        public static void SetMicrophonePitch(float pitch)
        {
            if (!PhotonNetwork.InRoom)
                return;

            Recorder mic = GorillaTagger.Instance.myRecorder;

            if (pitch != 1f)
            {
                MicPitchShifter pitchShifter = mic.gameObject.GetComponent<MicPitchShifter>() ?? null;
                if (pitchShifter != null && pitchShifter.PitchFactor == pitch)
                    return;

                MicPitchShifter microphoneAmplifier = mic.gameObject.GetOrAddComponent<MicPitchShifter>();
                microphoneAmplifier.PitchFactor = pitch;
            }
            else
            {
                if (mic.gameObject.GetComponent<MicPitchShifter>())
                {
                    MicPitchShifter microphoneAmplifier = mic.gameObject.GetComponent<MicPitchShifter>();
                    microphoneAmplifier.enabled = false;
                    UnityEngine.Object.Destroy(mic.gameObject.GetComponent<MicPitchShifter>());
                }
                else
                    return;
            }

            CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
        }

        public static void SetDebugEchoMode(bool value)
        {
            if (GorillaTagger.Instance.myRecorder != null)
                GorillaTagger.Instance.myRecorder.DebugEchoMode = value;
        }

        private static LoopbackFactory factory;
        private static float copyVoiceGunDelay;
        public static void CopyVoiceGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && Time.time > copyVoiceGunDelay)
                    {
                        copyVoiceGunDelay = Time.time + 0.5f;

                        gunLocked = true;
                        lockTarget = gunTarget;

                        Patches.SpeakerPatch.enabled = true;

                        Patches.SpeakerPatch.targetSpeaker = lockTarget.gameObject.GetComponent<GorillaSpeakerLoudness>().speaker;

                        if (factory != null)
                            factory.Dispose();

                        factory = new LoopbackFactory();

                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Factory;
                        GorillaTagger.Instance.myRecorder.InputFactory = () =>
                        {
                            return factory;
                        };
                        CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;

                    if (factory != null)
                        factory.Dispose();

                    Patches.SpeakerPatch.enabled = false;

                    Sound.FixMicrophone();
                }
            }
        }

        public static void ProcessFrameBuffer(float[] data) =>
            factory.Feed(data);

        public static void ReloadMicrophone() =>
            GorillaTagger.Instance.myRecorder.RestartRecording(true);

        public static IEnumerator DelayReloadMicrophone()
        {
            yield return new WaitForSeconds(0.25f);
            ReloadMicrophone();
        }

        public static void ObjectToPointGun(string objectName)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    ThrowableBug bug = GetBug(objectName);
                    if (bug != null)
                        bug.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
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
                            glider.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                        else
                            glider.OnHover(null, null);
                    }
                }
            }
        }

        public static Coroutine dropBoard;
        public static void BetaDropBoard(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
        {
            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 5f)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = position + Vector3.down * 4f;

                if (dropBoard != null)
                    CoroutineManager.instance.StopCoroutine(dropBoard);

                dropBoard = CoroutineManager.instance.StartCoroutine(EnableRig());
            }

            FreeHoverboardManager.instance.SendDropBoardRPC(position, rotation, velocity, avelocity, boardColor);
            RPCProtection();
        }

        private static float hoverboardGunDelay;
        public static void HoverboardGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > hoverboardGunDelay)
                {
                    hoverboardGunDelay = Time.time + 0.25f;
                    BetaDropBoard(NewPointer.transform.position + Vector3.up, RandomQuaternion(), Vector3.zero, Vector3.zero, RandomColor());
                }
            }
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
                    RequestCreatePiece(pieceIdSet, NewPointer.transform.position + Vector3.up * 0.1f, Quaternion.identity, 0);
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
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + gunTarget.displayName + ".");
                    }
                }
            }
        }

        public static void CopyBlockInfoGun()
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
                        GUIUtility.systemCopyBuffer = @$"{gunTarget.displayName}
Piece Type: {gunTarget.pieceType}
Piece Name: {gunTarget.name}";
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied piece data of " + gunTarget.displayName + ".");
                    }
                }
            }
        }

        public static void SelectBlock(int type, string name)
        {
            pieceIdSet = type;
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + name.Replace("(Clone)", "") + ".");
        }

        private static Dictionary<int, string> blocks;
        public static Dictionary<int, string> GetAllBlockData()
        {
            if (blocks == null)
            {
                if (GetBuilderTable() == null)
                    return new Dictionary<int, string> { };

                blocks = new Dictionary<int, string> { };
                foreach (List<BuilderPiece> list in GetBuilderTable().builderPool.piecePools)
                {
                    foreach (BuilderPiece piece in list)
                    {
                        try
                        {
                            blocks.Add(piece.name.Replace("(Clone)", "").GetStaticHash(), piece.displayName ?? piece.displayName);
                        }
                        catch { }
                    }
                }
            }

            return blocks;
        }

        public static int[] GetAllBlockTypes() =>
            GetAllBlockData().Keys.ToArray();

        public static int GetRandomBlockType()
        {
            int[] blockTypes = GetAllBlockTypes();
            return blockTypes[UnityEngine.Random.Range(0, blockTypes.Length - 1)];
        }

        public static void BlockBrowser()
        {
            rememberdirectory = pageNumber;
            currentCategoryName = "Temporary Category";

            Dictionary<int, string> blocks = GetAllBlockData();
            List<ButtonInfo> blockButtons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Building Block Browser", method = () => RemoveCosmeticBrowser(), isTogglable = false, toolTip = "Returns you back to the fun mods." } };

            int i = 0;
            foreach (KeyValuePair<int, string> block in blocks)
            {
                blockButtons.Add
                (
                    new ButtonInfo
                    {
                        buttonText = $"SelectBlock{i}",
                        overlapText = block.Value,
                        method =() => SelectBlock(block.Key, block.Value),
                        isTogglable = false,
                        toolTip = $"Selects the block \"{block.Value}\" to be used for the building mods."
                    }
                );
                i++;
            }

            Buttons.buttons[29] = blockButtons.ToArray();
        }

        public static void SetRespawnDistance(string objectName, float respawnDistance = float.MaxValue)
        {
            GameObject bugObject = null;
            if (objectName == "Firefly")
                bugObject = GetAllType<ThrowableBug>().Where(bug => bug.gameObject.activeInHierarchy && bug.gameObject.name == "Floating Bug Holdable").ToArray()[1].gameObject;
            else
                bugObject = GetObject(objectName);

            bugObject.GetComponent<ThrowableBug>().maxDistanceFromOriginBeforeRespawn = float.MaxValue;
            bugObject.GetComponent<ThrowableBug>().maxDistanceFromTargetPlayerBeforeRespawn = float.MaxValue;
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

        // These mods are kind of suggestive
        // I've seen way more graphic stuff on other menus so don't you come at me for my suggestive mods
        public static void SnowballButtocks()
        {
            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.TransformDirection(
                new Vector3(-0.0436f, -0.3f, -0.1563f)
            );
            VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.TransformDirection(
                new Vector3(-0.0072f, -0.2964f, -0.1563f)
            );

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation * Quaternion.Euler(330f, 344.5f, 0f);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation * Quaternion.Euler(340f, 165.5f, 160f);

            VRRig.LocalRig.leftIndex.calcT = 1f;
            VRRig.LocalRig.leftMiddle.calcT = 1f;
            VRRig.LocalRig.leftThumb.calcT = 1f;

            VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
            VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

            VRRig.LocalRig.rightIndex.calcT = 1f;
            VRRig.LocalRig.rightMiddle.calcT = 1f;
            VRRig.LocalRig.rightThumb.calcT = 1f;

            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.rightThumb.LerpFinger(1f, false);

            GrowingSnowballThrowable LeftHandSnowball = GetProjectile("GrowingSnowballLeftAnchor") as GrowingSnowballThrowable;
            if (!LeftHandSnowball.gameObject.activeSelf)
            {
                LeftHandSnowball.SetSnowballActiveLocal(true);
                LeftHandSnowball.SetSizeLevelAuthority(3);

                VRRig.LocalRig.SetThrowableProjectileColor(true, VRRig.LocalRig.playerColor);
                LeftHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }

            GrowingSnowballThrowable RightHandSnowball = GetProjectile("GrowingSnowballRightAnchor") as GrowingSnowballThrowable;
            if (!RightHandSnowball.gameObject.activeSelf)
            {
                RightHandSnowball.SetSnowballActiveLocal(true);
                RightHandSnowball.SetSizeLevelAuthority(3);

                VRRig.LocalRig.SetThrowableProjectileColor(false, VRRig.LocalRig.playerColor);
                RightHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }
        }

        public static void SnowballBreasts()
        {
            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.TransformDirection(
                new Vector3(-0.08f, -0.0691f, 0f)
            );
            VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.TransformDirection(
                new Vector3(-0.0073f, -0.2182f, 0.0164f)
            );

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation * Quaternion.Euler(350f, 140f, 62f);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation * Quaternion.Euler(8f, 30f, 8f);

            VRRig.LocalRig.leftIndex.calcT = 1f;
            VRRig.LocalRig.leftMiddle.calcT = 1f;
            VRRig.LocalRig.leftThumb.calcT = 1f;

            VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
            VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

            VRRig.LocalRig.rightIndex.calcT = 1f;
            VRRig.LocalRig.rightMiddle.calcT = 1f;
            VRRig.LocalRig.rightThumb.calcT = 1f;

            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.rightThumb.LerpFinger(1f, false);

            GrowingSnowballThrowable LeftHandSnowball = GetProjectile("GrowingSnowballLeftAnchor") as GrowingSnowballThrowable;
            if (!LeftHandSnowball.gameObject.activeSelf)
            {
                LeftHandSnowball.SetSnowballActiveLocal(true);
                LeftHandSnowball.IncreaseSize(3);

                VRRig.LocalRig.SetThrowableProjectileColor(true, VRRig.LocalRig.playerColor);
                LeftHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }

            GrowingSnowballThrowable RightHandSnowball = GetProjectile("GrowingSnowballRightAnchor") as GrowingSnowballThrowable;
            if (!RightHandSnowball.gameObject.activeSelf)
            {
                RightHandSnowball.SetSnowballActiveLocal(true);
                RightHandSnowball.IncreaseSize(3);

                VRRig.LocalRig.SetThrowableProjectileColor(false, VRRig.LocalRig.playerColor);
                RightHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }
        }

        public static void DisableSnowballGenitals()
        {
            VRRig.LocalRig.enabled = true;

            GetProjectile("GrowingSnowballLeftAnchor").SetSnowballActiveLocal(false);
            GetProjectile("GrowingSnowballRightAnchor").SetSnowballActiveLocal(false);
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
            BetaDropBoard(VRRig.LocalRig.transform.position, VRRig.LocalRig.transform.rotation, Vector3.zero, Vector3.zero, RandomColor());
            GorillaLocomotion.GTPlayer.Instance.SetHoverAllowed(true);
        }

        private static float hoverboardSpamDelay;
        public static void HoverboardSpam()
        {
            if (rightGrab && Time.time > hoverboardSpamDelay)
            {
                hoverboardSpamDelay = Time.time + 0.5f;

                BetaDropBoard(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength, Vector3.zero, RandomColor());
            }
        }

        public static void OrbitHoverboards()
        {
            if (Time.time > hoverboardSpamDelay)
            {
                hoverboardSpamDelay = Time.time + 0.25f;

                float offset = 0f;
                Vector3 position = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                offset = -25f;
                Vector3 position2 = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                BetaDropBoard(GorillaTagger.Instance.headCollider.transform.position + position, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - position).normalized), (position2 - position).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomColor());

                offset = 180f;
                position = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                offset = 155f;
                position2 = new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);

                BetaDropBoard(GorillaTagger.Instance.headCollider.transform.position + position, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - position).normalized), (position2 - position).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomColor());
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

        public static void RainbowHoverboard()
        {
            if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
            {
                float h = (Time.frameCount / 180f) % 1f;
                Color rgbColor = Color.HSVToRGB(h, 1f, 1f);
                VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, rgbColor);
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

        public static void RandomHoverboard()
        {
            if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
                VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, RandomColor());
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

        public static Coroutine bugCoroutine;
        public static IEnumerator ReturnRig()
        {
            yield return new WaitForSeconds(0.2f);
            VRRig.LocalRig.enabled = true;
            bugCoroutine = null;
        }

        public static float getOwnershipDelay;
        public static ThrowableBug GetBugObject(string name)
        {
            GameObject bugObject = null;
            if (name == "Firefly")
                bugObject = GetAllType<ThrowableBug>().Where(bug => bug.gameObject.activeInHierarchy && bug.gameObject.name == "Floating Bug Holdable").ToArray()[1].gameObject;
            else
                bugObject = GetObject(name);
            if (bugObject == null)
                return null;

            ThrowableBug bug = bugObject.GetComponent<ThrowableBug>();
            if (bug == null)
                return null;

            return bug;
        }

        public static ThrowableBug GetBug(string name)
        {
            ThrowableBug bug = GetBugObject(name);

            if (bug == null)
                return null;

            GameObject bugObject = bug?.gameObject;

            if (!PhotonNetwork.InRoom)
                return bug;

            RequestableOwnershipGuard guard = bug.worldShareableInstance.guard;
            if (guard == null)
                return null;

            if (!bug.IsMyItem())
            {
                if (bug.currentState != TransferrableObject.PositionState.Dropped && bug.currentState != TransferrableObject.PositionState.None)
                    return null;
                
                VRRig.LocalRig.enabled = true;
                if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, bugObject.transform.position) > 15f)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = bugObject.transform.position;

                    if (bugCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(bugCoroutine);

                    bugCoroutine = CoroutineManager.instance.StartCoroutine(ReturnRig());
                }

                if (Vector3.Distance(ServerPos, bugObject.transform.position) > 15f)
                    return null;

                if (Time.time < getOwnershipDelay)
                    return null;

                getOwnershipDelay = Time.time + 0.5f;

                guard.RequestOwnership(() => bug.currentState = TransferrableObject.PositionState.Dropped, null);
                return null;
            }
            else
            {
                if (bugCoroutine != null)
                {
                    CoroutineManager.instance.StopCoroutine(bugCoroutine);
                    VRRig.LocalRig.enabled = true;
                }

                bug.worldShareableInstance.transferableObjectState = TransferrableObject.PositionState.Dropped;
                return bug;
            }
        }

        private static float bugSpamDelay;
        private static bool bugSpamToggle;
        public static void BugSpam()
        {
            ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");

            if ((!bugObject.IsMyItem() || (bugObject.currentState != TransferrableObject.PositionState.Dropped && bugObject.currentState != TransferrableObject.PositionState.None)) && bugObject.GetComponent<ClampPosition>() != null)
                UnityEngine.Object.Destroy(bugObject.GetComponent<ClampPosition>());

            ThrowableBug fireflyObject = GetBugObject("Firefly");

            if ((!fireflyObject.IsMyItem() || (fireflyObject.currentState != TransferrableObject.PositionState.Dropped && fireflyObject.currentState != TransferrableObject.PositionState.None)) && fireflyObject.GetComponent<ClampPosition>() != null)
                UnityEngine.Object.Destroy(fireflyObject.GetComponent<ClampPosition>());

            ThrowableBug bug = GetBug("Floating Bug Holdable");
            ThrowableBug firefly = bug != null ? GetBug("Firefly") : bug;

            if (rightGrab)
            {
                ThrowableBug targetBug = bugSpamToggle ? bug : firefly;
                if (Time.time > bugSpamDelay)
                {
                    bugSpamToggle = !bugSpamToggle;
                    bugSpamDelay = Time.time + 0.5f;

                    targetBug = bugSpamToggle ? bug : firefly;
                    
                    GameObject bugSpamObject = new GameObject("iiMenu_BugSpamObject");
                    bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                    bugSpamObject.layer = 3;

                    if (GetIndex("Bug Colliders").enabled)
                    {
                        SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                        if (GetIndex("Bouncy Bug").enabled)
                        {
                            collider.material.bounciness = 1f;
                            collider.material.bounceCombine = PhysicMaterialCombine.Maximum;
                            collider.material.dynamicFriction = 0f;
                        }
                    }

                    bugSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                    bugSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                    Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                    rigidbody.velocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                    rigidbody.angularVelocity = RandomVector3(100f);

                    rigidbody.useGravity = !GetIndex("Zero Gravity Bugs").enabled;

                    targetBug.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                    bugSpamObject.AddComponent<DestroyOnRest>();
                    UnityEngine.Object.Destroy(bugSpamObject, 30f);
                }
            }
        }

        public static void HolsterObject(string objectName, TransferrableObject.PositionState state)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null && (bug.currentState == TransferrableObject.PositionState.Dropped || bug.currentState == TransferrableObject.PositionState.None))
                bug.currentState = state;
        }

        public static void ObjectToHand(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (rightGrab && bug != null)
                bug.transform.position = GorillaTagger.Instance.rightHandTransform.position;
        }

        public static void GrabGliders()
        {
            if (rightGrab)
            {
                foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
                {
                    if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                        glider.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    else
                        glider.OnHover(null, null);
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

        public static void BuildingBlockMinigun()
        {
            if (rightGrab)
            {
                RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0, null, false, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength);
                RPCProtection();
            }
        }

        public static void DestroyObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void RespawnGliders()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                    glider.Respawn();
                else
                    glider.OnHover(null, null);
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

                foreach (BuilderPiece piece in 
                    PhotonNetwork.IsMasterClient ? GetAllType<BuilderPiece>() : 
                    GetAllType<BuilderPiece>()
                        .Where(piece => piece.gameObject.activeInHierarchy)
                        .Where(piece => Vector3.Distance(piece.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 2.5f))
                {
                    if (count > 400)
                        break;
                    if (piece.gameObject.activeSelf)
                    {
                        count++;
                        RequestRecyclePiece(piece, true, 2);
                    }
                }
            }
        }

        private static float startTimeBuilding = 0f;
        public static void EnableAtticAntiReport() =>
            startTimeBuilding = Time.time + 5f;

        public static void AtticAntiReport()
        {
            if (Time.time > startTimeBuilding)
                GetIndex("Attic Anti Report").enabled = false;

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                {
                    RequestCreatePiece(-566818631, line.reportButton.transform.position + RandomVector3(0.3f), RandomQuaternion(), 0, null, true);
                    RPCProtection();
                }
            }
        }

        public static void AtticDrawGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    else
                        CoroutineManager.RunCoroutine(DrawSmallDelay(NewPointer.transform.position));
                }
            }
        }

        public static void AtticBuildGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    else
                    {
                        RequestCreatePiece(pieceIdSet, NewPointer.transform.position + RandomVector3(0.3f), RandomQuaternion(), 0, null, true);
                        RPCProtection();
                    }
                }
            }
        }

        public static IEnumerator DrawSmallDelay(Vector3 position)
        {
            GameObject Temporary = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Temporary.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            Temporary.transform.position = position;
            UnityEngine.Object.Destroy(Temporary.GetComponent<Collider>());
            yield return new WaitForSeconds(0.5f);
            RequestCreatePiece(pieceIdSet, Temporary.transform.position + RandomVector3(0.3f), RandomQuaternion(), 0, null, true);
            UnityEngine.Object.Destroy(Temporary);
            RPCProtection();
        }

        public static void AtticFreezeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    else
                    {
                        Player target = NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget));
                        RequestCreatePiece(-566818631, lockTarget.headMesh.transform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                        RequestCreatePiece(-566818631, lockTarget.leftHandTransform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                        RequestCreatePiece(-566818631, lockTarget.rightHandTransform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                        RPCProtection();
                    }
                }
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

        public static void AtticFreezeAll()
        {
            if (rightTrigger > 0.5f)
            {
                Player target = GetRandomPlayer(false);

                if (!PhotonNetwork.IsMasterClient)
                    NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                else
                {
                    VRRig rigTarget = GetVRRigFromPlayer(target);
                    RequestCreatePiece(-566818631, lockTarget.headMesh.transform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                    RequestCreatePiece(-566818631, lockTarget.leftHandTransform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                    RequestCreatePiece(-566818631, lockTarget.rightHandTransform.position + RandomVector3(0.4f), RandomQuaternion(), 0, target, true);
                    RPCProtection();
                }
            }
        }

        private static float floatPower = 0.35f;
        public static void AtticFloatGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>");
                    else
                    {
                        floatPower += (0.3f - floatPower) * 0.05f;
                        RequestCreatePiece(-566818631, lockTarget.transform.position + Vector3.down * floatPower, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 350f), 0f), 0, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)), true);
                        RPCProtection();
                    }
                }
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
                floatPower = 0.35f;
                if (gunLocked)
                    gunLocked = false;
            }
        }

        public static Vector3 position = Vector3.zero;
        public static void AtticTowerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (position != Vector3.zero)
                {
                    RequestCreatePiece(pieceIdSet, position, Quaternion.Euler(0f, Time.frameCount % 360, 0f), 0, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)), true);
                    RPCProtection();

                    position += new Vector3(0f, 0.1f, 0f);
                }

                if (GetGunInput(true))
                    position = NewPointer.transform.position;
            }
            else
                position = Vector3.zero;
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

            if (!File.Exists($"{PluginInfo.BaseDirectory}/shotgun.wav"))
                LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/refs/heads/main/shotgun.wav", "shotgun.wav");

            Sound.PlayAudio("shotgun.wav");

            BuilderPiece bullet = null;

            yield return CreateGetPiece(1925587737, piece => bullet = piece);
            while (bullet == null)
                yield return null;

            RequestGrabPiece(bullet, true, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestDropPiece(bullet, TrueRightHand().position + TrueRightHand().forward * 0.65f + TrueRightHand().right * 0.03f + TrueRightHand().up * 0.05f, TrueRightHand().rotation, TrueRightHand().forward * 19.9f, Vector3.zero);
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

        public static void PlaceBlockGun()
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
                        RequestPlacePiece(Patches.PlacePatch._piece, gunTarget, Patches.PlacePatch._bumpOffsetX, Patches.PlacePatch._bumpOffsetZ, Patches.PlacePatch._twist, Patches.PlacePatch._parentPiece, Patches.PlacePatch._attachIndex, Patches.PlacePatch._parentAttachIndex);
                        RPCProtection();
                    }
                }
            }
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
                        RequestRecyclePiece(gunTarget, true, 2);
                        RPCProtection();
                    }
                }
            }
        }

        public static int blockDebounceIndex = 2;
        public static float blockDebounce = 0.1f;
        public static void ChangeBlockDelay(bool positive = true)
        {
            if (positive)
                blockDebounceIndex++;
            else
                blockDebounceIndex--;

            if (blockDebounceIndex > 20)
                blockDebounceIndex = 1;
            if (blockDebounceIndex < 1)
                blockDebounceIndex = 20;

            blockDebounce = blockDebounceIndex / 20f;
            GetIndex("Change Block Delay").overlapText = "Change Block Delay <color=grey>[</color><color=green>" + blockDebounce.ToString() + "</color><color=grey>]</color>";
        }

        public static void RequestCreatePiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, Player target = null, bool overrideFreeze = false, Vector3? velocity = null, Vector3? angVelocity = null)
        {
            if (GetIndex("Random Block Type").enabled)
                pieceType = GetRandomBlockType();

            BuilderTable table = GetBuilderTable();
            BuilderTableNetworking Networking = table.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                if (Time.time > blockDelay)
                {
                    int pieceId = table.CreatePieceId();

                    object[] args = new object[]
                    {
                        pieceType,
                        pieceId,
                        BitPackUtils.PackWorldPosForNetwork(position),
                        BitPackUtils.PackQuaternionForNetwork(rotation),
                        materialType,
                        (byte)4,
                        1,
                        PhotonNetwork.LocalPlayer
                    };

                    if (target == null)
                        Networking.photonView.RPC("PieceCreatedByShelfRPC", RpcTarget.All, args);
                    else
                        Networking.photonView.RPC("PieceCreatedByShelfRPC", target, args);

                    if (overrideFreeze || !GetIndex("Zero Gravity Blocks").enabled)
                    {
                        blockDelay = Time.time + 0.02f;

                        args = new object[]
                        {
                            Networking.CreateLocalCommandId(),
                            pieceId,
                            true,
                            BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity),
                            PhotonNetwork.LocalPlayer
                        };

                        if (target == null)
                            Networking.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, args);
                        else
                            Networking.photonView.RPC("PieceGrabbedRPC", target, args);

                        args = new object[]
                        {
                            Networking.CreateLocalCommandId(),
                            pieceId,
                            position,
                            rotation,
                            velocity ?? Vector3.zero,
                            angVelocity ?? Vector3.zero,
                            PhotonNetwork.LocalPlayer
                        };

                        if (target == null)
                            Networking.photonView.RPC("PieceDroppedRPC", RpcTarget.All, args);
                        else
                            Networking.photonView.RPC("PieceDroppedRPC", target, args);
                    }
                }
            }
            else
            {
                if (Time.time > blockDelay)
                {
                    blockDelay = Time.time + blockDebounce;
                    BuilderPiece piece = GetAllType<BuilderPiece>()
                        .Where(piece => piece.gameObject.activeInHierarchy)
                        .Where(piece => piece.pieceType == pieceType)
                        .Where(piece => !piece.isBuiltIntoTable)
                        .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                        .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                        .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                        .FirstOrDefault()
                        ?? null;

                    if (piece == null)
                        piece = GetAllType<BuilderPiece>()
                            .Where(piece => piece.gameObject.activeInHierarchy)
                            .Where(piece => !piece.isBuiltIntoTable)
                            .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                            .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                            .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                            .FirstOrDefault()
                            ?? null;

                    if (piece == null)
                        return;

                    if (Vector3.Distance(ServerLeftHandPos, position) > 2.5f)
                        position = ServerLeftHandPos + ((position - ServerLeftHandPos).normalized * 2.5f);

                    Networking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                    Networking.RequestDropPiece(piece, position, rotation, velocity ?? Vector3.zero, angVelocity ?? Vector3.zero);
                }
            }
        }

        public static void RequestCreatePiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, RpcTarget target, bool overrideFreeze = false)
        {
            if (GetIndex("Random Block Type").enabled)
                pieceType = GetRandomBlockType();

            BuilderTable table = GetBuilderTable();
            BuilderTableNetworking Networking = table.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                if (Time.time > blockDelay)
                {
                    int pieceId = table.CreatePieceId();

                    object[] args = new object[]
                    {
                        pieceType,
                        pieceId,
                        BitPackUtils.PackWorldPosForNetwork(position),
                        BitPackUtils.PackQuaternionForNetwork(rotation),
                        materialType,
                        (byte)4,
                        1,
                        PhotonNetwork.LocalPlayer
                    };

                    Networking.photonView.RPC("PieceCreatedByShelfRPC", target, args);

                    if (overrideFreeze || !GetIndex("Zero Gravity Blocks").enabled)
                    {
                        blockDelay = Time.time + 0.02f;

                        args = new object[]
                        {
                            Networking.CreateLocalCommandId(),
                            pieceId,
                            true,
                            BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity),
                            PhotonNetwork.LocalPlayer
                        };

                        Networking.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, args);

                        args = new object[]
                        {
                            Networking.CreateLocalCommandId(),
                            pieceId,
                            position,
                            rotation,
                            Vector3.zero,
                            Vector3.zero,
                            PhotonNetwork.LocalPlayer
                        };

                        Networking.photonView.RPC("PieceDroppedRPC", RpcTarget.All, args);
                    }
                }
            }
            else
            {
                if (Time.time > blockDelay)
                {
                    blockDelay = Time.time + blockDebounce;
                    BuilderPiece piece = GetAllType<BuilderPiece>()
                        .Where(piece => piece.gameObject.activeInHierarchy)
                        .Where(piece => piece.pieceType == pieceType)
                        .Where(piece => !piece.isBuiltIntoTable)
                        .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                        .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                        .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                        .FirstOrDefault()
                        ?? null;

                    if (piece == null)
                        piece = GetAllType<BuilderPiece>()
                            .Where(piece => piece.gameObject.activeInHierarchy)
                            .Where(piece => !piece.isBuiltIntoTable)
                            .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                            .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                            .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                            .FirstOrDefault()
                            ?? null;

                    if (piece == null)
                        return;

                    if (Vector3.Distance(ServerLeftHandPos, position) > 2.5f)
                        position = ServerLeftHandPos + ((position - ServerLeftHandPos).normalized * 2.5f);

                    Networking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                    Networking.RequestDropPiece(piece, position, rotation, Vector3.zero, Vector3.zero);
                }
            }
        }

        public static void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
        {
            BuilderTableNetworking Networking = GetBuilderTable().builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, new object[]
                {
                    Networking.CreateLocalCommandId(),
                    piece.pieceId,
                    isLefHand,
                    BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation),
                    PhotonNetwork.LocalPlayer
                });
            } 
            else
                Networking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
        }

        public static void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
        {
            if (attachPiece.isBuiltIntoTable)
                return;

            BuilderTableNetworking Networking = GetBuilderTable().builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PiecePlacedRPC", RpcTarget.All, new object[]
                {
                    Networking.CreateLocalCommandId(),
                    piece.pieceId,
                    attachPiece != null ? attachPiece.pieceId : -1,
                    BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ),
                    (parentPiece != null) ? parentPiece.pieceId : -1,
                    attachIndex,
                    parentAttachIndex,
                    PhotonNetwork.LocalPlayer,
                    PhotonNetwork.ServerTimestamp
                });
            }
            else
            {
                Networking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                Networking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
            }
        }

        public static void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
        {
            BuilderTableNetworking Networking = GetBuilderTable().builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceDroppedRPC", RpcTarget.All, new object[]
                {
                    Networking.CreateLocalCommandId(),
                    piece.pieceId,
                    position,
                    rotation,
                    velocity,
                    angVelocity,
                    PhotonNetwork.LocalPlayer
                });
            } 
            else
                Networking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
        }

        public static void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
        {
            if (piece.isBuiltIntoTable)
                return;

            BuilderTable table = GetBuilderTable();
            BuilderTableNetworking Networking = table.builderNetworking;

            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceDestroyedRPC", RpcTarget.All, new object[]
                {
                    piece.pieceId,
                    BitPackUtils.PackWorldPosForNetwork(piece.transform.position),
                    BitPackUtils.PackQuaternionForNetwork(piece.transform.rotation),
                    playFX,
                    (short)recyclerID
                });
            }
            else
            {
                if (Time.time > blockDelay)
                {
                    blockDelay = Time.time + blockDebounce;
                    if (piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position) && Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                    {
                        BuilderDropZone dropZone = table.dropZones
                            .Where(zone => (int)zone.dropType >= 1)
                            .Where(zone => Vector3.Distance(zone.transform.position, ServerLeftHandPos) < 2.5f)
                            .OrderBy(zone => Vector3.Distance(zone.transform.position, ServerLeftHandPos))
                            .FirstOrDefault() ?? null;

                        Vector3 dropPosition = dropZone != null ? dropZone.transform.position : ServerLeftHandPos + Vector3.down * 2f;

                        RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                        RequestDropPiece(piece, dropPosition, RandomQuaternion(), Vector3.down * 20f, Vector3.zero);
                    }
                }
            }
        }

        public static void BuildingBlockAura()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + RandomVector3().normalized * 2f, Quaternion.identity, 0);
            RPCProtection();
        }

        public static void RainBuildingBlocks()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 4f, UnityEngine.Random.Range(-3f, 3f)), Quaternion.identity, 0);
            RPCProtection();
        }

        public static void SpazObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.transform.rotation = RandomQuaternion();
        }

        public static void SpazGliders()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                    glider.gameObject.transform.rotation = RandomQuaternion();
                else
                    glider.OnHover(null, null);
            }
        }

        public static void SpazHoverboard()
        {
            if (VRRig.LocalRig.hoverboardVisual != null && VRRig.LocalRig.hoverboardVisual.IsHeld)
                VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, RandomQuaternion(), RandomColor());
        }

        public static void OrbitObject(string objectName, float offset = 0)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)), 2, MathF.Sin(offset + ((float)Time.frameCount / 30)));
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
                    glider.OnHover(null, null);
                
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
            GameObject bugObject = null;
            if (objectName == "Firefly")
                bugObject = GetAllType<ThrowableBug>().Where(bug => bug.gameObject.activeInHierarchy && bug.gameObject.name == "Floating Bug Holdable").ToArray()[1].gameObject;
            else
                bugObject = GetObject(objectName);

            TeleportPlayer(bugObject.transform.position);
            GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
        }

        private static bool lastWasNull;
        public static void BecomeObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            
            if (bug != null)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 99999f;

                bug.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                bug.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
            } else
            {
                if (lastWasNull)
                    VRRig.LocalRig.enabled = true;
            }

            lastWasNull = bug != null;
        }

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

            RequestCreatePiece(pieceType, VRRig.LocalRig.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, 0, null, true);
            RPCProtection();

            while (pieceId < 0)
                yield return null;
            
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

            yield return CreateGetPiece(-1927069002, piece => basea = piece);
            while (basea == null)
                yield return null;

            RequestGrabPiece(basea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
            yield return null;

            BuilderPiece base2a = null;

            yield return CreateGetPiece(-1621444201, piece => base2a = piece);
            while (base2a == null)
                yield return null;

            RequestGrabPiece(base2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(base2a, base2a, 0, 0, 0, basea, 1, 0);
            yield return null;

            BuilderPiece slopea = null;

            yield return CreateGetPiece(-993249117, piece => slopea = piece);
            while (slopea == null)
                yield return null;

            RequestGrabPiece(slopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(slopea, slopea, 0, 0, 2, base2a, 1, 0);
            yield return null;

            BuilderPiece trigger = null;

            yield return CreateGetPiece(251444537, piece => trigger = piece);
            while (trigger == null)
                yield return null;

            RequestGrabPiece(trigger, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(trigger, trigger, -1, -2, 3, slopea, 1, 0);
            yield return null;

            BuilderPiece slopeb = null;

            yield return CreateGetPiece(-993249117, piece => slopeb = piece);
            while (slopeb == null)
                yield return null;

            RequestGrabPiece(slopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(basea, trigger, 0, -2, 3, slopeb, 1, 0);
            yield return null;

            BuilderPiece base2b = null;

            yield return CreateGetPiece(-1621444201, piece => base2b = piece);
            while (base2b == null)
                yield return null;

            RequestGrabPiece(base2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(slopeb, slopeb, 0, 0, 2, base2b, 1, 0);
            yield return null;

            BuilderPiece baseb = null;

            yield return CreateGetPiece(-1927069002, piece => baseb = piece);
            while (baseb == null)
                yield return null;

            RequestGrabPiece(baseb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(base2b, base2b, 0, 0, 0, baseb, 1, 0);
            yield return null;

            BuilderPiece minislopeb = null;

            yield return CreateGetPiece(1700655257, piece => minislopeb = piece);
            while (minislopeb == null)
                yield return null;

            RequestGrabPiece(minislopeb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(baseb, slopea, 0, -3, 2, minislopeb, 2, 0);
            yield return null;

            BuilderPiece minislopea = null;

            yield return CreateGetPiece(1700655257, piece => minislopea = piece);
            while (minislopea == null)
                yield return null;

            RequestGrabPiece(minislopea, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(minislopeb, slopeb, 0, -3, 2, minislopea, 2, 0);
            yield return null;

            BuilderPiece minislope2a = null;

            yield return CreateGetPiece(1700655257, piece => minislope2a = piece);
            while (minislope2a == null)
                yield return null;

            RequestGrabPiece(minislope2a, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(minislopea, minislopeb, 0, 0, 2, minislope2a, 1, 0);
            yield return null;

            BuilderPiece minislope2b = null;

            yield return CreateGetPiece(1700655257, piece => minislope2b = piece);
            while (minislope2b == null)
                yield return null;

            RequestGrabPiece(minislope2b, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(minislope2a, minislopea, 0, 0, 2, minislope2b, 1, 0);
            yield return null;

            BuilderPiece flatthinga = null;

            yield return CreateGetPiece(477262573, piece => flatthinga = piece);
            while (flatthinga == null) 
                yield return null;

            RequestGrabPiece(flatthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(minislope2b, minislope2b, 0, -1, 2, flatthinga, 2, 0);
            yield return null;

            BuilderPiece flatthingb = null;

            yield return CreateGetPiece(477262573, piece => flatthingb = piece);
            while (flatthingb == null)
                yield return null;

            RequestGrabPiece(flatthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(flatthinga, minislope2a, 0, -1, 2, flatthingb, 2, 0);
            yield return null;

            BuilderPiece connectorthinga = null;

            yield return CreateGetPiece(251444537, piece => connectorthinga = piece);
            while (connectorthinga == null)
                yield return null;

            RequestGrabPiece(connectorthinga, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(flatthingb, flatthinga, -1, 1, 3, connectorthinga, 1, 0);
            yield return null;

            BuilderPiece connectorthingb = null;

            yield return CreateGetPiece(661312857, piece => connectorthingb = piece);
            while (connectorthingb == null)
                yield return null;

            RequestGrabPiece(connectorthingb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(connectorthinga, connectorthinga, -1, 0, 1, connectorthingb, 1, 0);
            yield return null;

            BuilderPiece connectorthingc = null;

            yield return CreateGetPiece(661312857, piece => connectorthingc = piece);
            while (connectorthingc == null)
                yield return null;

            RequestGrabPiece(connectorthingc, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(connectorthingb, connectorthinga, 0, 0, 1, connectorthingc, 1, 0);
            yield return null;

            BuilderPiece barrela = null;

            yield return CreateGetPiece(661312857, piece => barrela = piece);
            while (barrela == null)
                yield return null;

            RequestGrabPiece(barrela, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(connectorthingc, connectorthingb, 0, 0, 1, barrela, 1, 0);
            yield return null;

            BuilderPiece barrelb = null;

            yield return CreateGetPiece(661312857, piece => barrelb = piece);
            while (barrelb == null)
                yield return null;

            RequestGrabPiece(barrelb, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(barrela, barrela, 0, 0, 2, barrelb, 1, 0);
            yield return null;

            BuilderPiece scope = null;

            yield return CreateGetPiece(-648273975, piece => scope = piece);
            while (scope == null)
                yield return null;

            RequestGrabPiece(scope, false, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestPlacePiece(barrelb, minislope2a, -2, 1, 3, scope, 1, 0);
            yield return null;
            RequestDropPiece(scope, GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, Vector3.zero);
            yield return null;
            RequestGrabPiece(basea, false, new Vector3(-0.2f, 0.01f, -0.3f), new Quaternion(0f, 0.1f, 0.75f, -0.6f));
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

            yield return CreateGetPiece(pieceIdSet, piece => stupid = piece);
            while (stupid == null)
                yield return null;
            
            yield return new WaitForSeconds(0.2f);

            VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
            yield return null;
            RequestGrabPiece(stupid, false, Vector3.zero, Quaternion.identity);
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
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
                {
                    if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
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
                        if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetAllType<MonkeyeAI>();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                float offset = (360f / (float)them.Length) * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + ((float)Time.frameCount / 30)) * 2f, 1f, MathF.Sin(offset + ((float)Time.frameCount / 30)) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> <color=white>You are not master client.</color>"); return; }
                monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
            }
        }

        private static List<BuilderPiece> potentialgrabbedpieces = new List<BuilderPiece> { };
        public static void GrabAllBlocksNearby()
        {
            if (rightGrab && Time.time > blockDelay)
            {
                blockDelay = Time.time + blockDebounce;
                BuilderPiece piece = GetAllType<BuilderPiece>()
                    .Where(piece => piece.gameObject.activeInHierarchy)
                    .Where(piece => !piece.isBuiltIntoTable)
                    .Where(piece => piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                    .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                    .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                    .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                    .FirstOrDefault();

                if (piece == null)
                    return;

                RequestGrabPiece(piece, false, GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomVector3(0.5f), GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomQuaternion());
                potentialgrabbedpieces.Add(piece);

                RPCProtection();
            }

            if (rightTrigger > 0.5f && Time.time > blockDelay)
            {
                int amount = 0;
                blockDelay = Time.time + 0.3f;
                foreach (BuilderPiece piece in GetAllType<BuilderPiece>().Where(piece => piece.gameObject.activeInHierarchy).Where(piece => piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    if (amount > 100)
                        break;

                    amount++;

                    RequestDropPiece(piece, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, RandomVector3(19f), RandomVector3(19f));
                }

                potentialgrabbedpieces.Clear();
                RPCProtection();
            }
        }

        public static void GrabAllSelectedNearby()
        {
            if (rightGrab && Time.time > blockDelay)
            {
                blockDelay = Time.time + blockDebounce;
                BuilderPiece piece = GetAllType<BuilderPiece>()
                    .Where(piece => piece.gameObject.activeInHierarchy)
                    .Where(piece => piece.pieceType == pieceIdSet)
                    .Where(piece => !piece.isBuiltIntoTable)
                    .Where(piece => piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                    .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                    .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                    .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                    .FirstOrDefault();

                if (piece == null)
                    return;

                RequestGrabPiece(piece, false, GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomVector3(0.5f), GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomQuaternion());
                potentialgrabbedpieces.Add(piece);

                RPCProtection();
            }

            if (rightTrigger > 0.5f && Time.time > blockDelay)
            {
                int amount = 0;
                blockDelay = Time.time + 0.3f;
                foreach (BuilderPiece piece in GetAllType<BuilderPiece>().Where(piece => piece.gameObject.activeInHierarchy).Where(piece => piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    if (amount > 100)
                        break;

                    amount++;

                    RequestDropPiece(piece, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, RandomVector3(19f), RandomVector3(19f));
                }

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
                    if (balloon.ownerRig.isLocal)
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
                if (balloon.ownerRig.isLocal)
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
                if (balloon.ownerRig.isLocal)
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
                        if (balloon.ownerRig.isLocal)
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
                if (balloon.ownerRig.isLocal)
                    balloon.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
                else
                    balloon.WorldShareableRequestOwnership();
            }
        }

        // I would like to apologize to anyone who had this beforehand
        // "Tubski" will be long loved forever because that was the original name of this mod I don't know what that means
        public static void BecomeBalloon()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.inTryOnRoom = true;
            VRRig.LocalRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

            bool FoundBalloon = false;
            foreach (BalloonHoldable Balloon in GetAllType<BalloonHoldable>())
            {
                if (Balloon.ownerRig.isLocal && Balloon.gameObject.name.Contains("LMAMI"))
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
        public static int cycleSpeedIndex = 2;
        public static float nameCycleDebounce = 1f;

        public static void ChangeCycleDelay(bool positive = true)
        {
            if (positive)
                cycleSpeedIndex++;
            else
                cycleSpeedIndex--;

            if (cycleSpeedIndex > 4)
                cycleSpeedIndex = 1;
            if (cycleSpeedIndex < 1)
                cycleSpeedIndex = 4;

            nameCycleDebounce = cycleSpeedIndex / 2f;
            GetIndex("Change Cycle Delay").overlapText = "Change Name Cycle Delay <color=grey>[</color><color=green>" + nameCycleDebounce.ToString() + "</color><color=grey>]</color>";
        }

        public static void NameCycle(string[] names)
        {
            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex++;
                if (nameCycleIndex > names.Length - 1)
                    nameCycleIndex = 0;
                
                ChangeName(names[nameCycleIndex]);
                nameCycleDelay = Time.time + nameCycleDebounce;
            }
        }

        public static void RandomNameCycle() =>
            NameCycle(new string[] { GenerateRandomString(8) });

        public static string[] names = new string[] { };
        public static void EnableCustomNameCycle()
        {
            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt"))
                names = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt").Split('\n');
            else
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt","YOUR\nTEXT\nHERE");
        }

        public static void StrobeColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.05f;
                strobeColor = !strobeColor;
                ChangeColor(new Color(strobeColor ? 1 : 0, strobeColor ? 1 : 0, strobeColor ? 1 : 0));
            }
        }

        public static void RainbowColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.05f;
                float h = (Time.frameCount / 180f) % 1f;
                ChangeColor(Color.HSVToRGB(h, 1f, 1f));
            }
        }

        public static void HardRainbowColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.5f;
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
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton")
                            .GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)")
                            .GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)")
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
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }

            }
            lastHitR = rightGrab;
            if (leftPrimary && !lastHitLP)
            {
                GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeLeftButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                switch (hat)
                {
                    case 1:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                }
            }
            lastHitLP = leftPrimary;

            if (rightPrimary && !lastHitRP)
            {
                GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeRightItem").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                switch (hat)
                {
                    case 1:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 2:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
                        break;
                    case 3:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
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
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardobeHatButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 2:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeFaceButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 3:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeBadgeButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
                        break;
                    case 4:
                        GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeHoldableButton").GetComponent<WardrobeFunctionButton>().ButtonActivation();
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
                        ownedarchive.Add(dearlord.itemName);
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
            GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").GetComponent<CosmeticBoundaryTrigger>().enabled = false;
        }

        public static void NoAutoLoadCosmetics()
        {
            GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom").GetComponent<CosmeticBoundaryTrigger>().enabled = true;
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
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Item \"" + ToTitleCase(hat.overrideDisplayName) + "\" has been purchased.", 5000);
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

        private static float idgundelay = 0f;
        public static void CopyIDGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        idgundelay = Time.time + 0.5f;
                        string id = GetPlayerFromVRRig(gunTarget).UserId;
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                        GUIUtility.systemCopyBuffer = id;
                    }
                }
            }
        }

        public static void CopySelfID()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
            GUIUtility.systemCopyBuffer = id;
        }

        public static void NarrateIDGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        idgundelay = Time.time + 0.5f;
                        CoroutineManager.RunCoroutine(SpeakText("Name: " + GetPlayerFromVRRig(gunTarget).NickName + ". I D: " + string.Join(" ", GetPlayerFromVRRig(gunTarget).UserId)));
                    }
                }
            }
        }

        public static void NarrateSelfID() =>
            CoroutineManager.RunCoroutine(SpeakText("Name: " + PhotonNetwork.LocalPlayer.NickName + ". I D: " + string.Join(" ", PhotonNetwork.LocalPlayer.UserId)));

        private static float cgdgd = 0f;
        public static void CopyCreationDateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && Time.time > cgdgd)
                    {
                        cgdgd = Time.time + 0.5f;
                        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = GetPlayerFromVRRig(gunTarget).UserId }, delegate (GetAccountInfoResult result) // Who designed this
                        {
                            string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy h:mm tt");
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
                            GUIUtility.systemCopyBuffer = date;
                        }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not copy creation date."); }, null, null);
                    }
                }
            }
        }

        public static void CopyCreationDateSelf()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = PhotonNetwork.LocalPlayer.UserId }, delegate (GetAccountInfoResult result) // Who designed this
            {
                string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy h:mm tt");
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
                GUIUtility.systemCopyBuffer = date;
            }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not copy creation date."); }, null, null);
        }

        public static void NarrateCreationDateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && Time.time > cgdgd)
                    {
                        cgdgd = Time.time + 0.5f;
                        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = GetPlayerFromVRRig(gunTarget).UserId }, delegate (GetAccountInfoResult result) // Who designed this
                        {
                            string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy at h mm");
                            CoroutineManager.RunCoroutine(SpeakText(date));
                        }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not narrate creation date."); }, null, null);
                    }
                }
            }
        }

        public static void NarrateCreationDateSelf()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = PhotonNetwork.LocalPlayer.UserId }, delegate (GetAccountInfoResult result) // Who designed this
            {
                string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy at h mm");
                CoroutineManager.RunCoroutine(SpeakText(date));
            }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not narrate creation date."); }, null, null);
        }

        public static void GrabPlayerInfo()
        {
            string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                float r = 0f;
                float g = 0f;
                float b = 0f;
                string cosmetics = "";
                try
                {
                    VRRig plr = GetVRRigFromPlayer(player);
                    r = plr.playerColor.r * 255;
                    g = plr.playerColor.g * 255;
                    b = plr.playerColor.b * 255;
                    cosmetics = plr.concatStringOfCosmeticsAllowed;
                }
                catch { LogManager.Log("Failed to log colors, rig most likely nonexistent"); }
                try
                {
                    text += "\n====================================\n";
                    text += string.Concat(new string[]
                    {
                        "Player Name: \"",
                        player.NickName,
                        "\", Player ID: \"",
                        player.UserId,
                        "\", Player Color: (R: ",
                        r.ToString(),
                        ", G: ",
                        g.ToString(),
                        ", B: ",
                        b.ToString(),
                        "), Cosmetics: ",
                        cosmetics
                    });
                }
                catch { LogManager.Log("Failed to log player"); }
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/PlayerInfo/" + PhotonNetwork.CurrentRoom.Name + ".txt";

            File.WriteAllText(fileName, text);

            string filePath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;

            Process.Start(filePath);
        }
    }
}
