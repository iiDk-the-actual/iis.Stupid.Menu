using iiMenu.Classes;
using iiMenu.Menu;
using iiMenu.Notifications;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using static iiMenu.Menu.Main;
using System;
using iiMenu.Mods.Spammers;

namespace iiMenu.Mods
{
    internal class Settings
    {
        public static void EnableSettings()
        {
            buttonsType = 1;
            pageNumber = 0;
        }

        public static void ReturnToMain()
        {
            buttonsType = 0;
            pageNumber = 0;
        }

        public static void EnableMenuSettings()
        {
            buttonsType = 2;
            pageNumber = 0;
        }

        public static void EnableRoomSettings()
        {
            buttonsType = 3;
            pageNumber = 0;
        }

        public static void EnableMovementSettings()
        {
            buttonsType = 4;
            pageNumber = 0;
        }

        public static void EnableProjectileSettings()
        {
            buttonsType = 5;
            pageNumber = 0;
        }

        public static void EnableRoom()
        {
            buttonsType = 6;
            pageNumber = 0;
        }

        public static void EnableImportant()
        {
            buttonsType = 7;
            pageNumber = 0;
        }

        public static void EnableSafety()
        {
            buttonsType = 8;
            pageNumber = 0;
        }

        public static void EnableMovement()
        {
            buttonsType = 9;
            pageNumber = 0;
        }

        public static void EnableAdvantage()
        {
            buttonsType = 10;
            pageNumber = 0;
        }

        public static void EnableVisual()
        {
            buttonsType = 11;
            pageNumber = 0;
        }

        public static void EnableFun()
        {
            buttonsType = 12;
            pageNumber = 0;
        }

        public static void EnableSpam()
        {
            buttonsType = 13;
            pageNumber = 0;
        }

        public static void EnableSoundSpam()
        {
            buttonsType = 14;
            pageNumber = 0;
        }

        public static void EnableProjectileSpam()
        {
            buttonsType = 15;
            pageNumber = 0;
        }

        public static void EnableMaster()
        {
            buttonsType = 16;
            pageNumber = 0;
        }

        public static void EnableOverpowered()
        {
            buttonsType = 17;
            pageNumber = 0;
        }

        public static void EnableExperimental()
        {
            buttonsType = 18;
            pageNumber = 0;
        }

        public static void EnableFavorites()
        {
            buttonsType = 19;
            pageNumber = 0;
        }

        public static void EnableMenuPresets()
        {
            buttonsType = 20;
            pageNumber = 0;
        }

        public static void RightHand()
        {
            rightHand = true;
        }

        public static void LeftHand()
        {
            rightHand = false;
        }

        public static void BothHandsOn()
        {
            bothHands = true;
        }

        public static void BothHandsOff()
        {
            bothHands = false;
        }

        public static void WristThingOn()
        {
            wristThing = true;
        }

        public static void WristThingOff()
        {
            wristThing = false;
            wristOpen = false;
        }

        public static void LongMenuOn()
        {
            longmenu = true;
        }

        public static void LongMenuOff()
        {
            longmenu = false;
        }

