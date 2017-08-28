using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roca : MonoBehaviour {

    public bool switchRocaOn = false;
    public bool isArbol = false;
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
            animRoca.SetBool("isArbol", isArbol);
        }
    
	}

    private void OnTriggerEnter(Collider other)
    {
        isArbol = true;
    }
}
