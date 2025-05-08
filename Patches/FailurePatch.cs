using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(GorillaComputer), "GeneralFailureMessage")]
    public class FailurePatch
    {
        public static void Prefix(string failMessage)
        {
            if (failMessage.Contains("YOUR ACCOUNT"))
                CoroutineManager.instance.StartCoroutine(iiMenu.Menu.Main.ReportFailureMessage(failMessage));
        }
    }
}
