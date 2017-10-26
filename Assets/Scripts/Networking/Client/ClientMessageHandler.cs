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
    private int enemiesToRegister;

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
            case "ChangePosition":
                HandleChangePosition(msg);
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
            case "Attack":
                HandleUpdatedAttackState(msg);
                break;
            case "AttackWarrior":
                HandleUpdatedAttackStateWarrior(msg);
                break;
            case "CastFireball":
                HandleCastFireball(msg);
                break;
            case "Power":
                HandleUpdatedPowerState(msg);
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
            case "SetControlOverEnemies":
                SetControlOverEnemies();
                break;
            case "CastProyectile":
                HandleCastProyectile(msg);
                break;
            case "PlayersAreDead":
                HandlePlayersAreDead(msg);
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
            case "IgnoreBoxCircleCollision":
                HandleIgnoreCollision(msg);
                break;
            default:
                break;
        }
    }

    private void HandleIgnoreCollision(string[] msg)
    {
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.IgnoreBoxCircleCollision(msg);
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
        int enemyId = int.Parse(msg[1]);
        registeredEnemies.Add(enemyId);

        if (registeredEnemies.Count == enemiesToRegister)
        {
            string message = "EnemiesStartPatrolling/true";
            Client.instance.SendMessageToServer(message);
        }

    }

    public void EnemiesStartPatrolling()
    {
        string message = "EnemiesStartPatrolling/true";
        Client.instance.SendMessageToServer(message);
    }

    public void EnemiesRegisterOnRoom()
    {
        // Agregar al enemigo local al networking
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        enemiesToRegister = enemies.Length;

        Debug.Log("Activating " + enemiesToRegister + " enemies");

        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            if (enemyController.fromEditor)
            {
                enemyController.SendIdToRegister();
            }
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
        EnemyController enemyController = client.GetEnemy(enemyId);
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
        float posX = float.Parse(msg[2]);
        float posY = float.Parse(msg[3]);
        EnemyController enemyScript = client.GetEnemy(enemyId);
        enemyScript.SetPosition(posX, posY);
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
            Debug.Log("Activating enemies...");
            EnemiesRegisterOnRoom();
        }
    }

    private void HandleChangePosition(string[] data)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        float positionX = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[3], CultureInfo.InvariantCulture);
        int charId = Int32.Parse(data[1]);
        bool isGrounded = bool.Parse(data[4]);
        float speed = float.Parse(data[5], CultureInfo.InvariantCulture);
        int direction = Int32.Parse(data[6]);
        bool pressingJump = bool.Parse(data[7]);
        bool pressingLeft = bool.Parse(data[8]);
        bool pressingRight = bool.Parse(data[9]);
        PlayerController script = client.GetPlayerController(charId);
        script.SetVariablesFromServer(positionX, positionY, isGrounded, speed, direction, pressingRight, pressingLeft, pressingJump);
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
        PlayerController script = client.GetById(Int32.Parse(msg[1]));
        script.RemoteSetter(bool.Parse(msg[2]));
    }

    private void HandleUpdatedAttackState(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int charId = Int32.Parse(msg[1]);
        bool state = bool.Parse(msg[2]);
        PlayerController script = client.GetPlayerController(charId);
        script.remoteAttacking = state;
    }

    private void HandleUpdatedAttackStateWarrior(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int charId = Int32.Parse(msg[1]);
        bool state = bool.Parse(msg[2]);
        int numHits = Int32.Parse(msg[3]);
        WarriorController script = client.GetWarrior();
        script.remoteAttacking = state;
        script.numHits = numHits;
    }

    private void HandleCastFireball(string[] data)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int direction = Int32.Parse(data[1]);
        float speed = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionX = float.Parse(data[3], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[4], CultureInfo.InvariantCulture);
        MageController script = client.GetMage();
        script.CastLocalFireball(direction, speed, positionX, positionY, script);
    }

    private void HandleCastProyectile(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        int direction = Int32.Parse(msg[1]);
        float speed = float.Parse(msg[2], CultureInfo.InvariantCulture);
        float positionX = float.Parse(msg[3], CultureInfo.InvariantCulture);
        float positionY = float.Parse(msg[4], CultureInfo.InvariantCulture);
        EngineerController script = client.GetEngineer();
        script.CastLocalProyectile(direction, positionX, positionY, script);
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
