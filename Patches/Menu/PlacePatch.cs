using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(BuilderTable), "RequestPlacePiece")]
    public class PlacePatch
    {
        public static BuilderPiece _piece;

        public static sbyte _bumpOffsetX;
        public static sbyte _bumpOffsetZ;
        public static byte _twist;
        public static BuilderPiece _parentPiece;

        public static int _attachIndex;
        public static int _parentAttachIndex;

        private static void Postfix(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
        {
            _piece = piece;

            _bumpOffsetX = bumpOffsetX;
            _bumpOffsetZ = bumpOffsetZ;
            _twist = twist;
            _parentPiece = parentPiece;

            _attachIndex = attachIndex;
            _parentAttachIndex = parentAttachIndex;
        }
    }
}
