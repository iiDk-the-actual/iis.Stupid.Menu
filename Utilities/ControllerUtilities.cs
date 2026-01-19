/*
 * ii's Stupid Menu  Utilities/ControllerUtilities.cs
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

using GorillaLocomotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace iiMenu.Utilities
{
    public class ControllerUtilities
    {
        public enum ControllerType
        {
            Unknown,
            Quest2,
            Quest3,
            ValveIndex,
            VIVE
        }

        private static readonly Dictionary<bool, ControllerInfo> controllerInfo = new Dictionary<bool, ControllerInfo>
        {
            { true, new ControllerInfo { type = ControllerType.Unknown, dataCacheTime = -1f } },
            { false, new ControllerInfo { type = ControllerType.Unknown, dataCacheTime = -1f } }
        };

        private static readonly Dictionary<string, ControllerType> controllerNames = new Dictionary<string, ControllerType>
        {
            { "quest2", ControllerType.Quest2 },
            { "quest3", ControllerType.Quest3 },
            { "knuckles", ControllerType.ValveIndex },
            { "vive", ControllerType.VIVE }
        };

        private struct ControllerInfo
        {
            public ControllerType type;
            public float dataCacheTime;
        }

        /// <summary>
        /// Determines the type of VR controller currently in use for the specified hand.
        /// The result is derived from the XR device name and cached briefly to avoid
        /// repeated string comparisons.
        /// </summary>
        /// <param name="left">
        /// If true, queries the left controller; otherwise queries the right controller.
        /// </param>
        /// <returns>
        /// The detected <see cref="ControllerType"/> for the specified hand,
        /// or <see cref="ControllerType.Unknown"/> if XR is inactive, the controller
        /// is unavailable, or detection fails.
        /// </returns>
        public static ControllerType GetControllerType(bool left)
        {
            try
            {
                if (!XRSettings.isDeviceActive)
                    return ControllerType.Unknown;

                var controller = left
                    ? ControllerInputPoller.instance.leftControllerDevice
                    : ControllerInputPoller.instance.rightControllerDevice;

                if (controllerInfo.TryGetValue(left, out var info) && !(Time.time > info.dataCacheTime + 1f))
                    return info.type;
                var controllerName = controller.name;
                var controllerType = ControllerType.Unknown;

                foreach (var kvp in controllerNames)
                {
                    if (controllerName.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    controllerType = kvp.Value;
                    break;
                }

                info = new ControllerInfo
                {
                    type = controllerType,
                    dataCacheTime = Time.time
                };
                controllerInfo[left] = info;

                return info.type;
            }
            catch
            {
                return ControllerType.Unknown;
            }
        }

        /// <summary>
        /// Determines the type of VR controller currently in use for the left hand.
        /// </summary>
        /// <returns>
        /// The detected left-hand <see cref="ControllerType"/>,
        /// or <see cref="ControllerType.Unknown"/> if detection fails.
        /// </returns>
        public static ControllerType GetLeftControllerType() => GetControllerType(true);

        /// <summary>
        /// Determines the type of VR controller currently in use for the right hand.
        /// </summary>
        /// <returns>
        /// The detected right-hand <see cref="ControllerType"/>,
        /// or <see cref="ControllerType.Unknown"/> if detection fails.
        /// </returns>
        public static ControllerType GetRightControllerType() => GetControllerType(false);

        /// <summary>
        /// Calculates the true world-space pose of a player's hand by combining the
        /// controller transform with the hand-specific positional and rotational offsets.
        /// </summary>
        /// <param name="left">
        /// If true, uses the left hand/controller; otherwise uses the right hand/controller.
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>position</c> � The final world-space position of the hand.</description></item>
        /// <item><description><c>rotation</c> � The final world-space rotation of the hand.</description></item>
        /// <item><description><c>up</c> � The hand�s up direction in world space.</description></item>
        /// <item><description><c>forward</c> � The hand�s forward direction in world space.</description></item>
        /// <item><description><c>right</c> � The hand�s right direction in world space.</description></item>
        /// </list>
        /// </returns>
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetTrueHandPosition(bool left)
        {
            Transform controllerTransform = left ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            GTPlayer.HandState handState = left ? GTPlayer.Instance.LeftHand : GTPlayer.Instance.RightHand;

            Quaternion rot = controllerTransform.rotation * handState.handRotOffset;
            return (controllerTransform.position + controllerTransform.rotation * (handState.handOffset * GTPlayer.Instance.scale), rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        /// <summary>
        /// Gets the true world-space pose of the player's left hand,
        /// including position, rotation, and orientation vectors.
        /// </summary>
        /// <returns>
        /// A tuple containing the left hand�s world-space position, rotation,
        /// and its up, forward, and right direction vectors.
        /// </returns>
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right)
            GetTrueLeftHand() => GetTrueHandPosition(true);

        /// <summary>
        /// Gets the true world-space pose of the player's right hand,
        /// including position, rotation, and orientation vectors.
        /// </summary>
        /// <returns>
        /// A tuple containing the right hand�s world-space position, rotation,
        /// and its up, forward, and right direction vectors.
        /// </returns>
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right)
            GetTrueRightHand() => GetTrueHandPosition(false);
    }
}