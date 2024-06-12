using GorillaNetworking;
using GorillaTag;
using iiMenu.Classes;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UIElements;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Visuals
    {
        public static void WatchOn()
        {
            GameObject mainwatch = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            regwatchobject = UnityEngine.Object.Instantiate(mainwatch, rightHand ? GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);
            UnityEngine.Object.Destroy(regwatchobject.GetComponent<GorillaHuntComputer>());
            regwatchobject.SetActive(true);

            Transform thething = regwatchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
            thething.Find("Hat").gameObject.SetActive(false);
            thething.Find("Face").gameObject.SetActive(false);
            thething.Find("Badge").gameObject.SetActive(false);
            thething.Find("Material").gameObject.SetActive(false);
            thething.Find("Left Hand").gameObject.SetActive(false);
            thething.Find("Right Hand").gameObject.SetActive(false);

            regwatchText = thething.Find("Text").gameObject;
            regwatchShell = regwatchobject.transform.Find("HuntWatch_ScreenLocal").gameObject;

            regwatchShell.GetComponent<Renderer>().material = OrangeUI;

            if (rightHand)
            {
                regwatchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                regwatchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                regwatchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }

        public static void WatchStep()
        {
            regwatchText.GetComponent<UnityEngine.UI.Text>().text = "ii's Stupid Menu";
            if (doCustomName)
            {
                regwatchText.GetComponent<UnityEngine.UI.Text>().text = customMenuName;
            }
            regwatchText.GetComponent<UnityEngine.UI.Text>().text += "\n<color=grey>" + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString() + " FPS\n" + DateTime.Now.ToString("hh:mm tt") + "</color>";
            regwatchText.GetComponent<UnityEngine.UI.Text>().color = titleColor;

            if (lowercaseMode)
            {
                regwatchText.GetComponent<UnityEngine.UI.Text>().text = regwatchText.GetComponent<UnityEngine.UI.Text>().text.ToLower();
            }
        }

        public static void WatchOff()
        {
            UnityEngine.Object.Destroy(regwatchobject);
        }

        public static Material oldSkyMat = null;
        public static void DoCustomSkyboxColor()
        {
            GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
            oldSkyMat = sky.GetComponent<Renderer>().material;
        }

        public static void CustomSkyboxColor()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky").GetComponent<Renderer>().material = OrangeUI;
        }

        public static void UnCustomSkyboxColor()
        {
            GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
            sky.GetComponent<Renderer>().material = oldSkyMat;
        }

        public static void GreenScreen()
        {
            Color bgcolor = Color.green;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);
        }

        public static void BlueScreen()
        {
            Color bgcolor = Color.blue;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);
        }

        public static void RedScreen()
        {
            Color bgcolor = Color.red;

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgcolor;
            UnityEngine.Object.Destroy(a, Time.deltaTime * 2f);
        }

        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

        public static void FixRigColors()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                {
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                }
            }
        }

        /*public static void EnableRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("smallleaves"))
                {
                    g.SetActive(false);
                    leaves.Add(g);
                }
            }
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in leaves)
            {
                l.SetActive(true);
            }
            leaves.Clear();
        }*/
        public static Material oldmat = null;
        public static Material noleafmat = null;
        public static Texture2D forestTexture = null;
        public static List<GameObject> atlases = new List<GameObject> { };
        public static void EnableRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("forestatlas (combined") && g.GetComponent<Renderer>() != null && g.GetComponent<Renderer>().material.name.Contains("forest"))
                {
                    if (oldmat == null)
                    {
                        oldmat = g.GetComponent<Renderer>().material;
                    }
                    if (noleafmat == null)
                    {
                        noleafmat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                        noleafmat.SetFloat("_Surface", 1);
                        noleafmat.SetFloat("_Blend", 0);
                        noleafmat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        noleafmat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        noleafmat.SetFloat("_ZWrite", 0);
                        noleafmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        noleafmat.renderQueue = (int)RenderQueue.Transparent;

                        noleafmat.SetFloat("_Glossiness", 0.0f);
                        noleafmat.SetFloat("_Metallic", 0.0f);

                        if (forestTexture == null)
                        {
                            forestTexture = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/main/forestatlasv2.png", "noLeavesTexture.png");
                            forestTexture.filterMode = FilterMode.Point;
                            forestTexture.wrapMode = TextureWrapMode.Clamp;
                        }

                        noleafmat.color = new Color(1, 1, 1, 1);
                        noleafmat.mainTexture = forestTexture;
                    }
                    g.GetComponent<Renderer>().material = noleafmat;
                    g.GetComponent<Renderer>().sortingOrder = UnityEngine.Random.Range(0, 255);

                    atlases.Add(g);
                }
            }
            hasFoundAllBoards = false;
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in atlases)
            {
                //Material share = l.GetComponent<Renderer>().sharedMaterial;
                //Material reg = l.GetComponent<Renderer>().material;
                //l.GetComponent<Renderer>().material.shader = share.shader;
                l.GetComponent<Renderer>().material = oldmat;//.CopyPropertiesFromMaterial(share);
            }
            atlases.Clear();
            hasFoundAllBoards = false;
        }

        public static void EnableRemoveCherryBlossoms()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("Cherry Blossoms"))
                {
                    g.SetActive(false);
                    cblos.Add(g);
                }
            }
        }

        public static void DisableRemoveCherryBlossoms()
        {
            foreach (GameObject l in cblos)
            {
                l.SetActive(true);
            }
            cblos.Clear();
        }

        public static void DisableCosmetics()
        {
            Transform transform = GorillaTagger.Instance.offlineVRRig.mainCamera.transform.Find("FirstPersonCosmeticsOverrides");
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject v = transform.GetChild(i).gameObject;
                if (v.activeSelf)
                {
                    v.SetActive(false);
                    cosmetics.Add(v);
                }
            }
        }

        public static void EnableCosmetics()
        {
            foreach (GameObject c in cosmetics)
            {
                c.SetActive(true);
            }
            cosmetics.Clear();
        }

        public static void NoSmoothRigs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.lerpValueBody = 1f;
                    vrrig.lerpValueFingers = 1f;
                }
            }
        }

        public static void ReSmoothRigs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.lerpValueBody = GorillaTagger.Instance.offlineVRRig.lerpValueBody;
                    vrrig.lerpValueFingers = GorillaTagger.Instance.offlineVRRig.lerpValueFingers;
                }
            }
        }

        public static void CasualTracers()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); } if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
            }
        }

        public static void InfectionTracers()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                            liner.SetPosition(1, vrrig.transform.position);
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
                            liner.SetPosition(1, vrrig.transform.position);
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject line = new GameObject("Line");
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                        liner.SetPosition(1, vrrig.transform.position);
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntTracers()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
            }
        }

        public static void CasualBoneESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                UnityEngine.Color thecolor = vrrig.playerColor;
                if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    }
                }
            }
        }

        public static void InfectionBoneESP()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, Time.deltaTime);
                            }
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, Time.deltaTime);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                        liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                        UnityEngine.Object.Destroy(liner, Time.deltaTime);
                        for (int i = 0; i < bones.Count<int>(); i += 2)
                        {
                            liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                            liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                            UnityEngine.Object.Destroy(liner, Time.deltaTime);
                        }
                    }
                }
            }
        }

        public static void HuntBoneESP()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    }
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, Time.deltaTime);
                    }
                }
            }
        }

        public static void CasualChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                }
            }
        }

        public static void InfectionChams()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                        else
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void HuntChams()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                } else {
                    if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = Color.red;
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    }
                    else
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    }
                }
            }
        }

        public static void DisableChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }

        public static void CasualBeacons()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
            }
        }

        public static void InfectionBeacons()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                            liner.material.shader = Shader.Find("GUI/Text Shader");
                            UnityEngine.Object.Destroy(line, Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject line = new GameObject("Line");
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                        liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntBeacons()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                    liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
            }
        }

        public static void CasualBoxESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f,0.5f,0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, Time.deltaTime);
                }
            }
        }

        public static void InfectionBoxESP()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        box.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(box, Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntBoxESP()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, Time.deltaTime);
                }
            }
        }

        public static void CasualHollowBoxESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        public static void HollowInfectionBoxESP()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().enabled = false;

                            GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            UnityEngine.Object.Destroy(box);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().enabled = false;

                            GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, Time.deltaTime);

                            UnityEngine.Object.Destroy(box);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        box.GetComponent<Renderer>().enabled = false;

                        GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, Time.deltaTime);

                        UnityEngine.Object.Destroy(box);
                    }
                }
            }
        }

        public static void HollowHuntBoxESP()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().enabled = false;

                    GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        public static void CasualBreadcrumbs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
            }
        }

        public static void InfectionBreadcrumbs()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                            sphere.GetComponent<Renderer>().material.color = thecolor;
                            sphere.transform.position = vrrig.transform.position;
                            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            UnityEngine.Object.Destroy(sphere, 10f);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!vrrig.mainSkin.material.name.Contains("fected") && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                            sphere.GetComponent<Renderer>().material.color = thecolor;
                            sphere.transform.position = vrrig.transform.position;
                            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            UnityEngine.Object.Destroy(sphere, 10f);
                        }
                    }
                }
            }
            else
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        UnityEngine.Color thecolor = vrrig.playerColor;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                        sphere.GetComponent<Renderer>().material.color = thecolor;
                        sphere.transform.position = vrrig.transform.position;
                        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        UnityEngine.Object.Destroy(sphere, 10f);
                    }
                }
            }
        }

        public static void HuntBreadcrumbs()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            Photon.Realtime.Player target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
                if (sillyComputer.GetTargetOf(player) == PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
            }
        }

        public static void ShowButtonColliders()
        {
            GameObject left = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            left.transform.parent = GorillaTagger.Instance.leftHandTransform;
            left.GetComponent<Renderer>().material.color = bgColorA;
            left.transform.localPosition = pointerOffset;
            left.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            UnityEngine.Object.Destroy(left.GetComponent<SphereCollider>());
            UnityEngine.Object.Destroy(left, Time.deltaTime);

            GameObject right = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            right.transform.parent = GorillaTagger.Instance.rightHandTransform;
            right.GetComponent<Renderer>().material.color = bgColorA;
            right.transform.localPosition = pointerOffset;
            right.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            UnityEngine.Object.Destroy(right.GetComponent<SphereCollider>());
            UnityEngine.Object.Destroy(right, Time.deltaTime);
        }
    }
}
