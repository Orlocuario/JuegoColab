using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    #region Attributes


    public PlayerController localPlayer;
    public GameObject[] players;
    public GameObject canvas;
    public GameObject npcLog;

    private List<Vector3> itemsOriginalPositions;
    private GameObject createGameObject;
    private GameObject[] itemsInLevel;
    private GameObject reconnectText;
    private GameObject player;
    private Client client;

    private float waitToKillNPCCountdown;
    private float waitToResetItemPos;
    private float waitToGrabItem;

    public float waitToRespawn;

    private Text npcLogText;

    #endregion

    #region Start

    void Start()
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
        }

        canvas.SetActive(true); // 8=D

        itemsOriginalPositions = new List<Vector3>();

        waitToKillNPCCountdown = 10f;
        waitToGrabItem = 5f;

        npcLog = GameObject.Find("NPCLog");
        npcLog.SetActive(false);

        client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();

        reconnectText = GameObject.Find("ReconnectingText");
        reconnectText.SetActive(false);
    }

    #endregion

    #region Common

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

    private void ReadNPCMessage(NPCtrigger NPC)
    {
		if (NPC.activeFeedback != null && NPC.activeFeedback.particles != null && NPC.activeFeedback.particles.isPlaying)
        {
            NPC.activeFeedback.particles.Stop();
        }

        if (NPC.feedbackCount >= NPC.feedbacks.Length)
        {
            npcLog.SetActive(false);
            return;
        }

        NPC.activeFeedback = NPC.feedbacks[NPC.feedbackCount];

		if (NPC.activeFeedback != null)
        {
            if (NPC.activeFeedback.particles != null)
            {
                NPC.activeFeedback.particles.Play();
            }

            if (NPC.activeFeedback.message != null)
            {
                npcLogText.text = NPC.activeFeedback.message;
            }
        }

		NPC.feedbackCount += 1;

        StartCoroutine(WaitToReadNPCMessage(NPC));
    }

    public void ActivateNPCLog(string message)
    {
        npcLog.SetActive(true);
        Text npcLogText = GameObject.Find("NPCLogText").GetComponent<Text>();
        npcLogText.text = message;
        StartCoroutine("WaitToKillNPC");
    }

    public void ActivateNPCLog(NPCtrigger NPCtrigger)
    {
		if (!npcLog.activeInHierarchy)
        {
            npcLog.SetActive(true);
        }

		npcLogText = GameObject.Find("NPCLogText").GetComponent<Text>();

        ReadNPCMessage(NPCtrigger);
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
                localPlayer = players[0].GetComponent<MageController>();
                Debug.Log("Activating Mage local player");
                break;
            case 1:
                Debug.Log("Activating Warrior local player");
                localPlayer = players[1].GetComponent<WarriorController>();
                break;
            case 2:
                Debug.Log("Activating Engineer local player");
                localPlayer = players[2].GetComponent<EngineerController>();
                break;
        }

        localPlayer.Activate(id);
        Camera.main.GetComponent<CameraController>().SetTarget(localPlayer.gameObject);
    }

    public void Respawn()
    {
        StartCoroutine("Respawning");
    }

    public void GoToNextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string currentSceneNumber = Regex.Match(currentSceneName, @"\d+").Value;
        int nextSceneNumber = int.Parse(currentSceneNumber) + 1;
        string nextSceneName = "Escena" + nextSceneNumber;

        Debug.Log("Changing to scene " + nextSceneName);

        client.SendMessageToServer("ChangeScene/" + nextSceneName);
    }

    public void MostrarReconectando(bool valor)
    {
        reconnectText.SetActive(valor);
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

    public void ReloadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Utils

    public GameObject GetLocalPlayer()
    {
        return localPlayer.gameObject;
    }

    public PlayerController GetLocalPlayerController()
    {
        return localPlayer;
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

    public void IgnoreCollisionBetweenObjects(string[] array)
    {
        bool ignores = bool.Parse(array[1]);

        GameObject objectA = GameObject.Find(array[2]);
        GameObject objectB = GameObject.Find(array[3]);

        if (!objectA || !objectB)
        {
            return;
        }

        Collider2D[] collidersA = objectA.GetComponents<Collider2D>();
        Collider2D[] collidersB = objectB.GetComponents<Collider2D>();

        foreach (Collider2D colliderA in collidersA)
        {
            if (!colliderA.isTrigger)
            {
                foreach (Collider2D colliderB in collidersB)
                {
                    if (!colliderB.isTrigger)
                    {
                        Physics2D.IgnoreCollision(colliderA, colliderB, ignores);
                    }
                }
            }
        }

    }

    #endregion

    #region Coroutines

    public IEnumerator Respawning()
    {
        localPlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitToRespawn);

        localPlayer.transform.position = localPlayer.respawnPosition + Vector3.up * .1f;
        localPlayer.gameObject.SetActive(true);
        localPlayer.IgnoreCollisionBetweenPlayers();
        localPlayer.SendPlayerDataToServer();
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

    private IEnumerator WaitToReadNPCMessage(NPCtrigger NPC)
    {
        yield return new WaitForSeconds(NPC.feedbackTime);
        ReadNPCMessage(NPC);
    }

    #endregion

}
