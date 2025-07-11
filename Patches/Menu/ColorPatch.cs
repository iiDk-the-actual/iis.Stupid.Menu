using HarmonyLib;
using iiMenu.Classes;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "RequestMaterialColor")]
    public class ColorPatch
    {
        public static bool patchEnabled = false;
        public static bool nameSpoofEnabled = false;

        public static bool Prefix(VRRig __instance, int askingPlayerID, PhotonMessageInfoWrapped info)
        {
            if (__instance == RigManager.LocalRig)
            {
                if (nameSpoofEnabled)
                    return false;
                
                if (patchEnabled)
                {
                    Photon.Realtime.Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", playerRef, new object[]
                    {
                        UnityEngine.Random.Range(0f, 255f) / 255f,
                        UnityEngine.Random.Range(0f, 255f) / 255f,
                        UnityEngine.Random.Range(0f, 255f) / 255f
                    });
                    Menu.Main.RPCProtection();
                    return false;
                }
            }
            return true;
        }
    }
}
