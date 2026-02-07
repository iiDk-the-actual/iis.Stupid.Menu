/*
 * ii's Stupid Menu  Patches/Menu/ClimbablePatch.cs
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

using GorillaLocomotion.Climbing;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GorillaHandClimber), nameof(GorillaHandClimber.GetClosestClimbable))]
    public class ClimbablePatch
    {
        public static bool enabled;
        private static void Postfix(GorillaHandClimber __instance, ref GorillaClimbable __result)
        {
            if (enabled && __result == null)
            {
                switch (__instance.potentialClimbables.Count)
                {
                    case 0:
                    case 1:
                        return;
                }

                Vector3 position = __instance.transform.position;
                Bounds bounds = __instance.col.bounds;

                float closestDistance = (__instance.col as SphereCollider).radius + 0.05f;

                GorillaClimbable gorillaClimbable = null;
                foreach (GorillaClimbable potentialClimbable in __instance.potentialClimbables)
                {
                    float distance;
                    if (potentialClimbable.colliderCache)
                    {
                        if (!bounds.Intersects(potentialClimbable.colliderCache.bounds))
                            continue;
                        
                        Vector3 vector = potentialClimbable.colliderCache.ClosestPoint(position);
                        distance = Vector3.Distance(position, vector);
                    }
                    else
                        distance = Vector3.Distance(position, potentialClimbable.transform.position);

                    if (distance < closestDistance)
                    {
                        gorillaClimbable = potentialClimbable;
                        closestDistance = distance;
                    }
                }

                __result = gorillaClimbable;
            }
        }
    }
}
