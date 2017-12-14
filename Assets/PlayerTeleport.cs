using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour {

    public GameObject playerTeleported;
    private LevelManager theLevelManager;
    public Vector2 teleportPosition;


    // Use this for initialization

    void Start () {

        theLevelManager = GameObject.FindObjectOfType<LevelManager>();

        if (playerTeleported == null)
        {
            Debug.Log("You need a player to telepor on the teleport " + this.gameObject.name);
        }
        if (teleportPosition.Equals(default(Vector3)))
        {
            Debug.Log("You need a teleport position");
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            PlayerController teleportedController = other.gameObject.GetComponent<PlayerController>();

            if (other.gameObject.name == playerTeleported.name)
            {
                if (teleportedController.localPlayer)
                {
                    teleportedController.respawnPosition = teleportPosition;
                    theLevelManager.Respawn();
                }
            }
        }
    }
}
