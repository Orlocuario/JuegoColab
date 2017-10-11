using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{ 

	public PlannerItem itemObj = null;


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
        else if (other.collider.tag == "Arrow")
        {
            if (Client.instance.GetLocalPlayer().tag == "Player3")
            {
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        string itemName = this.gameObject.name;
        Client.instance.SendMessageToServer("DestroyObject/" + itemName);
        Inventory.instance.AddItemToInventory(this.gameObject);
		if (itemObj != null) {
			itemObj.PickUp (Client.instance.GetLocalPlayer ().playerObj);
		}
    }

    public void PickUpExp(string item_name)
    {
        string itemName = this.gameObject.name;
        Client.instance.SendMessageToServer("DestroyItem/" + itemName);
        Client.instance.SendMessageToServer("GainExp/" + "50");
    }
}