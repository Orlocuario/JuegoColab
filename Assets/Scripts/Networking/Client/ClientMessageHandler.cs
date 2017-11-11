using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientMessageHandler
{
    private static char[] separator = new char[1] { '/' };

    private List<int> registeredEnemies;

    GameObject[] enemies;
    Client client;

    public ClientMessageHandler()
    {
        registeredEnemies = new List<int>();
        client = Client.instance;
    }

    public void HandleMessage(string message)
    {
        string[] msg = message.Split(separator);

        switch (msg[0])
        {
            case "ChangeScene":
                HandleChangeScene(msg);
                break;
            case "SetCharId":
                HandleSetCharId(msg);
                break;
            case "ChangeObjectPosition":
                HandleChangeObjectPosition(msg);
                break;
            case "InstantiateObject":
                HandleInstantiateObject(msg);
                break;
            case "NewChatMessage":
                HandleNewChatMessage(msg);
                break;
            case "DisplayChangeHPToClient":
                HandleChangeHpHUDToClient(msg);
                break;
            case "DisplayChangeMPToClient":
                HandleChangeMpHUDToClient(msg);
                break;
            case "DisplayChangeExpToClient":
                HandleChangeExpHUDToClient(msg);
                break;
            case "EnemyDie":
                EnemyDie(msg);
                break;
            case "EnemyRegistered":
                EnemyRegistered(msg);
                break;
            case "EnemyStartPatrolling":
                EnemyStartPatrolling(msg);
                break;
            case "EnemyChangePosition":
                ChangeEnemyPosition(msg);
                break;
            case "EnemyPatrollingPoint":
                ChangeEnemyPatrollingPoint(msg);
                break;
            case "SetControlOverEnemies":
                SetControlOverEnemies();
                break;
            case "PlayersAreDead":
                HandlePlayersAreDead(msg);
                break;
            case "PlayerChangePosition":
                HandleChangePlayerPosition(msg);
                break;
            case "PlayerAttack":
                HandleUpdatedAttackState(msg);
                break;
            case "PlayerTookDamage":
                HandlePlayerTookDamage(msg);
                break;
            case "PlayerPower":
                HandleUpdatedPowerState(msg);
                break;
            case "CreateGameObject":
                HandleCreateGameObject(msg);
                break;
            case "DestroyObject":
                HandleDestroyObject(msg);
                break;
            case "OthersDestroyObject":
                HandleDestroyObject(msg);
                break;
            case "ChangeSwitchStatus":
                HandleChangeSwitchStatus(msg);
                break;
            case "SwitchGroupReady":
                HandleSwitchGroupReady(msg);
                break;
            case "ActivateRuneDoor":
                HandleActivationDoor(msg);
                break;
            case "ActivateMachine":
                HandleActivationMachine(msg);
                break;
            case "ActivateNPCLog":
                HandleActivationNpcLog(msg);
                break;
            case "IgnoreCollisionBetweenObjects":
                HandleIgnoreCollision(msg);
                break;
            default:
                break;
        }
    }

    private void HandleIgnoreCollision(string[] msg)
    {
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.IgnoreCollisionBetweenObjects(msg);
    }

    private void HandleActivationNpcLog(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ActivateNPCLog(msg[1]);
    }

    private void HandleActivationMachine(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ActivateMachine(msg[1]);
    }

    private void HandleActivationDoor(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ActivateRuneDoor(msg[1]);
    }

    private void HandleChangeObjectPosition(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.MoveItemInGame(msg[1], msg[2], msg[3], msg[4]);
    }

    private void HandleInstantiateObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.InsantiateGameObject(msg);
    }

    private void HandleSwitchGroupReady(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int groupId = Int32.Parse(msg[1]);
        SwitchManager manager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<SwitchManager>();
        manager.CallAction(groupId);
    }

    private void HandleChangeSwitchStatus(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int groupId = Int32.Parse(msg[1]);
        int individualId = Int32.Parse(msg[2]);
        bool on = bool.Parse(msg[3]);
        SwitchManager manager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<SwitchManager>();
        Switch switchi = manager.GetSwitch(groupId, individualId);
        switchi.ReceiveDataFromServer(on);
    }

    public void EnemyRegistered(string[] msg)
    {
        int instanceId = int.Parse(msg[1]);
        int enemyId = int.Parse(msg[2]);
        int directionX = Int32.Parse(msg[3]);
        float posX = float.Parse(msg[4]);
        float posY = float.Parse(msg[5]);

        if (client.GetLocalPlayer().controlOverEnemies)
        {
            registeredEnemies.Add(enemyId);

            if (registeredEnemies.Count == enemies.Length)
            {
                Debug.Log("Start enemy patrolling");
                EnemiesStartPatrolling();
            }
        }

        else
        {
            if (enemies == null)
            {
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
            }

            foreach (GameObject enemy in enemies)
            {

                if (enemy.GetInstanceID() == instanceId)
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.Initialize(enemyId, directionX, posX, posY);
                }
            }
        }
    }

    public void EnemiesStartPatrolling()
    {
        string message = "EnemiesStartPatrolling/true";
        Client.instance.SendMessageToServer(message);
    }

    public void EnemiesRegisterOnRoom()
    {
        int enemyId = 0;

        // Agregar al enemigo local al networking
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Debug.Log("Activating " + enemies.Length + " enemies");

        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            enemyController.Register(enemyId++);
        }
    }

    private void SetControlOverEnemies()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        PlayerController localPlayer = client.GetLocalPlayer();

        if (localPlayer == null)
        {
            Debug.Log("No local player at this point");
            return;
        }

        localPlayer.controlOverEnemies = true;

    }

    private void EnemyStartPatrolling(string[] msg)
    {
        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);
        float patrolX = float.Parse(msg[5]);
        float patrolY = float.Parse(msg[6]);

        EnemyController enemyController = client.GetEnemy(enemyId);
        enemyController.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
        enemyController.StartPatrolling();
    }

    private void ChangeEnemyPosition(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);

        EnemyController enemyScript = client.GetEnemy(enemyId);
        enemyScript.SetPosition(directionX, posX, posY);
    }

    private void ChangeEnemyPatrollingPoint(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);
        float patrolX = float.Parse(msg[5]);
        float patrolY = float.Parse(msg[6]);

        EnemyController enemyScript = client.GetEnemy(enemyId);

        if (enemyScript)
        {
            enemyScript.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
        }
    }

    private void EnemyDie(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int enemyId = Int32.Parse(msg[1]);

        EnemyController enemyController = client.GetEnemy(enemyId);
        if (enemyController != null)
        {
            enemyController.Die();
        }
    }

    private void HandleChangeHpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        GlobalDisplayHUD displayHudScript = GameObject.Find("Canvas").GetComponent<GlobalDisplayHUD>();
        displayHudScript.CurrentHP(msg[1]);
    }

    private void HandleChangeMpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        GlobalDisplayHUD displayHudScript = GameObject.Find("Canvas").GetComponent<GlobalDisplayHUD>();
        displayHudScript.CurrentMP(msg[1]);
    }

    private void HandleChangeExpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        GlobalDisplayHUD displayHudScript = GameObject.Find("Canvas").GetComponent<GlobalDisplayHUD>();
        displayHudScript.ExperienceBar(msg[1]);
    }

    private void HandleCreateGameObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        int charId = Int32.Parse(msg[2]);
        scriptLevel.CreateGameObject(msg[1], charId);
    }

    private void HandleDestroyObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        GameObject objectToDestroy = GameObject.Find(msg[1]);
        scriptLevel.DestroyObjectInGame(objectToDestroy);
    }

    private void HandleChangeScene(string[] msg)
    {
        string scene = msg[1];
        Scene currentScene = SceneManager.GetActiveScene();

        if (!(currentScene.name == scene))
        {
            SceneManager.LoadScene(scene);
        }

    }

    private void HandleSetCharId(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene") // ??
        {
            return;
        }

        string charId = msg[1];
        bool controlOverEnemies = bool.Parse(msg[2]);
        int charIdint = Convert.ToInt32(charId);

        LevelManager levelManager = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        levelManager.SetLocalPlayer(charIdint);

        PlayerController scriptPlayer = client.GetLocalPlayer();
        scriptPlayer.controlOverEnemies = controlOverEnemies;

        if (controlOverEnemies)
        {
            client.StartFirstPlan();
            EnemiesRegisterOnRoom();
        }
    }

    private void HandleChangePlayerPosition(string[] data)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int charId = Int32.Parse(data[1]);
        float positionX = float.Parse(data[2]);
        float positionY = float.Parse(data[3]);
        int directionX = Int32.Parse(data[4]);
        int directionY = Int32.Parse(data[5]);
        float speedX = float.Parse(data[6]);
        bool isGrounded = bool.Parse(data[7]);
        bool pressingJump = bool.Parse(data[8]);
        bool pressingLeft = bool.Parse(data[9]);
        bool pressingRight = bool.Parse(data[10]);

        PlayerController playerController = client.GetPlayerController(charId);
        playerController.SetPlayerDataFromServer(positionX, positionY, directionX, directionY, speedX, isGrounded, pressingJump, pressingLeft, pressingRight);
    }

    private void HandleNewChatMessage(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        string chatMessage = msg[1];
        Chat.instance.UpdateChat(chatMessage);
    }

    private void HandleUpdatedPowerState(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int playerId = Int32.Parse(msg[1]);
        bool powerState = bool.Parse(msg[2]);

        PlayerController playerController = client.GetById(playerId);
        playerController.SetPowerState(powerState);
    }

    private void HandleUpdatedAttackState(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int charId = Int32.Parse(msg[1]);

        PlayerController playerController = client.GetPlayerController(charId);
        playerController.SetAttack();
    }

    private void HandlePlayerTookDamage(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int charId = Int32.Parse(msg[1]);
        float forceX = float.Parse(msg[2]);
        float forceY = float.Parse(msg[3]);

        Vector2 force = new Vector2(forceX, forceY);

        PlayerController playerController = client.GetPlayerController(charId);
        playerController.SetDamageFromServer(force);
    }

    private void HandlePlayersAreDead(string[] array)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ReloadLevel(array[1]);
    }
}
