using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupOfSwitchs{

    public int size;
    private List<Switch> switchs;
    public int groupId;

    public GroupOfSwitchs(int groupId)
    {
        switchs = new List<Switch>();
        size = 0;
        this.groupId = groupId;
    }

    public void AddSwitch(Switch switchi)
    {
        switchi.switchGroup = this;
        switchs.Add(switchi);
        size++;
    }

    public Switch GetSwitch(int id)
    {
        foreach(Switch switchi in switchs)
        {
            if(switchi.individualId == id)
            {
                return switchi;
            }
        }
        return null;
    }

    public void CheckIfReady()
    {
        foreach(Switch switchi in switchs)
        {
            if (!switchi.on)
            {
                return;
            }
        }
        SwitchActions handler = new SwitchActions(this);
        handler.DoSomething();
    }

    public List<Switch> GetSwitchs()
    {
        return switchs;
    }
}
