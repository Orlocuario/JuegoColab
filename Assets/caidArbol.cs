using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caidArbol : MonoBehaviour {

    private Animator animArbol;


	// Use this for initialization
	void Start () {
		animArbol = this.gameObject.GetComponent<Animator> ();
		animArbol.SetBool("RockBottom", false);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void onTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "rocaCaida")
        {
            animArbol.SetBool("RockBottom", true);
			Debug.Log("ccccccabdfjbfjsbfjhasbdf");
        }
    }
}
