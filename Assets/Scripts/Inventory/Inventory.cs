using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public const int numSlots = 8;

    public static Inventory instance;
    public SpriteRenderer[] items = new SpriteRenderer[numSlots];
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
            Sprite sprite = items[i].sprite;
            if (sprite == null)
            {
                items[i] = itemToAdd.GetComponent<SpriteRenderer>();
                items[i].sprite = itemToAdd.GetComponent<SpriteRenderer>().sprite;
                items[i].enabled = true;
                return;
            }
        }
        return;
    }

    public void RemoveItemFromInventory(GameObject itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove.GetComponent<SpriteRenderer>())
            {
                items[i].sprite = null;
                items[i] = null;
                items[i].enabled = false;
                //make server drop and record it
                return;
            }
        }
    }

    public void GetItemName(string slot)
    {
        SpriteRenderer itemSpriteRenderer = GameObject.Find("Sprite" + slot).GetComponent<SpriteRenderer>();

        if (itemSpriteRenderer.sprite != null)
        {
            Items.instance.ItemsInGame(itemSpriteRenderer);
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

        SpriteRenderer actualSpriteRenderer = actualItem.GetComponent<SpriteRenderer>();
        actualSpriteRenderer.sprite = null;
        actualSpriteRenderer = null;

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
