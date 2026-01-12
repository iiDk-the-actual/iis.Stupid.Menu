/*
 * ii's Stupid Menu  Utilities/RandomUtilities.cs
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

using UnityEngine;

namespace iiMenu.Utilities
{
    public class RandomUtilities
    {
        /// <summary>
        /// Generates a Vector3 with each component set to a random value between -range and range.
        /// </summary>
        /// <param name="range">The maximum absolute value for each component of the generated vector.</param>
        /// <returns>A Vector3 with random x, y, and z components within the specified range.</returns>
        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(Random.Range(-range, range),
                        Random.Range(-range, range),
                        Random.Range(-range, range));

        /// <summary>
        /// Generates a random Quaternion with each Euler angle component in the range [0, range).
        /// </summary>
        /// <param name="range">The upper bound (exclusive) for each Euler angle in degrees. Defaults to 360.</param>
        /// <returns>A Quaternion with random rotation based on the specified range.</returns>
        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(Random.Range(0f, range),
                        Random.Range(0f, range),
                        Random.Range(0f, range));

        /// <summary>
        /// Generates a random color with each RGB component in the range [0, range) and the specified alpha value.
        /// </summary>
        /// <param name="range">The exclusive upper bound for the random RGB component values.</param>
        /// <param name="alpha">The alpha (transparency) value of the generated color.</param>
        /// <returns>A randomly generated Color with the specified range and alpha.</returns>
        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        alpha);

        /// <summary>
        /// Generates a random alphanumeric string of the specified length using uppercase letters and digits.
        /// </summary>
        /// <param name="length">The length of the generated string. Defaults to 4.</param>
        /// <returns>A random string consisting of uppercase letters and digits.</returns>
        public static string RandomString(int length = 4)
        {
            string random = "";
            for (int i = 0; i < length; i++)
            {
                int rand = Random.Range(0, 36);
                char c = rand < 26
                    ? (char)('A' + rand)
                    : (char)('0' + (rand - 26));
                random += c;
            }

            return random;
        }
    }
}
