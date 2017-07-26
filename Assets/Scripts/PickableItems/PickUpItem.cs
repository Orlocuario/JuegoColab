using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player1")
        {

        }
        else if (other.gameObject.tag == "Player2")
        {

        }
        else if (other.gameObject.tag == "Player3")
        {

        }
        else
        {
            return;
        }
    }

    private void PickUp()
    {
        
    }

    private void Destroy(GameObject item)
    {
        Destroy(item);
    }

    public void Create(GameObject parent)
    {

    }
}
