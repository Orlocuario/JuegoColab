using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    public PlayerController thePlayer;
    public GameObject[] players;
    public GameObject canvas;
    public GameObject npcLog;

    private Client client;
    private GameObject createGameObject;
    private GameObject player;
    private GameObject[] itemsInLevel;
    private List<Vector3> itemsOriginalPositions = new List<Vector3>();
    private GameObject reconectando;

    private float waitToKillNPCCountdown;
    private float waitToResetItemPos;
    private float waitToGrabItem;
    public float waitToRespawn;

    void Start ()
    {
        canvas.SetActive(true);

        waitToKillNPCCountdown = 10f;
        waitToGrabItem = 5f;

        npcLog = GameObject.Find("NPCLog");
        npcLog.SetActive(false);

        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();

        reconectando = GameObject.Find("ReconnectingText");
        reconectando.SetActive(false);
	}

    public void MostrarReconectando(bool valor)
    {
        reconectando.SetActive(valor);
    }

    public void SetLocalPlayer(int id)
    {
        players = new GameObject[3];

        players[0] = GameObject.FindGameObjectsWithTag("Player1")[0];
        players[1] = GameObject.FindGameObjectsWithTag("Player2")[0];
        players[2] = GameObject.FindGameObjectsWithTag("Player3")[0];

        switch (id)
        {
            case 0:
                thePlayer = players[0].GetComponent<MageController>();
                break;
            case 1:
                thePlayer = players[1].GetComponent<WarriorController>();
                break;
            case 2:
                thePlayer = players[2].GetComponent<EngineerController>();
                break;
        }

        thePlayer.Activate(id);
        Camera.main.GetComponent<CameraController>().SetTarget(thePlayer.gameObject) ;
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

    public void MoveItemInGame(string itemName, string posX, string posY, string rotZ)
    {
        GameObject obj = GameObject.Find(itemName);
        Transform gameObjectToMove;
        if (!gameObject)
        {
            return;
        }
        gameObjectToMove = obj.GetComponent<Transform>();
        gameObjectToMove.position = new Vector3(float.Parse(posX), float.Parse(posY), gameObjectToMove.position.z);
        Quaternion gORotation = gameObjectToMove.rotation;
        gORotation = new Quaternion(gameObjectToMove.rotation.x, gameObjectToMove.rotation.y, float.Parse(rotZ), gameObjectToMove.rotation.w);
    }

    public void DestroyObjectInGame(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
        return;
    }

    public void ActivateRuneDoor(string doorName)
    {
        GameObject door = GameObject.Find(doorName);
        door.GetComponent<BoxCollider2D>().isTrigger = true;
        SpriteRenderer doorSpriteRenderer = door.GetComponent<SpriteRenderer>();
        SpriteRenderer[] doorSlotSpriteRenderer = door.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < doorSlotSpriteRenderer.Length; i++)
        {
            doorSlotSpriteRenderer[i].sprite = null;
        }

        doorSpriteRenderer.sprite = door.GetComponent<RuneSystem>().doorIsOpen;
    }

    internal void ActivateNPCLog(string message)
    {
        npcLog.SetActive(true);
        Text npcLogText = GameObject.Find("NPCLogText").GetComponent<Text>();
        npcLogText.text = message;
        StartCoroutine("WaitToKillNPC");
    }

    public void IgnoreBoxCircleCollision(string[] array)
    {
        if (array[1] == "true")
        {
            GameObject boxObject = GameObject.Find(array[2]);
            GameObject circleObject = GameObject.Find(array[3]);
            Physics2D.IgnoreCollision(boxObject.GetComponent<BoxCollider2D>(), circleObject.GetComponent<CircleCollider2D>());
        }
        else
        {
            GameObject boxObject = GameObject.Find(array[2]);
            GameObject circleObject = GameObject.Find(array[3]);
            Physics2D.IgnoreCollision(boxObject.GetComponent<BoxCollider2D>(), circleObject.GetComponent<CircleCollider2D>(), false);
        }
    }

    public void ActivateMachine(string machineName)
    {
        GameObject machine = GameObject.Find(machineName);
        SpriteRenderer maquinaSpriteRenderer = machine.gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer[] maquinaSlotSpriteRenderer = machine.gameObject.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < maquinaSlotSpriteRenderer.Length; i++)
        {
            maquinaSlotSpriteRenderer[i].sprite = null;
        }

        maquinaSpriteRenderer.sprite = machine.GetComponent<EngranajeSystem>().maquinaIsOpen;

        if (machineName == "MaquinaEngranajeA")
        {
            GameObject viga = GameObject.Find("GiantBlocker");
            GameObject viga2 = GameObject.Find("GiantBlocker (1)");
            viga.SetActive(false);
            viga2.SetActive(false);
        }
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

    public void InsantiateGameObject(string[] arreglo)
    {
        string objectName = "";
        for (int i = 1; i < arreglo.Length; i++)
        {
            objectName += arreglo[i];
            if (i != arreglo.Length - 1)
            {
                objectName += "/";
            }
        }
        createGameObject = (GameObject)Instantiate(Resources.Load(objectName));
    }

    private IEnumerator WaitForCollision()
    {
        yield return new WaitForSeconds(waitToGrabItem);
        Physics2D.IgnoreCollision(createGameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }

    private IEnumerator WaitToKillNPC()
    {
        yield return new WaitForSeconds(waitToGrabItem);
        npcLog.SetActive(false);
    }

    public void ReloadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
