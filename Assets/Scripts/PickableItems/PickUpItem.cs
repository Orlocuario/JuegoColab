using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player1" || other.collider.tag == "Player2" || other.collider.tag == "Player3")
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        string itemName = this.gameObject.name;
        Inventory.instance.AddItemToInventory(this.gameObject);
        Client.instance.SendMessageToServer("DestroyItem/" + itemName);
    }

    public void Drop(GameObject parent)
    {

    }
}