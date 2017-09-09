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

		if (caidaOn == true && isReady == false) {
			Debug.Log ("Faltan los otros Switch");
		} 
		else {
			animRoca.SetBool ("caidaOn", caidaOn);
		}
	}
}
