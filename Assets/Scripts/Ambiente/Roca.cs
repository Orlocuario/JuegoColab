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
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	private void onTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "ArbolCaida") {
			Animator animArbol = other.gameObject.GetComponent<Animator> ();
			animArbol.SetBool ("RockBottom", true);
			
		}
	}
}
