using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour {

    public GameObject inventory;
    public GameObject canvas;

	void Start ()
    {
        GameObject.Find("DisplayPanel").SetActive(false);
        Inventory.instance = inventory.GetComponent<Inventory>();
    }

    // Update is called once per frame
    public void ActionToDo()
    {
        if (inventory.GetComponent<Inventory>().actualItemSlot.GetComponent<Image>().sprite != null)
        {
            UseItem();
        }
        else
        {
            Debug.Log("asd");
        }
    }
    
    public void UseItem()
    {
        string who = Items.instance.ItemInformation()[0];
        string actionToDo = Items.instance.ItemInformation()[1];

        switch (who)
        {
            case "Everyone":
                EveryoneActions(actionToDo);
                break;
            case "Mage":
                MageActions(actionToDo);
                break;
            case "Warrior":
                WarriorActions(actionToDo);
                break;
            case "Engineer":
                EngineerActions(actionToDo);
                break;
            default:
                break;
        }
    }

    private void EveryoneActions(string actionToDo)
    {
        switch (actionToDo)
        {
            case "Climb":
                break;
            default:
                break;
        }
    }

    private void MageActions(string actionToDo)
    {
        throw new NotImplementedException();
    }

    private void WarriorActions(string actionToDo)
    {
        throw new NotImplementedException();
    }

    private void EngineerActions(string actionToDo)
    {
        throw new NotImplementedException();
    }
}
