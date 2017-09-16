using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roca : MonoBehaviour {

    public bool caidaOn;
    public bool isArbol;
	public bool isReady;

    private Animator animRoca;



	// Use this for initialization
	void Start () {
        caidaOn = false;
        isReady = false;

	}
	
	// Update is called once per frame
	void Update ()
	{
        if (isReady == true)
        {
            animRoca.SetBool("isReady", true);
        }

        if (caidaOn == true)
        {
            animRoca.SetBool("caidaOn", true);
        }
     

	}

	private void onTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "ArbolCaida") {
			Animator animArbol = other.gameObject.GetComponent<Animator> ();
			animArbol.SetBool ("RockBottom", true);
			
		}
	}
}
