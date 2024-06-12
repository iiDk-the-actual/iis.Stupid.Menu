using BepInEx;
using ExitGames.Client.Photon;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static UnityEngine.Object;


namespace iiMenu.Mods
{
    internal class Movement
    {
        public static void DisableJoystick()
        {
            GorillaSnapTurn turning = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer").GetComponent<GorillaSnapTurn>();
            turnAmnt = turning.turnAmount;
            turning.turnAmount = 0f;
        }

        public static void EnableJoystick()
        {
            GorillaSnapTurn turning = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer").GetComponent<GorillaSnapTurn>();
            turning.turnAmount = turnAmnt;
        }

        public static void ChangePlatformType()
        {
            platformMode++;
            if (platformMode > 11)
            {
                platformMode = 0;
            }

            string[] platformNames = new string[] {
                "Normal",
                "Invisible",
                "Rainbow",
                "Random Color",
                "Noclip",
                "Glass",
                "Snowball",
                "Water Balloon",
                "Rock",
                "Present",
                "Mentos",
                "Fish Food"
            };

            GetIndex("Change Platform Type").overlapText = "Change Platform Type <color=grey>[</color><color=green>" + platformNames[platformMode] + "</color><color=grey>]</color>";
        }

        public static void ChangePlatformShape()
        {
            platformShape++;
            if (platformShape > 6)
            {
                platformShape = 0;
            }

            string[] platformShapes = new string[] {
                "Sphere",
                "Cube",
                "Cylinder",
                "Legacy",
                "Small",
                "Long",
                "1x1"
            };

            GetIndex("Change Platform Shape").overlapText = "Change Platform Shape <color=grey>[</color><color=green>" + platformShapes[platformShape] + "</color><color=grey>]</color>";
        }

