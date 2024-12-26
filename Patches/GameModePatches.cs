using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace iiMenu.Patches
{
    public class GameModePatches
    {
        public static bool enabled = false;
        public static string gameMode = "AMBUSH";

        [HarmonyPatch(typeof(GorillaGameManager), "ValidGameMode")]
        public class ValidGameModePatch
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
