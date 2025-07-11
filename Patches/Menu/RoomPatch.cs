using HarmonyLib;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GorillaNetworking;
using System.Linq;
using Photon.Pun;
using System.Runtime.CompilerServices;
using System;
using GorillaTag.GuidedRefs;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(NetworkSystemPUN), "MakeOrFindRoom")]
    internal class RoomPatch
    {
        public static bool enabled;
        public static bool duplicate;

        private static bool Prefix(NetworkSystemPUN __instance, string roomName, RoomConfig opts, int regionIndex, ref Task<NetJoinResult> __result)
        {
            if (!enabled)
                return true;

            if (duplicate)
                __result = ForceJoinDuplicate(__instance, roomName, opts);
            else
                __result = FastJoinRoom(__instance, roomName, opts);

            return false;
        }

        private static async Task<NetJoinResult> ForceJoinDuplicate(NetworkSystemPUN instance, string roomName, RoomConfig opts)
        {
            if (instance.InRoom)
                await instance.InternalDisconnect();

            List<string> publicRegions = await GetEmptyRegions(roomName);
            string publicRegion = publicRegions[0];

            int regionIndex = Array.IndexOf(NetworkSystem.Instance.regionNames, publicRegions);

            instance.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;

            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = publicRegion;
            instance.currentRegionIndex = regionIndex;
            instance.UpdateZoneInfo(opts.isPublic, null);
            PhotonNetwork.ConnectUsingSettings();

            bool waitForState = await instance.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster });

            NetJoinResult netJoinResult;
            if (!waitForState)
                netJoinResult = NetJoinResult.Failed_Other;

            else
            {
                instance.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
                PhotonNetwork.CreateRoom(roomName, opts.ToPUNOpts(), null, null);
                waitForState = await instance.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
                {
                    NetworkSystemPUN.InternalState.Searching_Created,
                    NetworkSystemPUN.InternalState.Searching_CreateFailed
                });

                if (!waitForState)
                    netJoinResult = NetJoinResult.Failed_Other;
                else if (instance.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
                    netJoinResult = NetJoinResult.Failed_Other;
                else
                    netJoinResult = NetJoinResult.FallbackCreated;
            }

            return netJoinResult;
        }

        private static async Task<NetJoinResult> FastJoinRoom(NetworkSystemPUN instance, string roomName, RoomConfig opts)
        {
            if (instance.InRoom)
                await instance.InternalDisconnect();

            var (region, count) = await GetLargestRegion(roomName);

            if (count == 0)
                return await ForceJoinDuplicate(instance, roomName, opts);

            if (count >= 10)
            {
                GorillaComputer.instance.roomFull = true;
                GorillaComputer.instance.UpdateScreen();
                return NetJoinResult.Failed_Full;
            }

            int regionIndex = Array.IndexOf(NetworkSystem.Instance.regionNames, region);
            return await instance.TryJoinRoomInRegion(roomName, opts, regionIndex) ? NetJoinResult.Success : NetJoinResult.Failed_Other;
        }

        private static async Task<List<string>> GetEmptyRegions(string roomName)
        {
            string[] regions = NetworkSystem.Instance.regionNames;
            List<string> emptyRegions = regions.ToList();

            foreach (string region in regions.ToArray())
            {
                PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
                {
                    SharedGroupId = roomName + region.ToUpper()
                }, delegate (GetSharedGroupDataResult result)
                {
                    if (result.Data.Count > 0)
                        emptyRegions.Remove(region);
                }, null, null, null);
            }

            await Task.Delay(500);
            return emptyRegions;
        }

        private static async Task<(string region, int count)> GetLargestRegion(string roomName)
        {
            string[] regions = NetworkSystem.Instance.regionNames;
            Dictionary<string, int> regionData = new Dictionary<string, int> { };

            foreach (string region in regions.ToArray())
            {
                PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest
                {
                    SharedGroupId = roomName + region.ToUpper()
                }, delegate (GetSharedGroupDataResult result)
                {
                    regionData.Add(region, result.Data.Count);
                }, null, null, null);
            }

            await Task.Delay(500);
            KeyValuePair<string, int> largest = regionData.OrderByDescending(kv => kv.Value).First();
            return (largest.Key, largest.Value);
        }
    }
}
