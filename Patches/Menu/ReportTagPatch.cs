using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaTagManager), "ReportTag")]
    public class ReportTagPatch
    {
        public static List<NetPlayer> blacklistedPlayers = new List<NetPlayer>();
        public static List<NetPlayer> invinciblePlayers = new List<NetPlayer>();

        public static bool Prefix(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
        {
            if (blacklistedPlayers.Contains(taggingPlayer) || invinciblePlayers.Contains(taggedPlayer))
                return false;

            return true;
        }
    }
}
