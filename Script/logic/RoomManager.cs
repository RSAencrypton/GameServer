using System;

public class RoomManager
{
    private static int maxID = 1;
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

    public static Room GetRoom(int id) {
        if (rooms.ContainsKey(id)) {
            return rooms[id];
        }

        return null;
    }

    public static Room AddRoom() {
        maxID++;
        Room room = new Room();
        room.id = maxID;
        rooms.Add(room.id, room);
        return room;
    }

    public static bool RemoveRoom(int id) {
        rooms.Remove(id);
        return true;
    }

    public static MsgBase ToMsg() {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = rooms.Count;
        msg.rooms = new RoomInfo[count];

        int index = 0;
        foreach (Room item in rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo();

            roomInfo.id = item.id;
            roomInfo.count = item.playerIDs.Count;
            roomInfo.state = (int)item.state;

            msg.rooms[index] = roomInfo;
            index++;
        }

        return msg;
    }

    public static void Update() {
        foreach (Room item in rooms.Values)
        {
            item.Update();
        }
    }
}

