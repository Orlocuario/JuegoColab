using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneSystem : MonoBehaviour
{
    LevelManager levelManager;
    Vector2 myPosition;
    // public const int numOfRunesForDoor = 3;
    public List<string> runesThatDoorRequires = new List<string>();
    public GameObject[] runeSlots;
    public Sprite doorIsOpen;

    private string[] runesThatDoorRequiresArray;
    private List<string> runesThatTheDoorHas = new List<string>();
    private string[] runesThatTheDoorHasArray = new string[3];
    // private string[] runes = new string[numOfRunesForDoor];
    private bool lockValue;
    private bool doorHasBeenChecked;

    private void Start()
    {
        lockValue = false;
        doorHasBeenChecked = false;
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        runesThatDoorRequiresArray = runesThatDoorRequires.ToArray();
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
            for (int i = 0; i < runesThatDoorRequiresArray.Length; i++)
            {
                if (runesThatDoorRequiresArray[i] == itemName && !runesThatTheDoorHas.Contains(itemName))
                {
                    runesThatTheDoorHas.Add(itemName);
                    for (int j = 0; j < runesThatTheDoorHasArray.Length; j++)
                    {
                        if (runesThatTheDoorHasArray[i] == null)
                        {
                            runesThatTheDoorHasArray[i] = itemName;
                            break;
                        }
                    }
                    runesThatDoorRequires.Remove(itemName);
                    runesThatDoorRequiresArray = runesThatDoorRequires.ToArray();

                    BagButton.instance.usingRune = false;
                    AddRuneSpriteToDoor(GetItemImage(itemName).GetComponent<Image>());
                    UpdateDoorSprite(GetItemImage(itemName).GetComponent<Image>(), i);
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

            if (RuneDoorIsFull() && doorHasBeenChecked)
            {
                doorHasBeenChecked = false;
                ActionToDo(this.gameObject.name);
            }
        }
    }

    private void UpdateDoorSprite(Image spriteImage, int i)
    {
        SpriteRenderer slotSprite = GameObject.Find("RuneSlot" + i.ToString()).GetComponent<SpriteRenderer>();
        slotSprite.sprite = spriteImage.sprite;
    }

    private void AddRuneSpriteToDoor(Image spriteImage)
    {
        for (int i = 0; i < runeSlots.Length; i++)
        {
            if (runeSlots[i].GetComponent<SpriteRenderer>().sprite == null)
            {
                SpriteRenderer runeSlotSprite = runeSlots[i].GetComponent<SpriteRenderer>();
                runeSlotSprite.sprite = spriteImage.sprite;
            }
        }
        return;
    }

    private Image GetItemImage(string itemName) //For Removing item from inventory
    {
        Image[] items = Inventory.instance.items;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].sprite != null && items[i].sprite.name == itemName)
            {
                return items[i];
            }
        }
        return null;
    }

    public bool RuneDoorIsFull()
    {
        if (runesThatTheDoorHas != null) // W/O this it crashes the if !RuneDoorFull()
        {
            for (int i = 0; i < runesThatDoorRequiresArray.Length; i++)
            {
                if (runesThatDoorRequiresArray[i] != null)
                {
                    return false;
                }
            }
            doorHasBeenChecked = true;
            return true;
        }
        return false;
    }

    public void ActionToDo(string doorName)
    {
        if (doorName == "DoorN0")
        {
            this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            SpriteRenderer doorSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer[] doorSlotSpriteRenderer = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < doorSlotSpriteRenderer.Length; i++)
            {
                doorSlotSpriteRenderer[i].sprite = null;
            }
            doorSpriteRenderer.sprite = doorIsOpen;
        }
    }
}
