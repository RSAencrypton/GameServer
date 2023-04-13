using System.Collections;
using System.Collections.Generic;

public class MsgMove : MsgBase {
    public int posX { get; set; }
    public int posY { get; set; }
    public int posZ { get; set; }
    public MsgMove() { msgName = "MsgMove"; }
}

public class MsgAttack : MsgBase {
    public MsgAttack() { msgName = "MsgAttack"; }
    public string des = "127.0.0.1:6543";
}
