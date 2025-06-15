using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;
using iiMenu.Mods;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(PropHauntHandFollower), "GeoCollisionPoint")]
    public class PropPatch
    {
        public static bool enabled = false;

        public static void Postfix(ref Vector3 __result, Vector3 sourcePos, Vector3 targetPos)
        {
            if (enabled)
                __result = targetPos;
        }
    }
}
