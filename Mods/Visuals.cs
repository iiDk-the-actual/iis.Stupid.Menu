using GorillaNetworking;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Visuals
    {
        public static void LightningStrike(Vector3 position)
        {
            GameObject line = new GameObject("Line");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = Color.yellow; liner.endColor = Color.yellow; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 5; liner.useWorldSpace = true;
            Vector3 victim = position;
            for (int i = 0; i < 5; i++)
            {
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, false, 99999f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68, true, 99999f);
                liner.SetPosition(i, victim);
                victim += new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
            }
            liner.material.shader = Shader.Find("GUI/Text Shader");
            UnityEngine.Object.Destroy(line, 2f);
        }

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

        public static void VelocityLabel()
        {
            GameObject go = new GameObject("Lbl");
            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            TextMesh textMesh = go.AddComponent<TextMesh>();
            textMesh.color = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity.magnitude >= GorillaLocomotion.Player.Instance.maxJumpSpeed ? Color.green : Color.white;
            textMesh.fontSize = 24;
            textMesh.fontStyle = activeFontStyle;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.text = string.Format("{0:F1}m/s", GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity.magnitude);

            go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.1f, 0f);
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
            UnityEngine.Object.Destroy(go, Time.deltaTime);
        }

        private static float startTime = 0f;
        private static float endTime = 0f;
        private static bool lastWasTagged = false;
        public static void TimeLabel()
        {
            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = false;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (PlayerIsTagged(vrrig))
                    {
                        isThereTagged = true;
                        break;
                    }
                }
                if (isThereTagged)
                {
                    bool playerIsTagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
                    if (playerIsTagged && !lastWasTagged)
                    {
                        endTime = Time.time - startTime;
                    }
                    if (!playerIsTagged && lastWasTagged)
                    {
                        startTime = Time.time;
                    }
                    lastWasTagged = playerIsTagged;

                    GameObject go = new GameObject("Lbl");
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.color = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) ? Color.green : Color.white;
                    textMesh.fontSize = 24;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.text = !playerIsTagged ?
                        FormatUnix(Mathf.FloorToInt(Time.time - startTime)) :
                        FormatUnix(Mathf.FloorToInt(endTime));

                    go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, GetIndex("Velocity Label").enabled ? 0.2f : 0.1f, 0f);
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, Time.deltaTime);
                }
                else
                {
                    startTime = Time.time;
                }
            }
        }

        public static void NearbyTaggerLabel()
        {
            if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                float closest = float.MaxValue;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig && PlayerIsTagged(vrrig))
                    {
                        float dist = Vector3.Distance(GorillaTagger.Instance.headCollider.transform.position, vrrig.headMesh.transform.position);
                        if (dist < closest)
                        {
                            closest = dist;
                        }
                    }
                }
                if (closest != float.MaxValue)
                {
                    Color colorn = Color.green;
                    if (closest < 30f)
                    {
                        colorn = Color.yellow;
                    }
                    if (closest < 20f)
                    {
                        colorn = new Color32(255, 90, 0, 255);
                    }
                    if (closest < 10f)
                    {
                        colorn = Color.red;
                    }
                    GameObject go = new GameObject("Lbl");
                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.color = colorn;
                    textMesh.fontSize = 24;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.text = string.Format("{0:F1}m", closest);

                    go.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, GetIndex("Last Label").enabled ? 0.2f : 0.1f, 0f);
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, Time.deltaTime);
                }
            }
        }

        public static void LastLabel()
        {
            bool isThereTagged = false;
            int left = PhotonNetwork.PlayerList.Length;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (PlayerIsTagged(vrrig))
                {
                    isThereTagged = true;
                    left--;
                }
            }
            if (PhotonNetwork.InRoom && isThereTagged)
            {
                GameObject go = new GameObject("Lbl");
                go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                TextMesh textMesh = go.AddComponent<TextMesh>();
                textMesh.color = left <= 1 && !PlayerIsTagged(GorillaTagger.Instance.offlineVRRig) ? Color.green : Color.white;
                textMesh.fontSize = 24;
                textMesh.fontStyle = activeFontStyle;
                textMesh.characterSize = 0.1f;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
                textMesh.text = left.ToString() + " left";

                go.transform.position = GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, 0.1f, 0f);
                go.transform.LookAt(Camera.main.transform.position);
                go.transform.Rotate(0f, 180f, 0f);
                UnityEngine.Object.Destroy(go, Time.deltaTime);
            }
        }

        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

        private static GameObject visualizerObject = null;
        private static GameObject visualizerOutline = null;
        public static void CreateAudioVisualizer()
        {
            visualizerObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visualizerOutline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            UnityEngine.Object.Destroy(visualizerObject.GetComponent<Collider>());
            UnityEngine.Object.Destroy(visualizerOutline.GetComponent<Collider>());
            UnityEngine.Object.Destroy(visualizerObject.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(visualizerOutline.GetComponent<Rigidbody>());
        }

        public static void AudioVisualizer()
        {
            visualizerObject.GetComponent<Renderer>().material.color = GetBGColor(0f);
            visualizerOutline.GetComponent<Renderer>().material.color = GetBRColor(0f);

            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GorillaLocomotion.Player.Instance.locomotionEnabledLayers);
            visualizerObject.transform.position = Ray.point;
            visualizerObject.transform.rotation = Quaternion.LookRotation(Ray.normal) * Quaternion.Euler(90f, 0f, 0f);

            float size = 0f;
            GameObject recorder = GameObject.Find("P_NetworkWrapper(Clone)/VoiceNetworkObject");
            if (recorder != null)
            {
                size = recorder.GetComponent<Recorder>().LevelMeter.CurrentAvgAmp;
            }

            size *= 16f;
            visualizerObject.transform.localScale = new Vector3(size, 0.05f, size);

            visualizerObject.GetComponent<Renderer>().enabled = size > 0.05f;
            visualizerOutline.GetComponent<Renderer>().enabled = size > 0.05f;

            visualizerOutline.transform.position = visualizerObject.transform.position;
            visualizerOutline.transform.rotation = visualizerObject.transform.rotation;
            visualizerOutline.transform.localScale = new Vector3(size + 0.05f, 0.025f, size + 0.05f);
        }

        public static void DestroyAudioVisualizer()
        {
            UnityEngine.Object.Destroy(visualizerObject);
            UnityEngine.Object.Destroy(visualizerOutline);
        }

        public static void ShowPlayspaceCenter()
        {
            GameObject playspaceCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Collider>());
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Renderer>());

            playspaceCenter.GetComponent<Renderer>().material.color = GetBGColor(0f);
            playspaceCenter.transform.localScale = new Vector3(0.1f, 0.1f, 0.15f);

            Physics.Raycast(new Vector3(GorillaTagger.Instance.transform.position.x, GorillaTagger.Instance.headCollider.transform.position.y, GorillaTagger.Instance.transform.position.x), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
            playspaceCenter.transform.position = Ray.point;
            playspaceCenter.transform.rotation = GorillaTagger.Instance.transform.rotation;

            UnityEngine.Object.Destroy(playspaceCenter, Time.deltaTime);
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
        public static void EnableRemoveLeaves()
        {
            foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (g.activeSelf && g.name.Contains("leaves_green"))
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
        }

        /*
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
        }*/

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

        public static void CosmeticESP() // Messy code lol
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    bool showtracersplz = false;
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBADE") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAGS"))
                    {
                        thecolor = Color.green;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBACP"))
                    {
                        thecolor = Color.blue;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK"))
                    {
                        thecolor = Color.red;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAD"))
                    {
                        thecolor = Color.white;
                        showtracersplz = true;
                    }
                    if (showtracersplz)
                    {
                        GameObject line = new GameObject("Line");
                        LineRenderer liner = line.AddComponent<LineRenderer>();
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

        private static GameObject l = null;
        private static GameObject r = null;

        private static void UpdateLimbColor()
        {
            Color limbcolor = GorillaTagger.Instance.offlineVRRig.playerColor;
            if (PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                limbcolor = new Color32(255, 111, 0, 255);
            }

            l.GetComponent<Renderer>().material.color = limbcolor;
            r.GetComponent<Renderer>().material.color = limbcolor;
        }

        public static void StartNoLimb()
        {
            l = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(l.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

            l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(r.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(r.GetComponent<SphereCollider>());

            r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            UpdateLimbColor();
        }

        public static void NoLimbMode()
        {
            l.transform.position = TrueLeftHand().position;
            r.transform.position = TrueRightHand().position;
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = new Color(GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.r, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.g, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.b, 0f);
            UpdateLimbColor();
        }

        public static void EndNoLimb()
        {
            UnityEngine.Object.Destroy(l);
            UnityEngine.Object.Destroy(r);

            GorillaTagger.Instance.offlineVRRig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
            GorillaTagger.Instance.offlineVRRig.mainSkin.material.color = new Color(GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.r, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.g, GorillaTagger.Instance.offlineVRRig.mainSkin.material.color.b, 1f);
        }

        public static void CasualTracers()
        {
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); } if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
            }
        }

        public static void InfectionTracers()
        {
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            GameObject line = new GameObject("Line");
                            LineRenderer liner = line.AddComponent<LineRenderer>();
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
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
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
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
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
                    liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    liner.SetPosition(1, vrrig.transform.position);
                    liner.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(line, Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    GameObject line = new GameObject("Line");
                    LineRenderer liner = line.AddComponent<LineRenderer>();
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = lineWidth; liner.endWidth = lineWidth; liner.positionCount = 2; liner.useWorldSpace = true;
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
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
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                } else {
                    if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
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
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
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
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
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
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
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
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
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
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
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

        public static void CasualDistanceESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, Time.deltaTime);
                }
            }
        }

        public static void InfectionDistanceESP()
        {
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
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = new Color32(255, 111, 0, 255);
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            UnityEngine.Color thecolor2 = Color.white;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                            if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                            GameObject go = new GameObject("Dist");
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 18;
                            textMesh.fontStyle = activeFontStyle;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;
                            textMesh.color = thecolor2;
                            go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                            textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                            bg.transform.parent = go.transform;
                            bg.transform.localPosition = Vector3.zero;
                            bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                            bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            bg.GetComponent<Renderer>().material.color = thecolor;
                            textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                            go.transform.LookAt(Camera.main.transform.position);
                            go.transform.Rotate(0f, 180f, 0f);
                            UnityEngine.Object.Destroy(go, Time.deltaTime);
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            UnityEngine.Color thecolor = vrrig.playerColor;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                            UnityEngine.Color thecolor2 = Color.white;
                            if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                            if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                            GameObject go = new GameObject("Dist");
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 18;
                            textMesh.fontStyle = activeFontStyle;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;
                            textMesh.color = thecolor2;
                            go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                            textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                            bg.transform.parent = go.transform;
                            bg.transform.localPosition = Vector3.zero;
                            bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                            bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            bg.GetComponent<Renderer>().material.color = thecolor;
                            textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                            go.transform.LookAt(Camera.main.transform.position);
                            go.transform.Rotate(0f, 180f, 0f);
                            UnityEngine.Object.Destroy(go, Time.deltaTime);
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
                        UnityEngine.Color thecolor2 = Color.white;
                        if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                        if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                        GameObject go = new GameObject("Dist");
                        TextMesh textMesh = go.AddComponent<TextMesh>();
                        textMesh.fontSize = 18;
                        textMesh.fontStyle = activeFontStyle;
                        textMesh.characterSize = 0.1f;
                        textMesh.anchor = TextAnchor.MiddleCenter;
                        textMesh.alignment = TextAlignment.Center;
                        textMesh.color = thecolor2;
                        go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                        textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                        bg.transform.parent = go.transform;
                        bg.transform.localPosition = Vector3.zero;
                        bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        bg.GetComponent<Renderer>().material.color = thecolor;
                        textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                        go.transform.LookAt(Camera.main.transform.position);
                        go.transform.Rotate(0f, 180f, 0f);
                        UnityEngine.Object.Destroy(go, Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntDistanceESP()
        {
            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    UnityEngine.Color thecolor2 = Color.white;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor2 = titleColor; }
                    if (GetIndex("Transparent Theme").enabled) { thecolor2.a = 0.5f; }
                    GameObject go = new GameObject("Dist");
                    TextMesh textMesh = go.AddComponent<TextMesh>();
                    textMesh.fontSize = 18;
                    textMesh.fontStyle = activeFontStyle;
                    textMesh.characterSize = 0.1f;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.alignment = TextAlignment.Center;
                    textMesh.color = thecolor2;
                    go.transform.position = vrrig.transform.position + new Vector3(0f, -0.2f, 0f);
                    textMesh.text = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, vrrig.transform.position));
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(bg.GetComponent<Collider>());
                    bg.transform.parent = go.transform;
                    bg.transform.localPosition = Vector3.zero;
                    bg.transform.localScale = new Vector3(textMesh.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    bg.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    bg.GetComponent<Renderer>().material.color = thecolor;
                    textMesh.GetComponent<Renderer>().material.renderQueue = bg.GetComponent<Renderer>().material.renderQueue + 1;
                    go.transform.LookAt(Camera.main.transform.position);
                    go.transform.Rotate(0f, 180f, 0f);
                    UnityEngine.Object.Destroy(go, Time.deltaTime);
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
