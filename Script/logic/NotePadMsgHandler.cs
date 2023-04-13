using System;
using GameServer.Script.net;
using GameServer.Script.db;
namespace GameServer.Script.logic
{
    public partial class MsgHandler {
        public static void MsgGetText(ClientState clientState, MsgBase msgBase) {
            MsgGetText msgGetText = (MsgGetText)msgBase;
            Player player = clientState.player;

            if (player == null) return;
            msgGetText.getText = player.playerDate.text;
            player.SendMsg(msgGetText);
        }

        public static void MsgSaveText(ClientState clientState, MsgBase msgBase) {
            MsgSaveText msgSaveText = (MsgSaveText)msgBase;
            Player player = clientState.player;

            if (player == null) return;
            player.playerDate.text = msgSaveText.saveText;
            DBManager.UpdatePlayerData(player.ID, player.playerDate);
            player.SendMsg(msgSaveText);
        }


    }

}

