using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    public Sprite flagClosed;
    public Sprite flagOpen;
    private SpriteRenderer theSpriteRenderer;

    public bool checkpointActive; 

	// Use this for initialization
	void Start ()
    {
        theSpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player1")
        {
            theSpriteRenderer.sprite = flagOpen;
            checkpointActive = true;
        }
		else if(other.tag == "Player2")
		{
			theSpriteRenderer.sprite = flagOpen;
			checkpointActive = true;
		}
		else if(other.tag == "Player3")
		{
			theSpriteRenderer.sprite = flagOpen;
			checkpointActive = true;
		}
    }
    
}
