/*
 * ii's Stupid Menu  Utilities/RandomUtilities.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2025  Goldentrophy Software
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
        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(Random.Range(-range, range),
                        Random.Range(-range, range),
                        Random.Range(-range, range));

        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(Random.Range(0f, range),
                        Random.Range(0f, range),
                        Random.Range(0f, range));

        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        (byte)Random.Range(0, range),
                        alpha);

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
