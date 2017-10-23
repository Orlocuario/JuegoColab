﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateNPC : MonoBehaviour {

	private LevelManager theLevelManager;

	// Use this for initialization
	void Start () {

		theLevelManager = FindObjectOfType<LevelManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (IsCollisionLocalPlayer (other)) {
			OnEnter ();
		}
	}

	public void OnTriggerExit2D(Collider2D other){
		if (IsCollisionLocalPlayer (other)) {
			OnExit ();
		}
	}

	private bool IsCollisionLocalPlayer(Collider2D collider)
	{
		string tag = collider.gameObject.tag;

		if (tag == "Player1" || tag == "Player2" || tag == "Player3"){
			PlayerController script = collider.gameObject.GetComponent<PlayerController>();
			if (script.localPlayer == true) {
				return true;
			}
		}
		return false;
	}

	private void OnEnter (Collider2D other)
	{
		theLevelManager.ActivateNPCLog ("Para saltar, presiona el botón inferior de tu derecha"); 
	}

	private void OnExit(Collider2D other)
	{
		
	}

}
}