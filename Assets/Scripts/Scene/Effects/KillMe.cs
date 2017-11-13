using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMe : MonoBehaviour {

	private bool matame;

	// Use this for initialization
	void Start () {

		matame = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (matame == true) 
		{
			Destroy (gameObject, 5);
		}
	}
}
