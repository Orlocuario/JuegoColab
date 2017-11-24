using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSprite : MonoBehaviour {

	private SpriteRenderer renderer;
	public Sprite newSprite;

	// Use this for initialization
	void Start () {
		renderer = GetComponent <SpriteRenderer> ();
	}
	
	public void SpriteChanger()
	{
		renderer.sprite = newSprite;
	}
}
