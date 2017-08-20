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
        itemsInGame.Add("RunaA1");
        itemsInGame.Add("RunaA2");
        itemsInGame.Add("RunaA3");
        itemsInGame.Add("RunaA4");
        itemsInGame.Add("RunaA5");
        itemsInGame.Add("RunaM1");
        itemsInGame.Add("RunaM2");
        itemsInGame.Add("RunaM3");
        itemsInGame.Add("RunaM4");
        itemsInGame.Add("RunaM5");
        itemsInGame.Add("RunaR1");
        itemsInGame.Add("RunaR2");
        itemsInGame.Add("RunaR3");
        itemsInGame.Add("RunaR4");
        itemsInGame.Add("RunaR5");
        itemsInGame.Add("RunaV1");
        itemsInGame.Add("RunaV2");
        itemsInGame.Add("RunaV3");
        itemsInGame.Add("RunaV4");
        itemsInGame.Add("RunaV5");
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
            case ("RunaA1"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 4;
                break;
            case ("RunaA2"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 5;
                break;
            case ("RunaA3"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 6;
                break;
            case ("RunaA4"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 7;
                break;
            case ("RunaA5"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 8;
                break;
            case ("RunaM1"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 9;
                break;
            case ("RunaM2"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 10;
                break;
            case ("RunaM3"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 11;
                break;
            case ("RunaM4"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 12;
                break;
            case ("RunaM5"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 13;
                break;
            case ("RunaR1"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 14;
                break;
            case ("RunaR2"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 15;
                break;
            case ("RunaR3"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 16;
                break;
            case ("RunaR4"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 17;
                break;
            case ("RunaR5"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 18;
                break;
            case ("RunaV1"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 19;
                break;
            case ("RunaV2"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 20;
                break;
            case ("RunaV3"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 21;
                break;
            case ("RunaV4"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 22;
                break;
            case ("RunaV5"):
                info = "Una piedra para abrir puertas mágicas";
                actionToDo = "Everyone/RuneOpenDoors";
                itemId = 23;
                break;
            //etc;
            default:
                break;
        }

        string[] itemInfo;
        itemInfo = new string[3] { actionToDo, info, itemName };
        return itemInfo;
    }
}