        public static void ChangeMenuTheme()
        {
            themeType++;
            if (themeType > 14)
            {
                themeType = 1;
            }

            if (themeType == 1)
            {
                bgColorA = new Color32(255, 128, 0, 128);
                bgColorB = new Color32(255, 102, 0, 128);
                buttonDefaultA = new Color32(170, 85, 0, 255);
                buttonDefaultB = new Color32(170, 85, 0, 255);
                buttonClickedA = new Color32(85, 42, 0, 255);
                buttonClickedB = new Color32(85, 42, 0, 255);
                textColor = new Color32(255, 190, 125, 255);
            }
            if (themeType == 2)
            {
                bgColorA = Color.blue;
                bgColorB = Color.magenta;
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = Color.blue;
                buttonClickedB = Color.blue;
                textColor = Color.white;
            }
            if (themeType == 3)
            {
                bgColorA = Color.black;
                bgColorB = Color.black;
                buttonDefaultA = new Color32(50, 50, 50, 255);
                buttonDefaultB = new Color32(50, 50, 50, 255);
                buttonClickedA = new Color32(20, 20, 20, 255);
                buttonClickedB = new Color32(20, 20, 20, 255);
                textColor = Color.white;
            }
            if (themeType == 4)
            {
                bgColorA = Color.white;
                bgColorB = Color.black;
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.white;
                buttonClickedA = Color.white;
                buttonClickedB = Color.white;
                textColor = Color.black;
            }
            if (themeType == 5)
            {
                bgColorA = Color.black;
                bgColorB = new Color32(110, 0, 0, 255);
                buttonDefaultA = Color.black;
                buttonDefaultB = new Color32(110, 0, 0, 255);
                buttonClickedA = new Color32(110, 0, 0, 255);
                buttonClickedB = new Color32(110, 0, 0, 255);
                textColor = Color.white;
            }
            if (themeType == 6)
            {
                bgColorA = Color.black;
                bgColorB = Color.black;
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = Color.black;
                buttonClickedB = Color.black;
                textColor = Color.white;
            }
            if (themeType == 7)
            {
                bgColorA = new Color32(255, 128, 0, 128);
                bgColorB = new Color32(255, 128, 0, 128);
                buttonDefaultA = new Color32(170, 85, 0, 255);
                buttonDefaultB = new Color32(170, 85, 0, 255);
                buttonClickedA = new Color32(85, 42, 0, 255);
                buttonClickedB = new Color32(85, 42, 0, 255);
                textColor = new Color32(255, 190, 125, 255);
                GetIndex("Thin Menu").enabled = true;
                FATMENU = true;
            }
            if (themeType == 8)
            {
                bgColorA = Color.black;
                bgColorB = Color.black;
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = Color.black;
                buttonClickedB = Color.black;
                textColor = Color.white;
            }
            if (themeType == 9)
            {
                bgColorA = Color.black;
                bgColorB = new Color32(255, 111, 0, 255);
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = new Color32(255, 111, 0, 255);
                buttonClickedB = Color.black;
                textColor = Color.white;
            }
            if (themeType == 10)
            {
                bgColorA = Color.black;
                bgColorB = Color.red;
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = Color.red;
                buttonClickedB = Color.black;
                textColor = Color.white;
            }
            if (themeType == 11)
            {
                bgColorA = Color.black;
                bgColorB = new Color32(0, 174, 255, 255);
                buttonDefaultA = Color.black;
                buttonDefaultB = Color.black;
                buttonClickedA = new Color32(0, 174, 255, 255);
                buttonClickedB = Color.black;
                textColor = Color.white;
            }
            if (themeType == 12)
            {
                bgColorA = new Color32(0, 136, 255, 255);
                bgColorB = new Color32(0, 174, 255, 255);
                buttonDefaultA = new Color32(0, 100, 188, 255);
                buttonDefaultB = new Color32(0, 100, 188, 255);
                buttonClickedA = new Color32(0, 174, 255, 255);
                buttonClickedB = new Color32(0, 136, 255, 255);
                textColor = Color.black;
            }
            if (themeType == 13)
            {
                bgColorA = new Color32(0, 255, 246, 255);
                bgColorB = new Color32(0, 255, 144, 255);
                buttonDefaultA = new Color32(0, 255, 144, 255);
                buttonDefaultB = new Color32(0, 255, 144, 255);
                buttonClickedA = new Color32(0, 255, 246, 255);
                buttonClickedB = new Color32(0, 255, 246, 255);
                textColor = Color.black;
            }
            if (themeType == 14)
            {
                bgColorA = new Color32(255, 130, 255, 255);
                bgColorB = Color.white;
                buttonDefaultA = new Color32(255, 130, 255, 255);
                buttonDefaultB = new Color32(255, 130, 255, 255);
                buttonClickedA = Color.white;
                buttonClickedB = Color.white;
                textColor = Color.black;
            }
        }

        public static void ChangePageType()
        {
            pageButtonType++;
            if (pageButtonType > 4)
            {
                pageButtonType = 1;
            }

            if (pageButtonType == 1)
            {
                pageSize = 6;
                buttonOffset = 2;
            }
            if (pageButtonType == 2)
            {
                pageSize = 8;
                buttonOffset = 0;
            }
            if (pageButtonType == 3)
            {
                pageSize = 8;
                buttonOffset = 0;
            }
        }

