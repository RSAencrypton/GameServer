using System;
using GameServer.Script.net;
using GameServer.Script.db;
using GameServer.Script.logic;
namespace GameServer.Script.logic
{
	public class EventHandler
	{

		public static void onDisconnect(ClientState clientState) {
			Console.WriteLine("Close");

			if (clientState.player != null) {
				int roomID = clientState.player.roomID;
				if (roomID > 0) {
					Room room = RoomManager.GetRoom(roomID);
					room.RemovePlayer(clientState.player.ID);
				}
				DBManager.UpdatePlayerData(clientState.player.ID, clientState.player.playerDate);
				PlayerManger.RemovePlayer(clientState.player.ID);
			}
		}

		public static void OnTimer() {
			CheckPing();
			RoomManager.Update();
		}

		public static void CheckPing() {
			long curTime = NetManager.GetTimeStamp();

			foreach (ClientState item in NetManager.clients.Values)
			{
				if (item.lastPingTime - curTime >= NetManager.pingInterval * 4) {
					Console.WriteLine("没有接收到该连接的Pong消息，该连接关闭");
					NetManager.Close(item);
					return;
				}
			}
		}
    }
}

