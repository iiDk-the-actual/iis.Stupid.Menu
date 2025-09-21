using HarmonyLib;

namespace iiMenu.Patches.Safety
{
    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    public class NoIncrementRPCCallLocal
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction) =>
            false;
    }
}