        public static void ChangeFontType()
        {
            fontCycle++;
            if (fontCycle > 5)
            {
                fontCycle = 0;
            }

            if (fontCycle == 0)
            {
                activeFont = agency;
            }
            if (fontCycle == 1)
            {
                activeFont = Arial;
            }
            if (fontCycle == 2)
            {
                activeFont = Verdana;
            }
            if (fontCycle == 3)
            {
                if (gtagfont == null)
                {
                    GameObject fart = LoadAsset("font", "font");
                    gtagfont = fart.transform.Find("text").gameObject.GetComponent<Text>().font;
                    UnityEngine.Object.Destroy(fart);
                }
                activeFont = gtagfont;
            }
            if (fontCycle == 4)
            {
                activeFont = sans;
            }
            if (fontCycle == 5)
            {
                activeFont = consolas;
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

        public static void DisableNotifications()
        {
            disableNotifications = true;
        }

        public static void EnableNotifications()
        {
            disableNotifications = false;
        }

        public static void SavePreferences()
        {
            string text = "";
            foreach (ButtonInfo[] buttonlist in Buttons.buttons)
            {
                foreach (ButtonInfo v in buttonlist)
                {
                    if (v.enabled && v.buttonText != "Save Preferences")
                    {
                        if (text == "")
                        {
                            text += v.buttonText;
                        }
                        else
                        {
                            text += "\n" + v.buttonText;
                        }
                    }
                }
            }

            string favz = "";
            foreach (string fav in favorites)
            {
                if (favz == "")
                {
                    favz += fav;
                }
                else
                {
                    favz += "\n" + fav;
                }
            }

            string ihateyouguys = platformMode+"\n"+platformShape+"\n"+flySpeedCycle+"\n"+longarmCycle+"\n"+speedboostCycle+"\n"+projmode+"\n"+trailmode+"\n"+shootCycle;

            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            File.WriteAllText("iisStupidMenu/iiMenu_EnabledMods.txt", text);
            File.WriteAllText("iisStupidMenu/iiMenu_FavoriteMods.txt", favz);
            File.WriteAllText("iisStupidMenu/iiMenu_ModData.txt", ihateyouguys.ToString());
            File.WriteAllText("iisStupidMenu/iiMenu_PageType.txt", pageButtonType.ToString());
            File.WriteAllText("iisStupidMenu/iiMenu_Theme.txt", themeType.ToString());
            File.WriteAllText("iisStupidMenu/iiMenu_Font.txt", fontCycle.ToString());
        }

        public static void LoadPreferences()
        {
            if (Directory.Exists("iisStupidMenu"))
            {
                Panic();

                try
                {
                    string config = File.ReadAllText("iisStupidMenu/iiMenu_EnabledMods.txt");
                    string[] activebuttons = config.Split("\n");
                    for (int index = 0; index < activebuttons.Length; index++)
                    {
                        Toggle(activebuttons[index]);
                    }
                }
                catch { }

                try
                {
                    string favez = File.ReadAllText("iisStupidMenu/iiMenu_FavoriteMods.txt");
                    string[] favz = favez.Split("\n");

                    favorites.Clear();
                    foreach (string fav in favz)
                    {
                        favorites.Add(fav);
                    }
                }
                catch { }

                try
                {
                    string MODDER = File.ReadAllText("iisStupidMenu/iiMenu_ModData.txt");
                    string[] data = MODDER.Split("\n");

                    platformMode = int.Parse(data[0]) - 1;
                    Movement.ChangePlatformType();
                    platformShape = int.Parse(data[1]) - 1;
                    Movement.ChangePlatformShape();
                    flySpeedCycle = int.Parse(data[2]) - 1;
                    Movement.ChangeFlySpeed();
                    longarmCycle = int.Parse(data[3]) - 1;
                    Movement.ChangeArmLength();
                    speedboostCycle = int.Parse(data[4]) - 1;
                    Movement.ChangeSpeedBoostAmount();
                    projmode = int.Parse(data[5]) - 1;
                    Projectiles.ChangeProjectile();
                    trailmode = int.Parse(data[6]) - 1;
                    Projectiles.ChangeTrail();
                }
                catch { }

                string pager = File.ReadAllText("iisStupidMenu/iiMenu_PageType.txt");
                string themer = File.ReadAllText("iisStupidMenu/iiMenu_Theme.txt");
                string fonter = File.ReadAllText("iisStupidMenu/iiMenu_Font.txt");

                pageButtonType = int.Parse(pager) - 1;
                Toggle("Change Page Type");
                themeType = int.Parse(themer) - 1;
                Toggle("Change Menu Theme");
                fontCycle = int.Parse(fonter) - 1;
                Toggle("Change Font Type");
                NotifiLib.ClearAllNotifications();
            } else
            {
                UnityEngine.Debug.Log("Could not load preferences, try migrating to folder?");
            }
        }

        public static void Panic()
        {
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

        public static void ThinMenuOn()
        {
            FATMENU = true;
        }

        public static void ThinMenuOff()
        {
            FATMENU = false;
        }

        public static void CrashAmount()
        {
            crashAmount++;
            if (crashAmount > 10)
            {
                crashAmount = 1;
            }
            
            GetIndex("Crash Amount").overlapText = "Crash Amount <color=grey>[</color><color=green>" + crashAmount.ToString() + "</color><color=grey>]</color>";
        }
    }
}
