using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

    private LevelManager theLevelManager;


	// Use this for initialization
	void Start () {

        theLevelManager = FindObjectOfType<LevelManager>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.tag == "Player1") 
		{
			theLevelManager.Respawn ();
		} 

		else if (other.tag == "Player2") 
		
		{
			theLevelManager.Respawn ();
		}

		else if (other.tag == "Player2") 

		{
			theLevelManager.Respawn ();
		}
			
    }
}
