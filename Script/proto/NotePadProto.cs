using System;

    public class MsgGetText : MsgBase {
        public MsgGetText() { msgName = "MsgGetText"; }
        public string getText = "";
    }

    public class MsgSaveText : MsgBase {
        public MsgSaveText() { msgName = "MsgSaveText"; }
        public string saveText = "";
        public int res = 0;
    }


