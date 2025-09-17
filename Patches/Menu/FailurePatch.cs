using GorillaNetworking;
using HarmonyLib;
using iiMenu.Classes.Menu;
using iiMenu.Managers;

namespace iiMenu.Patches.Menu
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
