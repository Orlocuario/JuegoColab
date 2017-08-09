using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneSystem : MonoBehaviour
{
    LevelManager levelManager;
    Vector2 myPosition;
    public const int numOfRunesForDoor = 3;
    public List<string> runesThatDoorRequires = new List<string>();

    private List<string> runesThatTheDoorHas = new List<string>();
    private string[] runesThatTheDoorRequiresArray;
    private string[] runesThatTheDoorHasArray;
    private string[] runes = new string[numOfRunesForDoor];
    private bool lockValue;
    private bool doorHasBeenChecked;

    private void Start()
    {
        lockValue = false;
        doorHasBeenChecked = false;
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        for (int i = 0; i < runes.Length; i++)
        {
            if (runes[i] != null)
            {
                runesThatDoorRequires.Add(runes[i]);
            }
        }

        runesThatTheDoorRequiresArray = runesThatDoorRequires.ToArray();
    }

    private void Update()
    {
        PlayerController player = levelManager.thePlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance <= 0.4f && BagButton.instance.usingRune &&!RuneDoorIsFull())
        {
            lockValue = true;
            string itemName = Items.instance.itemName;
            for (int i = 0; i < runesThatTheDoorRequiresArray.Length; i++)
            {
                if (runesThatTheDoorRequiresArray[i] == itemName && !runesThatTheDoorHas.Contains(itemName))
                {
                    runesThatTheDoorHas.Add(itemName);
                    runesThatTheDoorHasArray = runesThatTheDoorHas.ToArray();

                    Sprite door = this.gameObject.GetComponent<SpriteRenderer>().sprite;
                    //door = GetDoorSprite(runesThatTheDoorHasArray);

                    BagButton.instance.usingRune = false;
                    Inventory.instance.RemoveItemFromInventory(GetItemImage(itemName));
                }
            }
        }
        else
        {
            BagButton.instance.usingRune = false;
            if (lockValue)
            {
                lockValue = false;
            }

            if (RuneDoorIsFull() && !doorHasBeenChecked)
            {
                //ActivateDoorFunction();
            }
        }
    }

    private Image GetItemImage(string itemName)
    {
        Image[] items = Inventory.instance.items;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].sprite.name == itemName)
            {
                return items[i];
            }
        }
        return null;
    }

    /*private Sprite GetDoorSprite(string[] runesThatTheDoorHas)
    {
        Sprite newSprite;

        return newSprite;
    }*/

    public bool RuneDoorIsFull()
    {
        if (runesThatTheDoorHasArray != null)
        {
            for (int i = 0; i < runesThatTheDoorHasArray.Length; i++)
            {
                if (runesThatTheDoorHasArray[i] == null)
                {
                    return false;
                }
            }
            doorHasBeenChecked = true;
            return true;
        }
        return false;
    }
}
