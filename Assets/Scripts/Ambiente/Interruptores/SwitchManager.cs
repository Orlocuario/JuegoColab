using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour {

    List<GroupOfSwitchs> listOfGroups;
    public Sprite On11;
    public Sprite Off11;
    public Sprite On12;
    public Sprite Off12;
    public Sprite On21;
    public Sprite Off21;
    public Sprite On22;
    public Sprite Off22;
    public Sprite On31;
    public Sprite Off31;
    public Sprite On32;
    public Sprite Off32;
    public Sprite On01;
    public Sprite Off01;
    public Sprite On02;
    public Sprite Off02;    

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

    public void CallAction(int groupId)
    {
        foreach(GroupOfSwitchs group in listOfGroups)
        {
            if(group.groupId == groupId)
            {
                group.CallAction();
            }
        }
    }
}
