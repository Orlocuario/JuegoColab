using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableTutorial : MonoBehaviour {

	public Sprite wasShot;
	private SpriteRenderer spriteRenderer;
	public Vector2 instantiatePoints;


	void Start () {
		spriteRenderer = this.gameObject.GetComponent <SpriteRenderer> ();
	}
	
	private void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.name == "Fireball" || name == "Arrow" || name == "Punch") 
		{
			GameObject platform = (GameObject)Instantiate (Resources.Load ("Prefabs/SueloMetalFlotante"));
			platform.transform.position = new Vector2 (this.gameObject.transform.position.x - 3f, this.gameObject.transform.position.y);
			GameObject platform2 = (GameObject)Instantiate (Resources.Load ("Prefabs/SueloMetalFlotante"));
			platform2.transform.position = new Vector2 (instantiatePoints.x, instantiatePoints.y);
			spriteRenderer.sprite = wasShot;
		}
	}

}
