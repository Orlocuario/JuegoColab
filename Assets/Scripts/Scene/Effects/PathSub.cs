using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSub : MonoBehaviour {

	public bool killMe;

	// Use this for initialization
	void Start () {

		killMe = false; 
		
	}
	
// Update is called once per frame
	void Update () {
		
		if (killMe == true) 
		{
			Destroy (gameObject, 2f);
		}
	}
}
