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
    public HUDDisplay hpAndMp;
    public GameObject canvas;
    public GameObject npcLog;
    public GameObject spiderLog;

    private List<Vector3> itemsOriginalPositions;
    private GameObject[] itemsInLevel;
    private GameObject reconnectText;
    private Client client;

    private float waitToKillNPCCountdown;
    private float waitToKillSpiderCountdown;
    private float waitToGrabItem;

    public float waitToRespawn;

    private Text NPCFeedbackText;
    private Text SpiderFeedbackText;

    #endregion

    #region Start

    void Start()
    {
        SetCanvas();
        SetLoggers();
        StorePlayers();

        hpAndMp = canvas.GetComponent<HUDDisplay>();
        itemsOriginalPositions = new List<Vector3>();

        waitToKillSpiderCountdown = 5f;
        waitToKillNPCCountdown = 5f;
        waitToGrabItem = 2f;

        if (GameObject.Find("ClientObject"))
        {
            client = GameObject.Find("ClientObject").GetComponent<Client>();
            client.RequestPlayerIdToServer();
        }
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

    public void ActivateSystem(string systemName)
    {

        GameObject activableSystem = GameObject.Find(systemName);

        if (activableSystem)
        {
            new ActivableSystemActions().DoSomething(activableSystem, false);
        }
        else
        {
            Debug.LogError("ActivableSystem " + systemName + " does not exists");
        }

    }


    // NPC FeedBack System
    public void ActivateNPCFeedback(string message)
    {
        SetNPCText(message);
        ShutNPCFeedback(false);
    }

    public void ShutNPCFeedback(bool now)
    {
        if (now)
        {
            KillNPC();
        }
        else
        {
            StartCoroutine(WaitToKillNPC());
        }
    }

    public void SetNPCText(string message)
    {
        if (!npcLog.activeInHierarchy)
        {
            npcLog.SetActive(true);
        }

        if (!NPCFeedbackText)
        {
            if (GameObject.Find("NPCLogText"))
            {
                NPCFeedbackText = GameObject.Find("NPCLogText").GetComponent<Text>();
            }
        }

        if (NPCFeedbackText)
        {
            NPCFeedbackText.text = message;
        }
    }

    public void ActivateSpiderFeedback(string message)
    {
        SetSpiderText(message);
        ShutSpiderFeedBack(false);
    }

    public void ShutSpiderFeedBack(bool now)
    {
        if (now)
        {
            KillSpider();
        }
        else
        {
            StartCoroutine(WaitToKillSpider());
        }
    }

    public void SetSpiderText(string message)
    {
        if (!spiderLog.activeInHierarchy)
        {
            spiderLog.SetActive(true);
        }

        if (!SpiderFeedbackText)
        {
            if (GameObject.Find("SpiderLogText"))
            {
                SpiderFeedbackText = GameObject.Find("SpiderLogText").GetComponent<Text>();
            }
        }

        if (SpiderFeedbackText)
        {
            SpiderFeedbackText.text = message;
        }
    }


    public void SetLocalPlayer(int id)
    {

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
        StartCoroutine(Respawning());
    }

    public void Respawn(PlayerController player)
    {
        StartCoroutine(Respawning(player));
    }

    public void GoToNextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string currentSceneNumber = Regex.Match(currentSceneName, @"\d+").Value;
        int nextSceneNumber = int.Parse(currentSceneNumber) + 1;
        string nextSceneName = "Escena" + nextSceneNumber;

        Debug.Log("Changing to scene " + nextSceneName);

        client.SendMessageToServer("ChangeScene/" + nextSceneName, true);
    }

    public void ShowReconnectingMessage(bool valor)
    {
        reconnectText.SetActive(valor);
    }

    public void CreateGameObject(string spriteName, int playerId)
    {
        GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Items/" + spriteName));
        GameObject player = null;

        switch (playerId)
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


        Physics2D.IgnoreCollision(newObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        newObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 1);

        StartCoroutine(WaitForCollision(newObject, player));
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

       Instantiate(Resources.Load(objectName));
    }

    public void ReloadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Utils

    protected void SetLoggers()
    {
        npcLog = GameObject.Find("NPCLog");
        npcLog.SetActive(false);

        spiderLog = GameObject.Find("SpiderLog");
        spiderLog.SetActive(false);

        reconnectText = GameObject.Find("ReconnectingText");
        reconnectText.SetActive(false);
    }

    protected void SetCanvas()
    {
        if (!canvas)
        {
            canvas = GameObject.Find("Canvas");
        }

        if (!canvas.activeInHierarchy)
        {
            canvas.SetActive(true);
        }
    }

    protected void StorePlayers()
    {
        players = new GameObject[3];

        players[0] = GameObject.Find("Mage");
        players[1] = GameObject.Find("Warrior");
        players[2] = GameObject.Find("Engineer");
    }

    public GameObject GetLocalPlayer()
    {
        return localPlayer.gameObject;
    }

    public PlayerController GetLocalPlayerController()
    {
        return localPlayer;
    }

    public GameObject GetPlayer(int position)
    {
        if (position < players.Length)
        {
            return players[position];
        }

        return null;
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

    public void DestroyObject(string name, float time)
    {
        GameObject gameObject = GameObject.Find(name);

        if (gameObject)
        {
            Destroy(gameObject, time);
        }

    }

    #endregion

    #region Coroutines

    public IEnumerator Respawning(PlayerController player)
    {
        player.StopMoving();
        player.ResetTransform();
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitToRespawn * .9f); // Respawn a bit sooner than local

		player.transform.position = player.respawnPosition;
        player.gameObject.SetActive(true);
        player.ResetTransform(); 
        player.ResumeMoving();
    }

    public IEnumerator Respawning()
    {
        localPlayer.StopMoving();
        localPlayer.ResetTransform();
        localPlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitToRespawn);

        localPlayer.transform.position = localPlayer.respawnPosition + Vector3.up * .1f;
        localPlayer.gameObject.SetActive(true);
        localPlayer.ResetTransform();
        localPlayer.SendPlayerDataToServer();
        localPlayer.ResumeMoving();
    }

    private IEnumerator WaitForCollision(GameObject gameObject, GameObject player)
    {
        yield return new WaitForSeconds(waitToGrabItem);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }

    private IEnumerator WaitToKillNPC()
    {
        yield return new WaitForSeconds(waitToKillNPCCountdown);
        KillNPC();
    }
    private IEnumerator WaitToKillSpider()
    {
        yield return new WaitForSeconds(waitToKillSpiderCountdown);
        KillSpider();
    }

    private void KillNPC()
    {

        if (NPCFeedbackText)
        {
            NPCFeedbackText.text = "";
        }

        npcLog.SetActive(false);
    }

    private void KillSpider()
    {

        if (SpiderFeedbackText)
        {
            SpiderFeedbackText.text = "";
        }

        spiderLog.SetActive(false);
    }

    #endregion

}
