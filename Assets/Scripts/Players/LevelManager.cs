using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

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

    void Start()
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
        }

        canvas.SetActive(true); // 8=D

        waitToKillNPCCountdown = 10f;
        waitToGrabItem = 5f;

        npcLog = GameObject.Find("NPCLog");
        npcLog.SetActive(false);

        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();

        reconectando = GameObject.Find("ReconnectingText");
        reconectando.SetActive(false);
    }

    public MageController GetMage()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[0];
        MageController magecontroller = player.GetComponent<MageController>();
        return magecontroller;
    }

    public WarriorController GetWarrior()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[1];
        WarriorController script = player.GetComponent<WarriorController>();
        return script;
    }

    public EngineerController GetEngineer()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[2];
        EngineerController script = player.GetComponent<EngineerController>();
        return script;
    }

    public void MostrarReconectando(bool valor)
    {
        reconectando.SetActive(valor);
    }

    public void SetLocalPlayer(int id)
    {
        players = new GameObject[3];

        players[0] = GameObject.Find("Mage");
        players[1] = GameObject.Find("Warrior");
        players[2] = GameObject.Find("Engineer");

        switch (id)
        {
            case 0:
                thePlayer = players[0].GetComponent<MageController>();
                Debug.Log("Activating Mage local player");
                break;
            case 1:
                Debug.Log("Activating Warrior local player");
                thePlayer = players[1].GetComponent<WarriorController>();
                break;
            case 2:
                Debug.Log("Activating Engineer local player");
                thePlayer = players[2].GetComponent<EngineerController>();
                break;
        }

        thePlayer.Activate(id);
        Camera.main.GetComponent<CameraController>().SetTarget(thePlayer.gameObject);
    }

    public void Respawn()
    {
        StartCoroutine("RespawnCo");
    }

    public IEnumerator RespawnCo()
    {
        thePlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitToRespawn);

        thePlayer.transform.position = thePlayer.respawnPosition + Vector3.up * 0.1f;
        thePlayer.gameObject.SetActive(true);
        thePlayer.IgnoreCollisionBetweenPlayers();
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

    public void ActivateNPCLog(string message)
    {
        npcLog.SetActive(true);
        Text npcLogText = GameObject.Find("NPCLogText").GetComponent<Text>();
        npcLogText.text = message;
        StartCoroutine("WaitToKillNPC");
    }

    public void IgnoreBoxCircleCollision(string[] array)
    {
        bool isThereCollison = bool.Parse(array[1]);

        GameObject boxObject = GameObject.Find(array[2]);
        GameObject circleObject = GameObject.Find(array[3]);

        CircleCollider2D[] circleColliders = circleObject.GetComponents<CircleCollider2D>();

        foreach (CircleCollider2D collider in circleColliders)
        {
            if (!collider.isTrigger)
            {
                Physics2D.IgnoreCollision(boxObject.GetComponent<BoxCollider2D>(), collider, isThereCollison);
            }
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

    public void InsantiateGameObject(string[] msg)
    {
        string objectName = "";
        for (int i = 1; i < msg.Length; i++)
        {
            objectName += msg[i];
            if (i != msg.Length - 1)
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
