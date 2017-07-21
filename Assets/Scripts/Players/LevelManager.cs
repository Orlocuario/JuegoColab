using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public float waitToRespawn;
    public PlayerController thePlayer;
    private Client client;


	// Use this for initialization
	void Start () {
        thePlayer = null;
        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();
	}

    public void SetCharAsLocal(int id)
    {
        PlayerController player = null;
        GameObject[] players = new GameObject[3];
        players[0] = GameObject.FindGameObjectsWithTag("Player1")[0];
        players[1] = GameObject.FindGameObjectsWithTag("Player2")[0];
        players[2] = GameObject.FindGameObjectsWithTag("Player3")[0];
        switch (id)
        {
            case 0:
                player = GameObject.FindGameObjectsWithTag("Player1")[0].GetComponent<MageController>();                
                break;
            case 1:
                player = GameObject.FindGameObjectsWithTag("Player2")[0].GetComponent<WarriorController>();
                break;
            case 2:
                player = GameObject.FindGameObjectsWithTag("Player3")[0].GetComponent<EngineerController>();
                break;
            default:
                break;
        }
        player.Activate(id);
        thePlayer = player;
        Camera.main.GetComponent<CameraController>().target = player.gameObject;        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReloadLevel(string[] message)
    {
        Debug.Log("PLAYERS ARE DEAD MUAJAJAJA");
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
        thePlayer.SendObjectDataToServer();
    }

}
