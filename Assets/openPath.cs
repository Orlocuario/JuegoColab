using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPath : MonoBehaviour {

	public bool killMePlease;

	// Use this for initialization
	void Start () {
		killMePlease = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (killMePlease == true) 
		{
			GameObject particulasWarrior = (GameObject)Instantiate (Resources.Load ("Prefabs/ParticulasWarriorMaleta"));
			particulasWarrior.GetComponent<Transform>().position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);
			Destroy (gameObject, 5f);
		}
			
	}
}
