/*using GorillaExtensions;
using HarmonyLib;
using System;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(Slingshot))]
    [HarmonyPatch("OnRelease", MethodType.Normal)]
    internal class imbored
    {
        private static void Postfix(DropZone zoneReleased, GameObject releasingHand)
        {
            //UnityEngine.Debug.Log(zoneReleased.transform.parent.GetPath());
            UnityEngine.Debug.Log(releasingHand.GetPath());
        }
    }
}*/