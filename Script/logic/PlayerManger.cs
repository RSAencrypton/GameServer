using System;
using System.Collections.Generic;
namespace GameServer.Script.logic
{
	public static class PlayerManger
	{
		static Dictionary<string, Player> playerList = new Dictionary<string, Player>();

		public static bool IsOnline(string id) {
			return playerList.ContainsKey(id);
		}

		public static Player GetPlayer(string id) {
			Player player = null;
			playerList.TryGetValue(id, out player);
			return player;
		}

		public static bool AddPlayer(string id, Player player) {
			return playerList.TryAdd(id, player);
		}

		public static void RemovePlayer(string id) {
			playerList.Remove(id);
		}
	}
}

