using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using static iiMenu.Menu.Main;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BundleManager), "CheckIfBundlesOwned")]
    public class PostGetData
    {
        public static bool CosmeticsInitialized;
        private static void Postfix()
        {
            CosmeticsInitialized = true;
            CosmeticsOwned = CosmeticsController.instance.concatStringCosmeticsAllowed;
        }
    }
}
