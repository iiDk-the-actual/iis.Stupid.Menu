/*
 * ii's Stupid Menu  Patches/Menu/FirePatch.cs
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
using iiMenu.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static iiMenu.Utilities.GameModeUtilities;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(SIGadgetChargeBlaster), nameof(SIGadgetChargeBlaster.FireProjectile))]
    public class FirePatch
    {
        public static bool enabled;

        public static void Prefix(SIGadgetChargeBlaster __instance, float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
        {
            if (enabled && __instance.blaster.LocalEquippedOrActivated)
            {
                List<NetPlayer> infected = InfectedList();
                List<VRRig> rigs = GorillaParent.instance.vrrigs
                    .Where(rig => !rig.isLocal)
                    .Where(rig => !infected.Contains(rig.GetPlayer()))
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

                rotation = Quaternion.LookRotation((targetRig.headMesh.transform.position - position).normalized);
            }
        }
    }
}
