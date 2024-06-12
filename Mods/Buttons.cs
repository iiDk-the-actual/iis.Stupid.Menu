using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Mods.Spammers;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Menu
{
    internal class Buttons
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
                new ButtonInfo { buttonText = "Experimental Mods", method =() => Settings.EnableExperimental(), isTogglable = false, toolTip = "Opens the experimental mods."},
            },

            new ButtonInfo[] { // Settings [1]
                new ButtonInfo { buttonText = "Exit Settings", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
                new ButtonInfo { buttonText = "Menu Settings", method =() => Settings.EnableMenuSettings(), isTogglable = false, toolTip = "Opens the settings for the menu."},
                new ButtonInfo { buttonText = "Room Settings", method =() => Settings.EnableRoomSettings(), isTogglable = false, toolTip = "Opens the settings for the room mods."},
                new ButtonInfo { buttonText = "Movement Settings", method =() => Settings.EnableMovementSettings(), isTogglable = false, toolTip = "Opens the settings for the movement mods."},
                new ButtonInfo { buttonText = "Advantage Settings", method =() => Settings.EnableAdvantageSettings(), isTogglable = false, toolTip = "Opens the settings for the advantage mods."},
                new ButtonInfo { buttonText = "Visual Settings", method =() => Settings.EnableVisualSettings(), isTogglable = false, toolTip = "Opens the settings for the visual mods."},
                new ButtonInfo { buttonText = "Projectile Settings", method =() => Settings.EnableProjectileSettings(), isTogglable = false, toolTip = "Opens the settings for the projectiles."}
            },

            new ButtonInfo[] { // Menu (in Settings) [2]
                new ButtonInfo { buttonText = "Exit Menu Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},

                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => Settings.RightHand(), disableMethod =() => Settings.LeftHand(), toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Both Hands", enableMethod =() => Settings.BothHandsOn(), disableMethod =() => Settings.BothHandsOff(), toolTip = "Puts the menu on your both of your hands."},

                new ButtonInfo { buttonText = "One Handed Menu", enableMethod =() => Settings.BarkMenuOn(), disableMethod =() => Settings.BarkMenuOff(), toolTip = "Makes the menu open in front of you, so you can use it with one hand."},
                new ButtonInfo { buttonText = "Joystick Menu", enableMethod =() => Settings.JoystickMenuOn(), disableMethod =() => Settings.JoystickMenuOff(), toolTip = "Makes the menu like colossus, click your joysticks to open, joysticks to move between mods and pages, and click your left joystick to toggle a mod."},
                new ButtonInfo { buttonText = "Outline Menu", enableMethod =() => Settings.OutlineMenuOn(), disableMethod =() => Settings.OutlineMenuOff(), toolTip = "Gives the menu objects an outline."},
                new ButtonInfo { buttonText = "Watch Menu", enableMethod =() => Settings.WatchMenuOn(), disableMethod =() => Settings.WatchMenuOff(), toolTip = "Makes the menu like pocket watch, click your joystick to toggle, and move your joystick to select a mod."},
                new ButtonInfo { buttonText = "Shiny Menu", enableMethod =() => Settings.ShinyMenu(), disableMethod =() => Settings.NoShinyMenu(), toolTip = "Makes the menu's textures use the old shader."},
                new ButtonInfo { buttonText = "Thin Menu", enableMethod =() => Settings.ThinMenuOn(), disableMethod =() => Settings.ThinMenuOff(), toolTip = "Makes the menu thin."},
                new ButtonInfo { buttonText = "Long Menu", enableMethod =() => Settings.LongMenuOn(), disableMethod =() => Settings.LongMenuOff(), toolTip = "Makes the menu long."},
                new ButtonInfo { buttonText = "Flip Menu", enableMethod =() => Settings.FlipMenu(), disableMethod =() => Settings.NonFlippedMenu(), toolTip = "Flips the menu to the back of your hand."},

                new ButtonInfo { buttonText = "Freeze Player in Menu", method =() => Settings.FreezePlayerInMenu(), enableMethod =() => Settings.FreezePlayerInMenuEnabled(), toolTip = "Freezes your character when inside the menu."},
                new ButtonInfo { buttonText = "Freeze Rig in Menu", method =() => Settings.FreezeRigInMenu(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig when inside the menu."},
                new ButtonInfo { buttonText = "Zero Gravity Menu", toolTip = "Disables gravity on the menu when dropping it."},
                new ButtonInfo { buttonText = "Disorganize Menu", method =() => Settings.DisorganizeMenu(), isTogglable = false, toolTip = "Disorganizes the entire menu. This cannot be undone."},
                new ButtonInfo { buttonText = "Alphabetize Menu", toolTip = "Alphabetizes the entire menu."},
                new ButtonInfo { buttonText = "Custom Menu Name", enableMethod =() => Settings.CustomMenuName(), disableMethod =() => Settings.NoCustomMenuName(), toolTip = "Changes the name of the menu to whatever. You can change the text inside of your Gorilla Tag files (iisStupidMenu/iiMenu_CustomMenuName.txt)."},

                new ButtonInfo { buttonText = "Annoying Mode", enableMethod =() => Settings.AnnoyingModeOn(), disableMethod =() => Settings.AnnoyingModeOff(), toolTip = "Turns on the April Fools 2024 settings."},
                new ButtonInfo { buttonText = "Lowercase Mode", enableMethod =() => Settings.LowercaseMode(), disableMethod =() => Settings.NoLowercaseMode(), toolTip = "Makes the entire menu's text lowercase."},
                new ButtonInfo { buttonText = "Overflow Mode", enableMethod =() => Settings.OverflowMode(), disableMethod =() => Settings.NoOverflowMode(), toolTip = "Makes the entire menu's text overflow."},

                new ButtonInfo { buttonText = "Change Menu Theme", method =() => Settings.ChangeMenuTheme(), isTogglable = false, toolTip = "Changes the theme of the menu."},
                new ButtonInfo { buttonText = "Custom Menu Theme", method =() => Settings.CustomMenuTheme(), disableMethod =() => Settings.FixTheme(), toolTip = "Changes the theme of the menu to a custom one. You can change the theme inside of your Gorilla Tag File (iisStupidMenu/iiMenu_CustomMenuTheme.txt)."},
                new ButtonInfo { buttonText = "Change Page Type", method =() => Settings.ChangePageType(), isTogglable = false, toolTip = "Changes the type of page buttons."},
                new ButtonInfo { buttonText = "Change Arrow Type", method =() => Settings.ChangeArrowType(), isTogglable = false, toolTip = "Changes the type of arrows on the page buttons."},
                new ButtonInfo { buttonText = "Change Font Type", method =() => Settings.ChangeFontType(), isTogglable = false, toolTip = "Changes the type of font."},
                new ButtonInfo { buttonText = "Change Font Style Type", method =() => Settings.ChangeFontStyleType(), isTogglable = false, toolTip = "Changes the style of the font."},
                new ButtonInfo { buttonText = "Change PC Menu Background", method =() => Settings.ChangePCUI(), isTogglable = false, toolTip = "Changes the background of the PC ui."},
                new ButtonInfo { buttonText = "Change Notification Time", overlapText = "Change Notification Time <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Settings.ChangeNotificationTime(), isTogglable = false, toolTip = "Changes the time before a notification is removed."},
                new ButtonInfo { buttonText = "Change Pointer Position", method =() => Settings.ChangePointerPosition(), isTogglable = false, toolTip = "Changes the position of the pointer."},

                new ButtonInfo { buttonText = "Small Gun Pointer", enableMethod =() => Settings.SmallGunPointer(), disableMethod =() => Settings.BigGunPointer(), toolTip = "Makes the ball at the end of every gun mod smaller."},
                new ButtonInfo { buttonText = "Disable Gun Pointer", enableMethod =() => Settings.NoGunPointer(), disableMethod =() => Settings.YesGunPointer(), toolTip = "Disables the ball at the end of every gun mod."},
                new ButtonInfo { buttonText = "Disable Gun Line", enableMethod =() => Settings.NoGunLine(), disableMethod =() => Settings.YesGunLine(), toolTip = "Disables the gun from your hand to the end of every gun mod."},

                new ButtonInfo { buttonText = "Checkbox Buttons", enableMethod =() => Settings.CheckboxButtons(), disableMethod =() => Settings.CheckboxButtonsOff(), toolTip = "Makes the buttons like checkboxes."},
                new ButtonInfo { buttonText = "cbsound", overlapText  = "Change Button Sound <color=grey>[</color><color=green>Wood</color><color=grey>]</color>", method =() => Settings.ChangeButtonSound(), isTogglable = false, toolTip = "Changes the button click sound."},
                new ButtonInfo { buttonText = "cbvol", overlapText  = "Change Button Volume <color=grey>[</color><color=green>4</color><color=grey>]</color>", method =() => Settings.ChangeButtonVolume(), isTogglable = false, toolTip = "Changes the volume of the buttons."},
                new ButtonInfo { buttonText = "Serversided Button Sounds", toolTip = "Lets everyone in the the room hear the buttons."},
                new ButtonInfo { buttonText = "Disable Button Vibration", enableMethod =() => Settings.DisableButtonVibration(), disableMethod =() => Settings.EnableButtonVibration(), toolTip = "Disables the slight vibration that happens when you click a button."},

                new ButtonInfo { buttonText = "Disable Notifications", enableMethod =() => Settings.DisableNotifications(), disableMethod =() => Settings.EnableNotifications(), toolTip = "Disables all notifications."},
                new ButtonInfo { buttonText = "Disable Enabled GUI", enableMethod =() => Settings.DisableEnabledGUI(), disableMethod =() => Settings.EnableEnabledGUI(), toolTip = "Disables the GUI that shows the enabled mods."},
                new ButtonInfo { buttonText = "Disable Disconnect Button", enableMethod =() => Settings.DisableDisconnectButton(), disableMethod =() => Settings.EnableDisconnectButton(), toolTip = "Disables the disconnect button at the top of the menu."},
                new ButtonInfo { buttonText = "Disable Search Button", enableMethod =() => Settings.DisableSearchButton(), disableMethod =() => Settings.EnableSearchButton(), toolTip = "Disables the search button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Return Button", enableMethod =() => Settings.DisableReturnButton(), disableMethod =() => Settings.EnableReturnButton(), toolTip = "Disables the return button at the bottom of the menu."},
                new ButtonInfo { buttonText = "Disable Page Buttons", enableMethod =() => Settings.DisablePageButtons(), disableMethod =() => Settings.EnablePageButtons(), toolTip = "Disables the page buttons. Recommended with Joystick Menu."},
                new ButtonInfo { buttonText = "Disable Page Number", enableMethod =() => Settings.DisablePageText(), disableMethod =() => Settings.EnablePageText(), toolTip = "Disables the current page number in the title text."},
                new ButtonInfo { buttonText = "Disable FPS Counter", enableMethod =() => Settings.DisableFPSCounter(), disableMethod =() => Settings.EnableFPSCounter(), toolTip = "Disables the FPS counter."},
                new ButtonInfo { buttonText = "Disable Drop Menu", enableMethod =() => Settings.DropMenu(), disableMethod =() => Settings.DropMenuOff(), toolTip = "Makes the menu despawn instead of falling."},
                new ButtonInfo { buttonText = "Disable Board Colors", overlapText = "Disable Custom Boards", enableMethod =() => Settings.DisableBoardColors(), disableMethod =() => Settings.EnableBoardColors(), toolTip = "Disables the board colors to look legitimate on screen share."},

                new ButtonInfo { buttonText = "Disable Ghostview", enableMethod =() => Settings.DisableGhostview(), disableMethod =() => Settings.EnableGhostview(), toolTip = "Disables the transparent rig when you're in ghost."},
                new ButtonInfo { buttonText = "Legacy Ghostview", enableMethod =() => Settings.LegacyGhostview(), disableMethod =() => Settings.NewGhostview(), toolTip = "Reverts the transparent rig to the two balls when you're in ghost."},

                new ButtonInfo { buttonText = "Menu Presets", method =() => Settings.EnableMenuPresets(), isTogglable = false, toolTip = "Opens the page of presets."},
                new ButtonInfo { buttonText = "Save Preferences", method =() => Settings.SavePreferences(), isTogglable = false, toolTip = "Saves your preferences to a file."},
                new ButtonInfo { buttonText = "Load Preferences", method =() => Settings.LoadPreferences(), isTogglable = false, toolTip = "Loads your preferences from a file."},
                new ButtonInfo { buttonText = "Panic", method =() => Settings.Panic(), isTogglable = false, toolTip = "Disables every single active mod."},
            },

            new ButtonInfo[] { // Room (in Settings) [3]
                new ButtonInfo { buttonText = "Exit Room Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},

                new ButtonInfo { buttonText = "crTime", overlapText = "Change Reconnect Time <color=grey>[</color><color=green>2</color><color=grey>]</color>", method =() => Settings.ChangeReconnectTime(), isTogglable = false, toolTip = "Changes the amount of time waited before attempting to reconnect again."},

                new ButtonInfo { buttonText = "Primary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking primary."},
                new ButtonInfo { buttonText = "Secondary Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking secondary."},
                new ButtonInfo { buttonText = "Joystick Room Mods", enableMethod =() => Settings.EnablePrimaryRoomMods(), disableMethod =() => Settings.DisablePrimaryRoomMods(), toolTip = "Makes the room mods (disconnect, reconnect, etc) only run when clicking your joystick."},
            },

            new ButtonInfo[] { // Movement (in Settings) [4]
                new ButtonInfo { buttonText = "Exit Movement Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},
                new ButtonInfo { buttonText = "Change Platform Type", overlapText = "Change Platform Type <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangePlatformType(), isTogglable = false, toolTip = "Changes the type of the platforms."},
                new ButtonInfo { buttonText = "Change Platform Shape", overlapText = "Change Platform Shape <color=grey>[</color><color=green>Sphere</color><color=grey>]</color>", method =() => Movement.ChangePlatformShape(), isTogglable = false, toolTip = "Changes the shape of the platforms."},
                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeFlySpeed(), isTogglable = false, toolTip = "Changes the speed of the fly mods, including iron man."},
                new ButtonInfo { buttonText = "Change Arm Length", overlapText = "Change Arm Length <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeArmLength(), isTogglable = false, toolTip = "Changes the length of the long arm mods, not including iron man."},
                new ButtonInfo { buttonText = "Change Speed Boost Amount", overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Movement.ChangeSpeedBoostAmount(), isTogglable = false, toolTip = "Changes the speed of the speed boosts."},
                new ButtonInfo { buttonText = "Reverse Intercourse", toolTip = "Turns you into the bottom when using the intercourse gun."}, // i hate you guys
            },

            new ButtonInfo[] { // Projectiles (in Settings) [5]
                new ButtonInfo { buttonText = "Exit Projectile Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},
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
                new ButtonInfo { buttonText = "Blue Team Projectiles", toolTip = "Makes projectiles colored towards the blue team." },
                new ButtonInfo { buttonText = "Orange Team Projectiles", toolTip = "Makes projectiles colored towards the orange team." },
                new ButtonInfo { buttonText = "Red", overlapText = "Red <color=grey>[</color><color=green>1</color><color=grey>]</color>", method =() => Projectiles.IncreaseRed(), isTogglable = false, toolTip = "Makes projectiles more red." },
                new ButtonInfo { buttonText = "Green", overlapText = "Green <color=grey>[</color><color=green>0.5</color><color=grey>]</color>", method =() => Projectiles.IncreaseGreen(), isTogglable = false, toolTip = "Makes projectiles more green." },
                new ButtonInfo { buttonText = "Blue", overlapText = "Blue <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Projectiles.IncreaseBlue(), isTogglable = false, toolTip = "Makes projectiles more blue." },
                new ButtonInfo { buttonText = "Custom Colored Projectiles", toolTip = "Makes the projectile color the custom color (buttons above)." },
                new ButtonInfo { buttonText = "Client Sided Projectiles", toolTip = "Only fires the bullets for you. Impacts are still server sided." },
                //new ButtonInfo { buttonText = "Legacy Projectiles", toolTip = "Uses the old method of firing projectiles. Grab a snowball.", enabled = true },
                new ButtonInfo { buttonText = "Projectile Delay", overlapText = "Projectile Delay <color=grey>[</color><color=green>0.1</color><color=grey>]</color>", method =() => Projectiles.ProjectileDelay(), isTogglable = false, toolTip = "Gives the projectiles a delay before spawning another." },
            },

            new ButtonInfo[] { // Room Mods [6]
                new ButtonInfo { buttonText = "Exit Room Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Disconnect", method =() => Important.Disconnect(), isTogglable = false, toolTip = "Disconnects you from the the room."},
                new ButtonInfo { buttonText = "Reconnect", method =() => Important.Reconnect(), isTogglable = false, toolTip = "Reconnects you from and to the the room."},
                new ButtonInfo { buttonText = "Cancel Reconnect", method =() => Important.CancelReconnect(), isTogglable = false, toolTip = "Cancels the reconnection loop."},
                new ButtonInfo { buttonText = "Join Last Room", method =() => Important.JoinLastRoom(), isTogglable = false, toolTip = "Joins the last room you left."},
                new ButtonInfo { buttonText = "Join Random", method =() => Important.JoinRandom(), isTogglable = false, toolTip = "Joins a random public room." },
                new ButtonInfo { buttonText = "Create Public", method =() => Important.CreatePublic(), isTogglable = false, toolTip = "Creates a public room."},

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
                new ButtonInfo { buttonText = "First Person Camera", method =() => Important.MoveFPC(), disableMethod =() => Important.DisableFPC(), toolTip = "Makes your camera output what you see in VR."},
                new ButtonInfo { buttonText = "Force Enable Hands", method =() => Important.ForceEnableHands(), toolTip = "Prevents your hands from disconnecting."},

                new ButtonInfo { buttonText = "Oculus Report Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", method =() => Important.OculusReportMenu(), toolTip = "Opens the Oculus report menu when holding <color=green>X</color>."},

                new ButtonInfo { buttonText = "Accept TOS", enableMethod =() => Important.AcceptTOS(), toolTip = "Accepts the Terms of Service for you."},

                new ButtonInfo { buttonText = "Copy Player Position", method =() => Important.CopyPlayerPosition(), isTogglable = false, toolTip = "Copies the current player position to the clipboard." },

                new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => Important.EnableAntiAFK(), disableMethod =() => Important.DisableAntiAFK(), toolTip = "Doesn't let you get kicked for being AFK."},
                new ButtonInfo { buttonText = "Disable Network Triggers", enableMethod =() => Important.DisableNetworkTriggers(), disableMethod =() => Important.EnableNetworkTriggers(), toolTip = "Disables the network triggers, so you can change maps without disconnecting."},
                new ButtonInfo { buttonText = "Disable Quit Box", enableMethod =() => Important.DisableQuitBox(), disableMethod =() => Important.EnableQuitBox(), toolTip = "Disables the box under the map that closes your game."},
                new ButtonInfo { buttonText = "Physical Quit Box", enableMethod =() => Important.PhysicalQuitbox(), disableMethod =() => Important.NotPhysicalQuitbox(), toolTip = "Makes the quitbox physical, letting you see and walk on it."},

                new ButtonInfo { buttonText = "Disable Mouth Movement", enableMethod =() => Important.DisableMouthMovement(), disableMethod =() => Important.EnableMouthMovement(), toolTip = "Disables your mouth from moving."},

                new ButtonInfo { buttonText = "60 FPS", method =() => Important.ForceLagGame(), toolTip = "Caps your FPS at 60 frames per second."},
                new ButtonInfo { buttonText = "Grip 60 FPS <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Important.GripForceLagGame(), toolTip = "Caps your FPS at 60 frames per second when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Unlock FPS", method =() => Important.UncapFPS(), toolTip = "Unlocks your FPS."},

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

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you from the the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Reconnect</color><color=grey>]</color>", method =() => Safety.AntiReportReconnect(), toolTip = "Disconnects and reconnects you from the the room when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Join Random</color><color=grey>]</color>", method =() => Safety.AntiReportJoinRandom(), toolTip = "Connects you to a random the room when anyone comes near your report button."},

                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>", enableMethod =() => Safety.EnableACReportSelf(), disableMethod =() => Safety.DisableACReportSelf(), toolTip = "Gives you a notification every time you have been reported by the anti cheat."},
                new ButtonInfo { buttonText = "Show Anti Cheat Reports <color=grey>[</color><color=green>All</color><color=grey>]</color>", enableMethod =() => Safety.EnableACReportAll(), disableMethod =() => Safety.DisableACReportAll(), toolTip = "Gives you a notification every time anyone has been reported by the anti cheat."},

                new ButtonInfo { buttonText = "Change Identity", method =() => Safety.ChangeIdentity(), isTogglable = false, toolTip = "Changes your name on the leaderboard to a random string, but not on your rig."},
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
                new ButtonInfo { buttonText = "Joystick Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickFly(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Sends your character in whatever direction you are pointing your <color=green>joystick</color> in."},
                new ButtonInfo { buttonText = "Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.BarkFly(), toolTip = "Acts like the fly that Bark has. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "Hand Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.HandFly(), toolTip = "Sends your character in your hand's direction when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Zero Gravity Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.ZeroGravitySlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner without gravity, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), toolTip = "Moves your rig with <color=green>WASD</color>."},
                
                new ButtonInfo { buttonText = "Iron Man", method =() => Movement.IronMan(), toolTip = "Turns you into iron man, rotate your hands around to change direction."},
                new ButtonInfo { buttonText = "Spider Man", method =() => Movement.SpiderMan(), disableMethod =() => Movement.DisableSpiderMan(), toolTip = "Turns you into spider man, use your <color=green>grips</color> to shoot webs."},
                new ButtonInfo { buttonText = "Grappling Hooks", method =() => Movement.GrapplingHooks(), disableMethod =() => Movement.DisableSpiderMan(), toolTip = "Gives you grappling hooks, use your <color=green>grips</color> to shoot them."},
                new ButtonInfo { buttonText = "Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.Drive(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Lets you drive around in your invisible car. Use the <color=green>joystick</color> to move."},
                
                new ButtonInfo { buttonText = "Up And Down", method =() => Movement.UpAndDown(), toolTip = "Makes you go up when holding your <color=green>trigger</color>, and down when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Left And Right", method =() => Movement.LeftAndRight(), toolTip = "Makes you go left when holding your <color=green>trigger</color>, and right when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Forwards And Backwards", method =() => Movement.ForwardsAndBackwards(), toolTip = "Makes you go forwards when holding your <color=green>trigger</color>, and backwards when holding your <color=green>grip</color>."},

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
                new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), toolTip = "Teleports to wherever your hand desires."},
                new ButtonInfo { buttonText = "Airstrike", method =() => Movement.Airstrike(), toolTip = "Teleports to wherever your hand desires, except farther up, then launches you back down."},
               
                new ButtonInfo { buttonText = "Checkpoint <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Checkpoint(), disableMethod =() => Movement.DisableCheckpoint(), toolTip = "Place a checkpoint with <color=green>grip</color> and teleport to it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "C4 <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Bomb(), disableMethod =() => Movement.DisableBomb(), toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>."},
                
                new ButtonInfo { buttonText = "Speed Boost", method =() => Movement.SpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to."},
                new ButtonInfo { buttonText = "Grip Speed Boost <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripSpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Speed Boost <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickSpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to when holding <color=green>joystick</color>."},
                new ButtonInfo { buttonText = "Uncap Max Velocity", method =() => Movement.UncapMaxVelocity(), toolTip = "Lets you go as fast as you want without hitting the velocity limit."},
                new ButtonInfo { buttonText = "Always Max Velocity", method =() => Movement.AlwaysMaxVelocity(), toolTip = "Always makes you go as fast as the velocity limit."},

                new ButtonInfo { buttonText = "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Movement.Noclip(), toolTip = "Makes you go through objects when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Ghost(), disableMethod =() => Movement.EnableRig(), toolTip = "Keeps your rig still when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", method =() => Movement.Invisible(), disableMethod =() => Movement.DisableInvisible(), toolTip = "Makes you go invisible when holding <color=green>B</color>."},
                
                new ButtonInfo { buttonText = "Rig Gun", method =() => Movement.RigGun(), toolTip = "Moves your rig to wherever your hand desires."},
                new ButtonInfo { buttonText = "Grab Rig <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GrabRig(), toolTip = "Lets you grab your rig when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Spaz Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", enableMethod =() => Movement.EnableSpazRig(), method =() => Movement.SpazRig(), disableMethod =() => Movement.DisableSpazRig(), toolTip = "Makes every part of your rig spaz out a little bit when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Rig Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazHands(), toolTip = "Makes your rig's hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazRealHands(), toolTip = "Makes your hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Head", method =() => Movement.SpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head spaz out."},
                new ButtonInfo { buttonText = "Random Spaz Head", method =() => Movement.RandomSpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Laggy Rig", method =() => Movement.LaggyRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig laggy."},
                new ButtonInfo { buttonText = "Update Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.UpdateRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig in place. Whenever you click <color=green>A</color>, your rig will update."},
                new ButtonInfo { buttonText = "Freeze Rig Limbs <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigLimbs(), toolTip = "Makes your hands and head freeze on your rig, but not your body, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Freeze Rig Body <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigBody(), toolTip = "Makes your body freeze on your rig, but not your hands and head, when holding <color=green>A</color>."},
                
                new ButtonInfo { buttonText = "Auto Dance <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoDance(), toolTip = "Makes you dance when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto Griddy <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoGriddy(), toolTip = "Makes you griddy when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto T Pose <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoTPose(), toolTip = "Makes you t pose when holding <color=green>A</color>. Good for fly trolling."},
                new ButtonInfo { buttonText = "Helicopter <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Helicopter(), toolTip = "Turns you into a helicopter when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Beyblade <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Beyblade(), toolTip = "Turns you into a beyblade when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Fan <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Fan(), toolTip = "Turns you into a fan when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Minecraft Animations", method =() => Movement.GhostAnimations(), disableMethod =() => Movement.EnableRig(), toolTip = "Puts your hands down, and makes you walk when holding <color=green>A</color>. You can also point with <color=green>B</color>"},
                new ButtonInfo { buttonText = "Stare at Nearby", overlapText = "Stare At Player Nearby", method =() => Movement.StareAtNearby(), toolTip = "Makes you stare at the nearest player."},
                new ButtonInfo { buttonText = "Stare at Player Gun", method =() => Movement.StareAtGun(), toolTip = "Makes you stare at whoever your hand desires."},
                new ButtonInfo { buttonText = "Floating Rig", enableMethod =() => Movement.EnableFloatingRig(), method =() => Movement.FloatingRig(), disableMethod =() => Movement.DisableFloatingRig(), toolTip = "Makes your rig float."},
                new ButtonInfo { buttonText = "Bees", method =() => Movement.Bees(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig teleport to random players, imitating the bees ghost."},
                
                new ButtonInfo { buttonText = "Size Changer", method =() => Movement.SizeChanger(), enableMethod =() => Movement.EnableSizeChanger(), disableMethod =() => Movement.EnableSizeChanger(), toolTip = "Increase your size by holding <color=green>trigger</color>, and decrease your size by holding <color=green>grip</color>."},
                
                new ButtonInfo { buttonText = "Slippery Hands", enableMethod =() => Movement.EnableSlipperyHands(), disableMethod =() => Movement.DisableSlipperyHands(), toolTip = "Makes everything ice, as in extremely slippery."},
                new ButtonInfo { buttonText = "Grippy Hands", enableMethod =() => Movement.EnableGrippyHands(), disableMethod =() => Movement.DisableGrippyHands(), toolTip = "Covers your hands in grip tape, letting you wallrun as high as you want."},
                new ButtonInfo { buttonText = "Sticky Hands", method =() => Movement.StickyHands(), disableMethod =() => Movement.DisableStickyHands(), toolTip = "Makes your hands really sticky."},
                
                new ButtonInfo { buttonText = "Slide Control", enableMethod =() => Movement.EnableSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice perfectly."},
                new ButtonInfo { buttonText = "Weak Slide Control", enableMethod =() => Movement.EnableWeakSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice a little more perfect than before."},
                
                new ButtonInfo { buttonText = "Punch Mod", method =() => Movement.PunchMod(), toolTip = "Lets people punch you across the map."},
                new ButtonInfo { buttonText = "Solid Players", method =() => Movement.SolidPlayers(), toolTip = "Lets you walk on top of other players."},
                
                new ButtonInfo { buttonText = "Throw Controllers", method =() => Movement.ThrowControllers(), toolTip = "Lets you throw your controllers with <color=green>X</color> and <color=green>A</color>."},

                new ButtonInfo { buttonText = "Steam Long Arms", method =() => Movement.EnableSteamLongArms(), disableMethod =() => Movement.DisableSteamLongArms(), toolTip = "Gives you long arms similar to override world scale."},
                new ButtonInfo { buttonText = "Stick Long Arms", method =() => Movement.StickLongArms(), toolTip = "Makes you look like you're using sticks."},
                new ButtonInfo { buttonText = "Multiplied Long Arms", method =() => Movement.MultipliedLongArms(), toolTip = "Gives you a weird version of long arms."},
                new ButtonInfo { buttonText = "Vertical Long Arms", method =() => Movement.VerticalLongArms(), toolTip = "Gives you a version of long arms to help you vertically."},
                new ButtonInfo { buttonText = "Horizontal Long Arms", method =() => Movement.HorizontalLongArms(), toolTip = "Gives you a version of long arms to help you horizontally."},
                new ButtonInfo { buttonText = "Velocity Long Arms", enableMethod =() => Movement.CreateVelocityTrackers(), method =() => Movement.VelocityLongArms(), disableMethod =() => Movement.DestroyVelocityTrackers(),  toolTip = "Gives you long arms similar to override world scale."},

                new ButtonInfo { buttonText = "Flick Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FlickJump(), toolTip = "Makes your hand go down really fast when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Long Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.LongJump(), toolTip = "Makes you look like you're legitimately long jumping when holding <color=green>A</color>."},

                new ButtonInfo { buttonText = "Bunny Hop", method =() => Movement.BunnyHop(), toolTip = "Makes you automatically jump when on the ground."},
                new ButtonInfo { buttonText = "Strafe", method =() => Movement.Strafe(), toolTip = "Makes you strafe when in the air."},
                new ButtonInfo { buttonText = "Grip Bunny Hop <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripBunnyHop(), toolTip = "Makes you automatically jump when on the ground when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Grip Strafe <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GripStrafe(), toolTip = "Makes you strafe when in the air when holding <color=green>grip</color>."},

                new ButtonInfo { buttonText = "Bouncy", enableMethod =() => Movement.PreBouncy(), method =() => Movement.Bouncy(), disableMethod =() => Movement.PostBouncy(), toolTip = "Makes you really bouncy when on the ground."},

                new ButtonInfo { buttonText = "Solid Water", enableMethod =() => Beach.SolidWater(), disableMethod =() => Beach.FixWater(), toolTip = "Makes the water solid in the beach map." },
                new ButtonInfo { buttonText = "Disable Water", enableMethod =() => Beach.DisableWater(), disableMethod =() => Beach.FixWater(), toolTip = "Disables the water in the beach map." },
                new ButtonInfo { buttonText = "Air Swim", method =() => Beach.AirSwim(), disableMethod =() => Beach.DisableAirSwim(), toolTip = "Puts you in a block of water, letting you swim in the air." },
                new ButtonInfo { buttonText = "Fast Swim", method =() => Beach.FastSwim(), toolTip = "Whenever you are in water, your velocity is slowly multiplied." },
                
                new ButtonInfo { buttonText = "Disable Air", enableMethod =() => Movement.DisableAir(), disableMethod =() => Movement.EnableAir(), toolTip = "Disables air in every map." },
                
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

                new ButtonInfo { buttonText = "Tag Self", method =() => Advantages.TagSelf(), toolTip = "Attempts to tags yourself."},
                new ButtonInfo { buttonText = "Tag Aura", method =() => Advantages.PhysicalTagAura(), toolTip = "Moves your hand into nearby players when tagged."},
                new ButtonInfo { buttonText = "Grip Tag Aura <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Advantages.GripTagAura(), toolTip = "Moves your hand into nearby players when tagged and when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Joystick Tag Aura <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Advantages.JoystickTagAura(), toolTip = "Moves your hand into nearby players when tagged and when pressing <color=green>joystick</color>."},
                // new ButtonInfo { buttonText = "RPC Tag Aura", method =() => Advantages.RPCTagAura(), toolTip = "Sends the tag RPC when close enough to nearby players."},
                new ButtonInfo { buttonText = "Tag Gun", method =() => Advantages.TagGun(), toolTip = "Tags whoever your hand desires."},
                new ButtonInfo { buttonText = "Flick Tag Gun", method =() => Advantages.FlickTagGun(), toolTip = "Moves your hand to wherever your hand desires in an attempt to tag whoever your hand desires."},
                new ButtonInfo { buttonText = "Tag All", method =() => Advantages.TagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the the room."},
                new ButtonInfo { buttonText = "Hunt Tag All", method =() => Advantages.HuntTagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the the room, but for hunt."},
                new ButtonInfo { buttonText = "Tag Bot", method =() => Advantages.TagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, use <color=green>B</color> to turn it off."},
                new ButtonInfo { buttonText = "Hunt Tag Bot", method =() => Advantages.HuntTagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, but for hunt. Use <color=green>B</color> to turn it off."},
                new ButtonInfo { buttonText = "No Tag on Join", method =() => Advantages.NoTagOnJoin(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "When you join a the room, you won't be tagged when you join."},
            },

            new ButtonInfo[] { // Visual Mods [11]
                new ButtonInfo { buttonText = "Exit Visual Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Morning Time", method =() => Fun.MorningTime(), toolTip = "Makes time day."},
                new ButtonInfo { buttonText = "Day Time", method =() => Fun.DayTime(), toolTip = "Makes time day."},
                new ButtonInfo { buttonText = "Evening Time", method =() => Fun.EveningTime(), toolTip = "Makes time evening."},
                new ButtonInfo { buttonText = "Night Time", method =() => Fun.NightTime(), toolTip = "Makes time night."},

                new ButtonInfo { buttonText = "Custom Skybox Color", enableMethod =() => Visuals.DoCustomSkyboxColor(), method =() => Visuals.CustomSkyboxColor(), disableMethod =() => Visuals.UnCustomSkyboxColor(), toolTip = "Changes the skybox color to match the menu."},

                new ButtonInfo { buttonText = "Rainy Weather", method =() => Fun.Rain(), toolTip = "Forces the weather to rain."},
                new ButtonInfo { buttonText = "Regular Weather", method =() => Fun.NoRain(), toolTip = "Forces the weather to stop raining."},

                new ButtonInfo { buttonText = "Green Screen", method =() => Visuals.GreenScreen(), toolTip = "Puts a green screen in city." },
                new ButtonInfo { buttonText = "Blue Screen", method =() => Visuals.BlueScreen(), toolTip = "Puts a blue screen in city." },
                new ButtonInfo { buttonText = "Red Screen", method =() => Visuals.RedScreen(), toolTip = "Puts a red screen in city." },

                new ButtonInfo { buttonText = "Info Watch", enableMethod =() => Visuals.WatchOn(), method =() => Visuals.WatchStep(), disableMethod =() => Visuals.WatchOff(), toolTip = "Puts a watch on your hand that tells you the time and your FPS."},

                new ButtonInfo { buttonText = "FPS Boost", enableMethod =() => Important.EnableFPSBoost(), disableMethod =() => Important.DisableFPSBoost(), toolTip = "Makes everything low quality in an attempt to boost your FPS."},

                new ButtonInfo { buttonText = "Fake Unban Self", method =() => Visuals.FakeUnbanSelf(), isTogglable = false, toolTip = "Makes it appear as if you're not banned." },

                new ButtonInfo { buttonText = "Fix Rig Colors", method =() => Visuals.FixRigColors(), toolTip = "Fixes a steam bug where other players' color would be wrong between servers."},
                new ButtonInfo { buttonText = "Disable Rig Lerping", method =() => Visuals.NoSmoothRigs(), disableMethod =() => Visuals.ReSmoothRigs(), toolTip = "Disables lerping on rigs."},
                new ButtonInfo { buttonText = "Remove Leaves", enableMethod =() => Visuals.EnableRemoveLeaves(), disableMethod =() => Visuals.DisableRemoveLeaves(), toolTip = "Removes leaves on trees, good for branching."},
                new ButtonInfo { buttonText = "Remove Cosmetics", enableMethod =() => Visuals.DisableCosmetics(), disableMethod =() => Visuals.EnableCosmetics(), toolTip = "Locally toggles off your cosmetics, so you can wear stuff like the eyepatch or something."},

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

                new ButtonInfo { buttonText = "Show Pointers", method =() => Visuals.ShowButtonColliders(), toolTip = "Shows dots near your hands, such as when you open the menu."},
            },

            new ButtonInfo[] { // Fun Mods [12]
                new ButtonInfo { buttonText = "Exit Fun Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

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

                new ButtonInfo { buttonText = "Water Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.WaterSplashHands(), toolTip = "Splashes water when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Water Splash Aura", method =() => Fun.WaterSplashAura(), toolTip = "Splashes water around you at random positions."},
                new ButtonInfo { buttonText = "Orbit Water Splash", method =() => Fun.OrbitWaterSplash(), toolTip = "Splashes water orbitally around you."},
                new ButtonInfo { buttonText = "Water Splash Gun", method =() => Fun.WaterSplashGun(), toolTip = "Splashes water wherever your hand desires."},

                new ButtonInfo { buttonText = "Boop", method =() => Fun.Boop(), toolTip = "Makes a pop sound when you touch someone's nose."},
                new ButtonInfo { buttonText = "Slap", method =() => Fun.Slap(), toolTip = "Makes a bong sound when you hit someone's face."},

                new ButtonInfo { buttonText = "Get Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GetHoneyComb(), toolTip = "Gives you a party bracelet without needing to be in a party."},
                new ButtonInfo { buttonText = "Spam Bracelet <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.HoneycombSpam(), toolTip = "Spams the party bracelet on and off."},
                new ButtonInfo { buttonText = "Remove Bracelet", method =() => Fun.RemoveBracelet(), isTogglable = false, toolTip = "Disables the party bracelet. This does not kick you from the party."},

                new ButtonInfo { buttonText = "Fast Gliders", enableMethod =() => Fun.FastGliders(), disableMethod =() => Fun.FixGliderSpeed(), toolTip = "Makes the gliders fast."},
                new ButtonInfo { buttonText = "Slow Gliders", enableMethod =() => Fun.SlowGliders(), disableMethod =() => Fun.FixGliderSpeed(), toolTip = "Makes the gliders slow."},

                new ButtonInfo { buttonText = "Glider Blind Gun", method =() => Overpowered.GliderBlindGun(), toolTip = "Moves all of the gliders to whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Glider Blind All", method =() => Overpowered.GliderBlindAll(), toolTip = "Moves all of the gliders to everyone's faces." },

                new ButtonInfo { buttonText = "No Respawn Bug", enableMethod =() => Fun.NoRespawnBug(), disableMethod =() => Fun.PleaseRespawnBug(), toolTip = "Doesn't respawn the bug if it goes too far outside the bounds of forest."},
                new ButtonInfo { buttonText = "No Respawn Bat", enableMethod =() => Fun.NoRespawnBat(), disableMethod =() => Fun.PleaseRespawnBat(), toolTip = "Doesn't respawn the bat if it goes too far outside the bounds of caves."},
                new ButtonInfo { buttonText = "No Respawn Gliders", enableMethod =() => Fun.NoRespawnGliders(), disableMethod =() => Fun.PleaseRespawnGliders(), toolTip = "Doesn't respawn gliders that go too far outside the bounds of clouds."},

                new ButtonInfo { buttonText = "Break Bug", enableMethod =() => Fun.BreakBug(), disableMethod =() => Fun.FixBug(), toolTip = "Makes the bug ungrabbable."},
                new ButtonInfo { buttonText = "Break Bat", enableMethod =() => Fun.BreakBat(), disableMethod =() => Fun.FixBat(), toolTip = "Makes the bat ungrabbable."},

                new ButtonInfo { buttonText = "Grab Bug <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBug(), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Bat <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBat(), toolTip = "Forces the bat into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Beach Ball <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBeachBall(), toolTip = "Forces the beach ball into your hand when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Grab Balloons <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabBalloons(), toolTip = "Puts every single balloon cosmetic in your hand." },
                new ButtonInfo { buttonText = "Grab Gliders <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.GrabGliders(), toolTip = "Forces the bug into your hand when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Bug Gun", method =() => Fun.BugGun(), toolTip = "Moves the bug to wherever your hand desires." },
                new ButtonInfo { buttonText = "Bat Gun", method =() => Fun.BatGun(), toolTip = "Moves the bat to wherever your hand desires." },
                new ButtonInfo { buttonText = "Beach Ball Gun", method =() => Fun.BeachBallGun(), toolTip = "Moves the beach ball to wherever your hand desires." },
                new ButtonInfo { buttonText = "Balloon Gun", method =() => Fun.BalloonGun(), toolTip = "Moves every single balloon cosmetic to wherever your hand desires." },
                new ButtonInfo { buttonText = "Glider Gun", method =() => Fun.GliderGun(), toolTip = "Moves the gliders to wherever your hand desires." },

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

                new ButtonInfo { buttonText = "Unacid Self", method =() => Fun.UnacidSelf(), isTogglable = false, toolTip = "Unturns you into acid." },
                new ButtonInfo { buttonText = "Unacid Gun", method =() => Fun.UnacidGun(), toolTip = "Unturns whoever your hand desires into acid." },
                new ButtonInfo { buttonText = "Unacid All", method =() => Fun.UnacidAll(), isTogglable = false, toolTip = "Unturns everyone into acid." },

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
                new ButtonInfo { buttonText = "Impossible Color <color=grey>[</color><color=red>Stump</color><color=grey>]</color>", method =() => Fun.NegativeColor(), isTogglable = false, toolTip = "Sets your colors to above the integer limit." },
                
                new ButtonInfo { buttonText = "Become \"goldentrophy\"", method =() => Fun.BecomeGoldentrophy(), isTogglable = false, toolTip = "Sets your name to \"goldentrophy\" and color to orange." },
                new ButtonInfo { buttonText = "Become \"PBBV\"", method =() => Fun.BecomePBBV(), isTogglable = false, toolTip = "Sets your name to \"PBBV\" and color to sky blue." },
                new ButtonInfo { buttonText = "Become \"J3VU\"", method =() => Fun.BecomeJ3VU(), isTogglable = false, toolTip = "Sets your name to \"J3VU\" and color to green." },
                new ButtonInfo { buttonText = "Become \"ECHO\"", method =() => Fun.BecomeECHO(), isTogglable = false, toolTip = "Sets your name to \"ECHO\" and color to salmon." },
                new ButtonInfo { buttonText = "Become \"DAISY09\"", method =() => Fun.BecomeDAISY09(), isTogglable = false, toolTip = "Sets your name to \"DAISY09\" and color to a light pink." },
                
                new ButtonInfo { buttonText = "Become Hidden on Leaderboard", method =() => Fun.BecomeHiddenOnLeaderboard(), isTogglable = false, toolTip = "Sets your name to nothing and your color to a dark red, matching the leaderboard." },
                new ButtonInfo { buttonText = "Copy Identity Gun", method =() => Fun.CopyIdentityGun(), toolTip = "Steals the identity of whoever your hand desires." },

                new ButtonInfo { buttonText = "Change Accessories", method =() => Fun.ChangeAccessories(), toolTip = "Use your grips to change what hat you're wearing." },
                new ButtonInfo { buttonText = "Spaz Accessories", method =() => Fun.SpazAccessories(), toolTip = "Spazzes your accessories out." },
                new ButtonInfo { buttonText = "Custom Sound on Join", enableMethod =() => Fun.EnableCustomSoundOnJoin(), disableMethod =() => Fun.DisableCustomSoundOnJoin(), toolTip = "Writes to a file when someone joins or leaves. You must use the python script in the Discord for this to work." },

                new ButtonInfo { buttonText = "Fast Ropes", enableMethod =() => Canyons.FastRopes(), disableMethod =() => Canyons.RegularRopes(), toolTip = "Makes the ropes really fast." },

                new ButtonInfo { buttonText = "Copy ID Gun", method =() => Miscellaneous.CopyIDGun(), toolTip = "Copies the player ID of whoever your hand desires to the clipboard." },
                new ButtonInfo { buttonText = "Grab Player Info", method =() => Miscellaneous.GrabPlayerInfo(), isTogglable = false, toolTip = "Saves every player's name, color, and player ID as a text file and opens it." },
            },

            new ButtonInfo[] { // Spam Mods [13]
                new ButtonInfo { buttonText = "Exit Spam Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Sound Mods", method =() => Settings.EnableSoundSpam(), isTogglable = false, toolTip = "Opens the sound mods."},
                new ButtonInfo { buttonText = "Projectile Mods", method =() => Settings.EnableProjectileSpam(), isTogglable = false, toolTip = "Opens the projectile mods."},
            },

            new ButtonInfo[] { // Sound Spam Mods [14]
                new ButtonInfo { buttonText = "Exit Sound Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Returns you back to the sound page."},

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
                new ButtonInfo { buttonText = "Exit Projectile Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Returns you back to the projectile page."},

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

                new ButtonInfo { buttonText = "Float Self", method =() => Basement.FloatSelf(), toolTip = "Makes you float in the air." },
                new ButtonInfo { buttonText = "Float Gun", method =() => Basement.FloatGun(), toolTip = "Makes whoever your hand desires float in the air." },

                new ButtonInfo { buttonText = "Freeze Self", method =() => Basement.FreezeSelf(), toolTip = "Makes you stop moving." },
                new ButtonInfo { buttonText = "Freeze Gun", method =() => Basement.FreezeGun(), toolTip = "Makes whoever your hand desires stop moving." },

                new ButtonInfo { buttonText = "Acid Self", method =() => Overpowered.AcidSelf(), isTogglable = false, toolTip = "Turns you into acid." },
                new ButtonInfo { buttonText = "Acid Gun", method =() => Overpowered.AcidGun(), toolTip = "Turns whoever your hand desires into acid." },
                new ButtonInfo { buttonText = "Acid All", method =() => Overpowered.AcidAll(), isTogglable = false, toolTip = "Turns everyone into acid." },

                new ButtonInfo { buttonText = "Anti Tag", method =() => Advantages.AntiTag(), toolTip = "Removes you from the list of tagged players when tagged."},
                new ButtonInfo { buttonText = "Untag Self", method =() => Advantages.UntagSelf(), isTogglable = false, toolTip = "Removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Untag Gun", method =() => Advantages.UntagGun(), toolTip = "Untags whoever your hand desires."},
                new ButtonInfo { buttonText = "Untag All", method =() => Advantages.UntagAll(), isTogglable = false, toolTip = "Removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Spam Tag Self", method =() => Advantages.SpamTagSelf(), toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag Gun", method =() => Advantages.SpamTagGun(), toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag All", method =() => Advantages.SpamTagAll(), toolTip = "Adds and removes everyone from the list of tagged players."},

                new ButtonInfo { buttonText = "Infection to Tag", method =() => Overpowered.InfectionToTag(), isTogglable = false, toolTip = "Turns the game into tag instead of infection." },
                new ButtonInfo { buttonText = "Tag to Infection", method =() => Overpowered.TagToInfection(), isTogglable = false, toolTip = "Turns the game into infection instead of tag." },

                new ButtonInfo { buttonText = "Tag Lag", enableMethod =() => Advantages.TagLag(), disableMethod =() => Advantages.NahTagLag(), toolTip = "Forces tag lag in the lobby, letting no one get tagged."},

                new ButtonInfo { buttonText = "Slow Monsters", enableMethod =() => Basement.SlowMonsters(), disableMethod =() => Basement.FixMonsters(), toolTip = "Slows down the basement monsters." },
                new ButtonInfo { buttonText = "Fast Monsters", enableMethod =() => Basement.FastMonsters(), disableMethod =() => Basement.FixMonsters(), toolTip = "Speeds up the basement monsters." },
                
                new ButtonInfo { buttonText = "Grab Monsters <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Basement.GrabMonsters(), toolTip = "Puts the basement monsters in your hand." },
                new ButtonInfo { buttonText = "Monster Gun", method =() => Basement.MonsterGun(), toolTip = "Moves the basement monsters to wherever your hand desires." },
                new ButtonInfo { buttonText = "Spaz Monsters", method =() => Basement.SpazMonsters(), toolTip = "Gives the basement monsters a seizure." },
                new ButtonInfo { buttonText = "Orbit Monsters", method =() => Basement.OrbitMonsters(), toolTip = "Orbits the basement monsters around you." },
                new ButtonInfo { buttonText = "Destroy Monsters", method =() => Basement.DestroyMonsters(), isTogglable = false, toolTip = "Sends the basement monsters to hell." },

                new ButtonInfo { buttonText = "Bonk Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BonkSoundSpam(), toolTip = "Plays the bonk sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CountSoundSpam(), toolTip = "Plays the count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlCountSoundSpam(), toolTip = "Plays the brawl count sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Brawl Start Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlStartSoundSpam(), toolTip = "Plays the brawl start sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Tag Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.TagSoundSpam(), toolTip = "Plays the tag sound when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Round End Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RoundEndSoundSpam(), toolTip = "Plays the round end sound when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Battle Start Game", method =() => Battle.BattleStartGame(), isTogglable = false, toolTip = "Starts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle End Game", method =() => Battle.BattleEndGame(), isTogglable = false, toolTip = "Ends the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Game", method =() => Battle.BattleRestartGame(), isTogglable = false, toolTip = "Restarts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Spam", method =() => Battle.BattleRestartGame(), toolTip = "Spam starts and ends the game. Requires master." },
                
                new ButtonInfo { buttonText = "Battle Balloon Spam Self", method =() => Battle.BattleBalloonSpamSelf(), toolTip = "Spam pops and unpops your balloons. Requires master." },
                new ButtonInfo { buttonText = "Battle Balloon Spam All", method =() => Battle.BattleBalloonSpam(), toolTip = "Spam pops and unpops everyone's balloons. Requires master." },
                
                new ButtonInfo { buttonText = "Battle Kill Gun", method =() => Battle.BattleKillGun(), toolTip = "Kills whoever your hand desires. Requires master." },
                new ButtonInfo { buttonText = "Battle Kill Self", method =() => Battle.BattleKillSelf(), isTogglable = false, toolTip = "Kills yourself. Requires master." },
                new ButtonInfo { buttonText = "Battle Kill All", method =() => Battle.BattleKillAll(), isTogglable = false, toolTip = "Kills everyone. Requires master." },
                
                new ButtonInfo { buttonText = "Battle Revive Gun", method =() => Battle.BattleReviveGun(), toolTip = "Revives whoever your hand desires. Requires master." },
                new ButtonInfo { buttonText = "Battle Revive Self", method =() => Battle.BattleReviveSelf(), isTogglable = false, toolTip = "Revives yourself. Requires master." },
                new ButtonInfo { buttonText = "Battle Revive All", method =() => Battle.BattleReviveAll(), isTogglable = false, toolTip = "Revives everyone. Requires master." },

                new ButtonInfo { buttonText = "Battle God Mode", method =() => Battle.BattleGodMode(), toolTip = "Gives you god mode in brawl. Requires master." },

                new ButtonInfo { buttonText = "Slow Gun", method =() => Overpowered.SlowGun(), toolTip = "Forces tag freeze on whoever your hand desires." },
                new ButtonInfo { buttonText = "Slow All", method =() => Overpowered.SlowAll(), toolTip = "Forces tag freeze on everyone in the the room." },

                new ButtonInfo { buttonText = "Vibrate Gun", method =() => Overpowered.VibrateGun(), toolTip = "Makes whoever your hand desires' controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate All", method =() => Overpowered.VibrateAll(), toolTip = "Makes everyone in the the room's controllers vibrate." },
            },

            new ButtonInfo[] { // Overpowered Mods [17]
                new ButtonInfo { buttonText = "Exit Overpowered Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Anti Ban", overlapText = "Anti Ban <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>", method =() => Overpowered.AntiBan(), toolTip = "Prevents you from getting banned. This mod is very experimental, if you get banned, I take ZERO responsibility."},
                new ButtonInfo { buttonText = "Anti Ban Check", method =() => Overpowered.AntiBanCheck(), isTogglable = false, toolTip = "Tests if the the room is modded or not."},

                new ButtonInfo { buttonText = "Set Master", overlapText = "Set Master <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>", method =() => Overpowered.FastMaster(), isTogglable = false, toolTip = "Sets you as master client."},
                new ButtonInfo { buttonText = "Set Master Gun", overlapText = "Set Master Gun <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>", method =() => Overpowered.SetMasterGun(), toolTip = "Sets whoever your hand desires as master client."},
                new ButtonInfo { buttonText = "Auto Set Master", overlapText = "Auto Set Master <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>", method =() => Experimental.AutoSetMaster(), toolTip = "Sets you as master client when in modded lobbies or when using the anti ban."},

                new ButtonInfo { buttonText = "Spawn Second Look", method =() => Overpowered.SpawnSecondLook(), isTogglable = false, toolTip = "Spawns the ghost in the rotational map." },
                new ButtonInfo { buttonText = "Anger Second Look", method =() => Overpowered.AngerSecondLook(), isTogglable = false, toolTip = "Makes the ghost in the rotational map chase after people." },
                new ButtonInfo { buttonText = "Cancel Second Look", method =() => Overpowered.ThrowSecondLook(), isTogglable = false, toolTip = "Makes the ghost in the rotational map throw whoever it's holding." },
                new ButtonInfo { buttonText = "Spaz Second Look", method =() => Overpowered.SpazSecondLook(), toolTip = "Makes the ghost in the rotational map automatically try chasing people and throwing things." },

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

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Lag</color><color=grey>]</color>", method =() => Safety.AntiReportLag(), toolTip = "Lags whoever comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>", method =() => Safety.AntiReportCrash(), toolTip = "Crashes whoever comes near your report button."},

                new ButtonInfo { buttonText = "Change Name Gun", method =() => Experimental.ChangeNameGun(), toolTip = "Changes whoever your hand desires' name to your name. Credits to kman for creating the original method." },
                new ButtonInfo { buttonText = "Change Name All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Experimental.ChangeNameAll(), toolTip = "Changes everyone's name to your name. Credits to kman for creating the original method." },

                new ButtonInfo { buttonText = "Blind Gun", method =() => Overpowered.BlindGun(), toolTip = "Spawns a bunch of black water balloons in front of whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Blind All", method =() => Overpowered.BlindAll(), toolTip = "Spawns a bunch of black water balloons in front of everyone's faces." },

                new ButtonInfo { buttonText = "Destroy Gun", method =() => Overpowered.DestroyGun(), toolTip = "Makes new players not see whoever your hand desires." },
                new ButtonInfo { buttonText = "Destroy All", method =() => Overpowered.DestroyAll(), isTogglable = false, toolTip = "Every player that joins after you will not be able to see anyone." },

                new ButtonInfo { buttonText = "Leave Party", method =() => Fun.LeaveParty(), isTogglable = false, toolTip = "Leaves the party, incase you can't pull off the string." },

                new ButtonInfo { buttonText = "Kick All in Party", method =() => Fun.KickAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Ban All in Party", method =() => Fun.BanAllInParty(), isTogglable = false, toolTip = "Sends everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Auto Party Kick", method =() => Fun.AutoPartyKick(), toolTip = "When you party, you will automatically send everyone in your party to a random room." },
                new ButtonInfo { buttonText = "Auto Party Ban", method =() => Fun.AutoPartyBan(), toolTip = "When you party, you will automatically send everyone in your party to a bannable code." },

                new ButtonInfo { buttonText = "Kick Gun <color=grey>[</color><color=red>Stump</color><color=grey>]</color> <color=grey>[</color><color=red>Private</color><color=grey>]</color>", method =() => Overpowered.KickGun(), toolTip = "Kicks whoever your hand desires to a random public." },
                new ButtonInfo { buttonText = "Kick All <color=grey>[</color><color=red>Stump</color><color=grey>]</color> <color=grey>[</color><color=red>Private</color><color=grey>]</color>", method =() => Overpowered.KickAll(), isTogglable = false, toolTip = "Kicks everyone inside of stump to a random public." },

                new ButtonInfo { buttonText = "Break Audio Gun", method =() => Overpowered.BreakAudioGun(), toolTip = "Attempts to break the audio of whoever your hand desires." },
                new ButtonInfo { buttonText = "Break Audio All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BreakAudioAll(), toolTip = "Attempts to breaks everyone's audio when holding trigger." },
            },

            new ButtonInfo[] { // Experimental Mods [18]
                new ButtonInfo { buttonText = "Exit Experimental Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Disable Auto Anti Ban", toolTip = "Disables the anti ban from automatically turning on."},

                new ButtonInfo { buttonText = "Experimental RPC Protection", toolTip = "Uses an experimental method of protecting your RPCs."},
                new ButtonInfo { buttonText = "Anti RPC Ban", method =() => Experimental.AntiRPCBan(), isTogglable = false, toolTip = "An experimental anti RPC ban, not letting you get banned for sending RPCs."},

                new ButtonInfo { buttonText = "Get Sound Data", method =() => Miscellaneous.DumpSoundData(), isTogglable = false, toolTip = "Dumps the hand tap sounds to a file."},
                new ButtonInfo { buttonText = "Get Cosmetic Data", method =() => Miscellaneous.DumpCosmeticData(), isTogglable = false, toolTip = "Dumps the cosmetics and their data to a file."},
                new ButtonInfo { buttonText = "Get RPC Data", method =() => Miscellaneous.DumpRPCData(), isTogglable = false, toolTip = "Dumps the data of every RPC to a file."},

                new ButtonInfo { buttonText = "Better FPS Boost", enableMethod =() => Experimental.BetterFPSBoost(), toolTip = "Makes everything one color, boosting your FPS."},

                new ButtonInfo { buttonText = "Low Quality Microphone", enableMethod =() => Fun.LowQualityMicrophone(), disableMethod =() => Fun.HighQualityMicrophone(), toolTip = "Makes your microphone have really bad quality."},
                new ButtonInfo { buttonText = "Loud Microphone", enableMethod =() => Fun.LoudMicrophone(), disableMethod =() => Fun.NotLoudMicrophone(), toolTip = "Makes your microphone really loud."},
                new ButtonInfo { buttonText = "Reload Microphone", method =() => Fun.ReloadMicrophone(), isTogglable = false,  toolTip = "Reloads / fixes your microphone."},

                new ButtonInfo { buttonText = "Wrist Menu", overlapText = "Watch Menu <color=grey>[</color><color=red>Legacy</color><color=grey>]</color>", enableMethod =() => Settings.WristThingOn(), disableMethod =() => Settings.WristThingOff(), toolTip = "Makes the menu like a weird wrist watch, click your hand to open it."},

                new ButtonInfo { buttonText = "Canyons Rope Control", method =() => Canyons.CanyonsRopeControl(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Moves every rope based off of your joystick." },

                new ButtonInfo { buttonText = "Lag Gun <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color>", method =() => Overpowered.LagGun(), toolTip = "Lags whoever your hand desires." },
                new ButtonInfo { buttonText = "Lag All <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color> <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.LagAll(), toolTip = "Lags everyone when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Crash Gun <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color>", method =() => Overpowered.CrashGun(), toolTip = "Crashes whoever your hand desires." },
                new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=purple>Experimental</color><color=grey>]</color> <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes everyone when holding <color=green>trigger</color>." },

                new ButtonInfo { buttonText = "Spaz Targets", method =() => Overpowered.SpazTargets(), toolTip = "Spazzes out the targets." },
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
            },

            new ButtonInfo[] { // Advantage (in Settings) [21]
                new ButtonInfo { buttonText = "Exit Advantage Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},

                new ButtonInfo { buttonText = "ctaRange", overlapText = "Change Tag Aura Distance <color=grey>[</color><color=green>Normal</color><color=grey>]</color>", method =() => Advantages.ChangeTagAuraRange(), isTogglable = false, toolTip = "Changes the range of the tag aura."},
            },

            new ButtonInfo[] { // Visual (in Settings) [22]
                new ButtonInfo { buttonText = "Exit Visual Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},

                new ButtonInfo { buttonText = "Follow Menu Theme", toolTip = "Makes visual mods match the theme of the menu, rather than the color of the player."},
                new ButtonInfo { buttonText = "Transparent Theme", toolTip = "Makes visual mods transparent."},
            },

            new ButtonInfo[] { // Admin Mods (owner only) [23]
                new ButtonInfo { buttonText = "Exit Admin Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Kick All Using", enableMethod =() => Experimental.KickAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Kicks everyone using the menu."},
                new ButtonInfo { buttonText = "Fly All Using", enableMethod =() => Experimental.FlyAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Sends everyone using the menu flying away upwards."},
                new ButtonInfo { buttonText = "Become Goldentrophy All Using", enableMethod =() => Experimental.BecomeAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Makes everyone using the menu become \"goldentrophy\"."},
                new ButtonInfo { buttonText = "Bring All Using", enableMethod =() => Experimental.BringAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Brings everyone using the menu to you."},
                new ButtonInfo { buttonText = "Bring Hand All Using", enableMethod =() => Experimental.BringHandAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Brings everyone using the menu to your hand."},
                new ButtonInfo { buttonText = "Bring Head All Using", enableMethod =() => Experimental.BringHeadAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Brings everyone using the menu to your head."},
                new ButtonInfo { buttonText = "Orbit All Using", enableMethod =() => Experimental.OrbitAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Makes everyone using the menu orbit you."},
                new ButtonInfo { buttonText = "Copy All Using", enableMethod =() => Experimental.CopyAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Makes everyone using the menu copy you."},
                new ButtonInfo { buttonText = "Tag All Using", enableMethod =() => Experimental.TagAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Enables Tag All for everyone using the menu."},
                new ButtonInfo { buttonText = "Spam Notifications All Using", enableMethod =() => Experimental.SpamNotifsAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Spams notifications for everyone using the menu."},
                new ButtonInfo { buttonText = "Update Warning All Using", enableMethod =() => Experimental.UpdateWarningAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Shows everyone using the menu to update it."},
                new ButtonInfo { buttonText = "Remove Menu All Using", enableMethod =() => Experimental.NoMenuAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Disables the menu for everyone who's using it."},
                new ButtonInfo { buttonText = "Disable Mods All Using", enableMethod =() => Experimental.NoModsAllUsing(), disableMethod =() => Experimental.FixName(), toolTip = "Disables all mods for everyone using the menu."},
            },

            new ButtonInfo[] { // Enabled Mods [24]
                new ButtonInfo { buttonText = "Exit Enabled Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Internal Mods (hidden from user) [25]
                new ButtonInfo { buttonText = "Search", method =() => Settings.Search(), isTogglable = false, toolTip = "Lets you search for specific mods."},
                new ButtonInfo { buttonText = "Global Return", method =() => Settings.GlobalReturn(), isTogglable = false, toolTip = "Returns you to the main page."}
            }
        };
    }
}

// These are not on the menu
// new ButtonInfo { buttonText = "Crash Amount", overlapText = "Crash Amount <color=grey>[</color><color=green>2</color><color=grey>]</color>", method =() => Settings.CrashAmount(), isTogglable = false, toolTip = "Changes the amount of projectiles the crash mods send."},
//new ButtonInfo { buttonText = "Projectile Gun", method =() => Projectiles.ProjectileGun(), toolTip = "Acts like the projectile spam, but the projectiles only show up for you and whoever your hand desires." },

//new ButtonInfo { buttonText = "Crash Gun", method =() => Overpowered.CrashGun(), toolTip = "Crashes or lags whoever your hand desires." },
//new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes every quest player, and lags/crashes every steam player when holding <color=green>trigger</color>" },
//new ButtonInfo { buttonText = "Random Color Snowballs", enableMethod =() => Projectiles.RandomColorSnowballs(), disableMethod =() => Projectiles.NoRandomColorSnowballs(), toolTip = "Makes your snowballs random colors." },
//new ButtonInfo { buttonText = "Black Snowballs", enableMethod =() => Projectiles.BlackSnowballs(), disableMethod =() => Projectiles.FixBlackSnowballs(), toolTip = "Makes your snowballs black." },

//new ButtonInfo { buttonText = "Lag Gun", method =() => Overpowered.BubbleGun(), toolTip = "Spawns a massive bubble which lags whoever your hand desires." },
//new ButtonInfo { buttonText = "Lag All", method =() => Overpowered.BubbleAll(), toolTip = "Spawns a massive bubble which lags everyone." },

//new ButtonInfo { buttonText = "Break Bug", method =() => Fun.BreakBug(), isTogglable = false, toolTip = "Breaks the bug." },
//new ButtonInfo { buttonText = "Break Bat", method =() => Fun.BreakBat(), isTogglable = false, toolTip = "Breaks the bat." },

//new ButtonInfo { buttonText = "Steal Bug", method =() => Fun.StealBug(), toolTip = "Steals the bug." },
//new ButtonInfo { buttonText = "Steal Bat", method =() => Fun.StealBat(), toolTip = "Steals the bat." },

//new ButtonInfo { buttonText = "Spaz Voice", method =() => Fun.SpazVoice(), disableMethod =() => Fun.UnspazVoice(), toolTip = "Spazzes your voice out. Only works with monke speak on."},

//new ButtonInfo { buttonText = "Acid Self", method =() => Basement.SodaSelf(), isTogglable = false, toolTip = "Turns you into soda."},
//new ButtonInfo { buttonText = "Unacid Self", method =() => Basement.UnsodaSelf(), isTogglable = false, toolTip = "Turns you not into soda."},

/*new ButtonInfo { buttonText = "Grab Train", method =() => Fun.GrabTrain(), toolTip = "Puts the train in your hand." },
new ButtonInfo { buttonText = "Train Gun", method =() => Fun.TrainGun(), toolTip = "Moves the train to wherever your hand desires." },
new ButtonInfo { buttonText = "Destroy Train", method =() => Fun.DestroyTrain(), isTogglable = false, toolTip = "Sends the train to hell." },
new ButtonInfo { buttonText = "Slow Train", enableMethod =() => Fun.SlowTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train slower." },
new ButtonInfo { buttonText = "Fast Train", enableMethod =() => Fun.FastTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train faster." },*/

//new ButtonInfo { buttonText = "Lava Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.LavaSplashHands(), toolTip = "Splashes lava when holding <color=green>grip</color>."},
//new ButtonInfo { buttonText = "Lava Splash Aura", method =() => Fun.LavaSplashAura(), toolTip = "Splashes lava around you at random positions."},
//new ButtonInfo { buttonText = "Lava Splash Gun", method =() => Fun.LavaSplashGun(), toolTip = "Splashes lava wherever your hand desires."},

/*new ButtonInfo { buttonText = "Force Erupt Lava", method =() => Overpowered.ForceEruptLava(), isTogglable = false, toolTip = "Forcibly rises the lava." },
                new ButtonInfo { buttonText = "Force Drain Lava", method =() => Overpowered.ForceUneruptLava(), isTogglable = false, toolTip = "Forcibly drains the lava." },
                new ButtonInfo { buttonText = "Instant Rise Lava", method =() => Overpowered.ForceRiseLava(), isTogglable = false, toolTip = "Instantly rises the lava." },
                new ButtonInfo { buttonText = "Instant Drain Lava", method =() => Overpowered.ForceDrainLava(), isTogglable = false, toolTip = "Instantly drains the lava." },
                new ButtonInfo { buttonText = "Spaz Lava", method =() => Overpowered.SpazLava(), toolTip = "Spazzes out the lava." },*/

/*new ButtonInfo { buttonText = "Kill Bees", method =() => Fun.KillBees(), isTogglable = false, toolTip = "Sends the bees to hell."},
new ButtonInfo { buttonText = "Anger Bees Self", method =() => Fun.AngerBees(), isTogglable = false, toolTip = "Angers the bees on you."},
new ButtonInfo { buttonText = "Anger Bees Gun", method =() => Fun.AngerBeesGun(), toolTip = "Angers the bees wherever your hand desires."},
new ButtonInfo { buttonText = "Anger Bees All", method =() => Fun.AngerBeesAll(), toolTip = "Angers the bees on everyone."},

new ButtonInfo { buttonText = "Sting Self", method =() => Fun.StingSelf(), isTogglable = false, toolTip = "Makes the bees attack you."},
new ButtonInfo { buttonText = "Sting Gun", method =() => Fun.StingGun(), toolTip = "Makes the bees attack whoever your hand desires."},
new ButtonInfo { buttonText = "Sting All", method =() => Fun.StingAll(), toolTip = "Makes the bees attack everyone."},*/

// new ButtonInfo { buttonText = "Remove Christmas Lights", enableMethod =() => Advantages.EnableRemoveChristmasLights(), disableMethod =() => Advantages.DisableRemoveChristmasLights(), toolTip = "Removes lights, good for walls."},
// new ButtonInfo { buttonText = "Remove Winter Decorations", enableMethod =() => Advantages.EnableRemoveChristmasDecorations(), disableMethod =() => Advantages.DisableRemoveChristmasDecorations(), toolTip = "Removes snowmen and such, good for anyone but very obvious."},
//new ButtonInfo { buttonText = "Projectile Bomb <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.ProjectileBomb(), disableMethod =() => Projectiles.DisableProjectileBomb(), toolTip = "Acts like C4, but instead of launching you, it spawns 5 projectiles in random directions." },

//new ButtonInfo { buttonText = "Gorilla Voice <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Fun.GorillaVoice(), toolTip = "Turns your voice into the gorilla voice when holding <color=green>A</color>."},
//new ButtonInfo { buttonText = "Spam Eat Honey Comb", method =() => Fun.HoneycombSpam(), toolTip = "Spam eats the honey comb when holding <color=green>grip</color>."},
//new ButtonInfo { buttonText = "Remove Cherry Blossoms", enableMethod = () => Visuals.EnableRemoveCherryBlossoms(), disableMethod = () => Visuals.DisableRemoveCherryBlossoms(), toolTip = "Removes cherry blossoms on trees, good for branching." },