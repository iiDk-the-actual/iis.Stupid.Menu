/*
 * ii's Stupid Menu  Mods/Movement.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Swimming;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using iiMenu.Patches.Menu;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Valve.Newtonsoft.Json.Linq;
using static iiMenu.Managers.RigManager;
using static iiMenu.Menu.Main;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace iiMenu.Mods
{
    public static class Movement
    {
        public static int platformMode;
        public static void ChangePlatformType(bool positive = true)
        {
            string[] platformNames = {
                "Normal",
                "Invisible",
                "Rainbow",
                "Random Color",
                "Noclip",
                "Glass",
                "Projectile"
            };

            if (positive)
                platformMode++;
            else
                platformMode--;

            platformMode %= platformNames.Length;
            if (platformMode < 0)
                platformMode = platformNames.Length - 1;

            GetIndex("Change Platform Type").overlapText = "Change Platform Type <color=grey>[</color><color=green>" + platformNames[platformMode] + "</color><color=grey>]</color>";
        }

        public static int platformShape;
        public static void ChangePlatformShape(bool positive = true)
        {
            string[] platformShapes = {
                "Sphere",
                "Cube",
                "Cylinder",
                "Legacy",
                "Small",
                "Long",
                "1x1",
                "Massive"
            };

            if (positive)
                platformShape++;
            else
                platformShape--;

            platformShape %= platformShapes.Length;
            if (platformShape < 0)
                platformShape = platformShapes.Length - 1;

            GetIndex("Change Platform Shape").overlapText = "Change Platform Shape <color=grey>[</color><color=green>" + platformShapes[platformShape] + "</color><color=grey>]</color>";
        }

        public static PrimitiveType GetPlatformPrimitiveType()
        {
            return platformShape switch
            {
                0 => PrimitiveType.Sphere,
                1 => PrimitiveType.Cube,
                2 => PrimitiveType.Cylinder,
                _ => PrimitiveType.Cube,
            };
        }

        public static Vector3 GetPlatformScale()
        {
            return new[]
            {
                new Vector3(0.333f, 0.333f, 0.333f),
                new Vector3(0.333f, 0.333f, 0.333f),
                new Vector3(0.333f, 0.333f, 0.333f),
                new Vector3(0.025f, 0.3f, 0.4f),
                new Vector3(0.025f, 0.15f, 0.2f),
                new Vector3(0.025f, 0.3f, 0.8f),
                new Vector3(0.1f, 0.1f, 0.1f),
                new Vector3(0.025f, 1f, 1f)
            }[platformShape] * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
        }

        public static GameObject CreatePlatform()
        {
            GameObject platform = GameObject.CreatePrimitive(GetPlatformPrimitiveType());
            platform.transform.localScale = GetPlatformScale();

            Renderer platformRenderer = platform.GetComponent<Renderer>();

            switch (platformMode)
            {
                case 1:
                case 6:
                    platformRenderer.enabled = false;
                    break;
                case 4:
                    UpdateClipColliders(false);
                    break;
                case 5:
                    platform.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
                    if (glass == null)
                        glass = new Material(Shader.Find("GUI/Text Shader"));

                    platformRenderer.material = glass;
                    break;
            }

            if (!GetIndex("Non-Sticky Platforms").enabled)
                FixStickyColliders(platform);

            if (platformRenderer.enabled && platform)
            {
                ColorChanger colorChanger = platform.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;

                if (platformMode == 2 || platformMode == 3)
                {
                    colorChanger.colors = colorChanger.colors.Clone();
                    colorChanger.colors.rainbow |= platformMode == 2;
                    colorChanger.colors.epileptic |= platformMode == 3;
                }
            }

            if (GetIndex("Platform Outlines").enabled)
            {
                GameObject gameObject = GameObject.CreatePrimitive(GetPlatformPrimitiveType());
                Object.Destroy(gameObject.GetComponent<Collider>());
                gameObject.transform.parent = platform.transform;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.95f, 1.05f, 1.05f);

                ColorChanger outlineColorChanger = gameObject.AddComponent<ColorChanger>();
                outlineColorChanger.colors = buttonColors[0];
            }
            return platform;
        }

        public static int flySpeedCycle = 1;
        public static float _flySpeed = 10f;

        public static float flySpeed
        {
            get => _flySpeed * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
            set => _flySpeed = value;
        }

        public static int speedboostCycle = 1;
        public static float jspeed = 7.5f;
        public static float jmulti = 1.1f;

        public static int longarmCycle = 2;
        public static float armlength = 1.25f;

        public static GameObject leftplat;
        public static GameObject rightplat;
        public static void Platforms(bool? left = null, bool? right = null)
        {
            if (platformMode == 6)
                Projectiles.GrabProjectile();

            if (left ?? leftGrab)
            {
                if (leftplat == null)
                {
                    leftplat = CreatePlatform();
                    var leftHandTransform = TrueLeftHand();
                    leftplat.transform.position = leftHandTransform.position;
                    leftplat.transform.rotation = leftHandTransform.rotation;

                    if (GetIndex("Stick Long Arms").enabled)
                        leftplat.transform.position += GorillaTagger.Instance.leftHandTransform.forward * (armlength - 0.917f);
                    if (GetIndex("Multiplied Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        MultipliedLongArms();
                        leftplat.transform.position = TrueLeftHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Vertical Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        VerticalLongArms();
                        leftplat.transform.position = TrueLeftHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Horizontal Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        HorizontalLongArms();
                        leftplat.transform.position = TrueLeftHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Non-Sticky Platforms").enabled)
                        leftplat.transform.position += TrueLeftHand().right * ((0.025f + leftplat.transform.localScale.x / 2f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));

                    FriendManager.PlatformSpawned(true, leftplat.transform.position, leftplat.transform.rotation, leftplat.transform.localScale, GetPlatformPrimitiveType());
                }
            }
            else
            {
                if (leftplat != null)
                {
                    if (GetIndex("Platform Gravity").enabled)
                    {
                        leftplat.AddComponent(typeof(Rigidbody));
                        Object.Destroy(leftplat.GetComponent<Collider>());
                        Object.Destroy(leftplat, 2f);
                    }
                    else
                        Object.Destroy(leftplat);
                    
                    leftplat = null;
                    if (platformMode == 4 && rightplat == null)
                        UpdateClipColliders(true);

                    FriendManager.PlatformDespawned(true);
                }
            }

            if (right ?? rightGrab)
            {
                if (rightplat == null)
                {
                    rightplat = CreatePlatform();
                    var rightHandTransform = TrueRightHand();
                    rightplat.transform.position = rightHandTransform.position;
                    rightplat.transform.rotation = rightHandTransform.rotation;
                    if (GetIndex("Stick Long Arms").enabled)
                        rightplat.transform.position += GorillaTagger.Instance.rightHandTransform.forward * (armlength - 0.917f);
                    
                    if (GetIndex("Multiplied Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        MultipliedLongArms();
                        rightplat.transform.position = TrueRightHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Vertical Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        VerticalLongArms();
                        rightplat.transform.position = TrueRightHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Horizontal Long Arms").enabled)
                    {
                        Vector3 legacyPosL = GTPlayer.Instance.leftControllerTransform.transform.position;
                        Vector3 legacyPosR = GTPlayer.Instance.rightControllerTransform.transform.position;
                        HorizontalLongArms();
                        rightplat.transform.position = TrueRightHand().position;
                        GTPlayer.Instance.leftControllerTransform.transform.position = legacyPosL;
                        GTPlayer.Instance.rightControllerTransform.transform.position = legacyPosR;
                    }
                    if (GetIndex("Non-Sticky Platforms").enabled)
                        rightplat.transform.position -= TrueRightHand().right * ((0.025f + rightplat.transform.localScale.x / 2f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));

                    FriendManager.PlatformSpawned(false, rightplat.transform.position, rightplat.transform.rotation, rightplat.transform.localScale, GetPlatformPrimitiveType());
                }
            }
            else
            {
                if (rightplat != null)
                {
                    if (GetIndex("Platform Gravity").enabled)
                    {
                        rightplat.AddComponent(typeof(Rigidbody));
                        Object.Destroy(rightplat.GetComponent<Collider>());
                        Object.Destroy(rightplat, 2f);
                    }
                    else
                        Object.Destroy(rightplat);
                    
                    rightplat = null;
                    if (platformMode == 4 && leftplat == null)
                        UpdateClipColliders(true);

                    FriendManager.PlatformDespawned(false);
                }
            }
        }

        public static void Frozone()
        {
            if (leftGrab)
            {
                GameObject slipperyPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slipperyPlatform.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
                slipperyPlatform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                slipperyPlatform.transform.localPosition = TrueLeftHand().position + TrueLeftHand().right * 0.05f;
                slipperyPlatform.transform.rotation = TrueLeftHand().rotation;

                slipperyPlatform.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                Object.Destroy(slipperyPlatform, 1);
            }

            if (rightGrab)
            {
                GameObject slipperyPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slipperyPlatform.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
                slipperyPlatform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                slipperyPlatform.transform.localPosition = TrueRightHand().position + TrueRightHand().right * -0.05f;
                slipperyPlatform.transform.rotation = TrueRightHand().rotation;

                slipperyPlatform.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                Object.Destroy(slipperyPlatform, 1);
            }

            GorillaTagger.Instance.bodyCollider.enabled = !(leftGrab || rightGrab);
        }

        public static void ChangeSpeedBoostAmount(bool positive = true)
        {
            float[] jspeedamounts = { 2f, 7.5f, 8f, 9f, 200f };
            float[] jmultiamounts = { 0.5f, 1.1f, 1.5f, 2f, 10f };
            string[] speedNames = { "Slow", "Normal", "Middle", "Fast", "Ultra Fast" };

            if (positive)
                speedboostCycle++;
            else
                speedboostCycle--;

            speedboostCycle %= jspeedamounts.Length;
            if (speedboostCycle < 0)
                speedboostCycle = jspeedamounts.Length - 1;

            jspeed = jspeedamounts[speedboostCycle];
            jmulti = jmultiamounts[speedboostCycle];
            
            GetIndex("Change Speed Boost Amount").overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>" + speedNames[speedboostCycle] + "</color><color=grey>]</color>";
        }

        public static void PlatformSpam()
        {
            if (rightGrab)
            {
                GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.Destroy(platform.GetComponent<BoxCollider>());
                platform.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
                platform.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                platform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                platform.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                platform.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                Object.Destroy(platform, 1f);
                PhotonNetwork.RaiseEvent(69, new object[] { platform.transform.position, platform.transform.rotation }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void PlatformGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(platform.GetComponent<BoxCollider>());
                    platform.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();
                    platform.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    platform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platform.transform.position = NewPointer.transform.position;
                    platform.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    Object.Destroy(platform, 1f);
                    PhotonNetwork.RaiseEvent(69, new object[] { platform.transform.position, platform.transform.rotation }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                }
            }
        }

        public static void ChangeFlySpeed(bool positive = true)
        {
            float[] speedamounts = { 5f, 10f, 30f, 60f, 0.5f };
            string[] speedNames = { "Slow", "Normal", "Fast", "Extra Fast", "Extra Slow" };

            if (positive)
                flySpeedCycle++;
            else
                flySpeedCycle--;

            flySpeedCycle %= speedamounts.Length;
            if (flySpeedCycle < 0)
                flySpeedCycle = speedamounts.Length - 1;

            flySpeed = speedamounts[flySpeedCycle];

            GetIndex("Change Fly Speed").overlapText = "Change Fly Speed <color=grey>[</color><color=green>" + speedNames[flySpeedCycle] + "</color><color=grey>]</color>";
        }

        public static int playspaceAbuseIndex;
        public static void ChangePlayspaceAbuseSpeed(bool positive = true)
        {
            float[] speedamounts = { 0.004f, 0.01f, 0.1f, 0.001f, 0.002f };
            string[] speedNames = { "Normal", "Fast", "Extra Fast", "Extra Slow", "Slow" };

            if (positive)
                playspaceAbuseIndex++;
            else
                playspaceAbuseIndex--;

            playspaceAbuseIndex %= speedamounts.Length;
            if (playspaceAbuseIndex < 0)
                playspaceAbuseIndex = speedamounts.Length - 1;

            playspaceAbusePower = speedamounts[playspaceAbuseIndex];

            GetIndex("Change Playspace Abuse Speed").overlapText = "Change Playspace Abuse Speed <color=grey>[</color><color=green>" + speedNames[playspaceAbuseIndex] + "</color><color=grey>]</color>";
        }

        public static void ChangeArmLength(bool positive = true)
        {
            float[] lengthAmounts = { 0.75f, 1.1f, 1.25f, 1.5f, 2f };
            string[] lengthNames = { "Shorter", "Unnoticable", "Normal", "Long", "Extreme" };

            if (positive)
                longarmCycle++;
            else
                longarmCycle--;

            longarmCycle %= lengthAmounts.Length;
            if (longarmCycle < 0)
                longarmCycle = lengthAmounts.Length - 1;

            armlength = lengthAmounts[longarmCycle];
            GetIndex("Change Arm Length").overlapText = "Change Arm Length <color=grey>[</color><color=green>" + lengthNames[longarmCycle] + "</color><color=grey>]</color>";
        }

        public static void Fly()
        {
            if (rightPrimary)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * flySpeed);
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static void TriggerFly()
        {
            if (rightTrigger > 0.5f)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * flySpeed);
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static bool noclip;
        public static void NoclipFly()
        {
            if (rightPrimary)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * flySpeed);
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                if (!noclip)
                {
                    noclip = true;
                    UpdateClipColliders(false);
                }
            } else
            {
                if (noclip)
                {
                    noclip = false;
                    UpdateClipColliders(true);
                }
            }
        }

        public static void JoystickFly()
        {
            Vector2 joy = leftJoystick;

            if (Mathf.Abs(joy.x) > 0.3 || Mathf.Abs(joy.y) > 0.3)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * (Time.deltaTime * (joy.y * flySpeed)) + GorillaTagger.Instance.headCollider.transform.right * (Time.deltaTime * (joy.x * flySpeed));
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static void BarkFly()
        {
            Vector3 inputDirection = new Vector3(leftJoystick.x, rightJoystick.y, leftJoystick.y);

            Vector3 playerForward = GTPlayer.Instance.bodyCollider.transform.forward.X_Z();
            Vector3 playerRight = GTPlayer.Instance.bodyCollider.transform.right.X_Z();

            ZeroGravity();

            Vector3 velocity = inputDirection.x * playerRight + inputDirection.y * Vector3.up + inputDirection.z * playerForward;
            velocity *= flySpeed;
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.Lerp(GorillaTagger.Instance.rigidbody.linearVelocity, velocity, 0.12875f);
        }

        public static void VelocityBarkFly()
        {
            if (Mathf.Abs(leftJoystick.x) > 0.3 || Mathf.Abs(leftJoystick.y) > 0.3 || Mathf.Abs(rightJoystick.x) > 0.3 || Mathf.Abs(rightJoystick.y) > 0.3)
                BarkFly();
        }

        public static void HandFly()
        {
            if (rightPrimary)
            {
                GTPlayer.Instance.transform.position += TrueRightHand().forward * (Time.deltaTime * flySpeed);
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static void FlyTowardsGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    GTPlayer.Instance.transform.position += (lockTarget.transform.position - GorillaTagger.Instance.bodyCollider.transform.position) * (Time.deltaTime * flySpeed);
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
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

        public static void SlingshotFly()
        {
            if (rightPrimary)
                GorillaTagger.Instance.rigidbody.linearVelocity += GTPlayer.Instance.headCollider.transform.forward * (Time.deltaTime * (flySpeed * 2));
        }

        public static void ZeroGravitySlingshotFly()
        {
            if (rightPrimary)
            {
                ZeroGravity();
                GorillaTagger.Instance.rigidbody.linearVelocity += GTPlayer.Instance.headCollider.transform.forward * (Time.deltaTime * flySpeed);
            }
        }

        public static float startX = -1f;
        public static float startY = -1f;

        public static float subThingy;
        public static float subThingyZ;

        public static Vector3 lastPosition = Vector3.zero;
        public static void WASDFly()
        {
            bool stationary = !GetIndex("Disable Stationary WASD Fly").enabled;

            bool W = UnityInput.Current.GetKey(KeyCode.W);
            bool A = UnityInput.Current.GetKey(KeyCode.A);
            bool S = UnityInput.Current.GetKey(KeyCode.S);
            bool D = UnityInput.Current.GetKey(KeyCode.D);
            bool Space = UnityInput.Current.GetKey(KeyCode.Space);
            bool Ctrl = UnityInput.Current.GetKey(KeyCode.LeftControl);
            bool Shift = UnityInput.Current.GetKey(KeyCode.LeftShift);
            bool Alt = UnityInput.Current.GetKey(KeyCode.LeftAlt);

            if (stationary || W || A || S || D || Space || Ctrl)
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;

            if (!menu)
            {
                if (Mouse.current.rightButton.isPressed)
                {
                    Transform parentTransform = GTPlayer.Instance.rightControllerTransform.parent;
                    Quaternion currentRotation = parentTransform.rotation;
                    Vector3 euler = currentRotation.eulerAngles;

                    if (startX < 0)
                    {
                        startX = euler.y;
                        subThingy = Mouse.current.position.value.x / Screen.width;
                    }
                    if (startY < 0)
                    {
                        startY = euler.x;
                        subThingyZ = Mouse.current.position.value.y / Screen.height;
                    }

                    float newX = startY - (Mouse.current.position.value.y / Screen.height - subThingyZ) * 360 * 1.33f;
                    float newY = startX + (Mouse.current.position.value.x / Screen.width - subThingy) * 360 * 1.33f;

                    newX = newX > 180f ? newX - 360f : newX;
                    newX = Mathf.Clamp(newX, -90f, 90f);

                    parentTransform.rotation = Quaternion.Euler(newX, newY, euler.z);
                }
                else
                {
                    startX = -1;
                    startY = -1;
                }

                float speed = flySpeed;
                if (Shift)
                    speed *= 2f;
                else if (Alt)
                    speed /= 2;

                if (W)
                    GorillaTagger.Instance.rigidbody.transform.position += GTPlayer.Instance.rightControllerTransform.parent.forward * (Time.deltaTime * speed);

                if (S)
                    GorillaTagger.Instance.rigidbody.transform.position += GTPlayer.Instance.rightControllerTransform.parent.forward * (Time.deltaTime * -speed);

                if (A)
                    GorillaTagger.Instance.rigidbody.transform.position += GTPlayer.Instance.rightControllerTransform.parent.right * (Time.deltaTime * -speed);

                if (D)
                    GorillaTagger.Instance.rigidbody.transform.position += GTPlayer.Instance.rightControllerTransform.parent.right * (Time.deltaTime * speed);

                if (Space)
                    GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * speed, 0f);

                if (Ctrl)
                    GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * -speed, 0f);

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
            }

            if (!W && !A && !S && !D && !Space && !Ctrl && lastPosition != Vector3.zero && stationary)
                GorillaTagger.Instance.rigidbody.transform.position = lastPosition;
            else
                lastPosition = GorillaTagger.Instance.rigidbody.transform.position;
        }

        private static float driveSpeed;
        public static int driveInt;
        public static void ChangeDriveSpeed(bool positive = true)
        {
            float[] speedamounts = { 10f, 30f, 50f, 100f, 3f };
            string[] speedNames = { "Normal", "Fast", "Ultra Fast", "The Flash", "Slow",  };

            if (positive)
                driveInt++;
            else
                driveInt--;

            driveInt %= speedamounts.Length;
            if (driveInt < 0)
                driveInt = speedamounts.Length - 1;

            driveSpeed = speedamounts[driveInt];
            GetIndex("cdSpeed").overlapText = "Change Drive Speed <color=grey>[</color><color=green>" + speedNames[driveInt] + "</color><color=grey>]</color>";
        }

        public static Vector2 lerpygerpy = Vector2.zero;
        public static void Drive()
        {
            Vector2 joy = leftJoystick;
            lerpygerpy = Vector2.Lerp(lerpygerpy, joy, 0.05f);

            Vector3 addition = GorillaTagger.Instance.bodyCollider.transform.forward * lerpygerpy.y + GorillaTagger.Instance.bodyCollider.transform.right * lerpygerpy.x;
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GTPlayer.Instance.locomotionEnabledLayers);
            Vector3 targetVelocity = addition * driveSpeed;

            if (Ray.distance < 0.2f && (Mathf.Abs(lerpygerpy.x) > 0.05f || Mathf.Abs(lerpygerpy.y) > 0.05f))
                GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(targetVelocity.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, targetVelocity.z);
        }

        public static void HardDrive()
        {
            bool elevatedStickyDrive = GetIndex("Elevated Sticky Drive").enabled;
            if ((Mathf.Abs(leftJoystick.x) > 0.05f || Mathf.Abs(leftJoystick.y) > 0.05f) && closePosition == Vector3.zero)
            {
                Vector3 direction = GorillaTagger.Instance.bodyCollider.transform.forward * leftJoystick.y
                                  + GorillaTagger.Instance.bodyCollider.transform.right * leftJoystick.x;

                Vector3 raycastPosition = GorillaTagger.Instance.bodyCollider.transform.position
                    + Vector3.up * 5f
                    + direction * (Time.deltaTime * driveSpeed);
                Physics.Raycast(raycastPosition, Vector3.down, out var Ray, 50f, GTPlayer.Instance.locomotionEnabledLayers);

                Vector3 targetPosition = Ray.point == Vector3.zero ? raycastPosition : Ray.point;

                TeleportPlayer(targetPosition + Vector3.up * (elevatedStickyDrive ? 1f : 0.2f));
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        private static bool previousDash;
        public static void Dash()
        {
            if (rightPrimary && !previousDash)
                GorillaTagger.Instance.rigidbody.linearVelocity += GTPlayer.Instance.headCollider.transform.forward * flySpeed;
            
            previousDash = rightPrimary;
        }

        public static void IronMan()
        {
            Rigidbody rb = GorillaTagger.Instance.rigidbody;

            if (leftPrimary)
            {
                Vector3 leftForce = flySpeed * -GorillaTagger.Instance.leftHandTransform.right;
                rb.AddForce(leftForce * Time.deltaTime, ForceMode.VelocityChange);

                float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 50f * rb.linearVelocity.magnitude;
                GorillaTagger.Instance.StartVibration(true, hapticStrength, GorillaTagger.Instance.tapHapticDuration);
            }

            if (rightPrimary)
            {
                Vector3 rightForce = flySpeed * GorillaTagger.Instance.rightHandTransform.right;
                rb.AddForce(rightForce * Time.deltaTime, ForceMode.VelocityChange);

                float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 50f * rb.linearVelocity.magnitude;
                GorillaTagger.Instance.StartVibration(false, hapticStrength, GorillaTagger.Instance.tapHapticDuration);
            }
        }


        private static float loaoalsode;
        private static BalloonHoldable GetTargetBalloon()
        {
            foreach (BalloonHoldable balloo in GetAllType<BalloonHoldable>())
            {
                if (balloo.IsMyItem())
                    return balloo;
            }
            if (Time.time > loaoalsode)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must equip a balloon.");
                loaoalsode = Time.time + 1f;
            }
            return null;
        }

        public static Vector3 rightgrapplePoint;
        public static Vector3 leftgrapplePoint;
        public static SpringJoint rightjoint;
        public static SpringJoint leftjoint;
        public static bool isLeftGrappling;
        public static bool isRightGrappling;

        public static void SpiderMan()
        {
            if (leftGrab)
            {
                if (!isLeftGrappling)
                {
                    isLeftGrappling = true;
                    GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.leftHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                    } else
                        VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                    
                    RPCProtection();

                    leftgrapplePoint = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;

                    leftjoint = GorillaTagger.Instance.gameObject.AddComponent<SpringJoint>();
                    leftjoint.autoConfigureConnectedAnchor = false;
                    leftjoint.connectedAnchor = leftgrapplePoint;

                    float leftdistanceFromPoint = Vector3.Distance(GorillaTagger.Instance.rigidbody.position, leftgrapplePoint);

                    leftjoint.maxDistance = leftdistanceFromPoint * 0.8f;
                    leftjoint.minDistance = leftdistanceFromPoint * 0.25f;

                    leftjoint.spring = 10f;
                    leftjoint.damper = 50f;
                    leftjoint.massScale = 12f;
                }

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, leftgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Vector3 EndPosition = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                liner.material.shader = Shader.Find("Sprites/Default");
                liner.startColor = backgroundColor.GetCurrentColor() - new Color32(0, 0, 0, 128);
                liner.endColor = backgroundColor.GetCurrentColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, EndPosition);
                Object.Destroy(line, Time.deltaTime);

                isLeftGrappling = false;
                Object.Destroy(leftjoint);
            }

            if (rightGrab)
            {
                if (!isRightGrappling)
                {
                    isRightGrappling = true;
                    GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.rightHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
                        RPCProtection();
                    }
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);

                    rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;

                    if (rightgrapplePoint == Vector3.zero)
                        rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 512f;

                    rightjoint = GorillaTagger.Instance.gameObject.AddComponent<SpringJoint>();
                    rightjoint.autoConfigureConnectedAnchor = false;
                    rightjoint.connectedAnchor = rightgrapplePoint;

                    float rightdistanceFromPoint = Vector3.Distance(GorillaTagger.Instance.rigidbody.position, rightgrapplePoint);

                    rightjoint.maxDistance = rightdistanceFromPoint * 0.8f;
                    rightjoint.minDistance = rightdistanceFromPoint * 0.25f;

                    rightjoint.spring = 10f;
                    rightjoint.damper = 50f;
                    rightjoint.massScale = 12f;
                }

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, rightgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Vector3 EndPosition = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                liner.material.shader = Shader.Find("Sprites/Default");
                liner.startColor = backgroundColor.GetCurrentColor() - new Color32(0, 0, 0, 128);
                liner.endColor = backgroundColor.GetCurrentColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, EndPosition);
                Object.Destroy(line, Time.deltaTime);

                isRightGrappling = false;
                Object.Destroy(rightjoint);
            }
        }

        public static void GrapplingHooks()
        {
            if (leftGrab)
            {
                if (!isLeftGrappling)
                {
                    isLeftGrappling = true;
                    GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.leftHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                    }
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);

                    RPCProtection();
                    leftgrapplePoint = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;
                }
                else
                    GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(leftgrapplePoint - GorillaTagger.Instance.leftHandTransform.position) * 0.5f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, leftgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Vector3 EndPosition = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                liner.material.shader = Shader.Find("Sprites/Default");
                liner.startColor = backgroundColor.GetCurrentColor() - new Color32(0, 0, 0, 128);
                liner.endColor = backgroundColor.GetCurrentColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, EndPosition);
                Object.Destroy(line, Time.deltaTime);

                isLeftGrappling = false;
                Object.Destroy(leftjoint);
            }

            if (rightGrab)
            {
                if (!isRightGrappling)
                {
                    isRightGrappling = true;
                    GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.rightHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
                        RPCProtection();
                    }
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);

                    rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;
                } else
                    GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(rightgrapplePoint - GorillaTagger.Instance.rightHandTransform.position) * 0.5f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, rightgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Vector3 EndPosition = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                if (smoothLines)
                {
                    liner.numCapVertices = 10;
                    liner.numCornerVertices = 5;
                }
                liner.material.shader = Shader.Find("Sprites/Default");
                liner.startColor = backgroundColor.GetCurrentColor() - new Color32(0, 0, 0, 128);
                liner.endColor = backgroundColor.GetCurrentColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, EndPosition);
                Object.Destroy(line, Time.deltaTime);

                isRightGrappling = false;
            }
        }

        public static void DisableSpiderMan()
        {
            isLeftGrappling = false;
            Object.Destroy(leftjoint);
            isRightGrappling = false;
            Object.Destroy(rightjoint);
        }

        public static void NetworkedGrappleMods()
        {
            if (GetIndex("Spider Man").enabled || GetIndex("Grappling Hooks").enabled)
            {
                if (isLeftGrappling || isRightGrappling)
                {
                    BalloonHoldable tb = GetTargetBalloon();

                    tb.balloonState = BalloonHoldable.BalloonStates.Normal;
                    tb.maxDistanceFromOwner = float.MaxValue;

                    tb.rigidbodyInstance.isKinematic = true;
                    tb.gameObject.GetComponent<BalloonDynamics>().stringLength = 512f;
                    tb.gameObject.GetComponent<BalloonDynamics>().stringStrength = 512f;
                    tb.gameObject.GetComponent<BalloonDynamics>().enableDynamics = false;

                    if (tb != null)
                    {
                        if (isLeftGrappling || isRightGrappling)
                        {
                            if (!tb.lineRenderer.enabled)
                                tb.currentState = TransferrableObject.PositionState.InLeftHand;
                        }

                        if (isLeftGrappling)
                        {
                            tb.transform.position = leftgrapplePoint;
                            tb.transform.LookAt(GorillaTagger.Instance.leftHandTransform.position);
                        }
                        else
                        {
                            tb.transform.position = rightgrapplePoint;
                            tb.transform.LookAt(GorillaTagger.Instance.rightHandTransform.position);
                            tb.transform.Rotate(Vector3.left, 90f, Space.Self);
                        }
                    }
                }
                else
                {
                    BalloonHoldable tb = GetTargetBalloon();
                    BalloonHoldable.BalloonStates balloonState = tb.balloonState;

                    if (balloonState != BalloonHoldable.BalloonStates.Pop && balloonState != BalloonHoldable.BalloonStates.Waiting && balloonState != BalloonHoldable.BalloonStates.Refilling && balloonState != BalloonHoldable.BalloonStates.Returning)
                        tb.balloonState = BalloonHoldable.BalloonStates.Normal;
                    
                    tb.rigidbodyInstance.isKinematic = false;
                    tb.gameObject.GetComponent<BalloonDynamics>().stringLength = 0.5f;
                    tb.gameObject.GetComponent<BalloonDynamics>().stringStrength = 0.9f;
                    tb.gameObject.GetComponent<BalloonDynamics>().enableDynamics = true;

                    if (tb != null)
                        tb.currentState = TransferrableObject.PositionState.Dropped;
                }
            }
        }

        public static void UpAndDown()
        {
            if (rightTrigger > 0.5f || rightGrab)
                ZeroGravity();
            
            if (rightTrigger > 0.5f)
                GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.up * (Time.deltaTime * flySpeed * 3f);

            if (rightGrab)
                GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.up * (Time.deltaTime * flySpeed * -3f);
        }

        public static void LeftAndRight()
        {
            if (rightTrigger > 0.5f || rightGrab)
                ZeroGravity();

            if (rightTrigger > 0.5f)
                GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.bodyCollider.transform.right * (Time.deltaTime * flySpeed * -3f);

            if (rightGrab)
                GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.bodyCollider.transform.right * (Time.deltaTime * flySpeed * 3f);
        }

        public static void ForwardsAndBackwards()
        {
            if (rightTrigger > 0.5f || rightGrab)
                ZeroGravity();
            
            if (rightTrigger > 0.5f)
                GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.bodyCollider.transform.forward * (Time.deltaTime * flySpeed * 3f);

            if (rightGrab)
                GorillaTagger.Instance.rigidbody.linearVelocity += GorillaTagger.Instance.bodyCollider.transform.forward * (Time.deltaTime * flySpeed * -3f);
        }

        public static void AutoWalk()
        {
            Vector2 joy = leftJoystick;

            float armLength = 0.45f;
            float animSpeed = 9f;

            if (leftJoystickClick)
                animSpeed *= 1.5f;

            if (Mathf.Abs(joy.y) > 0.05f || Mathf.Abs(joy.x) > 0.05f)
            {
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * (Mathf.Sin(Time.time * animSpeed) * (joy.y * armLength)) + GorillaTagger.Instance.bodyCollider.transform.right * (Mathf.Sin(Time.time * animSpeed) * (joy.x * armLength) - 0.2f) + new Vector3(0f, -0.3f + Mathf.Cos(Time.time * animSpeed) * 0.2f, 0f);
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * (-Mathf.Sin(Time.time * animSpeed) * (joy.y * armLength)) + GorillaTagger.Instance.bodyCollider.transform.right * (-Mathf.Sin(Time.time * animSpeed) * (joy.x * armLength) + 0.2f) + new Vector3(0f, -0.3f + Mathf.Cos(Time.time * animSpeed) * -0.2f, 0f);
            }
        }

        public static void AutoFunnyRun()
        {
            if (rightGrab)
            {
                if (bothHands)
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time) / 10 + new Vector3(0, -0.5f - MathF.Sin(time) / 7, 0) + GorillaTagger.Instance.headCollider.transform.right * -0.05f;
                    GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time + 180) / 10 + new Vector3(0, -0.5f - MathF.Sin(time + 180) / 7, 0) + GorillaTagger.Instance.headCollider.transform.right * 0.05f;
                }
                else
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time) / 10 + new Vector3(0, -0.5f - MathF.Sin(time) / 7, 0);
                }
            }
        }

        public static void AutoPinchClimb()
        {
            if (rightGrab)
            {
                float time = Time.frameCount / 3f;
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.right * (0.4f + MathF.Cos(time) * 0.4f) + GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f) + GorillaTagger.Instance.headCollider.transform.forward * 0.75f;
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.right * -(0.4f + MathF.Cos(time) * 0.4f) + GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f) + GorillaTagger.Instance.headCollider.transform.forward * 0.75f;
            }
        }

        public static void AutoElevatorClimb()
        {
            if (rightGrab)
            {
                float time = Time.frameCount / 3f;
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.right * (0.4f + MathF.Cos(time) * 0.4f) + GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f) + GorillaTagger.Instance.headCollider.transform.forward * 0.75f;
            }
        }

        private static List<Vector3> posArchive;
        public static Vector3[] GetAllTreeBranchPositions()
        {
            if (posArchive != null)
                return posArchive.ToArray();

            posArchive = new List<Vector3>();

            Vector3[] TreeBranchOffsets = {
                new Vector3(-2.383f, 3.784f, 0.738f),
                new Vector3(1.55f, 5.559f, -1.56f),
                new Vector3(-2.225f, 7.214f, 0.063f),
                new Vector3(1.365f, 6.62f, 0.82f),
                new Vector3(0.405f, 8.865f, -2.759f),
                new Vector3(-2.227f, 9.763f, 2.071f),
                new Vector3(2.421f, 10.91f, 1.313f),
                new Vector3(1.618f, 13.169f, -1.216f),
                new Vector3(2.175f, 12.959f, -0.229f),
                new Vector3(1.855f, 13.837f, 1.215f),
                new Vector3(-0.265f, 14.953f, 2.935f),
                new Vector3(-2.049f, 14.962f, -1.708f),
                new Vector3(-1.249f, 18.93f, -1.62f),
            };

            string[] SmallTreeTargets = {
                "Environment Objects/LocalObjects_Prefab/Forest/Terrain/SmallTrees/Group1",
                "Environment Objects/LocalObjects_Prefab/Forest/Terrain/SmallTrees/Group2"
            };

            foreach (string SmallTreeTarget in SmallTreeTargets)
            {
                GameObject TreeGroupGO = GetObject(SmallTreeTarget);

                for (int i = 0; i < TreeGroupGO.transform.childCount; i++)
                {
                    GameObject v = TreeGroupGO.transform.GetChild(i).gameObject;

                    Vector3 oldlocalscale = v.transform.localScale;
                    v.transform.localScale *= 5;

                    foreach (Vector3 TreeBranchOffset in TreeBranchOffsets)
                        posArchive.Add(v.transform.TransformPoint(TreeBranchOffset));
                    
                    v.transform.localScale = oldlocalscale;
                }
            }

            return posArchive.ToArray();
        }

        public static Vector3 leftPos = Vector3.zero;
        public static Vector3 rightPos = Vector3.zero;
        public static bool lastOnBranch;
        public static void AutoBranch()
        {
            bool isOnBranch = false;
            float branchDist = 5f;
            float lerpTime = 0.3f;

            if (rightGrab)
            {
                float dist = float.MaxValue;
                Vector3 closeDist = Vector3.zero;
                Vector3 compareDist = GorillaTagger.Instance.bodyCollider.transform.position;

                foreach (Vector3 treeBranchPos in GetAllTreeBranchPositions())
                {
                    float foundDist = Vector3.Distance(compareDist, treeBranchPos);
                    float dot = Vector3.Dot(treeBranchPos - compareDist, GorillaTagger.Instance.bodyCollider.transform.right);

                    if (foundDist < dist && dot > 0)
                    {
                        dist = foundDist;
                        closeDist = treeBranchPos;
                    }
                }

                if (dist < branchDist)
                {
                    isOnBranch = true;
                    rightPos = Vector3.Lerp(rightPos, closeDist, lerpTime);
                }
                else
                    rightPos = Vector3.Lerp(rightPos, GorillaTagger.Instance.rightHandTransform.position, lerpTime);
                
                GorillaTagger.Instance.rightHandTransform.position = rightPos;

                Vector3 lastFoundDist = closeDist;
                dist = float.MaxValue;
                closeDist = Vector3.zero;
                compareDist = GorillaTagger.Instance.bodyCollider.transform.position;

                foreach (Vector3 treeBranchPos in GetAllTreeBranchPositions())
                {
                    float foundDist = Vector3.Distance(compareDist, treeBranchPos);
                    float dot = Vector3.Dot(treeBranchPos - compareDist, GorillaTagger.Instance.bodyCollider.transform.right);

                    if (foundDist < dist && treeBranchPos != lastFoundDist && dot < 0)
                    {
                        dist = foundDist;
                        closeDist = treeBranchPos;
                    }
                }

                if (dist < branchDist)
                {
                    isOnBranch = true;
                    leftPos = Vector3.Lerp(leftPos, closeDist, lerpTime);
                }
                else
                    leftPos = Vector3.Lerp(leftPos, GorillaTagger.Instance.leftHandTransform.position, lerpTime);

                GorillaTagger.Instance.leftHandTransform.position = leftPos;
            } else
            {
                leftPos = GorillaTagger.Instance.leftHandTransform.position;
                rightPos = GorillaTagger.Instance.rightHandTransform.position;
            }

            if (isOnBranch)
                GorillaTagger.Instance.rigidbody.linearVelocity = GTPlayer.Instance.headCollider.transform.forward * 10f;

            switch (isOnBranch)
            {
                case true when !lastOnBranch:
                    UpdateClipColliders(false);
                    break;
                case false when lastOnBranch:
                    UpdateClipColliders(true);
                    break;
            }

            lastOnBranch = isOnBranch;
        }

        public static void ForceTagFreeze() =>
            GTPlayer.Instance.disableMovement = true;

        public static void NoTagFreeze() =>
            GTPlayer.Instance.disableMovement = false;

        public static void LowGravity() =>
            GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 6.66f, ForceMode.Acceleration);

        public static void ZeroGravity() =>
            GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);

        public static void HighGravity() =>
            GorillaTagger.Instance.rigidbody.AddForce(Vector3.down * 7.77f, ForceMode.Acceleration);

        public static void ReverseGravity()
        {
            GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 19.62f, ForceMode.Acceleration);
            GTPlayer.Instance.rightControllerTransform.parent.rotation = Quaternion.Euler(180f, 0f, 0f);
        }

        public static void UnflipCharacter() =>
            GTPlayer.Instance.rightControllerTransform.parent.rotation = Quaternion.identity;

        private static readonly List<object[]> playerPositions = new List<object[]>();
        public static void Rewind()
        {
            if (rightTrigger > 0.5f)
            {
                if (playerPositions.Count > 0)
                {
                    object[] targetPos = playerPositions[^1];

                    TeleportPlayer((Vector3)targetPos[0]);

                    GorillaTagger.Instance.leftHandTransform.position = (Vector3)targetPos[1];
                    GorillaTagger.Instance.leftHandTransform.rotation = (Quaternion)targetPos[2];

                    GorillaTagger.Instance.rightHandTransform.position = (Vector3)targetPos[3];
                    GorillaTagger.Instance.rightHandTransform.rotation = (Quaternion)targetPos[4];

                    GorillaTagger.Instance.rigidbody.linearVelocity = (Vector3)targetPos[5] * -1f;

                    playerPositions.RemoveAt(playerPositions.Count - 1);
                }
            } else
            {
                playerPositions.Add(new object[] {
                    GorillaTagger.Instance.bodyCollider.transform.position,

                    GorillaTagger.Instance.leftHandTransform.position,
                    GorillaTagger.Instance.leftHandTransform.rotation,

                    GorillaTagger.Instance.rightHandTransform.position,
                    GorillaTagger.Instance.rightHandTransform.rotation,

                    GorillaTagger.Instance.rigidbody.linearVelocity
                });

                if (playerPositions.Count > 8640)
                    playerPositions.RemoveAt(0);
            }
        }

        public static void ClearRewind() => playerPositions.Clear();

        public const float macroStepDuration = 0.1f;

        public static float macroPlaybackRange = 1f;
        public static int macroPlaybackRangeIndex = 1;
        public static void ChangeMacroPlaybackRange(bool positive = true)
        {
            float[] rangeAmounts = { 0.5f, 1f, 2f, 3f, 0.25f };
            string[] rangeNames = { "Small", "Normal", "Large", "Extra Large", "Extra Small", };

            if (positive)
                macroPlaybackRangeIndex++;
            else
                macroPlaybackRangeIndex--;

            macroPlaybackRangeIndex %= rangeNames.Length;
            if (macroPlaybackRangeIndex < 0)
                macroPlaybackRangeIndex = rangeNames.Length - 1;

            macroPlaybackRange = rangeAmounts[macroPlaybackRangeIndex];
            GetIndex("Change Macro Playback Range").overlapText = "Change Macro Playback Range <color=grey>[</color><color=green>" + rangeNames[macroPlaybackRangeIndex] + "</color><color=grey>]</color>";
        }

        public struct PlayerPosition
        {
            public Vector3 position;
            public Vector3 velocity;

            public (Vector3 position, Quaternion rotation) leftHand;
            public (Vector3 position, Quaternion rotation) rightHand;

            public readonly void MoveTo()
            {
                TeleportPlayer(position);
                GorillaTagger.Instance.rigidbody.linearVelocity = velocity;

                GorillaTagger.Instance.leftHandTransform.position = leftHand.position;
                GorillaTagger.Instance.leftHandTransform.rotation = leftHand.rotation;

                GorillaTagger.Instance.rightHandTransform.position = rightHand.position;
                GorillaTagger.Instance.rightHandTransform.rotation = rightHand.rotation;
            }

            public static PlayerPosition CurrentPosition() =>
                new PlayerPosition
                {
                    position = GorillaTagger.Instance.bodyCollider.transform.position,
                    velocity = GorillaTagger.Instance.rigidbody.linearVelocity,

                    leftHand = (GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.rotation),
                    rightHand = (GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation)
                };

            public JObject ToJObject()
            {
                return new JObject
                {
                    ["position"] = Vec3ToJObject(position),
                    ["velocity"] = Vec3ToJObject(velocity),

                    ["leftHand"] = new JObject
                    {
                        ["position"] = Vec3ToJObject(leftHand.position),
                        ["rotation"] = QuatToJObject(leftHand.rotation)
                    },
                    ["rightHand"] = new JObject
                    {
                        ["position"] = Vec3ToJObject(rightHand.position),
                        ["rotation"] = QuatToJObject(rightHand.rotation)
                    }
                };
            }

            public static PlayerPosition FromJObject(JObject obj)
            {
                return new PlayerPosition
                {
                    position = JObjectToVec3((JObject)obj["position"]),
                    velocity = JObjectToVec3((JObject)obj["velocity"]),

                    leftHand = (
                        JObjectToVec3((JObject)obj["leftHand"]["position"]),
                        JObjectToQuat((JObject)obj["leftHand"]["rotation"])
                    ),
                    rightHand = (
                        JObjectToVec3((JObject)obj["rightHand"]["position"]),
                        JObjectToQuat((JObject)obj["rightHand"]["rotation"])
                    )
                };
            }

            private static JObject Vec3ToJObject(Vector3 v) =>
                new JObject { ["x"] = v.x, ["y"] = v.y, ["z"] = v.z };

            private static Vector3 JObjectToVec3(JObject obj) =>
                new Vector3((float)obj["x"], (float)obj["y"], (float)obj["z"]);

            private static JObject QuatToJObject(Quaternion q) =>
                new JObject { ["x"] = q.x, ["y"] = q.y, ["z"] = q.z, ["w"] = q.w };

            private static Quaternion JObjectToQuat(JObject obj) =>
                new Quaternion((float)obj["x"], (float)obj["y"], (float)obj["z"], (float)obj["w"]);
        }


        public struct Macro
        {
            public List<PlayerPosition> positions;
            public string name;
            public bool enabled;

            public readonly string DumpJSON()
            {
                var obj = new JObject
                {
                    ["name"] = name,
                    ["enabled"] = enabled,
                    ["positions"] = new JArray(positions.ConvertAll(p => p.ToJObject()))
                };

                return obj.ToString();
            }

            public static Macro LoadJSON(string json)
            {
                var obj = JObject.Parse(json);

                var macro = new Macro
                {
                    name = (string)obj["name"],
                    enabled = (bool)obj["enabled"],
                    positions = new List<PlayerPosition>()
                };

                foreach (var token in (JArray)obj["positions"])
                    macro.positions.Add(PlayerPosition.FromJObject((JObject)token));

                return macro;
            }
        }

        public static string FormatMacroName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder result = new StringBuilder();
            foreach (char c in input)
                result.Append(char.IsLetterOrDigit(c) ? c : '-');

            return result.ToString();
        }

        public static Dictionary<string, Macro> macros = new Dictionary<string, Macro>();
        public static void LoadMacros()
        {
            macros.Clear();

            string[] files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Macros");
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (!file.EndsWith(".json")) continue;

                string fileName = Path.GetFileNameWithoutExtension(file);
                Macro macro = Macro.LoadJSON(File.ReadAllText(file));

                macros[fileName] = macro;
            }

            List<ButtonInfo> buttons = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Exit Macros", method =() => currentCategoryName = "Movement Mods", isTogglable = false, toolTip = "Returns you back to the movement mods."},
            };

            int index = 0;
            foreach (KeyValuePair<string, Macro> macroData in macros)
            {
                Macro macro = macroData.Value;
                string macroName = macroData.Key;
                
                buttons.Add(new ButtonInfo { buttonText = $"Macro{macroName}", overlapText = macro.name, enabled = macro.enabled, enableMethod =() => ToggleMacro(macroName, true), method =() => ExecuteMacroButton(macro), disableMethod = () => ToggleMacro(macroName, false), toolTip = $"Toggles on and off the {macro.name} macro." });
                index++;
            }

            buttons.AddRange(new[]
            {
                new ButtonInfo { buttonText = "Record <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = RecordMacro, toolTip = "Record your macros with your <color=green>left trigger</color>." },
                new ButtonInfo { buttonText = "Open Macros Folder", method = OpenMacrosFolder, isTogglable = false, toolTip = "Opens the folder in which your plugins are located." },
                new ButtonInfo { buttonText = "Reload Macros", method = LoadMacros, isTogglable = false, toolTip = "Reloads your macros." },
                new ButtonInfo { buttonText = "Disable Macros", enableMethod =() => Movement.disableMacros = true, disableMethod =() => Movement.disableMacros = false, toolTip = "Disables all macros." }
            });
            Buttons.buttons[42] = buttons.ToArray();
        }

        public static void OpenMacrosFolder()
        {
            string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/Macros");
            filePath = filePath.Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/Macros";
            Process.Start(filePath);
        }

        public static void ToggleMacro(string macroName, bool enabled)
        {
            string filePath = $"{PluginInfo.BaseDirectory}/Macros/{macroName}.json";
            if (!File.Exists(filePath))
                return;

            Macro macro = macros[macroName];
            macro.enabled = enabled;

            File.WriteAllText(filePath, macro.DumpJSON());
        }

        public static bool recordingMacro;
        public static float positionDelay;
        public static List<PlayerPosition> recordingData = new List<PlayerPosition>();

        public static void RecordMacro()
        {
            if (leftTrigger > 0.5f)
            {
                if (!recordingMacro)
                {
                    recordingData.Clear();
                    recordingMacro = true;

                    NotifiLib.SendNotification("<color=grey>[</color><color=green>RECORDING</color><color=grey>]</color> Started recording...");
                }

                if (recordingMacro && Time.time > positionDelay)
                {
                    positionDelay = Time.time + macroStepDuration;
                    recordingData.Add(PlayerPosition.CurrentPosition());
                }

                if (recordingMacro && recordingData.Count > 0)
                    VisualizePlayerPosition(recordingData[0], Color.green);
            } else
            {
                if (recordingMacro)
                {
                    recordingMacro = false;

                    NotifiLib.SendNotification("<color=grey>[</color><color=green>RECORDING</color><color=grey>]</color> Stopped recording.");
                    FinalizeRecording();
                }
            }
        }

        public static void FinalizeRecording()
        {
            List<PlayerPosition> savedRecordingData = recordingData;
            Prompt("Would you like to save your macro?", () =>
            {
                PromptText("Please name your macro:", () =>
                {
                    string name = keyboardInput;
                    if (name.IsNullOrEmpty())
                        name = $"Macro #{macros.Count + 1}";

                    Macro macro = new Macro
                    {
                        name = name,
                        positions = savedRecordingData,
                        enabled = true
                    };

                    string filePath = $"{PluginInfo.BaseDirectory}/Macros/{FormatMacroName(name)}.json";

                    File.WriteAllText(filePath, macro.DumpJSON());
                    LoadMacros();
                }, null, "Done", "Cancel");
            });
        }

        public static Coroutine activeMacro;
        public static System.Collections.IEnumerator PlayMacro(Macro macro, int startFromPosition = 0)
        {
            List<PlayerPosition> positions = macro.positions;
            PlayerPosition startPosition = PlayerPosition.CurrentPosition();

            if (startFromPosition > 0 && startFromPosition < positions.Count)
                positions = positions.GetRange(startFromPosition, positions.Count - startFromPosition);

            float macroStartTime = Time.time;
            float macroEndTime = positions.Count * macroStepDuration;

            while (Time.time < macroStartTime + macroEndTime)
            {
                if (rightTrigger < 0.5f)
                {
                    StopMacro();
                    yield break;
                }

                float elapsed = Time.time - macroStartTime;
                float stepElapsed = elapsed % macroStepDuration;

                int currentMacroPosition = Mathf.FloorToInt(elapsed / macroStepDuration);

                currentMacroPosition = Mathf.Clamp(currentMacroPosition, 0, positions.Count);

                PlayerPosition lastPosition = currentMacroPosition - 1 < 0 ? startPosition : positions[currentMacroPosition - 1];
                PlayerPosition currentPosition = positions[currentMacroPosition];

                float t = stepElapsed / macroStepDuration;
                TeleportPlayer(lastPosition.position.Lerp(currentPosition.position, t));
                GorillaTagger.Instance.rigidbody.linearVelocity = lastPosition.velocity.Lerp(currentPosition.velocity, t);

                GorillaTagger.Instance.leftHandTransform.position = lastPosition.leftHand.position.Lerp(currentPosition.leftHand.position, t);
                GorillaTagger.Instance.leftHandTransform.rotation = lastPosition.leftHand.rotation.Lerp(currentPosition.leftHand.rotation, t);

                GorillaTagger.Instance.rightHandTransform.position = lastPosition.rightHand.position.Lerp(currentPosition.rightHand.position, t);
                GorillaTagger.Instance.rightHandTransform.rotation = lastPosition.rightHand.rotation.Lerp(currentPosition.rightHand.rotation, t);

                NotifiLib.information["Macro"] = $"{macroEndTime - elapsed:F1}s";

                yield return null;

                if (currentMacroPosition + 5 < positions.Count)
                {
                    PlayerPosition futurePosition = positions[currentMacroPosition + 5];
                    VisualizePositionCoroutine(futurePosition, Color.cyan);
                }
                else
                    RemovePosition(Color.cyan);

                VisualizePositionCoroutine(positions[^1], Color.red);
            }

            StopMacro();
            yield break;
        }

        public static void StopMacro()
        {
            if (activeMacro != null)
            {
                CoroutineManager.instance.StopCoroutine(activeMacro);
                activeMacro = null;
            }

            NotifiLib.information.Remove("Macro");

            RemovePosition(Color.cyan);
            RemovePosition(Color.red);
        }

        public static void VisualizePlayerPosition(PlayerPosition position, Color color, float alpha = 0.15f)
        {
            VisualizeCube(position.position, Quaternion.LookRotation(position.velocity), new Vector3(0.1f, 0.1f, 0.25f), color, alpha);
            VisualizeCube(position.position + position.velocity.normalized * 0.125f, Quaternion.LookRotation(position.velocity), new Vector3(0.15f, 0.15f, 0.05f), color, alpha);
            VisualizeAura(position.leftHand.position, 0.15f, color, null, alpha);
            VisualizeAura(position.rightHand.position, 0.15f, color, null, alpha);
        }

        // Unity decided to vomit on my day and not let me run my VisualizeCube or VisualizeAura methods properly, so here's my bad workaround.
        public static Dictionary<Color, (GameObject head, GameObject leftHand, GameObject rightHand)> positions = new Dictionary<Color, (GameObject head, GameObject leftHand, GameObject rightHand)>();
        public static void VisualizePositionCoroutine(PlayerPosition position, Color color)
        {
            if (!positions.TryGetValue(color, out var data))
            {
                data = (VisualizeCubeObject(position.position, Quaternion.LookRotation(position.velocity), new Vector3(0.1f, 0.1f, 0.25f), color), VisualizeAuraObject(position.leftHand.position, 0.15f, color), VisualizeAuraObject(position.rightHand.position, 0.15f, color));
                positions[color] = data;
            }

            data.head.transform.position = position.position;
            data.head.transform.rotation = Quaternion.LookRotation(position.velocity);
            data.leftHand.transform.position = position.leftHand.position;
            data.rightHand.transform.position = position.rightHand.position;
        }

        public static void RemovePosition(Color color)
        {
            if (positions.TryGetValue(color, out var data))
            {
                Object.Destroy(data.head);
                Object.Destroy(data.leftHand);
                Object.Destroy(data.rightHand);

                positions.Remove(color);
            }
        }

        public static bool midpointMacros;
        public static bool didMacro;
        public static bool directionBased;
        public static bool disableMacros;
        public static void ExecuteMacroButton(Macro macro)
        {
            if (disableMacros)
                return;
            
            didMacro = midpointMacros && (didMacro
                ? rightTrigger >= 0.5f
                : activeMacro != null);

            if (rightTrigger < 0.5f || activeMacro != null || didMacro)
                return;

            int position = 0;
            if (midpointMacros)
            {
                position = macro.positions
                    .Select((position, index) => new { position, index, distance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position.position) })
                    .OrderBy(x => x.distance)
                    .FirstOrDefault()
                    .index;
            }

            PlayerPosition startPosition = macro.positions[position];
            bool doMacro = !directionBased || (GorillaTagger.Instance.rigidbody.linearVelocity.magnitude > 2f && Vector3.Angle(startPosition.velocity.normalized, GorillaTagger.Instance.rigidbody.linearVelocity.normalized) < 70f);

            VisualizePlayerPosition(startPosition, doMacro ? buttonColors[1].GetCurrentColor() : Color.white, doMacro ? 0.15f : 0.05f);
            if (doMacro) VisualizeAura(startPosition.position, 1f, buttonColors[1].GetCurrentColor(), null, 0.05f);

            if (!doMacro)
                return;

            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, startPosition.position) < 1f)
                activeMacro = CoroutineManager.instance.StartCoroutine(PlayMacro(macro, position));
        }

        private static Vector3 walkPos;
        private static Vector3 walkNormal;

        public static int wallWalkStrengthIndex = 2;

        public static bool leftWallWalk;
        public static bool bothWallWalk;

        public static void ChangeWallWalkStrength(bool positive = true)
        {
            float[] strengthAmounts = { 2f, 5f, 9.81f, 15f, 50f };
            string[] strengthNames = { "Very Weak", "Weak", "Normal", "Strong", "Very Strong" };

            if (positive)
                wallWalkStrengthIndex++;
            else
                wallWalkStrengthIndex--;

            wallWalkStrengthIndex %= strengthAmounts.Length;
            if (wallWalkStrengthIndex < 0)
                wallWalkStrengthIndex = strengthAmounts.Length - 1;

            GetIndex("Change Wall Walk Strength").overlapText = "Change Wall Walk Strength <color=grey>[</color><color=green>" + strengthNames[wallWalkStrengthIndex] + "</color><color=grey>]</color>";
        }

        public static void WallWalk()
        {
            if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            bool wallWalkKey = 
                bothWallWalk ? leftGrab || rightGrab :
                leftWallWalk ? leftGrab : rightGrab;

            if (walkPos != Vector3.zero && wallWalkKey)
            {
                GorillaTagger.Instance.rigidbody.AddForce(walkNormal * -9.81f, ForceMode.Acceleration);
                ZeroGravity();
            }
        }

        // Credits to Intelligence for the idea (You can't code a mod menu, but you are insanely good at making competitive cheats)
        public static void LegitimateWallWalk()
        {
            float range = 0.2f;
            float power = -2f;

            if (leftGrab && (leftWallWalk || bothHands))
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;

                if (Physics.Raycast(TrueLeftHand().position, -ray.normal, out var Ray, range, GTPlayer.Instance.locomotionEnabledLayers))
                    GorillaTagger.Instance.rigidbody.AddForce(Ray.normal * power, ForceMode.Acceleration);
            }

            if (rightGrab && (!leftWallWalk || bothHands))
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;

                if (Physics.Raycast(TrueRightHand().position, -ray.normal, out var Ray, range, GTPlayer.Instance.locomotionEnabledLayers))
                    GorillaTagger.Instance.rigidbody.AddForce(Ray.normal * power, ForceMode.Acceleration);
            }
        }

        public static void SpiderWalk()
        {
            if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
            {
                RaycastHit ray = GTPlayer.Instance.lastHitInfoHand;
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            if (walkPos != Vector3.zero)
            {
                GorillaTagger.Instance.rigidbody.AddForce(walkNormal * -9.81f, ForceMode.Acceleration);
                GTPlayer.Instance.rightControllerTransform.parent.rotation = Quaternion.Lerp(GTPlayer.Instance.rightControllerTransform.parent.rotation, Quaternion.LookRotation(walkNormal) * Quaternion.Euler(90f, 0f, 0f), Time.deltaTime);
                ZeroGravity();
            }
        }

        public static void TeleportToRandom()
        {
            closePosition = Vector3.zero;
            TeleportPlayer(GetRandomVRRig(false).transform.position);
            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
        }

        public static void TeleportToPlayer(NetPlayer plr) => TeleportPlayer(GetVRRigFromPlayer(plr).headMesh.transform.position);

        public static void ExitTeleportToMap()
        {
            currentCategoryName = "Movement Mods";
            pageNumber = rememberPageNumber;
        }

        private static int rememberPageNumber;
        public static readonly string[][] mapData = {
            new[] // Forest
            {
                "Forest",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/TreeRoomSpawnForestZone",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit"
            },
            new[] // City
            {
                "City",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCity",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front"
            },
            new[] // Canyons
            {
                "Canyons",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestCanyonTransition",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon"
            },
            new[] // Clouds
            {
                "Clouds",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToSkyJungle",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds From Computer"
            },
            new[] // Caves
            {
                "Caves",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCave",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave"
            },
            new[] // Beach
            {
                "Beach",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BeachToForest",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach for Computer"
            },
            new[] // Mountains
            {
                "Mountains",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToMountain",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain"
            },
            new[] // Basement
            {
                "Basement",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToBasement",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer"
            },
            new[] // Metropolis
            {
                "Metropolis",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/MetropolisOnly",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Metropolis from Computer"
            },
            new[] // Arcade
            {
                "Arcade",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToArcade",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City frm Arcade"
            },
            new[] // Rotating
            {
                "Rotating",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToRotating",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Rotating Map"
            },
            new[] // Bayou
            {
                "Bayou",
                "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BayouOnly",
                "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2"
            },
            new[] // Virtual Stump
            {
                "Virtual Stump",
                "VSTUMP",
                "VSTUMP"
            },
        };

        public static void EnterTeleportToMap() // Credits to Malachi for the positions
        {
            rememberPageNumber = pageNumber;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> tpbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Teleport to Map", method = ExitTeleportToMap, isTogglable = false, toolTip = "Returns you back to the movement mods." } };

            foreach (string[] Data in mapData)
                tpbuttons.Add(new ButtonInfo { buttonText = "TeleportMap" + tpbuttons.Count, overlapText = Data[0], method = () => TeleportToMap(Data[1], Data[2]), isTogglable = false, toolTip = "Teleports you to the " + Data[0] + " map." });
            
            Buttons.buttons[29] = tpbuttons.ToArray();
        }

        public static void TeleportToMap(string zone, string pos)
        {
            if (zone == "VSTUMP")
            {
                VirtualStumpTeleporter tele = GetObject("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/TeleportTriggers_1/VRHeadsetTrigger_1").GetComponent<VirtualStumpTeleporter>();

                tele.gameObject.transform.parent.parent.parent.parent.parent.parent.gameObject.SetActive(true); // wtf
                tele.gameObject.transform.parent.parent.parent.parent.gameObject.SetActive(true);

                tele.TeleportPlayer();
            } else
            {
                GetObject(zone).GetComponent<GorillaSetZoneTrigger>().OnBoxTriggered();
                TeleportPlayer(GetObject(pos).transform.position);
            }
        }

        public static bool previousTeleportTrigger;
        public static void TeleportGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && !previousTeleportTrigger)
                {
                    closePosition = Vector3.zero;
                    TeleportPlayer(NewPointer.transform.position + Vector3.up);
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                }

                previousTeleportTrigger = GetGunInput(true);
            }
        }

        public static void Airstrike()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && !previousTeleportTrigger)
                {
                    GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, -20f, 0f);
                    TeleportPlayer(NewPointer.transform.position + new Vector3(0f, 30f, 0f));
                    GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, -20f, 0f);
                }

                previousTeleportTrigger = GetGunInput(true);
            }
        }

        public static GameObject CheckPoint;
        public static void Checkpoint()
        {
            if (rightGrab)
            {
                if (CheckPoint == null)
                {
                    CheckPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(CheckPoint.GetComponent<SphereCollider>());
                    CheckPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                CheckPoint.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (CheckPoint != null)
            {
                if (rightPrimary)
                {
                    CheckPoint.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
                    TeleportPlayer(CheckPoint.transform.position);
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                }
                else
                    CheckPoint.GetComponent<Renderer>().material.color = buttonColors[0].GetColor(0);
            }
        }

        public static void DisableCheckpoint()
        {
            if (CheckPoint != null)
            {
                Object.Destroy(CheckPoint);
                CheckPoint = null;
            }
        }

        public static int selectedCheckpoint = -1;
        public static float selectedCheckpointDelay;
        public static readonly List<GameObject> checkpoints = new List<GameObject>();
        public static void AdvancedCheckpoints()
        {
            if (rightGrab)
            {
                bool isNearCheckpoint = false;
                foreach (var checkpoint in checkpoints.Where(checkpoint => Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.2f))
                {
                    isNearCheckpoint = true;
                    checkpoint.transform.position = GorillaTagger.Instance.rightHandTransform.transform.position;
                    break;
                }

                if (!isNearCheckpoint)
                {
                    GameObject newCheckpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(newCheckpoint.GetComponent<SphereCollider>());
                    newCheckpoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    newCheckpoint.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    newCheckpoint.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    newCheckpoint.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();

                    GameObject MeshHolder = new GameObject("Label");
                    MeshHolder.transform.parent = newCheckpoint.transform;
                    MeshHolder.transform.localPosition = Vector3.zero;
                    TextMesh newMesh = MeshHolder.AddComponent<TextMesh>();

                    Renderer MeshRender = newMesh.GetComponent<Renderer>();
                    MeshRender.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    newMesh.fontSize = 12;
                    newMesh.fontStyle = activeFontStyle;
                    newMesh.characterSize = 0.1f;
                    newMesh.anchor = TextAnchor.MiddleCenter;
                    newMesh.alignment = TextAlignment.Center;
                    newMesh.color = Color.white;
                    newMesh.text = (checkpoints.Count + 1).ToString();

                    MeshRender.material.renderQueue = newCheckpoint.GetComponent<Renderer>().material.renderQueue + 2;
                    checkpoints.Add(newCheckpoint);
                }
            }

            if (rightTrigger > 0.5f)
            {
                foreach (var checkpoint in checkpoints.ToList().Where(checkpoint => Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.2f))
                {
                    checkpoints.Remove(checkpoint);
                    Object.Destroy(checkpoint);
                }
            }

            if (rightPrimary)
                TeleportPlayer(checkpoints[selectedCheckpoint].transform.position);

            foreach (GameObject checkpoint in checkpoints)
            {
                checkpoint.GetComponent<Renderer>().material.color = backgroundColor.GetCurrentColor();

                GameObject textObject = checkpoint.transform.Find("Label").gameObject;
                textObject.transform.LookAt(Camera.main.transform.position);
                textObject.transform.Rotate(0f, 180f, 0f);
            }

            if (Mathf.Abs(rightJoystick.y) > 0.5f && Time.time > selectedCheckpointDelay)
            {
                selectedCheckpointDelay = Time.time + 0.2f;
                selectedCheckpoint += rightJoystick.y > 0 ? 1 : -1;

                if (selectedCheckpoint < 0)
                    selectedCheckpoint = checkpoints.Count - 1;
            }

            if (selectedCheckpoint < 0 && checkpoints.Count > 0)
                selectedCheckpoint = 0;

            if (selectedCheckpoint > checkpoints.Count - 1)
                selectedCheckpoint = 0;

            GameObject go = new GameObject("Lbl");
            if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            TextMesh textMesh = go.AddComponent<TextMesh>();
            textMesh.color = GorillaTagger.Instance.rigidbody.linearVelocity.magnitude >= GTPlayer.Instance.maxJumpSpeed ? Color.green : Color.white;
            textMesh.fontSize = 24;
            textMesh.fontStyle = activeFontStyle;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.text = (selectedCheckpoint + 1).ToString();

            go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.1f, 0f);
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
            Object.Destroy(go, Time.deltaTime);
        }

        public static void DisableAdvancedCheckpoints()
        {
            selectedCheckpoint = 0;

            foreach (GameObject checkpoint in checkpoints)
                Object.Destroy(checkpoint);
            
            checkpoints.Clear();
        }

        public static GameObject BombObject;
        public static void Bomb()
        {
            if (rightGrab)
            {
                if (BombObject == null)
                {
                    BombObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Object.Destroy(BombObject.GetComponent<SphereCollider>());
                    BombObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                BombObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (BombObject != null)
            {
                if (rightPrimary)
                {
                    Vector3 dir = GorillaTagger.Instance.bodyCollider.transform.position - BombObject.transform.position;
                    dir.Normalize();
                    GorillaTagger.Instance.rigidbody.linearVelocity += 25f * dir;
                    Object.Destroy(BombObject);
                    BombObject = null;
                }
                else
                    BombObject.GetComponent<Renderer>().material.color = buttonColors[0].GetColor(0);
            }
        }

        public static void DisableBomb()
        {
            if (BombObject != null)
            {
                Object.Destroy(BombObject);
                BombObject = null;
            }
        }

        private static GameObject pearl;
        private static Texture2D pearltxt;
        private static Material pearlmat;
        private static bool isrighthandedpearl;
        public static void EnderPearl()
        {
            if (rightGrab || leftGrab)
            {
                if (pearl == null)
                {
                    pearl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.Destroy(pearl.GetComponent<Collider>());

                    pearl.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    if (pearlmat == null)
                    {
                        pearlmat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                        {
                            color = Color.white
                        };
                        if (pearltxt == null)
                        {
                            pearltxt = LoadTextureFromURL($"{PluginInfo.ServerResourcePath}/Images/Mods/Movement/pearl.png", "Images/Mods/Movement/pearl.png");
                            pearltxt.filterMode = FilterMode.Point;
                            pearltxt.wrapMode = TextureWrapMode.Clamp;
                        }
                        pearlmat.mainTexture = pearltxt;

                        pearlmat.SetFloat("_Surface", 1);
                        pearlmat.SetFloat("_Blend", 0);
                        pearlmat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        pearlmat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        pearlmat.SetFloat("_ZWrite", 0);
                        pearlmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        pearlmat.renderQueue = (int)RenderQueue.Transparent;
                    }
                    pearl.GetComponent<Renderer>().material = pearlmat;
                }

                if (pearl.GetComponent<Rigidbody>() != null)
                    Object.Destroy(pearl.GetComponent<Rigidbody>());
                
                isrighthandedpearl = rightGrab;
                pearl.transform.position = rightGrab ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position;
            } else
            {
                if (pearl != null)
                {
                    if (pearl.GetComponent<Rigidbody>() == null)
                    {
                        Rigidbody comp = pearl.AddComponent(typeof(Rigidbody)) as Rigidbody;
                        comp.linearVelocity = isrighthandedpearl ? GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0) : GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    }
                    Physics.Raycast(pearl.transform.position, pearl.GetComponent<Rigidbody>().linearVelocity, out var Ray, 0.25f, GTPlayer.Instance.locomotionEnabledLayers);
                    if (Ray.collider != null)
                    {
                        TeleportPlayer(pearl.transform.position);
                        if (PhotonNetwork.InRoom)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 84, true, 999999f);
                        else
                            VRRig.LocalRig.PlayHandTapLocal(84, true, 999999f);
                        
                        RPCProtection();
                        Object.Destroy(pearl);
                    }
                }
            }
            if (pearl != null)
            {
                pearl.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                pearl.GetComponent<Rigidbody>()?.AddForce(Vector3.up * (Time.deltaTime * (6.66f / Time.deltaTime)), ForceMode.Acceleration);
            }
        }

        public static void DestroyEnderPearl()
        {
            if (pearl != null)
                Object.Destroy(pearl);
        }

        public static void SpeedBoost()
        {
            float jspt = jspeed;
            float jmpt = jmulti;

            if (GetIndex("Factored Speed Boost").enabled)
            {
                jspt = jspt / 6.5f * GTPlayer.Instance.maxJumpSpeed;
                jmpt = jmpt / 1.1f * GTPlayer.Instance.jumpMultiplier;
            }

            if (!GetIndex("Disable Max Speed Modification").enabled)
                GTPlayer.Instance.maxJumpSpeed = jspt;

            GTPlayer.Instance.jumpMultiplier = jmpt;
        }

        public static float funMoveDelay;
        public static Vector3 funLastVel;
        public static void FunMove()
        {
            if (Time.time > funMoveDelay)
            {
                funMoveDelay = Time.time + 0.2f;
                GorillaTagger.Instance.rigidbody.linearVelocity = (GorillaTagger.Instance.rigidbody.transform.position - funLastVel) * 6f;
                funLastVel = GorillaTagger.Instance.rigidbody.transform.position;
            }
        }
        public static void VelocityMultiplier()
        {
            if (Time.time > funMoveDelay)
            {
                funMoveDelay = Time.time + 0.05f;
                GorillaTagger.Instance.rigidbody.linearVelocity = (GorillaTagger.Instance.rigidbody.transform.position - funLastVel) * 5.17f;
                funLastVel = GorillaTagger.Instance.rigidbody.transform.position;
            }
        } 

        public static void DynamicSpeedBoost()
        {
            bool isTagged = PlayerIsTagged(VRRig.LocalRig);

            VRRig closestRig = GorillaParent.instance.vrrigs
                .Where(rig => rig != null && !rig.isLocal && 
                                  (isTagged ? !PlayerIsTagged(rig) : PlayerIsTagged(rig)))
                .OrderBy(rig => Vector3.Distance(rig.transform.position, GorillaTagger.Instance.bodyCollider.transform.position))
                .FirstOrDefault();

            float rigDistance = closestRig == null ? float.MaxValue :
                          Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, closestRig.transform.position);

            if (rigDistance < 15f)
            {
                float jspt = jspeed;
                float jmpt = jmulti;

                if (GetIndex("Factored Speed Boost").enabled)
                {
                    jspt = jspt / 6.5f * GTPlayer.Instance.maxJumpSpeed;
                    jmpt = jmpt / 1.1f * GTPlayer.Instance.jumpMultiplier;
                }

                jspt = Mathf.Lerp(GTPlayer.Instance.maxJumpSpeed, jspt, Mathf.Clamp(rigDistance, 1f, 15f) / 15f);
                jmpt = Mathf.Lerp(GTPlayer.Instance.jumpMultiplier, jmpt, Mathf.Clamp(rigDistance, 1f, 15f) / 15f);

                if (!GetIndex("Disable Max Speed Modification").enabled)
                    GTPlayer.Instance.maxJumpSpeed = jspt;

                GTPlayer.Instance.jumpMultiplier = jmpt;
            }
        }

        public static void AlwaysMaxVelocity()
        {
            if (GetIndex("Uncap Max Velocity").enabled)
                Toggle("Uncap Max Velocity");
            else
                GTPlayer.Instance.jumpMultiplier = 99999f;
        }

        public static Playspace playspace;
        public static void DisableVelocityCap()
        {
            playspace = GetAllType<Playspace>().FirstOrDefault();
            playspace.enabled = false;
        }

        public static void UpdateClipColliders(bool enabled)
        {
            foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                v.enabled = enabled;
        }

        public static void Noclip()
        {
            bool gripNoclip = GetIndex("Grip Noclip").enabled;
            if (gripNoclip ? rightGrab : rightTrigger > 0.5f || UnityInput.Current.GetKey(KeyCode.E))
            {
                if (!noclip)
                {
                    noclip = true;
                    UpdateClipColliders(false);
                }
            }
            else
            {
                if (noclip)
                {
                    noclip = false;
                    UpdateClipColliders(true);
                }
            }
        }

        public static bool wasDisabledAlready;
        public static bool invisMonke;
        public static void Invisible()
        {
            bool hit = rightSecondary;
            if (GetIndex("Non-Togglable Invisible").enabled)
                invisMonke = hit;
            if (invisMonke)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 99999f;
            }
            if (hit && !lastHit2)
            {
                invisMonke = !invisMonke;
                if (invisMonke)
                    wasDisabledAlready = VRRig.LocalRig.enabled;
                else
                    VRRig.LocalRig.enabled = wasDisabledAlready;
            }
            lastHit2 = hit;
        }

        private static bool ghostMonke;
        public static void Ghost()
        {
            bool hit = rightPrimary;
            if (GetIndex("Non-Togglable Ghost").enabled)
                ghostMonke = hit;
            
            VRRig.LocalRig.enabled = !ghostMonke;
            if (hit && !lastHit)
                ghostMonke = !ghostMonke;
            
            lastHit = hit;
        }

        public static void EnableRig()
        {
            VRRig.LocalRig.enabled = true;
            ghostException = false;
        }

        public static void RigGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position + new Vector3(0, 1, 0);
                }
                else
                    VRRig.LocalRig.enabled = true;
            }
        }

        private static Quaternion grabHeadRot;

        private static Vector3 grabLeftHandPos;
        private static Quaternion grabLeftHandRot;

        private static Vector3 grabRightHandPos;
        private static Quaternion grabRightHandRot;

        public static void GrabRig()
        {
            if (rightGrab)
            {
                if (grabHeadRot == Quaternion.identity)
                    grabHeadRot = VRRig.LocalRig.transform.InverseTransformRotation(VRRig.LocalRig.head.rigTarget.transform.rotation);

                if (grabLeftHandPos == Vector3.zero)
                    grabLeftHandPos = VRRig.LocalRig.transform.InverseTransformPoint(VRRig.LocalRig.leftHand.rigTarget.transform.position);

                if (grabLeftHandRot == Quaternion.identity)
                    grabLeftHandRot = VRRig.LocalRig.transform.InverseTransformRotation(VRRig.LocalRig.leftHand.rigTarget.transform.rotation);

                if (grabRightHandPos == Vector3.zero)
                    grabRightHandPos = VRRig.LocalRig.transform.InverseTransformPoint(VRRig.LocalRig.rightHand.rigTarget.transform.position);

                if (grabRightHandRot == Quaternion.identity)
                    grabRightHandRot = VRRig.LocalRig.transform.InverseTransformRotation(VRRig.LocalRig.rightHand.rigTarget.transform.rotation);

                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                VRRig.LocalRig.transform.rotation = Quaternion.Euler(new Vector3(0f, GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles.y, 0f));

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.TransformRotation(grabHeadRot);

                VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.rightHandTransform.TransformPoint(grabLeftHandPos);
                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.TransformRotation(grabLeftHandRot);

                VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.rightHandTransform.TransformPoint(grabRightHandPos);
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.TransformRotation(grabRightHandRot);
            }
            else
            {
                VRRig.LocalRig.enabled = true;

                grabHeadRot = Quaternion.identity;

                grabLeftHandPos = Vector3.zero;
                grabRightHandPos = Vector3.zero;

                grabLeftHandRot = Quaternion.identity;
                grabRightHandRot = Quaternion.identity;
            }
        }

        public static Vector3 offsetLH = Vector3.zero;
        public static Vector3 offsetRH = Vector3.zero;
        public static Vector3 offsetH = Vector3.zero;
        public static void EnableSpazRig()
        {
            ghostException = true;
            offsetLH = VRRig.LocalRig.leftHand.trackingPositionOffset;
            offsetRH = VRRig.LocalRig.rightHand.trackingPositionOffset;
            offsetH = VRRig.LocalRig.head.trackingPositionOffset;
        }

        public static void SpazRig()
        {
            if (rightPrimary)
            {
                float spazAmount = 0.1f;
                ghostException = true;
                VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH + RandomVector3(spazAmount);
                VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH + RandomVector3(spazAmount);
                VRRig.LocalRig.head.trackingPositionOffset = offsetH + RandomVector3(spazAmount);
            }
            else
            {
                ghostException = false;
                VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH;
                VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH;
                VRRig.LocalRig.head.trackingPositionOffset = offsetH;
            }
        }

        public static void DisableSpazRig()
        {
            ghostException = false;
            VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH;
            VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH;
            VRRig.LocalRig.head.trackingPositionOffset = offsetH;
        }

        public static void SpazHands()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.leftHandTransform.position + VRRig.LocalRig.leftHand.rigTarget.transform.forward * 3f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.rightHandTransform.position + VRRig.LocalRig.rightHand.rigTarget.transform.forward * 3f;
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void SpazRealHands()
        {
            if (rightPrimary)
            {
                GTPlayer.Instance.leftControllerTransform.rotation = RandomQuaternion();
                GTPlayer.Instance.leftControllerTransform.position = GorillaTagger.Instance.leftHandTransform.position + GTPlayer.Instance.leftControllerTransform.forward * 3f;

                GTPlayer.Instance.rightControllerTransform.rotation = RandomQuaternion();
                GTPlayer.Instance.rightControllerTransform.position = GorillaTagger.Instance.rightHandTransform.position + GTPlayer.Instance.rightControllerTransform.forward * 3f;
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void FreezeRigLimbs()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void FixRigHandRotation()
        {
            VRRig.LocalRig.leftHand.rigTarget.transform.rotation *= Quaternion.Euler(VRRig.LocalRig.leftHand.trackingRotationOffset);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation *= Quaternion.Euler(VRRig.LocalRig.rightHand.trackingRotationOffset);
        }

        public static void FreezeRigBody()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                var leftHandTransform = TrueLeftHand();
                var rightHandTransform = TrueRightHand();

                VRRig.LocalRig.leftHand.rigTarget.transform.position = leftHandTransform.position;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = rightHandTransform.position;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = leftHandTransform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = rightHandTransform.rotation;

                FixRigHandRotation();

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void ParalyzeRig()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.08f + GorillaTagger.Instance.bodyCollider.transform.up * 0.12f;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.08f + GorillaTagger.Instance.bodyCollider.transform.up * 0.12f;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 180f);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 180f);
        }

        public static void ChickenRig()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        
            VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.2f + GorillaTagger.Instance.bodyCollider.transform.up * -0.2f;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.2f + GorillaTagger.Instance.bodyCollider.transform.up * -0.2f;
        
            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
        }

        public static void AmputateRig()
        {
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(160f, 90f, 0f);

            VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.08f + GorillaTagger.Instance.bodyCollider.transform.up * 0.12f;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.08f + GorillaTagger.Instance.bodyCollider.transform.up * 0.12f;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 180f);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 180f);
        }

        public static void SetBodyPatch(bool enabled, int mode = 0)
        {
            TorsoPatch.enabled = enabled;
            TorsoPatch.mode = mode;

            if (!enabled && recBodyRotary != null)
                Object.Destroy(recBodyRotary);
        }

        public static GameObject recBodyRotary;
        public static void RecRoomBody()
        {
            SetBodyPatch(true, 3);

            if (recBodyRotary == null)
                recBodyRotary = new GameObject("ii_recBodyRotary");

            recBodyRotary.transform.rotation = Quaternion.Lerp(recBodyRotary.transform.rotation, Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 6.5f);
        }

        public static void FreezeBodyRotation()
        {
            SetBodyPatch(true, 3);

            if (recBodyRotary == null)
                recBodyRotary = new GameObject("ii_recBodyRotary");

            recBodyRotary.transform.rotation = rightGrab ? recBodyRotary.transform.rotation : Quaternion.Euler(0f, GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles.y, 0f);
        }

        public static void AutoDance()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                Vector3 bodyOffset = GorillaTagger.Instance.bodyCollider.transform.right * (Mathf.Cos(Time.frameCount / 20f) * 0.3f) + new Vector3(0f, Mathf.Abs(Mathf.Sin(Time.frameCount / 20f) * 0.2f), 0f);
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f) + bodyOffset;
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                
                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.forward * 0.2f + VRRig.LocalRig.transform.right * -0.4f + VRRig.LocalRig.transform.up * (0.3f + Mathf.Sin(Time.frameCount / 20f) * 0.2f);
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.forward * 0.2f + VRRig.LocalRig.transform.right * 0.4f + VRRig.LocalRig.transform.up * (0.3f + Mathf.Sin(Time.frameCount / 20f) * -0.2f);

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void AutoGriddy()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                Vector3 bodyOffset = VRRig.LocalRig.transform.forward * (5f * Time.deltaTime);
                VRRig.LocalRig.transform.position = VRRig.LocalRig.transform.position + bodyOffset;
                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -0.33f + VRRig.LocalRig.transform.forward * (0.5f * Mathf.Cos(Time.frameCount / 10f)) + VRRig.LocalRig.transform.up * (-0.5f * Mathf.Abs(Mathf.Sin(Time.frameCount / 10f)));
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 0.33f + VRRig.LocalRig.transform.forward * (0.5f * Mathf.Cos(Time.frameCount / 10f)) + VRRig.LocalRig.transform.up * (-0.5f * Mathf.Abs(Mathf.Sin(Time.frameCount / 10f)));

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void AutoTPose()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void Helicopter()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position += new Vector3(0f, 0.05f, 0f);
                VRRig.LocalRig.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));

                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static void Beyblade()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                VRRig.LocalRig.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));

                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static Vector3 stillBeybladeStartPos = Vector3.zero;
        public static void StillBeyblade()
        {
            if (rightPrimary)
            {
                if (stillBeybladeStartPos == Vector3.zero)
                    stillBeybladeStartPos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = stillBeybladeStartPos;
                VRRig.LocalRig.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));

                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
            {
                stillBeybladeStartPos = Vector3.zero;
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void Fan()
        {
            if (rightPrimary)
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + (VRRig.LocalRig.transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + VRRig.LocalRig.transform.right * (Mathf.Sin(Time.time * 15f) * 2f));
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position - (VRRig.LocalRig.transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + VRRig.LocalRig.transform.right * (Mathf.Sin(Time.time * 15f) * 2f));

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        private static Vector3 headPos = Vector3.zero;
        private static Vector3 headRot = Vector3.zero;

        private static Vector3 handPos_L = Vector3.zero;
        private static Vector3 handRot_L = Vector3.zero;

        private static Vector3 handPos_R = Vector3.zero;
        private static Vector3 handRot_R = Vector3.zero;

        public static void GhostAnimations()
        {
            VRRig.LocalRig.enabled = false;

            if (headPos == Vector3.zero)
                headPos = GorillaTagger.Instance.headCollider.transform.position;
            if (headRot == Vector3.zero)
                headRot = GorillaTagger.Instance.headCollider.transform.rotation.eulerAngles;

            if (handPos_L == Vector3.zero)
                handPos_L = GorillaTagger.Instance.leftHandTransform.transform.position;
            if (handRot_L == Vector3.zero)
                handRot_L = GorillaTagger.Instance.leftHandTransform.transform.rotation.eulerAngles;

            if (handPos_R == Vector3.zero)
                handPos_R = GorillaTagger.Instance.rightHandTransform.transform.position;
            if (handRot_R == Vector3.zero)
                handRot_R = GorillaTagger.Instance.rightHandTransform.transform.rotation.eulerAngles;

            float positionSpeed = 0.01f; 
            float rotationSpeed = 2.0f; 
            float positionThreshold = 0.05f;
            float rotationThreshold = 11.5f; 
            if (Vector3.Distance(headPos, GorillaTagger.Instance.headCollider.transform.position) > positionThreshold)
                headPos += Vector3.Normalize(GorillaTagger.Instance.headCollider.transform.position - headPos) * positionSpeed;

            if (Quaternion.Angle(Quaternion.Euler(headRot), GorillaTagger.Instance.headCollider.transform.rotation) > rotationThreshold)
                headRot = Quaternion.RotateTowards(Quaternion.Euler(headRot), GorillaTagger.Instance.headCollider.transform.rotation, rotationSpeed).eulerAngles;

            if (Vector3.Distance(handPos_L, GorillaTagger.Instance.leftHandTransform.transform.position) > positionThreshold)
                handPos_L += Vector3.Normalize(GorillaTagger.Instance.leftHandTransform.transform.position - handPos_L) * positionSpeed;

            if (Quaternion.Angle(Quaternion.Euler(handRot_L), GorillaTagger.Instance.leftHandTransform.transform.rotation) > rotationThreshold)
                handRot_L = Quaternion.RotateTowards(Quaternion.Euler(handRot_L), GorillaTagger.Instance.leftHandTransform.transform.rotation, rotationSpeed).eulerAngles;

            if (Vector3.Distance(handPos_R, GorillaTagger.Instance.rightHandTransform.transform.position) > positionThreshold)
                handPos_R += Vector3.Normalize(GorillaTagger.Instance.rightHandTransform.transform.position - handPos_R) * positionSpeed;

            if (Quaternion.Angle(Quaternion.Euler(handRot_R), GorillaTagger.Instance.rightHandTransform.transform.rotation) > rotationThreshold)
                handRot_R = Quaternion.RotateTowards(Quaternion.Euler(handRot_R), GorillaTagger.Instance.rightHandTransform.transform.rotation, rotationSpeed).eulerAngles;

            VRRig.LocalRig.transform.position = headPos - new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = Quaternion.Euler(new Vector3(0f, headRot.y, 0f));

            VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(headRot);

            VRRig.LocalRig.leftHand.rigTarget.transform.position = handPos_L;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = handPos_R;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(handRot_L);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(handRot_R);

            VRRig.LocalRig.leftIndex.calcT = leftTrigger;
            VRRig.LocalRig.leftMiddle.calcT = leftGrab ? 1 : 0;
            VRRig.LocalRig.leftThumb.calcT = leftPrimary || leftSecondary ? 1 : 0;

            VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
            VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

            VRRig.LocalRig.rightIndex.calcT = rightTrigger;
            VRRig.LocalRig.rightMiddle.calcT = rightGrab ? 1 : 0;
            VRRig.LocalRig.rightThumb.calcT = rightPrimary || rightSecondary ? 1 : 0;

            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
            VRRig.LocalRig.rightThumb.LerpFinger(1f, false);

            FixRigHandRotation();
        }

        public static void DisableGhostAnimations()
        {
            headPos = Vector3.zero;
            headRot = Vector3.zero;

            handPos_L = Vector3.zero;
            handRot_L = Vector3.zero;

            handPos_R = Vector3.zero;
            handRot_R = Vector3.zero;

            VRRig.LocalRig.enabled = true;
        }

        public static void MinecraftAnimations()
        {
            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            if (rightPrimary)
            {
                VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.25f + GorillaTagger.Instance.bodyCollider.transform.up * -1f + GorillaTagger.Instance.bodyCollider.transform.forward * Mathf.Sin(Time.frameCount / 10f);
                VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.25f + GorillaTagger.Instance.bodyCollider.transform.up * -1f + -(GorillaTagger.Instance.bodyCollider.transform.forward * Mathf.Sin(Time.frameCount / 10f));
            } else
            {
                VRRig.LocalRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.25f + GorillaTagger.Instance.bodyCollider.transform.up * -1f;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.25f + GorillaTagger.Instance.bodyCollider.transform.up * -1f;
            }

            if (rightSecondary)
            {
                VRRig.LocalRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.25f + Vector3.Lerp(GorillaTagger.Instance.rightHandTransform.forward, - GorillaTagger.Instance.rightHandTransform.up, 0.5f) * 2f;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }

            FixRigHandRotation();
        }

        public static void StareAtNearby() =>
            VRRig.LocalRig.head.rigTarget.LookAt(GetClosestVRRig().headMesh.transform.position);

        public static void StareAtTarget() =>
            VRRig.LocalRig.head.rigTarget.LookAt(lockTarget.headMesh.transform.position);

        private static bool hasAdded;
        public static void StareAtGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        if (!hasAdded)
                        {
                            hasAdded = true;
                            TorsoPatch.VRRigLateUpdate += StareAtTarget;
                        }

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
                    if (hasAdded)
                    {
                        hasAdded = false;
                        TorsoPatch.VRRigLateUpdate -= StareAtTarget;
                    }
                }
            }
        }

        public static void StareAtAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Quaternion headRotArchive = VRRig.LocalRig.head.rigTarget.transform.rotation;
                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(GetVRRigFromPlayer(Player).headMesh.transform.position));
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();
                VRRig.LocalRig.head.rigTarget.transform.rotation = headRotArchive;

                return false;
            };
        }

        public static void EyeContact()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs.Where(rig => !rig.IsLocal()))
            {
                if (Physics.SphereCast(rig.headMesh.transform.position + (rig.headMesh.transform.forward * 0.25f), 0.25f, rig.headMesh.transform.forward, out var ray, 512f, NoInvisLayerMask()))
                {
                    VRRig.LocalRig.head.rigTarget.LookAt(rig.headMesh.transform.position);
                    break;
                }
            }
        }

        public static void EnableFloatingRig() =>
            offsetH = VRRig.LocalRig.head.trackingPositionOffset;

        public static void FloatingRig() =>
            VRRig.LocalRig.head.trackingPositionOffset = offsetH + new Vector3(0f, 0.65f + Mathf.Sin(Time.frameCount / 40f) * 0.2f, 0f);

        public static void DisableFloatingRig() =>
            VRRig.LocalRig.head.trackingPositionOffset = offsetH;

        public static void Bees()
        {
            VRRig.LocalRig.enabled = false;
            if (Time.time > beesDelay)
            {
                VRRig target = GetRandomVRRig(false);

                VRRig.LocalRig.transform.position = target.transform.position + new Vector3(0f, 1f, 0f);

                VRRig.LocalRig.leftHand.rigTarget.transform.position = target.transform.position;
                VRRig.LocalRig.rightHand.rigTarget.transform.position = target.transform.position;

                beesDelay = Time.time + 0.777f;
            }
        }

        public static void SizeChanger()
        {
            float increment = 0.05f;
            if (!GetIndex("Disable Size Changer Buttons").enabled)
            {
                if (leftTrigger > 0.5f)
                    increment = 0.2f;
                
                if (leftGrab)
                    increment = 0.01f;
                
                if (rightTrigger > 0.5f)
                    sizeScale += increment;
                
                if (rightGrab)
                    sizeScale -= increment;
                
                if (rightPrimary)
                    sizeScale = 1f;
            }

            if (sizeScale < 0.05f)
                sizeScale = 0.05f;

            VRRig.LocalRig.transform.localScale = Vector3.one * sizeScale;
            VRRig.LocalRig.NativeScale = sizeScale;
            GTPlayer.Instance.nativeScale = sizeScale;
        }

        public static void DisableSizeChanger()
        {
            sizeScale = 1f;

            VRRig.LocalRig.transform.localScale = Vector3.one * sizeScale;
            VRRig.LocalRig.NativeScale = sizeScale;
            GTPlayer.Instance.nativeScale = sizeScale;
        }

        public static GameObject stickpart;
        public static void StickyHands()
        {
            if (stickpart == null)
            {
                stickpart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                FixStickyColliders(stickpart);
                stickpart.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                stickpart.GetComponent<Renderer>().enabled = false;
            }
            if (GTPlayer.Instance.IsHandTouching(true))
                stickpart.transform.position = TrueLeftHand().position;

            if (GTPlayer.Instance.IsHandTouching(false))
                stickpart.transform.position = TrueRightHand().position;

            if (GTPlayer.Instance.IsHandTouching(true) && GTPlayer.Instance.IsHandTouching(false))
                stickpart.transform.position = Vector3.zero;
        }

        public static void DisableStickyHands()
        {
            if (stickpart != null)
            {
                Object.Destroy(stickpart);
                stickpart = null;
            }
        }

        private static bool leftisclimbing;
        private static bool rightisclimbing;
        private static GameObject climb;
        public static void ClimbyHands()
        {
            if (climb == null)
            {
                climb = new GameObject("GR");
                climb.AddComponent<GorillaClimbable>();
            }
            if (leftGrab)
            {
                if (GTPlayer.Instance.IsHandTouching(true) && !leftisclimbing)
                {
                    climb.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    leftisclimbing = true;
                    GTPlayer.Instance.BeginClimbing(climb.AddComponent<GorillaClimbable>(), GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller/GorillaHandClimber").GetComponent<GorillaHandClimber>());
                }
            } else
                leftisclimbing = false;
            
            if (rightGrab)
            {
                if (GTPlayer.Instance.IsHandTouching(false) && !rightisclimbing)
                {
                    climb.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    rightisclimbing = true;
                    GTPlayer.Instance.BeginClimbing(climb.AddComponent<GorillaClimbable>(), GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller/GorillaHandClimber").GetComponent<GorillaHandClimber>());
                }
            }
            else
                rightisclimbing = false;
        }

        public static void DisableClimbyHands()
        {
            if (climb != null)
            {
                Object.Destroy(climb);
                climb = null;
            }
        }

        public static void SetHandEnabled(bool value)
        {
            GTPlayer.Instance.leftHandHolding = !value;
            GTPlayer.Instance.rightHandHolding = !value;
        }

        public static void EnableSlideControl()
        {
            oldSlide = GTPlayer.Instance.slideControl;
            GTPlayer.Instance.slideControl = 1f;
        }

        public static void EnableWeakSlideControl()
        {
            oldSlide = GTPlayer.Instance.slideControl;
            GTPlayer.Instance.slideControl = oldSlide*2f;
        }

        public static void DisableSlideControl() =>
            GTPlayer.Instance.slideControl = oldSlide;

        public static readonly Vector3[] lastLeft = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        public static readonly Vector3[] lastRight = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        public static void PunchMod()
        {
            int index = -1;
            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                index++;

                Vector3 they = vrrig.rightHandTransform.position;
                Vector3 notthem = VRRig.LocalRig.head.rigTarget.position;
                float distance = Vector3.Distance(they, notthem);

                if (distance < 0.25f)
                    GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(vrrig.rightHandTransform.position - lastRight[index]) * 10f;
                    
                lastRight[index] = vrrig.rightHandTransform.position;

                they = vrrig.leftHandTransform.position;
                distance = Vector3.Distance(they, notthem);

                if (distance < 0.25f)
                    GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(vrrig.leftHandTransform.position - lastLeft[index]) * 10f;
                    
                lastLeft[index] = vrrig.leftHandTransform.position;
            }
        }

        private static VRRig sithlord;
        private static bool sithright;
        private static float sithdist = 1f;
        public static void Telekinesis()
        {
            if (sithlord == null)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    try
                    {
                        if (!vrrig.isLocal)
                        {
                            if (vrrig.rightIndex.calcT < 0.5f && vrrig.rightMiddle.calcT > 0.5f)
                            {
                                Vector3 dir = vrrig.transform.Find("GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up;
                                Physics.SphereCast(vrrig.rightHandTransform.position + dir * 0.1f, 0.3f, dir, out var Ray, 512f, NoInvisLayerMask());
                                {
                                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                                    if (gunTarget && gunTarget.isLocal)
                                    {
                                        sithlord = vrrig;
                                        sithright = true;
                                        sithdist = Ray.distance;
                                    }
                                }
                            }
                            if (vrrig.leftIndex.calcT < 0.5f && vrrig.leftMiddle.calcT > 0.5f)
                            {
                                Vector3 dir = vrrig.transform.Find("GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up;
                                Physics.SphereCast(vrrig.leftHandTransform.position + dir * 0.1f, 0.3f, dir, out var Ray, 512f, NoInvisLayerMask());
                                {
                                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                                    if (gunTarget && gunTarget.isLocal)
                                    {
                                        sithlord = vrrig;
                                        sithright = false;
                                        sithdist = Ray.distance;
                                    }
                                }
                            }
                        }
                    } catch { }
                }
            } else
            {
                if (sithright ? sithlord.rightIndex.calcT < 0.5f && sithlord.rightMiddle.calcT > 0.5f : sithlord.leftMiddle.calcT < 0.5f && sithlord.leftMiddle.calcT > 0.5f)
                {
                    Transform hand = sithright ? sithlord.rightHandTransform : sithlord.leftHandTransform;
                    Vector3 dir = sithright ? sithlord.transform.Find("GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up : sithlord.transform.Find("GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up;
                    TeleportPlayer(Vector3.Lerp(GorillaTagger.Instance.bodyCollider.transform.position, hand.position + dir * sithdist, 0.1f));
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                    ZeroGravity();
                } else
                    sithlord = null;
            }
        }

        public static void SafetyBubble()
        {
            foreach (VRRig rig in 
                GorillaParent.instance.vrrigs
                    .Where(rig => rig != null && !rig.isLocal)
                    .OrderBy(rig => Vector3.Distance(rig.transform.position, GorillaTagger.Instance.bodyCollider.transform.position)))
            {
                if (Vector3.Distance(rig.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) < 2f)
                {
                    Vector3 direction = GorillaTagger.Instance.bodyCollider.transform.position - rig.transform.position;
                    direction = new Vector3(direction.x, 0f, direction.z).normalized;

                    TeleportPlayer(GorillaTagger.Instance.bodyCollider.transform.position + direction * 2f);
                }
            }
        }

        public static readonly Dictionary<VRRig, List<GameObject>> RigColliders = new Dictionary<VRRig, List<GameObject>>();
        public static void SolidPlayers()
        {
            List<VRRig> toRemove = new List<VRRig>();
            foreach (VRRig rig in RigColliders.Keys)
            {
                if (!GorillaParent.instance.vrrigs.Contains(rig))
                    toRemove.Add(rig);
            }

            foreach (GameObject gameObject in toRemove.SelectMany(removeRig => RigColliders[removeRig]))
                Object.Destroy(gameObject);

            toRemove.Clear();

            foreach (var vrrig in GorillaParent.instance.vrrigs.Where(vrrig => !vrrig.isLocal))
            {
                if (!RigColliders.TryGetValue(vrrig, out List<GameObject> colliders))
                {
                    colliders = new List<GameObject>();

                    GameObject bodyCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bodyCollider.GetComponent<Renderer>().enabled = false;
                    bodyCollider.transform.localScale = new Vector3(0.3f, 0.55f, 0.3f);

                    for (int i = 0; i < 19; i++)
                    {
                        bodyCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        bodyCollider.GetComponent<Renderer>().enabled = false;
                        bodyCollider.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    }
                }

                colliders[0].transform.position = vrrig.head.rigTarget.transform.position + new Vector3(0f, -0.12f, 0f);
                colliders[0].transform.rotation = vrrig.transform.rotation;

                for (int i = 0; i < 19; i++)
                {
                    GameObject boneCollider = colliders[i + 1];

                    Vector3 pointA = vrrig.mainSkin.bones[bones[i * 2]].position;
                    Vector3 pointB = vrrig.mainSkin.bones[bones[i * 2 + 1]].position;

                    boneCollider.GetComponent<Renderer>().enabled = false;
                    boneCollider.transform.position = Vector3.Lerp(pointA, pointB, 0.5f);
                    boneCollider.transform.LookAt(pointB);
                    boneCollider.transform.localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(pointA, pointB));
                }
            }
        }

        public static void DisableSolidPlayers()
        {
            foreach (var gameObject in RigColliders.Values.SelectMany(gameObjects => gameObjects))
                Object.Destroy(gameObject);

            RigColliders.Clear();
        }

        public static int pullPowerInt;
        public static void ChangePullModPower(bool positive = true)
        {
            float[] powers = {
                0.05f,
                0.1f,
                0.2f,
                0.4f
            };
            string[] powerNames = {
                "Normal",
                "Medium",
                "Strong",
                "Powerful"
            };

            if (positive)
                pullPowerInt++;
            else
                pullPowerInt--;

            pullPowerInt %= powerNames.Length;
            if (pullPowerInt < 0)
                pullPowerInt = powerNames.Length - 1;

            pullPower = powers[pullPowerInt];
            GetIndex("Change Pull Mod Power").overlapText = "Change Pull Mod Power <color=grey>[</color><color=green>" + powerNames[pullPowerInt] + "</color><color=grey>]</color>";
        }

        private static float pullPower = 0.05f;
        private static bool lasttouchleft;
        private static bool lasttouchright;
        public static void PullMod()
        {
            if (((!GTPlayer.Instance.IsHandTouching(true) && lasttouchleft) || (!GTPlayer.Instance.IsHandTouching(false) && lasttouchright)) && rightGrab)
            {
                Vector3 vel = GorillaTagger.Instance.rigidbody.linearVelocity;
                GTPlayer.Instance.transform.position += new Vector3(vel.x * pullPower, 0f, vel.z * pullPower);
            }
            lasttouchleft = GTPlayer.Instance.IsHandTouching(true);
            lasttouchright = GTPlayer.Instance.IsHandTouching(false);
        }

        public static GameObject leftThrow;
        public static GameObject rightThrow;
        public static void ThrowControllers()
        {
            if (leftPrimary)
            {
                if (leftThrow != null)
                {
                    GTPlayer.Instance.leftControllerTransform.position = leftThrow.transform.position;
                    GTPlayer.Instance.leftControllerTransform.rotation = leftThrow.transform.rotation;
                }
                else
                {
                    leftThrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leftThrow.GetComponent<Renderer>().enabled = false;
                    Object.Destroy(leftThrow.GetComponent<BoxCollider>());

                    leftThrow.transform.position = GTPlayer.Instance.leftControllerTransform.position;
                    leftThrow.transform.rotation = GTPlayer.Instance.leftControllerTransform.rotation;
                    Rigidbody comp = leftThrow.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.linearVelocity = GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    try
                    {
                        if (GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                            GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").AddComponent<GorillaVelocityEstimator>();
                        
                        comp.angularVelocity = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                    } catch { }
                }
            }
            else
            {
                if (leftThrow != null)
                {
                    Object.Destroy(leftThrow);
                    leftThrow = null;
                }
            }

            if (rightPrimary)
            {
                if (rightThrow != null)
                {
                    GTPlayer.Instance.rightControllerTransform.position = rightThrow.transform.position;
                    GTPlayer.Instance.rightControllerTransform.rotation = rightThrow.transform.rotation;
                }
                else
                {
                    rightThrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rightThrow.GetComponent<Renderer>().enabled = false;
                    Object.Destroy(rightThrow.GetComponent<BoxCollider>());

                    rightThrow.transform.position = GTPlayer.Instance.rightControllerTransform.position;
                    rightThrow.transform.rotation = GTPlayer.Instance.rightControllerTransform.rotation;
                    Rigidbody comp = rightThrow.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.linearVelocity = GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    try
                    {
                        if (GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                            GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").AddComponent<GorillaVelocityEstimator>();
                        
                        comp.angularVelocity = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                    } catch { }
                }
            }
            else
            {
                if (rightThrow != null)
                {
                    Object.Destroy(rightThrow);
                    rightThrow = null;
                }
            }
        }

        public static GameObject flickLeft;
        public static GameObject flickRight;
        public static void EnableControllerFlick()
        {
            flickLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(flickLeft.GetComponent<BoxCollider>());
            flickLeft.GetComponent<Renderer>().enabled = false;
            flickLeft.AddComponent<GorillaVelocityTracker>();
            Rigidbody leftRigid = flickLeft.AddComponent<Rigidbody>();
            leftRigid.isKinematic = true;
            leftRigid.useGravity = false;

            flickRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(flickRight.GetComponent<BoxCollider>());
            flickRight.GetComponent<Renderer>().enabled = false;
            flickRight.AddComponent<GorillaVelocityTracker>();
            Rigidbody rightRigid = flickRight.AddComponent<Rigidbody>();
            rightRigid.isKinematic = true;
            rightRigid.useGravity = false;
        }

        public static void DisableControllerFlick()
        {
            Object.Destroy(flickLeft);
            Object.Destroy(flickRight);
        }

        private static Quaternion initialRotationLeft = Quaternion.identity;
        private static Quaternion initialRotationRight = Quaternion.identity;
        public static void ControllerFlick()
        {
            if (leftPrimary)
            {
                if (initialRotationLeft == Quaternion.identity)
                    initialRotationLeft = GorillaTagger.Instance.leftHandTransform.rotation;

                Rigidbody rigidbody = flickLeft.GetComponent<Rigidbody>();
                if (rigidbody.isKinematic)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.linearVelocity = rigidbody.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0);
                }
                
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position - flickLeft.transform.position;
                GorillaTagger.Instance.leftHandTransform.rotation = initialRotationLeft;
            } 
            else
            {
                Rigidbody rigidbody = flickLeft.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                flickLeft.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
                initialRotationLeft = Quaternion.identity;
            }

            if (rightPrimary)
            {
                if (initialRotationRight == Quaternion.identity)
                    initialRotationRight = GorillaTagger.Instance.rightHandTransform.rotation;

                Rigidbody rigidbody = flickRight.GetComponent<Rigidbody>();
                if (rigidbody.isKinematic)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.linearVelocity = rigidbody.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0);
                }

                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position - flickRight.transform.position;
                GorillaTagger.Instance.rightHandTransform.rotation = initialRotationRight;
            }
            else
            {
                Rigidbody rigidbody = flickRight.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                flickRight.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                initialRotationRight = Quaternion.identity;
            }
        }

        public static void StickLongArms()
        {
            GTPlayer.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * (armlength - 0.917f);
            GTPlayer.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * (armlength - 0.917f);
        }

        public static void EnableSteamLongArms() =>
            GTPlayer.Instance.transform.localScale = new Vector3(armlength, armlength, armlength);

        public static void DisableSteamLongArms() =>
            GTPlayer.Instance.transform.localScale = Vector3.one;

        public static float extendingTime;
        public static void Extenders()
        {
            extendingTime += rightJoystickClick ? 0 - Time.unscaledDeltaTime : Time.unscaledDeltaTime;
            if (extendingTime > 1) extendingTime = 1;
            if (extendingTime < 0) extendingTime = 0;

            float delayedLength = (armlength - 1f) * extendingTime + 1f;
            GTPlayer.Instance.transform.localScale = new Vector3(delayedLength, delayedLength, delayedLength);
        }

        public static void MultipliedLongArms()
        {
            GTPlayer.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position) * armlength;
            GTPlayer.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position) * armlength;
        }

        public static void VerticalLongArms()
        {
            Vector3 lefty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            lefty.y *= armlength;
            Vector3 righty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            righty.y *= armlength;
            GTPlayer.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - lefty;
            GTPlayer.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - righty;
        }

        public static void HorizontalLongArms()
        {
            Vector3 lefty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            lefty.x *= armlength;
            lefty.z *= armlength;
            Vector3 righty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            righty.x *= armlength;
            righty.z *= armlength;
            GTPlayer.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - lefty;
            GTPlayer.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - righty;
        }

        public static GameObject lvT;
        public static GameObject rvT;
        public static void CreateVelocityTrackers()
        {
            lvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(lvT.GetComponent<BoxCollider>());
            lvT.GetComponent<Renderer>().enabled = false;
            lvT.AddComponent<GorillaVelocityTracker>();

            rvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(rvT.GetComponent<BoxCollider>());
            rvT.GetComponent<Renderer>().enabled = false;
            rvT.AddComponent<GorillaVelocityTracker>();
        }

        public static void DestroyVelocityTrackers()
        {
            Object.Destroy(lvT);
            Object.Destroy(rvT);
        }

        public static float predCount = 0.0125f;
        public static int predInt = 1;
        public static void ChangePredictionAmount(bool positive = true)
        {
            float[] predAmnts = {
                0.00625f,
                0.0125f,
                0.025f,
                0.05f
            };
            string[] predAmntNames = {
                "Low",
                "Normal",
                "High",
                "Extreme"
            };

            if (positive)
                predInt++;
            else
                predInt--;

            predInt %= predAmnts.Length;
            if (predInt < 0)
                predInt = predAmnts.Length - 1;

            predCount = predAmnts[predInt];
            GetIndex("Change Prediction Amount").overlapText = "Change Prediction Amount <color=grey>[</color><color=green>" + predAmntNames[predInt] + "</color><color=grey>]</color>";
        }

        public static void VelocityLongArms()
        {
            lvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            rvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            GTPlayer.Instance.leftControllerTransform.transform.position -= lvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * predCount;
            GTPlayer.Instance.rightControllerTransform.transform.position -= rvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * predCount;
        }

        public static void LagRange()
        {
            bool isTagged = PlayerIsTagged(VRRig.LocalRig);

            VRRig closestRig = GorillaParent.instance.vrrigs
                .Where(rig => rig != null && !rig.isLocal &&
                                  (isTagged ? !PlayerIsTagged(rig) : PlayerIsTagged(rig)))
                .OrderBy(rig => Vector3.Distance(rig.transform.position, GorillaTagger.Instance.bodyCollider.transform.position))
                .FirstOrDefault();

            float rigDistance = closestRig == null ? float.MaxValue :
                          Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, closestRig.transform.position);

            if (rigDistance < 15f)
            {
                float lagPower = Mathf.Clamp(rigDistance, 1f, 15f) / 15f;
                PhotonNetwork.SerializationRate = 4 + (int)Math.Ceiling(lagPower * 6f);
            }
        }

        public static int timerPowerIndex = 1;
        public static void ChangeTimerSpeed(bool positive = true)
        {
            float[] timerPowers = {
                0.5f,
                1.25f,
                2f,
                5f
            };
            string[] timerNames = {
                "Slow",
                "Normal",
                "Fast",
                "Extreme"
            };

            if (positive)
                timerPowerIndex++;
            else
                timerPowerIndex--;

            timerPowerIndex %= timerPowers.Length;
            if (timerPowerIndex < 0)
                timerPowerIndex = timerPowers.Length - 1;

            timerPower = timerPowers[timerPowerIndex];
            GetIndex("Change Timer Speed").overlapText = "Change Timer Speed <color=grey>[</color><color=green>" + timerNames[timerPowerIndex] + "</color><color=grey>]</color>";
        }

        private static float timerPower = 1.25f;
        public static void Timer()
        {
            float extraFactor = timerPower - 1f;
            if (extraFactor <= 0f) return;

            Rigidbody rb = GorillaTagger.Instance.rigidbody;

            Vector3 displacement = (rb.linearVelocity + Physics.gravity) * (Time.fixedDeltaTime * extraFactor);
            rb.MovePosition(rb.position + displacement);
        }

        public static void FlickJump()
        {
            if (rightPrimary)
                GTPlayer.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -1.5f, 0f);
        }

        public static Vector3 longJumpPower = Vector3.zero;
        public static float playspaceAbusePower = 0.004f;
        public static void LongJump()
        {
            if (rightPrimary)
            {
                if (longJumpPower == Vector3.zero)
                    longJumpPower = (GorillaTagger.Instance.rigidbody.linearVelocity * playspaceAbusePower).X_Z();
                
                GTPlayer.Instance.transform.position += longJumpPower;
            }
            else
                longJumpPower = Vector3.zero;
        }

        public static void BunnyHop()
        {
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GTPlayer.Instance.locomotionEnabledLayers);

            if (Ray.distance < 0.15f)
                GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(GorillaTagger.Instance.rigidbody.linearVelocity.x, GTPlayer.Instance.jumpMultiplier * 2.727272727f, GorillaTagger.Instance.rigidbody.linearVelocity.z);
        }

        public static void Strafe()
        {
            Vector3 direction = GetIndex("Hand Oriented Strafe").enabled 
                ? new Vector3(-GorillaTagger.Instance.rightHandTransform.up.x, 0f, -GorillaTagger.Instance.rightHandTransform.up.z).normalized 
                : GorillaTagger.Instance.bodyCollider.transform.forward;

            float power = GTPlayer.Instance.maxJumpSpeed;
            Vector3 velocity = direction * power;

            GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(velocity.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, velocity.z);
        }

        public static void DynamicStrafe()
        {
            Vector3 direction = GetIndex("Hand Oriented Strafe").enabled
                ? new Vector3(-GorillaTagger.Instance.rightHandTransform.up.x, 0f, -GorillaTagger.Instance.rightHandTransform.up.z).normalized
                : GorillaTagger.Instance.bodyCollider.transform.forward;

            float power = new Vector3(GorillaTagger.Instance.rigidbody.linearVelocity.x, 0, GorillaTagger.Instance.rigidbody.linearVelocity.z).magnitude;
            Vector3 velocity = direction * power;

            GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(velocity.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, velocity.z);
        }

        public static void GroundHelper()
        {
            if (rightGrab)
            {
                Vector3 x3 = GorillaTagger.Instance.rigidbody.linearVelocity;
                if (x3.y > 0f)
                    GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(x3.x, 0f, x3.z);
            }
        }

        private static float preBounciness;
        private static PhysicsMaterialCombine whateverthisis = PhysicsMaterialCombine.Maximum;
        private static float preFrictiness;

        public static void PreBouncy()
        {
            preBounciness = GorillaTagger.Instance.bodyCollider.material.bounciness;
            whateverthisis = GorillaTagger.Instance.bodyCollider.material.bounceCombine;
            preFrictiness = GorillaTagger.Instance.bodyCollider.material.dynamicFriction;
        }

        public static void Bouncy()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = 1f;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
        }

        public static void PostBouncy()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = preBounciness;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = whateverthisis;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = preFrictiness;
        }

        public static void DisableWater()
        {
            foreach (WaterVolume waterVolume in GetAllType<WaterVolume>())
            {
                GameObject v = waterVolume.gameObject;
                v.layer = LayerMask.NameToLayer("TransparentFX");
            }
        }

        public static void SolidWater()
        {
            foreach (WaterVolume waterVolume in GetAllType<WaterVolume>())
            {
                GameObject v = waterVolume.gameObject;
                v.layer = LayerMask.NameToLayer("Default");
            }
        }

        public static void FixWater()
        {
            foreach (WaterVolume waterVolume in GetAllType<WaterVolume>())
            {
                GameObject v = waterVolume.gameObject;
                v.layer = LayerMask.NameToLayer("Water");
            }
        }

        public static GameObject airSwimPart;
        public static void AirSwim()
        {
            if (airSwimPart == null)
            {
                airSwimPart = Object.Instantiate(GetObject("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/CaveWaterVolume"));
                airSwimPart.transform.localScale = new Vector3(5f, 5f, 5f);
                airSwimPart.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                GTPlayer.Instance.audioManager.UnsetMixerSnapshot();
                airSwimPart.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 2.5f, 0f);
            }
        }

        public static void DisableAirSwim()
        {
            if (airSwimPart != null)
                Object.Destroy(airSwimPart);
        }

        public static void SetSwimSpeed(float speed = 3f) =>
            GTPlayer.Instance.swimmingParams.swimmingVelocityOutOfWaterDrainRate = speed;

        public static void PiggybackGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    TeleportPlayer(lockTarget.transform.position + new Vector3(0f, 0.5f, 0f));
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
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

        public static void PiggybackAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig.LocalRig.transform.position = GetVRRigFromPlayer(Player).headMesh.transform.position;
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;

                return false;
            };
        }

        public static void CopyMovementGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    CopyMovementPlayer(GetPlayerFromVRRig(lockTarget));
                
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void CopyMovementPlayer(NetPlayer player, bool fingers = true)
        {
            VRRig targetRig = GetVRRigFromPlayer(player);
            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = targetRig.transform.position;
            VRRig.LocalRig.transform.rotation = targetRig.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.leftHandTransform.position;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.rightHandTransform.position;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = targetRig.leftHandTransform.rotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = targetRig.rightHandTransform.rotation;

            if (fingers)
            {
                VRRig.LocalRig.leftIndex.calcT = targetRig.leftIndex.calcT;
                VRRig.LocalRig.leftMiddle.calcT = targetRig.leftMiddle.calcT;
                VRRig.LocalRig.leftThumb.calcT = targetRig.leftThumb.calcT;

                VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                VRRig.LocalRig.rightIndex.calcT = targetRig.rightIndex.calcT;
                VRRig.LocalRig.rightMiddle.calcT = targetRig.rightMiddle.calcT;
                VRRig.LocalRig.rightThumb.calcT = targetRig.rightThumb.calcT;

                VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
            }

            VRRig.LocalRig.head.rigTarget.transform.rotation = targetRig.headMesh.transform.rotation;
        }

        public static void CopyMovementAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    CopyMovementPlayer(Player, false);
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.enabled = true;

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void FollowPlayer(NetPlayer player, bool fingers = true)
        {
            VRRig targetRig = GetVRRigFromPlayer(player);
            VRRig.LocalRig.enabled = false;

            Vector3 look = targetRig.transform.position - VRRig.LocalRig.transform.position;
            look.Normalize();

            Vector3 position = VRRig.LocalRig.transform.position + look * (flySpeed / 2f * Time.deltaTime);

            VRRig.LocalRig.transform.position = position;
            VRRig.LocalRig.transform.LookAt(targetRig.transform.position);

            VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
            VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
            VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

            FixRigHandRotation();

            if (fingers)
            {
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

        public static void FollowPlayerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    FollowPlayer(GetPlayerFromVRRig(lockTarget));
                
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static readonly Dictionary<VRRig, Vector3> followPositions = new Dictionary<VRRig, Vector3>();
        public static void FollowAllPlayers()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    Vector3 position = followPositions.GetValueOrDefault(targetRig, archivePos);

                    Vector3 look = targetRig.transform.position - position;
                    look.Normalize();

                    position += look * (flySpeed / 2f * Time.deltaTime);

                    followPositions.Remove(targetRig);

                    followPositions.Add(targetRig, position);

                    VRRig.LocalRig.transform.position = position;
                    VRRig.LocalRig.transform.LookAt(targetRig.transform.position);

                    VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                    FixRigHandRotation();

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void OrbitPlayerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position = lockTarget.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f));
                    VRRig.LocalRig.transform.LookAt(lockTarget.transform.position);

                    VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                    FixRigHandRotation();

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void OrbitAllPlayers()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    VRRig.LocalRig.transform.position = targetRig.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f));
                    VRRig.LocalRig.transform.LookAt(targetRig.transform.position);

                    VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void JumpscareGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.forward * Random.Range(0.1f, 0.5f);
                    VRRig.LocalRig.head.rigTarget.transform.LookAt(lockTarget.headMesh.transform.position);
                    Quaternion dirLook = VRRig.LocalRig.head.rigTarget.transform.rotation;

                    VRRig.LocalRig.transform.rotation = dirLook;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.right * 0.2f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.right * -0.2f;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = dirLook;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                    FixRigHandRotation();

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void JumpscareAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    VRRig.LocalRig.transform.position = targetRig.headMesh.transform.position + targetRig.headMesh.transform.forward * Random.Range(0.1f, 0.5f);
                    VRRig.LocalRig.head.rigTarget.transform.LookAt(targetRig.headMesh.transform.position);
                    Quaternion dirLook = VRRig.LocalRig.head.rigTarget.transform.rotation;

                    VRRig.LocalRig.transform.rotation = dirLook;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.headMesh.transform.position + targetRig.headMesh.transform.right * 0.2f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.headMesh.transform.position + targetRig.headMesh.transform.right * -0.2f;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = dirLook;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void AnnoyPlayerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    Vector3 position = lockTarget.transform.position + RandomVector3();

                    VRRig.LocalRig.transform.position = position;
                    VRRig.LocalRig.transform.LookAt(lockTarget.transform.position);

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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void AnnoyAllPlayers()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    Vector3 position = targetRig.transform.position + RandomVector3();

                    VRRig.LocalRig.transform.position = position;
                    VRRig.LocalRig.transform.LookAt(targetRig.transform.position);

                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.transform.position + RandomVector3();
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.transform.position + RandomVector3();

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void ConfusePlayerGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = lockTarget.transform.position - new Vector3(0f, 2f, 0f);

                    if (Time.time > Fun.splashDel)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", GetPlayerFromVRRig(lockTarget), lockTarget.transform.position + RandomVector3(0.5f), RandomQuaternion(), 4f, 100f, true, false);
                        RPCProtection();
                        Fun.splashDel = Time.time + 0.1f;
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void ConfuseAllPlayers()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    VRRig.LocalRig.transform.position = targetRig.transform.position - Vector3.up * 2f;
                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;

                return false;
            };
        }

        public static void ConfuseAllPlayersSplash()
        {
            if (Time.time > Fun.splashDel)
            {
                Fun.splashDel = Time.time + 0.05f;
                VRRig rig = GetRandomVRRig(false);
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", GetPlayerFromVRRig(rig), rig.transform.position + RandomVector3(0.5f), RandomQuaternion(), 4f, 100f, true, false);
            }
        }

        public static void SchizophrenicGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !gunLocked)
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;

                        SerializePatch.OverrideSerialization = () => 
                        {
                            NetPlayer target = GetPlayerFromVRRig(lockTarget);
                            MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                            Vector3 positionArchive = VRRig.LocalRig.transform.position;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = AllActorNumbersExcept(target.ActorNumber) });

                            VRRig.LocalRig.transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { target.ActorNumber } });

                            RPCProtection();
                            VRRig.LocalRig.transform.position = positionArchive;

                            return false;
                        };
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    SerializePatch.OverrideSerialization = null;
                }
            }
        }

        public static void ReverseSchizoGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget) && !gunLocked)
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;

                        SerializePatch.OverrideSerialization = () =>
                        {
                            NetPlayer target = GetPlayerFromVRRig(lockTarget);
                            MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                            Vector3 positionArchive = VRRig.LocalRig.transform.position;
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { target.ActorNumber } });

                            VRRig.LocalRig.transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
                            SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = AllActorNumbersExcept(target.ActorNumber) });

                            RPCProtection();
                            VRRig.LocalRig.transform.position = positionArchive;

                            return false;
                        };
                    }
                }
            }
            else
            {
                if (gunLocked)
                {
                    gunLocked = false;
                    SerializePatch.OverrideSerialization = null;
                }
            }
        }

        public static void IntercourseGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    if (!GetIndex("Reverse Intercourse").enabled)
                    {
                        VRRig.LocalRig.transform.position = lockTarget.transform.position + lockTarget.transform.forward * -(0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f);
                        VRRig.LocalRig.transform.rotation = lockTarget.transform.rotation;

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * -0.2f + lockTarget.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * 0.2f + lockTarget.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = lockTarget.transform.rotation;
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = lockTarget.transform.rotation;
                    } else
                    {
                        VRRig.LocalRig.transform.position = lockTarget.transform.position + lockTarget.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f);
                        VRRig.LocalRig.transform.rotation = lockTarget.transform.rotation;

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * -0.2f + lockTarget.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * 0.2f + lockTarget.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = lockTarget.transform.rotation;
                    }

                    FixRigHandRotation();
                    IntercourseNoises();

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

                    VRRig.LocalRig.head.rigTarget.transform.rotation = lockTarget.transform.rotation;
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void IntercourseNoises()
        {
            if (Time.frameCount % 45 == 0)
            {
                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 64, true, 999999f);

                    if (GetIndex("Splash Intercourse").enabled)
                        Fun.BetaWaterSplash(VRRig.LocalRig.transform.position, VRRig.LocalRig.transform.rotation, 4f, 100f, true, false);

                    RPCProtection();
                }
                else
                    VRRig.LocalRig.PlayHandTapLocal(64, true, 999999f);
            }
        }

        public static void IntercourseAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    if (!GetIndex("Reverse Intercourse").enabled)
                    {
                        VRRig.LocalRig.transform.position = targetRig.transform.position + targetRig.transform.forward * -(0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f);
                        VRRig.LocalRig.transform.rotation = targetRig.transform.rotation;

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * -0.2f + targetRig.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * 0.2f + targetRig.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = targetRig.transform.rotation;
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = targetRig.transform.rotation;
                    }
                    else
                    {
                        VRRig.LocalRig.transform.position = targetRig.transform.position + targetRig.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f);
                        VRRig.LocalRig.transform.rotation = targetRig.transform.rotation;

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * -0.2f + targetRig.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * 0.2f + targetRig.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = targetRig.transform.rotation;
                    }

                    FixRigHandRotation();

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void HeadGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    if (!GetIndex("Reverse Intercourse").enabled)
                    {
                        VRRig.LocalRig.transform.position = lockTarget.transform.position + lockTarget.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f) + lockTarget.transform.up * -0.4f;
                        VRRig.LocalRig.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * 0.2f + lockTarget.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * -0.2f + lockTarget.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    }
                    else
                    {
                        VRRig.LocalRig.transform.position = lockTarget.transform.position + lockTarget.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f) + lockTarget.transform.up * 0.4f;
                        VRRig.LocalRig.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * 0.2f + lockTarget.transform.up * 0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + lockTarget.transform.right * -0.2f + lockTarget.transform.up * 0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(lockTarget.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    }

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

                    FixRigHandRotation();
                    IntercourseNoises();
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
                {
                    gunLocked = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void HeadAll()
        {
            SerializePatch.OverrideSerialization = () => {
                MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                Vector3 archivePos = VRRig.LocalRig.transform.position;
                Quaternion archiveRot = VRRig.LocalRig.transform.rotation;

                Vector3 archivePosLeft = VRRig.LocalRig.leftHand.rigTarget.position;
                Quaternion archiveRotLeft = VRRig.LocalRig.leftHand.rigTarget.rotation;

                Vector3 archivePosRight = VRRig.LocalRig.rightHand.rigTarget.position;
                Quaternion archiveRotRight = VRRig.LocalRig.rightHand.rigTarget.rotation;

                Quaternion archiveHeadRot = VRRig.LocalRig.head.rigTarget.transform.rotation;

                foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                {
                    VRRig targetRig = GetVRRigFromPlayer(Player);

                    if (!GetIndex("Reverse Intercourse").enabled)
                    {
                        VRRig.LocalRig.transform.position = targetRig.transform.position + targetRig.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f) + targetRig.transform.up * -0.4f;
                        VRRig.LocalRig.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * 0.2f + targetRig.transform.up * -0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * -0.2f + targetRig.transform.up * -0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    }
                    else
                    {
                        VRRig.LocalRig.transform.position = targetRig.transform.position + targetRig.transform.forward * (0.2f + Mathf.Sin(Time.frameCount / 8f) * 0.1f) + targetRig.transform.up * 0.4f;
                        VRRig.LocalRig.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.leftHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * 0.2f + targetRig.transform.up * 0.4f;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = targetRig.transform.position + targetRig.transform.right * -0.2f + targetRig.transform.up * 0.4f;

                        VRRig.LocalRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                        VRRig.LocalRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                        VRRig.LocalRig.head.rigTarget.transform.rotation = Quaternion.Euler(targetRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    }

                    FixRigHandRotation();

                    SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });
                }

                RPCProtection();

                VRRig.LocalRig.transform.position = archivePos;
                VRRig.LocalRig.transform.rotation = archiveRot;

                VRRig.LocalRig.leftHand.rigTarget.position = archivePosLeft;
                VRRig.LocalRig.leftHand.rigTarget.rotation = archiveRotLeft;

                VRRig.LocalRig.rightHand.rigTarget.position = archivePosRight;
                VRRig.LocalRig.rightHand.rigTarget.rotation = archiveRotRight;

                VRRig.LocalRig.head.rigTarget.transform.rotation = archiveHeadRot;

                return false;
            };
        }

        public static void RemoveCopy()
        {
            gunLocked = false;
            lockTarget = null;
            VRRig.LocalRig.enabled = true;
        }

        public static void SpazHead()
        {
            if (VRRig.LocalRig.enabled)
            {
                VRRig.LocalRig.head.trackingRotationOffset.x = Random.Range(0f, 360f);
                VRRig.LocalRig.head.trackingRotationOffset.y = Random.Range(0f, 360f);
                VRRig.LocalRig.head.trackingRotationOffset.z = Random.Range(0f, 360f);
            }
            else
                VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
        }

        public static float headspazDelay;
        public static void RandomSpazHead()
        {
            if (headspazType)
            {
                SpazHead();
                if (Time.time > headspazDelay)
                {
                    headspazType = false;
                    headspazDelay = Time.time + Random.Range(1000f, 4000f) / 1000f;
                }
            }
            else
            {
                Fun.FixHead();
                if (Time.time > headspazDelay)
                {
                    headspazType = true;
                    headspazDelay = Time.time + Random.Range(200f, 1000f) / 1000f;
                }
            }
        }

        private static Vector3 headoffs = Vector3.zero;
        public static void EnableSpazHead() =>
            headoffs = VRRig.LocalRig.head.trackingPositionOffset;

        public static void SpazHeadPosition() =>
            VRRig.LocalRig.head.trackingPositionOffset = headoffs + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

        public static void FixHeadPosition() =>
            VRRig.LocalRig.head.trackingPositionOffset = headoffs;

        public static void RandomSpazHeadPosition()
        {
            if (headspazType)
            {
                SpazHeadPosition();
                if (Time.time > headspazDelay)
                {
                    headspazType = false;
                    headspazDelay = Time.time + Random.Range(1000f, 4000f) / 1000f;
                }
            }
            else
            {
                FixHeadPosition();
                if (Time.time > headspazDelay)
                {
                    headspazType = true;
                    headspazDelay = Time.time + Random.Range(200f, 1000f) / 1000f;
                }
            }
        }

        public static bool idiotfixthingy;
        public static void LaggyRig()
        {
            ghostException = true;
            if (Time.time > laggyRigDelay)
            {
                VRRig.LocalRig.enabled = true;
                idiotfixthingy = true;
                laggyRigDelay = Time.time + 0.5f;
            } else
            {
                if (idiotfixthingy)
                    idiotfixthingy = false;
                else
                    VRRig.LocalRig.enabled = false;
            }
        }

        public static void UpdateRig()
        {
            ghostException = true;
            if (rightPrimary && !lastprimaryhit)
            {
                VRRig.LocalRig.enabled = true;
                idiotfixthingy = true;
            }
            else
            {
                if (idiotfixthingy)
                    idiotfixthingy = false;
                else
                    VRRig.LocalRig.enabled = false;
            }
            lastprimaryhit = rightPrimary;
        }
    }
}
