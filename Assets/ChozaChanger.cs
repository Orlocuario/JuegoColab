using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChozaChanger : MonoBehaviour {

    private SpriteRenderer theRenderer;
    public Sprite insideHouse;
    private Sprite nobodyIn;

    // Use this for initialization
    void Start() {
        theRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        nobodyIn = theRenderer.sprite;
	}
	
	// Update is called once per frame
	private void OnTriggerrEnter2D (Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>().localPlayer)
        {
            theRenderer.sprite = insideHouse;
            theRenderer.sortingLayerName = "Suelo";
            theRenderer.sortingOrder = -3;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>().localPlayer)
        {
            theRenderer.sprite = nobodyIn;
            theRenderer.sortingOrder = -1;
        }
    }
}
