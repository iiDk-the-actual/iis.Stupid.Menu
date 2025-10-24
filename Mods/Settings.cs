/*
 * ii's Stupid Menu  Mods/Settings.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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

using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using iiMenu.Classes.Menu;
using iiMenu.Extensions;
using iiMenu.Managers;
using iiMenu.Menu;
using iiMenu.Mods.Spam;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RigUtilities;
using Console = iiMenu.Classes.Menu.Console;
using Object = UnityEngine.Object;

namespace iiMenu.Mods
{
    public static class Settings
    {
        public static void Search() // This took me like 4 hours
        {
            isSearching = !isSearching;

            pageNumber = 0;
            keyboardInput = "";

            if (isSearching)
                SpawnKeyboard();
            else
                DestroyKeyboard();
        }

        public static void SpawnKeyboard()
        {
            isKeyboardPc = isOnPC;
            inTextInput = true;
            keyboardInput = "";

            shift = false;
            lockShift = false;

            if (isOnPC)
                lastPressedKeys.Add(KeyCode.Q);

            if (!isKeyboardPc)
            {
                if (VRKeyboard == null)
                {
                    VRKeyboard = LoadObject<GameObject>("VRKeyboard");
                    VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                    VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                    menuSpawnPosition = VRKeyboard.transform.Find("MenuSpawnPosition").gameObject;
                    VRKeyboard.transform.Find("Canvas").AddComponent<ColorChanger>().colors = textColors[1];

                    VRKeyboard.transform.localScale *= scaleWithPlayer ? GTPlayer.Instance.scale * menuScale : menuScale;
                    menuSpawnPosition.transform.localScale *= scaleWithPlayer ? GTPlayer.Instance.scale * menuScale : menuScale;

                    ColorChanger backgroundColorChanger = VRKeyboard.transform.Find("Background").gameObject.AddComponent<ColorChanger>();
                    backgroundColorChanger.colors = backgroundColor;

                    foreach (GameObject key in VRKeyboard.transform.Find("Seperate").Children()
                        .Select(t => t.gameObject)
                        .Concat(new[] { VRKeyboard.transform.Find("Keys/default").gameObject }))
                    {
                        ColorChanger keyColorChanger = key.AddComponent<ColorChanger>();
                        keyColorChanger.colors = buttonColors[0];
                    }

                    if (shouldOutline)
                        OutlineObjNonMenu(VRKeyboard.transform.Find("Background").gameObject, true);

                    var keys = new[] { "Numbers", "Letters", "Special", "Seperate" }
                        .Select(name => VRKeyboard.transform.Find(name))
                        .Where(t => t != null)
                        .SelectMany(t => t.Children())
                        .Select(t => t.gameObject); 

                    foreach (GameObject v in keys)
                    {
                        v.AddComponent<KeyboardKey>().key = v.name;
                        v.layer = 2;

                        if (shouldOutline)
                            OutlineObjNonMenu(v, true);
                    }
                }
            }

            if (lKeyReference == null)
            {
                lKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                lKeyReference.transform.parent = GorillaTagger.Instance.leftHandTransform;
                lKeyReference.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
                lKeyReference.transform.localPosition = pointerOffset;
                lKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                lKeyCollider = lKeyReference.GetComponent<SphereCollider>();

                ColorChanger colorChanger = lKeyReference.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;
            }

            if (rKeyReference == null)
            {
                rKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rKeyReference.transform.parent = GorillaTagger.Instance.rightHandTransform;
                rKeyReference.GetComponent<Renderer>().material.color = backgroundColor.GetColor(0);
                rKeyReference.transform.localPosition = pointerOffset;
                rKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                rKeyCollider = rKeyReference.GetComponent<SphereCollider>();

                ColorChanger colorChanger = rKeyReference.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;
            }
        }

        public static void DestroyKeyboard()
        {
            inTextInput = false;
            isKeyboardPc = false;

            if (lKeyReference != null)
            {
                Object.Destroy(lKeyReference);
                lKeyReference = null;
            }

            if (rKeyReference != null)
            {
                Object.Destroy(rKeyReference);
                rKeyReference = null;
            }

            if (VRKeyboard != null)
            {
                Object.Destroy(VRKeyboard);
                VRKeyboard = null;
            }

            if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
            {
                isOnPC = false;
                TPC.transform.position = TPC.transform.parent.position;
                TPC.transform.rotation = TPC.transform.parent.rotation;
            }
        }

        public static void GlobalReturn()
        {
            NotificationManager.ClearAllNotifications();
            Toggle(Buttons.buttons[currentCategoryIndex][0].buttonText, true);
            IsPrompting = false;
        }

        public static GameObject TutorialObject;
        public static LineRenderer TutorialSelector;
        public static void ShowTutorial()
        {
            if (TutorialObject != null)
                Object.Destroy(TutorialObject);

            TutorialObject = LoadObject<GameObject>("Tutorial");

            TutorialObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1f + Vector3.up * 0.25f;
            TutorialObject.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            Dictionary<string, string> videoByName = new Dictionary<string, string>
            {
                { "quest2", "q2" },
                { "quest3", "q3" },
                { "knuckles", "index" }
            };

            string videoName = "q3";
            string controllerName = ControllerInputPoller.instance.leftControllerDevice.name.ToLower();

            foreach (var video in videoByName)
            {
                if (controllerName.Contains(video.Key))
                {
                    videoName = video.Value;
                    break;
                }
            }

            VideoPlayer videoPlayer = TutorialObject.transform.Find("Video").GetComponent<VideoPlayer>();
            videoPlayer.url = $"{PluginInfo.ServerResourcePath}/Videos/Tutorial/tutorial-{videoName}.mp4";
            videoPlayer.isLooping = true;

            videoPlayer.AddComponent<TutorialButton>().buttonType = TutorialButton.ButtonType.Pause;

            TutorialObject.transform.Find("Close").AddComponent<TutorialButton>().buttonType = TutorialButton.ButtonType.Close;
        }

        private static bool lastTrigger;
        public static void UpdateTutorial()
        {
            if (Vector3.Distance(TutorialObject.transform.position, GorillaTagger.Instance.bodyCollider.transform.position) > 2f)
            {
                TutorialObject.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1f + Vector3.up * 0.25f;
                TutorialObject.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            }

            if (TutorialSelector == null)
            {
                TutorialSelector = new GameObject("iiMenu_TutorialSelector").AddComponent<LineRenderer>();
                TutorialSelector.material.shader = Shader.Find("Sprites/Default");

                TutorialSelector.startWidth = 0.01f;
                TutorialSelector.endWidth = 0.01f;

                TutorialSelector.positionCount = 2;

                TutorialSelector.useWorldSpace = true;
            }

            TutorialSelector.startColor = BrightenColor(new Color32(255, 128, 0, 128));
            TutorialSelector.endColor = BrightenColor(new Color32(255, 102, 0, 128));

            Vector3 Direction = TrueRightHand().forward;
            Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position + Direction / 4f, Direction, out var Ray, 512f, NoInvisLayerMask());
            if (!XRSettings.isDeviceActive)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out Ray, 512f, NoInvisLayerMask());
            }

            TutorialSelector.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
            TutorialSelector.SetPosition(1, Ray.point == Vector3.zero ? GorillaTagger.Instance.rightHandTransform.position : Ray.point);

            if ((rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && !lastTrigger)
            {
                TutorialButton gunTarget = Ray.collider.GetComponentInParent<TutorialButton>();
                if (gunTarget)
                    gunTarget.ClickButton();
            }

            lastTrigger = rightTrigger > 0.5f || Mouse.current.leftButton.isPressed;
        }

        public class TutorialButton : MonoBehaviour
        {
            public enum ButtonType
            {
                Pause,
                Close
            }

            public ButtonType buttonType;
            public void ClickButton()
            {
                switch (buttonType)
                {
                    case ButtonType.Pause:
                        VideoPlayer videoPlayer = TutorialObject.transform.Find("Video").GetComponent<VideoPlayer>();
                        if (videoPlayer.isPlaying)
                            videoPlayer.Pause();
                        else
                            videoPlayer.Play();

                        break;
                    case ButtonType.Close:
                        Destroy(TutorialObject);
                        Destroy(TutorialSelector.gameObject);
                        break;
                }
            }
        }

        public static void ShowDebug()
        {
            int category = GetCategory("Temporary Category");

            string version = PluginInfo.Version;
            if (PluginInfo.BetaBuild) version = "<color=blue>Beta</color> " + version;
            AddButton(category, new ButtonInfo { buttonText = "DebugMenuName", overlapText = "<color=grey><b>ii's Stupid Menu </b></color>" + version, label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugColor", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugName", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugId", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugClip", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugFps", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugRoomA", overlapText = "Loading...", label = true });
            AddButton(category, new ButtonInfo { buttonText = "DebugRoomB", overlapText = "Loading...", label = true });

            Debug();
            currentCategoryName = "Temporary Category";
        }

        public static bool hideId;
        public static void Debug()
        {
            string red = "<color=red>" + MathF.Floor(PlayerPrefs.GetFloat("redValue") * 255f) + "</color>";
            string green = ", <color=green>" + MathF.Floor(PlayerPrefs.GetFloat("greenValue") * 255f) + "</color>";
            string blue = ", <color=blue>" + MathF.Floor(PlayerPrefs.GetFloat("blueValue") * 255f) + "</color>";
            GetIndex("DebugColor").overlapText = "Color: " + red + green + blue;

            string master = PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient ? "<color=red> [Master]</color>" : "";
            GetIndex("DebugName").overlapText = PhotonNetwork.LocalPlayer.NickName + master;

            GetIndex("DebugId").overlapText = "<color=green>ID: </color>" + (hideId ? "Hidden" : PhotonNetwork.LocalPlayer.UserId);
            GetIndex("DebugClip").overlapText = "<color=green>Clip: </color>" + (GUIUtility.systemCopyBuffer.Length > 25 ? GUIUtility.systemCopyBuffer[..25] : GUIUtility.systemCopyBuffer);
            GetIndex("DebugFps").overlapText = "<b>" + lastDeltaTime + "</b> FPS <b>" + PhotonNetwork.GetPing() + "</b> Ping";
            GetIndex("DebugRoomA").overlapText = "<color=blue>" + NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper() + "</color> " + PhotonNetwork.PlayerList.Length + " Players";

            string priv = PhotonNetwork.InRoom ? NetworkSystem.Instance.SessionIsPrivate ? "Private" : "Public" : "";
            GetIndex("DebugRoomB").overlapText = "<color=blue>" + priv + "</color> " + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room");
        }
        public static void HideDebug()
        {
            currentCategoryName = "Main";
            int category = GetCategory("Temporary Category");

            RemoveButton(category, "DebugMenuName");
            RemoveButton(category, "DebugColor");
            RemoveButton(category, "DebugName");
            RemoveButton(category, "DebugId");
            RemoveButton(category, "DebugClip");
            RemoveButton(category, "DebugFps");
            RemoveButton(category, "DebugRoomA");
            RemoveButton(category, "DebugRoomB");
        }

        public static readonly Dictionary<string, Assembly> LoadedPlugins = new Dictionary<string, Assembly>();
        public static List<string> disabledPlugins = new List<string>();
        public static void LoadPlugins()
        {
            Buttons.buttons[GetCategory("Plugin Settings")] = new[] { new ButtonInfo { buttonText = "Exit Plugin Settings", method = () => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu." } };

            if (LoadedPlugins.Count > 0)
            {
                foreach (KeyValuePair<string, Assembly> Plugin in LoadedPlugins)
                {
                    if (!disabledPlugins.Contains(Plugin.Key))
                        DisablePlugin(Plugin.Value);
                }
            }

            cacheAssembly.Clear();

            cacheUpdate.Clear();
            cacheOnGUI.Clear();

            LoadedPlugins.Clear();

            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/Plugins"))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/Plugins");

            if (!File.Exists($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt", "");
            else
            {
                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt");
                if (text.Length > 1)
                    disabledPlugins = text.Split("\n").ToList();
            }

            string[] Files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Plugins");
            foreach (string File in Files)
            {
                try
                {
                    if (GetFileExtension(File) == "dll")
                    {
                        string PluginName = File.Replace($"{PluginInfo.BaseDirectory}/Plugins/", "");
                        LoadedPlugins.Add(PluginName, GetAssembly(File));
                    }
                } catch (Exception e) { LogManager.Log("Error with loading plugin " + File + ": " + e); }
            }

            foreach (KeyValuePair<string, Assembly> Plugin in LoadedPlugins)
            {
                try
                {
                    string[] PluginInfo = GetPluginInfo(Plugin.Value);
                    AddButton(33, new ButtonInfo { buttonText = Plugin.Key, overlapText = (disabledPlugins.Contains(Plugin.Key) ? "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>" : "<color=grey>[</color><color=green>ON</color><color=grey>]</color>") + " " + PluginInfo[0], method = () => TogglePlugin(Plugin), isTogglable = false, toolTip = PluginInfo[1] });
                    if (!disabledPlugins.Contains(Plugin.Key))
                        EnablePlugin(Plugin.Value);
                }
                catch (Exception e) { LogManager.Log("Error with enabling plugin " + Plugin.Key + ": " + e); }
            }

            AddButton(33, new ButtonInfo { buttonText = "Open Plugins Folder", method = OpenPluginsFolder, isTogglable = false, toolTip = "Opens a folder containing all of your plugins." });
            AddButton(33, new ButtonInfo { buttonText = "Reload Plugins", method = ReloadPlugins, isTogglable = false, toolTip = "Reloads all of your plugins." });
            AddButton(33, new ButtonInfo { buttonText = "Get More Plugins", method = LoadPluginLibrary, isTogglable = false, toolTip = "Opens a public plugin library, where you can download your own plugins." });
        }

        public static void ReloadPlugins()
        {
            SavePreferences();
            LoadPlugins();
            LoadPreferences();

            if (isSearching)
                Search();

            currentCategoryName = "Main";
        }

        public static void OpenPluginsFolder()
        {
            string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, $"{PluginInfo.BaseDirectory}/Plugins");
            filePath = filePath.Split("BepInEx\\")[0] + $"{PluginInfo.BaseDirectory}/Plugins";
            Process.Start(filePath);
        }

        public static void LoadPluginLibrary()
        {
            currentCategoryName = "Sound Library";

            string library = GetHttp($"{PluginInfo.ServerResourcePath}/Plugins/PluginLibrary.txt");
            string[] plugins = AlphabetizeNoSkip(library.Split("\n"));

            List<ButtonInfo> pluginbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Plugin Library", method = () => currentCategoryName = "Plugin Settings", isTogglable = false, toolTip = "Returns you back to the plugin settings." } };
            int index = 0;

            foreach (string plugin in plugins)
            {
                if (plugin.Length > 2)
                {
                    index++;
                    string[] Data = plugin.Split(";");
                    pluginbuttons.Add(new ButtonInfo { buttonText = "PluginDownload" + index, overlapText = Data[0], method =() => DownloadPlugin(Data[0], Data[2]), isTogglable = false, toolTip = Data[1] });
                }
            }
            Buttons.buttons[GetCategory("Sound Library")] = pluginbuttons.ToArray();
        }

        public static void DownloadPlugin(string name, string url)
        {
            if (name.Contains(".."))
                name = name.Replace("..", "");

            string filename = url.Split("/")[^1];

            if (File.Exists($"{PluginInfo.BaseDirectory}/Plugins/" + filename))
                File.Delete($"{PluginInfo.BaseDirectory}/Plugins/" + filename);

            WebClient stream = new WebClient();
            stream.DownloadFile(url, $"{PluginInfo.BaseDirectory}/Plugins/" + filename);

            LoadPlugins();
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to your plugins.");
        }

        public static void TogglePlugin(KeyValuePair<string, Assembly> Plugin)
        {
            if (disabledPlugins.Contains(Plugin.Key))
            {
                disabledPlugins.Remove(Plugin.Key);
                EnablePlugin(Plugin.Value);
            } else
            {
                disabledPlugins.Add(Plugin.Key);
                DisablePlugin(Plugin.Value);
            }

            string disabledPluginsString = "";
            foreach (string disabledPlugin in disabledPlugins)
                disabledPluginsString += disabledPlugin + "\n";

            File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt", disabledPluginsString);

            GetIndex(Plugin.Key).overlapText = (disabledPlugins.Contains(Plugin.Key) ? "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>" : "<color=grey>[</color><color=green>ON</color><color=grey>]</color>") + " " + GetPluginInfo(Plugin.Value)[0];
        }

        public static void PlayersTab()
        {
            currentCategoryName = "Players";

            List<ButtonInfo> buttons = new List<ButtonInfo> { 
                new ButtonInfo { 
                    buttonText = "Exit Players", 
                    method =() => currentCategoryName = "Main", 
                    isTogglable = false, 
                    toolTip = "Returns you back to the main page." 
                } 
            };

            if (!PhotonNetwork.InRoom)
                buttons.Add(new ButtonInfo { buttonText = "Not in a Room", label = true });
            else
            {
                for (int i = 0; i < NetworkSystem.Instance.PlayerListOthers.Length; i++)
                {
                    NetPlayer player = NetworkSystem.Instance.PlayerListOthers[i];
                    string playerColor = "#ffffff";
                    try
                    {
                        playerColor = $"#{ColorToHex(GetVRRigFromPlayer(player).playerColor)}";
                    }
                    catch { }

                    buttons.Add(new ButtonInfo
                    {
                        buttonText = $"PlayerButton{i}",
                        overlapText = $"<color={playerColor}>" + player.NickName + "</color>",
                        method =() => NavigatePlayer(player),
                        isTogglable = false,
                        toolTip = $"See information on the player {player.NickName}."
                    });
                }
            }

            Buttons.buttons[37] = buttons.ToArray();
        }

        public static void NavigatePlayer(NetPlayer player)
        {
            currentCategoryName = "Temporary Category";
            string targetName = player.NickName;

            VRRig playerRig = GetVRRigFromPlayer(player) ?? null;

            List<ButtonInfo> buttons = new List<ButtonInfo> {
                new ButtonInfo {
                    buttonText = "Exit PlayerInspect",
                    overlapText = $"Exit {targetName}",
                    method =() => PlayersTab(),
                    isTogglable = false,
                    toolTip = "Returns you back to the players tab."
                },

                new ButtonInfo {
                    buttonText = "Teleport to Player",
                    overlapText = $"Teleport to {targetName}",
                    method =() => Movement.TeleportToPlayer(player),
                    isTogglable = false,
                    toolTip = $"Teleports you to {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Give Player Guns",
                    overlapText = $"Give {targetName} Guns",
                    method =() => giveGunTarget = playerRig,
                    disableMethod =() => giveGunTarget = null,
                    toolTip = $"Gives {targetName} every gun on the menu."
                },
                new ButtonInfo {
                    buttonText = "Copy Movement",
                    overlapText = $"Copy Movement {targetName}",
                    method =() => Movement.CopyMovementPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Copies the movement of {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Follow Player",
                    overlapText = $"Follow {targetName}",
                    method =() => Movement.FollowPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Follows {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Tag Player",
                    overlapText = $"Tag {targetName}",
                    method =() => Advantages.TagPlayer(player),
                    disableMethod = Movement.EnableRig,
                    toolTip = $"Tags {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Snowball Fling Player",
                    overlapText = $"Snowball Fling {targetName}",
                    method =() => Overpowered.FlingPlayer(player),
                    toolTip = $"Flings {targetName} with snowballs."
                },
                new ButtonInfo {
                    buttonText = "Projectile Blind Player",
                    overlapText = $"Projectile Blind {targetName}",
                    method =() => Projectiles.ProjectileBlindPlayer(player),
                    toolTip = $"Blinds {targetName} using the egg projectiles."
                },
                new ButtonInfo {
                    buttonText = "Projectile Lag Player",
                    overlapText = $"Projectile Lag {targetName}",
                    method =() => Projectiles.ProjectileLagPlayer(player),
                    toolTip = $"Lags {targetName} using the firework projectiles."
                },
                new ButtonInfo {
                    buttonText = "Destroy Player",
                    overlapText = $"Destroy {targetName}",
                    method =() => Overpowered.DestroyPlayer(player),
                    toolTip = $"Stops all new players from seeing {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Bring Player",
                    overlapText = $"Guardian Bring {targetName}",
                    method =() => Overpowered.BringPlayer(player),
                    toolTip = $"Brings {targetName} to you."
                },
                new ButtonInfo {
                    buttonText = "Guardian Bring Player Gun",
                    overlapText = $"Guardian Bring {targetName} Gun",
                    method =() => Overpowered.BringPlayerGun(player),
                    toolTip = $"Brings {targetName} to wherever your hand desires."
                },
                new ButtonInfo {
                    buttonText = "Guardian Kick Player",
                    overlapText = $"Guardian Kick {targetName}",
                    method =() => Overpowered.KickPlayer(player),
                    toolTip = $"Kicks {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Obliterate Player",
                    overlapText = $"Guardian Obliterate {targetName}",
                    method =() => Overpowered.ObliteratePlayer(player),
                    toolTip = $"Obliterates {targetName}."
                },
                new ButtonInfo {
                    buttonText = "Guardian Crash Player",
                    overlapText = $"Guardian Crash {targetName}",
                    method =() => Overpowered.CrashPlayer(player),
                    toolTip = $"Crashes {targetName}."
                },

                new ButtonInfo {
                    buttonText = "Barrel Kick Player",
                    overlapText = $"Barrel Kick {targetName}",
                    enableMethod =() => Fun.CheckOwnedThrowable(618),
                    method =() => { Vector3 targetDirection = new Vector3(-71.33718f, 101.4977f, -93.09029f) - playerRig.headMesh.transform.position; Fun.SendBarrelProjectile(playerRig.transform.position + (GorillaTagger.Instance.headCollider.transform.position - playerRig.headMesh.transform.position).normalized * 0.1f, targetDirection.normalized * 50f, Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } }); },
                    toolTip = $"Kicks {targetName} using the barrels."
                },
                new ButtonInfo {
                    buttonText = "Barrel Crash Player",
                    overlapText = $"Barrel Crash {targetName}",
                    enableMethod =() => Fun.CheckOwnedThrowable(618),
                    method =() => Fun.SendBarrelProjectile(playerRig.transform.position, new Vector3(0f, 5000f, 0f), Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } }),
                    toolTip = $"Crashes {targetName} using the barrels."
                },
            };

            if (PhotonNetwork.IsMasterClient)
            {
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo {
                            buttonText = "Vibrate Player",
                            overlapText = $"Vibrate {targetName}",
                            method =() => Overpowered.BetaSetStatus(RoomSystem.StatusEffects.JoinedTaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } }),
                            toolTip = $"Vibrates {targetName}'s controllers."
                        },
                        new ButtonInfo {
                            buttonText = "Slow Player",
                            overlapText = $"Slow {targetName}",
                            method =() => Overpowered.BetaSetStatus(RoomSystem.StatusEffects.TaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } } ),
                            toolTip = $"Gives {targetName} tag freeze."
                        },

                        new ButtonInfo {
                            buttonText = "Lucy Chase Player",
                            overlapText = $"Lucy Chase {targetName}",
                            method =() => Overpowered.LucyChase(player),
                            toolTip = $"Makes lucy chase {targetName}."
                        },
                        new ButtonInfo {
                            buttonText = "Lucy Attack Player",
                            overlapText = $"Lucy Attack {targetName}",
                            method =() => Overpowered.LucyAttack(player),
                            toolTip = $"Makes lucy attack {targetName}."
                        },

                        new ButtonInfo {
                            buttonText = "Lurker Attack Player",
                            overlapText = $"Lurker Attack {targetName}",
                            method =() => Overpowered.LurkerAttack(player),
                            toolTip = $"Makes the lurker ghost attack {targetName}."
                        }
                    }
                );
            }

            if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
            {
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo {
                            buttonText = "Admin Kick Player",
                            overlapText = $"Admin Kick {targetName}",
                            method =() => Console.ExecuteCommand("kick", ReceiverGroup.All, player.UserId),
                            isTogglable = false,
                            toolTip = $"Kicks {targetName} if they're using the menu."
                        },
                        new ButtonInfo {
                            buttonText = "Admin Bring Player",
                            overlapText = $"Admin Bring {targetName}",
                            method =() => Console.ExecuteCommand("tp", player.ActorNumber, GorillaTagger.Instance.headCollider.transform.position),
                            isTogglable = false,
                            toolTip = $"Brings {targetName} to you if they're using the menu."
                        },
                        new ButtonInfo {
                            buttonText = "Admin Crash Player",
                            overlapText = $"Admin Crash {targetName}",
                            method =() => Console.ExecuteCommand("crash", player.ActorNumber),
                            isTogglable = false,
                            toolTip = $"Crashes {targetName} if they're using the menu."
                        },
                    }
                );
            }

            Color playerColor = playerRig?.playerColor ?? Color.black;
            if (playerRig)
                buttons.AddRange(
                    new[]
                    {
                        new ButtonInfo
                        {
                            buttonText = "Player Name",
                            overlapText = $"Name: {player.NickName}",
                            method = () => ChangeName(player.NickName),
                            isTogglable = false,
                            toolTip = $"Sets your name to \"{player.NickName}\"."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Color",
                            overlapText =
                                $"Color: {playerColor.ToRichRGBString()}",
                            method = () => ChangeColor(playerColor),
                            isTogglable = false,
                            toolTip = $"Sets your color to the same as {targetName}."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player User ID",
                            overlapText = $"User ID: {player.UserId}",
                            method = () =>
                            {
                                NotificationManager.SendNotification(
                                    $"<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied {player.UserId} to the clipboard!",
                                    5000);
                                GUIUtility.systemCopyBuffer = player.UserId;
                            },
                            isTogglable = false,
                            toolTip = $"Copies {player.UserId} to your clipboard."
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Creation Date",
                            overlapText =
                                $"Creation Date: {GetCreationDate(player.UserId, creationDate => { GetIndex("Player Creation Date").overlapText = $"Creation Date: {creationDate}"; ReloadMenu(); })}",
                            label = true
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player Platform",
                            overlapText =
                                $"Platform: {((playerRig?.IsSteam() ?? false) ? "Steam" : "Quest")}",
                            label = true
                        },
                        new ButtonInfo
                        {
                            buttonText = "Player FPS",
                            overlapText = $"FPS: {playerRig.fps}",
                            label = true
                        }
                    }
                );

            Buttons.buttons[29] = buttons.ToArray();
        }

        public static void RightHand()
        {
            rightHand = true;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotificationManager.ClearAllNotifications();
            }

            if (!GetIndex("Info Watch").enabled) return;
            Toggle("Info Watch");
            Toggle("Info Watch");
            NotificationManager.ClearAllNotifications();
        }

        public static void LeftHand()
        {
            rightHand = false;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotificationManager.ClearAllNotifications();
            }

            if (!GetIndex("Info Watch").enabled) return;
            Toggle("Info Watch");
            Toggle("Info Watch");
            NotificationManager.ClearAllNotifications();
        }

        public static void ClearAllKeybinds()
        {
            foreach (KeyValuePair<string, List<string>> bind in ModBindings)
            {
                foreach (string modName in bind.Value)
                    GetIndex(modName).customBind = null;

                bind.Value.Clear();
            }
        }

        public static void StartBind(string bind)
        {
            if (IsRebinding)
                return;
            IsBinding = true;
            BindInput = bind;
        }
        public static void StartRebind(string bind)
        {
            if (IsBinding)
                return;
            IsRebinding = true;
            BindInput = bind;
        }

        public static void RemoveRebinds()
        {
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    v.rebindKey = "";
                }
            }
            NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed all rebinds.");
        }

        // The code below is fully safe. I know, it seems suspicious.
        public static void UpdateMenu()
        {
            if (SystemInfo.operatingSystemFamily.Equals(OperatingSystemFamily.Windows))
            {
                string updateScript = @"@echo off
title ii's Stupid Menu
color 0E

cls
echo.
echo      ••╹   ┏┓     • ┓  ┳┳┓      
echo      ┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏
echo      ┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻
echo                 ┛               
echo.

echo Your menu is updating, please wait...
echo.

set ""PLUGIN_PATH=BepInEx\plugins""
dir ""%PLUGIN_PATH%\*iiMenu_AutoUpdater*.dll"" >nul 2>&1
if %ERRORLEVEL%==0 (
    goto restart
)

for %%F in (""%PLUGIN_PATH%\*stupid*menu*.dll"") do (
    set ""MENU_FILE=%%F""
    goto update
)

echo No menu file found, skipping update.
goto restart

:update
echo Downloading latest release of ii's Stupid Menu...

curl -L -o ""%MENU_FILE%"" ^
""https://github.com/iiDk-the-actual/iis.Stupid.Menu/releases/latest/download/iis_Stupid_Menu.dll""

goto restart

:restart

:WAIT_LOOP
tasklist /FI ""IMAGENAME eq Gorilla Tag.exe"" | find /I ""Gorilla Tag.exe"" >nul
if %ERRORLEVEL%==0 (
    timeout /t 1 >nul
    goto WAIT_LOOP
)

echo Launching Gorilla Tag...
start steam://run/1533390
exit";

                string fileName = $"{PluginInfo.BaseDirectory}/UpdateScript.bat";

                File.WriteAllText(fileName, updateScript);

                string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, fileName);
                filePath = filePath.Split("BepInEx\\")[0] + fileName;

                Process.Start(filePath);
                Application.Quit();   
            }
            if (SystemInfo.operatingSystemFamily.Equals(OperatingSystemFamily.Linux))
            {
        
string updateScript = @"#!/bin/bash
clear
echo
echo ""      ••╹   ┏┓     • ┓  ┳┳┓      ""
echo ""      ┓┓ ┏  ┗┓╋┓┏┏┓┓┏┫  ┃┃┃┏┓┏┓┓┏""
echo ""      ┗┗ ┛  ┗┛┗┗┻┣┛┗┗┻  ┛ ┗┗ ┛┗┗┻""
echo ""                 ┛               ""
echo
echo ""Your menu is updating, please wait...""
echo

PLUGIN_PATH=""BepInEx/plugins""
MENU_FILE=""""

if ls ""$PLUGIN_PATH""/*iiMenu_AutoUpdater*.dll 1> /dev/null 2>&1; then
    echo ""Auto-updater found. Restarting game...""
else
    for f in ""$PLUGIN_PATH""/*stupid*menu*.dll; do
        if [ -f ""$f"" ]; then
            MENU_FILE=""$f""
            break
        fi
    done

    if [ -z ""$MENU_FILE"" ]; then
        echo ""No menu file found, skipping update.""
    else
        echo ""Downloading latest release of ii's Stupid Menu...""
        curl -L -o ""$MENU_FILE"" \
        ""https://github.com/iiDk-the-actual/iis.Stupid.Menu/releases/latest/download/iis_Stupid_Menu.dll""
    fi
fi

while pgrep -f ""GorillaTag.exe"" > /dev/null; do
    sleep 1
done

echo ""Launching Gorilla Tag...""
xdg-open ""steam://run/1533390""
exit 0";

            string fileName = $"{PluginInfo.BaseDirectory}/UpdateScript.sh";
            File.WriteAllText(fileName, updateScript);
            Process.Start("chmod", $"+x \"{fileName}\"");
            Process.Start(new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"\"{fileName}\"",
                UseShellExecute = false
            });
                Application.Quit();

            }
        }

        public static void JoystickMenuOff()
        {
            joystickMenu = false;
            joystickOpen = false;
        }

        public static void PhysicalMenuOn()
        {
            physicalMenu = true;
            physicalOpenPosition = Vector3.zero;
        }

        public static void PhysicalMenuOff()
        {
            physicalMenu = false;
            physicalOpenPosition = Vector3.zero;
        }

        public static void WatchMenuOn()
        {
            watchMenu = true;
            GameObject mainwatch = GetObject("Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            watchobject = Object.Instantiate(mainwatch, 
                rightHand ?
                GetObject("Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform :
                GetObject("Player Objects/Local VRRig/Local Gorilla Player/GorillaPlayerNetworkedRigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);

            Object.Destroy(watchobject.GetComponent<GorillaHuntComputer>());
            watchobject.SetActive(true);

            Transform watchCanvas = watchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
            watchCanvas.Find("Hat").gameObject.SetActive(false);
            watchCanvas.Find("Face").gameObject.SetActive(false);
            watchCanvas.Find("Badge").gameObject.SetActive(false);
            watchCanvas.Find("Material").gameObject.SetActive(false);
            watchCanvas.Find("Right Hand").gameObject.SetActive(false);

            watchText = watchCanvas.Find("Text").gameObject;
            watchEnabledIndicator = watchCanvas.Find("Left Hand").gameObject;
            watchShell = watchobject.transform.Find("HuntWatch_ScreenLocal").gameObject;

            watchShell.GetComponent<Renderer>().material = OrangeUI;

            if (rightHand)
            {
                watchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
                watchShell.transform.parent.localPosition += new Vector3(0.025f, 0f, 0f);
                watchShell.transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
            }
        }

        public static void WatchMenuOff()
        {
            watchMenu = false;
            Object.Destroy(watchobject);
        }

        public static int langInd;
        public static void ChangeMenuLanguage(bool positive = true)
        {
            string[] languageNames = {
                "English",
                "Español",
                "Français",
                "Deutsch",
                "日本語",
                "Italiano",
                "Português",
                "Nederlands",
                "Русский",
                "Polski"
            };

            string[] codenames = {
                "en",
                "es",
                "fr",
                "de",
                "ja",
                "it",
                "pt",
                "nl",
                "ru",
                "pl"
            };

            if (positive)
                langInd++;
            else
                langInd--;

            langInd %= languageNames.Length;
            if (langInd < 0)
                langInd = languageNames.Length - 1;

            translateCache.Clear();
            language = codenames[langInd];

            GetIndex("Change Menu Language").overlapText = "Change Menu Language <color=grey>[</color><color=green>" + languageNames[langInd] + "</color><color=grey>]</color>";

            translate = langInd != 0;
        }

        public static void ChangeMenuButton(bool positive = true)
        {
            string[] buttonNames = {
                "Primary",
                "Secondary",
                "Grip",
                "Trigger",
                "Joystick"
            };

            if (positive)
                menuButtonIndex++;
            else
                menuButtonIndex--;

            menuButtonIndex %= buttonNames.Length;
            if (menuButtonIndex < 0)
                menuButtonIndex = buttonNames.Length - 1;

            GetIndex("Change Menu Button").overlapText = "Change Menu Button <color=grey>[</color><color=green>" + buttonNames[menuButtonIndex] + "</color><color=grey>]</color>";
        }

        // I know there's better ways to do this. Trust me.
        public static void ChangeMenuTheme(bool increment = true)
        {
            if (increment) 
                themeType++; 
            else 
                themeType--;

            const int themeCount = 65;

            if (themeType > themeCount)
                themeType = 1;

            if (themeType < 1)
                themeType = themeCount;

            if (GetIndex("Custom Menu Theme").enabled)
                return;

            switch (themeType)
            {
                case 1: // Orange
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 128, 0, 128), new Color32(255, 102, 0, 128))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(170, 85, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(85, 42, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        }
                    };
                    break;
                case 2: // Blue Magenta
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.blue, Color.magenta)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 3: // Dark Mode
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(20, 20, 20, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 4: // Strobe
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.white, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 5: // Kman
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(110, 0, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(110, 0, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(110, 0, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 6: // Rainbow
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        rainbow = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            rainbow = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 7: // Cone
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 128))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(170, 85, 0, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(85, 42, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 190, 125, 255))
                        }
                    };
                    GetIndex("Thin Menu").enabled = true;
                    thinMenu = true;
                    break;
                case 8: // Player Material
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        copyRigColor = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            copyRigColor = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 9: // Lava
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(255, 111, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(255, 111, 0, 255), Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 10: // Rock
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.red)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 11: // Ice
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(0, 174, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 174, 255, 255), Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 12: // Water
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 136, 255, 255), new Color32(0, 174, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 100, 188, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 174, 255, 255), new Color32(0, 136, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 13: // Minty
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 255, 246, 255), new Color32(0, 255, 144, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(0, 255, 144, 255), new Color32(0, 255, 246, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 14: // Pink
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 130, 255, 255), Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 130, 255, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 15: // Purple
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(122, 35, 159, 255), new Color32(60, 26, 89, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(60, 26, 89, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(122, 35, 159, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 16: // Magenta Cyan
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 17: // Red Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 18: // Orange Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 128, 0, 255), Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 128, 0, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 19: // Yellow Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.yellow, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 20: // Green Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.green, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 21: // Blue Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.blue, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.blue)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 22: // Purple Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(119, 0, 255, 255), Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(119, 0, 255, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 23: // Magenta Fade
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 24: // Banana
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(255, 255, 130, 255), Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 255, 130, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 25: // Pride
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.red, Color.green)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 26: // Trans
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(245, 169, 184, 255), new Color32(91, 206, 250, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 169, 184, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(91, 206, 250, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 169, 184, 255))
                        }
                    };
                    break;
                case 27: // MLM or Gay
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(7, 141, 112, 255), new Color32(61, 26, 220, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(7, 141, 112, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(61, 26, 220, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(7, 141, 112, 255))
                        }
                    };
                    break;
                case 28: // Steal (old)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(75, 75, 75, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 29: // Silence
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(80, 0, 80, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 30: // Transparent
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        transparent = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white),
                            transparent = true
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green),
                            transparent = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 31: // King
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(100, 60, 170, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(150, 100, 240, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(150, 100, 240, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.cyan)
                        }
                    };
                    break;
                case 32: // Scoreboard
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(0, 59, 4, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(192, 190, 171, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 33: // Scoreboard (banned)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(225, 73, 43, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(192, 190, 171, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 34: // Rift
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 40, 40, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(167, 66, 191, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 35: // Blurple Dark
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(26, 26, 61, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(26, 26, 61, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(43, 17, 84, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 36: // ShibaGT Gold
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.gray)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.yellow)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.magenta)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 37: // ShibaGT Genesis
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 38: // wyvern
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(199, 115, 173, 255), new Color32(165, 233, 185, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(99, 58, 86, 255), new Color32(83, 116, 92, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(99, 58, 86, 255), new Color32(83, 116, 92, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    break;
                case 39: // Steal (new)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(27, 27, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(50, 50, 50, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(66, 66, 66, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 40: // USA Menu (lol)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(100, 25, 125, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 41: // Watch
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(27, 27, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.green)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 42: // AZ Menu
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, new Color32(100, 0, 0, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(100, 0, 0, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 43: // ImGUI
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(21, 22, 23, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(32, 50, 77, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(60, 127, 206, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 44: // Clean Dark
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(10, 10, 10, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 45: // Discord Light Mode (lmfao)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(245, 245, 245, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 46: // The Hub
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(255, 163, 26, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 47: // EPILEPTIC
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.black),
                        epileptic = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black),
                            epileptic = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 48: // Discord Blurple
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(111, 143, 255, 255), new Color32(163, 184, 255, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(96, 125, 219, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(147, 167, 226, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(33, 33, 101, 255))
                        }
                    };
                    break;
                case 49: // VS Zero
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(19, 22, 27, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(19, 22, 27, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(16, 18, 22, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(82, 96, 122, 255))
                        }
                    };
                    break;
                case 50: // Weed theme (for v4.2.0) (also 50th theme)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(new Color32(0, 136, 16, 255), new Color32(0, 127, 14, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 158, 15, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 112, 11, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 51: // Pastel Rainbow
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.white),
                        pastelRainbow = true
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white),
                            pastelRainbow = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    break;
                case 52: // Rift Light
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(25, 25, 25, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 40, 40, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(165, 137, 255, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(144, 144, 144, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 53: // Rose (Solace)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(176, 12, 64, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(140, 10, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(250, 2, 81, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 54: // Tenacity (Solace)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(124, 25, 194, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(88, 9, 145, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(136, 9, 227, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 55: // e621 (for version 6.2.1)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(1, 73, 149, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(1, 46, 87, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(0, 37, 74, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(252, 179, 40, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 56: // Catppuccin Mocha
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(30, 30, 46, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(88, 91, 112, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(49, 50, 68, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(205, 214, 244, 255))
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(186, 194, 222, 255))
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(166, 173, 200, 255))
                        }
                    };
                    break;
                case 57: // Rexon
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(45, 25, 75, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(40, 15, 60, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(100, 30, 140, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 58: // Tenacity (Minecraft)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(231, 133, 209, 255), new Color32(56, 155, 193, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 59: // Mint Blue (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(40, 94, 93, 255), new Color32(66, 158, 157, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
            };
                    break;
                case 60: // Pink Blood (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(255, 166, 201, 255), new Color32(228, 0, 70, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 61: // Purple Fire (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(177, 162, 202, 255), new Color32(104, 71, 141, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 62: // Deep Ocean (Opal v2)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(new Color32(32, 32, 32, 255))
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(new Color32(45, 46, 51, 255))
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSimpleGradient(new Color32(60, 82, 145, 255), new Color32(0, 20, 64, 255))
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 63: // Bad Apple (thanks random person in vc for idea)
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            transparent = true
                        },
                        new ExtGradient // Pressed
                        {
                            transparent = true
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 64: // coolkidd
                    backgroundColor = new ExtGradient
                    {
                        colors = ExtGradient.GetSolidGradient(Color.red)
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.red)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
                case 65: // Old ShibaGT RGB
                    backgroundColor = new ExtGradient
                    {
                        colors = new[]
                        {
                            new GradientColorKey(Color.red, 0f),
                            new GradientColorKey(Color.green, 0.333f),
                            new GradientColorKey(Color.blue, 0.666f),
                            new GradientColorKey(Color.red, 1f),
                        }
                    };
                    buttonColors = new[]
                    {
                        new ExtGradient // Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.black)
                        },
                        new ExtGradient // Pressed
                        {
                            colors = new[]
                            {
                                new GradientColorKey(Color.red, 0f),
                                new GradientColorKey(Color.green, 0.333f),
                                new GradientColorKey(Color.blue, 0.666f),
                                new GradientColorKey(Color.red, 1f),
                            }
                        }
                    };
                    textColors = new[]
                    {
                        new ExtGradient // Title
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Released
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        },
                        new ExtGradient // Button Clicked
                        {
                            colors = ExtGradient.GetSolidGradient(Color.white)
                        }
                    };
                    break;
            }
        }

        private static int menuScaleIndex = 10;
        public static void ChangeMenuScale(bool positive = true)
        {
            if (positive)
                menuScaleIndex++;
            else
                menuScaleIndex--;

            if (menuScaleIndex > 30)
                menuScaleIndex = 2;
            if (menuScaleIndex < 2)
                menuScaleIndex = 30;

            menuScale = menuScaleIndex / 10f;

            GetIndex("Change Menu Scale").overlapText = "Change Menu Scale <color=grey>[</color><color=green>" + menuScale + "</color><color=grey>]</color>";
        }

        private static int notificationScaleIndex = 6;
        public static void ChangeNotificationScale(bool positive = true)
        {
            if (positive)
                notificationScaleIndex++;
            else
                notificationScaleIndex--;

            if (notificationScaleIndex > 20)
                notificationScaleIndex = 1;
            if (notificationScaleIndex < 1)
                notificationScaleIndex = 20;

            notificationScale = notificationScaleIndex * 5;

            GetIndex("Change Notification Scale").overlapText = "Change Notification Scale <color=grey>[</color><color=green>" + notificationScaleIndex + "</color><color=grey>]</color>";
        }

        private static int arraylistScaleIndex = 4;
        public static void ChangeArraylistScale(bool positive = true)
        {
            if (positive)
                arraylistScaleIndex++;
            else
                arraylistScaleIndex--;

            if (arraylistScaleIndex > 20)
                arraylistScaleIndex = 1;
            if (arraylistScaleIndex < 1)
                arraylistScaleIndex = 20;

            arraylistScale = arraylistScaleIndex * 5;

            GetIndex("Change Arraylist Scale").overlapText = "Change Arraylist Scale <color=grey>[</color><color=green>" + arraylistScaleIndex + "</color><color=grey>]</color>";
        }

        private static int overlayScaleIndex = 6;
        public static void ChangeOverlayScale(bool positive = true)
        {
            if (positive)
                overlayScaleIndex++;
            else
                overlayScaleIndex--;

            if (overlayScaleIndex > 20)
                overlayScaleIndex = 1;
            if (overlayScaleIndex < 1)
                overlayScaleIndex = 20;

            overlayScale = overlayScaleIndex * 5;

            GetIndex("Change Overlay Scale").overlapText = "Change Overlay Scale <color=grey>[</color><color=green>" + overlayScaleIndex + "</color><color=grey>]</color>";
        }

        private static int modifyWhatId;
        public static void CMTRed(bool increase = true)
        {
            int r = 0;
            switch (modifyWhatId)
            {
                case 0:
                    r = (int)Math.Round(backgroundColor.GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(0, new Color(r / 10f, backgroundColor.GetColor(0).g, backgroundColor.GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                    break;
                case 1:
                    r = (int)Math.Round(backgroundColor.GetColor(1).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(1, new Color(r / 10f, backgroundColor.GetColor(1).g, backgroundColor.GetColor(1).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                    break;
                case 2:
                    r = (int)Math.Round(buttonColors[0].GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(0, new Color(r / 10f, buttonColors[0].GetColor(0).g, buttonColors[0].GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 3:
                    r = (int)Math.Round(buttonColors[0].GetColor(1).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(1, new Color(r / 10f, buttonColors[0].GetColor(1).g, buttonColors[0].GetColor(1).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                    break;
                case 4:
                    r = (int)Math.Round(buttonColors[1].GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(0, new Color(r / 10f, buttonColors[1].GetColor(0).g, buttonColors[1].GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 5:
                    r = (int)Math.Round(buttonColors[1].GetColor(1).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(1, new Color(r / 10f, buttonColors[1].GetColor(1).g, buttonColors[1].GetColor(1).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                    break;
                case 6:
                    r = (int)Math.Round(textColors[0].GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[0].SetColors(new Color(r / 10f, textColors[0].GetColor(0).g, textColors[0].GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 7:
                    r = (int)Math.Round(textColors[1].GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    textColors[1].SetColors(new Color(r / 10f, textColors[1].GetColor(0).g, textColors[1].GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 8:
                    r = (int)Math.Round(textColors[2].GetColor(0).r * 10f);

                    if (increase)
                        r++;
                    else
                        r--;

                    r %= 11;
                    if (r < 0)
                        r = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[2].SetColors(new Color(r / 10f, textColors[2].GetColor(0).g, textColors[2].GetColor(0).b));

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }
        public static void CMTGreen(bool increase = true)
        {
            int g = 0;
            switch (modifyWhatId)
            {
                case 0:
                    g = (int)Math.Round(backgroundColor.GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(0, new Color(backgroundColor.GetColor(0).r, g / 10f, backgroundColor.GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                    break;
                case 1:
                    g = (int)Math.Round(backgroundColor.GetColor(1).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(1, new Color(backgroundColor.GetColor(1).r, g / 10f, backgroundColor.GetColor(1).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                    break;
                case 2:
                    g = (int)Math.Round(buttonColors[0].GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(0, new Color(buttonColors[0].GetColor(0).r, g / 10f, buttonColors[0].GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 3:
                    g = (int)Math.Round(buttonColors[0].GetColor(1).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(1, new Color(buttonColors[0].GetColor(1).r, g / 10f, buttonColors[0].GetColor(1).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                    break;
                case 4:
                    g = (int)Math.Round(buttonColors[1].GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(0, new Color(buttonColors[1].GetColor(0).r, g / 10f, buttonColors[1].GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 5:
                    g = (int)Math.Round(buttonColors[1].GetColor(1).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(1, new Color(buttonColors[1].GetColor(1).r, g / 10f, buttonColors[1].GetColor(1).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                    break;
                case 6:
                    g = (int)Math.Round(textColors[0].GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[0].SetColors(new Color(textColors[0].GetColor(0).r, g / 10f, textColors[0].GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 7:
                    g = (int)Math.Round(textColors[1].GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[1].SetColors(new Color(textColors[1].GetColor(0).r, g / 10f, textColors[1].GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 8:
                    g = (int)Math.Round(textColors[2].GetColor(0).g * 10f);

                    if (increase)
                        g++;
                    else
                        g--;

                    g %= 11;
                    if (g < 0)
                        g = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[2].SetColors(new Color(textColors[2].GetColor(0).r, g / 10f, textColors[2].GetColor(0).b));

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }
        public static void CMTBlue(bool increase = true)
        {
            int b = 0;
            switch (modifyWhatId)
            {
                case 0:
                    b = (int)Math.Round(backgroundColor.GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(0, new Color(backgroundColor.GetColor(0).r, backgroundColor.GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>";
                    break;
                case 1:
                    b = (int)Math.Round(backgroundColor.GetColor(1).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        backgroundColor.SetColor(1, new Color(backgroundColor.GetColor(1).r, backgroundColor.GetColor(1).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>";
                    break;
                case 2:
                    b = (int)Math.Round(buttonColors[0].GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(0, new Color(buttonColors[0].GetColor(0).r, buttonColors[0].GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 3:
                    b = (int)Math.Round(buttonColors[0].GetColor(1).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[0].SetColor(1, new Color(buttonColors[0].GetColor(1).r, buttonColors[0].GetColor(1).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>";
                    break;
                case 4:
                    b = (int)Math.Round(buttonColors[1].GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(0, new Color(buttonColors[1].GetColor(0).r, buttonColors[1].GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 5:
                    b = (int)Math.Round(buttonColors[1].GetColor(1).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonColors[1].SetColor(1, new Color(buttonColors[1].GetColor(1).r, buttonColors[1].GetColor(1).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>";
                    break;
                case 6:
                    b = (int)Math.Round(textColors[0].GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[0].SetColors(new Color(textColors[0].GetColor(0).r, textColors[0].GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>";
                    break;
                case 7:
                    b = (int)Math.Round(textColors[1].GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[1].SetColors(new Color(textColors[1].GetColor(0).r, textColors[1].GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>";
                    break;
                case 8:
                    b = (int)Math.Round(textColors[2].GetColor(0).b * 10f);

                    if (increase)
                        b++;
                    else
                        b--;

                    b %= 11;
                    if (b < 0)
                        b = 10;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColors[2].SetColors(new Color(textColors[2].GetColor(0).r, textColors[2].GetColor(0).g, b / 10f));

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }

        private static int rememberdirectory;
        public static void CustomMenuTheme()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt"))
                UpdateWriteCustomTheme();
            
            UpdateReadCustomTheme();
        }

        public static void ChangeCustomMenuTheme()
        {
            rememberdirectory = pageNumber;
            CustomMenuThemePage();
        }

        public static void CustomMenuThemePage()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Custom Menu Theme", method = () => ExitCustomMenuTheme(), isTogglable = false, toolTip = "Returns you back to the settings menu." },
                new ButtonInfo { buttonText = "Background", method = () => CMTBackground(), isTogglable = false, toolTip = "Choose what segment of the background you would like to modify." },
                new ButtonInfo { buttonText = "Buttons", method = () => CMTButton(), isTogglable = false, toolTip = "Choose what segment of the button you would like to modify." },
                new ButtonInfo { buttonText = "Text", method = () => CMTText(), isTogglable = false, toolTip = "Choose what segment of the text you would like to modify." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void CMTBackground()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Background", method = () => CustomMenuThemePage(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = () => CMTBackgroundFirst(), isTogglable = false, toolTip = "Change the color of the first color of the background." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTBackgroundSecond(), isTogglable = false, toolTip = "Change the color of the second color of the background." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTBackgroundFirst()
        {
            modifyWhatId = 0;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTBackground(), isTogglable = false, toolTip = "Returns you back to the background menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTBackgroundSecond()
        {
            modifyWhatId = 1;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTBackground(), isTogglable = false, toolTip = "Returns you back to the background menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the second color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the second color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(backgroundColor.GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the second color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(backgroundColor.GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void CMTButton()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Buttons", method = CustomMenuThemePage, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Enabled", method = CMTButtonEnabled, isTogglable = false, toolTip = "Choose what type of button color to modify." },
                new ButtonInfo { buttonText = "Disabled", method = CMTButtonDisabled, isTogglable = false, toolTip = "Change the color of the second color of the background." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonEnabled()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Enabled", method = CMTButton, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = CMTButtonEnabledFirst, isTogglable = false, toolTip = "Change the color of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTButtonEnabledSecond(), isTogglable = false, toolTip = "Change the color of the second color of the enabled button color." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonDisabled()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Enabled", method = () => CMTButton(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = () => CMTButtonDisabledFirst(), isTogglable = false, toolTip = "Change the color of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Second Color", method = () => CMTButtonDisabledSecond(), isTogglable = false, toolTip = "Change the color of the second color of the disabled button color." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonEnabledFirst()
        {
            modifyWhatId = 4;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Returns you back to the enabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonEnabledSecond()
        {
            modifyWhatId = 5;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Returns you back to the enabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[1].GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[1].GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonDisabledFirst()
        {
            modifyWhatId = 2;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTButtonDisabled(), isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonDisabledSecond()
        {
            modifyWhatId = 3;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = CMTButtonDisabled, isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(buttonColors[0].GetColor(1).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonColors[0].GetColor(1)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void CMTText()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Text", method = CustomMenuThemePage, isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Title", method = CMTTextTitle, isTogglable = false, toolTip = "Change the color of the title." },
                new ButtonInfo { buttonText = "Enabled", method = CMTTextEnabled, isTogglable = false, toolTip = "Change the color of the enabled text." },
                new ButtonInfo { buttonText = "Disabled", method = CMTTextDisabled, isTogglable = false, toolTip = "Change the color of the disabled text." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextTitle()
        {
            modifyWhatId = 6;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Title", method = CMTText, isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the title color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the title color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the title color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[0].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextEnabled()
        {
            modifyWhatId = 8;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the enabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the enabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[2].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the enabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[2].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextDisabled()
        {
            modifyWhatId = 7;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>", method =() => CMTRed(), enableMethod =() => CMTRed(), disableMethod =() => CMTRed(false), incremental = true, isTogglable = false, toolTip = "Change the red of the disabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>", method =() => CMTGreen(), enableMethod =() => CMTGreen(), disableMethod =() => CMTGreen(false), incremental = true, isTogglable = false, toolTip = "Change the green of the disabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(textColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>", method =() => CMTBlue(), enableMethod =() => CMTBlue(), disableMethod =() => CMTBlue(false), incremental = true, isTogglable = false, toolTip = "Change the blue of the disabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColors[1].GetColor(0)) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void ExitCustomMenuTheme()
        {
            currentCategoryName = "Menu Settings";
            pageNumber = rememberdirectory;
        }

        public static void UpdateReadCustomTheme()
        {
            string[] linesplit = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt").Split("\n");

            string[] a = linesplit[0].Split(",");
            backgroundColor.SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[1].Split(",");
            backgroundColor.SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[2].Split(",");
            buttonColors[0].SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[3].Split(",");
            buttonColors[0].SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[4].Split(",");
            buttonColors[1].SetColor(0, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[5].Split(",");
            buttonColors[1].SetColor(1, new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));

            a = linesplit[6].Split(",");
            textColors[0].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[7].Split(",");
            textColors[1].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
            a = linesplit[8].Split(",");
            textColors[2].SetColors(new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255));
        }

        public static void UpdateWriteCustomTheme()
        {
            Color[] clrs = {
                backgroundColor.GetColor(0),
                backgroundColor.GetColor(1),
                buttonColors[0].GetColor(0),
                buttonColors[0].GetColor(1),
                buttonColors[1].GetColor(0),
                buttonColors[1].GetColor(1),
                textColors[0].GetColor(0),
                textColors[1].GetColor(0),
                textColors[2].GetColor(0)
            };

            string output = "";
            foreach (Color clr in clrs)
            {
                if (output != "")
                    output += "\n";

                output += Math.Round(Mathf.Round(clr.r * 10) / 10 * 255f) + "," + Math.Round(Mathf.Round(clr.g * 10) / 10 * 255f) + "," + Math.Round(Mathf.Round(clr.b * 10) / 10 * 255f);
            }
            File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomThemeColor.txt", output);
        }

        public static void FixTheme()
        {
            themeType--;
            ChangeMenuTheme();
        }

        public static void CustomMenuBackground()
        {
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuBackground.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuBackground.txt", "");

            if (File.Exists($"{PluginInfo.BaseDirectory}/MenuBG.png"))
                File.Delete($"{PluginInfo.BaseDirectory}/MenuBG.png");
            
            doCustomMenuBackground = true;
            customMenuBackgroundImage = LoadTextureFromURL(File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuBackground.txt"), "MenuBG.png");
            ReloadMenu();
        }

        public static void FixMenuBackground()
        {
            customMenuBackgroundImage = null;
            doCustomMenuBackground = false;
        }

        public static void ChangePageType(bool positive = true)
        {
            if (positive)
                pageButtonType++;
            else
                pageButtonType--;

            if (pageButtonType > 6)
                pageButtonType = 1;

            if (pageButtonType < 1)
                pageButtonType = 6;

            buttonOffset = pageButtonType == 1 ? 2 : 0;
        }

        public static void ChangePageSize(bool positive = true)
        {
            if (positive)
                _pageSize++;
            else
                _pageSize--;

            if (_pageSize > 16)
                _pageSize = 4;

            if (_pageSize < 4)
                _pageSize = 16;

            GetIndex("Change Page Size").overlapText = $"Change Page Size <color=grey>[</color><color=green>{_pageSize}</color><color=grey>]</color>";
        }

        public static void ChangeArrowType(bool positive = true)
        {
            if (positive)
                arrowType++;
            else
                arrowType--;

            arrowType %= arrowTypes.Length;
            if (arrowType < 0)
                arrowType = arrowTypes.Length - 1;
        }

        public static void ChangeFontType(bool positive = true)
        {
            if (positive)
                fontCycle++;
            else
                fontCycle--;

            fontCycle %= 14;
            if (fontCycle < 0)
                fontCycle = 13;

            switch (fontCycle) {
                case 0:
                    activeFont = AgencyFB;
                    return;
                case 1:
                    activeFont = Arial;
                    return;
                case 2:
                    activeFont = Verdana;
                    return;
                case 3:
                    if (GTFont == null)
                        GTFont = LoadAsset<Font>("Utopium");

                    activeFont = GTFont;
                    return;
                case 4:
                    activeFont = ComicSans;
                    return;
                case 5:
                    activeFont = Consolas;
                    return;
                case 6:
                    activeFont = Candara;
                    return;
                case 7:
                    activeFont = MSGothic;
                    return;
                case 8:
                    activeFont = Impact;
                    return;
                case 9:
                    activeFont = SimSun;
                    return;
                case 10:
                    if (Minecraft == null)
                        Minecraft = LoadAsset<Font>("Minecraft");

                    activeFont = Minecraft;
                    return;
                case 11:
                    if (Terminal == null)
                        Terminal = LoadAsset<Font>("Terminal");

                    activeFont = Terminal;
                    return;
                case 12:
                    if (OpenDyslexic == null)
                        OpenDyslexic = LoadAsset<Font>("OpenDyslexic");

                    activeFont = OpenDyslexic;
                    return;
                case 13:
                    if (Taiko == null)
                        Taiko = LoadAsset<Font>("Taiko");

                    activeFont = Taiko;
                    return;
            }
        }

        public static float fontTime;
        public static void ChangeFontRapid()
        {
            if (Time.time > fontTime)
            {
                ChangeFontType();
                fontTime = Time.time + 0.4f;

                ReloadMenu();
            }
        }

        public static void ChangeFontStyleType(bool positive = true)
        {
            if (positive)
                fontStyleType++;
            else
                fontStyleType--;

            fontStyleType %= 4;
            if (fontStyleType < 0)
                fontStyleType = 3;

            activeFontStyle = (FontStyle)fontStyleType;
        }

        public static int inputTextColorInt = 3;
        public static void ChangeInputTextColor(bool positive = true)
        {
            string[] textColors = {
                "Red",
                "Orange",
                "Yellow",
                "Green",
                "Blue",
                "Cyan",
                "Purple",
                "Pink",
                "White",
                "Grey",
                "Black",
                "Rose"
            };
            string[] realinputcolor = {
                "red",
                "#ff8000",
                "yellow",
                "green",
                "blue",
                "cyan",
                "#7700ff",
                "magenta",
                "white",
                "grey",
                "black",
                "#ff005d"
            };

            if (positive)
                inputTextColorInt++;
            else
                inputTextColorInt--;

            inputTextColorInt %= realinputcolor.Length;
            if (inputTextColorInt < 0)
                inputTextColorInt = realinputcolor.Length - 1;

            inputTextColor = realinputcolor[inputTextColorInt];
            GetIndex("Change Input Text Color").overlapText = $"Change Input Text Color <color=grey>[</color><color=green>{textColors[inputTextColorInt]}</color><color=grey>]</color>";
        }

        public static void ChangePCUI(bool positive = true)
        {
            if (positive)
                pcbg++;
            else
                pcbg--;

            pcbg %= 6;
            if (pcbg < 0)
                pcbg = 5;
        }

        public static void ChangeJoystickMenuPosition(bool positive = true)
        {
            if (positive)
                joystickMenuPosition++;
            else
                joystickMenuPosition--;

            joystickMenuPosition %= joystickMenuPositions.Length;
            if (joystickMenuPosition < 0)
                joystickMenuPosition = joystickMenuPositions.Length - 1;
        }

        public static void ChangeNotificationTime(bool positive = true)
        {
            if (positive)
                notificationDecayTime += 1000;
            else
                notificationDecayTime -= 1000;

            notificationDecayTime %= 6000;
            if (notificationDecayTime < 0)
                notificationDecayTime = 5000;

            GetIndex("Change Notification Time").overlapText = "Change Notification Time <color=grey>[</color><color=green>" + notificationDecayTime / 1000 + "</color><color=grey>]</color>";
        }

        public static readonly Dictionary<string, string> notificationSounds = new Dictionary<string, string>
        {
            { "None",        "none" },
            { "Pop",         "pop" },
            { "Ding",        "ding" },
            { "Twitter",     "twitter" },
            { "Discord",     "discord" },
            { "Whatsapp",    "whatsapp" },
            { "Grindr",      "grindr" },
            { "iOS",         "ios" },
            { "XP Notify",   "xpnotify" },
            { "XP Ding",   "xptrueding" },
            { "XP Question", "xpding" },
            { "XP Error",    "xperror" },
            { "Roblox Bass", "robloxbass" },
            { "Oculus",      "oculus" },
            { "Nintendo",    "nintendo" }
        };

        public static void ChangeNotificationSound(bool positive = true, bool fromMenu = false)
        {
            if (positive)
                notificationSoundIndex++;
            else
                notificationSoundIndex--;

            notificationSoundIndex %= notificationSounds.Keys.Count;
            if (notificationSoundIndex < 0)
                notificationSoundIndex = notificationSounds.Keys.Count - 1;

            GetIndex("Change Notification Sound").overlapText = "Change Notification Sound <color=grey>[</color><color=green>" + notificationSounds.Keys.ToArray()[notificationSoundIndex] + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                audioMgr.GetComponent<AudioSource>().Stop();
                NotificationManager.PlayNotificationSound();
            }
        }

        public static void ChangeNarrationVoice(bool positive = true)
        {
            string[] narratorNames = {
                "Default",
                "Kimberly",
                "Brian",
                "Matthew",
                "Joey",
                "Justin",
                "Cristiano",
                "Giorgio",
                "Ewa"
            };

            if (positive)
                narratorIndex++;
            else
                narratorIndex--;

            narratorIndex %= narratorNames.Length;
            if (narratorIndex < 0)
                narratorIndex = narratorNames.Length - 1;

            GetIndex("Change Narration Voice").overlapText = "Change Narration Voice <color=grey>[</color><color=green>" + narratorNames[narratorIndex] + "</color><color=grey>]</color>";
            narratorName = narratorNames[narratorIndex];
        }

        public static void KickToSpecificRoom()
        {
            if (Time.time < timeMenuStarted + 5f)
            {
                GetIndex("Kick to Specific Room").enabled = false;
                return;
            }

            PromptText("What would you like the room code to be?", () => Fun.specificRoom = keyboardInput.ToUpper(), () => Toggle("Kick to Specific Room"), "Done", "Cancel");
        }
        public static void ChangePointerPosition(bool positive = true)
        {
            Vector3[] pointerPos = {
                new Vector3(0f, -0.1f, 0f),
                new Vector3(0f, -0.1f, -0.15f),
                new Vector3(0f, 0.1f, -0.05f),
                new Vector3(0f, 0.0666f, 0.1f)
            };

            if (positive)
                pointerIndex++;
            else
                pointerIndex--;

            pointerIndex %= pointerPos.Length;
            if (pointerIndex < 0)
                pointerIndex = pointerPos.Length - 1;

            pointerOffset = pointerPos[pointerIndex];
            try { reference.transform.localPosition = pointerOffset; } catch { }
        }

        // Credits to Scintilla for the idea
        public static void ChangeGunVariation(bool positive = true)
        {
            string[] VariationNames = {
                "Default",
                "Lightning",
                "Wavy",
                "Blocky",
                "Zigzag",
                "Spring",
                "Bouncy",
                "Audio",
                "Bezier"
            };

            if (positive)
                gunVariation++;
            else
                gunVariation--;

            gunVariation %= VariationNames.Length;
            if (gunVariation < 0)
                gunVariation = VariationNames.Length - 1;

            GetIndex("Change Gun Variation").overlapText = "Change Gun Variation <color=grey>[</color><color=green>" + VariationNames[gunVariation] + "</color><color=grey>]</color>";
        }

        public static void ChangeGunDirection(bool positive = true)
        {
            string[] DirectionNames = {
                "Default",
                "Legacy",
                "Laser",
                "Finger",
                "Face"
            };

            if (positive)
                GunDirection++;
            else
                GunDirection--;

            GunDirection %= DirectionNames.Length;
            if (GunDirection < 0)
                GunDirection = DirectionNames.Length - 1;

            GetIndex("Change Gun Direction").overlapText = "Change Gun Direction <color=grey>[</color><color=green>" + DirectionNames[GunDirection] + "</color><color=grey>]</color>";
        }

        private static int gunLineQualityIndex = 2;
        public static void ChangeGunLineQuality(bool positive = true)
        {
            string[] Names = {
                "Potato",
                "Low",
                "Normal",
                "High",
                "Extreme"
            };

            int[] Qualities = {
                10,
                25,
                50,
                100,
                250
            };

            if (positive)
                gunLineQualityIndex++;
            else
                gunLineQualityIndex--;

            gunLineQualityIndex %= Names.Length;
            if (gunLineQualityIndex < 0)
                gunLineQualityIndex = Names.Length - 1;

            GunLineQuality = Qualities[gunLineQualityIndex];
            GetIndex("Change Gun Line Quality").overlapText = "Change Gun Line Quality <color=grey>[</color><color=green>" + Names[gunLineQualityIndex] + "</color><color=grey>]</color>";
        }

        public static void FreezePlayerInMenu()
        {
            if (physicalMenu ? isMenuButtonHeld : menu != null)
            {
                if (closePosition == Vector3.zero)
                    closePosition = GorillaTagger.Instance.rigidbody.transform.position;
                else
                    GorillaTagger.Instance.rigidbody.transform.position = closePosition;
                GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
            } else
                closePosition = Vector3.zero;
        }

        public static bool currentmentalstate;
        public static void FreezeRigInMenu()
        {
            if (menu != null)
            {
                if (!currentmentalstate)
                {
                    currentmentalstate = true;
                    VRRig.LocalRig.enabled = false;
                }
            }
            else
            {
                if (currentmentalstate)
                {
                    currentmentalstate = false;
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void DisorganizeMenu()
        {
            if (!disorganized)
            {
                disorganized = true;
                foreach (ButtonInfo[] buttonArray in Buttons.buttons)
                {
                    if (buttonArray.Length > 0)
                    {
                        for (int i = 0; i < buttonArray.Length; i++)
                            Buttons.buttons[0] = Buttons.buttons[0].Concat(new[] { buttonArray[i] }).ToArray();

                        Array.Clear(buttonArray, 0, buttonArray.Length);
                    }
                }
            }
        }

        public static void AnnoyingModeOff()
        {
            annoyingMode = false;
            themeType--;
            ChangeMenuTheme();
        }

        public static void DisablePageButtons()
        {
            if (GetIndex("Joystick Menu").enabled) {
                disablePageButtons = true;
            } else
            {
                GetIndex("Disable Page Buttons").enabled = false;
                NotificationManager.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> <color=white>Disable Page Buttons can only be used when using Joystick Menu.</color>");
            }
        }

        public static void CustomMenuName()
        {
            doCustomName = true;
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt", "Your Text Here");
            
            customMenuName = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt");
        }

        private static bool lastFocused;
        public static void CheckFocus()
        {
            if (!Application.isFocused && lastFocused && Time.time > timeMenuStarted + 5f)
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not focused on Gorilla Tag. Voice transcription mods will not function. Please focus/click on the game.");

            lastFocused = Application.isFocused;
        }

        // Thanks to kingofnetflix for inspiration and support with voice recognition
        private static KeywordRecognizer mainPhrases;
        private static KeywordRecognizer modPhrases;
        private static string[] keyWords = { "jarvis", "ii", "i i", "eye eye", "siri", "google", "alexa", "dummy", "computer", "stinky", "silly", "stupid", "console", "go go gadget", "monika", "wikipedia", "gideon", "a i", "ai", "a.i", "chat gpt", "chatgpt" };
        private static readonly string[] cancelKeywords = { "nevermind", "cancel", "never mind", "stop", "i hate you", "die" };
        public static void VoiceRecognitionOn()
        {
            mainPhrases = new KeywordRecognizer(keyWords);
            mainPhrases.OnPhraseRecognized += ModRecognition;
            mainPhrases.Start();
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt"))
                File.WriteAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt", keyWords);
            keyWords = File.ReadAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt");
        }

        private static Coroutine timeoutCoroutine;
        public static void ModRecognition(PhraseRecognizedEventArgs args)
        {
            mainPhrases.Stop();

            if (!GetIndex("Chain Voice Commands").enabled)
                timeoutCoroutine = CoroutineManager.RunCoroutine(Timeout(string.Empty));
            
            List<string> rawbuttonnames = cancelKeywords.ToList();

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    string buttonName = v.overlapText ?? v.buttonText;

                    if (buttonName.Contains(" <color"))
                        buttonName = buttonName.Split(" <color")[0];

                    rawbuttonnames.Add(buttonName);
                }
            }


            modPhrases = new KeywordRecognizer(rawbuttonnames.ToArray());
            modPhrases.OnPhraseRecognized += ExecuteVoiceCommand;
            modPhrases.Start();

            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/select.ogg", "Audio/Menu/select.ogg"), buttonClickVolume / 10f);
            
            NotificationManager.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
        }

        public static void ExecuteVoiceCommand(PhraseRecognizedEventArgs args)
        {
            if (!GetIndex("Chain Voice Commands").enabled)
            {
                modPhrases.Stop();
                mainPhrases.Start();
                CoroutineManager.EndCoroutine(timeoutCoroutine);
            }

            if (cancelKeywords.Contains(args.text))
            {
                CancelModRecognition(args.text);
                return;
            }

            string modTarget = null;
            bool exactMatch = false;

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                if (exactMatch)
                    break;

                foreach (ButtonInfo v in buttonlist)
                {
                    if (exactMatch)
                        break;

                    string buttonName = v.overlapText ?? v.buttonText;

                    if (buttonName.Contains(" <color"))
                        buttonName = buttonName.Split(" <color")[0];

                    if (args.text.ToLower() == buttonName.ToLower())
                    {
                        modTarget = v.buttonText;
                        exactMatch = true;
                    } else
                    {
                        if (args.text.Contains(buttonName.ToLower()))
                            modTarget = v.buttonText;
                    }
                }
            }

            if (modTarget != null)
            {
                ButtonInfo mod = GetIndex(modTarget);
                NotificationManager.SendNotification("<color=grey>[</color><color=" + (mod.enabled ? "red" : "green") + ">VOICE</color><color=grey>]</color> " + (mod.enabled ? "Disabling " : "Enabling ") + (mod.overlapText ?? mod.buttonText) +"...", 3000);
                if (dynamicSounds)
                    Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg"), buttonClickVolume / 10f);
                
                Toggle(modTarget, true, true);
            } else
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> No command found ("+args.text+").", 3000);
                if (dynamicSounds)
                    Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
            }
        }

        public static IEnumerator Timeout(string text)
        {
            yield return new WaitForSeconds(10f);
            CancelModRecognition(text);
        }

        public static void CancelModRecognition(string text)
        {
            modPhrases.Stop();
            mainPhrases.Start();
            try
            {
                CoroutineManager.EndCoroutine(timeoutCoroutine);
            } catch { }
            
            NotificationManager.SendNotification($"<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> {(text == "i hate you" ? "I hate you too." : "Cancelling...")}", 3000);
            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
        }

        public static void VoiceRecognitionOff()
        {
            mainPhrases?.Stop();
            mainPhrases?.Dispose();
            modPhrases?.Stop();
            modPhrases?.Dispose();

            mainPhrases = null;
            modPhrases = null;
        }

        // Thanks to kingofnetflix for inspiration and support with voice recognition
        public static DictationRecognizer drec;
        public static bool listening;
        public static bool debugDictation;
        public static bool restartOnFocus;
        public static void DictationOn()
        {
            ButtonInfo mod = GetIndex("AI Assistant");
            if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt"))
                File.WriteAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt", keyWords);
            keyWords = File.ReadAllLines($"{PluginInfo.BaseDirectory}/iiMenu_Keywords.txt");

            if (Application.platform == RuntimePlatform.WindowsPlayer && Environment.OSVersion.Version.Major < 10)
                PromptSingle("Your version of Windows is too old for this mod to run.", () => mod.enabled = false);
            else if (Application.platform != RuntimePlatform.WindowsPlayer) 
                PromptSingle("You must be on Windows 10 or greater for this mod to run.", () => mod.enabled = false);

            drec = new DictationRecognizer();
            drec.DictationResult += (text, confidence) =>
            {
                LogManager.Log($"Dictation result: {text}");
                if (!listening)
                {
                    if (keyWords.Contains(text.ToLower()))
                    {
                        if (dynamicSounds)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/select.ogg", "Audio/Menu/select.ogg"), buttonClickVolume / 10f);

                        NotificationManager.SendNotification("<color=grey>[</color><color=blue>AI</color><color=grey>]</color> Listening...", 3000);
                        listening = true;
                        return;
                    } else
                    {
                        if (debugDictation)
                            LogManager.LogWarning("Ignoring input as we aren't supposted to be listening");
                    }
                }
                if (listening)
                {
                    if (cancelKeywords.Contains(text.ToLower()))
                    {
                        if (dynamicSounds)
                            Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
                        
                        NotificationManager.SendNotification($"<color=grey>[</color><color=red>AI</color><color=grey>]</color> {(text.ToLower() == "i hate you" ? "I hate you too." : "Cancelling...")}", 3000);
                        listening = false;
                        return;
                    }

                    NotificationManager.SendNotification($"<color=grey>[</color><color=blue>AI</color><color=grey>]</color> Generating response..");
                    CoroutineManager.instance.StartCoroutine(AIManager.AskAI(text));
                }
                else return;
                
                    
            };
            drec.DictationComplete += (completionCause) =>
            {
                if (debugDictation)
                    LogManager.Log($"completion cause: {completionCause}");
                if (!listening)
                {
                    if (Application.isFocused)
                        drec?.Start();
                    else
                        restartOnFocus = true;
                    return;
                }
                if (dynamicSounds)
                    Play2DAudio(LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Menu/close.ogg", "Audio/Menu/close.ogg"), buttonClickVolume / 10f);
                NotificationManager.SendNotification($"<color=grey>[</color><color=red>AI</color><color=grey>]</color> Cancelling...", 3000);
            };
            drec.DictationError += (error, hresult) =>
            {
                if (debugDictation)
                    LogManager.LogError($"Dictation error: {error}");
                if (error.Contains("Dictation support is not enabled on this device"))
                {
                    drec.Stop();
                    drec.Dispose();

                    NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Online Speech Recognition is not enabled on this device. Either open the menu to enable it, or check your internet connection.", 3000);
                    Prompt("Online Speech Recognition is not enabled on your device. Would you like to open the Settings page to enable it?", () => { Process.Start("ms-settings:privacy-speech"); PromptSingle("Once you enable Online Speech Recognition, turn this mod back on!", () => mod.enabled = false, "Ok! :)"); }, () => PromptSingle("You will not be able to use this mod until you enable Online Speech Recognition.", () => mod.enabled = false, "Ok :("));
                }
            };
            drec.DictationHypothesis += (text) =>
            {
                if (debugDictation)
                    LogManager.Log($"Hypothesis: {text}");

                if (listening)
                {
                    NotificationManager.ClearAllNotifications();
                    NotificationManager.SendNotification($"<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> {text}");
                }
            };
            drec?.Start();
        }

        public static void FocusCheck()
        {
            CheckFocus();
            if (!Application.isFocused)
                restartOnFocus = true;
            if (Application.isFocused && restartOnFocus)
            {
                drec?.Start();
                restartOnFocus = false;
            }
        }
        public static void DictationOff()
        {
            listening = false;
            drec?.Stop();
            drec?.Dispose();
        }

        public static GameObject selectObject;
        public static VRRig lastTarget;
        public static bool lastTriggerSelect;
        public static void PlayerSelect()
        {
            bool leftHand = rightHand || (bothHands && ControllerInputPoller.instance.rightControllerSecondaryButton);

            var targetHand = leftHand ? TrueLeftHand() : TrueRightHand();
            bool canSelect = NetworkSystem.Instance.InRoom && menu != null && reference != null && Vector3.Distance(menu.transform.position, reference.transform.position) > 0.5f;

            if (canSelect)
            {
                if (selectObject == null)
                    selectObject = new GameObject("iiMenu_PingLine");

                Color targetColor = GetIndex("Swap GUI Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor();
                Color lineColor = targetColor;
                lineColor.a = 0.15f;

                LineRenderer pingLine = selectObject.GetOrAddComponent<LineRenderer>();
                pingLine.material.shader = Shader.Find("GUI/Text Shader");
                pingLine.startColor = lineColor;
                pingLine.endColor = lineColor;
                pingLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                pingLine.endWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
                pingLine.positionCount = 2;
                pingLine.useWorldSpace = true;
                if (smoothLines)
                {
                    pingLine.numCapVertices = 10;
                    pingLine.numCornerVertices = 5;
                }

                Vector3 StartPosition = SwapGunHand ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position;
                Vector3 Direction = targetHand.forward;

                Physics.SphereCast(StartPosition + Direction / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f), 0.15f, Direction, out var Ray, 512f, NoInvisLayerMask());
                Vector3 EndPosition = Ray.point == Vector3.zero ? StartPosition + (Direction * 512f) : Ray.point;

                pingLine.SetPosition(0, StartPosition);
                pingLine.SetPosition(1, EndPosition);

                VRRig rigTarget = Ray.collider.GetComponentInParent<VRRig>();
                if (Ray.collider != null && rigTarget != null && !rigTarget.IsLocal())
                {
                    if (lastTarget != null && lastTarget != rigTarget)
                    {
                        lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                            lastTarget.mainSkin.material.color = lastTarget.playerColor;

                        lastTarget = null;
                    }

                    if (lastTarget == null)
                    {
                        Visuals.FixRigMaterialESPColors(rigTarget);

                        rigTarget.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        rigTarget.mainSkin.material.color = targetColor;

                        GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);

                        lastTarget = rigTarget;
                    }

                    bool trigger = leftHand ? leftTrigger > 0.5f : rightTrigger > 0.5f;

                    if (trigger && !lastTriggerSelect)
                    {
                        VRRig.LocalRig.PlayHandTapLocal(50, leftHand, 0.4f);
                        GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);

                        NavigatePlayer(GetPlayerFromVRRig(rigTarget));
                        ReloadMenu();

                        NotificationManager.SendNotification($"<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> <color=white>Selected player {GetPlayerFromVRRig(rigTarget).NickName}.</color>");
                    }

                    lastTriggerSelect = trigger;
                } else
                {
                    if (lastTarget != null)
                    {
                        lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                            lastTarget.mainSkin.material.color = lastTarget.playerColor;

                        lastTarget = null;
                    }
                }
            } else
            {
                if (selectObject != null)
                {
                    Object.Destroy(selectObject);
                    selectObject = null;
                }

                if (lastTarget != null)
                {
                    lastTarget.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                    if (lastTarget.mainSkin.material.name.Contains("gorilla_body"))
                        lastTarget.mainSkin.material.color = lastTarget.playerColor;

                    lastTarget = null;
                }

                lastTriggerSelect = false;
            }
        }

        public static string SavePreferencesToText()
        {
            string seperator = ";;";

            string enabledtext = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled && v.buttonText != "Save Preferences")
                    {
                        if (enabledtext == "")
                            enabledtext += v.buttonText;
                        else
                            enabledtext += seperator + v.buttonText;
                    }
                }
            }

            string favoritetext = "";
            foreach (string fav in favorites)
            {
                if (favoritetext == "")
                    favoritetext += fav;
                else
                    favoritetext += seperator + fav;
            }

            string[] settings = {
                Movement.platformMode.ToString(),
                Movement.platformShape.ToString(),
                Movement.flySpeedCycle.ToString(),
                Movement.longarmCycle.ToString(),
                Movement.speedboostCycle.ToString(),
                Projectiles.projmode.ToString(),
                Movement.timerPowerIndex.ToString(),
                Projectiles.shootCycle.ToString(),
                pointerIndex.ToString(),
                Advantages.tagAuraIndex.ToString(),
                notificationDecayTime.ToString(),
                fontStyleType.ToString(),
                arrowType.ToString(),
                pcbg.ToString(),
                Important.reconnectDelay.ToString(),
                Safety.fpsSpoofValue.ToString(),
                buttonClickIndex.ToString(),
                buttonClickVolume.ToString(),
                Safety.antiReportRangeIndex.ToString(),
                Advantages.tagRangeIndex.ToString(),
                Sound.BindMode.ToString(),
                Movement.driveInt.ToString(),
                langInd.ToString(),
                inputTextColorInt.ToString(),
                Movement.pullPowerInt.ToString(),
                notificationSoundIndex.ToString(),
                Visuals.PerformanceModeStepIndex.ToString(),
                gunVariation.ToString(),
                GunDirection.ToString(),
                narratorIndex.ToString(),
                Movement.predInt.ToString(),
                gunLineQualityIndex.ToString(),
                Projectiles.projDebounceIndex.ToString(),
                Projectiles.red.ToString(),
                Projectiles.green.ToString(),
                Projectiles.blue.ToString(),
                Safety.rankIndex.ToString(),
                Overpowered.snowballScale.ToString(),
                Overpowered.lagIndex.ToString(),
                Fun.blockDebounceIndex.ToString(),
                Fun.nameCycleIndex.ToString(),
                menuScaleIndex.ToString(),
                soundId.ToString(),
                Fun.targetQuestScore.ToString(),
                notificationScaleIndex.ToString(),
                overlayScaleIndex.ToString(),
                arraylistScaleIndex.ToString(),
                ((int)MathF.Ceiling(playTime)).ToString(),
                PhotonNetwork.LocalPlayer?.UserId ?? "null",
                _pageSize.ToString(),
                Overpowered.snowballMultiplicationFactor.ToString(),
                menuButtonIndex.ToString(),
                Safety.targetElo.ToString(),
                Safety.targetBadge.ToString(),
                Movement.playspaceAbuseIndex.ToString(),
                Movement.wallWalkStrengthIndex.ToString(),
                Fun.headSpinIndex.ToString(),
                Movement.macroPlaybackRangeIndex.ToString(),
                joystickMenuPosition.ToString(),
                Movement.multiplicationAmount.ToString()
            };

            string settingstext = string.Join(seperator, settings);

            string bindingtext = "";
            foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
            {
                if (bindingtext != "")
                    bindingtext += "~~";

                string toAppend = Bind.Key;
                foreach (string modName in Bind.Value)
                    toAppend += seperator + modName;

                bindingtext += toAppend;
            }

            string quickActionString = string.Join(seperator, quickActions);
            
            string rebindingtext = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.rebindKey != "")
                    {
                        if (rebindingtext == "")
                            rebindingtext += v.buttonText + ";" + v.rebindKey;
                        else
                            rebindingtext += seperator + v.buttonText + ";" + v.rebindKey;
                    }
                }
            }
            
            string finaltext =
                enabledtext + "\n" +
                favoritetext + "\n" +
                settingstext + "\n" +
                pageButtonType + "\n" +
                themeType + "\n" +
                fontCycle + "\n" +
                bindingtext + "\n" +
                quickActionString + "\n" +
                rebindingtext;

            return finaltext;
        }

        public static void SavePreferences() =>
            File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt", SavePreferencesToText());

        public static int loadingPreferencesFrame;
        public static void LoadPreferencesFromText(string text)
        {
            loadingPreferencesFrame = Time.frameCount;

            Panic();
            string[] textData = text.Split("\n");

            string[] activebuttons = textData[0].Split(";;");
            for (int index = 0; index < activebuttons.Length; index++)
                Toggle(activebuttons[index]);

            string[] favoritesarray = textData[1].Split(";;");
            favorites.Clear();
            foreach (string favorite in favoritesarray)
                favorites.Add(favorite);

            try
            {
                string[] data = textData[2].Split(";;");
                Movement.platformMode = int.Parse(data[0]) - 1;
                Movement.ChangePlatformType();

                Movement.platformShape = int.Parse(data[1]) - 1;
                Movement.ChangePlatformShape();

                Movement.flySpeedCycle = int.Parse(data[2]) - 1;
                Movement.ChangeFlySpeed();

                Movement.longarmCycle = int.Parse(data[3]) - 1;
                Movement.ChangeArmLength();

                Movement.speedboostCycle = int.Parse(data[4]) - 1;
                Movement.ChangeSpeedBoostAmount();

                Projectiles.projmode = int.Parse(data[5]) - 1;
                Projectiles.ChangeProjectile();

                Movement.timerPowerIndex = int.Parse(data[6]) - 1;
                Movement.ChangeTimerSpeed();

                Projectiles.shootCycle = int.Parse(data[7]) - 1;
                Projectiles.ChangeShootSpeed();

                pointerIndex = int.Parse(data[8]) - 1;
                ChangePointerPosition();

                Advantages.tagAuraIndex = int.Parse(data[9]) - 1;
                Advantages.ChangeTagAuraRange();

                notificationDecayTime = int.Parse(data[10]) - 1000;
                ChangeNotificationTime();

                fontStyleType = int.Parse(data[11]) - 1;
                ChangeFontStyleType();

                arrowType = int.Parse(data[12]) - 1;
                ChangeArrowType();

                pcbg = int.Parse(data[13]) - 1;
                ChangePCUI();

                Important.reconnectDelay = int.Parse(data[14]) - 1;
                ChangeReconnectTime();

                Safety.fpsSpoofValue = string.IsNullOrWhiteSpace(data[15]) ? 85 : int.Parse(data[15]) - 5;
                Safety.ChangeFPSSpoofValue();

                buttonClickIndex = int.Parse(data[16]) - 1;
                ChangeButtonSound();

                buttonClickVolume = int.Parse(data[17]) - 1;
                ChangeButtonVolume();

                Safety.antiReportRangeIndex = int.Parse(data[18]) - 1;
                Safety.ChangeAntiReportRange();

                Advantages.tagRangeIndex = int.Parse(data[19]) - 1;
                Advantages.ChangeTagReachDistance();

                Sound.BindMode = int.Parse(data[20]) - 1;
                Sound.SoundBindings();

                Movement.driveInt = int.Parse(data[21]) - 1;
                Movement.ChangeDriveSpeed();

                langInd = int.Parse(data[22]) - 1;
                ChangeMenuLanguage();

                inputTextColorInt = int.Parse(data[23]) - 1;
                ChangeInputTextColor();

                Movement.pullPowerInt = int.Parse(data[24]) - 1;
                Movement.ChangePullModPower();

                notificationSoundIndex = int.Parse(data[25]) - 1;
                ChangeNotificationSound();

                Visuals.PerformanceModeStepIndex = int.Parse(data[26]) - 1;
                Visuals.ChangePerformanceModeVisualStep();

                gunVariation = int.Parse(data[27]) - 1;
                ChangeGunVariation();

                GunDirection = int.Parse(data[28]) - 1;
                ChangeGunDirection();

                narratorIndex = int.Parse(data[29]) - 1;
                ChangeNarrationVoice();

                Movement.predInt = int.Parse(data[30]) - 1;
                Movement.ChangePredictionAmount();

                gunLineQualityIndex = int.Parse(data[31]) - 1;
                ChangeGunLineQuality();

                Projectiles.projDebounceIndex = int.Parse(data[32]) - 1;
                Projectiles.ChangeProjectileDelay();

                Projectiles.red = int.Parse(data[33]) - 1;
                Projectiles.IncreaseRed();

                Projectiles.green = int.Parse(data[34]) - 1;
                Projectiles.IncreaseGreen();

                Projectiles.blue = int.Parse(data[35]) - 1;
                Projectiles.IncreaseBlue();

                Safety.rankIndex = int.Parse(data[36]) - 1;
                Safety.ChangeRankedTier();

                Overpowered.snowballScale = int.Parse(data[37]) - 1;
                Overpowered.ChangeSnowballScale();

                Overpowered.lagIndex = int.Parse(data[38]) - 1;
                Overpowered.ChangeLagPower();

                Fun.blockDebounceIndex = int.Parse(data[39]) - 1;
                Fun.ChangeBlockDelay();

                Fun.cycleSpeedIndex = int.Parse(data[40]) - 1;
                Fun.ChangeCycleDelay();

                menuScaleIndex = int.Parse(data[41]) - 1;
                ChangeMenuScale();

                soundId = int.Parse(data[42]) - 1;
                Sound.IncreaseSoundID();

                Fun.targetQuestScore = int.Parse(data[43]) - 1;
                Fun.ChangeCustomQuestScore();

                notificationScaleIndex = int.Parse(data[44]) - 1;
                ChangeNotificationScale();

                overlayScaleIndex = int.Parse(data[45]) - 1;
                ChangeOverlayScale();

                arraylistScaleIndex = int.Parse(data[46]) - 1;
                ChangeArraylistScale();

                playTime = int.Parse(data[47]);

                Important.oldId = data[48];

                _pageSize = int.Parse(data[49]) - 1;
                ChangePageSize();

                Overpowered.snowballMultiplicationFactor = int.Parse(data[50]) - 1;
                Overpowered.ChangeSnowballMultiplicationFactor();

                menuButtonIndex = int.Parse(data[51]) - 1;
                ChangeMenuButton();

                Safety.targetElo = int.Parse(data[52]) - 100;
                Safety.ChangeELOValue();

                Safety.targetBadge = int.Parse(data[53]) - 1;
                Safety.ChangeBadgeTier();

                Movement.playspaceAbuseIndex = int.Parse(data[54]) - 1;
                Movement.ChangePlayspaceAbuseSpeed();

                Movement.wallWalkStrengthIndex = int.Parse(data[55]) - 1;
                Movement.ChangeWallWalkStrength();

                Fun.headSpinIndex = int.Parse(data[56]) - 1;
                Fun.ChangeHeadSpinSpeed();

                Movement.macroPlaybackRangeIndex = int.Parse(data[57]) - 1;
                Movement.ChangeMacroPlaybackRange();

                joystickMenuPosition = int.Parse(data[58]) - 1;
                ChangeJoystickMenuPosition();

                Movement.multiplicationAmount = int.Parse(data[59]) - 1;
                Movement.MultiplicationAmount();
            }
            catch { LogManager.Log("Save file out of date"); }

            pageButtonType = int.Parse(textData[3]) - 1;
            Toggle("Change Page Type");
            themeType = int.Parse(textData[4]) - 1;
            Toggle("Change Menu Theme");
            fontCycle = int.Parse(textData[5]) - 1;
            Toggle("Change Font Type");

            try
            {
                foreach (string Bindings in textData[6].Split("~~"))
                {
                    if (Bindings.Contains(";;"))
                    {
                        string[] BindData = Bindings.Split(";;");
                        string BindName = BindData[0];

                        List<string> Binds = new List<string>();

                        for (int i = 1; i < BindData.Length; i++)
                        {
                            string ModName = BindData[i];
                            if (GetIndex(ModName) != null)
                                Binds.Add(ModName);
                        }

                        ModBindings[BindName] = Binds;
                    }
                }
            } catch { }

            try
            {
                quickActions.Clear();
                foreach (string quickAction in textData[7].Split(";;"))
                {
                    ButtonInfo button = GetIndex(quickAction);
                    if (button != null)
                        quickActions.Add(quickAction);
                }
            } catch { }
            
            try
            {
                foreach (string bind in textData[8].Split(";;"))
                {
                    string rebindText = bind.Split(";")[0];
                    string rebindKey = bind.Split(";")[1];
                    ButtonInfo button = GetIndex(rebindText);
                    if (button != null)
                        button.rebindKey = rebindKey;
                }
            } catch { }

            hasLoadedPreferences = true;
        }

        public static void LoadPreferences()
        {
            try
            {
                if (!File.Exists($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt"))
                {
                    hasLoadedPreferences = true;
                    return;
                }

                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/iiMenu_Preferences.txt");
                LoadPreferencesFromText(text);
            } catch (Exception e) { LogManager.Log("Error loading preferences: " + e.Message); }
        }

        public static void Panic()
        {
            AnnoyingModeOff();
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled)
                        Toggle(v.buttonText);
                }
            }
        }

        public static void ChangeReconnectTime(bool positive = true)
        {
            if (positive)
                Important.reconnectDelay++;
            else
                Important.reconnectDelay--;

            if (Important.reconnectDelay > 5)
                Important.reconnectDelay = 1;
            if (Important.reconnectDelay < 1)
                Important.reconnectDelay = 5;

            GetIndex("crTime").overlapText = "Change Reconnect Time <color=grey>[</color><color=green>" + Important.reconnectDelay + "</color><color=grey>]</color>";
        }

        public static void ChangeButtonSound(bool positive = true, bool fromMenu = false)
        {
            int[] sounds = {
                8,
                66,
                67,
                84,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                106,
                189,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66,
                66
            };
            string[] buttonSoundNames = {
                "Wood",
                "Keyboard",
                "Default",
                "Bubble",
                "Steal",
                "Anthrax",
                "Lever",
                "Minecraft",
                "Rec Room",
                "Watch",
                "Membrane",
                "Jar",
                "Wall",
                "Slider",
                "Can",
                "Cut",
                "Creamy",
                "Roblox Button",
                "Roblox Tick",
                "Mouse",
                "Valve",
                "Nintendo",
                "Windows"
            };

            if (positive)
                buttonClickIndex++;
            else
                buttonClickIndex--;

            buttonClickIndex %= sounds.Length;
            if (buttonClickIndex < 0)
                buttonClickIndex = sounds.Length - 1;

            buttonClickSound = sounds[buttonClickIndex];
            GetIndex("Change Button Sound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + buttonSoundNames[buttonClickIndex] + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                VRRig.LocalRig.leftHandPlayer.Stop();
                VRRig.LocalRig.rightHandPlayer.Stop();
                PlayButtonSound();
            }
        }

        public static void ChangeButtonVolume(bool positive = true, bool fromMenu = false)
        {
            if (positive)
                buttonClickVolume++;
            else
                buttonClickVolume--;

            buttonClickVolume %= 11;
            if (buttonClickVolume < 0)
                buttonClickVolume = 10;

            GetIndex("Change Button Volume").overlapText = "Change Button Volume <color=grey>[</color><color=green>" + buttonClickVolume + "</color><color=grey>]</color>";

            if (fromMenu)
            {
                VRRig.LocalRig.leftHandPlayer.Stop();
                VRRig.LocalRig.rightHandPlayer.Stop();
                PlayButtonSound();
            }
        }

        public static Material screenRed;
        public static Material screenBlack;
        public static void DisableBoardColors()
        {
            foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
            {
                try
                {
                    JoinTriggerUI ui = joinTrigger.ui;
                    JoinTriggerUITemplate temp = ui.template;

                    if (screenRed == null)
                    {
                        screenRed = new Material(Shader.Find("GorillaTag/UberShader"))
                        {
                            color = new Color32(226, 73, 41, 255)
                        };
                    }

                    if (screenBlack == null)
                    {
                        screenBlack = new Material(Shader.Find("GorillaTag/UberShader"))
                        {
                            color = new Color32(39, 34, 28, 255)
                        };
                    }

                    temp.ScreenBG_AbandonPartyAndSoloJoin = screenRed;
                    temp.ScreenBG_AlreadyInRoom = screenBlack;
                    temp.ScreenBG_Error = screenRed;
                } catch { }
            }

            disableBoardColor = true;
            motd.SetActive(false);
            motdText.SetActive(false);
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoommotdHeadingText").SetActive(true);
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(true);

            int boardIndex = 0;
            for (int i = 0; i < GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
            {
                GameObject v = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                if (v.name.Contains("UnityTempFile"))
                {
                    boardIndex++;
                    if (boardIndex == StumpLeaderboardIndex)
                        v.GetComponent<Renderer>().material = StumpMat;
                }
            }

            boardIndex = 0;
            for (int i = 0; i < GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
            {
                GameObject v = GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                if (v.name.Contains("UnityTempFile"))
                {
                    boardIndex++;
                    if (boardIndex == ForestLeaderboardIndex)
                        v.GetComponent<Renderer>().material = ForestMat;
                }
            }

            foreach (GameObject board in objectBoards.Values)
                Object.Destroy(board);

            objectBoards.Clear();
        }

        public static void EnableBoardColors()
        {
            foreach (GorillaNetworkJoinTrigger joinTrigger in PhotonNetworkController.Instance.allJoinTriggers)
            {
                try
                {
                    JoinTriggerUI ui = joinTrigger.ui;
                    JoinTriggerUITemplate temp = ui.template;

                    temp.ScreenBG_AbandonPartyAndSoloJoin = OrangeUI;
                    temp.ScreenBG_AlreadyInRoom = OrangeUI;
                    temp.ScreenBG_ChangingGameModeSoloJoin = OrangeUI;
                    temp.ScreenBG_Error = OrangeUI;
                    temp.ScreenBG_InPrivateRoom = OrangeUI;
                    temp.ScreenBG_LeaveRoomAndGroupJoin = OrangeUI;
                    temp.ScreenBG_LeaveRoomAndSoloJoin = OrangeUI;
                    temp.ScreenBG_NotConnectedSoloJoin = OrangeUI;
                }
                catch { }
            }
            hasFoundAllBoards = false;
            disableBoardColor = false;
            motd.SetActive(true);
            motdText.SetActive(true);
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(false);
            GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText").SetActive(false);
        }
    }
}
