using System;
using GameServer.Script.db;
using GameServer.Script.net;
namespace GameServer.Script.logic
{

    public partial class MsgHandler {
        public static void MsgRegister(ClientState clientState, MsgBase msgBase) {
            //注册账户，如果账户信息合规，则存入数据库
            //创建角色，把角色录入到角色数据库中
            MsgRegister msgRegister = (MsgRegister)msgBase;

            if (DBManager.RegisterAccount(msgRegister.id, msgRegister.password))
            {
                DBManager.CreatePlayer(msgRegister.id);
                msgRegister.res = 0;
            }
            else {
                msgRegister.res = 1;
            }

            NetManager.SendMsg(clientState, msgRegister);
        }

        public static void MsgLogIn(ClientState clientState, MsgBase msgBase) {
            //验证密码
            //登录状态判断
            //踢下线
            //读取玩家数据
            //构建player类
            MsgLogIn msgLogIn = (MsgLogIn)msgBase;

            if (!DBManager.CheckPassword(msgLogIn.id, msgLogIn.password)) {
                msgLogIn.res = 1;
                NetManager.SendMsg(clientState, msgLogIn);
                return;
            }

            if (clientState.player != null) {
                msgLogIn.res = 1;
                NetManager.SendMsg(clientState, msgLogIn);
                return;
            }

            if (PlayerManger.IsOnline(msgLogIn.id)) {
                Player other = PlayerManger.GetPlayer(msgLogIn.id);
                MsgKick msgKick = new MsgKick();
                msgKick.res = 0;
                other.SendMsg(msgKick);
                NetManager.Close(other.clientState);
            }

            PlayerData playerData = DBManager.GetPlayerDataByID(msgLogIn.id);
            if (playerData == null) {
                msgLogIn.res = 1;
                NetManager.SendMsg(clientState, msgLogIn);
                return;
            }

            Player player = new Player(clientState);
            player.ID = msgLogIn.id;
            player.playerDate = playerData;
            PlayerManger.AddPlayer(msgLogIn.id, player);
            clientState.player = player;

            msgLogIn.res = 0;
            player.SendMsg(msgLogIn);
        }
    }
}

