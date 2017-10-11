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

	public void CheckIfReady(PlannerSwitch switchObj)
	{
		foreach(Switch switchi in switchs)
		{
			if (!switchi.on)
			{
				return;
			}
		}
		CallAction();
		if (switchObj != null) {
			switchObj.ActivateSwitch ();
		}
	}

    public List<Switch> GetSwitchs()
    {
        return switchs;
    }

    private void SendNewEventToServer()
    {
        string message = "SwitchGroupReady/" + groupId;
        Client.instance.SendMessageToServer(message);
    }

    public void CallAction()
    {
        SwitchActions handler = new SwitchActions(this);
        handler.DoSomething();
        SendNewEventToServer();
    }
}
