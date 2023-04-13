using System;
using GameServer.Script.net;
using GameServer.Script.db;
namespace GameServer.Script.logic
{
    public partial class MsgHandler {
        public static void MsgGetAchieve(ClientState c, MsgBase msgBase) {
            MsgGetAchieve msg = (MsgGetAchieve)msgBase;
            Player player = c.player;

            if (player == null) return;

            msg.win = player.playerDate.win;
            msg.loss = player.playerDate.loss;

            player.SendMsg(msg);
        }

        public static void MsgGetRoomList(ClientState c, MsgBase msgBase) {
            MsgGetRoomList msg = (MsgGetRoomList)msgBase;
            Player player = c.player;
            if (player == null) return;

            player.SendMsg(RoomManager.ToMsg());
        }

        public static void MsgCreateRoom(ClientState c, MsgBase msgBase) {
            MsgCreateRoom msg = (MsgCreateRoom)msgBase;
            Player player = c.player;
            if (player == null) return;

            if (player.roomID > 0) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            Room room = RoomManager.AddRoom();
            room.AddPlayer(player.ID);

            msg.res = 0;
            player.SendMsg(msg);
        }

        public static void MsgEnterRoom(ClientState c, MsgBase msgBase) {
            MsgEnterRoom msg = (MsgEnterRoom)msgBase;
            Player player = c.player;
            if (player == null) return;

            //已经在房间了
            if (player.roomID >= 0) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            //房间不存在
            Room room = RoomManager.GetRoom(msg.id);
            if (room == null) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            //进入
            if (!room.AddPlayer(player.ID)) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            msg.res = 0;
            player.SendMsg(msg);
        }

        public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase) {
            MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
            Player player = c.player;
            if (player == null) return;

            Room room = RoomManager.GetRoom(player.roomID);

            if (room == null) {
                player.SendMsg(msg);
                return;
            }

            player.SendMsg(room.ToMsg());
        }

        public static void MsgLeaveRoom(ClientState c, MsgBase msgBase) {
            MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
            Player player = c.player;
            if (player == null) return;

            Room room = RoomManager.GetRoom(player.roomID);

            if (room == null) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            room.RemovePlayer(player.ID);
            msg.res = 0;
            player.SendMsg(msg);
        }


        public static void MsgStartBattle(ClientState c, MsgBase msgBase) {
            MsgStartBattle msg = (MsgStartBattle)msgBase;
            Player player = c.player;
            if (player == null) return;

            Room room = RoomManager.GetRoom(player.roomID);

            if (room == null) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            if (!room.isOwner(player)) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            if (!room.StartBattle()) {
                msg.res = 1;
                player.SendMsg(msg);
                return;
            }

            msg.res = 0;
            player.SendMsg(msg);
        }
    }
}

