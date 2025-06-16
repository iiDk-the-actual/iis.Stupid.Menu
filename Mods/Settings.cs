using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Settings
    {
        public static void Search() // This took me like 4 hours
        {
            isSearching = !isSearching;
            isPcWhenSearching = isOnPC;
            pageNumber = 0;
            searchText = "";
            lastPressedKeys.Add(KeyCode.Q);
            if (isSearching)
            {
                if (!isPcWhenSearching)
                {
                    if (VRKeyboard == null)
                    {
                        VRKeyboard = LoadAsset<GameObject>("keyboard");
                        VRKeyboard.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
                        VRKeyboard.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                        menuSpawnPosition = VRKeyboard.transform.Find("MenuSpawnPosition").gameObject;
                        VRKeyboard.transform.Find("Canvas/Text").GetComponent<Text>().color = textColor;

                        ColorChanger backgroundColorChanger = VRKeyboard.transform.Find("Background").gameObject.AddComponent<ColorChanger>();
                        backgroundColorChanger.colors = new Gradient
                        {
                            colorKeys = new[]
                            {
                                new GradientColorKey(bgColorA, 0f),
                                new GradientColorKey(bgColorB, 0.5f),
                                new GradientColorKey(bgColorA, 1f)
                            }
                        };
                        backgroundColorChanger.isRainbow = themeType == 6;
                        backgroundColorChanger.isEpileptic = themeType == 47;
                        backgroundColorChanger.isMonkeColors = themeType == 8;

                        ColorChanger keyColorChanger = VRKeyboard.transform.Find("Keys/default").gameObject.AddComponent<ColorChanger>();
                        keyColorChanger.colors = new Gradient
                        {
                            colorKeys = new[]
                            {
                                new GradientColorKey(buttonDefaultA, 0f),
                                new GradientColorKey(buttonDefaultB, 0.5f),
                                new GradientColorKey(buttonDefaultA, 1f)
                            }
                        };

                        if (shouldOutline)
                            OutlineObjNonMenu(VRKeyboard.transform.Find("Background").gameObject, true);

                        for (int i = 0; i < VRKeyboard.transform.childCount; i++)
                        {
                            GameObject v = VRKeyboard.transform.GetChild(i).gameObject;

                            if (v.name != "Canvas" && v.name != "MenuSpawnPosition" && v.name != "Background" && v.name != "Keys" && !v.name.Contains("Cube"))
                            {
                                v.AddComponent<KeyboardKey>().key = v.name;
                                v.layer = 2;

                                if (shouldOutline)
                                    OutlineObjNonMenu(v, true);
                            }
                        }
                    }
                }
                if (lKeyReference == null)
                {
                    lKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    lKeyReference.transform.parent = GorillaTagger.Instance.leftHandTransform;
                    lKeyReference.GetComponent<Renderer>().material.color = bgColorA;
                    lKeyReference.transform.localPosition = pointerOffset;
                    lKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    lKeyCollider = lKeyReference.GetComponent<SphereCollider>();

                    GradientColorKey[] array = new GradientColorKey[3];
                    array[0].color = bgColorA;
                    array[0].time = 0f;
                    array[1].color = bgColorB;
                    array[1].time = 0.5f;
                    array[2].color = bgColorA;
                    array[2].time = 1f;
                    ColorChanger colorChanger = lKeyReference.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.isMonkeColors = themeType == 8;
                }
                if (rKeyReference == null)
                {
                    rKeyReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    rKeyReference.transform.parent = GorillaTagger.Instance.rightHandTransform;
                    rKeyReference.GetComponent<Renderer>().material.color = bgColorA;
                    rKeyReference.transform.localPosition = pointerOffset;
                    rKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    rKeyCollider = rKeyReference.GetComponent<SphereCollider>();

                    GradientColorKey[] array = new GradientColorKey[3];
                    array[0].color = bgColorA;
                    array[0].time = 0f;
                    array[1].color = bgColorB;
                    array[1].time = 0.5f;
                    array[2].color = bgColorA;
                    array[2].time = 1f;
                    ColorChanger colorChanger = rKeyReference.AddComponent<ColorChanger>();
                    colorChanger.colors = new Gradient
                    {
                        colorKeys = array
                    };
                    colorChanger.isRainbow = themeType == 6;
                    colorChanger.isEpileptic = themeType == 47;
                    colorChanger.isMonkeColors = themeType == 8;
                }
            } else
            {
                if (lKeyReference != null)
                {
                    UnityEngine.Object.Destroy(lKeyReference);
                    lKeyReference = null;
                }
                if (rKeyReference != null)
                {
                    UnityEngine.Object.Destroy(rKeyReference);
                    rKeyReference = null;
                }
                if (VRKeyboard != null)
                {
                    UnityEngine.Object.Destroy(VRKeyboard);
                    VRKeyboard = null;
                }
                if (TPC != null && TPC.transform.parent.gameObject.name.Contains("CameraTablet") && isOnPC)
                {
                    isOnPC = false;
                    TPC.transform.position = TPC.transform.parent.position;
                    TPC.transform.rotation = TPC.transform.parent.rotation;
                }
            }
        }

        public static void GlobalReturn()
        {
            NotifiLib.ClearAllNotifications();
            Toggle(Buttons.buttons[currentCategoryIndex][0].buttonText);
        }

        public static Dictionary<string, Assembly> LoadedPlugins = new Dictionary<string, Assembly> { };
        public static List<string> disabledPlugins = new List<string> { };
        public static void LoadPlugins()
        {
            Buttons.buttons[GetCategory("Plugin Settings")] = new ButtonInfo[] { new ButtonInfo { buttonText = "Exit Plugin Settings", method = () => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu." } };

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

            if (!Directory.Exists("iisStupidMenu/Plugins"))
                Directory.CreateDirectory("iisStupidMenu/Plugins");

            if (!File.Exists("iisStupidMenu/Plugins/DisabledPlugins.txt"))
                File.WriteAllText("iisStupidMenu/Plugins/DisabledPlugins.txt", "");
            else
            {
                string text = File.ReadAllText("iisStupidMenu/Plugins/DisabledPlugins.txt");
                if (text.Length > 1)
                    disabledPlugins = text.Split("\n").ToList();
            }

            string[] Files = Directory.GetFiles("iisStupidMenu/Plugins");
            foreach (string File in Files)
            {
                try
                {
                    if (GetFileExtension(File) == "dll")
                    {
                        string PluginName = File.Replace("iisStupidMenu/Plugins/", "");
                        LoadedPlugins.Add(PluginName, GetAssembly(File));
                    }
                } catch (Exception e) { LogManager.Log("Error with loading plugin " + File + ": " + e.ToString()); }
            }

            foreach (KeyValuePair<string, Assembly> Plugin in LoadedPlugins)
            {
                try
                {
                    string[] PluginInfo = GetPluginInfo(Plugin.Value);
                    AddButton(33, new ButtonInfo { buttonText = Plugin.Key, overlapText = (disabledPlugins.Contains(Plugin.Key) ? "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>" : "<color=grey>[</color><color=green>ON</color><color=grey>]</color>") + " " + PluginInfo[0], method = () => TogglePlugin(Plugin), isTogglable = false, toolTip = PluginInfo[1] });
                    if (!disabledPlugins.Contains(Plugin.Key))
                    {
                        EnablePlugin(Plugin.Value);
                    }
                }
                catch (Exception e) { LogManager.Log("Error with enabling plugin " + Plugin.Key + ": " + e.ToString()); }
            }

            AddButton(33, new ButtonInfo { buttonText = "Open Plugins Folder", method = () => OpenPluginsFolder(), isTogglable = false, toolTip = "Opens a folder containing all of your plugins." });
            AddButton(33, new ButtonInfo { buttonText = "Reload Plugins", method = () => ReloadPlugins(), isTogglable = false, toolTip = "Reloads all of your plugins." });
            AddButton(33, new ButtonInfo { buttonText = "Get More Plugins", method = () => LoadPluginLibrary(), isTogglable = false, toolTip = "Opens a public plugin library, where you can download your own plugins." });
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
            string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "iisStupidMenu/Plugins");
            filePath = filePath.Split("BepInEx\\")[0] + "iisStupidMenu/Plugins";
            Process.Start(filePath);
        }

        public static void LoadPluginLibrary()
        {
            currentCategoryName = "Sound Library";

            string library = GetHttp("https://github.com/iiDk-the-actual/ModInfo/raw/main/Plugins/PluginLibrary.txt");
            string[] plugins = AlphabetizeNoSkip(library.Split("\n"));

            List<ButtonInfo> pluginbuttons = new List<ButtonInfo> { new ButtonInfo { buttonText = "Exit Plugin Library", method = () => currentCategoryName = "Plugin Settings", isTogglable = false, toolTip = "Returns you back to the plugin settings." } };
            int index = 0;

            foreach (string plugin in plugins)
            {
                if (plugin.Length > 2)
                {
                    index++;
                    string[] Data = plugin.Split(";");
                    pluginbuttons.Add(new ButtonInfo { buttonText = "PluginDownload" + index.ToString(), overlapText = Data[0], method =() => DownloadPlugin(Data[0], Data[2]), isTogglable = false, toolTip = Data[1] });
                }
            }
            Buttons.buttons[GetCategory("Sound Library")] = pluginbuttons.ToArray();
        }

        public static void DownloadPlugin(string name, string url)
        {
            if (name.Contains(".."))
                name = name.Replace("..", "");

            string filename = url.Split("/")[^1];

            if (File.Exists("iisStupidMenu/Plugins/" + filename))
                File.Delete("iisStupidMenu/Plugins/" + filename);

            WebClient stream = new WebClient();
            stream.DownloadFile(url, "iisStupidMenu/Plugins/" + filename);

            LoadPlugins();
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to your plugins.");
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
            {
                disabledPluginsString += disabledPlugin + "\n";
            }

            File.WriteAllText("iisStupidMenu/Plugins/DisabledPlugins.txt", disabledPluginsString);

            GetIndex(Plugin.Key).overlapText = (disabledPlugins.Contains(Plugin.Key) ? "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>" : "<color=grey>[</color><color=green>ON</color><color=grey>]</color>") + " " + GetPluginInfo(Plugin.Value)[0];
        }
        
        public static void RightHand()
        {
            rightHand = true;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotifiLib.ClearAllNotifications();
            }
            if (GetIndex("Info Watch").enabled)
            {
                Toggle("Info Watch");
                Toggle("Info Watch");
                NotifiLib.ClearAllNotifications();
            }
        }

        public static void LeftHand()
        {
            rightHand = false;
            if (watchMenu)
            {
                Toggle("Watch Menu");
                Toggle("Watch Menu");
                NotifiLib.ClearAllNotifications();
            }
            if (GetIndex("Info Watch").enabled)
            {
                Toggle("Info Watch");
                Toggle("Info Watch");
                NotifiLib.ClearAllNotifications();
            }
        }

        public static void ClearAllKeybinds()
        {
            foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
            {
                foreach (string modName in Bind.Value)
                    GetIndex(modName).customBind = null;

                Bind.Value.Clear();
            }
        }

        public static void StartBind(string Bind)
        {
            IsBinding = true;
            BindInput = Bind;
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
            GameObject mainwatch = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/huntcomputer (1)");
            watchobject = UnityEngine.Object.Instantiate(mainwatch, 
                rightHand ? 
                GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").transform : 
                GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform, false);

            UnityEngine.Object.Destroy(watchobject.GetComponent<GorillaHuntComputer>());
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
            UnityEngine.Object.Destroy(watchobject);
        }

        public static int langInd = 0;
        public static void ChangeMenuLanguage()
        {
            string[] langnames = new string[]
            {
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

            string[] codenames = new string[]
            {
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

            langInd++;
            if (langInd > langnames.Length - 1)
                langInd = 0;

            translateCache.Clear();
            language = codenames[langInd];

            GetIndex("Change Menu Language").overlapText = "Change Menu Language <color=grey>[</color><color=green>" + langnames[langInd] + "</color><color=grey>]</color>";

            if (langInd == 0)
                translate = false;
            else
                translate = true;
        }

        public static void ChangeMenuTheme()
        {
            themeType++;
            if (themeType > 57)
                themeType = 1;

            if (GetIndex("Custom Menu Theme").enabled)
                return;

            switch (themeType) {
                case 1: // Orange
                    bgColorA = new Color32(255, 128, 0, 128);
                    bgColorB = new Color32(255, 102, 0, 128);
                    buttonDefaultA = new Color32(170, 85, 0, 255);
                    buttonDefaultB = new Color32(170, 85, 0, 255);
                    buttonClickedA = new Color32(85, 42, 0, 255);
                    buttonClickedB = new Color32(85, 42, 0, 255);
                    titleColor = new Color32(255, 190, 125, 255);
                    textColor = new Color32(255, 190, 125, 255);
                    textClicked = new Color32(255, 190, 125, 255);
                    break;
                case 2: // Blue Magenta
                    bgColorA = Color.blue;
                    bgColorB = Color.magenta;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.blue;
                    buttonClickedB = Color.blue;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 3: // Dark Mode
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(20, 20, 20, 255);
                    buttonClickedB = new Color32(20, 20, 20, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 4: // Strobe
                    bgColorA = Color.white;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 5: // Kman
                    bgColorA = Color.black;
                    bgColorB = new Color32(110, 0, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = new Color32(110, 0, 0, 255);
                    buttonClickedA = new Color32(110, 0, 0, 255);
                    buttonClickedB = new Color32(110, 0, 0, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 6: // Rainbow
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 7: // Cone
                    bgColorA = new Color32(255, 128, 0, 128);
                    bgColorB = new Color32(255, 128, 0, 128);
                    buttonDefaultA = new Color32(170, 85, 0, 255);
                    buttonDefaultB = new Color32(170, 85, 0, 255);
                    buttonClickedA = new Color32(85, 42, 0, 255);
                    buttonClickedB = new Color32(85, 42, 0, 255);
                    titleColor = new Color32(255, 190, 125, 255);
                    textColor = new Color32(255, 190, 125, 255);
                    textClicked = new Color32(255, 190, 125, 255);
                    GetIndex("Thin Menu").enabled = true;
                    thinMenu = true;
                    break;
                case 8: // Player Material
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 9: // Lava
                    bgColorA = Color.black;
                    bgColorB = new Color32(255, 111, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(255, 111, 0, 255);
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 10: // Rock
                    bgColorA = Color.black;
                    bgColorB = Color.red;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 11: // Ice
                    bgColorA = Color.black;
                    bgColorB = new Color32(0, 174, 255, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(0, 174, 255, 255);
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 12: // Water
                    bgColorA = new Color32(0, 136, 255, 255);
                    bgColorB = new Color32(0, 174, 255, 255);
                    buttonDefaultA = new Color32(0, 100, 188, 255);
                    buttonDefaultB = new Color32(0, 100, 188, 255);
                    buttonClickedA = new Color32(0, 174, 255, 255);
                    buttonClickedB = new Color32(0, 136, 255, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 13: // Minty
                    bgColorA = new Color32(0, 255, 246, 255);
                    bgColorB = new Color32(0, 255, 144, 255);
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(0, 255, 144, 255);
                    buttonClickedB = new Color32(0, 255, 246, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 14: // Pink
                    bgColorA = new Color32(255, 130, 255, 255);
                    bgColorB = Color.white;
                    buttonDefaultA = new Color32(255, 130, 255, 255);
                    buttonDefaultB = new Color32(255, 130, 255, 255);
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 15: // Purple
                    bgColorA = new Color32(122, 35, 159, 255);
                    bgColorB = new Color32(60, 26, 89, 255);
                    buttonDefaultA = new Color32(60, 26, 89, 255);
                    buttonDefaultB = new Color32(60, 26, 89, 255);
                    buttonClickedA = new Color32(122, 35, 159, 255);
                    buttonClickedB = new Color32(122, 35, 159, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 16: // Magenta Cyan
                    bgColorA = Color.magenta;
                    bgColorB = Color.cyan;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.cyan;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.black;
                    break;
                case 17: // Red Fade
                    bgColorA = Color.red;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.red;
                    textColor = Color.red;
                    textClicked = Color.black;
                    break;
                case 18: // Orange Fade
                    bgColorA = new Color32(255, 128, 0, 255);
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(255, 128, 0, 255);
                    buttonClickedB = new Color32(255, 128, 0, 255);
                    titleColor = new Color32(255, 128, 0, 255);
                    textColor = new Color32(255, 128, 0, 255);
                    textClicked = Color.black;
                    break;
                case 19: // Yellow Fade
                    bgColorA = Color.yellow;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.yellow;
                    buttonClickedB = Color.yellow;
                    titleColor = Color.yellow;
                    textColor = Color.yellow;
                    textClicked = Color.black;
                    break;
                case 20: // Green Fade
                    bgColorA = Color.green;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.green;
                    textColor = Color.green;
                    textClicked = Color.black;
                    break;
                case 21: // Blue Fade
                    bgColorA = Color.blue;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.blue;
                    buttonClickedB = Color.blue;
                    titleColor = Color.blue;
                    textColor = Color.blue;
                    textClicked = Color.black;
                    break;
                case 22: // Purple Fade
                    bgColorA = new Color32(119, 0, 255, 255);
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(119, 0, 255, 255);
                    buttonClickedB = new Color32(119, 0, 255, 255);
                    titleColor = new Color32(119, 0, 255, 255);
                    textColor = new Color32(119, 0, 255, 255);
                    textClicked = Color.black;
                    break;
                case 23: // Magenta Fade
                    bgColorA = Color.magenta;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.magenta;
                    titleColor = Color.magenta;
                    textColor = Color.magenta;
                    textClicked = Color.black;
                    break;
                case 24: // Banana
                    bgColorA = new Color32(255, 255, 130, 255);
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(255, 255, 130, 255);
                    buttonClickedB = new Color32(255, 255, 130, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 25: // Pride
                    bgColorA = Color.red;
                    bgColorB = Color.green;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.white;
                    textClicked = Color.black;
                    break;
                case 26: // Trans
                    bgColorA = new Color32(245, 169, 184, 255);
                    bgColorB = new Color32(91, 206, 250, 255);
                    buttonDefaultA = new Color32(245, 169, 184, 255);
                    buttonDefaultB = new Color32(245, 169, 184, 255);
                    buttonClickedA = new Color32(91, 206, 250, 255);
                    buttonClickedB = new Color32(91, 206, 250, 255);
                    titleColor = new Color32(91, 206, 250, 255);
                    textColor = new Color32(91, 206, 250, 255);
                    textClicked = new Color32(245, 169, 184, 255);
                    break;
                case 27: // MLM or Gay
                    bgColorA = new Color32(7, 141, 112, 255);
                    bgColorB = new Color32(61, 26, 220, 255);
                    buttonDefaultA = new Color32(7, 141, 112, 255);
                    buttonDefaultB = new Color32(7, 141, 112, 255);
                    buttonClickedA = new Color32(61, 26, 220, 255);
                    buttonClickedB = new Color32(61, 26, 220, 255);
                    titleColor = new Color32(61, 26, 220, 255);
                    textColor = new Color32(61, 26, 220, 255);
                    textClicked = new Color32(7, 141, 112, 255);
                    break;
                case 28: // Steal (old)
                    bgColorA = new Color32(50, 50, 50, 255);
                    bgColorB = new Color32(50, 50, 50, 255);
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(75, 75, 75, 255);
                    buttonClickedB = new Color32(75, 75, 75, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 29: // Silence
                    bgColorA = Color.black;
                    bgColorB = new Color32(80, 0, 80, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 30: // Transparent
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 31: // King
                    bgColorA = new Color32(100, 60, 170, 255);
                    bgColorB = new Color32(100, 60, 170, 255);
                    buttonDefaultA = new Color32(150, 100, 240, 255);
                    buttonDefaultB = new Color32(150, 100, 240, 255);
                    buttonClickedA = new Color32(150, 100, 240, 255);
                    buttonClickedB = new Color32(150, 100, 240, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.cyan;
                    break;
                case 32: // Scoreboard
                    bgColorA = new Color32(0, 59, 4, 255);
                    bgColorB = new Color32(0, 59, 4, 255);
                    buttonDefaultA = new Color32(192, 190, 171, 255);
                    buttonDefaultB = new Color32(192, 190, 171, 255);
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 33: // Scoreboard (banned)
                    bgColorA = new Color32(225, 73, 43, 255);
                    bgColorB = new Color32(225, 73, 43, 255);
                    buttonDefaultA = new Color32(192, 190, 171, 255);
                    buttonDefaultB = new Color32(192, 190, 171, 255);
                    buttonClickedA = Color.red;
                    buttonClickedB = Color.red;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 34: // Rift
                    bgColorA = new Color32(25, 25, 25, 255);
                    bgColorB = new Color32(25, 25, 25, 255);
                    buttonDefaultA = new Color32(40, 40, 40, 255);
                    buttonDefaultB = new Color32(40, 40, 40, 255);
                    buttonClickedA = new Color32(167, 66, 191, 255);
                    buttonClickedB = new Color32(167, 66, 191, 255);
                    titleColor = new Color32(144, 144, 144, 255);
                    textColor = new Color32(144, 144, 144, 255);
                    textClicked = Color.white;
                    break;
                case 35: // Blurple Dark
                    bgColorA = new Color32(26, 26, 61, 255);
                    bgColorB = new Color32(26, 26, 61, 255);
                    buttonDefaultA = new Color32(26, 26, 61, 255);
                    buttonDefaultB = new Color32(26, 26, 61, 255);
                    buttonClickedA = new Color32(43, 17, 84, 255);
                    buttonClickedB = new Color32(43, 17, 84, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 36: // ShibaGT Gold
                    bgColorA = Color.black;
                    bgColorB = Color.gray;
                    buttonDefaultA = Color.yellow;
                    buttonDefaultB = Color.yellow;
                    buttonClickedA = Color.magenta;
                    buttonClickedB = Color.magenta;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 37: // ShibaGT Genesis
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(32, 32, 32, 255);
                    buttonDefaultB = new Color32(32, 32, 32, 255);
                    buttonClickedA = new Color32(32, 32, 32, 255);
                    buttonClickedB = new Color32(32, 32, 32, 255);
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.white;
                    break;
                case 38: // wyvern
                    bgColorA = new Color32(199, 115, 173, 255);
                    bgColorB = new Color32(165, 233, 185, 255);
                    buttonDefaultA = new Color32(99, 58, 86, 255);
                    buttonDefaultB = new Color32(83, 116, 92, 255);
                    buttonClickedA = new Color32(99, 58, 86, 255);
                    buttonClickedB = new Color32(83, 116, 92, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.green;
                    break;
                case 39: // Steal (new)
                    bgColorA = new Color32(27, 27, 27, 255);
                    bgColorB = new Color32(27, 27, 27, 255);
                    buttonDefaultA = new Color32(50, 50, 50, 255);
                    buttonDefaultB = new Color32(50, 50, 50, 255);
                    buttonClickedA = new Color32(66, 66, 66, 255);
                    buttonClickedB = new Color32(66, 66, 66, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 40: // USA Menu (lol)
                    bgColorA = Color.black;
                    bgColorB = new Color32(100, 25, 125, 255);
                    buttonDefaultA = new Color32(25, 25, 25, 255);
                    buttonDefaultB = new Color32(25, 25, 25, 255);
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 41: // Watch
                    bgColorA = new Color32(27, 27, 27, 255);
                    bgColorB = new Color32(27, 27, 27, 255);
                    buttonDefaultA = Color.red;
                    buttonDefaultB = Color.red;
                    buttonClickedA = Color.green;
                    buttonClickedB = Color.green;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 42: // AZ Menu
                    bgColorA = Color.black;
                    bgColorB = new Color32(100, 0, 0, 255);
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(100, 0, 0, 255);
                    buttonClickedB = new Color32(100, 0, 0, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 43: // ImGUI
                    bgColorA = new Color32(21, 22, 23, 255);
                    bgColorB = new Color32(21, 22, 23, 255);
                    buttonDefaultA = new Color32(32, 50, 77, 255);
                    buttonDefaultB = new Color32(32, 50, 77, 255);
                    buttonClickedA = new Color32(60, 127, 206, 255);
                    buttonClickedB = new Color32(60, 127, 206, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 44: // Clean Dark
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = new Color32(10, 10, 10, 255);
                    buttonClickedB = new Color32(10, 10, 10, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 45: // Discord Light Mode (lmfao)
                    bgColorA = Color.white;
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = new Color32(245, 245, 245, 255);
                    buttonClickedB = new Color32(245, 245, 245, 255);
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 46: // The Hub
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = new Color32(255, 163, 26, 255);
                    buttonDefaultB = new Color32(255, 163, 26, 255);
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.black;
                    textClicked = Color.white;
                    break;
                case 47: // EPILEPTIC
                    bgColorA = Color.black;
                    bgColorB = Color.black;
                    buttonDefaultA = Color.black;
                    buttonDefaultB = Color.black;
                    buttonClickedA = Color.black;
                    buttonClickedB = Color.black;
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 48: // Discord Blurple
                    bgColorA = new Color32(111, 143, 255, 255);
                    bgColorB = new Color32(163, 184, 255, 255);
                    buttonDefaultA = new Color32(96, 125, 219, 255);
                    buttonDefaultB = new Color32(96, 125, 219, 255);
                    buttonClickedA = new Color32(147, 167, 226, 255);
                    buttonClickedB = new Color32(147, 167, 226, 255);
                    titleColor = new Color32(33, 33, 101, 255);
                    textColor = new Color32(33, 33, 101, 255);
                    textClicked = new Color32(33, 33, 101, 255);
                    break;
                case 49: // VS Zero
                    bgColorA = new Color32(19, 22, 27, 255);
                    bgColorB = new Color32(19, 22, 27, 255);
                    buttonDefaultA = new Color32(19, 22, 27, 255);
                    buttonDefaultB = new Color32(19, 22, 27, 255);
                    buttonClickedA = new Color32(16, 18, 22, 255);
                    buttonClickedB = new Color32(16, 18, 22, 255);
                    titleColor = new Color32(82, 96, 122, 255);
                    textColor = new Color32(82, 96, 122, 255);
                    textClicked = new Color32(82, 96, 122, 255);
                    break;
                case 50: // Weed theme (for v4.2.0) (also 50th theme)
                    bgColorA = new Color32(0, 136, 16, 255);
                    bgColorB = new Color32(0, 127, 14, 255);
                    buttonDefaultA = new Color32(0, 158, 15, 255);
                    buttonDefaultB = new Color32(0, 158, 15, 255);
                    buttonClickedA = new Color32(0, 112, 11, 255);
                    buttonClickedB = new Color32(0, 112, 11, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 51: // Pastel Rainbow
                    bgColorA = Color.white;
                    bgColorB = Color.white;
                    buttonDefaultA = Color.white;
                    buttonDefaultB = Color.white;
                    buttonClickedA = Color.white;
                    buttonClickedB = Color.white;
                    titleColor = Color.black;
                    textColor = Color.black;
                    textClicked = Color.black;
                    break;
                case 52: // Rift Light
                    bgColorA = new Color32(25, 25, 25, 255);
                    bgColorB = new Color32(25, 25, 25, 255);
                    buttonDefaultA = new Color32(40, 40, 40, 255);
                    buttonDefaultB = new Color32(40, 40, 40, 255);
                    buttonClickedA = new Color32(165, 137, 255, 255);
                    buttonClickedB = new Color32(165, 137, 255, 255);
                    titleColor = new Color32(144, 144, 144, 255);
                    textColor = new Color32(144, 144, 144, 255);
                    textClicked = Color.white;
                    break;
                case 53: // Rose (Solace)
                    bgColorA = new Color32(176, 12, 64, 255);
                    bgColorB = new Color32(176, 12, 64, 255);
                    buttonDefaultA = new Color32(140, 10, 51, 255);
                    buttonDefaultB = new Color32(140, 10, 51, 255);
                    buttonClickedA = new Color32(250, 2, 81, 255);
                    buttonClickedB = new Color32(250, 2, 81, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 54: // Tenacity (Solace)
                    bgColorA = new Color32(124, 25, 194, 255);
                    bgColorB = new Color32(124, 25, 194, 255);
                    buttonDefaultA = new Color32(88, 9, 145, 255);
                    buttonDefaultB = new Color32(88, 9, 145, 255);
                    buttonClickedA = new Color32(136, 9, 227, 255);
                    buttonClickedB = new Color32(136, 9, 227, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 55: // e621 (for version 6.2.1)
                    bgColorA = new Color32(1, 73, 149, 255);
                    bgColorB = new Color32(1, 73, 149, 255);
                    buttonDefaultA = new Color32(1, 46, 87, 255);
                    buttonDefaultB = new Color32(1, 46, 87, 255);
                    buttonClickedA = new Color32(0, 37, 74, 255);
                    buttonClickedB = new Color32(0, 37, 74, 255);
                    titleColor = new Color32(252, 179, 40, 255);
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
                case 56: // Catppuccin Mocha
                    bgColorA = new Color32(30, 30, 46, 255);
                    bgColorB = new Color32(30, 30, 46, 255);
                    buttonDefaultA = new Color32(88, 91, 112, 255);
                    buttonDefaultB = new Color32(88, 91, 112, 255);
                    buttonClickedA = new Color32(49, 50, 68, 255);
                    buttonClickedB = new Color32(49, 50, 68, 255);
                    titleColor = new Color32(205, 214, 244, 255);
                    textColor = new Color32(186, 194, 222, 255);
                    textClicked = new Color32(166, 173, 200, 255);
                    break;
                case 57: // Rexon
                    bgColorA = new Color32(45, 25, 75, 255);
                    bgColorB = new Color32(45, 25, 75, 255);
                    buttonDefaultA = new Color32(40, 15, 60, 255);
                    buttonDefaultB = new Color32(40, 15, 60, 255);
                    buttonClickedA = new Color32(100, 30, 140, 255);
                    buttonClickedB = new Color32(100, 30, 140, 255);
                    titleColor = Color.white;
                    textColor = Color.white;
                    textClicked = Color.white;
                    break;
            }
        }


        private static int modifyWhatId = 0;
        public static void CMTIncreaseRed()
        {
            int r = 0;
            switch (modifyWhatId)
            {
                case 0:
                    r = (int)Math.Round(bgColorA.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorA = new Color(r / 10f, bgColorA.g, bgColorA.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorA) + ">Preview</color>";
                    break;
                case 1:
                    r = (int)Math.Round(bgColorB.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorB = new Color(r / 10f, bgColorB.g, bgColorB.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorB) + ">Preview</color>";
                    break;
                case 2:
                    r = (int)Math.Round(buttonDefaultA.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultA = new Color(r / 10f, buttonDefaultA.g, buttonDefaultA.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultA) + ">Preview</color>";
                    break;
                case 3:
                    r = (int)Math.Round(buttonDefaultB.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultB = new Color(r / 10f, buttonDefaultB.g, buttonDefaultB.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultB) + ">Preview</color>";
                    break;
                case 4:
                    r = (int)Math.Round(buttonClickedA.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedA = new Color(r / 10f, buttonClickedA.g, buttonClickedA.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedA) + ">Preview</color>";
                    break;
                case 5:
                    r = (int)Math.Round(buttonClickedB.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedB = new Color(r / 10f, buttonClickedB.g, buttonClickedB.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedB) + ">Preview</color>";
                    break;
                case 6:
                    r = (int)Math.Round(titleColor.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        titleColor = new Color(r / 10f, titleColor.g, titleColor.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(titleColor) + ">Preview</color>";
                    break;
                case 7:
                    r = (int)Math.Round(textColor.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    textColor = new Color(r / 10f, textColor.g, textColor.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColor) + ">Preview</color>";
                    break;
                case 8:
                    r = (int)Math.Round(textClicked.r * 10f);
                    r++;
                    if (r > 10)
                        r = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textClicked = new Color(r / 10f, textClicked.g, textClicked.b);

                    GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + r.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textClicked) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }
        public static void CMTIncreaseGreen()
        {
            int g = 0;
            switch (modifyWhatId)
            {
                case 0:
                    g = (int)Math.Round(bgColorA.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorA = new Color(bgColorA.r, g / 10f, bgColorA.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorA) + ">Preview</color>";
                    break;
                case 1:
                    g = (int)Math.Round(bgColorB.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorB = new Color(bgColorB.r, g / 10f, bgColorB.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorB) + ">Preview</color>";
                    break;
                case 2:
                    g = (int)Math.Round(buttonDefaultA.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultA = new Color(buttonDefaultA.r, g / 10f, buttonDefaultA.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultA) + ">Preview</color>";
                    break;
                case 3:
                    g = (int)Math.Round(buttonDefaultB.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultB = new Color(buttonDefaultB.r, g / 10f, buttonDefaultB.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultB) + ">Preview</color>";
                    break;
                case 4:
                    g = (int)Math.Round(buttonClickedA.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedA = new Color(buttonClickedA.r, g / 10f, buttonClickedA.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedA) + ">Preview</color>";
                    break;
                case 5:
                    g = (int)Math.Round(buttonClickedB.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedB = new Color(buttonClickedB.r, g / 10f, buttonClickedB.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedB) + ">Preview</color>";
                    break;
                case 6:
                    g = (int)Math.Round(titleColor.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        titleColor = new Color(titleColor.r, g / 10f, titleColor.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(titleColor) + ">Preview</color>";
                    break;
                case 7:
                    g = (int)Math.Round(textColor.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColor = new Color(textColor.r, g / 10f, textColor.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColor) + ">Preview</color>";
                    break;
                case 8:
                    g = (int)Math.Round(textClicked.g * 10f);
                    g++;
                    if (g > 10)
                        g = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textClicked = new Color(textClicked.r, g / 10f, textClicked.b);

                    GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + g.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textClicked) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }
        public static void CMTIncreaseBlue()
        {
            int b = 0;
            switch (modifyWhatId)
            {
                case 0:
                    b = (int)Math.Round(bgColorA.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorA = new Color(bgColorA.r, bgColorA.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorA) + ">Preview</color>";
                    break;
                case 1:
                    b = (int)Math.Round(bgColorB.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        bgColorB = new Color(bgColorB.r, bgColorB.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(bgColorB) + ">Preview</color>";
                    break;
                case 2:
                    b = (int)Math.Round(buttonDefaultA.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultA = new Color(buttonDefaultA.r, buttonDefaultA.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultA) + ">Preview</color>";
                    break;
                case 3:
                    b = (int)Math.Round(buttonDefaultB.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonDefaultB = new Color(buttonDefaultB.r, buttonDefaultB.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonDefaultB) + ">Preview</color>";
                    break;
                case 4:
                    b = (int)Math.Round(buttonClickedA.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedA = new Color(buttonClickedA.r, buttonClickedA.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedA) + ">Preview</color>";
                    break;
                case 5:
                    b = (int)Math.Round(buttonClickedB.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        buttonClickedB = new Color(buttonClickedB.r, buttonClickedB.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(buttonClickedB) + ">Preview</color>";
                    break;
                case 6:
                    b = (int)Math.Round(titleColor.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        titleColor = new Color(titleColor.r, titleColor.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(titleColor) + ">Preview</color>";
                    break;
                case 7:
                    b = (int)Math.Round(textColor.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textColor = new Color(textColor.r, textColor.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textColor) + ">Preview</color>";
                    break;
                case 8:
                    b = (int)Math.Round(textClicked.b * 10f);
                    b++;
                    if (b > 10)
                        b = 0;

                    if (GetIndex("Custom Menu Theme").enabled)
                        textClicked = new Color(textClicked.r, textClicked.g, b / 10f);

                    GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + b.ToString() + "</color><color=grey>]</color>";
                    GetIndex("PreviewLabel").overlapText = "<color=#" + ColorToHex(textClicked) + ">Preview</color>";
                    break;
            }
            UpdateWriteCustomTheme();
        }

        private static int rememberdirectory = 0;
        public static void CustomMenuTheme()
        {
            if (!File.Exists("iisStupidMenu/iiMenu_CustomThemeColor.txt"))
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
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorA.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the first color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorA.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the first color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorA.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the first color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(bgColorA) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTBackgroundSecond()
        {
            modifyWhatId = 1;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTBackground(), isTogglable = false, toolTip = "Returns you back to the background menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorB.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the second color of the background." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorB.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the second color of the background." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(bgColorB.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the second color of the background." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(bgColorB) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void CMTButton()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Buttons", method = () => CustomMenuThemePage(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Enabled", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Choose what type of button color to modify." },
                new ButtonInfo { buttonText = "Disabled", method = () => CMTButtonDisabled(), isTogglable = false, toolTip = "Change the color of the second color of the background." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonEnabled()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Enabled", method = () => CMTButton(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "First Color", method = () => CMTButtonEnabledFirst(), isTogglable = false, toolTip = "Change the color of the first color of the enabled button color." },
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
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedA.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedA.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedA.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonClickedA) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonEnabledSecond()
        {
            modifyWhatId = 5;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTButtonEnabled(), isTogglable = false, toolTip = "Returns you back to the enabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedB.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedB.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(buttonClickedB.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the first color of the enabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonClickedB) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonDisabledFirst()
        {
            modifyWhatId = 2;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit First Color", method = () => CMTButtonDisabled(), isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultA.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultA.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultA.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonDefaultA) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTButtonDisabledSecond()
        {
            modifyWhatId = 3;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTButtonDisabled(), isTogglable = false, toolTip = "Returns you back to the disabled button menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultB.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultB.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(buttonDefaultB.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the first color of the disabled button color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(buttonDefaultB) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }

        public static void CMTText()
        {
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Text", method = () => CustomMenuThemePage(), isTogglable = false, toolTip = "Returns you back to the customize menu." },
                new ButtonInfo { buttonText = "Title", method = () => CMTTextTitle(), isTogglable = false, toolTip = "Change the color of the title." },
                new ButtonInfo { buttonText = "Enabled", method = () => CMTTextEnabled(), isTogglable = false, toolTip = "Change the color of the enabled text." },
                new ButtonInfo { buttonText = "Disabled", method = () => CMTTextDisabled(), isTogglable = false, toolTip = "Change the color of the disabled text." },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextTitle()
        {
            modifyWhatId = 6;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Title", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(titleColor.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the title color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(titleColor.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the title color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(titleColor.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the title color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(titleColor) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextEnabled()
        {
            modifyWhatId = 8;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(textClicked.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the enabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(textClicked.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the enabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(textClicked.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the enabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textClicked) + ">Preview</color>", label = true },
            };

            Buttons.buttons[29] = literallybuttons.ToArray();
        }
        public static void CMTTextDisabled()
        {
            modifyWhatId = 7;
            currentCategoryName = "Temporary Category";

            List<ButtonInfo> literallybuttons = new List<ButtonInfo> {
                new ButtonInfo { buttonText = "Exit Second Color", method = () => CMTText(), isTogglable = false, toolTip = "Returns you back to the text menu." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>" + ((int)Math.Round(textColor.r * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseRed(), isTogglable = false, toolTip = "Change the red of the disabled text color." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>" + ((int)Math.Round(textColor.g * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseGreen(), isTogglable = false, toolTip = "Change the green of the disabled text color." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>" + ((int)Math.Round(textColor.b * 10f)).ToString() + "</color><color=grey>]</color>", method = () => CMTIncreaseBlue(), isTogglable = false, toolTip = "Change the blue of the disabled text color." },
                new ButtonInfo { buttonText = "PreviewLabel", overlapText = "<color=#" + ColorToHex(textColor) + ">Preview</color>", label = true },
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
            string[] linesplit = File.ReadAllText("iisStupidMenu/iiMenu_CustomThemeColor.txt").Split("\n");

            string[] a = linesplit[0].Split(",");
            bgColorA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[1].Split(",");
            bgColorB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[2].Split(",");
            buttonDefaultA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[3].Split(",");
            buttonDefaultB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[4].Split(",");
            buttonClickedA = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[5].Split(",");
            buttonClickedB = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);

            a = linesplit[6].Split(",");
            titleColor = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[7].Split(",");
            textColor = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
            a = linesplit[8].Split(",");
            textClicked = new Color32(byte.Parse(a[0]), byte.Parse(a[1]), byte.Parse(a[2]), 255);
        }

        public static void UpdateWriteCustomTheme()
        {
            Color[] clrs = new Color[]
            {
                bgColorA,
                bgColorB,
                buttonDefaultA,
                buttonDefaultB,
                buttonClickedA,
                buttonClickedB,
                titleColor,
                textColor,
                textClicked
            };

            string output = "";
            foreach (Color clr in clrs)
            {
                if (output != "")
                    output += "\n";

                output += Math.Round((Mathf.Round(clr.r * 10) / 10) * 255f).ToString() + "," + Math.Round((Mathf.Round(clr.g * 10) / 10) * 255f).ToString() + "," + Math.Round((Mathf.Round(clr.b * 10) / 10) * 255f).ToString();
            }
            File.WriteAllText("iisStupidMenu/iiMenu_CustomThemeColor.txt", output);
        }

        public static void FixTheme()
        {
            themeType--;
            ChangeMenuTheme();
        }

        public static void CustomMenuBackground()
        {
            if (!File.Exists("iisStupidMenu/iiMenu_CustomMenuBackground.txt"))
                File.WriteAllText("iisStupidMenu/iiMenu_CustomMenuBackground.txt", "");

            if (File.Exists("iisStupidMenu/MenuBG.png"))
                File.Delete("iisStupidMenu/MenuBG.png");
            
            doCustomMenuBackground = true;
            customMenuBackgroundImage = LoadTextureFromURL(File.ReadAllText("iisStupidMenu/iiMenu_CustomMenuBackground.txt"), "MenuBG.png");
            ReloadMenu();
        }

        public static void FixMenuBackground()
        {
            customMenuBackgroundImage = null;
            doCustomMenuBackground = false;
        }

        public static void ChangePageType() // Say goodbye to daily lagspikes and dirty if spam with new else
        {
            pageButtonType++;
            if (pageButtonType > 6)
                pageButtonType = 1;

            if (pageButtonType == 1)
            {
                pageSize = 6;
                buttonOffset = 2;
            }
            else
            {
                pageSize = 8;
                buttonOffset = 0;
            }
        }

        public static void ChangeArrowType()
        {
            arrowType++;
            if (arrowType > arrowTypes.Length - 1)
                arrowType = 0;
        }

        public static void ChangeFontType() // Say goodbye to daily lagspikes and dirty if spam with new switch (x) { }
        {
            fontCycle++;
            if (fontCycle > 8)
                fontCycle = 0;

            switch (fontCycle) {
                case 0:
                    activeFont = agency;
                    return;
                case 1:
                    activeFont = Arial;
                    return;
                case 2:
                    activeFont = Verdana;
                    return;
                case 3:
                    if (gtagfont == null)
                        gtagfont = LoadAsset<Font>("Utopium");

                    activeFont = gtagfont;
                    return;
                case 4:
                    activeFont = sans;
                    return;
                case 5:
                    activeFont = consolas;
                    return;
                case 6:
                    activeFont = ubuntu;
                    return;
                case 7:
                    activeFont = MSGOTHIC;
                    return;
                case 8:
                    activeFont = impact;
                    return;
            }
        }

        public static void ChangeFontStyleType()
        {
            fontStyleType++;
            if (fontStyleType > 3)
                fontStyleType = 0;

            activeFontStyle = (FontStyle)fontStyleType;
        }

        public static int inputTextColorInt = 3;
        public static void ChangeInputTextColor()
        {
            string[] textColors = new string[]
            {
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
            string[] realinputcolor = new string[]
            {
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

            inputTextColorInt++;
            if (inputTextColorInt > realinputcolor.Length - 1)
                inputTextColorInt = 0;

            inputTextColor = realinputcolor[inputTextColorInt];
            GetIndex("Change Input Text Color").overlapText = $"Change Input Text Color <color=grey>[</color><color=green>{textColors[inputTextColorInt]}</color><color=grey>]</color>";
        }

        public static void ChangePCUI()
        {
            pcbg++;
            if (pcbg > 3)
                pcbg = 0;
        }

        public static void ChangeNotificationTime()
        {
            notificationDecayTime += 1000;
            if (notificationDecayTime > 5500)
                notificationDecayTime = 1000;
            
            GetIndex("Change Notification Time").overlapText = "Change Notification Time <color=grey>[</color><color=green>" + (notificationDecayTime / 1000).ToString() + "</color><color=grey>]</color>";
        }

        public static void ChangeNotificationSound()
        {
            string[] notificationSoundNames = new string[]
            {
                "None",
                "Pop",
                "Ding",
                "Twitter",
                "Discord",
                "Whatsapp",
                "Grindr",
                "iOS",
                "XP Notify",
                "XP Ding",
                "XP Error",
                "Roblox Bass"
            };

            notificationSoundIndex++;
            if (notificationSoundIndex > notificationSoundNames.Length - 1)
                notificationSoundIndex = 0;

            GetIndex("Change Notification Sound").overlapText = "Change Notification Sound <color=grey>[</color><color=green>" + notificationSoundNames[notificationSoundIndex] + "</color><color=grey>]</color>";
        }

        public static void ChangeNarrationVoice()
        {
            string[] narratorNames = new string[]
            {
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

            narratorIndex++;
            if (narratorIndex > narratorNames.Length - 1)
                narratorIndex = 0;

            GetIndex("Change Narration Voice").overlapText = "Change Narration Voice <color=grey>[</color><color=green>" + narratorNames[narratorIndex] + "</color><color=grey>]</color>";
            narratorName = narratorNames[narratorIndex];
        }

        public static void ChangePointerPosition()
        {
            pointerIndex++;
            if (pointerIndex > 3)
            {
                pointerIndex = 0;
            }

            Vector3[] pointerPos = new Vector3[]
            {
                new Vector3(0f, -0.1f, 0f),
                new Vector3(0f, -0.1f, -0.15f),
                new Vector3(0f, 0.1f, -0.05f),
                new Vector3(0f, 0.0666f, 0.1f)
            };
            pointerOffset = pointerPos[pointerIndex];
            try { reference.transform.localPosition = pointerOffset; } catch { }
        }

        // Credits to Scintilla for the idea
        public static void ChangeGunVariation()
        {
            string[] VariationNames = new string[]
            {
                "Default",
                "Lightning",
                "Wavy",
                "Blocky",
                "Zigzag",
                "Spring",
                "Bouncy",
                "Audio"
            };

            gunVariation++;
            if (gunVariation > VariationNames.Length - 1)
                gunVariation = 0;

            GetIndex("Change Gun Variation").overlapText = "Change Gun Variation <color=grey>[</color><color=green>" + VariationNames[gunVariation] + "</color><color=grey>]</color>";
        }

        public static void ChangeGunDirection()
        {
            string[] DirectionNames = new string[]
            {
                "Default",
                "Legacy",
                "Laser",
                "Finger"
            };

            GunDirection++;
            if (GunDirection > DirectionNames.Length - 1)
                GunDirection = 0;

            GetIndex("Change Gun Direction").overlapText = "Change Gun Direction <color=grey>[</color><color=green>" + DirectionNames[GunDirection] + "</color><color=grey>]</color>";
        }

        private static int gunLineQualityIndex = 2;
        public static void ChangeGunLineQuality()
        {
            string[] Names = new string[]
            {
                "Potato",
                "Low",
                "Normal",
                "High",
                "Extreme"
            };

            int[] Qualities = new int[]
            {
                10,
                25,
                50,
                100,
                250
            };

            gunLineQualityIndex++;

            if (gunLineQualityIndex >= Names.Length)
                gunLineQualityIndex = 0;

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
                GorillaTagger.Instance.rigidbody.velocity = new Vector3(0f, 0f, 0f);
            } else
            {
                closePosition = Vector3.zero;
            }
        }

        public static bool currentmentalstate = false;
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
                        {
                            Buttons.buttons[0] = Buttons.buttons[0].Concat(new[] { buttonArray[i] }).ToArray();
                        }

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

        public static void EnablePrimaryRoomMods()
        {
            GetIndex("Disconnect").isTogglable = true;
            GetIndex("Reconnect").isTogglable = true;
            GetIndex("Join Random").isTogglable = true;
        }

        public static void DisablePrimaryRoomMods()
        {
            GetIndex("Disconnect").enabled = false;
            GetIndex("Reconnect").enabled = false;
            GetIndex("Join Random").enabled = false;

            GetIndex("Disconnect").isTogglable = false;
            GetIndex("Reconnect").isTogglable = false;
            GetIndex("Join Random").isTogglable = false;
        }

        public static void DisablePageButtons()
        {
            if (GetIndex("Joystick Menu").enabled) {
                disablePageButtons = true;
            } else
            {
                GetIndex("Disable Page Buttons").enabled = false;
                NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> <color=white>Disable Page Buttons can only be used when using Joystick Menu.</color>");
            }
        }

        public static void CustomMenuName()
        {
            doCustomName = true;
            if (!File.Exists("iisStupidMenu/iiMenu_CustomMenuName.txt"))
                File.WriteAllText("iisStupidMenu/iiMenu_CustomMenuName.txt", "Your Text Here");
            
            customMenuName = File.ReadAllText("iisStupidMenu/iiMenu_CustomMenuName.txt");
        }

        public static void NoCustomMenuName()
        {
            doCustomName = false;
        }

        // Thanks to kingofnetflix for inspiration and support with voice recognition
        private static KeywordRecognizer mainPhrases;
        private static KeywordRecognizer modPhrases;
        public static void VoiceRecognitionOn()
        {
            mainPhrases = new KeywordRecognizer(new string[] { "jarvis", "ii", "i i", "eye eye", "siri", "google", "alexa", "dummy", "computer", "stinky", "silly", "stupid", "console", "go go gadget" });
            mainPhrases.OnPhraseRecognized += ModRecognition;
            mainPhrases.Start();
        }

        private static Coroutine timeoutCoroutine;
        public static void ModRecognition(PhraseRecognizedEventArgs args)
        {
            mainPhrases.Stop();

            if (!GetIndex("Chain Voice Commands").enabled)
                timeoutCoroutine = CoroutineManager.RunCoroutine(Timeout());
            
            List<string> rawbuttonnames = new List<string> { "nevermind", "cancel", "never mind", "stop" };

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                    rawbuttonnames.Add(NoRichtextTags(v.overlapText == null ? v.buttonText : v.overlapText));
            }
            modPhrases = new KeywordRecognizer(rawbuttonnames.ToArray());
            modPhrases.OnPhraseRecognized += ExecuteVoiceCommand;
            modPhrases.Start();

            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/select.wav", "select.wav"), buttonClickVolume / 10f);
            
            NotifiLib.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
        }

        public static void ExecuteVoiceCommand(PhraseRecognizedEventArgs args)
        {
            if (!GetIndex("Chain Voice Commands").enabled)
            {
                modPhrases.Stop();
                mainPhrases.Start();
                CoroutineManager.EndCoroutine(timeoutCoroutine);
            }

            string output = args.text;
            if (output == "nevermind" || output == "cancel" || output == "never mind" || output == "stop")
            {
                CancelModRecognition();
                return;
            }

            string modTarget = null;
            bool exactMatch = false;

            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                if (exactMatch) { break; }
                foreach (ButtonInfo v in buttonlist)
                {
                    if (exactMatch) { break; }
                    string buttonName = NoRichtextTags(v.overlapText == null ? v.buttonText : v.overlapText);
                    if (output.ToLower() == buttonName.ToLower())
                    {
                        modTarget = v.buttonText;
                        exactMatch = true;
                    } else
                    {
                        if (output.Contains(buttonName.ToLower()))
                        {
                            modTarget = v.buttonText;
                        }
                    }
                }
            }
            if (modTarget != null)
            {
                ButtonInfo mod = GetIndex(modTarget);
                NotifiLib.SendNotification("<color=grey>[</color><color=" + (mod.enabled ? "red" : "green") + "> VOICE</color><color=grey>]</color> " + (mod.enabled ? "Disabling " : "Enabling ") + (mod.overlapText == null ? mod.buttonText : mod.overlapText)+"...", 3000);
                if (dynamicSounds)
                {
                    Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/confirm.wav", "confirm.wav"), buttonClickVolume / 10f);
                }
                Toggle(modTarget);
            } else
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> No command found ("+output+").", 3000);
                if (dynamicSounds)
                {
                    Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
                }
            }
        }

        public static IEnumerator Timeout()
        {
            yield return new WaitForSeconds(10f);
            CancelModRecognition();
        }

        public static void CancelModRecognition()
        {
            modPhrases.Stop();
            mainPhrases.Start();
            try
            {
                CoroutineManager.EndCoroutine(timeoutCoroutine);
            } catch { }

            NotifiLib.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> Cancelling...", 3000);
            if (dynamicSounds)
                Play2DAudio(LoadSoundFromURL("https://github.com/iiDk-the-actual/ModInfo/raw/main/close.wav", "close.wav"), buttonClickVolume / 10f);
        }

        public static void VoiceRecognitionOff()
        {
            mainPhrases?.Stop();
            
            modPhrases?.Stop();
            
            mainPhrases = null;
            modPhrases = null;
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

            string[] settings = new string[]
            {
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
                internetTime.ToString(),
                hotkeyButton.ToString(),
                buttonClickIndex.ToString(),
                buttonClickVolume.ToString(),
                Safety.antireportrangeindex.ToString(),
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
                gunLineQualityIndex.ToString()
            };

            string settingstext = string.Join(seperator, settings);

            string bindingtext = "";
            foreach (KeyValuePair<string, List<string>> Bind in ModBindings)
            {
                if (bindingtext != "")
                    bindingtext += "~~";

                string toAppend = Bind.Key;
                foreach (string modName in Bind.Value)
                {
                    toAppend += seperator + modName;
                }

                bindingtext += toAppend;
            }

            string finaltext =
                enabledtext + "\n" +
                favoritetext + "\n" +
                settingstext + "\n" +
                pageButtonType.ToString() + "\n" +
                themeType.ToString() + "\n" +
                fontCycle.ToString() + "\n" +
                bindingtext;

            return finaltext;
        }

        public static void SavePreferences()
        {
            File.WriteAllText("iisStupidMenu/iiMenu_Preferences.txt", SavePreferencesToText());
        }

        public static void LoadPreferencesFromText(string text)
        {
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

                internetTime = int.Parse(data[14]) - 1;
                ChangeReconnectTime();

                hotkeyButton = data[15];
                buttonClickIndex = int.Parse(data[16]) - 1;
                ChangeButtonSound();

                buttonClickVolume = int.Parse(data[17]) - 1;
                ChangeButtonVolume();

                Safety.antireportrangeindex = int.Parse(data[18]) - 1;
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
            } catch { LogManager.Log("Save file out of date"); }

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

                        List<string> Binds = new List<string> { };

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
            
            NotifiLib.ClearAllNotifications();
            hasLoadedPreferences = true;
        }

        public static void LoadPreferences()
        {
            try
            {
                string text = File.ReadAllText("iisStupidMenu/iiMenu_Preferences.txt");
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
                    {
                        Toggle(v.buttonText);
                    }
                }
            }
            NotifiLib.ClearAllNotifications();
        }

        public static void ChangeReconnectTime()
        {
            internetTime++;
            if (internetTime > 5)
                internetTime = 1;
            
            GetIndex("crTime").overlapText = "Change Reconnect Time <color=grey>[</color><color=green>" + internetTime.ToString() + "</color><color=grey>]</color>";
        }

        public static void ChangeButtonSound()
        {
            int[] sounds = new int[]
            {
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
                66
            };
            string[] buttonSoundNames = new string[]
            {
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
                "Valve"
            };

            buttonClickIndex++;
            if (buttonClickIndex > buttonSoundNames.Length - 1)
                buttonClickIndex = 0;
            
            buttonClickSound = sounds[buttonClickIndex];
            GetIndex("Change Button Sound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + buttonSoundNames[buttonClickIndex] + "</color><color=grey>]</color>";
        }

        public static void ChangeButtonVolume()
        {
            buttonClickVolume++;
            if (buttonClickVolume > 10)
                buttonClickVolume = 0;
            
            GetIndex("Change Button Volume").overlapText = "Change Button Volume <color=grey>[</color><color=green>" + buttonClickVolume.ToString() + "</color><color=grey>]</color>";
        }

        public static Material screenRed = null;
        public static Material screenBlack = null;
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
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)").SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").SetActive(true);

            int boardIndex = 0;
            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.childCount; i++)
            {
                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.GetChild(i).gameObject;
                if (v.name.Contains(StumpLeaderboardID))
                {
                    boardIndex++;
                    if (boardIndex == StumpLeaderboardIndex)
                        v.GetComponent<Renderer>().material = StumpMat;
                }
            }

            boardIndex = 0;
            for (int i = 0; i < GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.childCount; i++)
            {
                GameObject v = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(i).gameObject;
                if (v.name.Contains(ForestLeaderboardID))
                {
                    boardIndex++;
                    if (boardIndex == ForestLeaderboardIndex)
                        v.GetComponent<Renderer>().material = ForestMat;
                }
            }
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
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motd (1)").SetActive(false);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").SetActive(false);
        }
    }
}
