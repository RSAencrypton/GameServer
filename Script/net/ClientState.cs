using System;
using Tool;
using System.Net.Sockets;
using GameServer.Script.logic;
namespace GameServer.Script.net
{
	public class ClientState
	{
		public Socket socket;
		public ByteArray readbuffer = new ByteArray();
		public long lastPingTime = 0;

		public Player player;
	}
}

