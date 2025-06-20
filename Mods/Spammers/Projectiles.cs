﻿using ExitGames.Client.Photon;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
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
        public static GorillaVelocityEstimator VelocityEstimator;

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
            "HotDogRightAnchor"
        };

        public static Coroutine RigCoroutine;
        public static IEnumerator EnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color) // This code is really bad
        {
            if (VelocityEstimator == null)
            {
                GameObject thepointless = new GameObject("Blank GVE");
                VelocityEstimator = thepointless.AddComponent<GorillaVelocityEstimator>() as GorillaVelocityEstimator;
            }
            VelocityEstimator.enabled = false;

            SnowballThrowable Throwable = GetProjectile(projectileName);
            if (!Throwable.gameObject.activeSelf)
            {
                Throwable.SetSnowballActiveLocal(true);
                Throwable.velocityEstimator = VelocityEstimator;
                Throwable.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                Throwable.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
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

                    Vector3 oldVel = GorillaTagger.Instance.GetComponent<Rigidbody>().velocity;

                    Vector3 oldPos = Throwable.transform.position;
                    Throwable.randomizeColor = true;
                    Throwable.transform.position = startpos;

                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = charvel;
                    VRRig.LocalRig.SetThrowableProjectileColor(true, color);
                    Throwable.PerformSnowballThrowAuthority();

                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = oldVel;
                    RPCProtection();

                    Throwable.transform.position = oldPos;
                    Throwable.randomizeColor = false;
                } catch (Exception e) { LogManager.LogError($"Projectile error: {e.Message}"); }

                if (projDebounceType > 0f)
                    projDebounce = Time.time + Mathf.Max(projDebounceType, 0.16f);
            }
        }

        public static void BetaFireImpact(Vector3 position, float r, float g, float b)
        {
            if (Time.time > projDebounce)
            {
                object[] impactSendData = new object[6];
                impactSendData[0] = position;
                impactSendData[1] = r;
                impactSendData[2] = g;
                impactSendData[3] = b;
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

        public static void ChangeProjectile()
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
                "Hot Dog"
            };

            projmode++;
            if (projmode >= shortProjectileNames.Length)
                projmode = 0;

            GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + shortProjectileNames[projmode] + "</color><color=grey>]</color>";
        }

        public static int shootCycle = 1;
        public static void ChangeShootSpeed()
        {
            shootCycle++;
            if (shootCycle > 3)
                shootCycle = 0;

            float[] ShootStrengthTypes = new float[]
            {
                9.72f,
                19.44f,
                38.88f,
                10000000f
            };

            string[] ShootStrengthNames = new string[]
            {
                "Slow",
                "Medium",
                "Fast",
                "Instant"
            };

            ShootStrength = ShootStrengthTypes[shootCycle];
            GetIndex("Change Shoot Speed").overlapText = "Change Shoot Speed <color=grey>[</color><color=green>" + ShootStrengthNames[shootCycle] + "</color><color=grey>]</color>";
        }

        public static void IncreaseRed()
        {
            red += 0.1f;
            if (red > 1.05f)
                red = 0f;

            GetIndex("RedProj").overlapText = "Red <color=grey>[</color><color=green>" + Mathf.Floor(red * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseGreen()
        {
            green += 0.1f;

            if (green > 1.05f)
                green = 0f;

            GetIndex("GreenProj").overlapText = "Green <color=grey>[</color><color=green>" + Mathf.Floor(green * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseBlue()
        {
            blue += 0.1f;

            if (blue > 1.05f)
                blue = 0f;

            GetIndex("BlueProj").overlapText = "Blue <color=grey>[</color><color=green>" + Mathf.Floor(blue * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void ProjectileDelay()
        {
            projDebounceType += 0.1f;

            if (projDebounceType > 1.05f)
                projDebounceType = 0.1f; // Was 0 but that was bannable so uhh

            GetIndex("Projectile Delay").overlapText = "Projectile Delay <color=grey>[</color><color=green>" + (Mathf.Floor(projDebounceType * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
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
                    charvel = GorillaLocomotion.GTPlayer.Instance.RigidbodyVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * -ShootStrength);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= (ShootStrength * -2f);
                    }
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                    charvel = GorillaLocomotion.GTPlayer.Instance.RigidbodyVelocity + (TrueRightHand().forward * -ShootStrength);

                if (GetIndex("Random Direction").enabled)
                    charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));

                if (GetIndex("Above Players").enabled)
                {
                    charvel = Vector3.zero;
                    VRRig[] targetRigs = GorillaParent.instance.vrrigs
                        .Where(rig => rig != null && rig != VRRig.LocalRig && Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, rig.transform.position) < 3.9f)
                        .ToArray();

                    VRRig targetRig = targetRigs[UnityEngine.Random.Range(0, targetRigs.Length - 1)];
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

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                BetaFireProjectile(projectilename, startpos, charvel, new Color(randa / 255f, randb / 255f, randc / 255f));
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
                        charvel = (lockTarget.rightHandTransform.transform.forward * -ShootStrength);

                    if (GetIndex("Finger Gun Projectiles").enabled)
                        charvel = (lockTarget.rightHandTransform.transform.forward * -ShootStrength);

                    if (GetIndex("Random Direction").enabled)
                        charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));

                    if (GetIndex("Above Players").enabled)
                    {
                        charvel = Vector3.zero;
                        VRRig[] targetRigs = GorillaParent.instance.vrrigs
                            .Where(rig => rig != null && rig != VRRig.LocalRig && Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, rig.transform.position) < 3.9f)
                            .ToArray();

                        VRRig targetRig = targetRigs[UnityEngine.Random.Range(0, targetRigs.Length - 1)];
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

                    float randa = 255f;
                    float randb = 255f;
                    float randc = 255f;

                    if (GetIndex("Random Color").enabled)
                    {
                        randa = UnityEngine.Random.Range(0, 255);
                        randb = UnityEngine.Random.Range(0, 255);
                        randc = UnityEngine.Random.Range(0, 255);
                    }

                    if (GetIndex("Rainbow Projectiles").enabled)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                        randa = rgbcolor.r * 255;
                        randb = rgbcolor.g * 255;
                        randc = rgbcolor.b * 255;
                    }

                    if (GetIndex("Hard Rainbow Projectiles").enabled)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                        randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                        randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                        randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                    }

                    if (GetIndex("Black Projectiles").enabled)
                    {
                        randa = 0f;
                        randb = 0f;
                        randc = 0f;
                    }

                    if (GetIndex("No Texture Projectiles").enabled)
                    {
                        randa = 25500f;
                        randb = 0f;
                        randc = 25500f;
                    }

                    if (GetIndex("Custom Colored Projectiles").enabled)
                    {
                        randa = red * 255;
                        randb = green * 255;
                        randc = blue * 255;
                    }

                    BetaFireProjectile(projectilename, startpos, charvel, new Color(randa / 255f, randb / 255f, randc / 255f));
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
            GameObject slingy = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
            if (slingy != null)
            {
                Slingshot yay = slingy.GetComponent<Slingshot>();
                yay.itemState = TransferrableObject.ItemStates.State2;
            }
        }

        public static bool isFiring;
        public static void RapidFireSlingshot()
        {
            if (rightPrimary)
            {
                GameObject slingshotObject = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
                if (slingshotObject != null)
                {
                    Slingshot slingshot = slingshotObject.GetComponent<Slingshot>();
                    slingshot.itemState = TransferrableObject.ItemStates.State2;
                    slingshot.minTimeToLaunch = -1f;

                    ControllerInputPoller.instance.rightControllerIndexFloat = isFiring ? 1f : 0f;
                    isFiring = !isFiring;
                }
            }
        }

        public static void Aimbot()
        {
            if (rightPrimary)
            {
                foreach (SlingshotProjectile projectile in UnityEngine.Object.FindObjectsOfType<SlingshotProjectile>())
                {
                    if (projectile.projectileOwner == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    VRRig[] targetRigs = GorillaParent.instance.vrrigs
                        .Where(rig => rig != null && rig != VRRig.LocalRig && Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, rig.transform.position) < 3.9f)
                        .ToArray();

                    VRRig targetRig = targetRigs[UnityEngine.Random.Range(0, targetRigs.Length - 1)];
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

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                BetaFireImpact(startpos, randa / 255f, randb / 255f, randc / 255f);
                RPCProtection();

                if (projDebounceType > 0f)
                    projDebounce = Time.time + projDebounceType + 0.05f;
            }

            if (leftGrab && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.leftHandTransform.position;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out var Ray, 512f, NoInvisLayerMask());
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, TrueRightHand().forward, out var Ray, 512f, NoInvisLayerMask());
                    startpos = Ray.point;
                }

                if (GetIndex("Above Players").enabled)
                {
                    VRRig[] targetRigs = GorillaParent.instance.vrrigs
                        .Where(rig => rig != null && rig != VRRig.LocalRig && Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, rig.transform.position) < 3.9f)
                        .ToArray();

                    VRRig targetRig = targetRigs[UnityEngine.Random.Range(0, targetRigs.Length - 1)];
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

                float randa = 255f;
                float randb = 255f;
                float randc = 255f;

                if (GetIndex("Random Color").enabled)
                {
                    randa = UnityEngine.Random.Range(0, 255);
                    randb = UnityEngine.Random.Range(0, 255);
                    randc = UnityEngine.Random.Range(0, 255);
                }

                if (GetIndex("Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    Color rgbcolor = Color.HSVToRGB(h, 1f, 1f);
                    randa = (Mathf.Floor(rgbcolor.r * 2f) / 2f * 255f) * 100f;
                    randb = (Mathf.Floor(rgbcolor.g * 2f) / 2f * 255f) * 100f;
                    randc = (Mathf.Floor(rgbcolor.b * 2f) / 2f * 255f) * 100f;
                }

                if (GetIndex("Black Projectiles").enabled)
                {
                    randa = 0f;
                    randb = 0f;
                    randc = 0f;
                }

                if (GetIndex("No Texture Projectiles").enabled)
                {
                    randa = 25500f;
                    randb = 0f;
                    randc = 25500f;
                }

                if (GetIndex("Custom Colored Projectiles").enabled)
                {
                    randa = red * 255;
                    randb = green * 255;
                    randc = blue * 255;
                }

                BetaFireImpact(startpos, randa / 255f, randb / 255f, randc / 255f);
                RPCProtection();
            }
        }

        public static bool lastLeftGrab = false;
        public static bool lastRightGrab = false;
        public static void GrabProjectile()
        {
            int projIndex = projmode * 2;
            if (GetIndex("Random Projectile").enabled)
                projIndex = UnityEngine.Random.Range(0, ProjectileObjectNames.Length);

            if (leftGrab)
            {
                SnowballThrowable Projectile = GetProjectile(ProjectileObjectNames[projIndex]);
                if (!Projectile.gameObject.activeSelf)
                {
                    Projectile.SetSnowballActiveLocal(true);
                    Projectile.velocityEstimator = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>();
                    Projectile.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    Projectile.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                }
            }

            if (rightGrab)
            {
                SnowballThrowable Projectile = GetProjectile(ProjectileObjectNames[projIndex + 1]);

                if (!Projectile.gameObject.activeSelf)
                {
                    Projectile.SetSnowballActiveLocal(true);
                    Projectile.velocityEstimator = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>();
                    Projectile.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    Projectile.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                }
            }
        }

        public static void Urine()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * -8.33f;

                BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, new Color(255f, 255f, 0f));
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
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * -8.33f;

                BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, new Color(255f, 255f, 255f));
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * -8.33f;

                BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, new Color(0f, 255f, 0f));
            }
        }

        public static void Spit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * -8.33f;

                BetaFireProjectile("WaterBalloonLeftAnchor",  startpos, charvel, new Color(0f, 255f, 255f));
            }
        }

        public static void ServersidedTracers()
        {
            int projIndex = projmode * 2;
            if (GetIndex("Random Projectile").enabled)
                projIndex = UnityEngine.Random.Range(0, ProjectileObjectNames.Length);
            
            string projectilename = ProjectileObjectNames[projIndex];

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!PlayerIsTagged(VRRig.LocalRig))
                {
                    VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (PlayerIsTagged(vrrig) && vrrig != VRRig.LocalRig)
                    {
                        Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                        Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                        charvel.Normalize();
                        charvel *= 100f;

                        BetaFireProjectile(projectilename, startpos, charvel, new Color32(0, 255, 0, 255));
                    }
                }
                else
                {
                    VRRig vrrig = GetRandomVRRig(false);
                    if (!PlayerIsTagged(vrrig) && vrrig != VRRig.LocalRig)
                    {
                        Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                        Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                        charvel.Normalize();
                        charvel *= 100f;

                        BetaFireProjectile(projectilename, startpos, charvel, new Color32(0, 255, 0, 255));
                    }
                }
            }
            else
            {
                VRRig vrrig = GetRandomVRRig(false);
                if (vrrig != VRRig.LocalRig)
                {
                    Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                    Vector3 charvel = vrrig.transform.position - GorillaTagger.Instance.rightHandTransform.position;
                    charvel.Normalize();
                    charvel *= 100f;

                    BetaFireProjectile(projectilename, startpos, charvel, new Color32(0, 255, 0, 255));
                }
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
                    Vector3 charvel = lockTarget.transform.forward * -8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, new Color(255f, 255f, 0f, 1f));
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
                    Vector3 charvel = lockTarget.transform.forward *- 8.33f;

                    BetaFireProjectile("ScienceCandyLeftAnchor", startpos, charvel, new Color(255f, 255f, 255f, 1f));
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
                    Vector3 charvel = lockTarget.headMesh.transform.forward * -8.33f;

                    BetaFireProjectile("FishFoodLeftAnchor", startpos, charvel, new Color(0f, 255f, 0f, 1f));
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
                    Vector3 charvel = lockTarget.headMesh.transform.forward * -8.33f;

                    BetaFireProjectile("WaterBalloonLeftAnchor", startpos, charvel, new Color(0f, 255f, 255f, 1f));
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
                    BetaFireProjectile("EggLeftHand_Anchor Variant", lockTarget.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black);
                
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

        private static float playerSwapDelay;
        private static VRRig targetRig;
        public static void ProjectileBlindAll()
        {
            if (rightTrigger > 0.5f)
            {
                if (Time.time > playerSwapDelay || targetRig == null)
                {
                    playerSwapDelay = Time.time + 1f;
                    targetRig = GetRandomVRRig(false);
                }

                BetaFireProjectile("EggLeftHand_Anchor Variant", targetRig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -15f, 0f), Color.black);
            }
        }
    }
}
