using HarmonyLib;
using UnityEngine;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(GrowingSnowballThrowable), "SnowballThrowEventReceiver")]
    public class LimitPatch
    {
        public static bool Prefix(GrowingSnowballThrowable __instance, int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
        {
            NetPlayer player = info.Sender;
            if (player != null && iiMenu.Menu.Main.ShouldBypassChecks(player))
            {
                object obj = args[0];
                if (obj is Vector3)
                {
                    Vector3 vector = (Vector3)obj;
                    obj = args[1];
                    if (obj is Vector3)
                    {
                        Vector3 vector2 = (Vector3)obj;
                        obj = args[2];
                        if (obj is int)
                            __instance.LaunchSnowballRemote(vector, vector2, __instance.snowballModelTransform.lossyScale.x, (int)obj, info);
                    }
                }
                return false;
            }

            return true;
        }
    }
}
