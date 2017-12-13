using UnityEngine;

public class ChangeSprite : MonoBehaviour {

	private SpriteRenderer thyRenderer;
	public Sprite newSprite;

	// Use this for initialization
	void Start () {
		thyRenderer = GetComponent <SpriteRenderer> ();
	}
	
	public void SpriteChanger()
	{
		thyRenderer.sprite = newSprite;
	}
}
