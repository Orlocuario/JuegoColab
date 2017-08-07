using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Items
{
    public static Items instance;

    public List<string> itemsInGame = new List<string>();
    public Sprite itemSprite;
    public string itemName;
    public int itemId;

    public Items()
    {
        instance = this;
        itemsInGame.Add("engranaje");
        itemsInGame.Add("car");
        itemsInGame.Add("Rope");
    }

    public void ItemsInGame(Image itemImage)
    {
        itemSprite = itemImage.sprite;
        itemName = itemSprite.name;
        string[] itemsInGameArray = itemsInGame.ToArray();

        for (int i = 0; i < itemsInGameArray.Length; i++)
        {
            if (itemsInGameArray[i] == itemName)
            {
                DisplayItemInfo(itemImage);
                return;
            }
        }
    }

    public void DisplayItemInfo(Image itemImage)
    {
        string itemInfo = ItemInformation()[1];
        Inventory.instance.displayItemInfo.text = "";
        Inventory.instance.displayItemInfo.text = "<color=#e67f84ff><b>" + "Usando '" + itemName + "': </b></color>" + "\r\n";
        Inventory.instance.displayItemInfo.text += "<color=#f9ca45ff>" + itemInfo + "</color>";

        Image actualItemImage = Inventory.instance.actualItemSlot.GetComponent<Image>();
        actualItemImage.sprite = itemImage.sprite;

        Inventory.instance.displayPanel.SetActive(true);
        Inventory.instance.actualItemSlot.SetActive(true);
        /*Call this variable from a script from the room to interact with it,
        like the ChatZone (not monobehaviour). Try switch and case for all items */
    }

    public string[] ItemInformation()
    {
        string info = "";
        string actionToDo = "";

        switch (itemName)
        {//itemId = 0 implies the fact that there is no item selected item
            case ("car"):
                info = "Un medio de transporte";
                actionToDo = "Everyone/SpeedUp";
                itemId = 1;
                break;
            case ("engranaje"):
                info = "Una pieza hecha para construir armas";
                actionToDo = "Engineer/Create1";
                itemId = 2;
                break;
            case ("Rope"):
                info = "Una cuerda para llegar alto";
                actionToDo = "Everyone/Climb";
                itemId = 3;
                break;
            //etc;
            default:
                break;
        }

        string[] itemInfo;
        itemInfo = new string[2] { actionToDo, info };
        return itemInfo;
    }
}