        public static void Platforms()
        {
            if (leftGrab)
            {
                if (leftplat == null)
                {
                    if (platformShape == 0)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        leftplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 1)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        leftplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 2)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        leftplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 3)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        leftplat.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    }
                    if (platformShape == 4)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        leftplat.transform.localScale = new Vector3(0.025f, 0.15f, 0.2f);
                    }
                    if (platformShape == 5)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        leftplat.transform.localScale = new Vector3(0.025f, 0.3f, 0.8f);
                    }
                    if (platformShape == 6)
                    {
                        leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        leftplat.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                    leftplat.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    leftplat.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    if (platformMode != 5)
                    {
                        leftplat.GetComponent<Renderer>().material.color = GetBGColor(0f);
                    }
                    if (GetIndex("Stick Long Arms").enabled)
                    {
                        leftplat.transform.position = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * (armlength - 0.917f);
                    }
                    if (platformMode == 1)
                    {
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 2)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        leftplat.GetComponent<Renderer>().material.color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    if (platformMode == 3)
                    {
                        leftplat.GetComponent<Renderer>().material.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 128);
                    }
                    if (platformMode == 4)
                    {
                        foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                        {
                            v.enabled = false;
                        }
                    }
                    if (platformMode == 5)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
                        if (glass == null)
                        {
                            glass = new Material(Shader.Find("GUI/Text Shader"));
                            glass.color = new Color32(145, 187, 255, 100);
                        }
                        leftplat.GetComponent<Renderer>().material = glass;
                    }
                    if (platformMode == 6)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 32;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 7)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 204;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 8)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 231;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 9)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 240;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 10)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 249;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 11)
                    {
                        leftplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 252;
                        leftplat.GetComponent<Renderer>().enabled = false;
                    }
                }
                else
                {
                    if (platformMode != 5)
                    {
                        leftplat.GetComponent<Renderer>().material.color = GetBGColor(0f);
                    }
                    if (platformMode == 2)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        leftplat.GetComponent<Renderer>().material.color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    if (platformMode == 3)
                    {
                        leftplat.GetComponent<Renderer>().material.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 128);
                    }
                }
            }
            else
            {
                if (leftplat != null)
                {
                    Destroy(leftplat);
                    leftplat = null;
                    if (platformMode == 4 && rightplat == null)
                    {
                        foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                        {
                            v.enabled = true;
                        }
                    }
                }
            }

            if (rightGrab)
            {
                if (rightplat == null)
                {
                    if (platformShape == 0)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        rightplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 1)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        rightplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 2)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        rightplat.transform.localScale = new Vector3(0.333f, 0.333f, 0.333f);
                    }
                    if (platformShape == 3)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        rightplat.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    }
                    if (platformShape == 4)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        rightplat.transform.localScale = new Vector3(0.025f, 0.15f, 0.2f);
                    }
                    if (platformShape == 5)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        rightplat.transform.localScale = new Vector3(0.025f, 0.3f, 0.8f);
                    }
                    if (platformShape == 6)
                    {
                        rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        rightplat.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                    rightplat.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    rightplat.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    if (platformMode != 5)
                    {
                        rightplat.GetComponent<Renderer>().material.color = GetBGColor(0f);
                    }
                    if (GetIndex("Stick Long Arms").enabled)
                    {
                        rightplat.transform.position = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * (armlength - 0.917f);
                    }
                    if (platformMode == 1)
                    {
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 2)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        rightplat.GetComponent<Renderer>().material.color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    if (platformMode == 3)
                    {
                        rightplat.GetComponent<Renderer>().material.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 128);
                    }
                    if (platformMode == 4)
                    {
                        foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                        {
                            v.enabled = false;
                        }
                    }
                    if (platformMode == 5)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
                        if (glass == null)
                        {
                            glass = new Material(Shader.Find("GUI/Text Shader"));
                            glass.color = new Color32(145, 187, 255, 100);
                        }
                        rightplat.GetComponent<Renderer>().material = glass;
                    }
                    if (platformMode == 6)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 32;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 7)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 204;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 8)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 231;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 9)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 240;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 10)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 249;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                    if (platformMode == 11)
                    {
                        rightplat.AddComponent<GorillaSurfaceOverride>().overrideIndex = 252;
                        rightplat.GetComponent<Renderer>().enabled = false;
                    }
                }
                else
                {
                    if (platformMode != 5)
                    {
                        rightplat.GetComponent<Renderer>().material.color = GetBGColor(0f);
                    }
                    if (platformMode == 2)
                    {
                        float h = (Time.frameCount / 180f) % 1f;
                        rightplat.GetComponent<Renderer>().material.color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    }
                    if (platformMode == 3)
                    {
                        rightplat.GetComponent<Renderer>().material.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 128);
                    }
                }
            }
            else
            {
                if (rightplat != null)
                {
                    Destroy(rightplat);
                    rightplat = null;
                    if (platformMode == 4 && leftplat == null)
                    {
                        foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                        {
                            v.enabled = true;
                        }
                    }
                }
            }
        }

        public static void TriggerPlatforms()
        {
            bool lt = leftGrab;
            bool rt = rightGrab;
            leftGrab = leftTrigger > 0.5f;
            rightGrab = rightTrigger > 0.5f;
            Platforms();
            leftGrab = lt;
            rightGrab = rt;
        }

        public static void Frozone()
        {
            if (leftGrab)
            {
                GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lol.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                lol.transform.localPosition = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, -0.05f, 0f);
                lol.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                lol.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                UnityEngine.Object.Destroy(lol, 1);
            }

            if (rightGrab)
            {
                GameObject lol = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lol.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                lol.transform.localPosition = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -0.05f, 0f);
                lol.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                lol.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
                UnityEngine.Object.Destroy(lol, 1);
            }
        }

        public static void ChangeSpeedBoostAmount()
        {
            speedboostCycle++;
            if (speedboostCycle > 3)
            {
                speedboostCycle = 0;
            }

            float[] jspeedamounts = new float[] { 2f, 7.5f, 9f, 200f };
            jspeed = jspeedamounts[speedboostCycle];

            float[] jmultiamounts = new float[] { 0.5f, /*1.25f*/1.1f, 2f, 10f };
            jmulti = jmultiamounts[speedboostCycle];

            string[] speedNames = new string[] { "Slow", "Normal", "Fast", "Ultra Fast" };
            GetIndex("Change Speed Boost Amount").overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>" + speedNames[speedboostCycle] + "</color><color=grey>]</color>";
        }

        public static void PlatformSpam()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(platform.GetComponent<BoxCollider>());
                platform.GetComponent<Renderer>().material.color = bgColorA;
                platform.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                platform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                platform.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                platform.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                UnityEngine.Object.Destroy(platform, 1f);
                PhotonNetwork.RaiseEvent(69, new object[2] { platform.transform.position, platform.transform.rotation }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        public static void PlatformGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(platform.GetComponent<BoxCollider>());
                    platform.GetComponent<Renderer>().material.color = bgColorA;
                    platform.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    platform.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platform.transform.position = NewPointer.transform.position;
                    platform.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                    UnityEngine.Object.Destroy(platform, 1f);
                    PhotonNetwork.RaiseEvent(69, new object[2] { platform.transform.position, platform.transform.rotation }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
                }
            }
        }

        public static void ChangeFlySpeed()
        {
            flySpeedCycle++;
            if (flySpeedCycle > 3)
            {
                flySpeedCycle = 0;
            }

            float[] speedamounts = new float[] { 5f, 10f, 30f, 60f };
            flySpeed = speedamounts[flySpeedCycle];

            string[] speedNames = new string[] { "Slow", "Normal", "Fast", "Extra Fast" };
            GetIndex("Change Fly Speed").overlapText = "Change Fly Speed <color=grey>[</color><color=green>" + speedNames[flySpeedCycle] + "</color><color=grey>]</color>";
        }

        public static void ChangeArmLength()
        {
            longarmCycle++;
            if (longarmCycle > 4)
            {
                longarmCycle = 0;
            }

            float[] lengthAmounts = new float[] { 0.75f, 1.1f, 1.25f, 1.5f, 2f };
            armlength = lengthAmounts[longarmCycle];

            string[] lengthNames = new string[] { "Shorter", "Unnoticable", "Normal", "Long", "Extreme" };
            GetIndex("Change Arm Length").overlapText = "Change Arm Length <color=grey>[</color><color=green>" + lengthNames[longarmCycle] + "</color><color=grey>]</color>";
        }

        public static void Fly()
        {
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flySpeed;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void TriggerFly()
        {
            if (rightTrigger > 0.5f)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flySpeed;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void NoclipFly()
        {
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flySpeed;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                if (noclip == false)
                {
                    noclip = true;
                    foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    {
                        v.enabled = false;
                    }
                }
            } else
            {
                if (noclip == true)
                {
                    noclip = false;
                    foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    {
                        v.enabled = true;
                    }
                }
            }
        }

        public static void JoystickFly()
        {
            Vector2 joy = ControllerInputPoller.instance.rightControllerPrimary2DAxis;

            if (Mathf.Abs(joy.x) > 0.3 || Mathf.Abs(joy.y) > 0.3)
            {
                GorillaLocomotion.Player.Instance.transform.position += (GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * (joy.y * flySpeed)) + (GorillaTagger.Instance.headCollider.transform.right * Time.deltaTime * (joy.x * flySpeed));
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void BarkFly()
        {
            ZeroGravity();

            var rb = GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody;
            Vector2 xz = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
            float y = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis.y;

            Vector3 inputDirection = new Vector3(xz.x, y, xz.y);
            var playerForward = GorillaLocomotion.Player.Instance.bodyCollider.transform.forward;
            playerForward.y = 0;
            var playerRight = GorillaLocomotion.Player.Instance.bodyCollider.transform.right;
            playerRight.y = 0;

            var velocity = inputDirection.x * playerRight + y * Vector3.up + inputDirection.z * playerForward;
            velocity *= GorillaLocomotion.Player.Instance.scale * flySpeed;
            rb.velocity = Vector3.Lerp(rb.velocity, velocity, 0.12875f);
        }

        public static void HandFly()
        {
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").up * Time.deltaTime * flySpeed;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void SlingshotFly()
        {
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * (flySpeed * 2);
            }
        }

        public static void ZeroGravitySlingshotFly()
        {
            if (rightPrimary)
            {
                ZeroGravity();
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * flySpeed;
            }
        }

        public static void WASDFly()
        {
            GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0.067f, 0f);

            bool W = UnityInput.Current.GetKey(KeyCode.W);
            bool A = UnityInput.Current.GetKey(KeyCode.A);
            bool S = UnityInput.Current.GetKey(KeyCode.S);
            bool D = UnityInput.Current.GetKey(KeyCode.D);
            bool Space = UnityInput.Current.GetKey(KeyCode.Space);
            bool Ctrl = UnityInput.Current.GetKey(KeyCode.LeftControl);

            if (Mouse.current.rightButton.isPressed)
            {
                Vector3 Euler = GorillaLocomotion.Player.Instance.rightControllerTransform.parent.rotation.eulerAngles;
                if (startX < 0)
                {
                    startX = Euler.y;
                    subThingy = Mouse.current.position.value.x / UnityEngine.Screen.width;
                }
                Euler = new Vector3(Euler.x, startX + ((((Mouse.current.position.value.x / UnityEngine.Screen.width) - subThingy) * 360) * 1.33f), Euler.z);
                GorillaLocomotion.Player.Instance.rightControllerTransform.parent.rotation = Quaternion.Euler(Euler);
            }
            else
            {
                startX = -1;
            }

            if (W)
            {
                GorillaTagger.Instance.rigidbody.transform.position += GorillaLocomotion.Player.Instance.rightControllerTransform.parent.forward * Time.deltaTime * flySpeed;
            }

            if (S)
            {
                GorillaTagger.Instance.rigidbody.transform.position += GorillaLocomotion.Player.Instance.rightControllerTransform.parent.forward * Time.deltaTime * -flySpeed;
            }

            if (A)
            {
                GorillaTagger.Instance.rigidbody.transform.position += GorillaLocomotion.Player.Instance.rightControllerTransform.parent.right * Time.deltaTime * -flySpeed;
            }

            if (D)
            {
                GorillaTagger.Instance.rigidbody.transform.position += GorillaLocomotion.Player.Instance.rightControllerTransform.parent.right * Time.deltaTime * flySpeed;
            }

            if (Space)
            {
                GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * flySpeed, 0f);
            }

            if (Ctrl)
            {
                GorillaTagger.Instance.rigidbody.transform.position += new Vector3(0f, Time.deltaTime * -flySpeed, 0f);
            }
        }

        public static void Drive()
        {
            Vector2 joy = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
            lerpygerpy = Vector2.Lerp(lerpygerpy, joy, 0.05f);

            Vector3 addition = GorillaTagger.Instance.bodyCollider.transform.forward * lerpygerpy.y + GorillaTagger.Instance.bodyCollider.transform.right * lerpygerpy.x;// + new Vector3(0f, -1f, 0f);
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f);

            if (Ray.distance < 0.2f && (Mathf.Abs(lerpygerpy.x) > 0.05f || Mathf.Abs(lerpygerpy.y) > 0.05f))
            {
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity = addition * 10f;
            }
        }

        public static void IronMan()
        {
            if (leftPrimary)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(flySpeed * GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").right, ForceMode.Acceleration);
                GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 50f * GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity.magnitude, GorillaTagger.Instance.tapHapticDuration);
            }
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(flySpeed * -GorillaTagger.Instance.offlineVRRig.transform.Find("rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").right, ForceMode.Acceleration);
                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 50f * GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity.magnitude, GorillaTagger.Instance.tapHapticDuration);
            }
        }

        public static void SpiderMan()
        {
            if (leftGrab)
            {
                if (!isLeftGrappling)
                {
                    isLeftGrappling = true;
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.leftHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            true,
                            999999f
                        });
                    } else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                    }
                    RPCProtection();
                    RaycastHit lefthit;
                    if (Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out lefthit, 512f))
                    {
                        leftgrapplePoint = lefthit.point;

                        leftjoint = GorillaTagger.Instance.gameObject.AddComponent<SpringJoint>();
                        leftjoint.autoConfigureConnectedAnchor = false;
                        leftjoint.connectedAnchor = leftgrapplePoint;

                        float leftdistanceFromPoint = Vector3.Distance(GorillaTagger.Instance.bodyCollider.attachedRigidbody.position, leftgrapplePoint);

                        leftjoint.maxDistance = leftdistanceFromPoint * 0.8f;
                        leftjoint.minDistance = leftdistanceFromPoint * 0.25f;

                        leftjoint.spring = 10f;
                        leftjoint.damper = 50f;
                        leftjoint.massScale = 12f;
                    }
                }

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                UnityEngine.Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, leftgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out var Ray, 512f);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = buttonDefaultA - new Color32(0, 0, 0, 128);
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f) - new Color32(0, 0, 0, 128);
                liner.endColor = GetBGColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                isLeftGrappling = false;
                UnityEngine.Object.Destroy(leftjoint);
            }

            if (rightGrab)
            {
                if (!isRightGrappling)
                {
                    isRightGrappling = true;
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.rightHandTransform.forward * 5f;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            false,
                            999999f
                        });
                        RPCProtection();
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                    }
                    RaycastHit righthit;
                    if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out righthit, 512f))
                    {
                        rightgrapplePoint = righthit.point;

                        rightjoint = GorillaTagger.Instance.gameObject.AddComponent<SpringJoint>();
                        rightjoint.autoConfigureConnectedAnchor = false;
                        rightjoint.connectedAnchor = rightgrapplePoint;

                        float rightdistanceFromPoint = Vector3.Distance(GorillaTagger.Instance.bodyCollider.attachedRigidbody.position, rightgrapplePoint);

                        rightjoint.maxDistance = rightdistanceFromPoint * 0.8f;
                        rightjoint.minDistance = rightdistanceFromPoint * 0.25f;

                        rightjoint.spring = 10f;
                        rightjoint.damper = 50f;
                        rightjoint.massScale = 12f;
                    }
                }

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                UnityEngine.Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, rightgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray, 512f);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = buttonDefaultA - new Color32(0, 0, 0, 128);
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f) - new Color32(0, 0, 0, 128);
                liner.endColor = GetBGColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                isRightGrappling = false;
                UnityEngine.Object.Destroy(rightjoint);
            }
        }

        public static void GrapplingHooks()
        {
            if (leftGrab)
            {
                if (!isLeftGrappling)
                {
                    isLeftGrappling = true;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            true,
                            999999f
                        });
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, true, 999999f);
                    }
                    RPCProtection();
                    RaycastHit lefthit;
                    if (Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out lefthit, 512f))
                    {
                        leftgrapplePoint = lefthit.point;
                    }
                }

                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(leftgrapplePoint - GorillaTagger.Instance.leftHandTransform.position) * 0.5f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                UnityEngine.Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, leftgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.forward, out var Ray, 512f);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = buttonDefaultA - new Color32(0, 0, 0, 128);
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f) - new Color32(0, 0, 0, 128);
                liner.endColor = GetBGColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
                liner.SetPosition(1, Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                isLeftGrappling = false;
                UnityEngine.Object.Destroy(leftjoint);
            }

            if (rightGrab)
            {
                if (!isRightGrappling)
                {
                    isRightGrappling = true;
                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                            89,
                            false,
                            999999f
                        });
                        RPCProtection();
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(89, false, 999999f);
                    }
                    RaycastHit righthit;
                    if (Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out righthit, 512f))
                    {
                        rightgrapplePoint = righthit.point;
                    }
                }

                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(rightgrapplePoint - GorillaTagger.Instance.rightHandTransform.position) * 0.5f;

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                UnityEngine.Color thecolor = Color.red;
                liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, rightgrapplePoint);
                liner.material.shader = Shader.Find("GorillaTag/UberShader");
                UnityEngine.Object.Destroy(line, Time.deltaTime);
            }
            else
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray, 512f);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = buttonDefaultA - new Color32(0, 0, 0, 128);
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = GetBGColor(0f) - new Color32(0, 0, 0, 128);
                liner.endColor = GetBGColor(0.5f) - new Color32(0, 0, 0, 128);
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                isRightGrappling = false;
                UnityEngine.Object.Destroy(rightjoint);
            }
        }

        public static void DisableSpiderMan()
        {
            isLeftGrappling = false;
            UnityEngine.Object.Destroy(leftjoint);
            isRightGrappling = false;
            UnityEngine.Object.Destroy(rightjoint);
        }

        public static void UpAndDown()
        {
            if ((rightTrigger > 0.5f) || rightGrab)
            {
                ZeroGravity();
            }
            if (rightTrigger > 0.5f)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.up * Time.deltaTime * flySpeed * 3f;
            }

            if (rightGrab)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.up * Time.deltaTime * flySpeed * -3f;
            }
        }

        public static void LeftAndRight()
        {
            if ((rightTrigger > 0.5f) || rightGrab)
            {
                ZeroGravity();
            }
            if (rightTrigger > 0.5f)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.bodyCollider.transform.right * Time.deltaTime * flySpeed * -3f;
            }

            if (rightGrab)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.bodyCollider.transform.right * Time.deltaTime * flySpeed * 3f;
            }
        }

        public static void ForwardsAndBackwards()
        {
            if ((rightTrigger > 0.5f) || rightGrab)
            {
                ZeroGravity();
            }
            if (rightTrigger > 0.5f)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.bodyCollider.transform.forward * Time.deltaTime * flySpeed * 3f;
            }

            if (rightGrab)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += GorillaTagger.Instance.bodyCollider.transform.forward * Time.deltaTime * flySpeed * -3f;
            }
        }

        public static void AutoWalk()
        {
            Vector2 joy = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;

            float armLength = 0.45f;
            float animSpeed = 9f;

            if (SteamVR_Actions.gorillaTag_LeftJoystickClick.state)
            {
                animSpeed *= 1.5f;
            }
            
            if (Mathf.Abs(joy.y) > 0.05f || Mathf.Abs(joy.x) > 0.05f)
            {
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * (Mathf.Sin(Time.time * animSpeed) * (joy.y * armLength)) + GorillaTagger.Instance.bodyCollider.transform.right * ((Mathf.Sin(Time.time * animSpeed) * (joy.x * armLength)) - 0.2f) + new Vector3(0f, -0.3f + (Mathf.Cos(Time.time * animSpeed) * 0.2f), 0f);
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * (-Mathf.Sin(Time.time * animSpeed) * (joy.y * armLength)) + GorillaTagger.Instance.bodyCollider.transform.right * ((-Mathf.Sin(Time.time * animSpeed) * (joy.x * armLength)) + 0.2f) + new Vector3(0f, -0.3f + (Mathf.Cos(Time.time * animSpeed) * -0.2f), 0f);
            }
            /*if (rightPrimary) this shit LACED
            {
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * -0.2f + new Vector3(0f, -1f, 0f) + -GorillaTagger.Instance.bodyCollider.transform.forward;
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.right * 0.2f + new Vector3(0f, -1f, 0f) + -GorillaTagger.Instance.bodyCollider.transform.forward;
            }*/
        }

        public static void AutoFunnyRun()
        {
            if (rightGrab)
            {
                if (bothHands)
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time) / 10) + new Vector3(0, -0.5f - (MathF.Sin(time) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * -0.05f);
                    GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time + 180) / 10) + new Vector3(0, -0.5f - (MathF.Sin(time + 180) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * 0.05f);
                }
                else
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * MathF.Cos(time) / 10) + new Vector3(0, -0.5f - (MathF.Sin(time) / 7), 0);
                }
            }
        }

        public static void AutoPinchClimb()
        {
            if (rightGrab)
            {
                float time = Time.frameCount / 3f;
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.right * (0.4f+(MathF.Cos(time) * 0.4f))) + (GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f)) + (GorillaTagger.Instance.headCollider.transform.forward * 0.75f);
                GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.right * -(0.4f+(MathF.Cos(time) * 0.4f))) + (GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f)) + (GorillaTagger.Instance.headCollider.transform.forward * 0.75f);
            }
        }

        public static void AutoElevatorClimb()
        {
            if (rightGrab)
            {
                float time = Time.frameCount / 3f;
                GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.right * (0.4f + (MathF.Cos(time) * 0.4f))) + (GorillaTagger.Instance.headCollider.transform.up * (MathF.Sin(time) * 0.6f)) + (GorillaTagger.Instance.headCollider.transform.forward * 0.75f);
            }
        }

        public static void ForceTagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = true;
        }

        public static void NoTagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = false;
        }

        public static void LowGravity()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (6.66f / Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void ZeroGravity()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (9.81f/Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void HighGravity()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.down * (Time.deltaTime * (7.77f / Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void ReverseGravity()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (19.62f / Time.deltaTime)), ForceMode.Acceleration);
            GorillaLocomotion.Player.Instance.rightControllerTransform.parent.rotation = Quaternion.Euler(180f, 0f, 0f);
        }

        public static void UnflipCharacter()
        {
            GorillaLocomotion.Player.Instance.rightControllerTransform.parent.rotation = Quaternion.identity;
        }

        public static void WallWalk()
        {
            if ((GorillaLocomotion.Player.Instance.wasLeftHandTouching || GorillaLocomotion.Player.Instance.wasRightHandTouching) && rightGrab)
            {
                FieldInfo fieldInfo = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.Player.Instance);
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            if (!rightGrab)
            {
                walkPos = Vector3.zero;
            }

            if (walkPos != Vector3.zero)
            {
                //GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(walkNormal * -10, ForceMode.Acceleration);
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(walkNormal * -9.81f, ForceMode.Acceleration);
                ZeroGravity();
            }
        }

        public static void WeakWallWalk()
        {
            if ((GorillaLocomotion.Player.Instance.wasLeftHandTouching || GorillaLocomotion.Player.Instance.wasRightHandTouching) && rightGrab)
            {
                FieldInfo fieldInfo = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.Player.Instance);
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            if (!rightGrab)
            {
                walkPos = Vector3.zero;
            }

            if (walkPos != Vector3.zero)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(walkNormal * -5, ForceMode.Acceleration);
                ZeroGravity();
            }
        }

        public static void StrongWallWalk()
        {
            if ((GorillaLocomotion.Player.Instance.wasLeftHandTouching || GorillaLocomotion.Player.Instance.wasRightHandTouching) && rightGrab)
            {
                FieldInfo fieldInfo = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.Player.Instance);
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            if (!rightGrab)
            {
                walkPos = Vector3.zero;
            }

            if (walkPos != Vector3.zero)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(walkNormal * -50, ForceMode.Acceleration);
                ZeroGravity();
            }
        }

        public static void SpiderWalk()
        {
            if (GorillaLocomotion.Player.Instance.wasLeftHandTouching || GorillaLocomotion.Player.Instance.wasRightHandTouching)
            {
                FieldInfo fieldInfo = typeof(GorillaLocomotion.Player).GetField("lastHitInfoHand", BindingFlags.NonPublic | BindingFlags.Instance);
                RaycastHit ray = (RaycastHit)fieldInfo.GetValue(GorillaLocomotion.Player.Instance);
                walkPos = ray.point;
                walkNormal = ray.normal;
            }

            if (walkPos != Vector3.zero)
            {
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.AddForce(walkNormal * -9.81f, ForceMode.Acceleration);
                GorillaLocomotion.Player.Instance.rightControllerTransform.parent.rotation = Quaternion.Lerp(GorillaLocomotion.Player.Instance.transform.rotation, Quaternion.LookRotation(walkNormal) * Quaternion.Euler(90f, 0f, 0f), Time.deltaTime);
                ZeroGravity();
            }
        }

        public static void TeleportToRandom()
        {
            TeleportPlayer(GetRandomVRRig(false).transform.position);
        }

        public static void TeleportGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > teleDebounce)
                {
                    /*MeshCollider[] meshColliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
                    foreach (MeshCollider coll in meshColliders)
                    {
                        coll.enabled = false;
                    }
                    GorillaTagger.Instance.rigidbody.transform.position = NewPointer.transform.position + new Vector3(0f, 1f, 0f);
                    frameFixColliders = true;*/
                    TeleportPlayer(NewPointer.transform.position + new Vector3(0f, 1f, 0f));
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    teleDebounce = Time.time + 0.5f;
                }
            }
        }

        public static void Airstrike()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && Time.time > teleDebounce)
                {
                    TeleportPlayer(NewPointer.transform.position + new Vector3(0f, 30f, 0f));
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = new Vector3(0f, -20f, 0f);
                    teleDebounce = Time.time + 0.5f;
                }
            }
        }

        public static void Checkpoint()
        {
            if (rightGrab)
            {
                if (CheckPoint == null)
                {
                    CheckPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(CheckPoint.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(CheckPoint.GetComponent<SphereCollider>());
                    CheckPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                CheckPoint.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (CheckPoint != null)
            {
                if (rightPrimary)
                {
                    CheckPoint.GetComponent<Renderer>().material.color = bgColorA;
                    TeleportPlayer(CheckPoint.transform.position);
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                else
                {
                    CheckPoint.GetComponent<Renderer>().material.color = buttonDefaultA;
                }
            }
        }

        public static void DisableCheckpoint()
        {
            if (CheckPoint != null)
            {
                UnityEngine.Object.Destroy(CheckPoint);
                CheckPoint = null;
            }
        }

        public static void Bomb()
        {
            if (rightGrab)
            {
                if (BombObject == null)
                {
                    BombObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(BombObject.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(BombObject.GetComponent<SphereCollider>());
                    BombObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                BombObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            }
            if (BombObject != null)
            {
                if (rightPrimary)
                {
                    Vector3 dir = (GorillaTagger.Instance.bodyCollider.transform.position - BombObject.transform.position);
                    dir.Normalize();
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += 25f * dir;
                    UnityEngine.Object.Destroy(BombObject);
                    BombObject = null;
                }
                else
                {
                    BombObject.GetComponent<Renderer>().material.color = buttonDefaultA;
                }
            }
        }

        public static void DisableBomb()
        {
            if (BombObject != null)
            {
                UnityEngine.Object.Destroy(BombObject);
                BombObject = null;
            }
        }

        public static void SpeedBoost()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = jspeed;
            GorillaLocomotion.Player.Instance.jumpMultiplier = jmulti;
        }

        public static void GripSpeedBoost()
        {
            if (rightGrab)
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = jspeed;
                GorillaLocomotion.Player.Instance.jumpMultiplier = jmulti;
            }
        }

        public static void JoystickSpeedBoost()
        {
            if (SteamVR_Actions.gorillaTag_RightJoystickClick.state)
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = jspeed;
                GorillaLocomotion.Player.Instance.jumpMultiplier = jmulti;
            }
        }

        public static void DisableSpeedBoost()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
            GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
        }

        public static void UncapMaxVelocity()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 99999f;
        }

        public static void AlwaysMaxVelocity()
        {
            GorillaLocomotion.Player.Instance.jumpMultiplier = 99999f;
        }

        public static void Noclip()
        {
            if (rightTrigger > 0.5f || UnityInput.Current.GetKey(KeyCode.E))
            {
                if (noclip == false)
                {
                    noclip = true;
                    foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    {
                        v.enabled = false;
                    }
                }
            }
            else
            {
                if (noclip == true)
                {
                    noclip = false;
                    foreach (MeshCollider v in Resources.FindObjectsOfTypeAll<MeshCollider>())
                    {
                        v.enabled = true;
                    }
                }
            }
        }

        public static void Invisible()
        {
            bool hit = rightSecondary || Mouse.current.rightButton.isPressed;
            if (invisMonke)
            {
                ghostException = true;
                GorillaTagger.Instance.offlineVRRig.headBodyOffset = new Vector3(99999f, 99999f, 99999f);
            }
            else
            {
                ghostException = false;
                GorillaTagger.Instance.offlineVRRig.headBodyOffset = Vector3.zero;
            }
            if (hit == true && lastHit2 == false)
            {
                invisMonke = !invisMonke;
            }
            lastHit2 = hit;
        }

        public static void DisableInvisible()
        {
            GorillaTagger.Instance.offlineVRRig.headBodyOffset = Vector3.zero;
            ghostException = false;
        }

        public static void Ghost()
        {
            bool hit = rightPrimary || Mouse.current.leftButton.isPressed;
            GorillaTagger.Instance.offlineVRRig.enabled = !ghostMonke;
            if (hit == true && lastHit == false)
            {
                ghostMonke = !ghostMonke;
            }
            lastHit = hit;
        }

        public static void EnableRig()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
            ghostException = false;
        }

        public static void RigGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = NewPointer.transform.position + new Vector3(0, 1, 0);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = NewPointer.transform.position + new Vector3(0, 1, 0);
                    } catch { }
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void GrabRig()
        {
            if (rightGrab)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(new Vector3(0f, GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles.y, 0f));
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    GorillaTagger.Instance.myVRRig.transform.rotation = Quaternion.Euler(new Vector3(0f, GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles.y, 0f));
                } catch { }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void EnableSpazRig()
        {
            ghostException = true;
            offsetLH = GorillaTagger.Instance.offlineVRRig.leftHand.trackingPositionOffset;
            offsetRH = GorillaTagger.Instance.offlineVRRig.rightHand.trackingPositionOffset;
            offsetH = GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset;
        }

        public static void SpazRig()
        {
            if (rightPrimary)
            {
                float spazAmount = 0.1f;
                ghostException = true;
                GorillaTagger.Instance.offlineVRRig.leftHand.trackingPositionOffset = offsetLH + new Vector3(UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount));
                GorillaTagger.Instance.offlineVRRig.rightHand.trackingPositionOffset = offsetRH + new Vector3(UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount));
                GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset = offsetH + new Vector3(UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount), UnityEngine.Random.Range(-spazAmount, spazAmount));
            }
            else
            {
                ghostException = false;
                GorillaTagger.Instance.offlineVRRig.leftHand.trackingPositionOffset = offsetLH;
                GorillaTagger.Instance.offlineVRRig.rightHand.trackingPositionOffset = offsetRH;
                GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset = offsetH;
            }
        }

        public static void DisableSpazRig()
        {
            ghostException = false;
            GorillaTagger.Instance.offlineVRRig.leftHand.trackingPositionOffset = offsetLH;
            GorillaTagger.Instance.offlineVRRig.rightHand.trackingPositionOffset = offsetRH;
            GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset = offsetH;
        }

        public static void SpazHands()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                } catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                try {
                    GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                } catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

                float spazAmount = 360f;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount)));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount)));

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.forward * 3f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.forward * 3f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void SpazRealHands()
        {
            if (rightPrimary)
            {
                float spazAmount = 360f;
                GorillaLocomotion.Player.Instance.leftControllerTransform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount)));
                GorillaLocomotion.Player.Instance.leftControllerTransform.position = GorillaTagger.Instance.leftHandTransform.position + GorillaLocomotion.Player.Instance.leftControllerTransform.forward * 3f;

                GorillaLocomotion.Player.Instance.rightControllerTransform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount), UnityEngine.Random.Range(0, spazAmount)));
                GorillaLocomotion.Player.Instance.rightControllerTransform.position = GorillaTagger.Instance.rightHandTransform.position + GorillaLocomotion.Player.Instance.rightControllerTransform.forward * 3f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void FreezeRigLimbs()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                } catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                } catch { }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
        
        public static void FreezeRigBody()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.rightHandTransform.position;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void FakeOculusMenu()
        {
            if (leftPrimary)
            {
                Safety.NoFinger();
                GorillaTagger.Instance.rightHandTransform.transform.rotation = Quaternion.identity;
                GorillaTagger.Instance.leftHandTransform.transform.rotation = Quaternion.identity;
            }
            GorillaLocomotion.Player.Instance.inOverlay = leftPrimary;
        }

        public static void FakeReportMenu()
        {
            if (leftPrimary)
            {
                Safety.NoFinger();
            }
            System.Type type = GorillaLocomotion.Player.Instance.GetType();
            FieldInfo fieldInfo = type.GetField("leftHandHolding", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(GorillaLocomotion.Player.Instance, leftPrimary);
            type = GorillaLocomotion.Player.Instance.GetType();
            fieldInfo = type.GetField("rightHandHolding", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(GorillaLocomotion.Player.Instance, leftPrimary);
            GorillaLocomotion.Player.Instance.InReportMenu = leftPrimary;
        }

        public static Vector3 deadPosition = Vector3.zero;
        public static Vector3 lvel = Vector3.zero;
        public static void FakePowerOff()
        {
            if (SteamVR_Actions.gorillaTag_LeftJoystickClick.state)
            {
                if (deadPosition == Vector3.zero)
                {
                    deadPosition = GorillaTagger.Instance.rigidbody.transform.position;
                    lvel = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity;
                }
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.rigidbody.transform.position = deadPosition;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = lvel;
            } else
            {
                deadPosition = Vector3.zero;
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void AutoDance()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                Vector3 bodyOffset = (GorillaTagger.Instance.bodyCollider.transform.right * (Mathf.Cos((float)Time.frameCount / 20f) * 0.3f)) + (new Vector3(0f, Mathf.Abs(Mathf.Sin((float)Time.frameCount / 20f) * 0.2f), 0f));
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f) + bodyOffset;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f) + bodyOffset;
                } catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                } catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.forward * 0.2f + GorillaTagger.Instance.offlineVRRig.transform.right * -0.4f + GorillaTagger.Instance.offlineVRRig.transform.up * (0.3f + (Mathf.Sin((float)Time.frameCount / 20f) * 0.2f));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.forward * 0.2f + GorillaTagger.Instance.offlineVRRig.transform.right * 0.4f + GorillaTagger.Instance.offlineVRRig.transform.up * (0.3f + (Mathf.Sin((float)Time.frameCount / 20f) * -0.2f));

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void AutoGriddy()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                Vector3 bodyOffset = GorillaTagger.Instance.offlineVRRig.transform.forward * (5f * Time.deltaTime);
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + bodyOffset;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + bodyOffset;
                } catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -0.33f) + (GorillaTagger.Instance.offlineVRRig.transform.forward * (0.5f * Mathf.Cos((float)Time.frameCount / 10f))) + (GorillaTagger.Instance.offlineVRRig.transform.up * (-0.5f * Mathf.Abs(Mathf.Sin((float)Time.frameCount / 10f))));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 0.33f) + (GorillaTagger.Instance.offlineVRRig.transform.forward * (0.5f * Mathf.Cos((float)Time.frameCount / 10f))) + (GorillaTagger.Instance.offlineVRRig.transform.up * (-0.5f * Mathf.Abs(Mathf.Sin((float)Time.frameCount / 10f))));

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void AutoTPose()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                }
                catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                }
                catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Helicopter()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position += new Vector3(0f, 0.05f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position += new Vector3(0f, 0.05f, 0f);
                } catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
                } catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Beyblade()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                }
                catch { }

                GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.rotation = Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new Vector3(0f, 10f, 0f));
                }
                catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Fan()
        {
            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
                    GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
                }
                catch { }

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + GorillaTagger.Instance.offlineVRRig.transform.right * (Mathf.Sin(Time.time * 15f) * 2f));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position - (GorillaTagger.Instance.offlineVRRig.transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + GorillaTagger.Instance.offlineVRRig.transform.right * (Mathf.Sin(Time.time * 15f) * 2f));

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void GhostAnimations()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;

            GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            try
            {
                GorillaTagger.Instance.myVRRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f);
            } catch { }

            GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            try
            {
                GorillaTagger.Instance.myVRRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            } catch { }

            GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

            if (rightPrimary)
            {
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaTagger.Instance.bodyCollider.transform.right * -0.25f) + (GorillaTagger.Instance.bodyCollider.transform.up * -1f) + (GorillaTagger.Instance.bodyCollider.transform.forward * Mathf.Sin((float)Time.frameCount / 10f));
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaTagger.Instance.bodyCollider.transform.right * 0.25f) + (GorillaTagger.Instance.bodyCollider.transform.up * -1f) + -(GorillaTagger.Instance.bodyCollider.transform.forward * Mathf.Sin((float)Time.frameCount / 10f));
            } else
            {
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaTagger.Instance.bodyCollider.transform.right * -0.25f) + (GorillaTagger.Instance.bodyCollider.transform.up * -1f);
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaTagger.Instance.bodyCollider.transform.right * 0.25f) + (GorillaTagger.Instance.bodyCollider.transform.up * -1f);
            }

            if (rightSecondary)
            {
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + (GorillaTagger.Instance.bodyCollider.transform.right * 0.25f) + Vector3.Lerp(GorillaTagger.Instance.rightHandTransform.forward, - GorillaTagger.Instance.rightHandTransform.up, 0.5f) * 2f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
        }

        public static void StareAtNearby()
        {
            GorillaTagger.Instance.offlineVRRig.headConstraint.LookAt(GetClosestVRRig().headMesh.transform.position);
        }

        public static void StareAtGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.headConstraint.LookAt(whoCopy.headMesh.transform.position);
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
                }
            }
        }

        public static void EnableFloatingRig()
        {
            offsetH = GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset;
        }

        public static void FloatingRig()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset = offsetH + new Vector3(0f, 0.65f + (Mathf.Sin((float)Time.frameCount / 40f) * 0.2f), 0f);
        }

        public static void DisableFloatingRig()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingPositionOffset = offsetH;
        }

        public static void Bees()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            if (Time.time > beesDelay)
            {
                VRRig target = GetRandomVRRig(false);//GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];

                GorillaTagger.Instance.offlineVRRig.transform.position = target.transform.position + new Vector3(0f, 1f, 0f);
                try
                {
                    GorillaTagger.Instance.myVRRig.transform.position = target.transform.position + new Vector3(0f, 1f, 0f);
                } catch { }

                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = target.transform.position;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = target.transform.position;

                beesDelay = Time.time + 0.777f;
            }
        }

        public static void SizeChanger()
        {
            if (rightPrimary)
            {
                sizeScale = 1f;
            }
            if (rightTrigger > 0.5f)
            {
                sizeScale += 0.05f;
            }
            if (rightGrab)
            {
                sizeScale -= 0.05f;
            }
            if (sizeScale <= 0)
            {
                sizeScale = 0.05f;
            }
            GorillaLocomotion.Player.Instance.scale = sizeScale;
        }

        public static void EnableSizeChanger()
        {
            sizeScale = 1f;
            GorillaLocomotion.Player.Instance.scale = 1f;
        }

        public static void EnableSlipperyHands()
        {
            EverythingSlippery = true;
        }

        public static void DisableSlipperyHands()
        {
            EverythingSlippery = false;
        }

        public static void EnableGrippyHands()
        {
            EverythingGrippy = true;
        }

        public static void DisableGrippyHands()
        {
            EverythingGrippy = false;
        }

        public static void StickyHands()
        {
            if (stickpart == null)
            {
                stickpart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stickpart.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                stickpart.GetComponent<Renderer>().enabled = false;
            }
            if (Time.time > partDelay)
            {
                if (GorillaLocomotion.Player.Instance.wasLeftHandTouching)
                {
                    stickpart.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    //partDelay = Time.time + 0.1f;
                }
                if (GorillaLocomotion.Player.Instance.wasRightHandTouching)
                {
                    stickpart.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    //partDelay = Time.time + 0.1f;
                }
                if (GorillaLocomotion.Player.Instance.wasLeftHandTouching && GorillaLocomotion.Player.Instance.wasRightHandTouching)
                {
                    stickpart.transform.position = Vector3.zero;
                    //partDelay = Time.time;
                }
            }
        }

        public static void DisableStickyHands()
        {
            if (stickpart != null)
            {
                UnityEngine.Object.Destroy(stickpart);
                stickpart = null;
            }
        }

        public static void EnableSlideControl()
        {
            oldSlide = GorillaLocomotion.Player.Instance.slideControl;
            GorillaLocomotion.Player.Instance.slideControl = 1f;
        }

        public static void EnableWeakSlideControl()
        {
            oldSlide = GorillaLocomotion.Player.Instance.slideControl;
            GorillaLocomotion.Player.Instance.slideControl = oldSlide*2f;
        }

        public static void DisableSlideControl()
        {
            GorillaLocomotion.Player.Instance.slideControl = oldSlide;
        }

        public static void PunchMod()
        {
            int index = -1;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    index++;

                    Vector3 they = vrrig.rightHandTransform.position;
                    Vector3 notthem = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                    float distance = Vector3.Distance(they, notthem);

                    if (distance < 0.25)
                    {
                        GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(vrrig.rightHandTransform.position - lastRight[index]) * 10f;
                    }
                    lastRight[index] = vrrig.rightHandTransform.position;

                    they = vrrig.leftHandTransform.position;
                    distance = Vector3.Distance(they, notthem);

                    if (distance < 0.25)
                    {
                        GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity += Vector3.Normalize(vrrig.leftHandTransform.position - lastLeft[index]) * 10f;
                    }
                    lastLeft[index] = vrrig.leftHandTransform.position;
                }
            }
        }

        public static void SolidPlayers()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig && Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 5f)
                {
                    Vector3 pointA = vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f);
                    Vector3 pointB = vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f);
                    GameObject bodyCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bodyCollider.GetComponent<Rigidbody>());
                    bodyCollider.GetComponent<Renderer>().enabled = false;
                    bodyCollider.transform.position = Vector3.Lerp(pointA, pointB, 0.5f);
                    bodyCollider.transform.rotation = vrrig.transform.rotation;
                    bodyCollider.transform.localScale = new Vector3(0.3f, 0.55f, 0.3f);
                    UnityEngine.Object.Destroy(bodyCollider, Time.deltaTime * 2);

                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        pointA = vrrig.mainSkin.bones[bones[i]].position;
                        pointB = vrrig.mainSkin.bones[bones[i + 1]].position;
                        bodyCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(bodyCollider.GetComponent<Rigidbody>());
                        bodyCollider.GetComponent<Renderer>().enabled = false;
                        bodyCollider.transform.position = Vector3.Lerp(pointA, pointB, 0.5f);
                        bodyCollider.transform.LookAt(pointB);
                        bodyCollider.transform.localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(pointA, pointB));
                        UnityEngine.Object.Destroy(bodyCollider, Time.deltaTime * 2);
                    }
                }
            }
        }

        public static void ThrowControllers()
        {
            if (leftPrimary)
            {
                if (leftThrow != null)
                {
                    GorillaLocomotion.Player.Instance.leftControllerTransform.position = leftThrow.transform.position;
                    GorillaLocomotion.Player.Instance.leftControllerTransform.rotation = leftThrow.transform.rotation;
                }
                else
                {
                    leftThrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leftThrow.GetComponent<Renderer>().enabled = false;
                    UnityEngine.Object.Destroy(leftThrow.GetComponent<BoxCollider>());
                    UnityEngine.Object.Destroy(leftThrow.GetComponent<Rigidbody>());

                    leftThrow.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    leftThrow.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                    Rigidbody comp = leftThrow.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.velocity = GorillaLocomotion.Player.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    try
                    {
                        if (GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                        {
                            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").AddComponent<GorillaVelocityEstimator>();
                        }
                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                    } catch { }
                }
            }
            else
            {
                if (leftThrow != null)
                {
                    UnityEngine.Object.Destroy(leftThrow);
                    leftThrow = null;
                }
            }

            if (rightPrimary)
            {
                if (rightThrow != null)
                {
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position = rightThrow.transform.position;
                    GorillaLocomotion.Player.Instance.rightControllerTransform.rotation = rightThrow.transform.rotation;
                }
                else
                {
                    rightThrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    rightThrow.GetComponent<Renderer>().enabled = false;
                    UnityEngine.Object.Destroy(rightThrow.GetComponent<BoxCollider>());
                    UnityEngine.Object.Destroy(rightThrow.GetComponent<Rigidbody>());

                    rightThrow.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                    rightThrow.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                    Rigidbody comp = rightThrow.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    comp.velocity = GorillaLocomotion.Player.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0);
                    try
                    {
                        if (GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>() == null)
                        {
                            GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").AddComponent<GorillaVelocityEstimator>();
                        }
                        comp.angularVelocity = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
                    } catch { }
                }
            }
            else
            {
                if (rightThrow != null)
                {
                    UnityEngine.Object.Destroy(rightThrow);
                    rightThrow = null;
                }
            }
        }

        public static void StickLongArms()
        {
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.leftHandTransform.position + (GorillaTagger.Instance.leftHandTransform.forward * (armlength - 0.917f));
            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.rightHandTransform.position + (GorillaTagger.Instance.rightHandTransform.forward * (armlength - 0.917f));
        }

        public static void EnableSteamLongArms()
        {
            GorillaLocomotion.Player.Instance.transform.localScale = new Vector3(armlength, armlength, armlength);
        }

        public static void DisableSteamLongArms()
        {
            GorillaLocomotion.Player.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void MultipliedLongArms()
        {
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position) * armlength;
            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - (GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position) * armlength;
        }

        public static void VerticalLongArms()
        {
            Vector3 lefty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            lefty.y *= armlength;
            Vector3 righty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            righty.y *= armlength;
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - lefty;
            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - righty;
        }

        public static void HorizontalLongArms()
        {
            Vector3 lefty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            lefty.x *= armlength;
            lefty.z *= armlength;
            Vector3 righty = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            righty.x *= armlength;
            righty.z *= armlength;
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - lefty;
            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.headCollider.transform.position - righty;
        }

        public static GameObject lvT = null;
        public static GameObject rvT = null;
        public static void CreateVelocityTrackers()
        {
            lvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(lvT.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(lvT.GetComponent<Rigidbody>());
            lvT.GetComponent<Renderer>().enabled = false;
            lvT.AddComponent<GorillaVelocityTracker>();

            rvT = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(rvT.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(rvT.GetComponent<Rigidbody>());
            rvT.GetComponent<Renderer>().enabled = false;
            rvT.AddComponent<GorillaVelocityTracker>();
        }

        public static void DestroyVelocityTrackers()
        {
            UnityEngine.Debug.Log(lvT);
            UnityEngine.Debug.Log(rvT);
        }

        public static void VelocityLongArms()
        {
            lvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.leftHandTransform.position;
            rvT.transform.position = GorillaTagger.Instance.headCollider.transform.position - GorillaTagger.Instance.rightHandTransform.position;
            GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position -= lvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * 0.0125f;
            GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position -= rvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0) * 0.0125f;
        }

        public static void FlickJump()
        {
            if (rightPrimary)
            {
                GorillaLocomotion.Player.Instance.rightControllerTransform.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -1.5f, 0f);
            }
        }

        public static void LongJump()
        {
            if (rightPrimary)
            {
                if (longJumpPower == Vector3.zero)
                {
                    longJumpPower = GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity / 150f;
                    longJumpPower.y = 0f;
                }
                GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.transform.position += longJumpPower;
            }
            else
            {
                longJumpPower = Vector3.zero;
            }
        }

        public static void BunnyHop()
        {
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512);

            if (Ray.distance < 0.15f)
            {
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity = new Vector3(GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity.x, (GorillaLocomotion.Player.Instance.jumpMultiplier * 2.727272727f), GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity.z);
            }
        }

        public static void Strafe()
        {
            Vector3 funnyDir = GorillaTagger.Instance.bodyCollider.transform.forward * GorillaLocomotion.Player.Instance.maxJumpSpeed;
            GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity = new Vector3(funnyDir.x, GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity.y, funnyDir.z);
        }

        public static void GripBunnyHop()
        {
            if (rightGrab)
            {
                BunnyHop();
            }
        }

        public static void GripStrafe()
        {
            if (rightGrab)
            {
                Strafe();
            }
        }

        private static float preBounciness = 0f;
        private static PhysicMaterialCombine whateverthisis = PhysicMaterialCombine.Maximum;
        private static float preFrictiness = 0f;

        public static void PreBouncy()
        {
            preBounciness = GorillaTagger.Instance.bodyCollider.material.bounciness;
            whateverthisis = GorillaTagger.Instance.bodyCollider.material.bounceCombine;
            preFrictiness = GorillaTagger.Instance.bodyCollider.material.dynamicFriction;
        }

        public static void Bouncy()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = 1f;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = PhysicMaterialCombine.Maximum;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
        }

        public static void PostBouncy()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = preBounciness;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = whateverthisis;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = preFrictiness;
        }

        public static void DisableAir()
        {
            foreach (ForceVolume fv in Resources.FindObjectsOfTypeAll<ForceVolume>())
            {
                if (fv.enabled)
                {
                    fv.enabled = false;
                    fvol.Add(fv);
                }
            }
        }

        public static void EnableAir()
        {
            foreach (ForceVolume fv in fvol)
            {
                fv.enabled = true;
            }
            fvol.Clear();
        }

        public static void PiggybackGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.rigidbody.transform.position = whoCopy.transform.position + new Vector3(0f, 0.5f, 0f);
                    GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
                }
            }
        }

        public static void CopyMovementGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position;
                    } catch { }
                    GorillaTagger.Instance.offlineVRRig.transform.rotation = whoCopy.transform.rotation;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.rotation = whoCopy.transform.rotation;
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = whoCopy.leftHandTransform.position;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = whoCopy.rightHandTransform.position;

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = whoCopy.leftHandTransform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = whoCopy.rightHandTransform.rotation;

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = whoCopy.headMesh.transform.rotation;
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

        public static void FollowPlayerGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    Vector3 look = whoCopy.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position;
                    look.Normalize();

                    Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position + (look * ((flySpeed / 2f) * Time.deltaTime));

                    GorillaTagger.Instance.offlineVRRig.transform.position = position;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = position;
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.transform.LookAt(whoCopy.transform.position);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.LookAt(whoCopy.transform.position);
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
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

        public static void OrbitPlayerGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));
                    } catch { }
                    GorillaTagger.Instance.offlineVRRig.transform.LookAt(whoCopy.transform.position);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.LookAt(whoCopy.transform.position);
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
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

        public static void JumpscareGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * (UnityEngine.Random.Range(10f, 50f) / 100f));
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.forward * (UnityEngine.Random.Range(10f, 50f) / 100f));
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.LookAt(whoCopy.headMesh.transform.position);
                    Quaternion dirLook = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation;

                    GorillaTagger.Instance.offlineVRRig.transform.rotation = dirLook;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.rotation = dirLook;
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.right * 0.2f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = whoCopy.headMesh.transform.position + (whoCopy.headMesh.transform.right * -0.2f);

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = dirLook;

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
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

        public static void AnnoyPlayerGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    Vector3 position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                    GorillaTagger.Instance.offlineVRRig.transform.position = position;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = position;
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.transform.LookAt(whoCopy.transform.position);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.LookAt(whoCopy.transform.position);
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = whoCopy.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f, UnityEngine.Random.Range(-10f, 10f) / 10f);

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));

                    if (PhotonNetwork.InRoom)
                    {
                        GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                            91,
                            false,
                            999999f
                        });
                        RPCProtection();
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(91, false, 999999f);
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

        public static void IntercourseGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    if (!GetIndex("Reverse Intercourse").enabled)
                    {
                        GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * -(0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f)));
                        try
                        {
                            GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * -(0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f)));
                        } catch { }

                        GorillaTagger.Instance.offlineVRRig.transform.rotation = whoCopy.transform.rotation;
                        try
                        {
                            GorillaTagger.Instance.myVRRig.transform.rotation = whoCopy.transform.rotation;
                        } catch { }

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * -0.2f) + whoCopy.transform.up * -0.4f;
                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * 0.2f) + whoCopy.transform.up * -0.4f;

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = whoCopy.transform.rotation;
                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = whoCopy.transform.rotation;
                    } else
                    {
                        GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * (0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f)));
                        try
                        {
                            GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * (0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f)));
                        } catch { }

                        GorillaTagger.Instance.offlineVRRig.transform.rotation = whoCopy.transform.rotation;
                        try
                        {
                            GorillaTagger.Instance.myVRRig.transform.rotation = whoCopy.transform.rotation;
                        } catch { }

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * -0.2f) + whoCopy.transform.up * -0.4f;
                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * 0.2f) + whoCopy.transform.up * -0.4f;

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = whoCopy.transform.rotation;
                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = whoCopy.transform.rotation;

                        GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = whoCopy.transform.rotation;
                    }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = whoCopy.transform.rotation;

                    if ((Time.frameCount % 45) == 0)
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                                64,
                                false,
                                999999f
                            });
                            RPCProtection();
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(64, false, 999999f);
                        }
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

        public static void HeadGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (isCopying && whoCopy != null)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * (0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f))) + (whoCopy.transform.up * -0.4f);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = whoCopy.transform.position + (whoCopy.transform.forward * (0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f))) + (whoCopy.transform.up * -0.4f);
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.transform.rotation = Quaternion.Euler(whoCopy.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.rotation = Quaternion.Euler(whoCopy.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
                    } catch { }

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * 0.2f) + whoCopy.transform.up * -0.4f;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = (whoCopy.transform.position + whoCopy.transform.right * -0.2f) + whoCopy.transform.up * -0.4f;

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = whoCopy.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = whoCopy.transform.rotation;

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = Quaternion.Euler(whoCopy.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                    if ((Time.frameCount % 45) == 0)
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.All, new object[]{
                                64,
                                true,
                                999999f
                            });
                            RPCProtection();
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(64, true, 999999f);
                        }
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

        public static void RemoveCopy()
        {
            isCopying = false;
            whoCopy = null;
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        public static void SpazHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = UnityEngine.Random.Range(0f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = UnityEngine.Random.Range(0f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = UnityEngine.Random.Range(0f, 360f);
        }

        public static void LaggyRig()
        {
            ghostException = true;
            if (Time.time > laggyRigDelay)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                idiotfixthingy = true;
                laggyRigDelay = Time.time + 0.5f;
            } else
            {
                if (idiotfixthingy)
                {
                    idiotfixthingy = false;
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                }
            }
        }

        public static void UpdateRig()
        {
            ghostException = true;
            if (rightPrimary && !lastprimaryhit)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                idiotfixthingy = true;
            }
            else
            {
                if (idiotfixthingy)
                {
                    idiotfixthingy = false;
                } else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                }
                
            }
            lastprimaryhit = rightPrimary;
        }

        public static void RandomSpazHead()
        {
            if (headspazType)
            {
                SpazHead();
                if (Time.time > headspazDelay)
                {
                    headspazType = false;
                    headspazDelay = Time.time + UnityEngine.Random.Range(1000f,4000f)/1000f;
                }
            } else
            {
                Fun.FixHead();
                if (Time.time > headspazDelay)
                {
                    headspazType = true;
                    headspazDelay = Time.time + UnityEngine.Random.Range(200f, 1000f) / 1000f;
                }
            }
        }
    }
}
