/*
 * ii's Stupid Menu  Mods/Fun.cs
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
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using iiMenu.Classes.Menu;
using iiMenu.Classes.Mods;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using iiMenu.Utilities;
using Ionic.Zlib;
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
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows.Speech;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.AssetUtilities;
using static iiMenu.Utilities.GameModeUtilities;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace iiMenu.Mods
{
    public static class Fun
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
        public static readonly float BPM = 159f;
        public static void HeadBang()
        {
            if (Time.time > lastBangTime)
            {
                VRRig.LocalRig.head.trackingRotationOffset.x = 50f;
                lastBangTime = Time.time + 60f/BPM;
            } 
            else
                VRRig.LocalRig.head.trackingRotationOffset.x = Mathf.Lerp(VRRig.LocalRig.head.trackingRotationOffset.x, 0f, 0.1f);
        }

        public static int headSpinIndex;
        public static void ChangeHeadSpinSpeed(bool positive = true)
        {
            float[] speedAmounts = { 2f, 7.5f, 8f, 9f, 200f };
            string[] speedNames = { "Very Slow", "Slow", "Normal", "Fast", "Very Fast" };

            if (positive)
                headSpinIndex++;
            else
                headSpinIndex--;

            headSpinIndex %= speedAmounts.Length;
            if (headSpinIndex < 0)
                headSpinIndex = speedAmounts.Length - 1;

            headSpinSpeed = speedAmounts[headSpinIndex];

            Buttons.GetIndex("Change Head Spin Speed").overlapText = "Change Head Spin Speed <color=grey>[</color><color=green>" + speedNames[headSpinIndex] + "</color><color=grey>]</color>";
        }

        private static float headSpinSpeed = 10f;
        public static void SpinHead(string axis)
        {
            if (VRRig.LocalRig.enabled)
            {
                switch (axis.ToLower())
                {
                    case "x":
                        VRRig.LocalRig.head.trackingRotationOffset.x += headSpinSpeed;
                        break;
                    case "y":
                        VRRig.LocalRig.head.trackingRotationOffset.y += headSpinSpeed;
                        break;
                    case "z":
                        VRRig.LocalRig.head.trackingRotationOffset.z += headSpinSpeed;
                        break;
                    default:
                        return;
                }
            }
            else
            {
                switch (axis.ToLower())
                {
                    case "x":
                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(headSpinSpeed, 0f, 0f));
                        break;
                    case "y":
                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, headSpinSpeed, 0f));
                        break;
                    case "z":
                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.head.rigTarget.transform.rotation.eulerAngles + new Vector3(0f, 0f, headSpinSpeed));
                        break;
                    default:
                        return;
                }
            }
        }

        public static void SpazHead(string axis)
        {
            int offset = Random.Range(0, 360);
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

            GTPlayer.Instance.GetControllerTransform(false).transform.position = lh;
            GTPlayer.Instance.GetControllerTransform(true).transform.position = rh;

            GTPlayer.Instance.GetControllerTransform(false).transform.rotation = lhr;
            GTPlayer.Instance.GetControllerTransform(true).transform.rotation = rhr;
        }

        public static void FixHandTaps()
        {
            EffectDataPatch.enabled = false;
            EffectDataPatch.tapsEnabled = true;
            EffectDataPatch.doOverride = false;
            EffectDataPatch.overrideVolume = 0.1f;
            EffectDataPatch.tapMultiplier = 1;
            GorillaTagger.Instance.handTapVolume = 0.1f;
        }

        public static void LoudHandTaps()
        {
            EffectDataPatch.enabled = true;
            EffectDataPatch.tapsEnabled = true;
            EffectDataPatch.doOverride = true;
            EffectDataPatch.overrideVolume = 99999f;
            EffectDataPatch.tapMultiplier = 10;
            GorillaTagger.Instance.handTapVolume = 99999f;
        }

        public static void SilentHandTaps()
        {
            EffectDataPatch.enabled = true;
            EffectDataPatch.tapsEnabled = false;
            EffectDataPatch.doOverride = false;
            EffectDataPatch.overrideVolume = 0f;
            EffectDataPatch.tapMultiplier = 0;
            GorillaTagger.Instance.handTapVolume = 0;
        }

        public static void SilentHandTapsOnTag()
        {
            if (VRRig.LocalRig.IsTagged())
                SilentHandTaps();
            else
                FixHandTaps();
        }

        private static float instantPartyDelay;
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
                    members.AddRange(from player in PhotonNetwork.PlayerListOthers where FriendshipGroupDetection.Instance.IsInMyGroup(player.UserId) || provisionalMembers.Contains(player.ActorNumber) select player.ActorNumber);
                    FriendshipGroupDetection.Instance.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(targetColor), members.ToArray(), false);
                    RPCProtection();
                }
            }
        }

        public static Coroutine waterSplashCoroutine;
        public static IEnumerator EnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        public static void BetaWaterSplash(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, object general = null)
        {
            general ??= RpcTarget.All;

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

            object[] parameters = {
                splashPosition,
                splashRotation,
                splashScale,
                boundingRadius,
                bigSplash,
                enteringWater
            };

            try
            {
                switch (general)
                {
                    case NetPlayer player:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", NetPlayerToPlayer(player), parameters);
                        break;
                    case RpcTarget target:
                    {
                        if (target == RpcTarget.All)
                        {
                            ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f);
                            ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale);

                            target = RpcTarget.Others;
                        }

                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", target, parameters);
                        break;
                    }
                    case int[] targets:
                    {
                        if (targets.Contains(NetworkSystem.Instance.LocalPlayer.ActorNumber))
                            ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f);

                        Overpowered.SpecialTargetRPC(GorillaTagger.Instance.myVRRig.GetView, "RPC_PlaySplashEffect", new RaiseEventOptions { TargetActors = targets }, parameters);
                        break;
                    }
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

                if (gunLocked && lockTarget != null)
                {
                    if (lockTarget.rightMiddle.calcT > 0.5f || lockTarget.leftMiddle.calcT > 0.5f)
                    {
                        if (Time.time > splashDel)
                        {
                            Vector3 splashPosition = lockTarget.rightMiddle.calcT > 0.5f ? lockTarget.rightHandTransform.position : lockTarget.leftHandTransform.position;
                            Quaternion splashRotation = lockTarget.rightMiddle.calcT > 0.5f ? lockTarget.rightHandTransform.rotation : lockTarget.leftHandTransform.rotation;

                            BetaWaterSplash(splashPosition, splashRotation, 4f, 100f, true, false);
                            splashDel = Time.time + 0.1f;
                        }
                    }
                }
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

        private static bool lastlhboop;
        private static bool lastrhboop;
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
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, sound, true, 999999f);
                    RPCProtection();
                }
                else
                    VRRig.LocalRig.PlayHandTapLocal(sound, true, 999999f);
            }
            if (isBoopRight && !lastrhboop)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, sound, false, 999999f);
                    RPCProtection();
                }
                else
                    VRRig.LocalRig.PlayHandTapLocal(sound, false, 999999f);
            }
            lastlhboop = isBoopLeft;
            lastrhboop = isBoopRight;
        }

        private static bool autoclickstate;
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

        public static readonly List<object[]> keyLogs = new List<object[]>();
        
        public static void EventReceived_KeyboardTracker(EventData data)
        {
            try
            {
                if (data.Code == 200)
                {
                    string rpcName = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[5].ToString())];
                    object[] args = (object[])((Hashtable)data.CustomData)[4];
                    if (rpcName == "RPC_PlayHandTap" && (int)args[0] == 66)
                    {
                        VRRig target = GetVRRigFromPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender));

                        Transform keyboardTransform = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").transform;
                        if (Vector3.Distance(target.transform.position, keyboardTransform.position) < 3f)
                        {
                            string handPath = (bool)args[1]
                                ? "rig/hand.L/palm.01.L/f_index.01.L/f_index.02.L/f_index.03.L/f_index.03.L_end"
                                : "rig/hand.R/palm.01.R/f_index.01.R/f_index.02.R/f_index.03.R/f_index.03.R_end";

                            Vector3 position = target.gameObject.transform.Find(handPath).position;

                            GameObject keysParent = keyboardTransform.Find("Buttons/Keys").gameObject;
                            float minimalDist = float.MaxValue;
                            string closestKey = "[Null]";

                            foreach (Transform child in keysParent.transform)
                            {
                                float dist = Vector3.Distance(child.position, position);
                                if (dist < minimalDist)
                                {
                                    minimalDist = dist;
                                    closestKey = ToTitleCase(child.name);
                                }
                            }

                            if (closestKey.Length > 1)
                                closestKey = "[" + closestKey + "]";

                            bool isKeyLogged = false;
                            for (int i = 0; i < keyLogs.Count; i++)
                            {
                                object[] keyLog = keyLogs[i];
                                if ((VRRig)keyLog[0] == target)
                                {
                                    isKeyLogged = true;

                                    string currentText = (string)keyLog[1];

                                    keyLogs[i][1] = closestKey.Contains("Delete") ? currentText.Length == 0 ? currentText : currentText[..^1] : currentText + closestKey;

                                    keyLogs[i][2] = Time.time + 5f;
                                    break;
                                }
                            }

                            if (!isKeyLogged)
                            {
                                if (!closestKey.Contains("Delete"))
                                    keyLogs.Add(new object[] { target, closestKey, Time.time + 5f });
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static void EnableKeyboardTracker() =>
            PhotonNetwork.NetworkingClient.EventReceived += EventReceived_KeyboardTracker;

        public static void KeyboardTracker()
        {
            if (keyLogs.Count > 0)
            {
                foreach (var keylog in keyLogs.Where(keylog => Time.time > (float)keylog[2]).ToList())
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=purple>KEYLOGS</color><color=grey>]</color> " + (string)keylog[1], 5000);
                    keyLogs.Remove(keylog);
                }
            }
        }

        public static void DisableKeyboardTracker() =>
            PhotonNetwork.NetworkingClient.EventReceived -= EventReceived_KeyboardTracker;

        public static void PreloadTagSounds()
        {
            string[] sounds = {
                "firstblood",
                "doublekill",
                "triplekill",
                "killingspree",
                "wickedsick",
                "monsterkill",
                "rampage"
            };

            foreach (string sound in sounds)
                LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/TagSounds/{sound}.ogg", $"Audio/Mods/Fun/TagSounds/{sound}.ogg");
        }

        private static GameObject FreeCamObject;
        private static Vector3 CameraVelocity;
        public static void Freecam()
        {
            if (FreeCamObject == null)
            {
                FreeCamObject = new GameObject("iiMenu_CameraObj");
                FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            }

            Camera FreeCamera = FreeCamObject.GetOrAddComponent<Camera>();
            FreeCamera.nearClipPlane = 0.01f;
            FreeCamera.cameraType = CameraType.Game;

            Vector3 inputDirection = new Vector3(leftJoystick.x, rightJoystick.y, leftJoystick.y);

            Vector3 playerForward = GTPlayer.Instance.bodyCollider.transform.forward.X_Z();
            Vector3 playerRight = GTPlayer.Instance.bodyCollider.transform.right.X_Z();

            Vector3 velocity = inputDirection.x * playerRight + inputDirection.y * Vector3.up + inputDirection.z * playerForward;
            velocity *= Movement.FlySpeed;

            CameraVelocity = Vector3.Lerp(CameraVelocity, velocity, 0.12875f);
            FreeCamObject.transform.position += CameraVelocity * Time.unscaledDeltaTime;
            FreeCamObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }

        public static void ThirdPersonCamera()
        {
            if (FreeCamObject == null)
            {
                FreeCamObject = new GameObject("iiMenu_CameraObj");
                FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            }

            Camera FreeCamera = FreeCamObject.GetOrAddComponent<Camera>();
            FreeCamera.nearClipPlane = 0.01f;
            FreeCamera.cameraType = CameraType.Game;

            FreeCamObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.TransformPoint(new Vector3(0f, 0.5f, -1.5f));
            FreeCamObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }

        public static void FlipCamera()
        {
            if (FreeCamObject == null)
            {
                FreeCamObject = new GameObject("iiMenu_CameraObj");
                FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            }

            Camera FreeCamera = FreeCamObject.GetOrAddComponent<Camera>();
            FreeCamera.nearClipPlane = 0.01f;
            FreeCamera.cameraType = CameraType.Game;

            FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            FreeCamObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        public static void Nausea()
        {
            if (FreeCamObject == null)
            {
                FreeCamObject = new GameObject("iiMenu_CameraObj");
                FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            }

            float intensity = 15f;

            Camera FreeCamera = FreeCamObject.GetOrAddComponent<Camera>();
            FreeCamera.nearClipPlane = 0.01f;
            FreeCamera.cameraType = CameraType.Game;

            FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
            FreeCamObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation * Quaternion.Euler(Mathf.Sin(Time.time) * intensity, Mathf.Cos(Time.time * 0.7f) * intensity, Mathf.Sin(Time.time * 1.3f) * intensity);
        }

        public static void HueShift(Color color) =>
            ZoneShaderSettings.activeInstance.SetGroundFogValue(color, 0f, float.MaxValue, 0f);

        public static float elapsedTime = Time.time;

        private static bool wasTagged;
        public static void PreloadJumpscareData()
        {
            LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Fun/jumpscare.png", "Images/Mods/Fun/jumpscare.png");
            LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/jumpscare.ogg", "Audio/Mods/Fun/jumpscare.ogg");
        }
        
        public static void JumpscareOnTag()
        {
            bool isTagged = VRRig.LocalRig.IsTagged();

            if (isTagged && !wasTagged && Random.Range(0, 2000) == 1)
                Jumpscare();

            wasTagged = isTagged;
        }

        public static void Jumpscare() =>
            CoroutineManager.instance.StartCoroutine(JumpscareCoroutine());

        public static IEnumerator JumpscareCoroutine()
        {
            LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/jumpscare.ogg", "Audio/Mods/Fun/jumpscare.ogg").Play();

            HueShift(Color.black);
            GameObject jumpscareObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jumpscareObject.transform.SetParent(GorillaTagger.Instance.headCollider.transform, false);
            jumpscareObject.transform.localPosition = Vector3.forward * 1f;
            jumpscareObject.transform.localScale = new Vector3(1f, 1f, 0.01f);
            jumpscareObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
            Object.Destroy(jumpscareObject.GetComponent<Collider>());

            Material jumpscareMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

            jumpscareMaterial.SetFloat("_Surface", 1);
            jumpscareMaterial.SetFloat("_Blend", 0);
            jumpscareMaterial.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            jumpscareMaterial.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            jumpscareMaterial.SetFloat("_ZWrite", 0);
            jumpscareMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            jumpscareMaterial.renderQueue = (int)RenderQueue.Transparent;

            jumpscareObject.GetComponent<Renderer>().material = jumpscareMaterial;
            jumpscareObject.GetComponent<Renderer>().material.mainTexture = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Fun/jumpscare.png", "Images/Mods/Fun/jumpscare.png");

            for (int i = 0; i < 10; i++)
            {
                jumpscareObject.GetComponent<Renderer>().material.color = Color.white * ((i + 1) % 2);
                yield return new WaitForSeconds(0.05f);
            }

            Object.Destroy(jumpscareObject);

            HueShift(Color.clear);
        }

        public static void SpectateGun()
        {
            if (GetGunInput(false))
            {
                if (gunLocked && lockTarget != null)
                {
                    if (FreeCamObject == null)
                    {
                        FreeCamObject = new GameObject("iiMenu_CameraObj");
                        FreeCamObject.transform.position = GorillaTagger.Instance.headCollider.transform.position;
                    }

                    Camera FreeCamera = FreeCamObject.GetOrAddComponent<Camera>();
                    FreeCamera.nearClipPlane = 0.01f;
                    FreeCamera.cameraType = CameraType.Game;

                    FreeCamObject.transform.position = lockTarget.headMesh.transform.transform.TransformPoint(new Vector3(0f, 0.25f, 0.25f));
                    FreeCamObject.transform.rotation = lockTarget.headMesh.transform.rotation;
                } else
                {
                    var GunData = RenderGun();
                    RaycastHit Ray = GunData.Ray;

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
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    DisableFreecam();
                }
            }
        }

        public static void DisableFreecam()
        {
            if (FreeCamObject != null)
            {
                Object.Destroy(FreeCamObject.GetComponent<Camera>());
                Object.Destroy(FreeCamObject);
                FreeCamObject = null;
            }
        }

        public static int targetFOV = 90;
        public static void ChangeTargetFOV(bool positive = true)
        {
            if (positive)
                targetFOV += 5;
            else
                targetFOV -= 5;

            if (targetFOV > 180)
                targetFOV = 0;
            if (targetFOV < 0)
                targetFOV = 180;

            Buttons.GetIndex("Change Target FOV").overlapText = "Change Target FOV <color=grey>[</color><color=green>" + targetFOV + "</color><color=grey>]</color>";
        }

        public static void CameraFOV()
        {
            if (TPC != null)
                TPC.GetComponent<Camera>().fieldOfView = targetFOV;
        }

        public static void FixCameraFOV()
        {
            if (TPC != null)
                TPC.GetComponent<Camera>().fieldOfView = 60f;
        }

        public static void PrioritizeVoiceGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                foreach (VRRig rig in VRRigCache.Instance.GetAllRigs())
                    rig.voiceAudio.volume = rig != lockTarget ? 0.2f : 1f;

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
                gunLocked = false;
        }

        public static void DeprioritizeVoiceGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                foreach (VRRig rig in VRRigCache.Instance.GetAllRigs())
                    rig.voiceAudio.volume = rig != lockTarget ? 1f : 0.2f;

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
                gunLocked = false;
        }

        public static void ResetVoiceAll()
        {
            foreach (VRRig rig in VRRigCache.Instance.GetAllRigs())
                rig.voiceAudio.volume = 1f;
        }

        private static float muteDelay;
        public static void MuteGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > muteDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.linePlayer == GetPlayerFromVRRig(gunTarget)))
                        {
                            muteDelay = Time.time + 0.5f;

                            line.muteButton.isOn = !line.muteButton.isOn;
                            line.PressButton(line.muteButton.isOn, GorillaPlayerLineButton.ButtonType.Mute);
                        }
                    }
                }
            }
        }

        public static void MuteAll()
        {
            foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => !line.muteButton.isAutoOn))
            {
                line.muteButton.isOn = true;
                line.PressButton(true, GorillaPlayerLineButton.ButtonType.Mute);
            }
        }

        public static void UnmuteAll()
        {
            foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.muteButton.isAutoOn))
            {
                line.muteButton.isOn = false;
                line.PressButton(false, GorillaPlayerLineButton.ButtonType.Mute);
            }
        }

        public static void ReportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > muteDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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

        private static float pressButtonDelay;
        public static void TriggerAntiReportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    try
                    {
                        foreach (var report in from line in GorillaScoreboardTotalUpdater.allScoreboardLines where line.linePlayer == GetPlayerFromVRRig(lockTarget) && Vector3.Distance(line.reportButton.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < 50f select line.reportButton.gameObject.transform)
                        {
                            VRRig.LocalRig.transform.position = report.transform.position;
                            VRRig.LocalRig.leftHand.rigTarget.transform.position = report.transform.position;
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = report.transform.position;
                        }
                    }
                    catch { }

                    if (Time.time > pressButtonDelay)
                    {
                        pressButtonDelay = Time.time + 0.1f;
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 67, false, 999999f);
                        RPCProtection();
                    }
                }

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
                {
                    VRRig.LocalRig.enabled = true;
                    gunLocked = false;
                }
            }
        }

        public static void TriggerAntiReportAll()
        {
            VRRig.LocalRig.enabled = false;

            try
            {
                Player triggerAntiReportTarget = GetRandomPlayer(false);
                foreach (var report in from line in GorillaScoreboardTotalUpdater.allScoreboardLines where NetPlayerToPlayer(line.linePlayer) == triggerAntiReportTarget && Vector3.Distance(line.reportButton.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < 50f select line.reportButton.gameObject.transform)
                {
                    VRRig.LocalRig.transform.position = report.transform.position;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = report.transform.position;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = report.transform.position;
                }
            }
            catch { }

            if (Time.time > pressButtonDelay)
            {
                pressButtonDelay = Time.time + 0.1f;
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 67, false, 999999f);
                RPCProtection();
            }
        }

        public static void BypassAntiReport()
        {
            SerializePatch.OverrideSerialization = () =>
            {

                bool isNearReportButton = false;
                List<int> people = new List<int> { };
                try
                {
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        float D1 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, report.position);
                        float D2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, report.position);

                        if (D1 < 0.5f || D2 < 0.5f)
                        {
                            people.Add(line.linePlayer.ActorNumber);
                            isNearReportButton = true;
                        }
                    }
                }
                catch { }

                if (GorillaTagger.Instance.myVRRig != null && isNearReportButton)
                {
                    MassSerialize(true, new PhotonView[] { GorillaTagger.Instance.myVRRig.GetView });

                    Vector3 positionArchiveLeft = VRRig.LocalRig.leftHand.rigTarget.transform.position;
                    Vector3 positionArchiveRight = VRRig.LocalRig.rightHand.rigTarget.transform.position;
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = PhotonNetwork.PlayerListOthers.Where(player => !people.ToArray().Contains(player.ActorNumber)).Select(player => player.ActorNumber).ToArray() });

                    VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.forward * 100f);
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.forward * 100f);

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions() { TargetActors = people.ToArray() });

                    VRRig.LocalRig.leftHand.rigTarget.transform.position = positionArchiveLeft;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = positionArchiveRight;

                    RPCProtection();

                    return false;
                }

                return true;
            };
        }

        public static void BreakModCheckers()
        {
            Hashtable props = new Hashtable();
            foreach (string mod in Visuals.modDictionary.Keys)
                props[mod] = true;

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public static readonly Dictionary<string, string> modsToSpoof = new Dictionary<string, string>();

        public static void ReloadModsToSpoof(string key, string value, bool add = true)
        {
            switch (add)
            {
                case true:
                    modsToSpoof.Add(key, value);
                    break;
                case false:
                    modsToSpoof.Remove(key);
                    break;
            }
            Hashtable props = new Hashtable();

            foreach (string mod in modsToSpoof.Keys)
                props[mod] = true;

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        public static void CustomModSpoofer()
        {
            Prompt("Would you like to choose from a mod list or type the mod property?", () =>
            {
                List<ButtonInfo> modList = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Mod List", method = () => Buttons.CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." } };
                modList.AddRange(Visuals.modDictionary.Select((t, i) => Visuals.modDictionary.ElementAt(i))
                .Select((mod, i) => new ButtonInfo
                {
                    buttonText = $"Mod{i}",
                    overlapText = mod.Value,
                    enableMethod = () => ReloadModsToSpoof(mod.Key, mod.Value),
                    disableMethod = () => ReloadModsToSpoof(mod.Key, mod.Value, false),
                    toolTip = $"Show that you are using the mod {mod.Value} to other players."
                }));


                Buttons.buttons[Buttons.GetCategory("Mod List")] = modList.ToArray();
                Buttons.CurrentCategoryName = "Mod List";
            }, () =>
            {
                PromptSingleText("Please enter what you would like to spoof your mods to (seperated by commas).", () =>
                {
                    string[] input = keyboardInput.Split(',').Select(s => s.Trim()).ToArray();
                    Hashtable props = new Hashtable();

                    foreach(string mod in input)
                    {
                        string foundKey = null;

                        foreach (var kv in Visuals.modDictionary)
                        {
                            if (string.Equals(kv.Value, mod, StringComparison.OrdinalIgnoreCase))
                            {
                                foundKey = kv.Key;
                                break;
                            }
                        }

                        props[foundKey ?? mod] = true;
                    }

                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }, "Done");
            }, "Mod List", "Type");
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

        private static float tapDelay;
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

        public static void TriggerLeafPileGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position = NewPointer.transform.position + (Vector3.up * (Time.frameCount % 2 == 1 ? 10f : -10f));
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView);

                    VRRig.LocalRig.transform.position = NewPointer.transform.position + (Vector3.up * (Time.frameCount % 2 == 1 ? 10f : 0f));
                }
                else
                    VRRig.LocalRig.enabled = true;
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

        private static float hitDelay;
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
                    moleMachine.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All);
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
                        moleMachine.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All);
                        RPCProtection();
                    }
                }
            }
        }

        public static void SetBraceletState(bool enable, bool isLeftHand) => 
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, enable, isLeftHand);

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

        private static bool previousBraceletSpamState;
        private static float braceletSpamDelay;
        public static void BraceletSpam()
        {
            if (Time.time > braceletSpamDelay)
            {
                GetBracelet(Time.frameCount % 2 == 0);
                braceletSpamDelay = Time.time + 0.1f;

                previousBraceletSpamState = !previousBraceletSpamState;
            }
        }

        public static void RemoveBracelet()
        {
            SetBraceletState(false, true);
            SetBraceletState(false, false);
            RPCProtection();
        }

        public static float isDirtyDelay;
        public static void RainbowBracelet()
        {
            BraceletPatch.enabled = true;
            if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
            {
                SetBraceletState(true, false);
                RPCProtection();

                VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(true);
            }
            List<Color> rgbColors = new List<Color>();
            for (int i=0; i<10; i++)
                rgbColors.Add(Color.HSVToRGB((Time.frameCount / 180f + i / 10f) % 1f, 1f, 1f));
            
            VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
            VRRig.LocalRig.reliableState.braceletSelfIndex = 99;
            VRRig.LocalRig.reliableState.braceletBeadColors = rgbColors;
            VRRig.LocalRig.friendshipBraceletRightHand.UpdateBeads(rgbColors, 99);

            if (Time.time > isDirtyDelay)
            {
                isDirtyDelay = Time.time + 0.1f;
                VRRig.LocalRig.reliableState.SetIsDirty();
            }
        }

        public static void RemoveRainbowBracelet()
        {
            BraceletPatch.enabled = false;
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

            VRRig.LocalRig.reliableState.SetIsDirty();
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

        private static float lastTimeDingied;
        public static void QuestNoises()
        {
            if (rightTrigger > 0.5f && Time.time > lastTimeDingied)
            {
                lastTimeDingied = Time.time + VRRig.LocalRig.fxSettings.GetDelay(10);
                GetAllType<MonkeBusinessStation>().FirstOrDefault().photonView.RPC("BroadcastRedeemQuestPoints", RpcTarget.All, 50);
            }
        }

        private static float delaybetweenscore;
        public static void MaxQuestScore()
        {
            if (Time.time > delaybetweenscore)
            {
                delaybetweenscore = Time.time + 1f;
                VRRig.LocalRig.SetQuestScore(int.MaxValue);
            }
        }

        public static int targetQuestScore = 69;
        public static void CustomQuestScore()
        {
            if (Time.time > delaybetweenscore)
            {
                delaybetweenscore = Time.time + 1f;
                VRRig.LocalRig.SetQuestScore(targetQuestScore);
            }
        }

        private static float spamDelay;
        private static bool returnOrTeleport;
        public static void ArcadeTeleporterEffectSpam()
        {
            if (Time.time > spamDelay)
            {
                spamDelay = Time.time + 0.1f;
                returnOrTeleport = !returnOrTeleport;

                GetObject("City_Pretty/CosmeticsScoreboardAnchor/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", RpcTarget.All, returnOrTeleport, (short)Random.Range(0, 7));
                RPCProtection();
            }
        }

        public static void StumpTeleporterEffectSpam()
        {
            if (Time.time > spamDelay)
            {
                spamDelay = Time.time + 0.1f;
                returnOrTeleport = !returnOrTeleport;

                GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/StumpVRHeadset/VirtualStump_StumpTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", RpcTarget.All, returnOrTeleport, (short)0);
                RPCProtection();
            }
        }

        public static void SetBasementDoorState(bool open)
        {
            if (Time.time > spamDelay)
            {
                delay = Time.time + 0.1f;

                GetObject("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", RpcTarget.AllViaServer, open ? GTDoor.DoorState.Opening : GTDoor.DoorState.Closing);
                RPCProtection();
            }
        }

        public static void SetElevatorDoorState(bool open)
        {
            if (Time.time > spamDelay)
            {
                delay = Time.time + 0.1f;

                GRElevatorManager.ElevatorButtonPressed(open ? GRElevator.ButtonType.Open : GRElevator.ButtonType.Close, GRElevatorManager._instance.currentLocation);
                RPCProtection();
            }
        }

        private static bool openOrClose;
        public static void BasementDoorSpam()
        {
            if (Time.time > spamDelay)
            {
                delay = Time.time + 0.1f;
                openOrClose = !openOrClose;

                GetObject("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", RpcTarget.AllViaServer, openOrClose ? GTDoor.DoorState.Opening : GTDoor.DoorState.Closing);
                RPCProtection();
            }
        }

        public static void ElevatorDoorSpam()
        {
            if (Time.time > spamDelay)
            {
                delay = Time.time + 0.1f;
                openOrClose = !openOrClose;

                GRElevatorManager.ElevatorButtonPressed(openOrClose ? GRElevator.ButtonType.Open : GRElevator.ButtonType.Close, GRElevatorManager._instance.currentLocation);
                RPCProtection();
            }
        }

        private static VirtualStumpAd virtualStumpAd;
        public static void CustomVirtualStumpVideo() =>
            virtualStumpAd ??= new GameObject("iiMenu_VirtualStumpAd").AddComponent<VirtualStumpAd>();

        public static void DisableCustomVirtualStumpVideo()
        {
            virtualStumpAd.enabled = false;
            Object.Destroy(virtualStumpAd.gameObject);
        }

        public static void ChangeCustomQuestScore(bool positive = true)
        {
            if (positive)
                targetQuestScore++;
            else
                targetQuestScore--;

            targetQuestScore %= 100000;
            if (targetQuestScore < 0)
                targetQuestScore = 99999;

            Buttons.GetIndex("Change Custom Quest Score").overlapText = "Change Custom Quest Score <color=grey>[</color><color=green>" + targetQuestScore + "</color><color=grey>]</color>";
        }

        public static void FakeFPS()
        {
            FPSPatch.enabled = true;
            FPSPatch.spoofFPSValue = Random.Range(0, 255);
        }

        

        public static void GrabIDCard()
        {
            if (rightGrab)
            {
                foreach (var entity in GhostReactor.instance.employeeBadges.registeredBadges.Select(grBadge => grBadge.gameEntity).Where(entity => entity.onlyGrabActorNumber == PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = entity.transform.position;

                    ManagerRegistry.GhostReactor.GameEntityManager.RequestGrabEntity(entity.id, false, Vector3.zero, Quaternion.identity);
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
                ManagerRegistry.GhostReactor.GhostReactorManager.ToolPurchaseStationRequest(Random.Range(0, ManagerRegistry.GhostReactor.GhostReactorManager.reactor.toolPurchasingStations.Count - 1), GhostReactorManager.ToolPurchaseStationAction.TryPurchase);
                purchaseDelay = Time.time + 0.1f;
            }
        }

        public static void SetCurrencySelf(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).shiftCreditCache = currency;
        }

        public static void SetCurrencyGun(int currency = 0)
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
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).shiftCreditCache = currency;
                        else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
        }

        public static void SetCurrencyAll(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.shiftCreditCache = currency;
            }
        }

        public static void AddCurrencySelf(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).shiftCreditCache += currency;
        }

        public static void AddCurrencyGun(int currency = 0)
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
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).shiftCreditCache += currency;
                        else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
        }

        public static void AddCurrencyAll(int currency = 0)
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.shiftCreditCache += currency;
            }
        }

        public static void RemoveCurrencySelf()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { return; }
            GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).shiftCreditCache = 0;
        }

        public static void RemoveCurrencyGun()
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
                            GRPlayer.Get(GetPlayerFromVRRig(gunTarget).ActorNumber).shiftCreditCache = 0;
                        else
                            NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    }
                }
            }
        }

        public static void RemoveCurrencyAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }

            foreach (Player target in PhotonNetwork.PlayerList)
            {
                GRPlayer plr = GRPlayer.Get(target.ActorNumber);
                plr.shiftCreditCache = 0;
            }
        }

        public static void Invincibility()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }

            GRPlayer plr = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);

            if (plr.State == GRPlayer.GRPlayerState.Ghost)
                ManagerRegistry.GhostReactor.GhostReactorManager.RequestPlayerStateChange(plr, GRPlayer.GRPlayerState.Alive);

            plr.hp = plr.maxHp;
        }

        public static void StartShift()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
            ManagerRegistry.GhostReactor.GhostReactorManager.RequestShiftStartAuthority(GhostReactor.instance.shiftManager.ShiftState == GhostReactorShiftManager.State.WaitingForFirstShiftStart);
            RPCProtection();
        }

        public static void EndShift()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
            ManagerRegistry.GhostReactor.GhostReactorManager.RequestShiftEnd();
            RPCProtection();
        }

        public static void SetQuota()
        {
            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
            GhostReactor.instance.shiftManager.shiftStats.SetShiftStat(GRShiftStatType.CoresCollected, GhostReactor.instance.shiftManager.coresRequiredToDelveDeeper);
            RPCProtection();
        }

        public static void GhostReactorFreezeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    Overpowered.CreateItem(lockTarget.GetPlayer(), Overpowered.ObjectByName["GhostReactorEnergyCostGate"], lockTarget.headMesh.transform.position + RandomVector3(), RandomQuaternion(), Vector3.zero, Vector3.zero);

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

        public static void GhostReactorFreezeAll()
        {
            VRRig randomPlayer = GetRandomVRRig(false);
            Overpowered.CreateItem(randomPlayer.GetPlayer(), Overpowered.ObjectByName["GhostReactorEnergyCostGate"], randomPlayer.headMesh.transform.position + RandomVector3(), RandomQuaternion(), Vector3.zero, Vector3.zero);
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
                ManagerRegistry.GhostReactor.GhostReactorManager.RequestPlayerStateChange(GRPlayer, State);
                RPCProtection();
                return;
            }

            if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }

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

            int netId = ManagerRegistry.GhostReactor.GameEntityManager.CreateTypeNetId(Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"]);

            ManagerRegistry.GhostReactor.GameEntityManager.photonView.RPC("CreateItemRPC", Target, new[] { netId }, new[] { (int)ManagerRegistry.GhostReactor.GameEntityManager.zone }, new[] { Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"] }, new[] { BitPackUtils.PackWorldPosForNetwork(Rig.transform.position) }, new[] { BitPackUtils.PackQuaternionForNetwork(Rig.transform.rotation) }, new[] { 0L }, new[] { 0 });

            ManagerRegistry.GhostReactor.GhostReactorManager.gameAgentManager.photonView.RPC("ApplyBehaviorRPC", Target, new[] { netId }, new byte[] { 6 });

            GRPlayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, ManagerRegistry.GhostReactor.GhostReactorManager);

            RPCProtection();

            yield return null;
            yield return null;
            yield return null;

            ManagerRegistry.GhostReactor.GameEntityManager.photonView.RPC("DestroyItemRPC", Target, new[] { netId });

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

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
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

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > killDelay)
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
                ManagerRegistry.GhostReactor.GhostReactorManager.ToolPurchaseStationRequest(Random.Range(0, ManagerRegistry.GhostReactor.GhostReactorManager.reactor.toolPurchasingStations.Count - 1), (GhostReactorManager.ToolPurchaseStationAction)Random.Range(0, 2));
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
                    floatVoice.AddPostProcessor(floatProcessor);
                }
            }

            public class PitchProcessor : IProcessor<float>
            {
                private readonly float pitch;

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
            private readonly Queue<float> buffer = new Queue<float>();

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

            if (RecorderPatch.enabled)
            {
                if (VoiceManager.Get().SamplingRate != samplingRate)
                {
                    VoiceManager.Get().SamplingRate = samplingRate; 
                    VoiceManager.Get().RestartMicrophone();
                }
            }   
            else
            {
                Recorder mic = GorillaTagger.Instance.myRecorder;

                if (mic.SamplingRate == (SamplingRate)samplingRate && mic.Bitrate == bitrate)
                    return;

                mic.SamplingRate = (SamplingRate)samplingRate;
                mic.Bitrate = bitrate;

                CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
            }      
        }

        public static void SetMicrophoneAmplification(bool amplify)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (RecorderPatch.enabled)
                VoiceManager.Get().Gain = amplify ? 16f : 1;
            else
            {
                Recorder mic = GorillaTagger.Instance.myRecorder;

                if (amplify)
                {
                    if (mic.gameObject.GetComponent<MicAmplifier>() != null)
                        return;

                    MicAmplifier microphoneAmplifier = mic.gameObject.GetOrAddComponent<MicAmplifier>();
                    microphoneAmplifier.AmplificationFactor = 16;
                    microphoneAmplifier.BoostValue = 16;
                }
                else
                {
                    if (mic.gameObject.GetComponent<MicAmplifier>())
                    {
                        if (mic.gameObject.GetComponent<MicAmplifier>() == null)
                            return;

                        MicAmplifier microphoneAmplifier = mic.gameObject.GetComponent<MicAmplifier>();
                        microphoneAmplifier.enabled = false;
                        Object.Destroy(mic.gameObject.GetComponent<MicAmplifier>());
                    }
                }

                CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
            }
                
        }

        public static void EchoMicrophone(bool status)
        {
            ButtonInfo button = Buttons.GetIndex("Legacy Microphone");
            if (button.enabled)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
                button.enabled = false;
                return;
            }

            if (status)
            {
                int sampleRate = VoiceManager.Get().SamplingRate;
                int samples = Mathf.Max(1, sampleRate / 4);
                float[] delayedBuffer = new float[samples];
                int index = 0;

                if (!VoiceManager.Get().PostProcessors.ContainsKey("Echo"))
                {
                    VoiceManager.Get().PostProcessors["Echo"] = buffer =>
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            float delayed = delayedBuffer[index];
                            float raw = buffer[i];
                            float mix = raw + delayed * 0.5f;

                            buffer[i] = Mathf.Clamp(mix, -1f, 1f);

                            delayedBuffer[index] = mix;
                            index = (index + 1) % samples;
                        }
                    };
                }
            }
            else
            {
                VoiceManager.Get().PostProcessors.Remove("Echo");
            }
        }

        public static void GlitchyMicrophone(bool status)
        {
            ButtonInfo button = Buttons.GetIndex("Legacy Microphone");
            if (button.enabled)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
                button.enabled = false;
                return;
            }

            if (status)
            {
                int rate = VoiceManager.Get().SamplingRate;
                int repeatLength = Mathf.Max(1, rate / 2);
                float[] history = new float[repeatLength];
                int historyIndex = 0;

                float[] repeatBuffer = new float[repeatLength];
                int repeatIndex = 0;
                int repeatsLeft = 0;

                int samplesUntilNext = Random.Range(rate * 1, rate * 4);

                VoiceManager.Get().PostProcessClip = true;
                if (!VoiceManager.Get().PostProcessors.ContainsKey("Glitch"))
                {
                    VoiceManager.Get().PostProcessors["Glitch"] = buffer =>
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            history[historyIndex] = buffer[i];
                            historyIndex = (historyIndex + 1) % repeatLength;
                            if (repeatsLeft > 0)
                            {
                                buffer[i] = repeatBuffer[repeatIndex];
                                repeatIndex++;
                                if (repeatIndex >= repeatLength)
                                {
                                    repeatIndex = 0;
                                    repeatsLeft--;
                                }
                            }
                            else
                            {
                                samplesUntilNext--;
                                if (samplesUntilNext <= 0)
                                {
                                    for (int j = 0; j < repeatLength; j++)
                                    {
                                        int index = (historyIndex + j) % repeatLength;
                                        repeatBuffer[j] = history[index];
                                    }
                                    repeatsLeft = Random.Range(1, 2);
                                    repeatIndex = 0;
                                    samplesUntilNext = Random.Range(rate * 1, rate * 4);
                                }
                            }
                        }
                    };
                } 
            }
            else
            {
                VoiceManager.Get().PostProcessors.Remove("Glitch");
            }
        }

        public static void LaggyMicrophone(bool status)
        {
            ButtonInfo button = Buttons.GetIndex("Legacy Microphone");
            if (button.enabled)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
                button.enabled = false;
                return;
            }

            if (status)
            {
                if (!VoiceManager.Get().PostProcessors.ContainsKey("Lag"))
                {
                    VoiceManager.Get().PostProcessors["Lag"] = buffer =>
                    {
                        if (UnityEngine.Random.value < 0.25f)
                            Array.Clear(buffer, 0, buffer.Length);
                    };
                }
            }
            else
                VoiceManager.Get().PostProcessors.Remove("Lag");
        }

        public static void MuteMicrophone(bool mute)
        {
            if (RecorderPatch.enabled)
                VoiceManager.Get().MuteMicrophone = mute;
            else
            {
                if (!PhotonNetwork.InRoom)
                    return;
                Recorder mic = GorillaTagger.Instance.myRecorder;
                if (mic.IsRecording != mute)
                    return;
                mic.IsRecording = !mute;
            }
        }

        public static void SetMicrophonePitch(float pitch)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (RecorderPatch.enabled)
                VoiceManager.Get().Pitch = pitch;
            else
            {
                Recorder mic = GorillaTagger.Instance.myRecorder;

                if (!Mathf.Approximately(pitch, 1f))
                {
                    MicPitchShifter pitchShifter = mic.gameObject.GetComponent<MicPitchShifter>();
                    if (pitchShifter != null && Mathf.Approximately(pitchShifter.PitchFactor, pitch))
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
                        Object.Destroy(mic.gameObject.GetComponent<MicPitchShifter>());
                    }
                    else
                        return;
                }

                CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
            }
                
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

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        if (RecorderPatch.enabled)
                        {
                            SpeakerPatch.enabled = true;
                            SpeakerPatch.targetSpeaker = lockTarget.gameObject.GetComponent<GorillaSpeakerLoudness>().speaker;
                            if (!VoiceManager.Get().PostProcessors.ContainsKey("CopyVoice"))
                            {
                                VoiceManager.Get().PostProcessors["CopyVoice"] = buffer =>
                                {
                                    for (int i = 0; i < buffer.Length && i < SpeakerPatch.frameOut.Buf.Length; i++)
                                    {
                                        buffer[i] = SpeakerPatch.frameOut.Buf[i];
                                    }
                                };
                            }

                        }
                        else
                        {
                            if (Time.time > copyVoiceGunDelay)
                            {
                                copyVoiceGunDelay = Time.time + 0.5f;

                                gunLocked = true;
                                lockTarget = gunTarget;

                                SpeakerPatch.enabled = true;

                                SpeakerPatch.targetSpeaker = lockTarget.gameObject.GetComponent<GorillaSpeakerLoudness>().speaker;

                                RecorderPatch.enabled = !Buttons.GetIndex("Legacy Microphone").enabled;

                                VoiceManager.Get().PostProcessors["CopyVoice"] = null;

                                factory?.Dispose();

                                factory = new LoopbackFactory();

                                GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Factory;
                                GorillaTagger.Instance.myRecorder.InputFactory = () =>
                                {
                                    return factory;
                                };
                                CoroutineManager.instance.StartCoroutine(DelayReloadMicrophone());
                            }
                        }
                            
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;

                    DisableCopyVoice();
                }
            }
        }

        public static void DisableCopyVoice()
        {
            factory?.Dispose();

            VoiceManager.Get().PostProcessors.Remove("CopyVoice");

            SpeakerPatch.enabled = false;

            Sound.FixMicrophone();

            RecorderPatch.enabled = !Buttons.GetIndex("Legacy Microphone").enabled;
        }

        public static void SaveNarration(string text)
        {
            string path = $"{PluginInfo.BaseDirectory}/Sounds/Narrations";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            CoroutineManager.instance.StartCoroutine(TranscribeText(text, (audio) =>
            {
                PromptSingleText("The narration has been saved in your Soundboard!", null, "Ok");
            }, text, path));
        }

        public static DictationRecognizer drec;
        public static void MaskVoice()
        {
            drec = new DictationRecognizer();
            drec.DictationResult += (text, confidence) =>
            {
                if (Settings.debugDictation)
                    LogManager.Log($"Dictation result: {text}");
                NotificationManager.SendNotification($"<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> {text}");

                if (GorillaTagger.Instance.myRecorder != null)
                {
                    GorillaTagger.Instance.myRecorder.IsRecording = true;
                    if (PhotonNetwork.InRoom)
                        SpeakText(text, true);
                    else
                        NarrateText(text);
                }
                else
                    NarrateText(text);
            };
            drec.DictationComplete += (completionCause) =>
            {
                drec.Start();
            };
            drec.DictationError += (error, hresult) =>
            {
                if (Settings.debugDictation)
                    LogManager.LogError($"Dictation error: {error}; HResult = {hresult}.");
            };
            drec.DictationHypothesis += (text) =>
            {
                if (Settings.debugDictation)
                    LogManager.Log($"Hypothesis: {text}");
                NotificationManager.ClearAllNotifications();
                NotificationManager.SendNotification($"<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> {text}");
            };
            drec.Start();
        }

        public static void DisableMaskVoice()
        {
            drec?.Stop();
            drec?.Dispose();

            GorillaTagger.Instance.myRecorder.IsRecording = true;
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
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    ThrowableBug bug = GetBug(objectName);
                    if (bug != null)
                        bug.transform.position = NewPointer.transform.position + Vector3.up;
                }
            }
        }

        public static void CameraGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
                    camera.visible = true;
                    camera.recording = true;

                    camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                    camera.m_CameraVisuals.SetRecordingState(true);

                    camera.transform.position = NewPointer.transform.position + Vector3.up;
                }
            }
        }

        public static void TabletGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
                    camera.visible = true;
                    camera.recording = true;

                    camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                    camera.m_CameraVisuals.SetRecordingState(true);

                    camera.transform.position = NewPointer.transform.position + Vector3.up;
                }
            }
        }

        public static void GliderGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
                    {
                        if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                            glider.gameObject.transform.position = NewPointer.transform.position + Vector3.up;
                        else
                            glider.OnHover(null, null);
                    }
                }
            }
        }

        public static Coroutine dropBoard;
        // ReSharper disable once ParameterHidesMember
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
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > hoverboardGunDelay)
                {
                    hoverboardGunDelay = Time.time + 0.25f;
                    BetaDropBoard(NewPointer.transform.position + Vector3.up, RandomQuaternion(), Vector3.zero, Vector3.zero, RandomColor());
                }
            }
        }
        
        private static int pieceIdSet = -566818631;
        private static float blockDelay;
        public static void BlocksGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    RequestCreatePiece(pieceIdSet, NewPointer.transform.position + Vector3.up * 0.1f, Quaternion.identity, 0);
                    RPCProtection();
                }
            }
        }

        private static float gbgd;
        public static void SelectBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    BuilderPiece gunTarget = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (gunTarget && Time.time > gbgd)
                    {
                        gbgd = Time.time + 0.1f;
                        pieceIdSet = gunTarget.pieceType;
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + gunTarget.displayName + ".");
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

                if (GetGunInput(true))
                {
                    BuilderPiece gunTarget = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (gunTarget && Time.time > gbgd)
                    {
                        gbgd = Time.time + 0.1f;
                        GUIUtility.systemCopyBuffer = @$"{gunTarget.displayName}
Piece Type: {gunTarget.pieceType}
Piece Name: {gunTarget.name}";
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied piece data of " + gunTarget.displayName + ".");
                    }
                }
            }
        }

        public static void SelectBlock(int type, string name)
        {
            pieceIdSet = type;
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + name.Replace("(Clone)", "") + ".");
        }

        private static Dictionary<int, string> blocks;
        public static Dictionary<int, string> GetAllBlockData()
        {
            if (blocks == null)
            {
                if (ManagerRegistry.BuilderTable == null)
                    return new Dictionary<int, string>();

                blocks = new Dictionary<int, string>();
                foreach (var piece in ManagerRegistry.BuilderTable.builderPool.piecePools.SelectMany(list => list))
                {
                    try
                    {
                        blocks.Add(piece.name.Replace("(Clone)", "").GetStaticHash(), piece.displayName ?? piece.displayName);
                    }
                    catch { }
                }
            }

            return blocks;
        }

        public static int[] GetAllBlockTypes() =>
            GetAllBlockData().Keys.ToArray();

        public static int GetRandomBlockType()
        {
            int[] blockTypes = GetAllBlockTypes();
            return blockTypes[Random.Range(0, blockTypes.Length)];
        }

        public static void BlockBrowser()
        {
            rememberdirectory = pageNumber;

            Dictionary<int, string> allBlockData = GetAllBlockData();
            List<ButtonInfo> blockButtons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Building Block Browser", method = RemoveCosmeticBrowser, isTogglable = false, toolTip = "Returns you back to the fun mods." } };

            int i = 0;
            foreach (KeyValuePair<int, string> block in allBlockData)
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

            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = blockButtons.ToArray();
            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void SetRespawnDistance(string objectName, float respawnDistance = float.MaxValue)
        {
            ThrowableBug bugObject = GetBugObject(objectName);

            bugObject.maxDistanceFromOriginBeforeRespawn = respawnDistance;
            bugObject.maxDistanceFromTargetPlayerBeforeRespawn = respawnDistance;
        }

        public static void PermanentOwnership(string objectName)
        {
            OwnershipPatch.enabled = true;

            ThrowableBug bugObject = GetBugObject(objectName);

            if (!PhotonNetwork.InRoom)
            {
                OwnershipPatch.blacklistedGuards.Clear();
                return;
            }

            if (!bugObject.IsMyItem())
                return;

            if (bugObject.targetRig != VRRig.LocalRig)
                bugObject.SetTargetRig(VRRig.LocalRig);

            if (!OwnershipPatch.blacklistedGuards.Contains(bugObject.worldShareableInstance.guard))
                OwnershipPatch.blacklistedGuards.Add(bugObject.worldShareableInstance.guard);
        }

        public static void SpazSnowballs()
        {
            if (leftGrab)
            {
                GrowingSnowballThrowable Snowball = GetProjectile($"{Projectiles.SnowballName}LeftAnchor") as GrowingSnowballThrowable;
                Snowball.randomizeColor = true;
                Snowball.SetSnowballActiveLocal(true);
                Snowball.SetSizeLevelAuthority(Random.Range(1, 6));
            }

            if (rightGrab)
            {
                GrowingSnowballThrowable Snowball = GetProjectile($"{Projectiles.SnowballName}RightAnchor") as GrowingSnowballThrowable;
                Snowball.randomizeColor = true;
                Snowball.SetSnowballActiveLocal(true);
                Snowball.SetSizeLevelAuthority(Random.Range(1, 6));
            }
        }

        public static void FastSnowballs()
        {
            foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
            {
                foreach (SnowballThrowable Throwable in Maker.snowballs)
                {
                    Throwable.linSpeedMultiplier = 10f;
                    Throwable.maxLinSpeed = 99999f;
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
                }
            }
        }

        public static void ProjectileRange()
        {
            LoopingArray<ProjectileTracker.ProjectileInfo> projectileArray = ProjectileTracker.m_localProjectiles;
            if (projectileArray == null || projectileArray.Length <= 0) return;
            for (int index = 0; index < projectileArray.Length; index++)
            {
                SlingshotProjectile projectileInstance = projectileArray[index].projectileInstance;
                if (projectileInstance == null || !projectileInstance.gameObject.activeSelf) continue;

                foreach (var rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal()).Where(rig => rig.Distance(projectileInstance.transform.position) < 0.5f))
                    projectileInstance.transform.position = rig.headMesh.transform.position;
            }
        }

        public static Color projHookColor = Color.white;
        public static void HookProjectileColors()
        {
            SerializePatch.OverrideSerialization = () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                    List<SnowballThrowable> activeSnowballs = new List<SnowballThrowable>();

                    foreach (SnowballMaker Maker in new[] { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance })
                        activeSnowballs.AddRange(Maker.snowballs.Where(Throwable => Throwable.gameObject.activeSelf));

                    if (activeSnowballs.Count <= 0)
                    {
                        SendSerialize(GorillaTagger.Instance.myVRRig.GetView);
                        return false;
                    }

                    foreach (SnowballThrowable snowball in activeSnowballs)
                        snowball.SetSnowballActiveLocal(false);

                    VRRig.LocalRig.reliableState.SetIsDirty();
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView);

                    foreach (SnowballThrowable snowball in activeSnowballs)
                    {
                        if (snowball is GrowingSnowballThrowable growingSnowball)
                            growingSnowball.maintainSizeLevelUntilLocalTime = Time.time;

                        snowball.randomizeColor = true;
                        VRRig.LocalRig.SetThrowableProjectileColor(snowball.gameObject.name.ToLower().Contains("left"), projHookColor);
                        snowball.SetSnowballActiveLocal(true);
                        snowball.ApplyColor(projHookColor);
                    }

                    VRRig.LocalRig.reliableState.SetIsDirty();
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView);

                    return false;
                }
                return true;
            };
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

            GrowingSnowballThrowable LeftHandSnowball = GetProjectile($"{Projectiles.SnowballName}LeftAnchor") as GrowingSnowballThrowable;
            if (!LeftHandSnowball.gameObject.activeSelf)
            {
                LeftHandSnowball.SetSnowballActiveLocal(true);
                LeftHandSnowball.SetSizeLevelAuthority(3);

                VRRig.LocalRig.SetThrowableProjectileColor(true, VRRig.LocalRig.playerColor);
                LeftHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }

            GrowingSnowballThrowable RightHandSnowball = GetProjectile($"{Projectiles.SnowballName}RightAnchor") as GrowingSnowballThrowable;
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

            GrowingSnowballThrowable LeftHandSnowball = GetProjectile($"{Projectiles.SnowballName}LeftAnchor") as GrowingSnowballThrowable;
            if (!LeftHandSnowball.gameObject.activeSelf)
            {
                LeftHandSnowball.SetSnowballActiveLocal(true);
                LeftHandSnowball.IncreaseSize(3);

                VRRig.LocalRig.SetThrowableProjectileColor(true, VRRig.LocalRig.playerColor);
                LeftHandSnowball.ApplyColor(VRRig.LocalRig.playerColor);
            }

            GrowingSnowballThrowable RightHandSnowball = GetProjectile($"{Projectiles.SnowballName}RightAnchor") as GrowingSnowballThrowable;
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

            GetProjectile($"{Projectiles.SnowballName}LeftAnchor").SetSnowballActiveLocal(false);
            GetProjectile($"{Projectiles.SnowballName}lRightAnchor").SetSnowballActiveLocal(false);
        }

        public static void FastHoverboard()
        {
            GTPlayer.Instance.hoverboardPaddleBoostMax = float.MaxValue;
            GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 5f;
            GTPlayer.Instance.hoverboardBoostGracePeriod = 0f;
            GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 1f;
        }

        public static void SlowHoverboard()
        {
            GTPlayer.Instance.hoverboardPaddleBoostMax = 3.5f;
            GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.025f;
            GTPlayer.Instance.hoverboardBoostGracePeriod = 3f;
            GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.1f;
        }

        public static void FixHoverboard()
        {
            GTPlayer.Instance.hoverboardPaddleBoostMax = 10f;
            GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.1f;
            GTPlayer.Instance.hoverboardBoostGracePeriod = 1f;
            GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.2f;
        }

        private static bool hasGrabbedHoverboard;
        public static void GlobalHoverboard()
        {
            if (!hasGrabbedHoverboard)
            {
                GTPlayer.Instance.GrabPersonalHoverboard(false, Vector3.zero, Quaternion.identity, Color.black);
                hasGrabbedHoverboard = true;
            }

            GTPlayer.Instance.SetHoverAllowed(true);
            GTPlayer.Instance.SetHoverActive(true);
            VRRig.LocalRig.hoverboardVisual.gameObject.SetActive(true);
        }

        public static void DisableGlobalHoverboard()
        {
            hasGrabbedHoverboard = false;

            GTPlayer.Instance.SetHoverAllowed(false);
            GTPlayer.Instance.SetHoverActive(false);
            VRRig.LocalRig.hoverboardVisual.gameObject.SetActive(false);
        }

        public static Coroutine DisableHoverboardCoroutine;
        public static IEnumerator DisableHoverboard()
        {
            yield return new WaitForSeconds(0.3f);

            GTPlayer.Instance.SetHoverActive(false);
            VRRig.LocalRig.hoverboardVisual.SetNotHeld();
        }

        public static void HoverboardScreenTarget(VRRig rig, Color color)
        {
            if (DisableHoverboardCoroutine != null)
                CoroutineManager.instance.StopCoroutine(DisableHoverboardCoroutine);

            DisableHoverboardCoroutine = CoroutineManager.instance.StartCoroutine(DisableHoverboard());

            Vector3 angVel = rig.headMesh.GetOrAddComponent<GorillaVelocityEstimator>().angularVelocity;

            Vector3 HoverboardPos = rig.headMesh.transform.TransformPoint(-0.3f, 0.1f, 0.3725f) + rig.LatestVelocity() * 0.5f;
            Quaternion HoverboardRotation = rig.headMesh.transform.rotation * Quaternion.Euler(angVel * (Mathf.Rad2Deg * 0.1f)) * Quaternion.Euler(0f, 90f, 270f);

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = HoverboardPos - Vector3.up * 0.5f;

            GTPlayer.Instance.SetHoverAllowed(true);

            HoverboardVisual hoverboardVisual = VRRig.LocalRig.hoverboardVisual;

            hoverboardVisual.SetIsHeld(true, hoverboardVisual.NominalParentTransform.InverseTransformPoint(HoverboardPos), hoverboardVisual.NominalParentTransform.InverseTransformRotation(HoverboardRotation), color);
            GTPlayer.Instance.SetHoverActive(false);

            hoverboardVisual.interpolatedLocalPosition = hoverboardVisual.NominalLocalPosition;
            hoverboardVisual.interpolatedLocalRotation = hoverboardVisual.NominalLocalRotation;

            GTPlayer.Instance.SetHoverboardPosRot(HoverboardPos, HoverboardRotation);
        }

        public static void HoverboardScreenGun(Color color)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    HoverboardScreenTarget(lockTarget, color);

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void HoverboardScreenAll(Color color)
        {
            SerializePatch.OverrideSerialization = () => {
                if (PhotonNetwork.InRoom)
                {
                    MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                    Vector3 archivePos = VRRig.LocalRig.transform.position;

                    foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                    {
                        HoverboardScreenTarget(GetVRRigFromPlayer(Player), color);
                        SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                    }

                    RPCProtection();

                    VRRig.LocalRig.enabled = true;

                    VRRig.LocalRig.transform.position = archivePos;

                    return false;
                }

                return true;
            };
        }

        public static void SpawnHoverboard()
        {
            BetaDropBoard(VRRig.LocalRig.transform.position, VRRig.LocalRig.transform.rotation, Vector3.zero, Vector3.zero, RandomColor());
            GTPlayer.Instance.SetHoverAllowed(true);
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
                Vector3 vector3 = new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 2f, 1f, MathF.Sin(offset + (float)Time.frameCount / 30) * 2f);

                offset = -25f;
                Vector3 position2 = new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 2f, 1f, MathF.Sin(offset + (float)Time.frameCount / 30) * 2f);

                BetaDropBoard(GorillaTagger.Instance.headCollider.transform.position + vector3, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - vector3).normalized), (position2 - vector3).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomColor());

                offset = 180f;
                vector3 = new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 2f, 1f, MathF.Sin(offset + (float)Time.frameCount / 30) * 2f);

                offset = 155f;
                position2 = new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 2f, 1f, MathF.Sin(offset + (float)Time.frameCount / 30) * 2f);

                BetaDropBoard(GorillaTagger.Instance.headCollider.transform.position + vector3, Quaternion.Euler((GorillaTagger.Instance.headCollider.transform.position - vector3).normalized), (position2 - vector3).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomColor());
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
                float h = Time.frameCount / 180f % 1f;
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

        public static void RopeGrabReach()
        {
            foreach (GorillaHandClimber climber in new[] { EquipmentInteractor.instance.LeftClimber, EquipmentInteractor.instance.rightClimber })
                (climber.col as SphereCollider).radius = 0.5f;
        }

        public static void DebugSlingshotAimbot()
        {
            if (VRRig.LocalRig.GetSlingshot() == null)
                return;

            if (VRRig.LocalRig.GetSlingshot().InLeftHand() ? leftTrigger > 0.5f : rightTrigger > 0.5f)
                return;

            List<NetPlayer> infected = InfectedList();
            List<VRRig> rigs = GorillaParent.instance.vrrigs
                .Where(rig => !rig.isLocal)
                .Where(rig => !infected.Contains(GetPlayerFromVRRig(rig)))
                .ToList();

            Transform head = GorillaTagger.Instance.headCollider.transform;
            VRRig targetRig = rigs
                .Where(rig => rig != null)
                .Select(rig => new {
                    Rig = rig,
                    ToRig = (rig.transform.position - head.position).normalized,
                    Distance = Vector3.Distance(head.position, rig.transform.position)
                })
                .OrderBy(x => Vector3.Angle(head.forward, x.ToRig)) // only angle matters
                .ThenBy(x => x.Distance) // tiebreaker if multiple at same angle
                .Select(x => x.Rig)
                .FirstOrDefault();

            if (targetRig == null)
                return;

            Visuals.VisualizeAura(targetRig.headMesh.transform.position, 0.1f, Color.green, -91752);
        }

        private static bool lastDrawing;
        public static void AngryBirdsSounds()
        {
            if (!dynamicSounds) return;
            Slingshot slingshot = VRRig.LocalRig.GetSlingshot() as Slingshot;

            if (!slingshot) return;

            if (slingshot.InDrawingState() && !lastDrawing)
                LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/AngryBirds/drawing.ogg", "Audio/Mods/Fun/AngryBirds/drawing.ogg").Play(buttonClickVolume / 10f);

            lastDrawing = slingshot.InDrawingState();
        }

        public static int oldIndex = -1;
        public static void SlingshotSelf(bool active = true)
        {
            try
            {
                var slingshot = VRRig.LocalRig.projectileWeapon;
                if (oldIndex == -1)
                    oldIndex = VRRig.LocalRig.ActiveTransferrableObjectIndex(0);
                if (slingshot == null)
                    slingshot = VRRig.LocalRig.transform.Find("rig/body_pivot/Slingshot Chest Snap/DropZoneAnchor/Slingshot").GetComponent<Slingshot>();
                VRRig.LocalRig.SetActiveTransferrableObjectIndex(0, active ? 212 : oldIndex);
                slingshot.gameObject.SetActive(active);
            }
            catch { }
        }
        public static void SlingshotHelper()
        {
            Slingshot slingshot = VRRig.LocalRig.GetSlingshot() as Slingshot;
            if (slingshot == null)
                return;

            if (slingshot.ForLeftHandSlingshot() ? rightGrab : leftGrab)
                slingshot.itemState = slingshot.ForLeftHandSlingshot() ? TransferrableObject.ItemStates.State2 : TransferrableObject.ItemStates.State3;
        }

        public static LineRenderer paintbrawlTriggerLine;
        public static float triggerBotDelay;
        public static void SlingshotTriggerBot()
        {
            if (paintbrawlTriggerLine != null)
            {
                if (!paintbrawlTriggerLine.gameObject.activeSelf)
                {
                    paintbrawlTriggerLine = null;
                    Object.Destroy(paintbrawlTriggerLine.gameObject);
                }
                else
                    paintbrawlTriggerLine.gameObject.SetActive(false);
            }

            Slingshot localSlingshot = VRRig.LocalRig.GetSlingshot() as Slingshot;
            if (localSlingshot == null || !localSlingshot.InDrawingState())
                return;

            if (paintbrawlTriggerLine == null)
            {
                GameObject lineObject = new GameObject("LineObject");
                paintbrawlTriggerLine = lineObject.AddComponent<LineRenderer>();
                paintbrawlTriggerLine.positionCount = 25;
            }

            paintbrawlTriggerLine.gameObject.SetActive(true);

            paintbrawlTriggerLine.startColor = Color.black;
            paintbrawlTriggerLine.endColor = Color.black;

            Vector3 localPosition = localSlingshot.drawingHand.transform.position + (localSlingshot.centerOrigin.position - localSlingshot.drawingHand.transform.position).normalized * ((EquipmentInteractor.instance.grabRadius - localSlingshot.dummyProjectileColliderRadius) * (localSlingshot.dummyProjectileInitialScale * Mathf.Abs(localSlingshot.transform.lossyScale.x)));
            Vector3 localVelocity = localSlingshot.GetLaunchVelocity();

            Visuals.DrawTrajectory(localPosition, localVelocity, paintbrawlTriggerLine, NoInvisLayerMask(), Vector3.down * 10.79f);

            paintbrawlTriggerLine.enabled = false;

            if (paintbrawlTriggerLine.startColor == Color.green && Time.time > triggerBotDelay)
                triggerBotDelay = Time.time + 0.5f;

            if (Time.time < triggerBotDelay)
            {
                if (localSlingshot.ForLeftHandSlingshot())
                {
                    ControllerInputPoller.instance.rightControllerGripFloat = 0f;
                    ControllerInputPoller.instance.rightGrab = false;
                }
                else
                {
                    ControllerInputPoller.instance.leftControllerGripFloat = 0f;
                    ControllerInputPoller.instance.leftGrab = false;
                }
            }
        }

        public static Coroutine BugCoroutine;
        public static IEnumerator ReturnRig()
        {
            yield return new WaitForSeconds(0.2f);
            VRRig.LocalRig.enabled = true;
            BugCoroutine = null;
        }

        public static ThrowableBug _firefly;
        public static ThrowableBug Firefly
        {
            get
            {
                if (_firefly == null)
                    _firefly = GetAllType<ThrowableBug>().Where(bug => bug.gameObject.activeInHierarchy && bug.gameObject.name == "Floating Bug Holdable").ToArray()[0];

                return _firefly;
            }
        }

        public static float getOwnershipDelay;
        public static ThrowableBug GetBugObject(string name)
        {
            GameObject bugObject;
            bugObject = name == "Firefly" ? Firefly.gameObject : GetObject(name);
            if (bugObject == null)
                return null;

            ThrowableBug bug = bugObject.GetComponent<ThrowableBug>();
            return bug ?? null;
        }

        public static ThrowableBug GetBug(string name)
        {
            ThrowableBug bug = GetBugObject(name);

            if (bug == null)
                return null;

            GameObject bugObject = bug.gameObject;

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
                if (Vector3.SqrMagnitude(bugObject.transform.position - GorillaTagger.Instance.bodyCollider.transform.position) > 15f)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = bugObject.transform.position;

                    if (BugCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(BugCoroutine);

                    BugCoroutine = CoroutineManager.instance.StartCoroutine(ReturnRig());
                }

                if (Vector3.SqrMagnitude(bugObject.transform.position - ServerPos) > 15f)
                    return null;

                if (Time.time < getOwnershipDelay)
                    return null;

                getOwnershipDelay = Time.time + 0.5f;

                NetworkingState guardState = guard.currentState; // Thanks C# 8.0
                Action failureAction = null;
                if ((int)guardState < 3)
                    failureAction = () => guard.currentState = guardState;

                switch (guard.currentState)
                {
                    case NetworkingState.IsOwner:
                        return null;
                    case NetworkingState.IsBlindClient:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        guard.currentState = NetworkingState.RequestingOwnershipWaitingForSight;
                        return null;
                    case NetworkingState.IsClient:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        guard.ownershipRequestNonce = Guid.NewGuid().ToString();
                        guard.currentState = NetworkingState.RequestingOwnership;
                        guard.netView.SendRPC("OwnershipRequested", guard.actualOwner, guard.ownershipRequestNonce);
                        return null;
                    case NetworkingState.ForcefullyTakingOver:
                    case NetworkingState.RequestingOwnership:
                    case NetworkingState.RequestingOwnershipWaitingForSight:
                    case NetworkingState.ForcefullyTakingOverWaitingForSight:
                        guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, failureAction);
                        return null;
                    default:
                        return null;
                }
            }

            if (BugCoroutine != null)
            {
                CoroutineManager.instance.StopCoroutine(BugCoroutine);
                VRRig.LocalRig.enabled = true;
            }

            bug.worldShareableInstance.transferableObjectState = TransferrableObject.PositionState.Dropped;
            return bug;
        }

        private static float bugSpamDelay;
        private static bool bugSpamToggle;
        public static void BugSpam()
        {
            ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");

            if ((!bugObject.IsMyItem() || (bugObject.currentState != TransferrableObject.PositionState.Dropped && bugObject.currentState != TransferrableObject.PositionState.None)) && bugObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(bugObject.GetComponent<ClampPosition>());

            ThrowableBug fireflyObject = GetBugObject("Firefly");

            if ((!fireflyObject.IsMyItem() || (fireflyObject.currentState != TransferrableObject.PositionState.Dropped && fireflyObject.currentState != TransferrableObject.PositionState.None)) && fireflyObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(fireflyObject.GetComponent<ClampPosition>());

            ThrowableBug bug = GetBug("Floating Bug Holdable");
            ThrowableBug firefly = bug != null ? GetBug("Firefly") : bug;

            if (rightGrab)
            {
                if (Time.time > bugSpamDelay)
                {
                    bugSpamToggle = !bugSpamToggle;
                    bugSpamDelay = Time.time + 0.5f;

                    ThrowableBug targetBug = bugSpamToggle ? bug : firefly;
                    
                    GameObject bugSpamObject = new GameObject("iiMenu_BugSpamObject");
                    bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                    bugSpamObject.layer = 3;

                    if (Buttons.GetIndex("Bug Colliders").enabled)
                    {
                        SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                        if (Buttons.GetIndex("Bouncy Bug").enabled)
                        {
                            collider.material.bounciness = 1f;
                            collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                            collider.material.dynamicFriction = 0f;
                        }
                    }

                    bugSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                    bugSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                    Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                    rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                    rigidbody.angularVelocity = RandomVector3(100f);

                    rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                    targetBug.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                    bugSpamObject.AddComponent<DestroyOnRest>();
                    Object.Destroy(bugSpamObject, 30f);
                }
            }
        }

        private static float cameraSpamDelay;
        private static bool cameraSpamType;
        public static void CameraSpam()
        {
            if (rightGrab && Time.time > cameraSpamDelay)
            {
                cameraSpamDelay = Time.time + 0.25f;
                cameraSpamType = !cameraSpamType;

                LckSocialCamera camera = cameraSpamType ? LckSocialCameraManager.Instance._socialCameraCococamInstance : LckSocialCameraManager.Instance._socialCameraTabletInstance;

                GameObject cameraSpamObject = new GameObject("iiMenu_CameraSpamObject");
                cameraSpamObject.transform.localScale = Vector3.one * 0.2f;
                cameraSpamObject.layer = 3;

                if (Buttons.GetIndex("Bug Colliders").enabled)
                {
                    SphereCollider collider = cameraSpamObject.AddComponent<SphereCollider>();

                    if (Buttons.GetIndex("Bouncy Bug").enabled)
                    {
                        collider.material.bounciness = 1f;
                        collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                        collider.material.dynamicFriction = 0f;
                    }
                }

                cameraSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                cameraSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                Rigidbody rigidbody = cameraSpamObject.AddComponent<Rigidbody>();
                rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                rigidbody.angularVelocity = RandomVector3(100f);

                rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                camera.visible = true;
                camera.recording = true;

                camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                camera.m_CameraVisuals.SetRecordingState(true);

                camera.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = cameraSpamObject.transform;

                cameraSpamObject.AddComponent<DestroyOnRest>();
                Object.Destroy(cameraSpamObject, 30f);
            }
        }

        private static int objectIndex;
        private static float everythingSpamDelay;
        public static void EverythingSpam()
        {
            ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");

            if ((!bugObject.IsMyItem() || (bugObject.currentState != TransferrableObject.PositionState.Dropped && bugObject.currentState != TransferrableObject.PositionState.None)) && bugObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(bugObject.GetComponent<ClampPosition>());

            ThrowableBug fireflyObject = GetBugObject("Firefly");

            if ((!fireflyObject.IsMyItem() || (fireflyObject.currentState != TransferrableObject.PositionState.Dropped && fireflyObject.currentState != TransferrableObject.PositionState.None)) && fireflyObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(fireflyObject.GetComponent<ClampPosition>());

            ThrowableBug bug = GetBug("Floating Bug Holdable");
            ThrowableBug firefly = bug != null ? GetBug("Firefly") : bug;

            string projectileName = Projectiles.ProjectileObjectNames[Projectiles.projMode * 2];
            
            if (rightGrab && Time.time > everythingSpamDelay)
            {
                SnowballThrowable projectile = GetProjectile(projectileName);
                projectile.SetSnowballActiveLocal(true);
                CoroutineManager.instance.StartCoroutine(Projectiles.DisableProjectile(projectile));

                if (Overpowered.DisableCoroutine != null)
                    CoroutineManager.instance.StopCoroutine(Overpowered.DisableCoroutine);

                Overpowered.DisableCoroutine = CoroutineManager.instance.StartCoroutine(Overpowered.DisableSnowball(false));
                GetProjectile($"{Projectiles.SnowballName}RightAnchor").SetSnowballActiveLocal(true);

                everythingSpamDelay = Time.time + 0.0625f;

                objectIndex++;
                objectIndex %= 8; 

                switch (objectIndex)
                {
                    case 0:
                        {
                            ThrowableBug targetBug = bug;
                            GameObject bugSpamObject = new GameObject("iiMenu_BugSpamObject");
                            bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                            bugSpamObject.layer = 3;

                            if (Buttons.GetIndex("Bug Colliders").enabled)
                            {
                                SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                                if (Buttons.GetIndex("Bouncy Bug").enabled)
                                {
                                    collider.material.bounciness = 1f;
                                    collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                                    collider.material.dynamicFriction = 0f;
                                }
                            }

                            bugSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                            bugSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                            Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                            rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                            rigidbody.angularVelocity = RandomVector3(100f);

                            rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                            targetBug.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                            bugSpamObject.AddComponent<DestroyOnRest>();
                            Object.Destroy(bugSpamObject, 30f);
                            break;
                        }
                    case 1:
                        {
                            ThrowableBug targetBug = firefly;
                            GameObject bugSpamObject = new GameObject("iiMenu_FireflySpamObject");
                            bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                            bugSpamObject.layer = 3;

                            if (Buttons.GetIndex("Bug Colliders").enabled)
                            {
                                SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                                if (Buttons.GetIndex("Bouncy Bug").enabled)
                                {
                                    collider.material.bounciness = 1f;
                                    collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                                    collider.material.dynamicFriction = 0f;
                                }
                            }

                            bugSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                            bugSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                            Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                            rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                            rigidbody.angularVelocity = RandomVector3(100f);

                            rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                            targetBug.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                            bugSpamObject.AddComponent<DestroyOnRest>();
                            Object.Destroy(bugSpamObject, 30f);
                            break;
                        }
                    case 2:
                        {
                            if (!PhotonNetwork.InRoom)
                                break;

                            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;

                            GameObject cameraSpamObject = new GameObject("iiMenu_CameraSpamObject");
                            cameraSpamObject.transform.localScale = Vector3.one * 0.2f;
                            cameraSpamObject.layer = 3;

                            if (Buttons.GetIndex("Bug Colliders").enabled)
                            {
                                SphereCollider collider = cameraSpamObject.AddComponent<SphereCollider>();

                                if (Buttons.GetIndex("Bouncy Bug").enabled)
                                {
                                    collider.material.bounciness = 1f;
                                    collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                                    collider.material.dynamicFriction = 0f;
                                }
                            }

                            cameraSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                            cameraSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                            Rigidbody rigidbody = cameraSpamObject.AddComponent<Rigidbody>();
                            rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                            rigidbody.angularVelocity = RandomVector3(100f);

                            rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                            camera.visible = true;
                            camera.recording = true;

                            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                            camera.m_CameraVisuals.SetRecordingState(true);

                            camera.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = cameraSpamObject.transform;

                            cameraSpamObject.AddComponent<DestroyOnRest>();
                            Object.Destroy(cameraSpamObject, 30f);
                            break;
                        }
                    case 3:
                        {
                            if (!PhotonNetwork.InRoom)
                                break;

                            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;

                            GameObject cameraSpamObject = new GameObject("iiMenu_CameraSpamObject");
                            cameraSpamObject.transform.localScale = Vector3.one * 0.2f;
                            cameraSpamObject.layer = 3;

                            if (Buttons.GetIndex("Bug Colliders").enabled)
                            {
                                SphereCollider collider = cameraSpamObject.AddComponent<SphereCollider>();

                                if (Buttons.GetIndex("Bouncy Bug").enabled)
                                {
                                    collider.material.bounciness = 1f;
                                    collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                                    collider.material.dynamicFriction = 0f;
                                }
                            }

                            cameraSpamObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
                            cameraSpamObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                            Rigidbody rigidbody = cameraSpamObject.AddComponent<Rigidbody>();
                            rigidbody.linearVelocity = GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                            rigidbody.angularVelocity = RandomVector3(100f);

                            rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                            camera.visible = true;
                            camera.recording = true;

                            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                            camera.m_CameraVisuals.SetRecordingState(true);

                            camera.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = cameraSpamObject.transform;

                            cameraSpamObject.AddComponent<DestroyOnRest>();
                            Object.Destroy(cameraSpamObject, 30f);
                            break;
                        }
                    case 4:
                    case 5:
                        BetaDropBoard(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength, Vector3.zero, RandomColor());
                        break;
                    case 6:
                        Projectiles.BetaFireProjectile(projectileName, GorillaTagger.Instance.rightHandTransform.position, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength, RandomColor());
                        break;
                    case 7:
                        Overpowered.BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength, 0);
                        break;
                }
            }
        }

        public static void DisableCameraSpam()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;

            if (camera.GetComponent<ClampPosition>() != null)
                Object.Destroy(camera.GetComponent<ClampPosition>());
        }

        public static void DisableEverythingSpam()
        {
            DisableBugSpam();
            DisableCameraSpam();
        }

        public static void DisableBugSpam()
        {
            ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");

            if (bugObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(bugObject.GetComponent<ClampPosition>());

            ThrowableBug fireflyObject = GetBugObject("Firefly");

            if (fireflyObject.GetComponent<ClampPosition>() != null)
                Object.Destroy(fireflyObject.GetComponent<ClampPosition>());
        }

        public static void BugPhallus()
        {
            ThrowableBug Bug = GetBug("Floating Bug Holdable");
            ThrowableBug Firefly = GetBug("Firefly");

            if (Bug != null && Firefly != null)
            {
                Bug.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.TransformDirection(new Vector3(0f, -0.22f, 0.123f));
                Firefly.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.TransformDirection(new Vector3(0f, -0.22f, 0.24f));

                Bug.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0, 270, 0);
                Firefly.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0, 90, 0);
            }
        }

        public static void BugPhallusGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    ThrowableBug Bug = GetBug("Floating Bug Holdable");
                    ThrowableBug Firefly = GetBug("Firefly");

                    if (Bug != null && Firefly != null)
                    {
                        Bug.transform.position = lockTarget.transform.position + lockTarget.transform.TransformDirection(new Vector3(0f, -0.4f, 0.123f));
                        Firefly.transform.position = lockTarget.transform.position + lockTarget.transform.TransformDirection(new Vector3(0f, -0.4f, 0.24f));

                        Bug.transform.rotation = lockTarget.transform.rotation * Quaternion.Euler(0, 270, 0);
                        Firefly.transform.rotation = lockTarget.transform.rotation * Quaternion.Euler(0, 90, 0);
                    }
                }
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void BugVibrateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    ThrowableBug Bug = GetBug("Floating Bug Holdable");
                    ThrowableBug Firefly = Bug != null ? GetBug("Firefly") : null;

                    if (Bug != null)
                    {
                        Bug.transform.position = lockTarget.leftHandTransform.position;
                        Bug.transform.rotation = RandomQuaternion();
                    }

                    if (Firefly != null)
                    {
                        Firefly.transform.position = lockTarget.rightHandTransform.position;
                        Firefly.transform.rotation = RandomQuaternion();
                    }
                }
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

        public static void BugVibrateAll()
        {
            ThrowableBug Bug = GetBug("Floating Bug Holdable");
            if (Bug != null)
                GetBug("Firefly");
        }

        public static void EnableBugVibrateAll()
        {
            SerializePatch.OverrideSerialization = () =>
            {
                ThrowableBug Bug = GetBug("Floating Bug Holdable");
                ThrowableBug Firefly = Bug != null ? GetBug("Firefly") : Bug;

                PhotonView BugView = Bug?.reliableState?.gameObject != null
                    ? Bug.reliableState.gameObject.GetComponent<GorillaNetworkTransform>().punView
                    : null;

                PhotonView FireflyView = Firefly?.reliableState?.gameObject != null
                    ? Firefly.reliableState.gameObject.GetComponent<GorillaNetworkTransform>().punView
                    : null;

                if (BugView == null || FireflyView == null)
                    return true;

                MassSerialize(true, new[] { BugView, FireflyView }.Where(v => v != null).ToArray());

                Vector3 bugArchivePosition = Bug.reliableState.gameObject?.transform?.position ?? Vector3.zero;
                Quaternion bugArchiveRotation = Bug.reliableState.gameObject?.transform?.rotation ?? Quaternion.identity;

                Vector3 fireflyArchivePosition = Firefly.reliableState.gameObject?.transform?.position ?? Vector3.zero;
                Quaternion fireflyArchiveRotation = Firefly.reliableState.gameObject?.transform?.rotation ?? Quaternion.identity;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);
                    if (targetRig == null)
                        continue;

                    if (Bug != null && Bug.transform != null && targetRig.leftHandTransform != null && BugView != null)
                    {
                        Bug.reliableState.gameObject.transform.position = targetRig.leftHandTransform.position;
                        Bug.reliableState.gameObject.transform.rotation = RandomQuaternion();
                        SendSerialize(BugView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                    }

                    if (Firefly != null && Firefly.transform != null && targetRig.rightHandTransform != null && FireflyView != null)
                    {
                        Firefly.reliableState.gameObject.transform.position = targetRig.rightHandTransform.position;
                        Firefly.reliableState.gameObject.transform.rotation = RandomQuaternion();
                        SendSerialize(FireflyView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                    }
                }

                if (Bug?.transform != null)
                {
                    Bug.reliableState.gameObject.transform.position = bugArchivePosition;
                    Bug.reliableState.gameObject.transform.rotation = bugArchiveRotation;
                }

                if (Firefly?.transform != null)
                {
                    Firefly.reliableState.gameObject.transform.position = fireflyArchivePosition;
                    Firefly.reliableState.gameObject.transform.rotation = fireflyArchiveRotation;
                }

                RPCProtection();
                return false;
            };
        }

        public static void HolsterObject(string objectName, TransferrableObject.PositionState state)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null && (bug.currentState == TransferrableObject.PositionState.Dropped || bug.currentState == TransferrableObject.PositionState.None))
                bug.currentState = state;
        }

        public static void FreezeObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
            {
                bug.bugRotationalVelocity = Quaternion.identity;
                bug.targetVelocity = Vector3.zero;
                bug.thrownVeloicity = Vector3.zero;
                bug.thrownYVelocity = 0f;
                bug.reliableState.travelingDirection = Vector3.zero;
            }
        }

        public static void SetObjectSpeed(string objectName, float speed = 1f)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.maxNaturalSpeed = speed;
        }

        private static readonly Dictionary<string, bool> lastInAirValues = new Dictionary<string, bool>();
        public static void PhysicalObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
            {
                GorillaVelocityTracker tracker = bug.gameObject.GetOrAddComponent<GorillaVelocityTracker>();

                if ((bug.currentState == TransferrableObject.PositionState.InLeftHand || bug.currentState == TransferrableObject.PositionState.InRightHand) && bug.GetComponent<ClampPosition>() != null)
                    Object.Destroy(bug.GetComponent<ClampPosition>());

                bool inAir = bug.currentState == TransferrableObject.PositionState.None || bug.currentState == TransferrableObject.PositionState.Dropped;
                lastInAirValues.TryGetValue(objectName, out bool lastInAir);

                if (inAir && !lastInAir)
                {
                    GameObject bugSpamObject = new GameObject("iiMenu_BugSpamObject");
                    bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                    bugSpamObject.layer = 3;

                    if (Buttons.GetIndex("Bug Colliders").enabled)
                    {
                        SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                        if (Buttons.GetIndex("Bouncy Bug").enabled)
                        {
                            collider.material.bounciness = 1f;
                            collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                            collider.material.dynamicFriction = 0f;
                        }
                    }

                    bugSpamObject.transform.position = bug.transform.position;
                    bugSpamObject.transform.rotation = bug.transform.rotation;

                    Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                    rigidbody.linearVelocity = tracker.GetAverageVelocity(true, 0);
                    rigidbody.angularVelocity = bug.velocityEstimator.angularVelocity;

                    rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                    bug.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                    bugSpamObject.AddComponent<DestroyOnRest>();
                    Object.Destroy(bugSpamObject, 30f);
                }

                lastInAirValues[objectName] = inAir;
            }
        }

        private static bool grabbingCamera;
        private static bool grabbingHand;

        public static void PhysicalCamera()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;

            if (!camera.visible)
            {
                camera.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                camera.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                camera.visible = true;
                camera.recording = true;

                camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                camera.m_CameraVisuals.SetRecordingState(true);
            }

            if (!grabbingCamera)
            {
                bool canGrabLeft = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, camera.transform.position) < 0.3f && leftGrab;
                bool canGrabRight = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, camera.transform.position) < 0.3f && rightGrab;

                if (canGrabLeft || canGrabRight)
                {
                    grabbingCamera = true;
                    grabbingHand = canGrabLeft;
                }
            } else
            {
                Transform targetTransform = grabbingHand ?
                    GorillaTagger.Instance.leftHandTransform :
                    GorillaTagger.Instance.rightHandTransform;

                camera.transform.position = targetTransform.position;
                camera.transform.rotation = targetTransform.rotation;

                GorillaVelocityTracker tracker = camera.gameObject.GetOrAddComponent<GorillaVelocityTracker>();
                GorillaVelocityEstimator estimator = camera.gameObject.GetOrAddComponent<GorillaVelocityEstimator>();

                if (grabbingHand ? !leftGrab : !rightGrab)
                {
                    grabbingCamera = false;

                    GameObject bugSpamObject = new GameObject("iiMenu_BugSpamObject");
                    bugSpamObject.transform.localScale = Vector3.one * 0.2f;
                    bugSpamObject.layer = 3;

                    if (Buttons.GetIndex("Bug Colliders").enabled)
                    {
                        SphereCollider collider = bugSpamObject.AddComponent<SphereCollider>();

                        if (Buttons.GetIndex("Bouncy Bug").enabled)
                        {
                            collider.material.bounciness = 1f;
                            collider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
                            collider.material.dynamicFriction = 0f;
                        }
                    }

                    bugSpamObject.transform.position = camera.transform.position;
                    bugSpamObject.transform.rotation = camera.transform.rotation;

                    Rigidbody rigidbody = bugSpamObject.AddComponent<Rigidbody>();
                    rigidbody.linearVelocity = tracker.GetAverageVelocity(true, 0);
                    rigidbody.angularVelocity = estimator.angularVelocity;

                    rigidbody.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;

                    camera.gameObject.GetOrAddComponent<ClampPosition>().targetTransform = bugSpamObject.transform;

                    bugSpamObject.AddComponent<DestroyOnRest>();
                    Object.Destroy(bugSpamObject, 30f);
                }
            }
        }

        public static void ObjectToHand(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (rightGrab && bug != null)
            {
                bug.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                bug.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
        }

        public static void GrabCamera()
        {
            if (rightGrab)
            {
                LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
                camera.visible = true;
                camera.recording = true;

                camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                camera.m_CameraVisuals.SetRecordingState(true);

                camera.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                camera.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
        }

        public static void GrabTablet()
        {
            if (rightGrab)
            {
                LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
                camera.visible = true;
                camera.recording = true;

                camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                camera.m_CameraVisuals.SetRecordingState(true);

                camera.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                camera.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
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
                        glider.gameObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    }
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
                RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0, null, false, false, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength);
                RPCProtection();
            }
        }

        public static void DestroyObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.transform.position = new Vector3(99999f, 99999f, 99999f);
        }

        public static void DestroyCamera()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
            camera.visible = false;
            camera.recording = false;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(false);
            camera.m_CameraVisuals.SetRecordingState(false);
        }

        public static void DestroyTablet()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
            camera.visible = false;
            camera.recording = false;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(false);
            camera.m_CameraVisuals.SetRecordingState(false);
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
                delayer = Time.time + 0.3f;

                BuilderPiece[] totalPieces = PhotonNetwork.IsMasterClient ? GetAllType<BuilderPiece>() :
                    GetAllType<BuilderPiece>()
                        .Where(piece => piece.gameObject.activeInHierarchy)
                        .Where(piece => Vector3.Distance(piece.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 2.5f).ToArray();

                for (int i = 0; i < 100; i++)
                {
                    BuilderPiece piece = totalPieces[Random.Range(0, totalPieces.Length)];
                    if (piece.gameObject.activeSelf)
                        RequestRecyclePiece(piece, true, 2);
                }
            }
        }

        public static void SaveBuilderTableData()
        {
            string fileName = $"{PluginInfo.BaseDirectory}/BuilderTableData.json";

            File.WriteAllText(fileName, ManagerRegistry.BuilderTable.WriteTableToJson());

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
        }

        public static void LoadBuilderTableData()
        {
            string fileName = $"{PluginInfo.BaseDirectory}/BuilderTableData.json";

            if (!File.Exists(fileName))
                return;

            string tableData = File.ReadAllText(fileName);
            BuilderTable table = ManagerRegistry.BuilderTable;

            table.tableData ??= new BuilderTableData();

            table.SetIsDirty(false);
            table.tableData.numEdits++;

            tableData = Convert.ToBase64String(GZipStream.CompressString(tableData));
            SharedBlocksManager.instance.OnSavePrivateScanSuccess += table.OnSaveScanSuccess;
            SharedBlocksManager.instance.OnSavePrivateScanFailed += table.OnSaveScanFailure;
            SharedBlocksManager.instance.RequestSavePrivateScan(table.currentSaveSlot, tableData);
        }

        public static Coroutine DisableThrowableCoroutine;
        public static IEnumerator DisableThrowable(int index)
        {
            yield return new WaitForSeconds(0.3f);

            DistancePatch.enabled = false;
            VRRig.LocalRig.enabled = true;
            
            GameObject proj = VRRig.LocalRig.myBodyDockPositions.allObjects[index].gameObject;
            proj.SetActive(true);

            VRRig.LocalRig.myBodyDockPositions.allObjects[index].storedZone = BodyDockPositions.DropPositions.RightArm;
            VRRig.LocalRig.myBodyDockPositions.allObjects[index].currentState = TransferrableObject.PositionState.OnRightArm;
        }

        private static void EquipCosmetic(string cosmeticName)
        {
            CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.currentWornSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, CosmeticsController.instance.GetItemFromDict(cosmeticName), true, false);
            CosmeticsController.instance.UpdateWornCosmetics(PhotonNetwork.InRoom);
            RPCProtection();
        }

        public static void CheckOwnedCosmetic(string cosmeticName)
        {
            if (Time.frameCount == Settings.loadingPreferencesFrame)
                return;

            CosmeticsController.CosmeticItem cosmetic = CosmeticsController.instance.GetItemFromDict(cosmeticName);
            if (!CosmeticsOwned.Contains(cosmeticName))
            {
                if (!cosmetic.canTryOn)
                {
                    PromptSingle($"Looks like you don't own the cosmetic required for this mod ({ToTitleCase(cosmetic.overrideDisplayName)}), but this cosmetic is currently offsale. This mod will only work for people with cosmetic giving mods.", null, "Ok");
                } else if (CosmeticsController.instance.CurrencyBalance >= cosmetic.cost)
                    Prompt($"Looks like you don't own the cosmetic required for this mod ({ToTitleCase(cosmetic.overrideDisplayName)}), meaning it will only work in city. Would you like to purchase the cosmetic? ({cosmetic.cost}SR)", () => PurchaseCosmetic(cosmetic.itemName));
                else
                    PromptSingle($"Looks like you don't own the cosmetic required for this mod ({ToTitleCase(cosmetic.overrideDisplayName)}), meaning it will only work in city.", null, "Ok");
            }
        }

        public static void CheckOwnedThrowable(int index)
        {
            if (Time.frameCount == Settings.loadingPreferencesFrame)
                return;

            string cosmeticName = VRRig.LocalRig.myBodyDockPositions.allObjects[index].gameObject.name;
            CheckOwnedCosmetic(cosmeticName);
        }

        public static void FireSoundSpam()
        {
            if (rightTrigger > 0.5f)
                EquipCosmetic("LBALH.");
        }

        public static void BubblerGun(int index, Quaternion handRotation, float handOffset = 0.5f)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun(GTPlayer.Instance.locomotionEnabledLayers);
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    DisableThrowableCoroutine = CoroutineManager.instance.StartCoroutine(DisableThrowable(index));
                    TransferrableObject transferrableObject = VRRig.LocalRig.myBodyDockPositions.allObjects[index];

                    if (!CosmeticsOwned.Contains(transferrableObject.gameObject.name))
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = TryOnRoom.transform.position;
                    }

                    if (!transferrableObject.gameObject.activeSelf)
                    {
                        VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, index);
                        transferrableObject.gameObject.SetActive(true);
                    }

                    transferrableObject.storedZone = BodyDockPositions.DropPositions.RightArm;
                    transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;

                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position - Vector3.up * 0.5f;

                    VRRig.LocalRig.rightHand.rigTarget.transform.position = NewPointer.transform.position + Vector3.up * handOffset;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = handRotation;

                    VRRig.LocalRig.rightIndex.calcT = 1f;
                    VRRig.LocalRig.rightMiddle.calcT = 1f;

                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                }
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void WhiteColorTarget(VRRig rig)
        {
            int index = 629;
            DisableThrowableCoroutine = CoroutineManager.instance.StartCoroutine(DisableThrowable(index));
            TransferrableObject transferrableObject = VRRig.LocalRig.myBodyDockPositions.allObjects[index];

            if (!CosmeticsOwned.Contains(transferrableObject.gameObject.name))
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = TryOnRoom.transform.position;
            }

            if (!transferrableObject.gameObject.activeSelf)
            {
                VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, index);
                transferrableObject.gameObject.SetActive(true);
            }

            transferrableObject.storedZone = BodyDockPositions.DropPositions.RightArm;
            transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = rig.transform.position - Vector3.up;

            VRRig.LocalRig.rightHand.rigTarget.transform.position = rig.transform.position;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

            VRRig.LocalRig.rightIndex.calcT = 1f;
            VRRig.LocalRig.rightMiddle.calcT = 1f;

            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
        }

        public static void WhiteColorGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    WhiteColorTarget(lockTarget);

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void BlackColorTarget(VRRig rig) =>
            SendThrowableProjectile(600, rig.transform.position, Vector3.zero, Quaternion.identity);

        public static void BlackColorGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    BlackColorTarget(lockTarget);

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void ChickenTarget(VRRig rig)
        {
            int index = 651;
            DisableThrowableCoroutine = CoroutineManager.instance.StartCoroutine(DisableThrowable(index));
            TransferrableObject transferrableObject = VRRig.LocalRig.myBodyDockPositions.allObjects[index];

            if (!CosmeticsOwned.Contains(transferrableObject.gameObject.name))
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = TryOnRoom.transform.position;
            }

            if (!transferrableObject.gameObject.activeSelf)
                EquipCosmetic("LMAQL.");

            transferrableObject.storedZone = BodyDockPositions.DropPositions.RightBack;
            transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = rig.transform.position;

            VRRig.LocalRig.rightHand.rigTarget.transform.position = rig.transform.position + Vector3.up * (Time.time * 5f % 1f - 0.5f);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.identity;

            VRRig.LocalRig.rightIndex.calcT = 1f;
            VRRig.LocalRig.rightMiddle.calcT = 1f;

            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
        }

        public static void ChickenGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    ChickenTarget(lockTarget);

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        private static float throwableProjectileTimeout;
        public static void SendThrowableProjectile(int index, Vector3 pos, Vector3 vel, Quaternion rot)
        {
            DistancePatch.enabled = true;

            if (DisableThrowableCoroutine != null)
                CoroutineManager.instance.StopCoroutine(DisableThrowableCoroutine);

            DisableThrowableCoroutine = CoroutineManager.instance.StartCoroutine(DisableThrowable(index));
            TransferrableObject transferrableObject = VRRig.LocalRig.myBodyDockPositions.allObjects[index];

            if (!CosmeticsOwned.Contains(transferrableObject.gameObject.name))
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = TryOnRoom.transform.position;
            }

            if (!transferrableObject.gameObject.activeSelf)
            {
                VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, index);
                transferrableObject.gameObject.SetActive(true);
            }

            transferrableObject.storedZone = BodyDockPositions.DropPositions.RightArm;
            transferrableObject.currentState = TransferrableObject.PositionState.InRightHand;

            if (Time.time > throwableProjectileTimeout)
            {
                ThrowableHoldableCosmetic projectile = transferrableObject.gameObject.GetComponent<ThrowableHoldableCosmetic>();
                throwableProjectileTimeout = Time.time + 0.31f;

                Vector3 archivePosition = VRRig.LocalRig.transform.position;
                VRRig.LocalRig.transform.position = pos;

                SendSerialize(GorillaTagger.Instance.myVRRig.GetView, null, -10);
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView);

                projectile._events.Activate.RaiseAll(pos, rot, vel, 1f);

                VRRig.LocalRig.transform.position = archivePosition;
                SendSerialize(GorillaTagger.Instance.myVRRig.GetView);

                RPCProtection();
            }
        }

        public static void ThrowableProjectileSpam(int projectileIndex)
        {
            if (rightGrab)
                SendThrowableProjectile(projectileIndex, GorillaTagger.Instance.rightHandTransform.position, Vector3.zero, RandomQuaternion());
        }

        public static void ThrowableProjectileMinigun(int projectileIndex)
        {
            if (rightGrab)
                SendThrowableProjectile(projectileIndex, GorillaTagger.Instance.rightHandTransform.position, GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength, RandomQuaternion());
        }

        public static void ThrowableProjectileGun(int projectileIndex)
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                    SendThrowableProjectile(projectileIndex, NewPointer.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0f, 0f), RandomQuaternion());
            }
        }

        private static float startTimeBuilding;
        public static void EnableAtticAntiReport() =>
            startTimeBuilding = Time.time + 5f;

        public static void AtticAntiReport()
        {
            if (Time.time > startTimeBuilding)
                Buttons.GetIndex("Attic Anti Report").enabled = false;

            foreach (var line in GorillaScoreboardTotalUpdater.allScoreboardLines.Where(line => line.linePlayer == NetworkSystem.Instance.LocalPlayer))
            {
                RequestCreatePiece(-566818631, line.reportButton.transform.position + RandomVector3(0.3f), RandomQuaternion(), 0, null, true);
                RPCProtection();
            }
        }

        public static void AtticDrawGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                        CoroutineManager.instance.StartCoroutine(DrawSmallDelay(NewPointer.transform.position));
                }
            }
        }

        public static void AtticBuildGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                    {
                        RequestCreatePiece(pieceIdSet, NewPointer.transform.position, RandomQuaternion(), 0, null, true);
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
            Object.Destroy(Temporary.GetComponent<Collider>());
            yield return new WaitForSeconds(0.5f);
            RequestCreatePiece(pieceIdSet, Temporary.transform.position, RandomQuaternion(), 0, null, true);
            Object.Destroy(Temporary);
            RPCProtection();
        }

        public static void AtticFreezeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
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

        public static void AtticFreezeAll()
        {
            if (rightTrigger > 0.5f)
            {
                Player target = GetRandomPlayer(false);

                if (!PhotonNetwork.IsMasterClient)
                    NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                else
                {
                    GetVRRigFromPlayer(target);
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

                if (gunLocked && lockTarget != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                    {
                        floatPower += (0.3f - floatPower) * 0.05f;
                        RequestCreatePiece(-566818631, lockTarget.transform.position + Vector3.down * floatPower, Quaternion.Euler(0f, Random.Range(0f, 350f), 0f), 0, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)), true);
                        RPCProtection();
                    }
                }
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
                floatPower = 0.35f;
                if (gunLocked)
                    gunLocked = false;
            }
        }

		public static void AtticFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                    else
                    {
                        RequestCreatePiece(-566818631, lockTarget.transform.position + Vector3.down * 0.35f, Quaternion.Euler(0f, Random.Range(0f, 350f), 0f), 0, NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)), false, true, Vector3.up * 50f);
                        RPCProtection();
                    }
                }
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
                GameObject NewPointer = GunData.NewPointer;

                if (position != Vector3.zero)
                {
                    RequestCreatePiece(pieceIdSet, position, Quaternion.Euler(0f, Time.frameCount % 360, 0f), 0, null, true);
                    RPCProtection();

                    position += new Vector3(0f, 0.1f, 0f);
                }

                if (GetGunInput(true))
                    position = NewPointer.transform.position;
            }
            else
                position = Vector3.zero;
        }

        private static bool isFiring;
        public static IEnumerator FireShotgun()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                yield break;
            }

            isFiring = true;

            if (!File.Exists($"{PluginInfo.BaseDirectory}/shotgun.wav"))
                LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/shotgun.ogg", "Audio/Mods/Fun/shotgun.ogg");

            Sound.PlayAudio("shotgun.wav");

            BuilderPiece bullet = null;

            yield return CreateGetPiece(1925587737, piece => bullet = piece);
            while (bullet == null)
                yield return null;

            RequestGrabPiece(bullet, true, Vector3.zero, Quaternion.identity);
            yield return null;
            RequestDropPiece(bullet, ControllerUtilities.GetTrueRightHand().position + ControllerUtilities.GetTrueRightHand().forward * 0.65f + ControllerUtilities.GetTrueRightHand().right * 0.03f + ControllerUtilities.GetTrueRightHand().up * 0.05f, ControllerUtilities.GetTrueRightHand().rotation, ControllerUtilities.GetTrueRightHand().forward * 19.9f, Vector3.zero);
            yield return null;
        }

        public static void UnlimitedBuilding()
        {
            BuilderPieceInteractor.instance.maxHoldablePieceStackCount = int.MaxValue;
            UnlimitPatches.enabled = true;
        }

        public static void DisableUnlimitedBuilding()
        {
            BuilderPieceInteractor.instance.maxHoldablePieceStackCount = 50;
            UnlimitPatches.enabled = false;
        }

        public static void PlaceBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    BuilderPiece gunTarget = Ray.collider.GetComponentInParent<BuilderPiece>();
                    if (gunTarget)
                    {
                        RequestPlacePiece(PlacePatch._piece, gunTarget, PlacePatch._bumpOffsetX, PlacePatch._bumpOffsetZ, PlacePatch._twist, PlacePatch._parentPiece, PlacePatch._attachIndex, PlacePatch._parentAttachIndex);
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
            Buttons.GetIndex("Change Block Delay").overlapText = "Change Block Delay <color=grey>[</color><color=green>" + blockDebounce + "</color><color=grey>]</color>";
        }

        public static void RequestCreatePiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, object target = null, bool overrideFreeze = false, bool forceGravity = false, Vector3? velocity = null, Vector3? angVelocity = null)
        {
            if (Buttons.GetIndex("Random Block Type").enabled)
                pieceType = GetRandomBlockType();

            BuilderTable table = ManagerRegistry.BuilderTable;
            BuilderTableNetworking Networking = table.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                if (Time.time > blockDelay)
                {
                    int pieceId = table.CreatePieceId();

                    object[] args = {
                        pieceType,
                        pieceId,
                        BitPackUtils.PackWorldPosForNetwork(position),
                        BitPackUtils.PackQuaternionForNetwork(rotation),
                        materialType,
                        (byte)4,
                        1,
                        PhotonNetwork.LocalPlayer
                    };

                    switch (target)
                    {
                        case RpcTarget rpcTargetCreate:
                            Networking.photonView.RPC("PieceCreatedByShelfRPC", rpcTargetCreate, args);
                            break;
                        case Player playerCreate:
                            Networking.photonView.RPC("PieceCreatedByShelfRPC", playerCreate, args);
                            break;
                        default:
                            Networking.photonView.RPC("PieceCreatedByShelfRPC", RpcTarget.All, args);
                            break;
                    }

                    if ((!overrideFreeze && !Buttons.GetIndex("Zero Gravity Blocks").enabled) || forceGravity)
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

                        switch (target)
                        {
                            case RpcTarget rpcTargetGrab:
                                Networking.photonView.RPC("PieceGrabbedRPC", rpcTargetGrab, args);
                                break;
                            case Player playerGrab:
                                Networking.photonView.RPC("PieceGrabbedRPC", playerGrab, args);
                                break;
                            default:
                                Networking.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, args);
                                break;
                        }

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

                        switch (target)
                        {
                            case RpcTarget rpcTargetDrop:
                                Networking.photonView.RPC("PieceDroppedRPC", rpcTargetDrop, args);
                                break;
                            case Player playerDrop:
                                Networking.photonView.RPC("PieceDroppedRPC", playerDrop, args);
                                break;
                            default:
                                Networking.photonView.RPC("PieceDroppedRPC", RpcTarget.All, args);
                                break;
                        }
                    }
                }
            }
            else
            {
                if (Time.time > blockDelay)
                {
                    blockDelay = Time.time + blockDebounce;
                    BuilderPiece piece = (GetAllType<BuilderPiece>()
                        .Where(piece => piece.gameObject.activeInHierarchy)
                        .Where(piece => piece.pieceType == pieceType)
                        .Where(piece => !piece.isBuiltIntoTable)
                        .Where(piece => piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, piece.transform.position))
                        .Where(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos) < 2.5f)
                        .OrderBy(piece => Vector3.Distance(piece.transform.position, ServerLeftHandPos))
                        .FirstOrDefault()
                        ?? null) ?? GetAllType<BuilderPiece>()
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
                        position = ServerLeftHandPos + (position - ServerLeftHandPos).normalized * 2.5f;

                    pieceId = piece.pieceId;

                    Networking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                    Networking.RequestDropPiece(piece, position, rotation, velocity ?? Vector3.zero, angVelocity ?? Vector3.zero);
                }
            }
        }

        public static void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
        {
            BuilderTableNetworking Networking = ManagerRegistry.BuilderTable.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, Networking.CreateLocalCommandId(), piece.pieceId, isLefHand, BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation), PhotonNetwork.LocalPlayer);
            } 
            else
                Networking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
        }

        public static void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
        {
            if (attachPiece.isBuiltIntoTable)
                return;

            BuilderTableNetworking Networking = ManagerRegistry.BuilderTable.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PiecePlacedRPC", RpcTarget.All, Networking.CreateLocalCommandId(), piece.pieceId, attachPiece != null ? attachPiece.pieceId : -1, BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ), parentPiece != null ? parentPiece.pieceId : -1, attachIndex, parentAttachIndex, PhotonNetwork.LocalPlayer, PhotonNetwork.ServerTimestamp);
            }
            else
            {
                Networking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
                Networking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
            }
        }

        public static void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
        {
            BuilderTableNetworking Networking = ManagerRegistry.BuilderTable.builderNetworking;
            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceDroppedRPC", RpcTarget.All, Networking.CreateLocalCommandId(), piece.pieceId, position, rotation, velocity, angVelocity, PhotonNetwork.LocalPlayer);
            } 
            else
                Networking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
        }

        public static void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
        {
            if (piece.isBuiltIntoTable)
                return;

            BuilderTable table = ManagerRegistry.BuilderTable;
            BuilderTableNetworking Networking = table.builderNetworking;

            if (NetworkSystem.Instance.IsMasterClient)
            {
                Networking.photonView.RPC("PieceDestroyedRPC", RpcTarget.All, piece.pieceId, BitPackUtils.PackWorldPosForNetwork(piece.transform.position), BitPackUtils.PackQuaternionForNetwork(piece.transform.rotation), playFX, (short)recyclerID);
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

        public static void BuildingBlockTextGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (Overpowered.basePosition == null)
                        Overpowered.basePosition = NewPointer.transform.position + Vector3.up;

                    if (Time.time > Overpowered.textDelay)
                    {
                        Overpowered.textDelay = Time.time + 0.1f;
                        bool[][] characterData = Overpowered.Letters[Overpowered.textToRender[Overpowered.characterIndex].ToString()];

                        List<Vector3> positions = new List<Vector3>();
                        for (int i = 0; i < characterData.Length; i++)
                        {
                            bool[] column = characterData[i];

                            for (int j = 0; j < column.Length; j++)
                            {
                                bool currentIndex = column[j];
                                Vector3 offset = new Vector3((j * 0.2f) + (Overpowered.characterIndex * 1.2f), i * -0.2f, 0f);

                                if (currentIndex)
                                    positions.Add(Overpowered.basePosition.Value + offset);
                            }
                        }

                        Overpowered.characterIndex++;

                        foreach (Vector3 position in positions)
                            RequestCreatePiece(pieceIdSet, position, Quaternion.identity, 0);

                        RPCProtection();
                    }
                }
                else
                {
                    Overpowered.basePosition = null;
                    Overpowered.characterIndex = 0;
                }
            }
        }

        public static void RainBuildingBlocks()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + new Vector3(Random.Range(-3f, 3f), 4f, Random.Range(-3f, 3f)), Quaternion.identity, 0);
            RPCProtection();
        }

        public static void BuildingBlockFountain()
        {
            RequestCreatePiece(pieceIdSet, VRRig.LocalRig.transform.position + Vector3.up * 3f, Quaternion.identity, 0, velocity: RandomVector3(15f));
            RPCProtection();
        }

        public static void SpazObject(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
                bug.transform.rotation = RandomQuaternion();
        }

        public static void SpazCamera()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.rotation = RandomQuaternion();
        }

        public static void SpazTablet()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.rotation = RandomQuaternion();
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
                bug.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30), 1, MathF.Sin(offset + (float)Time.frameCount / 30));
        }

        public static void OrbitCamera()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(240f + (float)Time.frameCount / 30), 1, MathF.Sin(240f + (float)Time.frameCount / 30));
        }

        public static void OrbitTablet()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(60f + (float)Time.frameCount / 30), 1, MathF.Sin(240f + (float)Time.frameCount / 30));
        }

        public static void ObjectAura(string objectName)
        {
            ThrowableBug bug = GetBug(objectName);
            if (bug != null)
            {
                bug.transform.position = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                bug.transform.rotation = RandomQuaternion();
            }
        }

        public static void CameraAura()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
            camera.transform.rotation = RandomQuaternion();
        }

        public static void TabletAura()
        {
            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
            camera.transform.rotation = RandomQuaternion();
        }

        public static void BalloonAura()
        {
            foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
            {
                if (balloon.ownerRig.isLocal)
                {
                    balloon.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                    balloon.gameObject.transform.rotation = RandomQuaternion();
                }
                else
                    balloon.WorldShareableRequestOwnership();
            }
        }

        public static void GliderAura()
        {
            foreach (GliderHoldable glider in GetAllType<GliderHoldable>())
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                {
                    glider.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                    glider.gameObject.transform.rotation = RandomQuaternion();
                }
                else
                    glider.OnHover(null, null);
            }
        }

        private static float hoverboardAuraDelay;
        public static void HoverboardAura()
        {
            if (Time.time > hoverboardAuraDelay)
            {
                hoverboardAuraDelay = Time.time + 0.25f;

                for (int i = 0; i < 2; i++)
                    BetaDropBoard(GorillaTagger.Instance.headCollider.transform.position + RandomVector3(), RandomQuaternion(), RandomVector3() * 20f, RandomVector3() * 20f, RandomColor());
            }
        }

        public static void OrbitGliders()
        {
            GliderHoldable[] them = GetAllType<GliderHoldable>();
            int index = 0;
            foreach (GliderHoldable glider in them)
            {
                if (glider.GetView.Owner == PhotonNetwork.LocalPlayer)
                {
                    float offset = 360f / them.Length * index;
                    glider.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 5f, 2, MathF.Sin(offset + (float)Time.frameCount / 30) * 5f);
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
            GameObject bugObject = GetBugObject(objectName).gameObject;

            TeleportPlayer(bugObject.transform.position);
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
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

        public static void BecomeCamera()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 99999f;

            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraCococamInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
            camera.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }

        public static void BecomeTablet()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 99999f;

            LckSocialCamera camera = LckSocialCameraManager.Instance._socialCameraTabletInstance;
            camera.visible = true;
            camera.recording = true;

            camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
            camera.m_CameraVisuals.SetRecordingState(true);

            camera.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
            camera.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }

        private static float noclipBuildingDelay;
        public static void NoclipBuilding()
        {
            if (Time.time > noclipBuildingDelay)
                noclipBuildingDelay = Time.time + 5f;
            else
                return;

            foreach (BuilderPiece piece in GetAllType<BuilderPiece>().Where(block => block.gameObject.activeInHierarchy && !block.isBuiltIntoTable))
                piece.SetColliderLayers(piece.colliders, BuilderTable.heldLayerLocal);
        }

		public static void DisableNoclipBuilding()
		{
			foreach (BuilderPiece piece in GetAllType<BuilderPiece>().Where(block => block.gameObject.activeInHierarchy && !block.isBuiltIntoTable))
				piece.SetColliderLayers(piece.colliders, piece.state == BuilderPiece.State.AttachedAndPlaced ? BuilderTable.placedLayer : BuilderTable.droppedLayer);
		}

		public static void MultiGrab()
        {
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
        }

        public static int pieceId = -1;
        public static IEnumerator CreateGetPiece(int pieceType, Action<BuilderPiece> onComplete)
        {
            CreatePatch.enabled = true;
            CreatePatch.pieceTypeSearch = pieceType;

            yield return null;

            RequestCreatePiece(pieceType, VRRig.LocalRig.transform.position + Vector3.up, Quaternion.identity, 0, null, true);
            RPCProtection();

            while (pieceId < 0)
                yield return null;
            
            yield return null;

            pieceId = -1;
            CreatePatch.enabled = false;
            CreatePatch.pieceTypeSearch = 0;

            onComplete?.Invoke(ManagerRegistry.BuilderTable.GetPiece(pieceId)); // so bad
        }

        public static IEnumerator CreateShotgun()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
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

        private static bool previousGripDown;
        private static bool previousTriggerDown;
        public static void Shotgun()
        {
            if (isFiring)
                ControllerInputPoller.instance.leftControllerGripFloat = 1f;

            if (rightGrab && !previousGripDown)
                CoroutineManager.instance.StartCoroutine(CreateShotgun());

            if (rightGrab && rightTrigger > 0.5f && !previousTriggerDown)
                CoroutineManager.instance.StartCoroutine(FireShotgun());

            previousGripDown = rightGrab;
            previousTriggerDown = rightTrigger > 0.5f;
        }

        public static IEnumerator CreateMassiveBlock()
        {
            VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
            yield return new WaitForSeconds(0.6f);

            BuilderPiece stupid = null;

            yield return CreateGetPiece(pieceIdSet, piece => stupid = piece);
            while (stupid == null)
                yield return null;

            RequestGrabPiece(stupid, false, Vector3.zero, Quaternion.identity);

            yield return new WaitForSeconds(0.2f);

            VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
            yield return null;
        }

        public static void MassiveBlock()
        {
            if (rightGrab && !previousGripDown)
                CoroutineManager.instance.StartCoroutine(CreateMassiveBlock());

            previousGripDown = rightGrab;
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
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                monkeyeAI.speed = 0.02f;
            }
        }

        public static void FastMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                monkeyeAI.speed = 0.5f;
            }
        }

        public static void FixMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                monkeyeAI.speed = 0.1f;
            }
        }

        public static void GrabMonsters()
        {
            if (rightGrab)
            {
                foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
                {
                    if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                    monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                }
            }
        }

        public static void MonsterGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
                    {
                        if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                        monkeyeAI.gameObject.transform.position = NewPointer.transform.position + Vector3.up;
                    }
                }
            }
        }

        public static void SpazMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                monkeyeAI.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            }
        }

        public static void OrbitMonsters()
        {
            MonkeyeAI[] them = GetAllType<MonkeyeAI>();
            int index = 0;
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                float offset = 360f / them.Length * index;
                monkeyeAI.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 2f, 1f, MathF.Sin(offset + (float)Time.frameCount / 30) * 2f);
                index++;
            }
        }

        public static void DestroyMonsters()
        {
            foreach (MonkeyeAI monkeyeAI in GetAllType<MonkeyeAI>())
            {
                if (!NetworkSystem.Instance.IsMasterClient) { NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client."); return; }
                monkeyeAI.gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
            }
        }

        private static readonly List<BuilderPiece> potentialgrabbedpieces = new List<BuilderPiece>();
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

                RequestGrabPiece(piece, false, Buttons.GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomVector3(0.5f), Buttons.GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomQuaternion());
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

                RequestGrabPiece(piece, false, Buttons.GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomVector3(0.5f), Buttons.GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomQuaternion());
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
                    balloon.gameObject.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
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
                    float offset = 360f / them.Length * index;
                    balloon.gameObject.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30) * 5f, 2, MathF.Sin(offset + (float)Time.frameCount / 30) * 5f);
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
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
                    {
                        if (balloon.ownerRig.isLocal)
                            balloon.gameObject.transform.position = NewPointer.transform.position + Vector3.up;
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

        public static void BecomeBalloon()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 99999f;

            foreach (BalloonHoldable balloon in GetAllType<BalloonHoldable>())
            {
                if (balloon.ownerRig.isLocal)
                {
                    balloon.gameObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                    balloon.gameObject.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
                    break;
                }

                balloon.WorldShareableRequestOwnership();
            }
        }

        public static void BecomeHoverboard()
        {
            Vector3 HoverboardPos = GorillaTagger.Instance.bodyCollider.transform.position;
            Quaternion HoverboardRotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = HoverboardPos - Vector3.up * 1f;

            GTPlayer.Instance.SetHoverAllowed(true);
            GTPlayer.Instance.SetHoverActive(true);

            HoverboardVisual hoverboardVisual = VRRig.LocalRig.hoverboardVisual;

            hoverboardVisual.SetIsHeld(true, hoverboardVisual.NominalParentTransform.InverseTransformPoint(HoverboardPos), hoverboardVisual.NominalParentTransform.InverseTransformRotation(HoverboardRotation), VRRig.LocalRig.playerColor);

            hoverboardVisual.interpolatedLocalPosition = hoverboardVisual.NominalLocalPosition;
            hoverboardVisual.interpolatedLocalRotation = hoverboardVisual.NominalLocalRotation;

            GTPlayer.Instance.SetHoverboardPosRot(HoverboardPos, HoverboardRotation);
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
            Buttons.GetIndex("Change Cycle Delay").overlapText = "Change Name Cycle Delay <color=grey>[</color><color=green>" + nameCycleDebounce + "</color><color=grey>]</color>";
        }

        public static void GoldenNameTag(bool isGolden)
        {
			VRRig.LocalRig.ShowGoldNameTag = isGolden;
            VRRig.LocalRig.playerText1.color = VRRig.LocalRig.ShowGoldNameTag ? SubscriptionManager.SUBSCRIBER_NAME_COLOR : Color.white;
		}

        public static void FlashNameTag() =>
            GoldenNameTag((Time.time % 0.2f) > 0.1f);

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
            NameCycle(new[] { RandomString(8) });

        public static string[] names = { };
        public static void EnableCustomNameCycle()
        {
            if (File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt"))
                names = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt").Split('\n');
            else
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomNameCycle.txt","YOUR\nTEXT\nHERE");
        }

        public static string name;
        public static void AnimatedName()
        {
            if (!PhotonNetwork.InRoom)
            {
                ChangeName(name);
                return;
            }
            if (string.IsNullOrEmpty(name))
                name = PhotonNetwork.LocalPlayer.NickName;
            int length = Mathf.Clamp((int)Mathf.PingPong(Time.time / 0.25f, name.Length) + 1, 1, name.Length);

            ChangeName(name[..length]);
        }

        public static float colorChangerDelay;

        public static int colorChangeType;
        public static bool strobeColor;

        public static void FlashColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.05f;
                strobeColor = !strobeColor;
                ChangeColor(strobeColor ? Color.white : Color.black);
            }
        }

        public static void StrobeColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.05f;
                ChangeColor(RandomColor());
            }
        }

        public static void RainbowColor()
        {
            if (Time.time > colorChangerDelay)
            {
                colorChangerDelay = Time.time + 0.05f;
                float h = Time.frameCount / 180f % 1f;
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
                
                Color[] colors = {
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
            string[] names = {
                "MINIGAMES",
                "MINIGAMESKID",
                "LITTLETIMMY",
                "TIMMY",
                "SILLYBILLY"
            };
            Color[] colors = {
                Color.cyan,
                Color.green,
                Color.red,
                Color.magenta
            };

            BecomePlayer(names[Random.Range(0, names.Length)], colors[Random.Range(0, colors.Length)]);
        }

        public static float stealIdentityDelay;
        public static void CopyIdentityGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > stealIdentityDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        ChangeName(GetPlayerFromVRRig(gunTarget).NickName);
                        ChangeColor(gunTarget.playerColor);
                        stealIdentityDelay = Time.time + 0.5f;
                    }
                }
            }
        }

        private static float stealCosmeticsDelay;
        public static void CopyCosmeticsGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > stealCosmeticsDelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, gunTarget.cosmeticSet.ToPackedIDArray(), gunTarget.tryOnSet.ToPackedIDArray(), false);
                        stealCosmeticsDelay = Time.time + 0.5f;
                    }
                }
            }
        }

        public static int accessoryType;
        public static int hat;

        public static bool lastHitL;
        public static bool lastHitR;
        public static bool lastHitLP;
        public static bool lastHitRP;
        public static bool lastHitRS;

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

        private static readonly Dictionary<string[], int[]> cachePacked = new Dictionary<string[], int[]>();
        public static int[] PackCosmetics(string[] unpackedCosmetics)
        {
            if (cachePacked.TryGetValue(unpackedCosmetics, out var cosmetics))
                return cosmetics;

            CosmeticsController.CosmeticSet Set = new CosmeticsController.CosmeticSet(unpackedCosmetics, CosmeticsController.instance);
            int[] packedIDs = Set.ToPackedIDArray();
            cachePacked.Add(unpackedCosmetics, packedIDs);
            return packedIDs;
        }

        private static List<string> ownedArchive;
        private static string[] GetOwnedCosmetics()
        {
            if (ownedArchive != null) return ownedArchive.ToArray();
            ownedArchive = new List<string>();
            foreach (var cosmeticItem in CosmeticsController.instance.allCosmetics.Where(cosmeticItem => VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName)))
                ownedArchive.Add(cosmeticItem.itemName);
            
            return ownedArchive.ToArray();
        }
        private static List<string> tryOnCosmetics;
        private static string[] GetTryOnCosmetics()
        {
            if (tryOnCosmetics != null) return tryOnCosmetics.ToArray();
            tryOnCosmetics = new List<string>();
            foreach (var cosmeticItem in CosmeticsController.instance.allCosmetics.Where(cosmeticItem => cosmeticItem.canTryOn))
                tryOnCosmetics.Add(cosmeticItem.itemName);
            return tryOnCosmetics.ToArray();
        }

        private static string[] GetTryOnBalloons()
        {
            if (tryOnCosmetics != null) return tryOnCosmetics.ToArray();
            tryOnCosmetics = new List<string>();
            foreach (var cosmeticItem in CosmeticsController.instance.allCosmetics.Where(cosmeticItem => cosmeticItem.canTryOn && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon")))
                tryOnCosmetics.Add(cosmeticItem.itemName);
            
            return tryOnCosmetics.ToArray();
        }

        private static string[] GetOwnedBalloons()
        {
            if (ownedArchive == null)
            {
                ownedArchive = new List<string>();
                foreach (var cosmeticItem in CosmeticsController.instance.allCosmetics.Where(cosmeticItem => VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName) && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon")))
                    ownedArchive.Add(cosmeticItem.itemName);
            }
            return ownedArchive.ToArray();
        }


        private static float delay;
        public static void SpazAccessories()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                delay = Time.time + 0.05f;
                string[] owned = VRRig.LocalRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> randomCosmetics = new List<string>();
                    for (int i = 0; i <= amnt; i++)
                        randomCosmetics.Add(owned[Random.Range(0, owned.Length)]);
                    
                    if (VRRig.LocalRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(randomCosmetics.ToArray()), PackCosmetics(randomCosmetics.ToArray()), false);
                    RPCProtection();
                }
            }
        }

        public static void SpazAccessoriesBalloon()
        {
            if (rightTrigger > 0.5f && Time.time > delay)
            {
                delay = Time.time + 0.05f;
                string[] owned = VRRig.LocalRig.inTryOnRoom ? GetTryOnBalloons() : GetOwnedBalloons();
                int amnt = Math.Clamp(owned.Length, 0, 15);
                if (amnt > 0)
                {
                    List<string> randomCosmetics = new List<string>();
                    for (int i = 0; i <= amnt; i++)
                        randomCosmetics.Add(owned[Random.Range(0, owned.Length)]);

                    if (VRRig.LocalRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(randomCosmetics.ToArray()), PackCosmetics(randomCosmetics.ToArray()), false);
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
                    List<string> randomCosmetics = new List<string>();
                    for (int i = 0; i <= amnt; i++)
                        randomCosmetics.Add(owned[Random.Range(0, owned.Length)]);
                    if (VRRig.LocalRig.inTryOnRoom)
                    {
                        CosmeticsController.instance.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.tryOnSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    else
                    {
                        CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                        VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(randomCosmetics.ToArray(), CosmeticsController.instance);
                    }
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(randomCosmetics.ToArray()), PackCosmetics(randomCosmetics.ToArray()), false);
                    RPCProtection();
                }
            }
        }

        private static float delayonhold;
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

        private static int[] archiveCosmetics;
        public static void TryOnAnywhere()
        {
            string[] cosmeticArray = { "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU." };

            archiveCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(cosmeticArray, CosmeticsController.instance);
            VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(cosmeticArray, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(cosmeticArray), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
            RPCProtection();
        }

        public static void TryOffAnywhere()
        {
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, archiveCosmetics, CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
            RPCProtection();
        }

        public static void AddCosmeticToCart(string cosmetic)
        {
            CosmeticsController.instance.currentCart.Insert(0, CosmeticsController.instance.GetItemFromDict(cosmetic));
            CosmeticsController.instance.UpdateShoppingCart();
        }

        public static void PurchaseCosmetic(string cosmetic)
        {
            CosmeticsController.CosmeticItem hat = CosmeticsController.instance.GetItemFromDict(cosmetic);
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
            {
                ItemId = hat.itemName,
                Price = hat.cost,
                VirtualCurrency = CosmeticsController.instance.currencyName,
                CatalogVersion = CosmeticsController.instance.catalog
            }, delegate
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Item \"" + ToTitleCase(hat.overrideDisplayName) + "\" has been purchased.", 5000);
                CosmeticsController.instance.ProcessExternalUnlock(hat.itemName, false, false);
                CosmeticsController.instance.currencyBalance -= hat.cost;
                CosmeticsOwned += hat.itemName;
            }, null);
        }

        private static int rememberdirectory;
        public static void CosmeticBrowser()
        {
            rememberdirectory = pageNumber;

            List<ButtonInfo> cosmeticbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Cosmetic Browser", method = RemoveCosmeticBrowser, isTogglable = false, toolTip = "Returns you back to the fun mods." } };
            foreach (CosmeticsController.CosmeticItem hat in CosmeticsController.instance.allCosmetics)
            {
                if (hat.canTryOn)
                    cosmeticbuttons.Add(new ButtonInfo { buttonText = ToTitleCase(hat.overrideDisplayName), method = () => AddCosmeticToCart(hat.itemName), isTogglable = false, toolTip = "Adds the " + hat.overrideDisplayName.ToLower() + "to your cart." });
            }
            Buttons.buttons[Buttons.GetCategory("Temporary Category")] = cosmeticbuttons.ToArray();

            Buttons.CurrentCategoryName = "Temporary Category";
        }

        public static void RemoveCosmeticBrowser()
        {
            pageNumber = rememberdirectory;
            Buttons.CurrentCategoryName = "Fun Mods";
        }

        public static void AutoLoadCosmetics()
        {
            RequestPatch.enabled = true;
            RequestPatch.currentCoroutine ??= CoroutineManager.instance.StartCoroutine(RequestPatch.LoadCosmetics());
            TryOnRoom.GetComponent<CosmeticBoundaryTrigger>().enabled = false;
        }

        public static void NoAutoLoadCosmetics()
        {
            TryOnRoom.GetComponent<CosmeticBoundaryTrigger>().enabled = true;
            RequestPatch.enabled = false;
        }

        private static float lastTimeCosmeticsChecked;
        public static void AutoPurchaseCosmetics()
        {
            if (!GorillaComputer.instance.isConnectedToMaster)
                lastTimeCosmeticsChecked = Time.time + 120f;

            if (Time.time > lastTimeCosmeticsChecked)
            {
                lastTimeCosmeticsChecked = Time.time + 120f;
                foreach (var hat in CosmeticsController.instance.allCosmetics.Where(hat => hat.cost == 0 && hat.canTryOn && !CosmeticsOwned.Contains(hat.itemName)))
                    PurchaseCosmetic(hat.itemName);
            }
        }

        private static float lastTimePaidCosmeticsChecked;
        public static void AutoPurchasePaidCosmetics()
        {
            if (!GorillaComputer.instance.isConnectedToMaster)
                lastTimePaidCosmeticsChecked = Time.time + 120f;

            if (Time.time > lastTimePaidCosmeticsChecked)
            {
                lastTimePaidCosmeticsChecked = Time.time + 120f;

                CosmeticsController.CosmeticItem[] items = CosmeticsController.instance.currentWornSet.items;
                int allocatedShinyRocks = CosmeticsController.instance.CurrencyBalance;

                foreach (CosmeticsController.CosmeticItem item in items
                    .OrderByDescending(i => i.itemCategory == CosmeticsController.CosmeticCategory.Hat)
                    .ThenBy(i => i.itemCategory == CosmeticsController.CosmeticCategory.Hat ? i.itemName : null))
                {
                    if (allocatedShinyRocks < item.cost || !item.canTryOn ||
                        CosmeticsOwned.Contains(item.itemName)) continue;
                    PurchaseCosmetic(item.itemName);
                    allocatedShinyRocks -= item.cost;
                }
            }
        }

        private static bool lasttagged;
        public static void DisableCosmeticsOnTag()
        {
            if (!lasttagged && VRRig.LocalRig.IsTagged())
            {
                string[] cosmetics = { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, PackCosmetics(cosmetics), PackCosmetics(cosmetics), false);
                RPCProtection();
            }
            if (lasttagged && !VRRig.LocalRig.IsTagged())
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
                RPCProtection();
            }
            lasttagged = VRRig.LocalRig.IsTagged();
        }

        public static bool hasGivenCosmetics;
        public static void UnlockAllCosmetics()
        {
            CosmeticPatch.enabled = true;
            if (!PostGetData.CosmeticsInitialized || hasGivenCosmetics) return;
            hasGivenCosmetics = true;
            MethodInfo unlockItem = typeof(CosmeticsController).GetMethod("UnlockItem", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var item in CosmeticsController.instance.allCosmetics.Where(item => !CosmeticsController.instance.concatStringCosmeticsAllowed.Contains(item.itemName)))
            {
                try
                {
                    unlockItem.Invoke(CosmeticsController.instance, new object[] { item.itemName, false });
                }
                catch
                {
                }
            }
        }

        private static float idgundelay;
        public static void CopyIDGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        idgundelay = Time.time + 0.5f;
                        string id = GetPlayerFromVRRig(gunTarget).UserId;
                        NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                        GUIUtility.systemCopyBuffer = id;
                    }
                }
            }
        }

        public static void CopyIDAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0)
            {
                foreach (var id in nearbyPlayers.Select(nearbyPlayer => GetPlayerFromVRRig(nearbyPlayer).UserId))
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                    GUIUtility.systemCopyBuffer = id;
                }
            }
        }

        public static void CopyIDOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0)
            {
                foreach (var id in touchedPlayers.Select(rig => GetPlayerFromVRRig(rig).UserId))
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                    GUIUtility.systemCopyBuffer = id;
                }
            }
        }

        public static void CopyIDAll()
        {
            foreach (var id in GorillaParent.instance.vrrigs.Select(vrrig => GetPlayerFromVRRig(vrrig).UserId))
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                GUIUtility.systemCopyBuffer = id;
            }
        }

        public static void CopySelfID()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
            GUIUtility.systemCopyBuffer = id;
        }

        public static void NarrateIDGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        idgundelay = Time.time + 0.5f;
                        SpeakText("Name: " + GetPlayerFromVRRig(gunTarget).NickName + ". I D: " + string.Join(" ", GetPlayerFromVRRig(gunTarget).UserId));
                    }
                }
            }
        }
        
        public static void NarrateIDAll()
        {
            string ids = "";
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                    ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I D: " + string.Join(" ", GetPlayerFromVRRig(vrrig).UserId) + ". ";
            }
            SpeakText(ids);
        }

        private static float allNarrationDelay;
        public static void NarrateIDAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0 && Time.time > allNarrationDelay)
            {
                allNarrationDelay = Time.time + 10f;

                string ids = "";
                foreach (VRRig vrrig in nearbyPlayers)
                        ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I D: " + string.Join(" ", GetPlayerFromVRRig(vrrig).UserId) + ". ";
                SpeakText(ids);
            }
        }

        public static void NarrateIDOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0 && Time.time > allNarrationDelay)
            {
                allNarrationDelay = Time.time + 10f;

                string ids = "";
                foreach (VRRig vrrig in touchedPlayers)
                    ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I D: " + string.Join(" ", GetPlayerFromVRRig(vrrig).UserId) + ". ";
                SpeakText(ids);
            }
        }

        public static void NarrateSelfID() =>
            SpeakText("Name: " + PhotonNetwork.LocalPlayer.NickName + ". I D: " + string.Join(" ", PhotonNetwork.LocalPlayer.UserId));

        public static void NarrateFakeDoxxGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal())
                    {
                        idgundelay = Time.time + 0.5f;
                        SpeakText("Name: " + GetPlayerFromVRRig(gunTarget).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}"));
                    }
                }
            }
        }

        public static void NarrateFakeDoxxAll()
        {
            string ids = "";
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                    ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
            }
            SpeakText(ids);
        }

        public static void NarrateFakeDoxxAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count > 0 && Time.time > allNarrationDelay)
            {
                allNarrationDelay = Time.time + 10f;

                string ids = "";
                foreach (VRRig vrrig in nearbyPlayers)
                        ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
                SpeakText(ids);
            }
        }

        public static void NarrateFakeDoxxOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0 && Time.time > allNarrationDelay)
            {
                allNarrationDelay = Time.time + 10f;

                string ids = "";
                foreach (VRRig vrrig in touchedPlayers)
                    ids += "Name: " + GetPlayerFromVRRig(vrrig).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
                SpeakText(ids);
            }
        }

        public static void NarrateFakeDoxxSelf() =>
            SpeakText("Name: " + PhotonNetwork.LocalPlayer.NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}"));

        private static float creationDateDelay;
        public static void CopyCreationDateSelf()
        {
            string date = GetCreationDate(PhotonNetwork.LocalPlayer.UserId, CopyCreationDate);
            if (date != "Loading...")
                CopyCreationDate(date);
        }

        public static void CopyCreationDateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > creationDateDelay)
                    {
                        creationDateDelay = Time.time + 0.5f;

                        string date = GetCreationDate(GetPlayerFromVRRig(gunTarget).UserId, CopyCreationDate);
                        if (date != "Loading...")
                            CopyCreationDate(date);
                    }
                }
            }
        }

        public static void CopyCreationDateAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count <= 0) return;
            foreach (var date in nearbyPlayers.Select(nearbyPlayer => GetCreationDate(GetPlayerFromVRRig(nearbyPlayer).UserId, CopyCreationDate)).Where(date => date != "Loading..."))
                CopyCreationDate(date);
        }

        public static void CopyCreationDateOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = new List<VRRig>();

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.IsLocal())
                {
                    if (Vector3.Distance(rig.transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f ||
                        Vector3.Distance(rig.transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f)
                    {
                        touchedPlayers.Add(rig);
                    }
                }
            }

            if (touchedPlayers.Count > 0)
            {
                foreach (var date in touchedPlayers.Select(rig => GetCreationDate(GetPlayerFromVRRig(rig).UserId, CopyCreationDate)).Where(date => date != "Loading..."))
                    CopyCreationDate(date);
            }
        }

        public static void CopyCreationDateAll()
        {
            foreach (var date in GorillaParent.instance.vrrigs.Select(vrrig => GetCreationDate(GetPlayerFromVRRig(vrrig).UserId, CopyCreationDate)).Where(date => date != "Loading..."))
            {
                CopyCreationDate(date);
            }
        }

        public static void CopyCreationDate(string date)
        {
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
            GUIUtility.systemCopyBuffer = date;
        }

        public static void NarrateCreationDateSelf()
        {
            string date = GetCreationDate(PhotonNetwork.LocalPlayer.UserId, SpeakText);
            if (date != "Loading...")
                SpeakText(date);
        }

        public static void NarrateCreationDateAll()
        {
            foreach (var date in GorillaParent.instance.vrrigs.Select(vrrig => GetCreationDate(GetPlayerFromVRRig(vrrig).UserId, SpeakText)).Where(date => date != "Loading..."))
                SpeakText(date);
        }

        public static void NarrateCreationDateAura()
        {
            if (!PhotonNetwork.InRoom) return;
            List<VRRig> nearbyPlayers = new List<VRRig>();

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(vrrig.transform.position, VRRig.LocalRig.transform.position) < 4 && !vrrig.IsLocal())
                    nearbyPlayers.Add(vrrig);
                else if (nearbyPlayers.Contains(vrrig))
                    nearbyPlayers.Remove(vrrig);
            }

            if (nearbyPlayers.Count <= 0 || Time.time < allNarrationDelay) return;
            allNarrationDelay = Time.time + 10f;

            foreach (var date in nearbyPlayers.Select(nearbyPlayer => GetCreationDate(GetPlayerFromVRRig(nearbyPlayer).UserId, date => SpeakText(date))).Where(date => date != "Loading..."))
                SpeakText(date);
        }

        public static void NarrateCreationDateOnTouch()
        {
            if (!PhotonNetwork.InRoom) return;

            List<VRRig> touchedPlayers = GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal()).Where(rig => Vector3.Distance(rig.transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(rig.transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f).ToList();

            if (touchedPlayers.Count <= 0 || Time.time < allNarrationDelay) return;
            allNarrationDelay = Time.time + 10f;

            foreach (var date in touchedPlayers.Select(rig => GetCreationDate(GetPlayerFromVRRig(rig).UserId, date => SpeakText(date))).Where(date => date != "Loading..."))
                SpeakText(date);
        }

        public static void NarrateCreationDateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !gunTarget.IsLocal() && Time.time > creationDateDelay)
                    {
                        creationDateDelay = Time.time + 0.5f;

                        string date = GetCreationDate(GetPlayerFromVRRig(gunTarget).UserId, date => SpeakText(date));
                        if (date != "Loading...")
                            SpeakText(date);
                    }
                }
            }
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
                    cosmetics = plr.rawCosmeticString;
                }
                catch { LogManager.Log("Failed to log colors, rig most likely nonexistent"); }
                try
                {
                    text += "\n====================================\n";
                    text += string.Concat("Player Name: \"", player.NickName, "\", Player ID: \"", player.UserId, "\", Player Color: (R: ", r.ToString(), ", G: ", g.ToString(), ", B: ", b.ToString(), "), Cosmetics: ", cosmetics);
                }
                catch { LogManager.Log("Failed to log player"); }
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = $"{PluginInfo.BaseDirectory}/PlayerInfo/" + PhotonNetwork.CurrentRoom.Name + ".txt";

            File.WriteAllText(fileName, text);

            string filePath = FileUtilities.GetGamePath() + "/" + fileName;
            Process.Start(filePath);
        }

        /*
        public static string consoleTyped = "";
        public static int currentModIndex = 0;
        public static int pageNumber = 0;
        public static void ConsoleFrame()
        {
            int halfPoint = Mathf.FloorToInt((System.Console.WindowWidth - 1) / 2f);
            string logoPrefix = "";
            for (int i = 0; i < halfPoint - math.floor(PluginInfo.Logo.Split("\n")[0].Length * 0.5); i++)
                logoPrefix += " ";
            string logo = logoPrefix + PluginInfo.Logo.Replace("\n", "\n" + logoPrefix);
            
            int pageSize = System.Console.WindowHeight - 9 - (logo.Split("\n").Length - 1);
            if (Time.frameCount % 1000 == 0)
                System.Console.Clear();

            string largeNewLine = "";
            for (int i = 0; i < 100; i++)
                largeNewLine += Environment.NewLine;
            
            string modList = "";

            ButtonInfo[] categoryButtonsPre = Buttons.buttons[currentCategoryIndex];
            ButtonInfo[] categoryButtons = new ButtonInfo[categoryButtonsPre.Length + 2];
            int pageCount = (int)Math.Ceiling((double)categoryButtons.Length / pageSize) - 1;
            categoryButtons[0] = new ButtonInfo { buttonText = "Previous Page", method =() => { pageNumber--; if (pageNumber < 0) { pageNumber = pageCount; } }, isTogglable = false, toolTip = "Takes you to the previous page."};
            categoryButtons[1] = new ButtonInfo { buttonText = "Next Page", method =() => { pageNumber++; pageNumber %= pageCount + 1; }, isTogglable = false, toolTip = "Takes you to the previous page."};
            Array.Copy(categoryButtonsPre, 0, categoryButtons, 2, categoryButtonsPre.Length);
            for (int index = 0; index < categoryButtons.Length; index++)
            {
                ButtonInfo mod = categoryButtons[index];
                if (index != 0 && index != 1)
                {
                    int start = 2 + pageNumber * pageSize;
                    int end = start + pageSize;

                    if (index < start || index >= end)
                        continue;
                }
                string modName = NoRichtextTags(mod.overlapText ?? mod.buttonText);
                if (index == currentModIndex + (pageNumber * pageSize) || (index < 2 && index == currentModIndex))
                    modList += Environment.NewLine + "> " + (mod.enabled ? "[E] " : "" ) + modName;
                else
                    modList += Environment.NewLine + "  " + (mod.enabled ? "[E] " : "" ) + modName;
            }

            if (System.Console.KeyAvailable)
            {
                int buttonCount = categoryButtonsPre.Length % pageSize;
                if (pageCount != pageNumber)
                    buttonCount = pageSize;
                
                var key = System.Console.ReadKey(true);
                string stringKey = key.KeyChar.ToString();
                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        consoleTyped = consoleTyped.Length != 0 ? consoleTyped[..^1] : consoleTyped;
                        break;
                    case ConsoleKey.Enter:
                        if (consoleTyped != "")
                        {

                            ButtonInfo selButton = Buttons.GetIndex(consoleTyped);
                            if (selButton == null)
                            {
                                for (int i = 0; i < Buttons.buttons.Length; i++)
                                {
                                    ButtonInfo[] buttonList = Buttons.buttons[i];
                                    foreach (ButtonInfo buttonInfo in buttonList)
                                    {
                                        string text = (buttonInfo.overlapText ?? buttonInfo.buttonText).ToLower();
                                        if (text.Contains(consoleTyped.ToLower()) &&
                                            (selButton == null || i == currentCategoryIndex))
                                            selButton = buttonInfo;
                                    }
                                }
                            }

                            if (selButton != null)
                                Toggle(selButton.buttonText, true);
                            else
                                NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"{consoleTyped}\" does not exist.");
                            consoleTyped = "";
                            break;
                        }
                        goto case ConsoleKey.RightArrow;
                    case ConsoleKey.UpArrow:
                        currentModIndex--;
                        if (currentModIndex < 0)
                            currentModIndex = buttonCount + 1;
                        break;
                    case ConsoleKey.DownArrow:
                        currentModIndex++;
                        currentModIndex %= buttonCount + 2;
                        break;
                    case ConsoleKey.RightArrow:
                        ButtonInfo button = categoryButtons[currentModIndex + (pageNumber * pageSize)];
                        if (currentModIndex < 2)
                        {
                            button = categoryButtons[currentModIndex];
                            button.method();
                            break;
                        }
                        int prevCategory = currentCategoryIndex;
                        Toggle(button.buttonText, true);
                        if (prevCategory != currentCategoryIndex)
                        {
                            pageNumber = 0;
                            currentModIndex = 0;
                        }
                        break;
                    default:
                        if (stringKey == " ")
                        {
                            if (key.Key != ConsoleKey.Spacebar)
                                break;
                        }
                        consoleTyped += stringKey;
                        break;
                }
            }
            
            string screenLine = "";
            for (int i = 0; i < System.Console.WindowWidth - 1; i++)
                screenLine += "-";
            
            string infoPrefix = "";
            for (int i = 0; i < halfPoint - 16; i++)
                infoPrefix += " ";
            
            System.Console.WriteLine(
$@"{largeNewLine}
{logo}
{infoPrefix}> Use the arrow keys to navigate.
{screenLine}{modList}
{screenLine}

> {consoleTyped}");
        }*/
    }
}
