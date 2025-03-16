using GorillaTagScripts;
using HarmonyLib;
using UnityEngine;
using static hykmMenu.Menu.Main;

namespace hykmMenu.Patches
{
    [HarmonyPatch(typeof(BuilderTableNetworking), "PieceCreatedRPC")]
    public class CreatePatch
    {
        public static bool enabled = false;
        public static int pieceTypeSearch = 0;

        private static void Postfix(int pieceType, int pieceId)
        {
            if (enabled)
            {
                if (pieceTypeSearch == pieceType)
                {
                    iiMenu.Mods.Fun.pieceId = pieceId;
                    enabled = false;
                }
            }
        }
    }
}
