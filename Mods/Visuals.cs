using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Menu;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Visuals
    {
        public static void WatchOn()
        {
            GameObject mainwatch = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            regwatchobject = UnityEngine.Object.Instantiate(mainwatch, rightHand ? GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);
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

        public static bool infoWatchMenuName = false;
        public static bool infoWatchTime = false;
        public static bool infoWatchClip = false;
        public static bool infoWatchFPS = false;
        public static bool infoWatchCode = false;
        public static void WatchStep()
        {
            if (!infoWatchMenuName && !infoWatchTime && !infoWatchClip && !infoWatchFPS && !infoWatchCode)
            {
                regwatchText.GetComponent<UnityEngine.UI.Text>().text = "ii's Stupid Menu";
                if (doCustomName)
                    regwatchText.GetComponent<UnityEngine.UI.Text>().text = customMenuName;
                regwatchText.GetComponent<UnityEngine.UI.Text>().text += "\n<color=grey>";
                regwatchText.GetComponent<UnityEngine.UI.Text>().text += Main.lastDeltaTime.ToString() + " FPS\n";
                regwatchText.GetComponent<UnityEngine.UI.Text>().text += DateTime.Now.ToString("hh:mm tt") + "\n</color>";
            }
            else
            {
                if (infoWatchMenuName) regwatchText.GetComponent<UnityEngine.UI.Text>().text = "ii's Stupid Menu\n<color=grey>";
                if (doCustomName && infoWatchMenuName)
                {
                    regwatchText.GetComponent<UnityEngine.UI.Text>().text = customMenuName + "\n<color=grey>";
                }
                if (!infoWatchMenuName)
                {
                    regwatchText.GetComponent<UnityEngine.UI.Text>().text = "<color=grey>";
                }
                if (infoWatchFPS) regwatchText.GetComponent<UnityEngine.UI.Text>().text += Main.lastDeltaTime.ToString() + " FPS\n";
                if (infoWatchTime) regwatchText.GetComponent<UnityEngine.UI.Text>().text += DateTime.Now.ToString("hh:mm tt") + "\n";
                if (infoWatchClip) regwatchText.GetComponent<UnityEngine.UI.Text>().text += "Clip: " + (GUIUtility.systemCopyBuffer.Length > 20 ? GUIUtility.systemCopyBuffer.Substring(0, 20) : GUIUtility.systemCopyBuffer) + "\n";
                if (infoWatchCode) regwatchText.GetComponent<UnityEngine.UI.Text>().text += (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room") + "\n";
                regwatchText.GetComponent<UnityEngine.UI.Text>().text += "</color>";
                regwatchText.GetComponent<UnityEngine.UI.Text>().color = titleColor;
            }
            if (lowercaseMode)
                    regwatchText.GetComponent<UnityEngine.UI.Text>().text = regwatchText.GetComponent<UnityEngine.UI.Text>().text.ToLower();
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

        public static bool PerformanceVisuals;
        public static void PerformanceVisualsEnabled()
        {
            PerformanceVisuals = true;
        }
        public static void PerformanceVisualsDisabled()
        {
            PerformanceVisuals = false;
        }

        public static float PerformanceModeStep = 0.2f;
        public static int PerformanceModeStepIndex = 2;
        public static void ChangePerformanceModeVisualStep()
        {
            PerformanceModeStepIndex++;
            if (PerformanceModeStepIndex > 10)
                PerformanceModeStepIndex = 0;

            PerformanceModeStep = PerformanceModeStepIndex / 10f;
            GetIndex("Change Performance Visuals Step").overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>" + PerformanceModeStep.ToString() + "</color><color=grey>]</color>";
        }
        public static float PerformanceVisualDelay;
        public static int DelayChangeStep;


        private static void FakeScreenColor(Color bgColor)
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                {
                    PerformanceVisualDelay = Time.time + PerformanceModeStep;
                    DelayChangeStep = Time.frameCount;
                }
            }

            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0404f, 16.2321f, -124.5915f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.8359f);
            a.GetComponent<Renderer>().material.color = bgColor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-52.7365f, 17.5233f, -122.333f);
            a.transform.localScale = new Vector3(14.0131f, 6.4907f, 0.0305f);
            a.GetComponent<Renderer>().material.color = bgColor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-51.6623f, 17.5233f, -125.9925f);
            a.transform.localScale = new Vector3(15.5363f, 6.4907f, 0.0305f);
            a.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            a.GetComponent<Renderer>().material.color = bgColor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);

            a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(a.GetComponent<BoxCollider>());
            a.transform.position = new Vector3(-54.0606f, 18.8161f, -124.6264f);
            a.transform.localScale = new Vector3(14.0131f, 0.0347f, 15.5983f);
            a.GetComponent<Renderer>().material.color = bgColor;
            UnityEngine.Object.Destroy(a, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime * 2f);
        }

        public static void GreenScreen()
        {
            FakeScreenColor(Color.green);
        }

        public static void BlueScreen()
        {
            FakeScreenColor(Color.blue);
        }

        public static void RedScreen()
        {
            FakeScreenColor(Color.red);
        }

        public static void VelocityLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GameObject go = new GameObject("Lbl");
            if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            TextMesh textMesh = go.AddComponent<TextMesh>();
            textMesh.color = GorillaTagger.Instance.rigidbody.velocity.magnitude >= GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed ? Color.green : Color.white;
            textMesh.fontSize = 24;
            textMesh.fontStyle = activeFontStyle;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.text = string.Format("{0:F1}m/s", GorillaTagger.Instance.rigidbody.velocity.magnitude);

            go.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.1f, 0f);
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
            UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
        }

        private static float startTime = 0f;
        private static float endTime = 0f;
        private static bool lastWasTagged = false;
        public static void TimeLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                    if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
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
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                else
                {
                    startTime = Time.time;
                }
            }
        }

        public static void NearbyTaggerLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            if (!PlayerIsTagged(GorillaTagger.Instance.offlineVRRig))
            {
                float closest = float.MaxValue;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig && PlayerIsTagged(vrrig))
                    {
                        float dist = Vector3.Distance(GorillaTagger.Instance.headCollider.transform.position, vrrig.headMesh.transform.position);
                        if (dist < closest)
                            closest = dist;
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
                    if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
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
                    UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void LastLabel()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                if (GetIndex("Hidden Labels").enabled) { go.layer = 19; }
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
                UnityEngine.Object.Destroy(go, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
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
        }

        public static void AudioVisualizer()
        {
            visualizerObject.GetComponent<Renderer>().material.color = GetBGColor(0f);
            visualizerOutline.GetComponent<Renderer>().material.color = GetBRColor(0f);

            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
            visualizerObject.transform.position = Ray.point;
            visualizerObject.transform.rotation = Quaternion.LookRotation(Ray.normal) * Quaternion.Euler(90f, 0f, 0f);

            float size = 0f;
            GorillaSpeakerLoudness recorder = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
            if (recorder != null)
            {
                size = recorder.Loudness;
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

        public static void VisualizeNetworkTriggers()
        {
            GameObject triggers = GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        VisualizeCube(child.position, child.rotation, child.localScale, Color.red);
                    }
                } catch { }
            }
        }

        public static void VisualizeMapTriggers()
        {
            GameObject triggers = GameObject.Find("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        VisualizeCube(child.position, child.rotation, child.localScale, GetBGColor(0f));
                    }
                } catch { }
            }
        }

        private static Dictionary<VRRig, List<int>> ntDistanceList = new Dictionary<VRRig, List<int>> { };
        public static float GetTagDistance(VRRig rig)
        {
            if (ntDistanceList.ContainsKey(rig))
            {
                if (ntDistanceList[rig][0] == Time.frameCount)
                {
                    ntDistanceList[rig].Add(Time.frameCount);
                    return 0.25f + (ntDistanceList[rig].Count * 0.15f);
                } else
                {
                    ntDistanceList[rig].Clear();
                    ntDistanceList[rig].Add(Time.frameCount);
                    return 0.25f + (ntDistanceList[rig].Count * 0.15f);
                }
            } else
            {
                ntDistanceList.Add(rig, new List<int> { Time.frameCount });
                return 0.4f;
            }
        }

        private static Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject> { };
        public static void NameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    if (!nametags.ContainsKey(vrrig))
                    {
                        GameObject go = new GameObject("iiMenu_Nametag");
                        go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                        TextMesh textMesh = go.AddComponent<TextMesh>();
                        textMesh.fontSize = 48;
                        textMesh.characterSize = 0.1f;
                        textMesh.anchor = TextAnchor.MiddleCenter;
                        textMesh.alignment = TextAlignment.Center;

                        nametags.Add(vrrig, go);
                    }

                    GameObject nameTag = nametags[vrrig];
                    nameTag.GetComponent<TextMesh>().text = RigManager.GetPlayerFromVRRig(vrrig).NickName;
                    nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                    nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                    nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                    nameTag.transform.LookAt(Camera.main.transform.position);
                    nameTag.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableNameTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
                UnityEngine.Object.Destroy(nametag.Value);
            
            nametags.Clear();
        }

        private static Dictionary<VRRig, GameObject> velnametags = new Dictionary<VRRig, GameObject> { };
        public static void VelocityTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in velnametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    velnametags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!velnametags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Veltag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            velnametags.Add(vrrig, go);
                        }

                        GameObject nameTag = velnametags[vrrig];
                        nameTag.GetComponent<TextMesh>().text = string.Format("{0:F1}m/s", vrrig.LatestVelocity().magnitude);
                        nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableVelocityTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in velnametags)
                UnityEngine.Object.Destroy(nametag.Value);
            
            velnametags.Clear();
        }

        private static Dictionary<VRRig, GameObject> FPSnametags = new Dictionary<VRRig, GameObject> { };
        public static void FPSTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in FPSnametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    FPSnametags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!FPSnametags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_FPStag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            FPSnametags.Add(vrrig, go);
                        }

                        GameObject nameTag = FPSnametags[vrrig];
                        nameTag.GetComponent<TextMesh>().text = $"{vrrig.fps} FPS";
                        nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableFPSTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in FPSnametags)
                UnityEngine.Object.Destroy(nametag.Value);

            FPSnametags.Clear();
        }

        private static Dictionary<VRRig, GameObject> turnNameTags = new Dictionary<VRRig, GameObject> { };
        public static void TurnTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in turnNameTags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    turnNameTags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!turnNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Turntag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            turnNameTags.Add(vrrig, go);
                        }

                        string turnType = vrrig.turnType;
                        int turnFactor = vrrig.turnFactor;

                        GameObject nameTag = turnNameTags[vrrig];
                        nameTag.GetComponent<TextMesh>().text = turnType == "NONE" ? "None" : ToTitleCase(turnType) + " " + turnFactor.ToString();
                        nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableTurnTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in turnNameTags)
                UnityEngine.Object.Destroy(nametag.Value);

            turnNameTags.Clear();
        }

        private static Dictionary<VRRig, GameObject> taggedNameTags = new Dictionary<VRRig, GameObject> { };
        public static void TaggedTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in taggedNameTags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    taggedNameTags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!taggedNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Taggedtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            taggedNameTags.Add(vrrig, go);
                        }

                        
                        GameObject nameTag = taggedNameTags[vrrig];
                        if (PlayerIsTagged(vrrig))
                        {
                            int taggedById = vrrig.taggedById;
                            NetPlayer tagger = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(taggedById, false);

                            if (tagger != null)
                                nameTag.GetComponent<TextMesh>().text = "Tagged by " + tagger?.NickName;
                        } else
                        {
                            nameTag.GetComponent<TextMesh>().text = "";
                        }
                            
                        nameTag.GetComponent<TextMesh>().color = vrrig.playerColor;
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                } catch { }
            }
        }

        public static void DisableTaggedTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in taggedNameTags)
                UnityEngine.Object.Destroy(nametag.Value);

            taggedNameTags.Clear();
        }

        public static void ShowPlayspaceCenter()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GameObject playspaceCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Collider>());
            UnityEngine.Object.Destroy(playspaceCenter.GetComponent<Renderer>());

            playspaceCenter.GetComponent<Renderer>().material.color = GetBGColor(0f);
            playspaceCenter.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            playspaceCenter.transform.localScale = new Vector3(0.1f, 0.1f, 0.15f);

            Physics.Raycast(new Vector3(GorillaTagger.Instance.transform.position.x, GorillaTagger.Instance.headCollider.transform.position.y, GorillaTagger.Instance.transform.position.x), Vector3.down, out var Ray, 512f, NoInvisLayerMask());
            playspaceCenter.transform.position = Ray.point;
            playspaceCenter.transform.rotation = GorillaTagger.Instance.transform.rotation;

            UnityEngine.Object.Destroy(playspaceCenter, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
        }

        public static void FixRigColors()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                    vrrig.mainSkin.material.color = vrrig.playerColor;
            }
        }

        public static string leavesName = "UnityTempFile-63cb50cfea10ced4d91f469791d0d539";
        public static List<GameObject> leaves = new List<GameObject> { };
        public static void EnableRemoveLeaves()
        {
            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
            {
                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                if (v.name.Contains(leavesName))
                {
                    v.SetActive(false);
                    leaves.Add(v);
                }
            }
        }

        public static void DisableRemoveLeaves()
        {
            foreach (GameObject l in leaves)
                l.SetActive(true);
            
            leaves.Clear();
        }

        public static void EnableStreamerRemoveLeaves()
        {
            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
            {
                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                if (v.name.Contains(leavesName))
                {
                    v.layer = 16;
                    leaves.Add(v);
                }
            }
        }

        public static void DisableStreamerRemoveLeaves()
        {
            foreach (GameObject l in leaves)
                l.layer = 0;
            
            leaves.Clear();
        }

        public static List<GameObject> cosmetics = new List<GameObject> { };
        public static void DisableCosmetics()
        {
            try
            {
                foreach (GameObject Cosmetic in GorillaTagger.Instance.offlineVRRig.cosmetics)
                {
                    if (Cosmetic.activeSelf && Cosmetic.transform.parent == GorillaTagger.Instance.offlineVRRig.mainCamera.transform.Find("HeadCosmetics"))
                    {
                        cosmetics.Add(Cosmetic);
                        Cosmetic.SetActive(false);
                    }
                }
            }
            catch { }
        }

        public static void EnableCosmetics()
        {
            foreach (GameObject c in cosmetics)
                c.SetActive(true);
            
            cosmetics.Clear();
        }

        public static void NoSmoothRigs()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    vrrig.lerpValueBody = 2f;
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
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK") || vrrig.concatStringOfCosmeticsAllowed.Contains("LMAPY."))
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
                        if (GetIndex("Hidden on Camera").enabled) { line.layer = 19; }
                        LineRenderer liner = line.AddComponent<LineRenderer>();
                        if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        liner.startColor = thecolor; liner.endColor = thecolor; liner.startWidth = 0.025f; liner.endWidth = 0.025f; liner.positionCount = 2; liner.useWorldSpace = true;
                        liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
                        liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
                        liner.material.shader = Shader.Find("GUI/Text Shader");
                        UnityEngine.Object.Destroy(line, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        private static Texture2D voicetxt = null;
        public static void VoiceESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                    {
                        size = recorder.Loudness * 3f;
                    }
                    if (size > 0f)
                    {
                        GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                        UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        volIndicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        if (voicetxt == null)
                            voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");
                        volIndicator.GetComponent<Renderer>().material.mainTexture = voicetxt;
                        volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f);
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f;
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                }
            }
        }

        // Credits to zvbex for the 'FIRST LOGIN' concat check-+
        private static Material platformMat;
        private static Texture2D steamtxt;
        private static Texture2D oculustxt;
        public static void PlatformIndicators()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                    UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    volIndicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    if (platformMat == null)
                    {
                        platformMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                        platformMat.SetFloat("_Surface", 1);
                        platformMat.SetFloat("_Blend", 0);
                        platformMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        platformMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        platformMat.SetFloat("_ZWrite", 0);
                        platformMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        platformMat.renderQueue = (int)RenderQueue.Transparent;

                        platformMat.SetFloat("_Glossiness", 0f);
                        platformMat.SetFloat("_Metallic", 0f);
                    }
                    volIndicator.GetComponent<Renderer>().material = platformMat;

                    if (steamtxt == null)
                        steamtxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/oculus.png", "oculus.png");

                    if (oculustxt == null)
                        oculustxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/steam.png", "steam.png");

                    volIndicator.GetComponent<Renderer>().material.mainTexture = vrrig.concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") ? oculustxt : steamtxt;
                    volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;

                    volIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                    volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f;
                    volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void PlatformESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                    UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    volIndicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                    if (steamtxt == null)
                        steamtxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/oculus.png", "oculus.png");

                    if (oculustxt == null)
                        oculustxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/steam.png", "steam.png");

                    volIndicator.GetComponent<Renderer>().material.mainTexture = vrrig.concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") ? oculustxt : steamtxt;
                    volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;
                        
                    volIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                    volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f;
                    volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        private static Material voiceMat = null;
        public static void VoiceIndicators()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                    {
                        size = recorder.Loudness * 3f;
                    }
                    if (size > 0f)
                    {
                        GameObject volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                        UnityEngine.Object.Destroy(volIndicator, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        if (voiceMat == null)
                        {
                            voiceMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                            if (voicetxt == null)
                            {
                                voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");
                            }
                            voiceMat.mainTexture = voicetxt;

                            voiceMat.SetFloat("_Surface", 1);
                            voiceMat.SetFloat("_Blend", 0);
                            voiceMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                            voiceMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                            voiceMat.SetFloat("_ZWrite", 0);
                            voiceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            voiceMat.renderQueue = (int)RenderQueue.Transparent;

                            voiceMat.SetFloat("_Glossiness", 0f);
                            voiceMat.SetFloat("_Metallic", 0f);
                        }
                        volIndicator.GetComponent<Renderer>().material = voiceMat;
                        volIndicator.GetComponent<Renderer>().material.color = PlayerIsTagged(vrrig) ? (Color)new Color32(255, 111, 0, 255) : vrrig.playerColor;
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f);
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * 0.8f; ;
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
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
            UnityEngine.Object.Destroy(l.GetComponent<SphereCollider>());

            l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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

        public static void CasualBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                UnityEngine.Color thecolor = vrrig.playerColor;
                if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void InfectionBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                            if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
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
                            LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                            if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                            liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                            for (int i = 0; i < bones.Count<int>(); i += 2)
                            {
                                liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                                liner.startWidth = 0.025f;
                                liner.endWidth = 0.025f;

                                liner.startColor = thecolor;
                                liner.endColor = thecolor;

                                liner.material.shader = Shader.Find("GUI/Text Shader");

                                liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                                UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
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
                        LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                        if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                        liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        for (int i = 0; i < bones.Count<int>(); i += 2)
                        {
                            liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                            liner.startWidth = 0.025f;
                            liner.endWidth = 0.025f;

                            liner.startColor = thecolor;
                            liner.endColor = thecolor;

                            liner.material.shader = Shader.Find("GUI/Text Shader");

                            liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                            liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                            UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                        }
                    }
                }
            }
        }

        public static void HuntBoneESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetOrAddComponent<GorillaHuntManager>();
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                if (player == target)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    LineRenderer liner = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                    if (GetIndex("Hidden on Camera").enabled) { liner.gameObject.layer = 19; }
                    liner.startWidth = 0.025f;
                    liner.endWidth = 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.material.shader = Shader.Find("GUI/Text Shader");

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    for (int i = 0; i < bones.Count<int>(); i += 2)
                    {
                        liner = vrrig.mainSkin.bones[bones[i]].gameObject.GetOrAddComponent<LineRenderer>();

                        liner.startWidth = 0.025f;
                        liner.endWidth = 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);

                        UnityEngine.Object.Destroy(liner, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        private static Dictionary<VRRig, float> delays = new Dictionary<VRRig, float> { };
        public static void FixRigMaterialESPColors(VRRig rig)
        {
            if ((delays.ContainsKey(rig) && Time.time > delays[rig]) || !delays.ContainsKey(rig))
            {
                if (delays.ContainsKey(rig))
                    delays[rig] = Time.time + 5f;
                else
                    delays.Add(rig, Time.time + 5f);

                rig.mainSkin.sharedMesh.colors32 = Enumerable.Repeat((Color32)Color.white, rig.mainSkin.sharedMesh.colors32.Length).ToArray();
                rig.mainSkin.sharedMesh.colors = Enumerable.Repeat(Color.white, rig.mainSkin.sharedMesh.colors.Length).ToArray();
            }
        }

        public static void CasualChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    FixRigMaterialESPColors(vrrig);

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
                            FixRigMaterialESPColors(vrrig);

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
                            FixRigMaterialESPColors(vrrig);

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
                        FixRigMaterialESPColors(vrrig);

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
                    FixRigMaterialESPColors(vrrig);

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

        public static void CasualBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    UnityEngine.Color thecolor = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { thecolor = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void InfectionBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                            if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
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
                            if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                            box.transform.position = vrrig.transform.position;
                            UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                            box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                            box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                            box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            box.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
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
                        if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        box.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                    }
                }
            }
        }

        public static void HuntBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
                if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                {
                    UnityEngine.Color thecolor = Color.red;
                    if (GetIndex("Transparent Theme").enabled) { thecolor.a = 0.5f; }
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { box.layer = 19; }
                    box.transform.position = vrrig.transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);
                }
            }
        }

        public static void CasualHollowBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        public static void HollowInfectionBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

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
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                            outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                            outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                            UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                            outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                            outl.transform.rotation = box.transform.rotation;
                            outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            outl.GetComponent<Renderer>().material.color = thecolor;
                            UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

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
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                        outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                        outl.transform.rotation = box.transform.rotation;
                        outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        outl.GetComponent<Renderer>().material.color = thecolor;
                        UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                        UnityEngine.Object.Destroy(box);
                    }
                }
            }
        }

        public static void HollowHuntBoxESP()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return;
                }
                else
                { PerformanceVisualDelay = Time.time + PerformanceModeStep; DelayChangeStep = Time.frameCount; }
            }

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
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

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
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.5f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.up * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.55f, 0.05f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * 0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (GetIndex("Hidden on Camera").enabled) { outl.layer = 19; }
                    outl.transform.position = vrrig.transform.position + (box.transform.right * -0.25f);
                    UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                    outl.transform.localScale = new Vector3(0.05f, 0.55f, 0f);
                    outl.transform.rotation = box.transform.rotation;
                    outl.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    outl.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(outl, PerformanceVisuals ? PerformanceModeStep : Time.deltaTime);

                    UnityEngine.Object.Destroy(box);
                }
            }
        }

        //these are actually fine because they have a delayed destroy
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
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
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
                            if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
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
                            if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
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
                        if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
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
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
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
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (GetIndex("Hidden on Camera").enabled) { sphere.layer = 19; }
                    UnityEngine.Object.Destroy(sphere.GetComponent<SphereCollider>());
                    sphere.GetComponent<Renderer>().material.color = thecolor;
                    sphere.transform.position = vrrig.transform.position;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    UnityEngine.Object.Destroy(sphere, 10f);
                }
            }
        }

        // Thanks DrPerky for rewriting visual mods <@427495360517111809>
        private static bool DoPerformanceCheck()
        {
            if (PerformanceVisuals)
            {
                if (Time.time < PerformanceVisualDelay)
                {
                    if (Time.frameCount != DelayChangeStep)
                        return true;
                }
                else
                {
                    PerformanceVisualDelay = Time.time + PerformanceModeStep;
                    DelayChangeStep = Time.frameCount;
                }
            }

            return false;
        }

        static Color infectedColor = new Color32(255, 111, 0, 255);

        static GameObject LeftSphere = null;
        static GameObject RightSphere = null;
        public static void ShowButtonColliders()
        {
            if (DoPerformanceCheck())
                return;

            if (LeftSphere == null || RightSphere == null)
            {
                if (LeftSphere == null)
                {
                    LeftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(LeftSphere.GetComponent<SphereCollider>());

                    LeftSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    LeftSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (RightSphere == null)
                {
                    RightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(RightSphere.GetComponent<SphereCollider>());

                    RightSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    RightSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
            }

            LeftSphere.GetComponent<Renderer>().material.color = bgColorA;
            LeftSphere.transform.localPosition = pointerOffset;

            RightSphere.GetComponent<Renderer>().material.color = bgColorA;
            RightSphere.transform.localPosition = pointerOffset;
        }

        public static void HideButtonColliders()
        {
            UnityEngine.GameObject.Destroy(LeftSphere);
            UnityEngine.GameObject.Destroy(RightSphere);

            LeftSphere = null;
            RightSphere = null;
        }

        // Tracers
        public static void CasualTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;

            Color menuColor = GetBDColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }

        public static void InfectionTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;

            bool LocalTagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
            bool NoInfected = InfectedList().Count == 0;

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                Color lineColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (PlayerIsTagged(playerRig))
                            continue;
                    }
                    else
                    {
                        if (!PlayerIsTagged(playerRig))
                            continue;

                        lineColor = infectedColor;
                    }
                }

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (transparentTheme)
                {
                    lineColor.a = 0.5f;
                }

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, playerRig.transform.position);
            }
        }

        public static void HuntTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.Hunt)
                return;

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();

            if (sillyComputer == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;
            float lineWidth = GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f;

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                if (GetPlayerFromVRRig(playerRig) == currentTarget)
                {
                    Color lineColor = playerRig.playerColor;

                    LineRenderer line = GetLineRender(hiddenOnCamera);

                    if (transparentTheme)
                    {
                        lineColor.a = 0.5f;
                    }

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;
                    line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    line.SetPosition(1, playerRig.transform.position);
                }
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color lineColor = Color.red;

                    LineRenderer line = GetLineRender(hiddenOnCamera);

                    if (transparentTheme)
                    {
                        lineColor.a = 0.5f;
                    }

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = lineWidth;
                    line.endWidth = lineWidth;
                    line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    line.SetPosition(1, playerRig.transform.position);
                }
            }
        }

        // Beacons
        public static void CasualBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            Color menuColor = GetBDColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = 0.025f;
                line.endWidth = 0.025f;
                line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
            }
        }

        public static void InfectionBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            bool LocalTagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = GetBDColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                Color lineColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (PlayerIsTagged(playerRig))
                            continue;
                    }
                    else
                    {
                        if (!PlayerIsTagged(playerRig))
                            continue;

                        lineColor = infectedColor;
                    }
                }

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                line.startWidth = 0.025f;
                line.endWidth = 0.025f;
                line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
            }
        }

        public static void HuntBeacons()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.Hunt)
                return;

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();

            if (sillyComputer == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            Color menuColor = GetBDColor(0f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig)
                    continue;

                if (GetPlayerFromVRRig(playerRig) == currentTarget)
                {
                    Color lineColor = playerRig.playerColor;
                    LineRenderer line = GetLineRender(hiddenOnCamera);

                    if (followMenuTheme)
                        lineColor = menuColor;

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = 0.025f;
                    line.endWidth = 0.025f;
                    line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                    line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
                } 
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color lineColor = Color.red;

                    LineRenderer line = GetLineRender(hiddenOnCamera);

                    if (followMenuTheme)
                        lineColor = menuColor;

                    if (transparentTheme)
                        lineColor.a = 0.5f;

                    line.startColor = lineColor;
                    line.endColor = lineColor;
                    line.startWidth = 0.025f;
                    line.endWidth = 0.025f;
                    line.SetPosition(0, playerRig.transform.position + new Vector3(0f, 9999f, 0f));
                    line.SetPosition(1, playerRig.transform.position - new Vector3(0f, 9999f, 0f));
                }
            }
        }

        // Distance ESP
        public static void CasualDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            Color menuColor = GetBDColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig) // Skip local player
                    continue;

                Color tagColor = followMenuTheme ? titleColor : Color.white;
                Color backgroundColor = playerRig.playerColor;

                if (followMenuTheme)
                    backgroundColor = menuColor;

                if (transparentTheme)
                {
                    backgroundColor.a = 0.5f;
                    tagColor.a = 0.5f;
                }

                TextMesh nameTagText = GetNameTag(hiddenOnCamera);

                nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                nameTagText.color = tagColor;

                string finalString = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, playerRig.transform.position));

                foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) //background color
                {
                    if (transform.gameObject.name == "bg")
                    {
                        transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                        transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    }
                    else if (transform.gameObject.name == "oh") // Outline holder
                    {
                        nameTagText.GetComponent<TextMesh>().text = finalString;

                        foreach (Transform secondTransform in transform.gameObject.GetComponentsInChildren<Transform>())
                        {
                            if (secondTransform.parent == transform)
                            {
                                secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
                            }
                        }
                    }
                }
            }
        }

        public static void InfectionDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            bool LocalTagged = PlayerIsTagged(GorillaTagger.Instance.offlineVRRig);
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = GetBDColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig) //skip local player
                    continue;

                Color tagColor = followMenuTheme ? titleColor : Color.white;
                Color backgroundColor = playerRig.playerColor;

                if (!NoInfected)
                {
                    if (LocalTagged)
                    {
                        if (PlayerIsTagged(playerRig))
                            continue;
                    }
                    else
                    {
                        if (!PlayerIsTagged(playerRig))
                            continue;

                        backgroundColor = infectedColor;
                    }
                }

                if (followMenuTheme)
                    backgroundColor = menuColor;

                if (transparentTheme)
                {
                    backgroundColor.a = 0.5f;
                    tagColor.a = 0.5f;
                }

                TextMesh nameTagText = GetNameTag(hiddenOnCamera);

                nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                nameTagText.color = tagColor;

                string finalString = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, playerRig.transform.position));

                foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) //background color
                {
                    if (transform.gameObject.name == "bg")
                    {
                        transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                        transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                    }
                    else if (transform.gameObject.name == "oh") //outline holder
                    {
                        nameTagText.GetComponent<TextMesh>().text = finalString;

                        foreach (Transform secondTransform in transform.gameObject.GetComponentsInChildren<Transform>())
                        {
                            if (secondTransform.parent == transform)
                            {
                                secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
                            }
                        }
                    }
                }
            }

        }

        public static void HuntDistanceESP()
        {
            if (DoPerformanceCheck())
                return;

            // Sanity checks, dont remove these

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.Hunt)
                return;

            GorillaHuntManager sillyComputer = GorillaGameManager.instance.gameObject.GetComponent<GorillaHuntManager>();
            
            if (sillyComputer == null)
                return;

            // Cache these here so your not finding the values from GetIndex every call (GetIndex is fucking slow)
            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            Color menuColor = GetBDColor(0f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            // Color bgColor = GetBGColor(0f); //dont need to call this function twice, just use a variable

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig == GorillaTagger.Instance.offlineVRRig) // Skip local player
                    continue;

                if (GetPlayerFromVRRig(playerRig) == currentTarget) // Use ID for quick comparison
                {
                    Color tagColor = followMenuTheme ? titleColor : Color.white;
                    Color backgroundColor = playerRig.playerColor;

                    if (followMenuTheme)
                        backgroundColor = menuColor;

                    if (transparentTheme)
                    {
                        backgroundColor.a = 0.5f;
                        tagColor.a = 0.5f;
                    }

                    TextMesh nameTagText = GetNameTag(hiddenOnCamera);

                    nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    nameTagText.color = tagColor;

                    string finalString = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, playerRig.transform.position));

                    foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) // Background color
                    {
                        if (transform.gameObject.name == "bg")
                        {
                            transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                            transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        }
                        else if (transform.gameObject.name == "oh") // Outline holder
                        {
                            nameTagText.GetComponent<TextMesh>().text = finalString;

                            foreach (Transform secondTransform in transform.gameObject.GetComponentsInChildren<Transform>())
                            {
                                if (secondTransform.parent == transform)
                                {
                                    secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
                                }
                            }
                        }
                    }
                } 
                else if (sillyComputer.IsTargetOf(GetPlayerFromVRRig(playerRig), PhotonNetwork.LocalPlayer))
                {
                    Color tagColor = followMenuTheme ? titleColor : Color.white;
                    Color backgroundColor = Color.red;

                    if (followMenuTheme)
                        backgroundColor = menuColor;

                    if (transparentTheme)
                    {
                        backgroundColor.a = 0.5f;
                        tagColor.a = 0.5f;
                    }

                    TextMesh nameTagText = GetNameTag(hiddenOnCamera);

                    nameTagText.gameObject.transform.position = playerRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    nameTagText.color = tagColor;

                    string finalString = string.Format("{0:F1}m", Vector3.Distance(Camera.main.transform.position, playerRig.transform.position));

                    foreach (Transform transform in nameTagText.gameObject.GetComponentsInChildren<Transform>()) // Background color
                    {
                        if (transform.gameObject.name == "bg")
                        {
                            transform.gameObject.GetComponent<Renderer>().material.color = backgroundColor;
                            transform.localScale = new Vector3(nameTagText.GetComponent<Renderer>().bounds.size.x + 0.2f, 0.2f, 0.01f);
                        }
                        else if (transform.gameObject.name == "oh") // Outline holder
                        {
                            nameTagText.GetComponent<TextMesh>().text = finalString;

                            foreach (Transform secondTransform in transform.gameObject.GetComponentsInChildren<Transform>())
                            {
                                if (secondTransform.parent == transform)
                                {
                                    secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Cache backend

        private static List<TextMesh> nameTagPool = new List<TextMesh>();

        private static GameObject nameTagHolder = null;

        public static bool isNameTagQueued = false;

        private static TextMesh GetNameTag(bool hideOnCamera)
        {
            if (nameTagHolder == null)
            {
                nameTagHolder = new GameObject("NameTag_Holder");
            }

            TextMesh finalTextMesh = null;

            foreach (TextMesh textMesh in nameTagPool)
            {
                if (finalTextMesh == null && !textMesh.gameObject.activeInHierarchy)
                {
                    textMesh.gameObject.SetActive(true);
                    textMesh.gameObject.transform.LookAt(Camera.main.transform.position);
                    textMesh.gameObject.transform.Rotate(0f, 180f, 0f);

                    textMesh.fontStyle = activeFontStyle;

                    // Update font style of outline here

                    finalTextMesh = textMesh;
                }
            }

            if (finalTextMesh == null)
            {
                GameObject MeshHolder = new GameObject("TextMeshObject");
                MeshHolder.transform.parent = nameTagHolder.transform;
                TextMesh newMesh = MeshHolder.AddComponent<TextMesh>();

                Renderer MeshRender = newMesh.GetComponent<Renderer>();

                newMesh.fontSize = 18;
                newMesh.fontStyle = activeFontStyle;
                newMesh.characterSize = 0.1f;
                newMesh.anchor = TextAnchor.MiddleCenter;
                newMesh.alignment = TextAlignment.Center;
                newMesh.color = Color.white;


                GameObject backgroundObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.GameObject.Destroy(backgroundObject.GetComponent<Collider>());

                Renderer backgroundRender = backgroundObject.GetComponent<Renderer>();

                backgroundObject.name = "bg";
                backgroundObject.transform.parent = MeshHolder.transform;
                backgroundObject.transform.localPosition = Vector3.zero;
                backgroundObject.transform.localScale = new Vector3(MeshRender.bounds.size.x + 0.2f, 0.2f, 0.01f);
                backgroundRender.material.shader = Shader.Find("GUI/Text Shader");
                backgroundRender.material.color = Color.white;
                MeshRender.material.renderQueue = backgroundRender.material.renderQueue + 2;

                // I could do this a better way but right now, I just couldnt care less

                GameObject outlineHolder = new GameObject("oh"); // Outline holder
                outlineHolder.transform.parent = MeshHolder.transform;

                GameObject textOutline1 = new GameObject("outline1");
                textOutline1.transform.parent = outlineHolder.transform;
                textOutline1.transform.localPosition = new Vector3(-0.01f, 0.01f, 0);

                TextMesh textOutline1_mesh = textOutline1.AddComponent<TextMesh>();
                textOutline1_mesh.GetComponent<Renderer>().material.renderQueue = backgroundRender.material.renderQueue + 1;
                textOutline1_mesh.fontSize = 18;
                textOutline1_mesh.fontStyle = activeFontStyle;
                textOutline1_mesh.characterSize = 0.1f;
                textOutline1_mesh.anchor = TextAnchor.MiddleCenter;
                textOutline1_mesh.alignment = TextAlignment.Center;
                textOutline1_mesh.color = Color.black;

                GameObject textOutline2 = new GameObject("outline2");
                textOutline2.transform.parent = outlineHolder.transform;
                textOutline2.transform.localPosition = new Vector3(0.01f, -0.01f, 0);

                TextMesh textOutline2_mesh = textOutline2.AddComponent<TextMesh>();
                textOutline2_mesh.GetComponent<Renderer>().material.renderQueue = backgroundRender.material.renderQueue + 1;
                textOutline2_mesh.fontSize = 18;
                textOutline2_mesh.fontStyle = activeFontStyle;
                textOutline2_mesh.characterSize = 0.1f;
                textOutline2_mesh.anchor = TextAnchor.MiddleCenter;
                textOutline2_mesh.alignment = TextAlignment.Center;
                textOutline2_mesh.color = Color.black;

                GameObject textOutline3 = new GameObject("outline3");
                textOutline3.transform.parent = outlineHolder.transform;
                textOutline3.transform.localPosition = new Vector3(-0.01f, -0.01f, 0);

                TextMesh textOutline3_mesh = textOutline3.AddComponent<TextMesh>();
                textOutline3_mesh.GetComponent<Renderer>().material.renderQueue = backgroundRender.material.renderQueue + 1;
                textOutline3_mesh.fontSize = 18;
                textOutline3_mesh.fontStyle = activeFontStyle;
                textOutline3_mesh.characterSize = 0.1f;
                textOutline3_mesh.anchor = TextAnchor.MiddleCenter;
                textOutline3_mesh.alignment = TextAlignment.Center;
                textOutline3_mesh.color = Color.black;

                GameObject textOutline4 = new GameObject("outline4");
                textOutline4.transform.parent = outlineHolder.transform;
                textOutline4.transform.localPosition = new Vector3(0.01f, 0.01f, 0);

                TextMesh textOutline4_mesh = textOutline4.AddComponent<TextMesh>();
                textOutline4_mesh.GetComponent<Renderer>().material.renderQueue = backgroundRender.material.renderQueue + 1;
                textOutline4_mesh.fontSize = 18;
                textOutline4_mesh.fontStyle = activeFontStyle;
                textOutline4_mesh.characterSize = 0.1f;
                textOutline4_mesh.anchor = TextAnchor.MiddleCenter;
                textOutline4_mesh.alignment = TextAlignment.Center;
                textOutline4_mesh.color = Color.black;

                nameTagPool.Add(newMesh);

                finalTextMesh = newMesh;
            }

            if (hideOnCamera)
                finalTextMesh.gameObject.layer = 19; // What does 19 actually do?
            else
                finalTextMesh.gameObject.layer = nameTagHolder.layer;

            return finalTextMesh;
        }

        public static void ClearNameTagPool(bool destroy = false) // Set destroy when you disable a feature that needs a lot of nameTags
        {
            if (DoPerformanceCheck())
                return;

            foreach (TextMesh textMesh in nameTagPool)
            {
                if (destroy || isNameTagQueued)
                    UnityEngine.Object.Destroy(textMesh.gameObject);
                else
                    textMesh.gameObject.SetActive(false);
            }

            if (destroy || isNameTagQueued)
                nameTagPool.Clear();

            isNameTagQueued = false;
        }


        private static List<LineRenderer> linePool = new List<LineRenderer>();

        private static GameObject lineRenderHolder = null;

        public static bool isLineRenderQueued = false;

        private static LineRenderer GetLineRender(bool hideOnCamera)
        {
            if (lineRenderHolder == null)
                lineRenderHolder = new GameObject("LineRender_Holder");

            LineRenderer finalRender = null;

            foreach (LineRenderer line in linePool)
            {
                if (finalRender != null) continue;

                if (!line.gameObject.activeInHierarchy)
                {
                    line.gameObject.SetActive(true);
                    finalRender = line;
                }
            }

            if (finalRender == null)
            {
                GameObject lineHolder = new GameObject("LineObject");
                lineHolder.transform.parent = lineRenderHolder.transform;
                LineRenderer newLine = lineHolder.AddComponent<LineRenderer>();
                newLine.material.shader = Shader.Find("GUI/Text Shader");
                newLine.startWidth = 0.025f;
                newLine.endWidth = 0.025f;
                newLine.positionCount = 2;
                newLine.useWorldSpace = true;

                linePool.Add(newLine);

                finalRender = newLine;
            }

            if (hideOnCamera)
                finalRender.gameObject.layer = 19;
            else
                finalRender.gameObject.layer = lineRenderHolder.layer;

            return finalRender;
        }

        public static void ClearLinePool(bool destroy = false) // Set destroy when you disable a feature that needs a lot of lines
        {
            if (DoPerformanceCheck())
                return;

            foreach (LineRenderer line in linePool)
            {
                if (destroy || isLineRenderQueued)
                    UnityEngine.Object.Destroy(line.gameObject);
                else
                    line.gameObject.SetActive(false);
            }

            if (destroy || isLineRenderQueued)
                linePool.Clear();
        }
    }
}
