using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{ 

	public PlannerItem itemObj = null;

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();
            if (localPlayer.gameObject.name == other.collider.name)
            {
                PickUp();
            }
        }
        else if (other.collider.tag == "Arrow")
        {
            if (Client.instance.GetLocalPlayer().name == "Engineer")
            {
                PickUp();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (this.gameObject.tag == "ExperienceItem" && GameObjectIsPlayer(other.gameObject))
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();
            if (localPlayer.gameObject.name == other.name)
            {
                PickUpExp();
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
			Planner planner = FindObjectOfType<Planner> ();
			planner.Monitor ();
		}
    }

    public void PickUpExp()
    {
		Destroy (this.gameObject);
		Client.instance.SendMessageToServer("OthersDestroyObject/" + this.gameObject.name);
        Client.instance.SendMessageToServer("GainExp/" + "50");
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

}