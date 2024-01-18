using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace iiMenu.Classes
{
    internal class ExtRoomSystem
    {
        internal static void SendEvent(in byte code, in object evData, in RaiseEventOptions reo, in SendOptions so)
        {
            object[] sendEventData = new object[3];
            sendEventData[0] = PhotonNetwork.ServerTimestamp;
            sendEventData[1] = code;
            sendEventData[2] = evData;
            PhotonNetwork.RaiseEvent(3, sendEventData, reo, so);
        }
    }
}
