using HarmonyLib;
using iiMenu.Classes;
using System;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "UpdateName", new Type[] { typeof(bool) })]
    public class NamePatch
    {
        private static void Postfix(VRRig __instance, bool isNamePermissionEnabled)
        {
            NetPlayer player = RigManager.GetPlayerFromVRRig(__instance) ?? null;
            if (__instance != VRRig.LocalRig && player != null && FriendManager.IsPlayerFriend(player))
            {
                __instance.playerText1.text = player.NickName;
                __instance.playerText2.text = Menu.Main.NoRichtextTags(player.NickName);
            }
        }
    }
}
