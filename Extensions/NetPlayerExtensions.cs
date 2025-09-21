using ExitGames.Client.Photon;
using iiMenu.Managers;
using Photon.Realtime;

namespace iiMenu.Extensions
{
    public static class NetPlayerExtensions
    {
        public static Player GetPlayer(this NetPlayer self) =>
            RigManager.NetPlayerToPlayer(self);

        public static Hashtable GetCustomProperties(this NetPlayer self) =>
            self.GetPlayer().CustomProperties;
    }
}
