using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using HarmonyLib;
using iiMenu.Extensions;
using iiMenu.Managers;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public static class Detected
    {
        public static float del;
        public static void CrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    if (Time.time > del)
                    {
                        PhotonNetwork.SetMasterClient(lockTarget.GetPlayer().GetPlayer());
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                        del = Time.time + 0.02f;
                    }
                    RPCProtection();
                }

                if (GetGunInput(true))
                {
                    VRRig gunTarget = Ray.collider.GetComponentInParent<VRRig>();
                    if (gunTarget && !PlayerIsLocal(gunTarget))
                    {
                        gunLocked = true;
                        lockTarget = gunTarget;
                    }
                }
            }
            else
            {
                if (gunLocked)
                    gunLocked = false;
            }
        }

        private static Coroutine disablePatchCoroutine;
        public static IEnumerator DisablePatch()
        {
            while (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                yield return null;
            }
            Patches.GameModePatch.enabled = false;
        }

        public static void ChangeGamemode(GameModeType gamemode)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (disablePatchCoroutine != null)
                disablePatchCoroutine = CoroutineManager.instance.StartCoroutine(DisablePatch());

            Patches.GameModePatch.enabled = true;

            NetworkView netView = (NetworkView)Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").Field("netView").GetValue();
            NetworkSystem.Instance.NetDestroy(netView.gameObject);

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
            {
                { "gameMode", PhotonNetworkController.Instance.currentJoinTrigger.networkZone + GorillaComputer.instance.currentQueue + gamemode.ToString() }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);

            GorillaGameManager ggs = GameMode.activeGameMode;
            ggs.StopPlaying();
            Traverse.Create(typeof(GameMode)).Field("activeGameMode").SetValue(null);
            Traverse.Create(typeof(GameMode)).Field("activeNetworkHandler").SetValue(null);

            GameMode.LoadGameMode(gamemode.ToString());
        }
    }
}
