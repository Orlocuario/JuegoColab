using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roca : MonoBehaviour {

    public bool switchRocaOn;
    public bool isArbol;
    private Animator animRoca;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (switchRocaOn == true)
        {
            animRoca.SetBool("switchRocaOn", switchRocaOn);
        }

        if (isArbol == true)
        {
            animRoca.SetBool("isArbol", true);
        }
    
	}

    private void OnColliderEnter2D(Collider2D other)
    {
		if (other.gameObject.tag == "ArbolCaida") 
		{
			isArbol = true;
			other.GetComponent<Animator>().SetBool("Rockbottom", true);
		}
    }

	public void IgnoreCollision()
	{
		GameObject arbolQl = GameObject.FindGameObjectWithTag ("ArbolCaida");

		Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), arbolQl.GetComponent<Collider2D> ()); 
	}
}