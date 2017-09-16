using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caidArbol : MonoBehaviour {

    public Animator animArbol;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void onTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "rocaCaida")
        {
            animArbol.SetBool("RockBottom", true);

        }
    }
}
