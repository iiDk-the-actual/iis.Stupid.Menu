using UnityEngine;
using static iiMenu.Mods.Reconnect;

namespace iiMenu.Mods
{
    internal class RoomJoiners
    {
        public static void AutoJoinRoomRUN()
        {
            rejRoom = "RUN";
            rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomDAISY()
        {
            rejRoom = "DAISY";
            rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomDAISY09()
        {
            rejRoom = "DAISY09";
            rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomPBBV()
        {
            rejRoom = "PBBV";
            rejDebounce = Time.time + 2f;
        }

        public static void AutoJoinRoomBOT()
        {
            rejRoom = "BOT";
            rejDebounce = Time.time + 2f;
        }
    }
}
