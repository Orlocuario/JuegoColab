using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillzone : MonoBehaviour {

	private GameObject killiZone;

    // TODO: REFACTOR THIS

	// Use this for initialization
	void Start () {
		
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

		if(tag == "Player"){
			PlayerController script = collider.gameObject.GetComponent<PlayerController>();
			if (script.localPlayer == true) {
				return true;
			}
		}
		return false;
	}

	private void OnEnter ()
	{
		killiZone = (GameObject)Instantiate (Resources.Load ("Prefabs/KillZones/KillZoneEnginAir"));
		killiZone.transform.position = new Vector2 (37.3f, 5.4f);
	}
		
	private void OnExit()
	{
		Destroy (killiZone); 
	}

}
