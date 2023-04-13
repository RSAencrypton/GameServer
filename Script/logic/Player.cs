using System;
using GameServer.Script.net;
namespace GameServer.Script.logic
{
	public class Player
	{

		public string ID = "";
		public ClientState clientState;
		public float x;
		public float y;
		public float z;
		public float eulX;
		public float eulY;
		public float eulZ;
		public int camp = 1;
		public int roomID = -1;
		public int hp = 100;
		public PlayerData playerDate;
		public string Text;

		public Player(ClientState clientState) {
			this.clientState = clientState;
		}

		public void SendMsg(MsgBase msgBase) {
			NetManager.SendMsg(clientState, msgBase);
		}
	}
}

