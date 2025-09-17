using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(RequestableOwnershipGuard), "OwnershipRequested")]
    public class OwnershipPatch
    {
        public static bool enabled;
        public static List<RequestableOwnershipGuard> blacklistedGuards = new List<RequestableOwnershipGuard>();

        public static bool Prefix(RequestableOwnershipGuard __instance, string nonce, PhotonMessageInfo info) =>
            !enabled || (__instance.photonView.IsMine && !blacklistedGuards.Contains(__instance));
    }
}
