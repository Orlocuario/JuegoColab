using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour {

    public static BagButton instance;

	void Start ()
    {
        instance = this;
        GameObject.Find("DisplayPanel").SetActive(false);
	}
	
	// Update is called once per frame
	public void ActionToDo()
    {
        if (Inventory.instance.actualItem.GetComponent<Image>().sprite != null)
        {
            UseItem();
        }
        else
        {
            return;
        }
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
