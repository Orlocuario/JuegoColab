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
		roca = gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
        
        if (caidaOn == true)
		{
            Debug.Log("me fui a la chucha");
			animRoca.SetBool("caidaOn", true);
			Destroy (roca, 5.5f);
            Debug.Log("La Mateeeeee");
        }
	}

    public void KilledMySelf()
    {

        Destroy(this.gameObject);
    }
}
