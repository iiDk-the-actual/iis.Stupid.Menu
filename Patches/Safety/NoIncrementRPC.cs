using HarmonyLib;
using System;

namespace iiMenu.Patches.Safety
{
    // Thanks DrPerky
    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class NoIncrementRPC
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall) =>
            false;
    }
}
