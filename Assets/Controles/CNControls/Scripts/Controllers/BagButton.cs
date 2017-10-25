using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour {

    public static BagButton instance;
    public GameObject inventory;
    public GameObject canvas;

    public bool usingRune;
    public bool usingEngranaje;

	void Start ()
    {
        instance = this;
        usingRune = false;
        usingEngranaje = false;
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
        string actionToDo = Items.instance.ItemInformation()[0];
        char[] separator = new char[1];
        separator[0] = '/';
        string[] msg = actionToDo.Split(separator);

        switch (msg[0])
        {
            case "Everyone":
                EveryoneActions(msg[1]);
                break;
            case "Mage":
                MageActions(msg[1]);
                break;
            case "Warrior":
                WarriorActions(msg[1]);
                break;
            case "Engineer":
                EngineerActions(msg[1]);
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
            case "RuneOpenDoors":
                usingRune = true;
                break;
            default:
                break;
        }
    }

    private void MageActions(string actionToDo)
    {
        if (Client.instance.GetMage().localPlayer)
        {
            switch (actionToDo)
            {
                case "Climb":
                    break;
                case "RuneOpenDoors":
                    usingRune = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void WarriorActions(string actionToDo)
    {
        if (Client.instance.GetWarrior().localPlayer)
        {
            switch (actionToDo)
            {
                case "Climb":
                    break;
                case "RuneOpenDoors":
                    usingRune = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void EngineerActions(string actionToDo)
    {
        if (Client.instance.GetEngineer().localPlayer)
        {
            switch (actionToDo)
            {
                case "Climb":
                    break;
                case "FixMachine":
                    usingEngranaje = true;
                    break;
                case "RuneOpenDoors":
                    usingRune = true;
                    break;
                default:
                    break;
            }
        }
    }
}
