using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    public static Items instance;
    public List<string> itemsInGame;

    public void Start()
    {
        instance = this;
    }
    
    public void ItemsInGame(string itemName)
    {
        string[] itemsInGameArray = itemsInGame.ToArray();
        for (int i = 0; i < itemsInGameArray.Length; i++)
        {
            if (itemsInGameArray[i] == itemName)
            {
                DisplayItemInfo(itemName);
                return;
            }
        }
    }

    public void DisplayItemInfo(string itemName)
    {
        string info = "";
        switch (itemName)
        {
            case ("Engranaje"):
                info = "Una pieza hecha para construir armas";
                break;
            //etc;
        }
        Inventory.instance.displayItemInfo.text = "<b><color=E67F84FF>" + itemName + "</color></b>" + "\r\n" + "\r\n" + "<color=F9CA45FF>" + info + "</color>";
        Inventory.instance.displayPanel.SetActive(true);
    }

    public void AddItem(string item)
    {
        itemsInGame.Add(item);
    }

}
