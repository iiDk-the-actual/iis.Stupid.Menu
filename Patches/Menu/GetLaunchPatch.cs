/*
 * ii's Stupid Menu  Patches/Menu/GetLaunchPatch.cs
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

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;
using static iiMenu.Utilities.GameModeUtilities;
using static iiMenu.Utilities.RigUtilities;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(Slingshot), nameof(Slingshot.GetLaunchVelocity))]
    public class GetLaunchPatch
    {
        public static bool enabled;

        public static void Postfix(Slingshot __instance, ref Vector3 __result)
        {
            if (enabled)
            {
                if (__instance.InLeftHand() ? leftTrigger > 0.5f : rightTrigger > 0.5f)
                    return;

                List<NetPlayer> infected = InfectedList();
                List<VRRig> rigs = GorillaParent.instance.vrrigs
                    .Where(rig => !rig.isLocal)
                    .Where(rig => !infected.Contains(GetPlayerFromVRRig(rig)))
                    .ToList();


                Transform head = GorillaTagger.Instance.headCollider.transform;
                VRRig targetRig = rigs
                    .Where(rig => rig != null)
                    .Select(rig => new {
                        Rig = rig,
                        ToRig = (rig.transform.position - head.position).normalized,
                        Distance = Vector3.Distance(head.position, rig.transform.position)
                    })
                    .OrderBy(x => Vector3.Angle(head.forward, x.ToRig) + x.Distance * 0.1f)
                    .Select(x => x.Rig)
                    .FirstOrDefault();

                if (targetRig == null)
                    return;

                __result = CalcMinSpeed(__instance.center.transform.position, targetRig);
            }
        }

        // ChatGPT used for math.. because I'm only 14 and haven't learned this yet and am not taking a class for a Gorilla Tag mod
        private static Vector3 CalcMinSpeed(Vector3 origin, VRRig targetRig)
        {
            Vector3 targetPos = targetRig.headMesh.transform.position;
            Vector3 targetVel = targetRig.LatestVelocity();
            targetVel.y /= 3f;

            Vector3 displacement = targetPos - origin;
            Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

            float g = -Physics.gravity.y;

            float x = displacementXZ.magnitude;
            float roughSpeed = 20f;
            float time = x / roughSpeed;

            Vector3 futurePos = targetPos + targetVel * time;
            displacement = futurePos - origin;
            displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            float y = displacement.y;
            x = displacementXZ.magnitude;

            float minSpeed = Mathf.Sqrt(g * (y + Mathf.Sqrt(x * x + y * y)));
            float launchSpeed = minSpeed * 2.5f;

            return CalcVelocity(displacement, launchSpeed);
        }

        private static Vector3 CalcVelocity(Vector3 displacement, float speed)
        {
            Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
            float x = displacementXZ.magnitude;
            float y = displacement.y;
            float g = -Physics.gravity.y;
            float v2 = speed * speed;

            float underSqrt = v2 * v2 - g * (g * x * x + 2 * y * v2);
            if (underSqrt <= 0f)
                return displacement.normalized * speed;

            float sqrt = Mathf.Sqrt(underSqrt);
            float angle = Mathf.Atan((v2 - sqrt) / (g * x));

            Vector3 dirXZ = displacementXZ.normalized;
            Vector3 result = dirXZ * Mathf.Cos(angle) * speed + Vector3.up * Mathf.Sin(angle) * speed;
            return result;
        }
    }
}
