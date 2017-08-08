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

	void Start ()
    {
        instance = this;
        usingRune = false;
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
        string[] arreglo = actionToDo.Split(separator);

        switch (arreglo[0])
        {
            case "Everyone":
                EveryoneActions(arreglo[1]);
                break;
            case "Mage":
                MageActions(arreglo[1]);
                break;
            case "Warrior":
                WarriorActions(arreglo[1]);
                break;
            case "Engineer":
                EngineerActions(arreglo[1]);
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
