using HarmonyLib;
using iiMenu.Mods;
using System;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(SizeManager), "ControllingChanger")]
    public class ScalePatch
    {
        private static void Postfix(ref SizeChanger __result, Transform t)
        {
            try
            {
                if (Movement.schanging && t == GorillaTagger.Instance.offlineVRRig.transform)
                {
                    __result = Movement.newSC;
                }
            }
            catch { }
        }
    }
}
