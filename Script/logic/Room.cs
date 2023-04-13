using System;
using GameServer.Script.logic;
using GameServer.Script.net;

public class Room
{
    public int id = 0;
    public int maxPlayer = 2;
    public Dictionary<string, bool> playerIDs = new Dictionary<string, bool>();
    public string ownerID = "";
    public enum State {
        PREPARE = 0,
        FIGHT = 1
    }

    public State state = State.PREPARE;

    static float[,,] startPoints = new float[2, 1, 6] {
        {
            { 64f, 0f, 62f, 0f, -135f, 0f}
        },
        {
            { -57f, 0f, -58f, 0f, 45f, 0f}
        },
    };

    private void SetBirthPos(Player player, int index) {
        int camp = player.camp;

        player.x = startPoints[camp - 1, index, 0];
        player.y = startPoints[camp - 1, index, 1];
        player.z = startPoints[camp - 1, index, 2];
        player.eulX = startPoints[camp - 1, index, 3];
        player.eulY = startPoints[camp - 1, index, 4];
        player.eulZ = startPoints[camp - 1, index, 5];
    }

    private void ResetPlayer() {
        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            if (player.camp == 1)
            {
                SetBirthPos(player, 0);
            }
            else {
                SetBirthPos(player, 0);
            }
        }

        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);
            player.hp = 100;
        }
    }

    public bool AddPlayer(string id) {
        Player player = PlayerManger.GetPlayer(id);

        if (player == null) {
            Console.WriteLine("获取不到玩家数据");
            return false;
        }

        if (playerIDs.Count >= maxPlayer) {
            Console.WriteLine("超过房间最大人数");
            return false;
        }

        if (state != State.PREPARE) {
            Console.WriteLine("游戏已经开始");
            return false;
        }

        playerIDs[id] = true;
        player.camp = SwitchCamp();
        player.roomID = this.id;
        if (ownerID == "") {
            ownerID = player.ID;
        }

        BroadCast(ToMsg());
        return true;
    }

    public int SwitchCamp() {
        int count1 = 0;
        int count2 = 0;

        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            if (player.camp == 1) count1++;
            if (player.camp == 2) count2++;
        }

        if (count1 <= count2)
        {
            return 1;
        }
        else {
            return 2;
        }
    }

    public bool isOwner(Player player) {
        return player.ID == ownerID;
    }

    public string SwitchOwner() {
        foreach (string item in playerIDs.Keys)
        {
            return item;
        }

        return "";
    }

    public void BroadCast(MsgBase msgBase) {
        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);
            player.SendMsg(msgBase);
        }
    }

    public MsgBase ToMsg() {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerIDs.Count;
        msg.players = new PlayerInfo[count];
        int index = 0;
        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);
            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.id = player.ID;
            playerInfo.camp = player.camp;
            playerInfo.win = player.playerDate.win;
            playerInfo.loss = player.playerDate.loss;
            playerInfo.isOwner = 0;
            if (isOwner(player)) playerInfo.isOwner = 1;

            msg.players[index] = playerInfo;
            index++;
        }

        return msg;
    }
    public bool RemovePlayer(string id) {
        Player player = PlayerManger.GetPlayer(id);

        if (player == null) {
            Console.WriteLine("删除角色失败，没有角色数据");
            return false;
        }

        if (!playerIDs.ContainsKey(id)) {
            Console.WriteLine("删除角色失败，角色不在当前房间");
            return false;
        }

        playerIDs.Remove(id);
        player.camp = 0;
        player.roomID = -1;

        if (state == State.FIGHT) {
            player.playerDate.loss++;
            MsgLeaveBattle msg = new MsgLeaveBattle();
            msg.id = player.ID;
            BroadCast(msg);
        }

        if (playerIDs.Count == 0) {
            RoomManager.RemoveRoom(this.id);
        }

        BroadCast(ToMsg());
        return true;
    }

    public bool CanStartBattle() {
        if (state != State.PREPARE) {
            return false;
        }

        int count1 = 0;
        int count2 = 0;

        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            if (player.camp == 1) { count1++; }
            else { count2++; }
        }

        if (count1 < 1 || count2 < 1)
        {
            return false;
        }

        return true;
    }


    public TankInfo PlayerToTankInfo(Player player) {
        TankInfo tankInfo = new TankInfo();

        tankInfo.posX = player.x;
        tankInfo.posY = player.y;
        tankInfo.posZ = player.z;
        tankInfo.eulX = player.eulX;
        tankInfo.eulY = player.eulY;
        tankInfo.eulZ = player.eulZ;

        tankInfo.hp = player.hp;
        tankInfo.id = player.ID;
        tankInfo.camp = player.camp;

        return tankInfo;
    }

    public bool StartBattle() {
        if (!CanStartBattle()) {
            return false;
        }

        state = State.FIGHT;
        ResetPlayer();

        MsgEnterBattle msg = new MsgEnterBattle();
        msg.mapID = 1;
        msg.tanks = new TankInfo[playerIDs.Count];

        int i = 0;
        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            msg.tanks[i] = PlayerToTankInfo(player);
            i++;
        }

        BroadCast(msg);
        return true;
    }

    public bool IsDie(Player player) {
        return player.hp <= 0 ? true : false;
    }

    public int EndGame() {
        int count1 = 0;
        int count2 = 0;

        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            if (!IsDie(player)) {
                if (player.camp == 1) count1++;
                if (player.camp == 2) count2++;
            }
        }

        if (count1 <= 0) {
            return 2;
        }

        if (count2 <= 0) {
            return 1;
        }

        return 0;
    }

    private long lastJudgeTime = 0;
    public void Update() {
        if (state != State.FIGHT) {
            return;
        }

        if (NetManager.GetTimeStamp() - lastJudgeTime < 10f) {
            return;
        }

        lastJudgeTime = NetManager.GetTimeStamp();

        int winCamp = EndGame();

        state = State.PREPARE;

        foreach (string item in playerIDs.Keys)
        {
            Player player = PlayerManger.GetPlayer(item);

            if (player.camp == 1) { player.playerDate.win++; }
            else { player.playerDate.loss++; }
        }

        MsgBattleResult msg = new MsgBattleResult();
        msg.winCamp = winCamp;
        BroadCast(msg);
    }
}


