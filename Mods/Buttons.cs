using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Mods.Spammers;
using iiMenu.Notifications;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Menu
{
    public class Buttons
    {
        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] { // Main Stuff [0]
                new ButtonInfo { buttonText = "Join Discord", method =() => Important.JoinDiscord(), isTogglable = false, toolTip = "Invites you to join the ii's <b>Stupid</b> Mods Discord server."},
                new ButtonInfo { buttonText = "Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Opens the settings menu."},
                
                new ButtonInfo { buttonText = "Favorite Mods", method =() => Settings.EnableFavorites(), isTogglable = false, toolTip = "Opens your favorite mods. Favorite mods with left grip."},
                new ButtonInfo { buttonText = "Enabled Mods", method =() => Settings.EnableEnabled(), isTogglable = false, toolTip = "Shows all mods you have enabled."},
                new ButtonInfo { buttonText = "Room Mods", method =() => Settings.EnableRoom(), isTogglable = false, toolTip = "Opens the room mods."},
                new ButtonInfo { buttonText = "Important Mods", method =() => Settings.EnableImportant(), isTogglable = false, toolTip = "Opens the important mods."},
                new ButtonInfo { buttonText = "Safety Mods", method =() => Settings.EnableSafety(), isTogglable = false, toolTip = "Opens the safety mods."},
                new ButtonInfo { buttonText = "Movement Mods", method =() => Settings.EnableMovement(), isTogglable = false, toolTip = "Opens the movement mods."},
                new ButtonInfo { buttonText = "Advantage Mods", method =() => Settings.EnableAdvantage(), isTogglable = false, toolTip = "Opens the advantage giving mods."},
                new ButtonInfo { buttonText = "Visual Mods", method =() => Settings.EnableVisual(), isTogglable = false, toolTip = "Opens the visual mods."},
                new ButtonInfo { buttonText = "Fun Mods", method =() => Settings.EnableFun(), isTogglable = false, toolTip = "Opens the fun mods."},
                new ButtonInfo { buttonText = "Spam Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Opens the spam mods."},
                new ButtonInfo { buttonText = "Master Mods", method =() => Settings.EnableMaster(), isTogglable = false, toolTip = "Opens the master mods."},
                new ButtonInfo { buttonText = "Overpowered Mods", method =() => Settings.EnableOverpowered(), isTogglable = false, toolTip = "Opens the overpowered mods."},
                new ButtonInfo { buttonText = "Experimental Mods", method =() => Settings.EnableBuggy(), isTogglable = false, toolTip = "Opens the experimental mods."},
            },

            new ButtonInfo[] { // Settings [1]
                new ButtonInfo { buttonText = "Exit Settings", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
                new ButtonInfo { buttonText = "Menu Settings", method =() => Settings.EnableMenuSettings(), isTogglable = false, toolTip = "Opens the settings for the menu."},
                new ButtonInfo { buttonText = "Room Settings", method =() => Settings.EnableRoomSettings(), isTogglable = false, toolTip = "Opens the settings for the room mods."},
                new ButtonInfo { buttonText = "Safety Settings", method =() => Settings.EnableSafetySettings(), isTogglable = false, toolTip = "Opens the settings for the safety mods."},
                new ButtonInfo { buttonText = "Movement Settings", method =() => Settings.EnableMovementSettings(), isTogglable = false, toolTip = "Opens the settings for the movement mods."},
                new ButtonInfo { buttonText = "Advantage Settings", method =() => Settings.EnableAdvantageSettings(), isTogglable = false, toolTip = "Opens the settings for the advantage mods."},
                new ButtonInfo { buttonText = "Visual Settings", method =() => Settings.EnableVisualSettings(), isTogglable = false, toolTip = "Opens the settings for the visual mods."},
                new ButtonInfo { buttonText = "Overpowered Settings", method =() => Settings.EnableOverpoweredSettings(), isTogglable = false, toolTip = "Opens the settings for the overpowered mods."},
                new ButtonInfo { buttonText = "Soundboard Settings", method =() => Settings.EnableSoundboardSettings(), isTogglable = false, toolTip = "Opens the settings for the soundboard."},
                new ButtonInfo { buttonText = "Projectile Settings", method =() => Settings.EnableProjectileSettings(), isTogglable = false, toolTip = "Opens the settings for the projectiles."}
            },

            new ButtonInfo[] { // Menu (in Settings) [2]
                new ButtonInfo { buttonText = "Exit Menu Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => Settings.RightHand(), disableMethod =() => Settings.LeftHand(), toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Both Hands", enableMethod =() => Settings.BothHandsOn(), disableMethod =() => Settings.BothHandsOff(), toolTip = "Puts the menu on your both of your hands."},

                new ButtonInfo { buttonText = "One Handed Menu", enableMethod =() => Settings.BarkMenuOn(), disableMethod =() => Settings.BarkMenuOff(), toolTip = "Makes the menu open in front of you, so you can use it with one hand."},
                new ButtonInfo { buttonText = "Joystick Menu", enableMethod =() => Settings.JoystickMenuOn(), disableMethod =() => Settings.JoystickMenuOff(), toolTip = "Makes the menu into something like Colossal, click your joysticks to open, joysticks to move between mods and pages, and click your left joystick to toggle a mod."},
                new ButtonInfo { buttonText = "Physical Menu", enableMethod =() => Settings.PhysicalMenuOn(), disableMethod =() => Settings.PhysicalMenuOff(), toolTip = "Freezes the menu in world space."},
                new ButtonInfo { buttonText = "Wrist Menu", enableMethod =() => Settings.WristThingOn(), disableMethod =() => Settings.WristThingOff(), toolTip = "Turns the menu into a weird wrist watch, click your hand to open it."},
                new ButtonInfo { buttonText = "Watch Menu", enableMethod =() => Settings.WatchMenuOn(), disableMethod =() => Settings.WatchMenuOff(), toolTip = "Turns the menu into a watch, click your joystick to toggle, and move your joystick to select a mod."},
                new ButtonInfo { buttonText = "Shiny Menu", enableMethod =() => Settings.ShinyMenu(), disableMethod =() => Settings.NoShinyMenu(), toolTip = "Makes the menu's textures use the old shader."},
                new ButtonInfo { buttonText = "Thick Menu", enableMethod =() => Settings.ThinMenuOn(), disableMethod =() => Settings.ThinMenuOff(), toolTip = "Makes the menu thin."},
                new ButtonInfo { buttonText = "Long Menu", enableMethod =() => Settings.LongMenuOn(), disableMethod =() => Settings.LongMenuOff(), toolTip = "Makes the menu long."},
                new ButtonInfo { buttonText = "Flip Menu", enableMethod =() => Settings.FlipMenu(), disableMethod =() => Settings.NonFlippedMenu(), toolTip = "Flips the menu to the back of your hand."},

                new ButtonInfo { buttonText = "Outline Menu", enableMethod =() => Settings.OutlineMenuOn(), disableMethod =() => Settings.OutlineMenuOff(), toolTip = "Gives the menu objects an outline."},
                new ButtonInfo { buttonText = "Inner Outline Menu", toolTip = "Gives the menu an outline on the inside."},

                new ButtonInfo { buttonText = "Freeze Player in Menu", method =() => Settings.FreezePlayerInMenu(), enableMethod =() => Settings.FreezePlayerInMenuEnabled(), toolTip = "Freezes your character when inside the menu."},
                new ButtonInfo { buttonText = "Freeze Rig in Menu", method =() => Settings.FreezeRigInMenu(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig when inside the menu."},
                new ButtonInfo { buttonText = "Zero Gravity Menu", toolTip = "Disables gravity on the menu when dropping it."},
                new ButtonInfo { buttonText = "Player Scale Menu", enableMethod =() => Settings.ScaleMenuWithPlayer(), disableMethod =() => Settings.DontScaleMenuWithPlayer(), toolTip = "Scales the menu with your player scale."},
                new ButtonInfo { buttonText = "Alphabetize Menu", toolTip = "Alphabetizes the entire menu."},
                new ButtonInfo { buttonText = "Custom Menu Name", enableMethod =() => Settings.CustomMenuName(), disableMethod =() => Settings.NoCustomMenuName(), toolTip = "Changes the name of the menu to whatever. You can change the text inside of your Gorilla Tag files (iisStupidMenu/iiMenu_CustomMenuName.txt)."},

                new ButtonInfo { buttonText = "Dynamic Animations", enableMethod =() => Settings.DynamicAnimations(), disableMethod =() => Settings.NoDynamicAnimations(), toolTip = "Adds more animations to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Dynamic Gradients", enableMethod =() => Settings.DynamicGradients(), disableMethod =() => Settings.NoDynamicGradients(), toolTip = "Makes gradients dynamic, showing you the full gradient instead of a pulsing color."},
                new ButtonInfo { buttonText = "Dynamic Sounds", enableMethod =() => Settings.DynamicSounds(), disableMethod =() => Settings.NoDynamicSounds(), toolTip = "Adds more sounds to the menu, giving you a better sense of control."},
                new ButtonInfo { buttonText = "Voice Commands", enableMethod =() => Settings.VoiceRecognitionOn(), disableMethod =() => Settings.VoiceRecognitionOff(), toolTip = "Enable and disable sounds with your voice. Activate it like how you would any other voice assistant, such as \"Jarvis\"."},
                new ButtonInfo { buttonText = "Chain Voice Commands", toolTip = "Makes voice commands chain together, so you don't have to repeatedly ask it to listen to you."},

                new ButtonInfo { buttonText = "Annoying Mode", enableMethod =() => Settings.AnnoyingModeOn(), disableMethod =() => Settings.AnnoyingModeOff(), toolTip = "Turns on the April Fools 2024 settings."},
                new ButtonInfo { buttonText = "Lowercase Mode", enableMethod =() => Settings.LowercaseMode(), disableMethod =() => Settings.NoLowercaseMode(), toolTip = "Makes the entire menu's text lowercase."},
                new ButtonInfo { buttonText = "Overflow Mode", enableMethod =() => Settings.OverflowMode(), disableMethod =() => Settings.NoOverflowMode(), toolTip = "Makes the entire menu's text overflow."},

                new ButtonInfo { buttonText = "Change Menu Language", overlapText = "Change Menu Language <color=grey>[</color><color=green>English</color><color=grey>]</color>", method =() => Settings.ChangeMenuLanguage(), isTogglable = false, toolTip = "Changes the language of the menu."},
                new ButtonInfo { buttonText = "Change Menu Theme", method =() => Settings.ChangeMenuTheme(), isTogglable = false, toolTip = "Changes the theme of the menu."},
                new ButtonInfo { buttonText = "Custom Menu Theme", enableMethod =() => Settings.CustomMenuTheme(), disableMethod =() => Settings.FixTheme(), toolTip = "Changes the theme of the menu to a custom one."},
                new ButtonInfo { buttonText = "Change Custom Menu Theme", method =() => Settings.ChangeCustomMenuTheme(), isTogglable = false, toolTip = "Changes the theme of custom the menu."},
                new ButtonInfo { buttonText = "Custom Menu Background", enableMethod =() => Settings.CustomMenuBackground(), disableMethod =() => Settings.FixMenuBackground(), toolTip = "Changes the background of the menu to a custom image. You can change the photo inside of your Gorilla Tag File (iisStupidMenu/iiMenu_CustomMenuBackground.txt)."},
                new ButtonInfo { buttonText = "Change Page Type", method =() => Settings.ChangePageType(), isTogglable = false, toolTip = "Changes the type of page buttons."},
                new ButtonInfo { buttonText = "Change Arrow Type", method =() => Settings.ChangeArrowType(), isTogglable = false, toolTip = "Changes the type of arrows on the page buttons."},
                new ButtonInfo { buttonText = "Change Font Type", method =() => Settings.ChangeFontType(), isTogglable = false, toolTip = "Changes the type of font."},
                new ButtonInfo { buttonText = "Change Font Style Type", method =() => Settings.ChangeFontStyleType(), isTogglable = false, toolTip = "Changes the style of the font."},
                new ButtonInfo { buttonText = "Change Input Text Color", overlapText = "Change Input Text Color <color=grey>[</color><color=green>Green</color><color=grey>]</color>", method =() => Settings.ChangeInputTextColor(), isTogglable = false, toolTip = "Changes the color of the input indicator next to the buttons."},
                new ButtonInfo { buttonText = "Change PC Menu Background", method =() => Settings.ChangePCUI(), isTogglable = false, toolTip = "Changes the background of the PC ui."},
                new ButtonInfo { buttonText = "Change Notification Time", overlapText = "Change Notification Time <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeNotificationTime(), isTogglable = false, toolTip = "Changes the time before a notification is removed."},
                new ButtonInfo { buttonText = "Change Pointer Position", method =() => Settings.ChangePointerPosition(), isTogglable = false, toolTip = "Changes the position of the pointer."},

                new ButtonInfo { buttonText = "Swap GUI Colors", toolTip = "Swaps the GUI colors to the enabled color, for darker themes."},

                new ButtonInfo { buttonText = "Swap Gun Hand", enableMethod =() => Settings.EnableSwapGunHand(), disableMethod =() => Settings.DisableSwapGunHand(), toolTip = "Swaps the hand gun mods work with."},
                new ButtonInfo { buttonText = "Small Gun Pointer", enableMethod =() => Settings.SmallGunPointer(), disableMethod =() => Settings.BigGunPointer(), toolTip = "Makes the ball at the end of every gun mod smaller."},
                new ButtonInfo { buttonText = "Smooth Gun Pointer", enableMethod =() => Settings.DoSmoothGunPointer(), disableMethod =() => Settings.NoSmoothGunPointer(), toolTip = "Makes the ball at the end of every gun mod smoother."},
                new ButtonInfo { buttonText = "Disable Gun Pointer", enableMethod =() => Settings.NoGunPointer(), disableMethod =() => Settings.YesGunPointer(), toolTip = "Disables the ball at the end of every gun mod."},
                new ButtonInfo { buttonText = "Disable Gun Line", enableMethod =() => Settings.NoGunLine(), disableMethod =() => Settings.YesGunLine(), toolTip = "Disables the gun from your hand to the end of every gun mod."},
                new ButtonInfo { buttonText = "Legacy Gun Direction", enableMethod =() => Settings.LegacyGunDirection(), disableMethod =() => Settings.NewGunDirection(), toolTip = "Makes the guns come out of the bottom of your hand instead of your thumb."},

                new ButtonInfo { buttonText = "Checkbox Buttons", enableMethod =() => Settings.CheckboxButtons(), disableMethod =() => Settings.CheckboxButtonsOff(), toolTip = "Turns the buttons into checkboxes."},
                new ButtonInfo { buttonText = "cbsound", overlapText  = "Change Button Sound <color=grey>[</color><color=green>Wood</color><color=grey>]</color>", method =() => Settings.ChangeButtonSound(), isTogglable = false, toolTip = "Changes the button click sound."},
                new ButtonInfo { buttonText = "cbvol", overlapText  = "Change Button Volume <color=grey>[</color><color=green>4</color><color=grey>]</color>", method =() => Settings.ChangeButtonVolume(), isTogglable = false, toolTip = "Changes the volume of the buttons."},
                new ButtonInfo { buttonText = "Serversided Button Sounds", toolTip = "Lets everyone in the the room hear the buttons."},
                new ButtonInfo { buttonText = "Disable Button Vibration", enableMethod =() => Settings.DisableButtonVibration(), disableMethod =() => Settings.EnableButtonVibration(), toolTip = "Disables the slight vibration that happens when you click a button."},

                new ButtonInfo { buttonText = "Clear Notifications on Disconnect", toolTip = "Clears all notifications on disconnect."},
                new ButtonInfo { buttonText = "Hide Notifications on Camera", overlapText = "Streamer Mode Notifications", toolTip = "Makes notifications only render in VR."},
                new ButtonInfo { buttonText = "Disable Notifications", enableMethod =() => Settings.DisableNotifications(), disableMethod =() => Settings.EnableNotifications(), toolTip = "Disables all notifications."},
                new ButtonInfo { buttonText = "Disable Enabled GUI", overlapText = "Disable Arraylist GUI", enableMethod =() => Settings.DisableEnabledGUI(), disableMethod =() => Settings.EnableEnabledGUI(), toolTip = "Disables the GUI that shows the enabled mods."},
                new ButtonInfo { buttonText = "Disable Disconnect Button", enableMethod =() => Settings.DisableDisconnectButton(), disableMethod =() => Settings.EnableDisconnectButton(), toolTip = "Disables the disconnect button at the top of the menu."},
                new ButtonInfo { buttonText = "Disable Search Button", enableMethod =() => Settings.DisableSearchButton(), disableMethod =() => Settings.EnableSearchButton(), toolTip = "Disables the search button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Return Button", enableMethod =() => Settings.DisableReturnButton(), disableMethod =() => Settings.EnableReturnButton(), toolTip = "Disables the return button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Page Buttons", enableMethod =() => Settings.DisablePageButtons(), disableMethod =() => Settings.EnablePageButtons(), toolTip = "Disables the page buttons. Recommended with Joystick Menu."},
                new ButtonInfo { buttonText = "Disable Page Number", enableMethod =() => Settings.DisablePageText(), disableMethod =() => Settings.EnablePageText(), toolTip = "Disables the current page number in the title text."},
                new ButtonInfo { buttonText = "Disable FPS Counter", enableMethod =() => Settings.DisableFPSCounter(), disableMethod =() => Settings.EnableFPSCounter(), toolTip = "Disables the FPS counter."},
                new ButtonInfo { buttonText = "Disable Drop Menu", enableMethod =() => Settings.DropMenu(), disableMethod =() => Settings.DropMenuOff(), toolTip = "Makes the menu despawn instead of falling."},
                new ButtonInfo { buttonText = "Disable Board Colors", overlapText = "Disable Custom Boards", enableMethod =() => Settings.DisableBoardColors(), disableMethod =() => Settings.EnableBoardColors(), toolTip = "Disables the board colors to look legitimate on screen share."},
                new ButtonInfo { buttonText = "Disable Custom Text Colors", enableMethod =() => Settings.DisableBoardTextColors(), disableMethod =() => Settings.EnableBoardTextColors(), toolTip = "Disables the text colors on the boards to make them match their original theme."},
                new ButtonInfo { buttonText = "Hide Text on Camera", overlapText = "Streamer Mode Menu Text", toolTip = "Makes the menu's text only render on VR."},

                new ButtonInfo { buttonText = "Disable Ghostview", enableMethod =() => Settings.DisableGhostview(), disableMethod =() => Settings.EnableGhostview(), toolTip = "Disables the transparent rig when you're in ghost."},
                new ButtonInfo { buttonText = "Legacy Ghostview", enableMethod =() => Settings.LegacyGhostview(), disableMethod =() => Settings.NewGhostview(), toolTip = "Reverts the transparent rig to the two balls when you're in ghost."},

                new ButtonInfo { buttonText = "No Global Search", enableMethod =() => Settings.NoGlobalSearch(), disableMethod =() => Settings.PleaseGlobalSearch(), toolTip = "Makes the search button only search for mods in the current subcategory, unless on the main page."},

                new ButtonInfo { buttonText = "Menu Presets", method =() => Settings.EnableMenuPresets(), isTogglable = false, toolTip = "Opens the page of presets."},
                new ButtonInfo { buttonText = "Save Preferences", method =() => Settings.SavePreferences(), isTogglable = false, toolTip = "Saves your preferences to a file."},
                new ButtonInfo { buttonText = "Load Preferences", method =() => Settings.LoadPreferences(), isTogglable = false, toolTip = "Loads your preferences from a file."},
                new ButtonInfo { buttonText = "Disable Autosave", enableMethod =() => Settings.SavePreferences(), method =() => Settings.NoAutoSave(), toolTip = "Disables the auto save mechanism."},
                new ButtonInfo { buttonText = "Panic", method =() => Settings.Panic(), isTogglable = false, toolTip = "Disables every single active mod."},
            },

            new ButtonInfo[] { // Room (in Settings) [3]
                new ButtonInfo { buttonText = "Exit Room Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "crTime", overlapText = "Change Reconnect Time <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Settings.ChangeReconnectTime(), isTogglable = false, toolTip = "Changes the amount of time waited before attempting to reconnect again."},

                new ButtonInfo { buttonText = "Primary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking primary."},
                new ButtonInfo { buttonText = "Secondary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking secondary."},
                new ButtonInfo { buttonText = "Joystick Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking your joystick."},
            },

            new ButtonInfo[] { // Movement (in Settings) [4]
                new ButtonInfo { buttonText = "Exit Movement Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Change Platform Type", overlapText = "Change Platform Type <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePlatformType(), isTogglable = false, toolTip = "Changes the type of the platforms."},
                new ButtonInfo { buttonText = "Change Platform Shape", overlapText = "Change Platform Shape <color=grey>[</color><color=green>Sphere</color><color=grey>]</color>", method =() => Movement.ChangePlatformShape(), isTogglable = false, toolTip = "Changes the shape of the platforms."},
                new ButtonInfo { buttonText = "Platform Gravity", toolTip = "Makes platforms fall instead of instantly deleting them."},
                new ButtonInfo { buttonText = "Platform Outlines", toolTip = "Makes platforms have outlines."},

                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeFlySpeed(), isTogglable = false, toolTip = "Changes the speed of the fly mods, including iron man."},
                new ButtonInfo { buttonText = "Change Arm Length", overlapText = "Change Arm Length <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeArmLength(), isTogglable = false, toolTip = "Changes the length of the long arm mods, not including iron man."},
                new ButtonInfo { buttonText = "Change Speed Boost Amount", overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeSpeedBoostAmount(), isTogglable = false, toolTip = "Changes the speed of the speed boost mod."},
                new ButtonInfo { buttonText = "Change Pull Mod Power", overlapText = "Change Pull Mod Power <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePullModPower(), isTogglable = false, toolTip = "Changes the power of the pull mod."},
                new ButtonInfo { buttonText = "cdSpeed", overlapText = "Change Drive Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeDriveSpeed(), isTogglable = false, toolTip = "Changes the speed of the drive mod."},
                new ButtonInfo { buttonText = "Factored Speed Boost", toolTip = "Factors your current speed into the speed boost, giving you a positive effect even if you're tagged."},
                new ButtonInfo { buttonText = "Disable Max Speed Modification", toolTip = "Makes your max speed not change, so you can't be detected of using a speed boost."},
                new ButtonInfo { buttonText = "Disable Size Changer Buttons", toolTip = "Disables the size changer's buttons, so hitting grip or trigger or whatever won't do anything."},


                new ButtonInfo { buttonText = "Networked Platforms", method =() => Movement.NetworkedPlatforms(), toolTip = "Makes the platforms networked. This requires attic."},
                new ButtonInfo { buttonText = "Networked Grapple Mods", method =() => Movement.NetworkedGrappleMods(), toolTip = "Makes the spider man and grappling hook mods networked, showing the line for everyone. This requires a balloon."},

                new ButtonInfo { buttonText = "Non-Togglable Ghost", toolTip = "Makes the ghost mod only activate when holding down the button."},
                new ButtonInfo { buttonText = "Non-Togglable Invisible", toolTip = "Makes the invisible mod only activate when holding down the button"},

                new ButtonInfo { buttonText = "Reverse Intercourse", toolTip = "Turns you into the bottom when using the intercourse gun."}
            },

            new ButtonInfo[] { // Projectiles (in Settings) [5]
                new ButtonInfo { buttonText = "Exit Projectile Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},
                new ButtonInfo { buttonText = "Change Projectile", overlapText = "Change Projectile <color=grey>[</color><color=green>Snowball</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectile(), isTogglable = false, toolTip = "Changes the projectile of the projectile spam." },
                //new ButtonInfo { buttonText = "Change Trail", overlapText = "Change Trail <color=grey>[</color><color=green>Regular</color><color=grey>]</color>", method =() => Projectiles.ChangeTrail(), isTogglable = false, toolTip = "Changes the trail of the projectile spam." },
                new ButtonInfo { buttonText = "Random Projectile", toolTip = "Makes the projectiles random." },
                //new ButtonInfo { buttonText = "Random Trail", toolTip = "Makes the projectiles have a random trail." },
                new ButtonInfo { buttonText = "Random Direction", toolTip = "Makes the projectiles go everywhere." },
                new ButtonInfo { buttonText = "Random Color", toolTip = "Makes the projectiles random colors." },
                new ButtonInfo { buttonText = "Change Shoot Speed", overlapText = "Change Shoot Speed <color=grey>[</color><color=green>Medium</color><color=grey>]</color>", method =() => Projectiles.ChangeShootSpeed(), isTogglable = false, toolTip = "Changes the speed of shooting projectiles." },
                new ButtonInfo { buttonText = "Shoot Projectiles", toolTip = "Shoots projectiles like a gun." },
                new ButtonInfo { buttonText = "Finger Gun Projectiles", toolTip = "Shoots projectiles like a finger gun." },
                new ButtonInfo { buttonText = "Include Hand Velocity", toolTip = "Adds the hand velocity to the projectile velocity." },
                new ButtonInfo { buttonText = "Above Players", toolTip = "Makes projectiles go above players." },
                new ButtonInfo { buttonText = "Rain Projectiles", toolTip = "Makes projectiles fall around you like rain." },
                new ButtonInfo { buttonText = "Projectile Aura", toolTip = "Makes projectiles go around you." },
                new ButtonInfo { buttonText = "Projectile Fountain", toolTip = "Makes projectiles spurt out of your head, like a fountain." },
                new ButtonInfo { buttonText = "Rainbow Projectiles", toolTip = "Makes projectiles be rainbow (real RGB)." },
                new ButtonInfo { buttonText = "Hard Rainbow Projectiles", toolTip = "Makes projectiles be rainbow but ye rainbow tis very harsh (real RGB)." },
                new ButtonInfo { buttonText = "Black Projectiles", toolTip = "Makes projectiles black." },
                new ButtonInfo { buttonText = "No Texture Projectiles", toolTip = "Makes projectiles look like they have no texture." },
                new ButtonInfo { buttonText = "RedProj", overlapText = "Red <color=grey>[</color><color=green>10</color><color=grey>]</color>", method =() => Projectiles.IncreaseRed(), isTogglable = false, toolTip = "Makes projectiles more red." },
                new ButtonInfo { buttonText = "GreenProj", overlapText = "Green <color=grey>[</color><color=green>5</color><color=grey>]</color>", method =() => Projectiles.IncreaseGreen(), isTogglable = false, toolTip = "Makes projectiles more green." },
                new ButtonInfo { buttonText = "BlueProj", overlapText = "Blue <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Projectiles.IncreaseBlue(), isTogglable = false, toolTip = "Makes projectiles more blue." },
                new ButtonInfo { buttonText = "Custom Colored Projectiles", toolTip = "Makes the projectile color the custom color (buttons above)." },
                //new ButtonInfo { buttonText = "Legacy Projectiles", toolTip = "Uses the old method of firing projectiles. Grab a snowball.", enabled = true },
                new ButtonInfo { buttonText = "Projectile Delay", overlapText = "Projectile Delay <color=grey>[</color><color=green>0.1</color><color=grey>]</color>", method =() => Projectiles.ProjectileDelay(), isTogglable = false, toolTip = "Gives the projectiles a delay before spawning another." },
            },

            new ButtonInfo[] { // Room Mods [6]
                new ButtonInfo { buttonText = "Exit Room Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Disconnect", method =() => Important.DisconnectR(), isTogglable = false, toolTip = "Disconnects you from the the room."},
                new ButtonInfo { buttonText = "Reconnect", method =() => Important.ReconnectR(), isTogglable = false, toolTip = "Reconnects you from and to the the room."},
                new ButtonInfo { buttonText = "Cancel Reconnect", method =() => Important.CancelReconnect(), isTogglable = false, toolTip = "Cancels the reconnection loop."},
                new ButtonInfo { buttonText = "Join Last Room", method =() => Important.JoinLastRoom(), isTogglable = false, toolTip = "Joins the last room you left."},
                new ButtonInfo { buttonText = "Join Random", method =() => Important.JoinRandomR(), isTogglable = false, toolTip = "Joins a random public room." },
                new ButtonInfo { buttonText = "Create Public", method =() => Important.CreatePublic(), isTogglable = false, toolTip = "Creates a public room."},

                new ButtonInfo { buttonText = "Join Menu Room", method =() => Important.iisStupidMenuRoom(), isTogglable = false, toolTip = "Connects you to a room that is exclusive to ii's <b>Stupid</b> Menu users." },

                new ButtonInfo { buttonText = "Auto Join Room \"RUN\"", method =() => Important.AutoJoinRoomRUN(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"RUN\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY\"", method =() => Important.AutoJoinRoomDAISY(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY09\"", method =() => Important.AutoJoinRoomDAISY09(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY09\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"PBBV\"", method =() => Important.AutoJoinRoomPBBV(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"PBBV\" every couple of seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"BOT\"", method =() => Important.AutoJoinRoomBOT(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"BOT\" every couple of seconds until connected." },
            },

            new ButtonInfo[] { // Important Mods [7]
                new ButtonInfo { buttonText = "Exit Important Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Exit Gorilla Tag", method =() => Application.Quit(), isTogglable = false, toolTip = "Closes Gorilla Tag."},
                new ButtonInfo { buttonText = "Restart Gorilla Tag", method =() => Important.RestartGame(), isTogglable = false, toolTip = "Restarts Gorilla Tag."},

                new ButtonInfo { buttonText = "Anti Hand Tap", enableMethod =() => Safety.EnableAntiHandTap(), disableMethod =() => Safety.DisableAntiHandTap(), toolTip = "Stops all hand tap sounds from being played."},
                new ButtonInfo { buttonText = "First Person Camera", enableMethod =() => Important.EnableFPC(), method =() => Important.MoveFPC(), disableMethod =() => Important.DisableFPC(), toolTip = "Makes your camera output what you see in VR."},
                new ButtonInfo { buttonText = "Force Enable Hands", method =() => Important.ForceEnableHands(), toolTip = "Prevents your hands from disconnecting."},

                new ButtonInfo { buttonText = "Oculus Report Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Important.OculusReportMenu(), toolTip = "Opens the Oculus report menu when holding <color=green>X</color>."},

                new ButtonInfo { buttonText = "Accept TOS", method =() => Important.AcceptTOS(), disableMethod =() => Important.DisableAcceptTOS(), toolTip = "Accepts the Terms of Service for you."},

                new ButtonInfo { buttonText = "Copy Player Position", method =() => Important.CopyPlayerPosition(), isTogglable = false, toolTip = "Copies the current player position to the clipboard." },

                new ButtonInfo { buttonText = "Clear Notifications", method =() => NotifiLib.ClearAllNotifications(), isTogglable = false, toolTip = "Clears your notifications. Good for when they get stuck."},

                new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => Important.EnableAntiAFK(), disableMethod =() => Important.DisableAntiAFK(), toolTip = "Doesn't let you get kicked for being AFK."},
                new ButtonInfo { buttonText = "Disable Network Triggers", enableMethod =() => Important.DisableNetworkTriggers(), disableMethod =() => Important.EnableNetworkTriggers(), toolTip = "Disables the network triggers, so you can change maps without disconnecting."},
                new ButtonInfo { buttonText = "Disable Map Triggers", enableMethod =() => Important.DisableMapTriggers(), disableMethod =() => Important.EnableMapTriggers(), toolTip = "Disables the map triggers, so you can change maps without loading them."},
                new ButtonInfo { buttonText = "Disable Quit Box", enableMethod =() => Important.DisableQuitBox(), disableMethod =() => Important.EnableQuitBox(), toolTip = "Disables the box under the map that closes your game."},
                new ButtonInfo { buttonText = "Physical Quit Box", enableMethod =() => Important.PhysicalQuitbox(), disableMethod =() => Important.NotPhysicalQuitbox(), toolTip = "Makes the quitbox physical, letting you see and walk on it."},

                new ButtonInfo { buttonText = "Disable Mouth Movement", method =() => Important.DisableMouthMovement(), disableMethod =() => Important.EnableMouthMovement(), toolTip = "Disables your mouth from moving."},

                new ButtonInfo { buttonText = "60 FPS", method =() => Important.ForceLagGame(), toolTip = "Caps your FPS at 60 frames per second."},
                new ButtonInfo { buttonText = "Grip 60 FPS <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Important.GripForceLagGame(), toolTip = "Caps your FPS at 60 frames per second when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Unlock FPS", method =() => Important.UncapFPS(), disableMethod =() => Important.CapFPS(), toolTip = "Unlocks your FPS."},

                new ButtonInfo { buttonText = "PC Button Click", method =() => Important.PCButtonClick(), toolTip = "Lets you click in-game buttons with your mouse."},

                new ButtonInfo { buttonText = "Unlock Competitive Queue", method =() => Important.UnlockCompetitiveQueue(), isTogglable = false, toolTip = "Permanently unlocks the competitive queue."},

                new ButtonInfo { buttonText = "Tag Lag Detector", method =() => Important.TagLagDetector(), toolTip = "Detects when the master client is not currently allowing tag requests."},

                new ButtonInfo { buttonText = "Connect to US", method =() => Important.USServers(), isTogglable = false, toolTip = "Connects you to the United States servers."},
                new ButtonInfo { buttonText = "Connect to US West", method =() => Important.USWServers(), isTogglable = false, toolTip = "Connects you to the western United States servers."},
                new ButtonInfo { buttonText = "Connect to EU", method =() => Important.EUServers(), isTogglable = false, toolTip = "Connects you to the Europe servers."},
            },

            new ButtonInfo[] { // Safety Mods [8]
                new ButtonInfo { buttonText = "Exit Safety Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "No Finger Movement", method =() => Safety.NoFinger(), toolTip = "Makes your fingers not move, so you can use wall walk without getting called out." },

                new ButtonInfo { buttonText = "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Movement.FakeOculusMenu(), toolTip = "Imitates opening your Oculus menu when holding <color=green>X</color>."},
                new ButtonInfo { buttonText = "Fake Report Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Movement.FakeReportMenu(), toolTip = "Imitates opening the report menu when holding <color=green>X</color>."},
                new ButtonInfo { buttonText = "Fake Power Off <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.FakePowerOff(), toolTip = "Imitates turning off your headset when holding down your <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Toggle Igloo <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Safety.ToggleIgloo(), toolTip = "Toggles the igloo in mountains when clicking your <color=green>joystick</color>."},

                new ButtonInfo { buttonText = "Disable Gamemode Buttons", enableMethod =() => Safety.DisableGamemodeButtons(), disableMethod =() => Safety.EnableGamemodeButtons(), toolTip = "Disables the gamemode buttons."},
                new ButtonInfo { buttonText = "Spoof Support Page", method =() => Safety.SpoofSupportPage(), toolTip = "Makes the support page appear as if you are on Oculus."},

                new ButtonInfo { buttonText = "Flush RPCs", method =() => RPCProtection(), isTogglable = false, toolTip = "Flushes all RPC calls, good after you stop spamming." },
                new ButtonInfo { buttonText = "Anti Crash", enableMethod =() => Safety.AntiCrashEnabled(), disableMethod =() => Safety.AntiCrashDisabled(), toolTip = "Prevents crashers from completely annihilating your computer."},
                new ButtonInfo { buttonText = "Anti Moderator", method =() => Safety.AntiModerator(), toolTip = "When someone with the stick joins, you get disconnected and their player ID and room code gets saved to a file."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Reconnect</color><color=grey>]</color>", method =() => Safety.AntiReportReconnect(), toolTip = "Disconnects and reconnects you from the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Join Random</color><color=grey>]</color>", method =() => Safety.AntiReportJoinRandom(), toolTip = "Connects you to a random the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>", enableMethod =() => Safety.AntiOculusReportOn(), disableMethod =() => Safety.AntiOculusReportOff(), toolTip = "Disconnects you from the room when you get reported with the Oculus report menu."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Anti Cheat</color><color=grey>]</color>", enableMethod =() => Safety.AntiACReportOn(), disableMethod =() => Safety.AntiACReportOff(), toolTip = "Disconnects you from the room when you get reported by the anti cheat."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Notify</color><color=grey>]</color>", method =() => Safety.AntiReportNotify(), toolTip = "Tells you when people come near your report button, but doesn't do anything."},

                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>", enableMethod =() => Safety.EnableACReportSelf(), disableMethod =() => Safety.DisableACReportSelf(), toolTip = "Gives you a notification every time you have been reported by the anti cheat."},
                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>All</color><color=grey>]</color>", enableMethod =() => Safety.EnableACReportAll(), disableMethod =() => Safety.DisableACReportAll(), toolTip = "Gives you a notification every time anyone has been reported by the anti cheat."},

                new ButtonInfo { buttonText = "Change Identity", overlapText = "Change Identity <color=grey>[</color><color=green>New</color><color=grey>]</color>", method =() => Safety.ChangeIdentity(), isTogglable = false, toolTip = "Changes your name and color to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeIdentityRegular(), isTogglable = false, toolTip = "Changes your name and color to something a regular player would have."},

                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>New</color><color=grey>]</color>", method =() => Safety.ChangeIdentityOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a new player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeIdentityRegularOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a regular player would have."},
                new ButtonInfo { buttonText = "Change Identity on Disconnect <color=grey>[</color><color=green>Child</color><color=grey>]</color>", method =() => Safety.ChangeIdentityMinigamesOnDisconnect(), toolTip = "When you leave, your name and color will be set to something a kid would have."},

                new ButtonInfo { buttonText = "Name Spoof", enableMethod =() => Safety.NameSpoofEnabled(), disableMethod =() => Safety.NameSpoofDisabled(), toolTip = "Changes your name on the leaderboard to something random, but not on your rig."},
                new ButtonInfo { buttonText = "Color Spoof", enableMethod =() => Safety.ColorSpoof(), disableMethod =() => Safety.NoColorSpoof(), toolTip = "Makes your color appear different to every player."},
            },

            new ButtonInfo[] { // Movement Mods [9]
                new ButtonInfo { buttonText = "Exit Movement Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

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
                new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), toolTip = "Moves your rig with <color=green>WASD</color>."},

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

                new ButtonInfo { buttonText = "Force Tag Freeze", method =() => Movement.ForceTagFreeze(), disableMethod =() => Movement.NoTagFreeze(), toolTip = "Forces tag freeze on your character."},
                new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), toolTip = "Disables tag freeze on your character."},
                new ButtonInfo { buttonText = "Low Gravity", method =() => Movement.LowGravity(), toolTip = "Makes gravity lower on your character."},
                new ButtonInfo { buttonText = "Zero Gravity", method =() => Movement.ZeroGravity(), toolTip = "Disables gravity on your character."},
                new ButtonInfo { buttonText = "High Gravity", method =() => Movement.HighGravity(), toolTip = "Makes gravity higher on your character."},
                new ButtonInfo { buttonText = "Reverse Gravity", method =() => Movement.ReverseGravity(), disableMethod =() => Movement.UnflipCharacter(), toolTip = "Reverses gravity on your character."},

                new ButtonInfo { buttonText = "Weak Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WeakWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but weaker."},
                new ButtonInfo { buttonText = "Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Strong Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.StrongWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but stronger."},
                new ButtonInfo { buttonText = "Spider Walk", method =() => Movement.SpiderWalk(), disableMethod =() => Movement.UnflipCharacter(), toolTip = "Makes your gravity and character towards any wall you touch. This may cause motion sickness."},

                new ButtonInfo { buttonText = "Teleport to Random", method =() => Movement.TeleportToRandom(), isTogglable = false, toolTip = "Teleports you to a random player."},
                new ButtonInfo { buttonText = "Teleport to Player", method =() => Movement.EnterTeleportToPlayer(), isTogglable = false, toolTip = "Teleports you to a player of your choosing."},
                new ButtonInfo { buttonText = "Teleport to Map", method =() => Movement.EnterTeleportToMap(), isTogglable = false, toolTip = "Teleports you to a map of your choosing."},
                new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), toolTip = "Teleports to wherever your hand desires."},
                new ButtonInfo { buttonText = "Airstrike", method =() => Movement.Airstrike(), toolTip = "Teleports to wherever your hand desires, except farther up, then launches you back down."},

                new ButtonInfo { buttonText = "Checkpoint <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Checkpoint(), disableMethod =() => Movement.DisableCheckpoint(), toolTip = "Place a checkpoint with <color=green>grip</color> and teleport to it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ender Pearl <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.EnderPearl(), disableMethod =() => Movement.DestroyEnderPearl(), toolTip = "Gives you a throwable ender pearl when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "C4 <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Bomb(), disableMethod =() => Movement.DisableBomb(), toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>."},

                new ButtonInfo { buttonText = "Punch Mod", method =() => Movement.PunchMod(), toolTip = "Lets people punch you across the map."},
                new ButtonInfo { buttonText = "Telekinesis", method =() => Movement.Telekinesis(), toolTip = "Lets people control you with nothing but the power of their finger."},
                new ButtonInfo { buttonText = "Solid Players", method =() => Movement.SolidPlayers(), toolTip = "Lets you walk on top of other players."},
                new ButtonInfo { buttonText = "Pull Mod <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.PullMod(), toolTip = "Pulls you more whenever you walk to simulate speed without modifying your velocity."},

                new ButtonInfo { buttonText = "Speed Boost", method =() => Movement.SpeedBoost(), toolTip = "Changes your speed to whatever you set it to."},
                new ButtonInfo { buttonText = "Grip Speed Boost <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripSpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Speed Boost <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickSpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to when holding <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Uncap Max Velocity", method =() => Movement.UncapMaxVelocity(), toolTip = "Lets you go as fast as you want without hitting the velocity limit."},
                new ButtonInfo { buttonText = "Always Max Velocity", method =() => Movement.AlwaysMaxVelocity(), toolTip = "Always makes you go as fast as the velocity limit."},

                new ButtonInfo { buttonText = "Slippery Hands", enableMethod =() => Movement.EnableSlipperyHands(), disableMethod =() => Movement.DisableSlipperyHands(), toolTip = "Makes everything ice, as in extremely slippery."},
                new ButtonInfo { buttonText = "Grippy Hands", enableMethod =() => Movement.EnableGrippyHands(), disableMethod =() => Movement.DisableGrippyHands(), toolTip = "Covers your hands in grip tape, letting you wallrun as high as you want."},
                new ButtonInfo { buttonText = "Sticky Hands", method =() => Movement.StickyHands(), disableMethod =() => Movement.DisableStickyHands(), toolTip = "Makes your hands really sticky."},
                new ButtonInfo { buttonText = "Climby Hands", method =() => Movement.ClimbyHands(), disableMethod =() => Movement.DisableClimbyHands(), toolTip = "Lets you climb everything like a rope."},

                new ButtonInfo { buttonText = "Slide Control", enableMethod =() => Movement.EnableSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice perfectly."},
                new ButtonInfo { buttonText = "Weak Slide Control", enableMethod =() => Movement.EnableWeakSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice a little more perfect than before."},

                new ButtonInfo { buttonText = "Throw Controllers", method =() => Movement.ThrowControllers(), toolTip = "Lets you throw your controllers with <color=green>X</color> and <color=green>A</color>."},

                new ButtonInfo { buttonText = "Steam Long Arms", method =() => Movement.EnableSteamLongArms(), disableMethod =() => Movement.DisableSteamLongArms(), toolTip = "Gives you long arms similar to override world scale."},
                new ButtonInfo { buttonText = "Stick Long Arms", method =() => Movement.StickLongArms(), toolTip = "Makes you look like you're using sticks."},
                new ButtonInfo { buttonText = "Multiplied Long Arms", method =() => Movement.MultipliedLongArms(), toolTip = "Gives you a weird version of long arms."},
                new ButtonInfo { buttonText = "Vertical Long Arms", method =() => Movement.VerticalLongArms(), toolTip = "Gives you a version of long arms to help you vertically."},
                new ButtonInfo { buttonText = "Horizontal Long Arms", method =() => Movement.HorizontalLongArms(), toolTip = "Gives you a version of long arms to help you horizontally."},
                new ButtonInfo { buttonText = "Velocity Long Arms", enableMethod =() => Movement.CreateVelocityTrackers(), method =() => Movement.VelocityLongArms(), disableMethod =() => Movement.DestroyVelocityTrackers(),  toolTip = "Gives you long arms similar to having high predictions."},

                new ButtonInfo { buttonText = "Flick Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FlickJump(), toolTip = "Makes your hand go down really fast when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Long Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.LongJump(), toolTip = "Makes you look like you're legitimately long jumping when holding <color=green>A</color>."},

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
                new ButtonInfo { buttonText = "Fast Swim", method =() => Movement.FastSwim(), toolTip = "Whenever you are in water, your velocity is slowly multiplied." },
                new ButtonInfo { buttonText = "Disable Air", overlapText = "Disable Wind Barriers", method =() => Movement.DisableAir(), disableMethod =() => Movement.EnableAir(), toolTip = "Disables the wind barriers in every map." },

                new ButtonInfo { buttonText = "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Ghost(), disableMethod =() => Movement.EnableRig(), toolTip = "Keeps your rig still when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", method =() => Movement.Invisible(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes you go invisible when holding <color=green>B</color>."},

                new ButtonInfo { buttonText = "Rig Gun", method =() => Movement.RigGun(), toolTip = "Moves your rig to wherever your hand desires."},
                new ButtonInfo { buttonText = "Grab Rig <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GrabRig(), toolTip = "Lets you grab your rig when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Spaz Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", enableMethod =() => Movement.EnableSpazRig(), method =() => Movement.SpazRig(), disableMethod =() => Movement.DisableSpazRig(), toolTip = "Makes every part of your rig spaz out a little bit when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Rig Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazHands(), toolTip = "Makes your rig's hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazRealHands(), toolTip = "Makes your hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Head Position", enableMethod =() => Movement.EnableSpazHead(), method =() => Movement.SpazHeadPosition(), disableMethod =() => Movement.FixHeadPosition(), toolTip = "Makes your head position spaz out."},
                new ButtonInfo { buttonText = "Random Spaz Head Position", enableMethod =() => Movement.EnableSpazHead(), method =() => Movement.RandomSpazHeadPosition(), disableMethod =() => Movement.FixHeadPosition(), toolTip = "Makes your head position spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Spaz Head", overlapText = "Spaz Head Rotation", method =() => Movement.SpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head rotation spaz out."},
                new ButtonInfo { buttonText = "Random Spaz Head", overlapText = "Random Spaz Head Rotation", method =() => Movement.RandomSpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head rotation spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Laggy Rig", method =() => Movement.LaggyRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig laggy."},
                new ButtonInfo { buttonText = "Update Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.UpdateRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig in place. Whenever you click <color=green>A</color>, your rig will update."},
                new ButtonInfo { buttonText = "Freeze Rig Limbs <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigLimbs(), toolTip = "Makes your hands and head freeze on your rig, but not your body, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Freeze Rig Body <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigBody(), toolTip = "Makes your body freeze on your rig, but not your hands and head, when holding <color=green>A</color>."},

                new ButtonInfo { buttonText = "Auto Dance <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoDance(), toolTip = "Makes you dance when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto Griddy <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoGriddy(), toolTip = "Makes you griddy when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", overlapText = "T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoTPose(), toolTip = "Makes you t pose when holding <color=green>A</color>. Good for fly trolling."},
                new ButtonInfo { buttonText = "Helicopter <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Helicopter(), toolTip = "Turns you into a helicopter when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Beyblade <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Beyblade(), toolTip = "Turns you into a beyblade when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Fan <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Fan(), toolTip = "Turns you into a fan when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ghost Animations", method =() => Movement.GhostAnimations(), disableMethod =() => Movement.DisableGhostAnimations(), toolTip = "Makes you look like a ghost, making your movement snappy and slow."},
                new ButtonInfo { buttonText = "Minecraft Animations", method =() => Movement.MinecraftAnimations(), disableMethod =() => Movement.EnableRig(), toolTip = "Puts your hands down, and makes you walk when holding <color=green>A</color>. You can also point with <color=green>B</color>."},
                new ButtonInfo { buttonText = "Stare at Nearby", overlapText = "Stare At Player Nearby", method =() => Movement.StareAtNearby(), toolTip = "Makes you stare at the nearest player."},
                new ButtonInfo { buttonText = "Stare at Player Gun", method =() => Movement.StareAtGun(), toolTip = "Makes you stare at whoever your hand desires."},
                new ButtonInfo { buttonText = "Floating Rig", enableMethod =() => Movement.EnableFloatingRig(), method =() => Movement.FloatingRig(), disableMethod =() => Movement.DisableFloatingRig(), toolTip = "Makes your rig float."},
                new ButtonInfo { buttonText = "Bees", method =() => Movement.Bees(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig teleport to random players, imitating the bees ghost."},

                new ButtonInfo { buttonText = "Piggyback Gun", method =() => Movement.PiggybackGun(), toolTip = "Teleports you on top of whoever your hand desires repeatedly."},
                new ButtonInfo { buttonText = "Copy Movement Gun", method =() => Movement.CopyMovementGun(), toolTip = "Makes your rig copy the movement of whoever your hand desires."},
                new ButtonInfo { buttonText = "Follow Player Gun", method =() => Movement.FollowPlayerGun(), toolTip = "Flies your rig towards whoever your hand desires."},
                new ButtonInfo { buttonText = "Orbit Player Gun", method =() => Movement.OrbitPlayerGun(), toolTip = "Orbits your rig around whoever your hand desires."},
                new ButtonInfo { buttonText = "Jumpscare Gun", method =() => Movement.JumpscareGun(), toolTip = "Makes you jumpscare whoever your hand desires."},
                new ButtonInfo { buttonText = "Annoy Player Gun", method =() => Movement.AnnoyPlayerGun(), toolTip = "Spazzes your body around whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Intercourse Gun", method =() => Movement.IntercourseGun(), toolTip = "Makes you thrust whoever your hand desires, with sounds."},
                new ButtonInfo { buttonText = "Head Gun", method =() => Movement.HeadGun(), toolTip = "Makes you thrust whoever your hand desires, but lower, with sounds. I hate you all."}
            },

            new ButtonInfo[] { // Advantage Mods [10]
                new ButtonInfo { buttonText = "Exit Advantage Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Tag Self", method =() => Advantages.TagSelf(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tags yourself."},

                new ButtonInfo { buttonText = "Tag Aura", method =() => Advantages.PhysicalTagAura(), toolTip = "Moves your hand into nearby players when tagged."},
                new ButtonInfo { buttonText = "Grip Tag Aura <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Advantages.GripTagAura(), toolTip = "Moves your hand into nearby players when tagged and when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Tag Aura <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Advantages.JoystickTagAura(), toolTip = "Moves your hand into nearby players when tagged and when pressing <color=green>joystick</color>."},

                new ButtonInfo { buttonText = "Tag Reach", method =() => Advantages.TagReach(), disableMethod =() => Advantages.DisableTagReach(), toolTip = "Makes your hand tag hitbox larger."},

                new ButtonInfo { buttonText = "Tag Gun", method =() => Advantages.TagGun(), toolTip = "Tags whoever your hand desires."},
                new ButtonInfo { buttonText = "Flick Tag Gun", method =() => Advantages.FlickTagGun(), toolTip = "Moves your hand to wherever your hand desires in an attempt to tag whoever your hand desires."},

                new ButtonInfo { buttonText = "Tag All", method =() => Advantages.TagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the the room."},
                new ButtonInfo { buttonText = "Hunt Tag All", method =() => Advantages.HuntTagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the the room, but for hunt."},

                new ButtonInfo { buttonText = "Tag Bot", method =() => Advantages.TagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, use <color=green>B</color> to turn it off."},
                new ButtonInfo { buttonText = "Hunt Tag Bot", method =() => Advantages.HuntTagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, but for hunt. Use <color=green>B</color> to turn it off."},

                new ButtonInfo { buttonText = "No Tag on Join", method =() => Advantages.NoTagOnJoin(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "When you join a the room, you won't be tagged when you join."},
                new ButtonInfo { buttonText = "Untag Self", method =() => Advantages.UntagSelf(), isTogglable = false, toolTip = "Removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Anti Tag", method =() => Advantages.AntiTag(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "Removes you from the list of tagged players when tagged."},
            },

            new ButtonInfo[] { // Visual Mods [11]
                new ButtonInfo { buttonText = "Exit Visual Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Morning Time", method =() => Fun.MorningTime(), toolTip = "Makes time day."},
                new ButtonInfo { buttonText = "Day Time", method =() => Fun.DayTime(), toolTip = "Makes time day."},
                new ButtonInfo { buttonText = "Evening Time", method =() => Fun.EveningTime(), toolTip = "Makes time evening."},
                new ButtonInfo { buttonText = "Night Time", method =() => Fun.NightTime(), toolTip = "Makes time night."},
                new ButtonInfo { buttonText = "Fullbright", enableMethod =() => Fun.Fullbright(), disableMethod =() => Fun.Fullshade(), toolTip = "Makes everything bright."},

                new ButtonInfo { buttonText = "Rainy Weather", method =() => Fun.Rain(), toolTip = "Forces the weather to rain."},
                new ButtonInfo { buttonText = "Clear Weather", method =() => Fun.NoRain(), toolTip = "Forces the weather to sunny skies all day."},

                new ButtonInfo { buttonText = "Custom Skybox Color", enableMethod =() => Visuals.DoCustomSkyboxColor(), method =() => Visuals.CustomSkyboxColor(), disableMethod =() => Visuals.UnCustomSkyboxColor(), toolTip = "Changes the skybox color to match the menu."},

                new ButtonInfo { buttonText = "Green Screen", method =() => Visuals.GreenScreen(), toolTip = "Puts a green screen in city." },
                new ButtonInfo { buttonText = "Blue Screen", method =() => Visuals.BlueScreen(), toolTip = "Puts a blue screen in city." },
                new ButtonInfo { buttonText = "Red Screen", method =() => Visuals.RedScreen(), toolTip = "Puts a red screen in city." },

                new ButtonInfo { buttonText = "Velocity Label", method =() => Visuals.VelocityLabel(), toolTip = "Puts text on your right hand, showing your velocity."},
                new ButtonInfo { buttonText = "Nearby Label", method =() => Visuals.NearbyTaggerLabel(), toolTip = "Puts text on your left hand, showing you the distance of the nearest tagger."},
                new ButtonInfo { buttonText = "Last Label", method =() => Visuals.LastLabel(), toolTip = "Puts text on your left hand, showing you how many untagged people are left."},
                new ButtonInfo { buttonText = "Time Label", method =() => Visuals.TimeLabel(), toolTip = "Puts text on your right hand, showing how long you've been playing for without getting tagged."},

                new ButtonInfo { buttonText = "Info Watch", enableMethod =() => Visuals.WatchOn(), method =() => Visuals.WatchStep(), disableMethod =() => Visuals.WatchOff(), toolTip = "Puts a watch on your hand that tells you the time and your FPS."},

                new ButtonInfo { buttonText = "FPS Boost", enableMethod =() => Important.EnableFPSBoost(), disableMethod =() => Important.DisableFPSBoost(), toolTip = "Makes everything low quality in an attempt to boost your FPS."},

                new ButtonInfo { buttonText = "Fake Unban Self", method =() => Visuals.FakeUnbanSelf(), isTogglable = false, toolTip = "Makes it appear as if you're not banned." },

                new ButtonInfo { buttonText = "Audio Visualizer", enableMethod =() => Visuals.CreateAudioVisualizer(), method =() => Visuals.AudioVisualizer(), disableMethod =() => Visuals.DestroyAudioVisualizer(), toolTip = "Shows a visualizer of your microphone loudness below your player."},
                new ButtonInfo { buttonText = "Show Playspace Center", method =() => Visuals.ShowPlayspaceCenter(), toolTip = "Shows the center of your playspace below your player."},

                new ButtonInfo { buttonText = "Visualize Network Triggers", method =() => Visuals.VisualizeNetworkTriggers(), toolTip = "Visualizes the network joining and leaving triggers."},
                new ButtonInfo { buttonText = "Visualize Map Triggers", method =() => Visuals.VisualizeMapTriggers(), toolTip = "Visualizes the map loading and unloading triggers."},

                new ButtonInfo { buttonText = "Name Tags", method =() => Visuals.NameTags(), disableMethod =() => Visuals.DisableNameTags(), toolTip = "Gives players name tags above their heads."},

                new ButtonInfo { buttonText = "Fix Rig Colors", method =() => Visuals.FixRigColors(), toolTip = "Fixes a Steam bug where other players' color would be wrong between servers."},
                new ButtonInfo { buttonText = "Disable Rig Lerping", method =() => Visuals.NoSmoothRigs(), disableMethod =() => Visuals.ReSmoothRigs(), toolTip = "Disables rig movement smoothing."},
                new ButtonInfo { buttonText = "Remove Leaves", enableMethod =() => Visuals.EnableRemoveLeaves(), disableMethod =() => Visuals.DisableRemoveLeaves(), toolTip = "Removes leaves on trees, good for branching."},
                new ButtonInfo { buttonText = "Streamer Remove Leaves", enableMethod =() => Visuals.EnableStreamerRemoveLeaves(), disableMethod =() => Visuals.DisableStreamerRemoveLeaves(), toolTip = "Removes leaves on trees in VR, but not on the camera. Good for streaming."},
                new ButtonInfo { buttonText = "Remove Cosmetics", enableMethod =() => Visuals.DisableCosmetics(), disableMethod =() => Visuals.EnableCosmetics(), toolTip = "Locally toggles off your cosmetics, so you can wear sight-blocking cosmetics such as the eyepatch."},

                new ButtonInfo { buttonText = "Cosmetic ESP", method =() => Visuals.CosmeticESP(), toolTip = "Shows beacons above people's heads if they are a Finger Painter, Illustrator, Administrator, Stick, or if they have any unreleased cosmetics."},

                new ButtonInfo { buttonText = "Voice Indicators", method =() => Visuals.VoiceIndicators(), toolTip = "Puts voice indicators above people's heads when they're talking."},
                new ButtonInfo { buttonText = "Voice ESP", method =() => Visuals.VoiceESP(), toolTip = "Puts voice indicators above people's heads when they're talking, but now they go through walls."},

                new ButtonInfo { buttonText = "No Limb Mode", enableMethod =() => Visuals.StartNoLimb(), method =() => Visuals.NoLimbMode(), disableMethod =() => Visuals.EndNoLimb(), toolTip = "Makes your regular rig invisible, and puts balls on your hands."},

                new ButtonInfo { buttonText = "Casual Tracers", method =() => Visuals.CasualTracers(), toolTip = "Puts tracers on your right hand. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Infection Tracers", method =() => Visuals.InfectionTracers(), toolTip = "Puts tracers on your right hand. Shows everyone."},
                new ButtonInfo { buttonText = "Hunt Tracers", method =() => Visuals.HuntTracers(), toolTip = "Puts tracers on your right hand. Shows your target and who is hunting you."},

                new ButtonInfo { buttonText = "Casual Box ESP", method =() => Visuals.CasualBoxESP(), toolTip = "Acts like casual tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Infection Box ESP", method =() => Visuals.InfectionBoxESP(), toolTip = "Acts like infection tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Hunt Box ESP", method =() => Visuals.HuntBoxESP(), toolTip = "Acts like hunt tracers color wise, but with boxes."},

                new ButtonInfo { buttonText = "Casual Hollow Box ESP", method =() => Visuals.CasualHollowBoxESP(), toolTip = "Acts like casual box ESP, except the box is hollow."},
                new ButtonInfo { buttonText = "Infection Hollow Box ESP", method =() => Visuals.HollowInfectionBoxESP(), toolTip = "Acts like infection box ESP, except the box is hollow."},
                new ButtonInfo { buttonText = "Hunt Hollow Box ESP", method =() => Visuals.HollowHuntBoxESP(), toolTip = "Acts like hunt box ESP, except the box is hollow."},

                new ButtonInfo { buttonText = "Casual Breadcrumbs", method =() => Visuals.CasualBreadcrumbs(), toolTip = "Acts like casual tracers color wise, but with breadcrumbs."},
                new ButtonInfo { buttonText = "Infection Breadcrumbs", method =() => Visuals.InfectionBreadcrumbs(), toolTip = "Acts like infection tracers color wise, but with breadcrumbs."},
                new ButtonInfo { buttonText = "Hunt Breadcrumbs", method =() => Visuals.HuntBreadcrumbs(), toolTip = "Acts like hunt tracers color wise, but with breadcrumbs."},

                new ButtonInfo { buttonText = "Casual Bone ESP", method =() => Visuals.CasualBoneESP(), toolTip = "Acts like casual tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Infection Bone ESP", method =() => Visuals.InfectionBoneESP(), toolTip = "Acts like infection tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Hunt Bone ESP", method =() => Visuals.HuntBoneESP(), toolTip = "Acts like hunt tracers color wise, but with bones."},

                new ButtonInfo { buttonText = "Casual Chams", method =() => Visuals.CasualChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like casual tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Infection Chams", method =() => Visuals.InfectionChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like infection tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Hunt Chams", method =() => Visuals.HuntChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like hunt tracers color wise, but lets you see their fur through walls."},

                new ButtonInfo { buttonText = "Casual Beacons", method =() => Visuals.CasualBeacons(), toolTip = "Acts like casual tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Infection Beacons", method =() => Visuals.InfectionBeacons(), toolTip = "Acts like infection tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Hunt Beacons", method =() => Visuals.HuntBeacons(), toolTip = "Acts like hunt tracers color wise, but it's just a giant line."},

                new ButtonInfo { buttonText = "Casual Distance ESP", method =() => Visuals.CasualDistanceESP(), toolTip = "Shows your distance from players."},
                new ButtonInfo { buttonText = "Infection Distance ESP", method =() => Visuals.InfectionDistanceESP(), toolTip = "Acts like infection tracers color wise, but with text."},
                new ButtonInfo { buttonText = "Hunt Distance ESP", method =() => Visuals.HuntDistanceESP(), toolTip = "Acts like infection tracers color wise, but with text."},

                new ButtonInfo { buttonText = "Show Pointers", method =() => Visuals.ShowButtonColliders(), toolTip = "Shows dots near your hands, such as when you open the menu."},
            },

            new ButtonInfo[] { // Fun Mods [12]
                new ButtonInfo { buttonText = "Exit Fun Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Soundboard", method =() => Sound.LoadSoundboard(), isTogglable = false, toolTip = "A working, customizable soundboard that lets you play audios through your microphone."},

                new ButtonInfo { buttonText = "Upside Down Head", method =() => Fun.UpsideDownHead(), disableMethod =() => Fun.FixHead(), toolTip = "Flips your head upside down on the Z axis."},
                new ButtonInfo { buttonText = "Backwards Head", method =() => Fun.BackwardsHead(), disableMethod =() => Fun.FixHead(), toolTip = "Rotates your head 180 degrees on the Y axis."},
                new ButtonInfo { buttonText = "Broken Neck", method =() => Fun.BrokenNeck(), disableMethod =() => Fun.FixHead(), toolTip = "Rotates your head 90 degrees on the Z axis."},

                new ButtonInfo { buttonText = "Head Bang", method =() => Fun.HeadBang(), disableMethod =() => Fun.FixHead(), toolTip = "Bangs your head at the BPM of Paint it Black (159)."},

                new ButtonInfo { buttonText = "Spin Head X", method =() => Fun.SpinHeadX(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the X axis."},
                new ButtonInfo { buttonText = "Spin Head Y", method =() => Fun.SpinHeadY(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Y axis."},
                new ButtonInfo { buttonText = "Spin Head Z", method =() => Fun.SpinHeadZ(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Z axis."},

                new ButtonInfo { buttonText = "Flip Hands", method =() => Fun.FlipHands(), toolTip = "Swaps your hands, left is right and right is left."},
                new ButtonInfo { buttonText = "Loud Hand Taps", method =() => Fun.LoudHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really loud."},
                new ButtonInfo { buttonText = "Silent Hand Taps", method =() => Fun.SilentHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really quiet."},
                new ButtonInfo { buttonText = "Instant Hand Taps", method =() => Fun.EnableInstantHandTaps(), disableMethod =() => Fun.DisableInstantHandTaps(), toolTip = "Removes the hand tap cooldown."},
                new ButtonInfo { buttonText = "Silent Hand Taps on Tag", method =() => Fun.SilentHandTapsOnTag(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really quiet when you're tagged, good for ambush."},

                new ButtonInfo { buttonText = "Water Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.WaterSplashHands(), toolTip = "Splashes water when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Water Splash Walk", method =() => Fun.WaterSplashWalk(), toolTip = "Splashes water whenever you take a step."},
                new ButtonInfo { buttonText = "Water Splash Aura", method =() => Fun.WaterSplashAura(), toolTip = "Splashes water around you at random positions."},
                new ButtonInfo { buttonText = "Orbit Water Splash", method =() => Fun.OrbitWaterSplash(), toolTip = "Splashes water orbitally around you."},
                new ButtonInfo { buttonText = "Water Splash Gun", method =() => Fun.WaterSplashGun(), toolTip = "Splashes water wherever your hand desires."},
                new ButtonInfo { buttonText = "Confuse Player Gun", method =() => Movement.ConfusePlayerGun(), toolTip = "Makes whoever your hand desires look like they're going crazy by splashing water on their screen."},

                new ButtonInfo { buttonText = "Boop", method =() => Fun.Boop(), toolTip = "Makes a pop sound when you touch someone's nose."},
                new ButtonInfo { buttonText = "Slap", method =() => Fun.Slap(), toolTip = "Makes a bong sound when you hit someone's face."},

                new ButtonInfo { buttonText = "Auto Clicker <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Fun.AutoClicker(), toolTip = "Automatically presses  trigger for you when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Keyboard Tracker", method =() => Fun.KeyboardTracker(), disableMethod =() => Fun.DisableKeyboardTracker(), toolTip = "Tracks everyone's keyboard inputs in the lobby."},

                new ButtonInfo { buttonText = "Mute Gun", method =() => Fun.MuteGun(), toolTip = "Mutes or unmutes whoever your hand desires."},
                new ButtonInfo { buttonText = "Mute All", method =() => Fun.MuteAll(), disableMethod =() => Fun.UnmuteAll(), toolTip = "Mutes everyone in the lobby."},

                new ButtonInfo { buttonText = "Low Quality Microphone", enableMethod =() => Fun.LowQualityMicrophone(), disableMethod =() => Fun.HighQualityMicrophone(), toolTip = "Makes your microphone have really bad quality."},
                new ButtonInfo { buttonText = "Loud Microphone", enableMethod =() => Fun.LoudMicrophone(), disableMethod =() => Fun.NotLoudMicrophone(), toolTip = "Makes your microphone really loud."},
                new ButtonInfo { buttonText = "Reload Microphone", method =() => Fun.ReloadMicrophone(), isTogglable = false,  toolTip = "Reloads / fixes your microphone."},

                new ButtonInfo { buttonText = "Microphone Feedback", method =() => Fun.MicrophoneFeedback(), toolTip = "Plays sound coming through your microphone back to your speakers."},
                new ButtonInfo { buttonText = "Copy Voice Gun", method =() => Fun.CopyVoiceGun(), toolTip = "Copies the voice of whoever your hand desires."},

                new ButtonInfo { buttonText = "Activate All Doors <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.ActivateAllDoors(), toolTip = "Activates all doors when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Crystals <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.TapAllCrystals(), toolTip = "Taps all crystals when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Tap All Bells <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.TapAllBells(), toolTip = "Taps all bells when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Get Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GetHoneyComb(), toolTip = "Gives you a party bracelet without needing to be in a party."},
                new ButtonInfo { buttonText = "Spam Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.HoneycombSpam(), toolTip = "Spams the party bracelet on and off."},
                new ButtonInfo { buttonText = "Remove Bracelet", method =() => Fun.RemoveBracelet(), isTogglable = false, toolTip = "Disables the party bracelet. This does not kick you from the party."},

                new ButtonInfo { buttonText = "Grab ID Card <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabIDCard(), toolTip = "Puts the ID card in your hand." },

                new ButtonInfo { buttonText = "Fast Gliders", enableMethod =() => Fun.FastGliders(), disableMethod =() => Fun.FixGliderSpeed(), toolTip = "Makes the gliders fast."},
                new ButtonInfo { buttonText = "Slow Gliders", enableMethod =() => Fun.SlowGliders(), disableMethod =() => Fun.FixGliderSpeed(), toolTip = "Makes the gliders slow."},

                new ButtonInfo { buttonText = "Glider Blind Gun", method =() => Overpowered.GliderBlindGun(), toolTip = "Moves all of the gliders to whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Glider Blind All", method =() => Overpowered.GliderBlindAll(), toolTip = "Moves all of the gliders to everyone's faces." },

                new ButtonInfo { buttonText = "No Respawn Bug", enableMethod =() => Fun.NoRespawnBug(), disableMethod =() => Fun.PleaseRespawnBug(), toolTip = "Doesn't respawn the bug if it goes too far outside the bounds of forest."},
                new ButtonInfo { buttonText = "No Respawn Bat", enableMethod =() => Fun.NoRespawnBat(), disableMethod =() => Fun.PleaseRespawnBat(), toolTip = "Doesn't respawn the bat if it goes too far outside the bounds of caves."},
                new ButtonInfo { buttonText = "No Respawn Gliders", enableMethod =() => Fun.NoRespawnGliders(), disableMethod =() => Fun.PleaseRespawnGliders(), toolTip = "Doesn't respawn gliders that go too far outside the bounds of clouds."},

                new ButtonInfo { buttonText = "Anti Grab", enableMethod =() => Fun.AntiGrab(), disableMethod =() => Fun.AntiGrabDisabled(), toolTip = "Prevents players from picking you up in guardian."},

                new ButtonInfo { buttonText = "Break Bug", enableMethod =() => Fun.BreakBug(), disableMethod =() => Fun.FixBug(), toolTip = "Makes the bug ungrabbable."},
                new ButtonInfo { buttonText = "Break Bat", enableMethod =() => Fun.BreakBat(), disableMethod =() => Fun.FixBat(), toolTip = "Makes the bat ungrabbable."},

                new ButtonInfo { buttonText = "Small Building", enableMethod =() => Fun.SmallBuilding(), disableMethod =() => Fun.BigBuilding(), toolTip = "Lets you build in the attic while small."},
                new ButtonInfo { buttonText = "Multi Grab", method =() => Fun.MultiGrab(), toolTip = "Lets you grab multiple objects."},

                new ButtonInfo { buttonText = "Attic Size Toggle", method =() => Fun.AtticSizeToggle(), toolTip = "Toggles your scale when pressing <color=green>grip</color> or <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Grab All Nearby <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabAllBlocksNearby(), toolTip = "Grabs every nearby building block when holding <color=green>G</color>."},

                new ButtonInfo { buttonText = "Shotgun <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.Shotgun(), toolTip = "Spawns you a shotgun when you press <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Massive Block <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.MassiveBlock(), toolTip = "Spawns you a massive block when you press <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Select Block Gun", method =() => Fun.SelectBlockGun(), toolTip = "Selects whatever building block your hand desires to be used for the building mods." },
                
                new ButtonInfo { buttonText = "Spaz All Moles", method =() => Fun.SpazMoleMachines(), toolTip = "Gives the moles a seizure."},
                new ButtonInfo { buttonText = "Auto Start Moles", method =() => Fun.AutoStartMoles(), toolTip = "Automatically starts the mole games."},
                new ButtonInfo { buttonText = "Auto Hit Moles", method =() => Fun.AutoHitMoles(), toolTip = "Hits all of the moles automatically."},
                new ButtonInfo { buttonText = "Auto Hit Hazards", method =() => Fun.AutoHitHazards(), toolTip = "Hits all of the hazards automatically."},

                new ButtonInfo { buttonText = "Grab Bug <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBug(), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Bat <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBat(), toolTip = "Forces the bat into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Beach Ball <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBeachBall(), toolTip = "Forces the beach ball into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Balloons <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBalloons(), toolTip = "Forces every single balloon cosmetic into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Gliders <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabGliders(), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Building Blocks <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.SpamGrabBlocks(), toolTip = "Forces the building block into your hand when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Bug Gun", method =() => Fun.BugGun(), toolTip = "Moves the bug to wherever your hand desires." },
                new ButtonInfo { buttonText = "Bat Gun", method =() => Fun.BatGun(), toolTip = "Moves the bat to wherever your hand desires." },
                new ButtonInfo { buttonText = "Beach Ball Gun", method =() => Fun.BeachBallGun(), toolTip = "Moves the beach ball to wherever your hand desires." },
                new ButtonInfo { buttonText = "Balloon Gun", method =() => Fun.BalloonGun(), toolTip = "Moves every single balloon cosmetic to wherever your hand desires." },
                new ButtonInfo { buttonText = "Glider Gun", method =() => Fun.GliderGun(), toolTip = "Moves the gliders to wherever your hand desires." },
                new ButtonInfo { buttonText = "Building Block Gun", method =() => Fun.BlocksGun(), toolTip = "Moves the building blocks to wherever your hand desires." },

                new ButtonInfo { buttonText = "Spaz Bug", method =() => Fun.SpazBug(), toolTip = "Gives the bug a seizure." },
                new ButtonInfo { buttonText = "Spaz Bat", method =() => Fun.SpazBat(), toolTip = "Gives the bat a seizure." },
                new ButtonInfo { buttonText = "Spaz Beach Ball", method =() => Fun.SpazBeachBall(), toolTip = "Gives the beach ball a seizure." },
                new ButtonInfo { buttonText = "Spaz Balloons", method =() => Fun.SpazBalloons(), toolTip = "Gives the gliders a seizure." },
                new ButtonInfo { buttonText = "Spaz Gliders", method =() => Fun.SpazGliders(), toolTip = "Gives the gliders a seizure." },

                new ButtonInfo { buttonText = "Orbit Bug", method =() => Fun.BugHalo(), toolTip = "Orbits the bug around you." },
                new ButtonInfo { buttonText = "Orbit Bat", method =() => Fun.BatHalo(), toolTip = "Orbits the bat around you." },
                new ButtonInfo { buttonText = "Orbit Beach Ball", method =() => Fun.BeachBallHalo(), toolTip = "Orbits the beach ball around you." },
                new ButtonInfo { buttonText = "Orbit Balloons", method =() => Fun.OrbitBalloons(), toolTip = "Orbits the balloons around you." },
                new ButtonInfo { buttonText = "Orbit Gliders", method =() => Fun.OrbitGliders(), toolTip = "Orbits the gliders around you." },
                new ButtonInfo { buttonText = "Orbit Building Blocks", method =() => Fun.OrbitBlocks(), toolTip = "Orbits the building blocks around you." },
                new ButtonInfo { buttonText = "Rain Building Blocks", method =() => Fun.RainBuildingBlocks(), toolTip = "Makes the building blocks fall around you like rain." },
                new ButtonInfo { buttonText = "Building Block Aura", method =() => Fun.BuildingBlockAura(), toolTip = "Moves the building blocks around you at random positions." },

                new ButtonInfo { buttonText = "Ride Bug", method =() => Fun.RideBug(), toolTip = "Repeatedly teleports you on top of the bug." },
                new ButtonInfo { buttonText = "Ride Bat", method =() => Fun.RideBat(), toolTip = "Repeatedly teleports you on top of the bat." },
                new ButtonInfo { buttonText = "Ride Beach Ball", method =() => Fun.RideBeachBall(), toolTip = "Repeatedly teleports you on top of the beach ball." },

                new ButtonInfo { buttonText = "Destroy Bug", method =() => Fun.DestroyBug(), isTogglable = false, toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Bat", method =() => Fun.DestroyBat(), isTogglable = false, toolTip = "Sends the bat to hell." },
                new ButtonInfo { buttonText = "Destroy Beach Ball", method =() => Fun.DestroyBeachBall(), isTogglable = false, toolTip = "Sends the beach ball to hell." },
                new ButtonInfo { buttonText = "Destroy Balloons", method =() => Fun.DestroyBalloons(), isTogglable = false, toolTip = "Sends every single balloon cosmetic to hell." },
                new ButtonInfo { buttonText = "Destroy Gliders", method =() => Fun.DestroyGliders(), isTogglable = false, toolTip = "Sends every single glider to hell." },

                new ButtonInfo { buttonText = "Respawn Gliders", method =() => Fun.RespawnGliders(), isTogglable = false, toolTip = "Respawns all the gliders." },
                new ButtonInfo { buttonText = "Pop All Balloons", method =() => Fun.PopAllBalloons(), isTogglable = false, toolTip = "Pops every single balloon cosmetic." },

                new ButtonInfo { buttonText = "Remove Name", method =() => Fun.RemoveName(), isTogglable = false, toolTip = "Sets your name to nothing." },
                new ButtonInfo { buttonText = "Set Name to \"STATUE\"", method =() => Fun.SetNameToSTATUE(), isTogglable = false, toolTip = "Sets your name to \"STATUE\"." },
                new ButtonInfo { buttonText = "Set Name to \"RUN\"", method =() => Fun.SetNameToRUN(), isTogglable = false, toolTip = "Sets your name to \"RUN\"." },
                new ButtonInfo { buttonText = "Set Name to \"BEHINDYOU\"", method =() => Fun.SetNameToBEHINDYOU(), isTogglable = false, toolTip = "Sets your name to \"BEHINDYOU\"." },
                new ButtonInfo { buttonText = "Set Name to \"iiOnTop\"", method =() => Fun.SetNameToiiOnTop(), isTogglable = false, toolTip = "Sets your name to \"iiOnTop\"." },

                new ButtonInfo { buttonText = "PBBV Name Cycle", method =() => Fun.PBBVNameCycle(), toolTip = "Sets your name on a loop to \"PBBV\", \"IS\", and \"HERE\"." },
                new ButtonInfo { buttonText = "J3VU Name Cycle", method =() => Fun.J3VUNameCycle(), toolTip = "Sets your name on a loop to \"J3VU\", \"HAS\", \"BECOME\", and \"HOSTILE\"" },
                new ButtonInfo { buttonText = "Run Rabbit Name Cycle", method =() => Fun.RunRabbitNameCycle(), toolTip = "Sets your name on a loop to \"RUN\" and \"RABBIT\"." },
                new ButtonInfo { buttonText = "Random Name Cycle", method =() => Fun.RandomNameCycle(), toolTip = "Sets your name on a loop to a bunch of random characters." },
                new ButtonInfo { buttonText = "Custom Name Cycle", enableMethod =() => Fun.EnableCustomNameCycle(), method =() => Fun.CustomNameCycle(), toolTip = "Sets your name on a loop to whatever's in the file." },

                new ButtonInfo { buttonText = "Strobe Color", method =() => Fun.StrobeColor(), toolTip = "Makes your character flash." },
                new ButtonInfo { buttonText = "Rainbow Color", method =() => Fun.RainbowColor(), toolTip = "Makes your character rainbow." },
                new ButtonInfo { buttonText = "Hard Rainbow Color", method =() => Fun.HardRainbowColor(), toolTip = "Makes your character flash from red, green, blue, and magenta." },

                new ButtonInfo { buttonText = "Become \"goldentrophy\"", method =() => Fun.BecomeGoldentrophy(), isTogglable = false, toolTip = "Sets your name to \"goldentrophy\" and color to orange." },
                new ButtonInfo { buttonText = "Become \"PBBV\"", method =() => Fun.BecomePBBV(), isTogglable = false, toolTip = "Sets your name to \"PBBV\" and color to sky blue." },
                new ButtonInfo { buttonText = "Become \"J3VU\"", method =() => Fun.BecomeJ3VU(), isTogglable = false, toolTip = "Sets your name to \"J3VU\" and color to green." },
                new ButtonInfo { buttonText = "Become \"ECHO\"", method =() => Fun.BecomeECHO(), isTogglable = false, toolTip = "Sets your name to \"ECHO\" and color to salmon." },
                new ButtonInfo { buttonText = "Become \"DAISY09\"", method =() => Fun.BecomeDAISY09(), isTogglable = false, toolTip = "Sets your name to \"DAISY09\" and color to a light pink." },
                new ButtonInfo { buttonText = "Become Child", method =() => Fun.BecomeMinigamesKid(), isTogglable = false, toolTip = "Sets your name and color to something a child would pick." },

                new ButtonInfo { buttonText = "Become Hidden on Leaderboard", method =() => Fun.BecomeHiddenOnLeaderboard(), isTogglable = false, toolTip = "Sets your name to nothing and your color to a dark red, matching the leaderboard." },
                new ButtonInfo { buttonText = "Copy Identity Gun", method =() => Fun.CopyIdentityGun(), toolTip = "Steals the identity of whoever your hand desires." },

                new ButtonInfo { buttonText = "Change Accessories", overlapText = "Change Cosmetics", method =() => Fun.ChangeAccessories(), toolTip = "Use your grips to change what hat you're wearing." },
                new ButtonInfo { buttonText = "Spaz Accessories", overlapText = "Spaz Cosmetics <color=grey>[</color><color=green>All</color><color=grey>]</color>", method =() => Fun.SpazAccessories(), toolTip = "Spazzes your hats out for everyone when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Spaz Cosmetics <color=grey>[</color><color=green>Others</color><color=grey>]</color>", method =() => Fun.SpazAccessoriesOthers(), toolTip = "Spazzes your hats out for everyone except you when holding <color=green>trigger</color>." },
                new ButtonInfo { buttonText = "Sticky Holdables", method =() => Fun.StickyHoldables(), toolTip = "Makes your holdables sticky." },
                new ButtonInfo { buttonText = "Spaz Holdables", method =() => Fun.SpazHoldables(), toolTip = "Spazzes out the positions of your holdables." },
                new ButtonInfo { buttonText = "Cosmetic Spoof", enableMethod =() => Fun.TryOnAnywhere(), disableMethod =() => Fun.TryOffAnywhere(), toolTip = "Lets you try on cosmetics from anywhere. Enable this mod after wearing the cosmetics." },
                new ButtonInfo { buttonText = "Cosmetic Browser", method =() => Fun.CosmeticBrowser(), isTogglable = false, toolTip = "Browse through every cosmetic that you can try on and add it to your cart." },
                new ButtonInfo { buttonText = "Auto Spoof Cosmetics", enableMethod =() => Fun.AutoLoadCosmetics(), disableMethod =() => Fun.NoAutoLoadCosmetics(), toolTip = "Automatically spoofs your cosmetics, making you appear with anything you're able to try-on." },
                new ButtonInfo { buttonText = "Disable Cosmetics on Tag", method =() => Fun.DisableCosmeticsOnTag(), toolTip = "Disables your cosmetics when you get tagged, good for ambush." },

                new ButtonInfo { buttonText = "Fast Ropes", enableMethod =() => Fun.FastRopes(), disableMethod =() => Fun.RegularRopes(), toolTip = "Makes the ropes really fast." },

                new ButtonInfo { buttonText = "Get ID Self", method =() => Miscellaneous.CopySelfID(), isTogglable = false, toolTip = "Gets your player ID and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get ID Gun", method =() => Miscellaneous.CopyIDGun(), toolTip = "Gets the player ID of whoever your hand desires and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Get Creation Date Self", method =() => Miscellaneous.CopyCreationDateSelf(), isTogglable = false, toolTip = "Gets the creation date of your account and copies it to the clipboard."},
                new ButtonInfo { buttonText = "Get Creation Date Gun", method =() => Miscellaneous.CopyCreationDateGun(), toolTip = "Gets the creation date of whoever your hand desires' account and copies it to the clipboard." },

                new ButtonInfo { buttonText = "Grab Player Info", method =() => Miscellaneous.GrabPlayerInfo(), isTogglable = false, toolTip = "Saves every player's name, color, and player ID as a text file and opens it." },
            },

            new ButtonInfo[] { // Spam Mods [13]
                new ButtonInfo { buttonText = "Exit Spam Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Sound Mods", method =() => Settings.EnableSoundSpam(), isTogglable = false, toolTip = "Opens the sound mods."},
                new ButtonInfo { buttonText = "Projectile Mods", method =() => Settings.EnableProjectileSpam(), isTogglable = false, toolTip = "Opens the projectile mods."},
            },

            new ButtonInfo[] { // Sound Spam Mods [14]
                new ButtonInfo { buttonText = "Exit Sound Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Returns you back to the spam page."},

                new ButtonInfo { buttonText = "Bass Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BassSoundSpam(), toolTip = "Plays the loud drum sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Metal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.MetalSoundSpam(), toolTip = "Plays the metal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Wolf Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.WolfSoundSpam(), toolTip = "Plays the wolf howl when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Cat Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CatSoundSpam(), toolTip = "Plays the cat meow when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Turkey Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.TurkeySoundSpam(), toolTip = "Plays the turkey sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Frog Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.FrogSoundSpam(), toolTip = "Plays the frog creak when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Bee Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BeeSoundSpam(), toolTip = "Plays the bee buzz when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Squeak Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SqueakSoundSpam(), toolTip = "Plays the squeak sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Random Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RandomSoundSpam(), toolTip = "Plays random sounds when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Earrape Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.EarrapeSoundSpam(), toolTip = "Plays a high-pitched sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Ding Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.DingSoundSpam(), toolTip = "Plays a ding sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CrystalSoundSpam(), toolTip = "Plays some crystal noises when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Big Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BigCrystalSoundSpam(), toolTip = "Plays a long crystal sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Pan Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.PanSoundSpam(), toolTip = "Plays a pan sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "AK-47 Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.AK47SoundSpam(), toolTip = "Plays a sound that sounds like an AK-47 when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Siren Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SirenSoundSpam(), toolTip = "Plays a siren sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "- Sound ID", method =() => Sound.DecreaseSoundID(), isTogglable = false, toolTip = "Lowers the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "+ Sound ID", method =() => Sound.IncreaseSoundID(), isTogglable = false, toolTip = "Increases the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "Custom Sound Spam", overlapText = "Custom Sound Spam <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Sound.CustomSoundSpam(), toolTip = "Plays the selected sound when holding <color=green>grip</color>." },
            },

            new ButtonInfo[] { // Projectile Spam Mods [15]
                new ButtonInfo { buttonText = "Exit Projectile Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Returns you back to the spam page."},

                new ButtonInfo { buttonText = "Grab Projectile <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.GrabProjectile(), toolTip = "Grabs your selected projectile(s) holding <color=green>grip</color>. You can change the projectile(s) in Settings > Projectile Settings" },
                new ButtonInfo { buttonText = "Projectile Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.ProjectileSpam(), toolTip = "Spams your selected projectile(s) when holding <color=green>grip</color>. You can change the projectile(s) in Settings > Projectile Settings" },
                new ButtonInfo { buttonText = "Give Projectile Spam Gun", method =() => Projectiles.GiveProjectileSpamGun(), toolTip = "Acts like the projectile spam, but you can give it to whoever your hand desires. They need to hold grip." },
                new ButtonInfo { buttonText = "Impact Spam", method =() => Projectiles.ImpactSpam(), toolTip = "Acts like the projectile spam, but uses the impacts instead." },

                new ButtonInfo { buttonText = "Paper Plane Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.PaperPlaneSpam(), toolTip = "Spams the plane cosmetic when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Fireball Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.FireballSpam(), toolTip = "Spams the fireball cosmetic when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Rapid Fire Slingshot <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.RapidFireSlingshot(), toolTip = "Spams the slingshot." },
                new ButtonInfo { buttonText = "Aimbot <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.Aimbot(), toolTip = "Sends all projectiles you fire into a random player's head." },
                new ButtonInfo { buttonText = "Slingshot Helper", method =() => Projectiles.SlingshotHelper(), toolTip = "Automatically puts the bullet in your right hand." },

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

                new ButtonInfo { buttonText = "Serversided Tracers", method =() => Projectiles.ServersidedTracers(), toolTip = "Spams projectiles that move really fast towards players, like tracers." },
            },

            new ButtonInfo[] { // Master Mods [16]
                new ButtonInfo { buttonText = "Exit Master Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Master Check", method =() => Overpowered.MasterCheck(), isTogglable = false, toolTip = "Checks if you are master client."},

                new ButtonInfo { buttonText = "Silent Guardian", method =() => Overpowered.SilentGuardian(), isTogglable = false, toolTip = "Makes you the guardian without scaling you up."},
                new ButtonInfo { buttonText = "Guardian Self", method =() => Overpowered.GuardianSelf(), isTogglable = false, toolTip = "Makes you the guardian."},
                new ButtonInfo { buttonText = "Guardian Gun", method =() => Overpowered.GuardianGun(), toolTip = "Makes whoever your hand desires the guardian."},
                new ButtonInfo { buttonText = "Guardian All", method =() => Overpowered.GuardianAll(), isTogglable = false, toolTip = "Makes everyone in the lobby the guardian."},

                new ButtonInfo { buttonText = "Unguardian Self", method =() => Overpowered.UnguardianSelf(), isTogglable = false, toolTip = "Removes you from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian Gun", method =() => Overpowered.UnguardianGun(), toolTip = "Removes whoever your hand desires from the guardian position."},
                new ButtonInfo { buttonText = "Unguardian All", method =() => Overpowered.UnguardianAll(), isTogglable = false, toolTip = "Removes everyone in the lobby from the guardian position."},

                new ButtonInfo { buttonText = "Guardian Spaz", method =() => Overpowered.GuardianSpaz(), toolTip = "Spams the guardian position for everyone in the lobby."},

                new ButtonInfo { buttonText = "Unlimited Building", enableMethod =() => Fun.UnlimitedBuilding(), disableMethod =() => Fun.DisableUnlimitedBuilding(), toolTip = "Unlimits building, disabling drop zones and letting you place on people's plots." },
                new ButtonInfo { buttonText = "Destroy Building Block Gun", method =() => Fun.DestroyBlockGun(), toolTip = "Shreds whatever building block your hand desires." },
                new ButtonInfo { buttonText = "Destroy Building Blocks", method =() => Fun.DestroyBlocks(), toolTip = "Shreds every building block." },

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

                new ButtonInfo { buttonText = "Tag Lag", enableMethod =() => Advantages.TagLag(), disableMethod =() => Advantages.NahTagLag(), toolTip = "Forces tag lag in the lobby, letting no one get tagged."},

                new ButtonInfo { buttonText = "Bonk Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BonkSoundSpam(), toolTip = "Plays the bonk sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CountSoundSpam(), toolTip = "Plays the count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlCountSoundSpam(), toolTip = "Plays the brawl count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Start Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlStartSoundSpam(), toolTip = "Plays the brawl start sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tag Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.TagSoundSpam(), toolTip = "Plays the tag sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Round End Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RoundEndSoundSpam(), toolTip = "Plays the round end sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Battle Start Game", method =() => Advantages.BattleStartGame(), isTogglable = false, toolTip = "Starts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle End Game", method =() => Advantages.BattleEndGame(), isTogglable = false, toolTip = "Ends the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Game", method =() => Advantages.BattleRestartGame(), isTogglable = false, toolTip = "Restarts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Spam", method =() => Advantages.BattleRestartGame(), toolTip = "Spam starts and ends the game. Requires master." },

                new ButtonInfo { buttonText = "Battle Balloon Spam Self", method =() => Advantages.BattleBalloonSpamSelf(), toolTip = "Spam pops and unpops your balloons. Requires master." },
                new ButtonInfo { buttonText = "Battle Balloon Spam All", method =() => Advantages.BattleBalloonSpam(), toolTip = "Spam pops and unpops everyone's balloons. Requires master." },

                new ButtonInfo { buttonText = "Battle Kill Gun", method =() => Advantages.BattleKillGun(), toolTip = "Kills whoever your hand desires. Requires master." },
                new ButtonInfo { buttonText = "Battle Kill Self", method =() => Advantages.BattleKillSelf(), isTogglable = false, toolTip = "Kills yourself. Requires master." },
                new ButtonInfo { buttonText = "Battle Kill All", method =() => Advantages.BattleKillAll(), isTogglable = false, toolTip = "Kills everyone. Requires master." },

                new ButtonInfo { buttonText = "Battle Revive Gun", method =() => Advantages.BattleReviveGun(), toolTip = "Revives whoever your hand desires. Requires master." },
                new ButtonInfo { buttonText = "Battle Revive Self", method =() => Advantages.BattleReviveSelf(), isTogglable = false, toolTip = "Revives yourself. Requires master." },
                new ButtonInfo { buttonText = "Battle Revive All", method =() => Advantages.BattleReviveAll(), isTogglable = false, toolTip = "Revives everyone. Requires master." },

                new ButtonInfo { buttonText = "Battle God Mode", method =() => Advantages.BattleGodMode(), toolTip = "Gives you god mode in brawl. Requires master." },

                new ButtonInfo { buttonText = "Slow Gun", method =() => Overpowered.SlowGun(), toolTip = "Forces tag freeze on whoever your hand desires." },
                new ButtonInfo { buttonText = "Slow All", method =() => Overpowered.SlowAll(), toolTip = "Forces tag freeze on everyone in the the room." },

                new ButtonInfo { buttonText = "Vibrate Gun", method =() => Overpowered.VibrateGun(), toolTip = "Makes whoever your hand desires' controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate All", method =() => Overpowered.VibrateAll(), toolTip = "Makes everyone in the the room's controllers vibrate." },
            },

            new ButtonInfo[] { // Overpowered Mods [17]
                new ButtonInfo { buttonText = "Exit Overpowered Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Always Guardian", method =() => Overpowered.AlwaysGuardian(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes you always the guardian."},
                new ButtonInfo { buttonText = "Grab Gun", method =() => Overpowered.GrabGun(), toolTip = "Grabs whoever your hand desires if you're the guardian you."},
                new ButtonInfo { buttonText = "Grab All <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.GrabAll(), toolTip = "Grabs everyone in the lobby if you're the guardian."},

                new ButtonInfo { buttonText = "Release Gun", method =() => Overpowered.ReleaseGun(), toolTip = "Releases whoever your hand desires if you're the guardian."},
                new ButtonInfo { buttonText = "Release All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.ReleaseAll(), toolTip = "Releases everyone in the lobby if you're the guardian."},

                new ButtonInfo { buttonText = "Fling Gun", method =() => Overpowered.FlingGun(), toolTip = "Flings whoever your hand desires."},
                new ButtonInfo { buttonText = "Fling All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FlingAll(), toolTip = "Flings everyone in the lobby."},

                new ButtonInfo { buttonText = "Bring Gun", method =() => Overpowered.BringGun(), toolTip = "Brings whoever your hand desires towards you."},
                new ButtonInfo { buttonText = "Bring All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BringAll(), toolTip = "Brings everyone in the lobby towards you."},

                new ButtonInfo { buttonText = "Bring All Gun", method =() => Overpowered.BringAllGun(), toolTip = "Brings everyone in the lobby towards wherever your hand desires."},

                new ButtonInfo { buttonText = "Give Fly Gun", method =() => Overpowered.GiveFlyGun(), toolTip = "Gives whoever you want fly when they hold their right thumb down."},
                new ButtonInfo { buttonText = "Give Fly All", method =() => Overpowered.GiveFlyAll(), toolTip = "Gives everyone in the lobby fly when they hold their right thumb down."},

                new ButtonInfo { buttonText = "Safety Bubble", method =() => Overpowered.SafetyBubble(), toolTip = "Anyone who gets too close to you will be launched away."},

                new ButtonInfo { buttonText = "Spaz Player Gun", method =() => Overpowered.SpazPlayerGun(), toolTip = "Spazzes out whoever your hand desires."},
                new ButtonInfo { buttonText = "Spaz All Players <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SpazAllPlayers(), toolTip = "Spazzes out everyone in the lobby."},

                new ButtonInfo { buttonText = "Effect Spam Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Overpowered.EffectSpamHands(), toolTip = "Spawns effects when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Effect Spam Gun", method =() => Overpowered.EffectSpamGun(), toolTip = "Spawns effects wherever your hand desires."},

                new ButtonInfo { buttonText = "Physical Freeze Gun", method =() => Overpowered.PhysicalFreezeGun(), toolTip = "Freezes whoever your hand desires." },
                new ButtonInfo { buttonText = "Physical Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.PhysicalFreezeAll(), toolTip = "Freezes everyone in the lobby when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Freeze All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.FreezeAll(), toolTip = "Freezes everyone in the lobby when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Serversided Mute Gun", method =() => Overpowered.MuteGun(), toolTip = "Mutes whoever your hand desires for everyone."},
                new ButtonInfo { buttonText = "Serversided Mute All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.MuteAll(), toolTip = "Mutes everybody in the lobby when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Lag Gun", method =() => Overpowered.LagGun(), toolTip = "Lags whoever your hand desires."},
                new ButtonInfo { buttonText = "Lag All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.LagAll(), toolTip = "Lags everybody in the lobby when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Kick Gun", method =() => Overpowered.KickGun(), disableMethod =() => Overpowered.DisableKickGun(), toolTip = "Kicks whoever your hand desires."},

                new ButtonInfo { buttonText = "Virtual Stump Kick Gun", method =() => Overpowered.VirtualStumpKickGun(), toolTip = "Kicks whoever your hand desires in the custom map."},
                new ButtonInfo { buttonText = "Virtual Stump Kick All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.VirtualStumpKickAll(), toolTip = "Kicks everybody in the custom map when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Force Unload Custom Map", method =() => Overpowered.ForceUnloadCustomMap(), isTogglable = false, toolTip = "Forcefully unloads the current custom map."},

                new ButtonInfo { buttonText = "Attic Crash Gun", method =() => Overpowered.AtticCrashGun(), toolTip = "Crashes whoever your hand desires in the attic."},
                new ButtonInfo { buttonText = "Attic Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.AtticCrashAll(), toolTip = "Crashes everybody inside of the attic."},

                new ButtonInfo { buttonText = "Destroy Gun", method =() => Overpowered.DestroyGun(), toolTip = "Block new players from seeing whoever your hand desires."},
                new ButtonInfo { buttonText = "Destroy All", method =() => Overpowered.DestroyAll(), isTogglable = false, toolTip = "Block new players from seeing everyone."},

                new ButtonInfo { buttonText = "Joystick Rope Control <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Overpowered.JoystickRopeControl(), toolTip = "Control the ropes in the direction of your joystick."},

                new ButtonInfo { buttonText = "Broken Ropes", method =() => Overpowered.SpazGrabbedRopes(), toolTip = "Gives any ropes currently being held onto a seizure."},
                new ButtonInfo { buttonText = "Confusing Ropes", method =() => Overpowered.ConfusingRopes(), toolTip = "Gives any ropes currently being held onto a seizure but only for the person holding the rope."},
                new ButtonInfo { buttonText = "Spaz Rope Gun", method =() => Overpowered.SpazRopeGun(), toolTip = "Gives whatever rope your hand desires a seizure."},
                new ButtonInfo { buttonText = "Spaz All Ropes <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.SpazAllRopes(), toolTip = "Gives every rope a seizure when holding <color=green>trigger</color>."},

                new ButtonInfo { buttonText = "Fling Rope Gun", method =() => Overpowered.FlingRopeGun(), toolTip = "Flings whatever rope your hand desires away from you."},
                new ButtonInfo { buttonText = "Fling All Ropes Gun", method =() => Overpowered.FlingAllRopesGun(), toolTip = "Flings every rope in whatever direction your hand desires."},

                new ButtonInfo { buttonText = "Spawn Second Look", method =() => Overpowered.SpawnSecondLook(), isTogglable = false, toolTip = "Spawns the ghost in the rotational map." },
                new ButtonInfo { buttonText = "Anger Second Look", method =() => Overpowered.AngerSecondLook(), isTogglable = false, toolTip = "Makes the ghost in the rotational map chase after people." },
                new ButtonInfo { buttonText = "Cancel Second Look", method =() => Overpowered.ThrowSecondLook(), isTogglable = false, toolTip = "Makes the ghost in the rotational map throw whoever it's holding." },
                new ButtonInfo { buttonText = "Spaz Second Look", method =() => Overpowered.SpazSecondLook(), toolTip = "Makes the ghost in the rotational map automatically try chasing people and throwing things." },

                new ButtonInfo { buttonText = "Leave Party", method =() => Fun.LeaveParty(), isTogglable = false, toolTip = "Leaves the party, incase you can't pull off the string." },

                new ButtonInfo { buttonText = "Kick All in Party", method =() => Fun.KickAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Ban All in Party", method =() => Fun.BanAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Auto Party Kick", method =() => Fun.AutoPartyKick(), toolTip = "When you party, you will automatically send everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Auto Party Ban", method =() => Fun.AutoPartyBan(), toolTip = "When you party, you will automatically send everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Break Audio Gun", method =() => Overpowered.BreakAudioGun(), toolTip = "Attempts to break the audio of whoever your hand desires." },
                new ButtonInfo { buttonText = "Break Audio All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BreakAudioAll(), toolTip = "Attempts to breaks everyone's audio when holding trigger." },
            },

            new ButtonInfo[] { // Soundboard [18]
                new ButtonInfo { buttonText = "Exit Soundboard", method = () => Settings.EnableFun(), isTogglable = false, toolTip = "Returns you back to the fun mods." }
            },

            new ButtonInfo[] { // Favorite Mods [19]
                new ButtonInfo { buttonText = "Exit Favorite Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Menu Presets [20]
                new ButtonInfo { buttonText = "Exit Menu Presets", method =() => Settings.EnableMenuSettings(), isTogglable = false, toolTip = "Returns to the settings for the menu."},

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
            },

            new ButtonInfo[] { // Advantage (in Settings) [21]
                new ButtonInfo { buttonText = "Exit Advantage Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Obnoxious Tag", toolTip = "Makes the tag mods more obnoxious. Instead of hiding in the ground, you teleport around the player like crazy."},
                new ButtonInfo { buttonText = "ctaRange", overlapText = "Change Tag Aura Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagAuraRange(), isTogglable = false, toolTip = "Changes the range of the tag aura mods."},
                new ButtonInfo { buttonText = "ctrRange", overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagReachDistance(), isTogglable = false, toolTip = "Changes the range of the tag reach mods."},

                new ButtonInfo { buttonText = "Visualize Tag Reach", toolTip = "Visualizes the distance threshold for the tag reach."},
            },

            new ButtonInfo[] { // Visual (in Settings) [22]
                new ButtonInfo { buttonText = "Exit Visual Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Follow Menu Theme", toolTip = "Makes visual mods match the theme of the menu, rather than the color of the player."},
                new ButtonInfo { buttonText = "Transparent Theme", toolTip = "Makes visual mods transparent."},
                new ButtonInfo { buttonText = "Hidden on Camera", overlapText = "Streamer Mode Visuals", toolTip = "Makes visual mods only render on VR."},
                new ButtonInfo { buttonText = "Hidden Labels", overlapText = "Streamer Mode Labels", toolTip = "Makes label mods only render on VR."},
                new ButtonInfo { buttonText = "Thin Tracers", toolTip = "Makes tracers thinner."},
            },

            new ButtonInfo[] { // Admin Mods (admins only) [23]
                new ButtonInfo { buttonText = "Exit Admin Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Get Menu Users", method =() => Experimental.GetMenuUsers(), isTogglable = false, toolTip = "Detects who is using the menu."},
                new ButtonInfo { buttonText = "Menu User Name Tags", enableMethod =() => Experimental.EnableAdminMenuUserTags(), method =() => Experimental.AdminMenuUserTags(), disableMethod =() => Experimental.DisableAdminMenuUserTags(), toolTip = "Detects who is using the menu."},

                new ButtonInfo { buttonText = "Admin Kick Gun", method =() => Experimental.AdminKickGun(), toolTip = "Kicks whoever your hand desires if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Kick All", method =() => Experimental.AdminKickAll(), isTogglable = false, toolTip = "Kicks everyone using the menu."},

                new ButtonInfo { buttonText = "Admin Flip Menu Gun", method =() => Experimental.FlipMenuGun(), toolTip = "Flips the menu of whoever your hand desires if they're using the menu."},

                new ButtonInfo { buttonText = "Admin Teleport Gun", method =() => Experimental.AdminTeleportGun(), toolTip = "Teleports whoever using the menu to wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Fling Gun", method =() => Experimental.AdminFlingGun(), toolTip = "Flings whoever your hand desires upwards."},
                new ButtonInfo { buttonText = "Admin Strangle", method =() => Experimental.AdminStrangle(), toolTip = "Strangles whoever you grab if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Fake Cosmetics", method =() => Experimental.AdminFakeCosmetics(), isTogglable = false, toolTip = "Makes everyone using the menu see whatever cosmetics you have on as if you owned them."},

                new ButtonInfo { buttonText = "Admin Lightning Gun", method =() => Experimental.LightningGun(), toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Aura", method =() => Experimental.LightningAura(), toolTip = "Spawns lightning wherever your hand desires."},
                new ButtonInfo { buttonText = "Admin Lightning Rain", method =() => Experimental.LightningRain(), toolTip = "Rains lightning around you and strikes whoever you hit."},

                new ButtonInfo { buttonText = "Admin Laser", method =() => Experimental.AdminLaser(), toolTip = "Shines a red laser out of your hand when holding <color=green>A</color> or <color=green>X</color>."},

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

                new ButtonInfo { buttonText = "Admin Force Soundboard", method =() => Experimental.AdminSoundMicGun(), toolTip = "Plays a sound through whoever your hand desires' microphone if they're using the menu."},
                new ButtonInfo { buttonText = "Admin Force Local Sound", method =() => Experimental.AdminSoundLocalGun(), toolTip = "Plays a sound through whoever your hand desires' headset if they're using the menu."},

                new ButtonInfo { buttonText = "No Admin Indicator", enableMethod =() => Experimental.EnableNoAdminIndicator(), method =() => Experimental.NoAdminIndicator(), disableMethod =() => Experimental.AdminIndicatorBack(), toolTip = "Disables the cone that appears above your head to others with the menu."},
            },

            new ButtonInfo[] { // Enabled Mods [24]
                new ButtonInfo { buttonText = "Exit Enabled Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Internal Mods (hidden from user) [25]
                new ButtonInfo { buttonText = "Search", method =() => Settings.Search(), isTogglable = false, toolTip = "Lets you search for specific mods."},
                new ButtonInfo { buttonText = "Global Return", method =() => Settings.GlobalReturn(), isTogglable = false, toolTip = "Returns you to the previous category."}
            },

            new ButtonInfo[] { // Sound Library [26]
                new ButtonInfo { buttonText = "Exit Sound Library", method = () => Sound.LoadSoundboard(), isTogglable = false, toolTip = "Returns you back to the soundboard." }
            },

            new ButtonInfo[] { // Experimental Mods [27]
                new ButtonInfo { buttonText = "Exit Experimental Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Experimental RPC Protection", toolTip = "Uses an experimental method of protecting your RPCs."},
                new ButtonInfo { buttonText = "Anti RPC Ban", method =() => Experimental.AntiRPCBan(), isTogglable = false, toolTip = "An experimental anti RPC ban, not letting you get banned for sending RPCs."},

                new ButtonInfo { buttonText = "Hyperflush", method =() => Experimental.Hyperflush(), isTogglable = false, toolTip = "An experimental way of flushing, that should be a little bit more powerful."},

                new ButtonInfo { buttonText = "Fix Broken Buttons", method =() => Experimental.FixDuplicateButtons(), isTogglable = false, toolTip = "Fixes any duplicate or broken buttons."},

                new ButtonInfo { buttonText = "Get Sound Data", method =() => Miscellaneous.DumpSoundData(), isTogglable = false, toolTip = "Dumps the hand tap sounds to a file."},
                new ButtonInfo { buttonText = "Get Cosmetic Data", method =() => Miscellaneous.DumpCosmeticData(), isTogglable = false, toolTip = "Dumps the cosmetics and their data to a file."},
                new ButtonInfo { buttonText = "Get RPC Data", method =() => Miscellaneous.DumpRPCData(), isTogglable = false, toolTip = "Dumps the data of every RPC to a file."},

                new ButtonInfo { buttonText = "Better FPS Boost", enableMethod =() => Experimental.BetterFPSBoost(), disableMethod =() => Experimental.DisableBetterFPSBoost(), toolTip = "Makes everything one color, boosting your FPS."},

                new ButtonInfo { buttonText = "Lowercase Name", method =() => Fun.LowercaseName(), isTogglable = false, toolTip = "Makes your name lowercase." },
                new ButtonInfo { buttonText = "Long Name", method =() => Fun.LongName(), isTogglable = false, toolTip = "Makes your name really long." },

                new ButtonInfo { buttonText = "Disorganize Menu", method =() => Settings.DisorganizeMenu(), isTogglable = false, toolTip = "Disorganizes the entire menu. This cannot be undone."},
            },

            new ButtonInfo[] { // Safety (in settings) [28]
                new ButtonInfo { buttonText = "Exit Safety Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "carrg", overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Safety.ChangeAntiReportRange(), isTogglable = false, toolTip = "Changes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Visualize Anti Report", toolTip = "Visualizes the distance threshold for the anti report mods."},
                new ButtonInfo { buttonText = "Smart Anti Report", enableMethod =() => Safety.SmartAntiReport(), disableMethod =() => Safety.StupidAntiReport(), toolTip = "Makes the anti report mods only activate in non-modded public lobbies."}
            },

            new ButtonInfo[] { // Temporary Category [29]

            },

            new ButtonInfo[] { // Soundboard (in settings) [30]
                new ButtonInfo { buttonText = "Exit Soundboard Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Loop Sounds", enableMethod =() => Sound.EnableLoopSounds(), disableMethod =() => Sound.EnableLoopSounds(), toolTip = "Makes sounds loop forever until stopped."},
                new ButtonInfo { buttonText = "sbds", overlapText = "Sound Bindings <color=grey>[</color><color=green>None</color><color=grey>]</color>", method =() => Sound.SoundBindings(), isTogglable = false, toolTip = "Changes the button used to play sounds on the soundboard."},
            },

            new ButtonInfo[] { // Overpowered (in Settings) [31]
                new ButtonInfo { buttonText = "Exit Overpowered Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Returns you back to the settings menu."},

                new ButtonInfo { buttonText = "Disable Kick Gun Reconnect", toolTip = "Disables automatically reconnecting to the room when the kick gun fails."},
            },
        };
    }
}

/*
The mod cemetary
Every mod listed below has been removed from the menu, for one reason or another

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
new ButtonInfo { buttonText = "Remove Cherry Blossoms", enableMethod = () => Visuals.EnableRemoveCherryBlossoms(), disableMethod = () => Visuals.DisableRemoveCherryBlossoms(), toolTip = "Removes cherry blossoms on trees, good for branching." }

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