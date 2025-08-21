using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using iiMenu.Classes;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Visuals
    {
        public static void WeatherChange(bool rain)
        {
            for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                BetterDayNightManager.instance.weatherCycle[i] = rain ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None;
        }

        private static bool previousFullbrightStatus;
        public static void SetFullbrightStatus(bool fullBright)
        {
            if (fullBright)
            {
                previousFullbrightStatus = GameLightingManager.instance.customVertexLightingEnabled;
                GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
            } else
            {
                if (previousFullbrightStatus)
                    GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
            }
        }

        private static float removeBlindfoldDelay;
        public static void RemoveBlindfold()
        {
            if (PhotonNetwork.InRoom && Time.time > removeBlindfoldDelay)
            {
                removeBlindfoldDelay = Time.time + 0.5f;
                GameObject mainCamera = GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/Main Camera");
                int childCount = mainCamera.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    GameObject v = mainCamera.transform.GetChild(i).gameObject;
                    if (v.name == "PropHaunt_Blindfold_ForCameras_Prefab(Clone)")
                        UnityEngine.Object.Destroy(v);
                }
            }
        }

        public static void WatchOn()
        {
            GameObject mainwatch = GetObject("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            regwatchobject = UnityEngine.Object.Instantiate(mainwatch, rightHand ? GetObject("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : GetObject("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);
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
            bool deafultWatch = false;
            string watchText = "";
            if (!infoWatchMenuName && !infoWatchTime && !infoWatchClip && !infoWatchFPS && !infoWatchCode)
                deafultWatch = true;

            UnityEngine.UI.Text watchTextComponent = regwatchText.GetComponent<UnityEngine.UI.Text>();

            if (infoWatchMenuName || deafultWatch) watchTextComponent.text = "ii's Stupid Menu\n<color=grey>";
            if (doCustomName && (infoWatchMenuName || deafultWatch))
                watchTextComponent.text = NoRichtextTags(customMenuName) + "\n<color=grey>";
            if (!infoWatchMenuName && !deafultWatch)
                watchTextComponent.text = "<color=grey>";
            
            if (infoWatchFPS || deafultWatch) watchText += lastDeltaTime.ToString() + " FPS\n";
            if (infoWatchTime || deafultWatch) watchText += DateTime.Now.ToString("hh:mm tt") + "\n";
            if (infoWatchCode) watchText += (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room") + "\n";
            if (infoWatchClip) watchText += "Clip: " + (GUIUtility.systemCopyBuffer.Length > 20 ? GUIUtility.systemCopyBuffer[..20] : GUIUtility.systemCopyBuffer) + "\n";

            watchText += "</color>";
            watchTextComponent.color = titleColor;
            watchTextComponent.text += watchText;
            if (lowercaseMode)
                watchTextComponent.text = watchTextComponent.text.ToLower();
            if (uppercaseMode)
                watchTextComponent.text = watchTextComponent.text.ToUpper();
        }

        public static void WatchOff() =>
            UnityEngine.Object.Destroy(regwatchobject);

        public static Material oldSkyMat = null;
        public static void DoCustomSkyboxColor()
        {
            GameObject sky = GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
            oldSkyMat = sky.GetComponent<Renderer>().material;
        }

        public static void CustomSkyboxColor() =>
            GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky").GetComponent<Renderer>().material = OrangeUI;

        public static void UnCustomSkyboxColor()
        {
            GameObject sky = GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
            sky.GetComponent<Renderer>().material = oldSkyMat;
        }

        public static TrailRenderer trailRenderer;
        public static void DrawGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun(GTPlayer.Instance.locomotionEnabledLayers);
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;
                
                if (trailRenderer == null)
                {
                    GameObject trailHolder = new GameObject("iiMenu_DrawGunTrail");

                    trailRenderer = trailHolder.AddComponent<TrailRenderer>();
                    trailRenderer.startWidth = 0.1f;
                    trailRenderer.endWidth = 0.1f;

                    trailRenderer.minVertexDistance = 0.05f;

                    trailRenderer.material.shader = Shader.Find("GUI/Text Shader");
                    trailRenderer.time = float.PositiveInfinity;
                    
                    trailRenderer.startColor = Color.black;
                    trailRenderer.endColor = Color.black;

                    if (smoothLines)
                    {
                        trailRenderer.numCapVertices = 10;
                        trailRenderer.numCornerVertices = 5;
                    }
                }

                trailRenderer.emitting = GetGunInput(true);
                trailRenderer.gameObject.transform.position = NewPointer.transform.position;
            }
        }

        public static void DisableDrawGun()
        {
            if (trailRenderer != null)
                UnityEngine.Object.Destroy(trailRenderer.gameObject);

            trailRenderer = null;
        }

        private static Material tapMat;
        private static Texture2D tapTxt;
        private static Texture2D warningTxt;

        private static List<object[]> handTaps = new List<object[]> { };
        public static void OnHandTapGamesenseRing(VRRig rig, Vector3 position)
        {
            if (rig.isLocal)
                return;

            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 20f)
                return;

            handTaps.Add(new object[]
            {
                rig,
                position,
                Time.time,
                null
            });
        }

        public static void GamesenseRing()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;

            List<object[]> toRemove = new List<object[]> { };
            for (int i = 0; i < handTaps.Count; i++)
            {
                object[] handTapData = handTaps[i];
                VRRig rig = (VRRig)handTapData[0];
                Vector3 position = (Vector3)handTapData[1];

                float timestamp = (float)handTapData[2];
                GameObject gameObject = (GameObject)handTapData[3] ?? null;

                if (Time.time > timestamp + 1f)
                {
                    toRemove.Add(handTapData);
                    continue;
                }

                if (gameObject == null)
                {
                    gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());

                    if (tapMat == null)
                    {
                        tapMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                        if (tapTxt == null)
                            tapTxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/footstep.png", "footstep.png");

                        if (warningTxt == null)
                            warningTxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/warning.png", "warning.png");

                        tapMat.SetFloat("_Surface", 1);
                        tapMat.SetFloat("_Blend", 0);
                        tapMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                        tapMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                        tapMat.SetFloat("_ZWrite", 0);
                        tapMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        tapMat.renderQueue = (int)RenderQueue.Transparent;
                    }

                    Color targetColor = GetPlayerColor(rig);

                    if (hoc)
                        gameObject.layer = 19;

                    if (fmt)
                        targetColor = GetBGColor(0f);
                    if (tt)
                        targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0.5f);


                    gameObject.GetComponent<Renderer>().material = tapMat;
                    gameObject.GetComponent<Renderer>().material.mainTexture = PlayerIsTagged(VRRig.LocalRig) ? (PlayerIsTagged(rig) ? tapTxt : warningTxt) : (PlayerIsTagged(rig) ? warningTxt : tapTxt);
                    gameObject.GetComponent<Renderer>().material.color = targetColor;

                    handTaps[i][3] = gameObject;
                }

                Renderer renderer = gameObject.GetComponent<Renderer>();

                Vector3 toTarget = position - Camera.main.transform.position;
                toTarget.Normalize();

                Vector3 camForward = Camera.main.transform.forward.normalized;
                Vector3 camRight = Camera.main.transform.right.normalized;
                Vector3 camUp = Camera.main.transform.up.normalized;

                float x = Vector3.Dot(toTarget, camRight);
                float y = Vector3.Dot(toTarget, camUp);
                float z = Vector3.Dot(toTarget, camForward);

                Vector2 dirInPlane = new Vector2(x, y).normalized;

                float ringRadius = 0.2f;
                Vector3 ringOffset = (camRight * dirInPlane.x + camUp * dirInPlane.y) * ringRadius;
                Vector3 ringCenter = Camera.main.transform.position + camForward * 0.5f;

                Vector3 finalPos = ringCenter + ringOffset;
                gameObject.transform.position = finalPos;

                gameObject.transform.rotation = Quaternion.LookRotation(finalPos - Camera.main.transform.position, -Camera.main.transform.up);
                Vector3 forwardFlat = Camera.main.transform.forward;
                forwardFlat.y = 0;

                float t = Mathf.Lerp(1f, 0f, Time.time - timestamp);

                Color color = renderer.material.color;
                color.a = Mathf.Clamp01(t);
                renderer.material.color = color;

                gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f);
            }

            foreach (object[] removal in toRemove)
            {
                handTaps.Remove(removal);
                if ((GameObject)removal[3] != null)
                    UnityEngine.Object.Destroy((GameObject)removal[3]);
            }
        }

        public static void DisableGamesenseRing()
        {
            Patches.HandTapPatch.OnHandTap -= OnHandTapGamesenseRing;

            foreach (object[] handTapData in handTaps)
            {
                VRRig rig = (VRRig)handTapData[0];
                Vector3 position = (Vector3)handTapData[1];

                float timestamp = (float)handTapData[2];
                GameObject gameObject = (GameObject)handTapData[3] ?? null;

                if (gameObject != null)
                    UnityEngine.Object.Destroy(gameObject);
            }

            handTaps.Clear();
        }

        public static bool PerformanceVisuals;

        public static float PerformanceModeStep = 0.2f;
        public static int PerformanceModeStepIndex = 2;
        public static void ChangePerformanceModeVisualStep(bool positive = true)
        {
            if (positive)
                PerformanceModeStepIndex++;
            else
                PerformanceModeStepIndex--;

            PerformanceModeStepIndex %= 11;
            if (PerformanceModeStepIndex < 0)
                PerformanceModeStepIndex = 10;

            PerformanceModeStep = PerformanceModeStepIndex / 10f;
            GetIndex("Change Performance Visuals Step").overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>" + PerformanceModeStep.ToString() + "</color><color=grey>]</color>";
        }

        public static float PerformanceVisualDelay;
        public static int DelayChangeStep;

        public static Dictionary<string, GameObject> labelDictionary = new Dictionary<string, GameObject> { };
        public static Dictionary<bool, List<int>> labelDistances = new Dictionary<bool, List<int>> { };
        public static float GetLabelDistance(bool leftHand)
        {
            if (labelDistances[leftHand][0] == Time.frameCount)
            {
                labelDistances[leftHand].Add(Time.frameCount);
                return 0.1f + (labelDistances[leftHand].Count * 0.1f);
            }
            else
            {
                labelDistances[leftHand].Clear();
                labelDistances[leftHand].Add(Time.frameCount);
                return 0.1f + (labelDistances[leftHand].Count * 0.1f);
            }
        }

        public static void GetLabel(string codeName, bool leftHand, string text, Color color)
        {
            if (!labelDictionary.TryGetValue(codeName, out GameObject go))
            {
                go = new GameObject(codeName);
                if (GetIndex("Hidden Labels").enabled)
                    go.layer = 19;

                go.transform.localScale = Vector3.one * 0.25f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

                labelDictionary.Add(codeName, go);
            }

            go.SetActive(true);
            
            TextMesh textMesh = go.GetOrAddComponent<TextMesh>();
            textMesh.color = color;
            textMesh.fontSize = 24;
            textMesh.fontStyle = activeFontStyle;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.text = text;

            go.transform.position = (leftHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform).position + Vector3.up * (GetLabelDistance(leftHand) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
            go.transform.LookAt(Camera.main.transform.position);
            go.transform.Rotate(0f, 180f, 0f);
        }

        public static void VelocityLabel()
        {
            if (DoPerformanceCheck())
                return;

            GetLabel
            (
                "Velocity",
                false,
                string.Format("{0:F1}m/s", GorillaTagger.Instance.rigidbody.velocity.magnitude),
                GorillaTagger.Instance.rigidbody.velocity.magnitude >= GTPlayer.Instance.maxJumpSpeed ? Color.green : Color.white
            );
        }

        private static float startTime = 0f;
        private static float endTime = 0f;
        private static bool lastWasTagged = false;
        public static void TimeLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = InfectedList().Count > 0;

                if (isThereTagged)
                {
                    bool playerIsTagged = PlayerIsTagged(VRRig.LocalRig);
                    if (playerIsTagged && !lastWasTagged)
                        endTime = Time.time - startTime;

                    if (!playerIsTagged && lastWasTagged)
                        startTime = Time.time;
                    
                    lastWasTagged = playerIsTagged;

                    GetLabel
                    (
                        "Time",
                        false,
                        !playerIsTagged ?
                        FormatUnix(Mathf.FloorToInt(Time.time - startTime)) :
                        FormatUnix(Mathf.FloorToInt(endTime)),
                        playerIsTagged ? Color.green : Color.white
                    );
                }
                else
                    startTime = Time.time;
            }
        }

        public static void NearbyTaggerLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (!PlayerIsTagged(VRRig.LocalRig))
            {
                float closest = float.MaxValue;
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (!vrrig.isLocal && PlayerIsTagged(vrrig))
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
                        colorn = Color.yellow;
                    
                    if (closest < 20f)
                        colorn = new Color32(255, 90, 0, 255);
                    
                    if (closest < 10f)
                        colorn = Color.red;

                    GetLabel
                    (
                        "NearbyTagger",
                        false,
                        string.Format("{0:F1}m", closest),
                        colorn
                    );
                }
            }
        }

        public static void LastLabel()
        {
            if (DoPerformanceCheck())
                return;

            if (PhotonNetwork.InRoom)
            {
                bool isThereTagged = InfectedList().Count > 0;
                int left = PhotonNetwork.PlayerList.Length - InfectedList().Count;

                if (isThereTagged)
                {
                    GetLabel
                    (
                        "LastLabel",
                        false,
                        left.ToString() + " left",
                        left <= 1 && !PlayerIsTagged(VRRig.LocalRig) ? Color.green : Color.white
                    );
                }
            }
        }

        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

        private static GameObject visualizerObject;
        private static GameObject visualizerOutline;
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

            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var Ray, 512f, GTPlayer.Instance.locomotionEnabledLayers);
            visualizerObject.transform.position = Ray.point;
            visualizerObject.transform.rotation = Quaternion.LookRotation(Ray.normal) * Quaternion.Euler(90f, 0f, 0f);

            float size = 0f;
            GorillaSpeakerLoudness recorder = VRRig.LocalRig.GetComponent<GorillaSpeakerLoudness>();
            if (recorder != null)
                size = recorder.Loudness;

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

        private static GameObject headPos;
        private static GameObject leftHandPos;
        private static GameObject rightHandPos;
        public static void ShowServerPosition()
        {
            if (headPos == null)
            {
                headPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(headPos.GetComponent<Collider>());
                headPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }

            if (leftHandPos == null)
            {
                leftHandPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(leftHandPos.GetComponent<Collider>());
                leftHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            if (rightHandPos == null)
            {
                rightHandPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(rightHandPos.GetComponent<Collider>());
                rightHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            headPos.transform.position = ServerPos;
            leftHandPos.transform.position = ServerLeftHandPos;
            rightHandPos.transform.position = ServerRightHandPos;
        }

        public static void DisableShowServerPosition()
        {
            if (headPos != null)
                UnityEngine.Object.Destroy(headPos);

            if (leftHandPos != null)
                UnityEngine.Object.Destroy(leftHandPos);

            if (rightHandPos != null)
                UnityEngine.Object.Destroy(rightHandPos);
        }

        private static Dictionary<VRRig, LineRenderer> predictions = new Dictionary<VRRig, LineRenderer> { };
        public static void JumpPredictions()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, LineRenderer> lines in predictions)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value.gameObject);
                }
            }

            foreach (VRRig rig in toRemove)
                predictions.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!predictions.TryGetValue(rig, out LineRenderer Line))
                {
                    GameObject LineObject = new GameObject("LineObject");
                    Line = LineObject.AddComponent<LineRenderer>();
                    if (smoothLines)
                    {
                        Line.numCapVertices = 10;
                        Line.numCornerVertices = 5;
                    }
                    Line.material.shader = Shader.Find("GUI/Text Shader");
                    Line.startWidth = 0.025f;
                    Line.endWidth = 0.025f;
                    Line.positionCount = 25;
                    Line.useWorldSpace = true;
                    predictions.Add(rig, Line);
                }

                if (hoc) 
                    Line.gameObject.layer = 19;

                Color color = GetPlayerColor(rig);

                if (fmt) 
                    color = GetBGColor(0f);
                if (tt) 
                    color = new Color(color.r, color.g, color.b, 0.5f);

                float width = thinTracers ? 0.0075f : 0.025f;
                Line.startWidth = width;
                Line.endWidth = width;

                Line.startColor = color;
                Line.endColor = color;

                Vector3 position = rig.syncPos;
                Vector3 velocity = rig.LatestVelocity();

                if (velocity.magnitude < 0.5f)
                {
                    Line.enabled = false;
                    continue;
                }

                Line.enabled = true;

                int stepCount = Mathf.Min(25, Mathf.CeilToInt(5f / 0.1f));
                Vector3[] points = new Vector3[stepCount];

                int i;
                for (i = 0; i < stepCount; i++)
                {
                    points[i] = position;

                    Vector3 nextVelocity = velocity + Physics.gravity * 0.1f;
                    Vector3 nextPosition = position + velocity * 0.1f;

                    if (Physics.Raycast(position, nextPosition - position, out RaycastHit hit, (nextPosition - position).magnitude, GTPlayer.Instance.locomotionEnabledLayers))
                    {
                        points[i] = hit.point;
                        i++;
                        break;
                    }

                    position = nextPosition;
                    velocity = nextVelocity;
                }

                Vector3[] finalPoints = new Vector3[i];
                for (int j = 0; j < i; j++)
                    finalPoints[j] = points[j];

                Line.positionCount = finalPoints.Length;
                Line.SetPositions(finalPoints);
            }
        }

        public static void DisableJumpPredictions()
        {
            foreach (KeyValuePair<VRRig, LineRenderer> pred in predictions)
                UnityEngine.Object.Destroy(pred.Value.gameObject);

            predictions.Clear();
        }

        public static void VisualizeNetworkTriggers()
        {
            GameObject triggers = GetObject("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                        VisualizeCube(child.position, child.rotation, child.localScale, Color.red);
                } catch { }
            }
        }

        public static void VisualizeMapTriggers()
        {
            GameObject triggers = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab");
            for (int i = 0; i < triggers.transform.childCount; i++)
            {
                try
                {
                    Transform child = triggers.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                        VisualizeCube(child.position, child.rotation, child.localScale, GetBGColor(0f));
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
                    return (0.25f + (ntDistanceList[rig].Count * 0.15f)) * rig.scaleFactor;
                } else
                {
                    ntDistanceList[rig].Clear();
                    ntDistanceList[rig].Add(Time.frameCount);
                    return (0.25f + (ntDistanceList[rig].Count * 0.15f)) * rig.scaleFactor;
                }
            } else
            {
                ntDistanceList.Add(rig, new List<int> { Time.frameCount });
                return 0.4f * rig.scaleFactor;
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
                if (!vrrig.isLocal)
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
                    nameTag.GetComponent<TextMesh>().text = CleanPlayerName(GetPlayerFromVRRig(vrrig).NickName);
                    nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                    nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                    nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

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
                    if (!vrrig.isLocal)
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
                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

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
                    if (!vrrig.isLocal)
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
                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

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

        private static Dictionary<VRRig, GameObject> Pingnametags = new Dictionary<VRRig, GameObject> { };
        public static void PingTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in Pingnametags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    Pingnametags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal)
                    {
                        if (!Pingnametags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Pingtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            Pingnametags.Add(vrrig, go);
                        }

                        GameObject nameTag = Pingnametags[vrrig];
                        nameTag.GetComponent<TextMesh>().text = $"{playerPing[vrrig]}ms";
                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisablePingTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in Pingnametags)
                UnityEngine.Object.Destroy(nametag.Value);

            Pingnametags.Clear();
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
                    if (!vrrig.isLocal)
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
                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

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
                    if (!vrrig.isLocal)
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
                            nameTag.GetComponent<TextMesh>().text = "";
                            
                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

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

        public static Dictionary<string, string> modDictionary = new Dictionary<string, string> {
            { "genesis", "Genesis" },
            { "HP_Left", "Holdable Pad" },
            { "GrateVersion", "Grate" },
            { "void", "Void" },
            { "BANANAOS", "Banana OS" },
            { "GC", "Gorilla Craft" },
            { "CarName", "Gorilla Vehicles" },
            { "6p72ly3j85pau2g9mda6ib8px", "CCM V2" },
            { "FPS-Nametags for Zlothy", "FPS Tags" },
            { "cronos", "Cronos" },
            { "ORBIT", "Orbit" },
            { "Violet On Top", "Violet" },
            { "MP25", "Monke Phone" },
            { "GorillaWatch", "Gorilla Watch" },
            { "InfoWatch", "Gorilla Info Watch" },
            { "BananaPhone", "Banana Phone" },
            { "Vivid", "Vivid" },
            { "RGBA", "Custom Cosmetics" },
            { "cheese is gouda", "Whos Icheating" },
            { "shirtversion", "Gorilla Shirts" },
            { "gpronouns", "Gorilla Pronouns" },
            { "gfaces", "Gorilla Faces" },
            { "monkephone", "Monke Phone" },
            { "pmversion", "Player Models" },
            { "gtrials", "Gorilla Trials" },
            { "msp", "Monke Smartphone" },
            { "gorillastats", "Gorilla Stats" },
            { "using gorilladrift", "Gorilla Drift" },
            { "monkehavocversion", "Monke Havoc" },
            { "tictactoe", "Tic Tac Toe" },
            { "ccolor", "Index" },
            { "imposter", "Gorilla Among Us" },
            { "spectapeversion", "Spec Tape" },
            { "cats", "Cats" },
            { "made by biotest05 :3", "Dogs" },
            { "fys cool magic mod", "Fys Magic Mod" },
            { "colour", "Custom Cosmetics" },
            { "chainedtogether", "Chained Together" },
            { "goofywalkversion", "Goofy Walk" },
            { "void_menu_open", "Void" },
            { "violetpaiduser", "Violet Paid" },
            { "violetfree", "Violet Free" },
            { "obsidianmc", "Obsidian.Lol" },
            { "dark", "Shiba GT Dark" },
            { "hidden menu", "Hidden" },
            { "oblivionuser", "Oblivion" },
            { "hgrehngio889584739_hugb\n", "Resurgence" },
            { "eyerock reborn", "Eye Rock" },
            { "asteroidlite", "Asteroid Lite" },
            { "elux", "Elux" },
            { "cokecosmetics", "Coke Cosmetx" },
            { "GFaces", "G Faces" },
            { "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
            { "ObsidianMC", "Obsidian" },
            { "hgrehngio889584739_hugb", "Resurgence" },
            { "GTrials", "G Trials" },
            { "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
            { "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info" },
            { "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils IW" },
            { "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
            { "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
            { "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
            { "MediaPad", "Media Pad" },
            { "GorillaCinema", "Gorilla Cinema" },
            { "ChainedTogetherActive", "Chained Together" },
            { "GPronouns", "G Pronouns" },
            { "CSVersion", "Custom Skin" },
            { "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zloth Rec Room Rig" },
            { "ShirtProperties", "Shirts Old" },
            { "GorillaShirts", "Shirts" },
            { "GS", "Old Shirts" },
            { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCM V2" },
            { "Body Tracking", "Body Track Old" },
            { "Body Estimation", "Han Body Est" },
            { "Gorilla Track", "Body Track" },
            { "CustomMaterial", "Custom Cosmetics" },
            { "I like cheese", "Rec Room Rig" },
            { "silliness", "Silliness" },
        };

        private static Dictionary<VRRig, GameObject> modNameTags = new Dictionary<VRRig, GameObject> { };
        public static void ModTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in modNameTags)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    modNameTags.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (!vrrig.isLocal)
                    {
                        if (!modNameTags.ContainsKey(vrrig))
                        {
                            GameObject go = new GameObject("iiMenu_Modtag");
                            go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                            TextMesh textMesh = go.AddComponent<TextMesh>();
                            textMesh.fontSize = 48;
                            textMesh.characterSize = 0.1f;
                            textMesh.anchor = TextAnchor.MiddleCenter;
                            textMesh.alignment = TextAlignment.Center;

                            modNameTags.Add(vrrig, go);
                        }

                        string specialMods = "";
                        Dictionary<string, object> customProps = new Dictionary<string, object>();
                        foreach (DictionaryEntry dictionaryEntry in NetPlayerToPlayer(GetPlayerFromVRRig(vrrig)).CustomProperties)
                            customProps[dictionaryEntry.Key.ToString().ToLower()] = dictionaryEntry.Value;

                        foreach (KeyValuePair<string, string> mod in modDictionary)
                        {
                            if (customProps.ContainsKey(mod.Key.ToLower()))
                            {
                                if (specialMods == null)
                                    specialMods = mod.Value;
                                else
                                {
                                    if (specialMods.Contains("&"))
                                        specialMods = mod.Value + ", " + specialMods;
                                    else
                                        specialMods += " & " + mod.Value;
                                }
                            }
                        }

                        CosmeticsController.CosmeticSet cosmeticSet = vrrig.cosmeticSet;
                        foreach (CosmeticsController.CosmeticItem cosmetic in cosmeticSet.items)
                        {
                            if (!cosmetic.isNullItem && !vrrig.concatStringOfCosmeticsAllowed.Contains(cosmetic.itemName))
                            {
                                if (specialMods == null)
                                    specialMods = "Cosmetx";
                                else
                                {
                                    if (specialMods.Contains("&"))
                                        specialMods = "Cosmetx, " + specialMods;
                                    else
                                        specialMods += " & Cosmetx";
                                }
                                break;
                            }
                        }

                        GameObject nameTag = modNameTags[vrrig];
                        nameTag.GetComponent<TextMesh>().text = "";

                        nameTag.GetComponent<TextMesh>().color = GetPlayerColor(vrrig);
                        nameTag.GetComponent<TextMesh>().fontStyle = activeFontStyle;

                        nameTag.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;

                        nameTag.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * GetTagDistance(vrrig);
                        nameTag.transform.LookAt(Camera.main.transform.position);
                        nameTag.transform.Rotate(0f, 180f, 0f);
                    }
                }
                catch { }
            }
        }

        public static void DisableModTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in modNameTags)
                UnityEngine.Object.Destroy(nametag.Value);

            modNameTags.Clear();
        }

        public static void FixRigColors()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                    vrrig.mainSkin.material.color = vrrig.playerColor;
            }
        }

        public static string _leavesName;
        public static string leavesName
        {
            get 
            {
                if (_leavesName == null)
                {
                    var matchingObjects = GetObject("Environment Objects/LocalObjects_Prefab/Forest")
                        .GetComponentsInChildren<Transform>(true)
                        .Where(t => t.name.StartsWith("UnityTempFile"))
                        .GroupBy(t => t.name)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault();

                    _leavesName = matchingObjects?.Key ?? "UnityTempFile";
                }

                return _leavesName;
            } 
        }

        public static List<GameObject> leaves = new List<GameObject> { };
        public static void EnableRemoveLeaves()
        {
            GameObject Forest = GetObject("Environment Objects/LocalObjects_Prefab/Forest");
            if (Forest != null)
            {
                for (int i = 0; i < Forest.transform.childCount; i++)
                {
                    GameObject v = Forest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(leavesName))
                    {
                        v.SetActive(false);
                        leaves.Add(v);
                    }
                }
            }

            GameObject RankedForest = GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
            if (RankedForest != null)
            {
                for (int i = 0; i < RankedForest.transform.childCount; i++)
                {
                    GameObject v = RankedForest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(leavesName))
                    {
                        v.SetActive(false);
                        leaves.Add(v);
                    }
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
            GameObject Forest = GetObject("Environment Objects/LocalObjects_Prefab/Forest");
            if (Forest != null)
            {
                for (int i = 0; i < Forest.transform.childCount; i++)
                {
                    GameObject v = Forest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(leavesName))
                    {
                        v.layer = 16;
                        leaves.Add(v);
                    }
                }
            }

            GameObject RankedForest = GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
            if (RankedForest != null)
            {
                for (int i = 0; i < RankedForest.transform.childCount; i++)
                {
                    GameObject v = RankedForest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(leavesName))
                    {
                        v.layer = 16;
                        leaves.Add(v);
                    }
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
                foreach (GameObject Cosmetic in VRRig.LocalRig.cosmetics)
                {
                    if (Cosmetic.activeSelf && Cosmetic.transform.parent == VRRig.LocalRig.mainCamera.transform.Find("HeadCosmetics"))
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
                if (!vrrig.isLocal)
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
                if (!vrrig.isLocal)
                {
                    vrrig.lerpValueBody = VRRig.LocalRig.lerpValueBody;
                    vrrig.lerpValueFingers = VRRig.LocalRig.lerpValueFingers;
                }
            }
        }

        public static void BetterRigLerping(VRRig rig)
        {
            rig.LocalTrajectoryOverrideBlend = 1f;
            rig.LocalTrajectoryOverridePosition = rig.syncPos;
            rig.LocalTrajectoryOverrideVelocity = rig.LatestVelocity();
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
                if (!vrrig.isLocal)
                {
                    bool showtracersplz = false;
                    Color thecolor = vrrig.playerColor;
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBADE") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAGS"))
                    {
                        thecolor = Color.green;
                        showtracersplz = true;
                    }
                    if (vrrig.concatStringOfCosmeticsAllowed.Contains("LBANI"))
                    {
                        thecolor = Color.magenta;
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
                        if (smoothLines)
                        {
                            liner.numCapVertices = 10;
                            liner.numCornerVertices = 5;
                        }
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

        // Credits to zvbex for the 'FIRST LOGIN' concat check
        // Credits to HanSolo1000Falcon/WhoIsThatMonke for improved checks

        private static Material platformMat;
        private static Material platformEspMat;
        private static Texture2D steamtxt;
        private static Texture2D oculustxt;
        private static Dictionary<VRRig, GameObject> platformIndicators = new Dictionary<VRRig, GameObject> { };

        public static bool IsPlayerSteam(VRRig Player)
        {
            string concat = Player.concatStringOfCosmeticsAllowed;
            int customPropsCount = NetPlayerToPlayer(GetPlayerFromVRRig(Player)).CustomProperties.Count;

            if (concat.Contains("S. FIRST LOGIN")) return true;
            if (concat.Contains("FIRST LOGIN") || customPropsCount >= 2) return true;
            if (concat.Contains("LMAKT.")) return false;

            return false;
        }

        public static void PlatformIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in platformIndicators)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    platformIndicators.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!platformIndicators.TryGetValue(vrrig, out GameObject indicator))
                    {
                        indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(indicator.GetComponent<Collider>());

                        if (platformMat == null)
                        {
                            platformMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                            platformMat.SetFloat("_Surface", 1);
                            platformMat.SetFloat("_Blend", 0);
                            platformMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                            platformMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                            platformMat.SetFloat("_ZWrite", 0);
                            platformMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            platformMat.renderQueue = (int)RenderQueue.Transparent;
                        }
                        indicator.GetComponent<Renderer>().material = platformMat;
                        platformIndicators.Add(vrrig, indicator);
                    }

                    if (steamtxt == null)
                        steamtxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/oculus.png", "oculus.png");

                    if (oculustxt == null)
                        oculustxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/steam.png", "steam.png");

                    indicator.GetComponent<Renderer>().material.mainTexture = IsPlayerSteam(vrrig) ? oculustxt : steamtxt;
                    indicator.GetComponent<Renderer>().material.color = GetPlayerColor(vrrig);

                    indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * vrrig.scaleFactor;
                    indicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (0.8f * vrrig.scaleFactor);
                    indicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void PlatformESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in platformIndicators)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    platformIndicators.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!platformIndicators.TryGetValue(vrrig, out GameObject indicator))
                    {
                        indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(indicator.GetComponent<Collider>());

                        indicator.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                        if (platformEspMat == null)
                            platformEspMat = new Material(Shader.Find("GUI/Text Shader"));
                        
                        indicator.GetComponent<Renderer>().material = platformEspMat;
                        platformIndicators.Add(vrrig, indicator);
                    }

                    if (steamtxt == null)
                        steamtxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/oculus.png", "oculus.png");

                    if (oculustxt == null)
                        oculustxt = LoadTextureFromURL("https://raw.githubusercontent.com/iiDk-the-actual/ModInfo/refs/heads/main/steam.png", "steam.png");

                    indicator.GetComponent<Renderer>().material.mainTexture = IsPlayerSteam(vrrig) ? oculustxt : steamtxt;
                    indicator.GetComponent<Renderer>().material.color = GetPlayerColor(vrrig);

                    indicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * vrrig.scaleFactor;
                    indicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (0.8f * vrrig.scaleFactor);
                    indicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void DisablePlatformIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in platformIndicators)
                UnityEngine.Object.Destroy(nametag.Value);

            platformIndicators.Clear();
        }

        private static Material voiceMat;
        private static Material voiceEspMat;
        private static Texture2D voicetxt;

        private static Dictionary<VRRig, GameObject> voiceIndicators = new Dictionary<VRRig, GameObject> { };
        public static void VoiceIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in voiceIndicators)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    voiceIndicators.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                        size = recorder.Loudness * 3f;
                    
                    if (size > 0f)
                    {
                        if (!voiceIndicators.TryGetValue(vrrig, out GameObject volIndicator))
                        {
                            volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());
                            
                            if (voiceMat == null)
                            {
                                voiceMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

                                if (voicetxt == null)
                                    voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");
                                
                                voiceMat.mainTexture = voicetxt;

                                voiceMat.SetFloat("_Surface", 1);
                                voiceMat.SetFloat("_Blend", 0);
                                voiceMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                                voiceMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                                voiceMat.SetFloat("_ZWrite", 0);
                                voiceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                voiceMat.renderQueue = (int)RenderQueue.Transparent;
                            }
                            volIndicator.GetComponent<Renderer>().material = voiceMat;
                            voiceIndicators.Add(vrrig, volIndicator);
                        }

                        volIndicator.GetComponent<Renderer>().material.color = GetPlayerColor(vrrig);
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f) * vrrig.scaleFactor;
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * (0.8f * vrrig.scaleFactor);
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    } else
                    {
                        if (voiceIndicators.TryGetValue(vrrig, out GameObject existing))
                        {
                            UnityEngine.Object.Destroy(existing);
                            voiceIndicators.Remove(vrrig);
                        }
                    }
                }
            }
        }

        public static void VoiceESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in voiceIndicators)
            {
                if (!GorillaParent.instance.vrrigs.Contains(nametag.Key))
                {
                    UnityEngine.Object.Destroy(nametag.Value);
                    voiceIndicators.Remove(nametag.Key);
                }
            }

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    float size = 0f;
                    GorillaSpeakerLoudness recorder = vrrig.GetComponent<GorillaSpeakerLoudness>();
                    if (recorder != null)
                        size = recorder.Loudness * 3f;

                    if (size > 0f)
                    {
                        if (!voiceIndicators.TryGetValue(vrrig, out GameObject volIndicator))
                        {
                            volIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            UnityEngine.Object.Destroy(volIndicator.GetComponent<Collider>());

                            if (voiceEspMat == null)
                                voiceEspMat = new Material(Shader.Find("GUI/Text Shader"));

                            if (voicetxt == null)
                                voicetxt = LoadTextureFromResource("iiMenu.Resources.speak.png");

                            voiceEspMat.mainTexture = voicetxt;

                            volIndicator.GetComponent<Renderer>().material = voiceEspMat;
                            voiceIndicators.Add(vrrig, volIndicator);
                        }

                        volIndicator.GetComponent<Renderer>().material.color = GetPlayerColor(vrrig);
                        volIndicator.transform.localScale = new Vector3(size, size, 0.01f) * vrrig.scaleFactor;
                        volIndicator.transform.position = vrrig.headMesh.transform.position + vrrig.headMesh.transform.up * ((ServerData.Administrators.ContainsKey(GetPlayerFromVRRig(vrrig).UserId) ? 1.3f : 0.8f) * vrrig.scaleFactor);
                        volIndicator.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    }
                    else
                    {
                        if (voiceIndicators.TryGetValue(vrrig, out GameObject existing))
                        {
                            UnityEngine.Object.Destroy(existing);
                            voiceIndicators.Remove(vrrig);
                        }
                    }
                }
            }
        }

        public static void DisableVoiceIndicators()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in voiceIndicators)
                UnityEngine.Object.Destroy(nametag.Value);

            voiceIndicators.Clear();
        }

        private static GameObject l;
        private static GameObject r;

        private static void UpdateLimbColor()
        {
            Color limbcolor = GetPlayerColor(VRRig.LocalRig);

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
            VRRig.LocalRig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            VRRig.LocalRig.mainSkin.material.color = new Color(VRRig.LocalRig.mainSkin.material.color.r, VRRig.LocalRig.mainSkin.material.color.g, VRRig.LocalRig.mainSkin.material.color.b, 0f);
            UpdateLimbColor();
        }

        public static void EndNoLimb()
        {
            UnityEngine.Object.Destroy(l);
            UnityEngine.Object.Destroy(r);

            VRRig.LocalRig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
            VRRig.LocalRig.mainSkin.material.color = new Color(VRRig.LocalRig.mainSkin.material.color.r, VRRig.LocalRig.mainSkin.material.color.g, VRRig.LocalRig.mainSkin.material.color.b, 1f);
        }

        private static Dictionary<VRRig, List<LineRenderer>> boneESP = new Dictionary<VRRig, List<LineRenderer>>() { };
        public static void CasualBoneESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, List<LineRenderer>> boness in boneESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(boness.Key))
                {
                    toRemove.Add(boness.Key);

                    foreach (LineRenderer renderer in boness.Value)
                        UnityEngine.Object.Destroy(renderer);
                }
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                    {
                        Lines = new List<LineRenderer> { };

                        LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            LineHead.numCapVertices = 10;
                            LineHead.numCornerVertices = 5;
                        }
                        LineHead.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(LineHead);

                        for (int i = 0; i < 19; i++)
                        {
                            LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                            if (smoothLines)
                            {
                                Line.numCapVertices = 10;
                                Line.numCornerVertices = 5;
                            }
                            Line.material.shader = Shader.Find("GUI/Text Shader");
                            Lines.Add(Line);
                        }

                        boneESP.Add(vrrig, Lines);
                    }

                    LineRenderer liner = Lines[0];

                    Color thecolor = vrrig.playerColor;
                    if (fmt) 
                        thecolor = GetBGColor(0f);
                    if (tt) 
                        thecolor.a = 0.5f;
                    if (hoc) 
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    for (int i = 0; i < 19; i++)
                    {
                        liner = Lines[i + 1];

                        if (hoc)
                            liner.gameObject.layer = 19;

                        liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                        liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[(i * 2) + 1]].position);
                    }
                }
            }
        }

        public static void InfectionBoneESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool selfTagged = PlayerIsTagged(VRRig.LocalRig);

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, List<LineRenderer>> boness in boneESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(boness.Key))
                {
                    toRemove.Add(boness.Key);

                    foreach (LineRenderer renderer in boness.Value)
                        UnityEngine.Object.Destroy(renderer);
                }
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                    {
                        Lines = new List<LineRenderer> { };

                        LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            LineHead.numCapVertices = 10;
                            LineHead.numCornerVertices = 5;
                        }
                        LineHead.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(LineHead);

                        for (int i = 0; i < 19; i++)
                        {
                            LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                            if (smoothLines)
                            {
                                Line.numCapVertices = 10;
                                Line.numCornerVertices = 5;
                            }
                            Line.material.shader = Shader.Find("GUI/Text Shader");
                            Lines.Add(Line);
                        }

                        boneESP.Add(vrrig, Lines);
                    }

                    LineRenderer liner = Lines[0];

                    bool playerTagged = PlayerIsTagged(vrrig);
                    Color thecolor = selfTagged ? vrrig.playerColor : GetPlayerColor(vrrig);

                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    for (int i = 0; i < 19; i++)
                    {
                        liner = Lines[i + 1];

                        if (hoc)
                            liner.gameObject.layer = 19;

                        liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                        liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[(i * 2) + 1]].position);
                    }
                }
            }
        }

        public static void HuntBoneESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (KeyValuePair<VRRig, List<LineRenderer>> boness in boneESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(boness.Key))
                {
                    toRemove.Add(boness.Key);

                    foreach (LineRenderer renderer in boness.Value)
                        UnityEngine.Object.Destroy(renderer);
                }
            }

            foreach (VRRig rig in toRemove)
                boneESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boneESP.TryGetValue(vrrig, out List<LineRenderer> Lines))
                    {
                        Lines = new List<LineRenderer> { };

                        LineRenderer LineHead = vrrig.head.rigTarget.gameObject.GetOrAddComponent<LineRenderer>();
                        if (smoothLines)
                        {
                            LineHead.numCapVertices = 10;
                            LineHead.numCornerVertices = 5;
                        }
                        LineHead.material.shader = Shader.Find("GUI/Text Shader");
                        Lines.Add(LineHead);

                        for (int i = 0; i < 19; i++)
                        {
                            LineRenderer Line = vrrig.mainSkin.bones[bones[i * 2]].gameObject.GetOrAddComponent<LineRenderer>();
                            if (smoothLines)
                            {
                                Line.numCapVertices = 10;
                                Line.numCornerVertices = 5;
                            }
                            Line.material.shader = Shader.Find("GUI/Text Shader");
                            Lines.Add(Line);
                        }

                        boneESP.Add(vrrig, Lines);
                    }

                    LineRenderer liner = Lines[0];

                    NetPlayer owner = GetPlayerFromVRRig(vrrig);
                    NetPlayer theirTarget = hunt.GetTargetOf(owner);
                    Color thecolor = owner == target ? GetPlayerColor(vrrig) : (theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear);

                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        liner.gameObject.layer = 19;

                    liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                    liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                    liner.startColor = thecolor;
                    liner.endColor = thecolor;

                    liner.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                    liner.SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    liner.SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));

                    for (int i = 0; i < 19; i++)
                    {
                        liner = Lines[i + 1];

                        if (hoc)
                            liner.gameObject.layer = 19;

                        liner.startWidth = thinTracers ? 0.0075f : 0.025f;
                        liner.endWidth = thinTracers ? 0.0075f : 0.025f;

                        liner.startColor = thecolor;
                        liner.endColor = thecolor;

                        liner.material.shader = Shader.Find("GUI/Text Shader");

                        liner.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                        liner.SetPosition(0, vrrig.mainSkin.bones[bones[i * 2]].position);
                        liner.SetPosition(1, vrrig.mainSkin.bones[bones[(i * 2) + 1]].position);
                    }
                }
            }
        }

        public static void DisableBoneESP()
        {
            foreach (KeyValuePair<VRRig, List<LineRenderer>> bones in boneESP)
            {
                foreach (LineRenderer renderer in bones.Value)
                    UnityEngine.Object.Destroy(renderer);
            }

            boneESP.Clear();
        }

        private static Dictionary<VRRig, SkinnedWireframeRenderer> wireframes = new Dictionary<VRRig, SkinnedWireframeRenderer> { };
        public static void CasualWireframeESP()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> lines in wireframes)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                Color color = GetPlayerColor(rig);

                if (fmt)
                    color = GetBGColor(0f);
                if (tt)
                    color = new Color(color.r, color.g, color.b, 0.5f);

                wireframe.meshRenderer.material.color = color;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                bool enabled = angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = color;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void InfectionWireframeESP()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> lines in wireframes)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool selfTagged = PlayerIsTagged(VRRig.LocalRig);

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                bool playerTagged = PlayerIsTagged(rig);
                Color thecolor = selfTagged ? rig.playerColor : GetPlayerColor(rig);

                if (fmt)
                    thecolor = GetBGColor(0f);
                if (tt)
                    thecolor.a = 0.5f;
                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                wireframe.meshRenderer.material.color = thecolor;

                bool enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                enabled &= angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = thecolor;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void HuntWireframeESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> lines in wireframes)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                wireframes.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;

            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!wireframes.TryGetValue(rig, out SkinnedWireframeRenderer wireframe))
                {
                    wireframe = rig.mainSkin.AddComponent<SkinnedWireframeRenderer>();
                    wireframes.Add(rig, wireframe);
                }

                NetPlayer owner = GetPlayerFromVRRig(rig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color thecolor = owner == target ? GetPlayerColor(rig) : (theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear);

                if (fmt)
                    thecolor = GetBGColor(0f);
                if (tt)
                    thecolor.a = 0.5f;
                if (hoc)
                    wireframe.wireframeObj.layer = 19;

                wireframe.meshRenderer.material.color = thecolor;

                bool enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;

                Vector3 toTarget = rig.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                float angle = Vector3.Angle(GorillaTagger.Instance.headCollider.transform.forward, toTarget);

                enabled &= angle <= Camera.main.fieldOfView / 1.75f;
                enabled &= Vector3.Distance(rig.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.leftHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f && Vector3.Distance(rig.rightHandTransform.position, GorillaTagger.Instance.headCollider.transform.position) < 35f;

                wireframe.enabled = enabled;
                wireframe.meshRenderer.enabled = enabled;

                if (!enabled)
                {
                    FixRigMaterialESPColors(rig);

                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    rig.mainSkin.material.color = thecolor;
                }
                else
                {
                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (rig.mainSkin.material.name.Contains("gorilla_body"))
                        rig.mainSkin.material.color = rig.playerColor;
                }
            }
        }

        public static void DisableWireframeESP()
        {
            foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> pred in wireframes)
            {
                pred.Key.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                UnityEngine.Object.Destroy(pred.Value);
            }

            wireframes.Clear();
        }

        public class SkinnedWireframeRenderer : MonoBehaviour
        {
            public SkinnedMeshRenderer skinnedMeshRenderer;
            public Mesh lineMesh;
            public MeshFilter meshFilter;
            public MeshRenderer meshRenderer;
            public GameObject wireframeObj;
            public Color color
            {
                get => meshRenderer.material.color;
                set => meshRenderer.material.color = value;
            }

            void Awake()
            {
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

                wireframeObj = new GameObject("Wireframe");
                wireframeObj.transform.SetParent(transform, false);

                meshFilter = wireframeObj.AddComponent<MeshFilter>();
                meshRenderer = wireframeObj.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                meshRenderer.material.color = Color.green;

                lineMesh = new Mesh();
                meshFilter.mesh = lineMesh;
            }

            void Update()
            {
                if (Time.frameCount % 3 > 0)
                    return;

                Mesh bakedMesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(bakedMesh);

                Vector3[] vertices = bakedMesh.vertices;
                int[] triangles = bakedMesh.triangles;

                List<Vector3> lineVertices = new List<Vector3>();
                List<int> lineIndices = new List<int>();

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int i0 = triangles[i];
                    int i1 = triangles[i + 1];
                    int i2 = triangles[i + 2];

                    lineVertices.Add(vertices[i0]);
                    lineVertices.Add(vertices[i1]);

                    lineVertices.Add(vertices[i1]);
                    lineVertices.Add(vertices[i2]);

                    lineVertices.Add(vertices[i2]);
                    lineVertices.Add(vertices[i0]);

                    int baseIndex = lineVertices.Count - 6;
                    for (int j = 0; j < 6; j++)
                        lineIndices.Add(baseIndex + j);
                }

                lineMesh.Clear();
                lineMesh.SetVertices(lineVertices);
                lineMesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
            }

            void OnDestroy()
            {
                if (lineMesh != null)
                {
                    Destroy(lineMesh);
                    lineMesh = null;
                }

                if (wireframeObj != null)
                {
                    Destroy(wireframeObj);
                    wireframeObj = null;
                }

                if (meshRenderer != null && meshRenderer.material != null)
                {
                    Destroy(meshRenderer.material);
                    meshRenderer.material = null;
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
                if (!vrrig.isLocal)
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
                if (!PlayerIsTagged(VRRig.LocalRig))
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (PlayerIsTagged(vrrig) && !vrrig.isLocal)
                        {
                            FixRigMaterialESPColors(vrrig);

                            vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.mainSkin.material.color = GetPlayerColor(vrrig);
                            if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                            if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                        }
                        else
                        {
                            vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                            if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                                vrrig.mainSkin.material.color = vrrig.playerColor;
                        }
                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (!PlayerIsTagged(vrrig) && !vrrig.isLocal)
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
                    if (!vrrig.isLocal)
                    {
                        FixRigMaterialESPColors(vrrig);

                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                            vrrig.mainSkin.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void HuntChams()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);
            foreach (NetPlayer player in PhotonNetwork.PlayerList)
            {
                VRRig vrrig = GetVRRigFromPlayer(player);
                if (player == target)
                {
                    FixRigMaterialESPColors(vrrig);

                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    vrrig.mainSkin.material.color = vrrig.playerColor;
                    if (GetIndex("Follow Menu Theme").enabled) { vrrig.mainSkin.material.color = GetBGColor(0f); }
                    if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                } 
                else 
                {
                    if (sillyComputer.GetTargetOf(player) == (NetPlayer)PhotonNetwork.LocalPlayer)
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        vrrig.mainSkin.material.color = Color.red;
                        if (GetIndex("Transparent Theme").enabled) { vrrig.mainSkin.material.color = new Color(vrrig.mainSkin.material.color.r, vrrig.mainSkin.material.color.g, vrrig.mainSkin.material.color.b, 0.5f); }
                    }
                    else
                    {
                        vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                            vrrig.mainSkin.material.color = vrrig.playerColor;
                    }
                }
            }
        }

        public static void DisableChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (vrrig.mainSkin.material.name.Contains("gorilla_body"))
                        vrrig.mainSkin.material.color = vrrig.playerColor;
                }
            }
        }

        private static Dictionary<VRRig, GameObject> boxESP = new Dictionary<VRRig, GameObject>() { };
        public static void CasualBoxESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, GameObject> box in boxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());

                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                        boxESP.Add(vrrig, box);
                    }

                    Color thecolor = vrrig.playerColor;
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void InfectionBoxESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool selfTagged = PlayerIsTagged(VRRig.LocalRig);

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, GameObject> box in boxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());

                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                        boxESP.Add(vrrig, box);
                    }

                    Color thecolor = selfTagged ? vrrig.playerColor : GetPlayerColor(vrrig);
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    bool playerTagged = PlayerIsTagged(vrrig);
                    box.SetActive((selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0);

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void HuntBoxESP()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (KeyValuePair<VRRig, GameObject> box in boxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                boxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!boxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());

                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");

                        boxESP.Add(vrrig, box);
                    }

                    NetPlayer owner = GetPlayerFromVRRig(vrrig);
                    NetPlayer theirTarget = hunt.GetTargetOf(owner);

                    Color thecolor = owner == target ? GetPlayerColor(vrrig) : (theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear);
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    bool playerTagged = PlayerIsTagged(vrrig);
                    box.SetActive(owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer);

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void DisableBoxESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> box in boxESP)
                UnityEngine.Object.Destroy(box.Value);

            boxESP.Clear();
        }

        private static Dictionary<VRRig, GameObject> hollowBoxESP = new Dictionary<VRRig, GameObject>() { };
        public static void CasualHollowBoxESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, GameObject> box in hollowBoxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        Renderer boxRenderer = box.GetComponent<Renderer>();
                        boxRenderer.enabled = false;
                        boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                        GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        hollowBoxESP.Add(vrrig, box);
                    }

                    Color thecolor = vrrig.playerColor;
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void HollowInfectionBoxESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool selfTagged = PlayerIsTagged(VRRig.LocalRig);

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, GameObject> box in hollowBoxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        Renderer boxRenderer = box.GetComponent<Renderer>();
                        boxRenderer.enabled = false;
                        boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                        GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        hollowBoxESP.Add(vrrig, box);
                    }

                    Color thecolor = selfTagged ? vrrig.playerColor : GetPlayerColor(vrrig);
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    bool playerTagged = PlayerIsTagged(vrrig);
                    box.SetActive((selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0);

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void HollowHuntBoxESP()
        {
            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            List<VRRig> toRemove = new List<VRRig>();
            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (KeyValuePair<VRRig, GameObject> box in hollowBoxESP)
            {
                if (!GorillaParent.instance.vrrigs.Contains(box.Key))
                {
                    toRemove.Add(box.Key);
                    UnityEngine.Object.Destroy(box.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                hollowBoxESP.Remove(rig);

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isLocal)
                {
                    if (!hollowBoxESP.TryGetValue(vrrig, out GameObject box))
                    {
                        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        box.transform.position = vrrig.transform.position;
                        UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                        box.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                        box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                        Renderer boxRenderer = box.GetComponent<Renderer>();
                        boxRenderer.enabled = false;
                        boxRenderer.material.shader = Shader.Find("GUI/Text Shader");

                        GameObject outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 1.025f : 1.1f, thinTracers ? 0.025f : 0.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        outl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        outl.transform.SetParent(box.transform);
                        outl.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
                        UnityEngine.Object.Destroy(outl.GetComponent<BoxCollider>());
                        outl.transform.localScale = new Vector3(thinTracers ? 0.025f : 0.1f, thinTracers ? 1.025f : 1.1f, 1f);
                        outl.transform.localRotation = Quaternion.identity;
                        outl.AddComponent<ClampColor>().targetRenderer = boxRenderer;

                        hollowBoxESP.Add(vrrig, box);
                    }

                    NetPlayer owner = GetPlayerFromVRRig(vrrig);
                    NetPlayer theirTarget = hunt.GetTargetOf(owner);

                    Color thecolor = owner == target ? GetPlayerColor(vrrig) : (theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear);
                    if (fmt)
                        thecolor = GetBGColor(0f);
                    if (tt)
                        thecolor.a = 0.5f;
                    if (hoc)
                        box.layer = 19;

                    box.GetComponent<Renderer>().material.color = thecolor;

                    bool playerTagged = PlayerIsTagged(vrrig);
                    box.SetActive(owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer);

                    box.transform.position = vrrig.transform.position;
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                }
            }
        }

        public static void DisableHollowBoxESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> box in hollowBoxESP)
                UnityEngine.Object.Destroy(box.Value);

            hollowBoxESP.Clear();
        }

        private static Dictionary<VRRig, TrailRenderer> breadcrumbs = new Dictionary<VRRig, TrailRenderer> { };
        public static void CasualBreadcrumbs()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, TrailRenderer> lines in breadcrumbs)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = GetIndex("Short Breadcrumbs").enabled;

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                if (hoc)
                    trail.gameObject.layer = 19;

                Color color = GetPlayerColor(rig);

                if (fmt)
                    color = GetBGColor(0f);
                if (tt)
                    color = new Color(color.r, color.g, color.b, 0.5f);

                trail.startColor = color;
                trail.endColor = color;
            }
        }

        public static void InfectionBreadcrumbs()
        {
            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, TrailRenderer> lines in breadcrumbs)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = GetIndex("Short Breadcrumbs").enabled;
            bool selfTagged = PlayerIsTagged(VRRig.LocalRig);

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                bool playerTagged = PlayerIsTagged(rig);
                Color thecolor = selfTagged ? rig.playerColor : GetPlayerColor(rig);

                if (fmt)
                    thecolor = GetBGColor(0f);
                if (tt)
                    thecolor.a = 0.5f;
                if (hoc)
                    trail.gameObject.layer = 19;

                trail.startColor = thecolor;
                trail.endColor = thecolor;

                trail.enabled = (selfTagged ? !playerTagged : playerTagged) || InfectedList().Count <= 0;
            }
        }

        public static void HuntBreadcrumbs()
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            List<VRRig> toRemove = new List<VRRig>();

            foreach (KeyValuePair<VRRig, TrailRenderer> lines in breadcrumbs)
            {
                if (!GorillaParent.instance.vrrigs.Contains(lines.Key))
                {
                    toRemove.Add(lines.Key);
                    UnityEngine.Object.Destroy(lines.Value);
                }
            }

            foreach (VRRig rig in toRemove)
                breadcrumbs.Remove(rig);

            bool fmt = GetIndex("Follow Menu Theme").enabled;
            bool hoc = GetIndex("Hidden on Camera").enabled;
            bool tt = GetIndex("Transparent Theme").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;
            bool shortBreadcrumbs = GetIndex("Short Breadcrumbs").enabled;

            GorillaHuntManager hunt = (GorillaHuntManager)GorillaGameManager.instance;
            NetPlayer target = hunt.GetTargetOf(NetworkSystem.Instance.LocalPlayer);

            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig.isLocal)
                    continue;

                if (!breadcrumbs.TryGetValue(rig, out TrailRenderer trail))
                {
                    trail = rig.head.rigTarget.gameObject.GetOrAddComponent<TrailRenderer>();

                    trail.minVertexDistance = 0.05f;

                    if (smoothLines)
                    {
                        trail.numCapVertices = 10;
                        trail.numCornerVertices = 5;
                    }

                    trail.material.shader = Shader.Find("GUI/Text Shader");
                    trail.time = shortBreadcrumbs ? 1f : 10f;

                    breadcrumbs.Add(rig, trail);
                }

                trail.startWidth = thinTracers ? 0.0075f : 0.025f;
                trail.endWidth = thinTracers ? 0.0075f : 0.025f;

                NetPlayer owner = GetPlayerFromVRRig(rig);
                NetPlayer theirTarget = hunt.GetTargetOf(owner);

                Color thecolor = owner == target ? GetPlayerColor(rig) : (theirTarget == NetworkSystem.Instance.LocalPlayer ? Color.red : Color.clear);

                if (fmt)
                    thecolor = GetBGColor(0f);
                if (tt)
                    thecolor.a = 0.5f;
                if (hoc)
                    trail.gameObject.layer = 19;

                trail.startColor = thecolor;
                trail.endColor = thecolor;

                trail.enabled = owner == target || theirTarget == NetworkSystem.Instance.LocalPlayer;
            }
        }

        public static void DisableBreadcrumbs()
        {
            foreach (KeyValuePair<VRRig, TrailRenderer> pred in breadcrumbs)
                UnityEngine.Object.Destroy(pred.Value);

            breadcrumbs.Clear();
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

        static GameObject LeftSphere = null;
        static GameObject RightSphere = null;
        public static void ShowButtonColliders()
        {
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
            UnityEngine.Object.Destroy(LeftSphere);
            UnityEngine.Object.Destroy(RightSphere);

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
            float lineWidth = (GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            Color menuColor = GetBGColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
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
            float lineWidth = (GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            bool LocalTagged = PlayerIsTagged(VRRig.LocalRig);
            bool NoInfected = InfectedList().Count == 0;

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
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

                        lineColor = GetPlayerColor(playerRig);
                    }
                }

                LineRenderer line = GetLineRender(hiddenOnCamera);

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

        public static void HuntTracers()
        {
            if (DoPerformanceCheck())
                return;

            if (GorillaGameManager.instance == null)
                return;

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;

            if (sillyComputer == null)
                return;

            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;
            float lineWidth = (GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                if (GetPlayerFromVRRig(playerRig) == currentTarget)
                {
                    Color lineColor = playerRig.playerColor;

                    LineRenderer line = GetLineRender(hiddenOnCamera);

                    if (transparentTheme)
                        lineColor.a = 0.5f;

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
                        lineColor.a = 0.5f;

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
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            Color menuColor = GetBGColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
                    continue;

                Color lineColor = playerRig.playerColor;

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                float width = thinTracers ? 0.0075f : 0.025f;
                line.startWidth = width;
                line.endWidth = width;
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
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            bool LocalTagged = PlayerIsTagged(VRRig.LocalRig);
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = GetBGColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
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

                        lineColor = GetPlayerColor(playerRig);
                    }
                }

                LineRenderer line = GetLineRender(hiddenOnCamera);

                if (followMenuTheme)
                    lineColor = menuColor;

                if (transparentTheme)
                    lineColor.a = 0.5f;

                line.startColor = lineColor;
                line.endColor = lineColor;
                float width = thinTracers ? 0.0075f : 0.025f;
                line.startWidth = width;
                line.endWidth = width;
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

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;

            if (sillyComputer == null)
                return;

            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;
            bool thinTracers = GetIndex("Thin Tracers").enabled;

            Color menuColor = GetBGColor(0f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal)
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
                    float width = thinTracers ? 0.0075f : 0.025f;
                    line.startWidth = width;
                    line.endWidth = width;
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
                    float width = thinTracers ? 0.0075f : 0.025f;
                    line.startWidth = width;
                    line.endWidth = width;
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

            Color menuColor = GetBGColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal) // Skip local player
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
                                secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
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

            bool LocalTagged = PlayerIsTagged(VRRig.LocalRig);
            bool NoInfected = InfectedList().Count == 0;

            Color menuColor = GetBGColor(0f);

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal) //skip local player
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

                        backgroundColor = GetPlayerColor(playerRig);
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
                                secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
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

            if (GorillaGameManager.instance.GameType() != GameModeType.HuntDown)
                return;

            GorillaHuntManager sillyComputer = (GorillaHuntManager)GorillaGameManager.instance;
            
            if (sillyComputer == null)
                return;

            // Cache these here so your not finding the values from GetIndex every call (GetIndex is fucking slow)
            bool followMenuTheme = GetIndex("Follow Menu Theme").enabled;
            bool transparentTheme = GetIndex("Transparent Theme").enabled;
            bool hiddenOnCamera = GetIndex("Hidden on Camera").enabled;

            Color menuColor = GetBGColor(0f);

            NetPlayer currentTarget = sillyComputer.GetTargetOf(PhotonNetwork.LocalPlayer);

            // Color bgColor = GetBGColor(0f); //dont need to call this function twice, just use a variable

            foreach (VRRig playerRig in GorillaParent.instance.vrrigs)
            {
                if (playerRig.isLocal) // Skip local player
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
                                    secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
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
                                    secondTransform.gameObject.GetComponent<TextMesh>().text = finalString;
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
                nameTagHolder = new GameObject("NameTag_Holder");

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
                UnityEngine.Object.Destroy(backgroundObject.GetComponent<Collider>());

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
                if (smoothLines)
                {
                    newLine.numCapVertices = 10;
                    newLine.numCornerVertices = 5;
                }
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
