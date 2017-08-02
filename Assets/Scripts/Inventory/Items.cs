using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Items
{
    public static Items instance;

    public List<string> itemsInGame = new List<string>();
    public string itemName;
    public int itemId;

    public Items()
    {
        instance = this;
        itemsInGame.Add("engranaje");
        itemsInGame.Add("car");
        itemsInGame.Add("Elementos_29");
    }

    public void ItemsInGame(SpriteRenderer itemSpriteRenderer)
    {
        itemName = itemSpriteRenderer.sprite.name;
        string[] itemsInGameArray = itemsInGame.ToArray();

        for (int i = 0; i < itemsInGameArray.Length; i++)
        {
            if (itemsInGameArray[i] == itemName)
            {
                DisplayItemInfo(itemSpriteRenderer);
                return;
            }
        }
    }

    public void DisplayItemInfo(SpriteRenderer itemSpriteRenderer)
    {
        string itemInfo = ItemInformation()[0];
        Inventory.instance.displayItemInfo.text = "";
        Inventory.instance.displayItemInfo.text = "<color=#e67f84ff><b>" + "Usando '" + itemName + "': </b></color>" + "\r\n";
        Inventory.instance.displayItemInfo.text += "<color=#f9ca45ff>" + itemInfo + "</color>";

        Image actualItemImage = Inventory.instance.actualItem.GetComponent<Image>();
        actualItemImage.sprite = itemSpriteRenderer.sprite;

        Inventory.instance.displayPanel.SetActive(true);
        Inventory.instance.actualItem.SetActive(true);
        /*Call this variable from a script from the room to interact with it,
        like the ChatZone (not monobehaviour). Try switch and case for all items */
    }

    public string[] ItemInformation()
    {
        string info = "";
        string actionToDo = "";

        switch (itemName)
        {
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
            case ("Elementos_29"): //Rope
                info = "Una cuerda para llegar alto";
                actionToDo = "Everyone/climb";
                itemId = 3;
                break;
            //etc;
            default:
                break;
        }

        string[] itemInfo;
        itemInfo = new string[2] { info, actionToDo };
        return itemInfo;
    }
}
