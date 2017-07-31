using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public const int numSlots = 8;

    public static Inventory instance;
    public Image[] items = new Image[numSlots];
    public GameObject displayPanel;
    public GameObject actualItem;
    public Text displayItemInfo;

    public void Start()
    {
        new Items();
        instance = this;
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
                return;
            }
            else
            {
                return;
            }
        }
    }

    public void RemoveItemFromInventory(GameObject itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove.GetComponent<Image>())
            {
                items[i].sprite = null;
                items[i] = null;
                items[i].enabled = false;
                //make server, drop or distroy or trade item (best is to drop and countdown to destroy) & make it gone in record
                return;
            }
        }
    }

    public void GetItemName(string slot)
    {
        Image itemImage = GameObject.Find("ItemImage" + slot).GetComponent<Image>();

        if (itemImage.sprite != null)
        {
            Items.instance.ItemsInGame(itemImage);
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

    public void UnselectItem()
    {
        Items.instance.itemName = null;
        Items.instance.itemId = 0;

        Image actualItemImage = actualItem.GetComponent<Image>();
        actualItemImage.sprite = null;
        actualItemImage = null;

        actualItem.SetActive(false);
        displayPanel.SetActive(false);
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return false;
            }
        }
        return true;
    }
}
