using ExitGames.Client.Photon;
using iiMenu.Classes;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using static iiMenu.Mods.CustomMaps.Manager;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public class ChimpCombat : CustomMap
    {
        public override long MapID => 5135423;
        public override ButtonInfo[] Buttons => new ButtonInfo[]
        {
            new ButtonInfo { buttonText = "Kill All Players", method =() => KillAll(), toolTip = "Kills everyone in the room."},
            new ButtonInfo { buttonText = "God Mode_", overlapText = "God Mode", enableMethod =() => GodMode(), disableMethod =() => DisableGodMode(), toolTip = "Prevents you from getting killed."},
            new ButtonInfo { buttonText = "No Grenade Cooldown", enableMethod =() => NoGrenadeCooldown(), disableMethod =() => DisableNoGrenadeCooldown(), toolTip = "Disables the cooldown on spawning grenades."},
            new ButtonInfo { buttonText = "No Shoot Cooldown", enableMethod =() => NoShootCooldown(), disableMethod =() => DisableNoShootCooldown(), toolTip = "Disables the cooldown on shooting."},
            new ButtonInfo { buttonText = "Rapid Fire", enableMethod =() => RapidFire(), disableMethod =() => DisableRapidFire(), toolTip = "Automatically shoots when holding down right trigger."},
            new ButtonInfo { buttonText = "Instant Kill", enableMethod =() => InstantKill(), disableMethod =() => DisableInstantKill(), toolTip = "Makes your gun instant kill players."},
            new ButtonInfo { buttonText = "Infinite Points", enableMethod =() => InfinitePoints(), disableMethod =() => DisableInfinitePoints(), toolTip = "Gives you an infinite amount of points."},
            new ButtonInfo { buttonText = "Infinite Ammo", enableMethod =() => InfiniteAmmo(), disableMethod =() => DisableInfiniteAmmo(), toolTip = "Gives you an infinite amount of ammo."},
        };

        public static void KillAll()
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "HitPlayer", (double)GetRandomPlayer(false).ActorNumber, (double)99999, (double)GetRandomPlayer(false).ActorNumber }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            RPCProtection();
        }

        public static void GodMode()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 957, "if not IsMe then PlayerData[Player.playerID].Health -= Modules.roundToQuarter(dmg) end" }
            });
        }

        public static void DisableGodMode() =>
            RevertCustomScript(957);

        public static void NoGrenadeCooldown()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1296, "grenadeCooldown = 0" }
            });
        }

        public static void DisableNoGrenadeCooldown() =>
            RevertCustomScript(1296);

        public static void NoShootCooldown()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1243, "shootCooldown = 0" }
            });
        }

        public static void DisableNoShootCooldown() =>
            RevertCustomScript(1243);

        public static void InfiniteAmmo()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1244, "" }
            });
        }

        public static void DisableInfiniteAmmo() =>
            RevertCustomScript(1244);

        public static void InstantKill()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 1278, "emitAndOnEvent(\"HitPlayer\", {found.playerID, 99999.0, LocalPlayer.playerID})" }
            });
        }

        public static void DisableInstantKill() =>
            RevertCustomScript(1278);

        public static void InfinitePoints()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 496, "saveData[\"Points\"] = 999999" }
            });
        }

        public static void DisableInfinitePoints() =>
            RevertCustomScript(496);

        public static void RapidFire()
        {
            ModifyCustomScript(new Dictionary<int, string>
            {
                { 2041, "needsLetGoR = false" }
            });
        }

        public static void DisableRapidFire() =>
            RevertCustomScript(2041);
    }
}