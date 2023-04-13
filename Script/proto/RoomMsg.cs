using System.Collections;
using System.Collections.Generic;

public class MsgGetAchieve : MsgBase
{
    public MsgGetAchieve() { msgName = "MsgGetAchieve"; }
    public int win = 0;
    public int loss = 0;
}

[System.Serializable]
public class RoomInfo
{
    public int id;
    public int count;
    public int state;
}
public class MsgGetRoomList : MsgBase
{
    public MsgGetRoomList() { msgName = "MsgGetRoomList"; }
    public RoomInfo[] rooms;
}

public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom() { msgName = "MsgCreateRoom"; }
    public int res = 0;
}

public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom() { msgName = "MsgEnterRoom"; }
    public int id = 0;
    public int res = 0;
}

[System.Serializable]
public class PlayerInfo
{
    public string id = "hello";
    public int camp = 0;
    public int win = 0;
    public int loss = 0;
    public int isOwner = 0;
}

public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo() { msgName = "MsgGetRoomInfo"; }
    public PlayerInfo[] players;
}

public class MsgLeaveRoom : MsgBase
{
    public MsgLeaveRoom() { msgName = "MsgLeaveRoom"; }
    public int res = 0;
}

public class MsgStartBattle : MsgBase
{
    public MsgStartBattle() { msgName = "MsgStartBattle"; }
    public int res = 0;
}


