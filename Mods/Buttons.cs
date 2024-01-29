using iiMenu.Classes;
using iiMenu.Mods;
using iiMenu.Mods.Spammers;
using UnityEngine.Animations.Rigging;
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
                new ButtonInfo { buttonText = "Projectile Settings", method =() => Settings.EnableProjectileSettings(), isTogglable = false, toolTip = "Opens the settings for the projectiles."}
            },

            new ButtonInfo[] { // Menu (in Settings) [2]
                new ButtonInfo { buttonText = "Exit Menu Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},
                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => Settings.RightHand(), disableMethod =() => Settings.LeftHand(), toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Both Hands", enableMethod =() => Settings.BothHandsOn(), disableMethod =() => Settings.BothHandsOff(), toolTip = "Puts the menu on your both of your hands."},
                new ButtonInfo { buttonText = "Wrist Menu", enableMethod =() => Settings.WristThingOn(), disableMethod =() => Settings.WristThingOff(), toolTip = "Makes the menu like a weird wrist watch, click your hand to open it."},
                new ButtonInfo { buttonText = "Thin Menu", enableMethod =() => Settings.ThinMenuOn(), disableMethod =() => Settings.ThinMenuOff(), toolTip = "Makes the menu thin."},
                new ButtonInfo { buttonText = "Long Menu", enableMethod =() => Settings.LongMenuOn(), disableMethod =() => Settings.LongMenuOff(), toolTip = "Makes the menu long."},
                new ButtonInfo { buttonText = "Disorganize Menu", method =() => Settings.DisorganizeMenu(), isTogglable = false, toolTip = "Disorganizes the entire menu. This cannot be undone."},
                new ButtonInfo { buttonText = "Change Menu Theme", method =() => Settings.ChangeMenuTheme(), isTogglable = false, toolTip = "Changes the theme of the menu."},
                new ButtonInfo { buttonText = "Change Page Type", method =() => Settings.ChangePageType(), isTogglable = false, toolTip = "Changes the type of page buttons."},
                new ButtonInfo { buttonText = "Change Font Type", method =() => Settings.ChangeFontType(), isTogglable = false, toolTip = "Changes the type of font."},
                new ButtonInfo { buttonText = "Serversided Button Sounds", toolTip = "Lets everyone in the lobby hear the buttons."},
                new ButtonInfo { buttonText = "Disable Notifications", enableMethod =() => Settings.DisableNotifications(), disableMethod =() => Settings.EnableNotifications(), toolTip = "Disables all notifications."},
                new ButtonInfo { buttonText = "Menu Presets", method =() => Settings.EnableMenuPresets(), isTogglable = false, toolTip = "Opens the page of presets."},
                new ButtonInfo { buttonText = "Save Preferences", method =() => Settings.SavePreferences(), isTogglable = false, toolTip = "Saves your preferences to a file."},
                new ButtonInfo { buttonText = "Load Preferences", method =() => Settings.LoadPreferences(), isTogglable = false, toolTip = "Loads your preferences from a file."},
                new ButtonInfo { buttonText = "Panic", method =() => Settings.Panic(), isTogglable = false, toolTip = "Disables every single active mod."},
            },

            new ButtonInfo[] { // Room (in Settings) [3]
                new ButtonInfo { buttonText = "Exit Room Settings", method =() => Settings.EnableSettings(), isTogglable = false, toolTip = "Brings you back to the settings menu."},
                
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
                //new ButtonInfo { buttonText = "Change Projectile", overlapText = "Change Projectile <color=grey>[</color><color=green>Slingshot</color><color=grey>]</color>", method =() => Projectiles.ChangeProjectile(), isTogglable = false, toolTip = "Changes the projectile of the projectile spam." },
                //new ButtonInfo { buttonText = "Change Trail", overlapText = "Change Trail <color=grey>[</color><color=green>Regular</color><color=grey>]</color>", method =() => Projectiles.ChangeTrail(), isTogglable = false, toolTip = "Changes the trail of the projectile spam." },
                new ButtonInfo { buttonText = "Random Projectile", toolTip = "Makes the projectiles random." },
                new ButtonInfo { buttonText = "Random Trail", toolTip = "Makes the projectiles have a random trail." },
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

                new ButtonInfo { buttonText = "Disconnect", method =() => Important.Disconnect(), isTogglable = false, toolTip = "Disconnects you from the lobby."},
                new ButtonInfo { buttonText = "Reconnect", method =() => Important.Reconnect(), isTogglable = false, toolTip = "Reconnects you from and to the lobby."},
                new ButtonInfo { buttonText = "Cancel Reconnect", method =() => Important.CancelReconnect(), isTogglable = false, toolTip = "Cancels the reconnection loop."},
                new ButtonInfo { buttonText = "Join Last Room", method =() => Important.JoinLastRoom(), isTogglable = false, toolTip = "Joins the last room you left."},
                new ButtonInfo { buttonText = "Join Random", method =() => Important.JoinRandom(), isTogglable = false, toolTip = "Joins a random public lobby." },
                new ButtonInfo { buttonText = "Create Public", method =() => Important.CreatePublic(), isTogglable = false, toolTip = "Creates a public lobby."},

                new ButtonInfo { buttonText = "Auto Join Room \"RUN\"", method =() => RoomJoiners.AutoJoinRoomRUN(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"RUN\" every 3 seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY\"", method =() => RoomJoiners.AutoJoinRoomDAISY(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY\" every 3 seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"DAISY09\"", method =() => RoomJoiners.AutoJoinRoomDAISY09(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"DAISY09\" every 3 seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"PBBV\"", method =() => RoomJoiners.AutoJoinRoomPBBV(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"PBBV\" every 3 seconds until connected." },
                new ButtonInfo { buttonText = "Auto Join Room \"BOT\"", method =() => RoomJoiners.AutoJoinRoomBOT(), isTogglable = false, toolTip = "Automatically attempts to connect to room \"BOT\" every 3 seconds until connected." },
            },

            new ButtonInfo[] // Important Mods [7]
            {
                new ButtonInfo { buttonText = "Exit Important Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "First Person Camera", enableMethod =() => Important.EnableFPC(), disableMethod =() => Important.DisableFPC(), toolTip = "Makes your camera output what you see in VR."},

                new ButtonInfo { buttonText = "Anti AFK", enableMethod =() => Important.EnableAntiAFK(), disableMethod =() => Important.DisableAntiAFK(), toolTip = "Doesn't let you get kicked for being AFK."},
                new ButtonInfo { buttonText = "Disable Network Triggers", enableMethod =() => Important.DisableNetworkTriggers(), disableMethod =() => Important.EnableNetworkTriggers(), toolTip = "Disables the network triggers, so you can change maps without disconnecting."},
                new ButtonInfo { buttonText = "Disable Quit Box", enableMethod =() => Important.DisableQuitBox(), disableMethod =() => Important.EnableQuitBox(), toolTip = "Disables the box under the map that closes your game."},

                new ButtonInfo { buttonText = "60 Hz / 60 FPS", method =() => Important.ForceLagGame(), toolTip = "Caps your FPS at 60 frames per second."},

                new ButtonInfo { buttonText = "Connect to US", method =() => Important.USServers(), isTogglable = false, toolTip = "Connects you to the United States servers."},
                new ButtonInfo { buttonText = "Connect to US West", method =() => Important.USWServers(), isTogglable = false, toolTip = "Connects you to the western United States servers."},
                new ButtonInfo { buttonText = "Connect to EU", method =() => Important.EUServers(), isTogglable = false, toolTip = "Connects you to the Europe servers."},
            },

            new ButtonInfo[] { // Safety Mods [8]
                new ButtonInfo { buttonText = "Exit Safety Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "No Finger Movement", method =() => Safety.NoFinger(), toolTip = "Makes your fingers not move, so you can use wall walk without getting called out." },

                new ButtonInfo { buttonText = "Flush RPCs", method =() => RPCProtection(), isTogglable = false, toolTip = "Flushes all RPC calls, good after you stop spamming." },
                new ButtonInfo { buttonText = "Anti Crash", enableMethod =() => Safety.AntiCrashEnabled(), disableMethod =() => Safety.AntiCrashDisabled(), toolTip = "Doesn't allow projectiles from other users that are far away."},
                new ButtonInfo { buttonText = "Anti Moderator", method =() => Safety.AntiModerator(), toolTip = "When someone with the stick joins, you get disconnected and their player ID and room code gets saved to a file."},

                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you from the lobby when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Reconnect</color><color=grey>]</color>", method =() => Safety.AntiReportReconnect(), toolTip = "Disconnects and reconnects you from the lobby when anyone comes near your report button."},
                new ButtonInfo { buttonText = "Anti Report <color=grey>[</color><color=green>Join Random</color><color=grey>]</color>", method =() => Safety.AntiReportJoinRandom(), toolTip = "Connects you to a random lobby when anyone comes near your report button."},

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
                new ButtonInfo { buttonText = "Joystick Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.JoystickFly(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Sends your character in whatever direction you are pointing your <color=green>joystick</color> in."},
                new ButtonInfo { buttonText = "Bark Fly <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.BarkFly(), disableMethod =() => Movement.DisableBarkFly(), toolTip = "Acts like the fly that Bark has. Credits to KyleTheScientist."},
                new ButtonInfo { buttonText = "Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Zero Gravity Slingshot Fly <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.ZeroGravitySlingshotFly(), toolTip = "Sends your character forwards, in a more elastic manner without gravity, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "WASD Fly", method =() => Movement.WASDFly(), toolTip = "Moves your rig with <color=green>WASD</color>."},
                new ButtonInfo { buttonText = "Drive <color=grey>[</color><color=green>J</color><color=grey>]</color>", method =() => Movement.Drive(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Lets you drive around in your invisible car. Use the <color=green>joystick</color> to move."},
                new ButtonInfo { buttonText = "Iron Man", method =() => Movement.IronMan(), toolTip = "Turns you into iron man, rotate your hands around to change direction."},
                new ButtonInfo { buttonText = "Spider Man", method =() => Movement.SpiderMan(), disableMethod =() => Movement.DisableSpiderMan(), toolTip = "Turns you into spider man, use your <color=green>grips</color> to shoot webs."},
                new ButtonInfo { buttonText = "Up And Down", method =() => Movement.UpAndDown(), toolTip = "Makes you go up when holding your trigger, and down when holding your <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Auto Funny Run <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.AutoFunnyRun(), toolTip = "Makes your character automatically funny run when folding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Force Tag Freeze", method =() => Movement.ForceTagFreeze(), disableMethod =() => Movement.NoTagFreeze(), toolTip = "Forces tag freeze on your character."},
                new ButtonInfo { buttonText = "No Tag Freeze", method =() => Movement.NoTagFreeze(), toolTip = "Disables tag freeze on your character."},
                new ButtonInfo { buttonText = "Low Gravity", method =() => Movement.LowGravity(), toolTip = "Makes gravity lower on your character."},
                new ButtonInfo { buttonText = "Zero Gravity", method =() => Movement.ZeroGravity(), toolTip = "Disables gravity on your character."},
                new ButtonInfo { buttonText = "High Gravity", method =() => Movement.HighGravity(), toolTip = "Makes gravity higher on your character."},
                new ButtonInfo { buttonText = "Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Weak Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.WeakWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but weaker."},
                new ButtonInfo { buttonText = "Strong Wall Walk <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.StrongWallWalk(), toolTip = "Makes you get brought towards any wall you touch when holding <color=green>grip</color>, but stronger."},
                new ButtonInfo { buttonText = "Teleport to Random", method =() => Movement.TeleportToRandom(), isTogglable = false, toolTip = "Teleports you to a random player."},
                new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), toolTip = "Teleports to wherever your hand desires."},
                new ButtonInfo { buttonText = "Airstrike", method =() => Movement.Airstrike(), toolTip = "Teleports to wherever your hand desires, except farther up, then launches you back down."},
                new ButtonInfo { buttonText = "Checkpoint <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Checkpoint(), disableMethod =() => Movement.DisableCheckpoint(), toolTip = "Place a checkpoint with <color=green>grip</color> and teleport to it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "C4 <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Bomb(), disableMethod =() => Movement.DisableBomb(), toolTip = "Place a C4 with <color=green>grip</color> and detonate it with <color=green>A</color>."},
                new ButtonInfo { buttonText = "Speed Boost", method =() => Movement.SpeedBoost(), /*disableMethod =() => Movement.DisableSpeedBoost(),*/ toolTip = "Changes your speed to whatever you set it to."},
                new ButtonInfo { buttonText = "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Movement.Noclip(), toolTip = "Makes you go through objects when holding <color=green>trigger</color>."},
                new ButtonInfo { buttonText = "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Ghost(), disableMethod =() => Movement.EnableRig(), toolTip = "Keeps your rig still when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", method =() => Movement.Invisible(), disableMethod =() => Movement.DisableInvisible(), toolTip = "Makes you go invisible when holding <color=green>B</color>."},
                new ButtonInfo { buttonText = "Rig Gun", method =() => Movement.RigGun(), toolTip = "Moves your rig to wherever your hand desires."},
                new ButtonInfo { buttonText = "Grab Rig <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Movement.GrabRig(), toolTip = "Lets you grab your rig when holding <color=green>G</color>."},
                new ButtonInfo { buttonText = "Spaz Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", enableMethod =() => Movement.EnableSpazRig(), method =() => Movement.SpazRig(), disableMethod =() => Movement.DisableSpazRig(), toolTip = "Makes every part of your rig spaz out a little bit when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Hands <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.SpazHands(), toolTip = "Makes your hands spaz out everywhere when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Spaz Head", method =() => Movement.SpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head spaz out."},
                new ButtonInfo { buttonText = "Random Spaz Head", method =() => Movement.RandomSpazHead(), disableMethod =() => Fun.FixHead(), toolTip = "Makes your head spaz out for 0 to 1 seconds every 1 to 4 seconds."},
                new ButtonInfo { buttonText = "Laggy Rig", method =() => Movement.LaggyRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig laggy."},
                new ButtonInfo { buttonText = "Update Rig <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.UpdateRig(), disableMethod =() => Movement.EnableRig(), toolTip = "Freezes your rig in place. Whenever you click <color=green>A</color>, your rig will update."},
                new ButtonInfo { buttonText = "Freeze Rig Limbs <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigLimbs(), toolTip = "Makes your hands and head freeze on your rig, but not your body, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Freeze Rig Body <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FreezeRigBody(), toolTip = "Makes your body freeze on your rig, but not your hands and head, when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Auto Dance <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.AutoDance(), toolTip = "Makes you dance when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Helicopter <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.Helicopter(), toolTip = "Turns you into a helicopter when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Ghost Animations", method =() => Movement.GhostAnimations(), disableMethod =() => Movement.EnableRig(), toolTip = "Puts your hands down, and makes you walk when holding <color=green>A</color>. You can also point with <color=green>B</color>"},
                new ButtonInfo { buttonText = "Stare at Nearby", method =() => Movement.StareAtNearby(), toolTip = "Makes you stare at the nearest player."},
                new ButtonInfo { buttonText = "Floating Rig", enableMethod =() => Movement.EnableFloatingRig(), method =() => Movement.FloatingRig(), disableMethod =() => Movement.DisableFloatingRig(), toolTip = "Makes your rig float."},
                new ButtonInfo { buttonText = "Bees", method =() => Movement.Bees(), disableMethod =() => Movement.EnableRig(), toolTip = "Makes your rig teleport to random players, imitating the bees ghost."},
                new ButtonInfo { buttonText = "Size Changer", method =() => Movement.SizeChanger(), enableMethod =() => Movement.EnableSizeChanger(), disableMethod =() => Movement.EnableSizeChanger(), toolTip = "Increase your size by holding <color=green>trigger</color>, and decrease your size by holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Slippery Hands", enableMethod =() => Movement.EnableSlipperyHands(), disableMethod =() => Movement.DisableSlipperyHands(), toolTip = "Makes everything ice, as in extremely slippery."},
                new ButtonInfo { buttonText = "Grippy Hands", enableMethod =() => Movement.EnableGrippyHands(), disableMethod =() => Movement.DisableGrippyHands(), toolTip = "Covers your hands in grip tape, letting you wallrun as high as you want."},
                new ButtonInfo { buttonText = "Slide Control", enableMethod =() => Movement.EnableSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice perfectly."},
                new ButtonInfo { buttonText = "Weak Slide Control", enableMethod =() => Movement.EnableWeakSlideControl(), disableMethod =() => Movement.DisableSlideControl(), toolTip = "Lets you control yourself on ice a little more perfect than before."},
                new ButtonInfo { buttonText = "Punch Mod", method =() => Movement.PunchMod(), toolTip = "Lets people punch you across the map."},
                new ButtonInfo { buttonText = "Solid Players", method =() => Movement.SolidPlayers(), toolTip = "Lets you walk on top of other players."},
                new ButtonInfo { buttonText = "Throw Controllers", method =() => Movement.ThrowControllers(), toolTip = "Lets you throw your controllers with <color=green>X</color> and <color=green>A</color>."},
                new ButtonInfo { buttonText = "Steam Long Arms", enableMethod =() => Movement.EnableSteamLongArms(), disableMethod =() => Movement.DisableSteamLongArms(), toolTip = "Gives you long arms similar to override world scale."},
                new ButtonInfo { buttonText = "Stick Long Arms", method =() => Movement.StickLongArms(), toolTip = "Makes you look like you're using sticks."},
                new ButtonInfo { buttonText = "Multiplied Long Arms", method =() => Movement.MultipliedLongArms(), toolTip = "Gives you a weird version of long arms."},
                new ButtonInfo { buttonText = "Vertical Long Arms", method =() => Movement.VerticalLongArms(), toolTip = "Gives you a version of long arms to help you vertically."},
                new ButtonInfo { buttonText = "Horizontal Long Arms", method =() => Movement.HorizontalLongArms(), toolTip = "Gives you a version of long arms to help you horizontally."},
                new ButtonInfo { buttonText = "Flick Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.FlickJump(), toolTip = "Makes your hand go down really fast when holding <color=green>A</color>."},
                new ButtonInfo { buttonText = "Long Jump <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Movement.LongJump(), toolTip = "Makes you look like you're legitimately long jumping when holding <color=green>A</color>."},
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
                new ButtonInfo { buttonText = "Intercourse Gun", method =() => Movement.IntercourseGun(), toolTip = "Makes you thrust whoever your hand desires, with sounds."}
            },

            new ButtonInfo[] { // Advantage Mods [10]
                new ButtonInfo { buttonText = "Exit Advantage Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Tag Self", method =() => Advantages.TagSelf(), toolTip = "Attempts to tags yourself."},
                new ButtonInfo { buttonText = "Physical Tag Aura", method =() => Advantages.PhysicalTagAura(), toolTip = "Moves your hand into nearby players when tagged."},
                // new ButtonInfo { buttonText = "RPC Tag Aura", method =() => Advantages.RPCTagAura(), toolTip = "Sends the tag RPC when close enough to nearby players."},
                new ButtonInfo { buttonText = "Tag Gun", method =() => Advantages.TagGun(), toolTip = "Tags whoever your hand desires."},
                new ButtonInfo { buttonText = "Flick Tag Gun", method =() => Advantages.FlickTagGun(), toolTip = "Moves your hand to wherever your hand desires in an attempt to tag whoever your hand desires."},
                new ButtonInfo { buttonText = "Tag All", method =() => Advantages.TagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the lobby."},
                new ButtonInfo { buttonText = "Hunt Tag All", method =() => Advantages.HuntTagAll(), disableMethod =() => Movement.EnableRig(), toolTip = "Attempts to tag everyone in the lobby, but for hunt."},
                new ButtonInfo { buttonText = "Tag Bot", method =() => Advantages.TagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, use <color=green>B</color> to turn it off."},
                new ButtonInfo { buttonText = "Hunt Tag Bot", method =() => Advantages.HuntTagBot(), disableMethod =() => Movement.EnableRig(), toolTip = "Automatically tags yourself and everyone else on a loop, but for hunt. Use <color=green>B</color> to turn it off."},
                new ButtonInfo { buttonText = "No Tag on Join", method =() => Advantages.NoTagOnJoin(), disableMethod =() => Advantages.TagOnJoin(), toolTip = "When you join a lobby, you won't be tagged when you join."},
                // new ButtonInfo { buttonText = "Remove Christmas Lights", enableMethod =() => Advantages.EnableRemoveChristmasLights(), disableMethod =() => Advantages.DisableRemoveChristmasLights(), toolTip = "Removes lights, good for walls."},
                new ButtonInfo { buttonText = "Remove Winter Decorations", enableMethod =() => Advantages.EnableRemoveChristmasDecorations(), disableMethod =() => Advantages.DisableRemoveChristmasDecorations(), toolTip = "Removes snowmen and such, good for anyone but very obvious."},
            },

            new ButtonInfo[] { // Visual Mods [11]
                new ButtonInfo { buttonText = "Exit Visual Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Day Time", method =() => Fun.DayTime(), toolTip = "Makes time day."},
                new ButtonInfo { buttonText = "Night Time", method =() => Fun.NightTime(), toolTip = "Makes time night."},

                new ButtonInfo { buttonText = "FPS Boost", enableMethod =() => Important.EnableFPSBoost(), disableMethod =() => Important.DisableFPSBoost(), toolTip = "Makes everything low quality in an attempt to boost your FPS."},

                new ButtonInfo { buttonText = "Fix Rig Colors", method =() => Visuals.FixRigColors(), toolTip = "Fixes a steam bug where other players' color would be wrong between servers."},
                new ButtonInfo { buttonText = "Remove Leaves", enableMethod =() => Visuals.EnableRemoveLeaves(), disableMethod =() => Visuals.DisableRemoveLeaves(), toolTip = "Removes leaves on trees, good for branching."},
                new ButtonInfo { buttonText = "Remove Cosmetics", enableMethod =() => Visuals.DisableCosmetics(), disableMethod =() => Visuals.EnableCosmetics(), toolTip = "Locally toggles off your cosmetics, so you can wear stuff like the eyepatch or something."},

                new ButtonInfo { buttonText = "Casual Tracers", method =() => Visuals.CasualTracers(), toolTip = "Puts tracers on your right hand. Shows untagged when tagged, vice versa."},
                new ButtonInfo { buttonText = "Infection Tracers", method =() => Visuals.InfectionTracers(), toolTip = "Puts tracers on your right hand. Shows everyone."},
                new ButtonInfo { buttonText = "Hunt Tracers", method =() => Visuals.HuntTracers(), toolTip = "Puts tracers on your right hand. Shows your target and who is hunting you."},

                new ButtonInfo { buttonText = "Casual Box ESP", method =() => Visuals.CasualBoxESP(), toolTip = "Acts like infection tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Infection Box ESP", method =() => Visuals.InfectionBoxESP(), toolTip = "Acts like casual tracers color wise, but with boxes."},
                new ButtonInfo { buttonText = "Hunt Box ESP", method =() => Visuals.HuntBoxESP(), toolTip = "Acts like hunt tracers color wise, but with boxes."},

                new ButtonInfo { buttonText = "Casual Bone ESP", method =() => Visuals.CasualBoneESP(), toolTip = "Acts like infection tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Infection Bone ESP", method =() => Visuals.InfectionBoneESP(), toolTip = "Acts like casual tracers color wise, but with bones."},
                new ButtonInfo { buttonText = "Hunt Bone ESP", method =() => Visuals.HuntBoneESP(), toolTip = "Acts like hunt tracers color wise, but with bones."},

                new ButtonInfo { buttonText = "Casual Chams", method =() => Visuals.CasualChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like infection tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Infection Chams", method =() => Visuals.InfectionChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like casual tracers color wise, but lets you see their fur through walls."},
                new ButtonInfo { buttonText = "Hunt Chams", method =() => Visuals.HuntChams(), disableMethod =() => Visuals.DisableChams(), toolTip = "Acts like hunt tracers color wise, but lets you see their fur through walls."},

                new ButtonInfo { buttonText = "Casual Beacons", method =() => Visuals.CasualBeacons(), toolTip = "Acts like infection tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Infection Beacons", method =() => Visuals.InfectionBeacons(), toolTip = "Acts like casual tracers color wise, but it's just a giant line."},
                new ButtonInfo { buttonText = "Hunt Beacons", method =() => Visuals.HuntBeacons(), toolTip = "Acts like hunt tracers color wise, but it's just a giant line."},

                new ButtonInfo { buttonText = "Show Button Colliders", method =() => Visuals.ShowButtonColliders(), toolTip = "Shows dots above your hand, such as when you open the menu."},
            },

            new ButtonInfo[] { // Fun Mods [12]
                new ButtonInfo { buttonText = "Exit Fun Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Upside Down Head", method =() => Fun.UpsideDownHead(), disableMethod =() => Fun.FixHead(), toolTip = "Flips your head upside down."},
                new ButtonInfo { buttonText = "Spin Head X", method =() => Fun.SpinHeadX(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the X axis."},
                new ButtonInfo { buttonText = "Spin Head Y", method =() => Fun.SpinHeadY(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Y axis."},
                new ButtonInfo { buttonText = "Spin Head Z", method =() => Fun.SpinHeadZ(), disableMethod =() => Fun.FixHead(), toolTip = "Spins your head on the Z axis."},

                new ButtonInfo { buttonText = "Flip Hands", method =() => Fun.FlipHands(), toolTip = "Swaps your hands, left is right and right is left."},
                new ButtonInfo { buttonText = "Loud Hand Taps", method =() => Fun.LoudHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really loud."},
                new ButtonInfo { buttonText = "Silent Hand Taps", method =() => Fun.SilentHandTaps(), disableMethod =() => Fun.FixHandTaps(), toolTip = "Makes your hand taps really quiet."},
                new ButtonInfo { buttonText = "Instant Hand Taps", method =() => Fun.EnableInstantHandTaps(), disableMethod =() => Fun.DisableInstantHandTaps(), toolTip = "Removes the hand tap cooldown."},

                new ButtonInfo { buttonText = "Water Splash Hands <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Fun.WaterSplashHands(), toolTip = "Splashes water when holding <color=green>grip</color>."},
                new ButtonInfo { buttonText = "Water Splash Aura", method =() => Fun.WaterSplashAura(), toolTip = "Splashes water around you at random positions."},
                new ButtonInfo { buttonText = "Water Splash Gun", method =() => Fun.WaterSplashGun(), toolTip = "Splashes water wherever your hand desires."},

                new ButtonInfo { buttonText = "Grab Bug", method =() => Fun.GrabBug(), toolTip = "Forces the bug into your hand when holding grip." },
                new ButtonInfo { buttonText = "Grab Bat", method =() => Fun.GrabBat(), toolTip = "Forces the bat into your hand when holding grip." },
                new ButtonInfo { buttonText = "Grab Beach Ball", method =() => Fun.GrabBeachBall(), toolTip = "Forces the beach ball into your hand when holding grip." },

                new ButtonInfo { buttonText = "Bug Gun", method =() => Fun.BugGun(), toolTip = "Moves the bug to wherever your hand desires." },
                new ButtonInfo { buttonText = "Bat Gun", method =() => Fun.BatGun(), toolTip = "Moves the bat to wherever your hand desires." },
                new ButtonInfo { buttonText = "Beach Ball Gun", method =() => Fun.BeachBallGun(), toolTip = "Moves the beach ball to wherever your hand desires." },

                new ButtonInfo { buttonText = "Destroy Bug", method =() => Fun.DestroyBug(), isTogglable = false, toolTip = "Sends the bug to hell." },
                new ButtonInfo { buttonText = "Destroy Bat", method =() => Fun.DestroyBat(), isTogglable = false, toolTip = "Sends the bat to hell." },
                new ButtonInfo { buttonText = "Destroy Beach Ball", method =() => Fun.DestroyBeachBall(), isTogglable = false, toolTip = "Sends the beach ball to hell." },

                new ButtonInfo { buttonText = "Pop All Balloons", method =() => Fun.PopAllBalloons(), isTogglable = false, toolTip = "Pops every single balloon cosmetic." },
                new ButtonInfo { buttonText = "Grab Balloons", method =() => Fun.GrabBalloons(), toolTip = "Puts every single balloon cosmetic in your hand." },
                new ButtonInfo { buttonText = "Balloon Gun", method =() => Fun.BalloonGun(), toolTip = "Moves every single balloon cosmetic to wherever your hand desires." },
                new ButtonInfo { buttonText = "Destroy Balloons", method =() => Fun.DestroyBalloons(), isTogglable = false, toolTip = "Sends every single balloon cosmetic to hell." },

                //new ButtonInfo { buttonText = "Acid Self", method =() => Basement.SodaSelf(), isTogglable = false, toolTip = "Turns you into soda."},
                //new ButtonInfo { buttonText = "Unacid Self", method =() => Basement.UnsodaSelf(), isTogglable = false, toolTip = "Turns you not into soda."},

                /*new ButtonInfo { buttonText = "Grab Train", method =() => Fun.GrabTrain(), toolTip = "Puts the train in your hand." },
                new ButtonInfo { buttonText = "Train Gun", method =() => Fun.TrainGun(), toolTip = "Moves the train to wherever your hand desires." },
                new ButtonInfo { buttonText = "Destroy Train", method =() => Fun.DestroyTrain(), isTogglable = false, toolTip = "Sends the train to hell." },
                new ButtonInfo { buttonText = "Slow Train", enableMethod =() => Fun.SlowTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train slower." },
                new ButtonInfo { buttonText = "Fast Train", enableMethod =() => Fun.FastTrain(), disableMethod =() => Fun.FixTrain(), toolTip = "Makes the train faster." },*/

                new ButtonInfo { buttonText = "Remove Name", method =() => Fun.RemoveName(), isTogglable = false, toolTip = "Sets your name to nothing." },
                new ButtonInfo { buttonText = "Set Name to \"STATUE\"", method =() => Fun.SetNameToSTATUE(), isTogglable = false, toolTip = "Sets your name to \"STATUE\"." },
                new ButtonInfo { buttonText = "Set Name to \"RUN\"", method =() => Fun.SetNameToRUN(), isTogglable = false, toolTip = "Sets your name to \"RUN\"." },
                new ButtonInfo { buttonText = "Set Name to \"BEHINDYOU\"", method =() => Fun.SetNameToBEHINDYOU(), isTogglable = false, toolTip = "Sets your name to \"BEHINDYOU\"." },
                new ButtonInfo { buttonText = "Set Name to \"iiOnTop\"", method =() => Fun.SetNameToiiOnTop(), isTogglable = false, toolTip = "Sets your name to \"iiOnTop\"." },
                new ButtonInfo { buttonText = "PBBV Name Cycle", method =() => Fun.PBBVNameCycle(), toolTip = "Sets your name on a loop to \"PBBV\", \"IS\", and \"HERE\"." },
                new ButtonInfo { buttonText = "J3VU Name Cycle", method =() => Fun.J3VUNameCycle(), toolTip = "Sets your name on a loop to \"J3VU\", \"HAS\", \"BECOME\", and \"HOSTILE\"" },
                new ButtonInfo { buttonText = "Run Rabbit Name Cycle", method =() => Fun.RunRabbitNameCycle(), toolTip = "Sets your name on a loop to \"RUN\" and \"RABBIT\"." },
                new ButtonInfo { buttonText = "Random Name Cycle", method =() => Fun.RandomNameCycle(), toolTip = "Sets your name on a loop to a bunch of random characters." },
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

                new ButtonInfo { buttonText = "Canyons Rope Control", method =() => Canyons.CanyonsRopeControl(), enableMethod =() => Movement.DisableJoystick(), disableMethod =() => Movement.EnableJoystick(), toolTip = "Moves every rope based off of your joystick." },

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

                new ButtonInfo { buttonText = "Bass Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BassSoundSpam(), toolTip = "Plays the loud drum sound when holding grip." },
                new ButtonInfo { buttonText = "Wolf Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.WolfSoundSpam(), toolTip = "Plays the wolf howl when holding grip." },
                new ButtonInfo { buttonText = "Cat Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CatSoundSpam(), toolTip = "Plays the cat meow when holding grip." },
                new ButtonInfo { buttonText = "Turkey Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.TurkeySoundSpam(), toolTip = "Plays the turkey sound when holding grip." },
                new ButtonInfo { buttonText = "Frog Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.FrogSoundSpam(), toolTip = "Plays the frog creak when holding grip." },
                new ButtonInfo { buttonText = "Bee Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BeeSoundSpam(), toolTip = "Plays the bee buzz when holding grip." },
                new ButtonInfo { buttonText = "Squeak Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SqueakSoundSpam(), toolTip = "Plays the squeak sound when holding grip." },
                new ButtonInfo { buttonText = "Random Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RandomSoundSpam(), toolTip = "Plays random sounds when holding grip." },
                new ButtonInfo { buttonText = "Earrape Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.EarrapeSoundSpam(), toolTip = "Plays a high-pitched sound when holding grip." },
                new ButtonInfo { buttonText = "Ding Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.DingSoundSpam(), toolTip = "Plays a ding sound when holding grip." },
                new ButtonInfo { buttonText = "Big Crystal Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BigCrystalSoundSpam(), toolTip = "Plays a long crystal sound when holding grip." },
                new ButtonInfo { buttonText = "Pan Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.PanSoundSpam(), toolTip = "Plays a pan sound when holding grip." },
                new ButtonInfo { buttonText = "AK-47 Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.AK47SoundSpam(), toolTip = "Plays a sound that sounds like an AK-47 when holding grip." },
                new ButtonInfo { buttonText = "Siren Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.SirenSoundSpam(), toolTip = "Plays a siren sound when holding grip." },

                new ButtonInfo { buttonText = "- Sound ID", method =() => Sound.DecreaseSoundID(), isTogglable = false, toolTip = "Lowers the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "+ Sound ID", method =() => Sound.IncreaseSoundID(), isTogglable = false, toolTip = "Increases the Sound ID of the Custom Sound Spam." },
                new ButtonInfo { buttonText = "Custom Sound Spam", overlapText = "Custom Sound Spam <color=grey>[</color><color=green>0</color><color=grey>]</color>", method =() => Sound.CustomSoundSpam(), toolTip = "Plays the selected sound when holding grip." },
            },

            new ButtonInfo[] { // Projectile Spam Mods [15]
                new ButtonInfo { buttonText = "Exit Projectile Mods", method =() => Settings.EnableSpam(), isTogglable = false, toolTip = "Returns you back to the projectile page."},

                new ButtonInfo { buttonText = "Projectile Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.ProjectileSpam(), toolTip = "Spams projectiles when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Give Projectile Spam Gun", method =() => Projectiles.GiveProjectileSpamGun(), toolTip = "Acts like the projectile spam, but you can give it to whoever your hand desires." },
                new ButtonInfo { buttonText = "Projectile Bomb <color=grey>[</color><color=green>A</color><color=grey>]</color>", method =() => Projectiles.ProjectileBomb(), disableMethod =() => Projectiles.DisableProjectileBomb(), toolTip = "Acts like C4, but instead of launching you, it spawns 5 projectiles in random directions." },
                new ButtonInfo { buttonText = "Impact Spam", method =() => Projectiles.ImpactSpam(), toolTip = "Acts like the projectile spam, but uses the impacts instead." },

                new ButtonInfo { buttonText = "Paper Plane Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.PaperPlaneSpam(), toolTip = "Spams the plane cosmetic when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Slingshot Helper", method =() => Projectiles.SlingshotHelper(), toolTip = "Automatically puts the bullet in your right hand." },

                new ButtonInfo { buttonText = "Urine <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Urine(), toolTip = "Makes you pee when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Feces <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Feces(), toolTip = "Makes you poo when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Semen <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Semen(), toolTip = "Makes you ejaculate when holding <color=green>grip</color>." },
                new ButtonInfo { buttonText = "Vomit <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Projectiles.Vomit(), toolTip = "Makes you throw up when holding <color=green>grip</color>." },

                new ButtonInfo { buttonText = "Urine Gun", method =() => Projectiles.UrineGun(), toolTip = "Makes whoever your hand desires pee." },
                new ButtonInfo { buttonText = "Feces Gun", method =() => Projectiles.FecesGun(), toolTip = "Makes whoever your hand desires poo." },
                new ButtonInfo { buttonText = "Semen Gun", method =() => Projectiles.SemenGun(), toolTip = "Makes whoever your hand desires ejaculate." },
                new ButtonInfo { buttonText = "Vomit Gun", method =() => Projectiles.VomitGun(), toolTip = "Makes whoever your hand desires throw up." },

                new ButtonInfo { buttonText = "Serversided Tracers", method =() => Projectiles.ServersidedTracers(), toolTip = "Spams projectiles that move really fast towards players, like tracers." },
            },

            new ButtonInfo[] { // Master Mods [16]
                new ButtonInfo { buttonText = "Exit Master Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Crash Gun <color=grey>[</color><color=purple>Krisp</color><color=grey>]</color>", method =() => Overpowered.BubbleGun(), toolTip = "Spawns a massive bubble which crashes whoever your hand desires." },
                new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=purple>Krisp</color><color=grey>]</color>", method =() => Overpowered.BubbleAll(), toolTip = "Spawns a massive bubble which crashes everyone." },

                new ButtonInfo { buttonText = "Infection Gamemode <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.InfectionGamemode(), isTogglable = false, toolTip = "Sets the gamemode to infection."},
                new ButtonInfo { buttonText = "Casual Gamemode <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.CasualGamemode(), isTogglable = false, toolTip = "Sets the gamemode to casual."},
                new ButtonInfo { buttonText = "Hunt Gamemode <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.HuntGamemode(), isTogglable = false, toolTip = "Sets the gamemode to hunt."},
                new ButtonInfo { buttonText = "Battle Gamemode <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.BattleGamemode(), isTogglable = false, toolTip = "Sets the gamemode to battle."},

                new ButtonInfo { buttonText = "Serversided Network Triggers <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.SSDisableNetworkTriggers(), isTogglable = false, toolTip = "Disables network triggers for everyone."},
                new ButtonInfo { buttonText = "Trap Stump <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Overpowered.TrapStump(), isTogglable = false, toolTip = "Anyone who enters the stump will be kicked."},

                new ButtonInfo { buttonText = "Untag Self", method =() => Advantages.UntagSelf(), isTogglable = false, toolTip = "Removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Untag All", method =() => Advantages.UntagAll(), isTogglable = false, toolTip = "Removes everyone from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag Self", method =() => Advantages.SpamTagSelf(), toolTip = "Adds and removes you from the list of tagged players."},
                new ButtonInfo { buttonText = "Spam Tag All", method =() => Advantages.SpamTagAll(), toolTip = "Adds and removes everyone from the list of tagged players."},

                /*new ButtonInfo { buttonText = "Slow Monsters", enableMethod =() => Basement.SlowMonsters(), disableMethod =() => Basement.FixMonsters(), toolTip = "Slows down the basement monsters." },
                new ButtonInfo { buttonText = "Fast Monsters", enableMethod =() => Basement.FastMonsters(), disableMethod =() => Basement.FixMonsters(), toolTip = "Speeds up the basement monsters." },
                new ButtonInfo { buttonText = "Grab Monsters <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Basement.GrabMonsters(), toolTip = "Puts the basement monsters in your hand." },
                new ButtonInfo { buttonText = "Monster Gun", method =() => Basement.MonsterGun(), toolTip = "Moves the basement monsters to wherever your hand desires." },
                new ButtonInfo { buttonText = "Destroy Monsters", method =() => Basement.DestroyMonsters(), isTogglable = false, toolTip = "Sends the basement monsters to hell." },*/

                new ButtonInfo { buttonText = "Bonk Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BonkSoundSpam(), toolTip = "Plays the bonk sound when holding grip." },
                new ButtonInfo { buttonText = "Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.CountSoundSpam(), toolTip = "Plays the count sound when holding grip." },
                new ButtonInfo { buttonText = "Brawl Count Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlCountSoundSpam(), toolTip = "Plays the brawl count sound when holding grip." },
                new ButtonInfo { buttonText = "Brawl Start Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.BrawlStartSoundSpam(), toolTip = "Plays the brawl start sound when holding grip." },
                new ButtonInfo { buttonText = "Tag Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.TagSoundSpam(), toolTip = "Plays the tag sound when holding grip." },
                new ButtonInfo { buttonText = "Round End Sound Spam <color=grey>[</color><color=green>G</color><color=grey>]</color>", method =() => Sound.RoundEndSoundSpam(), toolTip = "Plays the round end sound when holding grip." },

                new ButtonInfo { buttonText = "Battle Start Game", method =() => Battle.BattleStartGame(), isTogglable = false, toolTip = "Starts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle End Game", method =() => Battle.BattleEndGame(), isTogglable = false, toolTip = "Ends the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Game", method =() => Battle.BattleRestartGame(), isTogglable = false, toolTip = "Restarts the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Restart Spam", method =() => Battle.BattleRestartGame(), toolTip = "Spam starts and ends the game. Requires master." },
                new ButtonInfo { buttonText = "Battle Balloon Spam", method =() => Battle.BattleBalloonSpam(), toolTip = "Spam pops and unpops balloons. Requires master." },

                new ButtonInfo { buttonText = "Slow Gun", method =() => Overpowered.SlowGun(), toolTip = "Forces tag freeze on whoever your hand desires." },
                new ButtonInfo { buttonText = "Slow All", method =() => Overpowered.SlowAll(), toolTip = "Forces tag freeze on everyone in the lobby." },

                new ButtonInfo { buttonText = "Vibrate Gun", method =() => Overpowered.VibrateGun(), toolTip = "Makes whoever your hand desires' controllers vibrate." },
                new ButtonInfo { buttonText = "Vibrate All", method =() => Overpowered.VibrateAll(), toolTip = "Makes everyone in the lobby's controllers vibrate." },
            },

            new ButtonInfo[] { // Overpowered Mods [17]
                new ButtonInfo { buttonText = "Exit Overpowered Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Anti Ban <color=grey>[</color><color=purple>Krisp</color><color=grey>]</color>", method =() => Overpowered.AntiBan(), isTogglable = false, toolTip = "A working anti ban, created by Polar, kman, and Krisp, and leaked by REV." },

                new ButtonInfo { buttonText = "Blind Gun", method =() => Overpowered.BlindGun(), toolTip = "Spawns a bunch of black water balloons in front of whoever your hand desires' faces." },
                new ButtonInfo { buttonText = "Blind All", method =() => Overpowered.BlindAll(), toolTip = "Spawns a bunch of black water balloons in front of everyone's faces." },

                new ButtonInfo { buttonText = "Destroy Gun", method =() => Overpowered.DestroyGun(), toolTip = "Makes new players not see whoever your hand desires." },
                new ButtonInfo { buttonText = "Destroy All", method =() => Overpowered.DestroyAll(), isTogglable = false, toolTip = "Every player that joins after you will not be able to see anyone." },

                new ButtonInfo { buttonText = "Kick Gun <color=grey>[</color><color=red>Stump</color><color=grey>]</color> <color=grey>[</color><color=red>Private</color><color=grey>]</color>", method =() => Overpowered.KickGun(), toolTip = "Kicks whoever your hand desires to a random public." },
                new ButtonInfo { buttonText = "Kick All <color=grey>[</color><color=red>Stump</color><color=grey>]</color> <color=grey>[</color><color=red>Private</color><color=grey>]</color>", method =() => Overpowered.KickAll(), isTogglable = false, toolTip = "Kicks everyone inside of stump to a random public." },

                new ButtonInfo { buttonText = "Break Audio Gun", method =() => Overpowered.BreakAudioGun(), toolTip = "Attempts to break the audio of whoever your hand desires." },
                new ButtonInfo { buttonText = "Break Audio All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.BreakAudioAll(), toolTip = "Attempts to breaks everyone's audio when holding trigger." },
            },

            new ButtonInfo[] { // Experimental Mods [18]
                new ButtonInfo { buttonText = "Exit Experimental Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},

                new ButtonInfo { buttonText = "Experimental RPC Protection", toolTip = "Uses an experimental method of protecting your RPCs. Credits to Kante."},
                new ButtonInfo { buttonText = "Set Master <color=grey>[</color><color=red>Detected</color><color=grey>]</color>", method =() => Experimental.SetMaster(), isTogglable = false, toolTip = "Sets you as master client. Only use this in moddeds."},
            },

            new ButtonInfo[] { // Favorite Mods [19]
                new ButtonInfo { buttonText = "Exit Favorite Mods", method =() => Settings.ReturnToMain(), isTogglable = false, toolTip = "Returns you back to the main page."},
            },

            new ButtonInfo[] { // Menu Presets [20]
                new ButtonInfo { buttonText = "Exit Menu Presets", method =() => Settings.EnableMenuSettings(), isTogglable = false, toolTip = "Returns to the settings for the menu."},

                new ButtonInfo { buttonText = "Legitimate Preset", method =() => Presets.LegitimatePreset(), isTogglable = false, toolTip = "Mod preset using only visual mods, best for public lobbies."},
                new ButtonInfo { buttonText = "Ghost Preset", method =() => Presets.GhostPreset(), isTogglable = false, toolTip = "Mod preset best for ghost trolling."},
                new ButtonInfo { buttonText = "Goldentrophy Preset", method =() => Presets.GoldentrophyPreset(), isTogglable = false, toolTip = "Mod preset that goldentrophy uses."},
            }
        };
    }
}

// new ButtonInfo { buttonText = "Crash Amount", overlapText = "Crash Amount <color=grey>[</color><color=green>2</color><color=grey>]</color>", method =() => Settings.CrashAmount(), isTogglable = false, toolTip = "Changes the amount of projectiles the crash mods send."},
//new ButtonInfo { buttonText = "Projectile Gun", method =() => Projectiles.ProjectileGun(), toolTip = "Acts like the projectile spam, but the projectiles only show up for you and whoever your hand desires." },
//new ButtonInfo { buttonText = "Anti Ban", method =() => Experimental.AntiBan(), isTogglable = false, toolTip = "Enables a probably not working anti ban, created by Cunzaki."},

//new ButtonInfo { buttonText = "Crash Gun", method =() => Overpowered.CrashGun(), toolTip = "Crashes or lags whoever your hand desires." },
//new ButtonInfo { buttonText = "Crash All <color=grey>[</color><color=green>T</color><color=grey>]</color>", method =() => Overpowered.CrashAll(), toolTip = "Crashes every quest player, and lags/crashes every steam player when holding <color=green>trigger</color>" },
//new ButtonInfo { buttonText = "Random Color Snowballs", enableMethod =() => Projectiles.RandomColorSnowballs(), disableMethod =() => Projectiles.NoRandomColorSnowballs(), toolTip = "Makes your snowballs random colors." },
//new ButtonInfo { buttonText = "Black Snowballs", enableMethod =() => Projectiles.BlackSnowballs(), disableMethod =() => Projectiles.FixBlackSnowballs(), toolTip = "Makes your snowballs black." },
//