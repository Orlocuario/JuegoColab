using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caidArbol : MonoBehaviour {

    private Animator animArbol;
	public PolygonCollider2D collider;
	public bool colliderOn;

	// Use this for initialization
	void Start () {
		animArbol = this.gameObject.GetComponent<Animator> ();
		animArbol.SetBool("RockBottom", false);
		collider.enabled = false; 
		colliderOn = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (colliderOn) {
			collider.enabled = true;
		} 
		else 
		{
			collider.enabled = false; 
		}
	}
}