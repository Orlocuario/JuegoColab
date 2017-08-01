using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagButton : MonoBehaviour {

    public static BagButton instance;
    public GameObject inventory;

	void Start ()
    {
        instance = this;
        GameObject.Find("DisplayPanel").SetActive(false);
        inventory.SetActive(false);
    }
	
	// Update is called once per frame
	public void ActionToDo()
    {
        if (inventory.GetComponent<Inventory>().actualItem.GetComponent<Image>().sprite != null)
        {
            UseItem();
        }
        else
        {
            if (inventory.activeSelf == true)
            {
                inventory.SetActive(false);
            }
            else
            {
                inventory.SetActive(true);
            }
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
