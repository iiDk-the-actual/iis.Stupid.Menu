using HarmonyLib;
using System.Threading.Tasks;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(NetworkSystemPUN), "MakeOrFindRoom")]
    internal class RoomPatch
    {
        public static bool enabled;

        private static bool Prefix(NetworkSystemPUN __instance, string roomName, RoomConfig opts, int regionIndex, ref Task<NetJoinResult> __result)
        {
            if (!enabled)
                return true;

            __result = ForceCreateRoom(__instance, roomName, opts);
            return false;
        }

        private static async Task<NetJoinResult> ForceCreateRoom(NetworkSystemPUN instance, string roomName, RoomConfig opts)
        {
            if (instance.InRoom)
                await instance.InternalDisconnect();

            instance.currentRegionIndex = 0;
            return await instance.TryCreateRoom(roomName, opts);
        }
    }
}
