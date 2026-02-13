/*
 * ii's Stupid Menu  Mods/Projectiles.cs
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
using GorillaLocomotion;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Patches.Menu;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Extensions.VRRigExtensions;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;
using Random = UnityEngine.Random;

namespace iiMenu.Mods
{
    public static class Projectiles
    {
        public static readonly string[] ProjectileObjectNames = {
            "SnowballLeftAnchor",
            "SnowballRightAnchor",
            "GrowingSnowballLeftAnchor",
            "GrowingSnowballRightAnchor",
            "WaterBalloonLeftAnchor",
            "WaterBalloonRightAnchor",
            "LavaRockAnchor",
            "LavaRockAnchor",
            "BucketGiftFunctionalAnchor_Left",
            "BucketGiftFunctionalAnchor_Right",
            "ScienceCandyLeftAnchor",
            "ScienceCandyRightAnchor",
            "FishFoodLeftAnchor",
            "FishFoodRightAnchor",
            "AppleLeftAnchor",
            "AppleRightAnchor",
            "TrickTreatFunctionalAnchor",
            "TrickTreatFunctionalAnchorRIGHT Variant",
            "VotingRockAnchor_LEFT",
            "VotingRockAnchor_RIGHT",
            "BookLeftAnchor",
            "BookRightAnchor",
            "CoinLeftAnchor",
            "CoinRightAnchor",
            "EggLeftHand_Anchor Variant",
            "EggRightHand_Anchor Variant",
            "IceCreamLeftAnchor",
            "IceCreamRightAnchor",
            "HotDogLeftAnchor",
            "HotDogRightAnchor",
            "Fireworks_Anchor Variant_Left Hand",
            "Fireworks_Anchor Variant_Right Hand",
            "Papers_Anchor Variant_Left Hand",
            "Papers_Anchor Variant_Right Hand",
            "IceCreamScoopLeftAnchor",
            "IceCreamScoopRightAnchor",
            "ChipsLeftAnchor",
            "ChipsRightAnchor",
            "SalsaLeftAnchor",
            "SalsaRightAnchor",
            "ApplePieLeftAnchor",
            "ApplePieRightAnchor",
            "GrowingMashedPotatoLeftAnchor",
            "GrowingMashedPotatoRightAnchor",
            "BerryPieLeftAnchor",
            "BerryPieRightAnchor",
            "LayerDipLeftAnchor",
            "LayerDipRightAnchor",
            "PumpkinPieLeftAnchor",
            "PumpkinPieRightAnchor",
            "GrowingStuffingLeftAnchor",
            "GrowingStuffingRightAnchor",
            "CornLeftAnchor",
            "CornRightAnchor",
            "TurkeyLegLeftAnchor",
            "TurkeyLegRightAnchor",
            "GoalpostFootball_Anchor_LeftHand",
            "GoalpostFootball_Anchor_RightHand",
            "PopcornBall_Anchor_Left",
            "PopcornBall_Anchor_Right",
            "CrackedPlate_Lump_Projectile_Anchor_LEFT",
            "CrackedPlate_Lump_Projectile_Anchor_RIGHT",
            "PortableBonfire_Sticks_Anchor_LeftHand",
            "PortableBonfire_Sticks_Anchor_RightHand",
            "Walnut_Anchor_Left",
            "Walnut_Anchor_Right",
            "HotCocoaCup_Anchor_LEFT",
            "HotCocoaCup_Anchor_RIGHT",
            "SlingshotProjectile",
            "SlingshotProjectile"
        };

        public static string SnowballName = "GrowingSnowball";

        public static Coroutine RigCoroutine;
        public static IEnumerator EnableRig()
        {
            yield return new WaitForSeconds(projDebounceType + 0.2f);
            VRRig.LocalRig.enabled = true;
        }

        public static Coroutine DisableCoroutine;
        public static IEnumerator DisableProjectile(SnowballThrowable Throwable)
        {
            yield return new WaitForSeconds(projDebounceType + 0.2f);
            Throwable.SetSnowballActiveLocal(false);
        }

        public static bool friendSided;
        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, RaiseEventOptions options = null, bool bypassTeleport = false)
        {
            color.a = 1f;

            if (velocity.magnitude > 9999f)
                velocity = velocity.normalized * 9999f;

            options ??= new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All
                };

            SnowballThrowable Throwable = GetProjectile(projectileName);

            if (projectileName != "SlingshotProjectile")
            {
                if (Throwable == null)
                    return;

                if (!Throwable.gameObject.activeSelf)
                {
                    Throwable.SetSnowballActiveLocal(true);
                    Throwable.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    Throwable.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                    if (Buttons.GetIndex("Random Projectile").enabled)
                        CoroutineManager.instance.StartCoroutine(DisableProjectile(Throwable));
                    else
                    {
                        if (DisableCoroutine != null)
                            CoroutineManager.instance.StopCoroutine(DisableCoroutine);

                        DisableCoroutine = CoroutineManager.instance.StartCoroutine(DisableProjectile(Throwable));
                    }
                }
            }

            if (Time.time > projDebounce)
            {
                try
                {
                    if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 3.9f && !bypassTeleport)
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = position + new Vector3(0f, velocity.y > 0f ? -3f : 3f, 0f);

                        if (RigCoroutine != null)
                            CoroutineManager.instance.StopCoroutine(RigCoroutine);

                        RigCoroutine = CoroutineManager.instance.StartCoroutine(EnableRig());
                    }

                    bool showSelf = options.Receivers == ReceiverGroup.All || options.TargetActors.Contains(PhotonNetwork.LocalPlayer.ActorNumber);

                    if (showSelf)
                    {
                        if (options.Receivers == ReceiverGroup.All)
                            options.Receivers = ReceiverGroup.Others;

                        if (options.TargetActors.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
                        {
                            List<int> targetActors = options.TargetActors.ToList();
                            targetActors.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
                            options.TargetActors = targetActors.ToArray();
                        }
                    }

                    if (projectileName.Contains(SnowballName))
                    {
                        int scale = Overpowered.snowballScale;
                        GrowingSnowballThrowable GrowingSnowball = Throwable as GrowingSnowballThrowable;

                        int index = Overpowered.GetProjectileIncrement(position, velocity, Throwable.transform.lossyScale.x);

                        SlingshotProjectile slingshotProjectile;
                        if (showSelf)
                        {
                            slingshotProjectile = GrowingSnowball.SpawnGrowingSnowball(ref velocity, scale);
                            slingshotProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, index, scale, true, color);
                        }

                        if (PhotonNetwork.InRoom && !Buttons.GetIndex("Client Sided Projectiles").enabled)
                        {
                            if (friendSided)
                            {
                                Color32 color32 = color;

                                object[] projectileSendData = new object[8];
                                projectileSendData[0] = "sendSnowball";
                                projectileSendData[1] = position;
                                projectileSendData[2] = velocity;
                                projectileSendData[3] = color32.r;
                                projectileSendData[4] = color32.g;
                                projectileSendData[5] = color32.b;
                                projectileSendData[6] = GrowingSnowball.snowballSizeLevels[scale].snowballScale;
                                projectileSendData[7] = index;

                                PhotonNetwork.RaiseEvent(FriendManager.FriendByte, projectileSendData, options, SendOptions.SendUnreliable);
                            } else
                            {
                                PhotonNetwork.RaiseEvent(176, new object[]
                                {
                                    GrowingSnowball.changeSizeEvent._eventId,
                                    scale
                                }, options, new SendOptions
                                {
                                    Reliability = false,
                                    Encrypt = true
                                });

                                PhotonNetwork.RaiseEvent(176, new object[]
                                {
                                    GrowingSnowball.snowballThrowEvent._eventId,
                                    position,
                                    velocity,
                                    index
                                }, options, new SendOptions
                                {
                                    Reliability = false,
                                    Encrypt = true
                                });
                            }
                        }
                    }
                    else
                    {
                        SlingshotProjectile slingshotProjectile = null;
                        if (showSelf)
                        {
                            if (Throwable == null)
                            {
                                ProjectileWeapon weapon = VRRig.LocalRig.GetSlingshot();
                                GameObject projectile = ObjectPools.instance.Instantiate(PoolUtils.GameObjHashCode(weapon.projectilePrefab), true);

                                float projectileScale = Mathf.Abs(weapon.transform.lossyScale.x);
                                projectile.transform.localScale = Vector3.one * projectileScale;

                                weapon.AttachTrail(PoolUtils.GameObjHashCode(weapon.projectileTrail), projectile, position, false, false, true, color);

                                slingshotProjectile = projectile.GetComponent<SlingshotProjectile>();
                                slingshotProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, position, projectileScale), projectileScale, true, color);

                                slingshotProjectile.ApplyColor(slingshotProjectile.defaultBall, color);
                            }
                            else
                                slingshotProjectile = Throwable.LaunchSnowballLocal(position, velocity, Throwable.transform.lossyScale.x, true, color);
                        }

                        if (PhotonNetwork.InRoom && !Buttons.GetIndex("Client Sided Projectiles").enabled)
                        {
                            int index = showSelf ? slingshotProjectile.myProjectileCount : Overpowered.GetProjectileIncrement(position, velocity, Throwable.transform.lossyScale.x);

                            Color32 color32 = color;

                            object[] projectileSendData = new object[9];
                            projectileSendData[0] = position;
                            projectileSendData[1] = velocity;
                            projectileSendData[2] = projectileName == "SlingshotProjectile" ? 0 : (projectileName.ToLower().Contains("left") ? 1 : 2);
                            projectileSendData[3] = index;
                            projectileSendData[4] = true;
                            projectileSendData[5] = color32.r;
                            projectileSendData[6] = color32.g;
                            projectileSendData[7] = color32.b;
                            projectileSendData[8] = color32.a;

                            object[] sendEventData;
                            if (friendSided)
                            {
                                sendEventData = new object[2];
                                sendEventData[0] = "sendProjectile";
                                sendEventData[1] = projectileSendData;
                            } else
                            {
                                sendEventData = new object[3];
                                sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
                                sendEventData[1] = 0;
                                sendEventData[2] = projectileSendData;
                            }

                            PhotonNetwork.RaiseEvent((byte)(friendSided ? FriendManager.FriendByte : 3), sendEventData, options, SendOptions.SendUnreliable);
                            RPCProtection();
                        }
                    }
                }
                catch (Exception e) { LogManager.LogError($"Projectile error: {e.Message}"); }

                if (projDebounceType > 0f)
                    projDebounce = Time.time + (projDebounceType + (projDebounceType == 0f ? 0f : 0.01f));
            }
        }

        public static void BetaFireImpact(Vector3 position, Color color)
        {
            if (Time.time > projDebounce)
            {
                object[] impactSendData = new object[6];
                impactSendData[0] = position;
                impactSendData[1] = color.r;
                impactSendData[2] = color.g;
                impactSendData[3] = color.b;
                impactSendData[4] = 1f;
                impactSendData[5] = 1;

                object[] sendEventData = new object[3];
                sendEventData[0] = PhotonNetwork.ServerTimestamp;
                sendEventData[1] = (byte)1;
                sendEventData[2] = impactSendData;
                PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);

                if (projDebounceType > 0f)
                    projDebounce = Time.time + 0.1f;
            }
        }

        public static int projMode;
        public static void ChangeProjectile(bool positive = true)
        {
            string[] shortProjectileNames = {
                "Snowball",
                "Growing Snowball",
                "Water Balloon",
                "Lava Rock",
                "Present",
                "Science Candy",
                "Fish Food",
                "Apple",
                "Candy Corn",
                "Voting Rock",
                "Book",
                "Coin",
                "Egg",
                "Ice Cream",
                "Hot Dog",
                "Fireworks",
                "Paper",
                "Ice Cream Scoop",
                "Chips",
                "Salsa",
                "Apple Pie",
                "Mashed Potatoes",
                "Berry Pie",
                "Layer Dip",
                "Pumpkin Pie",
                "Stuffing",
                "Corn",
                "Turkey Leg",
                "Football",
                "Popcorn Ball",
                "Plate",
                "Stick",
                "Walnut",
                "Hot Cocoa",
                "Slingshot"
            };

            if (positive)
                projMode++;
            else
                projMode--;

            projMode %= shortProjectileNames.Length;
            if (projMode < 0)
                projMode = shortProjectileNames.Length - 1;

            Buttons.GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + shortProjectileNames[projMode] + "</color><color=grey>]</color>";
        }

        public static int snowballIndex;
        public static void ChangeGrowingProjectile(bool positive = true)
        {
            string[] shortProjectileNames = {
                "Growing Snowball",
                "Mashed Potatoes",
                "Stuffing"
            };

            string[] longProjectileNames =
            {
                "GrowingSnowball",
                "GrowingMashedPotato",
                "GrowingStuffing"
            };

            if (positive)
                snowballIndex++;
            else
                snowballIndex--;

            snowballIndex %= shortProjectileNames.Length;
            if (snowballIndex < 0)
                snowballIndex = shortProjectileNames.Length - 1;

            Buttons.GetIndex("Change Growing Projectile").overlapText = "Change Growing Projectile <color=grey>[</color><color=green>" + shortProjectileNames[snowballIndex] + "</color><color=grey>]</color>";
            SnowballName = longProjectileNames[snowballIndex];
        }

        public static int targetProjectileIndex;
        public static void ChangeProjectileIndex(bool positive = true)
        {
            if (positive)
                targetProjectileIndex++;
            else
                targetProjectileIndex--;

            targetProjectileIndex %= 16;
            if (targetProjectileIndex < 0)
                targetProjectileIndex = 15;

            Buttons.GetIndex("Change Projectile Index").overlapText = "Change Projectile Index <color=grey>[</color><color=green>" + (targetProjectileIndex + 1) + "</color><color=grey>]</color>";
        }

        public static int shootCycle = 1;
        public static void ChangeShootSpeed(bool positive = true)
        {
            float[] ShootStrengthTypes = {
                9.72f,
                19.44f,
                38.88f,
                200f,
                1000000f
            };

            string[] ShootStrengthNames = {
                "Slow",
                "Medium",
                "Fast",
                "Ultra Fast",
                "Instant"
            };

            if (positive)
                shootCycle++;
            else
                shootCycle--;

            shootCycle %= ShootStrengthTypes.Length;
            if (shootCycle < 0)
                shootCycle = ShootStrengthTypes.Length - 1;

            ShootStrength = ShootStrengthTypes[shootCycle];
            Buttons.GetIndex("Change Shoot Speed").overlapText = "Change Shoot Speed <color=grey>[</color><color=green>" + ShootStrengthNames[shootCycle] + "</color><color=grey>]</color>";
        }

        public static int red = 10;
        public static int green = 5;
        public static int blue;

        public static void IncreaseRed(bool positive = true)
        {
            if (positive)
                red++;
            else
                red--;

            red %= 11;
            if (red < 0)
                red = 10;

            Buttons.GetIndex("RedProj").overlapText = "Red <color=grey>[</color><color=green>" + red + "</color><color=grey>]</color>";
        }

        public static void IncreaseGreen(bool positive = true)
        {
            if (positive)
                green++;
            else
                green--;

            green %= 11;
            if (green < 0)
                green = 10;

            Buttons.GetIndex("GreenProj").overlapText = "Green <color=grey>[</color><color=green>" + green + "</color><color=grey>]</color>";
        }

        public static void IncreaseBlue(bool positive = true)
        {
            if (positive)
                blue++;
            else
                blue--;

            blue %= 11;
            if (blue < 0)
                blue = 10;

            Buttons.GetIndex("BlueProj").overlapText = "Blue <color=grey>[</color><color=green>" + blue + "</color><color=grey>]</color>";
        }

        public static float projDebounce;
        public static float projDebounceType = 0.1f;
        public static int projDebounceIndex = 2;
        public static void ChangeProjectileDelay(bool positive = true, bool fromMenu = false)
        {
            if (positive)
                projDebounceIndex++;
            else
                projDebounceIndex--;

            projDebounceIndex %= 21;
            if (projDebounceIndex < 0)
                projDebounceIndex = 20;

            if (projDebounceIndex < 8 && fromMenu && (!Buttons.GetIndex("Friend Sided Projectiles").enabled || !Buttons.GetIndex("Client Sided Projectiles").enabled))
                NotificationManager.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> Using a projectile delay lower than 0.4 could get you banned. Use at your own caution.", 5000);

            projDebounceType = projDebounceIndex / 20f;
            Overpowered.SnowballSpawnDelay = Mathf.Max(projDebounceType, 0.1f);
            Buttons.GetIndex("Change Projectile Delay").overlapText = "Change Projectile Delay <color=grey>[</color><color=green>" + projDebounceType + "</color><color=grey>]</color>";
        }

        public static Color CalculateProjectileColor()
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;

            if (Buttons.GetIndex("Random Color").enabled)
            {
                r = (byte)Random.Range(0, 255);
                g = (byte)Random.Range(0, 255);
                b = (byte)Random.Range(0, 255);
            }

            if (Buttons.GetIndex("Rainbow Projectiles").enabled)
            {
                float h = Time.frameCount / 180f % 1f;
                Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                r = (byte)(rgbcolor.r * 255);
                g = (byte)(rgbcolor.g * 255);
                b = (byte)(rgbcolor.b * 255);
            }

            if (Buttons.GetIndex("Hard Rainbow Projectiles").enabled)
            {
                float h = Time.frameCount / 180f % 1f;
                Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                r = (byte)(Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f);
                g = (byte)(Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f);
                b = (byte)(Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f);
            }

            if (Buttons.GetIndex("Custom Colored Projectiles").enabled)
            {
                r = (byte)(red / 10f * 255);
                g = (byte)(green / 10f * 255);
                b = (byte)(blue / 10f * 255);
            }

            return new Color32(r, g, b, 255);
        }

        public static void ProjectileSpam()
        {
            int projIndex = projMode * 2;

            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                if (Buttons.GetIndex("Random Projectile").enabled)
                    projIndex = Random.Range(0, ProjectileObjectNames.Length);
                
                string projectilename = ProjectileObjectNames[projIndex];

                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 charvel = GTPlayer.Instance.RigidbodyVelocity;

                if (Buttons.GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GTPlayer.Instance.RigidbodyVelocity + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= ShootStrength * 2f;
                    }
                }

                if (Buttons.GetIndex("Random Direction").enabled)
                    charvel = RandomVector3(100f);

                if (Buttons.GetIndex("Above Players").enabled)
                {
                    VRRig targetRig = GetTargetPlayer();
                    startpos = targetRig.transform.position + Vector3.up;
                }

                if (Buttons.GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
                    charvel = Vector3.zero;
                }

                if (Buttons.GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (Buttons.GetIndex("True Projectile Aura").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                    charvel = RandomVector3(10f);
                }

                if (Buttons.GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                    charvel = new Vector3(Random.Range(-10, 10), 15, Random.Range(-10, 10));
                }

                if (Buttons.GetIndex("Include Hand Velocity").enabled)
                    charvel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);

                BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
            }
        }

        public static void ProjectileGun()
        {
            int projIndex = projMode * 2;

            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    if (Buttons.GetIndex("Random Projectile").enabled)
                        projIndex = Random.Range(0, ProjectileObjectNames.Length);

                    string projectilename = ProjectileObjectNames[projIndex];

                    Vector3 startpos = NewPointer.transform.position + Vector3.up;
                    Vector3 charvel = Vector3.up * 30f;

                    if (Buttons.GetIndex("Shoot Projectiles").enabled)
                    {
                        charvel = GTPlayer.Instance.RigidbodyVelocity + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                            charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                            charvel.Normalize();
                            charvel *= ShootStrength * 2f;
                        }
                    }

                    if (Buttons.GetIndex("Random Direction").enabled)
                        charvel = RandomVector3(100f);

                    if (Buttons.GetIndex("Above Players").enabled)
                    {
                        VRRig targetRig = GetTargetPlayer();
                        startpos = targetRig.transform.position + Vector3.up;
                    }

                    if (Buttons.GetIndex("Rain Projectiles").enabled)
                    {
                        startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
                        charvel = Vector3.zero;
                    }

                    if (Buttons.GetIndex("Projectile Aura").enabled)
                    {
                        float time = Time.frameCount;
                        startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                    }

                    if (Buttons.GetIndex("True Projectile Aura").enabled)
                    {
                        startpos = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                        charvel = RandomVector3(10f);
                    }

                    if (Buttons.GetIndex("Projectile Fountain").enabled)
                    {
                        startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                        charvel = new Vector3(Random.Range(-10, 10), 15, Random.Range(-10, 10));
                    }

                    if (Buttons.GetIndex("Include Hand Velocity").enabled)
                        charvel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);

                    BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
                }
            }
        }

        public static void LazerSpam()
        {
            int projIndex = projMode * 2;

            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                if (Buttons.GetIndex("Random Projectile").enabled)
                    projIndex = Random.Range(0, ProjectileObjectNames.Length);
                
                string projectilename = ProjectileObjectNames[projIndex];

                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position;
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 30f;

                if (Buttons.GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GTPlayer.Instance.RigidbodyVelocity + GetGunDirection(GorillaTagger.Instance.rightHandTransform) * ShootStrength;
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= ShootStrength * 2f;
                    }
                }

                if (Buttons.GetIndex("Random Direction").enabled)
                    charvel = RandomVector3(100f);

                if (Buttons.GetIndex("Above Players").enabled)
                {
                    VRRig targetRig = GetTargetPlayer();
                    startpos = targetRig.transform.position + Vector3.up;
                }

                if (Buttons.GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
                    charvel = Vector3.zero;
                }

                if (Buttons.GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (Buttons.GetIndex("True Projectile Aura").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                    charvel = RandomVector3(10f);
                }

                if (Buttons.GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                    charvel = new Vector3(Random.Range(-10, 10), 15, Random.Range(-10, 10));
                }

                if (Buttons.GetIndex("Include Hand Velocity").enabled)
                    charvel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);

                BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
            }
        }

        public static void GiveProjectileSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    int projIndex = projMode * 2;
                    
                    if (Buttons.GetIndex("Random Projectile").enabled)
                        projIndex = Random.Range(0, ProjectileObjectNames.Length);
                    
                    string projectilename = ProjectileObjectNames[projIndex];

                    Vector3 startpos = lockTarget.rightHandTransform.position;
                    Vector3 charvel = Vector3.zero;

                    if (Buttons.GetIndex("Shoot Projectiles").enabled)
                        charvel = lockTarget.rightHandTransform.transform.forward * ShootStrength;

                    if (Buttons.GetIndex("Random Direction").enabled)
                        charvel = new Vector3(Random.Range(-33, 33), Random.Range(-33, 33), Random.Range(-33, 33));

                    if (Buttons.GetIndex("Above Players").enabled)
                    {
                        VRRig targetRig = GetTargetPlayer();
                        startpos = targetRig.transform.position + Vector3.up;
                    }

                    if (Buttons.GetIndex("Rain Projectiles").enabled)
                    {
                        startpos = lockTarget.headMesh.transform.position + new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));
                        charvel = Vector3.zero;
                    }

                    if (Buttons.GetIndex("Projectile Aura").enabled)
                    {
                        float time = Time.frameCount;
                        startpos = lockTarget.headMesh.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                    }

                    if (Buttons.GetIndex("True Projectile Aura").enabled)
                    {
                        startpos = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                        charvel = RandomVector3(10f);
                    }

                    if (Buttons.GetIndex("Projectile Fountain").enabled)
                    {
                        startpos = lockTarget.headMesh.transform.position + new Vector3(0, 1, 0);
                        charvel = new Vector3(Random.Range(-10, 10), -15, Random.Range(-10, 10));
                    }

                    BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
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

        public static void ImpactSpam()
        {
            if ((rightGrab || Mouse.current.leftButton.isPressed) && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;

                if (Buttons.GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GetGunDirection(GorillaTagger.Instance.rightHandTransform), out var Ray, 512f, NoInvisLayerMask());
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
                    }
                    startpos = Ray.point;
                }

                if (Buttons.GetIndex("Above Players").enabled)
                {
                    VRRig targetRig = GetTargetPlayer();
                    startpos = targetRig.transform.position + Vector3.up;
                }

                if (Buttons.GetIndex("Rain Projectiles").enabled)
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));

                if (Buttons.GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (Buttons.GetIndex("True Projectile Aura").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + RandomVector3();
                }

                if (Buttons.GetIndex("Projectile Fountain").enabled)
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);

                BetaFireImpact(startpos, CalculateProjectileColor());
                RPCProtection();

                if (projDebounceType > 0f)
                    projDebounce = Time.time + projDebounceType + 0.05f;
            }
        }

        private static readonly Dictionary<bool, bool> previousGripHeld = new Dictionary<bool, bool>();
        private static void HandleGrabProjectile(bool leftHand)
        {
            SnowballMaker snowballMaker = leftHand ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance;
            bool gripHeld = leftHand ? leftGrab : rightGrab;
            previousGripHeld.TryGetValue(leftHand, out bool lastGripHeld);

            if (gripHeld && !lastGripHeld)
            {
                int projIndex = projMode * 2;
                if (Buttons.GetIndex("Random Projectile").enabled)
                    projIndex = Random.Range(0, ProjectileObjectNames.Length / 2) * 2;

                SnowballThrowable snowballThrowable = GetProjectile(ProjectileObjectNames[projIndex + (leftHand ? 0 : 1)]);
                if (!snowballThrowable.gameObject.activeSelf)
                {
                    snowballThrowable.SetSnowballActiveLocal(true);
                    snowballThrowable.velocityEstimator = snowballMaker.velocityEstimator;

                    Transform handTransform = snowballMaker.handTransform;
                    snowballThrowable.transform.position = handTransform.TransformPoint(snowballThrowable.SpawnOffset.pos);
                    snowballThrowable.transform.rotation = handTransform.rotation * snowballThrowable.SpawnOffset.rot;

                    Color TargetProjectileColor = CalculateProjectileColor();
                    VRRig.LocalRig.SetThrowableProjectileColor(true, CalculateProjectileColor());

                    bool wasProjectileRandomized = snowballThrowable.randomizeColor;
                    snowballThrowable.randomizeColor = true;
                    snowballThrowable.ApplyColor(TargetProjectileColor);
                    snowballThrowable.randomizeColor = wasProjectileRandomized;
                }
            }

            previousGripHeld[leftHand] = gripHeld;
        }

        public static void GrabProjectile()
        {
            HandleGrabProjectile(true);
            HandleGrabProjectile(false);
        }
        
        public static void Urine()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.yellow);
            }
        }
        
        public static void Feces()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.3f, 0f);
                Vector3 charvel = Vector3.zero;

                BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.brown);
            }
        }

        public static void Period()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.3f, 0f);
                Vector3 charvel = Vector3.zero;

                BetaFireProjectile("IceCreamScoopRightAnchor", startpos, charvel, Color.red);
            }
        }

        public static void Semen()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.ghostWhite);
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * 0.1f + GorillaTagger.Instance.headCollider.transform.up * -0.15f;
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.green);
            }
        }

        public static void Spit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + GorillaTagger.Instance.headCollider.transform.forward * 0.1f + GorillaTagger.Instance.headCollider.transform.up * -0.15f;
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("WaterBalloonLeftAnchor",  startpos, charvel, Color.cyan);
            }
        }

        public static void LazerEyes()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position;
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 30f;
                
                BetaFireProjectile("Walnut_Anchor_Right", startpos, charvel, Color.red);
            }
        }

        public static void UrineGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.4f, 0f) + lockTarget.transform.forward * 0.2f;
                    Vector3 charvel = lockTarget.transform.forward * 8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.yellow);
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

        public static void FecesGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.brown);
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

        public static void PeriodGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("IceCreamScoopRightAnchor", startpos, charvel, Color.red);
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

        public static void SemenGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.4f, 0f) + lockTarget.transform.forward * 0.2f;
                    Vector3 charvel = lockTarget.transform.forward * 8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.ghostWhite);
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

        public static void VomitGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.forward * 0.4f + lockTarget.headMesh.transform.up * -0.05f;
                    Vector3 charvel = lockTarget.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.green);
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

        public static void SpitGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.forward * 0.4f + lockTarget.headMesh.transform.up * -0.05f;
                    Vector3 charvel = lockTarget.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("WaterBalloonLeftAnchor", startpos, charvel, Color.cyan);
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

        public static void LazerEyesGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.headMesh.transform.position + lockTarget.headMesh.transform.forward * 0.4f + lockTarget.headMesh.transform.up * -0.05f;
                    Vector3 charvel = lockTarget.headMesh.transform.forward * 30f;
                
                    BetaFireProjectile("Walnut_Anchor_Right", startpos, charvel, Color.red);
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

        public static void ProjectileBlindGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    ProjectileBlindPlayer(lockTarget);

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

        public static void ProjectileBlindAll()
        {
            SerializePatch.OverrideSerialization = () => {
                if (PhotonNetwork.InRoom)
                {
                    MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                    Vector3 archivePos = VRRig.LocalRig.transform.position;

                    foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                    {
                        VRRig rig = GetVRRigFromPlayer(Player);
                        VRRig.LocalRig.transform.position = rig.transform.position - Vector3.one * 3f;

                        SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });

                        BetaFireProjectile("EggLeftHand_Anchor Variant", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } }, true);
                    }

                    RPCProtection();

                    VRRig.LocalRig.enabled = true;

                    VRRig.LocalRig.transform.position = archivePos;

                    return false;
                }

                return true;
            };
        }

        public static void ProjectileBlindPlayer(NetPlayer player)
        {
            VRRig rig = GetVRRigFromPlayer(player);
            BetaFireProjectile("EggLeftHand_Anchor Variant", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } });
        }

        public static void ProjectileBlindPlayer(VRRig player) => ProjectileBlindPlayer(GetPlayerFromVRRig(player));

        public static void ProjectileLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    ProjectileLagPlayer(lockTarget);

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

        public static void ProjectileLagAll()
        {
            SerializePatch.OverrideSerialization = () => {
                if (PhotonNetwork.InRoom)
                {
                    MassSerialize(true, new[] { GorillaTagger.Instance.myVRRig.GetView });

                    Vector3 archivePos = VRRig.LocalRig.transform.position;

                    foreach (NetPlayer Player in NetworkSystem.Instance.PlayerListOthers)
                    {
                        VRRig rig = GetVRRigFromPlayer(Player);
                        VRRig.LocalRig.transform.position = rig.transform.position - Vector3.one * 3f;

                        SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = new[] { Player.ActorNumber } });

                        BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + rig.headMesh.transform.forward * -0.7f, new Vector3(0f, 15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } }, true);
                    }

                    RPCProtection();

                    VRRig.LocalRig.enabled = true;

                    VRRig.LocalRig.transform.position = archivePos;

                    return false;
                }

                return true;
            };
        }

        public static void ProjectileLagPlayer(NetPlayer player)
        {
            VRRig rig = GetVRRigFromPlayer(player);
            BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + rig.headMesh.transform.forward * -0.7f, new Vector3(0f, 15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } });
        }

        public static void ProjectileLagPlayer(VRRig player) => ProjectileLagPlayer(GetPlayerFromVRRig(player));
    }
}
