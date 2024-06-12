using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static GorillaNetworking.CosmeticsController;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods.Spammers
{
    internal class Projectiles
    {
        public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, bool noDelay = false)
        {
            ControllerInputPoller.instance.leftControllerGripFloat = 1f;
            GameObject lhelp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(lhelp, 0.1f);
            lhelp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            lhelp.transform.position = GorillaTagger.Instance.leftHandTransform.position;
            lhelp.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            int[] overrides = new int[]
            {
                32,
                204,
                231,
                240,
                249,
                252
            };
            lhelp.AddComponent<GorillaSurfaceOverride>().overrideIndex = overrides[Array.IndexOf(fullProjectileNames, projectileName)];
            lhelp.GetComponent<Renderer>().enabled = false;
            if (Time.time > projDebounce)
            {
                try
                {
                    Vector3 startpos = position;
                    Vector3 charvel = velocity;

                    Vector3 oldVel = GorillaTagger.Instance.GetComponent<Rigidbody>().velocity;
                    
                    string[] name2 = new string[]
                    {
                        "LMACE.",
                        "LMAEX.",
                        "LMAGD.",
                        "LMAHQ.",
                        "LMAIE.",
                        "LMAIO."
                    };
                    SnowballThrowable fart = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/" + fullProjectileNames[System.Array.IndexOf(fullProjectileNames, projectileName)] + "LeftAnchor").transform.Find(name2[System.Array.IndexOf(fullProjectileNames, projectileName)]).GetComponent<SnowballThrowable>();
                    Vector3 oldPos = fart.transform.position;
                    fart.randomizeColor = true;
                    fart.transform.position = startpos;

                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = charvel;
                    GorillaTagger.Instance.offlineVRRig.SetThrowableProjectileColor(true, color);
                    GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/EquipmentInteractor").GetComponent<EquipmentInteractor>().ReleaseLeftHand();

                    RPCProtection();
                    GorillaTagger.Instance.GetComponent<Rigidbody>().velocity = oldVel;
                    fart.transform.position = oldPos;
                    fart.randomizeColor = false;
                } catch { }
                if (projDebounceType > 0f && !noDelay)
                {
                    projDebounce = Time.time + projDebounceType;
                }
            }
        }

        public static void SysFireProjectile(string projectilename, string trailname, Vector3 position, Vector3 velocity, float r, float g, float b, bool bluet, bool oranget, bool noDelay = false)
        {
            BetaFireProjectile(projectilename, position, velocity, new Color(r, g, b, 1f), noDelay);
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
                "Fish Food"
            };

            projmode++;
            if (projmode > (shortProjectileNames.Length - 1))
            {
                projmode = 0;
            }

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

            GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + (Mathf.Floor(red * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseGreen()
        {
            green += 0.1f;
            if (green > 1.05f)
            {
                green = 0f;
            }

            GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + (Mathf.Floor(green * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
        }

        public static void IncreaseBlue()
        {
            blue += 0.1f;
            if (blue > 1.05f)
            {
                blue = 0f;
            }

            GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + (Mathf.Floor(blue * 10f) / 10f).ToString() + "</color><color=grey>]</color>";
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
                    projIndex = UnityEngine.Random.Range(0, 4);
                }
                string projectilename = fullProjectileNames[projIndex];

                if (true)
                {
                    trailIndex = UnityEngine.Random.Range(0, 8);
                }
                string trailname = fullTrailNames[trailIndex];

                Vector3 startpos = GorillaTagger.Instance.rightHandTransform.position;
                Vector3 charvel = GorillaLocomotion.Player.Instance.currentVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.rightHandTransform.transform.forward * ShootStrength);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out var hit, 100);
                        charvel = hit.point - GorillaTagger.Instance.rightHandTransform.transform.position;
                        charvel.Normalize();
                        charvel *= (ShootStrength * 2);
                    }
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * ShootStrength);
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

                SysFireProjectile(projectilename, trailname, startpos, charvel, randa / 255f, randb / 255f, randc / 255f, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled);
                //UnityEngine.Debug.Log("fried proj");
            }

            if (leftGrab)
            {
                if (GetIndex("Random Projectile").enabled)
                {
                    projIndex = UnityEngine.Random.Range(0, 4);
                }
                string projectilename = fullProjectileNames[projIndex];

                if (true)
                {
                    trailIndex = UnityEngine.Random.Range(0, 8);
                }
                string trailname = fullTrailNames[trailIndex];

                /*GameObject projectile = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + projectilename + "(Clone)");
                GameObject originalprojectile = projectile;
                projectile = ObjectPools.instance.Instantiate(projectile);

                GameObject trail = GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools/" + trailname + "(Clone)");

                SlingshotProjectile GameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPoolsGameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPoolsGameObject.Find("Environment Objects/PersistentObjects_Prefab/GlobalObjectPools = projectile.GetComponent<SlingshotProjectile>();*/

                Vector3 startpos = GorillaTagger.Instance.leftHandTransform.position;
                Vector3 charvel = GorillaLocomotion.Player.Instance.currentVelocity;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.leftHandTransform.transform.forward * ShootStrength);
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    charvel = GorillaLocomotion.Player.Instance.currentVelocity + (GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up * ShootStrength);
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
                    startpos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
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
                    charvel = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
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

                SysFireProjectile(projectilename, trailname, startpos, charvel, randa / 255f, randb / 255f, randc / 255f, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled);
            }
        }

        public static void GiveProjectileSpamGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    int projIndex = projmode;
                    int trailIndex = trailmode;
                    
                    if (whoCopy.rightMiddle.calcT > 0.5f)
                    {
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 4);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        if (true)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        Vector3 startpos = whoCopy.rightHandTransform.position;
                        Vector3 charvel = Vector3.zero;

                        if (GetIndex("Shoot Projectiles").enabled)
                        {
                            charvel = (whoCopy.rightHandTransform.transform.forward * ShootStrength);
                        }

                        if (GetIndex("Finger Gun Projectiles").enabled)
                        {
                            charvel = (whoCopy.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * ShootStrength);
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

                        SysFireProjectile(projectilename, trailname, startpos, charvel, randa / 255f, randb / 255f, randc / 255f, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled);
                    }

                    if (whoCopy.leftMiddle.calcT > 0.5f)
                    {
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 4);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        if (true)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        Vector3 startpos = whoCopy.leftHandTransform.position;
                        Vector3 charvel = Vector3.zero;

                        if (GetIndex("Shoot Projectiles").enabled)
                        {
                            charvel = (whoCopy.leftHandTransform.transform.forward * ShootStrength);
                        }

                        if (GetIndex("Finger Gun Projectiles").enabled)
                        {
                            charvel = (whoCopy.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up * ShootStrength);
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

                        SysFireProjectile(projectilename, trailname, startpos, charvel, randa / 255f, randb / 255f, randc / 255f, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled);
                    }
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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

        /*
        public static void ProjectileBomb()
        {
            if (rightGrab)
            {
                if (ProjBombObject == null)
                {
                    ProjBombObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(ProjBombObject.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(ProjBombObject.GetComponent<SphereCollider>());
                    ProjBombObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                ProjBombObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (ProjBombObject != null)
            {
                if (rightPrimary)
                {
                    Vector3 spawnPos = ProjBombObject.transform.position;
                    UnityEngine.Object.Destroy(ProjBombObject);
                    ProjBombObject = null;

                    for (var i = 0; i < 5; i++)
                    {
                        int projIndex = projmode;
                        if (GetIndex("Random Projectile").enabled)
                        {
                            projIndex = UnityEngine.Random.Range(0, 4);
                        }
                        string projectilename = fullProjectileNames[projIndex];

                        int trailIndex = trailmode;
                        if (true /*true)
                        {
                            trailIndex = UnityEngine.Random.Range(0, 8);
                        }
                        string trailname = fullTrailNames[trailIndex];

                        Vector3 startpos = spawnPos;
                        Vector3 charvel = new Vector3(UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33), UnityEngine.Random.Range(-33, 33));

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

                        SysFireProjectile(projectilename, trailname, startpos, charvel, randa / 255f, randb / 255f, randc / 255f, GetIndex("Blue Team Projectiles").enabled, GetIndex("Orange Team Projectiles").enabled, true);
                    }
                    if (projDebounceType > 0f)
                    {
                        projDebounce = Time.time + projDebounceType;
                    }
                }
                else
                {
                    ProjBombObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                }
            }
        }
        */
        public static void SlingshotHelper()
        {
            GameObject slingy = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
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
                GameObject slingy = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/palm.01.L/TransferrableItemLeftHand/Slingshot Anchor/Slingshot");
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
                    if (projectile.projectileOwner == PhotonNetwork.LocalPlayer)
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
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                    if (Mouse.current.leftButton.isPressed)
                    {
                        Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out Ray, 100);
                    }
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up, out var Ray);
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
                    projDebounce = Time.time + projDebounceType;
                }
            }

            if (leftGrab && Time.time > projDebounce)
            {
                Vector3 startpos = GorillaTagger.Instance.leftHandTransform.position;

                if (GetIndex("Shoot Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out var Ray);
                    startpos = Ray.point;
                }

                if (GetIndex("Finger Gun Projectiles").enabled)
                {
                    Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").up, out var Ray);
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
            int[] overrides = new int[]
            {
                32,
                204,
                231,
                240,
                249,
                252
            };
            
            if (leftGrab && !lastLeftGrab)
            {
                ControllerInputPoller.instance.leftControllerGripFloat = 1f;
                GameObject lhelp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(lhelp, 0.1f);
                lhelp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                lhelp.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                lhelp.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                lhelp.AddComponent<GorillaSurfaceOverride>().overrideIndex = overrides[projIndex];
                lhelp.GetComponent<Renderer>().enabled = false;
            }
            lastLeftGrab = leftGrab;

            if (rightGrab && !lastRightGrab)
            {
                ControllerInputPoller.instance.leftControllerGripFloat = 1f;
                GameObject lhelp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(lhelp, 0.1f);
                lhelp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                lhelp.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                lhelp.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                lhelp.AddComponent<GorillaSurfaceOverride>().overrideIndex = overrides[projIndex];
                lhelp.GetComponent<Renderer>().enabled = false;
            }
            lastRightGrab = rightGrab;
        }

        public static void PaperPlaneSpam()
        {
            if (rightGrab)
            {
                PaperPlaneThrowable funnyplane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/TransferrableItemLeftArm/DropZoneAnchor/PaperAirplaneAnchor/LMAHY.").GetComponent<PaperPlaneThrowable>();
                if (Time.time > projDebounce)
                {
                    projDebounce = Time.time + projDebounceType;
                    Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
                    Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    Vector3 velocity = GorillaTagger.Instance.rightHandTransform.forward * (ShootStrength * 2f);

                    typeof(PaperPlaneThrowable).GetMethod("LaunchProjectile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(funnyplane, new object[] { position, rotation, velocity });
                    int viewid = UnityEngine.Random.Range(-2147483647, 2147483647);
                    FieldInfo lol = typeof(PaperPlaneThrowable).GetField("gLaunchRPC", BindingFlags.NonPublic | BindingFlags.Static);
                    PhotonEvent lol2 = (PhotonEvent)lol.GetValue(funnyplane);
                    lol2.RaiseOthers(new object[]
                    {
                        viewid,
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
                PaperPlaneThrowable funnyplane = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/TransferrableItemLeftArm/DropZoneAnchor/FireballAnchor/LMAJM.").GetComponent<PaperPlaneThrowable>();
                if (Time.time > projDebounce)
                {
                    projDebounce = Time.time + projDebounceType;
                    Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
                    Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    Vector3 velocity = GorillaTagger.Instance.rightHandTransform.forward * (ShootStrength * 2f);

                    typeof(PaperPlaneThrowable).GetMethod("LaunchProjectile", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(funnyplane, new object[] { position, rotation, velocity });
                    int viewid = UnityEngine.Random.Range(-2147483647,2147483647);
                    FieldInfo lol = typeof(PaperPlaneThrowable).GetField("gLaunchRPC", BindingFlags.NonPublic | BindingFlags.Static);
                    PhotonEvent lol2 = (PhotonEvent)lol.GetValue(funnyplane);
                    lol2.RaiseOthers(new object[]
                    {
                        viewid,
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

                SysFireProjectile("Snowball", "none", startpos, charvel, 255f, 255f, 0f, false, false);
            }
        }

        public static void Feces()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.3f, 0f);
                Vector3 charvel = Vector3.zero;

                SysFireProjectile("FishFood", "none", startpos, charvel, 99f/255f, 43f/255f, 0f, false, false);
            }
        }

        public static void Semen()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, -0.15f, 0f);
                Vector3 charvel = GorillaTagger.Instance.bodyCollider.transform.forward * 8.33f;

                SysFireProjectile("Snowball", "none", startpos, charvel, 255f, 255f, 255f, false, false);
            }
        }

        public static void Vomit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                SysFireProjectile("Snowball", "none", startpos, charvel, 0f, 255f, 0f, false, false);
            }
        }

        public static void Spit()
        {
            if (rightGrab || Mouse.current.leftButton.isPressed)
            {
                Vector3 startpos = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * 0.1f) + (GorillaTagger.Instance.headCollider.transform.up * -0.15f);
                Vector3 charvel = GorillaTagger.Instance.headCollider.transform.forward * 8.33f;

                SysFireProjectile("Snowball", "none", startpos, charvel, 0f, 255f, 255f, false, false);
            }
        }

        public static void ServersidedTracers()
        {
            int projIndex = projmode;
            if (GetIndex("Random Projectile").enabled)
            {
                projIndex = UnityEngine.Random.Range(0, 4);
            }
            string projectilename = fullProjectileNames[projIndex];

            int trailIndex = trailmode;
            if (true /*true*/)
            {
                trailIndex = UnityEngine.Random.Range(0, 8);
            }
            string trailname = fullTrailNames[trailIndex];

            bool isInfectedPlayers = false;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("fected"))
                {
                    isInfectedPlayers = true;
                    break;
                }
            }
            if (isInfectedPlayers)
            {
                if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected"))
                {
                    VRRig vrrig = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                    if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    BetaFireProjectile("Snowball", startpos, charvel, new Color(255f, 255f, 0f, 1f));
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.65f, 0f);
                    Vector3 charvel = Vector3.zero;

                    BetaFireProjectile("FishFood", startpos, charvel, new Color32(99, 43, 0, 255));
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.transform.position + new Vector3(0f, -0.4f, 0f) + (whoCopy.transform.forward * 0.2f);
                    Vector3 charvel = whoCopy.transform.forward * 8.33f;

                    BetaFireProjectile("Snowball", startpos, charvel, new Color(255f, 255f, 255f, 1f));
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.4f) + (whoCopy.headMesh.transform.up * -0.05f);
                    Vector3 charvel = whoCopy.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("Snowball", startpos, charvel, new Color(0f, 255f, 0f, 1f));
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    Vector3 startpos = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * 0.4f) + (whoCopy.headMesh.transform.up * -0.05f);
                    Vector3 charvel = whoCopy.headMesh.transform.forward * 8.33f;

                    BetaFireProjectile("Snowball", startpos, charvel, new Color(0f, 255f, 255f, 1f));
                }
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
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
