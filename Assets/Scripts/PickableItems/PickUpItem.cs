using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {

    public GameObject PickUpButton;
    LevelManager levelManager;
    Vector2 myPosition;
    private bool lockValue;

    void Start()
    {
        lockValue = false;
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        PickUpButton.SetActive(false);
    }
    
	void Update ()
    {
        PlayerController player = levelManager.thePlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance < 0.2f)
        {
            lockValue = true;
            PickUpButton.SetActive(true);
        }

        else
        {
            PickUpButton.SetActive(false);
            if (lockValue)
            {
                lockValue = false;
            }
        }
    } 

    public void PickUp()
    {
        GameObject[] pickableItems = GameObject.FindGameObjectsWithTag("PickUpItems");
        
        for (int i = 0; i < pickableItems.Length; i++)
        {
            Vector2 itemPosition = new Vector2(pickableItems[i].transform.position.x, pickableItems[i].transform.position.y);
            if(itemPosition == myPosition)
            {
                Inventory.instance.AddItemToInventory(pickableItems[i]);
                Destroy(pickableItems[i]);
            }
        }
    }

    public void Drop(GameObject parent)
    {

    }
}
