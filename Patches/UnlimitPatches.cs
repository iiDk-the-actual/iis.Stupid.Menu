using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches
{
    public class UnlimitPatches
    {
        public static bool enabled = false;

        [HarmonyPatch(typeof(BuilderPiece), "CanPlayerGrabPiece")]
        public class UnlimitPatch1
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiece), "CanPlayerAttachPieceToPiece")]
        public class UnlimitPatch2
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerAttachToPlot")]
        public class UnlimitPatch3
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerAttachToPlot")]
        public class UnlimitPatch4
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderPiecePrivatePlot), "CanPlayerGrabFromPlot")]
        public class UnlimitPatch5
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateAttachPieceParams")]
        public class UnlimitPatch6
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateCreatePieceParams")]
        public class UnlimitPatch7
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateDropPieceParams")]
        public class UnlimitPatch8
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateDropPieceState")]
        public class UnlimitPatch9
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateFunctionalPieceState")]
        public class UnlimitPatch10
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceParams")]
        public class UnlimitPatch11
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceState")]
        public class UnlimitPatch12
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateGrabPieceState")]
        public class UnlimitPatch13
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePieceWorldTransform")]
        public class UnlimitPatch14
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePieceWorldTransform")]
        public class UnlimitPatch15
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePlacePieceParams")]
        public class UnlimitPatch16
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidatePlacePieceState")]
        public class UnlimitPatch17
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(BuilderTable), "ValidateRepelPiece")]
        public class UnlimitPatch18
        {
            public static bool Prefix(ref bool __result)
            {
                if (enabled)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }
    }
}
