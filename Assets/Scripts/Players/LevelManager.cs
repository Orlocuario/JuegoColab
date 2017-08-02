﻿using System.Collections;
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

    public void ReloadLevel()
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

        thePlayer.transform.position = thePlayer.respawnPosition + Vector3.up*0.1f;
        thePlayer.gameObject.SetActive(true);
        thePlayer.IgnoreCollisionStar2puntoCero();
        thePlayer.SendObjectDataToServer();
    }

    public void DestroyItemInGame(GameObject itemToDestroy)
    {
        Destroy(itemToDestroy);
        return;
    }

    public void CreateGameObject(string spriteName, int charId)
    {
        GameObject createGameObject = (GameObject)Instantiate(Resources.Load("Prefabs/Items" + spriteName));
        RectTransform playerTransform = null;

        switch (charId)
        {
            case 0:
                playerTransform = GameObject.Find("Mage").GetComponent<RectTransform>();
                break;
            case 1:
                playerTransform = GameObject.Find("Warrior").GetComponent<RectTransform>();
                break;
            case 2:
                playerTransform = GameObject.Find("Engineer").GetComponent<RectTransform>();
                break;
            default:
                break;
        }

        Vector2 createGameObjectPosition = createGameObject.GetComponent<RectTransform>().position;
        createGameObjectPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        //missing rate pick up
    }
}
