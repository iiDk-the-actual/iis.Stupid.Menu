using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Reflection;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "SetRankedInfo")]
    public class SetRankedPatch
    {
        public static bool enabled;

        public static bool Prefix(VRRig __instance, float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, bool broadcastToOtherClients = true)
        {
            if (__instance.isLocal && enabled)
            {
                __instance.SetRankedInfoLocal(Mods.Safety.targetElo, Mods.Safety.targetBadge, Mods.Safety.targetBadge);

                FieldInfo rankedChanged = typeof(VRRig).GetField("OnRankedSubtierChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (rankedChanged != null)
                {
                    Action<int, int> del = rankedChanged.GetValue(__instance) as Action<int, int>;
                    del?.Invoke(Mods.Safety.targetBadge, Mods.Safety.targetBadge);
                }

                if (__instance.netView != null && broadcastToOtherClients)
                    __instance.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[] { __instance.currentRankedELO, __instance.currentRankedSubTierQuest, __instance.currentRankedSubTierPC });

                return false;
            }
            return true;
        }
    }
}
