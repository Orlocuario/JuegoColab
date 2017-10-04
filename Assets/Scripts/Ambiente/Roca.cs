using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roca : MonoBehaviour {

    public bool caidaOn;
    public bool isArbol;
	public bool isReady;
	private Animator animRoca;
	private GameObject roca;



	// Use this for initialization
	void Start () {
		animRoca = this.gameObject.GetComponent <Animator>();

        caidaOn = false;
        isReady = false;
		roca = this.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{

        if (isReady == true)
        {
            animRoca.SetBool("IsReady", true);
        }

        if (caidaOn == true && isReady == true)
		{	
			animRoca.SetBool("caidaOn", true);
			Destroy (gameObject, 5.5f);
        }
	}

}
