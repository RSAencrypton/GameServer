using System;
using GameServer.Script.net;
namespace GameServer.Script.logic
{

    public partial class MsgHandler {
        public static void MsgPing(ClientState clientState, MsgBase msgBase) {
            Console.WriteLine("Ping");
            clientState.lastPingTime = NetManager.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            NetManager.SendMsg(clientState, msgPong);
        }
    }
}

