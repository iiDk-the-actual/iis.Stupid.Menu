using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BundleManager), "CheckIfBundlesOwned")]
    public class PostGetData
    {
        private static void Postfix() =>
            CosmeticsOwned = CosmeticsController.instance.concatStringCosmeticsAllowed;
    }
}
