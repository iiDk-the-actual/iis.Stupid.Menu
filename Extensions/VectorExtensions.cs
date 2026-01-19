/*
 * ii's Stupid Menu  Extensions/VectorExtensions.cs
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

using System;
using UnityEngine;
using static iiMenu.Utilities.RandomUtilities;

namespace iiMenu.Extensions
{
    public static class VectorExtensions
    {
        public static float Distance(this Vector3 point, Vector3 to) =>
            Vector3.Distance(point, to);

        public static Vector3 Lerp(this Vector3 a, Vector3 b, float t) =>
            Vector3.Lerp(a, b, t);

        public static Vector3 XyZ(this Vector3 a) =>
            new Vector3(a.x, Mathf.Max(a.y, 0f), a.z);

        public static long Pack(this Vector3 vec) =>
            BitPackUtils.PackWorldPosForNetwork(vec);

        public static Vector3 Random(this Vector3 _, float power = 1) =>
            RandomVector3(power);

        public static Vector3 ClampMagnitude(this Vector3 vec, float magnitude) =>
            Vector3.ClampMagnitude(vec, magnitude);

        public static Vector3 ClampSqrMagnitude(this Vector3 vec, float sqrMagnitude)
        {
            float currentSqrMag = vec.sqrMagnitude;

            if (!(currentSqrMag > sqrMagnitude) || !(currentSqrMag > 0f)) return vec;
            float scale = MathF.Sqrt(sqrMagnitude / currentSqrMag);
            vec *= scale;

            return vec;
        }
    }
}
