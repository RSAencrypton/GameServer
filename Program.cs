using System;
using Newtonsoft.Json;
using GameServer.Script.net;
using GameServer.Script.db;
using GameServer.Script.logic;

namespace Game {
    class MainFunction {
        public static void Main(string[] args) {
            if (!DBManager.ConnectSQL("localhost", "Game", "root", "")) return;
            NetManager.EnterLoop(8888);
        }
    }
}