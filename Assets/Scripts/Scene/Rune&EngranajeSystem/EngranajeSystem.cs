using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngranajeSystem : MonoBehaviour
{
    LevelManager levelManager;
    Vector2 myPosition;
    public List<string> engranajeThatMaquinaRequires = new List<string>();
    public GameObject[] maquinaSlots;
    public Sprite maquinaIsOpen;

    private string[] engranajesThatMaquinaRequiresArray;
    private List<string> engranajesThatMaquinaHas = new List<string>();
    private string[] engranajesThatMaquinaHasArray = new string[3];
    private bool lockValue;
    private bool engranajeHasBeenChecked;
	public PlannerSwitch switchObj = null;
    Animator machineAnim;


    private void Start()
    {
        lockValue = false;
        engranajeHasBeenChecked = false;
        machineAnim = this.gameObject.GetComponent<Animator>();
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        engranajesThatMaquinaRequiresArray = engranajeThatMaquinaRequires.ToArray();
    }

    private void Update()
    {

        if (levelManager.localPlayer == null)
        {
            return;
        }

        PlayerController player = levelManager.localPlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance <= 0.4f && BagButton.instance.usingEngranaje)
        {
            lockValue = true;
            string itemName = Items.instance.itemName;
            for (int i = 0; i < engranajesThatMaquinaRequiresArray.Length; i++)
            {
                if (engranajesThatMaquinaRequiresArray[i] == itemName && !engranajesThatMaquinaHas.Contains(itemName))
                {
                    engranajesThatMaquinaHas.Add(itemName);
                    for (int j = 0; j < engranajesThatMaquinaHasArray.Length; j++)
                    {
                        if (engranajesThatMaquinaHasArray[i] == null)
                        {
                            engranajesThatMaquinaHasArray[i] = itemName;
                            break;
                        }
                    }
                    engranajeThatMaquinaRequires.Remove(itemName);
                    engranajesThatMaquinaRequiresArray = engranajeThatMaquinaRequires.ToArray();

                    BagButton.instance.usingEngranaje = false;
                    AddEngranajeSprite(GetItemImage(itemName).GetComponent<Image>());
                    UpdateMaquinaSprite(GetItemImage(itemName).GetComponent<Image>(), i);
                    Inventory.instance.RemoveItemFromInventory(GetItemImage(itemName));
                }
            }
        }
        else
        { 
            if (lockValue)
            {
                lockValue = false;
                ActionToDo(this.gameObject.name);
            }
        }
    }

    private void UpdateMaquinaSprite(Image spriteImage, int i)
    {
        SpriteRenderer slotSprite = GameObject.Find("MaquinaSlot" + i.ToString()).GetComponent<SpriteRenderer>();
        slotSprite.sprite = spriteImage.sprite;
    }

    private void AddEngranajeSprite(Image spriteImage)
    {
        for (int i = 0; i < maquinaSlots.Length; i++)
        {
            if (maquinaSlots[i].GetComponent<SpriteRenderer>().sprite == null)
            {
                SpriteRenderer engranajeSlotSprite = maquinaSlots[i].GetComponent<SpriteRenderer>();
                engranajeSlotSprite.sprite = spriteImage.sprite;

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

    public void ActionToDo(string maquinaName)
    {
        if (maquinaName == "MaquinaEngranajeA")
        {
            Client.instance.SendMessageToServer("ActivateMachine/" + this.gameObject.name);
            SpriteRenderer maquinaSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            SpriteRenderer[] maquinaSlotSpriteRenderer = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < maquinaSlotSpriteRenderer.Length; i++)
            {
                maquinaSlotSpriteRenderer[i].sprite = null;
            }
            maquinaSpriteRenderer.sprite = maquinaIsOpen;
            machineAnim.SetBool("startMovingMAchine", true);
            GameObject viga = GameObject.Find("GiantBlocker");
            GameObject viga2 = GameObject.Find("GiantBlocker (1)");
            viga.SetActive(false);
            viga2.SetActive(false);
			if (switchObj != null) {
				switchObj.ActivateSwitch ();
				Planner planner = FindObjectOfType<Planner> ();
				planner.Monitor ();
			}
        }
    }
}
