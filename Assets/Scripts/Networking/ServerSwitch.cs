using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSwitch {
    public bool on;
    public int groupId;
    public int individualId;
    public Room room;

    public ServerSwitch(int groupId, int individualId, Room room)
    {
        this.groupId = groupId;
        this.individualId = individualId;
        this.room = room;
    }

}
