using System;
using Tool;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using GameServer.Script.logic;
using System.Net;
using System.Net.Sockets;
namespace GameServer.Script.net
{
	public static class NetManager
	{
		//监听sokcet
		public static Socket listenfd;
		//客户端socket和客户状态
		public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
		//select检查列表
		static List<Socket> checkList = new List<Socket>();

		//心跳机制
		public static int pingInterval = 10;

		public static void EnterLoop(int portID) {
			//Bind
			listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPAddress ipAdd = IPAddress.Parse("0.0.0.0");
			IPEndPoint ipEP = new IPEndPoint(ipAdd, portID);
			listenfd.Bind(ipEP);
			//Listen
			listenfd.Listen(0);
			Console.WriteLine("服务器启动");

			while (true) {
				//重置checkRead
				ResetcheckRead();
				Socket.Select(checkList, null, null, 1000);

				for (int i = checkList.Count - 1; i >= 0; i--)
				{
					Socket socket = checkList[i];

					if (socket == listenfd)
					{
						ReadListenfd(socket);
					}
					else {
						ReadClientfd(socket);
					}
				}

				Timer();
			}
		}


		public static void ResetcheckRead() {
			checkList.Clear();
			checkList.Add(listenfd);
			foreach (ClientState item in clients.Values)
			{
				checkList.Add(item.socket);
			}
		}

		public static void ReadListenfd(Socket listenfd) {
			try
			{
				Socket clientfd = listenfd.Accept();
				ClientState clientState = new ClientState();
				clientState.socket = clientfd;
				clients.Add(clientfd, clientState);
				Console.WriteLine("接收一名用户");
			}
			catch (SocketException ex) {
				Console.WriteLine("服务器接受失败：" + ex.ToString());
			}
		}

		public static void ReadClientfd(Socket clientfd) {
			ClientState clientState = clients[clientfd];
			ByteArray readBuffer = clientState.readbuffer;
			int count = 0;
			if (readBuffer.remain <= 0) {
				ReceiveData(clientState);
				readBuffer.MoveBytes();
			}
			if (readBuffer.remain <= 0) {
				Console.WriteLine("Msg length is too large!!!!!");
				Close(clientState);
				return;
			}

			try
			{
				count = clientfd.Receive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.remain, 0);
			}
			catch (SocketException ex) {
				Console.WriteLine("服务器接收失败：" + ex.ToString());
				Close(clientState);
				return;
			}

			if (count <= 0) {
                Console.WriteLine("该远端连接关闭：" + clientfd.RemoteEndPoint.ToString());
                Close(clientState);
				return;
			}

			readBuffer.writeIndex += count;
			ReceiveData(clientState);
			readBuffer.CheckAndMoveBytes();
		}


		public static void Close(ClientState clientState) {
			MethodInfo methodInfo = typeof(logic.EventHandler).GetMethod("onDisconnect");
			Object[] objects = { clientState };
			methodInfo.Invoke(null, objects);

			clientState.socket.Close();
			clients.Remove(clientState.socket);
		}

		public static void ReceiveData(ClientState clientState) {
			ByteArray readBuffer = clientState.readbuffer;
			byte[] bytes = readBuffer.bytes;
            if (readBuffer.length <= 2) return;
			Int16 bodyLength = (Int16)((bytes[readBuffer.readIndex + 1] << 8 | bytes[readBuffer.readIndex]));
			if (readBuffer.length < bodyLength + 2) return;
			readBuffer.readIndex += 2;

			int nameCount = 0;
			string protoName = MsgBase.DecodeName(readBuffer.bytes, readBuffer.readIndex, out nameCount);

			if (protoName == "") {
				Console.WriteLine("无法解析协议名称，该连接关闭");
				//Close(clientState);
				return;
			}
			readBuffer.readIndex += nameCount;

			int bodyCount = bodyLength - nameCount;
			MsgBase msgBase = MsgBase.Decode(protoName, readBuffer.bytes, readBuffer.readIndex, bodyCount);
			readBuffer.readIndex += bodyCount;
			readBuffer.CheckAndMoveBytes();




			MethodInfo methodInfo = typeof(MsgHandler).GetMethod(protoName);
			Object[] objects = { clientState, msgBase };
			Console.WriteLine(msgBase.msgName + "协议解析成功");
			if (methodInfo != null)
			{
				methodInfo.Invoke(null, objects);
			}
			else {
				Console.WriteLine(protoName + "该协议没有被定义");
			}

			if (readBuffer.length > 2) {
                ReceiveData(clientState);
			}
		}

		static void Timer() {
			MethodInfo methodInfo = typeof(logic.EventHandler).GetMethod("OnTimer");

			Object[] objects = { };
			methodInfo.Invoke(null, objects);
		}

		public static void SendMsg(ClientState clientState, MsgBase msgBase) {
			if (clientState == null) {
				return;
			}

			if (!clientState.socket.Connected) {
				return;
			}

			byte[] nameBytes = MsgBase.EncodeName(msgBase);
			byte[] bodyBytes = MsgBase.Encode(msgBase);
			int len = nameBytes.Length + bodyBytes.Length;
			byte[] sendBytes = new byte[2 + len];

			sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);

			Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
			Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

			
			try
			{
				clientState.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
			}
			catch (SocketException ex) {
				Console.WriteLine("服务器发送失败：" + ex.ToString());
			}
        }

		public static long GetTimeStamp() {
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);

			return Convert.ToInt64(timeSpan.TotalSeconds);
		}
	}
}

