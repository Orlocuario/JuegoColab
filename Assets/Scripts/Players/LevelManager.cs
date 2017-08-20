using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public PlayerController thePlayer;
    public float waitToRespawn;

    private Client client;
    private GameObject createGameObject;
    private GameObject player;
    private GameObject[] itemsInLevel;
    private List<Vector3> itemsOriginalPositions = new List<Vector3>();
    private float waitToGrabItem;
    private float waitToResetItemPos;

    void Start ()
    {
        waitToGrabItem = 5f;
        thePlayer = null;
        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();
        //SetHpAndManaToMax();
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
            case 2:
                player = GameObject.FindGameObjectsWithTag("Player1")[0].GetComponent<MageController>();
                break;
            case 1:
                player = GameObject.FindGameObjectsWithTag("Player2")[0].GetComponent<WarriorController>();
                break;
            case 0:
                player = GameObject.FindGameObjectsWithTag("Player3")[0].GetComponent<EngineerController>();
                break;
            default:
                break;
        }

        player.Activate(id);
        thePlayer = player;
        Camera.main.GetComponent<CameraController>().target = player.gameObject;
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
        createGameObject = (GameObject)Instantiate(Resources.Load("Prefabs/Items/" + spriteName));
        player = null;

        switch (charId)
        {
            case 0:
                player = GameObject.Find("Mage");
                break;
            case 1:
                player = GameObject.Find("Warrior");
                break;
            case 2:
                player = GameObject.Find("Engineer");
                break;
            default:
                break;
        }

        Physics2D.IgnoreCollision(createGameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
        RectTransform createGameObjectRectTransform = GameObject.Find(spriteName + "(Clone)").GetComponent<RectTransform>();
        createGameObjectRectTransform.position = new Vector3(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y, 1);
        StartCoroutine("WaitForCollision");
        StartCoroutine("ResetGameObjectPosition");
    }

    private IEnumerator WaitForCollision()
    {
        yield return new WaitForSeconds(waitToGrabItem);
        Physics2D.IgnoreCollision(createGameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }

    public void ReloadLevel()
    {
        Debug.Log("PLAYERS ARE DEAD MUAJAJAJA");
    }
}
