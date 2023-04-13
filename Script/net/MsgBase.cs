using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;

public class MsgBase
{
    public string msgName = "null";

    //JSON格式进行编码和解码
    public static byte[] Encode(MsgBase msgBase) {
        string str = JsonConvert.SerializeObject(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(str);
    }
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count) {
        string str = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        Console.WriteLine(str);
        Type proto = Type.GetType(protoName);
        var msgBase = JsonConvert.DeserializeObject(str, proto);
        return (MsgBase)msgBase;
    }

    //JSON格式编码和解码协议名
    public static byte[] EncodeName(MsgBase msgBase) {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.msgName);
        Int16 bytesLen = (Int16)nameBytes.Length;

        byte[] bytes = new byte[2 + bytesLen];
        bytes[0] = (byte)(bytesLen % 256);
        bytes[1] = (byte)(bytesLen / 256);

        Array.Copy(nameBytes, 0, bytes, 2, bytesLen);

        return bytes;
    }

    public static string DecodeName(byte[] bytes, int offset, out int count) {
        count = 0;

        if (2 + offset > bytes.Length) {
            return "";
        }

        Int16 bytesLen = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);

        if (bytesLen <= 0) return "";

        if (offset + 2 + bytesLen > bytes.Length) {
            Console.WriteLine(bytes.Length);
            return "";
        }

        count = 2 + bytesLen;
        return System.Text.Encoding.UTF8.GetString(bytes, offset + 2, bytesLen);
    }
}
