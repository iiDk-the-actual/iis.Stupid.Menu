using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BuilderTableNetworking), "PieceCreatedByShelfRPC")]
    public class CreatePatch
    {
        public static bool enabled;
        public static int pieceTypeSearch;

        private static void Postfix(int pieceType, int pieceId)
        {
            if (enabled)
            {
                if (pieceTypeSearch == pieceType)
                {
                    Mods.Fun.pieceId = pieceId;
                    enabled = false;
                }
            }
        }
    }
}
