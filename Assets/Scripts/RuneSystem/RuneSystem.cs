using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneSystem : MonoBehaviour
{
    LevelManager levelManager;
    Vector2 myPosition;
    GameObject createGameObject;
    public const int numOfRunesForDoor = 4;
    public List<string> runesThatDoorRequires = new List<string>();
    List<string> runesThatTheDoorHas = new List<string>();

    string[] runesThatTheDoorRequiresArray;
    string[] runesThatTheDoorHasArray;
    string[] runes = new string[numOfRunesForDoor];
    bool lockValue;

    private void Start()
    {
        lockValue = false;
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

        if (distance <= 0.4f && BagButton.instance.usingRune && !RuneDoorIsFull())
        {
            lockValue = true;
            string itemName = Items.instance.itemName;
            for (int i = 0; i < runesThatTheDoorRequiresArray.Length; i++)
            {
                if (runesThatTheDoorRequiresArray[i] == itemName)
                {
                    runesThatTheDoorHas.Add(itemName);
                    runesThatTheDoorHasArray = runesThatTheDoorHas.ToArray();

                    Sprite door = this.gameObject.GetComponent<SpriteRenderer>().sprite;
                    door = GetDoorSprite(runesThatTheDoorHasArray);

                    BagButton.instance.usingRune = false;
                    Inventory.instance.RemoveItemFromInventory(Inventory.instance.items[i]);
                }
            }

        }
        else
        {
            if (lockValue)
            {
                lockValue = false;
            }

            if (RuneDoorIsFull())
            {
                ActivateDoorFunction();
            }
        }
    }

    private Sprite GetDoorSprite(string[] runesThatTheDoorHas)
    {
        Sprite newSprite;

        return newSprite;
    }

    public bool RuneDoorIsFull()
    {
        for (int i = 0; i < runesThatTheDoorHasArray.Length; i++)
        {
            if (runesThatTheDoorHasArray[i] == null)
            {
                return false;
            }
        }
        return true;
    }
}
