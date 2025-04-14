﻿using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "get_ServerTimestamp")]
    public class PhotonTimePatch
    {
        public static bool enabled = false;
        public static int distTime = 0;

        public static void Postfix(ref int __result)
        {
            if (enabled)
                __result += distTime;
        }
    }
}
