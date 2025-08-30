using HarmonyLib;
using System.Collections.Generic;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(RankedProgressionManager), "SetLocalProgressionData")]
    public class SetRankedPatch
    {
        public static bool enabled;

        public static bool Prefix(RankedProgressionManager __instance, GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
        {
            if (enabled)
            {
                Dictionary<int, int[]> tierData = new Dictionary<int, int[]> 
                {
                    { 0, new int[] { 0, 0 } },
                    { 1, new int[] { 0, 1 } },

                    { 2, new int[] { 1, 0 } },
                    { 3, new int[] { 1, 1 } },
                    { 4, new int[] { 1, 2 } },

                    { 5, new int[] { 2, 0 } },
                    { 6, new int[] { 2, 1 } },
                    { 7, new int[] { 2, 2 } },
                };

                foreach (GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData platformData in data.platformData)
                {
                    platformData.elo = Mods.Safety.targetElo;
                    platformData.majorTier = tierData[Mods.Safety.targetBadge][0];
                    platformData.minorTier = tierData[Mods.Safety.targetBadge][1];
                }
                __instance.ProgressionData = data;
                return false;
            }

            return true;
        }
    }
}
