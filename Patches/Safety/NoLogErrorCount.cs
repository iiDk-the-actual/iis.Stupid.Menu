using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
    public class NoLogErrorCount
    {
        private static bool Prefix(string logString, string stackTrace, LogType type) =>
            false;
    }
}
