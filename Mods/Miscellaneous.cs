using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;
using iiMenu.Notifications;

namespace iiMenu.Mods
{
    public class Miscellaneous
    {
        public static float indicatorDelay = 0f;

        private static float idgundelay = 0f;
        public static void CopyIDGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true) && Time.time > idgundelay)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        idgundelay = Time.time + 0.5f;
                        string id = GetPlayerFromVRRig(possibly).UserId;
                        NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
                        GUIUtility.systemCopyBuffer = id;
                    }
                }
            }
        }

        public static void CopySelfID()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id, 5000);
            GUIUtility.systemCopyBuffer = id;
        }

        private static float cgdgd = 0f;
        public static void CopyCreationDateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                RaycastHit Ray = GunData.Ray;
                GameObject NewPointer = GunData.NewPointer;

                if (GetGunInput(true))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig && Time.time > cgdgd)
                    {
                        cgdgd = Time.time + 0.5f;
                        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = GetPlayerFromVRRig(possibly).UserId }, delegate (GetAccountInfoResult result) // Who designed this
                        {
                            string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy h:mm tt");
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
                            GUIUtility.systemCopyBuffer = date;
                        }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not copy creation date."); }, null, null);
                    }
                }
            }
        }

        public static void CopyCreationDateSelf()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = PhotonNetwork.LocalPlayer.UserId }, delegate (GetAccountInfoResult result) // Who designed this
            {
                string date = result.AccountInfo.Created.ToString("MMMM dd, yyyy h:mm tt");
                NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
                GUIUtility.systemCopyBuffer = date;
            }, delegate { NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not copy creation date."); }, null, null);
        }

        public static void GrabPlayerInfo()
        {
            string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                float r = 0f;
                float g = 0f;
                float b = 0f;
                string cosmetics = "";
                try
                {
                    VRRig plr = GetVRRigFromPlayer(player);
                    r = plr.playerColor.r * 255;
                    g = plr.playerColor.g * 255;
                    b = plr.playerColor.b * 255;
                    cosmetics = plr.concatStringOfCosmeticsAllowed;
                }
                catch { UnityEngine.Debug.Log("Failed to log colors, rig most likely nonexistent"); }
                try
                {
                    text += "\n====================================\n";
                    text += string.Concat(new string[]
                    {
                        "Player Name: \"",
                        player.NickName,
                        "\", Player ID: \"",
                        player.UserId,
                        "\", Player Color: (R: ",
                        r.ToString(),
                        ", G: ",
                        g.ToString(),
                        ", B: ",
                        b.ToString(),
                        "), Cosmetics: ",
                        cosmetics
                    });
                }
                catch { UnityEngine.Debug.Log("Failed to log player"); }
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = "iisStupidMenu/PlayerInfo/" + PhotonNetwork.CurrentRoom.Name + ".txt";
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            if (!Directory.Exists("iisStupidMenu/PlayerInfo"))
            {
                Directory.CreateDirectory("iisStupidMenu/PlayerInfo");
            }
            File.WriteAllText(fileName, text);

            //string filePath = System.IO.Path.Combine(Application.dataPath, fileName);
            string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;
            //filePath = filePath.Split("\\")[0] + "/" + filePath.Split("\\")[1];
            try
            {
                Process.Start(filePath);
            }
            catch
            {
                UnityEngine.Debug.Log("Could not open process " + filePath);
            }
        }

        public static void DumpSoundData()
        {
            string text = "Handtap Sound Data\n(from GorillaLocomotion.Player.Instance.materialData)";
            int i = 0;
            foreach (GorillaLocomotion.Player.MaterialData oneshot in GorillaLocomotion.Player.Instance.materialData)
            {
                try
                {
                    text += "\n====================================\n";
                    text += i.ToString() + " ; " + oneshot.matName + " ; " + oneshot.slidePercent.ToString() + "% ; " + (oneshot.audio == null ? "none" : oneshot.audio.name);
                }
                catch { UnityEngine.Debug.Log("Failed to log sound"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = "iisStupidMenu/SoundData.txt";
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            File.WriteAllText(fileName, text);

            //string filePath = System.IO.Path.Combine(Application.dataPath, fileName);
            string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;
            //filePath = filePath.Split("\\")[0] + "/" + filePath.Split("\\")[1];
            try
            {
                Process.Start(filePath);
            }
            catch
            {
                UnityEngine.Debug.Log("Could not open process " + filePath);
            }
        }

        public static void DumpCosmeticData()
        {
            string text = "Cosmetic Data\n(from GorillaNetworking.CosmeticsController.allCosmeticsDict)";
            int i = 0;
            foreach (GorillaNetworking.CosmeticsController.CosmeticItem hat in GorillaNetworking.CosmeticsController.instance.allCosmetics)
            {
                try
                {
                    text += "\n====================================\n";
                    text += hat.itemName + " ; " + hat.displayName + " (override " + hat.overrideDisplayName + ") ; " + hat.cost.ToString() + "SR ; canTryOn = " + hat.canTryOn.ToString();
                }
                catch { UnityEngine.Debug.Log("Failed to log hat"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = "iisStupidMenu/CosmeticData.txt";
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            File.WriteAllText(fileName, text);

            //string filePath = System.IO.Path.Combine(Application.dataPath, fileName);
            string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;
            //filePath = filePath.Split("\\")[0] + "/" + filePath.Split("\\")[1];
            try
            {
                Process.Start(filePath);
            }
            catch
            {
                UnityEngine.Debug.Log("Could not open process " + filePath);
            }
        }

        public static void DumpRPCData()
        {
            string text = "RPC Data\n(from PhotonNetwork.PhotonServerSettings.RpcList)";
            int i = 0;
            foreach (string name in PhotonNetwork.PhotonServerSettings.RpcList)
            {
                try
                {
                    text += "\n====================================\n";
                    text += i.ToString() + " ; " + name;
                }
                catch { UnityEngine.Debug.Log("Failed to log RPC"); }
                i++;
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = "iisStupidMenu/RPCData.txt";
            if (!Directory.Exists("iisStupidMenu"))
            {
                Directory.CreateDirectory("iisStupidMenu");
            }
            File.WriteAllText(fileName, text);

            //string filePath = System.IO.Path.Combine(Application.dataPath, fileName);
            string filePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, fileName);
            filePath = filePath.Split("BepInEx\\")[0] + fileName;
            //filePath = filePath.Split("\\")[0] + "/" + filePath.Split("\\")[1];
            try
            {
                Process.Start(filePath);
            }
            catch
            {
                UnityEngine.Debug.Log("Could not open process " + filePath);
            }
        }
    }
}
