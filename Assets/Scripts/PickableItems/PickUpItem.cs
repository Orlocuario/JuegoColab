using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{ 
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (this.gameObject.tag == "ExperienceItem" && (other.collider.tag == "Player1" || other.collider.tag == "Player2" || other.collider.tag == "Player3"))
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();
            if (localPlayer.gameObject.tag == other.collider.tag)
            {
                PickUpExp(this.gameObject.name);
            }
        }
        else if (other.collider.tag == "Player1" || other.collider.tag == "Player2" || other.collider.tag == "Player3")
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();
            if (localPlayer.gameObject.tag == other.collider.tag)
            {
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        string itemName = this.gameObject.name;
        Inventory.instance.AddItemToInventory(this.gameObject);
        Client.instance.SendMessageToServer("DestroyItem/" + itemName);
    }

    public void PickUpExp(string item_name)
    {
        string itemName = this.gameObject.name;
        Client.instance.SendMessageToServer("GainExp/" + "50");
        Client.instance.SendMessageToServer("DestroyItem/" + itemName);
    }
}