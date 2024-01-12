using Photon.Pun;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    internal class Miscellaneous
    {
        public static void CopyIDGun()
        {
            if (rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, out var Ray);
                if (shouldBePC)
                {
                    Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out Ray, 100);
                }

                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.color = bgColorA;
                NewPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                NewPointer.transform.position = Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);
                if (rightTrigger > 0.5f || Mouse.current.leftButton.isPressed)
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        GUIUtility.systemCopyBuffer = GetPlayerFromVRRig(possibly).UserId;
                    }
                }
            }
        }

        public static void GrabPlayerInfo()
        {
            string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                float r = 0f;
                float g = 0f;
                float b = 0f;
                try
                {
                    VRRig plr = GorillaGameManager.instance.FindPlayerVRRig(player);
                    r = plr.playerColor.r * 255;
                    g = plr.playerColor.r * 255;
                    b = plr.playerColor.r * 255;
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
                        ")"
                });
                }
                catch { UnityEngine.Debug.Log("Failed to log player"); }
            }
            text += "\n====================================\n";
            text += "Text file generated with ii's Stupid Menu";
            string fileName = "iisStupidMenu/" + PhotonNetwork.CurrentRoom.Name + " - Player Info.txt";
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
