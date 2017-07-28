using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour {

	void Start ()
    {
        GameObject.Find("DisplayPanel").SetActive(false);
	}
	
	// Update is called once per frame
	public void ActionToDo()
    {
        if (Inventory.instance.actualItem.GetComponent<Image>() == null)
        {
            GrabItem();
        }
        else
        {
            UseItem();
        }
    }

    public void GrabItem()
    {
        
    }

    public void UseItem()
    {
        string actionToDo = Items.instance.ItemInformation()[1];

        switch (actionToDo)
        {
            case "climb":
                break;
            default:
                break;
        }
    }
}
