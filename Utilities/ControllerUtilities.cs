/*
 * ii's Stupid Menu  Utilities/ControllerUtilities.cs
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

using iiMenu.Extensions;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

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

        private struct ControllerInfo
        {
            public ControllerType type;
            public float dataCacheTime;
        }

        public static ControllerType GetControllerType(bool left)
        {
            if (XRSettings.isDeviceActive)
            {
                if (!controllerInfo.TryGetValue(left, out ControllerInfo info) || Time.time > info.dataCacheTime + 60f)
                {
                    Dictionary<string, ControllerType> controllerNames = new Dictionary<string, ControllerType>
                {
                    { "quest2", ControllerType.Quest2 },
                    { "quest3", ControllerType.Quest3 },
                    { "knuckles", ControllerType.ValveIndex },
                    { "vive", ControllerType.VIVE }
                };

                    ControllerType controllerType = ControllerType.Unknown;

                    string controllerName = ControllerInputPoller.instance.leftControllerDevice.name.ToLower();
                    if (controllerName is null)
                        return ControllerType.Unknown;
                    foreach (var controller in controllerNames)
                    {
                        if (controllerName.Contains(controller.Key))
                        {
                            controllerType = controller.Value;
                            break;
                        }
                    }

                    controllerInfo[left] = new ControllerInfo
                    {
                        type = controllerType,
                        dataCacheTime = Time.time
                    };
                }

                return info.type;
            }
            return ControllerType.Unknown;
        }

        public static ControllerType GetLeftControllerType() => GetControllerType(true);
        public static ControllerType GetRightControllerType() => GetControllerType(false);
    }
}