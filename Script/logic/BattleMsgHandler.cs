using System;
using GameServer.Script.net;

namespace GameServer.Script.logic
{
    public partial class MsgHandler {
        public static void MsgMove(ClientState clientState, MsgBase msgBase) {
            MsgMove msgMove = (MsgMove)msgBase;
            Console.WriteLine(msgMove.posX);
            msgMove.posX += 100;
            NetManager.SendMsg(clientState, msgMove);
        }
    }
}

