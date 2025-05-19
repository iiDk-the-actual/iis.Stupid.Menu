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
            if (ServerData.ServerDataEnabled && failMessage.Contains("YOUR ACCOUNT"))
                CoroutineManager.instance.StartCoroutine(ServerData.ReportFailureMessage(failMessage));
        }
    }
}
