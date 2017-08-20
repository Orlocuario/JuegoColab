using System;
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
    public GameObject actualItemSlot;
    public Text displayItemInfo;
    private int numSlot;

    public void Start()
    {
        new Items();
        //instance setteada desde el bag button
    }

    public void AddItemToInventory(GameObject itemToAdd)
    {
        if (!IsFull())
        {
            for (int i = 0; i < items.Length; i++)
            {
                Sprite actualItemSprite = items[i].GetComponent<Image>().sprite;
                if (actualItemSprite == null)
                {
                    items[i].sprite = itemToAdd.GetComponent<SpriteRenderer>().sprite;
                    items[i].enabled = true;

                    Client.instance.SendMessageToServer("InventoryUpdate/Add/" + i.ToString() + "/" + items[i].sprite.name);
                    UpdateInventory(items[i], i);
                    return;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void RemoveItemFromInventory(Image itemToRemove)
    {
        if (Client.instance.GetLocalPlayer().isGrounded)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == itemToRemove)
                {
                    Image actualImage = actualItemSlot.GetComponent<Image>();
                    actualImage.sprite = null;
                    items[i].sprite = null;
                    items[i].enabled = false;
                    actualItemSlot.SetActive(false);
                    displayPanel.SetActive(false);

                    Client.instance.SendMessageToServer("InventoryUpdate/Remove/" + i.ToString());
                    UpdateInventory(items[i], i);
                    return;
                }
            }
        }   
    }

    public void UpdateInventory(Image spriteImage, int i)
    {
        Image slotSprite = GameObject.Find("SlotSprite" + i.ToString()).GetComponent<Image>();
        slotSprite.sprite = spriteImage.sprite;
    }

    public void GetItemName(string slot)
    {
        numSlot = Int32.Parse(slot);
        Image itemImage = GameObject.Find("SlotSprite" + slot).GetComponent<Image>();

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

    public void DropItem()
    {
        displayPanel.SetActive(false);
        RemoveItemFromInventory(GameObject.Find("SlotSprite" + numSlot).GetComponent<Image>());
        string actualItemSpriteName = Items.instance.itemSprite.name;
        Client.instance.SendMessageToServer("CreateGameObject/" + actualItemSpriteName);
    }

    public void UnselectItem()
    {
        Items.instance.itemName = null;
        Items.instance.itemId = 0; //enviar id en vez de name propuesta a futuro

        Image actualImage = actualItemSlot.GetComponent<Image>();
        actualImage.sprite = null;

        actualItemSlot.SetActive(false);
        displayPanel.SetActive(false);
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].sprite == null)
            {
                return false;
            }
        }
        return true;
    }
}
