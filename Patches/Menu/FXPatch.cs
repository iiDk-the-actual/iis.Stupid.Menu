using HarmonyLib;
using iiMenu.Classes;
using System;

namespace iiMenu.Patches
{
    [HarmonyPatch(typeof(FXSystem), "PlayFXForRig", new Type[] { typeof(FXType), typeof(IFXContext), typeof(PhotonMessageInfoWrapped) })]
    public class FXPatch
    {
        public static bool Prefix(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
        {
            NetPlayer player = info.Sender;
            if (player != NetworkSystem.Instance.LocalPlayer && player != null && FriendManager.IsPlayerFriend(player))
            {
                context.OnPlayFX();
                return false;
            }

            return true;
        }
    }
}
