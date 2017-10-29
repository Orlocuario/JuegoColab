using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roca : MonoBehaviour {

    public bool caidaOn;
    public bool isArbol;
	public bool isReady;
	private Animator animRoca;
	private GameObject roca;
	private CameraController camera;



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
			animRoca.SetBool("caidaOn", true);
        }
	}

	public void KilledMySelf(string s)
    {
		GameObject particulas = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/Humo"));
		particulas.GetComponent <Transform>().position = new Vector2 (34.1f, -7.07f);
		Destroy (particulas,5f); 
        Destroy(this.gameObject,1f);
    }
}
