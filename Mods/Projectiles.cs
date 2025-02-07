using BepInEx;
using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Patches;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using System;
using System.Reflection;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    public class Projectiles
    {
        public static GorillaVelocityEstimator jackshit = null;

        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, bool nodelay = false) // This code is really bad
        {
            if (jackshit == null)
            {
                GameObject thepointless = new GameObject("Blank GVE");
                jackshit = thepointless.AddComponent<GorillaVelocityEstimator>() as GorillaVelocityEstimator;
            }
            jackshit.enabled = false;

            SnowballThrowable fart = GetProjectile(InternalProjectileNames[Array.IndexOf(ExternalProjectileNames, projectileName)]);
            if (!fart.gameObject.activeSelf)
            {
                fart.SetSnowballActiveLocal(true);
                fart.velocityEstimator = jackshit;
                fart.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                fart.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
            if (Time.time > projDebounce)
            {
                try
                {
                    Vector3 startpos = position;
                    Vector3 charvel = velocity;

                    Vector3 oldVel = GorillaTagger.Instance.GetComponent<Rigidbody>().velocity;

                    Vector3 oldPos = fart.transform.position;
                    fart.randomizeColor = true;
                    fart.transform.position = startpos;

                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = charvel;
                    GorillaTagger.Instance.offlineVRRig.SetThrowableProjectileColor(true, color);
                    MethodInfo lsm = typeof(SnowballThrowable).GetMethod("PerformSnowballThrowAuthority", BindingFlags.NonPublic | BindingFlags.Instance);
                    lsm.Invoke(fart, new object[] { });
                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = oldVel;
                    RPCProtection();

                    fart.transform.position = oldPos;
                    fart.randomizeColor = false;
                } catch (Exception e) { UnityEngine.Debug.Log(e.Message); }

                if (projDebounceType > 0f && !nodelay)
                    projDebounce = Time.time + projDebounceType + 0.07f;
            }
        }

        public static void BetaFireImpact(Vector3 position, float r, float g, float b, bool noDelay = false)
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
                try
                {
                    PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
                }
                catch { /*wtf*/ }
                if (projDebounceType > 0f && !noDelay)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }
        }

        public static void ChangeProjectile()
        {
            string[] shortProjectileNames = new string[] {
                "Snowball",
                "Water Balloon",
                "Lava Rock",
                "Present",
                "Mentos",
                "Fish Food",
                "Candy",
                "Voting Rock",
                "Bat",
                "Explosion",
                "Apple"
            };

            projmode++;
            if (projmode > (shortProjectileNames.Length - 1))
            {
                projmode = 0;
            }

            SnowballPatch.enabled = projmode == 8 || projmode == 9;
            SnowballPatch.minusIndex = projmode == 9 ? 2 : 1;

            GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + shortProjectileNames[projmode] + "</color><color=grey>]</color>";
        }

        public static void ChangeTrail()
        {
            trailmode++;
            if (trailmode > 8)
            {
                trailmode = 0;
            }

            string[] shortTrailNames = new string[]
            {
                "Regular",
                "Laser",
                "Pride",
                "Pink",
                "Ice",
                "Bow",
                "Lava",
                "Spider",
                "None"
            };
        }

        public static void ChangeShootSpeed()
        {
            shootCycle++;
            if (shootCycle > 3)
            {
                shootCycle = 0;
            }

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
            {
                red = 0f;
            }

            GetIndex("RedProj").overlapText = "Red <color=grey>[</color><color=green>" + Mathf.Floor(red * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseGreen()
        {
            green += 0.1f;
            if (green > 1.05f)
            {
                green = 0f;
            }

            GetIndex("GreenProj").overlapText = "Green <color=grey>[</color><color=green>" + Mathf.Floor(green * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseBlue()
        {
            blue += 0.1f;
            if (blue > 1.05f)
            {
                blue = 0f;
            }

            GetIndex("BlueProj").overlapText = "Blue <color=grey>[</color><color=green>" + Mathf.Floor(blue * 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void ProjectileDelay()
        {
            projDebounceType += 0.1f;
            if (projDebounceType > 1.05f)
            {
                projDebounceType = 0.1f; // Was 0 but that was bannable so uhh
            }

            GetIndex("Projectile Delay").overlapText = "Projectile Delay <color=grey>[</color><color=green>" + (Mathf.Floor(projDebounceType * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void ProjectileSpam()
        {
            int projIndex = projmode;
            int trailIndex = trailmode;

            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                if (GetIndex("Random Projectile").enabled)
                {
                    projIndex = UnityEngine.Random.Range(0, ExternalProjectileNames.Length - 1);
                }
                string projectilename = ExternalProjectileNames[projIndex];

                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 charvel = GorillaLocomotion.Player.Instance.RigidbodyVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.RigidbodyVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 512f, NoInvisLayerMask());
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= (ShootStrength * 2);
                    }
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.RigidbodyVelocity + (TrueRightHand().forward * ShootStrength);
                    //charvel = GorillaLocomotion.Player.Instance.currentVelocity + GorillaTagger.Instance.rightHandTransform.forward - GorillaTagger.Instance.offlineVRRig.rightHand.trackingRotationOffset;
                }

                if (GetIndex("Random Direction").enabled)
                {
                    charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                }

                if (GetIndex("Above Players").enabled)
                {
                    charvel = Vector3.zero;
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
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
                    charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
                }

                if (GetIndex("Include Hand Velocity").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
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
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
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
                //UnityEngine.Debug.Log("updated stuff");

                BetaFireProjectile(projectilename, startpos, charvel, new Color(randa / 255f, randb / 255f, randc / 255f));
                //UnityEngine.Debug.Log("fried proj");
            }
        }

        public static void GiveProjectileSpamGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    int projIndex = projmode;
                    int trailIndex = trailmode;
                    
                    if (GetIndex("Random Projectile").enabled)
                    {
                        projIndex = UnityEngine.Random.Range(0, 4);
                    }
                    string projectilename = ExternalProjectileNames[projIndex];

                    Vector3 startpos = whoCopy.rightHandTransform.position;
                    Vector3 charvel = Vector3.zero;

                    if (GetIndex("Shoot Projectiles").enabled)
                    {
                        charvel = (whoCopy.rightHandTransform.transform.forward * ShootStrength);
                    }

                    if (GetIndex("Finger Gun Projectiles").enabled)
                    {
                        charvel = (TrueRightHand().forward * ShootStrength);
                    }

                    if (GetIndex("Random Direction").enabled)
                    {
                        charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));
                    }

                    if (GetIndex("Above Players").enabled)
                    {
                        charvel = Vector3.zero;
                        //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                        startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                    }

                    if (GetIndex("Rain Projectiles").enabled)
                    {
                        startpos = whoCopy.headMesh.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                        charvel = Vector3.zero;
                    }

                    if (GetIndex("Projectile Aura").enabled)
                    {
                        float time = Time.frameCount;
                        startpos = whoCopy.headMesh.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                    }

                    if (GetIndex("Projectile Fountain").enabled)
                    {
                        startpos = whoCopy.headMesh.transform.position + new Vector3(0, 1, 0);
                        charvel = new Vector3(UnityEngine.Random.Range(-10, 10), 15, UnityEngine.Random.Range(-10, 10));
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
                        UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                        randa = rgbcolor.r * 255;
                        randb = rgbcolor.g * 255;
                        randc = rgbcolor.b * 255;
                    }

                    if (GetIndex("Hard Rainbow Projectiles").enabled)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
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
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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
        
        public static void RapidFireSlingshot()
        {
            if (rightPrimary)
            {
                GameObject slingy = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
                if (slingy != null)
                {
                    Slingshot yay = slingy.GetComponent<Slingshot>();
                    yay.itemState = TransferrableObject.ItemStates.State2;
                    System.Type type = yay.GetType();
                    FieldInfo fieldInfo = type.GetField("minTimeToLaunch", BindingFlags.NonPublic | BindingFlags.Instance);
                    fieldInfo.SetValue(yay, -1f);
                    ControllerInputPoller.instance.rightControllerIndexFloat = lastSlingThing ? 1f : 0f;
                    lastSlingThing = !lastSlingThing;
                }
            }
        }

        public static void Aimbot()
        {
            if (rightPrimary)
            {
                foreach (SlingshotProjectile projectile in GameObject.FindObjectsOfType<SlingshotProjectile>())
                {
                    if (projectile.projectileOwner == (NetPlayer)PhotonNetwork.LocalPlayer)
                    {
                        projectile.gameObject.transform.position = RigManager.GetRandomVRRig(false).headConstraint.transform.position;
                        RPCProtection();
                    }
                }
            }
        }

        /*
        public static void DisableProjectileBomb()
        {
            if (ProjBombObject != null)
            {
                UnityEngine.Object.Destroy(ProjBombObject);
                ProjBombObject = null;
            }
        }*/

        /*
        public static void RandomColorSnowballs()
        {
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomizeColor = true;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomizeColor = true;
        }

        public static void NoRandomColorSnowballs()
        {
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomizeColor = false;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomizeColor = false;
        }

        public static void BlackSnowballs()
        {
            //currentProjectileColor = Color.black;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomizeColor = true;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomColorHSVRanges = new GTColor.HSVRanges(0f, 0f, 0f, 0f, 0f, 0f);
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomizeColor = true;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomColorHSVRanges = new GTColor.HSVRanges(0f, 0f, 0f, 0f, 0f, 0f);
        }

        public static void FixBlackSnowballs()
        {
            //currentProjectileColor = Color.white;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomizeColor = false;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor").transform.Find("LMACF.").GetComponent<SnowballThrowable>().randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomizeColor = true;
            GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballLeftAnchor").transform.Find("LMACE.").GetComponent<SnowballThrowable>().randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);
        }
        */

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
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
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
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
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

                /*PhotonView.Get(GorillaGameManager.instance).RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, new object[]
                {
                    startpos,
                    randa / 255f,
                    randb / 255f,
                    randc / 255f,
                    1f,
                    1
                });*/
                BetaFireImpact(startpos, randa / 255f, randb / 255f, randc / 255f);
                RPCProtection();

                if (projDebounceType > 0f)
                {
                    projDebounce = Time.time + projDebounceType + 0.05f;
                }
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
                    //List<VRRig> rigs = GorillaParent.instance.vrrigs;
                    startpos = GetRandomVRRig(false).transform.position + new Vector3(0f, 1f, 0f);//rigs[UnityEngine.Random.Range(0, rigs.Count)].transform.position + new Vector3(0, 1, 0);
                }

                if (GetIndex("Rain Projectiles").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
                }

                if (GetIndex("Projectile Aura").enabled)
                {
                    float time = Time.frameCount;
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(MathF.Cos(time / 20), 2, MathF.Sin(time / 20));
                }

                if (GetIndex("Projectile Fountain").enabled)
                {
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0, 1, 0);
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
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    randa = rgbcolor.r * 255;
                    randb = rgbcolor.g * 255;
                    randc = rgbcolor.b * 255;
                }

                if (GetIndex("Hard Rainbow Projectiles").enabled)
                {
                    float h = (Time.frameCount / 180f) % 1f;
                    UnityEngine.Color rgbcolor = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
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

                /*PhotonView.Get(GorillaGameManager.instance).RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, new object[]
                {
                    startpos,
                    randa / 255f,
                    randb / 255f,
                    randc / 255f,
                    1f,
                    1
                });*/
                BetaFireImpact(startpos, randa / 255f, randb / 255f, randc / 255f);
                RPCProtection();
            }
        }

        public static bool lastLeftGrab = false;
        public static bool lastRightGrab = false;
        public static void GrabProjectile()
        {
            int projIndex = projmode;
            if (GetIndex("Random Projectile").enabled)
            {
                projIndex = UnityEngine.Random.Range(0, 4);
            }
            if (jackshit == null)
            {
                GameObject thepointless = new GameObject("Blank GVE");
                jackshit = thepointless.AddComponent<GorillaVelocityEstimator>() as GorillaVelocityEstimator;
            }
            jackshit.enabled = false;

            if (leftGrab)
            {
                SnowballThrowable fart = GetProjectile(InternalProjectileNames[projIndex]);
                if (!fart.gameObject.activeSelf)
                {
                    fart.SetSnowballActiveLocal(true);
                    fart.velocityEstimator = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>();
                    fart.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    fart.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                }
            }

            if (rightGrab)
            {
                string lol = InternalProjectileNamesRight[projIndex];
                
                SnowballThrowable fart = GetProjectile(lol);
                if (!fart.gameObject.activeSelf)
                {
                    fart.SetSnowballActiveLocal(true);
                    fart.velocityEstimator = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>();
                    fart.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    fart.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                }
            }
        }
        /*
        public static void InstantCrankElf()
        {
            foreach (ElfLauncher elf in GetElves())
            {
                TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                if (transobj.IsLocalObject())
                {
                    elf.GetType().GetField("crankShootThreshold", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).SetValue(elf, 0f);
                }
            }
        }

        public static void DisableInstantCrankElf()
        {
            foreach (ElfLauncher elf in GetElves())
            {
                TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                if (transobj.IsLocalObject())
                {
                    elf.GetType().GetField("crankShootThreshold", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).SetValue(elf, 360f);
                }
            }
        }

        public static void ElfLauncherSpam()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                bool didThing = false;
                foreach (ElfLauncher elf in GetElves())
                {
                    TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                    if (transobj.IsLocalObject())
                    {
                        didThing = true;
                        RubberDuckEvents rde = (RubberDuckEvents)elf.GetType().GetField("_events", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(elf);
                        rde.Activate.RaiseAll(new object[] { GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward * (ShootStrength / 2f) });
                        RPCProtection();
                    }
                }
                if (!didThing)
                {
                    CosmeticsController.instance.ApplyCosmeticItemToSet(GorillaTagger.Instance.offlineVRRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMANE."), true, false);
                    CosmeticsController.instance.UpdateWornCosmetics(true);
                    archiveelves = null;
                }
            } else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void ElfGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                    bool didThing = false;
                    foreach (ElfLauncher elf in GetElves())
                    {
                        TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                        if (transobj.IsLocalObject())
                        {
                            didThing = true;
                            RubberDuckEvents rde = (RubberDuckEvents)elf.GetType().GetField("_events", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(elf);
                            rde.Activate.RaiseAll(new object[] { NewPointer.transform.position + new Vector3(0f, 1f, 0f), Vector3.zero });
                            RPCProtection();
                        }
                    }
                    if (!didThing)
                    {
                        CosmeticsController.instance.ApplyCosmeticItemToSet(GorillaTagger.Instance.offlineVRRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMANE."), true, false);
                        CosmeticsController.instance.UpdateWornCosmetics(true);
                        archiveelves = null;
                    }
                } else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void ElfAirstrikeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);

                    bool didThing = false;
                    foreach (ElfLauncher elf in GetElves())
                    {
                        TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                        if (transobj.IsLocalObject())
                        {
                            didThing = true;
                            RubberDuckEvents rde = (RubberDuckEvents)elf.GetType().GetField("_events", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(elf);
                            rde.Activate.RaiseAll(new object[] { whoCopy.headMesh.transform.position + new Vector3(0f, 20f, 0f), Vector3.down * (ShootStrength / 2f) });
                            RPCProtection();
                        }
                    }
                    if (!didThing)
                    {
                        CosmeticsController.instance.ApplyCosmeticItemToSet(GorillaTagger.Instance.offlineVRRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMANE."), true, false);
                        CosmeticsController.instance.UpdateWornCosmetics(true);
                        archiveelves = null;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void ElfAnnoyGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.4897f, 16.9286f, -120.1083f);
                    
                    bool didThing = false;
                    foreach (ElfLauncher elf in GetElves())
                    {
                        TransferrableObject transobj = (TransferrableObject)Traverse.Create(elf).Field("parentHoldable").GetValue();
                        if (transobj.IsLocalObject())
                        {
                            didThing = true;
                            RubberDuckEvents rde = (RubberDuckEvents)elf.GetType().GetField("_events", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(elf);
                            Vector3 position = whoCopy.headMesh.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 2f, UnityEngine.Random.Range(-2f, 2f));
                            rde.Activate.RaiseAll(new object[] { position, (whoCopy.headMesh.transform.position - position).normalized * (ShootStrength / 2f) });
                            RPCProtection();
                        }
                    }
                    if (!didThing)
                    {
                        CosmeticsController.instance.ApplyCosmeticItemToSet(GorillaTagger.Instance.offlineVRRig.tryOnSet, CosmeticsController.instance.GetItemFromDict("LMANE."), true, false);
                        CosmeticsController.instance.UpdateWornCosmetics(true);
                        archiveelves = null;
                    }
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }*/

        public static void PaperPlaneSpam()
        {
            if (rightGrab)
            {
                PaperPlaneThrowable funnyplane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/TransferrableItemLeftArm/DropZoneAnchor/PaperAirplaneAnchor(Clone)/LMAHY.").GetComponent<PaperPlaneThrowable>();
                if (Time.time > projDebounce)
                {
                    projDebounce = Time.time + projDebounceType;
                    Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
                    Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    Vector3 velocity = GorillaTagger.Instance.rightHandTransform.forward * (ShootStrength * 2f);

                    ((PhotonEvent)Traverse.Create(funnyplane).Field("gLaunchRPC").GetValue()).RaiseAll(new object[]
                    {
                        GorillaTagger.Instance.myVRRig.ViewID,
                        position,
                        rotation,
                        velocity
                    });
                }
            }
        }

        public static void FireballSpam()
        {
            if (rightGrab)
            {
                PaperPlaneThrowable funnyplane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/TransferrableItemLeftArm/DropZoneAnchor/FireballAnchor(Clone)/LMAJM.").GetComponent<PaperPlaneThrowable>();
                if (Time.time > projDebounce)
                {
                    projDebounce = Time.time + projDebounceType;
                    Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
                    Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    Vector3 velocity = GorillaTagger.Instance.rightHandTransform.forward * (ShootStrength * 2f);

                    typeof(PaperPlaneThrowable).GetMethod("LaunchProjectile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(funnyplane, new object[] { position, rotation, velocity });
                    FieldInfo lol = typeof(PaperPlaneThrowable).GetField("gLaunchRPC", BindingFlags.NonPublic | BindingFlags.Static);
                    PhotonEvent lol2 = (PhotonEvent)lol.GetValue(funnyplane);
                    lol2.RaiseOthers(new object[]
                    {
                        Traverse.Create(typeof(PaperPlaneThrowable)).Method("FetchViewID").GetValue(funnyplane),
                        position,
                        rotation,
                        velocity
                    });
                }
            }
        }

        public static void Urine()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(255f, 255f, 0f));
            }
        }

        public static void Feces()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.3f, 0f);
                Vector3 charvel = Vector3.zero;

                BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(99f/255f, 43f/255f, 0f));
            }
        }

        public static void Semen()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(255f, 255f, 255f));
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(0f, 255f, 0f));
            }
        }

        public static void Spit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                BetaFireProjectile("SnowballLeft",  startpos, charvel, new Color(0f, 255f, 255f));
            }
        }

        public static void ServersidedTracers()
        {
            int projIndex = projmode;
            if (GetIndex("Random Projectile").enabled)
            {
                projIndex = UnityEngine.Random.Range(0, 4);
            }
            string projectilename = ExternalProjectileNames[projIndex];

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
                if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
                {
                    VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                    VRRig vrrig = GetRandomVRRig(false);// GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
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

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(255f, 255f, 0f, 1f));
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("SnowballLeft", startpos, charvel, new Color32(99, 43, 0, 255));
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(255f, 255f, 255f, 1f));
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.4f) + (whoCopy.headMesh.transform.up * -0.05f);
                    Vector3 charvel = whoCopy.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(0f, 255f, 0f, 1f));
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.4f) + (whoCopy.headMesh.transform.up * -0.05f);
                    Vector3 charvel = whoCopy.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("SnowballLeft", startpos, charvel, new Color(0f, 255f, 255f, 1f));
                }
                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        isCopying = true;
                        whoCopy = possibly;
                    }
                }
            }
            else
            {
                if (isCopying)
                {
                    isCopying = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }
    }
}
