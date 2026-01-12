/*
 * ii's Stupid Menu  Patches/Menu/UnlimitPatches.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

﻿using GorillaTagScripts;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    public class UnlimitPatches
    {
        public static bool enabled;

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
