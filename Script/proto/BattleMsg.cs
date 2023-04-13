[System.Serializable]
public class TankInfo
{
    public string id = "";
    public int camp = 0;
    public int hp = 0;

    public float posX = 0;
    public float posY = 0;
    public float posZ = 0;
    public float eulX = 0;
    public float eulY = 0;
    public float eulZ = 0;
}

public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle() { msgName = "MsgEnterBattle"; }

    public TankInfo[] tanks;
    public int mapID = 1;
}

public class MsgBattleResult : MsgBase
{
    public MsgBattleResult() { msgName = "MsgBattleResult"; }
    public int winCamp = 0;
}

public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle() { msgName = "MsgLeaveBattle"; }
    public string id = "";
}


