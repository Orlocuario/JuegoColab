﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions {

    int groupId;

    public SwitchActions(GroupOfSwitchs group)
    {
        this.groupId = group.groupId;
        foreach(Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

	public void DoSomething()
    {
        switch(groupId)
        {
            case 0:
                GameObject.Find("ArbolPlatBut0").SetActive(true);
                break;
            case 1:
                //asdada
                break;
            default:
                break;
        }
    }
}
