/*
 * ii's Stupid Menu  Patches/Menu/ReleasePatch.cs
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

﻿using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(TakeMyHand_HandLink), nameof(TakeMyHand_HandLink.OnRelease))]
    public class ReleasePatch
    {
        public static bool enabled;

        public static bool Prefix(TakeMyHand_HandLink __instance, bool __result, DropZone zoneReleased, GameObject releasingHand)
        {
            if (enabled)
            {
                if (!__instance.myRig.isOfflineVRRig)
                {
                    bool grounded = false;

                    TakeMyHand_HandLink handLink = releasingHand == EquipmentInteractor.instance.leftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
                   
                    HandLinkAuthorityStatus selfHandLinkAuthority = GTPlayer.Instance.TakeMyHand_GetSelfHandLinkAuthority();
                    HandLinkAuthorityStatus selfChainAuthority = handLink.GetChainAuthority(out _);

                    if (selfHandLinkAuthority.type >= HandLinkAuthorityType.ButtGrounded && selfChainAuthority.type < selfHandLinkAuthority.type)
                        grounded = true;
                    else if (handLink.myOtherHandLink.grabbedLink != null)
                    {
                        HandLinkAuthorityStatus otherChainAuthority = handLink.myOtherHandLink.GetChainAuthority(out _);
                        if (otherChainAuthority.type >= HandLinkAuthorityType.ButtGrounded && selfChainAuthority.type < otherChainAuthority.type)
                            grounded = true;
                    }

                    if (grounded)
                    {
                        Vector3 averageVelocity = (handLink.isLeftHand ? GTPlayer.Instance.LeftHand.velocityTracker : GTPlayer.Instance.RightHand.velocityTracker).GetAverageVelocity(true).normalized * 20f;
                        __instance.myRig.netView.SendRPC("DroppedByPlayer", __instance.myRig.OwningNetPlayer, averageVelocity);
                        __instance.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
                    }

                    handLink.BreakLink();
                }
                return false;
            }
            
            return true;
        }
    }
}
