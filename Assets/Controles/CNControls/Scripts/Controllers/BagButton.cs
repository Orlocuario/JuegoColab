using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagButton : MonoBehaviour {

    public GameObject inventory;

	// Use this for initialization
	void Start ()
    {
        inventory.SetActive(false);	
	}
	
	// Update is called once per frame
	public void ToggleInventory()
    {
        if (inventory.activeSelf == true)
        {
            inventory.SetActive(false);
        }
        else
        {
            inventory.SetActive(true);
        }
    }
}
