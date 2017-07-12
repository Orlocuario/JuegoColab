using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public float waitToRespawn;
    public PlayerController thePlayer;
    private Client client;


	// Use this for initialization
	void Start () {
        thePlayer = FindObjectOfType<PlayerController>();
        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();
	}

    public void SetCharAsLocal(int id)
    {
        PlayerController player = null;
        switch (id)
        {
            case 0:
                player = GameObject.FindGameObjectsWithTag("Player1")[0].GetComponent<PlayerController>();
                break;
            case 1:
                player = GameObject.FindGameObjectsWithTag("Player2")[0].GetComponent<PlayerController>();
                break;
            case 2:
                player = GameObject.FindGameObjectsWithTag("Player3")[0].GetComponent<PlayerController>();
                break;
            default:
                break;
        }
        player.Activate();
        thePlayer = player;
        GameObject touchObject = GameObject.Find("TouchController");
        touchObject.GetComponent<TouchScript>().script = player;
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Respawn()
    {
        StartCoroutine("RespawnCo");
    }

    public IEnumerator RespawnCo()
    {
        thePlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitToRespawn); 

        thePlayer.transform.position = thePlayer.respawnPosition;
        thePlayer.gameObject.SetActive(true);
    }

}
