using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour {

    List<GroupOfSwitchs> listOfGroups;

	// Use this for initialization
	void Awake () {
        listOfGroups = new List<GroupOfSwitchs>();
	}

	
    public Switch GetSwitch(int groupId, int individualId)
    {
        foreach(GroupOfSwitchs group in listOfGroups)
        {
            if(group.groupId == groupId)
            {
                return group.GetSwitch(individualId);
            }
        }
        return null;
    }

    public void Add(Switch switchi)
    {
        GroupOfSwitchs group = GetGroup(switchi.groupId);
        group.AddSwitch(switchi);
    }

    private GroupOfSwitchs GetGroup(int id)
    {
        foreach(GroupOfSwitchs group in listOfGroups)
        {
            if(group.groupId == id)
            {
                return group;
            }
        }
        return NewGroup(id);
    }

    private GroupOfSwitchs NewGroup(int id)
    {
        GroupOfSwitchs group = new GroupOfSwitchs(id);
        listOfGroups.Add(group);
        return group;
    }
}
