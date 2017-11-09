using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillzone : MonoBehaviour {

	private GameObject killZone;

    // TODO: REFACTOR THIS

	// Use this for initialization
	void Start () {
		
	}
		
	public void OnTriggerEnter2D(Collider2D other)
	{
		if (GameObjectIsPlayer(other.gameObject)) {
			OnEnter ();
		}
	}

	public void OnTriggerExit2D(Collider2D other){
		if (GameObjectIsPlayer(other.gameObject)) {
			OnExit ();
		}
	}

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    private void OnEnter ()
	{
		killZone = (GameObject)Instantiate (Resources.Load ("Prefabs/KillZones/KillZoneEnginAir"));
		killZone.transform.position = new Vector2 (37.3f, 5.4f);
	}
		
	private void OnExit()
	{
		Destroy (killZone); 
	}

}
