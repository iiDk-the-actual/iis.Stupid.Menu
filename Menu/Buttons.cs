/*
 * ii's Stupid Menu  Menu/Buttons.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
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

using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.ObstacleCourse;
using iiMenu.Classes.Menu;
using iiMenu.Managers;
using iiMenu.Mods;
using iiMenu.Patches.Menu;
using iiMenu.Patches.Safety;
using iiMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.RandomUtilities;
using static iiMenu.Utilities.RigUtilities;
using Console = iiMenu.Classes.Menu.Console;
using Random = UnityEngine.Random;

namespace iiMenu.Menu
{
    public static class Buttons
    {
        public static ButtonInfo[][] buttons =
        {
            new[] { // Main [0]
                
                new ButtonInfo { buttonText = "Join Discord", method = Important.JoinDiscord, isTogglable = false, toolTip = "Invites you to join the ii's <b>Stupid</b> Mods Discord server."},

                new ButtonInfo { buttonText = "Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Opens the settings tab."},
                new ButtonInfo { buttonText = "Friends", method =() => CurrentCategoryName = "Friends", isTogglable = false, toolTip = "Opens the friends tab."},
                new ButtonInfo { buttonText = "Players", method = Settings.PlayersTab, isTogglable = false, toolTip = "Opens the players tab."},

                new ButtonInfo { buttonText = "Favorite Mods", method =() => CurrentCategoryName = "Favorite Mods", isTogglable = false, toolTip = "Opens your favorite mods. Favorite mods with left grip."},
                new ButtonInfo { buttonText = "Enabled Mods", method =() => CurrentCategoryName = "Enabled Mods", isTogglable = false, toolTip = "Shows all mods you have enabled."},
                new ButtonInfo { buttonText = "Room Mods", method =() => CurrentCategoryName = "Room Mods", isTogglable = false, toolTip = "Opens the room mods."},
                new ButtonInfo { buttonText = "Important Mods", method =() => CurrentCategoryName = "Important Mods", isTogglable = false, toolTip = "Opens the important mods."},
                new ButtonInfo { buttonText = "Safety Mods", method =() => CurrentCategoryName = "Safety Mods", isTogglable = false, toolTip = "Opens the safety mods."},
                new ButtonInfo { buttonText = "Movement Mods", method =() => CurrentCategoryName = "Movement Mods", isTogglable = false, toolTip = "Opens the movement mods."},
                new ButtonInfo { buttonText = "Advantage Mods", method =() => CurrentCategoryName = "Advantage Mods", isTogglable = false, toolTip = "Opens the advantage mods."},
                new ButtonInfo { buttonText = "Visual Mods", method =() => CurrentCategoryName = "Visual Mods", isTogglable = false, toolTip = "Opens the visual mods."},
                new ButtonInfo { buttonText = "Fun Mods", method =() => CurrentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Opens the fun mods."},
                new ButtonInfo { buttonText = "Sound Mods", method =() => CurrentCategoryName = "Sound Mods", isTogglable = false, toolTip = "Opens the sound mods."},
                new ButtonInfo { buttonText = "Projectile Mods", method =() => CurrentCategoryName = "Projectile Mods", isTogglable = false, toolTip = "Opens the projectile mods."},
                new ButtonInfo { buttonText = "Master Mods", method =() => CurrentCategoryName = "Master Mods", isTogglable = false, toolTip = "Opens the master mods."},
                new ButtonInfo { buttonText = "Overpowered Mods", method =() => CurrentCategoryName = "Overpowered Mods", isTogglable = false, toolTip = "Opens the overpowered mods."},
                new ButtonInfo { buttonText = "Experimental Mods", method =() => CurrentCategoryName = "Experimental Mods", isTogglable = false, toolTip = "Opens the experimental mods."},
                new ButtonInfo { buttonText = "Detected Mods", method = Detected.EnterDetectedTab, isTogglable = false, toolTip = "Opens the detected mods."},

                new ButtonInfo { buttonText = "Achievements", method = AchievementManager.EnterAchievementTab, isTogglable = false, toolTip = "Opens the achievements page."},
                new ButtonInfo { buttonText = "Credits", method =() => CurrentCategoryName = "Credits", isTogglable = false, toolTip = "Opens the credits page."}
            },

            new[] { // Settings [1]
                new ButtonInfo { buttonText = "Exit Settings", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Menu Settings", method =() => CurrentCategoryName = "Menu Settings", isTogglable = false, toolTip = "Opens the settings for the menu."},
                new ButtonInfo { buttonText = "Category Settings", method = Settings.CategorySettings, isTogglable = false, toolTip = "Opens the settings for the categories."},

                new ButtonInfo { buttonText = "Keybind Settings", method =() => CurrentCategoryName = "Keybind Settings", isTogglable = false, toolTip = "Opens the settings for the keybinds."},
                new ButtonInfo { buttonText = "Rebind Settings", method =() => CurrentCategoryName = "Rebind Settings", isTogglable = false, toolTip = "Opens the settings for rebinds."},
                new ButtonInfo { buttonText = "Plugin Settings", method =() => CurrentCategoryName = "Plugin Settings", isTogglable = false, toolTip = "Opens the settings for the plugins."},

                new ButtonInfo { buttonText = "Soundboard Settings", method =() => CurrentCategoryName = "Soundboard Settings", isTogglable = false, toolTip = "Opens the settings for the soundboard."},
                new ButtonInfo { buttonText = "Friend Settings", method =() => CurrentCategoryName = "Friend Settings", isTogglable = false, toolTip = "Opens the settings for the friend system."},

                new ButtonInfo { buttonText = "Room Settings", method =() => CurrentCategoryName = "Room Settings", isTogglable = false, toolTip = "Opens the settings for the room mods."},
                new ButtonInfo { buttonText = "Safety Settings", method =() => CurrentCategoryName = "Safety Settings", isTogglable = false, toolTip = "Opens the settings for the safety mods."},
                new ButtonInfo { buttonText = "Movement Settings", method =() => CurrentCategoryName = "Movement Settings", isTogglable = false, toolTip = "Opens the settings for the movement mods."},
                new ButtonInfo { buttonText = "Advantage Settings", method =() => CurrentCategoryName = "Advantage Settings", isTogglable = false, toolTip = "Opens the settings for the advantage mods."},
                new ButtonInfo { buttonText = "Visual Settings", method =() => CurrentCategoryName = "Visual Settings", isTogglable = false, toolTip = "Opens the settings for the visual mods."},
                new ButtonInfo { buttonText = "Fun Settings", method =() => CurrentCategoryName = "Fun Settings", isTogglable = false, toolTip = "Opens the settings for the fun mods."},
                new ButtonInfo { buttonText = "Overpowered Settings", method =() => CurrentCategoryName = "Overpowered Settings", isTogglable = false, toolTip = "Opens the settings for the overpowered mods."},
                new ButtonInfo { buttonText = "Detected Settings", method =() => CurrentCategoryName = "Detected Settings", isTogglable = false, toolTip = "Opens the settings for the detected mods."},
                new ButtonInfo { buttonText = "Projectile Settings", method =() => CurrentCategoryName = "Projectile Settings", isTogglable = false, toolTip = "Opens the settings for the projectiles."}
            },

            new[] { // Menu Settings [2]
                new ButtonInfo { buttonText = "Exit Menu Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Right Hand", enableMethod = Settings.RightHand, disableMethod = Settings.LeftHand, toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Both Hands", enableMethod =() => bothHands = true, disableMethod =() => bothHands = false, toolTip = "Puts the menu on your both of your hands."},

                new ButtonInfo { buttonText = "One Handed Menu", enableMethod =() => oneHand = true, disableMethod =() => oneHand = false, toolTip = "Makes the menu open in front of you, so you can use it with one hand."},
                new ButtonInfo { buttonText = "Joystick Menu", enableMethod =() => joystickMenu = true, disableMethod = Settings.JoystickMenuOff, toolTip = "Makes the menu into something like Colossal, click your joysticks to open, joysticks to move between mods and pages, and click your left joystick to toggle a mod."},
                new ButtonInfo { buttonText = "Physical Menu", enableMethod = Settings.PhysicalMenuOn, disableMethod = Settings.PhysicalMenuOff, toolTip = "Freezes the menu in world space."},
                new ButtonInfo { buttonText = "Bark Menu", enableMethod =() => barkMenu = true, disableMethod =() => barkMenu = false, toolTip = "Allows you to spawn the menu similar to bark by banging on your chest."},
                new ButtonInfo { buttonText = "Wrist Menu", enableMethod =() => wristMenu = true, disableMethod =() => wristMenu = false, toolTip = "Turns the menu into a weird wrist watch, click your hand to open it."},
                new ButtonInfo { buttonText = "Watch Menu", enableMethod = Settings.WatchMenuOn, method = Settings.CheckWatchMenu, disableMethod = Settings.WatchMenuOff, toolTip = "Turns the menu into a watch, click your joystick to toggle, and move your joystick to select a mod."},
                new ButtonInfo { buttonText = "Shiny Menu", enableMethod =() => shinyMenu = true, disableMethod =() => shinyMenu = false, toolTip = "Makes the menu's textures use the old shader."},
                new ButtonInfo { buttonText = "Transparent Menu", enableMethod =() => transparentMenu = true, disableMethod =() => transparentMenu = false, toolTip = "Makes the menu transparent."},
                new ButtonInfo { buttonText = "Crystallize Menu", enableMethod =() => { crystallizeMenu = true; CustomBoardManager.BoardMaterial = CrystalMaterial; }, disableMethod =() => { crystallizeMenu = false ; CustomBoardManager.BoardMaterial = null; }, toolTip = "Turns the menu into crystals."},
                new ButtonInfo { buttonText = "Explode Menu", enableMethod =() => explodeMenu = true, disableMethod =() => explodeMenu = false, toolTip = "Makes the menu explode when closing it."},
                new ButtonInfo { buttonText = "Thick Menu", enableMethod =() => thinMenu = false, disableMethod =() => thinMenu = true, toolTip = "Makes the menu thin."},
                new ButtonInfo { buttonText = "Long Menu", enableMethod =() => longmenu = true, disableMethod =() => longmenu = false, toolTip = "Makes the menu long."},
                new ButtonInfo { buttonText = "Flip Menu", enableMethod =() => flipMenu = true, disableMethod =() => flipMenu = false, toolTip = "Flips the menu to the back of your hand."},

                new ButtonInfo { buttonText = "Round Menu", enableMethod =() => shouldRound = true, disableMethod =() => shouldRound = false, toolTip = "Makes the menu objects round."},
                new ButtonInfo { buttonText = "Outline Menu", enableMethod =() => shouldOutline = true, disableMethod =() => shouldOutline = false, toolTip = "Gives the menu objects an outline."},
                new ButtonInfo { buttonText = "Outline Text", enableMethod =() => outlineText = true, disableMethod =() => outlineText = false, toolTip = "Gives the text objects an outline."},
                new ButtonInfo { buttonText = "Strikethrough Text", enableMethod =() => strikethroughText = true, disableMethod =() => strikethroughText = false, toolTip = "Strikes out all text on the menu."},
                new ButtonInfo { buttonText = "Underline Text", enableMethod =() => underlineText = true, disableMethod =() => underlineText = false, toolTip = "Underlines all text on the menu."},
                new ButtonInfo { buttonText = "Small-Caps Text", enableMethod =() => smallCapsText = true, disableMethod =() => smallCapsText = false, toolTip = "Turns all text into a small capital version."},
                new ButtonInfo { buttonText = "Redact Text", enableMethod =() => redactText = true, disableMethod =() => redactText = false, toolTip = "Redacts all text on the menu."},
                new ButtonInfo { buttonText = "Inner Outline Menu", enableMethod =() => innerOutline = true, disableMethod =() => innerOutline = false, toolTip = "Gives the menu an outline on the inside."},
                new ButtonInfo { buttonText = "Smooth Menu Position", enableMethod =() => smoothMenuPosition = true, disableMethod =() => smoothMenuPosition = false, toolTip = "Smoothes the menu's position."},
                new ButtonInfo { buttonText = "Smooth Menu Rotation", enableMethod =() => smoothMenuRotation = true, disableMethod =() => smoothMenuRotation = false, toolTip = "Smoothes the menu's rotation."},

                new ButtonInfo { buttonText = "Freeze Player in Menu", method = Settings.FreezePlayerInMenu, enableMethod =() => closePosition = GorillaTagger.Instance.rigidbody.transform.position, toolTip = "Freezes your character when inside the menu."},
                new ButtonInfo { buttonText = "Freeze Rig in Menu", overlapText = "Ghost Rig in Menu", method = Settings.FreezeRigInMenu, disableMethod = Movement.EnableRig, toolTip = "Freezes your rig when inside the menu."},
                new ButtonInfo { buttonText = "Zero Gravity Menu", enableMethod =() => zeroGravityMenu = true, disableMethod =() => zeroGravityMenu = false, toolTip = "Disables gravity on the menu when dropping it."},
                new ButtonInfo { buttonText = "Menu Collisions", enableMethod =() => menuCollisions = true, disableMethod =() => menuCollisions = false, toolTip = "Gives the menu collisions when dropping it."},
                new ButtonInfo { buttonText = "Player Scale Menu", enableMethod =() => scaleWithPlayer = true, disableMethod =() => scaleWithPlayer = false, toolTip = "Scales the menu with your player scale."},
                new ButtonInfo { buttonText = "Alphabetize Menu", toolTip = "Alphabetizes the entire menu."},
                new ButtonInfo { buttonText = "Custom Menu Name", enableMethod = Settings.CustomMenuName, disableMethod =() => doCustomName = false, toolTip = $"Changes the name of the menu to whatever. You can change the text inside of your Gorilla Tag files ({PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt)."},
                new ButtonInfo { buttonText = "Menu Trail", enableMethod =() => menuTrail = true, disableMethod =() => menuTrail = false, toolTip = "Gives the menu a trail when you drop."},

                new ButtonInfo { buttonText = "Dynamic Animations", enableMethod =() => dynamicAnimations = true, disableMethod =() => dynamicAnimations = false, toolTip = "Adds more animations to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Slow Dynamic Animations", enableMethod =() => slowDynamicAnimations = true, disableMethod =() => slowDynamicAnimations = false, toolTip = "Makes Dynamic Animations slower."},
                new ButtonInfo { buttonText = "Dynamic Gradients", enableMethod =() => dynamicGradients = true, disableMethod =() => dynamicGradients = false, toolTip = "Makes gradients dynamic, showing you the full gradient instead of a pulsing color."},
                new ButtonInfo { buttonText = "Horizontal Gradients", enableMethod =() => { horizontalGradients = true; cacheGradients.Clear(); }, disableMethod =() => { horizontalGradients = false; cacheGradients.Clear(); }, toolTip = "Rotates the dynamic gradients by 90 degrees."},
                new ButtonInfo { buttonText = "Scrolling Gradients", enableMethod =() => scrollingGradients = true, disableMethod =() => scrollingGradients = false, toolTip = "Scrolls the dynamic gradients over time."},
                new ButtonInfo { buttonText = "Dynamic Sounds", enableMethod =() => dynamicSounds = true, disableMethod =() => dynamicSounds = false, toolTip = "Adds more sounds to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Disable Adaptive Buttons", enableMethod =() => adaptiveButtons = false, disableMethod =() => adaptiveButtons = true, toolTip = "Disables the rebinding of buttons to make your experience better based on what controllers you're using."},
                new ButtonInfo { buttonText = "Incremental Boost", enableMethod =() => incrementalBoost = true, disableMethod =() => incrementalBoost = false, toolTip = "Allows you to increment faster by holding down your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Page Scrolling", enableMethod =() => pageScrolling = true, disableMethod =() => { pageScrolling = false; pageOffset = 0; }, toolTip = "Allows you to scroll through the mods with your joystick instead of flipping through pages."},
                new ButtonInfo { buttonText = "Exclusive Page Sounds", enableMethod =() => exclusivePageSounds = true, disableMethod =() => exclusivePageSounds = false, toolTip = "Makes the sound that joystick menu makes when switching pages using the menu."},
                new ButtonInfo { buttonText = "Particle Spawn Effect", enableMethod =() => particleSpawnEffect = true, disableMethod =() => particleSpawnEffect = false, toolTip = "Spawns particles when opening the menu."},
                new ButtonInfo { buttonText = "Gradient Title", enableMethod =() => gradientTitle = true, disableMethod =() => gradientTitle = false, toolTip = "Gives a gradient to the title of the menu depending on your theme."},
                new ButtonInfo { buttonText = "Animated Title", enableMethod =() => animatedTitle = true, disableMethod =() => animatedTitle = false, toolTip = "Animates the title of the menu."},
                new ButtonInfo { buttonText = "Voice Commands", enableMethod = Settings.VoiceRecognitionOn, method = Settings.CheckFocus, disableMethod = Settings.VoiceRecognitionOff, toolTip = "Enable and disable mods using your voice. Activate it like how you would any other voice assistant, such as \"Jarvis, Platforms\"."},
                new ButtonInfo { buttonText = "Chain Voice Commands", toolTip = "Makes voice commands chain together, so you don't have to repeatedly ask it to listen to you."},
                new ButtonInfo { buttonText = "AI Assistant", enableMethod =() => CoroutineManager.instance.StartCoroutine(Settings.DictationOn()), method = Settings.CheckFocus, disableMethod = Settings.DictationOff, toolTip = "A voice assistant with artificial intelligence capabilities."},
                new ButtonInfo { buttonText = "Click GUI", enableMethod = Settings.EnableClickGUI, method = Settings.ClickGUI, disableMethod = Settings.DisableClickGUI, toolTip = "A modern version of the menu."},

                new ButtonInfo { buttonText = "Narrate Assistant", toolTip = "Narrates what the voice assistant says locally."},
                new ButtonInfo { buttonText = "Global Narrate Assistant", toolTip = "Narrates what the voice assistant says globally."},
                new ButtonInfo { buttonText = "Global Dynamic Sounds", toolTip = "Plays the dynamic sounds through your microphone."},

                new ButtonInfo { buttonText = "Debug Dictation", enableMethod =() => Settings.debugDictation = true, disableMethod =() => Settings.debugDictation = false, toolTip = "Debug what you say to the AI Assistant in your Unity console."},

                new ButtonInfo { buttonText = "Custom System Prompt", enableMethod =() => AIManager.customPrompt = true, disableMethod =() => AIManager.customPrompt = false, toolTip = "Never resets the system prompt, allowing you to edit the file."},

                new ButtonInfo { buttonText = "Reset Voice Commands Keywords", method = Settings.ResetVoiceCommandsKeywords, isTogglable = false, toolTip = "Resets the keywords for all the voice command related mods."},
                new ButtonInfo { buttonText = "Reset System Prompt", method = Settings.ResetSystemPrompt, isTogglable = false, toolTip = "Resets the system prompt for the AI Assistant."},

                new ButtonInfo { buttonText = "Player Select", method = Settings.PlayerSelect, toolTip = "Spawns a line in your hand when moving your hand away from the menu that you can select players with."},
                new ButtonInfo { buttonText = "Menu Intro", enableMethod = Settings.MenuIntro, toolTip = "Plays an intro for the menu."},

                new ButtonInfo { buttonText = "Annoying Mode", enableMethod =() => annoyingMode = true, disableMethod = Settings.AnnoyingModeOff, toolTip = "Turns on the April Fools 2024 settings."},
                new ButtonInfo { buttonText = "Lowercase Mode", enableMethod =() => lowercaseMode = true, disableMethod =() => lowercaseMode = false, toolTip = "Makes the entire menu's text lowercase."},
                new ButtonInfo { buttonText = "Uppercase Mode", enableMethod =() => uppercaseMode = true, disableMethod =() => uppercaseMode = false, toolTip = "Makes the entire menu's text uppercase."},
                new ButtonInfo { buttonText = "Overflow Mode", enableMethod =() => NoAutoSizeText = true, disableMethod =() => NoAutoSizeText = false, toolTip = "Makes the entire menu's text overflow."},

                new ButtonInfo { buttonText = "Change Menu Language", overlapText = "Change Menu Language <color=grey>[</color><color=green>English</color><color=grey>]</color>", method =() => Settings.ChangeMenuLanguage(), enableMethod =() => Settings.ChangeMenuLanguage(), disableMethod =() => Settings.ChangeMenuLanguage(false), incremental = true, isTogglable = false, toolTip = "Changes the language of the menu."},
                new ButtonInfo { buttonText = "Change Menu Button", overlapText = "Change Menu Button <color=grey>[</color><color=green>Secondary</color><color=grey>]</color>", method =() => Settings.ChangeMenuButton(), enableMethod =() => Settings.ChangeMenuButton(), disableMethod =() => Settings.ChangeMenuButton(false), incremental = true, isTogglable = false, toolTip = "Changes the button used to open menu."},
                new ButtonInfo { buttonText = "Menu Toggle Button", enableMethod =() => toggleButton = true, disableMethod =() => toggleButton = false, toolTip = "Allows the menu to be toggled on and off with the menu button."},
                new ButtonInfo { buttonText = "Change Menu Theme", method =() => Settings.ChangeMenuTheme(), enableMethod =() => Settings.ChangeMenuTheme(), disableMethod =() => Settings.ChangeMenuTheme(false), incremental = true, isTogglable = false, toolTip = "Changes the theme of the menu."},
                new ButtonInfo { buttonText = "Slow Gradient Fade", enableMethod =() => slowFadeColors = true, disableMethod =() => slowFadeColors = false, toolTip = "Makes gradient themes fade slower on the background."},
                new ButtonInfo { buttonText = "Change Menu Scale", overlapText = "Change Menu Scale <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeMenuScale(), enableMethod =() => Settings.ChangeMenuScale(), disableMethod =() => Settings.ChangeMenuScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the menu."},
                new ButtonInfo { buttonText = "Change Notification Scale", overlapText = "Change Notification Scale <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangeNotificationScale(), enableMethod =() => Settings.ChangeNotificationScale(), disableMethod =() => Settings.ChangeNotificationScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the notifications."},
                new ButtonInfo { buttonText = "Change Arraylist Scale", overlapText = "Change Arraylist Scale <color=grey>[</color><color=green>4</color><color=grey>]</color>", method =() => Settings.ChangeArraylistScale(), enableMethod =() => Settings.ChangeArraylistScale(), disableMethod =() => Settings.ChangeArraylistScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the arraylist."},
                new ButtonInfo { buttonText = "Change Overlay Scale", overlapText = "Change Overlay Scale <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangeOverlayScale(), enableMethod =() => Settings.ChangeOverlayScale(), disableMethod =() => Settings.ChangeOverlayScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the overlay."},
                new ButtonInfo { buttonText = "Change Page Size", overlapText = "Change Page Size <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangePageSize(), enableMethod =() => Settings.ChangePageSize(), disableMethod =() => Settings.ChangePageSize(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of buttons per page."},
                new ButtonInfo { buttonText = "Change Character Distance", overlapText = "Change Character Distance <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeCharacterDistance(), enableMethod =() => Settings.ChangeCharacterDistance(), disableMethod =() => Settings.ChangeCharacterDistance(false), incremental = true, isTogglable = false, toolTip = "Changes the distance between characters on the menu."},
                new ButtonInfo { buttonText = "Custom Menu Theme", enableMethod = Settings.CustomMenuTheme, disableMethod = Settings.FixTheme, toolTip = "Changes the theme of the menu to a custom one."},
                new ButtonInfo { buttonText = "Change Custom Menu Theme", method = Settings.ChangeCustomMenuTheme, isTogglable = false, toolTip = "Changes the theme of custom the menu."},
                new ButtonInfo { buttonText = "Custom Menu Background", enableMethod = Settings.CustomMenuBackground, disableMethod = Settings.FixMenuBackground, toolTip = $"Changes the background of the menu to a custom image. You can change the photo inside of your Gorilla Tag files ({PluginInfo.BaseDirectory}/CustomBackground.png)."},
                new ButtonInfo { buttonText = "Custom Watermark", enableMethod = Settings.CustomWatermark, disableMethod =() => customWatermark = null, toolTip = $"Changes the watermark on the UI and the back of the menu to a custom image. You can change the photo inside of your Gorilla Tag files ({PluginInfo.BaseDirectory}/CustomWatermark.png)."},
                new ButtonInfo { buttonText = "Disable Watermark", enableMethod =() => disableWatermark = true, disableMethod =() => disableWatermark = false, toolTip = "Disables the watermark on the UI and the back of the menu."},
                new ButtonInfo { buttonText = "Change Page Type", method =() => Settings.ChangePageType(), enableMethod =() => Settings.ChangePageType(), disableMethod =() => Settings.ChangePageType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of page buttons."},
                new ButtonInfo { buttonText = "Change Arrow Type", method =() => Settings.ChangeArrowType(), enableMethod =() => Settings.ChangeArrowType(), disableMethod =() => Settings.ChangeArrowType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of arrows on the page buttons."},
                new ButtonInfo { buttonText = "Change Font Type", method =() => Settings.ChangeFontType(), enableMethod =() => Settings.ChangeFontType(), disableMethod =() => Settings.ChangeFontType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of font."},
                new ButtonInfo { buttonText = "Rapid Font Changer", method = Settings.ChangeFontRapid, toolTip = "Changes the type of font every menu refresh."},
                new ButtonInfo { buttonText = "Custom Font Type", enableMethod = Settings.CustomFontType, method = Settings.PersistCustomFont, disableMethod = Settings.DisableCustomFont, toolTip = $"Changes the font type on the menu to a custom font. You can change the photo inside of your Gorilla Tag files ({PluginInfo.BaseDirectory}/iiMenu_CustomWatermark.txt)."},
                new ButtonInfo { buttonText = "Change Font Style Type", method =() => Settings.ChangeFontStyleType(), enableMethod =() => Settings.ChangeFontStyleType(), disableMethod =() => Settings.ChangeFontStyleType(false), incremental = true, isTogglable = false, toolTip = "Changes the style of the font."},
                new ButtonInfo { buttonText = "Change Input Text Color", overlapText = "Change Input Text Color <color=grey>[</color><color=green>Green</color><color=grey>]</color>", method =() => Settings.ChangeInputTextColor(), enableMethod =() => Settings.ChangeInputTextColor(), disableMethod =() => Settings.ChangeInputTextColor(false), incremental = true, isTogglable = false, toolTip = "Changes the color of the input indicator next to the buttons."},
                new ButtonInfo { buttonText = "Vibrant Text Colors", enableMethod =() => vibrantColors = true, disableMethod =() => vibrantColors = false, toolTip = "Makes certain green and purple colors more vibrant." },
                new ButtonInfo { buttonText = "Change PC Menu Background", method =() => Settings.ChangePCUI(), enableMethod =() => Settings.ChangePCUI(), disableMethod =() => Settings.ChangePCUI(false), incremental = true, isTogglable = false, toolTip = "Changes the background of the PC ui."},
                new ButtonInfo { buttonText = "Change Joystick Menu Position", method =() => Settings.ChangeJoystickMenuPosition(), enableMethod =() => Settings.ChangeJoystickMenuPosition(), disableMethod =() => Settings.ChangeJoystickMenuPosition(false), incremental = true, isTogglable = false, toolTip = "Changes the position of the joystick menu."},
                new ButtonInfo { buttonText = "Change Notification Time", overlapText = "Change Notification Time <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeNotificationTime(), enableMethod =() => Settings.ChangeNotificationTime(), disableMethod =() => Settings.ChangeNotificationTime(false), incremental = true, isTogglable = false, toolTip = "Changes the time before a notification is removed."},
                new ButtonInfo { buttonText = "Change Notification Sound", overlapText = "Change Notification Sound <color=grey>[</color><color=green>None</color><color=grey>]</color>", method =() => Settings.ChangeNotificationSound(true, true), enableMethod =() => Settings.ChangeNotificationSound(true, true), disableMethod =() => Settings.ChangeNotificationSound(false, true), incremental = true, isTogglable = false, toolTip = "Changes the sound that plays when receiving a notification."},
                new ButtonInfo { buttonText = "Notification Sound on Error", enableMethod =() => NotificationManager.soundOnError = true, disableMethod =() => NotificationManager.soundOnError = false, toolTip = "Plays your target notification sound when an error happens."},
                new ButtonInfo { buttonText = "Change Narration Voice", overlapText = "Change Narration Voice <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeNarrationVoice(), enableMethod =() => Settings.ChangeNarrationVoice(), disableMethod =() => Settings.ChangeNarrationVoice(false), incremental = true, isTogglable = false, toolTip = "Changes the voice of the narrator."},
                new ButtonInfo { buttonText = "Change Pointer Position", method =() => Settings.ChangePointerPosition(), enableMethod =() => Settings.ChangePointerPosition(), disableMethod =() => Settings.ChangePointerPosition(false), incremental = true, isTogglable = false, toolTip = "Changes the position of the pointer."},

                new ButtonInfo { buttonText = "Swap GUI Colors", toolTip = "Swaps the GUI's colors to the enabled color, for darker themes."},
                new ButtonInfo { buttonText = "Swap Button Colors", enableMethod =() => swapButtonColors = true, disableMethod =() => swapButtonColors = false, toolTip = "Swaps the colors of the page buttons, disconnect button, search button, and return button to be the opposite color."},
                new ButtonInfo { buttonText = "Swap Ghostview Colors", toolTip = "Swaps the ghostview's colors to the enabled color, for darker themes."},

                new ButtonInfo { buttonText = "Change Gun Line Quality", overlapText = "Change Gun Line Quality <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Settings.ChangeGunLineQuality(), enableMethod =() => Settings.ChangeGunLineQuality(), disableMethod =() => Settings.ChangeGunLineQuality(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of points on your gun."},
                new ButtonInfo { buttonText = "Change Gun Variation", overlapText = "Change Gun Variation <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeGunVariation(), enableMethod =() => Settings.ChangeGunVariation(), disableMethod =() => Settings.ChangeGunVariation(false), incremental = true, isTogglable = false, toolTip = "Changes the look of the gun."},
                new ButtonInfo { buttonText = "Change Gun Direction", overlapText = "Change Gun Direction <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeGunDirection(), enableMethod =() => Settings.ChangeGunDirection(), disableMethod =() => Settings.ChangeGunDirection(false), incremental = true, isTogglable = false, toolTip = "Changes the direction of the gun."},

                new ButtonInfo { buttonText = "Gun Sounds", enableMethod =() => GunSounds = true, disableMethod =() => GunSounds = false, toolTip = "Plays laser sounds when interacting with the gun."},
                new ButtonInfo { buttonText = "Gun Vibrations", enableMethod =() => GunVibrations = true, disableMethod =() => GunVibrations = false, toolTip = "Vibrates your controller when interacting with the gun."},
                new ButtonInfo { buttonText = "Gun Particles", enableMethod =() => GunParticles = true, disableMethod =() => GunParticles = false, toolTip = "Gives the gun particles when you shoot it."},
                new ButtonInfo { buttonText = "Swap Gun Hand", enableMethod =() => SwapGunHand = true, disableMethod =() => SwapGunHand = false, toolTip = "Swaps the hand gun mods work with."},
                new ButtonInfo { buttonText = "Gripless Guns", enableMethod =() => GriplessGuns = true, disableMethod =() => GriplessGuns = false, toolTip = "Forces your grip to be held for guns."},
                new ButtonInfo { buttonText = "Triggerless Guns", enableMethod =() => TriggerlessGuns = true, disableMethod =() => TriggerlessGuns = false, toolTip = "Forces your trigger to be held for guns."},
                new ButtonInfo { buttonText = "Hard Gun Lock", enableMethod =() => HardGunLocks = true, disableMethod =() => HardGunLocks = false, toolTip = "Locks the guns even when letting go of grip until you press <color=green>B</color>."},
                new ButtonInfo { buttonText = "Small Gun Pointer", enableMethod =() => smallGunPointer = true, disableMethod =() => smallGunPointer = false, toolTip = "Makes the ball at the end of every gun mod smaller."},
                new ButtonInfo { buttonText = "Smooth Gun Pointer", enableMethod =() => SmoothGunPointer = true, disableMethod =() => SmoothGunPointer = false, toolTip = "Makes the ball at the end of every gun mod smoother."},
                new ButtonInfo { buttonText = "Disable Gun Pointer", enableMethod =() => disableGunPointer = true, disableMethod =() => disableGunPointer = false, toolTip = "Disables the ball at the end of every gun mod."},
                new ButtonInfo { buttonText = "Disable Gun Line", enableMethod =() => disableGunLine = true, disableMethod =() => disableGunLine = false, toolTip = "Disables the gun from your hand to the end of every gun mod."},

                new ButtonInfo { buttonText = "Checkbox Buttons", enableMethod =() => checkMode = true, disableMethod =() => checkMode = false, toolTip = "Turns the buttons into checkboxes."},
                new ButtonInfo { buttonText = "Change Button Sound", overlapText = "Change Button Sound <color=grey>[</color><color=green>Wood</color><color=grey>]</color>", method =() => Settings.ChangeButtonSound(true, true), enableMethod =() => Settings.ChangeButtonSound(true, true), disableMethod =() => Settings.ChangeButtonSound(false, true), incremental = true, isTogglable = false, toolTip = "Changes the button click sound."},
                new ButtonInfo { buttonText = "Change Button Volume", overlapText = "Change Button Volume <color=grey>[</color><color=green>4</color><color=grey>]</color>", method =() => Settings.ChangeButtonVolume(true, true), enableMethod =() => Settings.ChangeButtonVolume(true, true), disableMethod =() => Settings.ChangeButtonVolume(false, true), incremental = true, isTogglable = false, toolTip = "Changes the volume of the buttons."},
                new ButtonInfo { buttonText = "Serversided Button Sounds", enableMethod =() => serversidedButtonSounds = true, disableMethod =() => serversidedButtonSounds = false, toolTip = "Lets everyone in the the room hear the buttons."},
                new ButtonInfo { buttonText = "Disable Button Vibration", enableMethod =() => doButtonsVibrate = false, disableMethod =() => doButtonsVibrate = true, toolTip = "Disables the slight vibration that happens when you click a button."},

                new ButtonInfo { buttonText = "Clear Notifications on Disconnect", enableMethod =() => clearNotificationsOnDisconnect = true, disableMethod =() => clearNotificationsOnDisconnect = false, toolTip = "Clears all notifications on disconnect."},
                new ButtonInfo { buttonText = "Hide Notifications on Camera", overlapText = "Streamer Mode Notifications", toolTip = "Makes notifications only render in VR."},
                new ButtonInfo { buttonText = "Stack Notifications", enableMethod =() => stackNotifications = true, disableMethod =() => stackNotifications = false, toolTip = "Stacks repeated notifications into one notification."},
                new ButtonInfo { buttonText = "Narrate Notifications", enableMethod =() => NotificationManager.narrateNotifications = true, disableMethod =() => NotificationManager.narrateNotifications = false, toolTip = "Narrates all notifications with text to speech."},
                new ButtonInfo { buttonText = "No Prefix Narration", enableMethod =() => NotificationManager.noPrefix = true, disableMethod =() => NotificationManager.noPrefix = false, toolTip = "Stops the prefix on notifications from narrating itself."},
                new ButtonInfo { buttonText = "Hide Notification Brackets", enableMethod =() => hideBrackets = true, disableMethod =() => hideBrackets = false, toolTip = "Hides brackets on all notifications."},

                new ButtonInfo { buttonText = "Conduct Notifications", enableMethod =() => { GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "II'S STUPID MENU"; GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true; }, method =() => GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().text = NotificationManager.notificationText.text, toolTip = "Shows notifications on the code of conduct instead."},
                new ButtonInfo { buttonText = "Disable Notification Rich Text", enableMethod =() => NotificationManager.noRichText = true, disableMethod =() => NotificationManager.noRichText = false, toolTip = "Removes rich text from notifications."},

                new ButtonInfo { buttonText = "Disable Notifications", enableMethod =() => disableNotifications = true, disableMethod =() => disableNotifications = false, toolTip = "Disables all notifications."},
                new ButtonInfo { buttonText = "Disable Master Client Notifications", enableMethod =() => disableMasterClientNotifications = true, disableMethod =() => disableMasterClientNotifications = false, toolTip = "Disables all notifications regarding master client."},
                new ButtonInfo { buttonText = "Disable Room Notifications", enableMethod =() => disableRoomNotifications = true, disableMethod =() => disableRoomNotifications = false, toolTip = "Disables all notifications regarding the room."},
                new ButtonInfo { buttonText = "Disable Player Notifications", enableMethod =() => disablePlayerNotifications = true, disableMethod =() => disablePlayerNotifications = false, toolTip = "Disables all notifications regarding players."},
                new ButtonInfo { buttonText = "Disable Enabled GUI", overlapText = "Disable Arraylist GUI", enableMethod =() => showEnabledModsVR = false, disableMethod =() => showEnabledModsVR = true, toolTip = "Disables the GUI that shows the enabled mods."},
                new ButtonInfo { buttonText = "Disable Incremental Buttons", enableMethod =() => incrementalButtons = false, disableMethod =() => incrementalButtons = true, toolTip = "Disables the buttons with the increment and decrement buttons next to it."},
                new ButtonInfo { buttonText = "Disable Disconnect Button", enableMethod =() => disableDisconnectButton = true, disableMethod =() => disableDisconnectButton = false, toolTip = "Disables the disconnect button at the top of the menu."},
                new ButtonInfo { buttonText = "Disable Menu Title", enableMethod =() => { buttonOffset = pageButtonType == 1 ? 1 : -1; hidetitle = true; }, method =() => { buttonOffset = pageButtonType == 1 ? 1 : -1; hidetitle = true; }, disableMethod =() => { buttonOffset = pageButtonType == 1 ? 2 : 0; hidetitle = false; }, toolTip = "Hides the menu title, allowing for more buttons per page."},
                new ButtonInfo { buttonText = "Disable Search Button", enableMethod =() => disableSearchButton = true, disableMethod =() => disableSearchButton = false, toolTip = "Disables the search button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Return Button", enableMethod =() => disableReturnButton = true, disableMethod =() => disableReturnButton = false, toolTip = "Disables the return button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Watermark Animation", enableMethod =() => rockWatermark = false, disableMethod =() => rockWatermark = true, toolTip = "Stops the watermark on the UI and the back of the menu from rocking back and forth."},
                new ButtonInfo { buttonText = "Disable Page Buttons", enableMethod = Settings.DisablePageButtons, disableMethod =() => disablePageButtons = false, toolTip = "Disables the page buttons. Recommended with Joystick Menu."},
                new ButtonInfo { buttonText = "Disable Page Number", enableMethod =() => noPageNumber = true, disableMethod =() => noPageNumber = false, toolTip = "Disables the current page number in the title text."},
                new ButtonInfo { buttonText = "Disable FPS Counter", enableMethod =() => disableFpsCounter = true, disableMethod =() => disableFpsCounter = false, toolTip = "Disables the FPS counter."},
                new ButtonInfo { buttonText = "Disable Drop Menu", enableMethod =() => dropOnRemove = false, disableMethod =() => dropOnRemove = true, toolTip = "Makes the menu despawn instead of falling."},
                new ButtonInfo { buttonText = "Disable Board Colors", overlapText = "Disable Custom Boards", enableMethod =() => CustomBoardManager.CustomBoardsEnabled = false, disableMethod =() => CustomBoardManager.CustomBoardsEnabled = true, toolTip = "Disables the board colors to look legitimate on screen share."},
                new ButtonInfo { buttonText = "Disable Custom Text Colors", enableMethod =() => CustomBoardManager.CustomBoardTextEnabled = false, disableMethod =() => CustomBoardManager.CustomBoardTextEnabled = true, toolTip = "Disables the text colors on the boards to make them match their original theme."},
                new ButtonInfo { buttonText = "Custom Board Fonts", enableMethod =() => CustomBoardManager.CustomBoardFonts = true, disableMethod =() => CustomBoardManager.CustomBoardFonts = false, toolTip = "Applies the menu's font to the boards."},

                new ButtonInfo { buttonText = "Disable Keyboard Delay", toolTip = "Disables the delay between pressing keys on the keyboard."},
                new ButtonInfo { buttonText = "Disable PC Keyboard Sounds", enableMethod =() => pcKeyboardSounds = false, disableMethod =() => pcKeyboardSounds = true, toolTip = "Disables the sound for pressing keys on PC."},

                new ButtonInfo { buttonText = "Info Hide ID", enableMethod =() => Settings.hideId = true, disableMethod =() => Settings.hideId = false, toolTip = "Hides your ID in the information page."},
                new ButtonInfo { buttonText = "Conduct Info", enableMethod =() => { GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "DEBUG INFO"; GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true; }, method = Visuals.ConductDebug, toolTip = "Shows debug information on the code of conduct."},
                new ButtonInfo { buttonText = "Info Button", enableMethod =() => enableDebugButton = true, disableMethod =() => enableDebugButton = false, toolTip = "Shows an information button at the bottom of the menu."},

                new ButtonInfo { buttonText = "Hide Text on Camera", enableMethod =() => hideTextOnCamera = true, disableMethod =() => hideTextOnCamera = false, overlapText = "Streamer Mode Menu Text", toolTip = "Makes the menu's text only render on VR."},
                new ButtonInfo { buttonText = "Hide Pointer", enableMethod =() => hidePointer = true, disableMethod =() => hidePointer = false, toolTip = "Hides the pointer above your hand."},
                new ButtonInfo { buttonText = "Hide Settings", enableMethod =() => hideSettings = true, disableMethod =() => hideSettings = false, toolTip = "Hides all settings from the Enabled Mods tab, and all arraylists."},
                new ButtonInfo { buttonText = "Hide Macros", enableMethod =() => hideMacros = true, disableMethod =() => hideMacros = false, toolTip = "Hides all macros from the Enabled Mods tab." },

                new ButtonInfo { buttonText = "Advanced Arraylist", enableMethod =() => advancedArraylist = true, disableMethod =() => advancedArraylist = false, toolTip = "Updates the FPS Counter less, making it easier to read."},
                new ButtonInfo { buttonText = "Flip Arraylist", enableMethod =() => flipArraylist = true, disableMethod =() => flipArraylist = false, toolTip = "Flips the arraylist at the top of the screen."},

                new ButtonInfo { buttonText = "Slow FPS Counter", enableMethod =() => fpsCountTimed = true, disableMethod =() => fpsCountTimed = false, toolTip = "Updates the FPS Counter less, making it easier to read."},
                new ButtonInfo { buttonText = "Average FPS Counter", enableMethod =() => fpsCountAverage = true, disableMethod =() => fpsCountAverage = false, toolTip = "Smooths out the FPS Counter, making it easier to read."},
                new ButtonInfo { buttonText = "Frametime Counter", enableMethod =() => ftCount = true, disableMethod =() => ftCount = false, toolTip = "Replace the FPS Counter to show frametime in ms instead."},

                new ButtonInfo { buttonText = "Disable Ghostview", enableMethod =() => disableGhostview = true, disableMethod =() => disableGhostview = false, toolTip = "Disables the transparent rig when you're in ghost."},
                new ButtonInfo { buttonText = "Legacy Ghostview", enableMethod =() => legacyGhostview = true, disableMethod =() => legacyGhostview = false, toolTip = "Reverts the transparent rig to the two balls when you're in ghost."},

                new ButtonInfo { buttonText = "No Global Search", enableMethod =() => nonGlobalSearch = true, disableMethod =() => nonGlobalSearch = false, toolTip = "Makes the search button only search for mods in the current subcategory, unless on the main page."},
                new ButtonInfo { buttonText = "Joystick Menu Search", enableMethod =() => joystickMenuSearching = true, disableMethod =() => joystickMenuSearching = false, toolTip = "Allows you to move your selected item down to the search button with joystick menu."},

                new ButtonInfo { buttonText = "Menu Presets", method =() => CurrentCategoryName = "Menu Presets", isTogglable = false, toolTip = "Opens the page of presets."},
                new ButtonInfo { buttonText = "Backup Preferences", enableMethod =() => backupPreferences = true, disableMethod =() => backupPreferences = false, toolTip = "Automatically saves a copy of your preferences every minute."},
                new ButtonInfo { buttonText = "Save Preferences", method = Settings.SavePreferences, isTogglable = false, toolTip = "Saves your preferences to a file."},
                new ButtonInfo { buttonText = "Load Preferences", method = Settings.LoadPreferences, isTogglable = false, toolTip = "Loads your preferences from a file."},
                new ButtonInfo { buttonText = "Disable Autosave", method =() => autoSaveDelay = Time.time + 1f, toolTip = "Disables the auto save mechanism."},
                new ButtonInfo { buttonText = "Panic", method = Settings.Panic, isTogglable = false, toolTip = "Disables every single active mod."},
            },

            new[] { // Room Settings [3]
                new ButtonInfo { buttonText = "Exit Room Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "crTime", overlapText = "Change Reconnect Time <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Settings.ChangeReconnectTime(), enableMethod =() => Settings.ChangeReconnectTime(), disableMethod =() => Settings.ChangeReconnectTime(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of time waited before attempting to reconnect again."},
            },

            new[] { // Movement Settings [4]
                new ButtonInfo { buttonText = "Exit Movement Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Platform Type", overlapText = "Change Platform Type <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePlatformType(), enableMethod =() => Movement.ChangePlatformType(), disableMethod =() => Movement.ChangePlatformType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of the platforms."},
                new ButtonInfo { buttonText = "Change Platform Shape", overlapText = "Change Platform Shape <color=grey>[</color><color=green>Sphere</color><color=grey>]</color>", method =() => Movement.ChangePlatformShape(), enableMethod =() => Movement.ChangePlatformShape(), disableMethod =() => Movement.ChangePlatformShape(false), incremental = true, isTogglable = false, toolTip = "Changes the shape of the platforms."},

                new ButtonInfo { buttonText = "Platform Gravity", toolTip = "Makes platforms fall instead of instantly deleting them."},
                new ButtonInfo { buttonText = "Platform Outlines", toolTip = "Makes platforms have outlines."},
                new ButtonInfo { buttonText = "Non-Sticky Platforms", toolTip = "Makes your platforms no longer sticky."},

                new ButtonInfo { buttonText = "Grip Noclip", toolTip = "Activates noclip with your <color=green>grip</color> instead."},
                new ButtonInfo { buttonText = "Constant Noclip", toolTip = "Keeps your noclip activated even when not holding any buttons."},
                new ButtonInfo { buttonText = "Left Hand Wall Walk", method =() => Movement.leftWallWalk = true, disableMethod =() => Movement.leftWallWalk = false, toolTip = "Swaps the wall walk mod to your left hand."},
                new ButtonInfo { buttonText = "Both Hands Wall Walk", method =() => Movement.bothWallWalk = true, disableMethod =() => Movement.bothWallWalk = false, toolTip = "Allows you to use wall walk with both of your hands."},

                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeFlySpeed(), enableMethod =() => Movement.ChangeFlySpeed(), disableMethod =() => Movement.ChangeFlySpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the fly mods, including iron man."},
                new ButtonInfo { buttonText = "Change Playspace Abuse Speed", overlapText = "Change Playspace Abuse Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePlayspaceAbuseSpeed(), enableMethod =() => Movement.ChangePlayspaceAbuseSpeed(), disableMethod =() => Movement.ChangePlayspaceAbuseSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the playspace abuse mod."},
                new ButtonInfo { buttonText = "Change Arm Length", overlapText = "Change Arm Length <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeArmLength(), enableMethod =() => Movement.ChangeArmLength(), disableMethod =() => Movement.ChangeArmLength(false), incremental = true, isTogglable = false, toolTip = "Changes the length of the long arm mods, not including iron man."},
                new ButtonInfo { buttonText = "Change Speed Boost Amount", overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeSpeedBoostAmount(), enableMethod =() => Movement.ChangeSpeedBoostAmount(), disableMethod =() => Movement.ChangeSpeedBoostAmount(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the speed boost mod."},
                new ButtonInfo { buttonText = "Change Wall Walk Strength", overlapText = "Change Wall Walk Strength <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeWallWalkStrength(), enableMethod =() => Movement.ChangeWallWalkStrength(), disableMethod =() => Movement.ChangeWallWalkStrength(false), incremental = true, isTogglable = false, toolTip = "Changes the strength of the wall walk mod."},
                new ButtonInfo { buttonText = "Change Pull Mod Power", overlapText = "Change Pull Mod Power <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePullModPower(), enableMethod =() => Movement.ChangePullModPower(), disableMethod =() => Movement.ChangePullModPower(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the pull mod."},
                new ButtonInfo { buttonText = "Change Prediction Amount", overlapText = "Change Prediction Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePredictionAmount(), enableMethod =() => Movement.ChangePredictionAmount(), disableMethod =() => Movement.ChangePredictionAmount(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the predictions."},
                new ButtonInfo { buttonText = "Change Timer Speed", overlapText = "Change Timer Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeTimerSpeed(), enableMethod =() => Movement.ChangeTimerSpeed(), disableMethod =() => Movement.ChangeTimerSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the timer mod."},
                new ButtonInfo { buttonText = "cdSpeed", overlapText = "Change Drive Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeDriveSpeed(), enableMethod =() => Movement.ChangeDriveSpeed(), disableMethod =() => Movement.ChangeDriveSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the drive mod."},
                new ButtonInfo { buttonText = "Factored Speed Boost", toolTip = "Factors your current speed into the speed boost, giving you a positive effect even if you're tagged."},
                new ButtonInfo { buttonText = "Disable Max Speed Modification", toolTip = "Makes your max speed not change, so you can't be detected of using a speed boost."},
                new ButtonInfo { buttonText = "Disable Size Changer Buttons", toolTip = "Disables the size changer's buttons, so hitting grip or trigger or whatever won't do anything."},
                new ButtonInfo { buttonText = "Pass World Scale Checks", enableMethod =() => Movement.passWorldScaleCheck = true, disableMethod =() => Movement.passWorldScaleCheck = false, toolTip = "Disables the Steam Long Arms mod when your hands are close to your head."},

                new ButtonInfo { buttonText = "Midpoint Macros", enableMethod =() => Movement.midpointMacros = true, disableMethod =() => Movement.midpointMacros = false, toolTip = "Allows for macros to be played from their middles." },
                new ButtonInfo { buttonText = "Direction Based Macros", enableMethod =() => Movement.directionBased = true, disableMethod =() => Movement.directionBased = false, toolTip = "Only plays macros if you match their velocity direction." },
                new ButtonInfo { buttonText = "Change Macro Playback Range", overlapText = "Change Macro Playback Range <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeMacroPlaybackRange(), enableMethod =() => Movement.ChangeMacroPlaybackRange(), disableMethod =() => Movement.ChangeMacroPlaybackRange(false), incremental = true, isTogglable = false, toolTip = "Changes the range where macros can play."},

                new ButtonInfo { buttonText = "Hand Oriented Strafe", toolTip = "Makes the strafe mods move you in the forward direction of your hand."},
                new ButtonInfo { buttonText = "Disable Stationary WASD Fly", toolTip = "Disables WASD Fly keeping you in-place when not moving."},

                new ButtonInfo { buttonText = "Networked Grapple Mods", method = Movement.NetworkedGrappleMods, toolTip = "Makes the spider man and grappling hook mods networked, showing the line for everyone. This requires a balloon."},

                new ButtonInfo { buttonText = "Non-Togglable Ghost", toolTip = "Makes the ghost mod only activate when holding down the button."},
                new ButtonInfo { buttonText = "Non-Togglable Invisible", toolTip = "Makes the invisible mod only activate when holding down the button"},

                new ButtonInfo { buttonText = "Splash Intercourse", toolTip = "Splashes water when \"impacting\" another player with the intercourse gun."},
                new ButtonInfo { buttonText = "Reverse Intercourse", toolTip = "Turns you into the receiver when using the intercourse gun."},

                new ButtonInfo { buttonText = "Elevated Sticky Drive", toolTip = "Makes you float higher in the air whenever you use Sticky Drive."},
                new ButtonInfo { buttonText = "High Quality Portals", toolTip = "Makes the view through the portals higher quality." }
            },

            new[] { // Projectile Settings [5]
                new ButtonInfo { buttonText = "Exit Projectile Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Projectile", overlapText = "Change Projectile <color=grey>[</color><color=green>Snowball</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectile(), enableMethod =() => Projectiles.ChangeProjectile(), disableMethod =() => Projectiles.ChangeProjectile(false), incremental = true, isTogglable = false, toolTip = "Changes the projectile of the projectile mods." },
                new ButtonInfo { buttonText = "Change Growing Projectile", overlapText = "Change Growing Projectile <color=grey>[</color><color=green>Growing Snowball</color><color=grey>]</color>", method =() => Projectiles.ChangeGrowingProjectile(), enableMethod =() => Projectiles.ChangeGrowingProjectile(), disableMethod =() => Projectiles.ChangeGrowingProjectile(false), incremental = true, isTogglable = false, toolTip = "Changes the projectile of the snowball mods." },
                new ButtonInfo { buttonText = "Random Projectile", toolTip = "Makes the projectiles random." },
                new ButtonInfo { buttonText = "Random Direction", toolTip = "Makes the projectiles go everywhere." },
                new ButtonInfo { buttonText = "Random Color", toolTip = "Makes the projectiles random colors." },

                new ButtonInfo { buttonText = "Change Shoot Speed", overlapText = "Change Shoot Speed <color=grey>[</color><color=green>Medium</color><color=grey>]</color>", method =() => Projectiles.ChangeShootSpeed(), enableMethod =() => Projectiles.ChangeShootSpeed(), disableMethod =() => Projectiles.ChangeShootSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of shooting projectiles." },
                new ButtonInfo { buttonText = "Shoot Projectiles", toolTip = "Shoots projectiles like a gun." },

                new ButtonInfo { buttonText = "Include Hand Velocity", toolTip = "Adds the hand velocity to the projectile velocity." },

                new ButtonInfo { buttonText = "Above Players", toolTip = "Makes projectiles go above players." },
                new ButtonInfo { buttonText = "Rain Projectiles", toolTip = "Makes projectiles fall around you like rain." },

                new ButtonInfo { buttonText = "Projectile Aura", overlapText = "Orbit Projectiles", toolTip = "Makes the projectiles orbit around your head." },
                new ButtonInfo { buttonText = "True Projectile Aura", overlapText = "Projectile Aura", toolTip = "Makes the projectiles random around you." },
                new ButtonInfo { buttonText = "Projectile Fountain", toolTip = "Makes projectiles spurt out of your head, like a fountain." },

                new ButtonInfo { buttonText = "Rainbow Projectiles", toolTip = "Makes projectiles be rainbow (real RGB)." },
                new ButtonInfo { buttonText = "Hard Rainbow Projectiles", toolTip = "Makes projectiles be rainbow but ye rainbow tis very harsh (real RGB)." },

                new ButtonInfo { buttonText = "RedProj", overlapText = "Red <color=grey>[</color><color=green>10</color><color=grey>]</color>", method =() => Projectiles.IncreaseRed(), enableMethod =() => Projectiles.IncreaseRed(), disableMethod =() => Projectiles.IncreaseRed(false), incremental = true, isTogglable = false, toolTip = "Makes projectiles more red." },
                new ButtonInfo { buttonText = "GreenProj", overlapText = "Green <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Projectiles.IncreaseGreen(), enableMethod =() => Projectiles.IncreaseGreen(), disableMethod =() => Projectiles.IncreaseGreen(false), incremental = true, isTogglable = false, toolTip = "Makes projectiles more green." },
                new ButtonInfo { buttonText = "BlueProj", overlapText = "Blue <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Projectiles.IncreaseBlue(), enableMethod =() => Projectiles.IncreaseBlue(), disableMethod =() => Projectiles.IncreaseBlue(false), incremental = true, isTogglable = false, toolTip = "Makes projectiles more blue." },

                new ButtonInfo { buttonText = "Custom Colored Projectiles", toolTip = "Makes the projectile color the custom color (buttons above)." },
                new ButtonInfo { buttonText = "Client Sided Projectiles", toolTip = "Makes projectiles only appear for you." },

                new ButtonInfo { buttonText = "Override Projectile Index", method =() => IndexPatch.enabled = true, disableMethod =() => IndexPatch.enabled = false, toolTip = "Forces a specific projectile index on random projectiles." },
                new ButtonInfo { buttonText = "Change Projectile Index", overlapText = "Change Projectile Index <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectileIndex(), enableMethod =() => Projectiles.ChangeProjectileIndex(), disableMethod =() => Projectiles.ChangeProjectileIndex(false), incremental = true, isTogglable = false, toolTip = "Changes the targetted projectile index on the \"Override Projectile Index\" mod." },

                new ButtonInfo { buttonText = "Change Projectile Delay", overlapText = "Change Projectile Delay <color=grey>[</color><color=green>0.1</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectileDelay(true, true), enableMethod =() => Projectiles.ChangeProjectileDelay(true, true), disableMethod =() => Projectiles.ChangeProjectileDelay(false, true), incremental = true, isTogglable = false, toolTip = "Gives the projectiles a delay before spawning another." },

                new ButtonInfo { buttonText = "Change Snowball Scale", overlapText = "Change Snowball Scale <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Overpowered.ChangeSnowballScale(), enableMethod =() => Overpowered.ChangeSnowballScale(), disableMethod =() => Overpowered.ChangeSnowballScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the snowballs." },
                new ButtonInfo { buttonText = "Change Snowball Multiplication Factor", overlapText = "Change Snowball Multiplication Factor <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Overpowered.ChangeSnowballMultiplicationFactor(), enableMethod =() => Overpowered.ChangeSnowballMultiplicationFactor(), disableMethod =() => Overpowered.ChangeSnowballMultiplicationFactor(false), incremental = true, isTogglable = false, toolTip = "Changes the multiplication factor of the snowballs." },

                new ButtonInfo { buttonText = "Disable Snowball Impact Effect", method = Overpowered.DisableSnowballImpactEffect, toolTip = "Disables the impact effect that people get when hit with snowballs."},
                new ButtonInfo { buttonText = "Invisible Snowballs", enableMethod =() => Overpowered.InvisibleSnowballs = true, disableMethod =() => Overpowered.InvisibleSnowballs = false, toolTip = "Makes the snowballs invisible."},
                new ButtonInfo { buttonText = "No Teleport Snowballs", enableMethod =() => Overpowered.NoTeleportSnowballs = true, disableMethod =() => Overpowered.NoTeleportSnowballs = false, toolTip = "Stops snowball mods from teleporting you." }
            },

            new[] { // Room Mods [6]
                new ButtonInfo { buttonText = "Exit Room Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the the room."},
                new ButtonInfo { buttonText = "Reconnect", method = Important.Reconnect, isTogglable = false, toolTip = "Reconnects you from and to the the room."},

                new ButtonInfo { buttonText = "Cancel Reconnect", method = Important.CancelReconnect, isTogglable = false, toolTip = "Cancels the reconnection loop."},

                new ButtonInfo { buttonText = "Join Last Room", method =() => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(lastRoom, JoinType.Solo), isTogglable = false, toolTip = "Joins the last room you left."},
                new ButtonInfo { buttonText = "Join Random", method = Important.JoinRandom, isTogglable = false, toolTip = "Joins a random public room." },

                new ButtonInfo { buttonText = "Create Public", method =() => Important.CreateRoom(Important.RandomRoomName(), true), isTogglable = false, toolTip = "Creates a public room."},

                new ButtonInfo { buttonText = "Fast Disconnect", method =() => SinglePlayerPatch.enabled = true, disableMethod =() =>  SinglePlayerPatch.enabled = false, toolTip = "Uses the fastest method of disconnecting possible."},
                new ButtonInfo { buttonText = "Join Menu Room", method =() => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom($"<$II_{PluginInfo.Version}>", JoinType.Solo), isTogglable = false, toolTip = "Connects you to a room that is exclusive to ii's <b>Stupid</b> Menu users." },

                new ButtonInfo { buttonText = "Bypass Join Room Type", enableMethod =() => JoinedRoomPatch.enabled = true, disableMethod =() => JoinedRoomPatch.enabled = false, toolTip = "Bypasses the immediate disconnection when trying to join a room that is in another map."},

                new ButtonInfo { buttonText = "Auto Join Room", method =() => PromptText("What room would you like to join?", () => Important.QueueRoom(keyboardInput), null, "Done", "Cancel"), isTogglable = false, toolTip = "Automatically attempts to connect to whatever room you desire every couple of seconds until connected." },

                new ButtonInfo { buttonText = "Auto Join Room \"RUN\"", method =() => Important.QueueRoom("RUN"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"RUN\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"HIDE\"", method =() => Important.QueueRoom("HIDE"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"HIDE\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY\"", method =() => Important.QueueRoom("DAISY"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY01\"", method =() => Important.QueueRoom("DAISY01"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY01\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY09\"", method =() => Important.QueueRoom("DAISY09"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY09\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"JV3U\"", method =() => Important.QueueRoom("JV3U"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"JV3U\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"J3VU\"", method =() => Important.QueueRoom("J3VU"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"J3VU\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"PBBV\"", method =() => Important.QueueRoom("PBBV"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"PBBV\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"BOT\"", method =() => Important.QueueRoom("BOT"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"BOT\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"LUCIO\"", method =() => Important.QueueRoom("LUCIO"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"LUCIO\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"VEN1\"", method =() => Important.QueueRoom("VEN1"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"VEN1\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"SREN16\"", method =() => Important.QueueRoom("SREN16"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"SREN16\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"SREN17\"", method =() => Important.QueueRoom("SREN17"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"SREN17\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"SREN18\"", method =() => Important.QueueRoom("SREN18"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"SREN18\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"HELP\"", method =() => Important.QueueRoom("HELP"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"HELP\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"STATUE\"", method =() => Important.QueueRoom("STATUE"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"STATUE\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"ECHO\"", method =() => Important.QueueRoom("ECHO"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"ECHO\" every couple of seconds until connected." },

                new ButtonInfo { buttonText = "Auto Join Room \"MOD\"", method =() => Important.QueueRoom("MOD"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"MOD\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"MODS\"", method =() => Important.QueueRoom("MODS"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"MODS\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"HACK\"", method =() => Important.QueueRoom("HACK"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"HACK\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"HACKER\"", method =() => Important.QueueRoom("HACKER"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"HACKER\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"LEMUR\"", method =() => Important.QueueRoom("LEMUR"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"LEMUR\" every couple of seconds until connected. Lemming joins this code very often." },
                new ButtonInfo { buttonText = "Auto Join Room \"JMANCURLY\"", method =() => Important.QueueRoom("JMANCURLY"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"JMANCURLY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"JMAN\"", method =() => Important.QueueRoom("JMAN"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"JMAN\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"ELLIOT\"", method =() => Important.QueueRoom("ELLIOT"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"ELLIOT\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"TYLERVR\"", method =() => Important.QueueRoom("TYLERVR"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"TYLERVR\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"JUANGTAG\"", method =() => Important.QueueRoom("JUANGTAG"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"JUANGTAG\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"GHOST\"", method =() => Important.QueueRoom("GHOST"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"GHOST\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"GULLIBLE\"", method =() => Important.QueueRoom("GULLIBLE"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"GULLIBLE\" every couple of seconds until connected." },

                new ButtonInfo { buttonText = "Auto Join Room \"GAY\"", method =() => Important.QueueRoom("GAY"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"GAY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"FURRY\"", method =() => Important.QueueRoom("FURRY"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"FURRY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"FORSAKEN\"", method =() => Important.QueueRoom("FORSAKEN"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"FORSAKEN\" every couple of seconds until connected." },
            },

            new[] { // Important Mods [7]
                new ButtonInfo { buttonText = "Exit Important Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Exit Gorilla Tag", aliases = new[] { "Quit Gorilla Tag", "Exit Game", "Quit Game", "Exit App", "Quit App" }, method = () => Prompt("Are you sure you want to exit Gorilla Tag?", Application.Quit), isTogglable = false, toolTip = "Closes Gorilla Tag." },
                new ButtonInfo { buttonText = "Restart Gorilla Tag", aliases = new[] { "Restart Game", "Restart App" }, method = () => Prompt("Are you sure you want to restart Gorilla Tag?", Important.RestartGame), isTogglable = false, toolTip = "Restarts Gorilla Tag." },
                new ButtonInfo { buttonText = "Open Gorilla Tag Folder", method = Important.OpenGorillaTagFolder, isTogglable = false, toolTip = "Opens the folder in which your game is located." },

                new ButtonInfo { buttonText = "Discord RPC", aliases = new[] { "Self Tracker" }, method = Important.DiscordRPC, disableMethod = Important.DisableDiscordRPC, toolTip = "Gives you a indicator on Discord that you are using ii's Stupid Menu."},
                new ButtonInfo { buttonText = "Media Integration", aliases = new[] { "Spotify" }, enableMethod = Important.EnsureIntegrationProgram, method = Important.MediaIntegration, disableMethod = Important.DisableMediaIntegration, toolTip = "Shows you what media you are watching/listening to in the top left. To switch media, open the menu and use your left joystick."},

                new ButtonInfo { buttonText = "Anti Hand Tap", enableMethod =() => HandTapPatch.enabled = true, disableMethod =() => HandTapPatch.enabled = false, toolTip = "Stops all hand tap sounds from being played."},
                new ButtonInfo { buttonText = "First Person Camera", enableMethod = Important.EnableFPC, postMethod = Important.MoveFPC, disableMethod = Important.DisableFPC, toolTip = "Makes your camera output what you see in VR."},
                new ButtonInfo { buttonText = "Force Enable Hands", method = Important.ForceEnableHands, toolTip = "Prevents your hands from disconnecting."},

                new ButtonInfo { buttonText = "Oculus Report Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method = Important.OculusReportMenu, toolTip = "Opens the Oculus report menu when holding <color=green>X</color>."},

                new ButtonInfo { buttonText = "Accept TOS", enableMethod =() => TOSPatches.enabled = true, method = Important.AcceptTOS, disableMethod =() => TOSPatches.enabled = false, toolTip = "Accepts the Terms of Service for you."},
                new ButtonInfo { buttonText = "Bypass K-ID Restrictions", overlapText = "Bypass k-ID Restrictions", method =() => PermissionPatch.enabled = true, disableMethod =() => PermissionPatch.enabled = false, toolTip = "Bypasses the permission restrictions held by k-ID for underage users."},
                new ButtonInfo { buttonText = "Redeem Shiny Rocks", aliases = new[] { "Free Shiny Rocks" }, method =() => CoroutineManager.instance.StartCoroutine(Important.RedeemShinyRocks()), isTogglable = false, toolTip = "Redeems the 500 Shiny Rocks k-ID gives you."},

                new ButtonInfo { buttonText = "Copy Player Position", method = Important.CopyPlayerPosition, isTogglable = false, toolTip = "Copies the current player position to the clipboard." },

                new ButtonInfo { buttonText = "Clear Notifications", method = NotificationManager.ClearAllNotifications, isTogglable = false, toolTip = "Clears your notifications. Good for when they get stuck."},

                new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => PhotonNetworkController.Instance.disableAFKKick = true, disableMethod =() => PhotonNetworkController.Instance.disableAFKKick = false, toolTip = "Doesn't let you get kicked for being AFK."},
                new ButtonInfo { buttonText = "Disable Network Triggers", enableMethod =() => NetworkTriggerPatch.enabled = true, disableMethod =() => NetworkTriggerPatch.enabled = false, toolTip = "Disables the network triggers, so you can change maps without disconnecting."},
                new ButtonInfo { buttonText = "Disable Map Triggers", enableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(false), disableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(true), toolTip = "Disables the map triggers, so you can change maps without loading them."},
                new ButtonInfo { buttonText = "Disable Quit Box", enableMethod =() => QuitBoxPatch.enabled = false, disableMethod =() => QuitBoxPatch.enabled = true, toolTip = "Disables the box under the map that closes your game."},
                new ButtonInfo { buttonText = "Physical Quit Box", enableMethod = Important.PhysicalQuitbox, disableMethod = Important.DisablePhysicalQuitbox, toolTip = "Makes the quitbox physical, letting you see and walk on it."},
                new ButtonInfo { buttonText = "Stump Quit Box", enableMethod =() => QuitBoxPatch.teleportToStump = true, disableMethod =() => QuitBoxPatch.teleportToStump = false, toolTip = "Disables the box under the map that closes your game."},
                new ButtonInfo { buttonText = "Block on Mute", method = Important.BlockOnMute, toolTip = "Disables any muted players' rig unless you need to see them."},

                new ButtonInfo { buttonText = "Steam Refund Timer", method =() => { if (playTime > 6000f) { NotificationManager.information["REFUND"] = "Refund soon"; } else { NotificationManager.information.Remove("REFUND"); } }, enableMethod = Important.CheckNewAcc, disableMethod =() => NotificationManager.information.Remove("REFUND"), toolTip = "Alerts you when you are nearby the steam refund time."},
                new ButtonInfo { buttonText = "Advanced Ban Message", enableMethod =() => ErrorPatches.enabled = true, disableMethod =() => ErrorPatches.enabled = false, toolTip = "Shows more information, such as remaining time and unban date, when banned."},

                new ButtonInfo { buttonText = "120 FPS", method =() => Important.CapFPS(120), toolTip = "Caps your FPS at 120 frames per second."},
                new ButtonInfo { buttonText = "90 FPS", method =() => Important.CapFPS(90), toolTip = "Caps your FPS at 90 frames per second."},
                new ButtonInfo { buttonText = "72 FPS", method =() => Important.CapFPS(72), toolTip = "Caps your FPS at 72 frames per second."},
                new ButtonInfo { buttonText = "60 FPS", method =() => Important.CapFPS(60), toolTip = "Caps your FPS at 60 frames per second."},
                new ButtonInfo { buttonText = "45 FPS", method =() => Important.CapFPS(45), toolTip = "Caps your FPS at 45 frames per second."},
                new ButtonInfo { buttonText = "30 FPS", method =() => Important.CapFPS(30), toolTip = "Caps your FPS at 30 frames per second."},
                new ButtonInfo { buttonText = "15 FPS", method =() => Important.CapFPS(15), toolTip = "Caps your FPS at 15 frames per second."},
                new ButtonInfo { buttonText = "Unlock FPS", method = Important.UncapFPS, disableMethod =() => Application.targetFrameRate = 144, toolTip = "Unlocks your FPS."},

                new ButtonInfo { buttonText = "PC Button Click", aliases = new[] { "PC Click" }, method = Important.PCButtonClick, disableMethod = Important.DisablePCButtonClick, toolTip = "Lets you click in-game buttons with your mouse."},
                new ButtonInfo { buttonText = "PC Controller Emulation", method = Important.PCControllerEmulation, toolTip = "Allows you to press buttons on your in-game controllers using your keyboard."},
                new ButtonInfo { buttonText = "Unlock Competitive Queue", method =() => GorillaComputer.instance.CompQueueUnlockButtonPress(), isTogglable = false, toolTip = "Permanently unlocks the competitive queue."},
                new ButtonInfo { buttonText = "Change Queue to Default", overlapText = "Change Queue <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => GorillaComputer.instance.currentQueue = "DEFAULT", isTogglable = false, toolTip = "Changes your queue to default."},
                new ButtonInfo { buttonText = "Change Queue to Minigames", overlapText = "Change Queue <color=grey>[</color><color=green>Minigames</color><color=grey>]</color>", method =() => GorillaComputer.instance.currentQueue = "MINIGAMES", isTogglable = false, toolTip = "Changes your queue to minigames."},
                new ButtonInfo { buttonText = "Change Queue to Competitive", overlapText = "Change Queue <color=grey>[</color><color=green>Competitive</color><color=grey>]</color>", method =() => GorillaComputer.instance.currentQueue = "COMPETITIVE", isTogglable = false, toolTip = "Changes your queue to competitive."},

                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Casual</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Casual"), isTogglable = false, toolTip = "Changes your target gamemode to casual."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Infection</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Infection"), isTogglable = false, toolTip = "Changes your target gamemode to infection."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Hunt</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("HuntDown"), isTogglable = false, toolTip = "Changes your target gamemode to hunt."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Paintbrawl</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Paintbrawl"), isTogglable = false, toolTip = "Changes your target gamemode to paintbrawl."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Ambush</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Ambush"), isTogglable = false, toolTip = "Changes your target gamemode to ambush."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Freeze Tag</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("FreezeTag"), isTogglable = false, toolTip = "Changes your target gamemode to freeze tag."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Ghost Tag</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Ghost"), isTogglable = false, toolTip = "Changes your target gamemode to ghost tag."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Custom</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Custom"), isTogglable = false, toolTip = "Changes your target gamemode to custom."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Guardian</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("Guardian"), isTogglable = false, toolTip = "Changes your target gamemode to guardian."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Prop Hunt</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("PropHunt"), isTogglable = false, toolTip = "Changes your target gamemode to prop hunt."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Super Infection</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("SuperInfect"), isTogglable = false, toolTip = "Changes your target gamemode to super infection."},
                new ButtonInfo { buttonText = "Change Target Gamemode <color=grey>[</color><color=green>Error</color><color=grey>]</color>", method =() => GorillaComputer.instance.SetGameModeWithoutButton("None"), isTogglable = false, toolTip = "Changes your target gamemode to none."},

                new ButtonInfo { buttonText = "Connect to US", aliases = new[] { "US VPN", "Connect to United States", "United States VPN" }, method =() => Important.ConnectToRegion("us"), toolTip = "Connects you to the United States servers."},
                new ButtonInfo { buttonText = "Connect to US West", aliases = new[] { "Connect to USW", "USW VPN", "US West VPN", "Connect to United States West", "United States West VPN" }, method =() => Important.ConnectToRegion("usw"), toolTip = "Connects you to the western United States servers."},
                new ButtonInfo { buttonText = "Connect to EU", aliases = new[] { "EU VPN", "Connect to Europe", "Europe VPN" }, method =() => Important.ConnectToRegion("eu"), toolTip = "Connects you to the Europe servers."},

                new ButtonInfo { buttonText = "Reauthenticate", method = MothershipAuthenticator.Instance.BeginLoginFlow, isTogglable = false, toolTip = "Restarts the login flow that happens at the beginning of the game."},
            },

            new[] { // Safety Mods [8]
                new ButtonInfo { buttonText = "Exit Safety Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Recommended Safety Mods", aliases = new[] { "Anti Ban" }, method = Safety.GeneralSafety, disableMethod = Safety.DisableGeneral, toolTip = "Has the effects of some good general safety mods while enabled." },

                new ButtonInfo { buttonText = "No Finger Movement", aliases = new[] { "Disable Fingers" }, method = Safety.NoFinger, toolTip = "Makes your fingers not move, so you can use wall walk without getting called out." },

                new ButtonInfo { buttonText = "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method = Safety.FakeOculusMenu, toolTip = "Imitates opening your Oculus menu when holding <color=green>X</color>."},
                new ButtonInfo { buttonText = "Fake Report Menu <color=grey>[</color><color=green>Y</color><color=grey>]</color>", method = Safety.FakeReportMenu, toolTip = "Imitates opening the report menu when holding <color=green>Y</color>."},
                new ButtonInfo { buttonText = "Fake Broken Controller <color=grey>[</color><color=green>X</color><color=grey>]</color>", enableMethod =() => GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<Collider>().enabled = false, method = Safety.FakeBrokenController, disableMethod =() => GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<Collider>().enabled = true, toolTip = "Makes you look like your left controller is broken, hold <color=green>X</color> to move your right hand with your left hand."},
                new ButtonInfo { buttonText = "Fake Power Off <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Safety.FakePowerOff, toolTip = "Imitates turning off your headset when holding down your <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Fake Valve Tracking <color=grey>[</color><color=green>J</color><color=grey>]</color>", enableMethod =() => TorsoPatch.VRRigLateUpdate += Safety.FakeValveTracking, disableMethod =() => TorsoPatch.VRRigLateUpdate -= Safety.FakeValveTracking, toolTip = "Imitates what happens when your headset disconnects on a Valve Index when holding your <color=green>right joystick</color>."},

                new ButtonInfo { buttonText = "Disable Gamemode Buttons", enableMethod =() => Safety.SetGamemodeButtonActive(false), disableMethod =() => Safety.SetGamemodeButtonActive(), toolTip = "Disables the gamemode buttons."},
                new ButtonInfo { buttonText = "Support Page Spoof", method = Safety.SpoofSupportPage, toolTip = "Makes the support page appear as if you are on Oculus."},

                new ButtonInfo { buttonText = "Flush RPCs", method = RPCProtection, isTogglable = false, toolTip = "Flushes all RPC calls, good after you stop spamming." },
                new ButtonInfo { buttonText = "Anti Crash", enableMethod =() => AntiCrashPatches.enabled = true, disableMethod =() => AntiCrashPatches.enabled = false, toolTip = "Prevents crashers from completely annihilating your computer."},
                new ButtonInfo { buttonText = "Anti Ban Crash", enableMethod =() => BanPatches.AntiBanCrash1.enabled = true, disableMethod =() => BanPatches.AntiBanCrash1.enabled = false, toolTip = "Prevents your game from crashing when you are banned."},
                new ButtonInfo { buttonText = "Anti Kick", enableMethod = Experimental.OnlySerializeNecessary, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Only networks the necessities to prevent getting kicked."},
                new ButtonInfo { buttonText = "Anti Name Ban", enableMethod =() => BanPatches.enabled = true, method = Safety.AntiNameBan, disableMethod =() => BanPatches.enabled = false, toolTip = "Prevents you from getting banned for setting your name to bad things."},
                new ButtonInfo { buttonText = "Anti Stump Kick", enableMethod =() => GroupPatch.enabled = true, disableMethod =() => GroupPatch.enabled = false, toolTip = "Stops people from group kicking you."},
                new ButtonInfo { buttonText = "Auto Clear Cache", method = Safety.AutoClearCache, toolTip = "Automatically clears your game's cache (garbage collector) every minute to prevent memory leaks."},
                new ButtonInfo { buttonText = "Anti Moderator", method = Safety.AntiModerator, toolTip = "When someone with the stick joins, you get disconnected and their player ID and room code gets saved to a file."},
                new ButtonInfo { buttonText = "Anti Content Creator", method = Safety.AntiContentCreator, toolTip = "When a content creator joins, you get disconnected and their player ID and room code gets saved to a file."},
                new ButtonInfo { buttonText = "Cosmetic Notifications", method = Safety.CosmeticNotifications, toolTip = "Sends you a notification if there is a Finger Painter, Illustrator, Administrator, Stick, Forest Guide, or Another Axiom Creator in your room."},
                new ButtonInfo { buttonText = "Steam Detector", method = Important.SteamDetector, toolTip = "Detects when a player in your room is on Steam."},

                new ButtonInfo { buttonText = "Bypass Automod", method = Safety.BypassAutomod, toolTip = "Attempts to bypass automod muting yourself and others."},
                new ButtonInfo { buttonText = "Bypass Mod Checkers", enableMethod =() => PropertiesPatches.enabled = true, method = Safety.BypassModCheckers, disableMethod =() => PropertiesPatches.enabled = false, toolTip = "Tells players using mod checkers that you have no mods."},
                new ButtonInfo { buttonText = "Bypass Cosmetic Check", method =() => RequestPatch.bypassCosmeticCheck = true, disableMethod =() => RequestPatch.bypassCosmeticCheck = false, toolTip = "Turns off the networking for any cosmetic mods, stopping people from seeing if you're using one."},
                new ButtonInfo { buttonText = "Anti Predictions", enableMethod = Safety.AntiPredictions, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Prevents people from checking if your predictions are too high."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", method = Safety.AntiReportDisconnect, toolTip = "Disconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Reconnect</color><color=grey>]</color>", method = Safety.AntiReportReconnect, toolTip = "Disconnects and reconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Join Random</color><color=grey>]</color>", method = Safety.AntiReportJoinRandom, toolTip = "Connects you to a random the room when anyone comes near your report button."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>", enableMethod = Safety.EnableAntiOculusReport, disableMethod = Safety.DisableAntiOculusReport, toolTip = "Disconnects you from the room when you get reported with the Oculus report menu."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Anti Cheat</color><color=grey>]</color>", enableMethod =() => AntiCheatPatches.SendReportPatch.AntiACReport = true, disableMethod =() => AntiCheatPatches.SendReportPatch.AntiACReport = false, toolTip = "Disconnects you from the room when you get reported by the anti cheat."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Notify</color><color=grey>]</color>", method = Safety.AntiReportNotify, toolTip = "Tells you when people come near your report button, but doesn't do anything."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Overlay</color><color=grey>]</color>", method = Safety.AntiReportOverlay, toolTip = "Shows you an overlay when people come near your report button, but doesn't do anything."},

                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>", enableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatSelf = true, disableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatSelf = false, toolTip = "Gives you a notification every time you have been reported by the anti cheat."},
                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>All</color><color=grey>]</color>", enableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatAll = true, disableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatAll = false, toolTip = "Gives you a notification every time anyone has been reported by the anti cheat."},

                new ButtonInfo { buttonText = "Change Identity", method = Safety.ChangeIdentity, isTogglable = false, toolTip = "Changes your name and color to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method = Safety.ChangeIdentityRegular, isTogglable = false, toolTip = "Changes your name and color to something a regular player would have."},
                new ButtonInfo { buttonText = "Change Identity <color=grey>[</color><color=green>Custom</color><color=grey>]</color>", method = Safety.ChangeIdentityCustom, isTogglable = false, toolTip = "Changes your name and color to whatever you desire."},

                new ButtonInfo { buttonText = "Change Identity on Disconnect", method =() => Safety.ChangeIdentityOnDisconnect(Safety.ChangeIdentity), toolTip = "When you leave, your name and color will be set to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeIdentityOnDisconnect(Safety.ChangeIdentityRegular), toolTip = "When you leave, your name and color will be set to something a regular player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Child</color><color=grey>]</color>", method =() => Safety.ChangeIdentityOnDisconnect(Safety.ChangeIdentityCustom), toolTip = "When you leave, your name and color will be set to whatever you desire."},

                new ButtonInfo { buttonText = "FPS Spoof", method = Safety.FPSSpoof, disableMethod =() => FPSPatch.enabled = false, toolTip = "Makes your FPS appear different for other players and the competitive bot."},
                new ButtonInfo { buttonText = "Ping Spoof", enableMethod = Safety.PingSpoof, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes your ping appear different for other players and the competitive bot."},
                new ButtonInfo { buttonText = "Name Spoof", method = Safety.NameSpoof, toolTip = "Changes your name on the leaderboard to something random, but not on your rig."},
                new ButtonInfo { buttonText = "Color Spoof", method = Safety.ColorSpoof, toolTip = "Makes your color appear different to every player."},

                new ButtonInfo { buttonText = "Unload Menu", method = () => Prompt("Are you sure you want unload the menu?", UnloadMenu), isTogglable = false, toolTip = "Unloads the menu from your game."},
                new ButtonInfo { buttonText = "Disable Anti Telemetry", enableMethod =() => TelemetryPatches.enabled = false, disableMethod =() => TelemetryPatches.enabled = true, toolTip = "Allows the game to send log data to Gorilla Tag's servers." }
            },

            new[] { // Movement Mods [9]
                new ButtonInfo { buttonText = "Exit Movement Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Platforms", overlapText = "Platforms <color=grey>[</color><color=green>G</color><color=grey>]</color>", postMethod =() => Movement.Platforms(), toolTip = "Spawns platforms on your hands when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Trigger Platforms", overlapText = "Trigger Platforms <color=grey>[</color><color=green>T</color><color=grey>]</color>", postMethod =() => Movement.Platforms(leftTrigger > 0.5f, rightTrigger > 0.5f), toolTip = "Spawns platforms on your hands when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Frozone", overlapText = "Frozone <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.Frozone, toolTip = "Spawns slippery blocks under your hands using <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Platform Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.PlatformSpam, toolTip = "Spawns legacy platforms rapidly at your hand for those who have networked platforms."},
                new ButtonInfo { buttonText = "Platform Gun", method = Movement.PlatformGun, toolTip = "Spawns legacy platforms rapidly wherever your hand desires for those who have networked platforms."},

                new ButtonInfo { buttonText = "Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", aliases = new[] { "Super Monke" }, method = Movement.Fly, toolTip = "Sends your character forwards when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Trigger Fly <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Movement.TriggerFly, toolTip = "Sends your character forwards when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Noclip Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.NoclipFly, toolTip = "Sends your character forwards and makes you go through objects when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Joystick Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Movement.JoystickFly, toolTip = "Sends your character in whatever direction you are pointing your <color=green>joystick</color> in."},
                new ButtonInfo { buttonText = "Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Movement.BarkFly, toolTip = "Acts like the fly that Bark has. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "Hand Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.HandFly, toolTip = "Sends your character in your hand's direction when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Fly Towards Gun", method = Movement.FlyTowardsGun, toolTip = "Sends your character towards whoever your hand desires."},
                new ButtonInfo { buttonText = "Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.SlingshotFly, toolTip = "Sends your character forwards, in a more elastic manner, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Zero Gravity Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.ZeroGravitySlingshotFly, toolTip = "Sends your character forwards, in a more elastic manner without gravity, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Slingshot Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Movement.VelocityBarkFly, toolTip = "Acts like the fly that Bark has, mixed with slingshot fly. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "WASD Fly", aliases = new[] { "PC Fly" }, enableMethod =() => { Movement.lastPosition = GorillaTagger.Instance.rigidbody.transform.position; }, postMethod = Movement.WASDFly, disableMethod =() => GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.Euler(0, 0, 0), toolTip = "Moves your rig with <color=green>WASD</color>."},

                new ButtonInfo { buttonText = "Dash <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.Dash, toolTip = "Flings your character forwards when pressing <color=green>A</color>."},
                new ButtonInfo { buttonText = "Reverse Velocity <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.ReverseVelocity, toolTip = "Reverses your current velocity when you press <color=green>A</color>."},
                new ButtonInfo { buttonText = "Bird Fly", method = Movement.BirdFly, toolTip = "Makes you fly like a bird when you flap your wings."},
                new ButtonInfo { buttonText = "Iron Man", aliases = new[] { "Iron Monke" }, method = Movement.IronMan, toolTip = "Turns you into iron man, rotate your hands around to change direction."},
                new ButtonInfo { buttonText = "Spider Man", aliases = new[] { "Spider Monke" }, method = Movement.SpiderMan, disableMethod = Movement.DisableSpiderMan, toolTip = "Turns you into spider man, use your <color=green>grips</color> to shoot webs."},
                new ButtonInfo { buttonText = "Grappling Hooks", method = Movement.GrapplingHooks, disableMethod = Movement.DisableSpiderMan, toolTip = "Gives you grappling hooks, use your <color=green>grips</color> to shoot them."},
                new ButtonInfo { buttonText = "Portal Gun", aliases = new[] { "Portals" }, method = Movement.PortalGun, disableMethod = Movement.DisablePortalGun, toolTip = "Gives you a gun to spawn portals that can be seen and walked through."},

                new ButtonInfo { buttonText = "Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", aliases = new[] { "Car Monke" }, method = Movement.Drive, toolTip = "Lets you drive around in your invisible car. Use the <color=green>joystick</color> to move."},
                new ButtonInfo { buttonText = "Hard Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", overlapText = "Sticky Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Movement.HardDrive, toolTip = "Similar to drive, but locks you to the ground."},

                new ButtonInfo { buttonText = "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Movement.Noclip, toolTip = "Makes you go through objects when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Up And Down", method = Movement.UpAndDown, toolTip = "Makes you go up when holding your <color=green>trigger</color>, and down when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Left And Right", method = Movement.LeftAndRight, toolTip = "Makes you go left when holding your <color=green>trigger</color>, and right when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Forwards And Backwards", method = Movement.ForwardsAndBackwards, toolTip = "Makes you go forwards when holding your <color=green>trigger</color>, and backwards when holding your <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Size Changer", method = Movement.SizeChanger, enableMethod = Movement.DisableSizeChanger, disableMethod = Movement.DisableSizeChanger, toolTip = "Increase your size by holding <color=green>trigger</color>, and decrease your size by holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Auto Walk <color=grey>[</color><color=green>J</color><color=grey>]</color>", aliases = new[] { "Ghost Walk", "Walk Simulator" }, method = Movement.AutoWalk, toolTip = "Makes your character automatically walk when using the <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Auto Funny Run <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.AutoFunnyRun, toolTip = "Makes your character automatically funny run when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Pinch Climb <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.AutoPinchClimb, toolTip = "Makes your character automatically pinch climb when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Elevator Climb <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.AutoElevatorClimb, toolTip = "Makes your character automatically elevator climb when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Branch <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.AutoBranch, toolTip = "Makes your character automatically branch when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Force Tag Freeze", method = Movement.ForceTagFreeze, disableMethod = Movement.NoTagFreeze, toolTip = "Forces tag freeze on your character."},
                new ButtonInfo { buttonText = "No Tag Freeze", method = Movement.NoTagFreeze, toolTip = "Disables tag freeze on your character."},
                new ButtonInfo { buttonText = "Feather Falling", method = Movement.FeatherFalling, toolTip = "Makes you fall like a feather."},
                new ButtonInfo { buttonText = "Low Gravity", aliases = new[] { "Moon Gravity" }, method = Movement.LowGravity, toolTip = "Makes gravity lower on your character."},
                new ButtonInfo { buttonText = "Zero Gravity", aliases = new[] { "No Gravity" }, method = Movement.ZeroGravity, toolTip = "Disables gravity on your character."},
                new ButtonInfo { buttonText = "High Gravity", aliases = new[] { "Mars Gravity" }, method = Movement.HighGravity, toolTip = "Makes gravity higher on your character."},
                new ButtonInfo { buttonText = "Reverse Gravity", aliases = new[] { "Flip Gravity" }, method = Movement.ReverseGravity, disableMethod = Movement.UnflipCharacter, toolTip = "Reverses gravity on your character."},

                new ButtonInfo { buttonText = "Rewind <color=grey>[</color><color=green>T</color><color=grey>]</color>", aliases = new[] { "Reverse" }, method = Movement.Rewind, disableMethod = Movement.ClearRewind, toolTip = "Brings you back in time when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Macros", method =() => CurrentCategoryName = "Macros", isTogglable = false, toolTip = "Opens a category to manage your macros."},

                new ButtonInfo { buttonText = "Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.WallWalk, toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Legitimate Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.LegitimateWallWalk, toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but less noticable."},
                new ButtonInfo { buttonText = "Spider Walk", method = Movement.SpiderWalk, disableMethod = Movement.UnflipCharacter, toolTip = "Makes your gravity and character towards any wall you touch. This may cause motion sickness."},

                new ButtonInfo { buttonText = "Teleport to Random", method = Movement.TeleportToRandom, isTogglable = false, toolTip = "Teleports you to a random player."},
                new ButtonInfo { buttonText = "Teleport to Map", method = Movement.EnterTeleportToMap, isTogglable = false, toolTip = "Teleports you to a map of your choosing."},
                new ButtonInfo { buttonText = "Teleport Gun", method = Movement.TeleportGun, toolTip = "Teleports to wherever your hand desires."},
                new ButtonInfo { buttonText = "Airstrike", method = Movement.Airstrike, toolTip = "Teleports to wherever your hand desires, except farther up, then launches you back down."},

                new ButtonInfo { buttonText = "Checkpoint <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.Checkpoint, disableMethod = Movement.DisableCheckpoint, toolTip = "Place a checkpoint with <color=green>grip</color> and teleport to it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Advanced Checkpoints <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.AdvancedCheckpoints, disableMethod = Movement.DisableAdvancedCheckpoints, toolTip = "Place checkpoints with <color=green>grip</color>, use your joystick to swap between checkpoints, and teleport to your selected checkpoint with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ender Pearl <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.EnderPearl, disableMethod = Movement.DestroyEnderPearl, toolTip = "Gives you a throwable ender pearl when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "C4 <color=grey>[</color><color=green>G</color><color=grey>]</color>", aliases = new[] { "Bomb" }, method = Movement.Bomb, disableMethod = Movement.DisableBomb, toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>."},

                new ButtonInfo { buttonText = "Punch Mod", method = Movement.PunchMod, toolTip = "Lets people punch you across the map."},
                new ButtonInfo { buttonText = "Telekinesis", method = Movement.Telekinesis, toolTip = "Lets people control you with nothing but the power of their finger."},
                new ButtonInfo { buttonText = "Safety Bubble", method = Movement.SafetyBubble, toolTip = "Moves you away from players if they get too close to you."},
                new ButtonInfo { buttonText = "Solid Players", aliases = new[] { "Solid Monke" }, method = Movement.SolidPlayers, disableMethod = Movement.DisableSolidPlayers, toolTip = "Lets you walk on top of other players."},
                new ButtonInfo { buttonText = "Pull Mod <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.PullMod, toolTip = "Pulls you more whenever you walk to simulate speed without modifying your velocity."},
                new ButtonInfo { buttonText = "Long Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", overlapText = "Playspace Abuse <color=grey>[</color><color=green>A</color><color=grey>]</color>", aliases = new[] { "Long Jump" }, method = Movement.PlayspaceAbuse, toolTip = "Makes you look like you're legitimately long jumping when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Velocity Long Arms", overlapText = "Predictions", aliases = new[] { "Velocity Long Arms" }, enableMethod = Movement.CreateVelocityTrackers, method = Movement.VelocityLongArms, disableMethod = Movement.DestroyVelocityTrackers, toolTip = "Moves your arms farther depending on how fast you move them."},

                new ButtonInfo { buttonText = "Timer", enableMethod =() => TimerPatch.enabled = true, method = Movement.Timer, disableMethod =() => { TimerPatch.enabled = false; Time.timeScale = 1f; GTPlayer.Instance.debugMovement = false;  }, toolTip = "Speeds up or slows down the time of your game."},

                new ButtonInfo { buttonText = "Speed Boost", method = Movement.SpeedBoost, toolTip = "Changes your speed to whatever you set it to."},
                new ButtonInfo { buttonText = "Grip Speed Boost", method =() => { if (rightGrab) { Movement.SpeedBoost(); } }, toolTip = "Changes your speed to whatever you set it to, if you're holding right grip."},
                new ButtonInfo { buttonText = "Dynamic Speed Boost", method = Movement.DynamicSpeedBoost, toolTip = "Dynamically changes your speed to whatever you set it to when tagged players get closer to you."},
                new ButtonInfo { buttonText = "Uncap Max Velocity", aliases = new[] { "Velmax" }, method =() => GTPlayer.Instance.maxJumpSpeed = float.MaxValue, toolTip = "Removes the velocity limit of walking."},
                new ButtonInfo { buttonText = "Always Max Velocity", method = Movement.AlwaysMaxVelocity, toolTip = "Always makes you go as fast as the velocity limit."},
                new ButtonInfo { buttonText = "Disable Velocity Cap", enableMethod = Movement.DisableVelocityCap, disableMethod =() => Movement.playspace.enabled = true, toolTip = "Lets you go as fast as you want without hitting the velocity limit."},

                new ButtonInfo { buttonText = "Funny Movement", overlapText = "Exponential Movement", aliases = new[] { "Funny Movement" }, method = Movement.FunMove, toolTip = "Multiplies your velocity every frame, making you exponential."},

                new ButtonInfo { buttonText = "Slip Slap", enableMethod = Movement.SlipSlap, disableMethod = Movement.DisableSlipSlap, toolTip = "Allows you to slip slap again."},
                new ButtonInfo { buttonText = "Slippery Hands", overlapText = "Slippery Surfaces", aliases = new[] { "Slippery Hands" }, enableMethod =() => SlidePatch.everythingSlippery = true, disableMethod =() => SlidePatch.everythingSlippery = false, toolTip = "Makes everything ice, as in extremely slippery."},
                new ButtonInfo { buttonText = "Grippy Hands", overlapText = "No Slippery Surfaces", aliases = new[] { "Grippy Hands" }, enableMethod =() => SlidePatch.everythingGrippy = true, disableMethod =() => SlidePatch.everythingGrippy = false, toolTip = "Disables any slipperiness of any surfaces."},
                new ButtonInfo { buttonText = "Slippery Surface Helper", aliases = new[] { "Slippery Wall Helper" }, enableMethod =() => SlidePatch.minimalSlip = true, disableMethod =() => SlidePatch.minimalSlip = false, toolTip = "Helps you stick to slippery walls more, but doesn't remove the slipperiness."},
                new ButtonInfo { buttonText = "Remove Forest Colliders", enableMethod = Movement.RemoveForestColliders, disableMethod = Movement.RestoreForestColliders, toolTip = "Removes the colliders on the roof and the entrance to tutorial inside Forest."},
                new ButtonInfo { buttonText = "Sticky Hands", method = Movement.StickyHands, disableMethod = Movement.DisableStickyHands, toolTip = "Makes your hands really sticky."},
                new ButtonInfo { buttonText = "Climby Hands", method = Movement.ClimbyHands, disableMethod = Movement.DisableClimbyHands, toolTip = "Lets you climb everything like a rope."},
                new ButtonInfo { buttonText = "Disable Hands", method =() => Movement.SetHandEnabled(false), disableMethod =() => Movement.SetHandEnabled(true), toolTip = "Disables your hand colliders."},
                new ButtonInfo { buttonText = "Disable Body Collider", method =() => GorillaTagger.Instance.bodyCollider.enabled = false, disableMethod =() => GorillaTagger.Instance.bodyCollider.enabled = true, toolTip = "Disables your body's collider."},
                new ButtonInfo { buttonText = "Disable Head Collider", method =() => GorillaTagger.Instance.headCollider.enabled = false, disableMethod =() => GorillaTagger.Instance.headCollider.enabled = true, toolTip = "Disables your head's collider."},

                new ButtonInfo { buttonText = "Slide Control", enableMethod = Movement.EnableSlideControl, disableMethod = Movement.DisableSlideControl, toolTip = "Lets you control yourself on ice perfectly."},
                new ButtonInfo { buttonText = "Weak Slide Control", enableMethod = Movement.EnableWeakSlideControl, disableMethod = Movement.DisableSlideControl, toolTip = "Lets you control yourself on ice a little more perfect than before."},

                new ButtonInfo { buttonText = "Throw Controllers", method = Movement.ThrowControllers, toolTip = "Lets you throw your controllers with <color=green>X</color> or <color=green>A</color>."},
                new ButtonInfo { buttonText = "Controller Flick", aliases = new[] { "DC Flick", "Disconnect Flick" }, enableMethod = Movement.EnableControllerFlick, method = Movement.ControllerFlick, disableMethod = Movement.DisableControllerFlick, toolTip = "Flicks your controllers in a similar way to disconnecting them with <color=green>X</color> or <color=green>A</color>."},

                new ButtonInfo { buttonText = "Uncap Arm Length", aliases = new[] { "Armmax" }, method =() => { GTPlayer.Instance.leftHand.maxArmLength = float.MaxValue; GTPlayer.Instance.rightHand.maxArmLength = float.MaxValue; }, disableMethod =() => { GTPlayer.Instance.leftHand.maxArmLength = 1; GTPlayer.Instance.rightHand.maxArmLength = 1; }, toolTip = "Removes the arm distance limit."},
                new ButtonInfo { buttonText = "Steam Long Arms", method = Movement.EnableSteamLongArms, disableMethod = Movement.DisableSteamLongArms, toolTip = "Gives you long arms similar to override world scale."},
                new ButtonInfo { buttonText = "Stick Long Arms", method = Movement.StickLongArms, toolTip = "Makes you look like you're using sticks."},
                new ButtonInfo { buttonText = "Multiplied Long Arms", method = Movement.MultipliedLongArms, toolTip = "Gives you a weird version of long arms."},
                new ButtonInfo { buttonText = "Vertical Long Arms", method = Movement.VerticalLongArms, toolTip = "Gives you a version of long arms to help you vertically."},
                new ButtonInfo { buttonText = "Horizontal Long Arms", method = Movement.HorizontalLongArms, toolTip = "Gives you a version of long arms to help you horizontally."},

                new ButtonInfo { buttonText = "Extenders <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Movement.Extenders, enableMethod =() => Movement.extendingTime = 0f, disableMethod = Movement.DisableSteamLongArms, toolTip = "Steam long arms, but it slowly disables when holding the right trigger."},

                new ButtonInfo { buttonText = "Flick Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.FlickJump, toolTip = "Makes your hand go down really fast when holding <color=green>A</color>."},

                new ButtonInfo { buttonText = "Bunny Hop", aliases = new[] { "Bhop" }, method = Movement.BunnyHop, toolTip = "Makes you automatically jump when on the ground."},
                new ButtonInfo { buttonText = "Strafe", method = Movement.Strafe, toolTip = "Makes you strafe when in the air."},
                new ButtonInfo { buttonText = "Dynamic Strafe", method = Movement.DynamicStrafe, toolTip = "Makes you dynamically strafe when in the air."},
                new ButtonInfo { buttonText = "Ground Helper <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.GroundHelper, toolTip = "Helps you run on ground when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Bouncy", enableMethod = Movement.PreBouncy, method = Movement.Bouncy, disableMethod = Movement.PostBouncy, toolTip = "Makes you really bouncy when on the ground."},
                new ButtonInfo { buttonText = "Solid Water", aliases = new[] { "Jesus" }, enableMethod = Movement.SolidWater, disableMethod = Movement.FixWater, toolTip = "Makes the water solid in the beach map." },
                new ButtonInfo { buttonText = "Disable Water", enableMethod = Movement.DisableWater, disableMethod = Movement.FixWater, toolTip = "Disables the water in the beach map." },
                new ButtonInfo { buttonText = "Air Swim", aliases = new[] { "Fish" }, method = Movement.AirSwim, disableMethod = Movement.DisableAirSwim, toolTip = "Puts you in a block of water, letting you swim in the air." },
                new ButtonInfo { buttonText = "Fast Swim", method =() => Movement.SetSwimSpeed(10f), disableMethod =() => Movement.SetSwimSpeed(), toolTip = "Lets you swim faster in water." },
                new ButtonInfo { buttonText = "Water Run Helper", overlapText = "Water Run", enableMethod =() => Movement.WaterRunHelper(true), disableMethod =() => Movement.WaterRunHelper(false), toolTip = "Adds back water running to the game." },
                new ButtonInfo { buttonText = "Disable Air", overlapText = "Disable Wind Barriers", aliases = new[] { "Disable Air" }, enableMethod =() => { ForcePatches.enabled = true; GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(false); GetObject("Environment Objects/LocalObjects_Prefab/ForestToHoverboard/TurnOnInForestAndHoverboard/ForestDome_CollisionOnly").SetActive(false); }, disableMethod =() => { ForcePatches.enabled = false; GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(true); GetObject("Environment Objects/LocalObjects_Prefab/ForestToHoverboard/TurnOnInForestAndHoverboard/ForestDome_CollisionOnly").SetActive(true); }, toolTip = "Disables the wind barriers in every map." },

                new ButtonInfo { buttonText = "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.Ghost, disableMethod = Movement.EnableRig, toolTip = "Keeps your rig still when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", method = Movement.Invisible, disableMethod = Movement.EnableRig, toolTip = "Makes you go invisible when holding <color=green>B</color>."},

                new ButtonInfo { buttonText = "Rig Gun", method = Movement.RigGun, toolTip = "Moves your rig to wherever your hand desires."},
                new ButtonInfo { buttonText = "Grab Rig <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.GrabRig, toolTip = "Lets you grab your rig when holding <color=green>grip</color>."},
                
                new ButtonInfo { buttonText = "Spin Head X", method =() => Fun.SpinHead("x"), disableMethod = Fun.FixHead, toolTip = "Spins your head on the X axis."},
                new ButtonInfo { buttonText = "Spin Head Y", method =() => Fun.SpinHead("y"), disableMethod = Fun.FixHead, toolTip = "Spins your head on the Y axis."},
                new ButtonInfo { buttonText = "Spin Head Z", method =() => Fun.SpinHead("z"), disableMethod = Fun.FixHead, toolTip = "Spins your head on the Z axis."},

                new ButtonInfo { buttonText = "Spaz Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", enableMethod = Movement.EnableSpazRig, method = Movement.SpazRig, disableMethod = Movement.DisableSpazRig, toolTip = "Makes every part of your rig spaz out a little bit when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Rig Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.SpazHands, toolTip = "Makes your rig's hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.SpazRealHands, toolTip = "Makes your hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Random Spaz Head Position", enableMethod = Movement.EnableSpazHead, method = Movement.RandomSpazHeadPosition, disableMethod = Movement.FixHeadPosition, toolTip = "Makes your head position spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Random Spaz Head", overlapText = "Random Spaz Head Rotation", method = Movement.RandomSpazHead, disableMethod = Fun.FixHead, toolTip = "Makes your head rotation spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Spaz Head Position", enableMethod = Movement.EnableSpazHead, method = Movement.SpazHeadPosition, disableMethod = Movement.FixHeadPosition, toolTip = "Makes your head position spaz out."},
                new ButtonInfo { buttonText = "Spaz Head", overlapText = "Spaz Head Rotation", method = Movement.SpazHead, disableMethod = Fun.FixHead, toolTip = "Makes your head rotation spaz out."},
                new ButtonInfo { buttonText = "Spaz Head X", method =() => Fun.SpazHead("x"), disableMethod = Fun.FixHead, toolTip = "Spaz your head on the X axis."},
                new ButtonInfo { buttonText = "Spaz Head Y", method =() => Fun.SpazHead("y"), disableMethod = Fun.FixHead, toolTip = "Spaz your head on the Y axis."},
                new ButtonInfo { buttonText = "Spaz Head Z", method =() => Fun.SpazHead("z"), disableMethod = Fun.FixHead, toolTip = "Spaz your head on the Z axis."},

                new ButtonInfo { buttonText = "Laggy Rig", method = Movement.LaggyRig, disableMethod = Movement.EnableRig, toolTip = "Makes your rig laggy."},
                new ButtonInfo { buttonText = "Smooth Rig", method =() => PhotonNetwork.SerializationRate = 30, disableMethod =() => PhotonNetwork.SerializationRate = 10, toolTip = "Makes your rig really smooth."},
                new ButtonInfo { buttonText = "Update Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.UpdateRig, disableMethod = Movement.EnableRig, toolTip = "Freezes your rig in place. Whenever you click <color=green>A</color>, your rig will update."},

                new ButtonInfo { buttonText = "Freeze Rig Limbs", method = Movement.FreezeRigLimbs, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Makes your hands and head freeze on your rig, but not your body."},
                new ButtonInfo { buttonText = "Freeze Rig Body", method = Movement.FreezeRigBody, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Makes your body freeze on your rig, but not your hands and head."},
                new ButtonInfo { buttonText = "Freeze Rig", method = Movement.FreezeRig, disableMethod =() => { VRRig.LocalRig.enabled = true; Movement.startPosition = null; }, toolTip = "Makes your body freeze on your rig, but not your hands and head."},
                
                new ButtonInfo { buttonText = "Paralyze Rig", method = Movement.ParalyzeRig, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Removes your arms from your rig."},
                new ButtonInfo { buttonText = "Chicken Rig", method = Movement.ChickenRig, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Makes your rig look like a chicken."},
                new ButtonInfo { buttonText = "Amputate Rig", method = Movement.AmputateRig, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Removes all of your limbs from your rig."},
                new ButtonInfo { buttonText = "Decapitate Rig", enableMethod =() => TorsoPatch.VRRigLateUpdate += Movement.DecapitateRigUpdate, disableMethod =() => TorsoPatch.VRRigLateUpdate -= Movement.DecapitateRigUpdate, toolTip = "Removes the head from your rig."},

                new ButtonInfo { buttonText = "Spin Rig Body", method =() => Movement.SetBodyPatch(true), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Makes your body spin around, but not your head."},
                new ButtonInfo { buttonText = "Spaz Rig Body", method =() => Movement.SetBodyPatch(true, 1), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Gives your body a seizure, randomizing its rotation."},
                new ButtonInfo { buttonText = "Reverse Rig Body", method =() => Movement.SetBodyPatch(true, 2), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Flips your body around backwards, but not your head."},
                new ButtonInfo { buttonText = "Rec Room Body", method = Movement.RecRoomBody, disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Makes your rig like how the Rec Room bodies are."},
                new ButtonInfo { buttonText = "Freeze Body Rotation <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Movement.FreezeBodyRotation, disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Freezes your body rotation in place, but not your head, when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Auto Dance <color=grey>[</color><color=green>A</color><color=grey>]</color>", aliases = new[] { "Emote" }, method = Movement.AutoDance, toolTip = "Makes you dance when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto Griddy <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.AutoGriddy, toolTip = "Makes you griddy when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", overlapText = "T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.AutoTPose, toolTip = "Makes you t pose when holding <color=green>A</color>. Good for fly trolling."},
                new ButtonInfo { buttonText = "Helicopter <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.Helicopter, toolTip = "Turns you into a helicopter when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Beyblade <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.Beyblade, toolTip = "Turns you into a beyblade when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Still Beyblade <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.StillBeyblade, disableMethod =() => Movement.stillBeybladeStartPos = Vector3.zero, toolTip = "Turns you into a beyblade when holding <color=green>A</color>. Doesn't move you."},
                new ButtonInfo { buttonText = "Spin Bot", enableMethod =() => TorsoPatch.VRRigLateUpdate += Movement.TorsoPatch_VRRigLateUpdate, method =() => ghostException = true, disableMethod =() => { TorsoPatch.VRRigLateUpdate -= Movement.TorsoPatch_VRRigLateUpdate; ghostException = false; }, toolTip = "Makes your body spin around, but not your head."},
                new ButtonInfo { buttonText = "Fan <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Movement.Fan, toolTip = "Turns you into a fan when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ghost Animations", method = Movement.GhostAnimations, disableMethod = Movement.DisableGhostAnimations, toolTip = "Makes you look like a ghost, making your movement snappy and slow."},
                new ButtonInfo { buttonText = "Minecraft Animations", method = Movement.MinecraftAnimations, disableMethod = Movement.EnableRig, toolTip = "Puts your hands down, and makes you walk when holding <color=green>A</color>. You can also point with <color=green>B</color>."},

                new ButtonInfo { buttonText = "Stare at Nearby", overlapText = "Stare At Player Nearby", enableMethod =() => TorsoPatch.VRRigLateUpdate += Movement.StareAtNearby, disableMethod =() => TorsoPatch.VRRigLateUpdate -= Movement.StareAtNearby, toolTip = "Makes you stare at the nearest player."},
                new ButtonInfo { buttonText = "Stare at Player Gun", method = Movement.StareAtGun, disableMethod =() => TorsoPatch.VRRigLateUpdate -= Movement.StareAtTarget, toolTip = "Makes you stare at whoever your hand desires."},
                new ButtonInfo { buttonText = "Stare at All Players", aliases = new[] { "Owl" }, enableMethod = Movement.StareAtAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes you stare at everyone in the room."},
                new ButtonInfo { buttonText = "Eye Contact", enableMethod =() => TorsoPatch.VRRigLateUpdate += Movement.EyeContact, disableMethod =() => TorsoPatch.VRRigLateUpdate -= Movement.EyeContact, toolTip = "Makes you stare at anyone who is looking at you."},
                new ButtonInfo { buttonText = "Floating Rig", enableMethod = Movement.EnableFloatingRig, method = Movement.FloatingRig, disableMethod = Movement.DisableFloatingRig, toolTip = "Makes your rig float."},

                new ButtonInfo { buttonText = "Bees", method = Movement.Bees, disableMethod = Movement.EnableRig, toolTip = "Makes your rig teleport to random players, imitating the bees ghost."},
                new ButtonInfo { buttonText = "Bees <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() =>{ if (rightGrab) {Movement.Bees(); } }, disableMethod = Movement.EnableRig, toolTip = "Makes your rig teleport to random players when holding <color=green>grip</color>, imitating the bees ghost."},

                new ButtonInfo { buttonText = "Piggyback Gun", aliases = new[] { "Ride Gun" }, method = Movement.PiggybackGun, toolTip = "Teleports you on top of whoever your hand desires repeatedly."},
                new ButtonInfo { buttonText = "Piggyback All", aliases = new[] { "Ride All" }, enableMethod = Movement.PiggybackAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes you appear on top of everyone in the room"},

                new ButtonInfo { buttonText = "Copy Movement Gun", method = Movement.CopyMovementGun, toolTip = "Makes your rig copy the movement of whoever your hand desires."},
                new ButtonInfo { buttonText = "Copy Movement All", enableMethod = Movement.CopyMovementAll, disableMethod =() => { SerializePatch.OverrideSerialization = null; Movement.followPositions.Clear(); }, toolTip = "Makes your rig copy the movement of every player in the room."},

                new ButtonInfo { buttonText = "Follow Player Gun", method = Movement.FollowPlayerGun, toolTip = "Flies your rig towards whoever your hand desires."},
                new ButtonInfo { buttonText = "Follow All Players", enableMethod = Movement.FollowAllPlayers, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Flies your rig towards everyone in the room."},

                new ButtonInfo { buttonText = "Orbit Player Gun", method = Movement.OrbitPlayerGun, toolTip = "Orbits your rig around whoever your hand desires."},
                new ButtonInfo { buttonText = "Orbit All Players", enableMethod = Movement.OrbitAllPlayers, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Orbits your rig around everyone in the room."},

                new ButtonInfo { buttonText = "Jumpscare Gun", method = Movement.JumpscareGun, toolTip = "Makes you jumpscare whoever your hand desires."},
                new ButtonInfo { buttonText = "Jumpscare All", enableMethod = Movement.JumpscareAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes you jumpscare everyone in the room."},

                new ButtonInfo { buttonText = "Annoy Player Gun", method = Movement.AnnoyPlayerGun, toolTip = "Spazzes your body around whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Annoy All Players", enableMethod = Movement.AnnoyAllPlayers, method =() => Sound.SoundSpam(337, true), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Spazzes your body around everyone in the room, with sounds."},

                new ButtonInfo { buttonText = "Intercourse Gun", aliases = new[] { "Sex Gun" }, method = Movement.IntercourseGun, toolTip = "Makes you thrust whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Intercourse All", aliases = new[] { "Sex All" }, enableMethod = Movement.IntercourseAll, method = Movement.IntercourseNoises, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes you thrust everyone in the room, with sounds."},

                new ButtonInfo { buttonText = "Head Gun", aliases = new[] { "Blowjob Gun" }, method = Movement.HeadGun, toolTip = "Makes you thrust whoever your hand desires, but lower, with sounds."},
                new ButtonInfo { buttonText = "Head All", aliases = new[] { "Blowjob All" }, enableMethod = Movement.HeadAll, method = Movement.IntercourseNoises, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes you thrust everyone in the room, but lower, with sounds."}
            },

            new[] { // Advantage Mods [10]
                new ButtonInfo { buttonText = "Exit Advantage Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Tag Self", method = Advantages.TagSelf, disableMethod = Movement.EnableRig, toolTip = "Attempts to tags yourself."},
                new ButtonInfo { buttonText = "Tag Gun", method = Advantages.TagGun, toolTip = "Tags whoever your hand desires."},
                new ButtonInfo { buttonText = "Tag All", method = Advantages.TagAll, disableMethod = Movement.EnableRig, toolTip = "Attempts to tag everyone in the the room."},

                new ButtonInfo { buttonText = "Tag Aura", aliases = new[] { "Tag Range" }, method = Advantages.TagAura, toolTip = "Moves your hand into nearby players when tagged."},
                new ButtonInfo { buttonText = "Grip Tag Aura <color=grey>[</color><color=green>G</color><color=grey>]</color>", aliases = new[] { "Grip Tag Range" }, method = Advantages.GripTagAura, toolTip = "Moves your hand into nearby players when tagged and when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Tag Aura Gun", method = Advantages.TagAuraGun, toolTip = "Gives a player tag aura."},
                new ButtonInfo { buttonText = "Tag Aura All", method = Advantages.TagAuraAll, toolTip = "Gives all players tag aura."},

                new ButtonInfo { buttonText = "Tag Reach", method = Advantages.TagReach, disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Makes your hand tag hitbox larger."},

                new ButtonInfo { buttonText = "Flick Tag Gun", method = Advantages.FlickTagGun, toolTip = "Moves your hand to wherever your hand desires in an attempt to tag whoever your hand desires."},

                new ButtonInfo { buttonText = "Tag Bot", method = Advantages.TagBot, disableMethod = Movement.EnableRig, toolTip = "Automatically tags yourself and everyone else on a loop, use <color=green>B</color> to turn it off."},

                new ButtonInfo { buttonText = "No Tag on Join", method = Advantages.NoTagOnJoin, disableMethod = Advantages.TagOnJoin, toolTip = "When you join a the room, you won't be tagged when you join."},
                new ButtonInfo { buttonText = "Untag Self", method = Advantages.UntagSelf, isTogglable = false, toolTip = "Removes you from the list of tagged players."},

                new ButtonInfo { buttonText = "Anti Tag", method = Advantages.AntiTag, disableMethod = Advantages.TagOnJoin, toolTip = "Removes you from the list of tagged players when tagged."},
                new ButtonInfo { buttonText = "Report Anti Tag", enableMethod = Advantages.ReportAntiTag, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Prevents you from getting tagged, and whoever tries to tag you will just get reported."},

                new ButtonInfo { buttonText = "No Tag Limit", aliases = new[] { "Tagmax" }, method =() => GorillaTagger.Instance.maxTagDistance = float.MaxValue, disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Removes the distance check when tagging players."},
                new ButtonInfo { buttonText = "Tag Lag Detector", method = Important.TagLagDetector, toolTip = "Detects when the master client is not currently allowing tag requests."},
                
                new ButtonInfo { buttonText = "Fake Lag", method = Movement.FakeLag, disableMethod =() => { SerializePatch.OverrideSerialization = null; PlayerSerializePatch.delay = null; }, toolTip = "Forces your ping to be high."},
                new ButtonInfo { buttonText = "Lag Range", method = Movement.LagRange, toolTip = "Dynamically changes how much your rig updates depending on how close you are to others."},
                new ButtonInfo { buttonText = "Blink", method = Movement.Blink, disableMethod = Movement.DisableBlink, toolTip = "Stops your client from sending and receiving player update packets."},

                new ButtonInfo { buttonText = "Paintbrawl Aimbot", overlapText = "Slingshot Aimbot", enableMethod =() => GetLaunchPatch.enabled = true, method = Fun.DebugSlingshotAimbot, disableMethod =() => GetLaunchPatch.enabled = false, toolTip = "Redirects your slingshot to the closest nearby players."},
                new ButtonInfo { buttonText = "Slingshot Helper", method = Fun.SlingshotHelper, toolTip = "Helps you grab the small paintball on your slingshot."},
                new ButtonInfo { buttonText = "Slingshot Trigger Bot", method = Fun.SlingshotTriggerBot, toolTip = "Releases the small paintball on your slingshot when hovering over another player."},

                new ButtonInfo { buttonText = "Paintbrawl Kill Self", method = Advantages.PaintbrawlKillSelf, toolTip = "Kills yourself in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Kill Gun", method = Advantages.PaintbrawlKillGun, toolTip = "Kills whoever your hand desires in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Kill All", method = Advantages.PaintbrawlKillAll, toolTip = "Kills everyone in the room in paintbrawl." }

            },

            new[] { // Visual Mods [11]
                new ButtonInfo { buttonText = "Exit Visual Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Morning Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(1), toolTip = "Sets your time of day to morning."},
                new ButtonInfo { buttonText = "Day Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(3), toolTip = "Sets your time of day to daytime."},
                new ButtonInfo { buttonText = "Evening Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(7), toolTip = "Sets your time of day to evening."},
                new ButtonInfo { buttonText = "Night Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(0), toolTip = "Sets your time of day to night."},
                new ButtonInfo { buttonText = "Fullbright", method =() => Visuals.SetFullbrightStatus(true), disableMethod =() => Visuals.SetFullbrightStatus(false), toolTip = "Disables the dynamic lighting in maps that use it."},

                new ButtonInfo { buttonText = "Remove Blindfold", method = Visuals.RemoveBlindfold, toolTip = "Disables the blindfold in the prop hunt map."},
                
                new ButtonInfo { buttonText = "Core ESP", method = Visuals.CoreESP, toolTip = "Puts dots on your screen at where all of the cores in the ghost reactor map are."},
                new ButtonInfo { buttonText = "Critter ESP", method = Visuals.CritterESP, toolTip = "Puts dots on your screen at where all of the critters in the critter map are."},
                new ButtonInfo { buttonText = "Creature ESP", method = Visuals.CreatureESP, toolTip = "Puts dots on your screen at where all of the creatures are in forest and caves."},
                new ButtonInfo { buttonText = "Enemy ESP", method = Visuals.EnemyESP, toolTip = "Puts dots on your screen at where all of the cores in the ghost reactor map are."},
                new ButtonInfo { buttonText = "Resource ESP", method = Visuals.ResourceESP, toolTip = "Puts dots on your screen at where all of the resources are in the Super Infection gamemode."},

                new ButtonInfo { buttonText = "Enable Snow", aliases = new[] { "Winter" }, enableMethod =() => Visuals.ToggleSnow(true), disableMethod =() => Visuals.ToggleSnow(false), toolTip = "Forcibly enables the snow."},
                new ButtonInfo { buttonText = "Rainy Weather", aliases = new[] { "Enable Rain" }, method =() => Visuals.WeatherChange(true), toolTip = "Forces the weather to rain."},
                new ButtonInfo { buttonText = "Clear Weather", aliases = new[] { "Disable Rain" }, method =() => Visuals.WeatherChange(false), toolTip = "Forces the weather to sunny skies all day."},
                new ButtonInfo { buttonText = "Disable Fog", method = Visuals.DisableFog, disableMethod = Visuals.ResetFog, toolTip = "Disables the fog."},
                new ButtonInfo { buttonText = "Enable Fog", method = Visuals.EnableFog, disableMethod = Visuals.ResetFog, toolTip = "Enables the fog."},

                new ButtonInfo { buttonText = "Disable Ambience", enableMethod = Visuals.DisableAmbience, disableMethod = Visuals.EnableAmbience, toolTip = "Disables all ambient effects."},

                new ButtonInfo { buttonText = "Custom Skybox Color", aliases = new[] { "Custom Sky Color" }, enableMethod = Visuals.DoCustomSkyboxColor, method = Visuals.CustomSkyboxColor, disableMethod = Visuals.UnCustomSkyboxColor, toolTip = "Changes the skybox color to match the menu."},
                new ButtonInfo { buttonText = "Draw Gun", method = Visuals.DrawGun, disableMethod = Visuals.DisableDrawGun, toolTip = "Lets you draw on whatever your hand desires." },
                new ButtonInfo { buttonText = "Gamesense Ring", aliases = new[] { "Fortnite Ring" }, enableMethod =() => HandTapPatch.OnHandTap += Visuals.OnHandTapGamesenseRing, method = Visuals.GamesenseRing, disableMethod = Visuals.DisableGamesenseRing, toolTip = "Shows the direction of where people walk around you." },

                new ButtonInfo { buttonText = "Velocity Label", method = Visuals.VelocityLabel, toolTip = "Puts text on your right hand, showing your velocity."},
                new ButtonInfo { buttonText = "Nearby Label", method = Visuals.NearbyTaggerLabel, toolTip = "Puts text on your left hand, showing you the distance of the nearest tagger."},
                new ButtonInfo { buttonText = "Last Label", method = Visuals.LastLabel, toolTip = "Puts text on your left hand, showing you how many untagged people are left."},
                new ButtonInfo { buttonText = "Time Label", method = Visuals.TimeLabel, toolTip = "Puts text on your right hand, showing how long you've been playing for without getting tagged."},

                new ButtonInfo { buttonText = "FPS Overlay", method =() => NotificationManager.information["FPS"] = lastDeltaTime.ToString(), disableMethod =() => NotificationManager.information.Remove("FPS"), toolTip = "Displays your FPS on your screen."},
                new ButtonInfo { buttonText = "Ping Overlay", method = Visuals.PingOverlay, disableMethod =() => NotificationManager.information.Remove("Ping"), toolTip = "Displays the server's ping on your screen."},
                new ButtonInfo { buttonText = "Time Overlay", method =() => NotificationManager.information["Time"] = DateTime.Now.ToString("hh:mm tt"), disableMethod =() => NotificationManager.information.Remove("Time"), toolTip = "Displays your current time on your screen."},
                new ButtonInfo { buttonText = "Playtime Overlay", method =() => { NotificationManager.information["Playtime"] = Visuals.OverallPlaytime; Visuals.UpdatePlaytime(); }, disableMethod =() => NotificationManager.information.Remove("Playtime"), toolTip = "Displays your play time from when the mod was enabled on your screen."},
                new ButtonInfo { buttonText = "Room Information Overlay", method =() => { if (PhotonNetwork.InRoom) { NotificationManager.information["Room Code"] = PhotonNetwork.CurrentRoom.Name; NotificationManager.information["Players"] = PhotonNetwork.PlayerList.Length.ToString(); } else { NotificationManager.information.Remove("Room Code"); NotificationManager.information.Remove("Players"); } }, disableMethod =() => { NotificationManager.information.Remove("Room Code"); NotificationManager.information.Remove("Players"); }, toolTip = "Displays information about the room on your screen."},
                new ButtonInfo { buttonText = "Networking Overlay", method =() => { NotificationManager.information["Ping"] = PhotonNetwork.GetPing().ToString(); NotificationManager.information["Region"] = NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper(); }, disableMethod =() => { NotificationManager.information.Remove("Ping"); NotificationManager.information.Remove("Region"); }, toolTip = "Displays information about networking on your screen."},
                new ButtonInfo { buttonText = "Clipboard Overlay", method =() => NotificationManager.information["Clip"] = GUIUtility.systemCopyBuffer.Length > 20 ? GUIUtility.systemCopyBuffer[..20] : GUIUtility.systemCopyBuffer, disableMethod =() => NotificationManager.information.Remove("Clip"), toolTip = "Displays your current clipboard on your screen."},
                new ButtonInfo { buttonText = "Velocity Overlay", method =() => NotificationManager.information["Velocity"] = $"{GorillaTagger.Instance.rigidbody.linearVelocity.magnitude:F1}m/s", disableMethod =() => NotificationManager.information.Remove("Velocity"), toolTip = "Displays your velocity on your screen."},
                new ButtonInfo { buttonText = "Nearby Overlay", method = Visuals.NearbyTaggerOverlay, disableMethod =() => NotificationManager.information.Remove("Nearby"), toolTip = "Displays the distance to the nearest tagger/target on your screen."},
                new ButtonInfo { buttonText = "Info Overlay Gun", method = Visuals.InfoOverlayGun, toolTip = "Displays an overlay, showing the information of whoever your hand desires."},

                new ButtonInfo { buttonText = "Debug HUD", aliases = new[] { "Developer HUD", "Debug UI", "Developer UI" }, enableMethod = Visuals.EnableDebugHUD, disableMethod = Visuals.DisableDebugHUD, toolTip = "Displays the developer debug HUD."},

                new ButtonInfo { buttonText = "Info Watch", enableMethod = Visuals.WatchOn, method = Visuals.WatchStep, disableMethod = Visuals.WatchOff, toolTip = "Puts a watch on your hand that tells you the time and your FPS."},
                new ButtonInfo { buttonText = "Leaderboard Info", enableMethod =() => UpdatePatch.enabled = true, method = Visuals.LeaderboardInfo, disableMethod =() => UpdatePatch.enabled = false, toolTip = "Shows info next to players' names on the leaderboard."},

                new ButtonInfo { buttonText = "FPS Boost", aliases = new[] { "Low Quality" }, enableMethod =() => QualitySettings.globalTextureMipmapLimit = int.MaxValue, disableMethod =() => QualitySettings.globalTextureMipmapLimit = 1, toolTip = "Makes everything low quality in an attempt to boost your FPS."},
                new ButtonInfo { buttonText = "Freeze In Background", enableMethod =() => Application.runInBackground = false, disableMethod =() => Application.runInBackground = true, toolTip = "Freezes the game when the application is not focused."},

                new ButtonInfo { buttonText = "Fake Unban Self", method = Visuals.FakeUnbanSelf, isTogglable = false, toolTip = "Makes it appear as if you're not banned." },

                new ButtonInfo { buttonText = "Jump Predictions", overlapText = "Jump Trajectories", method = Visuals.JumpPredictions, disableMethod = Visuals.DisableJumpPredictions, toolTip = "Shows a visualizer of where the other players will jump."},
                new ButtonInfo { buttonText = "Hitbox Predictions", method = Visuals.HitboxPredictions, disableMethod = Visuals.DisableHitboxPredictions, toolTip = "Shows capsules where other players' hitboxes are."},
                new ButtonInfo { buttonText = "Paintbrawl Trajectories", overlapText = "Projectile Trajectories", method = Visuals.PaintbrawlTrajectories, disableMethod = Visuals.DisablePaintbrawlTrajectories, toolTip = "Shows a visualizer of where all projectiles and your slingshot will hit."},

                new ButtonInfo { buttonText = "Audio Visualizer", enableMethod = Visuals.CreateAudioVisualizer, method = Visuals.AudioVisualizer, disableMethod = Visuals.DestroyAudioVisualizer, toolTip = "Shows a visualizer of your microphone loudness below your player."},
                new ButtonInfo { buttonText = "Show Server Position", method = Visuals.ShowServerPosition, disableMethod = Visuals.DisableShowServerPosition, toolTip = "Shows your current syncronized position on the server."},
                new ButtonInfo { buttonText = "Show Scheduled Objects", enableMethod = Visuals.ShowScheduledObjects, toolTip = "Shows all scheduled and planned objects before their target date."},

                new ButtonInfo { buttonText = "Visualize Network Triggers", method = Visuals.VisualizeNetworkTriggers, toolTip = "Visualizes the network joining and leaving triggers."},
                new ButtonInfo { buttonText = "Visualize Wind Barriers", aliases = new[] { "Visualize Air" }, method = Visuals.VisualizeWindBarriers, toolTip = "Visualizes the wind barriers."},
                new ButtonInfo { buttonText = "Visualize Map Triggers", method = Visuals.VisualizeMapTriggers, toolTip = "Visualizes the map loading and unloading triggers."},

                new ButtonInfo { buttonText = "Name Tags", method = Visuals.NameTags, disableMethod = Visuals.DisableNameTags, toolTip = "Gives players name tags above their heads that show their nickname."},
                new ButtonInfo { buttonText = "Velocity Name Tags", method = Visuals.VelocityTags, disableMethod = Visuals.DisableVelocityTags, toolTip = "Gives players name tags above their heads that show their velocity."},
                new ButtonInfo { buttonText = "FPS Name Tags", method = Visuals.FPSTags, disableMethod = Visuals.DisableFPSTags, toolTip = "Gives players name tags above their heads that show their FPS."},
                new ButtonInfo { buttonText = "ID Name Tags", method = Visuals.IDTags, disableMethod = Visuals.DisableIDTags, toolTip = "Gives players name tags above their heads that show their ID."},
                new ButtonInfo { buttonText = "Platform Name Tags", method = Visuals.PlatformTags, disableMethod = Visuals.DisablePlatformTags, toolTip = "Gives players name tags above their heads that show what platform they're playing on."},
                new ButtonInfo { buttonText = "k-ID Name Tags", method = Visuals.KIDNameTags, disableMethod = Visuals.DisableKIDNameTags, toolTip = "Gives players name tags above their heads that show if they have k-ID restrictions."},
                new ButtonInfo { buttonText = "Subscriber Name Tags", method = Visuals.SubscriberNameTags, disableMethod = Visuals.DisableSubscriberNameTags, toolTip = "Gives players name tags above their heads that show if they're subscribed to the fan club."},
                new ButtonInfo { buttonText = "Creation Date Name Tags", method = Visuals.CreationDateTags, disableMethod = Visuals.DisableCreationDateTags, toolTip = "Gives players name tags above their heads that show their creation date."},
                new ButtonInfo { buttonText = "Ping Name Tags", method = Visuals.PingTags, disableMethod = Visuals.DisablePingTags, toolTip = "Gives players name tags above their heads that show their ping."},
                new ButtonInfo { buttonText = "Turn Name Tags", method = Visuals.TurnTags, disableMethod = Visuals.DisableTurnTags, toolTip = "Gives players name tags above their heads that show their turn settings."},
                new ButtonInfo { buttonText = "Tagged Name Tags", method = Visuals.TaggedTags, disableMethod = Visuals.DisableTaggedTags, toolTip = "Gives players name tags above their heads that show who tagged them."},
                new ButtonInfo { buttonText = "Mod Name Tags", method = Visuals.ModTags, disableMethod = Visuals.DisableModTags, toolTip = "Gives players name tags above their heads that show what mods they have."},
                new ButtonInfo { buttonText = "Cosmetic Name Tags", method = Visuals.CosmeticTags, disableMethod = Visuals.DisableCosmeticTags, toolTip = "Gives players name tags above their heads that show what special cosmetics they have."},
                new ButtonInfo { buttonText = "Verified Name Tags", method = Visuals.VerifiedTags, disableMethod = Visuals.DisableVerifiedTags, toolTip = "Gives players name tags above their heads if they are a verified player."},
                new ButtonInfo { buttonText = "Lag Name Tags", method = Visuals.CrashedTags, disableMethod = Visuals.DisableCrashedTags, toolTip = "Gives players name tags above their heads if they are lagging."},
                new ButtonInfo { buttonText = "Compact Name Tags", overlapText = "VRChat Name Tags", aliases = new[] { "Compact Name Tags" }, method = Visuals.CompactTags, disableMethod = Visuals.DisableCompactTags, toolTip = "Gives players name tags above their heads that show a lot of information compactly. Credits to snake for the mod idea."},
                
                new ButtonInfo { buttonText = "Fix Rig Colors", method = Visuals.FixRigColors, toolTip = "Fixes a Steam bug where other players' color would be wrong between servers."},
                new ButtonInfo { buttonText = "Disable Rig Lerping", overlapText = "Disable Rig Smoothing", method = Visuals.NoSmoothRigs, disableMethod = Visuals.ReSmoothRigs, toolTip = "Disable the smoothing on the other player's rigs."},
                new ButtonInfo { buttonText = "Better Rig Lerping", overlapText = "Better Rig Smoothing", enableMethod =() => PlayerSerializePatch.OnPlayerSerialize += Visuals.BetterRigLerping, disableMethod =() => PlayerSerializePatch.OnPlayerSerialize -= Visuals.BetterRigLerping, toolTip = "Estimates the inbetween positions using a real velocity emulator on the other player's rigs."},
                new ButtonInfo { buttonText = "Remove Leaves", enableMethod = Visuals.EnableRemoveLeaves, disableMethod = Visuals.DisableRemoveLeaves, toolTip = "Removes leaves on trees, good for branching."},
                new ButtonInfo { buttonText = "Streamer Remove Leaves", enableMethod = Visuals.EnableStreamerRemoveLeaves, disableMethod = Visuals.DisableStreamerRemoveLeaves, toolTip = "Removes leaves on trees in VR, but not on the camera. Good for streaming."},
                new ButtonInfo { buttonText = "Remove Cosmetics", enableMethod = Visuals.DisableCosmetics, disableMethod = Visuals.EnableCosmetics, toolTip = "Locally toggles off your cosmetics, so you can wear sight-blocking cosmetics such as the eyepatch."},
                new ButtonInfo { buttonText = "X-Ray <color=grey>[</color><color=green>T</color><color=grey>]</color>", aliases = new[] { "See Through" }, method = Visuals.Xray, toolTip = "Lets you see through objects when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Cosmetic ESP", method = Visuals.CosmeticESP, disableMethod = Visuals.DisableCosmeticESP, toolTip = "Shows icons above people's heads if they are a Finger Painter, Illustrator, Administrator, Stick, Forest Guide, or Another Axiom Creator."},

                new ButtonInfo { buttonText = "Voice Indicators", method = Visuals.VoiceIndicators, disableMethod = Visuals.DisableVoiceIndicators, toolTip = "Puts voice indicators above people's heads when they're talking."},
                new ButtonInfo { buttonText = "Voice ESP", method = Visuals.VoiceESP, disableMethod = Visuals.DisableVoiceIndicators, toolTip = "Puts voice indicators above people's heads when they're talking, but now they go through walls."},

                new ButtonInfo { buttonText = "Platform Indicators", method = Visuals.PlatformIndicators, disableMethod = Visuals.DisablePlatformIndicators, toolTip = "Puts indicators above people's heads that show what platform they are playing on."},
                new ButtonInfo { buttonText = "Platform ESP", method = Visuals.PlatformESP, disableMethod = Visuals.DisablePlatformIndicators, toolTip = "Puts indicators above people's heads that show what platform they are playing on, but now they go through walls."},

                new ButtonInfo { buttonText = "No Limb Mode", enableMethod = Visuals.StartNoLimb, method = Visuals.NoLimbMode, disableMethod = Visuals.EndNoLimb, toolTip = "Makes your regular rig invisible, and puts balls on your hands."},

                new ButtonInfo { buttonText = "Casual Tracers", method = Visuals.CasualTracers, disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Tracers", method = Visuals.InfectionTracers, disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Tracers", method = Visuals.HuntTracers, disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Tracers", method =() => Visuals.AutomaticESP(Visuals.InfectionTracers, Visuals.HuntTracers, Visuals.CasualTracers), disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows targets for the current gamemode."},
                new ButtonInfo { buttonText = "Nearest Tracer", method = Visuals.NearestTracer, disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows the nearest player."},

                new ButtonInfo { buttonText = "Casual Box ESP", method = Visuals.CasualBoxESP, disableMethod = Visuals.DisableBoxESP, toolTip = "Puts boxes over players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Box ESP", method = Visuals.InfectionBoxESP, disableMethod = Visuals.DisableBoxESP, toolTip = "Puts boxes over players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Box ESP", method = Visuals.HuntBoxESP, disableMethod = Visuals.DisableBoxESP, toolTip = "Puts boxes over players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Box ESP", method =() => Visuals.AutomaticESP(Visuals.InfectionBoxESP, Visuals.HuntBoxESP, Visuals.CasualBoxESP), disableMethod = Visuals.DisableBoxESP, toolTip = "Puts boxes over players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Hollow Box ESP", method = Visuals.CasualHollowBoxESP, disableMethod = Visuals.DisableHollowBoxESP, toolTip = "Puts hollow boxes over players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Hollow Box ESP", method = Visuals.HollowInfectionBoxESP, disableMethod = Visuals.DisableHollowBoxESP, toolTip = "Puts hollow boxes over players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Hollow Box ESP", method = Visuals.HollowHuntBoxESP, disableMethod = Visuals.DisableHollowBoxESP, toolTip = "Puts hollow boxes over players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Hollow Box ESP", method =() => Visuals.AutomaticESP(Visuals.HollowInfectionBoxESP, Visuals.HollowHuntBoxESP, Visuals.CasualHollowBoxESP), disableMethod = Visuals.DisableHollowBoxESP, toolTip = "Puts hollow boxes over players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Breadcrumbs", method = Visuals.CasualBreadcrumbs, disableMethod = Visuals.DisableBreadcrumbs, toolTip = "Puts breadcrumb trails over players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Breadcrumbs", method = Visuals.InfectionBreadcrumbs, disableMethod = Visuals.DisableBreadcrumbs, toolTip = "Puts breadcrumb trails over players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Breadcrumbs", method = Visuals.HuntBreadcrumbs, disableMethod = Visuals.DisableBreadcrumbs, toolTip = "Puts breadcrumb trails over players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Breadcrumbs", method =() => Visuals.AutomaticESP(Visuals.InfectionBreadcrumbs, Visuals.HuntBreadcrumbs, Visuals.CasualBreadcrumbs), disableMethod = Visuals.DisableBreadcrumbs, toolTip = "Puts breadcrumb trails over players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Bone ESP", method = Visuals.CasualBoneESP, disableMethod = Visuals.DisableBoneESP, toolTip = "Puts bones over players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Bone ESP", method = Visuals.InfectionBoneESP, disableMethod = Visuals.DisableBoneESP, toolTip = "Puts bones over players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Bone ESP", method = Visuals.HuntBoneESP, disableMethod = Visuals.DisableBoneESP, toolTip = "Puts bones over players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Bone ESP", method =() => Visuals.AutomaticESP(Visuals.InfectionBoneESP, Visuals.HuntBoneESP, Visuals.CasualBoneESP), disableMethod = Visuals.DisableBoneESP, toolTip = "Puts bones over players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Skeleton ESP", method = Visuals.CasualSkeletonESP, disableMethod = Visuals.DisableSkeletonESP, toolTip = "Lets you see players skeletons through walls. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Skeleton ESP", method = Visuals.InfectionSkeletonESP, disableMethod = Visuals.DisableChams, toolTip = "Lets you see players skeletons through walls. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Skeleton ESP", method = Visuals.HuntSkeletonESP, disableMethod = Visuals.DisableChams, toolTip = "Lets you see players skeletons through walls. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Skeleton ESP", method =() => Visuals.AutomaticESP(Visuals.InfectionSkeletonESP, Visuals.HuntSkeletonESP, Visuals.CasualSkeletonESP), disableMethod = Visuals.DisableSkeletonESP, toolTip = "Lets you see players skeletons through walls. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Wireframe ESP", method = Visuals.CasualWireframeESP, disableMethod = Visuals.DisableWireframeESP, toolTip = "Puts wireframes over players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Wireframe ESP", method = Visuals.InfectionWireframeESP, disableMethod = Visuals.DisableWireframeESP, toolTip = "Puts wireframes over players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Wireframe ESP", method = Visuals.HuntWireframeESP, disableMethod = Visuals.DisableWireframeESP, toolTip = "Puts wireframes over players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Wireframe ESP", method =() => Visuals.AutomaticESP(Visuals.InfectionWireframeESP, Visuals.HuntWireframeESP, Visuals.CasualWireframeESP), disableMethod = Visuals.DisableWireframeESP, toolTip = "Puts wireframes over players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Chams", method = Visuals.Chams, disableMethod = Visuals.DisableShaderChams, toolTip = "Lets you see players through walls."},

                new ButtonInfo { buttonText = "Casual Chams", method = Visuals.CasualChams, disableMethod = Visuals.DisableChams, toolTip = "Lets you see players fur through walls. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Chams", method = Visuals.InfectionChams, disableMethod = Visuals.DisableChams, toolTip = "Lets you see players fur through walls. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Chams", method = Visuals.HuntChams, disableMethod = Visuals.DisableChams, toolTip = "Lets you see players fur through walls. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Chams", method =() => Visuals.AutomaticESP(Visuals.InfectionChams, Visuals.HuntChams, Visuals.CasualChams), disableMethod = Visuals.DisableChams, toolTip = "Lets you see players fur through walls. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Beacons", method = Visuals.CasualBeacons, disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Puts a beacon above players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Beacons", method = Visuals.InfectionBeacons, disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Puts a beacon above players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Beacons", method = Visuals.HuntBeacons, disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Puts a beacon above players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Beacons", method =() => Visuals.AutomaticESP(Visuals.InfectionBeacons, Visuals.HuntBeacons, Visuals.CasualBeacons), disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Puts a beacon above players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Casual Distance ESP", method = Visuals.CasualDistanceESP, disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Shows your distance from players. Shows everyone."},
                new ButtonInfo { buttonText = "Infection Distance ESP", method = Visuals.InfectionDistanceESP, disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Shows your distance from players. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Hunt Distance ESP", method = Visuals.HuntDistanceESP, disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Shows your distance from players. Shows your target and who is hunting you."},
                new ButtonInfo { buttonText = "Automatic Distance ESP", method =() => Visuals.AutomaticESP(Visuals.InfectionDistanceESP, Visuals.HuntDistanceESP, Visuals.CasualDistanceESP), disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Shows your distance from players. Shows targets for the current gamemode."},

                new ButtonInfo { buttonText = "Show Pointers", method = Visuals.ShowButtonColliders, disableMethod = Visuals.HideButtonColliders, toolTip = "Shows dots near your hands, such as when you open the menu."},

                new ButtonInfo { buttonText = "Info Watch Menu Name", enableMethod =() => Visuals.infoWatchMenuName = true, disableMethod =() => Visuals.infoWatchMenuName = false, toolTip = "Shows the menu name on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch FPS", enableMethod =() => Visuals.infoWatchFPS = true, disableMethod =() => Visuals.infoWatchFPS = false, toolTip = "Shows your framerate on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Time", enableMethod =() => Visuals.infoWatchTime = true, disableMethod =() => Visuals.infoWatchTime = false, toolTip = "Shows the current time on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Clipboard", enableMethod =() => Visuals.infoWatchClip = true, disableMethod =() => Visuals.infoWatchClip = false, toolTip = "Shows your clipboard on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Code", enableMethod =() => Visuals.infoWatchCode = true, disableMethod =() => Visuals.infoWatchCode = false, toolTip = "Shows the lobby code on the Info Watch mod."},
            },

            new[] { // Fun Mods [12]
                new ButtonInfo { buttonText = "Exit Fun Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Custom Maps", method =() => CurrentCategoryName = "Custom Maps", isTogglable = false, toolTip = "Opens the custom maps tab."},

                new ButtonInfo { buttonText = "Upside Down Head", method = Fun.UpsideDownHead, disableMethod = Fun.FixHead, toolTip = "Flips your head upside down on the Z axis."},
                new ButtonInfo { buttonText = "Backwards Head", method = Fun.BackwardsHead, disableMethod = Fun.FixHead, toolTip = "Rotates your head 180 degrees on the Y axis."},
                new ButtonInfo { buttonText = "Sideways Head", method = Fun.SidewaysHead, disableMethod = Fun.FixHead, toolTip = "Rotates your head 90 degrees on the Y axis."},

                new ButtonInfo { buttonText = "Broken Neck", method = Fun.BrokenNeck, disableMethod = Fun.FixHead, toolTip = "Rotates your head 90 degrees on the Z axis."},

                new ButtonInfo { buttonText = "Head Bang", method = Fun.HeadBang, disableMethod = Fun.FixHead, toolTip = "Bangs your head at the BPM of Paint it Black (159)."},

                new ButtonInfo { buttonText = "Flip Hands", aliases = new[] { "Fish Arms" }, method = Fun.FlipHands, toolTip = "Swaps your hands, left is right and right is left."},
                new ButtonInfo { buttonText = "Loud Hand Taps", method = Fun.LoudHandTaps, disableMethod = Fun.FixHandTaps, toolTip = "Makes your hand taps really loud."},
                new ButtonInfo { buttonText = "Silent Hand Taps", aliases = new[] { "No Hand Taps" }, method = Fun.SilentHandTaps, disableMethod = Fun.FixHandTaps, toolTip = "Makes your hand taps really quiet."},
                new ButtonInfo { buttonText = "Instant Hand Taps", method =() => GorillaTagger.Instance.tapCoolDown = 0f, disableMethod =() => GorillaTagger.Instance.tapCoolDown = 0.33f, toolTip = "Removes the hand tap cooldown."},
                new ButtonInfo { buttonText = "Silent Hand Taps on Tag", aliases = new[] { "No Hand Taps on Tag" }, method = Fun.SilentHandTapsOnTag, disableMethod = Fun.FixHandTaps, toolTip = "Makes your hand taps really quiet when you're tagged, good for ambush."},

                new ButtonInfo { buttonText = "Water Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.WaterSplashHands, toolTip = "Splashes water when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Give Water Splash Hands Gun", method = Fun.GiveWaterSplashHandsGun, toolTip = "Gives whoever your hand desires the water splash hands mod." },

                new ButtonInfo { buttonText = "Water Splash Walk", method = Fun.WaterSplashWalk, toolTip = "Splashes water whenever you take a step."},
                new ButtonInfo { buttonText = "Water Splash Aura", method = Fun.WaterSplashAura, toolTip = "Splashes water around you at random positions."},
                new ButtonInfo { buttonText = "Orbit Water Splash", method = Fun.OrbitWaterSplash, toolTip = "Splashes water orbitally around you."},
                new ButtonInfo { buttonText = "Water Splash Gun", method = Fun.WaterSplashGun, toolTip = "Splashes water wherever your hand desires."},

                new ButtonInfo { buttonText = "Confuse Player Gun", method = Movement.ConfusePlayerGun, toolTip = "Makes whoever your hand desires look like they're going crazy by splashing water on their screen."},
                new ButtonInfo { buttonText = "Confuse All Players", enableMethod = Movement.ConfuseAllPlayers, method = Movement.ConfuseAllPlayersSplash, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Splashes water on everyone's screens, making them look like they're going crazy."},

                new ButtonInfo { buttonText = "Tinnitus Gun", aliases = new[] { "Ear Rape All", "Earrape All" }, method = Movement.TinnitusGun, disableMethod = Movement.DisableTinnitus, toolTip = "Plays high pitched noises for whoever your hand desires."},
                new ButtonInfo { buttonText = "Tinnitus All", aliases = new[] { "Ear Rape All", "Earrape All" }, enableMethod = Movement.TinnitusAll, disableMethod = Movement.DisableTinnitus, toolTip = "Plays high pitched noises for everyone in the room."},

                new ButtonInfo { buttonText = "Overstimulate Gun", method = Movement.OverstimulateGun, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Overstimulates whoever your hand desires."},
                new ButtonInfo { buttonText = "Overstimulate All", method = Movement.OverstimulateAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Overstimulates everyone in the room."},

                new ButtonInfo { buttonText = "Shutdown Headset Gun", method = Movement.ShutdownHeadsetGun, disableMethod = Movement.DisableTinnitus, toolTip = "Pretends to shut down the headset of whoever your hand desires."},
                new ButtonInfo { buttonText = "Shutdown Headset All", enableMethod =() => Sound.PlayAudio(AssetUtilities.LoadSoundFromURL($"{PluginInfo.ServerResourcePath}/Audio/Mods/Fun/shutdown.ogg", "Audio/Mods/Fun/shutdown.ogg")), method = Movement.ShutdownHeadsetAll, disableMethod = Movement.DisableTinnitus, toolTip = "Pretends to shut down the headset of everyone in the room."},

                new ButtonInfo { buttonText = "Schizophrenic Gun", method = Movement.SchizophrenicGun, toolTip = "Makes you not appear for whoever your hand desires."},
                new ButtonInfo { buttonText = "Reverse Schizophrenic Gun", method = Movement.ReverseSchizoGun, toolTip = "Makes you only appear for whoever your hand desires."},

                new ButtonInfo { buttonText = "Boop", method =() => Fun.Boop(), toolTip = "Makes a pop sound when you touch someone's nose."},
                new ButtonInfo { buttonText = "Gong", method =() => Fun.Boop(248), toolTip = "Makes a gong sound when you hit someone's face."},
                new ButtonInfo { buttonText = "Slap", method =() => Fun.Boop(338), toolTip = "Makes a slap sound when you hit someone's face."},

                new ButtonInfo { buttonText = "Auto Clicker <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Fun.AutoClicker, toolTip = "Automatically presses  trigger for you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Keyboard Tracker", enableMethod = Fun.EnableKeyboardTracker, method = Fun.KeyboardTracker, disableMethod = Fun.DisableKeyboardTracker, toolTip = "Tracks everyone's keyboard inputs in the lobby."},

                new ButtonInfo { buttonText = "Tag Sounds", enableMethod = Fun.PreloadTagSounds, method =() => TagPatch.enabled = true, disableMethod =() => TagPatch.enabled = false, toolTip = "Plays a selection of dramatic sound effects when tagging players. Credits to Wyndigo for the idea."},

                new ButtonInfo { buttonText = "Free Camera <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Fun.Freecam, disableMethod = Fun.DisableFreecam, toolTip = "Exit your own body and fly around to your free will."},
                new ButtonInfo { buttonText = "Third Person Camera", method = Fun.ThirdPersonCamera, disableMethod = Fun.DisableFreecam, toolTip = "Moves your camera to behind your head."},
                new ButtonInfo { buttonText = "Flip Camera", method = Fun.FlipCamera, disableMethod = Fun.DisableFreecam, toolTip = "Flips your camera 180 degrees."},
                new ButtonInfo { buttonText = "Camera FOV", method = Fun.CameraFOV, disableMethod = Fun.FixCameraFOV, toolTip = "Changes the FOV of your PC camera."},
                new ButtonInfo { buttonText = "Spectate Gun", method = Fun.SpectateGun, disableMethod = Fun.DisableFreecam, toolTip = "Lets you see through the eyes of whoever your hand desires."},

                new ButtonInfo { buttonText = "Nausea", aliases = new[] { "Sick", "Nautious", "Drunk" }, method = Fun.Nausea, disableMethod = Fun.DisableFreecam, toolTip = "Gives you the Nausea effect from Minecraft."},
                new ButtonInfo { buttonText = "LSD", aliases = new[] { "Drugs", "High" }, method =() => { Color rgb = Color.HSVToRGB(Time.frameCount / 180f % 1f, 1f, 1f); Fun.HueShift(new Color(rgb.r, rgb.g, rgb.b, 0.1f)); }, disableMethod =() => Fun.HueShift(Color.clear), toolTip = "Hue shifts your game to a rainbow color."},
                new ButtonInfo { buttonText = "Jumpscare on Tag", enableMethod = Fun.PreloadJumpscareData, method = Fun.JumpscareOnTag, toolTip = "Gives a 1/2000 chance of a jumpscare happening when getting tagged."},
                new ButtonInfo { buttonText = "Spam Jumpscare", method = Fun.Jumpscare, toolTip = "Repeatedly jumpscares you."},
                new ButtonInfo { buttonText = "Jumpscare", method = Fun.Jumpscare, isTogglable = false, toolTip = "Jumpscares you."},

                new ButtonInfo { buttonText = "Prioritize Voice Gun", method = Fun.PrioritizeVoiceGun, toolTip = "Prioritizes whoever your hand desires' voice."},
                new ButtonInfo { buttonText = "Deprioritize Voice Gun", method = Fun.DeprioritizeVoiceGun, toolTip = "Deprioritizes whoever your hand desires' voice."},
                new ButtonInfo { buttonText = "Reset Voice All", method = Fun.ResetVoiceAll, toolTip = "Resets everyones voice back to normal."},

                new ButtonInfo { buttonText = "Mute Gun", method = Fun.MuteGun, toolTip = "Mutes or unmutes whoever your hand desires."},
                new ButtonInfo { buttonText = "Mute All", method = Fun.MuteAll, disableMethod = Fun.UnmuteAll, toolTip = "Mutes everyone in the room."},

                new ButtonInfo { buttonText = "Report Gun", method = Fun.ReportGun, toolTip = "Reports whoever your hand desires for cheating."},
                new ButtonInfo { buttonText = "Report All", method = Fun.ReportAll, isTogglable = false, toolTip = "Reports everyone in the room for cheating."},

                new ButtonInfo { buttonText = "Trigger Anti Report Gun", method = Fun.TriggerAntiReportGun, toolTip = "Triggers whoever your hand desires' anti report if enabled."},
                new ButtonInfo { buttonText = "Trigger Anti Report All", method = Fun.TriggerAntiReportAll, disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Triggers everyone in the room's anti report if enabled."},
                new ButtonInfo { buttonText = "Bypass Anti Report", method = Fun.BypassAntiReport, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Bypasses anti report mods when reporting players."},

                new ButtonInfo { buttonText = "Break Mod Checkers", enableMethod = Fun.BreakModCheckers, disableMethod = Safety.BypassModCheckers, toolTip = "Tells players using mod checkers that you have every mod possible."},
                new ButtonInfo { buttonText = "Custom Mod Spoofer", method = Fun.CustomModSpoofer, isTogglable = false, toolTip = "Make mod checkers see only what you allow."},

                new ButtonInfo { buttonText = "Mute DJ Sets", method = Fun.MuteDJSets, disableMethod = Fun.UnmuteDJSets, toolTip = "Mutes every DJ set so you don't have to hear the worst music known to man."},
                new ButtonInfo { buttonText = "Infinite Dreidel", method =() => DreidelPatch.enabled = true, disableMethod =() => DreidelPatch.enabled = false, toolTip = "Makes the dreidel cosmetic spin forever."},

                new ButtonInfo { buttonText = "Legacy Microphone", enableMethod =() => { RecorderPatch.enabled = false; Fun.ReloadMicrophone();  }, disableMethod =() => { RecorderPatch.enabled = true; Fun.ReloadMicrophone(); }, toolTip = "Reverts the microphone system into using the legacy input switcher. This is generally not recommended." },
                new ButtonInfo { buttonText = "Low Quality Microphone", method =() => Fun.SetMicrophoneQuality(6000, 4000), disableMethod =() => Fun.SetMicrophoneQuality(20000, 16000), toolTip = "Makes your microphone have really bad quality."},
                new ButtonInfo { buttonText = "Loud Microphone", method =() => Fun.SetMicrophoneAmplification(true), disableMethod =() => Fun.SetMicrophoneAmplification(false), toolTip = "Makes your microphone really loud."},
                new ButtonInfo { buttonText = "Echo Microphone", method =() => Fun.EchoMicrophone(true), disableMethod =() => Fun.EchoMicrophone(false), toolTip = "Makes your microphone echo."},
                new ButtonInfo { buttonText = "Glitchy Microphone", method =() => Fun.GlitchyMicrophone(true), disableMethod =() => Fun.GlitchyMicrophone(false), toolTip = "Makes your microphone glitchy."},
                new ButtonInfo { buttonText = "Laggy Microphone", method =() => Fun.LaggyMicrophone(true), disableMethod =() => Fun.LaggyMicrophone(false), toolTip = "Makes your microphone laggy."},

                new ButtonInfo { buttonText = "Mute Microphone", method =() => Fun.MuteMicrophone(true), disableMethod =() => Fun.MuteMicrophone(false), toolTip = "Disables your microphone."},

                new ButtonInfo { buttonText = "Very High Pitch Microphone", method =() => Fun.SetMicrophonePitch(2.5f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone very very high pitched."},
                new ButtonInfo { buttonText = "High Pitch Microphone", method =() => Fun.SetMicrophonePitch(1.5f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone high pitched."},
                new ButtonInfo { buttonText = "Low Pitch Microphone", method =() => Fun.SetMicrophonePitch(0.5f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone low pitched."},
                new ButtonInfo { buttonText = "Very Low Pitch Microphone", method =() => Fun.SetMicrophonePitch(0.01f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone very very low pitched."},

                new ButtonInfo { buttonText = "Reload Microphone", aliases = new[] { "Restart Microphone" }, method = Fun.ReloadMicrophone, isTogglable = false,  toolTip = "Restarts / fixes your microphone."},

                new ButtonInfo { buttonText = "Microphone Feedback", method =() => Fun.SetDebugEchoMode(true), disableMethod =() => Fun.SetDebugEchoMode(false), toolTip = "Plays sound coming through your microphone back to your speakers."},
                new ButtonInfo { buttonText = "Copy Voice Gun", method = Fun.CopyVoiceGun, disableMethod = Fun.DisableCopyVoice, toolTip = "Copies the voice of whoever your hand desires."},

                new ButtonInfo { buttonText = "Narrate Text", method =() => PromptText("What would you like to be narrated?", () => SpeakText(keyboardInput), null, "Done", "Cancel"), isTogglable = false, toolTip = "Narrates the text of your desire."},
                new ButtonInfo { buttonText = "Save Narration", method =() => PromptText("What would you like the narration to say?", () => Fun.SaveNarration(keyboardInput)), isTogglable = false, toolTip = "Saves whatever you want to narrate to your soundboard."},
                new ButtonInfo { buttonText = "Mask Voice", enableMethod = Fun.MaskVoice, method =() => { Settings.CheckFocus(); GorillaTagger.Instance.myRecorder.IsRecording = Sound.AudioIsPlaying; }, disableMethod = Fun.DisableMaskVoice, toolTip = "Masks your voice with a TTS voice."},

                new ButtonInfo { buttonText = "Disable Pitch Scaling", method = Important.DisablePitchScaling, disableMethod = Important.EnablePitchScaling, toolTip = "Disables the pitch effects on players' voices when they are a different scale."},
                new ButtonInfo { buttonText = "Disable Mouth Movement", method = Important.DisableMouthMovement, disableMethod = Important.EnableMouthMovement, toolTip = "Disables your mouth from moving."},

                new ButtonInfo { buttonText = "Activate All Doors <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.ActivateAllDoors, toolTip = "Activates all doors when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Crystals <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.TapAllClass<GorillaCaveCrystal>, toolTip = "Taps all crystals when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Bells <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.TapAllClass<TappableBell>, toolTip = "Taps all bells when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Trigger Leaf Pile Gun", method = Fun.TriggerLeafPileGun, toolTip = "Shows the effects on whatever leaf pile you desire."},

                new ButtonInfo { buttonText = "Get Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GetBracelet(true), toolTip = "Gives you a party bracelet without needing to be in a party."},
                new ButtonInfo { buttonText = "Spam Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.BraceletSpam, toolTip = "Spams the party bracelet on and off."},
                new ButtonInfo { buttonText = "Remove Bracelet", method = Fun.RemoveBracelet, isTogglable = false, toolTip = "Disables the party bracelet. This does not kick you from the party."},

                new ButtonInfo { buttonText = "Rainbow Bracelet", method = Fun.RainbowBracelet, disableMethod = Fun.RemoveRainbowBracelet, toolTip = "Gives you a rainbow party bracelet."},

                new ButtonInfo { buttonText = "Quest Noises <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Fun.QuestNoises, toolTip = "Makes noises at the quest machine in city when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Max Quest Score", method = Fun.MaxQuestScore, toolTip = "Gives you the maximum quest score in the game (99999)."},
                new ButtonInfo { buttonText = "Custom Quest Score", method = Fun.CustomQuestScore, toolTip = "Gives you a custom quest score. You can change this in the settings."},

                new ButtonInfo { buttonText = "Matchmaking Tier Spoof", method =() => Safety.SpoofRank(true, Safety.targetRank), disableMethod =() => Safety.SpoofRank(false), toolTip = "Spoofs your rank for competitive lobbies, letting you join higher or lower lobbies."},
                new ButtonInfo { buttonText = "Matchmaking Platform Spoof", method =() => Safety.SpoofPlatform(true, "Quest"), disableMethod =() => Safety.SpoofPlatform(false), toolTip = "Spoofs your platform for competitive lobbies, letting you join quest lobbies."},

                new ButtonInfo { buttonText = "Badge Tier Spoof", method = Safety.SpoofBadge, disableMethod =() => SetRankedPatch.enabled = false, toolTip = "Spoofs your competitive badge, showing that you have a higher rank than you really do."},
                new ButtonInfo { buttonText = "Ignore Friend Privacy", method =() => PopulatePatch.enabled = true, disableMethod =() => PopulatePatch.enabled = false, toolTip = "Allows you to join friends that have their privacy setting set to private."},

                new ButtonInfo { buttonText = "Arcade Teleporter Effect Spam", method = Fun.ArcadeTeleporterEffectSpam, toolTip = "Spams the effects on the virtual stump teleporters in the arcade when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Stump Teleporter Effect Spam", method = Fun.StumpTeleporterEffectSpam, toolTip = "Spams the effects on the virtual stump teleporter in forest when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Open Basement Door", method =() => Fun.SetBasementDoorState(true), isTogglable = false, toolTip = "Opens the basement door."},
                new ButtonInfo { buttonText = "Open Elevator Door", method =() => Fun.SetElevatorDoorState(true), isTogglable = false, toolTip = "Opens the elevator door."},

                new ButtonInfo { buttonText = "Close Basement Door", method =() => Fun.SetBasementDoorState(false), isTogglable = false, toolTip = "Closes the basement door."},
                new ButtonInfo { buttonText = "Close Elevator Door", method =() => Fun.SetElevatorDoorState(false), isTogglable = false, toolTip = "Closes the elevator door."},
                    
                new ButtonInfo { buttonText = "Spam Open Basement Door", method =() => Fun.SetBasementDoorState(true), toolTip = "Repeatedly opens the basement door."},
                new ButtonInfo { buttonText = "Spam Open Elevator Door", method =() => Fun.SetElevatorDoorState(true), toolTip = "Repeatedly opens the elevator door."},

                new ButtonInfo { buttonText = "Spam Close Basement Door", method =() => Fun.SetBasementDoorState(false), toolTip = "Repeatedly closes the basement door."},
                new ButtonInfo { buttonText = "Spam Close Elevator Door", method =() => Fun.SetElevatorDoorState(false), toolTip = "Repeatedly closes the elevator door."},

                new ButtonInfo { buttonText = "Basement Door Spam", method = Fun.BasementDoorSpam, toolTip = "Repeatedly opens and closes the basement door."},
                new ButtonInfo { buttonText = "Elevator Door Spam", method = Fun.ElevatorDoorSpam, toolTip = "Repeatedly opens and closes the elevator door."},
                
                new ButtonInfo { buttonText = "Custom Virtual Stump Video", enableMethod = Fun.CustomVirtualStumpVideo, disableMethod = Fun.DisableCustomVirtualStumpVideo, toolTip = "Plays a video by the virtual stump VR headset in stump."},

                new ButtonInfo { buttonText = "Fake FPS", method = Fun.FakeFPS, disableMethod =() => FPSPatch.enabled = false, toolTip = "Makes your FPS appear to be completely random to other players and the competitive bot."},

                new ButtonInfo { buttonText = "Get Builder Watch", method = Fun.GiveBuilderWatch, isTogglable = false, toolTip = "Gives you the builder watch without needing to be in attic."},
                new ButtonInfo { buttonText = "Remove Builder Watch", method = Fun.RemoveBuilderWatch, isTogglable = false, toolTip = "Disables the builder watch."},

                new ButtonInfo { buttonText = "Joystick Rope Control <color=grey>[</color><color=green>J</color><color=grey>]</color>", method = Overpowered.JoystickRopeControl, toolTip = "Control the ropes in the direction of your joystick."},

                new ButtonInfo { buttonText = "Broken Ropes", method = Overpowered.SpazGrabbedRopes, toolTip = "Gives any ropes currently being held onto a seizure."},
                new ButtonInfo { buttonText = "Spaz Rope Gun", method = Overpowered.SpazRopeGun, toolTip = "Gives whatever rope your hand desires a seizure."},
                new ButtonInfo { buttonText = "Spaz All Ropes <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SpazAllRopes, toolTip = "Gives every rope a seizure when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Fling Rope Gun", method = Overpowered.FlingRopeGun, toolTip = "Flings whatever rope your hand desires away from you."},
                new ButtonInfo { buttonText = "Fling All Ropes Gun", method = Overpowered.FlingAllRopesGun, toolTip = "Flings every rope in whatever direction your hand desires."},

                new ButtonInfo { buttonText = "Fast Gliders", enableMethod =() => Fun.ModifyGliderSpeed(0.5f, 0.5f), disableMethod =() => Fun.ModifyGliderSpeed(0.1f, 0.2f), toolTip = "Makes the gliders fast."},
                new ButtonInfo { buttonText = "Slow Gliders", enableMethod =() => Fun.ModifyGliderSpeed(0.05f, 0.05f), disableMethod =() => Fun.ModifyGliderSpeed(0.1f, 0.2f), toolTip = "Makes the gliders slow."},

                new ButtonInfo { buttonText = "Glider Blind Gun", method = Overpowered.GliderBlindGun, toolTip = "Moves all of the gliders to whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Glider Blind All", method = Overpowered.GliderBlindAll, toolTip = "Moves all of the gliders to everyone's faces." },

                new ButtonInfo { buttonText = "Fast Ropes", enableMethod =() => RopePatch.enabled = true, disableMethod =() => RopePatch.enabled = false, toolTip = "Makes ropes go five times faster when jumping on them."},
                new ButtonInfo { buttonText = "Rope Grab Reach", method = Fun.RopeGrabReach, disableMethod =() => ClimbablePatch.enabled = false, toolTip = "Allows you to grab ropes from farther away."},

                new ButtonInfo { buttonText = "No Respawn Gliders", enableMethod =() => GliderPatch.enabled = true, disableMethod =() => GliderPatch.enabled = false, toolTip = "Doesn't respawn gliders that go too far outside the bounds of clouds."},

                new ButtonInfo { buttonText = "Anti Grab", enableMethod =() => GrabPatch.enabled = true, disableMethod =() => GrabPatch.enabled = false, toolTip = "Prevents players from picking you up in guardian."},
                new ButtonInfo { buttonText = "Anti Knockback", enableMethod =() => KnockbackPatch.enabled = true, disableMethod =() => KnockbackPatch.enabled = false, toolTip = "Prevents any force from knocking you back."},
                new ButtonInfo { buttonText = "Multiply Knockback", enableMethod =() => MultiplyKnockback.enabled = true, disableMethod =() => MultiplyKnockback.enabled = false, toolTip = "Multiplies your knockback by an amount set in settings."},
                new ButtonInfo { buttonText = "Multiply Self Knockback", enableMethod =() => MultiplySelfKnockbackPatch.enabled = true, disableMethod =() => MultiplySelfKnockbackPatch.enabled = false, toolTip = "Multiplies your projectile knockback by an amount set in settings."},

                new ButtonInfo { buttonText = "Fast Throw", method =() => { VelocityPatches.enabled = true; VelocityPatches.multipleFactor = 10f; }, disableMethod =() => VelocityPatches.enabled = false, toolTip = "Multiplies your throw factor by 10."},
                new ButtonInfo { buttonText = "Slow Throw", method =() => { VelocityPatches.enabled = true; VelocityPatches.multipleFactor = 0.1f; }, disableMethod =() => VelocityPatches.enabled = false, toolTip = "Multiplies your throw factor by 0.1."},

                new ButtonInfo { buttonText = "Slingshot Self", enableMethod =() => Fun.SlingshotSelf(), disableMethod =() => Fun.SlingshotSelf(false), toolTip = "Gives you a client sided slingshot."},
                new ButtonInfo { buttonText = "Angry Birds", enableMethod =() => LaunchProjectilePatch.enabled = true, method = Fun.AngryBirdsSounds, disableMethod =() => LaunchProjectilePatch.enabled = false, toolTip = "Flings you in whatever direction your slingshot's projectiles are heading."},

                new ButtonInfo { buttonText = "Large Snowballs", enableMethod =() => EnablePatch.enabled = true, disableMethod =() => EnablePatch.enabled = false, toolTip = "Makes snowballs by default the largest size."},
                new ButtonInfo { buttonText = "Spaz Snowballs <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.SpazSnowballs, toolTip = "Randomizes the size of the snowballs. Credits to test for the idea."},

                new ButtonInfo { buttonText = "Multiply Snowballs", method =() => ThrowPatch.enabled = true, disableMethod =() => ThrowPatch.enabled = false, toolTip = "Multiplies the snowballs you throw by 5."},

                new ButtonInfo { buttonText = "Fast Snowballs", overlapText = "Fast Projectiles", method = Fun.FastSnowballs, disableMethod = Fun.FixSnowballs, toolTip = "Makes projectiles go really fast when thrown."},
                new ButtonInfo { buttonText = "Slow Snowballs", overlapText = "Slow Projectiles", method = Fun.SlowSnowballs, disableMethod = Fun.FixSnowballs, toolTip = "Makes projectiles go really slow when thrown."},

                new ButtonInfo { buttonText = "Projectile Range", method = Fun.ProjectileRange, toolTip = "Increases the hitbox scale of your projectiles."},

                new ButtonInfo { buttonText = "Rainbow Held Projectiles", enableMethod = Fun.HookProjectileColors, method =() => Fun.projHookColor = Color.HSVToRGB(Time.frameCount / 180f % 1f, 1f, 1f), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Changes your projectile's color to be rainbow"},
                new ButtonInfo { buttonText = "Flash Held Projectiles", enableMethod = Fun.HookProjectileColors, method =() => Fun.projHookColor = Time.time % 0.2f > 0.1f ? Color.white : Color.black, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Changes your projectile's color to be rainbow"},
                new ButtonInfo { buttonText = "Strobe Held Projectiles", enableMethod = Fun.HookProjectileColors, method =() => Fun.projHookColor = RandomColor(), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Changes your projectile's color to be rainbow"},
                new ButtonInfo { buttonText = "Custom Held Projectiles", enableMethod = Fun.HookProjectileColors, method =() => Fun.projHookColor = new Color(Projectiles.red / 10f, Projectiles.green / 10f, Projectiles.blue / 10f), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Changes your projectile's color to be whatever your custom projectile color is set to in the projectile settings."},

                new ButtonInfo { buttonText = "Snowball Buttocks", method = Fun.SnowballButtocks, disableMethod = Fun.DisableSnowballGenitals, toolTip = "Gives you fake buttocks using the snowballs." },
                new ButtonInfo { buttonText = "Snowball Breasts", method = Fun.SnowballBreasts, disableMethod = Fun.DisableSnowballGenitals, toolTip = "Gives you fake breasts using the snowballs." },

                new ButtonInfo { buttonText = "Fast Hoverboard", method = Fun.FastHoverboard, disableMethod = Fun.FixHoverboard, toolTip = "Makes your hoverboard go really fast."},
                new ButtonInfo { buttonText = "Slow Hoverboard", method = Fun.SlowHoverboard, disableMethod = Fun.FixHoverboard, toolTip = "Makes your hoverboard go really slow."},

                new ButtonInfo { buttonText = "Rainbow Hoverboard", method = Fun.RainbowHoverboard, toolTip = "Changes your hoverboard's color to be rainbow."},
                new ButtonInfo { buttonText = "Strobe Hoverboard", overlapText = "Flash Hoverboard", method = Fun.StrobeHoverboard, toolTip = "Changes your hoverboard's color to flash between black and white."},
                new ButtonInfo { buttonText = "Random Hoverboard", overlapText = "Strobe Hoverboard", method = Fun.RandomHoverboard, toolTip = "Changes your hoverboard's color to flash random colors."},

                new ButtonInfo { buttonText = "Global Hoverboard", method = Fun.GlobalHoverboard, disableMethod = Fun.DisableGlobalHoverboard, toolTip = "Gives you the hoverboard no matter where you are."},

                new ButtonInfo { buttonText = "Black Screen Gun", method =() => Fun.HoverboardScreenGun(Color.black), toolTip = "Uses the hoverboards to blind whoever your hand desires."},
                new ButtonInfo { buttonText = "Black Screen All", method =() => Fun.HoverboardScreenAll(Color.black), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the hoverboards to blind everyone in the room."},

                new ButtonInfo { buttonText = "White Screen Gun", method =() => Fun.HoverboardScreenGun(Color.white), toolTip = "Uses the hoverboards to make whoever your hand desires' screen white."},
                new ButtonInfo { buttonText = "White Screen All", method =() => Fun.HoverboardScreenAll(Color.white), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the hoverboards to flash the screen of everyone in the room."},

                new ButtonInfo { buttonText = "Flash Screen Gun", method =() => Fun.HoverboardScreenGun(Time.time % 0.2f > 0.1f ? Color.white : Color.black), toolTip = "Uses the hoverboards to flash the screen of whoever your hand desires."},
                new ButtonInfo { buttonText = "Flash Screen All", method =() => Fun.HoverboardScreenAll(Time.time % 0.2f > 0.1f ? Color.white : Color.black), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the hoverboards to blind everyone in the room."},

                new ButtonInfo { buttonText = "Strobe Screen Gun", method =() => Fun.HoverboardScreenGun(RandomColor()), toolTip = "Uses the hoverboards to flash the screen of whoever your hand desires."},
                new ButtonInfo { buttonText = "Strobe Screen All", method =() => Fun.HoverboardScreenAll(RandomColor()), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the hoverboards to blind everyone in the room."},

                new ButtonInfo { buttonText = "Rainbow Screen Gun", method =() => Fun.HoverboardScreenGun(Color.HSVToRGB(Time.frameCount / 180f % 1f, 1f, 1f)), toolTip = "Uses the hoverboards to make the screen of whoever your hand desires rainbow."},
                new ButtonInfo { buttonText = "Rainbow Screen All", method =() => Fun.HoverboardScreenAll(Color.HSVToRGB(Time.frameCount / 180f % 1f, 1f, 1f)), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the hoverboards to make the screen of everyone in the room rainbow."},

                new ButtonInfo { buttonText = "Hoverboard Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.HoverboardSpam, toolTip = "Spams hoverboards from your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Spam Spawn Hoverboards", method = Fun.SpawnHoverboard, toolTip = "Spam spawns hoverboards at your player position."},
                new ButtonInfo { buttonText = "Spawn Hoverboard", method = Fun.SpawnHoverboard, isTogglable = false, toolTip = "Spawns a hoverboard at your player position."},

                new ButtonInfo { buttonText = "Start All Races", method = Fun.StartAllRaces, isTogglable = false, toolTip = "Starts every race in the hoverboard map."},

                new ButtonInfo { buttonText = "Override Hand Link", method =() => GroundedPatch.enabled = true, disableMethod =() => GroundedPatch.enabled = false, toolTip = "Prioritizes you when you or others grab onto you."},
                new ButtonInfo { buttonText = "Disable Hand Link", method =() => { VRRig.LocalRig.leftHandLink.BreakLink(); VRRig.LocalRig.rightHandLink.BreakLink(); }, toolTip = "Disables you from grabbing onto other players and them from grabbing onto you."},
                new ButtonInfo { buttonText = "Anti Hand Link", method =() => HandLinkPatch.enabled = true, disableMethod =() => HandLinkPatch.enabled = false, toolTip = "Disables you from moving when grabbing onto other players."},

                new ButtonInfo { buttonText = "Fast Throw Players", method =() => ReleasePatch.enabled = true, disableMethod =() => ReleasePatch.enabled = false, toolTip = "Makes players go really fast when you throw them."},

                new ButtonInfo { buttonText = "Noclip Building", method = Fun.NoclipBuilding, disableMethod = Fun.DisableNoclipBuilding, toolTip = "Disables the colliders of every block in the block map."},
                new ButtonInfo { buttonText = "Overlap Building", enableMethod =() => OverlapPatch.enabled = true, disableMethod =() => OverlapPatch.enabled = false, toolTip = "Lets you place pieces inside of each other in the attic."},
                new ButtonInfo { buttonText = "Small Building", enableMethod =() => BuildPatch.enabled = true, disableMethod =() => BuildPatch.enabled = false, toolTip = "Lets you build in the block map while small."},
                new ButtonInfo { buttonText = "Multi Grab", method = Fun.MultiGrab, toolTip = "Lets you grab multiple objects."},

                new ButtonInfo { buttonText = "Block Size Toggle", method = Fun.AtticSizeToggle, toolTip = "Toggles your scale when pressing <color=green>grip</color> or <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Grab All Nearby Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabAllBlocksNearby, toolTip = "Grabs every nearby building block when holding <color=green>G</color>."},
                new ButtonInfo { buttonText = "Grab All Selected Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabAllSelectedNearby, toolTip = "Grabs every nearby building block that matches your selection when holding <color=green>G</color>."},

                new ButtonInfo { buttonText = "Massive Block <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.MassiveBlock, toolTip = "Spawns you a massive block when you press <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Select Block Gun", method = Fun.SelectBlockGun, toolTip = "Selects whatever building block your hand desires to be used for the building mods." },
                new ButtonInfo { buttonText = "Copy Block Info Gun", method = Fun.CopyBlockInfoGun, toolTip = "Copies whatever building block your hand desires to be used for the building mods to your clipboard." },
                new ButtonInfo { buttonText = "Building Block Browser", method = Fun.BlockBrowser, isTogglable = false, toolTip = "Browse through every block that you can spawn and select it." },

                new ButtonInfo { buttonText = "Grab Building Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.SpamGrabBlocks, toolTip = "Forces the building block into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Building Block Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.BuildingBlockMinigun, toolTip = "Spams building blocks out of your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Building Block Gun", method = Fun.BlocksGun, toolTip = "Moves the building blocks to wherever your hand desires." },

                new ButtonInfo { buttonText = "Orbit Building Blocks", method = Fun.OrbitBlocks, toolTip = "Orbits the building blocks around you." },
                new ButtonInfo { buttonText = "Rain Building Blocks", method = Fun.RainBuildingBlocks, toolTip = "Makes the building blocks fall around you like rain." },
                new ButtonInfo { buttonText = "Building Block Fountain", method = Fun.BuildingBlockFountain, toolTip = "Spurts building blocks out of your head like a fountain." },
                new ButtonInfo { buttonText = "Building Block Aura", method = Fun.BuildingBlockAura, toolTip = "Moves the building blocks around you at random positions." },

                new ButtonInfo { buttonText = "Building Block Text Gun", enableMethod =() => PromptText("What text would you like to show?", () => Overpowered.textToRender = keyboardInput.ToUpper(), null, "Done", "Cancel"), method = Fun.BuildingBlockTextGun, toolTip = "Spawns entities in the shape of the text you desire in the ghost reactor."},

                new ButtonInfo { buttonText = "Place Building Block Gun", method = Fun.PlaceBlockGun, toolTip = "Places whatever building block your hand desires on the last grid space you have placed blocks on." },

                new ButtonInfo { buttonText = "Destroy Building Block Gun", method = Fun.DestroyBlockGun, toolTip = "Shreds whatever building block your hand desires." },
                new ButtonInfo { buttonText = "Destroy Building Blocks", overlapText = "Destroy All Building Blocks", method = Fun.DestroyBlocks, toolTip = "Shreds every building block." },

                new ButtonInfo { buttonText = "Save Builder Table Data", method = Fun.SaveBuilderTableData, isTogglable = false, toolTip = "Dumps the data of your current build to a JSON file." },
                new ButtonInfo { buttonText = "Load Builder Table Data", method = Fun.LoadBuilderTableData, isTogglable = false, toolTip = "Loads the data of the dumped JSON files in your game directory and saves it to your current slot." },

                new ButtonInfo { buttonText = "Disable Critters Dome", enableMethod =() => GetObject("Critters/Critters_Environment/Landscape/Critters_Landscape_Dome").SetActive(false), disableMethod =() => GetObject("Critters/Critters_Environment/Landscape/Critters_Landscape_Dome").SetActive(true), toolTip = "Disables the critters dome." },
                new ButtonInfo { buttonText = "Enable Forest Dome", enableMethod =() => GetObject("Environment Objects/LocalObjects_Prefab/Forest/Super Infection Zone - Forest Variant/ForestDome_Prefab").SetActive(true), disableMethod =() => GetObject("Environment Objects/LocalObjects_Prefab/Forest/Super Infection Zone - Forest Variant/ForestDome_Prefab").SetActive(false), toolTip = "Enables the dome in forest." },

                new ButtonInfo { buttonText = "Critter Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.CritterSpam, toolTip = "Spawns critters on your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.CritterMinigun, toolTip = "Shoots critters out of your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Gun", method = Overpowered.CritterGun, toolTip = "Spawns critters at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Sticky Goo Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StickyGoo), toolTip = "Spams sticky goo in your hand when holding <color=green>grip</color>"},

                new ButtonInfo { buttonText = "Critter Food Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.Food), toolTip = "Spams food in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Noise Maker Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.NoiseMaker), toolTip = "Spams noise makers in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Stun Bomb Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StunBomb), toolTip = "Spams stun bombs in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Sticky Trap Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StickyTrap), toolTip = "Spams sticky traps in your hand when holding <color=green>grip</color>"},
  
                new ButtonInfo { buttonText = "Critter Sticky Goo Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StickyGoo), toolTip = "Spams sticky goo at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Food Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.Food), toolTip = "Spams food at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Noise Maker Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.NoiseMaker), toolTip = "Spams noise makers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Stun Bomb Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StunBomb), toolTip = "Spams stun bombs at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Sticky Trap Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StickyTrap), toolTip = "Spams sticky traps at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Shockwave Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.StunExplosion), toolTip = "Spams the shockwave particles in your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Critter Sticky Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.StickyDeployed), toolTip = "Spams the sticky particles in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Eating Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.StickyTriggered), toolTip = "Spams the sticky eating particles in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Noise Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.NoiseMakerTriggered), toolTip = "Spams the noise particles in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Particle Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam((CrittersManager.CritterEvent)Random.Range(0, 4)), toolTip = "Spams every particle in your hand when holding <color=green>grip</color>"},

                new ButtonInfo { buttonText = "Critter Shockwave Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.StunExplosion), toolTip = "Spams the shockwave particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Sticky Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.StickyDeployed), toolTip = "Spams the sticky particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Eating Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.StickyTriggered), toolTip = "Spams the sticky eating particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Noise Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.NoiseMakerTriggered), toolTip = "Spams the noise particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Particle Effect Gun", method =() => Overpowered.EffectGun((CrittersManager.CritterEvent)Random.Range(0, 4)), toolTip = "Spams every particle at wherever your hand desires."},

                new ButtonInfo { buttonText = "Grab ID Card <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabIDCard, toolTip = "Puts the ID card in your hand." },
                new ButtonInfo { buttonText = "Entity Reach", method =() => EntityGrabPatch.enabled = true, disableMethod =() => EntityGrabPatch.enabled = false, toolTip = "Gives you the ability to grab entities from farther away in the horror map." },

                new ButtonInfo { buttonText = "Infinite Prop Distance", method =() => Fun.SetPropDistanceLimit(float.MaxValue), disableMethod =() => Fun.SetPropDistanceLimit(0.35f), toolTip = "Removes the distance limit of props in the prop hunt map." },
                new ButtonInfo { buttonText = "Prop Noclip", method =() => PropPatch.enabled = true, disableMethod =() => PropPatch.enabled = false, toolTip = "Allows you to put props in walls in the prop hunt map." },

                new ButtonInfo { buttonText = "Spaz Tool Stations", method = Fun.SpazToolStations, toolTip = "Spazzes out the tool purchase stations in the horror map." },
                new ButtonInfo { buttonText = "Purchase All Tool Stations", method = Fun.PurchaseAllToolStations, toolTip = "Makes every tool purchase station force purchase in the horror map." },

                new ButtonInfo { buttonText = "Gate Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnergyCostGate"]), toolTip = "Spawns gates out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Core Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorCollectibleCore"]), toolTip = "Spawns collectible cores out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Slime Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorCollectibleSentientCore"]), toolTip = "Spawns sentient collectible cores out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tool Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.ToolSpamGrip, toolTip = "Spawns random tools out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Flower Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorCollectibleFlower"]), toolTip = "Spawns flowers out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Crate Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorBreakableCrate"]), toolTip = "Spawns barrels out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Barrel Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorBreakableBarrel"]), toolTip = "Spawns barrels out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Bug Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyPest"]), toolTip = "Spawns the annoying bug enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Large Bug Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyPestBig"]), toolTip = "Spawns the large annoying bug enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Ranged Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyRanged"]), toolTip = "Spawns ranged enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Chaser Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyChaser"]), toolTip = "Spawns chasing enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Armored Ranged Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyRangedArmored"]), toolTip = "Spawns armored ranged enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Armored Chaser Enemy Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectGrip(Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"]), toolTip = "Spawns armored chasing enemies out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Entity Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SpamEntityGrip, toolTip = "Spawns a random entity out of your hand when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Gate Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnergyCostGate"]), toolTip = "Spawns gates at wherever your hand desires."},
                new ButtonInfo { buttonText = "Core Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorCollectibleCore"]), toolTip = "Spawns collectible cores at wherever your hand desires."},
                new ButtonInfo { buttonText = "Slime Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorCollectibleSentientCore"]), toolTip = "Spawns sentient collectible cores at wherever your hand desires."},
                new ButtonInfo { buttonText = "Tool Gun", method = Overpowered.ToolSpamGun, toolTip = "Spawns random tools at wherever your hand desires."},
                new ButtonInfo { buttonText = "Flower Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorCollectibleFlower"]), toolTip = "Spawns flowers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Crate Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorBreakableCrate"]), toolTip = "Spawns barrels at wherever your hand desires."},
                new ButtonInfo { buttonText = "Barrel Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorBreakableBarrel"]), toolTip = "Spawns barrels at wherever your hand desires."},
                new ButtonInfo { buttonText = "Bug Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyPest"]), toolTip = "Spawns the annoying bug enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Large Bug Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyPestBig"]), toolTip = "Spawns the large annoying bug enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Ranged Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyRanged"]), toolTip = "Spawns ranged enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Chaser Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyChaser"]), toolTip = "Spawns chasing enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Armored Ranged Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyRangedArmored"]), toolTip = "Spawns armored ranged enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Armored Chaser Enemy Gun", method =() => Overpowered.SpamObjectGun(Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"]), toolTip = "Spawns armored chasing enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Entity Gun", method = Overpowered.SpamEntityGun, toolTip = "Spawns a random entity at wherever your hand desires."},

                new ButtonInfo { buttonText = "Rain Entities", method = Overpowered.RainEntities, toolTip = "Makes random entities fall around you like rain."},
                new ButtonInfo { buttonText = "Entity Aura", method = Overpowered.EntityAura, toolTip = "Creates a ball of random entities around you."},
                new ButtonInfo { buttonText = "Entity Fountain", method = Overpowered.EntityFountain, toolTip = "Spurts random entities out of your head like a fountain."},

                new ButtonInfo { buttonText = "Ghost Reactor Text Gun", enableMethod =() => PromptText("What text would you like to show?", () => Overpowered.textToRender = keyboardInput.ToUpper(), null, "Done", "Cancel"), method = Overpowered.GhostReactorTextGun, toolTip = "Spawns entities in the shape of the text you desire in the ghost reactor."},
                new ButtonInfo { buttonText = "Ghost Reactor Draw Gun", method = Overpowered.GhostReactorDrawGun, toolTip = "Allows you to draw with entities in ghost reactor."},
                
                new ButtonInfo { buttonText = "Destroy Entity Gun", method = Overpowered.DestroyEntityGun, toolTip = "Destroys any entity which your hand desires."},

                new ButtonInfo { buttonText = "Infinite Jet Fuel", method =() => FuelPatch.enabled = true, disableMethod =() => FuelPatch.enabled = false, toolTip = "Gives the jet gadgets in Super Infection infinite fuel."},
                new ButtonInfo { buttonText = "Infinite Platforms", method =() => PlatformPatch.enabled = true, disableMethod =() => PlatformPatch.enabled = false, toolTip = "Gives the platform spawner gadgets in Super Infection infinite platforms."},
                new ButtonInfo { buttonText = "Infinite Resources", method = Overpowered.InfiniteResources, toolTip = "Gives you infinite resources in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Complete All Quests", method = Overpowered.CompleteAllQuests, isTogglable = false, toolTip = "Completes every quest in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Claim All Terminals", method = Overpowered.ClaimAllTerminals, isTogglable = false, toolTip = "Claims every terminal in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Unlock All Gadgets", method = Overpowered.UnlockAllGadgets, toolTip = "Unlocks every gadget in the Super Infection gamemode."},

                new ButtonInfo { buttonText = "No Blaster Cooldown", method =() => CooldownPatch.enabled = true, disableMethod =() => CooldownPatch.enabled = false, toolTip = "Removes the cooldown on the blaster."},
                new ButtonInfo { buttonText = "Blaster Aimbot", enableMethod =() => FirePatch.enabled = true, method = Overpowered.DebugBlasterAimbot, disableMethod =() => FirePatch.enabled = false, toolTip = "Automatically aims the blaster towards players."},

                new ButtonInfo { buttonText = "Blaster Laser Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.BlasterLaserSpam, toolTip = "Spams lasers out of your hand when holding <color=green>grip</color>."},
                
                new ButtonInfo { buttonText = "Blaster Float Gun", method =() => Overpowered.BlasterFlingGun(Vector3.up), toolTip = "Uses the blasters to fling whoever your hand desires vertically."},
                new ButtonInfo { buttonText = "Blaster Float All", method =() => Overpowered.BlasterFlingAll(Vector3.up), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the blasters to fling everyone in the room vertically."},

                new ButtonInfo { buttonText = "Blaster Control Gun", method = Overpowered.BlasterControlGun, toolTip = "Uses the blasters to control whoever your hand desires."},

                new ButtonInfo { buttonText = "Blaster Fling Gun", method =() => Overpowered.BlasterFlingGun(RandomVector3()), toolTip = "Uses the blasters to fling whoever your hand desires."},
                new ButtonInfo { buttonText = "Blaster Fling All", method =() => Overpowered.BlasterFlingAll(RandomVector3()), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the blasters to fling everyone in the room."},

                new ButtonInfo { buttonText = "Blaster Fling Towards Gun", overlapText = "Blaster Bring Gun", aliases = new[] { "Blaster Fling Towards Gun" }, method = Overpowered.BlasterFlingTowardsGun, toolTip = "Uses the blasters to fling whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Blaster Fling Towards All", overlapText = "Blaster Bring All", aliases = new[] { "Blaster Fling Towards All" }, method = Overpowered.BlasterFlingTowardsAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the blasters to fling everyone in the room towards you."},

                new ButtonInfo { buttonText = "Blaster Fling Away Gun", overlapText = "Blaster Push Gun", aliases = new[] { "Blaster Fling Towards Gun" }, method = Overpowered.BlasterFlingAwayGun, toolTip = "Uses the blasters to fling whoever your hand desires away from you."},
                new ButtonInfo { buttonText = "Blaster Fling Away All", overlapText = "Blaster Push All", aliases = new[] { "Blaster Fling Towards All" }, method = Overpowered.BlasterFlingAwayAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Uses the blasters to fling everyone in the room away from you."},

                new ButtonInfo { buttonText = "Blaster Kick Gun", method = Overpowered.BlasterKickGun, toolTip = "Kicks whoever your hand desires using the blasters." },
                new ButtonInfo { buttonText = "Blaster Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BlasterKickAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Kicks everyone in the room when holding <color=green>trigger</color> using the blasters." },

                new ButtonInfo { buttonText = "Blaster Crash Gun", method = Overpowered.BlasterCrashGun, toolTip = "Crashes whoever your hand desires using the blasters." },
                new ButtonInfo { buttonText = "Blaster Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BlasterCrashAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Crashes everyone in the room when holding <color=green>trigger</color> using the blasters." },

                new ButtonInfo { buttonText = "Stilt Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["StiltGadget FixedScaledLong"]), toolTip = "Spawns stilts out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Turkey Stilt Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["StiltGadget Turkey"]), toolTip = "Spawns stilts out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Motorized Stilt Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["StiltGadget Motorized3"]), toolTip = "Spawns motorized stilts out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Thruster Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["WristJetGadgetPropellor"]), toolTip = "Spawns thrusters out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Yoyo Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["SIGadgetDashYoyo"]), toolTip = "Spawns yoyos out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Blaster Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["MegaChargeBlasterGadget"]), toolTip = "Spawns blasters out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Lobber Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["BlastLobberGadget"]), toolTip = "Spawns lobbers out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Strider Tentacle Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["TentacleArmGadget_Strider"]), toolTip = "Spawns tentacles out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Crawler Tentacle Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["TentacleArmGadget_Crawler"]), toolTip = "Spawns tentacles out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Platform Deployer Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamGadgetGrip(Overpowered.GadgetByName["PlatformDeployerGadget"]), toolTip = "Spawns platform deployers out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Gadget Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.GadgetSpamGrip, toolTip = "Spawns random gadgets out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Resource Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.ResourceSpamGrip, toolTip = "Spawns random resources out of your hand when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Stilt Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["StiltGadget FixedScaledLong"]), toolTip = "Spawns stilts at wherever your hand desires."},
                new ButtonInfo { buttonText = "Turkey Stilt Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["StiltGadget Turkey"]), toolTip = "Spawns stilts at wherever your hand desires."},
                new ButtonInfo { buttonText = "Motorized Stilt Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["StiltGadget Motorized3"]), toolTip = "Spawns motorized stilts at wherever your hand desires."},
                new ButtonInfo { buttonText = "Thruster Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["WristJetGadgetPropellor"]), toolTip = "Spawns thrusters at wherever your hand desires."},
                new ButtonInfo { buttonText = "Yoyo Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["SIGadgetDashYoyo"]), toolTip = "Spawns yoyos at wherever your hand desires."},
                new ButtonInfo { buttonText = "Blaster Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["MegaChargeBlasterGadget"]), toolTip = "Spawns blasters at wherever your hand desires."},
                new ButtonInfo { buttonText = "Lobber Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["BlastLobberGadget"]), toolTip = "Spawns lobbers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Strider Tentacle Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["TentacleArmGadget_Strider"]), toolTip = "Spawns tentacles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Crawler Tentacle Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["TentacleArmGadget_Crawler"]), toolTip = "Spawns tentacles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Platform Deployer Gun", method =() => Overpowered.SpamGadgetGun(Overpowered.GadgetByName["PlatformDeployerGadget"]), toolTip = "Spawns platform deployers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Gadget Gun", method = Overpowered.GadgetSpamGun, toolTip = "Spawns random gadgets at wherever your hand desires."},
                new ButtonInfo { buttonText = "Resource Gun", method = Overpowered.ResourceSpamGun, toolTip = "Spawns random resources at wherever your hand desires."},

                new ButtonInfo { buttonText = "Rain Gadgets", method = Overpowered.RainGadgets, toolTip = "Makes random gadgets fall around you like rain."},
                new ButtonInfo { buttonText = "Gadget Aura", method = Overpowered.GadgetAura, toolTip = "Creates a ball of random gadgets around you."},
                new ButtonInfo { buttonText = "Gadget Fountain", method = Overpowered.GadgetFountain, toolTip = "Spurts random gadgets out of your head like a fountain."},

                new ButtonInfo { buttonText = "Super Infection Text Gun", enableMethod =() => PromptText("What text would you like to show?", () => Overpowered.textToRender = keyboardInput.ToUpper(), null, "Done", "Cancel"), method = Overpowered.SuperInfectionTextGun, toolTip = "Spawns entities in the shape of the text you desire in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Super Infection Draw Gun", method = Overpowered.SuperInfectionDrawGun, toolTip = "Allows you to draw with entities in Super Infection."},

                new ButtonInfo { buttonText = "Destroy Gadget Gun", method = Overpowered.DestroyGadgetGun, toolTip = "Destroys any gadget which your hand desires."},

                new ButtonInfo { buttonText = "Fire Sound Spam <color=grey>[</color><color=green>T</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedCosmetic("LBALH."), method = Fun.FireSoundSpam, toolTip = "Spams fire sounds when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Bubble Gun", enableMethod =() => Fun.CheckOwnedThrowable(33), method =() => Fun.BubblerGun(33, Quaternion.identity, 0.1f), toolTip = "Uses the bubbler to spawn bubbles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Fire Gun", enableMethod =() => Fun.CheckOwnedThrowable(175), method =() => Fun.BubblerGun(175, Quaternion.Euler(180f, 0f, 0f)), toolTip = "Uses the bubbler to spawn bubbles at wherever your hand desires."},

                new ButtonInfo { buttonText = "White Color Gun", enableMethod =() => Fun.CheckOwnedThrowable(629), method = Fun.WhiteColorGun, toolTip = "Sprays whoever your hand desires with the sunblock spray cosmetic."},
                new ButtonInfo { buttonText = "White Color All", enableMethod =() => Fun.CheckOwnedThrowable(629), method =() => Fun.WhiteColorTarget(GetTargetPlayer()), disableMethod =() => VRRig.LocalRig.enabled = false, toolTip = "Sprays everyone in the room with the sunblock spray cosmetic."},

                new ButtonInfo { buttonText = "Black Color Gun", enableMethod =() => Fun.CheckOwnedThrowable(600), method = Fun.BlackColorGun, toolTip = "Uses the smoke bomb to make whoever your hand desires black."},
                new ButtonInfo { buttonText = "Black Color All", enableMethod =() => Fun.CheckOwnedThrowable(600), method =() => Fun.BlackColorTarget(GetTargetPlayer()), disableMethod =() => VRRig.LocalRig.enabled = false, toolTip = "Uses the smoke bomb to make everyone in the room black."},

                new ButtonInfo { buttonText = "Chicken Gun", enableMethod =() => Fun.CheckOwnedThrowable(651), method = Fun.ChickenGun, toolTip = "Uses the smoke bomb to make whoever your hand desires black."},
                new ButtonInfo { buttonText = "Chicken All", enableMethod =() => Fun.CheckOwnedThrowable(651), method =() => Fun.ChickenTarget(GetTargetPlayer()), disableMethod =() => VRRig.LocalRig.enabled = false, toolTip = "Uses the smoke bomb to make everyone in the room black."},

                new ButtonInfo { buttonText = "Whoopee Cushion Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(626), method =() => Fun.ThrowableProjectileSpam(626), toolTip = "Spawns whoopee cushions on your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Whoopee Cushion Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(626), method =() => Fun.ThrowableProjectileMinigun(626), toolTip = "Shoots whoopee cushions out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Whoopee Cushion Gun", enableMethod =() => Fun.CheckOwnedThrowable(626), method =() => Fun.ThrowableProjectileGun(626), toolTip = "Spawns whoopee cushions at wherever your hand desires."},

                new ButtonInfo { buttonText = "Smoke Bomb Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(600), method =() => Fun.ThrowableProjectileSpam(600), toolTip = "Spawns smoke bombs on your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Smoke Bomb Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(600), method =() => Fun.ThrowableProjectileMinigun(600), toolTip = "Shoots smoke bombs out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Smoke Bomb Gun", enableMethod =() => Fun.CheckOwnedThrowable(600), method =() => Fun.ThrowableProjectileGun(600), toolTip = "Spawns smoke bombs at wherever your hand desires."},

                new ButtonInfo { buttonText = "Firecracker Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(587), method =() => Fun.ThrowableProjectileSpam(587), toolTip = "Spawns firecrackers on your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Firecracker Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => Fun.CheckOwnedThrowable(587), method =() => Fun.ThrowableProjectileMinigun(587), toolTip = "Shoots firecrackers out of your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Firecracker Gun", enableMethod =() => Fun.CheckOwnedThrowable(587), method =() => Fun.ThrowableProjectileGun(587), toolTip = "Spawns firecrackers at wherever your hand desires."},

                new ButtonInfo { buttonText = "Spaz All Moles", method = Fun.SpazMoleMachines, toolTip = "Gives the moles a seizure."},
                new ButtonInfo { buttonText = "Auto Start Moles", method = Fun.AutoStartMoles, toolTip = "Automatically starts the mole games."},
                new ButtonInfo { buttonText = "Auto Hit Moles", method =() => Fun.AutoHitMoleType(false), toolTip = "Hits all of the moles automatically."},
                new ButtonInfo { buttonText = "Auto Hit Hazards", method =() => Fun.AutoHitMoleType(true), toolTip = "Hits all of the hazards automatically."},

                new ButtonInfo { buttonText = "No Respawn Bug", enableMethod =() => Fun.SetRespawnDistance("Floating Bug Holdable"), disableMethod =() => Fun.SetRespawnDistance("Floating Bug Holdable", 50f), toolTip = "Doesn't respawn the bug if it goes too far outside the bounds of forest."},
                new ButtonInfo { buttonText = "No Respawn Bat", enableMethod =() => Fun.SetRespawnDistance("Cave Bat Holdable"), disableMethod =() => Fun.SetRespawnDistance("Cave Bat Holdable", 50f), toolTip = "Doesn't respawn the bat if it goes too far outside the bounds of caves."},
                new ButtonInfo { buttonText = "No Respawn Firefly", enableMethod =() => Fun.SetRespawnDistance("Firefly"), disableMethod =() => Fun.SetRespawnDistance("Firefly", 50f), toolTip = "Doesn't respawn the firefly if it goes too far outside the bounds of forest."},

                new ButtonInfo { buttonText = "Permanent Bug", method =() => Fun.PermanentOwnership("Floating Bug Holdable"), disableMethod =() => OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the bug."},
                new ButtonInfo { buttonText = "Permanent Bat", method =() => Fun.PermanentOwnership("Cave Bat Holdable"), disableMethod =() => OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the bat."},
                new ButtonInfo { buttonText = "Permanent Firefly", method =() => Fun.PermanentOwnership("Firefly"), disableMethod =() => OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the firefly."},
 
                new ButtonInfo { buttonText = "Bug Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.BugSpam, disableMethod = Fun.DisableBugSpam, toolTip = "Shoots the bug and firefly out of your hand repeatedly." },
                new ButtonInfo { buttonText = "Camera Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.CameraSpam, disableMethod = Fun.DisableCameraSpam, toolTip = "Shoots the camera out of your hand repeatedly." },
                new ButtonInfo { buttonText = "Everything Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.EverythingSpam, disableMethod = Fun.DisableEverythingSpam, toolTip = "Shoots everything out of your hand repeatedly." },

                new ButtonInfo { buttonText = "Bug Phallus", method = Fun.BugPhallus, toolTip = "Gives you a phallus in the form of the bugs." },
                new ButtonInfo { buttonText = "Bug Phallus Gun", method = Fun.BugPhallusGun, toolTip = "Gives whoever your hand desires a phallus in the form of the bugs." },

                new ButtonInfo { buttonText = "Bug Vibrate Gun", method = Fun.BugVibrateGun, toolTip = "Vibrates the controllers of whoever your hand desires using the bugs." },
                new ButtonInfo { buttonText = "Bug Vibrate All", enableMethod = Fun.EnableBugVibrateAll, method = Fun.BugVibrateAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Vibrates the controllers of everyone in the room using the bugs." },

                new ButtonInfo { buttonText = "Holster Bug", method =() => Fun.HolsterObject("Floating Bug Holdable", TransferrableObject.PositionState.OnLeftArm), toolTip = "Holsters the bug on your left arm." },
                new ButtonInfo { buttonText = "Holster Bat", method =() => Fun.HolsterObject("Cave Bat Holdable", TransferrableObject.PositionState.OnChest), toolTip = "Holsters the bat on your chest." },
                new ButtonInfo { buttonText = "Holster Firefly", method =() => Fun.HolsterObject("Firefly", TransferrableObject.PositionState.OnRightArm), toolTip = "Holsters the firefly on your right arm." },

                new ButtonInfo { buttonText = "Freeze Bug", method =() => Fun.FreezeObject("Floating Bug Holdable"), toolTip = "Freezes the bug in place." },
                new ButtonInfo { buttonText = "Freeze Bat", method =() => Fun.FreezeObject("Cave Bat Holdable"), toolTip = "Freezes the bat in place." },
                new ButtonInfo { buttonText = "Freeze Firefly", method =() => Fun.FreezeObject("Firefly"), toolTip = "Freezes the firefly in place." },

                new ButtonInfo { buttonText = "Fast Bug", method =() => Fun.SetObjectSpeed("Floating Bug Holdable", 5f), disableMethod =() => Fun.SetObjectSpeed("Floating Bug Holdable"), toolTip = "Speeds up the bug." },
                new ButtonInfo { buttonText = "Fast Bat", method =() => Fun.SetObjectSpeed("Cave Bat Holdable", 5f), disableMethod =() => Fun.SetObjectSpeed("Cave Bat Holdable"), toolTip = "Speeds up the bat." },
                new ButtonInfo { buttonText = "Fast Firefly", method =() => Fun.SetObjectSpeed("Firefly", 5f), disableMethod =() => Fun.SetObjectSpeed("Firefly"), toolTip = "Speeds up the firefly." },

                new ButtonInfo { buttonText = "Slow Bug", method =() => Fun.SetObjectSpeed("Floating Bug Holdable", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Floating Bug Holdable"), toolTip = "Slows down the bug." },
                new ButtonInfo { buttonText = "Slow Bat", method =() => Fun.SetObjectSpeed("Cave Bat Holdable", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Cave Bat Holdable"), toolTip = "Slows down the bat." },
                new ButtonInfo { buttonText = "Slow Firefly", method =() => Fun.SetObjectSpeed("Firefly", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Firefly"), toolTip = "Slows down the firefly." },

                new ButtonInfo { buttonText = "Physical Bug", method =() => Fun.PhysicalObject("Floating Bug Holdable"), toolTip = "Gives the bug physics, letting you grab onto it and throw it." },
                new ButtonInfo { buttonText = "Physical Bat", method =() => Fun.PhysicalObject("Cave Bat Holdable"), toolTip = "Gives the bat physics, letting you grab onto it and throw it." },
                new ButtonInfo { buttonText = "Physical Firefly", method =() => Fun.PhysicalObject("Firefly"), toolTip = "Gives the firefly physics, letting you grab onto it and throw it." },
                new ButtonInfo { buttonText = "Physical Camera", method = Fun.PhysicalCamera, toolTip = "Gives the camera physics, letting you grab onto it and throw it." },

                new ButtonInfo { buttonText = "Grab Bug <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Floating Bug Holdable"), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Bat <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Cave Bat Holdable"), toolTip = "Forces the bat into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Firefly <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Firefly"), toolTip = "Forces the firefly into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Camera <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabCamera, toolTip = "Forces the camera into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Tablet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabTablet, toolTip = "Forces the tablet into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Balloons <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabBalloons, toolTip = "Forces every single balloon cosmetic into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Gliders <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabGliders, toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Bug Gun", method =() => Fun.ObjectToPointGun("Floating Bug Holdable"), toolTip = "Moves the bug to wherever your hand desires." },
                new ButtonInfo { buttonText = "Bat Gun", method =() => Fun.ObjectToPointGun("Cave Bat Holdable"), toolTip = "Moves the bat to wherever your hand desires." },
                new ButtonInfo { buttonText = "Firefly Gun", method =() => Fun.ObjectToPointGun("Firefly"), toolTip = "Moves the firefly to wherever your hand desires." },
                new ButtonInfo { buttonText = "Camera Gun", method = Fun.CameraGun, toolTip = "Moves the camera to wherever your hand desires." },
                new ButtonInfo { buttonText = "Tablet Gun", method = Fun.TabletGun, toolTip = "Moves the tablet to wherever your hand desires." },
                new ButtonInfo { buttonText = "Balloon Gun", method = Fun.BalloonGun, toolTip = "Moves every single balloon cosmetic to wherever your hand desires." },
                new ButtonInfo { buttonText = "Glider Gun", method = Fun.GliderGun, toolTip = "Moves the gliders to wherever your hand desires." },
                new ButtonInfo { buttonText = "Hoverboard Gun", method = Fun.HoverboardGun, toolTip = "Spawns hoverboards at wherever your hand desires."},

                new ButtonInfo { buttonText = "Spaz Bug", method =() => Fun.SpazObject("Floating Bug Holdable"), toolTip = "Gives the bug a seizure." },
                new ButtonInfo { buttonText = "Spaz Bat", method =() => Fun.SpazObject("Cave Bat Holdable"), toolTip = "Gives the bat a seizure." },
                new ButtonInfo { buttonText = "Spaz Firefly", method =() => Fun.SpazObject("Firefly"), toolTip = "Gives the firefly a seizure." },
                new ButtonInfo { buttonText = "Spaz Camera", method = Fun.SpazCamera, toolTip = "Gives the camera a seizure." },
                new ButtonInfo { buttonText = "Spaz Tablet", method = Fun.SpazTablet, toolTip = "Gives the tablet a seizure." },
                new ButtonInfo { buttonText = "Spaz Balloons", method = Fun.SpazBalloons, toolTip = "Gives the balloons a seizure." },
                new ButtonInfo { buttonText = "Spaz Gliders", method = Fun.SpazGliders, toolTip = "Gives the gliders a seizure." },
                new ButtonInfo { buttonText = "Spaz Hoverboard", method = Fun.SpazHoverboard, toolTip = "Gives your hoverboard a seizure while holding it."},

                new ButtonInfo { buttonText = "Orbit Bug", method =() => Fun.OrbitObject("Floating Bug Holdable"), toolTip = "Orbits the bug around you." },
                new ButtonInfo { buttonText = "Orbit Bat", method =() => Fun.OrbitObject("Cave Bat Holdable", 240f), toolTip = "Orbits the bat around you." },
                new ButtonInfo { buttonText = "Orbit Firefly", method =() => Fun.OrbitObject("Firefly", 120f), toolTip = "Orbits the firefly around you." },
                new ButtonInfo { buttonText = "Orbit Camera", method = Fun.OrbitCamera, toolTip = "Orbits the camera around you." },
                new ButtonInfo { buttonText = "Orbit Tablet", method = Fun.OrbitTablet, toolTip = "Orbits the tablet around you." },
                new ButtonInfo { buttonText = "Orbit Balloons", method = Fun.OrbitBalloons, toolTip = "Orbits the balloons around you." },
                new ButtonInfo { buttonText = "Orbit Gliders", method = Fun.OrbitGliders, toolTip = "Orbits the gliders around you." },
                new ButtonInfo { buttonText = "Orbit Hoverboards", method = Fun.OrbitHoverboards, toolTip = "Orbits the hoverboards around you."},

                new ButtonInfo { buttonText = "Bug Aura", method =() => Fun.ObjectAura("Floating Bug Holdable"), toolTip = "Teleports the bug around you in random positions." },
                new ButtonInfo { buttonText = "Bat Aura", method =() => Fun.ObjectAura("Cave Bat Holdable"), toolTip = "Teleports the bat around you in random positions." },
                new ButtonInfo { buttonText = "Firefly Aura", method =() => Fun.ObjectAura("Firefly"), toolTip = "Teleports the firefly around you in random positions." },
                new ButtonInfo { buttonText = "Camera Aura", method = Fun.CameraAura, toolTip = "Teleports the camera around you in random positions." },
                new ButtonInfo { buttonText = "Tablet Aura", method = Fun.TabletAura, toolTip = "Teleports the tablet around you in random positions." },
                new ButtonInfo { buttonText = "Balloon Aura", method = Fun.BalloonAura, toolTip = "Teleports the balloons around you in random positions." },
                new ButtonInfo { buttonText = "Glider Aura", method = Fun.GliderAura, toolTip = "Teleports the camera around you in random positions." },
                new ButtonInfo { buttonText = "Hoverboard Aura", method = Fun.HoverboardAura, toolTip = "Teleports the hoverboards around you in random positions."},

                new ButtonInfo { buttonText = "Ride Bug", method =() => Fun.RideObject("Floating Bug Holdable"), toolTip = "Repeatedly teleports you on top of the bug." },
                new ButtonInfo { buttonText = "Ride Bat", method =() => Fun.RideObject("Cave Bat Holdable"), toolTip = "Repeatedly teleports you on top of the bat." },
                new ButtonInfo { buttonText = "Ride Firefly", method =() => Fun.RideObject("Firefly"), toolTip = "Repeatedly teleports you on top of the firefly." },

                new ButtonInfo { buttonText = "Become Bug", method =() => Fun.BecomeObject("Floating Bug Holdable"), disableMethod = Movement.EnableRig, toolTip = "Turns you into the bug." },
                new ButtonInfo { buttonText = "Become Bat", method =() => Fun.BecomeObject("Cave Bat Holdable"), disableMethod = Movement.EnableRig, toolTip = "Turns you into the bat." },
                new ButtonInfo { buttonText = "Become Firefly", method =() => Fun.BecomeObject("Firefly"), disableMethod = Movement.EnableRig, toolTip = "Turns you into the firefly." },
                new ButtonInfo { buttonText = "Become Camera", method = Fun.BecomeCamera, disableMethod = Movement.EnableRig, toolTip = "Turns you into the camera." },
                new ButtonInfo { buttonText = "Become Tablet", method = Fun.BecomeTablet, disableMethod = Movement.EnableRig, toolTip = "Turns you into the tablet." },
                new ButtonInfo { buttonText = "Become Balloon", method = Fun.BecomeBalloon, disableMethod = Movement.EnableRig, toolTip = "Turns you into a balloon when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Become Hoverboard", method = Fun.BecomeHoverboard, disableMethod = Movement.EnableRig, toolTip = "Turns you into a hoverboard when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Destroy Bug", method =() => Fun.DestroyObject("Floating Bug Holdable"), toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Firefly", method =() => Fun.DestroyObject("Firefly"), toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Bat", method =() => Fun.DestroyObject("Cave Bat Holdable"), toolTip = "Sends the bat to hell." },
                new ButtonInfo { buttonText = "Destroy Camera", method = Fun.DestroyCamera, toolTip = "Sends the camera to hell." },
                new ButtonInfo { buttonText = "Destroy Tablet", method = Fun.DestroyTablet, toolTip = "Sends the tablet to hell." },
                new ButtonInfo { buttonText = "Destroy Balloons", method = Fun.DestroyBalloons, isTogglable = false, toolTip = "Sends every single balloon cosmetic to hell." },
                new ButtonInfo { buttonText = "Destroy Gliders", method = Fun.DestroyGliders, isTogglable = false, toolTip = "Sends every single glider to hell." },

                new ButtonInfo { buttonText = "Respawn Gliders", method = Fun.RespawnGliders, isTogglable = false, toolTip = "Respawns all the gliders." },
                new ButtonInfo { buttonText = "Pop All Balloons", method = Fun.PopAllBalloons, isTogglable = false, toolTip = "Pops every single balloon cosmetic." },

                new ButtonInfo { buttonText = "Golden Name Tag", method =() => Fun.GoldenNameTag(true), disableMethod =() => Fun.GoldenNameTag(false), toolTip = "Changes your name tag to a golden color. This mod only works if you are subscribed to the fan club." },
                new ButtonInfo { buttonText = "Flash Name Tag", method = Fun.FlashNameTag, toolTip = "Flashes your name tag to between golden and white. This mod only works if you are subscribed to the fan club." },

                new ButtonInfo { buttonText = "Set Name to \"STATUE\"", method =() => ChangeName("STATUE"), isTogglable = false, toolTip = "Sets your name to \"STATUE\"." },
                new ButtonInfo { buttonText = "Set Name to \"HIDE\"", method =() => ChangeName("HIDE"), isTogglable = false, toolTip = "Sets your name to \"HIDE\"." },
                new ButtonInfo { buttonText = "Set Name to \"RUN\"", method =() => ChangeName("RUN"), isTogglable = false, toolTip = "Sets your name to \"RUN\"." },
                new ButtonInfo { buttonText = "Set Name to \"BEHINDYOU\"", method =() => ChangeName("BEHINDYOU"), isTogglable = false, toolTip = "Sets your name to \"BEHINDYOU\"." },
                new ButtonInfo { buttonText = "Set Name to \"iiOnTop\"", method =() => ChangeName("iiOnTop"), isTogglable = false, toolTip = "Sets your name to \"iiOnTop\"." },

                new ButtonInfo { buttonText = "PBBV Name Cycle", method =() => Fun.NameCycle(new[] { "PPBV", "IS", "HERE" }), toolTip = "Sets your name on a loop to \"PBBV\", \"IS\", and \"HERE\"." },
                new ButtonInfo { buttonText = "J3VU Name Cycle", method =() => Fun.NameCycle(new[] { "J3VU", "HAS", "BECOME", "HOSTILE" }), toolTip = "Sets your name on a loop to \"J3VU\", \"HAS\", \"BECOME\", and \"HOSTILE\"" },
                new ButtonInfo { buttonText = "H1D3 Name Cycle", method =() => Fun.NameCycle(new[] { "H1D3", "HE", "HAS", "AWOKEN" }), toolTip = "Sets your name on a loop to \"H1D3\", \"HE\", \"HAS\", and \"AWOKEN\"" },
                new ButtonInfo { buttonText = "No Escape Name Cycle", method =() => Fun.NameCycle(new[] { "THERE", "IS", "NO", "ESCAPE" }), toolTip = "Sets your name on a loop to \"THERE\", \"IS\", \"NO\", and \"ESCAPE\"" },
                new ButtonInfo { buttonText = "Run Rabbit Name Cycle", method =() => Fun.NameCycle(new[] { "RUN", "RABBIT" }), toolTip = "Sets your name on a loop to \"RUN\" and \"RABBIT\"." },
                new ButtonInfo { buttonText = "Random Name Cycle", method = Fun.RandomNameCycle, toolTip = "Sets your name on a loop to a bunch of random characters." },
                new ButtonInfo { buttonText = "Custom Name Cycle", enableMethod = Fun.EnableCustomNameCycle, method =() => Fun.NameCycle(Fun.names), toolTip = "Sets your name on a loop to whatever's in the file." },
                new ButtonInfo { buttonText = "Animated Name", method = Fun.AnimatedName, disableMethod =() => { ChangeName(Fun.name); Fun.name = null;  }, toolTip = "Animates your current username." },

                new ButtonInfo { buttonText = "Strobe Color", overlapText = "Flash Color", method = Fun.FlashColor, toolTip = "Makes your character flash." },
                new ButtonInfo { buttonText = "Strobe Color", method = Fun.StrobeColor, toolTip = "Makes your character random colors." },
                new ButtonInfo { buttonText = "Rainbow Color", method = Fun.RainbowColor, toolTip = "Makes your character rainbow." },
                new ButtonInfo { buttonText = "Hard Rainbow Color", method = Fun.HardRainbowColor, toolTip = "Makes your character flash from red, green, blue, and magenta." },

                new ButtonInfo { buttonText = "Become \"goldentrophy\"", method =() => Fun.BecomePlayer("goldentrophy", new Color32(255, 128, 0, 255)), isTogglable = false, toolTip = "Sets your name to \"goldentrophy\" and color to orange." },
                new ButtonInfo { buttonText = "Become \"NOESCAPE\"", method =() => Fun.BecomePlayer("NOESCAPE", Color.black), isTogglable = false, toolTip = "Sets your name to \"NOESCAPE\" and color to black." },
                new ButtonInfo { buttonText = "Become \"H1D3\"", method =() => Fun.BecomePlayer("H1D3", Color.black), isTogglable = false, toolTip = "Sets your name to \"H1D3\" and color to black." },
                new ButtonInfo { buttonText = "Become \"PBBV\"", method =() => Fun.BecomePlayer("PBBV", new Color32(230, 127, 102, 255)), isTogglable = false, toolTip = "Sets your name to \"PBBV\" and color to salmon." },
                new ButtonInfo { buttonText = "Become \"J3VU\"", method =() => Fun.BecomePlayer("J3VU", Color.green), isTogglable = false, toolTip = "Sets your name to \"J3VU\" and color to green." },
                new ButtonInfo { buttonText = "Become \"ECHO\"", method =() => Fun.BecomePlayer("ECHO", new Color32(0, 150, 255, 255)), isTogglable = false, toolTip = "Sets your name to \"ECHO\" and color to sky blue." },
                new ButtonInfo { buttonText = "Become \"DAISY09\"", method =() => Fun.BecomePlayer("DAISY09", new Color32(255, 81, 231, 255)), isTogglable = false, toolTip = "Sets your name to \"DAISY09\" and color to a light pink." },
                new ButtonInfo { buttonText = "Become \"STATUE\"", method =() => Fun.BecomePlayer("STATUE", Color.black), isTogglable = false, toolTip = "Sets your name to \"STATUE\" and color to black." },
                new ButtonInfo { buttonText = "Become Child", method = Fun.BecomeMinigamesKid, isTogglable = false, toolTip = "Sets your name and color to something a child would pick." },

                new ButtonInfo { buttonText = "Become Hidden on Leaderboard", method =() => Fun.BecomePlayer("I", new Color32(0, 53, 2, 255)), isTogglable = false, toolTip = "Sets your name to \"I\" and your color to a dark green, matching the leaderboard." },
                new ButtonInfo { buttonText = "Copy Identity Gun", method = Fun.CopyIdentityGun, toolTip = "Steals the identity of whoever your hand desires." },
                new ButtonInfo { buttonText = "Copy Cosmetics Gun", method = Fun.CopyCosmeticsGun, toolTip = "Steals the cosmetics of whoever your hand desires." },

                new ButtonInfo { buttonText = "Change Accessories", overlapText = "Change Cosmetics", method = Fun.ChangeAccessories, toolTip = "Use your grips to change what hat you're wearing." },
                new ButtonInfo { buttonText = "Spaz Accessories", overlapText = "Spaz Cosmetics <color=grey>[</color><color=green>All</color><color=grey>]</color>", method = Fun.SpazAccessories, toolTip = "Spazzes your hats out for everyone when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Spaz Cosmetics <color=grey>[</color><color=green>Others</color><color=grey>]</color>", method = Fun.SpazAccessoriesOthers, toolTip = "Spazzes your hats out for everyone except you when holding <color=green>trigger</color>." },
                
                new ButtonInfo { buttonText = "Spaz Balloon Cosmetics <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Fun.SpazAccessoriesBalloon, toolTip = "Spazzes your balloons out for everyone when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Cosmetic Spoof", enableMethod = Fun.TryOnAnywhere, disableMethod = Fun.TryOffAnywhere, toolTip = "Lets you try on cosmetics from anywhere. Enable this mod after wearing the cosmetics." },
                new ButtonInfo { buttonText = "Cosmetic Browser", method = Fun.CosmeticBrowser, isTogglable = false, toolTip = "Browse through every cosmetic that you can try on and add it to your cart." },
                new ButtonInfo { buttonText = "Auto Spoof Cosmetics", enableMethod = Fun.AutoLoadCosmetics, disableMethod = Fun.NoAutoLoadCosmetics, toolTip = "Automatically spoofs your cosmetics, making you appear with anything you're able to try-on." },

                new ButtonInfo { buttonText = "Auto Purchase Cosmetics", overlapText = "Auto Purchase Free Cosmetics", method = Fun.AutoPurchaseCosmetics, toolTip = "Automatically purchases any free cosmetics." },
                new ButtonInfo { buttonText = "Auto Purchase Current Cosmetics", method = Fun.AutoPurchasePaidCosmetics, toolTip = "Automatically purchases all cosmetics on your outfit until you own everything. This does use shiny rocks." },
                new ButtonInfo { buttonText = "Disable Cosmetics on Tag", method = Fun.DisableCosmeticsOnTag, toolTip = "Disables your cosmetics when you get tagged, good for ambush." },

                new ButtonInfo { buttonText = "Unlock Fan Club Subscription", enableMethod =() => SubscriptionPatches.enabled = true, disableMethod =() => SubscriptionPatches.enabled = false, toolTip = "Unlocks the Gorilla Tag fan club subscription." },
                new ButtonInfo { buttonText = "Unlock All Cosmetics", method = Fun.UnlockAllCosmetics, toolTip = "Unlocks every cosmetic in the game. This mod is client-sided." },
                new ButtonInfo { buttonText = "Unlimited Shiny Rocks", enableMethod =() => PurchasePatch.enabled = true, method =() => CosmeticsController.instance.currencyBalance = int.MaxValue, disableMethod =() => PurchasePatch.enabled = false, toolTip = "Gives you 2 billion shiny rocks. This mod is client sided." },

                new ButtonInfo { buttonText = "Sticky Holdables", method = Fun.StickyHoldables, toolTip = "Makes your holdables sticky." },
                new ButtonInfo { buttonText = "Spaz Holdables", method = Fun.SpazHoldables, toolTip = "Spazzes out the positions of your holdables." },

                new ButtonInfo { buttonText = "Get ID Self", method = Fun.CopySelfID, isTogglable = false, toolTip = "Gets your player ID and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get ID Gun", method = Fun.CopyIDGun, toolTip = "Gets the player ID of whoever your hand desires and copies it to the clipboard." },
                new ButtonInfo { buttonText = "Get ID All", method = Fun.CopyIDAll, isTogglable = false, toolTip = "Gets the player IDs of everyone and copies them to the clipboard." },
                new ButtonInfo { buttonText = "Get ID Aura", method = Fun.CopyIDAura, toolTip = "Gets the player ID of players nearby you and copies it to the clipboard." },
                new ButtonInfo { buttonText = "Get ID On Touch", method = Fun.CopyIDOnTouch, toolTip = "Gets the player ID of players you touch and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Narrate ID Self", method = Fun.NarrateSelfID, isTogglable = false, toolTip = "Gets your player ID and speaks it through your microphone."},
                new ButtonInfo { buttonText = "Narrate ID Gun", method = Fun.NarrateIDGun, toolTip = "Gets the player ID of whoever your hand desires and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate ID All", method = Fun.NarrateIDAll, isTogglable = false, toolTip = "Gets the player IDs of everyone and speaks them through your microphone." },
                new ButtonInfo { buttonText = "Narrate ID Aura", method = Fun.NarrateIDAura, toolTip = "Gets the player ID of players nearby you and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate ID On Touch", method = Fun.NarrateIDOnTouch, toolTip = "Gets the player ID of players you touch and speaks it through your microphone." },

                new ButtonInfo { buttonText = "Narrate Fake IP Self", method = Fun.NarrateFakeDoxxSelf, isTogglable = false, toolTip = "Gets random numbers that look like an IP address and speaks it through your microphone."},
                new ButtonInfo { buttonText = "Narrate Fake IP Gun", method = Fun.NarrateFakeDoxxGun, toolTip = "Gets random numbers that look like an IP address and speaks it through your microphone towards whoever your hand desires." }, 
                new ButtonInfo { buttonText = "Narrate Fake IP All", method = Fun.NarrateFakeDoxxAll, isTogglable = false, toolTip = "Gets random numbers that look like an IP address for everyone and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Fake IP Aura", method = Fun.NarrateFakeDoxxAura, toolTip = "Gets random numbers that look like an IP address and speaks it through your microphone." }, 
                new ButtonInfo { buttonText = "Narrate Fake IP On Touch", method = Fun.NarrateFakeDoxxOnTouch, toolTip = "Gets random numbers that look like an IP address and speaks it through your microphone." }, 

                new ButtonInfo { buttonText = "Get Creation Date Self", method = Fun.CopyCreationDateSelf, isTogglable = false, toolTip = "Gets the creation date of your account and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get Creation Date Gun", method = Fun.CopyCreationDateGun, toolTip = "Gets the creation date of whoever your hand desires' account and copies it to the clipboard." },
                new ButtonInfo { buttonText = "Get Creation Date All", method = Fun.CopyCreationDateAll, isTogglable = false, toolTip = "Gets the creation date of everyones account and copies it to the clipboard." },
                new ButtonInfo { buttonText = "Get Creation Date Aura", method = Fun.CopyCreationDateAura, toolTip = "Gets the creation date of nearby players accounts and copies it to the clipboard." },
                new ButtonInfo { buttonText = "Get Creation Date On Touch", method = Fun.CopyCreationDateOnTouch, toolTip = "Gets the creation date of players you touch accounts and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Narrate Creation Date Self", method = Fun.NarrateCreationDateSelf, isTogglable = false, toolTip = "Gets the creation date of your account and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Creation Date Gun", method = Fun.NarrateCreationDateGun, toolTip = "Gets the creation date of whoever your hand desires' account and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Creation Date All", method = Fun.NarrateCreationDateAll, isTogglable = false, toolTip = "Gets the creation date of everyones account and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Creation Date Aura", method = Fun.NarrateCreationDateAura, toolTip = "Gets the creation date of nearby players accounts and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Creation Date On Touch", method = Fun.NarrateCreationDateOnTouch, toolTip = "Gets the creation date of players you touch accounts and speaks it through your microphone." },

                new ButtonInfo { buttonText = "Grab Player Info", method = Fun.GrabPlayerInfo, isTogglable = false, toolTip = "Saves every player's name, color, and player ID as a text file and opens it." },
            },

            new[] { // Rebind Settings [13]
                new ButtonInfo { buttonText = "Exit Rebind Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Rebind A", enableMethod =() => Settings.StartRebind("A"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind B", enableMethod =() => Settings.StartRebind("B"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind X", enableMethod =() => Settings.StartRebind("X"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Y", enableMethod =() => Settings.StartRebind("Y"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Left Grip", enableMethod =() => Settings.StartRebind("LG"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Right Grip", enableMethod =() => Settings.StartRebind("RG"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Left Trigger", enableMethod =() => Settings.StartRebind("LT"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Right Trigger", enableMethod =() => Settings.StartRebind("RT"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Left Joystick", enableMethod =() => Settings.StartRebind("LJ"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},
                new ButtonInfo { buttonText = "Rebind Right Joystick", enableMethod =() => Settings.StartRebind("RJ"), disableMethod =() => IsRebinding = false, toolTip = "Enables rebinding mode, letting you change a mod's button."},

                new ButtonInfo { buttonText = "Clear Rebinds", method = Settings.RemoveRebinds, isTogglable = false, toolTip = "Removes all rebinds."},
            },

            new[] { // Sound Spam Mods [14]
                new ButtonInfo { buttonText = "Exit Sound Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Soundboard", method =() => Sound.LoadSoundboard(), isTogglable = false, toolTip = "A working, customizable soundboard that lets you play audios through your microphone."},

                new ButtonInfo { buttonText = "Bass Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(68), toolTip = "Plays the loud drum sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Metal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(18), toolTip = "Plays the metal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Wolf Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(195), toolTip = "Plays the wolf howl when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Cat Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(236), toolTip = "Plays the cat meow when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Turkey Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(83), toolTip = "Plays the turkey sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Frog Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(91), toolTip = "Plays the frog creak when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Bee Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(191), toolTip = "Plays the bee buzz when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Squeak Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(215), toolTip = "Plays the squeak sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Okay Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(336), toolTip = "Plays the sound of jmancurly saying \"okay\" with autotune when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Scream Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(337), toolTip = "Plays the sound of jmancurly screaming when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Slap Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(338), toolTip = "Plays the slap sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Jmancurly Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Sound.JmancurlySoundSpam, toolTip = "Plays the sounds from the jmancurly statue when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Random Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Sound.RandomSoundSpam, toolTip = "Plays random sounds when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Earrape Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(215), toolTip = "Plays a high-pitched sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Ding Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(244), toolTip = "Plays a ding sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Sound.CrystalSoundSpam, toolTip = "Plays some crystal noises when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Piano Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(Random.Range(295, 307)), toolTip = "Plays some terrible piano when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Big Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(213), toolTip = "Plays a long crystal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Pan Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(248), toolTip = "Plays a pan sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "AK-47 Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(203), toolTip = "Plays a sound that sounds like an AK-47 when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Siren Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Sound.SirenSoundSpam, toolTip = "Plays a siren sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tick Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(148), toolTip = "Plays a tick sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Custom Sound ID", method = Sound.IncreaseSoundID, enableMethod = Sound.IncreaseSoundID, disableMethod = Sound.DecreaseSoundID, incremental = true, isTogglable = false, toolTip = "Changes the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "Custom Sound Spam", overlapText = "Custom Sound Spam <color=grey>[</color><color=green>0</color><color=grey>]</color>", method = Sound.CustomSoundSpam, toolTip = "Plays the selected sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Override Tap Sound", method =() => { EffectDataPatch.enabled = true; EffectDataPatch.material = Sound.soundId; }, disableMethod =() => {  EffectDataPatch.enabled = false;  EffectDataPatch.material = -1; }, toolTip = "Plays the selected sound when holding <color=green>grip</color>." },
            },

            new[] { // Projectile Spam Mods [15]
                new ButtonInfo { buttonText = "Exit Projectile Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Grab Projectile <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.GrabProjectile, toolTip = "Grabs your selected projectile(s) holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Projectile Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.ProjectileSpam, toolTip = "Spams your selected projectile(s) when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Projectile Gun", method = Projectiles.ProjectileGun, toolTip = "Spams your selected projectile(s) at wherever your hand desires." },
                new ButtonInfo { buttonText = "Laser Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.LazerSpam, toolTip = "Spams your selected projectile(s) out of your eyes like lasers when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Give Projectile Spam Gun", method = Projectiles.GiveProjectileSpamGun, toolTip = "Acts like the projectile spam, but you can give it to whoever your hand desires when they hold <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Impact Spam", method = Projectiles.ImpactSpam, toolTip = "Acts like the projectile spam, but uses the impacts instead." },

                new ButtonInfo { buttonText = "Laser Eyes <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.LazerEyes, toolTip = "Makes you shoot lasers out of your eyes when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Urine <color=grey>[</color><color=green>G</color><color=grey>]</color>", aliases = new[] { "Pee", "Piss" }, method = Projectiles.Urine, toolTip = "Makes you pee when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Feces <color=grey>[</color><color=green>G</color><color=grey>]</color>", aliases = new[] { "Poop", "Shit" }, method = Projectiles.Feces, toolTip = "Makes you poo when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Period <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.Period, toolTip = "Makes you have your period when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Semen <color=grey>[</color><color=green>G</color><color=grey>]</color>", aliases = new[] { "Cum", "Ejaculate" }, method = Projectiles.Semen, toolTip = "Makes you ejaculate when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Vomit <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.Vomit, toolTip = "Makes you throw up when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Spit <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Projectiles.Spit, toolTip = "Makes you spit when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Laser Eyes Gun", method = Projectiles.LazerEyesGun, toolTip = "Makes whoever your hand desires shoot lasers out of their eyes." },
                new ButtonInfo { buttonText = "Urine Gun", aliases = new[] { "Pee Gun", "Piss Gun" }, method = Projectiles.UrineGun, toolTip = "Makes whoever your hand desires pee." },
                new ButtonInfo { buttonText = "Feces Gun", aliases = new[] { "Poop Gun", "Shit Gun" }, method = Projectiles.FecesGun, toolTip = "Makes whoever your hand desires poo." },
                new ButtonInfo { buttonText = "Period Gun", method = Projectiles.PeriodGun, toolTip = "Makes whoever your hand desires have their period." },
                new ButtonInfo { buttonText = "Semen Gun", aliases = new[] { "Cum Gun", "Ejaculate Gun" }, method = Projectiles.SemenGun, toolTip = "Makes whoever your hand desires ejaculate." },
                new ButtonInfo { buttonText = "Vomit Gun", method = Projectiles.VomitGun, toolTip = "Makes whoever your hand desires throw up." },
                new ButtonInfo { buttonText = "Spit Gun", method = Projectiles.SpitGun, toolTip = "Makes whoever your hand desires spit." },

                new ButtonInfo { buttonText = "Projectile Blind Gun", method = Projectiles.ProjectileBlindGun, toolTip = "Blinds whoever your hand desires using the egg projectiles."},
                new ButtonInfo { buttonText = "Projectile Blind All", enableMethod = Projectiles.ProjectileBlindAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Blinds everybody in the room using the egg projectiles."},

                new ButtonInfo { buttonText = "Projectile Lag Gun", method = Projectiles.ProjectileLagGun, toolTip = "Lags whoever your hand desires using the firework projectiles."},
                new ButtonInfo { buttonText = "Projectile Lag All", enableMethod = Projectiles.ProjectileLagAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Lags everybody in the room using the firework projectiles."},

                new ButtonInfo { buttonText = "Snowball Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballSpam, toolTip = "Spams snowballs when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Snowball Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballMinigun, toolTip = "Spawns snowballs towards wherever your hand desires."},
                new ButtonInfo { buttonText = "Give Snowball Minigun", method = Overpowered.GiveSnowballMinigun, toolTip = "Gives whoever your hand desires a snowball minigun." },

                new ButtonInfo { buttonText = "Snowball Gun", method = Overpowered.SnowballGun, toolTip = "Spawns snowballs wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Nuke Gun", method = Overpowered.SnowballNukeGun, toolTip = "Spawns a lot of snowballs airstriking from the sky at wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Airstrike Gun", method = Overpowered.SnowballAirstrikeGun, toolTip = "Spawns a snowball airstrike wherever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Rain <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballRain, toolTip = "Rains snowballs around you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Hail <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballHail, toolTip = "Hails snowballs around you when holding <color=green>trigger</color>."},
                
                new ButtonInfo { buttonText = "Snowball Fountain <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballFountain, toolTip = "Fountains snowballs above you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Positional Fountain <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballPositionalFountain, disableMethod = Overpowered.DisableSnowballPositionalFountain, toolTip = "Place a fountain <color=green>grip</color> and use it with <color=green>trigger</color>. It will fountain snowballs at wherever the fountain is."},

                new ButtonInfo { buttonText = "Snowball Orbit <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballOrbit, toolTip = "Orbits snowballs around you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Aura <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballAura, toolTip = "Randomly spawns snowballs around you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Mushroom <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballMushroom, toolTip = "Spawns a mushroom cloud of snowballs on you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Shotgun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballShotgun, toolTip = "Spawns snowballs around wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Wall <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballWall, toolTip = "Spawns a wall of snowballs towards wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball C4 <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballBomb, disableMethod = Overpowered.DisableBomb, toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>. It will spawn snowballs at wherever the bomb is."},
                new ButtonInfo { buttonText = "Snowball Grenade <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => CollisionPatch.OnCollisionEnterEvent += Overpowered.OnSnowballHit, method = Overpowered.SnowballGrenade, disableMethod =() => CollisionPatch.OnCollisionEnterEvent -= Overpowered.OnSnowballHit, toolTip = "Spawn and throw with <color=green>grip</color> It will spawn snowballs at the landing point."},
                new ButtonInfo { buttonText = "Snowball RPG <color=grey>[</color><color=green>G</color><color=grey>]</color>", enableMethod =() => CollisionPatch.OnCollisionEnterEvent += Overpowered.OnSnowballHit, method = Overpowered.SnowballRPG, disableMethod =() => CollisionPatch.OnCollisionEnterEvent -= Overpowered.OnSnowballHit, toolTip = "Shoots a snowball with <color=green>grip</color>. It will spawn snowballs at the landing point."},

                new ButtonInfo { buttonText = "Snowball Punch Mod", method = Overpowered.SnowballPunchMod, toolTip = "Flings people when you punch them."},
                new ButtonInfo { buttonText = "Snowball Boxing", method = Overpowered.SnowballBoxing, toolTip = "Gives everyone the punch mod by using snowballs."},
                new ButtonInfo { buttonText = "Snowball Dash <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Overpowered.SnowballDash, toolTip = "Allows other players to dash themself into the air with the snowballs."},
                new ButtonInfo { buttonText = "Snowball High Jump", method = Overpowered.SnowballHighJump, toolTip = "Allow everyone to jump higher using snowballs."},
                new ButtonInfo { buttonText = "Snowball Particle Gun", method = Overpowered.SnowballParticleGun, toolTip = "Spawns snowball particles wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Impact Effect Gun", method = Overpowered.SnowballImpactEffectGun, toolTip = "Spawns snowball impact events on whoever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Kamehameha", enableMethod = Overpowered.Enable_Kamehameha, method = Overpowered.Kamehameha, disableMethod = Overpowered.Disable_Kamehameha, toolTip = "Spawns a flaming ball when holding down both triggers and grips." },

                new ButtonInfo { buttonText = "Snowball Fling Zone <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.SnowballFlingZone, disableMethod = Overpowered.DisableSnowballFlingZone, toolTip = "Spawn and move fling zones with your <color=green>right grip</color>. Press <color=green>trigger</color> to remove fling zones."},

                new ButtonInfo { buttonText = "Snowball Fling Gun", method = Overpowered.SnowballFlingGun, toolTip = "Flings whoever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballFlingAll, toolTip = "Flings everybody when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Fling Aura", method = Overpowered.SnowballSafetyBubble, toolTip = "Anyone who gets too close to you will be launched away."},

                new ButtonInfo { buttonText = "Snowball Fling Vertical Gun", method = Overpowered.SnowballFlingVerticalGun, toolTip = "Flings whoever your hand desires vertically."},
                new ButtonInfo { buttonText = "Snowball Fling Vertical All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SnowballFlingVerticalAll, toolTip = "Flings everybody vertically when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Fling Towards Gun", overlapText = "Snowball Bring Gun", aliases = new[] { "Snowball Fling Towards Gun" }, method = Overpowered.SnowballFlingTowardsGun, toolTip = "Flings everybody towards wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Fling Away Gun", method = Overpowered.SnowballFlingAwayGun, overlapText = "Snowball Push Gun", aliases = new[] { "Snowball Fling Away Gun" }, toolTip = "Flings everybody away from wherever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Fling Player Towards Gun", overlapText = "Snowball Bring Player Gun", aliases = new[] { "Snowball Fling Player Towards Gun" }, method = Overpowered.SnowballFlingPlayerTowardsGun, toolTip = "Flings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Snowball Fling Player Away Gun", overlapText = "Snowball Push Player Gun", aliases = new[] { "Snowball Fling Player Away Gun" }, method = Overpowered.SnowballFlingPlayerAwayGun, toolTip = "Flings whoever your hand desires away from you."},

                new ButtonInfo { buttonText = "Snowball Launch Gun", method = Overpowered.SnowballLaunchGun, toolTip = "Launches whoever your hand desires like a launch pad."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Snowball Fling</color><color=grey>]</color>", method = Overpowered.AntiReportSnowballFling, toolTip = "Flings whoever tries to report you with the snowballs."}
            },

            new[] { // Master Mods [16]
                new ButtonInfo { buttonText = "Exit Master Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "MasterLabel", overlapText = "You are not master client.", label = true},

                new ButtonInfo { buttonText = "Guardian Self", method = Overpowered.GuardianSelf, isTogglable = false, toolTip = "Makes you red."},
                new ButtonInfo { buttonText = "Guardian Gun", method = Overpowered.GuardianGun, toolTip = "Makes whoever your hand desires the guardian."},
                new ButtonInfo { buttonText = "Guardian All", method = Overpowered.GuardianAll, isTogglable = false, toolTip = "Makes everyone in the room the guardian."},

                new ButtonInfo { buttonText = "Unguardian Self", method = Overpowered.UnguardianSelf, isTogglable = false, toolTip = "Removes you from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian Gun", method = Overpowered.UnguardianGun, toolTip = "Removes whoever your hand desires from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian All", method = Overpowered.UnguardianAll, isTogglable = false, toolTip = "Removes everyone in the room from the guardian position."},

                new ButtonInfo { buttonText = "Guardian Spaz", method = Overpowered.GuardianSpaz, toolTip = "Spams the guardian position for everyone in the room."},

                new ButtonInfo { buttonText = "Red Color Self", method =() => Overpowered.SetColorSelf(1), isTogglable = false, toolTip = "Changes your color to red."},
                new ButtonInfo { buttonText = "Red Color Gun", method =() => Overpowered.SetColorGun(1), toolTip = "Changes whoever your hand desires' color to red."},
                new ButtonInfo { buttonText = "Red Color All", method =() => Overpowered.SetColorAll(1), isTogglable = false, toolTip = "Changes everyone in the room's color to red."},

                new ButtonInfo { buttonText = "Blue Color Self", method =() => Overpowered.SetColorSelf(0), isTogglable = false, toolTip = "Changes your color to blue."},
                new ButtonInfo { buttonText = "Blue Color Gun", method =() => Overpowered.SetColorGun(0), toolTip = "Changes whoever your hand desires' color to blue."},
                new ButtonInfo { buttonText = "Blue Color All", method =() => Overpowered.SetColorAll(0), isTogglable = false, toolTip = "Changes everyone in the room's color to blue."},

                new ButtonInfo { buttonText = "Reset Color Self", method =() => Overpowered.SetColorSelf(-1), isTogglable = false, toolTip = "Resets your color back to what it was."},
                new ButtonInfo { buttonText = "Reset Color Gun", method =() => Overpowered.SetColorGun(-1), toolTip = "Resets whoever your hand desires' color back to what it was."},
                new ButtonInfo { buttonText = "Reset Color All", method =() => Overpowered.SetColorAll(-1), isTogglable = false, toolTip = "Resets everyone in the room's color back to what it was."},

                new ButtonInfo { buttonText = "Strobe Color Self", method = Overpowered.StrobeColorSelf, toolTip = "Flashes your color between red and blue."},
                new ButtonInfo { buttonText = "Strobe Color Gun", method = Overpowered.StrobeColorGun, toolTip = "Flashes whoever your hand desires' color between red and blue."},
                new ButtonInfo { buttonText = "Strobe Color All", method = Overpowered.StrobeColorAll, toolTip = "Flashes everyone in the room's color between red and blue."},

                new ButtonInfo { buttonText = "Material Self", method =() => { if (!(Time.time > Overpowered.materialDelay)) return; Overpowered.MaterialTarget(VRRig.LocalRig); Overpowered.materialDelay = Time.time + 0.1f; }, toolTip = "Flashes the materials of yourself."},
                new ButtonInfo { buttonText = "Material Gun", method = Overpowered.MaterialGun, toolTip = "Flashes the materials of whoever your hand desires."},
                new ButtonInfo { buttonText = "Material All", method = Overpowered.MaterialAll, toolTip = "Flashes the materials of everyone in the room."},
                
                new ButtonInfo { buttonText = "Grey Screen Gun", method = ()=> Overpowered.ActivateGreyZoneGun(true), toolTip = "Makes whoever your hand desires' screen grey." },
                new ButtonInfo { buttonText = "Fix Screen Gun", method = ()=> Overpowered.ActivateGreyZoneGun(false), toolTip = "Makes whoever your hand desires' screen normal again." },
                new ButtonInfo { buttonText = "Grey Screen All", enableMethod = ()=> Overpowered.ActivateGreyZone(true), disableMethod =() => Overpowered.ActivateGreyZone(false), toolTip = "Makes everyone's screen grey." },

                new ButtonInfo { buttonText = "Spaz Grey Screen Gun", method = Overpowered.SpazGreyZoneGun, toolTip = "Makes whoever your hand desires' screen flash grey." },
                new ButtonInfo { buttonText = "Spaz Grey Screen All", method = Overpowered.SpazGreyZone, disableMethod =() => Overpowered.ActivateGreyZone(false), toolTip = "Makes everyone's screen flash grey." },

                new ButtonInfo { buttonText = "Spaz Prop Hunt", method = Overpowered.SpazPropHunt, toolTip = "Repeatedly starts and ends the prop hunt gamemode."},
                new ButtonInfo { buttonText = "Spaz Prop Hunt Objects", method = Overpowered.SpazPropHuntObjects, toolTip = "Repeatedly randomizes everyone's selected object in the prop hunt gamemode."},

                new ButtonInfo { buttonText = "Max Currency Self", method =() => Fun.SetCurrencySelf(int.MaxValue), isTogglable = false, toolTip = "Gives you the maximum amount of currency in the ghost reactor (2 billion)."},
                new ButtonInfo { buttonText = "Max Currency Gun", method =() => Fun.SetCurrencyGun(int.MaxValue), toolTip = "Gives whoever your hand desires the maximum amount of currency in the ghost reactor (2 billion)."},
                new ButtonInfo { buttonText = "Max Currency All", method =() => Fun.SetCurrencyAll(int.MaxValue), isTogglable = false, toolTip = "Gives everyone in the room the maximum amount of currency in the ghost reactor (2 billion)."},

                new ButtonInfo { buttonText = "Add Currency Self", method =() => Fun.AddCurrencySelf(100), isTogglable = false, toolTip = "Gives you 100 more currency in the ghost reactor."},
                new ButtonInfo { buttonText = "Add Currency Gun", method =() => Fun.AddCurrencyGun(100), toolTip = "Gives whoever your hand desires 100 more currency in the ghost reactor."},
                new ButtonInfo { buttonText = "Add Currency All", method =() => Fun.AddCurrencyAll(100), isTogglable = false, toolTip = "Gives everyone in the room 100 more currency in the ghost reactor."},

                new ButtonInfo { buttonText = "Remove Currency Self", method =() => Fun.SetCurrencySelf(), isTogglable = false, toolTip = "Removes all currency in the ghost reactor from yourself."},
                new ButtonInfo { buttonText = "Remove Currency Gun", method =() => Fun.SetCurrencyGun(), toolTip = "Removes all currency in the ghost reactor from whoever your hand desires."},
                new ButtonInfo { buttonText = "Remove Currency All", method =() => Fun.SetCurrencyAll(), isTogglable = false, toolTip = "Removes all currency in the ghost reactor from everyone in the room."},

                new ButtonInfo { buttonText = "Invincibility", method = Fun.Invincibility, toolTip = "Makes you unable to die in the ghost reactor."},

                new ButtonInfo { buttonText = "Start Shift", method = Fun.StartShift, isTogglable = false, toolTip = "Starts a new ghost reactor shift."},
                new ButtonInfo { buttonText = "End Shift", method = Fun.EndShift, isTogglable = false, toolTip = "Ends the current ghost reactor shift."},
                new ButtonInfo { buttonText = "Set Quota", method = Fun.SetQuota, toolTip = "Meets the quota for you."},

                new ButtonInfo { buttonText = "Virtual Stump Kick Gun", method = Overpowered.VirtualStumpKickGun, toolTip = "Kicks whoever your hand desires in the virtual stump."},
                new ButtonInfo { buttonText = "Virtual Stump Kick All", method = Overpowered.VirtualStumpKickAll, toolTip = "Kicks everyone in the virtual stump."},

                new ButtonInfo { buttonText = "Virtual Stump Crash Gun", method = Overpowered.VirtualStumpCrashGun, toolTip = "Crashes whoever your hand desires in the virtual stump."},
                new ButtonInfo { buttonText = "Virtual Stump Crash All", method = Overpowered.VirtualStumpCrashAll, toolTip = "Crashes everyone in the virtual stump."},
                
                new ButtonInfo { buttonText = "Ghost Reactor Freeze Gun", method = Fun.GhostReactorFreezeGun, toolTip = "Freezes whoever your hand desires in the ghost reactor."},
                new ButtonInfo { buttonText = "Ghost Reactor Freeze All", method = Fun.GhostReactorFreezeAll, toolTip = "Freezes everyone in the ghost reactor."},

                new ButtonInfo { buttonText = "Ghost Reactor Crash Gun", method = Overpowered.GhostReactorCrashGun, toolTip = "Crashes whoever your hand desires in the ghost reactor."},
                new ButtonInfo { buttonText = "Ghost Reactor Crash All", method = Overpowered.GhostReactorCrashAll, toolTip = "Crashes everyone in the ghost reactor."},

                new ButtonInfo { buttonText = "Super Infection Crash Gun", method = Overpowered.SuperInfectionCrashGun, toolTip = "Crashes whoever your hand desires in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Super Infection Crash All", method = Overpowered.SuperInfectionCrashAll, toolTip = "Crashes everyone in the Super Infection gamemode."},

                new ButtonInfo { buttonText = "Super Infection Break Audio Gun", method = Overpowered.SuperInfectionBreakAudioGun, toolTip = "Breaks the audio of whoever your hand desires in the Super Infection gamemode."},
                new ButtonInfo { buttonText = "Super Infection Break Audio All", method = Overpowered.SuperInfectionBreakAudioAll, toolTip = "Breaks the audio of everyone in the Super Infection gamemode."},

                new ButtonInfo { buttonText = "Kill Self", method =() => Fun.SetStateSelf(1), isTogglable = false, toolTip = "Turns you into a ghost."},
                new ButtonInfo { buttonText = "Kill Gun", method =() => Fun.SetStateGun(1), toolTip = "Turns whoever your hand desires into a ghost."},
                new ButtonInfo { buttonText = "Kill All", method =() => Fun.SetStateAll(1), isTogglable = false, toolTip = "Turns everyone in the room into a ghost."},

                new ButtonInfo { buttonText = "Revive Self", method =() => Fun.SetStateSelf(0), isTogglable = false, toolTip = "Revives you from death."},
                new ButtonInfo { buttonText = "Revive Gun", method =() => Fun.SetStateGun(0), toolTip = "Revives whoever your hand desires from death."},
                new ButtonInfo { buttonText = "Revive All", method =() => Fun.SetStateAll(0), isTogglable = false, toolTip = "Revives everyone in the room from death."},

                new ButtonInfo { buttonText = "Spaz Kill Self", method = Fun.SpazKillSelf, toolTip = "Repeatedly kills and revives you."},
                new ButtonInfo { buttonText = "Spaz Kill Gun", method = Fun.SpazKillGun, toolTip = "Repeatedly kills and revives whoever your hand desires."},
                new ButtonInfo { buttonText = "Spaz Kill All", method = Fun.SpazKillAll, toolTip = "Repeatedly kills and revives everyone in the room."},

                new ButtonInfo { buttonText = "Unlimited Building", enableMethod = Fun.UnlimitedBuilding, disableMethod = Fun.DisableUnlimitedBuilding, toolTip = "Unlimits building, disabling drop zones and letting you place on people's plots." },

                new ButtonInfo { buttonText = "Shotgun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.Shotgun, toolTip = "Spawns you a shotgun when you press <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Block Crash Gun", overlapText = "Building Block Crash Gun", method = Overpowered.BlockCrashGun, toolTip = "Crashes whoever your hand desires if they are inside of the block map."},
                new ButtonInfo { buttonText = "Block Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Building Block Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BlockCrashAll, toolTip = "Crashes everybody inside of the block map."},

                new ButtonInfo { buttonText = "Block Anti Report", enableMethod = Fun.EnableAtticAntiReport, method = Fun.AtticAntiReport, toolTip = "Automatically builds blocks around your report button."},

                new ButtonInfo { buttonText = "Block Draw Gun", overlapText = "Building Block Draw Gun", method = Fun.AtticDrawGun, toolTip = "Draw wherever your hand desires."},
                new ButtonInfo { buttonText = "Block Build Gun", overlapText = "Building Block Build Gun",method = Fun.AtticBuildGun, toolTip = "Draw wherever your hand desires with no delay."},
                new ButtonInfo { buttonText = "Block Tower Gun", overlapText = "Building Block Tower Gun",method = Fun.AtticTowerGun, toolTip = "Builds a tower wherever your hand desires."},

                new ButtonInfo { buttonText = "Block Freeze Gun", overlapText = "Building Block Freeze Gun", method = Fun.AtticFreezeGun, toolTip = "Freeze whoever your hand desires."},
                new ButtonInfo { buttonText = "Block Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Building Block Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Fun.AtticFreezeAll, toolTip = "Freezes everyone in the lobby when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Block Float Gun", overlapText = "Building Block Float Gun", method = Fun.AtticFloatGun, toolTip = "Makes whoever your hand desires float using the building blocks."},
                new ButtonInfo { buttonText = "Building Block Fling Gun", method = Fun.AtticFlingGun, toolTip = "Flings whoever your hand desires using the building blocks."},

                new ButtonInfo { buttonText = "Spaz Targets", method = Overpowered.TargetSpam, toolTip = "Gives the targets a seizure."},

                new ButtonInfo { buttonText = "Slow Monsters", enableMethod = Fun.SlowMonsters, disableMethod = Fun.FixMonsters, toolTip = "Slows down the basement monsters." },
                new ButtonInfo { buttonText = "Fast Monsters", enableMethod = Fun.FastMonsters, disableMethod = Fun.FixMonsters, toolTip = "Speeds up the basement monsters." },

                new ButtonInfo { buttonText = "Grab Monsters <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Fun.GrabMonsters, toolTip = "Puts the basement monsters in your hand." },
                new ButtonInfo { buttonText = "Monster Gun", method = Fun.MonsterGun, toolTip = "Moves the basement monsters to wherever your hand desires." },
                new ButtonInfo { buttonText = "Spaz Monsters", method = Fun.SpazMonsters, toolTip = "Gives the basement monsters a seizure." },
                new ButtonInfo { buttonText = "Orbit Monsters", method = Fun.OrbitMonsters, toolTip = "Orbits the basement monsters around you." },
                new ButtonInfo { buttonText = "Destroy Monsters", method = Fun.DestroyMonsters, isTogglable = false, toolTip = "Sends the basement monsters to hell." },

                new ButtonInfo { buttonText = "Infection to Tag", method = Overpowered.InfectionToTag, disableMethod = Overpowered.FixThreshold, toolTip = "Turns the game into tag instead of infection." },
                new ButtonInfo { buttonText = "Tag to Infection", method = Overpowered.TagToInfection, disableMethod = Overpowered.FixThreshold, toolTip = "Turns the game into infection instead of tag." },

                new ButtonInfo { buttonText = "Untag Gun", method = Advantages.UntagGun, toolTip = "Untags whoever your hand desires."},
                new ButtonInfo { buttonText = "Untag All", method = Advantages.UntagAll, isTogglable = false, toolTip = "Removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Break Tag", method = Advantages.UntagAll, toolTip = "Constantly removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Spam Tag Self", method = Advantages.SpamTagSelf, toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag Gun", method = Advantages.SpamTagGun, toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag All", method = Advantages.SpamTagAll, toolTip = "Adds and removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Rock Self", method = Overpowered.RockSelf, isTogglable = false, toolTip = "Sets yourself to rock."},
                new ButtonInfo { buttonText = "Rock Gun", method = Overpowered.RockGun, toolTip = "Sets whoever your hand desires to rock."},
                new ButtonInfo { buttonText = "Rock All", method = Overpowered.RockAll, toolTip = "Sets everyone in the room to rock."},
                new ButtonInfo { buttonText = "Rock Aura", method = Overpowered.RockAura, toolTip = "Sets players nearby you to rock."},
                new ButtonInfo { buttonText = "Rock On Touch", method = Overpowered.RockOnTouch, toolTip = "Sets whoever you touch to rock."},

                new ButtonInfo { buttonText = "Give Tag Lag Gun", method = Advantages.TagLagGun, toolTip = "Forces tag lag on whoever your hand desires, making them untaggable."},
                new ButtonInfo { buttonText = "Tag Lag Gun", method = Advantages.TagLagGun, toolTip = "Forces tag lag on whoever your hand desires, letting them not be able to tag anyone."},
                new ButtonInfo { buttonText = "Tag Lag", overlapText = "Tag Lag All", method =() => Advantages.SetTagCooldown(float.MaxValue), disableMethod =() => Advantages.SetTagCooldown(5f), toolTip = "Forces tag lag in the everyone in the room, letting no one get tagged."},

                new ButtonInfo { buttonText = "Unlock Driver", method =() => Overpowered.DriverStatus(false), isTogglable = false, toolTip = "Unlocks the driver in the virtual stump."},
                new ButtonInfo { buttonText = "Become Driver", method =() => Overpowered.DriverStatus(true), isTogglable = false, toolTip = "Makes you the driver in the virtual stump."},
                new ButtonInfo { buttonText = "Spaz Driver", method = Overpowered.SpazDriver, isTogglable = true, toolTip = "Spaz makes and unmakes you the driver in the virtual stump."},

                new ButtonInfo { buttonText = "Become Driver Gun", method =() => Overpowered.DriverStatusGun(true), isTogglable = true, toolTip = "Makes whoever your hand desires the driver in the virtual stump."},
                new ButtonInfo { buttonText = "Unlock Driver Gun", method =() => Overpowered.DriverStatusGun(false), isTogglable = true, toolTip = "Unlocks the driver for whoever your hand desires in the virtual stump."},
                new ButtonInfo { buttonText = "Spaz Driver Gun", method = Overpowered.SpazDriverStatusGun, isTogglable = true, toolTip = "Spaz makes and unmakes whoever your hand desires the driver in the virtual stump."},

                new ButtonInfo { buttonText = "Bonk Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(4), toolTip = "Plays the bonk sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(1), toolTip = "Plays the count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(6), toolTip = "Plays the brawl count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Start Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(7), toolTip = "Plays the brawl start sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tag Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(0), toolTip = "Plays the tag sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Round End Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(2), toolTip = "Plays the round end sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Paintbrawl Start Game", method = Advantages.PaintbrawlStartGame, isTogglable = false, toolTip = "Starts a game of paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl End Game", method = Advantages.PaintbrawlEndGame, isTogglable = false, toolTip = "Ends the current game of paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Restart Game", method = Advantages.PaintbrawlRestartGame, isTogglable = false, toolTip = "Restarts the current game of paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Restart Spam", method = Advantages.PaintbrawlRestartGame, toolTip = "Spam starts and ends games of paintbrawl." },

                new ButtonInfo { buttonText = "Paintbrawl Balloon Spam Self", method = Advantages.PaintbrawlBalloonSpamSelf, toolTip = "Spam pops and unpops your balloons in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Balloon Spam Gun", method = Advantages.PaintbrawlBalloonSpamGun, toolTip = "Spam pops and unpops whoever your hand desires' balloons in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Balloon Spam All", method = Advantages.PaintbrawlBalloonSpam, toolTip = "Spam pops and unpops everyone's balloons in paintbrawl." },

                new ButtonInfo { buttonText = "Paintbrawl Revive Self", method = Advantages.PaintbrawlReviveSelf, isTogglable = false, toolTip = "Revives yourself in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Revive Gun", method = Advantages.PaintbrawlReviveGun, toolTip = "Revives whoever your hand desires in paintbrawl." },
                new ButtonInfo { buttonText = "Paintbrawl Revive All", method = Advantages.PaintbrawlReviveAll, isTogglable = false, toolTip = "Revives everyone in paintbrawl." },

                new ButtonInfo { buttonText = "Paintbrawl No Delay", method = Advantages.PaintbrawlNoDelay, disableMethod = Advantages.DisablePaintbrawlNoDelay, toolTip = "Revives everyone in paintbrawl." },

                new ButtonInfo { buttonText = "Paintbrawl God Mode", method = Advantages.PaintbrawlGodMode, toolTip = "Gives you god mode in paintbrawl." },

                new ButtonInfo { buttonText = "Slow Self", method = Overpowered.SlowSelf, isTogglable = false, toolTip = "Forces tag freeze on yourself." },
                new ButtonInfo { buttonText = "Slow Gun", method = Overpowered.SlowGun, toolTip = "Forces tag freeze on whoever your hand desires." },
                new ButtonInfo { buttonText = "Slow All", method = Overpowered.SlowAll, toolTip = "Forces tag freeze on everyone in the the room." },
                new ButtonInfo { buttonText = "Slow Aura", method = Overpowered.SlowAura, toolTip = "Forces tag freeze on players nearby you."},
                new ButtonInfo { buttonText = "Slow On Touch", method = Overpowered.SlowOnTouch, toolTip = "Forces tag freeze on whoever you touch."},
                
                new ButtonInfo { buttonText = "Vibrate Self", method = Overpowered.VibrateSelf, isTogglable = false, toolTip = "Makes your controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate Gun", method = Overpowered.VibrateGun, toolTip = "Makes whoever your hand desires' controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate All", method = Overpowered.VibrateAll, toolTip = "Makes everyone in the the room's controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate Aura", method = Overpowered.VibrateAura, toolTip = "Makes players nearby you controllers vibrate."},
                new ButtonInfo { buttonText = "Vibrate On Touch", method = Overpowered.VibrateOnTouch, toolTip = "Makes whoever you touch controllers vibrate."},
            },

            new[] { // Overpowered Mods [17]
                new ButtonInfo { buttonText = "Exit Overpowered Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Always Guardian", method = Overpowered.AlwaysGuardian, disableMethod = Movement.EnableRig, toolTip = "Makes you always the guardian."},
                new ButtonInfo { buttonText = "Guardian Protector", method = Overpowered.GuardianProtector, toolTip = "Pushes people away from the guardian moon if they try to approach it."},

                new ButtonInfo { buttonText = "Grab Gun", overlapText = "Guardian Grab Gun", method = Overpowered.GrabGun, toolTip = "Grabs whoever your hand desires if you're the guardian."},
                new ButtonInfo { buttonText = "Grab All <color=grey>[</color><color=green>G</color><color=grey>]</color>", overlapText = "Guardian Grab All <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.GrabAll, toolTip = "Grabs everyone in the room if you're the guardian."},

                new ButtonInfo { buttonText = "Release Gun", overlapText = "Guardian Release Gun", method = Overpowered.ReleaseGun, toolTip = "Releases whoever your hand desires if you're the guardian."},
                new ButtonInfo { buttonText = "Release All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Release All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.ReleaseAll, toolTip = "Releases everyone in the room if you're the guardian."},

                new ButtonInfo { buttonText = "Fling Gun", overlapText = "Guardian Fling Gun", method = Overpowered.FlingGun, toolTip = "Flings whoever your hand desires."},
                new ButtonInfo { buttonText = "Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.FlingAll, toolTip = "Flings everyone in the room."},

                new ButtonInfo { buttonText = "Bring Gun", overlapText = "Guardian Bring Gun", method = Overpowered.BringGun, toolTip = "Brings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Bring All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Bring All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BringAll, toolTip = "Brings everyone in the room towards you."},

                new ButtonInfo { buttonText = "Bring All Gun", overlapText = "Guardian Bring All Gun", method = Overpowered.BringAllGun, toolTip = "Brings everyone in the room towards wherever your hand desires."},

                new ButtonInfo { buttonText = "Guardian Bring Away Gun", method = Overpowered.BringAwayGun, toolTip = "Brings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Guardian Bring Away All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BringAwayAll, toolTip = "Brings everyone in the room towards you."},

                new ButtonInfo { buttonText = "Bring Away All Gun", method = Overpowered.BringAwayAllGun, toolTip = "Brings everyone in the room towards wherever your hand desires."},

                new ButtonInfo { buttonText = "Guardian Anti Stump", method = Overpowered.AntiStump, toolTip = "Anyone who gets too close to the stump entrance will be launched away."},

                new ButtonInfo { buttonText = "Guardian Orbit All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.OrbitAll, toolTip = "Orbits everyone in the room around you."},

                new ButtonInfo { buttonText = "Guardian Punch Mod", method = Overpowered.PunchMod, toolTip = "Flings people when you punch them."},
                new ButtonInfo { buttonText = "Guardian Boxing", method = Overpowered.Boxing, toolTip = "Lets everyone in the room punch eachother."},

                new ButtonInfo { buttonText = "Guardian Give Fly Gun", method = Overpowered.GiveFlyGun, toolTip = "Gives whoever you want fly when they hold their right thumb down."},
                new ButtonInfo { buttonText = "Guardian Give Fly All", method = Overpowered.GiveFlyAll, toolTip = "Gives everyone in the room fly when they hold their right thumb down."},

                new ButtonInfo { buttonText = "Spaz Player Gun", overlapText = "Guardian Spaz Player Gun", method = Overpowered.SpazPlayerGun, toolTip = "Spazzes out whoever your hand desires."},
                new ButtonInfo { buttonText = "Spaz All Players <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Spaz All Players <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.SpazAllPlayers, toolTip = "Spazzes out everyone in the room."},

                new ButtonInfo { buttonText = "Effect Spam Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", overlapText = "Guardian Effect Spam Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method = Overpowered.EffectSpamHands, toolTip = "Spawns effects when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Effect Spam Gun", method = Overpowered.EffectSpamGun, overlapText = "Guardian Effect Spam Gun", toolTip = "Spawns effects wherever your hand desires."},

                new ButtonInfo { buttonText = "Physical Freeze Gun", overlapText = "Guardian Freeze Gun", method = Overpowered.PhysicalFreezeGun, toolTip = "Freezes whoever your hand desires." },
                new ButtonInfo { buttonText = "Physical Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.PhysicalFreezeAll, toolTip = "Freezes everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Guardian Kick Gun", method = Overpowered.GuardianKickGun, toolTip = "Kicks whoever your hand desires." },
                new ButtonInfo { buttonText = "Guardian Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.GuardianKickAll, toolTip = "Kicks everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Guardian Crash Gun", method = Overpowered.GuardianCrashGun, toolTip = "Crashes whoever your hand desires." },
                new ButtonInfo { buttonText = "Guardian Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.GuardianCrashAll, toolTip = "Crashes everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Lag Master Client", method = Overpowered.LagMasterClient, toolTip = "Lags the master client." },
                new ButtonInfo { buttonText = "Lag Master Client Gun", method = Overpowered.LagMasterClientGun, toolTip = "Lags whoever your hand desires, if they are master client. Credits to Charlotte for this stupid idea." },

                new ButtonInfo { buttonText = "Kick Master Client", enableMethod = () => Overpowered.kickCoroutine = CoroutineManager.instance.StartCoroutine(Overpowered.KickMasterClient()), method =() => { if (Overpowered.kickCoroutine == null) Toggle("Kick Master Client"); }, disableMethod =() => { SerializePatch.OverrideSerialization = null; Overpowered.kickCoroutine = null; }, toolTip = "Kicks the master client from the room." },
                new ButtonInfo { buttonText = "Kick Gun", method = Overpowered.KickGun, disableMethod =() => { SerializePatch.OverrideSerialization = null; Overpowered.kickCoroutine = null; }, toolTip = "Kick whoever your hand desires, if they are master client. Credits to Rexon for making such a stupid mod." },
                new ButtonInfo { buttonText = "Kick All", enableMethod = () => Overpowered.kickCoroutine = CoroutineManager.instance.StartCoroutine(Overpowered.KickAll()), method =() => { if (Overpowered.kickCoroutine == null) Toggle("Kick All"); }, disableMethod =() => { SerializePatch.OverrideSerialization = null; Overpowered.kickCoroutine = null; }, toolTip = "Kicks everyone above you from the room." },

                new ButtonInfo { buttonText = "Cache Kick Gun", method = Overpowered.CacheKickGun, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks everyone in the room by filling up the room cache." },
                new ButtonInfo { buttonText = "Cache Kick All", enableMethod = Overpowered.EnableCacheKickAll, method = Overpowered.CacheKickAll, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks everyone in the room by filling up the room cache." },

                new ButtonInfo { buttonText = "Delay Ban Gun", method = Overpowered.DelayBanGun, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Delay bans whoever your hand desires."},
                new ButtonInfo { buttonText = "Delay Ban All", enableMethod = Overpowered.DelayBanAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Delay bans everyone in the room."},

                new ButtonInfo { buttonText = "Force Grab", method = Overpowered.ForceGrab, toolTip = "Attempts to grab the hand of anyone who presses their grips." },
                new ButtonInfo { buttonText = "Fling on Grab", method = Overpowered.FlingOnGrab, toolTip = "Flings the player when they grab you." },
                new ButtonInfo { buttonText = "Kick on Grab", method =() => Overpowered.TowardsPositionOnGrab(new Vector3(-71.33718f, 101.4977f, -93.09029f)), toolTip = "Kicks the player when they grab you." },
                new ButtonInfo { buttonText = "Crash on Grab", method =() => Overpowered.DirectionOnGrab(Vector3.one), toolTip = "Crashes the player when they grab you." },
                new ButtonInfo { buttonText = "Destroy on Grab", method =() => Overpowered.DirectionOnGrab(new Vector3(1f, -1f, 1f)), toolTip = "Destroys the player when they grab you." },
                new ButtonInfo { buttonText = "Obliterate on Grab", method =() => Overpowered.DirectionOnGrab(Vector3.up), toolTip = "Obliterates the player when they grab you." },
                new ButtonInfo { buttonText = "Towards Point on Grab Gun", method = Overpowered.TowardsPointOnGrab, disableMethod = Overpowered.DisableTowardsPointOnGrab, toolTip = "Sends the player to your target position when they grab you." },

                new ButtonInfo { buttonText = "Lag Server", method =() => Overpowered.FreezeServer(1f, 11), toolTip = "Lags the room." },
                new ButtonInfo { buttonText = "Freeze Server", enableMethod =() => SerializePatch.OverrideSerialization = () => false, method =() => Overpowered.FreezeServer(), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Freezes the room." },
                new ButtonInfo { buttonText = "Crash Server", enableMethod =() => SerializePatch.OverrideSerialization = () => false, method =() => Overpowered.FreezeServer(0.1f, 40), disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Crashes the room." },
                new ButtonInfo { buttonText = "Za Warudo <color=grey>[</color><color=green>T</color><color=grey>]</color>", enableMethod = Overpowered.ZaWarudo_enableMethod, method = Overpowered.ZaWarudo, toolTip = "Freeze all, but with special effects." },

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Fling</color><color=grey>]</color>", method = Overpowered.AntiReportFling, toolTip = "Flings whoever tries to report you."},

                new ButtonInfo { buttonText = "Lag Gun", method = Overpowered.LagGun, toolTip = "Lags whoever your hand desires."},
                new ButtonInfo { buttonText = "Lag All", method = Overpowered.LagAll, toolTip = "Lags everyone in the room."},
                new ButtonInfo { buttonText = "Lag Aura", method = Overpowered.LagAura, toolTip = "Lags players nearby."},
                new ButtonInfo { buttonText = "Lag On Touch", method = Overpowered.LagOnTouch, toolTip = "Lags whoever you touch." },

                new ButtonInfo { buttonText = "Server Mute All", method = Overpowered.ServerMuteAll, toolTip = "Mutes everyone in the server."},
                new ButtonInfo { buttonText = "Deafen Gun", method = Overpowered.DeafenGun, toolTip = "Makes whoever your hand deseries not be able to hear anyone else."},
                new ButtonInfo { buttonText = "Deafen All", method = Overpowered.DeafenAll, toolTip = "Makes everyone not be able to hear anyone except you."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Lag</color><color=grey>]</color>", method = Overpowered.AntiReportLag, toolTip = "Lags whoever tries to report you."},

                new ButtonInfo { buttonText = "Barrel Punch Mod", method = Overpowered.BarrelPunchMod, toolTip = "Flings people when you punch them."},

                new ButtonInfo { buttonText = "Barrel Fling Gun", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelFlingGun, toolTip = "Flings whoever your hand desires using the barrels."},
                new ButtonInfo { buttonText = "Barrel Fling All", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelFlingAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Flings everyone in the room using the barrels."},

                new ButtonInfo { buttonText = "Barrel Fling Towards Gun", overlapText = "Barrel Bring Gun", aliases = new[] { "Barrel Fling Towards Gun" }, enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelFlingTowardsGun, toolTip = "Flings whoever your hand desires using the barrels towards you."},
                new ButtonInfo { buttonText = "Barrel Fling Towards All", overlapText = "Barrel Bring All", aliases = new[] { "Barrel Fling Towards All" }, enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelFlingTowardsAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Flings everyone in the room using the barrels towards you."},

                new ButtonInfo { buttonText = "Barrel Kick Gun", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelKickGun, toolTip = "Kicks whoever your hand desires using the barrels."},
                new ButtonInfo { buttonText = "Barrel Kick All", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelKickAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Kicks everyone in the room using the barrels."},

                new ButtonInfo { buttonText = "Barrel Crash Gun", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelCrashGun, toolTip = "Crashes whoever your hand desires using the barrels."},
                new ButtonInfo { buttonText = "Barrel Crash All", enableMethod =() => Fun.CheckOwnedThrowable(Overpowered.BarrelIndex), method = Overpowered.BarrelCrashAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Crashes everyone in the room using the barrels."},

                new ButtonInfo { buttonText = "Barrel City Kick Gun", method = Overpowered.CityKickGun, toolTip = "Flings whoever your hand desires using the barrels into the clouds map to kick them."},
                new ButtonInfo { buttonText = "Barrel City Kick All", method = Overpowered.CityKickAll, toolTip = "Flings everyone in the room using the barrels into the clouds map to kick them."},

                new ButtonInfo { buttonText = "Lock Room", method =() => Overpowered.SetRoomStatus(false), isTogglable = false, toolTip = "Locks the room so no one else can join."},
                new ButtonInfo { buttonText = "Unlock Room", method =() => Overpowered.SetRoomStatus(true), isTogglable = false, toolTip = "Unlocks the room so anyone can join."},
                new ButtonInfo { buttonText = "Spaz Room", method =() => { Overpowered.SetRoomStatus(false); Overpowered.SetRoomStatus(true); }, toolTip = "Locks and unlocks the room so people will get kicked when joining."},
                new ButtonInfo { buttonText = "Close Room", enableMethod =() => SerializePatch.OverrideSerialization = () => false, method = Overpowered.CloseRoom, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Kicks everyone in the room." },
                new ButtonInfo { buttonText = "Spy Room", enableMethod =() => Overpowered.CreatePeerBase(), disableMethod =() => Overpowered.UnloadPeerBase(), toolTip = "Allows you to hear people whilst being disconnected from the room." },

                new ButtonInfo { buttonText = "Destroy Gun", method = Overpowered.DestroyGun, toolTip = "Block new players from seeing whoever your hand desires."},
                new ButtonInfo { buttonText = "Destroy All", method = Overpowered.DestroyAll, isTogglable = false, toolTip = "Block new players from seeing everyone."},
                new ButtonInfo { buttonText = "Destroy Aura", method = Overpowered.DestroyAura, toolTip = "Block new players from seeing players nearby you."},
                new ButtonInfo { buttonText = "Destroy On Touch", method = Overpowered.DestroyOnTouch, toolTip = "Block new players from seeing players you touch."},

                new ButtonInfo { buttonText = "Stump Kick Gun", method = Overpowered.StumpKickGun, toolTip = "Kicks whoever your hand desires if they are in stump." },
                new ButtonInfo { buttonText = "Stump Kick All", method = Overpowered.StumpKickAll, isTogglable = false, toolTip = "Kicks everyone in stump." },

                new ButtonInfo { buttonText = "Elevator Kick Gun", method = Overpowered.ElevatorKickGun, toolTip = "Kicks whoever your hand desires if they are in the elevator."},
                new ButtonInfo { buttonText = "Elevator Kick All", method = Overpowered.ElevatorKickAll, isTogglable = false, toolTip = "Kicks everyone in the elevator."},
                new ButtonInfo { buttonText = "Elevator Kick Aura", method = Overpowered.ElevatorKickAura, toolTip = "Kicks players nearby you if they are in the elevator."},
                new ButtonInfo { buttonText = "Elevator Kick On Touch", method = Overpowered.ElevatorKickOnTouch, toolTip = "Kicks players you touch if they are in the elevator."},

                new ButtonInfo { buttonText = "Instant Party", method = Fun.InstantParty, toolTip = "Makes parties form instantly, instead of having to wait a couple of seconds." },
                new ButtonInfo { buttonText = "Leave Party", method =() => FriendshipGroupDetection.Instance.LeaveParty(), isTogglable = false, toolTip = "Leaves the party, incase you can't pull off the string." },
                new ButtonInfo { buttonText = "Party Break Network Triggers", method = Overpowered.PartyBreakNetworkTriggers, toolTip = "Breaks the network triggers for anyone in your party." },

                new ButtonInfo { buttonText = "Party Kick Gun", method = Overpowered.PartyKickGun, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks whoever your hand desires if they're in your party from the room."},
                new ButtonInfo { buttonText = "Party Kick All", method = Overpowered.PartyKickAll, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks everyone in your party from the room."},
                new ButtonInfo { buttonText = "Party Kick Aura", method = Overpowered.PartyKickAura, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks nearby party members from the room."},
                new ButtonInfo { buttonText = "Party Kick On Touch", method = Overpowered.PartyKickOnTouch, disableMethod =() => Overpowered.OptimizeEvents = false, toolTip = "Kicks party members you touch from the room."},

                new ButtonInfo { buttonText = "Kick All in Party", overlapText = "Party Send All", method = Overpowered.KickAllInParty, isTogglable = false, toolTip = "Sends everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Ban All in Party", overlapText = "Party Ban All", method = Overpowered.BanAllInParty, isTogglable = false, toolTip = "Sends everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Auto Party Kick", overlapText = "Auto Party Send", method = Overpowered.AutoPartyKick, toolTip = "When you party, you will automatically send everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Auto Party Ban", overlapText = "Auto Party Ban", method = Overpowered.AutoPartyBan, toolTip = "When you party, you will automatically send everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Break Audio Gun", method = Overpowered.BreakAudioGun, toolTip = "Attempts to break the audio of whoever your hand desires." },
                new ButtonInfo { buttonText = "Break Audio All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Overpowered.BreakAudioAll, toolTip = "Attempts to break everyone's audio when holding trigger." },
            },

            new[] { // Soundboard [18]
                new ButtonInfo { buttonText = "Exit Soundboard", method = () => CurrentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Returns you back to the fun mods." }
            },

            new[] { // Favorite Mods [19]
                new ButtonInfo { buttonText = "Exit Favorite Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new[] { // Menu Presets [20]
                new ButtonInfo { buttonText = "Exit Menu Presets", method =() => CurrentCategoryName = "Menu Settings", isTogglable = false, toolTip = "Returns to the settings for the menu."},

                new ButtonInfo { buttonText = "Legitimate Preset", method = Presets.LegitimatePreset, isTogglable = false, toolTip = "Enables a bunch of mods that make it impossible to mod check you."},
                new ButtonInfo { buttonText = "Goldentrophy Preset", method = Presets.GoldentrophyPreset, isTogglable = false, toolTip = "Enables the mods that \"goldentrophy\" uses."},
                new ButtonInfo { buttonText = "Performance Preset", method = Presets.PerformancePreset, isTogglable = false, toolTip = "Enables some mods that attempt to maximize your FPS as much as possible."},
                new ButtonInfo { buttonText = "Safety Preset", method = Presets.SafetyPreset, isTogglable = false, toolTip = "Enables some mods that attempt to keep you as safe as possible."},
                new ButtonInfo { buttonText = "Ghost Preset", method = Presets.GhostPreset, isTogglable = false, toolTip = "Enables a bunch of mods that are commonly used for ghost trolling."},

                new ButtonInfo { buttonText = "Save Custom Preset 1", method =() => Presets.SaveCustomPreset(1), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 1", method =() => Presets.LoadCustomPreset(1), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 2", method =() => Presets.SaveCustomPreset(2), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 2", method =() => Presets.LoadCustomPreset(2), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 3", method =() => Presets.SaveCustomPreset(3), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 3", method =() => Presets.LoadCustomPreset(3), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 4", method =() => Presets.SaveCustomPreset(4), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 4", method =() => Presets.LoadCustomPreset(4), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 5", method =() => Presets.SaveCustomPreset(5), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 5", method =() => Presets.LoadCustomPreset(5), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 6", method =() => Presets.SaveCustomPreset(6), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 6", method =() => Presets.LoadCustomPreset(6), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 7", method =() => Presets.SaveCustomPreset(7), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 7", method =() => Presets.LoadCustomPreset(7), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 8", method =() => Presets.SaveCustomPreset(8), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 8", method =() => Presets.LoadCustomPreset(8), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 9", method =() => Presets.SaveCustomPreset(9), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 9", method =() => Presets.LoadCustomPreset(9), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Save Custom Preset 10", method =() => Presets.SaveCustomPreset(10), isTogglable = false, toolTip = "Saves a custom preset."},
                new ButtonInfo { buttonText = "Load Custom Preset 10", method =() => Presets.LoadCustomPreset(10), isTogglable = false, toolTip = "Loads a custom preset."},

                new ButtonInfo { buttonText = "Quick Start Mods", method = Presets.SimplePreset, isTogglable = false, toolTip = "Enables some mods that attempt to improve your experience using the menu."},
            },

            new[] { // Advantage Settings [21]
                new ButtonInfo { buttonText = "Exit Advantage Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Instant Tag", enableMethod =() => Advantages.instantTag = true, disableMethod =() => Advantages.instantTag = false, toolTip = "Makes the tag instant."},
                new ButtonInfo { buttonText = "Obnoxious Tag", toolTip = "Makes the tag mods more obnoxious. Instead of hiding in the ground, you teleport around the player like crazy."},
                new ButtonInfo { buttonText = "Visualize Tag Reach", toolTip = "Visualizes the distance threshold for the tag reach."},

                new ButtonInfo { buttonText = "ctaRange", overlapText = "Change Tag Aura Range <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagAuraRange(), enableMethod =() => Advantages.ChangeTagAuraRange(), disableMethod =() => Advantages.ChangeTagAuraRange(false), incremental = true, isTogglable = false, toolTip = "Changes the range of the tag aura mods."},
                new ButtonInfo { buttonText = "ctrRange", overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagReachDistance(), enableMethod =() => Advantages.ChangeTagReachDistance(), disableMethod =() => Advantages.ChangeTagReachDistance(false), incremental = true, isTogglable = false, toolTip = "Changes the range of the tag reach mods."},

                new ButtonInfo { buttonText = "Fake Lag Others", toolTip = "Makes fake lag affect other players' rigs."},
                new ButtonInfo { buttonText = "Disable Fake Lag Self", toolTip = "Excludes yourself from fake lag." },
                new ButtonInfo { buttonText = "Change Fake Lag Strength", overlapText = "Change Fake Lag Strength <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Movement.ChangeFakeLagStrength(), enableMethod =() => Movement.ChangeFakeLagStrength(), disableMethod =() => Movement.ChangeFakeLagStrength(false), incremental = true, isTogglable = false, toolTip = "Changes the ping of the \"Fake Lag\" mod." }
            },

            new[] { // Visual Settings [22]
                new ButtonInfo { buttonText = "Exit Visual Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Performance Visuals Step", overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>0.2</color><color=grey>]</color>", method =() => Visuals.ChangePerformanceModeVisualStep(), enableMethod =() => Visuals.ChangePerformanceModeVisualStep(), disableMethod =() => Visuals.ChangePerformanceModeVisualStep(false), incremental = true, isTogglable = false, toolTip = "Changes the time between rendering visual mods."},
                new ButtonInfo { buttonText = "Performance Visuals", enableMethod =() => Visuals.PerformanceVisuals = true, disableMethod =() => Visuals.PerformanceVisuals = false, toolTip = "Makes visual mods render less often, to increase performange and decrease memory usage."},
                new ButtonInfo { buttonText = "Short Breadcrumbs", toolTip = "Shortens the length of the breadcrumbs."},
                new ButtonInfo { buttonText = "Follow Menu Theme", toolTip = "Makes visual mods match the theme of the menu, rather than the color of the player."},
                new ButtonInfo { buttonText = "Follow Player Colors", toolTip = "Makes the infection tracers appear their normal color instead of orange for tagged players."},
                new ButtonInfo { buttonText = "Transparent Theme", overlapText = "Transparent Visuals", toolTip = "Makes visual mods transparent."},
                new ButtonInfo { buttonText = "Nametag Chams", enableMethod =() => Visuals.nameTagChams = true, disableMethod =() => Visuals.nameTagChams = false, toolTip = "Make name tags show through objects."},
                new ButtonInfo { buttonText = "Show Self Nametag", enableMethod =() => Visuals.selfNameTag = true, disableMethod =() => Visuals.selfNameTag = false, toolTip = "Makes all the name tag mods render for you as well."},
                new ButtonInfo { buttonText = "Hidden on Camera", overlapText = "Streamer Mode Visuals", toolTip = "Makes visual mods only render on VR."},
                new ButtonInfo { buttonText = "Hidden Labels", overlapText = "Streamer Mode Labels", toolTip = "Makes label mods only render on VR."},
                new ButtonInfo { buttonText = "Thin Tracers", toolTip = "Makes the tracers thinner."},
                new ButtonInfo { buttonText = "Smooth Lines", enableMethod =() => smoothLines = true, disableMethod =() => smoothLines = false, toolTip = "Makes every line generated by the menu have smooth ends."},
                new ButtonInfo { buttonText = "Show Cosmetics", overlapText = "Show Cosmetics on Chams", toolTip = "If enabled, the cosmetics will also show through walls."}
            },

            new[] { // Admin Mods (admins only) [23]
                new ButtonInfo { buttonText = "Exit Admin Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Mod Givers", method =() => CurrentCategoryName = "Mod Givers", isTogglable = false, toolTip = "Opens the mod givers page."},

                new ButtonInfo { buttonText = "Get Menu Users", method = Experimental.GetMenuUsers, isTogglable = false, toolTip = "Detects who is using the menu."},
                new ButtonInfo { buttonText = "Auto Get Menu Users", enableMethod =() => NetworkSystem.Instance.OnJoinedRoomEvent += Experimental.GetMenuUsers, disableMethod =() => NetworkSystem.Instance.OnJoinedRoomEvent -= Experimental.GetMenuUsers, isTogglable = true, toolTip = "Detects who is using the menu on room join."},
                new ButtonInfo { buttonText = "Menu User Name Tags", enableMethod = Experimental.EnableAdminMenuUserTags, method = Experimental.AdminMenuUserTags, disableMethod = Experimental.DisableAdminMenuUserTags, toolTip = "Puts nametags on menu users."},
                new ButtonInfo { buttonText = "Conduct Menu Users", enableMethod =() => { Experimental.EnableAdminMenuUserTags(); GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText").GetComponent<TextMeshPro>().text = "CONSOLE USER LIST"; GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>().richText = true; }, method = Experimental.ConsoleOnConduct, toolTip = "Shows menu users on the code of conduct."},
                new ButtonInfo { buttonText = "Menu User Tracers", enableMethod = Experimental.EnableAdminMenuUserTracers, method = Experimental.MenuUserTracers, disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand to menu users."},

                new ButtonInfo { buttonText = "Admin Kick Gun", method = Experimental.AdminKickGun, toolTip = "Kicks whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Kick All", method = Experimental.AdminKickAll, isTogglable = false, toolTip = "Kicks everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Crash Gun", method = Experimental.AdminCrashGun, toolTip = "Crashes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Crash All", method = Experimental.AdminCrashAll, toolTip = "Crashes everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Break Game Gun", method = Experimental.AdminCrashBypassGun, toolTip = "Crashes menu users who attempt to bypass crashers, also breaks the game."},

                new ButtonInfo { buttonText = "Admin Flip Menu Gun", method = Experimental.FlipMenuGun, toolTip = "Flips the menu of whoever your hand desires if they're using the menu."},

                new ButtonInfo { buttonText = "Admin Freeze Gun", method =() => Experimental.AdminFreezeGun(true), toolTip = "Freezes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Unfreeze Gun", method =() => Experimental.AdminFreezeGun(false), toolTip = "Unfreezes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Spaz Freeze Gun", method =() => { Experimental.AdminFreezeGun(true); Experimental.AdminFreezeGun(false); }, toolTip = "Freezes and unfreezes whoever your hand desires if they're using the menu."},

                new ButtonInfo { buttonText = "Admin Mute Gun", method =() => Experimental.AdminEnableGun(true, "Mute Microphone"), toolTip = "Mutes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Unmute Gun", method =() => Experimental.AdminEnableGun(false, "Mute Microphone"), toolTip = "Unmutes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Stutter Voice Gun", method =() => { Experimental.AdminEnableGun(true, "Mute Microphone"); Experimental.AdminEnableGun(false, "Mute Microphone"); }, toolTip = "Stutters the voice of whoever your hand desires by muting and unmuting them if they're using the menu."},

                new ButtonInfo { buttonText = "Admin Jumpscare Gun", method = Experimental.AdminJumpscareGun, toolTip = "Jumpscares whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Jumpscare All", method = Experimental.AdminJumpscareAll, isTogglable = false, toolTip = "Jumpscares everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Mute All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Experimental.AdminMute, toolTip = "Mutes everyone while holding trigger."},

                new ButtonInfo { buttonText = "Admin Board Mute Gun", method =() => Experimental.AdminBMuteGun(true), toolTip = "Mutes whoever your hand desires for everyone using the menu."},
                new ButtonInfo { buttonText = "Admin Board Unmute Gun", method =() => Experimental.AdminBMuteGun(false), toolTip = "Unmutes whoever your hand desires for everyone using the menu."},
                new ButtonInfo { buttonText = "Admin Board Stutter Voice Gun", method =() => { Experimental.AdminBMuteGun(true); Experimental.AdminBMuteGun(false); }, toolTip = "Stutters the voice of whoever your hand desires for everyone using the menu by muting and unmuting them."},

                new ButtonInfo { buttonText = "Admin Board Mute All", method =() => Experimental.AdminBMuteAll(true), isTogglable = false, toolTip = "Mutes everyone for players using the menu."},
                new ButtonInfo { buttonText = "Admin Board Unmute All", method =() => Experimental.AdminBMuteAll(false), isTogglable = false, toolTip = "Unmutes everyone for players using the menu."},
                new ButtonInfo { buttonText = "Admin Board Stutter Voice All", method =() => { Experimental.AdminBMuteAll(true); Experimental.AdminBMuteAll(false); }, toolTip = "Stutters the voice of everyone for players using the menu by muting and unmuting them."},

                new ButtonInfo { buttonText = "Admin Disable Menu Gun", method =() => Experimental.AdminLockdownGun(true), toolTip = "Disables the menu of whoever your hand desires if they're using one."},
                new ButtonInfo { buttonText = "Admin Enable Menu Gun", method =() => Experimental.AdminLockdownGun(false), toolTip = "Enables the menu of whoever your hand desires if they're using one."},

                new ButtonInfo { buttonText = "Admin Disable Menu All", method =() => Experimental.AdminLockdownAll(true), toolTip = "Disables the menu of whoever your hand desires if they're using one."},
                new ButtonInfo { buttonText = "Admin Enable Menu All", method =() => Experimental.AdminLockdownAll(false), toolTip = "Enables the menu of whoever your hand desires if they're using one."},

                new ButtonInfo { buttonText = "Admin Fully Disable Menu Gun", method =() => Experimental.AdminFullLockdownGun(true), toolTip = "Disables the menu of whoever your hand desires and turns off their mods if they're using one."},
                new ButtonInfo { buttonText = "Admin Fully Enable Menu Gun", method =() => Experimental.AdminFullLockdownGun(false), toolTip = "Enables the menu of whoever your hand desires and turns off their mods if they're using one."},

                new ButtonInfo { buttonText = "Admin Fully Disable Menu All", method =() => Experimental.AdminFullLockdownAll(true), isTogglable = false, toolTip = "Disables the menu of whoever your hand desires and turns off their mods if they're using one."},
                new ButtonInfo { buttonText = "Admin Fully Enable Menu All", method =() => Experimental.AdminFullLockdownAll(false), isTogglable = false, toolTip = "Enables the menu of whoever your hand desires and turns off their mods if they're using one."},

                new ButtonInfo { buttonText = "Admin Teleport Gun", method = Experimental.AdminTeleportGun, toolTip = "Teleports whoever using the menu to wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Fling Gun", method = Experimental.AdminFlingGun, toolTip = "Flings whoever your hand desires upwards."},
                new ButtonInfo { buttonText = "Admin Strangle", method = Experimental.AdminStrangle, toolTip = "Strangles whoever you grab if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Fake Cosmetics", overlapText = "Admin Spoof Cosmetics", method =() => Experimental.AdminSpoofCosmetics(), enableMethod =() => { NetworkSystem.Instance.OnPlayerJoined += Experimental.OnPlayerJoinSpoof; Experimental.AdminSpoofCosmetics(true); }, disableMethod =() => { NetworkSystem.Instance.OnPlayerJoined -= Experimental.OnPlayerJoinSpoof; Experimental.oldCosmetics = null; }, toolTip = "Makes everyone using the menu see whatever cosmetics you have on as if you owned them."},

                new ButtonInfo { buttonText = "Admin Lightning Gun", method = Experimental.LightningGun, toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Aura", method = Experimental.LightningAura, toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Rain", method = Experimental.LightningRain, toolTip = "Rains lightning around you and strikes whoever you hit."},

                new ButtonInfo { buttonText = "Admin Laser <color=grey>[</color><color=green>A</color><color=grey>]</color>", method = Experimental.AdminLaser, toolTip = "Shines a red laser out of your hand when holding <color=green>A</color> or <color=green>X</color>."},
                new ButtonInfo { buttonText = "Admin Beam <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Experimental.AdminBeam, toolTip = "Shines a rainbow spinning laser out of your head when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Admin Fractals <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Experimental.AdminFractals, toolTip = "Shines white lines out of your body when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Admin Fear Gun", method = Experimental.AdminFearGun, toolTip = "Sends a person into pure fear and scarefulness."},
                new ButtonInfo { buttonText = "Admin Object Gun", method = Experimental.AdminObjectGun, toolTip = "Spawns an object wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Random Object Gun", method = Experimental.AdminRandomObjectGun, toolTip = "Spawns a random object wherever your hand desires."},

                new ButtonInfo { buttonText = "NotifLabel", overlapText = "No Notification Set", label = true},
                new ButtonInfo { buttonText = "Set Notification", isTogglable = false, method = Experimental.GetTargetNotification, toolTip = "Changes the notification text. The notification text is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Notify Self", isTogglable = false, method = Experimental.NotifySelf, toolTip = "Sends a notification to yourself. The notification text is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Notify Gun", method = Experimental.NotifyGun, toolTip = "Sends a notification to whoever your hand desires. The notification text is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Notify All", isTogglable = false, method = Experimental.NotifyAll, toolTip = "Sends a notification to everyone using the menu. The notification text is based off of what you type into the search bar."},

                new ButtonInfo { buttonText = "Admin Join Gun", enableMethod = Experimental.GetTargetNotification, method = Experimental.JoinGun, toolTip = "Brings whoever your hand desires to a room. The room is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Join All", isTogglable = false, method = Experimental.JoinAll, toolTip = "Brings everyone using the menu to a room. The room is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Network Scale", method = Experimental.AdminNetworkScale, disableMethod = Experimental.UnAdminNetworkScale, toolTip = "Networks your scale to others with the menu."},

                new ButtonInfo { buttonText = "Admin Confirm Notification", method = Experimental.ConfirmNotifyAllUsing, isTogglable = false, toolTip = "Sends a notification to everyone using the menu confirming that you're an admin."},

                new ButtonInfo { buttonText = "Admin Levitate All", method = Experimental.FlyAllUsing, toolTip = "Sends everyone using the menu flying away upwards."},
                new ButtonInfo { buttonText = "Admin Bouncy All", method = Experimental.BouncyAllUsing, toolTip = "Makes everyone using the menu bouncy."},
                new ButtonInfo { buttonText = "Admin Bring Gun", method = Experimental.AdminBringGun, toolTip = "Brings whoever your hand desires to you if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Bring All", method = Experimental.BringAllUsing, toolTip = "Brings everyone using the menu to you."},
                new ButtonInfo { buttonText = "Admin Organize Gun", method = Experimental.AdminOrganizeGun, toolTip = "Brings every menu user into a straight line."},
                new ButtonInfo { buttonText = "Admin Bring Hand All", method = Experimental.BringHandAllUsing, toolTip = "Brings everyone using the menu to your hand."},
                new ButtonInfo { buttonText = "Admin Bring Head All", method = Experimental.BringHeadAllUsing, toolTip = "Brings everyone using the menu to your head."},
                new ButtonInfo { buttonText = "Admin Orbit All", method = Experimental.OrbitAllUsing, toolTip = "Makes everyone using the menu orbit you."},

                new ButtonInfo { buttonText = "Admin Lag Gun", method = Experimental.AdminLagGun, toolTip = "Lags whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Lag All", method = Experimental.AdminLagAll, toolTip = "Lags everyone using the menu."},
                new ButtonInfo { buttonText = "Admin Lag Spike Gun", method = Experimental.AdminLagSpikeGun, toolTip = "Lag spikes whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Lag Spike All", method = Experimental.AdminLagSpikeAll, isTogglable = false, toolTip = "Lag spikes everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Vibrate Gun", method = Experimental.AdminVibrateGun, toolTip = "Vibrate whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Vibrate All", method = Experimental.AdminVibrateAll, isTogglable = false, toolTip = "Vibrates everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Block Gun", method = Experimental.AdminBlockGun, toolTip = "Disables whoever your hand desires from joining servers for 5 minutes if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Announce Block Gun", method =() => Experimental.AdminABlockGun(false), toolTip = "Block gun, but it sends a notification to everyone using the menu that the target was blocked."},
                new ButtonInfo { buttonText = "Silent Announce Block Gun", method =() => Experimental.AdminABlockGun(true), toolTip = "Block gun, but it sends a notification to everyone using the menu that the target was blocked. Hides your name."},

                new ButtonInfo { buttonText = "Admin Open Menu Gun", method =() => Experimental.AdminButtonPressGun("lSecondary"), toolTip = "Force a player to open their menu."},
                new ButtonInfo { buttonText = "Admin Toggle Invis Gun", method =() => Experimental.AdminButtonPressGun("rSecondary"), toolTip = "Force a player to toggle invisibility mod."},

                new ButtonInfo { buttonText = "Admin Punch Mod", method = Experimental.AdminPunchMod, toolTip = "Flings people when you punch them if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Find User", enableMethod =() => { Experimental.EnableAdminMenuUserTags(); Experimental.FindUserTime = Time.time; }, method = Experimental.AdminFindUser, toolTip = "Joins publics until a menu user is found."},

                new ButtonInfo { buttonText = "No Admin Indicator", enableMethod = Experimental.EnableNoAdminIndicator, method = Experimental.NoAdminIndicator, disableMethod = Experimental.AdminIndicatorBack, toolTip = "Disables the cone that appears above your head to others with the menu."},
                new ButtonInfo { buttonText = "Allow Kick Self", enableMethod =() => Console.allowKickSelf = true, disableMethod =() => Console.allowKickSelf = false, toolTip = "Lets other admins kick you."},
                new ButtonInfo { buttonText = "Disable Fling Self", enableMethod =() => Console.disableFlingSelf = true, disableMethod =() => Console.disableFlingSelf = false, toolTip = "Other admins can't fling you."},

                new ButtonInfo { buttonText = "Admin Platform Exclude Gun", method =() => Experimental.AdminPlatToggleGun(true), toolTip = "Puts a player who is included for platform networking to be excluded."},
                new ButtonInfo { buttonText = "Admin Platform Include Gun", method =() => Experimental.AdminPlatToggleGun(false), toolTip = "Puts a player who is excluded for platform networking to be included."},
            },

            new[] { // Enabled Mods [24]
                new ButtonInfo { buttonText = "Exit Enabled Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new[] { // Internal Mods (hidden from user) [25]
                new ButtonInfo { buttonText = "Search", method = Settings.Search, isTogglable = false, toolTip = "Lets you search for specific mods."},
                new ButtonInfo { buttonText = "Global Return", method = Settings.GlobalReturn, isTogglable = false, toolTip = "Returns you to the previous category."},
                new ButtonInfo { buttonText = "Info Screen", method = Settings.Debug, enableMethod = Settings.ShowDebug, disableMethod = Settings.HideDebug, toolTip = "Shows game and modding related information."},
                new ButtonInfo { buttonText = "Donate Button", method =() => { NotificationManager.ClearAllNotifications(); acceptedDonations = true; File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_HideDonationButton.txt", "true"); Prompt("I've spent nearly two years building this menu. Your Patreon support helps me keep it growing, want to check it out?", () => Process.Start("https://patreon.com/iiDk")); }, isTogglable = false, toolTip = "An advertisement for my Patreon." },
                new ButtonInfo { buttonText = "Update Button", method =() => UpdatePrompt(), isTogglable = false, toolTip = "Prompts you to update the menu." },

                new ButtonInfo { buttonText = "Accept Prompt", method =() => { NotificationManager.ClearAllNotifications(); if (inTextInput) Settings.DestroyKeyboard(); CurrentPrompt.AcceptAction?.Invoke(); Settings.StopCurrentPrompt(); }, isTogglable = false},
                new ButtonInfo { buttonText = "Decline Prompt", method =() => { NotificationManager.ClearAllNotifications(); if (inTextInput) Settings.DestroyKeyboard(); CurrentPrompt.DeclineAction?.Invoke(); Settings.StopCurrentPrompt(); }, isTogglable = false}
            },

            new[] { // Sound Library [26]
                new ButtonInfo { buttonText = "Exit Sound Library", method =() => Sound.LoadSoundboard(), isTogglable = false, toolTip = "Returns you back to the soundboard." }
            },

            new[] { // Experimental Mods [27]
                new ButtonInfo { buttonText = "Exit Experimental Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Safe Restart Game", enableMethod =() => { Experimental.restartDelay = Time.time; Experimental.restartIndex = 0; }, method = Experimental.SafeRestartGame, toolTip = "Restarts Gorilla Tag, saving room and position data."},

                new ButtonInfo { buttonText = "Fix Broken Buttons", method = Experimental.FixDuplicateButtons, isTogglable = false, toolTip = "Fixes any duplicate or broken buttons."},

                new ButtonInfo { buttonText = "Get Sound Data", method = Experimental.DumpSoundData, isTogglable = false, toolTip = "Dumps the hand tap sounds to a file."},
                new ButtonInfo { buttonText = "Get Cosmetic Data", method = Experimental.DumpCosmeticData, isTogglable = false, toolTip = "Dumps the cosmetics and their data to a file."},
                new ButtonInfo { buttonText = "Get Decryptable Cosmetic Data", method = Experimental.DecryptableCosmeticData, isTogglable = false, toolTip = "Dumps the cosmetics and their data to a easily decryptable file for databases."},
                new ButtonInfo { buttonText = "Get RPC Data", method = Experimental.DumpRPCData, isTogglable = false, toolTip = "Dumps the data of every RPC to a file."},

                new ButtonInfo { buttonText = "Blank Page", method = Experimental.BlankPage, isTogglable = false, toolTip = "Brings you to a blank category."},

                new ButtonInfo { buttonText = "Copy Custom Gamemode Script", method = Experimental.CopyCustomGamemodeScript, isTogglable = false, toolTip = "Copies the Lua script source code of the current custom map being played."},
                new ButtonInfo { buttonText = "Copy Custom Map ID", method = Experimental.CopyCustomMapID, isTogglable = false, toolTip = "Copies the map ID of the current custom map being played."},

                new ButtonInfo { buttonText = "Better FPS Boost", enableMethod = Experimental.BetterFPSBoost, disableMethod = Experimental.DisableBetterFPSBoost, toolTip = "Makes everything one color, boosting your FPS."},

                new ButtonInfo { buttonText = "Replay Tutorial", method = Settings.ShowTutorial, isTogglable = false, toolTip = "Replays the tutorial video."},
                new ButtonInfo { buttonText = "Disorganize Menu", method = Settings.DisorganizeMenu, isTogglable = false, toolTip = "Disorganizes the entire menu. This cannot be undone."},
            },

            new[] { // Safety Settings [28]
                new ButtonInfo { buttonText = "Exit Safety Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Anti Report Distance", overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeAntiReportRange(), enableMethod =() => Safety.ChangeAntiReportRange(), disableMethod =() => Safety.ChangeAntiReportRange(false), incremental = true, isTogglable = false, toolTip = "Changes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Change FPS Spoof Value", overlapText = "Change FPS Spoof Value <color=grey>[</color><color=green>90</color><color=grey>]</color>", method =() => Safety.ChangeFPSSpoofValue(), enableMethod =() => Safety.ChangeFPSSpoofValue(), disableMethod =() => Safety.ChangeFPSSpoofValue(false), incremental = true, isTogglable = false, toolTip = "Changes the target FPS for the FPS Spoof mod."},
                new ButtonInfo { buttonText = "Change Ping Spoof Value", overlapText = "Change Ping Spoof Value <color=grey>[</color><color=green>200</color><color=grey>]</color>", method =() => Safety.ChangePingSpoofValue(), enableMethod =() => Safety.ChangePingSpoofValue(), disableMethod =() => Safety.ChangePingSpoofValue(false), incremental = true, isTogglable = false, toolTip = "Changes the target ping for the Ping Spoof mod."},

                new ButtonInfo { buttonText = "Hide Anti Cheat Report Reasons", enableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatReasonHide = true, disableMethod =() => AntiCheatPatches.SendReportPatch.AntiCheatReasonHide = false, toolTip = "Hides the reason for Show Anti Cheat Reports."},

                new ButtonInfo { buttonText = "Visualize Anti Report", method = Safety.VisualizeAntiReport, toolTip = "Visualizes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Smart Anti Report", enableMethod = Safety.EnableSmartAntiReport, disableMethod = Safety.DisableSmartAntiReport, toolTip = "Makes the anti report mods only activate in non-modded public lobbies."},
                new ButtonInfo { buttonText = "Anti Mute", enableMethod =() => Safety.antiMute = true, disableMethod =() => Safety.antiMute = false, toolTip = "Includes the mute button with the anti report mods." }
            },

            new ButtonInfo[] { }, // Temporary Category [29]

            new[] { // Soundboard Settings [30]
                new ButtonInfo { buttonText = "Exit Soundboard Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Loop Sounds", enableMethod =() => Sound.LoopAudio = true, disableMethod =() => Sound.LoopAudio = false, toolTip = "Makes sounds loop forever until stopped."},
                new ButtonInfo { buttonText = "Overlap Sounds", enableMethod =() => Sound.OverlapAudio = true, disableMethod =() => Sound.OverlapAudio = false, toolTip = "Makes it so you can play sounds over and over again, making them overlap eachother."},
                new ButtonInfo { buttonText = "Sound Bindings", overlapText = "Sound Bindings <color=grey>[</color><color=green>None</color><color=grey>]</color>", method =() => Sound.SoundBindings(), enableMethod =() => Sound.SoundBindings(), disableMethod =() => Sound.SoundBindings(false), incremental = true, isTogglable = false, toolTip = "Changes the button used to play sounds on the soundboard."},
            },

            new[] { // Overpowered Settings [31]
                new ButtonInfo { buttonText = "Exit Overpowered Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Graphic Punch Mod", toolTip = "Spawns blood projectiles when hitting other players with the allowed punch mods."},

                new ButtonInfo { buttonText = "No Freeze Za Warudo", toolTip = "Disables the freezing on the \"Za Warudo\" mod, turning it into a fun mod." },
                new ButtonInfo { buttonText = "Legacy Kick Freeze", enableMethod =() => Overpowered.legacyKickFreeze = true, disableMethod =() => Overpowered.legacyKickFreeze = false, toolTip = "Makes call overflow related kick methods freeze the rig instead of putting it in the low event state." },

                new ButtonInfo { buttonText = "Change Lag Power", overlapText = "Change Lag Power <color=grey>[</color><color=green>Heavy</color><color=grey>]</color>", method =() => Overpowered.ChangeLagPower(), enableMethod =() => Overpowered.ChangeLagPower(), disableMethod =() => Overpowered.ChangeLagPower(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the lag mods." },
                new ButtonInfo { buttonText = "Change Lag Type", overlapText = "Change Lag Type <color=grey>[</color><color=green>Party</color><color=grey>]</color>", method =() => Overpowered.ChangeLagType(), enableMethod =() => Overpowered.ChangeLagType(), disableMethod =() => Overpowered.ChangeLagType(false), incremental = true, isTogglable = false, toolTip = "Changes the method used to lag players." },

                new ButtonInfo { buttonText = "Master Visualization Type", overlapText = "Master Visualization Type <color=grey>[</color><color=green>Sphere</color><color=grey>]</color>", method =() => Overpowered.MasterVisualizationType(), enableMethod =() => Overpowered.MasterVisualizationType(), disableMethod =() => Overpowered.MasterVisualizationType(false), incremental = true, isTogglable = false, toolTip = "Changes the indicator placed on the master client for mods that show one." },

                new ButtonInfo { buttonText = "Unlock on Crash", toolTip = "Unlocks the room when crashing someone. This makes the mod more powerful." },
                new ButtonInfo { buttonText = "Kick to Public", enableMethod =() => Overpowered.kickToPublic = true, disableMethod =() => Overpowered.kickToPublic = false, toolTip = "Makes the kick mods send the user to a public lobby. This allows for chaining of commands." },
                new ButtonInfo { buttonText = "Kick to Specific Room", enableMethod = Settings.KickToSpecificRoom, disableMethod =() => Overpowered.specificRoom = null, toolTip = "Makes the kick mods send the user to the specific room of your choice." },
                new ButtonInfo { buttonText = "Rejoin on Kick", enableMethod =() => Overpowered.rejoinOnKick = true, disableMethod =() => Overpowered.rejoinOnKick = false, toolTip = "Makes room based kick mods join the room you kicked the target in once they have been kicked." },
                new ButtonInfo { buttonText = "Fast Kick", enableMethod =() => Important.instantCreate = true, disableMethod =() => Important.instantCreate = false, toolTip = "Instantly creates a room instead of checking if one already exists." },
                new ButtonInfo { buttonText = "Kick Fix", enableMethod =() => JoinPatch.enabled = true, disableMethod =() => JoinPatch.enabled = false, toolTip = "Stops the super infection, virtual stump, and other kick mods from breaking." },

                new ButtonInfo { buttonText = "Mute All on Freeze", enableMethod =() => Overpowered.muteOnFreeze = true, disableMethod =() => Overpowered.muteOnFreeze = false, toolTip = "Whenever you freeze the server, everyone will be muted along with it" },
            },

            new[] { // Keybind Settings [32]
                new ButtonInfo { buttonText = "Exit Keybind Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Non-Toggle Keybinds", enableMethod =() => ToggleBindings = false, disableMethod =() => ToggleBindings = true, toolTip = "Enables mods while holding down the button, instead of toggling them."},
                new ButtonInfo { buttonText = "Overwrite Keybinds", enableMethod =() => OverwriteKeybinds = true, disableMethod =() => OverwriteKeybinds = false, toolTip = "Forces every button to be held down with keybinded mods."},
                new ButtonInfo { buttonText = "Clear All Keybinds", method = Settings.ClearAllKeybinds, isTogglable = false, toolTip = "Enables mods while holding down the button, instead of toggling them."},

                new ButtonInfo { buttonText = "Keybind A", enableMethod =() => Settings.StartBind("A"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind B", enableMethod =() => Settings.StartBind("B"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind X", enableMethod =() => Settings.StartBind("X"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Y", enableMethod =() => Settings.StartBind("Y"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Left Grip", enableMethod =() => Settings.StartBind("LG"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Right Grip", enableMethod =() => Settings.StartBind("RG"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Left Trigger", enableMethod =() => Settings.StartBind("LT"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Right Trigger", enableMethod =() => Settings.StartBind("RT"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Left Joystick", enableMethod =() => Settings.StartBind("LJ"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
                new ButtonInfo { buttonText = "Keybind Right Joystick", enableMethod =() => Settings.StartBind("RJ"), disableMethod =() => IsBinding = false, toolTip = "Enables binding mode, letting you bind a mod to a button."},
            },

            new[] { // Plugin Settings [33]
                new ButtonInfo { buttonText = "Exit Plugin Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},
                new ButtonInfo { buttonText = "Reload Plugins", method = PluginManager.ReloadPlugins, isTogglable = false, toolTip = "Reloads all of your plugins." }
            },

            new[] { // Friends [34]
                new ButtonInfo { buttonText = "Exit Friends", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
                new ButtonInfo { buttonText = "Loading...", label = true},
            },

            new[] { // Friend Settings [35]
                new ButtonInfo { buttonText = "Exit Friend Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Disable Rig Networking", enableMethod =() => FriendManager.RigNetworking = false, disableMethod =() => FriendManager.RigNetworking = true, toolTip = "Disables the networking between friends when your rig is disabled."},
                new ButtonInfo { buttonText = "Disable Platform Networking", enableMethod =() => FriendManager.PlatformNetworking = false, disableMethod =() => FriendManager.PlatformNetworking = true, toolTip = "Disables the platform networking between friends."},
                new ButtonInfo { buttonText = "Disable Pinging", enableMethod =() => FriendManager.Pinging = false, disableMethod =() => FriendManager.Pinging = true, toolTip = "Disables the pinging feature between friends."},
                new ButtonInfo { buttonText = "Disable Messaging", enableMethod =() => FriendManager.Messaging = false, disableMethod =() => FriendManager.Messaging = true, toolTip = "Disables the message feature between friends."},
                new ButtonInfo { buttonText = "Disable Friend Sounds", enableMethod =() => FriendManager.SoundEffects = false, disableMethod =() => FriendManager.SoundEffects = true, toolTip = "Disables the sound effects in the friend system."},
                new ButtonInfo { buttonText = "Friend Sided Projectiles", enableMethod =() => Projectiles.friendSided = true, disableMethod =() => Projectiles.friendSided = false, toolTip = "Makes projectiles only appear between friends."},

                new ButtonInfo { buttonText = "Disable Invite Notifications", enableMethod =() => FriendManager.InviteNotifications = false, disableMethod =() => FriendManager.InviteNotifications = true, toolTip = "Disables the prompt and notification when getting an invite from a friend."},
                new ButtonInfo { buttonText = "Disable Preference Sharing", enableMethod =() => FriendManager.PreferenceSharing = false, disableMethod =() => FriendManager.PreferenceSharing = true, toolTip = "Disables the prompt and notification when a friend shares their preferences with you."},

                new ButtonInfo { buttonText = "Physical Platforms", enableMethod =() => FriendManager.PhysicalPlatforms = true, disableMethod =() => FriendManager.PhysicalPlatforms = false, toolTip = "Allows networked platforms to be collided with between friends."},
            },

            new[] { // Fun Settings [36]
                new ButtonInfo { buttonText = "Exit Fun Settings", method =() => CurrentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Head Spin Speed", overlapText = "Change Head Spin Speed <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Fun.ChangeHeadSpinSpeed(), enableMethod =() => Fun.ChangeHeadSpinSpeed(), disableMethod =() => Fun.ChangeHeadSpinSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the head spin mods." },
                new ButtonInfo { buttonText = "Change Tinnitus Hertz", overlapText = "Change Tinnitus Hertz <color=grey>[</color><color=green>6000</color><color=grey>]</color>", method =() => Movement.ChangeTinnitusHz(), enableMethod =() => Movement.ChangeTinnitusHz(), disableMethod =() => Movement.ChangeTinnitusHz(false), incremental = true, isTogglable = false, toolTip = "Changes the target hertz for the tinnitus mods."},

                new ButtonInfo { buttonText = "Zero Gravity Bugs", toolTip = "Removes the gravity from the bugs on the Bug Spam mod."},
                new ButtonInfo { buttonText = "Bug Colliders", toolTip = "Gives the bug colliders on the Bug Spam mod."},
                new ButtonInfo { buttonText = "Bouncy Bug", toolTip = "Makes the bug bounce off of surfaces if using the bug colliders setting on the Bug Spam mod."},

                new ButtonInfo { buttonText = "Change Custom Quest Score", overlapText = "Change Custom Quest Score <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Fun.ChangeCustomQuestScore(), enableMethod =() => Fun.ChangeCustomQuestScore(), disableMethod =() => Fun.ChangeCustomQuestScore(false), incremental = true, isTogglable = false, toolTip = "Changes the value of the \"Custom Quest Score\" mod." },

                new ButtonInfo { buttonText = "Change Ranked Tier", overlapText = "Change Matchmaking Tier <color=grey>[</color><color=green>High</color><color=grey>]</color>", method =() => Safety.ChangeRankedTier(), enableMethod =() => Safety.ChangeRankedTier(), disableMethod =() => Safety.ChangeRankedTier(false), incremental = true, isTogglable = false, toolTip = "Changes the target tier for the matchmaking spoof mod."},
                new ButtonInfo { buttonText = "Change ELO Value", overlapText = "Change ELO Value <color=grey>[</color><color=green>4000</color><color=grey>]</color>", method =() => Safety.ChangeELOValue(), enableMethod =() => Safety.ChangeELOValue(), disableMethod =() => Safety.ChangeELOValue(false), incremental = true, isTogglable = false, toolTip = "Changes the target ELO for the badge spoof mod."},
                new ButtonInfo { buttonText = "Change Badge Tier", overlapText = "Change Badge Tier <color=grey>[</color><color=green>Banana</color><color=grey>]</color>", method =() => Safety.ChangeBadgeTier(), enableMethod =() => Safety.ChangeBadgeTier(), disableMethod =() => Safety.ChangeBadgeTier(false), incremental = true, isTogglable = false, toolTip = "Changes the target tier for the badge spoof mod."},

                new ButtonInfo { buttonText = "Change Target FOV", overlapText = "Change Target FOV <color=grey>[</color><color=green>90</color><color=grey>]</color>", method =() => Fun.ChangeTargetFOV(), enableMethod =() => Fun.ChangeTargetFOV(), disableMethod =() => Fun.ChangeTargetFOV(false), incremental = true, isTogglable = false, toolTip = "Changes the target field of view for the \"Camera FOV\" mod."},
                new ButtonInfo { buttonText = "Knockback Multiplication Amount", overlapText = "Knockback Multiplication Amount <color=grey>[</color><color=green>1.5</color><color=grey>]</color>", method =() => Movement.MultiplicationAmount(), enableMethod =() => Movement.MultiplicationAmount(), disableMethod =() => Movement.MultiplicationAmount(false), incremental = true, isTogglable = false, toolTip = "Adjusts how much your knockback is multiplied."},

                new ButtonInfo { buttonText = "Zero Gravity Blocks", toolTip = "Removes the gravity from the blocks."},
                new ButtonInfo { buttonText = "Random Block Type", toolTip = "Selects a random block when using block mods."},

                new ButtonInfo { buttonText = "No Random Position Grab", toolTip = "Disables the position randomization in the \"Grab All ### Blocks\" mods."},
                new ButtonInfo { buttonText = "No Random Rotation Grab", toolTip = "Disables the rotation randomization in the \"Grab All ### Blocks\" mods."},

                new ButtonInfo { buttonText = "Change Block Delay", overlapText = "Change Block Delay <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Fun.ChangeBlockDelay(), enableMethod =() => Fun.ChangeBlockDelay(), disableMethod =() => Fun.ChangeBlockDelay(false), incremental = true, isTogglable = false, toolTip = "Gives the blocks a delay before spawning." },
                new ButtonInfo { buttonText = "Change Cycle Delay", overlapText = "Change Name Cycle Delay <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Fun.ChangeCycleDelay(), enableMethod =() => Fun.ChangeCycleDelay(), disableMethod =() => Fun.ChangeCycleDelay(false), incremental = true, isTogglable = false, toolTip = "Changes the delay on name cycle mods." },

                new ButtonInfo { buttonText = "Entity Gravity", toolTip = "Gives gravity to any spawned entities in the ghost reactor or Super Infection gamemode."},

                new ButtonInfo { buttonText = "Tinnitus Self", enableMethod =() => Movement.tinnitusSelf = true, disableMethod =() => Movement.tinnitusSelf = false, toolTip = "Be able to hear the loud beep the menu creates with this mod on. God save your ears."},
            },

            new[] { // Players [37]
                new ButtonInfo { buttonText = "Exit Players", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." }
            },

            new[] { // Credits [38]
                new ButtonInfo { buttonText = "Exit Credits", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." },

                new ButtonInfo { buttonText = "iiDk", method =() => Process.Start("https://github.com/iiDk-the-actual"), isTogglable = false, toolTip = "iiDk is the main developer of ii's <b>Stupid</b> Menu, and has been working on it since 2023. He is also the owner of ii's Stupid Mods."},
                new ButtonInfo { buttonText = "Kingofnetflix", method =() => Process.Start("https://github.com/kingofnetflix"), isTogglable = false, toolTip = "Kingofnetflix is a developer for ii's <b>Stupid</b> Menu. Creating mods since 2022, he's been very impactful towards this menu."},
                new ButtonInfo { buttonText = "Twigcore", method =() => Process.Start("https://github.com/Twigcore"), isTogglable = false, toolTip = "Twigcore is one of the main developers of Console, the admin system in the menu. He helps create assets, moderate users, and give me ideas."},

                new ButtonInfo { buttonText = "Joseph", method =() => Process.Start("https://github.com/josephabyt"), isTogglable = false, toolTip = "Joseph is a contributor of ii's <b>Stupid</b> Menu. He is the creator of many mods, like the debug screen, extenders, disable menu title, steam refund timer, and many more."},
                new ButtonInfo { buttonText = "Tagdoesnothing", method =() => Process.Start("https://github.com/JuanLeoson"), isTogglable = false, toolTip = "Tag is a contributor of ii's <b>Stupid</b> Menu. She fixes small bugs and helps bug test the menu."},
                new ButtonInfo { buttonText = "DrPerky", method =() => Process.Start("https://github.com/DrPerkyLegit"), isTogglable = false, toolTip = "DrPerky is a contributor of ii's <b>Stupid</b> Menu. He helped me rewrite all of the visual mods."},
                new ButtonInfo { buttonText = "ShibaGT", method =() => Process.Start("https://github.com/ShibaGT"), isTogglable = false, toolTip = "ShibaGT is a contributor of ii's <b>Stupid</b> Menu. He gave me a coroutine manager, and creates minor things for the menu."},

                new ButtonInfo { buttonText = "TestofficialXD", method =() => Process.Start("https://github.com/TestofficialXD"), isTogglable = false, toolTip = "TestofficialXD is a contributor of ii's <b>Stupid</b> Menu. He wrote the initial critter mods, and gave me the idea for Tag Sounds."},
                new ButtonInfo { buttonText = "Leetus", method =() => Process.Start("https://github.com/leetus"), isTogglable = false, toolTip = "Leetus is a contributor of ii's <b>Stupid</b> Menu. He makes minor optimizations to mods in the menu."},

                new ButtonInfo { buttonText = "Graze", method =() => Process.Start("https://github.com/The-Graze"), isTogglable = false, toolTip = "Graze gave me permission to use their color detection system and Media Control buttons."},
                new ButtonInfo { buttonText = "Zvbex", method =() => Process.Start("https://guns.lol/zvbexisking"), isTogglable = false, toolTip = "Zvbex gave me permission to use their initial platform detection system."},
                new ButtonInfo { buttonText = "Shiny", method =() => Process.Start("https://github.com/Shiny003"), isTogglable = false, toolTip = "Shiny gave me permission to use their PlayFab display name spoof patch."},

                new ButtonInfo { buttonText = "Will", method =() => Process.Start("https://github.com/64will64"), isTogglable = false, toolTip = "Will gave me the idea to make body rotation mods."},
                new ButtonInfo { buttonText = "KyleTheScientist", method =() => Process.Start("https://github.com/KyleTheScientist"), isTogglable = false, toolTip = "KyleTheScientist gave me the idea to add \"Bark Fly\" to the menu and helped me create and use asset bundles."},
                new ButtonInfo { buttonText = "Gorilla Dev", method =() => Process.Start("https://github.com/GorillerDev"), isTogglable = false, toolTip = "Gorilla Dev gave me the idea to add \"Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>\" to the menu."},
                new ButtonInfo { buttonText = "EyeCantSee", method =() => Process.Start("https://github.com/charlottebutson-pixel"), isTogglable = false, toolTip = "EyeCantSee has pushed minor optimizations and features to the menu."},

                new ButtonInfo { buttonText = "GPL v3", method =() => Process.Start("https://www.gnu.org/licenses/gpl-3.0.html"), isTogglable = false, toolTip = "The GNU General Public License Version 3 is the license that my menu uses. It proveides a \"free, copyleft license for software and other kinds of works.\""},
            },

            new[] // Custom Maps [39]
            {
                new ButtonInfo { buttonText = "Exit Custom Maps", method =() => CurrentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Returns you back to the fun mods."},
                new ButtonInfo { buttonText = "You have not loaded a map.", label = true }
            },

            new[] // Admin Mod Givers [40]
            {
                new ButtonInfo { buttonText = "Exit Admin Mod Givers", method =() => CurrentCategoryName = "Admin Mods", isTogglable = false, toolTip = "Returns you back to the Admin mods."},
                new ButtonInfo { buttonText = "Give Fly Gun", method = Experimental.AdminGiveFlyGun, toolTip = "Gives whoever you want fly when they hold their right thumb down if they're using console."},
                new ButtonInfo { buttonText = "Give Trigger Fly Gun", method = Experimental.AdminGiveTriggerFlyGun, toolTip = "Gives whoever you want fly when they hold their trigger down if they're using console."},
                new ButtonInfo { buttonText = "Give Speed Boost Gun", method = Experimental.AdminGiveSpeedGun, toolTip = "Gives whoever you want speed boost if they're using console."},
                new ButtonInfo { buttonText = "Give Low Gravity Gun", method = Experimental.AdminGiveLowGravity, toolTip = "Gives whoever you want low gravity if they're using console."},
                new ButtonInfo { buttonText = "Give Platforms Gun", method = Experimental.AdminGivePlatforms, toolTip = "Gives whoever you want platforms if they're using console."},
            },

            new ButtonInfo[] { }, // Chat Messages [41] 

            new[] // Macros [42]
            {
                new ButtonInfo { buttonText = "Exit Macros", method =() => CurrentCategoryName = "Movement Mods", isTogglable = false, toolTip = "Returns you back to the movement mods." },
                new ButtonInfo { buttonText = "Record <color=grey>[</color><color=green>T</color><color=grey>]</color>", method = Movement.RecordMacro, toolTip = "Record your macros with your <color=green>left trigger</color>." },
                new ButtonInfo { buttonText = "Reload Macros", method = Movement.LoadMacros, isTogglable = false, toolTip = "Reloads your macros." },
                new ButtonInfo { buttonText = "Disable Macros", enableMethod =() => Movement.disableMacros = true, disableMethod =() => Movement.disableMacros = false, toolTip = "Disables all macros." }
            },

            new[] // Detected Mods [43]
            {
                new ButtonInfo { buttonText = "Exit Detected Mods", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Detected Auto Set Master Client", overlapText = "Auto Set Master Client", method = Detected.AutoSetMasterClient, detected = true, toolTip = "Automatically sets you as master client."},
                new ButtonInfo { buttonText = "Detected Set Master Client Self", overlapText = "Set Master Client Self", method =() => PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer), isTogglable = false, detected = true, toolTip = "Sets you as master client."},
                new ButtonInfo { buttonText = "Detected Set Master Client Gun", overlapText = "Set Master Client Gun", method = Detected.SetMasterClientGun, detected = true, toolTip = "Sets whoever your hand desires as master client."},
                new ButtonInfo { buttonText = "Detected Set Master Client All", overlapText = "Set Master Client All", method = Detected.SetMasterClientAll, detected = true, toolTip = "Sets everyone in the room as master client."},
                new ButtonInfo { buttonText = "Detected Set Master Client Aura", overlapText = "Set Master Client Aura", method = Detected.SetMasterClientAura, detected = true, toolTip = "Sets nearby players as master client."},
                new ButtonInfo { buttonText = "Detected Set Master Client On Touch", overlapText = "Set Master Client On Touch", method = Detected.SetMasterClientOnTouch, detected = true, toolTip = "Sets players you touch as master client."},

                new ButtonInfo { buttonText = "Detected Lag Gun", overlapText = "Lag Gun", method = Detected.LagGun, detected = true, toolTip = "Lags whoever your hand desires."},
                new ButtonInfo { buttonText = "Detected Lag All", overlapText = "Lag All", method = Detected.LagAll, detected = true, toolTip = "Lags everyone in the room."},
                new ButtonInfo { buttonText = "Detected Lag Aura", overlapText = "Lag Aura", method = Detected.LagAura, detected = true, toolTip = "Lags players nearby you."},
                new ButtonInfo { buttonText = "Detected Lag On Touch", overlapText = "Lag On Touch", method = Detected.LagOnTouch, detected = true, toolTip = "Lags players that you touch."},

                new ButtonInfo { buttonText = "Detected Crash Gun", overlapText = "Crash Gun", method = Detected.CrashGun, detected = true, toolTip = "Crashes whoever your hand desires."},
                new ButtonInfo { buttonText = "Detected Crash All", overlapText = "Crash All", method = Detected.CrashAll, detected = true, toolTip = "Crashes everyone in the room."},
                new ButtonInfo { buttonText = "Detected Crash Aura", overlapText = "Crash Aura", method = Detected.CrashAura, detected = true, toolTip = "Crashes players nearby you."},
                new ButtonInfo { buttonText = "Detected Crash On Touch", overlapText = "Crash On Touch", method = Detected.CrashOnTouch, detected = true, toolTip = "Crashes players that you touch."},
                new ButtonInfo { buttonText = "Detected Crash When Touched", overlapText = "Crash When Touched", method = Detected.CrashWhenTouched, detected = true, toolTip = "Crashes players that touch you."},

                new ButtonInfo { buttonText = "Detected Mute Gun", overlapText = "Mute Gun", method = Detected.MuteGun, detected = true, toolTip = "Mutes whoever your hand desires."},
                new ButtonInfo { buttonText = "Detected Mute All", overlapText = "Mute All", method = Detected.MuteAll, detected = true, toolTip = "Mutes everyone in the room."},
                new ButtonInfo { buttonText = "Detected Mute Aura", overlapText = "Mute Aura", method = Detected.MuteAura, detected = true, toolTip = "Mutes players nearby you."},
                new ButtonInfo { buttonText = "Detected Mute On Touch", overlapText = "Mute On Touch", method = Detected.MuteOnTouch, detected = true, toolTip = "Mutes players that you touch."},

                new ButtonInfo { buttonText = "Detected Ghost Gun", overlapText = "Ghost Gun", method = Detected.GhostGun, detected = true, toolTip = "Freezes whoever your hand desires, making them a ghost."},
                new ButtonInfo { buttonText = "Detected Ghost All", overlapText = "Ghost All", method = Detected.GhostAll, isTogglable = false, detected = true, toolTip = "Freezes everyone, making them a ghost."},
                new ButtonInfo { buttonText = "Detected Ghost Aura", overlapText = "Ghost Aura", method = Detected.GhostAura, isTogglable = true, detected = true, toolTip = "Freezes nearby players, making them a ghost."},
                new ButtonInfo { buttonText = "Detected Ghost On Touch", overlapText = "Ghost On Touch", method = Detected.GhostOnTouch, detected = true, toolTip = "Freezes players you touch, making them a ghost."},
                
                new ButtonInfo { buttonText = "Detected Unghost Gun", overlapText = "Unghost Gun", method = Detected.UnghostGun, detected = true, toolTip = "Unfreezes whoever your hand desires, making them no longer a ghost."},
                new ButtonInfo { buttonText = "Detected Unghost All", overlapText = "Unghost All", method = Detected.UnghostAll, isTogglable = false, detected = true, toolTip = "Unfreezes everyone, making them no longer a ghost."},
                new ButtonInfo { buttonText = "Detected Unghost Aura", overlapText = "Unghost Aura", method = Detected.UnghostAura, isTogglable = true, detected = true, toolTip = "Unfreezes players nearby you, making them no longer a ghost."},
                new ButtonInfo { buttonText = "Detected Unghost On Touch", overlapText = "Unghost On Touch", method = Detected.UnghostOnTouch, detected = true, toolTip = "Unfreeze players that you touch, making them no longer a ghost."},

                new ButtonInfo { buttonText = "Detected Spam Ghost Gun", overlapText = "Spam Ghost Gun", method =() => { Detected.GhostGun(); Detected.UnghostGun(); }, detected = true, toolTip = "Spam makes whoever your hand desires freeze and unfreeze again. Ghost and Unghost."},
                new ButtonInfo { buttonText = "Detected Spam Ghost All", overlapText = "Spam Ghost All", method =() => { Detected.GhostAll(); Detected.UnghostAll(); }, isTogglable = false, detected = true, toolTip = "Spam makes everyone freeze and unfreeze again. Ghost and Unghost."},
                new ButtonInfo { buttonText = "Detected Spam Ghost Aura", overlapText = "Spam Ghost Aura", method =() => { Detected.GhostAura(); Detected.UnghostAura(); }, isTogglable = true, detected = true, toolTip = "Spam makes players nearby freeze and unfreeze again. Ghost and Unghost."},
                new ButtonInfo { buttonText = "Detected Spam Ghost On Touch", overlapText = "Spam Ghost On Touch", method =() => { Detected.GhostOnTouch(); Detected.UnghostOnTouch(); }, detected = true, toolTip = "Spam makes players you touch freeze and unfreeze again. Ghost and Unghost."},

                new ButtonInfo { buttonText = "Leaderboard Ghost", method = Detected.LeaderboardGhost, disableMethod = Detected.DisableLeaderboardGhost, detected = true, toolTip = "Ghosts players when you report them on the leaderboard."},
                new ButtonInfo { buttonText = "Leaderboard Mute", method = Detected.LeaderboardMute, detected = true, toolTip = "Mutes players when you mute them on the leaderboard."},
                
                new ButtonInfo { buttonText = "Detected Isolate Gun", overlapText = "Isolate Gun", method = Detected.IsolateGun, detected = true, toolTip = "Makes whoever your hand desires only be able to see you."},
                new ButtonInfo { buttonText = "Detected Isolate All", overlapText = "Isolate All", method = Detected.IsolateAll, isTogglable = false, detected = true, toolTip = "Makes everyone only be able to see you."},
                new ButtonInfo { buttonText = "Detected Isolate Aura", overlapText = "Isolate Aura", method = Detected.IsolateAura, detected = true, toolTip = "Makes players nearby only be able to see you."},
                new ButtonInfo { buttonText = "Detected Isolate On Touch", overlapText = "Isolate On Touch", method = Detected.IsolateOnTouch, detected = true, toolTip = "Players that you touch will only be able to see you."},

                new ButtonInfo { buttonText = "Detected Change Name Gun", overlapText = "Change Name Gun", enableMethod = Detected.PromptNameChange, method = Detected.ChangeNameGun, detected = true, toolTip = "Changes the name of whoever your hand desires."},
                new ButtonInfo { buttonText = "Detected Change Name All", overlapText = "Change Name All", enableMethod = Detected.PromptNameChange, method = Detected.ChangeNameAll, detected = true, toolTip = "Changes the name of everyone in the room."},
                new ButtonInfo { buttonText = "Detected Change Name Aura", overlapText = "Change Name Aura", enableMethod = Detected.PromptNameChange, method = Detected.ChangeNameAura, detected = true, toolTip = "Changes the name of whoever is near you."},
                new ButtonInfo { buttonText = "Detected Change Name On Touch", overlapText = "Change Name On Touch", enableMethod = Detected.PromptNameChange, method = Detected.ChangeNameOnTouch, detected = true, toolTip = "Changes the name of players that you touch."},

                new ButtonInfo { buttonText = "Detected Ban Gun", overlapText = "Ban Gun", method = Detected.BanGun, detected = true, toolTip = "Changes the name of whoever your hand desires to a banned name."},
                new ButtonInfo { buttonText = "Detected Ban All", overlapText = "Ban All", method = Detected.BanGun, detected = true, toolTip = "Changes the name of everyone in the room to a banned name."},
                new ButtonInfo { buttonText = "Detected Ban Aura", overlapText = "Ban Aura", method = Detected.BanAura, detected = true, toolTip = "Changes the name of whoever is near you to a banned name."},
                new ButtonInfo { buttonText = "Detected Ban On Touch", overlapText = "Ban On Touch", method = Detected.BanOnTouch, detected = true, toolTip = "Changes the name of players that you touch to a banned name."},

                new ButtonInfo { buttonText = "Bypass Mod Checkers Gun", method = Detected.BypassModCheckersGun, detected = true, toolTip = "Tells players using mod checkers that whoever your hand desires has no mods."},
                new ButtonInfo { buttonText = "Bypass Mod Checkers All", method = Detected.BypassModCheckersAll, isTogglable = false, detected = true, toolTip = "Tells players using mod checkers that no one has no mods."},
                new ButtonInfo { buttonText = "Bypass Mod Checkers Aura", method = Detected.BypassModCheckersAura, detected = true, toolTip = "Tells players using mod checkers that players nearby you have no mods."},
                new ButtonInfo { buttonText = "Bypass Mod Checkers On Touch", method = Detected.BypassModCheckersOnTouch, detected = true, toolTip = "Tells players using mod checkers that players you touch have no mods."},

                new ButtonInfo { buttonText = "Break Mod Checkers Gun", method = Detected.BreakModCheckersGun, detected = true, toolTip = "Tells players using mod checkers that whoever your hand desires has every mod."},
                new ButtonInfo { buttonText = "Break Mod Checkers All", method = Detected.BreakModCheckersAll, isTogglable = false, detected = true, toolTip = "Tells players using mod checkers that everyone has every mod."},
                new ButtonInfo { buttonText = "Break Mod Checkers Aura", method = Detected.BreakModCheckersAura, detected = true, toolTip = "Tells players using mod checkers that players nearby you have every mod."},
                new ButtonInfo { buttonText = "Break Mod Checkers On Touch", method = Detected.BreakModCheckersOnTouch, detected = true, toolTip = "Tells players using mod checkers that players you touch have every mod."},

                new ButtonInfo { buttonText = "Gamemode Include Gun", method = Detected.GamemodeIncludeGun, detected = true, toolTip = "Includes whoever your hand desires from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Include All", method = Detected.GamemodeIncludeAll, isTogglable = false, detected = true, toolTip = "Includes everyone from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Include Aura", method = Detected.GamemodeIncludeAura, detected = true, toolTip = "Includes players nearby you from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Include On Touch", method = Detected.GamemodeIncludeOnTouch, detected = true, toolTip = "Includes players you touch from the current gamemode."},

                new ButtonInfo { buttonText = "Gamemode Exclude Gun", method = Detected.GamemodeExcludeGun, detected = true, toolTip = "Excludes whoever your hand desires from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Exclude All", method = Detected.GamemodeExcludeAll, isTogglable = false, detected = true, toolTip = "Excludes everyone from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Exclude Aura", method = Detected.GamemodeExcludeAura, detected = true, toolTip = "Excludes players nearby you from the current gamemode."},
                new ButtonInfo { buttonText = "Gamemode Exclude On Touch", method = Detected.GamemodeExcludeOnTouch, detected = true, toolTip = "Excludes players you touch from the current gamemode."},
                
                new ButtonInfo { buttonText = "Break Network Triggers", method = Detected.BreakNetworkTriggers, isTogglable = false, detected = true, toolTip = "Breaks the network triggers."},
                new ButtonInfo { buttonText = "Kick Network Triggers", method = Detected.KickNetworkTriggers, isTogglable = false, detected = true, toolTip = "Makes all network triggers kick you."},

                new ButtonInfo { buttonText = "Spaz Gamemode", method = Detected.SpazGamemode, detected = true, toolTip = "Rapidly changes the gamemode."},
                new ButtonInfo { buttonText = "Break Gamemode", enableMethod =() => Detected.BreakGamemode(true), disableMethod =() => Detected.BreakGamemode(false), detected = true, toolTip = "Breaks the current gamemode."},
                new ButtonInfo { buttonText = "Change Gamemode to None", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.None), isTogglable = false, detected = true, toolTip = "Changes the gamemode to error/none."},
                new ButtonInfo { buttonText = "Change Gamemode to Count", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Count), isTogglable = false, detected = true, toolTip = "Changes the gamemode to count."},
                new ButtonInfo { buttonText = "Change Gamemode to Casual", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Casual), isTogglable = false, detected = true, toolTip = "Changes the gamemode to casual."},
                new ButtonInfo { buttonText = "Change Gamemode to Infection", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Infection), isTogglable = false, detected = true, toolTip = "Changes the gamemode to infection."},
                new ButtonInfo { buttonText = "Change Gamemode to Competitive Infection", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.InfectionCompetitive), isTogglable = false, detected = true, toolTip = "Changes the gamemode to competitive infection."},
                new ButtonInfo { buttonText = "Change Gamemode to Super Infection", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.SuperInfect), isTogglable = false, detected = true, toolTip = "Changes the gamemode to super infection."},
                new ButtonInfo { buttonText = "Change Gamemode to Super Casual", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.SuperCasual), isTogglable = false, detected = true, toolTip = "Changes the gamemode to super casual."},
                new ButtonInfo { buttonText = "Change Gamemode to Hunt", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.HuntDown), isTogglable = false, detected = true, toolTip = "Changes the gamemode to hunt."},
                new ButtonInfo { buttonText = "Change Gamemode to Paintbrawl", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Paintbrawl), isTogglable = false, detected = true, toolTip = "Changes the gamemode to paintbrawl."},
                new ButtonInfo { buttonText = "Change Gamemode to Ambush", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Ambush), isTogglable = false, detected = true, toolTip = "Changes the gamemode to ambush."},
                new ButtonInfo { buttonText = "Change Gamemode to Ghost Tag", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Ghost), isTogglable = false, detected = true, toolTip = "Changes the gamemode to ghost tag."},
                new ButtonInfo { buttonText = "Change Gamemode to Guardian", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Guardian), isTogglable = false, detected = true, toolTip = "Changes the gamemode to guardian."},
                new ButtonInfo { buttonText = "Change Gamemode to Freeze Tag", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.FreezeTag), isTogglable = false, detected = true, toolTip = "Changes the gamemode to freeze tag."},
                new ButtonInfo { buttonText = "Change Gamemode to Prop Hunt", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.PropHunt), isTogglable = false, detected = true, toolTip = "Changes the gamemode to prop hunt."},
                new ButtonInfo { buttonText = "Change Gamemode to Custom", method =() => Detected.ChangeGamemode(GorillaGameModes.GameModeType.Custom), isTogglable = false, detected = true, toolTip = "Changes the gamemode to custom."}
            },

            new[] // Detected Settings [44]
            {
                new ButtonInfo { buttonText = "Exit Detected Settings", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Switch to Modded Gamemode", enableMethod =() => Detected.moddedGamemode = true, disableMethod =() => Detected.moddedGamemode = false, toolTip = "Automatically sets the gamemode as modded when changed."},
                new ButtonInfo { buttonText = "Isolate Others", toolTip = "Allows you to still be seen when isolating players."}
            },

            new[] // Achievements [45]
            {
                new ButtonInfo { buttonText = "Exit Achievements", method = () => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." }
            },

            new[] // Mod List [46]
            {
                new ButtonInfo { buttonText = "Exit Mod List", method = () => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." }
            },

            new[] // Patreon Mods [47]
            {
                new ButtonInfo { buttonText = "Exit Patreon Mods", method = () => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." },
                new ButtonInfo { buttonText = "No Patreon Indicator", enableMethod =() => PatreonManager.ShowIndicator(true), method = PatreonManager.ConstantHideIndicator, disableMethod =() => PatreonManager.ShowIndicator(false), toolTip = "Disables the membership that appears above your head to others with the menu."}
            },

            new[] // Patreon Settings [48]
            {
                new ButtonInfo { buttonText = "Exit Patreon Settings", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
                new ButtonInfo { buttonText = "Disable Patreon Indicators", enableMethod =() => PatreonManager.IndicatorsEnabled = false, disableMethod =() => PatreonManager.IndicatorsEnabled = true, toolTip = "Disables the memberships that appear above people's head with the menu."}
            }
        };

        public static string[] categoryNames = {
            "Main",
            "Settings",
            "Menu Settings",
            "Room Settings",
            "Movement Settings",
            "Projectile Settings",
            "Room Mods",
            "Important Mods",
            "Safety Mods",
            "Movement Mods",
            "Advantage Mods",
            "Visual Mods",
            "Fun Mods",
            "Rebind Settings",
            "Sound Mods",
            "Projectile Mods",
            "Master Mods",
            "Overpowered Mods",
            "Soundboard",
            "Favorite Mods",
            "Menu Presets",
            "Advantage Settings",
            "Visual Settings",
            "Admin Mods",
            "Enabled Mods",
            "Internal Mods",
            "Sound Library",
            "Experimental Mods",
            "Safety Settings",
            "Temporary Category",
            "Soundboard Settings",
            "Overpowered Settings",
            "Keybind Settings",
            "Plugin Settings",
            "Friends",
            "Friend Settings",
            "Fun Settings",
            "Players",
            "Credits",
            "Custom Maps",
            "Mod Givers",
            "Chat Messages",
            "Macros",
            "Detected Mods",
            "Detected Settings",
            "Achievements",
            "Mod List",
            "Patreon Mods",
            "Patreon Settings"
        };

        public static int _currentCategoryIndex;
        public static event Action OnCategoryChanged;

        public static int CurrentCategoryIndex
        {
            get => _currentCategoryIndex;
            set
            {
                _currentCategoryIndex = value;
                pageNumber = 0;
                pageOffset = 0;

                OnCategoryChanged?.Invoke();
            }
        }

        public static string CurrentCategoryName
        {
            get => Buttons.categoryNames[CurrentCategoryIndex];
            set =>
                CurrentCategoryIndex = Buttons.GetCategory(value);
        }

        private static readonly Dictionary<string, (int Category, int Index)> cacheGetIndex = new Dictionary<string, (int Category, int Index)>(); // Looping through 800 elements is not a light task :/

        /// <summary>
        /// Returns the ButtonInfo for the given button text.
        /// </summary>
        /// <param name="buttonText">Button Name</param>
        /// <returns>Button</returns>
        public static ButtonInfo GetIndex(string buttonText)
        {
            if (buttonText == null)
                return null;

            if (cacheGetIndex.TryGetValue(buttonText, out var cacheData))
            {
                try
                {
                    if (buttons[cacheData.Category][cacheData.Index].buttonText == buttonText)
                        return buttons[cacheData.Category][cacheData.Index];
                }
                catch { cacheGetIndex.Remove(buttonText); }
            }

            int categoryIndex = 0;
            foreach (ButtonInfo[] buttons in buttons)
            {
                int buttonIndex = 0;
                foreach (ButtonInfo button in buttons)
                {
                    if (button.buttonText == buttonText)
                    {
                        try
                        {
                            cacheGetIndex.Add(buttonText, (categoryIndex, buttonIndex));
                        }
                        catch
                        {
                            cacheGetIndex.Remove(buttonText);
                        }

                        return button;
                    }
                    buttonIndex++;
                }
                categoryIndex++;
            }

            return null;
        }

        /// <summary>
        /// Returns the category index for the given category name.
        /// </summary>
        /// <param name="categoryName">Category Name</param>
        /// <returns>Category Index</returns>
        public static int GetCategory(string categoryName) =>
            categoryNames.ToList().IndexOf(categoryName);

        /// <summary>
        /// Adds a category to the button list.
        /// </summary>
        /// <remarks>
        /// A button will not be automatically added to the main category. It must be manually created with <see cref="AddButton(int, ButtonInfo, int)"/>
        /// </remarks>
        /// <param name="categoryName">Category Name</param>
        /// <returns>Category Index</returns>
        public static int AddCategory(string categoryName)
        {
            List<ButtonInfo[]> buttonInfoList = buttons.ToList();
            buttonInfoList.Add(new ButtonInfo[] { });
            buttons = buttonInfoList.ToArray();

            List<string> categoryList = categoryNames.ToList();
            categoryList.Add(categoryName);
            categoryNames = categoryList.ToArray();

            return buttons.Length - 1;
        }

        /// <summary>
        /// Removes a category from the button list.
        /// </summary>
        /// <remarks>
        /// Any buttons leading to the category will not be removed from the main category. They must be manually removed with <see cref="RemoveButton(int, string, int)"/>
        /// </remarks>
        /// <param name="categoryName">Category Name</param>
        public static void RemoveCategory(string categoryName)
        {
            List<ButtonInfo[]> buttonInfoList = buttons.ToList();
            buttonInfoList.RemoveAt(GetCategory(categoryName));
            buttons = buttonInfoList.ToArray();

            List<string> categoryList = categoryNames.ToList();
            categoryList.Remove(categoryName);
            categoryNames = categoryList.ToArray();
        }

        /// <summary>
        /// Adds a button to the specified category.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="button">Button</param>
        /// <param name="index">Index Position</param>
        public static void AddButton(int category, ButtonInfo button, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = buttons[category].ToList();
            if (index > 0)
                buttonInfoList.Insert(index, button);
            else
                buttonInfoList.Add(button);

            buttons[category] = buttonInfoList.ToArray();
        }

        /// <summary>
        /// Adds multiple buttons to the specified category.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="buttons">Buttons</param>
        /// <param name="index">Index Position</param>
        public static void AddButtons(int category, ButtonInfo[] buttons, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = Buttons.buttons[category].ToList();
            if (index > 0)
            {
                for (int i = 0; i < buttons.Length; i++)
                    buttonInfoList.Insert(index + i, buttons[i]);
            }
            else
                buttonInfoList.AddRange(buttons);

            Buttons.buttons[category] = buttonInfoList.ToArray();
        }

        /// <summary>
        /// Removes a button from the specified category.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="name">Button Name</param>
        /// <param name="index">Index Position</param>
        public static void RemoveButton(int category, string name, int index = -1)
        {
            List<ButtonInfo> buttonInfoList = buttons[category].ToList();
            if (index > 0)
                buttonInfoList.RemoveAt(index);
            else
            {
                foreach (var button in buttonInfoList.Where(button => button.buttonText == name))
                {
                    buttonInfoList.Remove(button);
                    break;
                }
            }

            buttons[category] = buttonInfoList.ToArray();
        }
    }
}

/*
The mod cemetary
Every mod listed below has been removed from the menu, for one reason or another

new ButtonInfo { buttonText = "Lightning Time Overlay", method = Visuals.StrikeTimeOverlay, disableMethod =() => NotificationManager.information.Remove("Lightning"), toolTip = "Displays the time until lightning strikes again."},
new ButtonInfo { buttonText = "Spawn Lightning", method = Visuals.SpawnLightning, isTogglable = false, toolTip = "Spawns a manual lightning strike client sided." },

new ButtonInfo { buttonText = "Pumpkin Watcher", enableMethod =() => WatcherEyesPatch.enabled = true, disableMethod =() => WatcherEyesPatch.enabled = false, toolTip = "Make the pumpkin in stump always look at you."},
new ButtonInfo { buttonText = "Pumpkin Gazer", enableMethod =() => GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/2025_Halloween2_TreeRoom/SetDressing (1)/HalloweenWatchingEyes").GetComponent<HalloweenWatcherEyes>().durationToBeNormalWhenPlayerLooks = 0.01f, disableMethod =() => GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/2025_Halloween2_TreeRoom/SetDressing (1)/HalloweenWatchingEyes").GetComponent<HalloweenWatcherEyes>().durationToBeNormalWhenPlayerLooks = 0.01f, toolTip = "Make the pumpkin in stump instantly look at you when you look away."},

new ButtonInfo { buttonText = "Spawn Red Lucy", method = Overpowered.SpawnRedLucy, isTogglable = false, toolTip = "Summons the red lucy in forest." },
new ButtonInfo { buttonText = "Spawn Blue Lucy", method = Overpowered.SpawnBlueLucy, isTogglable = false, toolTip = "Summons the blue lucy in forest." },
new ButtonInfo { buttonText = "Despawn Lucy", method = Overpowered.DespawnLucy, isTogglable = false, toolTip = "Despawns lucy in forest." },

new ButtonInfo { buttonText = "Lucy Chase Self", method =() => Overpowered.LucyChase(NetworkSystem.Instance.LocalPlayer), isTogglable = false, toolTip = "Makes lucy chase you." },
new ButtonInfo { buttonText = "Lucy Chase Gun", method = Overpowered.LucyChaseGun, toolTip = "Makes lucy chase whoever your hand desires." },

new ButtonInfo { buttonText = "Lucy Attack Self", method =() => Overpowered.LucyAttack(NetworkSystem.Instance.LocalPlayer), isTogglable = false, toolTip = "Makes lucy attack you." },
new ButtonInfo { buttonText = "Lucy Attack Gun", method = Overpowered.LucyAttackGun, toolTip = "Makes lucy attack whoever your hand desires." },
new ButtonInfo { buttonText = "Lucy Attack All", method = Overpowered.LucyAttackAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes lucy attack everyone in the room." },
                
new ButtonInfo { buttonText = "Lucy Harass Gun", method = Overpowered.LucyHarassGun, toolTip = "Makes lucy attack harass your hand desires." },
new ButtonInfo { buttonText = "Move Lucy Gun", method = Overpowered.MoveLucyGun, toolTip = "Moves lucy to wherever your hand desires." },

new ButtonInfo { buttonText = "Spaz Lucy", method = Overpowered.SpazLucy, toolTip = "Gives lucy a seizure." },
new ButtonInfo { buttonText = "Break Lucy", method =() => { Overpowered.SpazLucy(); Overpowered.lucyDelay = 0f; }, toolTip = "Breaks lucy." },
new ButtonInfo { buttonText = "Annoying Lucy", method = Overpowered.AnnoyingLucy, toolTip = "Makes lucy really annoying, by attacking everyone and making sounds of the bells." },

new ButtonInfo { buttonText = "Become Lucy", method = Overpowered.BecomeLucy, disableMethod = Movement.EnableRig, toolTip = "Turns you into the bug." },

new ButtonInfo { buttonText = "Fast Lucy", method = Overpowered.FastLucy, toolTip = "Makes lucy become really fast." },
new ButtonInfo { buttonText = "Slow Lucy", method = Overpowered.SlowLucy, toolTip = "Makes lucy become really slow." },
                
new ButtonInfo { buttonText = "Lurker Attack Self", method =() => Overpowered.LurkerAttack(NetworkSystem.Instance.LocalPlayer), isTogglable = false, toolTip = "Makes the lurker ghost attack you." },
new ButtonInfo { buttonText = "Lurker Attack Gun", method = Overpowered.LurkerAttackGun, toolTip = "Makes the lurker ghost attack whoever your hand desires." },
new ButtonInfo { buttonText = "Lurker Attack All", method = Overpowered.LurkerAttackAll, disableMethod =() => SerializePatch.OverrideSerialization = null, toolTip = "Makes the lurker ghost attack everyone in the room." },

new ButtonInfo { buttonText = "Move Lurker Gun", method = Overpowered.MoveLurkerGun, toolTip = "Moves the lurker ghost to wherever your hand desires." },
new ButtonInfo { buttonText = "Despawn Lurker", method = Overpowered.DespawnLurker, isTogglable = false, toolTip = "Despawns the lurker ghost." },

new ButtonInfo { buttonText = "Spaz Lurker", method = Overpowered.SpazLurker, toolTip = "Gives the lurker ghost a seizure." },
new ButtonInfo { buttonText = "Break Lurker", method = Overpowered.BreakLurker, toolTip = "Breaks the lurker ghost." },
new ButtonInfo { buttonText = "Annoying Lurker", method = Overpowered.AnnoyingLurker, toolTip = "Makes the lurker ghost really annoying, by attacking everyone and making laugh sounds." },

new ButtonInfo { buttonText = "Become Lurker", method = Overpowered.BecomeLurker, disableMethod =() => { Movement.EnableRig(); SerializePatch.OverrideSerialization = null; }, toolTip = "Turns you into the firefly." },

new ButtonInfo { buttonText = "Lag Gun", method = Overpowered.LagGun, toolTip = "Lags whoever your hand desires."},
new ButtonInfo { buttonText = "Lag All", method = Overpowered.LagAll, toolTip = "Lags everyone in the room."},
new ButtonInfo { buttonText = "Lag Aura", method = Overpowered.LagAura, toolTip = "Lags players nearby you."},

new ButtonInfo { buttonText = "Lowercase Name", method =() => Fun.LowercaseName(), isTogglable = false, toolTip = "Makes your name lowercase." },
new ButtonInfo { buttonText = "Long Name", method =() => Fun.LongName(), isTogglable = false, toolTip = "Makes your name really long." },

new ButtonInfo { buttonText = "Shaders", enableMethod =() => Fun.EnableShaders(), disableMethod =() => Fun.DisableShaders(), toolTip = "Adds bloom, motion blur, and slight saturation to the game. Credits to leah / tagmonkevr for the code."},

new ButtonInfo { buttonText = "Barrel Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => OverpoweredObjectMinigun(-1724683316), toolTip = "Spawns barrels out of your hand."},
new ButtonInfo { buttonText = "Core Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => OverpoweredObjectMinigun(166197108), toolTip = "Spawns collectible cores out of your hand."},

new ButtonInfo { buttonText = "Remove Cherry Blossoms", enableMethod = () => Visuals.EnableRemoveCherryBlossoms(), disableMethod = () => Visuals.DisableRemoveCherryBlossoms(), toolTip = "Removes cherry blossoms on trees, good for branching." },

new ButtonInfo { buttonText = "Noclip Gun", method =() => Overpowered.NoclipGun(), toolTip = "Makes whoever your hand desires clip through the floor."},
new ButtonInfo { buttonText = "Noclip All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.NoclipAll(), toolTip = "Makes everyone clip through the floor when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Lag Gun", method =() => Overpowered.LagGun(), toolTip = "Lags whoever your hand desires."},
new ButtonInfo { buttonText = "Lag All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.LagAll(), toolTip = "Lags everybody in the lobby when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Lag Spike Gun", method =() => Overpowered.LagSpikeGun(), toolTip = "Lags whoever your hand desires hard, but with a delay."},
new ButtonInfo { buttonText = "Lag Spike All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.LagSpikeAll(), toolTip = "Lags everybody in the lobby when holding <color=green>trigger</color> hard, but with a delay."},

new ButtonInfo { buttonText = "Virtual Stump Kick Gun", method =() => Overpowered.VirtualStumpKickGun(), toolTip = "Kicks whoever your hand desires in the custom map."},
new ButtonInfo { buttonText = "Virtual Stump Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.VirtualStumpKickAll(), toolTip = "Kicks everybody in the custom map when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Force Unload Custom Map", method =() => Overpowered.ForceUnloadCustomMap(), isTogglable = false, toolTip = "Forcefully unloads the current custom map."},

new ButtonInfo { buttonText = "Serversided Size Changer", method =() => Overpowered.SizeChanger(), enableMethod =() => Overpowered.SizeChanger(), disableMethod =() => Movement.DisableSizeChanger(), toolTip = "Increase your size by holding <color=green>trigger</color>, and decrease your size by holding <color=green>grip</color>. Everyone can see you grow or shrink."},

new ButtonInfo { buttonText = "Set Master Client", method =() => Overpowered.SetMasterClient(), toolTip = "Sets you as the master client by kicking everyone above you on the leaderboard."},

new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes everybody in the room when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Rec Room Body", method =() => Movement.RecRoomBody(), disableMethod =() => Movement.FixBody(), toolTip = "Makes your body rotate like a Rec Room character."},
new ButtonInfo { buttonText = "Glasses on Grip <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GlassesOnGrip(), toolTip = "Equips glasses when you put your hand up to your face and press <color=green>grip</color>."},

new ButtonInfo { buttonText = "Master Crash Gun", method =() => Overpowered.MasterCrashGun(), toolTip = "Crashes whoever your hand desires if you're master client."},
new ButtonInfo { buttonText = "Master Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.MasterCrashAll(), toolTip = "Crashes everybody in the room when holding <color=green>trigger</color> if you're master client."},

new ButtonInfo { buttonText = "Attic Anti Report", enableMethod =() => Fun.EnableAtticAntiReport(), method =() => Fun.AtticAntiReport(), toolTip = "Automatically builds a block around your report button."},

new ButtonInfo { buttonText = "Attic Draw Gun", method =() => Fun.AtticDrawGun(), toolTip = "Draw wherever your hand desires."},
new ButtonInfo { buttonText = "Attic Build Gun", method =() => Fun.AtticBuildGun(), toolTip = "Draw wherever your hand desires with no delay."},
new ButtonInfo { buttonText = "Attic Tower Gun", method =() => Fun.AtticTowerGun(), toolTip = "Builds a tower wherever your hand desires."},

new ButtonInfo { buttonText = "Attic Freeze Gun", method =() => Fun.AtticFreezeGun(), toolTip = "Freeze whoever your hand desires."},
new ButtonInfo { buttonText = "Attic Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.AtticFreezeAll(), toolTip = "Freezes everyone in the room when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Attic Float Gun", method =() => Fun.AtticFloatGun(), toolTip = "Makes whoever your hand desires float."},
new ButtonInfo { buttonText = "Attic Float All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.AtticFloatAll(), toolTip = "Makes everyone in the room float when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Spaz Gamemode <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SpazGamemode(), toolTip = "Spam changes the gamemode every tenth of a second when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Change Gamemode to Casual", method =() => Overpowered.ChangeGamemode("Casual"), isTogglable = false, toolTip = "Changes the gamemode to casual."},
new ButtonInfo { buttonText = "Change Gamemode to Infection", method =() => Overpowered.ChangeGamemode("Infection"), isTogglable = false, toolTip = "Changes the gamemode to infection."},
new ButtonInfo { buttonText = "Change Gamemode to Hunt", method =() => Overpowered.ChangeGamemode("Hunt"), isTogglable = false, toolTip = "Changes the gamemode to hunt."},
new ButtonInfo { buttonText = "Change Gamemode to Paintbrawl", method =() => Overpowered.ChangeGamemode("Paintbrawl"), isTogglable = false, toolTip = "Changes the gamemode to paintbrawl."},
new ButtonInfo { buttonText = "Change Gamemode to Ambush", method =() => Overpowered.ChangeGamemode("Ambush"), isTogglable = false, toolTip = "Changes the gamemode to ambush."},
new ButtonInfo { buttonText = "Change Gamemode to Ghost Tag", method =() => Overpowered.ChangeGamemode("Ghost"), isTogglable = false, toolTip = "Changes the gamemode to ghost tag."},
new ButtonInfo { buttonText = "Change Gamemode to Guardian", method =() => Overpowered.ChangeGamemode("Guardian"), isTogglable = false, toolTip = "Changes the gamemode to guardian."},
new ButtonInfo { buttonText = "Change Gamemode to Freeze Tag", method =() => Overpowered.ChangeGamemode("FreezeTag"), isTogglable = false, toolTip = "Changes the gamemode to freeze tag."},

new ButtonInfo { buttonText = "Attic Serversided Blocks", method =() => Overpowered.AtticServersidedBlocks(), toolTip = "Lets you spawn and do anything with blocks in any map."},

new ButtonInfo { buttonText = "Change Text Location", overlapText = "Change Text Location <color=grey>[</color><color=green>Forest</color><color=grey>]</color>", method =() => Overpowered.ChangeFriendStationPosition(), isTogglable = false, toolTip = "Changes the friend position of where the text spawns."},
new ButtonInfo { buttonText = "Big Emoji <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BigEmoji(), toolTip = "Spawns a really big emoji at stump when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Black Box <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BlackScreenAll(), toolTip = "Spawns a really big black emoji when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Transgender Flag <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.TransgenderFlag(), toolTip = "Spawns a transgender flag when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Strobe <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.Strobe(), toolTip = "Spawns a rave when holding <color=green>trigger</color>."},
                
new ButtonInfo { buttonText = "Advertisement <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.Advertisement(), toolTip = "Spawns a really big advertisement when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Silly Face <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SillyFace(), toolTip = "Spawns a silly face when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Testicles <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.Testicles(), toolTip = "Spawns male testicles when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Firecracker Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.FirecrackerSpam(), toolTip = "Spams firecrackers out of your hand when holding <color=green>grip</color>."},
new ButtonInfo { buttonText = "Firecracker Spray <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.FirecrackerSpray(), toolTip = "Sprays firecrackers out of your hand when holding <color=green>grip</color>."},

new ButtonInfo { buttonText = "Firecracker Fountain <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FirecrackerFountain(), toolTip = "Spams a fountain of firecrackers when holding <color=green>trigger</color>."},
new ButtonInfo { buttonText = "Firecracker Rain <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FirecrackerRain(), toolTip = "Rains firecrackers when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Firecracker Gun", method =() => Overpowered.FirecrackerGun(), toolTip = "Spams firecrackers at wherever your hand desires."},
new ButtonInfo { buttonText = "Firecracker Airstrike Gun", method =() => Overpowered.FirecrackerAirstrikeGun(), toolTip = "Spams firecrackers down from the heavens at wherever your hand desires."},

new ButtonInfo { buttonText = "Become Firecrackers <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BecomeFirecrackers(), toolTip = "Turns you into a bunch of firecrackers when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Firecracker Crash Gun", method =() => Overpowered.FirecrackerCrashGun(), toolTip = "Crashes whoever your hand desires with the firecrackers."},
new ButtonInfo { buttonText = "Firecracker Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FirecrackerCrashAll(), toolTip = "Crashes everybody in the room when holding <color=green>trigger</color> with the firecrackers."},

new ButtonInfo { buttonText = "Firecracker Instant Crash Gun", method =() => Overpowered.FirecrackerInstantCrashGun(), toolTip = "Crashes whoever your hand desires with the firecrackers."},
new ButtonInfo { buttonText = "Firecracker Instant Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FirecrackerInstantCrashAll(), toolTip = "Crashes everybody in the room when holding <color=green>trigger</color> with the firecrackers."},
new ButtonInfo { buttonText = "Repair Kick", method =() => Overpowered.RepairKick(), isTogglable = false, toolTip = "Swaps the target used for kicking, to hopefully repair any kick mods."},
new ButtonInfo { buttonText = "Auto Repair Kick", method =() => Overpowered.AutoRepairKick(), toolTip = "Automatically swaps the target used for kicking, to hopefully repair any kick mods without needing to manually press that button."},

new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Kick</color><color=grey>]</color>", method =() => Overpowered.AntiReportKick(), toolTip = "Kicks whoever tries to report you."},
new ButtonInfo { buttonText = "Leaderboard Kick", method =() => Overpowered.LeaderboardKick(), disableMethod =() => Overpowered.DisableLeaderboardKick(), toolTip = "Changes the report button into a kick button."},

new ButtonInfo { buttonText = "Kick Gun", method =() => Overpowered.KickGun(), toolTip = "Kicks whoever your hand desires."},
new ButtonInfo { buttonText = "Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.KickAll(), toolTip = "Kicks everybody in the lobby when holding <color=green>trigger</color>."},

new ButtonInfo { buttonText = "Crash Gun", method =() => Overpowered.CrashGun(), toolTip = "Crashes whoever your hand desires." },
new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes everyone in the room when holding <color=green>grip</color>." },

new ButtonInfo { buttonText = "Instant Crash Gun", method =() => Overpowered.InstantCrashGun(), toolTip = "Crashes whoever your hand desires instantly." },
new ButtonInfo { buttonText = "Instant Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.InstantCrashAll(), toolTip = "Crashes everyone in the room instantly when holding <color=green>grip</color>." },

new ButtonInfo { buttonText = "Instant Crank Elves", method =() => Projectiles.InstantCrankElf(), disableMethod =() => Projectiles.DisableInstantCrankElf(), toolTip = "Makes the elf launcher instantly spawn elves when barely moving the handle." },
new ButtonInfo { buttonText = "Elf Launcher Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.ElfLauncherSpam(), toolTip = "Spams the elf launcher cosmetic when holding <color=green>grip</color>." },
new ButtonInfo { buttonText = "Elf Gun", method =() => Projectiles.ElfGun(), toolTip = "Spams elves wherever your hand desires." },
new ButtonInfo { buttonText = "Elf Annoy Gun", method =() => Projectiles.ElfAnnoyGun(), toolTip = "Spams the elf launcher cosmetic around and towards whoever your hand desires." },
new ButtonInfo { buttonText = "Elf Airstrike Gun", method =() => Projectiles.ElfAirstrikeGun(), toolTip = "Spams the elf launcher cosmetic above and down towards whoever your hand desires." },

new ButtonInfo { buttonText = "Piece Name Helper", method =() => Fun.PieceNameHelper(), toolTip = "Remove me later."},

new ButtonInfo { buttonText = "Crash Amount", overlapText = "Crash Amount <color=grey>[</color><color=green>2</color><color=grey>]</color>", method =() => Settings.CrashAmount(), isTogglable = false, toolTip = "Changes the amount of projectiles the crash mods send."},
new ButtonInfo { buttonText = "Projectile Gun", method =() => Projectiles.ProjectileGun(), toolTip = "Acts like the projectile spam, but the projectiles only show up for you and whoever your hand desires." },

new ButtonInfo { buttonText = "Anti Ban", overlapText = "Anti Ban <color=grey>[</color><color=red>Risky</color><color=grey>]</color>", method =() => Overpowered.AntiBan(), isTogglable = false, toolTip = "Prevents you from getting banned. This mod is very experimental, if you get banned, I take ZERO responsibility."},
new ButtonInfo { buttonText = "Anti Ban Check", method =() => Overpowered.AntiBanCheck(), isTogglable = false, toolTip = "Tests if the the room is modded or not."},

new ButtonInfo { buttonText = "Set Master", overlapText = "Set Master <color=grey>[</color><color=red>Risky</color><color=grey>]</color>", method =() => Overpowered.FastMaster(), isTogglable = false, toolTip = "Sets you as master client."},
new ButtonInfo { buttonText = "Set Master Gun", overlapText = "Set Master Gun <color=grey>[</color><color=red>Risky</color><color=grey>]</color>", method =() => Overpowered.SetMasterGun(), toolTip = "Sets whoever your hand desires as master client."},
new ButtonInfo { buttonText = "Auto Set Master", overlapText = "Auto Set Master <color=grey>[</color><color=red>Risky</color><color=grey>]</color>", method =() => Experimental.AutoSetMaster(), toolTip = "Sets you as master client when in modded lobbies or when using the anti ban."},

new ButtonInfo { buttonText = "Infection Gamemode", method =() => Overpowered.InfectionGamemode(), isTogglable = false, toolTip = "Sets the gamemode to infection."},
new ButtonInfo { buttonText = "Casual Gamemode", method =() => Overpowered.CasualGamemode(), isTogglable = false, toolTip = "Sets the gamemode to casual."},
new ButtonInfo { buttonText = "Hunt Gamemode", method =() => Overpowered.HuntGamemode(), isTogglable = false, toolTip = "Sets the gamemode to hunt."},
new ButtonInfo { buttonText = "Paintbrawl Gamemode", method =() => Overpowered.PaintbrawlGamemode(), isTogglable = false, toolTip = "Sets the gamemode to paintbrawl."},

new ButtonInfo { buttonText = "Break Network Triggers", method =() => Overpowered.SSDisableNetworkTriggers(), isTogglable = false, toolTip = "Disables network triggers for everyone."},
new ButtonInfo { buttonText = "Trap Stump", method =() => Overpowered.TrapStump(), isTogglable = false, toolTip = "Anyone who enters the stump will be kicked."},

new ButtonInfo { buttonText = "Make Room Private", method =() => Overpowered.MakeRoomPrivate(), isTogglable = false, toolTip = "Makes the room private."},
new ButtonInfo { buttonText = "Make Room Public", method =() => Overpowered.MakeRoomPublic(), isTogglable = false, toolTip = "Makes the room private."},

new ButtonInfo { buttonText = "Lag Gun", method =() => Experimental.LagGun(), toolTip = "Lags whoever your hand desires." },
new ButtonInfo { buttonText = "Lag All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.LagAll(), toolTip = "Lags everyone when holding <color=green>trigger</color>." },

new ButtonInfo { buttonText = "Crash Gun", method =() => Experimental.CrashGun(), toolTip = "Crashes whoever your hand desires." },
new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.CrashAll(), toolTip = "Crashes everyone when holding <color=green>trigger</color>." },

new ButtonInfo { buttonText = "Change Name Gun", method =() => Experimental.ChangeNameGun(), toolTip = "Changes whoever your hand desires' name to your name. Credits to kman for creating the original method." },
new ButtonInfo { buttonText = "Change Name All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.ChangeNameAll(), toolTip = "Changes everyone's name to your name. Credits to kman for creating the original method." },

new ButtonInfo { buttonText = "Destroy Gun", method =() => Overpowered.DestroyGun(), toolTip = "Makes new players not see whoever your hand desires." },
new ButtonInfo { buttonText = "Destroy All", method =() => Overpowered.DestroyAll(), isTogglable = false, toolTip = "Every player that joins after you will not be able to see anyone." },

new ButtonInfo { buttonText = "Acid Self", method =() => Overpowered.AcidSelf(), isTogglable = false, toolTip = "Turns you into acid." },
new ButtonInfo { buttonText = "Acid Gun", method =() => Overpowered.AcidGun(), toolTip = "Turns whoever your hand desires into acid." },
new ButtonInfo { buttonText = "Acid All", method =() => Overpowered.AcidAll(), isTogglable = false, toolTip = "Turns everyone into acid." },

new ButtonInfo { buttonText = "Lag Gun <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color>", method =() => Overpowered.LagGun(), toolTip = "Lags whoever your hand desires." },
new ButtonInfo { buttonText = "Lag All <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color> <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.LagAll(), toolTip = "Lags everyone when holding <color=green>trigger</color>." },

new ButtonInfo { buttonText = "Crash Gun <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color>", method =() => Overpowered.CrashGun(), toolTip = "Crashes whoever your hand desires." },
new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color> <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes everyone when holding <color=green>trigger</color>." },

new ButtonInfo { buttonText = "Unacid Self", method =() => Fun.UnacidSelf(), isTogglable = false, toolTip = "Unturns you into acid." },
new ButtonInfo { buttonText = "Unacid Gun", method =() => Fun.UnacidGun(), toolTip = "Unturns whoever your hand desires into acid." },
new ButtonInfo { buttonText = "Unacid All", method =() => Fun.UnacidAll(), isTogglable = false, toolTip = "Unturns everyone into acid." },

new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Lag</color><color=grey>]</color>", method =() => Safety.AntiReportLag(), toolTip = "Lags whoever comes near your report button."},
new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method =() => Safety.AntiReportCrash(), toolTip = "Crashes whoever comes near your report button."}

new ButtonInfo { buttonText = "Crash Gun", method =() => Overpowered.CrashGun(), toolTip = "Crashes or lags whoever your hand desires." },
new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes every quest player, and lags/crashes every steam player when holding <color=green>trigger</color>" },
new ButtonInfo { buttonText = "Random Color Snowballs", enableMethod =() => Projectiles.RandomColorSnowballs(), disableMethod =() => Projectiles.NoRandomColorSnowballs(), toolTip = "Makes your snowballs random colors." },
new ButtonInfo { buttonText = "Black Snowballs", enableMethod =() => Projectiles.BlackSnowballs(), disableMethod =() => Projectiles.FixBlackSnowballs(), toolTip = "Makes your snowballs black." },

new ButtonInfo { buttonText = "Lag Gun", method =() => Overpowered.BubbleGun(), toolTip = "Spawns a massive bubble which lags whoever your hand desires." },
new ButtonInfo { buttonText = "Lag All", method =() => Overpowered.BubbleAll(), toolTip = "Spawns a massive bubble which lags everyone." },

new ButtonInfo { buttonText = "Break Bug", method =() => Fun.BreakBug(), isTogglable = false, toolTip = "Breaks the bug." },
new ButtonInfo { buttonText = "Break Bat", method =() => Fun.BreakBat(), isTogglable = false, toolTip = "Breaks the bat." },

new ButtonInfo { buttonText = "Steal Bug", method =() => Fun.StealBug(), toolTip = "Steals the bug." },
new ButtonInfo { buttonText = "Steal Bat", method =() => Fun.StealBat(), toolTip = "Steals the bat." },

new ButtonInfo { buttonText = "Spaz Voice", method =() => Fun.SpazVoice(), disableMethod =() => Fun.UnspazVoice(), toolTip = "Spazzes your voice out. Only works with monke speak on."},

new ButtonInfo { buttonText = "Acid Self", method =() => Basement.SodaSelf(), isTogglable = false, toolTip = "Turns you into soda."},
new ButtonInfo { buttonText = "Unacid Self", method =() => Basement.UnsodaSelf(), isTogglable = false, toolTip = "Turns you not into soda."},

new ButtonInfo { buttonText = "Grab Train", method =() => Fun.GrabTrain(), toolTip = "Puts the train in your hand." },
new ButtonInfo { buttonText = "Train Gun", method =() => Fun.TrainGun(), toolTip = "Moves the train to wherever your hand desires." },
new ButtonInfo { buttonText = "Destroy Train", method =() => Fun.DestroyTrain(), isTogglable = false, toolTip = "Sends the train to hell." },
new ButtonInfo { buttonText = "Slow Train", enableMethod =() => Fun.SlowTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train slower." },
new ButtonInfo { buttonText = "Fast Train", enableMethod =() => Fun.FastTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train faster." },

new ButtonInfo { buttonText = "Lava Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.LavaSplashHands(), toolTip = "Splashes lava when holding <color=green>grip</color>."},
new ButtonInfo { buttonText = "Lava Splash Aura", method =() => Fun.LavaSplashAura(), toolTip = "Splashes lava around you at random positions."},
new ButtonInfo { buttonText = "Lava Splash Gun", method =() => Fun.LavaSplashGun(), toolTip = "Splashes lava wherever your hand desires."},

new ButtonInfo { buttonText = "Force Erupt Lava", method =() => Overpowered.ForceEruptLava(), isTogglable = false, toolTip = "Forcibly rises the lava." },
new ButtonInfo { buttonText = "Force Drain Lava", method =() => Overpowered.ForceUneruptLava(), isTogglable = false, toolTip = "Forcibly drains the lava." },
new ButtonInfo { buttonText = "Instant Rise Lava", method =() => Overpowered.ForceRiseLava(), isTogglable = false, toolTip = "Instantly rises the lava." },
new ButtonInfo { buttonText = "Instant Drain Lava", method =() => Overpowered.ForceDrainLava(), isTogglable = false, toolTip = "Instantly drains the lava." },
new ButtonInfo { buttonText = "Spaz Lava", method =() => Overpowered.SpazLava(), toolTip = "Spazzes out the lava." },

new ButtonInfo { buttonText = "Kill Bees", method =() => Fun.KillBees(), isTogglable = false, toolTip = "Sends the bees to hell."},
new ButtonInfo { buttonText = "Anger Bees Self", method =() => Fun.AngerBees(), isTogglable = false, toolTip = "Angers the bees on you."},
new ButtonInfo { buttonText = "Anger Bees Gun", method =() => Fun.AngerBeesGun(), toolTip = "Angers the bees wherever your hand desires."},
new ButtonInfo { buttonText = "Anger Bees All", method =() => Fun.AngerBeesAll(), toolTip = "Angers the bees on everyone."},

new ButtonInfo { buttonText = "Sting Self", method =() => Fun.StingSelf(), isTogglable = false, toolTip = "Makes the bees attack you."},
new ButtonInfo { buttonText = "Sting Gun", method =() => Fun.StingGun(), toolTip = "Makes the bees attack whoever your hand desires."},
new ButtonInfo { buttonText = "Sting All", method =() => Fun.StingAll(), toolTip = "Makes the bees attack everyone."},

new ButtonInfo { buttonText = "Remove Christmas Lights", enableMethod =() => Advantages.EnableRemoveChristmasLights(), disableMethod =() => Advantages.DisableRemoveChristmasLights(), toolTip = "Removes lights, good for walls."},
new ButtonInfo { buttonText = "Remove Winter Decorations", enableMethod =() => Advantages.EnableRemoveChristmasDecorations(), disableMethod =() => Advantages.DisableRemoveChristmasDecorations(), toolTip = "Removes snowmen and such, good for anyone but very obvious."},
new ButtonInfo { buttonText = "Projectile Bomb <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.ProjectileBomb(), disableMethod =() => Projectiles.DisableProjectileBomb(), toolTip = "Acts like C4, but instead of launching you, it spawns 5 projectiles in random directions." },

new ButtonInfo { buttonText = "Gorilla Voice <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Fun.GorillaVoice(), toolTip = "Turns your voice into the gorilla voice when holding <color=green>A</color>."},
new ButtonInfo { buttonText = "Spam Eat Honey Comb", method =() => Fun.HoneycombSpam(), toolTip = "Spam eats the honey comb when holding <color=green>grip</color>."},

new ButtonInfo { buttonText = "Remove Self from Leaderboard", method =() => Overpowered.RemoveSelfFromLeaderboard(), isTogglable = false, toolTip = "Removes yourself from the leaderboard." },

new ButtonInfo { buttonText = "Start Moon Event", method =() => Overpowered.StartMoonEvent(), isTogglable = false, toolTip = "Starts the moon event."},
new ButtonInfo { buttonText = "End Moon Event", method =() => Overpowered.EndMoonEvent(), isTogglable = false, toolTip = "Ends the moon event."},
new ButtonInfo { buttonText = "Spaz Moon Event", method =() => Overpowered.FlashScreen(), toolTip = "Spazzes out the moon event."},

new ButtonInfo { buttonText = "Spawn Red Lucy", method =() => Overpowered.SpawnRedLucy(), isTogglable = false, toolTip = "Summons the red Lucy in forest." },
new ButtonInfo { buttonText = "Spawn Blue Lucy", method =() => Overpowered.SpawnBlueLucy(), isTogglable = false, toolTip = "Summons the blue Lucy in forest." },
new ButtonInfo { buttonText = "Despawn Lucy", method =() => Overpowered.DespawnLucy(), isTogglable = false, toolTip = "Despawns lucy in forest." },
new ButtonInfo { buttonText = "Spaz Lucy", method =() => Overpowered.SpazLucy(), toolTip = "Gives lucy a seizure." },

new ButtonInfo { buttonText = "Lucy Chase Self", method =() => Overpowered.LucyChaseSelf(), isTogglable = false, toolTip = "Makes lucy chase you." },
new ButtonInfo { buttonText = "Lucy Chase Gun", method =() => Overpowered.LucyChaseGun(), toolTip = "Makes lucy chase whoever your hand desires." },
                
new ButtonInfo { buttonText = "Lucy Attack Self", method =() => Overpowered.LucyAttackSelf(), isTogglable = false, toolTip = "Makes lucy attack you." },
new ButtonInfo { buttonText = "Lucy Attack Gun", method =() => Overpowered.LucyAttackGun(), toolTip = "Makes lucy attack whoever your hand desires." },
new ButtonInfo { buttonText = "Annoying Lucy", method =() => Overpowered.AnnoyingLucy(), toolTip = "Makes lucy really annoying, by attacking everyone and making sounds of the bells." },

new ButtonInfo { buttonText = "Fast Lucy", method =() => Overpowered.FastLucy(), toolTip = "Makes lucy become really fast." },
new ButtonInfo { buttonText = "Slow Lucy", method =() => Overpowered.SlowLucy(), toolTip = "Makes lucy become really slow." },
new ButtonInfo { buttonText = "Anti Lucy", enableMethod =() => RisePatch.enabled = true, disableMethod =() => RisePatch.enabled = false, toolTip = "Prevents lucy from moving you."},
new ButtonInfo { buttonText = "Disable Lucy",  enableMethod =() => LucyPatch.enabled = true, disableMethod =() => LucyPatch.enabled = false, toolTip = "Prevents lucy from spawning."},
new ButtonInfo { buttonText = "Anti Lurker", enableMethod =() => LurkerPatch.enabled = true, method = Safety.AntiLurker, disableMethod =() => LurkerPatch.enabled = false, toolTip = "Prevents the lurker ghost from possessing you."},
 */
