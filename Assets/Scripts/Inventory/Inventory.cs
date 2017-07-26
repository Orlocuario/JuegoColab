using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public const int numSlots = 8;
    public Image[] items = new Image[numSlots];
    public GameObject displayPanel;
    public Text displayItemInfo;

    public void Start()
    {
        instance = this;
        displayPanel.SetActive(false);
    }

    public void AddItemToInventory(GameObject itemToAdd)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd.GetComponent<Image>();
                items[i].sprite = itemToAdd.GetComponent<Sprite>();
                items[i].enabled = true;
                //make server distroy item when player collides with object in server
                return;
            }
        }
    }

    public void RemoveItemOfInventory(GameObject itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove.GetComponent<Image>())
            {
                items[i] = null;
                items[i].sprite = null;
                items[i].enabled = false;
                //make server, drop or distroy or trade item (best is to drop and countdown to destroy)
                return;
            }
        }
    }

    public void GetItemName(string slot)
    {
        Sprite itemSprite = GameObject.Find("Slot" + slot).GetComponentInChildren<Image>().sprite;

        if (itemSprite != null)
        {
            Items.instance.ItemsInGame(itemSprite.name);
        }
        else
        {
            return;
        }
    }

    public void ToggleDisplayPanelOff()
    {
        displayPanel.SetActive(false);
    }
}
