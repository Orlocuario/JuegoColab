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
                PickUpExp();
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
		Destroy (this.gameObject);
		Client.instance.SendMessageToServer("OthersDestroyObject/" + this.gameObject.name);
        Inventory.instance.AddItemToInventory(this.gameObject);
		if (itemObj != null) {
			itemObj.PickUp (Client.instance.GetLocalPlayer ().playerObj);
		}
    }

    public void PickUpExp()
    {
		Destroy (this.gameObject);
		Client.instance.SendMessageToServer("DestroyItem/" + this.gameObject.name);
        Client.instance.SendMessageToServer("GainExp/" + "50");
    }
}