using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.ObstacleCourse;
using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using Photon.Pun;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Menu
{
    public class Buttons
    {
        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] { // Main [0]

                new ButtonInfo { buttonText = "Join Discord", method =() => Important.JoinDiscord(), isTogglable = false, toolTip = "Invites you to join the ii's <b>Stupid</b> Mods Discord server."},

                new ButtonInfo { buttonText = "Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Opens the settings tab."},
                new ButtonInfo { buttonText = "Friends", method =() => currentCategoryName = "Friends", isTogglable = false, toolTip = "Opens the friends tab."},
                new ButtonInfo { buttonText = "Players", method =() => Settings.PlayersTab(), isTogglable = false, toolTip = "Opens the players tab."},

                new ButtonInfo { buttonText = "Favorite Mods", method =() => currentCategoryName = "Favorite Mods", isTogglable = false, toolTip = "Opens your favorite mods. Favorite mods with left grip."},
                new ButtonInfo { buttonText = "Enabled Mods", method =() => currentCategoryName = "Enabled Mods", isTogglable = false, toolTip = "Shows all mods you have enabled."},
                new ButtonInfo { buttonText = "Room Mods", method =() => currentCategoryName = "Room Mods", isTogglable = false, toolTip = "Opens the room mods."},
                new ButtonInfo { buttonText = "Important Mods", method =() => currentCategoryName = "Important Mods", isTogglable = false, toolTip = "Opens the important mods."},
                new ButtonInfo { buttonText = "Safety Mods", method =() => currentCategoryName = "Safety Mods", isTogglable = false, toolTip = "Opens the safety mods."},
                new ButtonInfo { buttonText = "Movement Mods", method =() => currentCategoryName = "Movement Mods", isTogglable = false, toolTip = "Opens the movement mods."},
                new ButtonInfo { buttonText = "Advantage Mods", method =() => currentCategoryName = "Advantage Mods", isTogglable = false, toolTip = "Opens the advantage giving mods."},
                new ButtonInfo { buttonText = "Visual Mods", method =() => currentCategoryName = "Visual Mods", isTogglable = false, toolTip = "Opens the visual mods."},
                new ButtonInfo { buttonText = "Fun Mods", method =() => currentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Opens the fun mods."},
                new ButtonInfo { buttonText = "Spam Mods", method =() => currentCategoryName = "Spam Mods", isTogglable = false, toolTip = "Opens the spam mods."},
                new ButtonInfo { buttonText = "Master Mods", method =() => currentCategoryName = "Master Mods", isTogglable = false, toolTip = "Opens the master mods."},
                new ButtonInfo { buttonText = "Overpowered Mods", method =() => currentCategoryName = "Overpowered Mods", isTogglable = false, toolTip = "Opens the overpowered mods."},
                new ButtonInfo { buttonText = "Experimental Mods", method =() => currentCategoryName = "Experimental Mods", isTogglable = false, toolTip = "Opens the experimental mods."},
            },

            new ButtonInfo[] { // Settings [1]
                new ButtonInfo { buttonText = "Exit Settings", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Menu Settings", method =() => currentCategoryName = "Menu Settings", isTogglable = false, toolTip = "Opens the settings for the menu."},

                new ButtonInfo { buttonText = "Keybind Settings", method =() => currentCategoryName = "Keybind Settings", isTogglable = false, toolTip = "Opens the settings for the keybinds."},
                new ButtonInfo { buttonText = "Plugin Settings", method =() => currentCategoryName = "Plugin Settings", isTogglable = false, toolTip = "Opens the settings for the plugins."},

                new ButtonInfo { buttonText = "Soundboard Settings", method =() => currentCategoryName = "Soundboard Settings", isTogglable = false, toolTip = "Opens the settings for the soundboard."},
                new ButtonInfo { buttonText = "Friend Settings", method =() => currentCategoryName = "Friend Settings", isTogglable = false, toolTip = "Opens the settings for the friend system."},

                new ButtonInfo { buttonText = "Room Settings", method =() => currentCategoryName = "Room Settings", isTogglable = false, toolTip = "Opens the settings for the room mods."},
                new ButtonInfo { buttonText = "Safety Settings", method =() => currentCategoryName = "Safety Settings", isTogglable = false, toolTip = "Opens the settings for the safety mods."},
                new ButtonInfo { buttonText = "Movement Settings", method =() => currentCategoryName = "Movement Settings", isTogglable = false, toolTip = "Opens the settings for the movement mods."},
                new ButtonInfo { buttonText = "Advantage Settings", method =() => currentCategoryName = "Advantage Settings", isTogglable = false, toolTip = "Opens the settings for the advantage mods."},
                new ButtonInfo { buttonText = "Visual Settings", method =() => currentCategoryName = "Visual Settings", isTogglable = false, toolTip = "Opens the settings for the visual mods."},
                new ButtonInfo { buttonText = "Fun Settings", method =() => currentCategoryName = "Fun Settings", isTogglable = false, toolTip = "Opens the settings for the fun mods."},
                new ButtonInfo { buttonText = "Overpowered Settings", method =() => currentCategoryName = "Overpowered Settings", isTogglable = false, toolTip = "Opens the settings for the overpowered mods."},

                new ButtonInfo { buttonText = "Projectile Settings", method =() => currentCategoryName = "Projectile Settings", isTogglable = false, toolTip = "Opens the settings for the projectiles."}
            },

            new ButtonInfo[] { // Menu Settings [2]
                new ButtonInfo { buttonText = "Exit Menu Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => Settings.RightHand(), disableMethod =() => Settings.LeftHand(), toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Both Hands", enableMethod =() => bothHands = true, disableMethod =() => bothHands = false, toolTip = "Puts the menu on your both of your hands."},

                new ButtonInfo { buttonText = "One Handed Menu", enableMethod =() => oneHand = true, disableMethod =() => oneHand = false, toolTip = "Makes the menu open in front of you, so you can use it with one hand."},
                new ButtonInfo { buttonText = "Joystick Menu", enableMethod =() => joystickMenu = true, disableMethod =() => Settings.JoystickMenuOff(), toolTip = "Makes the menu into something like Colossal, click your joysticks to open, joysticks to move between mods and pages, and click your left joystick to toggle a mod."},
                new ButtonInfo { buttonText = "Physical Menu", enableMethod =() => Settings.PhysicalMenuOn(), disableMethod =() => Settings.PhysicalMenuOff(), toolTip = "Freezes the menu in world space."},
                new ButtonInfo { buttonText = "Wrist Menu", enableMethod =() => wristMenu = true, disableMethod =() => wristMenu = false, toolTip = "Turns the menu into a weird wrist watch, click your hand to open it."},
                new ButtonInfo { buttonText = "Watch Menu", enableMethod =() => Settings.WatchMenuOn(), disableMethod =() => Settings.WatchMenuOff(), toolTip = "Turns the menu into a watch, click your joystick to toggle, and move your joystick to select a mod."},
                new ButtonInfo { buttonText = "Shiny Menu", enableMethod =() => shinymenu = true, disableMethod =() => shinymenu = false, toolTip = "Makes the menu's textures use the old shader."},
                new ButtonInfo { buttonText = "Crystallize Menu", enableMethod =() => { crystallizemenu = true; OrangeUI = CrystalMaterial; hasFoundAllBoards = false; }, disableMethod =() => { crystallizemenu = false ; OrangeUI = new Material(Shader.Find("GorillaTag/UberShader")); hasFoundAllBoards = false; }, toolTip = "Turns the menu into crystals."},
                new ButtonInfo { buttonText = "Thick Menu", enableMethod =() => thinMenu = false, disableMethod =() => thinMenu = true, toolTip = "Makes the menu thin."},
                new ButtonInfo { buttonText = "Long Menu", enableMethod =() => longmenu = true, disableMethod =() => longmenu = false, toolTip = "Makes the menu long."},
                new ButtonInfo { buttonText = "Flip Menu", enableMethod =() => flipMenu = true, disableMethod =() => flipMenu = false, toolTip = "Flips the menu to the back of your hand."},

                new ButtonInfo { buttonText = "Round Menu", enableMethod =() => shouldRound = true, disableMethod =() => shouldRound = false, toolTip = "Makes the menu objects round."},
                new ButtonInfo { buttonText = "Outline Menu", enableMethod =() => shouldOutline = true, disableMethod =() => shouldOutline = false, toolTip = "Gives the menu objects an outline."},
                new ButtonInfo { buttonText = "Outline Text", enableMethod =() => outlineText = true, disableMethod =() => outlineText = false, toolTip = "Gives the text objects an outline."},
                new ButtonInfo { buttonText = "Inner Outline Menu", enableMethod =() => innerOutline = true, disableMethod =() => innerOutline = false, toolTip = "Gives the menu an outline on the inside."},
                new ButtonInfo { buttonText = "Smooth Menu Position", enableMethod =() => smoothMenuPosition = true, disableMethod =() => smoothMenuPosition = false, toolTip = "Smoothes the menu's position."},
                new ButtonInfo { buttonText = "Smooth Menu Rotation", enableMethod =() => smoothMenuRotation = true, disableMethod =() => smoothMenuRotation = false, toolTip = "Smoothes the menu's rotation."},

                new ButtonInfo { buttonText = "Freeze Player in Menu", method =() => Settings.FreezePlayerInMenu(), enableMethod =() => closePosition = GorillaTagger.Instance.rigidbody.transform.position, toolTip = "Freezes your character when inside the menu."},
                new ButtonInfo { buttonText = "Freeze Rig in Menu", method =() => Settings.FreezeRigInMenu(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig when inside the menu."},
                new ButtonInfo { buttonText = "Zero Gravity Menu", enableMethod =() => zeroGravityMenu = true, disableMethod =() => zeroGravityMenu = false, toolTip = "Disables gravity on the menu when dropping it."},
                new ButtonInfo { buttonText = "Player Scale Menu", enableMethod =() => scaleWithPlayer = true, disableMethod =() => scaleWithPlayer = false, toolTip = "Scales the menu with your player scale."},
                new ButtonInfo { buttonText = "Alphabetize Menu", toolTip = "Alphabetizes the entire menu."},
                new ButtonInfo { buttonText = "Custom Menu Name", enableMethod =() => Settings.CustomMenuName(), disableMethod =() => doCustomName = false, toolTip = $"Changes the name of the menu to whatever. You can change the text inside of your Gorilla Tag files ({PluginInfo.BaseDirectory}/iiMenu_CustomMenuName.txt)."},
                new ButtonInfo { buttonText = "Menu Trail", enableMethod =() => menuTrail = true, disableMethod =() => menuTrail = false, toolTip = "Gives the menu a trail when you drop."},

                new ButtonInfo { buttonText = "Dynamic Animations", enableMethod =() => dynamicAnimations = true, disableMethod =() => dynamicAnimations = false, toolTip = "Adds more animations to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Dynamic Gradients", enableMethod =() => dynamicGradients = true, disableMethod =() => dynamicGradients = false, toolTip = "Makes gradients dynamic, showing you the full gradient instead of a pulsing color."},
                new ButtonInfo { buttonText = "Horizontal Gradients", enableMethod =() => { horizontalGradients = true; cacheGradients.Clear(); }, disableMethod =() => { horizontalGradients = false; cacheGradients.Clear(); }, toolTip = "Rotates the dynamic gradients by 90 degrees."},
                new ButtonInfo { buttonText = "Dynamic Sounds", enableMethod =() => dynamicSounds = true, disableMethod =() => dynamicSounds = false, toolTip = "Adds more sounds to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Exclusive Page Sounds", enableMethod =() => exclusivePageSounds = true, disableMethod =() => exclusivePageSounds = false, toolTip = "Makes the sound that joystick menu makes when switching pages using the menu."},
                new ButtonInfo { buttonText = "Gradient Title", enableMethod =() => gradientTitle = true, disableMethod =() => gradientTitle = false, toolTip = "Gives a gradient to the title of the menu depending on your theme."},
                new ButtonInfo { buttonText = "Voice Commands", enableMethod =() => Settings.VoiceRecognitionOn(), disableMethod =() => Settings.VoiceRecognitionOff(), toolTip = "Enable and disable mods using your voice. Activate it like how you would any other voice assistant, such as \"Jarvis, Platforms\"."},
                new ButtonInfo { buttonText = "Chain Voice Commands", toolTip = "Makes voice commands chain together, so you don't have to repeatedly ask it to listen to you."},

                new ButtonInfo { buttonText = "Annoying Mode", enableMethod =() => annoyingMode = true, disableMethod =() => Settings.AnnoyingModeOff(), toolTip = "Turns on the April Fools 2024 settings."},
                new ButtonInfo { buttonText = "Lowercase Mode", enableMethod =() => lowercaseMode = true, disableMethod =() => lowercaseMode = false, toolTip = "Makes the entire menu's text lowercase."},
                new ButtonInfo { buttonText = "Uppercase Mode", enableMethod =() => uppercaseMode = true, disableMethod =() => uppercaseMode = false, toolTip = "Makes the entire menu's text uppercase."},
                new ButtonInfo { buttonText = "Overflow Mode", enableMethod =() => NoAutoSizeText = true, disableMethod =() => NoAutoSizeText = false, toolTip = "Makes the entire menu's text overflow."},

                new ButtonInfo { buttonText = "Change Menu Language", overlapText = "Change Menu Language <color=grey>[</color><color=green>English</color><color=grey>]</color>", method =() => Settings.ChangeMenuLanguage(), enableMethod =() => Settings.ChangeMenuLanguage(), disableMethod =() => Settings.ChangeMenuLanguage(false), incremental = true, isTogglable = false, toolTip = "Changes the language of the menu."},
                new ButtonInfo { buttonText = "Change Menu Theme", method =() => Settings.ChangeMenuTheme(), enableMethod =() => Settings.ChangeMenuTheme(), disableMethod =() => Settings.ChangeMenuTheme(false), incremental = true, isTogglable = false, toolTip = "Changes the theme of the menu."},
                new ButtonInfo { buttonText = "Slow Gradient Fade", enableMethod =() => slowFadeColors = true, disableMethod =() => slowFadeColors = false, toolTip = "Makes gradient themes fade slower on the background."},
                new ButtonInfo { buttonText = "Change Menu Scale", overlapText = "Change Menu Scale <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeMenuScale(), enableMethod =() => Settings.ChangeMenuScale(), disableMethod =() => Settings.ChangeMenuScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the menu."},
                new ButtonInfo { buttonText = "Change Notification Scale", overlapText = "Change Notification Scale <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangeNotificationScale(), enableMethod =() => Settings.ChangeNotificationScale(), disableMethod =() => Settings.ChangeNotificationScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the notifications."},
                new ButtonInfo { buttonText = "Change Arraylist Scale", overlapText = "Change Arraylist Scale <color=grey>[</color><color=green>4</color><color=grey>]</color>", method =() => Settings.ChangeArraylistScale(), enableMethod =() => Settings.ChangeArraylistScale(), disableMethod =() => Settings.ChangeArraylistScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the arraylist."},
                new ButtonInfo { buttonText = "Change Overlay Scale", overlapText = "Change Overlay Scale <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangeOverlayScale(), enableMethod =() => Settings.ChangeOverlayScale(), disableMethod =() => Settings.ChangeOverlayScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the overlay."},
                new ButtonInfo { buttonText = "Change Page Size", overlapText = "Change Page Size <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Settings.ChangePageSize(), enableMethod =() => Settings.ChangePageSize(), disableMethod =() => Settings.ChangePageSize(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of buttons per page."},
                new ButtonInfo { buttonText = "Custom Menu Theme", enableMethod =() => Settings.CustomMenuTheme(), disableMethod =() => Settings.FixTheme(), toolTip = "Changes the theme of the menu to a custom one."},
                new ButtonInfo { buttonText = "Change Custom Menu Theme", method =() => Settings.ChangeCustomMenuTheme(), isTogglable = false, toolTip = "Changes the theme of custom the menu."},
                new ButtonInfo { buttonText = "Custom Menu Background", enableMethod =() => Settings.CustomMenuBackground(), disableMethod =() => Settings.FixMenuBackground(), toolTip = $"Changes the background of the menu to a custom image. You can change the photo inside of your Gorilla Tag File ({PluginInfo.BaseDirectory}/iiMenu_CustomMenuBackground.txt)."},
                new ButtonInfo { buttonText = "Change Page Type", method =() => Settings.ChangePageType(), enableMethod =() => Settings.ChangePageType(), disableMethod =() => Settings.ChangePageType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of page buttons."},
                new ButtonInfo { buttonText = "Change Arrow Type", method =() => Settings.ChangeArrowType(), enableMethod =() => Settings.ChangeArrowType(), disableMethod =() => Settings.ChangeArrowType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of arrows on the page buttons."},
                new ButtonInfo { buttonText = "Change Font Type", method =() => Settings.ChangeFontType(), enableMethod =() => Settings.ChangeFontType(), disableMethod =() => Settings.ChangeFontType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of font."},
                new ButtonInfo { buttonText = "Change Font Style Type", method =() => Settings.ChangeFontStyleType(), enableMethod =() => Settings.ChangeFontStyleType(), disableMethod =() => Settings.ChangeFontStyleType(false), incremental = true, isTogglable = false, toolTip = "Changes the style of the font."},
                new ButtonInfo { buttonText = "Change Input Text Color", overlapText = "Change Input Text Color <color=grey>[</color><color=green>Green</color><color=grey>]</color>", method =() => Settings.ChangeInputTextColor(), enableMethod =() => Settings.ChangeInputTextColor(), disableMethod =() => Settings.ChangeInputTextColor(false), incremental = true, isTogglable = false, toolTip = "Changes the color of the input indicator next to the buttons."},
                new ButtonInfo { buttonText = "Change PC Menu Background", method =() => Settings.ChangePCUI(), enableMethod =() => Settings.ChangePCUI(), disableMethod =() => Settings.ChangePCUI(false), incremental = true, isTogglable = false, toolTip = "Changes the background of the PC ui."},
                new ButtonInfo { buttonText = "Change Notification Time", overlapText = "Change Notification Time <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeNotificationTime(), enableMethod =() => Settings.ChangeNotificationTime(), disableMethod =() => Settings.ChangeNotificationTime(false), incremental = true, isTogglable = false, toolTip = "Changes the time before a notification is removed."},
                new ButtonInfo { buttonText = "Change Notification Sound", overlapText = "Change Notification Sound <color=grey>[</color><color=green>None</color><color=grey>]</color>", method =() => Settings.ChangeNotificationSound(true, true), enableMethod =() => Settings.ChangeNotificationSound(true, true), disableMethod =() => Settings.ChangeNotificationSound(false, true), incremental = true, isTogglable = false, toolTip = "Changes the sound that plays when receiving a notification."},
                new ButtonInfo { buttonText = "Change Narration Voice", overlapText = "Change Narration Voice <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeNarrationVoice(), enableMethod =() => Settings.ChangeNarrationVoice(), disableMethod =() => Settings.ChangeNarrationVoice(false), incremental = true, isTogglable = false, toolTip = "Changes the voice of the narrator."},
                new ButtonInfo { buttonText = "Change Pointer Position", method =() => Settings.ChangePointerPosition(), enableMethod =() => Settings.ChangePointerPosition(), disableMethod =() => Settings.ChangePointerPosition(false), incremental = true, isTogglable = false, toolTip = "Changes the position of the pointer."},

                new ButtonInfo { buttonText = "Swap GUI Colors", toolTip = "Swaps the GUI's colors to the enabled color, for darker themes."},
                new ButtonInfo { buttonText = "Swap Button Colors", enableMethod =() => swapButtonColors = true, disableMethod =() => swapButtonColors = false, toolTip = "Swaps the colors of the page buttons, disconnect button, search button, and return button to be the opposite color."},
                new ButtonInfo { buttonText = "Swap Ghostview Colors", toolTip = "Swaps the ghostview's colors to the enabled color, for darker themes."},

                new ButtonInfo { buttonText = "Change Gun Line Quality", overlapText = "Change Gun Line Quality <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Settings.ChangeGunLineQuality(), enableMethod =() => Settings.ChangeGunLineQuality(), disableMethod =() => Settings.ChangeGunLineQuality(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of points on your gun."},
                new ButtonInfo { buttonText = "Change Gun Variation", overlapText = "Change Gun Variation <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeGunVariation(), enableMethod =() => Settings.ChangeGunVariation(), disableMethod =() => Settings.ChangeGunVariation(false), incremental = true, isTogglable = false, toolTip = "Changes the look of the gun."},
                new ButtonInfo { buttonText = "Change Gun Direction", overlapText = "Change Gun Direction <color=grey>[</color><color=green>Default</color><color=grey>]</color>", method =() => Settings.ChangeGunDirection(), enableMethod =() => Settings.ChangeGunDirection(), disableMethod =() => Settings.ChangeGunDirection(false), incremental = true, isTogglable = false, toolTip = "Changes the direction of the gun."},

                new ButtonInfo { buttonText = "Gun Sounds", enableMethod =() => GunSounds = true, disableMethod =() => GunSounds = false, toolTip = "Gives the gun laser sounds for when you press grip and trigger."},
                new ButtonInfo { buttonText = "Gun Particles", enableMethod =() => GunParticles = true, disableMethod =() => GunParticles = false, toolTip = "Gives the gun particles when you shoot it."},
                new ButtonInfo { buttonText = "Swap Gun Hand", enableMethod =() => SwapGunHand = true, disableMethod =() => SwapGunHand = false, toolTip = "Swaps the hand gun mods work with."},
                new ButtonInfo { buttonText = "Gripless Guns", enableMethod =() => GriplessGuns = true, disableMethod =() => GriplessGuns = false, toolTip = "Forces your grip to be held for guns."},
                new ButtonInfo { buttonText = "Triggerless Guns", enableMethod =() => TriggerlessGuns = true, disableMethod =() => TriggerlessGuns = false, toolTip = "Forces your trigger to be held for guns."},
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
                new ButtonInfo { buttonText = "Narrate Notifications", enableMethod =() => narrateNotifications = true, disableMethod =() => narrateNotifications = false, toolTip = "Narrates all notifications with text to speech."},

                new ButtonInfo { buttonText = "Disable Notifications", enableMethod =() => disableNotifications = true, disableMethod =() => disableNotifications = false, toolTip = "Disables all notifications."},
                new ButtonInfo { buttonText = "Disable Master Client Notifications", enableMethod =() => disableMasterClientNotifications = true, disableMethod =() => disableMasterClientNotifications = false, toolTip = "Disables all notifications regarding master client."},
                new ButtonInfo { buttonText = "Disable Room Notifications", enableMethod =() => disableRoomNotifications = true, disableMethod =() => disableRoomNotifications = false, toolTip = "Disables all notifications regarding the room."},
                new ButtonInfo { buttonText = "Disable Player Notifications", enableMethod =() => disablePlayerNotifications = true, disableMethod =() => disablePlayerNotifications = false, toolTip = "Disables all notifications regarding players."},
                new ButtonInfo { buttonText = "Disable Enabled GUI", overlapText = "Disable Arraylist GUI", enableMethod =() => showEnabledModsVR = false, disableMethod =() => showEnabledModsVR = true, toolTip = "Disables the GUI that shows the enabled mods."},
                new ButtonInfo { buttonText = "Disable Incremental Buttons", enableMethod =() => incrementalButtons = false, disableMethod =() => incrementalButtons = true, toolTip = "Disables the buttons with the increment and decrement buttons next to it."},
                new ButtonInfo { buttonText = "Disable Disconnect Button", enableMethod =() => disableDisconnectButton = true, disableMethod =() => disableDisconnectButton = false, toolTip = "Disables the disconnect button at the top of the menu."},
                new ButtonInfo { buttonText = "Disable Menu Title", method =() => { if (pageButtonType == 1) buttonOffset = 1; else buttonOffset = -1; hidetitle = true; }, disableMethod =() => { if (pageButtonType == 1) buttonOffset = 2; else buttonOffset = 0; hidetitle = false; }, toolTip = "Hides the menu title, allowing for more buttons per page."},
                new ButtonInfo { buttonText = "Disable Search Button", enableMethod =() => disableSearchButton = true, disableMethod =() => disableSearchButton = false, toolTip = "Disables the search button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Return Button", enableMethod =() => disableReturnButton = true, disableMethod =() => disableReturnButton = false, toolTip = "Disables the return button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Info Button", enableMethod =() => enableDebugButton = true, disableMethod =() => enableDebugButton = false, toolTip = "Shows an information button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Page Buttons", enableMethod =() => Settings.DisablePageButtons(), disableMethod =() => disablePageButtons = false, toolTip = "Disables the page buttons. Recommended with Joystick Menu."},
                new ButtonInfo { buttonText = "Disable Page Number", enableMethod =() => noPageNumber = true, disableMethod =() => noPageNumber = false, toolTip = "Disables the current page number in the title text."},
                new ButtonInfo { buttonText = "Disable FPS Counter", enableMethod =() => disableFpsCounter = true, disableMethod =() => disableFpsCounter = false, toolTip = "Disables the FPS counter."},
                new ButtonInfo { buttonText = "Disable Drop Menu", enableMethod =() => dropOnRemove = false, disableMethod =() => dropOnRemove = true, toolTip = "Makes the menu despawn instead of falling."},
                new ButtonInfo { buttonText = "Disable Board Colors", overlapText = "Disable Custom Boards", enableMethod =() => Settings.DisableBoardColors(), disableMethod =() => Settings.EnableBoardColors(), toolTip = "Disables the board colors to look legitimate on screen share."},
                new ButtonInfo { buttonText = "Disable Custom Text Colors", enableMethod =() => disableBoardTextColor = true, disableMethod =() => disableBoardTextColor = false, toolTip = "Disables the text colors on the boards to make them match their original theme."},

                new ButtonInfo { buttonText = "Info Hide ID", enableMethod =() => Settings.hideId = true, disableMethod =() => Settings.hideId = false, toolTip = "Hides your ID in the information page."},

                new ButtonInfo { buttonText = "Hide Text on Camera", enableMethod =() => hideTextOnCamera = true, disableMethod =() => hideTextOnCamera = false, overlapText = "Streamer Mode Menu Text", toolTip = "Makes the menu's text only render on VR."},
                new ButtonInfo { buttonText = "Hide Pointer", enableMethod =() => hidePointer = true, disableMethod =() => hidePointer = false, toolTip = "Hides the pointer above your head."},
                new ButtonInfo { buttonText = "Hide Settings", enableMethod =() => hideSettings = true, disableMethod =() => hideSettings = false, toolTip = "Hides all settings from the Enabled Mods tab, and all arraylists."},
                new ButtonInfo { buttonText = "High Quality Text", enableMethod =() => highQualityText = true, disableMethod =() => highQualityText = false, toolTip = "Makes the menu's text really high quality."},

                new ButtonInfo { buttonText = "Advanced Arraylist", enableMethod =() => advancedArraylist = true, disableMethod =() => advancedArraylist = false, toolTip = "Updates the FPS Counter less, making it easier to read."},
                new ButtonInfo { buttonText = "Flip Arraylist", enableMethod =() => flipArraylist = true, disableMethod =() => flipArraylist = false, toolTip = "Flips the arraylist at the top of the screen."},

                new ButtonInfo { buttonText = "Slow FPS Counter", enableMethod =() => fpsCountTimed = true, disableMethod =() => fpsCountTimed = false, toolTip = "Updates the FPS Counter less, making it easier to read."},

                new ButtonInfo { buttonText = "Disable Ghostview", enableMethod =() => disableGhostview = true, disableMethod =() => disableGhostview = false, toolTip = "Disables the transparent rig when you're in ghost."},
                new ButtonInfo { buttonText = "Legacy Ghostview", enableMethod =() => legacyGhostview = true, disableMethod =() => legacyGhostview = false, toolTip = "Reverts the transparent rig to the two balls when you're in ghost."},

                new ButtonInfo { buttonText = "No Global Search", enableMethod =() => nonGlobalSearch = true, disableMethod =() => nonGlobalSearch = false, toolTip = "Makes the search button only search for mods in the current subcategory, unless on the main page."},

                new ButtonInfo { buttonText = "Menu Presets", method =() => currentCategoryName = "Menu Presets", isTogglable = false, toolTip = "Opens the page of presets."},
                new ButtonInfo { buttonText = "Backup Preferences", enableMethod =() => BackupPreferences = true, disableMethod =() => BackupPreferences = false, toolTip = "Automatically saves a copy of your preferences every minute."},
                new ButtonInfo { buttonText = "Save Preferences", method =() => Settings.SavePreferences(), isTogglable = false, toolTip = "Saves your preferences to a file."},
                new ButtonInfo { buttonText = "Load Preferences", method =() => Settings.LoadPreferences(), isTogglable = false, toolTip = "Loads your preferences from a file."},
                new ButtonInfo { buttonText = "Disable Autosave", method =() => autoSaveDelay = Time.time + 1f, toolTip = "Disables the auto save mechanism."},
                new ButtonInfo { buttonText = "Panic", method =() => Settings.Panic(), isTogglable = false, toolTip = "Disables every single active mod."},
            },

            new ButtonInfo[] { // Room Settings [3]
                new ButtonInfo { buttonText = "Exit Room Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "crTime", overlapText = "Change Reconnect Time <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Settings.ChangeReconnectTime(), enableMethod =() => Settings.ChangeReconnectTime(), disableMethod =() => Settings.ChangeReconnectTime(false), incremental = true, isTogglable = false, toolTip = "Changes the amount of time waited before attempting to reconnect again."},

                new ButtonInfo { buttonText = "Primary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking primary."},
                new ButtonInfo { buttonText = "Secondary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking secondary."},
                new ButtonInfo { buttonText = "Joystick Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking your joystick."},
            },

            new ButtonInfo[] { // Movement Settings [4]
                new ButtonInfo { buttonText = "Exit Movement Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Platform Type", overlapText = "Change Platform Type <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePlatformType(), enableMethod =() => Movement.ChangePlatformType(), disableMethod =() => Movement.ChangePlatformType(false), incremental = true, isTogglable = false, toolTip = "Changes the type of the platforms."},
                new ButtonInfo { buttonText = "Change Platform Shape", overlapText = "Change Platform Shape <color=grey>[</color><color=green>Sphere</color><color=grey>]</color>", method =() => Movement.ChangePlatformShape(), enableMethod =() => Movement.ChangePlatformShape(), disableMethod =() => Movement.ChangePlatformShape(false), incremental = true, isTogglable = false, toolTip = "Changes the shape of the platforms."},

                new ButtonInfo { buttonText = "Platform Gravity", toolTip = "Makes platforms fall instead of instantly deleting them."},
                new ButtonInfo { buttonText = "Platform Outlines", toolTip = "Makes platforms have outlines."},
                new ButtonInfo { buttonText = "Non-Sticky Platforms", toolTip = "Makes your platforms no longer sticky."},

                new ButtonInfo { buttonText = "Grip Noclip", toolTip = "Activates noclip with your <color=green>grip</color> instead."},

                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeFlySpeed(), enableMethod =() => Movement.ChangeFlySpeed(), disableMethod =() => Movement.ChangeFlySpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the fly mods, including iron man."},
                new ButtonInfo { buttonText = "Change Arm Length", overlapText = "Change Arm Length <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeArmLength(), enableMethod =() => Movement.ChangeArmLength(), disableMethod =() => Movement.ChangeArmLength(false), incremental = true, isTogglable = false, toolTip = "Changes the length of the long arm mods, not including iron man."},
                new ButtonInfo { buttonText = "Change Speed Boost Amount", overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeSpeedBoostAmount(), enableMethod =() => Movement.ChangeSpeedBoostAmount(), disableMethod =() => Movement.ChangeSpeedBoostAmount(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the speed boost mod."},
                new ButtonInfo { buttonText = "Change Pull Mod Power", overlapText = "Change Pull Mod Power <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePullModPower(), enableMethod =() => Movement.ChangePullModPower(), disableMethod =() => Movement.ChangePullModPower(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the pull mod."},
                new ButtonInfo { buttonText = "Change Prediction Amount", overlapText = "Change Prediction Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePredictionAmount(), enableMethod =() => Movement.ChangePredictionAmount(), disableMethod =() => Movement.ChangePredictionAmount(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the predictions."},
                new ButtonInfo { buttonText = "Change Timer Speed", overlapText = "Change Timer Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeTimerSpeed(), enableMethod =() => Movement.ChangeTimerSpeed(), disableMethod =() => Movement.ChangeTimerSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the timer mod."},
                new ButtonInfo { buttonText = "cdSpeed", overlapText = "Change Drive Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeDriveSpeed(), enableMethod =() => Movement.ChangeDriveSpeed(), disableMethod =() => Movement.ChangeDriveSpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the drive mod."},
                new ButtonInfo { buttonText = "Factored Speed Boost", toolTip = "Factors your current speed into the speed boost, giving you a positive effect even if you're tagged."},
                new ButtonInfo { buttonText = "Disable Max Speed Modification", toolTip = "Makes your max speed not change, so you can't be detected of using a speed boost."},
                new ButtonInfo { buttonText = "Disable Size Changer Buttons", toolTip = "Disables the size changer's buttons, so hitting grip or trigger or whatever won't do anything."},

                new ButtonInfo { buttonText = "Hand Oriented Strafe", toolTip = "Makes the strafe mods move you in the forward direction of your hand."},
                new ButtonInfo { buttonText = "Disable Stationary WASD Fly", toolTip = "Disables WASD Fly keeping you in-place when not moving."},

                new ButtonInfo { buttonText = "Networked Grapple Mods", method =() => Movement.NetworkedGrappleMods(), toolTip = "Makes the spider man and grappling hook mods networked, showing the line for everyone. This requires a balloon."},

                new ButtonInfo { buttonText = "Non-Togglable Ghost", toolTip = "Makes the ghost mod only activate when holding down the button."},
                new ButtonInfo { buttonText = "Non-Togglable Invisible", toolTip = "Makes the invisible mod only activate when holding down the button"},

                new ButtonInfo { buttonText = "Reverse Intercourse", toolTip = "Turns you into the bottom when using the intercourse gun."}
            },

            new ButtonInfo[] { // Projectiles Settings [5]
                new ButtonInfo { buttonText = "Exit Projectile Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Projectile", overlapText = "Change Projectile <color=grey>[</color><color=green>Snowball</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectile(), enableMethod =() => Projectiles.ChangeProjectile(), disableMethod =() => Projectiles.ChangeProjectile(false), incremental = true, isTogglable = false, toolTip = "Changes the projectile of the projectile spam." },
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

                new ButtonInfo { buttonText = "Change Projectile Delay", overlapText = "Change Projectile Delay <color=grey>[</color><color=green>0.1</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectileDelay(true, true), enableMethod =() => Projectiles.ChangeProjectileDelay(true, true), disableMethod =() => Projectiles.ChangeProjectileDelay(false, true), incremental = true, isTogglable = false, toolTip = "Gives the projectiles a delay before spawning another." }
            },

            new ButtonInfo[] { // Room Mods [6]
                new ButtonInfo { buttonText = "Exit Room Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Disconnect", method =() => Important.DisconnectR(), isTogglable = false, toolTip = "Disconnects you from the the room."},
                new ButtonInfo { buttonText = "Reconnect", method =() => Important.ReconnectR(), isTogglable = false, toolTip = "Reconnects you from and to the the room."},

                new ButtonInfo { buttonText = "Cancel Reconnect", method =() => Important.CancelReconnect(), isTogglable = false, toolTip = "Cancels the reconnection loop."},

                new ButtonInfo { buttonText = "Join Last Room", method =() => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(lastRoom, JoinType.Solo), isTogglable = false, toolTip = "Joins the last room you left."},
                new ButtonInfo { buttonText = "Join Random", method =() => Important.JoinRandomR(), isTogglable = false, toolTip = "Joins a random public room." },

                new ButtonInfo { buttonText = "Create Public", method =() => Important.CreateRoom(Important.RandomRoomName(), true), isTogglable = false, toolTip = "Creates a public room."},
                new ButtonInfo { buttonText = "Create Duplicate", method =() => { Patches.RoomPatch.enabled = true; Patches.RoomPatch.duplicate = true; }, disableMethod =() =>  Patches.RoomPatch.enabled = false, toolTip = "Creates a duplicate code whenever trying to join a room."},

                new ButtonInfo { buttonText = "Fast Disconnect", method =() => Patches.SinglePlayerPatch.enabled = true, disableMethod =() =>  Patches.SinglePlayerPatch.enabled = false, toolTip = "Uses the fastest method of disconnecting possible."},
                new ButtonInfo { buttonText = "Fast Join Rooms", method =() => { Patches.RoomPatch.enabled = true; Patches.RoomPatch.duplicate = false; }, disableMethod =() =>  Patches.RoomPatch.enabled = false, toolTip = "Uses the fastest method of joining rooms possible when trying to join a room."},
                new ButtonInfo { buttonText = "Join Menu Room", method =() => PhotonNetworkController.Instance.AttemptToJoinSpecificRoom($"<$II_{PluginInfo.Version}>", JoinType.Solo), isTogglable = false, toolTip = "Connects you to a room that is exclusive to ii's <b>Stupid</b> Menu users." },

                new ButtonInfo { buttonText = "Bypass Join Room Type", method =() => Patches.JoinedRoomPatch.enabled = true, disableMethod =() => Patches.JoinedRoomPatch.enabled = false, toolTip = "Bypasses the immediate disconnection when trying to join a room that is in another map."},

                new ButtonInfo { buttonText = "Auto Join Room \"RUN\"", method =() => Important.QueueRoom("RUN"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"RUN\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY\"", method =() => Important.QueueRoom("DAISY"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY09\"", method =() => Important.QueueRoom("DAISY09"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY09\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"PBBV\"", method =() => Important.QueueRoom("PBBV"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"PBBV\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"BOT\"", method =() => Important.QueueRoom("BOT"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"BOT\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"LUCIO\"", method =() => Important.QueueRoom("LUCIO"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"LUCIO\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"VEN1\"", method =() => Important.QueueRoom("VEN1"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"VEN1\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"SREN17\"", method =() => Important.QueueRoom("SREN17"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"SREN17\" every couple of seconds until connected." },

                new ButtonInfo { buttonText = "Auto Join Room \"MOD\"", method =() => Important.QueueRoom("MOD"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"MOD\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"MODS\"", method =() => Important.QueueRoom("MODS"), isTogglable = false, toolTip = "Automatically attempts to connect to room \"MODS\" every couple of seconds until connected." },
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

            new ButtonInfo[] { // Important Mods [7]
                new ButtonInfo { buttonText = "Exit Important Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Exit Gorilla Tag", method =() => Application.Quit(), isTogglable = false, toolTip = "Closes Gorilla Tag."},
                new ButtonInfo { buttonText = "Restart Gorilla Tag", method =() => Important.RestartGame(), isTogglable = false, toolTip = "Restarts Gorilla Tag."},

                new ButtonInfo { buttonText = "Anti Hand Tap", enableMethod =() => Patches.HandTapPatch.enabled = true, disableMethod =() => Patches.HandTapPatch.enabled = false, toolTip = "Stops all hand tap sounds from being played."},
                new ButtonInfo { buttonText = "First Person Camera", enableMethod =() => Important.EnableFPC(), method =() => Important.MoveFPC(), disableMethod =() => Important.DisableFPC(), toolTip = "Makes your camera output what you see in VR."},
                new ButtonInfo { buttonText = "Force Enable Hands", method =() => Important.ForceEnableHands(), toolTip = "Prevents your hands from disconnecting."},

                new ButtonInfo { buttonText = "Oculus Report Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Important.OculusReportMenu(), toolTip = "Opens the Oculus report menu when holding <color=green>X</color>."},

                new ButtonInfo { buttonText = "Accept TOS", method =() => Important.AcceptTOS(), disableMethod =() => Patches.TOSPatch.enabled = false, toolTip = "Accepts the Terms of Service for you."},
                new ButtonInfo { buttonText = "Redeem Shiny Rocks", method =() => CoroutineManager.RunCoroutine(Important.RedeemShinyRocks()), isTogglable = false, toolTip = "Redeems the 500 Shiny Rocks K-ID gives you."},

                new ButtonInfo { buttonText = "Copy Player Position", method =() => Important.CopyPlayerPosition(), isTogglable = false, toolTip = "Copies the current player position to the clipboard." },

                new ButtonInfo { buttonText = "Clear Notifications", method =() => NotifiLib.ClearAllNotifications(), isTogglable = false, toolTip = "Clears your notifications. Good for when they get stuck."},

                new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => PhotonNetworkController.Instance.disableAFKKick = true, disableMethod =() => PhotonNetworkController.Instance.disableAFKKick = false, toolTip = "Doesn't let you get kicked for being AFK."},
                new ButtonInfo { buttonText = "Disable Network Triggers", enableMethod =() => Patches.NetworkTriggerPatch.enabled = true, disableMethod =() => Patches.NetworkTriggerPatch.enabled = false, toolTip = "Disables the network triggers, so you can change maps without disconnecting."},
                new ButtonInfo { buttonText = "Disable Map Triggers", enableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(false), disableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab").SetActive(true), toolTip = "Disables the map triggers, so you can change maps without loading them."},
                new ButtonInfo { buttonText = "Disable Quit Box", enableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(false), disableMethod =() => GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true), toolTip = "Disables the box under the map that closes your game."},
                new ButtonInfo { buttonText = "Physical Quit Box", enableMethod =() => Important.PhysicalQuitbox(), disableMethod =() => Important.NotPhysicalQuitbox(), toolTip = "Makes the quitbox physical, letting you see and walk on it."},

                new ButtonInfo { buttonText = "Steam Refund Timer", method =() => { if (playTime > 6000f) { NotifiLib.information["REFUND"] = "Refund soon"; } else { NotifiLib.information.Remove("REFUND"); } }, enableMethod =() => Important.CheckNewAcc(), disableMethod =() => NotifiLib.information.Remove("REFUND"), toolTip = "Alerts you when you are nearby the steam refund time."},

                new ButtonInfo { buttonText = "Disable Pitch Scaling", method =() => Important.DisablePitchScaling(), disableMethod =() => Important.EnablePitchScaling(), toolTip = "Disables the pitch effects on players' voices when they are a different scale."},
                new ButtonInfo { buttonText = "Disable Mouth Movement", method =() => Important.DisableMouthMovement(), disableMethod =() => Important.EnableMouthMovement(), toolTip = "Disables your mouth from moving."},

                new ButtonInfo { buttonText = "60 FPS", method =() => Important.CapFPS(60), toolTip = "Caps your FPS at 60 frames per second."},
                new ButtonInfo { buttonText = "45 FPS", method =() => Important.CapFPS(45), toolTip = "Caps your FPS at 45 frames per second."},
                new ButtonInfo { buttonText = "30 FPS", method =() => Important.CapFPS(30), toolTip = "Caps your FPS at 30 frames per second."},
                new ButtonInfo { buttonText = "15 FPS", method =() => Important.CapFPS(15), toolTip = "Caps your FPS at 15 frames per second."},
                new ButtonInfo { buttonText = "Unlock FPS", method =() => Important.UncapFPS(), disableMethod =() => Application.targetFrameRate = 144, toolTip = "Unlocks your FPS."},

                new ButtonInfo { buttonText = "PC Button Click", method =() => Important.PCButtonClick(), toolTip = "Lets you click in-game buttons with your mouse."},

                new ButtonInfo { buttonText = "Unlock Competitive Queue", method =() => GorillaComputer.instance.CompQueueUnlockButtonPress(), isTogglable = false, toolTip = "Permanently unlocks the competitive queue."},

                new ButtonInfo { buttonText = "Tag Lag Detector", method =() => Important.TagLagDetector(), toolTip = "Detects when the master client is not currently allowing tag requests."},

                new ButtonInfo { buttonText = "Anti Stump Kick", enableMethod =() => Patches.GroupPatch.enabled = true, disableMethod =() => Patches.GroupPatch.enabled = false, toolTip = "Stops people from group kicking you."},

                new ButtonInfo { buttonText = "Connect to US", method =() => PhotonNetwork.ConnectToRegion("us"), isTogglable = false, toolTip = "Connects you to the United States servers."},
                new ButtonInfo { buttonText = "Connect to US West", method =() => PhotonNetwork.ConnectToRegion("usw"), isTogglable = false, toolTip = "Connects you to the western United States servers."},
                new ButtonInfo { buttonText = "Connect to EU", method =() => PhotonNetwork.ConnectToRegion("eu"), isTogglable = false, toolTip = "Connects you to the Europe servers."},
            },

            new ButtonInfo[] { // Safety Mods [8]
                new ButtonInfo { buttonText = "Exit Safety Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "No Finger Movement", method =() => Safety.NoFinger(), toolTip = "Makes your fingers not move, so you can use wall walk without getting called out." },

                new ButtonInfo { buttonText = "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Safety.FakeOculusMenu(), toolTip = "Imitates opening your Oculus menu when holding <color=green>X</color>."},
                new ButtonInfo { buttonText = "Fake Report Menu <color=grey>[</color><color=green>Y</color><color=grey>]</color>", method =() => Safety.FakeReportMenu(), toolTip = "Imitates opening the report menu when holding <color=green>Y</color>."},
                new ButtonInfo { buttonText = "Fake Broken Controller <color=grey>[</color><color=green>X</color><color=grey>]</color>", enableMethod =() => GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<Collider>().enabled = false, method =() => Safety.FakeBrokenController(), disableMethod =() => GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHandTriggerCollider").GetComponent<Collider>().enabled = true, toolTip = "Makes you look like your left controller is broken, hold <color=green>X</color> to move your right hand with your left hand."},
                new ButtonInfo { buttonText = "Fake Power Off <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Safety.FakePowerOff(), toolTip = "Imitates turning off your headset when holding down your <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Fake Valve Tracking <color=grey>[</color><color=green>J</color><color=grey>]</color>", enableMethod =() => Patches.TorsoPatch.VRRigLateUpdate += Safety.FakeValveTracking, disableMethod =() => Patches.TorsoPatch.VRRigLateUpdate -= Safety.FakeValveTracking, toolTip = "Imitates what happens when your headset disconnects on a Valve Index when holding your <color=green>right joystick</color>."},

                new ButtonInfo { buttonText = "Disable Gamemode Buttons", enableMethod =() => Safety.SetGamemodeButtonActive(false), disableMethod =() => Safety.SetGamemodeButtonActive(), toolTip = "Disables the gamemode buttons."},
                new ButtonInfo { buttonText = "Spoof Support Page", method =() => Safety.SpoofSupportPage(), toolTip = "Makes the support page appear as if you are on Oculus."},

                new ButtonInfo { buttonText = "Flush RPCs", method =() => RPCProtection(), isTogglable = false, toolTip = "Flushes all RPC calls, good after you stop spamming." },
                new ButtonInfo { buttonText = "Anti Crash", enableMethod =() => Patches.AntiCrashPatch.enabled = true, disableMethod =() => Patches.AntiCrashPatch.enabled = false, toolTip = "Prevents crashers from completely annihilating your computer."},
                new ButtonInfo { buttonText = "Auto Clear Cache", method =() => Safety.AutoClearCache(), toolTip = "Automatically clears your game's cache (garbage collector) every minute to prevent memory leaks."},
                new ButtonInfo { buttonText = "Anti Moderator", method =() => Safety.AntiModerator(), toolTip = "When someone with the stick joins, you get disconnected and their player ID and room code gets saved to a file."},

                new ButtonInfo { buttonText = "Bypass Automod", method =() => Safety.BypassAutomod(), toolTip = "Attempts to bypass automod muting yourself and others."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Reconnect</color><color=grey>]</color>", method =() => Safety.AntiReportReconnect(), toolTip = "Disconnects and reconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Join Random</color><color=grey>]</color>", method =() => Safety.AntiReportJoinRandom(), toolTip = "Connects you to a random the room when anyone comes near your report button."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>", enableMethod =() => AntiOculusReport = true, disableMethod =() => AntiOculusReport = false, toolTip = "Disconnects you from the room when you get reported with the Oculus report menu."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Anti Cheat</color><color=grey>]</color>", enableMethod =() => Patches.Safety.AntiCheat.AntiACReport = true, disableMethod =() => Patches.Safety.AntiCheat.AntiACReport = false, toolTip = "Disconnects you from the room when you get reported by the anti cheat."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Notify</color><color=grey>]</color>", method =() => Safety.AntiReportNotify(), toolTip = "Tells you when people come near your report button, but doesn't do anything."},

                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>", enableMethod =() => Patches.Safety.AntiCheat.AntiCheatSelf = true, disableMethod =() => Patches.Safety.AntiCheat.AntiCheatSelf = false, toolTip = "Gives you a notification every time you have been reported by the anti cheat."},
                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>All</color><color=grey>]</color>", enableMethod =() => Patches.Safety.AntiCheat.AntiCheatAll = true, disableMethod =() => Patches.Safety.AntiCheat.AntiCheatAll = false, toolTip = "Gives you a notification every time anyone has been reported by the anti cheat."},

                new ButtonInfo { buttonText = "Change Identity", overlapText = "Change Identity <color=grey>[</color><color=green>New</color><color=grey>]</color>", method =() => Safety.ChangeIdentity(), isTogglable = false, toolTip = "Changes your name and color to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeIdentityRegular(), isTogglable = false, toolTip = "Changes your name and color to something a regular player would have."},

                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>New</color><color=grey>]</color>", method =() => Safety.ChangeIdentityOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeIdentityRegularOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a regular player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Child</color><color=grey>]</color>", method =() => Safety.ChangeIdentityMinigamesOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a kid would have."},

                new ButtonInfo { buttonText = "FPS Spoof", method =() => Safety.FPSSpoof(), disableMethod =() => Patches.FPSPatch.enabled = false, toolTip = "Makes your FPS appear to be 90 for other players and the competitive bot."},
                new ButtonInfo { buttonText = "Name Spoof", enableMethod =() => Patches.ColorPatch.nameSpoofEnabled = true, disableMethod =() => Patches.ColorPatch.nameSpoofEnabled = false, toolTip = "Changes your name on the leaderboard to something random, but not on your rig."},
                new ButtonInfo { buttonText = "Color Spoof", enableMethod =() => Patches.ColorPatch.patchEnabled = true, disableMethod =() => Patches.ColorPatch.patchEnabled = false, toolTip = "Makes your color appear different to every player."},

                new ButtonInfo { buttonText = "Ranked Tier Spoof", method =() => Safety.SpoofRank(true, Safety.targetRank), disableMethod =() => Safety.SpoofRank(false), toolTip = "Spoofs your rank for competitive lobbies, letting you join higher or lower lobbies."},
                new ButtonInfo { buttonText = "Ranked Platform Spoof", method =() => Safety.SpoofPlatform(true, "Quest"), disableMethod =() => Safety.SpoofPlatform(false), toolTip = "Spoofs your platform for competitive lobbies, letting you join quest lobbies."},
                new ButtonInfo { buttonText = "Unload Menu", method =() => UnloadMenu(), isTogglable = false, toolTip = "Unloads the menu from your game."}
            },

            new ButtonInfo[] { // Movement Mods [9]
                new ButtonInfo { buttonText = "Exit Movement Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Platforms", method =() => Movement.Platforms(), toolTip = "Platforms, they do not show for other players."},
                new ButtonInfo { buttonText = "Trigger Platforms", method =() => Movement.TriggerPlatforms(), toolTip = "Platforms, they do not show for other players."},
                new ButtonInfo { buttonText = "Frozone", method =() => Movement.Frozone(), toolTip = "Spawns slippery blocks under your hands using <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Platform Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.PlatformSpam(), toolTip = "Spawns legacy platforms rapidly at your hand for those who have networked platforms."},
                new ButtonInfo { buttonText = "Platform Gun", method =() => Movement.PlatformGun(), toolTip = "Spawns legacy platforms rapidly wherever your hand desires for those who have networked platforms."},

                new ButtonInfo { buttonText = "Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Fly(), toolTip = "Sends your character forwards when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Trigger Fly <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Movement.TriggerFly(), toolTip = "Sends your character forwards when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Noclip Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.NoclipFly(), toolTip = "Sends your character forwards and makes you go through objects when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Joystick Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickFly(), toolTip = "Sends your character in whatever direction you are pointing your <color=green>joystick</color> in."},
                new ButtonInfo { buttonText = "Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.BarkFly(), toolTip = "Acts like the fly that Bark has. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "Hand Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.HandFly(), toolTip = "Sends your character in your hand's direction when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Zero Gravity Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.ZeroGravitySlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner without gravity, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Slingshot Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.VelocityBarkFly(), toolTip = "Acts like the fly that Bark has, mixed with slingshot fly. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "WASD Fly", enableMethod=() => { Movement.lastPosition = GorillaTagger.Instance.rigidbody.transform.position; }, method =() => Movement.WASDFly(), disableMethod =() => { GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.parent.rotation = Quaternion.Euler(0, 0, 0); }, toolTip = "Moves your rig with <color=green>WASD</color>."},

                new ButtonInfo { buttonText = "Dash <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Dash(), toolTip = "Flings your character forwards when pressing <color=green>A</color>."},
                new ButtonInfo { buttonText = "Iron Man", method =() => Movement.IronMan(), toolTip = "Turns you into iron man, rotate your hands around to change direction."},
                new ButtonInfo { buttonText = "Spider Man", method =() => Movement.SpiderMan(), disableMethod =() => Movement.DisableSpiderMan(), toolTip = "Turns you into spider man, use your <color=green>grips</color> to shoot webs."},
                new ButtonInfo { buttonText = "Grappling Hooks", method =() => Movement.GrapplingHooks(), disableMethod =() => Movement.DisableSpiderMan(), toolTip = "Gives you grappling hooks, use your <color=green>grips</color> to shoot them."},
                new ButtonInfo { buttonText = "Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.Drive(), toolTip = "Lets you drive around in your invisible car. Use the <color=green>joystick</color> to move."},

                new ButtonInfo { buttonText = "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Movement.Noclip(), toolTip = "Makes you go through objects when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Up And Down", method =() => Movement.UpAndDown(), toolTip = "Makes you go up when holding your <color=green>trigger</color>, and down when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Left And Right", method =() => Movement.LeftAndRight(), toolTip = "Makes you go left when holding your <color=green>trigger</color>, and right when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Forwards And Backwards", method =() => Movement.ForwardsAndBackwards(), toolTip = "Makes you go forwards when holding your <color=green>trigger</color>, and backwards when holding your <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Size Changer", method =() => Movement.SizeChanger(), enableMethod =() => Movement.DisableSizeChanger(), disableMethod =() => Movement.DisableSizeChanger(), toolTip = "Increase your size by holding <color=green>trigger</color>, and decrease your size by holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Auto Walk <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.AutoWalk(), toolTip = "Makes your character automatically walk when using the <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Auto Funny Run <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AutoFunnyRun(), toolTip = "Makes your character automatically funny run when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Pinch Climb <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AutoPinchClimb(), toolTip = "Makes your character automatically pinch climb when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Elevator Climb <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AutoElevatorClimb(), toolTip = "Makes your character automatically elevator climb when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Branch <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AutoBranch(), toolTip = "Makes your character automatically branch when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Force Tag Freeze", method =() => Movement.ForceTagFreeze(), disableMethod =() => Movement.NoTagFreeze(), toolTip = "Forces tag freeze on your character."},
                new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), toolTip = "Disables tag freeze on your character."},
                new ButtonInfo { buttonText = "Low Gravity", method =() => Movement.LowGravity(), toolTip = "Makes gravity lower on your character."},
                new ButtonInfo { buttonText = "Zero Gravity", method =() => Movement.ZeroGravity(), toolTip = "Disables gravity on your character."},
                new ButtonInfo { buttonText = "High Gravity", method =() => Movement.HighGravity(), toolTip = "Makes gravity higher on your character."},
                new ButtonInfo { buttonText = "Reverse Gravity", method =() => Movement.ReverseGravity(), disableMethod =() => Movement.UnflipCharacter(), toolTip = "Reverses gravity on your character."},

                new ButtonInfo { buttonText = "Rewind <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Movement.Rewind(), disableMethod =() => Movement.ClearRewind(), toolTip = "Brings you back in time when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Weak Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WeakWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but weaker."},
                new ButtonInfo { buttonText = "Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Strong Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.StrongWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but stronger."},
                new ButtonInfo { buttonText = "Legitimate Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.LegitimateWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but less noticable."},
                new ButtonInfo { buttonText = "Spider Walk", method =() => Movement.SpiderWalk(), disableMethod =() => Movement.UnflipCharacter(), toolTip = "Makes your gravity and character towards any wall you touch. This may cause motion sickness."},

                new ButtonInfo { buttonText = "Teleport to Random", method =() => Movement.TeleportToRandom(), isTogglable = false, toolTip = "Teleports you to a random player."},
                new ButtonInfo { buttonText = "Teleport to Map", method =() => Movement.EnterTeleportToMap(), isTogglable = false, toolTip = "Teleports you to a map of your choosing."},
                new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), toolTip = "Teleports to wherever your hand desires."},
                new ButtonInfo { buttonText = "Airstrike", method =() => Movement.Airstrike(), toolTip = "Teleports to wherever your hand desires, except farther up, then launches you back down."},

                new ButtonInfo { buttonText = "Checkpoint <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.Checkpoint(), disableMethod =() => Movement.DisableCheckpoint(), toolTip = "Place a checkpoint with <color=green>grip</color> and teleport to it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Advanced Checkpoints <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AdvancedCheckpoints(), disableMethod =() => Movement.DisableAdvancedCheckpoints(), toolTip = "Place checkpoints with <color=green>grip</color>, use your joystick to swap between checkpoints, and teleport to your selected checkpoint with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ender Pearl <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.EnderPearl(), disableMethod =() => Movement.DestroyEnderPearl(), toolTip = "Gives you a throwable ender pearl when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "C4 <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.Bomb(), disableMethod =() => Movement.DisableBomb(), toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>."},

                new ButtonInfo { buttonText = "Punch Mod", method =() => Movement.PunchMod(), toolTip = "Lets people punch you across the map."},
                new ButtonInfo { buttonText = "Telekinesis", method =() => Movement.Telekinesis(), toolTip = "Lets people control you with nothing but the power of their finger."},
                new ButtonInfo { buttonText = "Solid Players", method =() => Movement.SolidPlayers(), toolTip = "Lets you walk on top of other players."},
                new ButtonInfo { buttonText = "Pull Mod <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.PullMod(), toolTip = "Pulls you more whenever you walk to simulate speed without modifying your velocity."},
                new ButtonInfo { buttonText = "Long Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", overlapText = "Playspace Abuse <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.LongJump(), toolTip = "Makes you look like you're legitimately long jumping when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Velocity Long Arms", overlapText = "Predictions", enableMethod =() => Movement.CreateVelocityTrackers(), method =() => Movement.VelocityLongArms(), disableMethod =() => Movement.DestroyVelocityTrackers(), toolTip = "Moves your arms farther depending on how fast you move them."},

                new ButtonInfo { buttonText = "Timer", method =() => Movement.Timer(), disableMethod =() => Time.timeScale = 1f, toolTip = "Speeds up or slows down the time of your game."},
                new ButtonInfo { buttonText = "Lag Range", method =() => Movement.LagRange(), toolTip = "Dynamically changes how much your rig updates depending on how close you are to others."},

                new ButtonInfo { buttonText = "Speed Boost", method =() => Movement.SpeedBoost(), toolTip = "Changes your speed to whatever you set it to."},
                new ButtonInfo { buttonText = "Grip Speed Boost <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripSpeedBoost(), toolTip = "Changes your speed to whatever you set it to when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Speed Boost <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickSpeedBoost(), toolTip = "Changes your speed to whatever you set it to when holding <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Dynamic Speed Boost", method =() => Movement.DynamicSpeedBoost(), toolTip = "Dynamically changes your speed to whatever you set it to when tagged players get closer to you."},
                new ButtonInfo { buttonText = "Uncap Max Velocity", method =() => GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = float.MaxValue, toolTip = "Lets you go as fast as you want without hitting the velocity limit."},
                new ButtonInfo { buttonText = "Always Max Velocity", method =() => Movement.AlwaysMaxVelocity(), toolTip = "Always makes you go as fast as the velocity limit."},

                new ButtonInfo { buttonText = "Slippery Hands", enableMethod =() => Patches.SlidePatch.everythingSlippery = true, disableMethod =() => Patches.SlidePatch.everythingSlippery = false, toolTip = "Makes everything ice, as in extremely slippery."},
                new ButtonInfo { buttonText = "Grippy Hands", enableMethod =() => Patches.SlidePatch.everythingGrippy = true, disableMethod =() => Patches.SlidePatch.everythingGrippy = false, toolTip = "Covers your hands in grip tape, letting you wallrun as high as you want."},
                new ButtonInfo { buttonText = "Sticky Hands", method =() => Movement.StickyHands(), disableMethod =() => Movement.DisableStickyHands(), toolTip = "Makes your hands really sticky."},
                new ButtonInfo { buttonText = "Climby Hands", method =() => Movement.ClimbyHands(), disableMethod =() => Movement.DisableClimbyHands(), toolTip = "Lets you climb everything like a rope."},
                new ButtonInfo { buttonText = "Disable Hands", method =() => Movement.SetHandEnabled(false), disableMethod =() => Movement.SetHandEnabled(true), toolTip = "Disables your hand colliders."},

                new ButtonInfo { buttonText = "Slide Control", enableMethod =() => Movement.EnableSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice perfectly."},
                new ButtonInfo { buttonText = "Weak Slide Control", enableMethod =() => Movement.EnableWeakSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice a little more perfect than before."},

                new ButtonInfo { buttonText = "Throw Controllers", method =() => Movement.ThrowControllers(), toolTip = "Lets you throw your controllers with <color=green>X</color> or <color=green>A</color>."},
                new ButtonInfo { buttonText = "Controller Flick", enableMethod =() => Movement.EnableControllerFlick(), method =() => Movement.ControllerFlick(), disableMethod =() => Movement.DisableControllerFlick(), toolTip = "Flicks your controllers in a similar way to disconnecting them with <color=green>X</color> or <color=green>A</color>."},

                new ButtonInfo { buttonText = "Uncap Arm Length", method =() => GorillaLocomotion.GTPlayer.Instance.maxArmLength = float.MaxValue, disableMethod =() => GorillaLocomotion.GTPlayer.Instance.maxArmLength = 1f, toolTip = "Lets you go as fast as you want without hitting the velocity limit."},
                new ButtonInfo { buttonText = "Steam Long Arms", method =() => Movement.EnableSteamLongArms(), disableMethod =() => Movement.DisableSteamLongArms(), toolTip = "Gives you long arms similar to override world scale."},
                new ButtonInfo { buttonText = "Stick Long Arms", method =() => Movement.StickLongArms(), toolTip = "Makes you look like you're using sticks."},
                new ButtonInfo { buttonText = "Multiplied Long Arms", method =() => Movement.MultipliedLongArms(), toolTip = "Gives you a weird version of long arms."},
                new ButtonInfo { buttonText = "Vertical Long Arms", method =() => Movement.VerticalLongArms(), toolTip = "Gives you a version of long arms to help you vertically."},
                new ButtonInfo { buttonText = "Horizontal Long Arms", method =() => Movement.HorizontalLongArms(), toolTip = "Gives you a version of long arms to help you horizontally."},

                new ButtonInfo { buttonText = "Extenders <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.Extenders(), enableMethod =() => Movement.extendingTime = 0f, disableMethod =() => Movement.DisableSteamLongArms(), toolTip = "Steam long arms, but it slowly disables when holding the right trigger."},

                new ButtonInfo { buttonText = "Flick Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FlickJump(), toolTip = "Makes your hand go down really fast when holding <color=green>A</color>."},

                new ButtonInfo { buttonText = "Bunny Hop", method =() => Movement.BunnyHop(), toolTip = "Makes you automatically jump when on the ground."},
                new ButtonInfo { buttonText = "Strafe", method =() => Movement.Strafe(), toolTip = "Makes you strafe when in the air."},
                new ButtonInfo { buttonText = "Dynamic Strafe", method =() => Movement.DynamicStrafe(), toolTip = "Makes you dynamically strafe when in the air."},
                new ButtonInfo { buttonText = "Grip Bunny Hop <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripBunnyHop(), toolTip = "Makes you automatically jump when on the ground when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Grip Strafe <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripStrafe(), toolTip = "Makes you strafe when in the air when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Grip Dynamic Strafe <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripDynamicStrafe(), toolTip = "Makes you dynamically strafe strafe when in the air when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Ground Helper <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GroundHelper(), toolTip = "Helps you run on ground when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Bouncy", enableMethod =() => Movement.PreBouncy(), method =() => Movement.Bouncy(), disableMethod =() => Movement.PostBouncy(), toolTip = "Makes you really bouncy when on the ground."},
                new ButtonInfo { buttonText = "Solid Water", enableMethod =() => Movement.SolidWater(), disableMethod =() => Movement.FixWater(), toolTip = "Makes the water solid in the beach map." },
                new ButtonInfo { buttonText = "Disable Water", enableMethod =() => Movement.DisableWater(), disableMethod =() => Movement.FixWater(), toolTip = "Disables the water in the beach map." },
                new ButtonInfo { buttonText = "Air Swim", method =() => Movement.AirSwim(), disableMethod =() => Movement.DisableAirSwim(), toolTip = "Puts you in a block of water, letting you swim in the air." },
                new ButtonInfo { buttonText = "Fast Swim", method =() => Movement.SetSwimSpeed(10f), disableMethod =() => Movement.SetSwimSpeed(), toolTip = "Lets you swim faster in water." },
                new ButtonInfo { buttonText = "Disable Air", overlapText = "Disable Wind Barriers", enableMethod =() => { Patches.ForcePatch.enabled = true; GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(false); }, disableMethod =() => { Patches.ForcePatch.enabled = false; GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(true); }, toolTip = "Disables the wind barriers in every map." },

                new ButtonInfo { buttonText = "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Ghost(), disableMethod =() => Movement.EnableRig(), toolTip = "Keeps your rig still when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", method =() => Movement.Invisible(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes you go invisible when holding <color=green>B</color>."},

                new ButtonInfo { buttonText = "Rig Gun", method =() => Movement.RigGun(), toolTip = "Moves your rig to wherever your hand desires."},
                new ButtonInfo { buttonText = "Grab Rig <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GrabRig(), toolTip = "Lets you grab your rig when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Spin Head X", method =() => Fun.SpinHead("x"), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the X axis."},
                new ButtonInfo { buttonText = "Spin Head Y", method =() => Fun.SpinHead("y"), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Y axis."},
                new ButtonInfo { buttonText = "Spin Head Z", method =() => Fun.SpinHead("z"), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Z axis."},

                new ButtonInfo { buttonText = "Spaz Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", enableMethod =() => Movement.EnableSpazRig(), method =() => Movement.SpazRig(), disableMethod =() => Movement.DisableSpazRig(), toolTip = "Makes every part of your rig spaz out a little bit when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Rig Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazHands(), toolTip = "Makes your rig's hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazRealHands(), toolTip = "Makes your hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Random Spaz Head Position", enableMethod =() => Movement.EnableSpazHead(), method =() => Movement.RandomSpazHeadPosition(), disableMethod =() => Movement.FixHeadPosition(), toolTip = "Makes your head position spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Random Spaz Head", overlapText = "Random Spaz Head Rotation", method =() => Movement.RandomSpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head rotation spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Spaz Head Position", enableMethod =() => Movement.EnableSpazHead(), method =() => Movement.SpazHeadPosition(), disableMethod =() => Movement.FixHeadPosition(), toolTip = "Makes your head position spaz out."},
                new ButtonInfo { buttonText = "Spaz Head", overlapText = "Spaz Head Rotation", method =() => Movement.SpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head rotation spaz out."},
                new ButtonInfo { buttonText = "Spaz Head X", method =() => Fun.SpazHead("x"), disableMethod =() => Fun.FixHead(), toolTip = "Spaz your head on the X axis."},
                new ButtonInfo { buttonText = "Spaz Head Y", method =() => Fun.SpazHead("y"), disableMethod =() => Fun.FixHead(), toolTip = "Spaz your head on the Y axis."},
                new ButtonInfo { buttonText = "Spaz Head Z", method =() => Fun.SpazHead("z"), disableMethod =() => Fun.FixHead(), toolTip = "Spaz your head on the Z axis."},

                new ButtonInfo { buttonText = "Laggy Rig", method =() => Movement.LaggyRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig laggy."},
                new ButtonInfo { buttonText = "Smooth Rig", method =() => PhotonNetwork.SerializationRate = 30, disableMethod =() => PhotonNetwork.SerializationRate = 10, toolTip = "Makes your rig really smooth."},
                new ButtonInfo { buttonText = "Update Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.UpdateRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig in place. Whenever you click <color=green>A</color>, your rig will update."},

                new ButtonInfo { buttonText = "Freeze Rig Limbs <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigLimbs(), toolTip = "Makes your hands and head freeze on your rig, but not your body, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Freeze Rig Body <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigBody(), toolTip = "Makes your body freeze on your rig, but not your hands and head, when holding <color=green>A</color>."},

                new ButtonInfo { buttonText = "Paralyze Rig", method =() => Movement.ParalyzeRig(), disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Removes your arms from your rig."},
                new ButtonInfo { buttonText = "Chicken Rig", method =() => Movement.ChickenRig(), disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Makes your rig look like a chicken."},
                new ButtonInfo { buttonText = "Amputate Rig", method =() => Movement.AmputateRig(), disableMethod =() => VRRig.LocalRig.enabled = true, toolTip = "Removes all of your limbs from your rig."},

                new ButtonInfo { buttonText = "Spin Rig Body", method =() => Movement.SetBodyPatch(true), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Makes your body spin around, but not your head."},
                new ButtonInfo { buttonText = "Spaz Rig Body", method =() => Movement.SetBodyPatch(true, 1), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Gives your body a seizure, randomizing its rotation."},
                new ButtonInfo { buttonText = "Reverse Rig Body", method =() => Movement.SetBodyPatch(true, 2), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Flips your body around backwards, but not your head."},
                new ButtonInfo { buttonText = "Rec Room Body", method =() => Movement.RecRoomBody(), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Flips your body around backwards, but not your head."},
                new ButtonInfo { buttonText = "Freeze Body Rotation <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.FreezeBodyRotation(), disableMethod =() => Movement.SetBodyPatch(false), toolTip = "Freezes your body rotation in place, but not your head, when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Auto Dance <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoDance(), toolTip = "Makes you dance when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto Griddy <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoGriddy(), toolTip = "Makes you griddy when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", overlapText = "T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoTPose(), toolTip = "Makes you t pose when holding <color=green>A</color>. Good for fly trolling."},
                new ButtonInfo { buttonText = "Helicopter <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Helicopter(), toolTip = "Turns you into a helicopter when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Beyblade <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Beyblade(), toolTip = "Turns you into a beyblade when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Fan <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Fan(), toolTip = "Turns you into a fan when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ghost Animations", method =() => Movement.GhostAnimations(), disableMethod =() => Movement.DisableGhostAnimations(), toolTip = "Makes you look like a ghost, making your movement snappy and slow."},
                new ButtonInfo { buttonText = "Minecraft Animations", method =() => Movement.MinecraftAnimations(), disableMethod =() => Movement.EnableRig(), toolTip = "Puts your hands down, and makes you walk when holding <color=green>A</color>. You can also point with <color=green>B</color>."},

                new ButtonInfo { buttonText = "Stare at Nearby", overlapText = "Stare At Player Nearby", enableMethod =() => Patches.TorsoPatch.VRRigLateUpdate += Movement.StareAtNearby, disableMethod =() => Patches.TorsoPatch.VRRigLateUpdate -= Movement.StareAtNearby, toolTip = "Makes you stare at the nearest player."},
                new ButtonInfo { buttonText = "Stare at Player Gun", method =() => Movement.StareAtGun(), disableMethod =() => Patches.TorsoPatch.VRRigLateUpdate -= Movement.StareAtTarget, toolTip = "Makes you stare at whoever your hand desires."},
                new ButtonInfo { buttonText = "Stare at All Players", enableMethod =() => Movement.StareAtAll(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Makes you stare at everyone in the room."},
                new ButtonInfo { buttonText = "Floating Rig", enableMethod =() => Movement.EnableFloatingRig(), method =() => Movement.FloatingRig(), disableMethod =() => Movement.DisableFloatingRig(), toolTip = "Makes your rig float."},

                new ButtonInfo { buttonText = "Bees", method =() => Movement.Bees(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig teleport to random players, imitating the bees ghost."},

                new ButtonInfo { buttonText = "Piggyback Gun", method =() => Movement.PiggybackGun(), toolTip = "Teleports you on top of whoever your hand desires repeatedly."},
                new ButtonInfo { buttonText = "Piggyback All", enableMethod =() => Movement.PiggybackAll(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Makes you appear on top of everyone in the room"},

                new ButtonInfo { buttonText = "Copy Movement Gun", method =() => Movement.CopyMovementGun(), toolTip = "Makes your rig copy the movement of whoever your hand desires."},
                new ButtonInfo { buttonText = "Copy Movement All", enableMethod =() => Movement.CopyMovementAll(), disableMethod =() => { Patches.SerializePatch.OverrideSerialization = null; Movement.followPositions.Clear(); }, toolTip = "Makes your rig copy the movement of every player in the room."},

                new ButtonInfo { buttonText = "Follow Player Gun", method =() => Movement.FollowPlayerGun(), toolTip = "Flies your rig towards whoever your hand desires."},
                new ButtonInfo { buttonText = "Follow All Players", enableMethod =() => Movement.FollowAllPlayers(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Flies your rig towards everyone in the room."},

                new ButtonInfo { buttonText = "Orbit Player Gun", method =() => Movement.OrbitPlayerGun(), toolTip = "Orbits your rig around whoever your hand desires."},
                new ButtonInfo { buttonText = "Orbit All Players", enableMethod =() => Movement.OrbitAllPlayers(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Orbits your rig around everyone in the room."},

                new ButtonInfo { buttonText = "Jumpscare Gun", method =() => Movement.JumpscareGun(), toolTip = "Makes you jumpscare whoever your hand desires."},
                new ButtonInfo { buttonText = "Jumpscare All", enableMethod =() => Movement.JumpscareAll(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Makes you jumpscare everyone in the room."},

                new ButtonInfo { buttonText = "Annoy Player Gun", method =() => Movement.AnnoyPlayerGun(), toolTip = "Spazzes your body around whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Annoy All Players", enableMethod =() => Movement.AnnoyAllPlayers(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Spazzes your body around everyone in the room, with sounds."},

                new ButtonInfo { buttonText = "Intercourse Gun", method =() => Movement.IntercourseGun(), toolTip = "Makes you thrust whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Intercourse All", enableMethod =() => Movement.IntercourseAll(), method =() => Movement.IntercourseNoises(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Makes you thrust everyone in the room, with sounds."},

                new ButtonInfo { buttonText = "Head Gun", method =() => Movement.HeadGun(), toolTip = "Makes you thrust whoever your hand desires, but lower, with sounds. I hate you all."},
                new ButtonInfo { buttonText = "Head All", enableMethod =() => Movement.HeadAll(), method =() => Movement.IntercourseNoises(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Makes you thrust everyone in the room, but lower, with sounds. I hate you all."}
            },

            new ButtonInfo[] { // Advantage Mods [10]
                new ButtonInfo { buttonText = "Exit Advantage Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Tag Self", method =() => Advantages.TagSelf(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tags yourself."},

                new ButtonInfo { buttonText = "Tag Aura", method =() => Advantages.TagAura(), toolTip = "Moves your hand into nearby players when tagged."},
                new ButtonInfo { buttonText = "Grip Tag Aura <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Advantages.GripTagAura(), toolTip = "Moves your hand into nearby players when tagged and when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Tag Aura <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Advantages.JoystickTagAura(), toolTip = "Moves your hand into nearby players when tagged and when pressing <color=green>joystick</color>."},

                new ButtonInfo { buttonText = "Tag Aura Gun", method =() => Advantages.TagAuraGun(), toolTip = "Gives a player tag aura."},
                new ButtonInfo { buttonText = "Tag Aura All", method =() => Advantages.TagAuraAll(), toolTip = "Gives all players tag aura."},

                new ButtonInfo { buttonText = "Tag Reach", method =() => Advantages.TagReach(), disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Makes your hand tag hitbox larger."},

                new ButtonInfo { buttonText = "Tag Gun", method =() => Advantages.TagGun(), toolTip = "Tags whoever your hand desires."},
                new ButtonInfo { buttonText = "Flick Tag Gun", method =() => Advantages.FlickTagGun(), toolTip = "Moves your hand to wherever your hand desires in an attempt to tag whoever your hand desires."},

                new ButtonInfo { buttonText = "Tag All", method =() => Advantages.TagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the the room."},
                new ButtonInfo { buttonText = "Tag Bot", method =() => Advantages.TagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, use <color=green>B</color> to turn it off."},

                new ButtonInfo { buttonText = "Instant Tag Gun", method =() => Advantages.InstantTagGun(), toolTip = "Tags whoever your hand desires instantly."},
                new ButtonInfo { buttonText = "Instant Tag All", method =() => Advantages.InstantTagAll(), isTogglable = false, toolTip = "Attempts to tag everyone in the the room instantly."},

                new ButtonInfo { buttonText = "No Tag on Join", method =() => Advantages.NoTagOnJoin(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "When you join a the room, you won't be tagged when you join."},
                new ButtonInfo { buttonText = "Untag Self", method =() => Advantages.UntagSelf(), isTogglable = false, toolTip = "Removes you from the list of tagged players."},

                new ButtonInfo { buttonText = "Anti Tag", method =() => Advantages.AntiTag(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "Removes you from the list of tagged players when tagged."},
                new ButtonInfo { buttonText = "Report Anti Tag", enableMethod =() => Advantages.ReportAntiTag(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Prevents you from getting tagged, and whoever tries to tag you will just get reported."},

                new ButtonInfo { buttonText = "No Tag Limit", method =() => GorillaTagger.Instance.maxTagDistance = float.MaxValue, disableMethod =() => GorillaTagger.Instance.maxTagDistance = 1.2f, toolTip = "Removes the distance check when tagging players."}
            },

            new ButtonInfo[] { // Visual Mods [11]
                new ButtonInfo { buttonText = "Exit Visual Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Morning Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(1), toolTip = "Sets your time of day to morning."},
                new ButtonInfo { buttonText = "Day Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(3), toolTip = "Sets your time of day to daytime."},
                new ButtonInfo { buttonText = "Evening Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(7), toolTip = "Sets your time of day to evening."},
                new ButtonInfo { buttonText = "Night Time", method =() => BetterDayNightManager.instance.SetTimeOfDay(0), toolTip = "Sets your time of day to night."},
                new ButtonInfo { buttonText = "Fullbright", enableMethod =() => Visuals.SetFullbrightStatus(true), disableMethod =() => Visuals.SetFullbrightStatus(false), toolTip = "Disables the dynamic lighting in maps that use it."},

                new ButtonInfo { buttonText = "Remove Blindfold", method =() => Visuals.RemoveBlindfold(), toolTip = "Disables the blindfold in the prop hunt map."},

                new ButtonInfo { buttonText = "Rainy Weather", method =() => Visuals.WeatherChange(true), toolTip = "Forces the weather to rain."},
                new ButtonInfo { buttonText = "Clear Weather", method =() => Visuals.WeatherChange(false), toolTip = "Forces the weather to sunny skies all day."},

                new ButtonInfo { buttonText = "Custom Skybox Color", enableMethod =() => Visuals.DoCustomSkyboxColor(), method =() => Visuals.CustomSkyboxColor(), disableMethod =() => Visuals.UnCustomSkyboxColor(), toolTip = "Changes the skybox color to match the menu."},

                new ButtonInfo { buttonText = "Draw Gun", method =() => Visuals.DrawGun(), disableMethod =() => Visuals.DisableDrawGun(), toolTip = "Lets you draw on whatever your hand desires." },

                new ButtonInfo { buttonText = "Gamesense Ring", enableMethod =() => Patches.HandTapPatch.OnHandTap += Visuals.OnHandTapGamesenseRing, method =() => Visuals.GamesenseRing(), disableMethod =() => Visuals.DisableGamesenseRing(), toolTip = "Shows the direction of where people walk around you." },

                new ButtonInfo { buttonText = "Velocity Label", method =() => Visuals.VelocityLabel(), toolTip = "Puts text on your right hand, showing your velocity."},
                new ButtonInfo { buttonText = "Nearby Label", method =() => Visuals.NearbyTaggerLabel(), toolTip = "Puts text on your left hand, showing you the distance of the nearest tagger."},
                new ButtonInfo { buttonText = "Last Label", method =() => Visuals.LastLabel(), toolTip = "Puts text on your left hand, showing you how many untagged people are left."},
                new ButtonInfo { buttonText = "Time Label", method =() => Visuals.TimeLabel(), toolTip = "Puts text on your right hand, showing how long you've been playing for without getting tagged."},

                new ButtonInfo { buttonText = "FPS Overlay", method =() => NotifiLib.information["FPS"] = lastDeltaTime.ToString(), disableMethod =() => NotifiLib.information.Remove("FPS"), toolTip = "Displays your FPS on your screen."},
                new ButtonInfo { buttonText = "Time Overlay", method =() => NotifiLib.information["Time"] = DateTime.Now.ToString("hh:mm tt"), disableMethod =() => NotifiLib.information.Remove("Time"), toolTip = "Displays your current time on your screen."},
                new ButtonInfo { buttonText = "Room Information Overlay", method =() => { if (PhotonNetwork.InRoom) { NotifiLib.information["Room Code"] = PhotonNetwork.CurrentRoom.Name; NotifiLib.information["Players"] = PhotonNetwork.PlayerList.Length.ToString(); } else { NotifiLib.information.Remove("Room Code"); NotifiLib.information.Remove("Players"); } }, disableMethod =() => { NotifiLib.information.Remove("Room Code"); NotifiLib.information.Remove("Players"); }, toolTip = "Displays information about the room on your screen."},
                new ButtonInfo { buttonText = "Networking Overlay", method =() => { NotifiLib.information["Ping"] = PhotonNetwork.GetPing().ToString(); NotifiLib.information["Region"] = NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper(); }, disableMethod =() => { NotifiLib.information.Remove("Ping"); NotifiLib.information.Remove("Region"); }, toolTip = "Displays information about networking on your screen."},
                new ButtonInfo { buttonText = "Clipboard Overlay", method =() => NotifiLib.information["Clip"] = GUIUtility.systemCopyBuffer.Length > 20 ? GUIUtility.systemCopyBuffer[..20] : GUIUtility.systemCopyBuffer, disableMethod =() => NotifiLib.information.Remove("Clip"), toolTip = "Displays your current clipboard on your screen."},

                new ButtonInfo { buttonText = "Info Watch", enableMethod =() => Visuals.WatchOn(), method =() => Visuals.WatchStep(), disableMethod =() => Visuals.WatchOff(), toolTip = "Puts a watch on your hand that tells you the time and your FPS."},

                new ButtonInfo { buttonText = "FPS Boost", enableMethod =() => QualitySettings.globalTextureMipmapLimit = int.MaxValue, disableMethod =() => QualitySettings.globalTextureMipmapLimit = 1, toolTip = "Makes everything low quality in an attempt to boost your FPS."},
                new ButtonInfo { buttonText = "Freeze In Background", enableMethod =() => Application.runInBackground = false, disableMethod =() => Application.runInBackground = true, toolTip = "Freezes the game when the application is not focused."},

                new ButtonInfo { buttonText = "Fake Unban Self", method =() => Visuals.FakeUnbanSelf(), isTogglable = false, toolTip = "Makes it appear as if you're not banned." },

                new ButtonInfo { buttonText = "Jump Predictions", method =() => Visuals.JumpPredictions(), disableMethod =() => Visuals.DisableJumpPredictions(), toolTip = "Shows a visualizer of where the other players will jump."},
                new ButtonInfo { buttonText = "Audio Visualizer", enableMethod =() => Visuals.CreateAudioVisualizer(), method =() => Visuals.AudioVisualizer(), disableMethod =() => Visuals.DestroyAudioVisualizer(), toolTip = "Shows a visualizer of your microphone loudness below your player."},
                new ButtonInfo { buttonText = "Show Server Position", method =() => Visuals.ShowServerPosition(), disableMethod =() => Visuals.DisableShowServerPosition(), toolTip = "Shows your current syncronized position on the server."},

                new ButtonInfo { buttonText = "Visualize Network Triggers", method =() => Visuals.VisualizeNetworkTriggers(), toolTip = "Visualizes the network joining and leaving triggers."},
                new ButtonInfo { buttonText = "Visualize Map Triggers", method =() => Visuals.VisualizeMapTriggers(), toolTip = "Visualizes the map loading and unloading triggers."},

                new ButtonInfo { buttonText = "Name Tags", method =() => Visuals.NameTags(), disableMethod =() => Visuals.DisableNameTags(), toolTip = "Gives players name tags above their heads that show their nickname."},
                new ButtonInfo { buttonText = "Velocity Name Tags", method =() => Visuals.VelocityTags(), disableMethod =() => Visuals.DisableVelocityTags(), toolTip = "Gives players name tags above their heads that show their velocity."},
                new ButtonInfo { buttonText = "FPS Name Tags", method =() => Visuals.FPSTags(), disableMethod =() => Visuals.DisableFPSTags(), toolTip = "Gives players name tags above their heads that show their FPS."},
                new ButtonInfo { buttonText = "Ping Name Tags", method =() => Visuals.PingTags(), disableMethod =() => Visuals.DisablePingTags(), toolTip = "Gives players name tags above their heads that show their ping."},
                new ButtonInfo { buttonText = "Turn Name Tags", method =() => Visuals.TurnTags(), disableMethod =() => Visuals.DisableTurnTags(), toolTip = "Gives players name tags above their heads that show their turn settings."},
                new ButtonInfo { buttonText = "Tagged Name Tags", method =() => Visuals.TaggedTags(), disableMethod =() => Visuals.DisableTaggedTags(), toolTip = "Gives players name tags above their heads that show who tagged them."},

                new ButtonInfo { buttonText = "Fix Rig Colors", method =() => Visuals.FixRigColors(), toolTip = "Fixes a Steam bug where other players' color would be wrong between servers."},
                new ButtonInfo { buttonText = "Disable Rig Lerping", method =() => Visuals.NoSmoothRigs(), disableMethod =() => Visuals.ReSmoothRigs(), toolTip = "Disables rig movement smoothing."},
                new ButtonInfo { buttonText = "Remove Leaves", enableMethod =() => Visuals.EnableRemoveLeaves(), disableMethod =() => Visuals.DisableRemoveLeaves(), toolTip = "Removes leaves on trees, good for branching."},
                new ButtonInfo { buttonText = "Streamer Remove Leaves", enableMethod =() => Visuals.EnableStreamerRemoveLeaves(), disableMethod =() => Visuals.DisableStreamerRemoveLeaves(), toolTip = "Removes leaves on trees in VR, but not on the camera. Good for streaming."},
                new ButtonInfo { buttonText = "Remove Cosmetics", enableMethod =() => Visuals.DisableCosmetics(), disableMethod =() => Visuals.EnableCosmetics(), toolTip = "Locally toggles off your cosmetics, so you can wear sight-blocking cosmetics such as the eyepatch."},

                new ButtonInfo { buttonText = "Cosmetic ESP", method =() => Visuals.CosmeticESP(), toolTip = "Shows beacons above people's heads if they are a Finger Painter, Illustrator, Administrator, Stick, or if they have any unreleased cosmetics."},

                new ButtonInfo { buttonText = "Voice Indicators", method =() => Visuals.VoiceIndicators(), disableMethod =() => Visuals.DisableVoiceIndicators(), toolTip = "Puts voice indicators above people's heads when they're talking."},
                new ButtonInfo { buttonText = "Voice ESP", method =() => Visuals.VoiceESP(), disableMethod =() => Visuals.DisableVoiceIndicators(), toolTip = "Puts voice indicators above people's heads when they're talking, but now they go through walls."},

                new ButtonInfo { buttonText = "Platform Indicators", method =() => Visuals.PlatformIndicators(), disableMethod =() => Visuals.DisablePlatformIndicators(), toolTip = "Puts indicators above people's heads that show what platform they are playing on."},
                new ButtonInfo { buttonText = "Platform ESP", method =() => Visuals.PlatformESP(), disableMethod =() => Visuals.DisablePlatformIndicators(), toolTip = "Puts indicators above people's heads that show what platform they are playing on, but now they go through walls."},

                new ButtonInfo { buttonText = "No Limb Mode", enableMethod =() => Visuals.StartNoLimb(), method =() => Visuals.NoLimbMode(), disableMethod =() => Visuals.EndNoLimb(), toolTip = "Makes your regular rig invisible, and puts balls on your hands."},

                new ButtonInfo { buttonText = "Casual Tracers", method =() => Visuals.CasualTracers(), disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Infection Tracers", method =() => Visuals.InfectionTracers(), disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows everyone."},
                new ButtonInfo { buttonText = "Hunt Tracers", method =() => Visuals.HuntTracers(), disableMethod =() => {Visuals.isLineRenderQueued = true;}, toolTip = "Puts tracers on your right hand. Shows your target and who is hunting you."},

                new ButtonInfo { buttonText = "Casual Box ESP", method =() => Visuals.CasualBoxESP(), disableMethod =() => Visuals.DisableBoxESP(), toolTip = "Acts like casual tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Infection Box ESP", method =() => Visuals.InfectionBoxESP(), disableMethod =() => Visuals.DisableBoxESP(), toolTip = "Acts like infection tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Hunt Box ESP", method =() => Visuals.HuntBoxESP(), disableMethod =() => Visuals.DisableBoxESP(), toolTip = "Acts like hunt tracers color wise, but with boxes."},

                new ButtonInfo { buttonText = "Casual Hollow Box ESP", method =() => Visuals.CasualHollowBoxESP(), disableMethod =() => Visuals.DisableHollowBoxESP(), toolTip = "Acts like casual box ESP, except the box is hollow."},
                new ButtonInfo { buttonText = "Infection Hollow Box ESP", method =() => Visuals.HollowInfectionBoxESP(), disableMethod =() => Visuals.DisableHollowBoxESP(), toolTip = "Acts like infection box ESP, except the box is hollow."},
                new ButtonInfo { buttonText = "Hunt Hollow Box ESP", method =() => Visuals.HollowHuntBoxESP(), disableMethod =() => Visuals.DisableHollowBoxESP(), toolTip = "Acts like hunt box ESP, except the box is hollow."},

                new ButtonInfo { buttonText = "Casual Breadcrumbs", method =() => Visuals.CasualBreadcrumbs(), disableMethod =() => Visuals.DisableBreadcrumbs(), toolTip = "Acts like casual tracers color wise, but with breadcrumbs."},
                new ButtonInfo { buttonText = "Infection Breadcrumbs", method =() => Visuals.InfectionBreadcrumbs(), disableMethod =() => Visuals.DisableBreadcrumbs(), toolTip = "Acts like infection tracers color wise, but with breadcrumbs."},
                new ButtonInfo { buttonText = "Hunt Breadcrumbs", method =() => Visuals.HuntBreadcrumbs(), disableMethod =() => Visuals.DisableBreadcrumbs(), toolTip = "Acts like hunt tracers color wise, but with breadcrumbs."},

                new ButtonInfo { buttonText = "Casual Bone ESP", method =() => Visuals.CasualBoneESP(), disableMethod =() => Visuals.DisableBoneESP(), toolTip = "Acts like casual tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Infection Bone ESP", method =() => Visuals.InfectionBoneESP(), disableMethod =() => Visuals.DisableBoneESP(), toolTip = "Acts like infection tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Hunt Bone ESP", method =() => Visuals.HuntBoneESP(), disableMethod =() => Visuals.DisableBoneESP(), toolTip = "Acts like hunt tracers color wise, but with bones."},

                new ButtonInfo { buttonText = "Casual Wireframe ESP", method =() => Visuals.CasualWireframeESP(), disableMethod =() => Visuals.DisableWireframeESP(), toolTip = "Acts like casual tracers color wise, but with wireframes."},
                new ButtonInfo { buttonText = "Infection Wireframe ESP", method =() => Visuals.InfectionWireframeESP(), disableMethod =() => Visuals.DisableWireframeESP(), toolTip = "Acts like infection tracers color wise, but with wireframes."},
                new ButtonInfo { buttonText = "Hunt Wireframe ESP", method =() => Visuals.HuntWireframeESP(), disableMethod =() => Visuals.DisableWireframeESP(), toolTip = "Acts like hunt tracers color wise, but with wireframes."},

                new ButtonInfo { buttonText = "Casual Chams", method =() => Visuals.CasualChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like casual tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Infection Chams", method =() => Visuals.InfectionChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like infection tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Hunt Chams", method =() => Visuals.HuntChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like hunt tracers color wise, but lets you see their fur through walls."},

                new ButtonInfo { buttonText = "Casual Beacons", method =() => Visuals.CasualBeacons(), disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Acts like casual tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Infection Beacons", method =() => Visuals.InfectionBeacons(), disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Acts like infection tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Hunt Beacons", method =() => Visuals.HuntBeacons(), disableMethod =() => Visuals.isLineRenderQueued = true, toolTip = "Acts like hunt tracers color wise, but it's just a giant line."},

                new ButtonInfo { buttonText = "Casual Distance ESP", method =() => Visuals.CasualDistanceESP(), disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Shows your distance from players."},
                new ButtonInfo { buttonText = "Infection Distance ESP", method =() => Visuals.InfectionDistanceESP(), disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Acts like infection tracers color wise, but with text."},
                new ButtonInfo { buttonText = "Hunt Distance ESP", method =() => Visuals.HuntDistanceESP(), disableMethod =() => Visuals.isNameTagQueued = true, toolTip = "Acts like infection tracers color wise, but with text."},

                new ButtonInfo { buttonText = "Show Pointers", method =() => Visuals.ShowButtonColliders(), disableMethod =() => Visuals.HideButtonColliders(), toolTip = "Shows dots near your hands, such as when you open the menu."},

                new ButtonInfo { buttonText = "Info Watch Menu Name", enableMethod =() => Visuals.infoWatchMenuName = true, disableMethod =() => Visuals.infoWatchMenuName = false, toolTip = "Shows the menu name on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch FPS", enableMethod =() => Visuals.infoWatchFPS = true, disableMethod =() => Visuals.infoWatchFPS = false, toolTip = "Shows your framerate on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Time", enableMethod =() => Visuals.infoWatchTime = true, disableMethod =() => Visuals.infoWatchTime = false, toolTip = "Shows the current time on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Clipboard", enableMethod =() => Visuals.infoWatchClip = true, disableMethod =() => Visuals.infoWatchClip = false, toolTip = "Shows your clipboard on the Info Watch mod."},
                new ButtonInfo { buttonText = "Info Watch Code", enableMethod =() => Visuals.infoWatchCode = true, disableMethod =() => Visuals.infoWatchCode = false, toolTip = "Shows the lobby code on the Info Watch mod."},
            },

            new ButtonInfo[] { // Fun Mods [12]
                new ButtonInfo { buttonText = "Exit Fun Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Soundboard", method =() => Sound.LoadSoundboard(), isTogglable = false, toolTip = "A working, customizable soundboard that lets you play audios through your microphone."},

                new ButtonInfo { buttonText = "Upside Down Head", method =() => Fun.UpsideDownHead(), disableMethod =() => Fun.FixHead(), toolTip = "Flips your head upside down on the Z axis."},
                new ButtonInfo { buttonText = "Backwards Head", method =() => Fun.BackwardsHead(), disableMethod =() => Fun.FixHead(), toolTip = "Rotates your head 180 degrees on the Y axis."},
                new ButtonInfo { buttonText = "Sideways Head", method =() => Fun.SidewaysHead(), disableMethod =() => Fun.FixHead(), toolTip = "Rotates your head 90 degrees on the Y axis."},

                new ButtonInfo { buttonText = "Broken Neck", method =() => Fun.BrokenNeck(), disableMethod =() => Fun.FixHead(), toolTip = "Rotates your head 90 degrees on the Z axis."},

                new ButtonInfo { buttonText = "Head Bang", method =() => Fun.HeadBang(), disableMethod =() => Fun.FixHead(), toolTip = "Bangs your head at the BPM of Paint it Black (159)."},

                new ButtonInfo { buttonText = "Flip Hands", method =() => Fun.FlipHands(), toolTip = "Swaps your hands, left is right and right is left."},
                new ButtonInfo { buttonText = "Loud Hand Taps", method =() => Fun.LoudHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really loud."},
                new ButtonInfo { buttonText = "Silent Hand Taps", method =() => Fun.SilentHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really quiet."},
                new ButtonInfo { buttonText = "Instant Hand Taps", method =() => GorillaTagger.Instance.tapCoolDown = 0f, disableMethod =() => GorillaTagger.Instance.tapCoolDown = 0.33f, toolTip = "Removes the hand tap cooldown."},
                new ButtonInfo { buttonText = "Silent Hand Taps on Tag", method =() => Fun.SilentHandTapsOnTag(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really quiet when you're tagged, good for ambush."},

                new ButtonInfo { buttonText = "Water Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.WaterSplashHands(), toolTip = "Splashes water when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Give Water Splash Hands Gun", method =() => Fun.GiveWaterSplashHandsGun(), toolTip = "Gives whoever your hand desires the water splash hands mod." },

                new ButtonInfo { buttonText = "Water Splash Walk", method =() => Fun.WaterSplashWalk(), toolTip = "Splashes water whenever you take a step."},
                new ButtonInfo { buttonText = "Water Splash Aura", method =() => Fun.WaterSplashAura(), toolTip = "Splashes water around you at random positions."},
                new ButtonInfo { buttonText = "Orbit Water Splash", method =() => Fun.OrbitWaterSplash(), toolTip = "Splashes water orbitally around you."},
                new ButtonInfo { buttonText = "Water Splash Gun", method =() => Fun.WaterSplashGun(), toolTip = "Splashes water wherever your hand desires."},

                new ButtonInfo { buttonText = "Confuse Player Gun", method =() => Movement.ConfusePlayerGun(), toolTip = "Makes whoever your hand desires look like they're going crazy by splashing water on their screen."},
                new ButtonInfo { buttonText = "Confuse All Players", enableMethod =() => Movement.ConfuseAllPlayers(), method =() => Movement.ConfuseAllPlayersSplash(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Splashes water on everyone's screens, making them look like they're going crazy."},

                new ButtonInfo { buttonText = "Schizophrenic Gun", method =() => Movement.SchizophrenicGun(), toolTip = "Makes you not appear for whoever your hand desires."},
                new ButtonInfo { buttonText = "Reverse Schizophrenic Gun", method =() => Movement.ReverseSchizoGun(), toolTip = "Makes you only appear for whoever your hand desires."},

                new ButtonInfo { buttonText = "Boop", method =() => Fun.Boop(), toolTip = "Makes a pop sound when you touch someone's nose."},
                new ButtonInfo { buttonText = "Slap", method =() => Fun.Boop(248), toolTip = "Makes a bong sound when you hit someone's face."},

                new ButtonInfo { buttonText = "Auto Clicker <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.AutoClicker(), toolTip = "Automatically presses  trigger for you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Keyboard Tracker", method =() => Fun.KeyboardTracker(), disableMethod =() => Fun.keyboardTrackerEnabled = false, toolTip = "Tracks everyone's keyboard inputs in the lobby."},

                new ButtonInfo { buttonText = "Tag Sounds", enableMethod =() => Fun.PreloadTagSounds(), method =() => Patches.TagPatch.enabled = true, disableMethod =() => Patches.TagPatch.enabled = false, toolTip = "Plays a selection of dramatic sound effects when tagging players. Credits to Wyndigo for the idea."},

                new ButtonInfo { buttonText = "Mute Gun", method =() => Fun.MuteGun(), toolTip = "Mutes or unmutes whoever your hand desires."},
                new ButtonInfo { buttonText = "Mute All", method =() => Fun.MuteAll(), disableMethod =() => Fun.UnmuteAll(), toolTip = "Mutes everyone in the room."},

                new ButtonInfo { buttonText = "Report Gun", method =() => Fun.ReportGun(), toolTip = "Reports whoever your hand desires for cheating."},
                new ButtonInfo { buttonText = "Report All", method =() => Fun.ReportAll(), isTogglable = false, toolTip = "Reports everyone in the room for cheating."},

                new ButtonInfo { buttonText = "Mute DJ Sets", method =() => Fun.MuteDJSets(), disableMethod =() => Fun.UnmuteDJSets(), toolTip = "Mutes every DJ set so you don't have to hear the worst music known to man."},

                new ButtonInfo { buttonText = "Low Quality Microphone", method =() => Fun.SetMicrophoneQuality(6000, 08000), disableMethod =() => Fun.SetMicrophoneQuality(20000, 16000), toolTip = "Makes your microphone have really bad quality."},
                new ButtonInfo { buttonText = "Loud Microphone", method =() => Fun.SetMicrophoneAmplification(true), disableMethod =() => Fun.SetMicrophoneAmplification(false), toolTip = "Makes your microphone really loud."},
                new ButtonInfo { buttonText = "Mute Microphone", method =() => Fun.MuteMicrophone(true), disableMethod =() => Fun.MuteMicrophone(false), toolTip = "Disables your microphone."},

                new ButtonInfo { buttonText = "High Pitch Microphone", method =() => Fun.SetMicrophonePitch(1.5f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone high pitched."},
                new ButtonInfo { buttonText = "Low Pitch Microphone", method =() => Fun.SetMicrophonePitch(0.5f), disableMethod =() => Fun.SetMicrophonePitch(1f), toolTip = "Makes your microphone low pitched."},

                new ButtonInfo { buttonText = "Reload Microphone", method =() => Fun.ReloadMicrophone(), isTogglable = false,  toolTip = "Reloads / fixes your microphone."},

                new ButtonInfo { buttonText = "Microphone Feedback", method =() => Fun.SetDebugEchoMode(true), disableMethod =() => Fun.SetDebugEchoMode(false), toolTip = "Plays sound coming through your microphone back to your speakers."},
                new ButtonInfo { buttonText = "Copy Voice Gun", method =() => Fun.CopyVoiceGun(), toolTip = "Copies the voice of whoever your hand desires."},

                new ButtonInfo { buttonText = "Activate All Doors <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ActivateAllDoors(), toolTip = "Activates all doors when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Crystals <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.TapAllClass<GorillaCaveCrystal>(), toolTip = "Taps all crystals when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Bells <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.TapAllClass<TappableBell>(), toolTip = "Taps all bells when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Get Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GetBracelet(true), toolTip = "Gives you a party bracelet without needing to be in a party."},
                new ButtonInfo { buttonText = "Spam Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.BraceletSpam(), toolTip = "Spams the party bracelet on and off."},
                new ButtonInfo { buttonText = "Remove Bracelet", method =() => Fun.RemoveBracelet(), isTogglable = false, toolTip = "Disables the party bracelet. This does not kick you from the party."},

                new ButtonInfo { buttonText = "Rainbow Bracelet", method =() => Fun.RainbowBracelet(), disableMethod =() => Fun.RemoveRainbowBracelet(), toolTip = "Gives you a rainbow party bracelet."},

                new ButtonInfo { buttonText = "Quest Noises <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.QuestNoises(), toolTip = "Makes noises at the quest machine in city when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Max Quest Score", method =() => Fun.MaxQuestScore(), toolTip = "Gives you the maximum quest score in the game (99999)."},
                new ButtonInfo { buttonText = "Custom Quest Score", method =() => Fun.CustomQuestScore(), toolTip = "Gives you a custom quest score. You can change this in the settings."},

                new ButtonInfo { buttonText = "Arcade Teleporter Effect Spam", method =() => Fun.ArcadeTeleporterEffectSpam(), toolTip = "Spams the effects on the virtual stump teleporters in the arcade when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Stump Teleporter Effect Spam", method =() => Fun.StumpTeleporterEffectSpam(), toolTip = "Spams the effects on the virtual stump teleporter in forest when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Basement Door Spam", method =() => Fun.BasementDoorSpam(), toolTip = "Repeatedly opens and closes the basement door."},
                new ButtonInfo { buttonText = "Elevator Door Spam", method =() => Fun.ElevatorDoorSpam(), toolTip = "Repeatedly opens and closes the elevator door."},

                new ButtonInfo { buttonText = "Fake FPS", method =() => Fun.FakeFPS(), disableMethod =() => Patches.FPSPatch.enabled = false, toolTip = "Makes your FPS appear to be completely random to other players and the competitive bot."},

                new ButtonInfo { buttonText = "Get Builder Watch", method =() => Fun.GiveBuilderWatch(), isTogglable = false, toolTip = "Gives you the builder watch without needing to be in attic."},
                new ButtonInfo { buttonText = "Remove Builder Watch", method =() => Fun.RemoveBuilderWatch(), isTogglable = false, toolTip = "Disables the builder watch."},

                new ButtonInfo { buttonText = "Grab ID Card <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabIDCard(), toolTip = "Puts the ID card in your hand." },
                new ButtonInfo { buttonText = "Entity Reach", method =() => Patches.EntityGrabPatch.enabled = true, disableMethod =() => Patches.EntityGrabPatch.enabled = false, toolTip = "Gives you the ability to grab entities from farther away in the horror map." },

                new ButtonInfo { buttonText = "Infinite Prop Distance", method =() => Fun.SetPropDistanceLimit(float.MaxValue), disableMethod =() => Fun.SetPropDistanceLimit(0.35f), toolTip = "Removes the distance limit of props in the prop hunt map." },
                new ButtonInfo { buttonText = "Prop Noclip", method =() => Patches.PropPatch.enabled = true, disableMethod =() => Patches.PropPatch.enabled = false, toolTip = "Allows you to put props in walls in the prop hunt map." },

                new ButtonInfo { buttonText = "Spaz Tool Stations", method =() => Fun.SpazToolStations(), toolTip = "Spazzes out the tool purchase stations in the horror map." },
                new ButtonInfo { buttonText = "Purchase All Tool Stations", method =() => Fun.PurchaseAllToolStations(), toolTip = "Makes every tool purchase station force purchase in the horror map." },

                new ButtonInfo { buttonText = "Fast Gliders", enableMethod =() => Fun.ModifyGliderSpeed(0.5f, 0.5f), disableMethod =() => Fun.ModifyGliderSpeed(0.1f, 0.2f), toolTip = "Makes the gliders fast."},
                new ButtonInfo { buttonText = "Slow Gliders", enableMethod =() => Fun.ModifyGliderSpeed(0.05f, 0.05f), disableMethod =() => Fun.ModifyGliderSpeed(0.1f, 0.2f), toolTip = "Makes the gliders slow."},

                new ButtonInfo { buttonText = "Glider Blind Gun", method =() => Overpowered.GliderBlindGun(), toolTip = "Moves all of the gliders to whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Glider Blind All", method =() => Overpowered.GliderBlindAll(), toolTip = "Moves all of the gliders to everyone's faces." },

                new ButtonInfo { buttonText = "Fast Ropes", enableMethod =() => Patches.RopePatch.enabled = true, disableMethod =() => Patches.RopePatch.enabled = false, toolTip = "Makes ropes go five times faster when jumping on them."},

                new ButtonInfo { buttonText = "No Respawn Gliders", enableMethod =() => Patches.GliderPatch.enabled = true, disableMethod =() => Patches.GliderPatch.enabled = false, toolTip = "Doesn't respawn gliders that go too far outside the bounds of clouds."},

                new ButtonInfo { buttonText = "Anti Grab", enableMethod =() => Patches.GrabPatch.enabled = true, disableMethod =() => Patches.GrabPatch.enabled = false, toolTip = "Prevents players from picking you up in guardian."},
                new ButtonInfo { buttonText = "Anti Knockback", enableMethod =() => Patches.KnockbackPatch.enabled = true, disableMethod =() => Patches.KnockbackPatch.enabled = false, toolTip = "Prevents players from knocking you back with snowballs."},

                new ButtonInfo { buttonText = "Fast Throw", method =() => { Patches.VelocityPatch.enabled = true; Patches.VelocityPatch.multipleFactor = 10f; }, disableMethod =() => Patches.VelocityPatch.enabled = false, toolTip = "Multiplies your throw factor by 10."},
                new ButtonInfo { buttonText = "Slow Throw", method =() => { Patches.VelocityPatch.enabled = true; Patches.VelocityPatch.multipleFactor = 0.1f; }, disableMethod =() => Patches.VelocityPatch.enabled = false, toolTip = "Multiplies your throw factor by 0.1."},

                new ButtonInfo { buttonText = "Large Snowballs", enableMethod =() => Patches.EnablePatch.enabled = true, disableMethod =() => Patches.EnablePatch.enabled = false, toolTip = "Makes snowballs by default the largest size."},
                new ButtonInfo { buttonText = "Fast Snowballs", method =() => Fun.FastSnowballs(), disableMethod =() => Fun.FixSnowballs(), toolTip = "Makes snowballs go really fast when thrown."},
                new ButtonInfo { buttonText = "Slow Snowballs", method =() => Fun.SlowSnowballs(), disableMethod =() => Fun.FixSnowballs(), toolTip = "Makes snowballs go really slow when thrown."},
                new ButtonInfo { buttonText = "Multiply Snowballs", method =() => Patches.ThrowPatch.enabled = true, disableMethod =() => Patches.ThrowPatch.enabled = false, toolTip = "Multiplies the snowballs you spawn by 5."},

                new ButtonInfo { buttonText = "Snowball Buttocks", method =() => Fun.SnowballButtocks(), disableMethod =() => Fun.DisableSnowballGenitals(), toolTip = "Gives you fake buttocks using the snowballs." },
                new ButtonInfo { buttonText = "Snowball Breasts", method =() => Fun.SnowballBreasts(), disableMethod =() => Fun.DisableSnowballGenitals(), toolTip = "Gives you fake breasts using the snowballs." },

                new ButtonInfo { buttonText = "Fast Hoverboard", method =() => Fun.FastHoverboard(), disableMethod =() => Fun.FixHoverboard(), toolTip = "Makes your hoverboard go really fast."},
                new ButtonInfo { buttonText = "Slow Hoverboard", method =() => Fun.SlowHoverboard(), disableMethod =() => Fun.FixHoverboard(), toolTip = "Makes your hoverboard go really slow."},

                new ButtonInfo { buttonText = "Rainbow Hoverboard", method =() => Fun.RainbowHoverboard(), toolTip = "Changes your hoverboard's color to be rainbow."},
                new ButtonInfo { buttonText = "Strobe Hoverboard", method =() => Fun.StrobeHoverboard(), toolTip = "Changes your hoverboard's color to flash between black and white."},
                new ButtonInfo { buttonText = "Random Hoverboard", method =() => Fun.RandomHoverboard(), toolTip = "Changes your hoverboard's color to flash random colors."},

                new ButtonInfo { buttonText = "Global Hoverboard", method =() => Fun.GlobalHoverboard(), disableMethod =() => Fun.DisableGlobalHoverboard(), toolTip = "Gives you the hoverboard no matter where you are."},

                new ButtonInfo { buttonText = "Hoverboard Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.HoverboardSpam(), toolTip = "Spams hoverboards from your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Spawn Hoverboard", method =() => Fun.SpawnHoverboard(), isTogglable = false, toolTip = "Spawns a hoverboard at your player position."},

                new ButtonInfo { buttonText = "Start All Races", method =() => Fun.StartAllRaces(), isTogglable = false, toolTip = "Starts every race in the hoverboard map."},

                new ButtonInfo { buttonText = "Override Hand Link", method =() => Patches.GroundedPatch.enabled = true, disableMethod =() => Patches.GroundedPatch.enabled = false, toolTip = "Prioritizes you when you or others grab onto you."},
                new ButtonInfo { buttonText = "Disable Hand Link", method =() => { VRRig.LocalRig.leftHandLink.BreakLink(); VRRig.LocalRig.rightHandLink.BreakLink(); }, toolTip = "Disables you from grabbing onto other players and them from grabbing onto you."},
                new ButtonInfo { buttonText = "Anti Hand Link", method =() => Patches.HandLinkPatch.enabled = true, disableMethod =() => Patches.HandLinkPatch.enabled = false, toolTip = "Disables you from moving when grabbing onto other players."},

                new ButtonInfo { buttonText = "Fast Throw Players", method =() => Patches.ReleasePatch.enabled = true, disableMethod =() => Patches.ReleasePatch.enabled = false, toolTip = "Makes players go really fast when you throw them."},

                new ButtonInfo { buttonText = "Noclip Building", method =() => Fun.NoclipBuilding(), disableMethod =() => Fun.DisableNoclipBuilding(), toolTip = "Disables the colliders of every block in the block map."},
                new ButtonInfo { buttonText = "Overlap Building", enableMethod =() => Patches.OverlapPatch.enabled = true, disableMethod =() => Patches.OverlapPatch.enabled = false, toolTip = "Lets you place pieces inside of each other in the attic."},
                new ButtonInfo { buttonText = "Small Building", enableMethod =() => Patches.BuildPatch.enabled = true, disableMethod =() => Patches.BuildPatch.enabled = false, toolTip = "Lets you build in the block map while small."},
                new ButtonInfo { buttonText = "Multi Grab", method =() => Fun.MultiGrab(), toolTip = "Lets you grab multiple objects."},

                new ButtonInfo { buttonText = "Block Size Toggle", method =() => Fun.AtticSizeToggle(), toolTip = "Toggles your scale when pressing <color=green>grip</color> or <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Grab All Nearby Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabAllBlocksNearby(), toolTip = "Grabs every nearby building block when holding <color=green>G</color>."},
                new ButtonInfo { buttonText = "Grab All Selected Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabAllSelectedNearby(), toolTip = "Grabs every nearby building block that matches your selection when holding <color=green>G</color>."},

                new ButtonInfo { buttonText = "Massive Block <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.MassiveBlock(), toolTip = "Spawns you a massive block when you press <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Select Block Gun", method =() => Fun.SelectBlockGun(), toolTip = "Selects whatever building block your hand desires to be used for the building mods." },
                new ButtonInfo { buttonText = "Copy Block Info Gun", method =() => Fun.CopyBlockInfoGun(), toolTip = "Copies whatever building block your hand desires to be used for the building mods to your clipboard." },
                new ButtonInfo { buttonText = "Building Block Browser", method =() => Fun.BlockBrowser(), isTogglable = false, toolTip = "Browse through every block that you can spawn and select it." },

                new ButtonInfo { buttonText = "Grab Building Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.SpamGrabBlocks(), toolTip = "Forces the building block into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Building Block Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.BuildingBlockMinigun(), toolTip = "Spams building blocks out of your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Building Block Gun", method =() => Fun.BlocksGun(), toolTip = "Moves the building blocks to wherever your hand desires." },

                new ButtonInfo { buttonText = "Orbit Building Blocks", method =() => Fun.OrbitBlocks(), toolTip = "Orbits the building blocks around you." },
                new ButtonInfo { buttonText = "Rain Building Blocks", method =() => Fun.RainBuildingBlocks(), toolTip = "Makes the building blocks fall around you like rain." },
                new ButtonInfo { buttonText = "Building Block Aura", method =() => Fun.BuildingBlockAura(), toolTip = "Moves the building blocks around you at random positions." },

                new ButtonInfo { buttonText = "Place Building Block Gun", method =() => Fun.PlaceBlockGun(), toolTip = "Places whatever building block your hand desires on the last grid space you have placed blocks on." },

                new ButtonInfo { buttonText = "Destroy Building Block Gun", method =() => Fun.DestroyBlockGun(), toolTip = "Shreds whatever building block your hand desires." },
                new ButtonInfo { buttonText = "Destroy Building Blocks", method =() => Fun.DestroyBlocks(), toolTip = "Shreds every building block." },

                new ButtonInfo { buttonText = "Critter Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.CritterSpam(), toolTip = "Spawns critters on your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.CritterMinigun(), toolTip = "Shoots critters out of your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Gun", method =() => Overpowered.CritterGun(), toolTip = "Spawns critters at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Sticky Goo Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StickyGoo), toolTip = "Spams sticky goo in your hand when holding <color=green>grip</color>"},

                new ButtonInfo { buttonText = "Critter Noise Maker Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.NoiseMaker), toolTip = "Spams noise makers in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Stun Bomb Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StunBomb), toolTip = "Spams stun bombs in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Sticky Trap Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.ObjectSpam(CrittersActor.CrittersActorType.StickyTrap), toolTip = "Spams sticky traps in your hand when holding <color=green>grip</color>"},

                new ButtonInfo { buttonText = "Critter Sticky Goo Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StickyGoo), toolTip = "Spams sticky goo at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Noise Maker Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.NoiseMaker), toolTip = "Spams noise makers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Stun Bomb Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StunBomb), toolTip = "Spams stun bombs at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Sticky Trap Gun", method =() => Overpowered.ObjectGun(CrittersActor.CrittersActorType.StickyTrap), toolTip = "Spams sticky traps at wherever your hand desires."},

                new ButtonInfo { buttonText = "Critter Shockwave Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.StunExplosion), toolTip = "Spams the shockwave particles in your hand when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Critter Sticky Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.StickyDeployed), toolTip = "Spams the sticky particles in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Noise Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam(CrittersManager.CritterEvent.NoiseMakerTriggered), toolTip = "Spams the noise particles in your hand when holding <color=green>grip</color>"},
                new ButtonInfo { buttonText = "Critter Particle Effect Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpam((CrittersManager.CritterEvent)UnityEngine.Random.Range(0, 4)), toolTip = "Spams every particle in your hand when holding <color=green>grip</color>"},

                new ButtonInfo { buttonText = "Critter Shockwave Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.StunExplosion), toolTip = "Spams the shockwave particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Sticky Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.StickyDeployed), toolTip = "Spams the sticky particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Noise Effect Gun", method =() => Overpowered.EffectGun(CrittersManager.CritterEvent.NoiseMakerTriggered), toolTip = "Spams the noise particles at wherever your hand desires."},
                new ButtonInfo { buttonText = "Critter Particle Effect Gun", method =() => Overpowered.EffectGun((CrittersManager.CritterEvent)UnityEngine.Random.Range(0, 4)), toolTip = "Spams every particle at wherever your hand desires."},

                new ButtonInfo { buttonText = "Spaz All Moles", method =() => Fun.SpazMoleMachines(), toolTip = "Gives the moles a seizure."},
                new ButtonInfo { buttonText = "Auto Start Moles", method =() => Fun.AutoStartMoles(), toolTip = "Automatically starts the mole games."},
                new ButtonInfo { buttonText = "Auto Hit Moles", method =() => Fun.AutoHitMoleType(false), toolTip = "Hits all of the moles automatically."},
                new ButtonInfo { buttonText = "Auto Hit Hazards", method =() => Fun.AutoHitMoleType(true), toolTip = "Hits all of the hazards automatically."},

                new ButtonInfo { buttonText = "No Respawn Bug", enableMethod =() => Fun.SetRespawnDistance("Floating Bug Holdable"), disableMethod =() => Fun.SetRespawnDistance("Floating Bug Holdable", 50f), toolTip = "Doesn't respawn the bug if it goes too far outside the bounds of forest."},
                new ButtonInfo { buttonText = "No Respawn Firefly", enableMethod =() => Fun.SetRespawnDistance("Firefly"), disableMethod =() => Fun.SetRespawnDistance("Firefly", 50f), toolTip = "Doesn't respawn the firefly if it goes too far outside the bounds of forest."},
                new ButtonInfo { buttonText = "No Respawn Bat", enableMethod =() => Fun.SetRespawnDistance("Cave Bat Holdable"), disableMethod =() => Fun.SetRespawnDistance("Cave Bat Holdable", 50f), toolTip = "Doesn't respawn the bat if it goes too far outside the bounds of caves."},

                new ButtonInfo { buttonText = "Permanent Bug", method =() => Fun.PermanentOwnership("Floating Bug Holdable"), disableMethod =() => Patches.OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the bug."},
                new ButtonInfo { buttonText = "Permanent Firefly", method =() => Fun.PermanentOwnership("Firefly"), disableMethod =() => Patches.OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the firefly."},
                new ButtonInfo { buttonText = "Permanent Bat", method =() => Fun.PermanentOwnership("Cave Bat Holdable"), disableMethod =() => Patches.OwnershipPatch.blacklistedGuards.Clear(), toolTip = "Disables other players from grabbing the bat."},

                new ButtonInfo { buttonText = "Bug Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.BugSpam(), disableMethod =() => Fun.DisableBugSpam(), toolTip = "Shoots the bug and firefly out of your hand repeatedly." },
                new ButtonInfo { buttonText = "Camera Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.CameraSpam(), disableMethod =() => Fun.DisableCameraSpam(), toolTip = "Shoots the camera out of your hand repeatedly." },
                new ButtonInfo { buttonText = "Everything Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.EverythingSpam(), disableMethod =() => Fun.DisableEverythingSpam(), toolTip = "Shoots everything out of your hand repeatedly." },

                new ButtonInfo { buttonText = "Bug Phallus", method =() => Fun.BugPhallus(), toolTip = "Gives you a phallus in the form of the bugs." },
                new ButtonInfo { buttonText = "Bug Phallus Gun", method =() => Fun.BugPhallusGun(), toolTip = "Gives whoever your hand desires a phallus in the form of the bugs." },

                new ButtonInfo { buttonText = "Bug Vibrate Gun", method =() => Fun.BugVibrateGun(), toolTip = "Vibrates the controllers of whoever your hand desires using the bugs." },
                new ButtonInfo { buttonText = "Bug Vibrate All", enableMethod =() => Fun.EnableBugVibrateAll(), method =() => Fun.BugVibrateAll(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Vibrates the controllers of everyone in the room using the bugs." },

                new ButtonInfo { buttonText = "Holster Bug", method =() => Fun.HolsterObject("Floating Bug Holdable", TransferrableObject.PositionState.OnLeftArm), toolTip = "Holsters the bug on your left arm." },
                new ButtonInfo { buttonText = "Holster Firefly", method =() => Fun.HolsterObject("Firefly", TransferrableObject.PositionState.OnRightArm), toolTip = "Holsters the firefly on your right arm." },
                new ButtonInfo { buttonText = "Holster Bat", method =() => Fun.HolsterObject("Cave Bat Holdable", TransferrableObject.PositionState.OnChest), toolTip = "Holsters the bat on your chest." },

                new ButtonInfo { buttonText = "Freeze Bug", method =() => Fun.FreezeObject("Floating Bug Holdable"), toolTip = "Freezes the bug in place." },
                new ButtonInfo { buttonText = "Freeze Firefly", method =() => Fun.FreezeObject("Firefly"), toolTip = "Freezes the firefly in place." },
                new ButtonInfo { buttonText = "Freeze Bat", method =() => Fun.FreezeObject("Cave Bat Holdable"), toolTip = "Freezes the bat in place." },

                new ButtonInfo { buttonText = "Fast Bug", method =() => Fun.SetObjectSpeed("Floating Bug Holdable", 5f), disableMethod =() => Fun.SetObjectSpeed("Floating Bug Holdable"), toolTip = "Speeds up the bug." },
                new ButtonInfo { buttonText = "Fast Firefly", method =() => Fun.SetObjectSpeed("Firefly", 5f), disableMethod =() => Fun.SetObjectSpeed("Firefly"), toolTip = "Speeds up the firefly." },
                new ButtonInfo { buttonText = "Fast Bat", method =() => Fun.SetObjectSpeed("Cave Bat Holdable", 5f), disableMethod =() => Fun.SetObjectSpeed("Cave Bat Holdable"), toolTip = "Speeds up the bat." },

                new ButtonInfo { buttonText = "Fast Bug", method =() => Fun.SetObjectSpeed("Floating Bug Holdable", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Floating Bug Holdable"), toolTip = "Slows down the bug." },
                new ButtonInfo { buttonText = "Fast Firefly", method =() => Fun.SetObjectSpeed("Firefly", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Firefly"), toolTip = "Slows down the firefly." },
                new ButtonInfo { buttonText = "Fast Bat", method =() => Fun.SetObjectSpeed("Cave Bat Holdable", 0.1f), disableMethod =() => Fun.SetObjectSpeed("Cave Bat Holdable"), toolTip = "Slows down the bat." },

                new ButtonInfo { buttonText = "Grab Bug <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Floating Bug Holdable"), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Firefly <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Firefly"), toolTip = "Forces the firefly into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Bat <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ObjectToHand("Cave Bat Holdable"), toolTip = "Forces the bat into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Camera <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabCamera(), toolTip = "Forces the camera into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Balloons <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBalloons(), toolTip = "Forces every single balloon cosmetic into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Gliders <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabGliders(), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Bug Gun", method =() => Fun.ObjectToPointGun("Floating Bug Holdable"), toolTip = "Moves the bug to wherever your hand desires." },
                new ButtonInfo { buttonText = "Firefly Gun", method =() => Fun.ObjectToPointGun("Firefly"), toolTip = "Moves the firefly to wherever your hand desires." },
                new ButtonInfo { buttonText = "Bat Gun", method =() => Fun.ObjectToPointGun("Cave Bat Holdable"), toolTip = "Moves the bat to wherever your hand desires." },
                new ButtonInfo { buttonText = "Camera Gun", method =() => Fun.CameraGun(), toolTip = "Moves the camera to wherever your hand desires." },
                new ButtonInfo { buttonText = "Balloon Gun", method =() => Fun.BalloonGun(), toolTip = "Moves every single balloon cosmetic to wherever your hand desires." },
                new ButtonInfo { buttonText = "Glider Gun", method =() => Fun.GliderGun(), toolTip = "Moves the gliders to wherever your hand desires." },
                new ButtonInfo { buttonText = "Hoverboard Gun", method =() => Fun.HoverboardGun(), toolTip = "Spawns hoverboards at wherever your hand desires."},

                new ButtonInfo { buttonText = "Spaz Bug", method =() => Fun.SpazObject("Floating Bug Holdable"), toolTip = "Gives the bug a seizure." },
                new ButtonInfo { buttonText = "Spaz Firefly", method =() => Fun.SpazObject("Firefly"), toolTip = "Gives the firefly a seizure." },
                new ButtonInfo { buttonText = "Spaz Bat", method =() => Fun.SpazObject("Cave Bat Holdable"), toolTip = "Gives the bat a seizure." },
                new ButtonInfo { buttonText = "Spaz Camera", method =() => Fun.SpazCamera(), toolTip = "Gives the camera a seizure." },
                new ButtonInfo { buttonText = "Spaz Balloons", method =() => Fun.SpazBalloons(), toolTip = "Gives the gliders a seizure." },
                new ButtonInfo { buttonText = "Spaz Gliders", method =() => Fun.SpazGliders(), toolTip = "Gives the gliders a seizure." },
                new ButtonInfo { buttonText = "Spaz Hoverboard", method =() => Fun.SpazHoverboard(), toolTip = "Gives your hoverboard a seizure while holding it."},

                new ButtonInfo { buttonText = "Orbit Bug", method =() => Fun.OrbitObject("Floating Bug Holdable"), toolTip = "Orbits the bug around you." },
                new ButtonInfo { buttonText = "Orbit Firefly", method =() => Fun.OrbitObject("Firefly", 120f), toolTip = "Orbits the firefly around you." },
                new ButtonInfo { buttonText = "Orbit Bat", method =() => Fun.OrbitObject("Cave Bat Holdable", 240f), toolTip = "Orbits the bat around you." },
                new ButtonInfo { buttonText = "Orbit Camera", method =() => Fun.OrbitCamera(), toolTip = "Orbits the camera around you." },
                new ButtonInfo { buttonText = "Orbit Balloons", method =() => Fun.OrbitBalloons(), toolTip = "Orbits the balloons around you." },
                new ButtonInfo { buttonText = "Orbit Gliders", method =() => Fun.OrbitGliders(), toolTip = "Orbits the gliders around you." },
                new ButtonInfo { buttonText = "Orbit Hoverboards", method =() => Fun.OrbitHoverboards(), toolTip = "Orbits the hoverboards around you."},

                new ButtonInfo { buttonText = "Bug Aura", method =() => Fun.ObjectAura("Floating Bug Holdable"), toolTip = "Teleports the bug around you in random positions." },
                new ButtonInfo { buttonText = "Firefly Aura", method =() => Fun.ObjectAura("Firefly"), toolTip = "Teleports the firefly around you in random positions." },
                new ButtonInfo { buttonText = "Bat Aura", method =() => Fun.ObjectAura("Cave Bat Holdable"), toolTip = "Teleports the bat around you in random positions." },
                new ButtonInfo { buttonText = "Camera Aura", method =() => Fun.CameraAura(), toolTip = "Teleports the camera around you in random positions." },
                new ButtonInfo { buttonText = "Balloon Aura", method =() => Fun.BalloonAura(), toolTip = "Teleports the balloons around you in random positions." },
                new ButtonInfo { buttonText = "Glider Aura", method =() => Fun.GliderAura(), toolTip = "Teleports the camera around you in random positions." },
                new ButtonInfo { buttonText = "Hoverboard Aura", method =() => Fun.HoverboardAura(), toolTip = "Teleports the hoverboards around you in random positions."},

                new ButtonInfo { buttonText = "Ride Bug", method =() => Fun.RideObject("Floating Bug Holdable"), toolTip = "Repeatedly teleports you on top of the bug." },
                new ButtonInfo { buttonText = "Ride Firefly", method =() => Fun.RideObject("Firefly"), toolTip = "Repeatedly teleports you on top of the firefly." },
                new ButtonInfo { buttonText = "Ride Bat", method =() => Fun.RideObject("Cave Bat Holdable"), toolTip = "Repeatedly teleports you on top of the bat." },

                new ButtonInfo { buttonText = "Become Bug", method =() => Fun.BecomeObject("Floating Bug Holdable"), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into the bug." },
                new ButtonInfo { buttonText = "Become Firefly", method =() => Fun.BecomeObject("Firefly"), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into the firefly." },
                new ButtonInfo { buttonText = "Become Bat", method =() => Fun.BecomeObject("Cave Bat Holdable"), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into the bat." },
                new ButtonInfo { buttonText = "Become Camera", method =() => Fun.BecomeCamera(), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into the camera." },
                new ButtonInfo { buttonText = "Become Balloon", method =() => Fun.BecomeBalloon(), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into a balloon when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Become Hoverboard", method =() => Fun.BecomeHoverboard(), disableMethod =() => Movement.EnableRig(), toolTip = "Turns you into a hoverboard when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Destroy Bug", method =() => Fun.DestroyObject("Floating Bug Holdable"), toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Firefly", method =() => Fun.DestroyObject("Firefly"), toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Bat", method =() => Fun.DestroyObject("Cave Bat Holdable"), toolTip = "Sends the bat to hell." },
                new ButtonInfo { buttonText = "Destroy Camera", method =() => Fun.DestroyCamera(), toolTip = "Sends the camera to hell." },
                new ButtonInfo { buttonText = "Destroy Balloons", method =() => Fun.DestroyBalloons(), isTogglable = false, toolTip = "Sends every single balloon cosmetic to hell." },
                new ButtonInfo { buttonText = "Destroy Gliders", method =() => Fun.DestroyGliders(), isTogglable = false, toolTip = "Sends every single glider to hell." },

                new ButtonInfo { buttonText = "Respawn Gliders", method =() => Fun.RespawnGliders(), isTogglable = false, toolTip = "Respawns all the gliders." },
                new ButtonInfo { buttonText = "Pop All Balloons", method =() => Fun.PopAllBalloons(), isTogglable = false, toolTip = "Pops every single balloon cosmetic." },

                new ButtonInfo { buttonText = "Set Name to \"STATUE\"", method =() => ChangeName("STATUE"), isTogglable = false, toolTip = "Sets your name to \"STATUE\"." },
                new ButtonInfo { buttonText = "Set Name to \"RUN\"", method =() => ChangeName("RUN"), isTogglable = false, toolTip = "Sets your name to \"RUN\"." },
                new ButtonInfo { buttonText = "Set Name to \"BEHINDYOU\"", method =() => ChangeName("BEHINDYOU"), isTogglable = false, toolTip = "Sets your name to \"BEHINDYOU\"." },
                new ButtonInfo { buttonText = "Set Name to \"iiOnTop\"", method =() => ChangeName("iiOnTop"), isTogglable = false, toolTip = "Sets your name to \"iiOnTop\"." },

                new ButtonInfo { buttonText = "PBBV Name Cycle", method =() => Fun.NameCycle(new string[] { "PPBV", "IS", "HERE" }), toolTip = "Sets your name on a loop to \"PBBV\", \"IS\", and \"HERE\"." },
                new ButtonInfo { buttonText = "J3VU Name Cycle", method =() => Fun.NameCycle(new string[] { "J3VU", "HAS", "BECOME", "HOSTILE" }), toolTip = "Sets your name on a loop to \"J3VU\", \"HAS\", \"BECOME\", and \"HOSTILE\"" },
                new ButtonInfo { buttonText = "Run Rabbit Name Cycle", method =() => Fun.NameCycle(new string[] { "RUN", "RABBIT" }), toolTip = "Sets your name on a loop to \"RUN\" and \"RABBIT\"." },
                new ButtonInfo { buttonText = "Random Name Cycle", method =() => Fun.RandomNameCycle(), toolTip = "Sets your name on a loop to a bunch of random characters." },
                new ButtonInfo { buttonText = "Custom Name Cycle", enableMethod =() => Fun.EnableCustomNameCycle(), method =() => Fun.NameCycle(Fun.names), toolTip = "Sets your name on a loop to whatever's in the file." },

                new ButtonInfo { buttonText = "Strobe Color", method =() => Fun.StrobeColor(), toolTip = "Makes your character flash." },
                new ButtonInfo { buttonText = "Rainbow Color", method =() => Fun.RainbowColor(), toolTip = "Makes your character rainbow." },
                new ButtonInfo { buttonText = "Hard Rainbow Color", method =() => Fun.HardRainbowColor(), toolTip = "Makes your character flash from red, green, blue, and magenta." },

                new ButtonInfo { buttonText = "Become \"goldentrophy\"", method =() => Fun.BecomePlayer("goldentrophy", new Color32(255, 128, 0, 255)), isTogglable = false, toolTip = "Sets your name to \"goldentrophy\" and color to orange." },
                new ButtonInfo { buttonText = "Become \"PBBV\"", method =() => Fun.BecomePlayer("PBBV", new Color32(230, 127, 102, 255)), isTogglable = false, toolTip = "Sets your name to \"PBBV\" and color to sky blue." },
                new ButtonInfo { buttonText = "Become \"J3VU\"", method =() => Fun.BecomePlayer("J3VU", Color.green), isTogglable = false, toolTip = "Sets your name to \"J3VU\" and color to green." },
                new ButtonInfo { buttonText = "Become \"ECHO\"", method =() => Fun.BecomePlayer("ECHO", new Color32(0, 150, 255, 255)), isTogglable = false, toolTip = "Sets your name to \"ECHO\" and color to salmon." },
                new ButtonInfo { buttonText = "Become \"DAISY09\"", method =() => Fun.BecomePlayer("DAISY09", new Color32(255, 81, 231, 255)), isTogglable = false, toolTip = "Sets your name to \"DAISY09\" and color to a light pink." },
                new ButtonInfo { buttonText = "Become Child", method =() => Fun.BecomeMinigamesKid(), isTogglable = false, toolTip = "Sets your name and color to something a child would pick." },

                new ButtonInfo { buttonText = "Become Hidden on Leaderboard", method =() => Fun.BecomePlayer("I", new Color32(0, 53, 2, 255)), isTogglable = false, toolTip = "Sets your name to nothing and your color to a dark red, matching the leaderboard." },
                new ButtonInfo { buttonText = "Copy Identity Gun", method =() => Fun.CopyIdentityGun(), toolTip = "Steals the identity of whoever your hand desires." },

                new ButtonInfo { buttonText = "Change Accessories", overlapText = "Change Cosmetics", method =() => Fun.ChangeAccessories(), toolTip = "Use your grips to change what hat you're wearing." },
                new ButtonInfo { buttonText = "Spaz Accessories", overlapText = "Spaz Cosmetics <color=grey>[</color><color=green>All</color><color=grey>]</color>", method =() => Fun.SpazAccessories(), toolTip = "Spazzes your hats out for everyone when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Spaz Cosmetics <color=grey>[</color><color=green>Others</color><color=grey>]</color>", method =() => Fun.SpazAccessoriesOthers(), toolTip = "Spazzes your hats out for everyone except you when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Sticky Holdables", method =() => Fun.StickyHoldables(), toolTip = "Makes your holdables sticky." },
                new ButtonInfo { buttonText = "Spaz Holdables", method =() => Fun.SpazHoldables(), toolTip = "Spazzes out the positions of your holdables." },
                new ButtonInfo { buttonText = "Cosmetic Spoof", enableMethod =() => Fun.TryOnAnywhere(), disableMethod =() => Fun.TryOffAnywhere(), toolTip = "Lets you try on cosmetics from anywhere. Enable this mod after wearing the cosmetics." },
                new ButtonInfo { buttonText = "Cosmetic Browser", method =() => Fun.CosmeticBrowser(), isTogglable = false, toolTip = "Browse through every cosmetic that you can try on and add it to your cart." },
                new ButtonInfo { buttonText = "Auto Spoof Cosmetics", enableMethod =() => Fun.AutoLoadCosmetics(), disableMethod =() => Fun.NoAutoLoadCosmetics(), toolTip = "Automatically spoofs your cosmetics, making you appear with anything you're able to try-on." },
                new ButtonInfo { buttonText = "Auto Purchase Cosmetics", method =() => Fun.AutoPurchaseCosmetics(), toolTip = "Automatically purchases any free cosmetics." },
                new ButtonInfo { buttonText = "Disable Cosmetics on Tag", method =() => Fun.DisableCosmeticsOnTag(), toolTip = "Disables your cosmetics when you get tagged, good for ambush." },

                new ButtonInfo { buttonText = "Get ID Self", method =() => Fun.CopySelfID(), isTogglable = false, toolTip = "Gets your player ID and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get ID Gun", method =() => Fun.CopyIDGun(), toolTip = "Gets the player ID of whoever your hand desires and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Narrate ID Self", method =() => Fun.NarrateSelfID(), isTogglable = false, toolTip = "Gets your player ID and speaks it through your microphone."},
                new ButtonInfo { buttonText = "Narrate ID Gun", method =() => Fun.NarrateIDGun(), toolTip = "Gets the player ID of whoever your hand desires and speaks it through your microphone." },

                new ButtonInfo { buttonText = "Get Creation Date Self", method =() => Fun.CopyCreationDateSelf(), isTogglable = false, toolTip = "Gets the creation date of your account and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get Creation Date Gun", method =() => Fun.CopyCreationDateGun(), toolTip = "Gets the creation date of whoever your hand desires' account and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Narrate Creation Date Gun", method =() => Fun.NarrateCreationDateGun(), toolTip = "Gets the creation date of whoever your hand desires' account and speaks it through your microphone." },
                new ButtonInfo { buttonText = "Narrate Creation Date Self", method =() => Fun.NarrateCreationDateSelf(), isTogglable = false, toolTip = "Gets the creation date of your account and speaks it through your microphone." },

                new ButtonInfo { buttonText = "Grab Player Info", method =() => Fun.GrabPlayerInfo(), isTogglable = false, toolTip = "Saves every player's name, color, and player ID as a text file and opens it." },
            },

            new ButtonInfo[] { // Spam Mods [13]
                new ButtonInfo { buttonText = "Exit Spam Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Sound Mods", method =() => currentCategoryName = "Sound Spam Mods", isTogglable = false, toolTip = "Opens the sound mods."},
                new ButtonInfo { buttonText = "Projectile Mods", method =() => currentCategoryName = "Projectile Spam Mods", isTogglable = false, toolTip = "Opens the projectile mods."},
            },

            new ButtonInfo[] { // Sound Spam Mods [14]
                new ButtonInfo { buttonText = "Exit Sound Mods", method =() => currentCategoryName = "Spam Mods", isTogglable = false, toolTip = "Returns you back to the spam page."},

                new ButtonInfo { buttonText = "Bass Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(68), toolTip = "Plays the loud drum sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Metal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(18), toolTip = "Plays the metal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Wolf Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(195), toolTip = "Plays the wolf howl when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Cat Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(236), toolTip = "Plays the cat meow when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Turkey Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(83), toolTip = "Plays the turkey sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Frog Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(91), toolTip = "Plays the frog creak when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Bee Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(191), toolTip = "Plays the bee buzz when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Squeak Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(215), toolTip = "Plays the squeak sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Random Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RandomSoundSpam(), toolTip = "Plays random sounds when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Earrape Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(215), toolTip = "Plays a high-pitched sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Ding Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(244), toolTip = "Plays a ding sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CrystalSoundSpam(), toolTip = "Plays some crystal noises when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Piano Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(UnityEngine.Random.Range(295, 307)), toolTip = "Plays some terrible piano when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Big Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(213), toolTip = "Plays a long crystal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Pan Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(248), toolTip = "Plays a pan sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "AK-47 Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(203), toolTip = "Plays a sound that sounds like an AK-47 when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Siren Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SirenSoundSpam(), toolTip = "Plays a siren sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tick Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SoundSpam(148), toolTip = "Plays a tick sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Custom Sound ID", method =() => Sound.IncreaseSoundID(), enableMethod =() => Sound.IncreaseSoundID(), disableMethod =() => Sound.DecreaseSoundID(), incremental = true, isTogglable = false, toolTip = "Changes the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "Custom Sound Spam", overlapText = "Custom Sound Spam <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Sound.CustomSoundSpam(), toolTip = "Plays the selected sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Override Tap Sound", method =() => { Patches.EffectDataPatch.enabled = true; Patches.EffectDataPatch.material = soundId; }, disableMethod =() => {  Patches.EffectDataPatch.enabled = false;  Patches.EffectDataPatch.material = -1; }, toolTip = "Plays the selected sound when holding <color=green>grip</color>." },
            },

            new ButtonInfo[] { // Projectile Spam Mods [15]
                new ButtonInfo { buttonText = "Exit Projectile Mods", method =() => currentCategoryName = "Spam Mods", isTogglable = false, toolTip = "Returns you back to the spam page."},

                new ButtonInfo { buttonText = "Grab Projectile <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.GrabProjectile(), toolTip = "Grabs your selected projectile(s) holding <color=green>grip</color>. You can change the projectile(s) in Settings > Projectile Settings" },
                new ButtonInfo { buttonText = "Projectile Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.ProjectileSpam(), toolTip = "Spams your selected projectile(s) when holding <color=green>grip</color>. You can change the projectile(s) in Settings > Projectile Settings" },
                new ButtonInfo { buttonText = "Give Projectile Spam Gun", method =() => Projectiles.GiveProjectileSpamGun(), toolTip = "Acts like the projectile spam, but you can give it to whoever your hand desires. They need to hold grip." },
                new ButtonInfo { buttonText = "Impact Spam", method =() => Projectiles.ImpactSpam(), toolTip = "Acts like the projectile spam, but uses the impacts instead." },

                new ButtonInfo { buttonText = "Aimbot <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.Aimbot(), toolTip = "Sends all projectiles you fire into a random player's head." },

                new ButtonInfo { buttonText = "Urine <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Urine(), toolTip = "Makes you pee when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Feces <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Feces(), toolTip = "Makes you poo when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Semen <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Semen(), toolTip = "Makes you ejaculate when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Vomit <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Vomit(), toolTip = "Makes you throw up when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Spit <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Spit(), toolTip = "Makes you spit when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Urine Gun", method =() => Projectiles.UrineGun(), toolTip = "Makes whoever your hand desires pee." },
                new ButtonInfo { buttonText = "Feces Gun", method =() => Projectiles.FecesGun(), toolTip = "Makes whoever your hand desires poo." },
                new ButtonInfo { buttonText = "Semen Gun", method =() => Projectiles.SemenGun(), toolTip = "Makes whoever your hand desires ejaculate." },
                new ButtonInfo { buttonText = "Vomit Gun", method =() => Projectiles.VomitGun(), toolTip = "Makes whoever your hand desires throw up." },
                new ButtonInfo { buttonText = "Spit Gun", method =() => Projectiles.SpitGun(), toolTip = "Makes whoever your hand desires spit." },

                new ButtonInfo { buttonText = "Projectile Blind Gun", method =() => Projectiles.ProjectileBlindGun(), toolTip = "Blinds whoever your hand desires using the egg projectiles."},
                new ButtonInfo { buttonText = "Projectile Blind All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Projectiles.ProjectileBlindAll(), toolTip = "Blinds everybody in the room using the egg projectiles."},

                new ButtonInfo { buttonText = "Projectile Lag Gun", method =() => Projectiles.ProjectileLagGun(), toolTip = "Lags whoever your hand desires using the firework projectiles."},
                new ButtonInfo { buttonText = "Projectile Lag All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Projectiles.ProjectileLagAll(), toolTip = "Blinds everybody in the room using the firework projectiles."}
            },

            new ButtonInfo[] { // Master Mods [16]
                new ButtonInfo { buttonText = "Exit Master Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "MasterLabel", overlapText = "You are not master client.", label = true},

                new ButtonInfo { buttonText = "Guardian Self", method =() => Overpowered.GuardianSelf(), isTogglable = false, toolTip = "Makes you the guardian."},
                new ButtonInfo { buttonText = "Guardian Gun", method =() => Overpowered.GuardianGun(), toolTip = "Makes whoever your hand desires the guardian."},
                new ButtonInfo { buttonText = "Guardian All", method =() => Overpowered.GuardianAll(), isTogglable = false, toolTip = "Makes everyone in the room the guardian."},

                new ButtonInfo { buttonText = "Unguardian Self", method =() => Overpowered.UnguardianSelf(), isTogglable = false, toolTip = "Removes you from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian Gun", method =() => Overpowered.UnguardianGun(), toolTip = "Removes whoever your hand desires from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian All", method =() => Overpowered.UnguardianAll(), isTogglable = false, toolTip = "Removes everyone in the room from the guardian position."},

                new ButtonInfo { buttonText = "Guardian Spaz", method =() => Overpowered.GuardianSpaz(), toolTip = "Spams the guardian position for everyone in the room."},

                new ButtonInfo { buttonText = "Spaz Prop Hunt", method =() => Overpowered.SpazPropHunt(), toolTip = "Repeatedly starts and ends the prop hunt gamemode."},
                new ButtonInfo { buttonText = "Spaz Prop Hunt Objects", method =() => Overpowered.SpazPropHuntObjects(), toolTip = "Repeatedly randomizes everyone's selected object in the prop hunt gamemode."},

                new ButtonInfo { buttonText = "Max Currency Self", method =() => Fun.SetCurrencySelf(int.MaxValue), isTogglable = false, toolTip = "Gives you the maximum amount of currency in the horror map (2 billion)."},
                new ButtonInfo { buttonText = "Max Currency Gun", method =() => Fun.SetCurrencyGun(int.MaxValue), toolTip = "Gives whoever your hand desires the maximum amount of currency in the horror map (2 billion)."},
                new ButtonInfo { buttonText = "Max Currency All", method =() => Fun.SetCurrencyAll(int.MaxValue), isTogglable = false, toolTip = "Gives everyone in the room the maximum amount of currency in the horror map (2 billion)."},

                new ButtonInfo { buttonText = "Remove Currency Self", method =() => Fun.SetCurrencySelf(), isTogglable = false, toolTip = "Removes all currency in the horror map from yourself."},
                new ButtonInfo { buttonText = "Remove Currency Gun", method =() => Fun.SetCurrencyGun(), toolTip = "Removes all currency in the horror map from whoever your hand desires."},
                new ButtonInfo { buttonText = "Remove Currency All", method =() => Fun.SetCurrencyAll(), isTogglable = false, toolTip = "Removes all currency in the horror map from everyone in the room."},

                new ButtonInfo { buttonText = "Invincibility", method =() => Fun.Invincibility(), toolTip = "Makes you unable to die in the horror map."},

                new ButtonInfo { buttonText = "Kill Self", method =() => Fun.SetStateSelf(1), isTogglable = false, toolTip = "Turns you into a ghost."},
                new ButtonInfo { buttonText = "Kill Gun", method =() => Fun.SetStateGun(1), toolTip = "Turns whoever your hand desires into a ghost."},
                new ButtonInfo { buttonText = "Kill All", method =() => Fun.SetStateAll(1), isTogglable = false, toolTip = "Turns everyone in the room into a ghost."},

                new ButtonInfo { buttonText = "Revive Self", method =() => Fun.SetStateSelf(0), isTogglable = false, toolTip = "Revives you from death."},
                new ButtonInfo { buttonText = "Revive Gun", method =() => Fun.SetStateGun(0), toolTip = "Revives whoever your hand desires from death."},
                new ButtonInfo { buttonText = "Revive All", method =() => Fun.SetStateAll(0), isTogglable = false, toolTip = "Revives everyone in the room from death."},

                new ButtonInfo { buttonText = "Spaz Kill Self", method =() => Fun.SpazKillSelf(), toolTip = "Repeatedly kills and revives you."},
                new ButtonInfo { buttonText = "Spaz Kill Gun", method =() => Fun.SpazKillGun(), toolTip = "Repeatedly kills and revives whoever your hand desires."},
                new ButtonInfo { buttonText = "Spaz Kill All", method =() => Fun.SpazKillAll(), toolTip = "Repeatedly kills and revives everyone in the room."},

                new ButtonInfo { buttonText = "Gate Spam Gun", method =() => Overpowered.SpamObjectGun(-735557236), toolTip = "Spawns gates at wherever your hand desires."},
                new ButtonInfo { buttonText = "Core Spam Gun", method =() => Overpowered.SpamObjectGun(166197108), toolTip = "Spawns collectible cores at wherever your hand desires."},
                new ButtonInfo { buttonText = "Tool Spam Gun", method =() => Overpowered.ToolSpamGun(), toolTip = "Spawns random tools at wherever your hand desires."},
                new ButtonInfo { buttonText = "Flower Spam Gun", method =() => Overpowered.SpamObjectGun(-20887423), toolTip = "Spawns flowers at wherever your hand desires."},
                new ButtonInfo { buttonText = "Barrel Spam Gun", method =() => Overpowered.SpamObjectGun(-1724683316), toolTip = "Spawns barrels at wherever your hand desires."},
                new ButtonInfo { buttonText = "Ranged Enemy Spam Gun", method =() => Overpowered.SpamObjectGun(-144412770), toolTip = "Spawns ranged enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Chaser Enemy Spam Gun", method =() => Overpowered.SpamObjectGun(48354877), toolTip = "Spawns chasing enemies at wherever your hand desires."},
                new ButtonInfo { buttonText = "Destroy Entity Gun", method =() => Overpowered.DestroyEntityGun(), toolTip = "Destroys any entity which your hand desires."},

                new ButtonInfo { buttonText = "Unlimited Building", enableMethod =() => Fun.UnlimitedBuilding(), disableMethod =() => Fun.DisableUnlimitedBuilding(), toolTip = "Unlimits building, disabling drop zones and letting you place on people's plots." },

                new ButtonInfo { buttonText = "Shotgun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.Shotgun(), toolTip = "Spawns you a shotgun when you press <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Block Crash Gun", method =() => Overpowered.BlockCrashGun(), toolTip = "Crashes whoever your hand desires if they are inside of the block map."},
                new ButtonInfo { buttonText = "Block Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BlockCrashAll(), toolTip = "Crashes everybody inside of the block map."},

                new ButtonInfo { buttonText = "Block Anti Report", enableMethod =() => Fun.EnableAtticAntiReport(), method =() => Fun.AtticAntiReport(), toolTip = "Automatically builds blocks around your report button."},

                new ButtonInfo { buttonText = "Block Draw Gun", method =() => Fun.AtticDrawGun(), toolTip = "Draw wherever your hand desires."},
                new ButtonInfo { buttonText = "Block Build Gun", method =() => Fun.AtticBuildGun(), toolTip = "Draw wherever your hand desires with no delay."},
                new ButtonInfo { buttonText = "Block Tower Gun", method =() => Fun.AtticTowerGun(), toolTip = "Builds a tower wherever your hand desires."},

                new ButtonInfo { buttonText = "Block Freeze Gun", method =() => Fun.AtticFreezeGun(), toolTip = "Freeze whoever your hand desires."},
                new ButtonInfo { buttonText = "Block Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.AtticFreezeAll(), toolTip = "Freezes everyone in the lobby when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Block Float Gun", method =() => Fun.AtticFloatGun(), toolTip = "Makes whoever your hand desires float."},

                new ButtonInfo { buttonText = "Spaz Targets", method =() => Overpowered.TargetSpam(), toolTip = "Gives the targets a seizure."},

                new ButtonInfo { buttonText = "Slow Monsters", enableMethod =() => Fun.SlowMonsters(), disableMethod =() => Fun.FixMonsters(), toolTip = "Slows down the basement monsters." },
                new ButtonInfo { buttonText = "Fast Monsters", enableMethod =() => Fun.FastMonsters(), disableMethod =() => Fun.FixMonsters(), toolTip = "Speeds up the basement monsters." },

                new ButtonInfo { buttonText = "Grab Monsters <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabMonsters(), toolTip = "Puts the basement monsters in your hand." },
                new ButtonInfo { buttonText = "Monster Gun", method =() => Fun.MonsterGun(), toolTip = "Moves the basement monsters to wherever your hand desires." },
                new ButtonInfo { buttonText = "Spaz Monsters", method =() => Fun.SpazMonsters(), toolTip = "Gives the basement monsters a seizure." },
                new ButtonInfo { buttonText = "Orbit Monsters", method =() => Fun.OrbitMonsters(), toolTip = "Orbits the basement monsters around you." },
                new ButtonInfo { buttonText = "Destroy Monsters", method =() => Fun.DestroyMonsters(), isTogglable = false, toolTip = "Sends the basement monsters to hell." },

                new ButtonInfo { buttonText = "Infection to Tag", method =() => Overpowered.InfectionToTag(), isTogglable = false, toolTip = "Turns the game into tag instead of infection." },
                new ButtonInfo { buttonText = "Tag to Infection", method =() => Overpowered.TagToInfection(), isTogglable = false, toolTip = "Turns the game into infection instead of tag." },

                new ButtonInfo { buttonText = "Untag Gun", method =() => Advantages.UntagGun(), toolTip = "Untags whoever your hand desires."},
                new ButtonInfo { buttonText = "Untag All", method =() => Advantages.UntagAll(), isTogglable = false, toolTip = "Removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Spam Tag Self", method =() => Advantages.SpamTagSelf(), toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag Gun", method =() => Advantages.SpamTagGun(), toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag All", method =() => Advantages.SpamTagAll(), toolTip = "Adds and removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Tag Lag", enableMethod =() => Advantages.SetTagCooldown(float.MaxValue), disableMethod =() => Advantages.SetTagCooldown(5f), toolTip = "Forces tag lag in the lobby, letting no one get tagged."},

                new ButtonInfo { buttonText = "Bonk Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(4), toolTip = "Plays the bonk sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(1), toolTip = "Plays the count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(6), toolTip = "Plays the brawl count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Start Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(7), toolTip = "Plays the brawl start sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tag Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(0), toolTip = "Plays the tag sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Round End Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BetaSoundSpam(2), toolTip = "Plays the round end sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Battle Start Game", method =() => Advantages.BattleStartGame(), isTogglable = false, toolTip = "Starts a game of battle." },
                new ButtonInfo { buttonText = "Battle End Game", method =() => Advantages.BattleEndGame(), isTogglable = false, toolTip = "Ends the current game of battle." },
                new ButtonInfo { buttonText = "Battle Restart Game", method =() => Advantages.BattleRestartGame(), isTogglable = false, toolTip = "Restarts the current game of battle." },
                new ButtonInfo { buttonText = "Battle Restart Spam", method =() => Advantages.BattleRestartGame(), toolTip = "Spam starts and ends games of battle." },

                new ButtonInfo { buttonText = "Battle Balloon Spam Self", method =() => Advantages.BattleBalloonSpamSelf(), toolTip = "Spam pops and unpops your balloons in battle." },
                new ButtonInfo { buttonText = "Battle Balloon Spam All", method =() => Advantages.BattleBalloonSpam(), toolTip = "Spam pops and unpops everyone's balloons in battle." },

                new ButtonInfo { buttonText = "Battle Kill Gun", method =() => Advantages.BattleKillGun(), toolTip = "Kills whoever your hand desires in battle." },
                new ButtonInfo { buttonText = "Battle Kill Self", method =() => Advantages.BattleKillSelf(), isTogglable = false, toolTip = "Kills yourself in battle." },
                new ButtonInfo { buttonText = "Battle Kill All", method =() => Advantages.BattleKillAll(), isTogglable = false, toolTip = "Kills everyone in the room in battle." },

                new ButtonInfo { buttonText = "Battle Revive Gun", method =() => Advantages.BattleReviveGun(), toolTip = "Revives whoever your hand desires in battle." },
                new ButtonInfo { buttonText = "Battle Revive Self", method =() => Advantages.BattleReviveSelf(), isTogglable = false, toolTip = "Revives yourself in battle." },
                new ButtonInfo { buttonText = "Battle Revive All", method =() => Advantages.BattleReviveAll(), isTogglable = false, toolTip = "Revives everyone in battle." },

                new ButtonInfo { buttonText = "Battle God Mode", method =() => Advantages.BattleGodMode(), toolTip = "Gives you god mode in battle." },

                new ButtonInfo { buttonText = "Slow Gun", method =() => Overpowered.SlowGun(), toolTip = "Forces tag freeze on whoever your hand desires." },
                new ButtonInfo { buttonText = "Slow All", method =() => Overpowered.SlowAll(), toolTip = "Forces tag freeze on everyone in the the room." },

                new ButtonInfo { buttonText = "Vibrate Gun", method =() => Overpowered.VibrateGun(), toolTip = "Makes whoever your hand desires' controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate All", method =() => Overpowered.VibrateAll(), toolTip = "Makes everyone in the the room's controllers vibrate." },
            },

            new ButtonInfo[] { // Overpowered Mods [17]
                new ButtonInfo { buttonText = "Exit Overpowered Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Always Guardian", method =() => Overpowered.AlwaysGuardian(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes you always the guardian."},
                new ButtonInfo { buttonText = "Guardian Protector", method =() => Overpowered.GuardianProtector(), toolTip = "Pushes people away from the guardian moon if they try to approach it."},

                new ButtonInfo { buttonText = "Grab Gun", overlapText = "Guardian Grab Gun", method =() => Overpowered.GrabGun(), toolTip = "Grabs whoever your hand desires if you're the guardian."},
                new ButtonInfo { buttonText = "Grab All <color=grey>[</color><color=green>G</color><color=grey>]</color>", overlapText = "Guardian Grab All <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.GrabAll(), toolTip = "Grabs everyone in the room if you're the guardian."},

                new ButtonInfo { buttonText = "Release Gun", overlapText = "Guardian Release Gun", method =() => Overpowered.ReleaseGun(), toolTip = "Releases whoever your hand desires if you're the guardian."},
                new ButtonInfo { buttonText = "Release All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Release All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.ReleaseAll(), toolTip = "Releases everyone in the room if you're the guardian."},

                new ButtonInfo { buttonText = "Fling Gun", overlapText = "Guardian Fling Gun", method =() => Overpowered.FlingGun(), toolTip = "Flings whoever your hand desires."},
                new ButtonInfo { buttonText = "Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FlingAll(), toolTip = "Flings everyone in the room."},

                new ButtonInfo { buttonText = "Bring Gun", overlapText = "Guardian Bring Gun", method =() => Overpowered.BringGun(), toolTip = "Brings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Bring All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Bring All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BringAll(), toolTip = "Brings everyone in the room towards you."},

                new ButtonInfo { buttonText = "Bring All Gun", overlapText = "Guardian Bring All Gun", method =() => Overpowered.BringAllGun(), toolTip = "Brings everyone in the room towards wherever your hand desires."},

                new ButtonInfo { buttonText = "Guardian Bring Away Gun", method =() => Overpowered.BringAwayGun(), toolTip = "Brings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Guardian Bring Away All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BringAwayAll(), toolTip = "Brings everyone in the room towards you."},

                new ButtonInfo { buttonText = "Bring Away All Gun", method =() => Overpowered.BringAwayAllGun(), toolTip = "Brings everyone in the room towards wherever your hand desires."},

                new ButtonInfo { buttonText = "Guardian Anti Stump", method =() => Overpowered.AntiStump(), toolTip = "Anyone who gets too close to the stump entrance will be launched away."},

                new ButtonInfo { buttonText = "Guardian Orbit All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.OrbitAll(), toolTip = "Orbits everyone in the room around you."},

                new ButtonInfo { buttonText = "Guardian Punch Mod", method =() => Overpowered.PunchMod(), toolTip = "Flings people when you punch them."},
                new ButtonInfo { buttonText = "Guardian Boxing", method =() => Overpowered.Boxing(), toolTip = "Lets everyone in the room punch eachother."},

                new ButtonInfo { buttonText = "Guardian Give Fly Gun", method =() => Overpowered.GiveFlyGun(), toolTip = "Gives whoever you want fly when they hold their right thumb down."},
                new ButtonInfo { buttonText = "Guardian Give Fly All", method =() => Overpowered.GiveFlyAll(), toolTip = "Gives everyone in the room fly when they hold their right thumb down."},

                new ButtonInfo { buttonText = "Spaz Player Gun", overlapText = "Guardian Spaz Player Gun", method =() => Overpowered.SpazPlayerGun(), toolTip = "Spazzes out whoever your hand desires."},
                new ButtonInfo { buttonText = "Spaz All Players <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Spaz All Players <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SpazAllPlayers(), toolTip = "Spazzes out everyone in the room."},

                new ButtonInfo { buttonText = "Effect Spam Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", overlapText = "Guardian Effect Spam Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpamHands(), toolTip = "Spawns effects when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Effect Spam Gun", method =() => Overpowered.EffectSpamGun(), overlapText = "Guardian Effect Spam Gun", toolTip = "Spawns effects wherever your hand desires."},

                new ButtonInfo { buttonText = "Physical Freeze Gun", overlapText = "Guardian Freeze Gun", method =() => Overpowered.PhysicalFreezeGun(), toolTip = "Freezes whoever your hand desires." },
                new ButtonInfo { buttonText = "Physical Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", overlapText = "Guardian Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.PhysicalFreezeAll(), toolTip = "Freezes everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Guardian Kick Gun", method =() => Overpowered.GuardianKickGun(), toolTip = "Kicks whoever your hand desires." },
                new ButtonInfo { buttonText = "Guardian Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.GuardianKickAll(), toolTip = "Kicks everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Guardian Crash Gun", method =() => Overpowered.GuardianCrashGun(), toolTip = "Crashes whoever your hand desires." },
                new ButtonInfo { buttonText = "Guardian Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.GuardianCrashAll(), toolTip = "Crashes everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Delay Ban Gun", method =() => Overpowered.DelayBanGun(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Delay bans whoever your hand desires."},
                new ButtonInfo { buttonText = "Delay Ban All", enableMethod =() => Overpowered.DelayBanAll(), disableMethod =() => Patches.SerializePatch.OverrideSerialization = null, toolTip = "Delay bans everyone in the room."},

                new ButtonInfo { buttonText = "Kick on Grab", method =() => Overpowered.TowardsPositionOnGrab(new Vector3(-71.33718f, 101.4977f, -93.09029f)), toolTip = "Kicks the player when they grab you." },
                new ButtonInfo { buttonText = "Crash on Grab", method =() => Overpowered.DirectionOnGrab(Vector3.down), toolTip = "Crashes the player when they grab you." },
                new ButtonInfo { buttonText = "Fling on Grab", method =() => Overpowered.FlingOnGrab(), toolTip = "Flings the player when they grab you." },
                new ButtonInfo { buttonText = "Obliterate on Grab", method =() => Overpowered.DirectionOnGrab(Vector3.up), toolTip = "Obliterates the player when they grab you." },
                new ButtonInfo { buttonText = "Towards Point on Grab Gun", method =() => Overpowered.TowardsPointOnGrab(), disableMethod =() => Overpowered.DisableTowardsPointOnGrab(), toolTip = "Sends the player to your target position when they grab you." },

                new ButtonInfo { buttonText = "Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FreezeAll(), toolTip = "Freezes everyone in the room when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Snowball Airstrike Gun", method =() => Overpowered.SnowballAirstrikeGun(), toolTip = "Spawns a snowball airstrike wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Gun", method =() => Overpowered.SnowballGun(), toolTip = "Spawns a snowball wherever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Rain <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballRain(), toolTip = "Rains snowballs around you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Hail <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballHail(), toolTip = "Hails snowballs around you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Orbit <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballOrbit(), toolTip = "Orbits snowballs around you when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Snowball Aura <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballAura(), toolTip = "Randomly spawns snowballs around you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Minigun", method =() => Overpowered.SnowballMinigun(), toolTip = "Spawns snowballs towards wherever your hand desires."},
                new ButtonInfo { buttonText = "Give Snowball Minigun", method =() => Overpowered.GiveSnowballMinigun(), toolTip = "Gives whoever your hand desires a snowball minigun." },

                new ButtonInfo { buttonText = "Snowball Particle Gun", method =() => Overpowered.SnowballParticleGun(), toolTip = "Spawns snowball particles wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Impact Effect Gun", method =() => Overpowered.SnowballImpactEffectGun(), toolTip = "Spawns snowball impact events on whoever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Punch Mod", method =() => Overpowered.SnowballPunchMod(), toolTip = "Flings people when you punch them."},
                new ButtonInfo { buttonText = "Snowball Safety Bubble", method =() => Overpowered.SnowballSafetyBubble(), toolTip = "Anyone who gets too close to you will be launched away."},

                new ButtonInfo { buttonText = "Snowball Fling Gun", method =() => Overpowered.SnowballFlingGun(), toolTip = "Flings whoever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballFlingAll(), toolTip = "Flings everybody when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Fling Vertical Gun", method =() => Overpowered.SnowballFlingVerticalGun(), toolTip = "Flings whoever your hand desires vertically."},
                new ButtonInfo { buttonText = "Snowball Fling Vertical All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SnowballFlingVerticalAll(), toolTip = "Flings everybody vertically when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Snowball Fling Towards Gun", method =() => Overpowered.SnowballFlingTowardsGun(), toolTip = "Flings everybody towards wherever your hand desires."},
                new ButtonInfo { buttonText = "Snowball Fling Away Gun", method =() => Overpowered.SnowballFlingAwayGun(), toolTip = "Flings everybody away from wherever your hand desires."},

                new ButtonInfo { buttonText = "Snowball Fling Player Towards Gun", method =() => Overpowered.SnowballFlingPlayerTowardsGun(), toolTip = "Flings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Snowball Fling Player Away Gun", method =() => Overpowered.SnowballFlingPlayerAwayGun(), toolTip = "Flings whoever your hand desires away from you."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Fling</color><color=grey>]</color>", method =() => Overpowered.AntiReportFling(), toolTip = "Flings whoever tries to report you."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Snowball Fling</color><color=grey>]</color>", method =() => Overpowered.AntiReportSnowballFling(), toolTip = "Flings whoever tries to report you with the snowballs."},

                new ButtonInfo { buttonText = "Lag Gun", method =() => Overpowered.LagGun(), toolTip = "Lags whoever your hand desires."},
                new ButtonInfo { buttonText = "Lag All", method =() => Overpowered.LagAll(), toolTip = "Lags everyone in the room."},
                new ButtonInfo { buttonText = "Lag Aura", method =() => Overpowered.LagAura(), toolTip = "Lags players nearby you."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Lag</color><color=grey>]</color>", method =() => Overpowered.AntiReportLag(), toolTip = "Lags whoever tries to report you."},

                new ButtonInfo { buttonText = "Lock Room", method =() => Overpowered.SetRoomLock(true), isTogglable = false, toolTip = "Locks the room so no one else can join."},
                new ButtonInfo { buttonText = "Unlock Room", method =() => Overpowered.SetRoomLock(false), isTogglable = false, toolTip = "Unlocks the room so anyone can join."},

                new ButtonInfo { buttonText = "Destroy Gun", method =() => Overpowered.DestroyGun(), toolTip = "Block new players from seeing whoever your hand desires."},
                new ButtonInfo { buttonText = "Destroy All", method =() => Overpowered.DestroyAll(), isTogglable = false, toolTip = "Block new players from seeing everyone."},

                new ButtonInfo { buttonText = "Joystick Rope Control <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Overpowered.JoystickRopeControl(), toolTip = "Control the ropes in the direction of your joystick."},

                new ButtonInfo { buttonText = "Broken Ropes", method =() => Overpowered.SpazGrabbedRopes(), toolTip = "Gives any ropes currently being held onto a seizure."},
                new ButtonInfo { buttonText = "Spaz Rope Gun", method =() => Overpowered.SpazRopeGun(), toolTip = "Gives whatever rope your hand desires a seizure."},
                new ButtonInfo { buttonText = "Spaz All Ropes <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SpazAllRopes(), toolTip = "Gives every rope a seizure when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Fling Rope Gun", method =() => Overpowered.FlingRopeGun(), toolTip = "Flings whatever rope your hand desires away from you."},
                new ButtonInfo { buttonText = "Fling All Ropes Gun", method =() => Overpowered.FlingAllRopesGun(), toolTip = "Flings every rope in whatever direction your hand desires."},

                new ButtonInfo { buttonText = "Stump Kick Gun", method =() => Fun.StumpKickGun(), toolTip = "Kicks whoever your hand desires if they are in stump." },
                new ButtonInfo { buttonText = "Stump Kick All", method =() => Fun.StumpKickAll(), isTogglable = false, toolTip = "Kicks everyone in stump." },

                new ButtonInfo { buttonText = "Instant Party", method =() => Fun.InstantParty(), toolTip = "Makes parties form instantly, instead of having to wait a couple of seconds." },
                new ButtonInfo { buttonText = "Leave Party", method =() => FriendshipGroupDetection.Instance.LeaveParty(), isTogglable = false, toolTip = "Leaves the party, incase you can't pull off the string." },

                new ButtonInfo { buttonText = "Kick All in Party", method =() => Fun.KickAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Ban All in Party", method =() => Fun.BanAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Auto Party Kick", method =() => Fun.AutoPartyKick(), toolTip = "When you party, you will automatically send everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Auto Party Ban", method =() => Fun.AutoPartyBan(), toolTip = "When you party, you will automatically send everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Break Audio Gun", method =() => Overpowered.BreakAudioGun(), toolTip = "Attempts to break the audio of whoever your hand desires." },
                new ButtonInfo { buttonText = "Break Audio All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BreakAudioAll(), toolTip = "Attempts to breaks everyone's audio when holding trigger." },
            },

            new ButtonInfo[] { // Soundboard [18]
                new ButtonInfo { buttonText = "Exit Soundboard", method = () => currentCategoryName = "Fun Mods", isTogglable = false, toolTip = "Returns you back to the fun mods." }
            },

            new ButtonInfo[] { // Favorite Mods [19]
                new ButtonInfo { buttonText = "Exit Favorite Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Menu Presets [20]
                new ButtonInfo { buttonText = "Exit Menu Presets", method =() => currentCategoryName = "Menu Settings", isTogglable = false, toolTip = "Returns to the settings for the menu."},

                new ButtonInfo { buttonText = "Legitimate Preset", method =() => Presets.LegitimatePreset(), isTogglable = false, toolTip = "Enables a bunch of mods that make it impossible to mod check you."},
                new ButtonInfo { buttonText = "Goldentrophy Preset", method =() => Presets.GoldentrophyPreset(), isTogglable = false, toolTip = "Enables the mods that \"goldentrophy\" uses."},
                new ButtonInfo { buttonText = "Performance Preset", method =() => Presets.PerformancePreset(), isTogglable = false, toolTip = "Enables some mods that attempt to maximize your FPS as much as possible."},
                new ButtonInfo { buttonText = "Safety Preset", method =() => Presets.SafetyPreset(), isTogglable = false, toolTip = "Enables some mods that attempt to keep you as safe as possible."},
                new ButtonInfo { buttonText = "Ghost Preset", method =() => Presets.GhostPreset(), isTogglable = false, toolTip = "Enables a bunch of mods that are commonly used for ghost trolling."},

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
                
                new ButtonInfo { buttonText = "Quick Start Mods", method =() => Presets.SimplePreset(), isTogglable = false, toolTip = "Enables some mods that attempt to improve your experience using the menu."},
            },

            new ButtonInfo[] { // Advantage Settings [21]
                new ButtonInfo { buttonText = "Exit Advantage Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Obnoxious Tag", toolTip = "Makes the tag mods more obnoxious. Instead of hiding in the ground, you teleport around the player like crazy."},
                new ButtonInfo { buttonText = "ctaRange", overlapText = "Change Tag Aura Range <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagAuraRange(), enableMethod =() => Advantages.ChangeTagAuraRange(), disableMethod =() => Advantages.ChangeTagAuraRange(false), incremental = true, isTogglable = false, toolTip = "Changes the range of the tag aura mods."},
                new ButtonInfo { buttonText = "ctrRange", overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagReachDistance(), enableMethod =() => Advantages.ChangeTagReachDistance(), disableMethod =() => Advantages.ChangeTagReachDistance(false), incremental = true, isTogglable = false, toolTip = "Changes the range of the tag reach mods."},

                new ButtonInfo { buttonText = "Visualize Tag Reach", toolTip = "Visualizes the distance threshold for the tag reach."},
            },

            new ButtonInfo[] { // Visual Settings [22]
                new ButtonInfo { buttonText = "Exit Visual Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Performance Visuals Step", overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>0.2</color><color=grey>]</color>", method =() => Visuals.ChangePerformanceModeVisualStep(), enableMethod =() => Visuals.ChangePerformanceModeVisualStep(), disableMethod =() => Visuals.ChangePerformanceModeVisualStep(false), incremental = true, isTogglable = false, toolTip = "Changes the time between rendering visual mods."},
                new ButtonInfo { buttonText = "Performance Visuals", enableMethod =() => Visuals.PerformanceVisuals = true, disableMethod =() => Visuals.PerformanceVisuals = false, toolTip = "Makes visual mods render less often, to increase performange and decrease memory usage."},
                new ButtonInfo { buttonText = "Short Breadcrumbs", toolTip = "Shortens the length of the breadcrumbs."},
                new ButtonInfo { buttonText = "Follow Menu Theme", toolTip = "Makes visual mods match the theme of the menu, rather than the color of the player."},
                new ButtonInfo { buttonText = "Follow Player Colors", toolTip = "Makes the infection tracers appear their normal color instead of orange for tagged players."},
                new ButtonInfo { buttonText = "Transparent Theme", toolTip = "Makes visual mods transparent."},
                new ButtonInfo { buttonText = "Hidden on Camera", overlapText = "Streamer Mode Visuals", toolTip = "Makes visual mods only render on VR."},
                new ButtonInfo { buttonText = "Hidden Labels", overlapText = "Streamer Mode Labels", toolTip = "Makes label mods only render on VR."},
                new ButtonInfo { buttonText = "Thin Tracers", toolTip = "Makes the tracers thinner."},
                new ButtonInfo { buttonText = "Smooth Lines", enableMethod =() => smoothLines = true, disableMethod =() => smoothLines = false, toolTip = "Makes every line generated by the menu have smooth ends."},
            },

            new ButtonInfo[] { // Admin Mods (admins only) [23]
                new ButtonInfo { buttonText = "Exit Admin Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Get Menu Users", method =() => Experimental.GetMenuUsers(), isTogglable = false, toolTip = "Detects who is using the menu."},
                new ButtonInfo { buttonText = "Menu User Name Tags", enableMethod =() => Experimental.EnableAdminMenuUserTags(), method =() => Experimental.AdminMenuUserTags(), disableMethod =() => Experimental.DisableAdminMenuUserTags(), toolTip = "Detects who is using the menu."},

                new ButtonInfo { buttonText = "Admin Kick Gun", method =() => Experimental.AdminKickGun(), toolTip = "Kicks whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Kick All", method =() => Experimental.AdminKickAll(), isTogglable = false, toolTip = "Kicks everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Flip Menu Gun", method =() => Experimental.FlipMenuGun(), toolTip = "Flips the menu of whoever your hand desires if they're using the menu."},

                new ButtonInfo { buttonText = "Admin Disable Menu Gun", method =() => Experimental.AdminLockdownGun(true), toolTip = "Disables the menu of whoever your hand desires if they're using one."},
                new ButtonInfo { buttonText = "Admin Enable Menu Gun", method =() => Experimental.AdminLockdownGun(false), toolTip = "Enables the menu of whoever your hand desires if they're using one."},

                new ButtonInfo { buttonText = "Admin Fully Disable Menu Gun", method =() => Experimental.AdminFullLockdownGun(true), toolTip = "Disables the menu of whoever your hand desires and turns off their mods if they're using one."},
                new ButtonInfo { buttonText = "Admin Fully Enable Menu Gun", method =() => Experimental.AdminFullLockdownGun(false), toolTip = "Enables the menu of whoever your hand desires and turns off their mods if they're using one."},

                new ButtonInfo { buttonText = "Admin Disable Menu All", method =() => Experimental.AdminLockdownAll(true), toolTip = "Disables the menu of whoever your hand desires if they're using one."},
                new ButtonInfo { buttonText = "Admin Enable Menu All", method =() => Experimental.AdminLockdownAll(false), toolTip = "Enables the menu of whoever your hand desires if they're using one."},
                
                new ButtonInfo { buttonText = "Admin Fully Disable Menu All", method =() => Experimental.AdminFullLockdownAll(true), isTogglable = false, toolTip = "Disables the menu of whoever your hand desires and turns off their mods if they're using one."},
                new ButtonInfo { buttonText = "Admin Fully Enable Menu All", method =() => Experimental.AdminFullLockdownAll(false), isTogglable = false, toolTip = "Enables the menu of whoever your hand desires and turns off their mods if they're using one."},

                new ButtonInfo { buttonText = "Admin Teleport Gun", method =() => Experimental.AdminTeleportGun(), toolTip = "Teleports whoever using the menu to wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Fling Gun", method =() => Experimental.AdminFlingGun(), toolTip = "Flings whoever your hand desires upwards."},
                new ButtonInfo { buttonText = "Admin Strangle", method =() => Experimental.AdminStrangle(), toolTip = "Strangles whoever you grab if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Fake Cosmetics", overlapText = "Admin Spoof Cosmetics", method =() => Experimental.AdminSpoofCosmetics(), enableMethod =() => { NetworkSystem.Instance.OnPlayerJoined += Experimental.OnPlayerJoinSpoof; Experimental.AdminSpoofCosmetics(true); }, disableMethod =() => { NetworkSystem.Instance.OnPlayerJoined -= Experimental.OnPlayerJoinSpoof; Experimental.oldCosmetics = null; }, toolTip = "Makes everyone using the menu see whatever cosmetics you have on as if you owned them."},

                new ButtonInfo { buttonText = "Admin Lightning Gun", method =() => Experimental.LightningGun(), toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Aura", method =() => Experimental.LightningAura(), toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Rain", method =() => Experimental.LightningRain(), toolTip = "Rains lightning around you and strikes whoever you hit."},

                new ButtonInfo { buttonText = "Admin Laser", method =() => Experimental.AdminLaser(), toolTip = "Shines a red laser out of your hand when holding <color=green>A</color> or <color=green>X</color>."},
                new ButtonInfo { buttonText = "Admin Beam <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.AdminBeam(), toolTip = "Shines a rainbow spinning laser out of your head when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Admin Fractals <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.AdminFractals(), toolTip = "Shines white lines out of your body when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Admin Fear Gun", method =() => Experimental.AdminFearGun(), toolTip = "Sends a person into pure fear and scarefulness."},
                new ButtonInfo { buttonText = "Admin Object Gun", method =() => Experimental.AdminObjectGun(), toolTip = "Spawns an object wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Notify Gun", method =() => Experimental.NotifyGun(), toolTip = "Sends a notification to whoever your hand desires. The notification text is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Notify All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.NotifyAll(), toolTip = "Sends a notification to everyone using the menu. The notification text is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Join Gun", method =() => Experimental.JoinGun(), toolTip = "Brings whoever your hand desires to a room. The room is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Join All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.JoinAll(), toolTip = "Brings everyone using the menu to a room. The room is based off of what you type into the search bar."},
                new ButtonInfo { buttonText = "Admin Network Scale", method =() => Experimental.AdminNetworkScale(), disableMethod =() => Experimental.UnAdminNetworkScale(), toolTip = "Networks your scale to others with the menu."},

                new ButtonInfo { buttonText = "Admin Confirm Notification", method =() => Experimental.ConfirmNotifyAllUsing(), isTogglable = false, toolTip = "Sends a notification to everyone using the menu confirming that you're an admin."},

                new ButtonInfo { buttonText = "Admin Levitate All", method =() => Experimental.FlyAllUsing(), toolTip = "Sends everyone using the menu flying away upwards."},
                new ButtonInfo { buttonText = "Admin Bring Gun", method =() => Experimental.AdminBringGun(), toolTip = "Brings whoever your hand desires to you if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Bring All", method =() => Experimental.BringAllUsing(), toolTip = "Brings everyone using the menu to you."},
                new ButtonInfo { buttonText = "Admin Bring Hand All", method =() => Experimental.BringHandAllUsing(), toolTip = "Brings everyone using the menu to your hand."},
                new ButtonInfo { buttonText = "Admin Bring Head All", method =() => Experimental.BringHeadAllUsing(), toolTip = "Brings everyone using the menu to your head."},
                new ButtonInfo { buttonText = "Admin Orbit All", method =() => Experimental.OrbitAllUsing(), toolTip = "Makes everyone using the menu orbit you."},

                new ButtonInfo { buttonText = "No Admin Indicator", enableMethod =() => Experimental.EnableNoAdminIndicator(), method =() => Experimental.NoAdminIndicator(), disableMethod =() => Experimental.AdminIndicatorBack(), toolTip = "Disables the cone that appears above your head to others with the menu."},
            },

            new ButtonInfo[] { // Enabled Mods [24]
                new ButtonInfo { buttonText = "Exit Enabled Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Internal Mods (hidden from user) [25]
                new ButtonInfo { buttonText = "Search", method =() => Settings.Search(), isTogglable = false, toolTip = "Lets you search for specific mods."},
                new ButtonInfo { buttonText = "Global Return", method =() => Settings.GlobalReturn(), isTogglable = false, toolTip = "Returns you to the previous category."},
                new ButtonInfo { buttonText = "Info Screen", method =() => Settings.Debug(), enableMethod =() => Settings.ShowDebug(), disableMethod =() => Settings.HideDebug(), toolTip = "Shows game and modding related information."},
                new ButtonInfo { buttonText = "Donate Button", method =() => { NotifiLib.ClearAllNotifications(); acceptedDonations = true; File.WriteAllText($"{PluginInfo.BaseDirectory}/iiMenu_HideDonationButton.txt", "true"); Prompt("I've spent nearly two years building this menu. Your Patreon support helps me keep it growing, want to check it out?", () => Process.Start("https://patreon.com/iiDk")); }, isTogglable = false },

                new ButtonInfo { buttonText = "Accept Prompt", method =() => { NotifiLib.ClearAllNotifications(); IsPrompting = false; AcceptAction?.Invoke(); }, isTogglable = false},
                new ButtonInfo { buttonText = "Decline Prompt", method =() => { NotifiLib.ClearAllNotifications(); IsPrompting = false; DeclineAction?.Invoke(); }, isTogglable = false}
            },

            new ButtonInfo[] { // Sound Library [26]
                new ButtonInfo { buttonText = "Exit Sound Library", method = () => Sound.LoadSoundboard(), isTogglable = false, toolTip = "Returns you back to the soundboard." }
            },

            new ButtonInfo[] { // Experimental Mods [27]
                new ButtonInfo { buttonText = "Exit Experimental Mods", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Overlap RPCs", enableMethod =() => NoOverlapRPCs = false, disableMethod =() => NoOverlapRPCs = true, toolTip = "Disables the check that only allows you to flush once a frame."},

                new ButtonInfo { buttonText = "Fix Broken Buttons", method =() => Experimental.FixDuplicateButtons(), isTogglable = false, toolTip = "Fixes any duplicate or broken buttons."},

                new ButtonInfo { buttonText = "Get Sound Data", method =() => Experimental.DumpSoundData(), isTogglable = false, toolTip = "Dumps the hand tap sounds to a file."},
                new ButtonInfo { buttonText = "Get Cosmetic Data", method =() => Experimental.DumpCosmeticData(), isTogglable = false, toolTip = "Dumps the cosmetics and their data to a file."},
                new ButtonInfo { buttonText = "Get Decryptable Cosmetic Data", method =() => Experimental.DecryptableCosmeticData(), isTogglable = false, toolTip = "Dumps the cosmetics and their data to a easily decryptable file for databases."},
                new ButtonInfo { buttonText = "Get RPC Data", method =() => Experimental.DumpRPCData(), isTogglable = false, toolTip = "Dumps the data of every RPC to a file."},

                new ButtonInfo { buttonText = "Copy Custom Gamemode Script", method =() => Experimental.CopyCustomGamemodeScript(), isTogglable = false, toolTip = "Copies the lua script source code of the current custom map being played."},

                new ButtonInfo { buttonText = "Better FPS Boost", enableMethod =() => Experimental.BetterFPSBoost(), disableMethod =() => Experimental.DisableBetterFPSBoost(), toolTip = "Makes everything one color, boosting your FPS."},

                new ButtonInfo { buttonText = "Replay Tutorial", method =() => Settings.ShowTutorial(), isTogglable = false, toolTip = "Replays the tutorial video."},
                new ButtonInfo { buttonText = "Disorganize Menu", method =() => Settings.DisorganizeMenu(), isTogglable = false, toolTip = "Disorganizes the entire menu. This cannot be undone."},
            },

            new ButtonInfo[] { // Safety Settings [28]
                new ButtonInfo { buttonText = "Exit Safety Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Anti Report Distance", overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeAntiReportRange(), enableMethod =() => Safety.ChangeAntiReportRange(), disableMethod =() => Safety.ChangeAntiReportRange(false), incremental = true, isTogglable = false, toolTip = "Changes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Change FPS Spoof Value", overlapText = "Change FPS Spoof Value <color=grey>[</color><color=green>90</color><color=grey>]</color>", method =() => Safety.ChangeFPSSpoofValue(), enableMethod =() => Safety.ChangeFPSSpoofValue(), disableMethod =() => Safety.ChangeFPSSpoofValue(false), incremental = true, isTogglable = false, toolTip = "Changes the target FPS for the FPS Spoof mod."},

                new ButtonInfo { buttonText = "Visualize Anti Report", toolTip = "Visualizes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Smart Anti Report", enableMethod =() => Safety.smartarp = true, disableMethod =() => Safety.smartarp = false, toolTip = "Makes the anti report mods only activate in non-modded public lobbies."},

                new ButtonInfo { buttonText = "Change Ranked Tier", overlapText = "Change Ranked Tier <color=grey>[</color><color=green>High</color><color=grey>]</color>", method =() => Safety.ChangeRankedTier(), enableMethod =() => Safety.ChangeRankedTier(), disableMethod =() => Safety.ChangeRankedTier(false), incremental = true, isTogglable = false, toolTip = "Changes the targetted tier for the rank spoof mod."},
            },

            new ButtonInfo[] { // Temporary Category [29]

            },

            new ButtonInfo[] { // Soundboard Settings [30]
                new ButtonInfo { buttonText = "Exit Soundboard Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Loop Sounds", enableMethod =() => Sound.LoopAudio = true, disableMethod =() => Sound.LoopAudio = false, toolTip = "Makes sounds loop forever until stopped."},
                new ButtonInfo { buttonText = "Sound Bindings", overlapText = "Sound Bindings <color=grey>[</color><color=green>None</color><color=grey>]</color>", method =() => Sound.SoundBindings(), enableMethod =() => Sound.SoundBindings(), disableMethod =() => Sound.SoundBindings(false), incremental = true, isTogglable = false, toolTip = "Changes the button used to play sounds on the soundboard."},
            },

            new ButtonInfo[] { // Overpowered Settings [31]
                new ButtonInfo { buttonText = "Exit Overpowered Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Disable Snowball Impact Effect", method =() => Overpowered.DisableSnowballImpactEffect(), toolTip = "Disables the impact effect that people get when hit with snowballs."},
                new ButtonInfo { buttonText = "Change Snowball Scale", overlapText = "Change Snowball Scale <color=grey>[</color><color=green>6</color><color=grey>]</color>", method =() => Overpowered.ChangeSnowballScale(true), enableMethod =() => Overpowered.ChangeSnowballScale(true), disableMethod =() => Overpowered.ChangeSnowballScale(false), incremental = true, isTogglable = false, toolTip = "Changes the scale of the snowballs." },

                new ButtonInfo { buttonText = "Change Lag Power", overlapText = "Change Lag Power <color=grey>[</color><color=green>Heavy</color><color=grey>]</color>", method =() => Overpowered.ChangeLagPower(true), enableMethod =() => Overpowered.ChangeLagPower(true), disableMethod =() => Overpowered.ChangeLagPower(false), incremental = true, isTogglable = false, toolTip = "Changes the power of the lag mods." },
            },

            new ButtonInfo[] { // Keybind Settings [32]
                new ButtonInfo { buttonText = "Exit Keybind Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Non-Toggle Keybinds", enableMethod =() => ToggleBindings = false, disableMethod =() => ToggleBindings = true, toolTip = "Enables mods while holding down the button, instead of toggling them."},
                new ButtonInfo { buttonText = "Clear All Keybinds", method =() => Settings.ClearAllKeybinds(), isTogglable = false, toolTip = "Enables mods while holding down the button, instead of toggling them."},

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

            new ButtonInfo[] { // Plugin Settings [33]
                new ButtonInfo { buttonText = "Exit Plugin Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},
                new ButtonInfo { buttonText = "Reload Plugins", method = () => Settings.ReloadPlugins(), isTogglable = false, toolTip = "Reloads all of your plugins." }
            },

            new ButtonInfo[] { // Friends [34]
                new ButtonInfo { buttonText = "Exit Friends", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},
                new ButtonInfo { buttonText = "Loading...", label = true},
            },

            new ButtonInfo[] { // Friend Settings [35]
                new ButtonInfo { buttonText = "Exit Friend Settings", method =() => currentCategoryName = "Settings", isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Disable Rig Networking", enableMethod =() => FriendManager.RigNetworking = false, disableMethod =() => FriendManager.RigNetworking = true, toolTip = "Disables the networking between friends when your rig is disabled."},
                new ButtonInfo { buttonText = "Disable Platform Networking", enableMethod =() => FriendManager.PlatformNetworking = false, disableMethod =() => FriendManager.PlatformNetworking = true, toolTip = "Disables the platform networking between friends."},
                new ButtonInfo { buttonText = "Friend Sided Projectiles", enableMethod =() => Projectiles.friendSided = true, disableMethod =() => Projectiles.friendSided = false, toolTip = "Makes projectiles only appear between friends."},

                new ButtonInfo { buttonText = "Physical Platforms", enableMethod =() => FriendManager.PhysicalPlatforms = true, disableMethod =() => FriendManager.PhysicalPlatforms = false, toolTip = "Allows networked platforms to be collided with between friends."},
            },

            new ButtonInfo[] { // Fun Settings [36]
                new ButtonInfo { buttonText = "Exit Fun Settings", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Zero Gravity Bugs", toolTip = "Removes the gravity from the bugs on the Bug Spam mod."},
                new ButtonInfo { buttonText = "Bug Colliders", toolTip = "Gives the bug colliders on the Bug Spam mod."},
                new ButtonInfo { buttonText = "Bouncy Bug", toolTip = "Makes the bug bounce off of surfaces if using the bug colliders setting on the Bug Spam mod."},

                new ButtonInfo { buttonText = "Change Custom Quest Score", overlapText = "Change Custom Quest Score <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Fun.ChangeCustomQuestScore(true), enableMethod =() => Fun.ChangeCustomQuestScore(true), disableMethod =() => Fun.ChangeCustomQuestScore(false), incremental = true, isTogglable = false, toolTip = "Changes the value of the \"Custom Quest Score\" mod." },

                new ButtonInfo { buttonText = "Zero Gravity Blocks", toolTip = "Removes the gravity from the blocks."},
                new ButtonInfo { buttonText = "Random Block Type", toolTip = "Selects a random block when using block mods."},

                new ButtonInfo { buttonText = "No Random Position Grab", toolTip = "Disables the position randomization in the \"Grab All ### Blocks\" mods."},
                new ButtonInfo { buttonText = "No Random Rotation Grab", toolTip = "Disables the rotation randomization in the \"Grab All ### Blocks\" mods."},

                new ButtonInfo { buttonText = "Change Block Delay", overlapText = "Change Block Delay <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Fun.ChangeBlockDelay(true), enableMethod =() => Fun.ChangeBlockDelay(true), disableMethod =() => Fun.ChangeBlockDelay(false), incremental = true, isTogglable = false, toolTip = "Gives the blocks a delay before spawning." },
                new ButtonInfo { buttonText = "Change Cycle Delay", overlapText = "Change Name Cycle Delay <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Fun.ChangeCycleDelay(true), enableMethod =() => Fun.ChangeCycleDelay(true), disableMethod =() => Fun.ChangeCycleDelay(false), incremental = true, isTogglable = false, toolTip = "Changes the delay on name cycle mods." }
            },

            new ButtonInfo[] { // Players [37]
                new ButtonInfo { buttonText = "Exit Players", method =() => currentCategoryName = "Main", isTogglable = false, toolTip = "Returns you back to the main page." }
            }
        };

        public static string[] categoryNames = new string[]
        {
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
            "Spam Mods",
            "Sound Spam Mods",
            "Projectile Spam Mods",
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
            "Players"
        };
    }
}

/*
The mod cemetary
Every mod listed below has been removed from the menu, for one reason or another

new ButtonInfo { buttonText = "Lowercase Name", method =() => Fun.LowercaseName(), isTogglable = false, toolTip = "Makes your name lowercase." },
new ButtonInfo { buttonText = "Long Name", method =() => Fun.LongName(), isTogglable = false, toolTip = "Makes your name really long." },

new ButtonInfo { buttonText = "Shaders", enableMethod =() => Fun.EnableShaders(), disableMethod =() => Fun.DisableShaders(), toolTip = "Adds bloom, motion blur, and slight saturation to the game. Credits to leah / tagmonkevr for the code."},

new ButtonInfo { buttonText = "Barrel Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectMinigun(-1724683316), toolTip = "Spawns barrels out of your hand."},
new ButtonInfo { buttonText = "Core Minigun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.SpamObjectMinigun(166197108), toolTip = "Spawns collectible cores out of your hand."},

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
new ButtonInfo { buttonText = "Battle Gamemode", method =() => Overpowered.BattleGamemode(), isTogglable = false, toolTip = "Sets the gamemode to battle."},

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
new ButtonInfo { buttonText = "Despawn Lucy", method =() => Overpowered.DespawnLucy(), isTogglable = false, toolTip = "Despawns the ghost Lucy in forest." },
new ButtonInfo { buttonText = "Spaz Lucy", method =() => Overpowered.SpazLucy(), toolTip = "Gives the ghost Lucy a seizure." },

new ButtonInfo { buttonText = "Lucy Chase Self", method =() => Overpowered.LucyChaseSelf(), isTogglable = false, toolTip = "Makes the ghost Lucy chase you." },
new ButtonInfo { buttonText = "Lucy Chase Gun", method =() => Overpowered.LucyChaseGun(), toolTip = "Makes the ghost Lucy chase whoever your hand desires." },
                
new ButtonInfo { buttonText = "Lucy Attack Self", method =() => Overpowered.LucyAttackSelf(), isTogglable = false, toolTip = "Makes the ghost Lucy attack you." },
new ButtonInfo { buttonText = "Lucy Attack Gun", method =() => Overpowered.LucyAttackGun(), toolTip = "Makes the ghost Lucy attack whoever your hand desires." },
new ButtonInfo { buttonText = "Annoying Lucy", method =() => Overpowered.AnnoyingLucy(), toolTip = "Makes the ghost Lucy really annoying, by attacking everyone and making sounds of the bells." },

new ButtonInfo { buttonText = "Fast Lucy", method =() => Overpowered.FastLucy(), toolTip = "Makes the ghost Lucy become really fast." },
new ButtonInfo { buttonText = "Slow Lucy", method =() => Overpowered.SlowLucy(), toolTip = "Makes the ghost Lucy become really slow." },
 */
