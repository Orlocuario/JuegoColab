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
        itemsInGame.Add("Engranaje");
        itemsInGame.Add("car");
    }

    public void ItemsInGame(Image itemImage)
    {
        itemName = itemImage.sprite.name;
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
        string itemInfo = ItemInformation()[0];
        Inventory.instance.displayItemInfo.text = "";
        Inventory.instance.displayItemInfo.text = "<color=#e67f84ff><b>" + "Usando " + itemName + ": </b></color>" + "\r\n";
        Inventory.instance.displayItemInfo.text += "<color=#f9ca45ff>" + itemInfo + "</color>";

        Image actualItemImage = Inventory.instance.actualItem.GetComponent<Image>();
        actualItemImage.sprite = itemImage.sprite;

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
                //actionToDo = "";
                itemId = 1;
                break;
            case ("Engranaje"):
                info = "Una pieza hecha para construir armas";
                //actionToDo = "";
                itemId = 2;
                break;
            case ("rope"):
                info = "Una cuerda para llegar alto";
                actionToDo = "climb";
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
