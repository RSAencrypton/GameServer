using System;

    public class MsgRegister : MsgBase {
        public MsgRegister() { msgName = "MsgRegister"; }
        public string id = "";
        public string password = "";
        public int res = 0;
    }

    public class MsgLogIn : MsgBase {
        public MsgLogIn() { msgName = "MsgLogIn"; }
        public string id = "";
        public string password = "";
        public int res = 0;
    }

    public class MsgKick : MsgBase {
        public MsgKick() { msgName = "MsgKick"; }
        public int res = 0;
    }


