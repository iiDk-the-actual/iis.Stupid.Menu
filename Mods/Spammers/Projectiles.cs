using ExitGames.Client.Photon;
using GorillaExtensions;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    public class Projectiles
    {
        // This file needs to be rewritten
        public static string[] ProjectileObjectNames = new string[]
        {
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
            "Fireworks_Anchor Variant_Right Hand"
        };

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
        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, RaiseEventOptions options = null)
        {
            if (velocity.magnitude > 9999f)
                velocity = velocity.normalized * 9999f;

            if (options == null)
                options = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All
                };

            SnowballThrowable Throwable = GetProjectile(projectileName);

            if (Throwable == null)
                return;

            if (!Throwable.gameObject.activeSelf)
            {
                Throwable.SetSnowballActiveLocal(true);
                Throwable.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                Throwable.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                if (GetIndex("Random Projectile").enabled)
                    CoroutineManager.instance.StartCoroutine(DisableProjectile(Throwable));
                else
                {
                    if (DisableCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(DisableCoroutine);

                    DisableCoroutine = CoroutineManager.instance.StartCoroutine(DisableProjectile(Throwable));
                }
            }

            if (Time.time > projDebounce)
            {
                try
                {
                    if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 3.9f)
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = position + new Vector3(0f, velocity.y > 0f ? -3f : 3f, 0f);

                        if (RigCoroutine != null)
                            CoroutineManager.instance.StopCoroutine(RigCoroutine);

                        RigCoroutine = CoroutineManager.instance.StartCoroutine(EnableRig());
                    }

                    Vector3 startpos = position;
                    Vector3 charvel = velocity;

                    Throwable.randomizeColor = true;
                    VRRig.LocalRig.SetThrowableProjectileColor(true, color);

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

                    if (projectileName.Contains("GrowingSnowball"))
                    {
                        int scale = Overpowered.snowballScale;
                        GrowingSnowballThrowable GrowingSnowball = Throwable as GrowingSnowballThrowable;

                        int index = Overpowered.GetProjectileIncrement(position, velocity, Throwable.transform.lossyScale.x);
                        int actorOwner = PhotonNetwork.InRoom ? NetworkSystem.Instance.LocalPlayer.ActorNumber : 0;

                        SlingshotProjectile slingshotProjectile = null;
                        if (showSelf)
                        {
                            slingshotProjectile = GrowingSnowball.SpawnGrowingSnowball(ref velocity, scale);
                            slingshotProjectile.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, index, scale, true, color);
                        }

                        if (PhotonNetwork.InRoom && !GetIndex("Client Sided Projectiles").enabled)
                        {
                            if (friendSided)
                            {
                                Color32 color32 = (Color32)color;

                                object[] projectileSendData = new object[8];
                                projectileSendData[0] = "sendSnowball";
                                projectileSendData[1] = position;
                                projectileSendData[2] = velocity;
                                projectileSendData[3] = color32.r;
                                projectileSendData[4] = color32.g;
                                projectileSendData[5] = color32.b;
                                projectileSendData[6] = GrowingSnowball.snowballSizeLevels[index].snowballScale;
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
                            slingshotProjectile = Throwable.LaunchSnowballLocal(position, velocity, Throwable.transform.lossyScale.x, true, color);

                        if (PhotonNetwork.InRoom && !GetIndex("Client Sided Projectiles").enabled)
                        {
                            int index = showSelf ? slingshotProjectile.myProjectileCount : Overpowered.GetProjectileIncrement(position, velocity, Throwable.transform.lossyScale.x);

                            Color32 color32 = (Color32)color;

                            object[] projectileSendData = new object[9];
                            projectileSendData[0] = position;
                            projectileSendData[1] = velocity;
                            projectileSendData[2] = 1;
                            projectileSendData[3] = index;
                            projectileSendData[4] = true;
                            projectileSendData[5] = color32.r;
                            projectileSendData[6] = color32.g;
                            projectileSendData[7] = color32.b;
                            projectileSendData[8] = color32.a;

                            object[] sendEventData = null;
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

                    Throwable.randomizeColor = false;
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

        public static int projmode;
        public static void ChangeProjectile(bool positive = true)
        {
            string[] shortProjectileNames = new string[] {
                "Snowball",
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
                "Fireworks"
            };

            if (positive)
                projmode++;
            else
                projmode--;

            projmode %= shortProjectileNames.Length;
            if (projmode < 0)
                projmode = shortProjectileNames.Length - 1;

            GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + shortProjectileNames[projmode] + "</color><color=grey>]</color>";
        }

        public static int shootCycle = 1;
        public static void ChangeShootSpeed(bool positive = true)
        {
            float[] ShootStrengthTypes = new float[]
            {
                9.72f,
                19.44f,
                38.88f,
                200f,
                1000000f
            };

            string[] ShootStrengthNames = new string[]
            {
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
            GetIndex("Change Shoot Speed").overlapText = "Change Shoot Speed <color=grey>[</color><color=green>" + ShootStrengthNames[shootCycle] + "</color><color=grey>]</color>";
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

            GetIndex("RedProj").overlapText = "Red <color=grey>[</color><color=green>" + red.ToString() + "</color><color=grey>]</color>";
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

            GetIndex("GreenProj").overlapText = "Green <color=grey>[</color><color=green>" + green.ToString() + "</color><color=grey>]</color>";
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

            GetIndex("BlueProj").overlapText = "Blue <color=grey>[</color><color=green>" + blue.ToString() + "</color><color=grey>]</color>";
        }

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

            if (projDebounceIndex < 8 && fromMenu)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> Using a projectile delay lower than 0.8 could get you banned. Use at your own caution.", 5000);

            projDebounceType = projDebounceIndex / 20f;
            Overpowered.snowballSpawnDelay = Mathf.Max(projDebounceIndex / 20f, 0.1f);
            GetIndex("Change Projectile Delay").overlapText = "Change Projectile Delay <color=grey>[</color><color=green>" + projDebounceType.ToString() + "</color><color=grey>]</color>";
        }

        public static Color CalculateProjectileColor()
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;

            if (GetIndex("Random Color").enabled)
            {
                r = (byte)UnityEngine.Random.Range(0, 255);
                g = (byte)UnityEngine.Random.Range(0, 255);
                b = (byte)UnityEngine.Random.Range(0, 255);
            }

            if (GetIndex("Rainbow Projectiles").enabled)
            {
                float h = (Time.frameCount / 180f) % 1f;
                Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                r = (byte)(rgbcolor.r * 255);
                g = (byte)(rgbcolor.g * 255);
                b = (byte)(rgbcolor.b * 255);
            }

            if (GetIndex("Hard Rainbow Projectiles").enabled)
            {
                float h = (Time.frameCount / 180f) % 1f;
                Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                r = (byte)(Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f);
                g = (byte)(Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f);
                b = (byte)(Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f);
            }

            if (GetIndex("Black Projectiles").enabled)
            {
                r = 0;
                g = 0;
                b = 0;
            }

            if (GetIndex("Custom Colored Projectiles").enabled)
            {
                r = (byte)((red / 10f) * 255);
                g = (byte)((green / 10f) * 255);
                b = (byte)((blue / 10f) * 255);
            }

            return new Color32(r, g, b, 255);
        }

        public static void ProjectileSpam()
        {
            int projIndex = projmode * 2;

            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                if (GetIndex("Random Projectile").enabled)
                    projIndex = UnityEngine.Random.Range(0, ProjectileObjectNames.Length - 1);
                
                string projectilename = ProjectileObjectNames[projIndex];

                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 charvel = GorillaLocomotion.GTPlayer.Instance.RigidbodyVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.GTPlayer.Instance.RigidbodyVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= ShootStrength * (shootCycle >= 2 ? 2f : -2f);
                    }
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                    charvel = GorillaLocomotion.GTPlayer.Instance.RigidbodyVelocity + (TrueRightHand().forward * ShootStrength);

                if (GetIndex("Random Direction").enabled)
                    charvel = RandomVector3(100f);

                if (GetIndex("Above Players").enabled)
                {
                    VRRig targetRig = GetCurrentTargetRig();
                    startpos = targetRig.transform.position + new Vector3(0f, 1f, 0f);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 2f, UnityEngine.Random.Range(-2f, 2f));
                    charvel = Vector3.zero;
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
                    charvel = new Vector3(UnityEngine.Random.Range(-10, 10), -15, UnityEngine.Random.Range(-10, 10));
                }

                if (GetIndex("Include Hand Velocity").enabled)
                    charvel = GorillaLocomotion.GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);

                BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
            }
        }

        public static void GiveProjectileSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    int projIndex = projmode * 2;
                    
                    if (GetIndex("Random Projectile").enabled)
                        projIndex = UnityEngine.Random.Range(0, ProjectileObjectNames.Length);
                    
                    string projectilename = ProjectileObjectNames[projIndex];

                    Vector3 startpos = lockTarget.rightHandTransform.position;
                    Vector3 charvel = Vector3.zero;

                    if (GetIndex("Shoot Projectiles").enabled)
                        charvel = (lockTarget.rightHandTransform.transform.forward * ShootStrength);

                    if (GetIndex("Finger Gun Projectiles").enabled)
                        charvel = (lockTarget.rightHandTransform.transform.forward * ShootStrength);

                    if (GetIndex("Random Direction").enabled)
                        charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));

                    if (GetIndex("Above Players").enabled)
                    {
                        VRRig targetRig = GetCurrentTargetRig();
                        startpos = targetRig.transform.position + new Vector3(0f, 1f, 0f);
                    }

                    if (GetIndex("Rain Projectiles").enabled)
                    {
                        startpos = lockTarget.headMesh.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 3f, UnityEngine.Random.Range(-3f, 3f));
                        charvel = Vector3.zero;
                    }

                    if (GetIndex("Projectile Aura").enabled)
                    {
                        float time = Time.frameCount;
                        startpos = lockTarget.headMesh.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                    }

                    if (GetIndex("Projectile Fountain").enabled)
                    {
                        startpos = lockTarget.headMesh.transform.position + new Vector3(0, 1, 0);
                        charvel = new Vector3(UnityEngine.Random.Range(-10, 10), -15, UnityEngine.Random.Range(-10, 10));
                    }

                    BetaFireProjectile(projectilename, startpos, charvel, CalculateProjectileColor());
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

        public static void SlingshotHelper()
        {
            GameObject slingy = GetObject("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
            if (slingy != null)
            {
                Slingshot yay = slingy.GetComponent<Slingshot>();
                yay.itemState = TransferrableObject.ItemStates.State2;
            }
        }

        public static void Aimbot()
        {
            if (rightPrimary)
            {
                foreach (SlingshotProjectile projectile in GetAllType<SlingshotProjectile>())
                {
                    if (projectile.projectileOwner == NetworkSystem.Instance.LocalPlayer)
                    {
                        projectile.gameObject.transform.position = GetRandomVRRig(false).headConstraint.transform.position;
                        RPCProtection();
                    }
                }
            }
        }

        public static void ImpactSpam()
        {
            if ((rightGrab || Mouse.current.leftButton.isPressed) && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray, 512f, NoInvisLayerMask());
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
                    }
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, TrueRightHand().forward, out var Ray, 512f, NoInvisLayerMask());
                    startpos = Ray.point;
                }

                if (GetIndex("Above Players").enabled)
                {
                    VRRig targetRig = GetCurrentTargetRig();
                    startpos = targetRig.transform.position + new Vector3(0f, 1f, 0f);
                }

                if (GetIndex("Rain Projectiles").enabled)
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-3f, 3f), 3f, UnityEngine.Random.Range(-3f, 3f));

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);

                BetaFireImpact(startpos, CalculateProjectileColor());
                RPCProtection();

                if (projDebounceType > 0f)
                    projDebounce = Time.time + projDebounceType + 0.05f;
            }
        }

        private static bool lastLeftGrab;
        private static bool lastRightGrab;
        public static void GrabProjectile()
        {
            int projIndex = projmode * 2;
            if (GetIndex("Random Projectile").enabled)
                projIndex = UnityEngine.Random.Range(0, ProjectileObjectNames.Length / 2) * 2;

            if (leftGrab && !lastLeftGrab)
            {
                SnowballThrowable Projectile = GetProjectile(ProjectileObjectNames[projIndex]);
                if (!Projectile.gameObject.activeSelf)
                {
                    Projectile.SetSnowballActiveLocal(true);
                    Projectile.velocityEstimator = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetOrAddComponent<GorillaVelocityEstimator>();
                    Projectile.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    Projectile.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                    Color TargetProjectileColor = CalculateProjectileColor();
                    VRRig.LocalRig.SetThrowableProjectileColor(true, CalculateProjectileColor());
                    Projectile.randomizeColor = true;
                    Projectile.ApplyColor(TargetProjectileColor);
                }
            }

            if (rightGrab && !lastRightGrab)
            {
                SnowballThrowable Projectile = GetProjectile(ProjectileObjectNames[projIndex + 1]);

                if (!Projectile.gameObject.activeSelf)
                {
                    Projectile.SetSnowballActiveLocal(true);
                    Projectile.velocityEstimator = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetOrAddComponent<GorillaVelocityEstimator>();
                    Projectile.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    Projectile.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                    Color TargetProjectileColor = CalculateProjectileColor();
                    VRRig.LocalRig.SetThrowableProjectileColor(false, CalculateProjectileColor());
                    Projectile.randomizeColor = true;
                    Projectile.ApplyColor(TargetProjectileColor);
                }
            }

            lastLeftGrab = leftGrab;
            lastRightGrab = rightGrab;
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

                BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.white);
            }
        }

        public static void Semen()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.white);
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.green);
            }
        }

        public static void Spit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("WaterBalloonLeftAnchor",  startpos, charvel, Color.cyan);
            }
        }

        public static void UrineGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.4f, 0f) + (lockTarget.transform.forward * 0.2f);
                    Vector3 charvel = lockTarget.transform.forward * 8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.yellow);
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

        public static void FecesGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.white);
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

        public static void SemenGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.transform.position + new Vector3(0f, -0.4f, 0f) + (lockTarget.transform.forward * 0.2f);
                    Vector3 charvel = lockTarget.transform.forward * 8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, Color.white);
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

        public static void VomitGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.headMesh.transform.position + (lockTarget.headMesh.transform.forward * 0.4f) + (lockTarget.headMesh.transform.up * -0.05f);
                    Vector3 charvel = lockTarget.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, Color.green);
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

        public static void SpitGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (gunLocked && lockTarget != null)
                {
                    Vector3 startpos = lockTarget.headMesh.transform.position + (lockTarget.headMesh.transform.forward * 0.4f) + (lockTarget.headMesh.transform.up * -0.05f);
                    Vector3 charvel = lockTarget.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("WaterBalloonLeftAnchor", startpos, charvel, Color.cyan);
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

        public static void ProjectileBlindGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    BetaFireProjectile("EggLeftHand_Anchor Variant", lockTarget.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)).ActorNumber } });
                
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

        public static void ProjectileBlindAll()
        {
            if (rightTrigger > 0.5f)
                BetaFireProjectile("EggLeftHand_Anchor Variant", GetCurrentTargetRig().headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(GetCurrentTargetRig())).ActorNumber } });
        }

        public static void ProjectileBlindPlayer(NetPlayer player)
        {
            VRRig rig = GetVRRigFromPlayer(player);
            BetaFireProjectile("EggLeftHand_Anchor Variant", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } });
        }

        public static void ProjectileLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                    BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", lockTarget.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + lockTarget.headMesh.transform.forward * -0.7f, new Vector3(0f, 15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(lockTarget)).ActorNumber } } );

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

        public static void ProjectileLagAll()
        {
            if (rightTrigger > 0.5f)
                BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", GetCurrentTargetRig().headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + GetCurrentTargetRig().headMesh.transform.forward * -0.7f, new Vector3(0f, 15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(GetCurrentTargetRig())).ActorNumber } });
        }

        public static void ProjectileLagPlayer(NetPlayer player)
        {
            VRRig rig = GetVRRigFromPlayer(player);
            BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", rig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + rig.headMesh.transform.forward * -0.7f, new Vector3(0f, 15f, 0f), Color.black, new RaiseEventOptions { TargetActors = new int[] { NetPlayerToPlayer(GetPlayerFromVRRig(rig)).ActorNumber } });
        }
    }
}
