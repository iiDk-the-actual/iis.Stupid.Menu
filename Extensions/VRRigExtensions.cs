/*
 * ii's Stupid Menu  Extensions/VRRigExtensions.cs
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

using iiMenu.Managers;
using Photon.Pun;
using System.Linq;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Extensions
{
    public static class VRRigExtensions
    {
        public static bool IsLocal(this VRRig rig) =>
            PlayerIsLocal(rig);

        public static bool IsTagged(this VRRig rig) =>
            PlayerIsTagged(rig);

        public static Color GetColor(this VRRig rig) =>
            GetPlayerColor(rig);

        public static bool Active(this VRRig rig) =>
            rig != null && GorillaParent.instance.vrrigs.Contains(rig);

        public static float Distance(this VRRig rig, Vector3 position) =>
            Vector3.Distance(rig.transform.position, position);

        public static float Distance(this VRRig rig, VRRig otherRig) =>
            rig.Distance(otherRig.transform.position);
        
        public static float Distance(this VRRig rig) =>
            rig.Distance(GorillaTagger.Instance.bodyCollider.transform.position);

        public static VRRig GetClosest(this VRRig rig) =>
            GorillaParent.instance.vrrigs.Where(targetRig => targetRig != null && targetRig != rig)
                                         .OrderBy(targetRig => rig.Distance(targetRig))
                                         .FirstOrDefault();

        public static int GetPing(this VRRig rig)
        {
            if (playerPing.TryGetValue(rig, out int ping))
                return ping;
            else
                return PhotonNetwork.GetPing();
        }

        public static string GetName(this VRRig rig) =>
            RigManager.GetPlayerFromVRRig(rig)?.NickName ?? "null";

        public static NetPlayer GetPlayer(this VRRig rig) =>
            RigManager.GetPlayerFromVRRig(rig);

        public static Slingshot GetSlingshot(this VRRig rig) =>
            rig.projectileWeapon as Slingshot;
    }
}
