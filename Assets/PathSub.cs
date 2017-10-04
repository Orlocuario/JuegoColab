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
			GameObject particulas = (GameObject)Instantiate (Resources.Load ("Prefabs/ParticulasWarriorMaleta"));
			particulas.GetComponent <Transform>().position = new Vector2 (34.1f, -7.07f);
			Destroy (gameObject, 5f);
		}
	}
}
