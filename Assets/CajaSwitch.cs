using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CajaSwitch : MonoBehaviour {

	public bool meVoy;
	public bool ahoraMeVoy;

	// Use this for initialization
	void Start () {
		meVoy = false;
		ahoraMeVoy = false;		
	}
	
	// Update is called once per frame
	void Update () {

		if (meVoy == true && ahoraMeVoy == true) 
		{
			Destroy (gameObject, 1f);
			GameObject particle = (GameObject)Instantiate(Resources.Load("Prefab/FeedbackParticles/FBMageButt"));
		}
		
	}
}
