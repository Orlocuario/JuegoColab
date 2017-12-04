﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientMessageHandler
{

    #region Attributes

    private int registeredEnemies;
    private static char[] separator = new char[1] { '/' };
    Client client;

    #endregion

    #region Constructor

    public ClientMessageHandler(Client instance)
    {
        registeredEnemies = 0;
        client = instance;
    }

    #endregion

    #region Common

    public void HandleMessage(string message)
    {
        string[] msg = message.Split(separator);

        switch (msg[0])
        {
            case "ChangeScene":
                HandleChangeScene(msg);
                break;
            case "ObjectMoved":
                HandleObjectMoved(msg);
                break;
            case "ObjectDestroyed":
                HandleObjectDestroyed(msg);
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
            case "DisplayStopChangeHPMPToClient":
                StopChangeHPMPToClient(msg);
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
            case "PlayerSetCharId":
                HandlePlayerSetCharId(msg);
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
            case "ActivateRuneSystem":
                HandleActivateRuneSystem(msg);
                break;
            case "ActivateGearSystem":
                HandleActivateGearSystem(msg);
                break;
            case "ActivateNPCLog":
                HandleActivationNpcLog(msg);
                break;
            case "IgnoreCollisionBetweenObjects":
                HandleIgnoreCollisionBetweenObjects(msg);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Handlers

    private void HandleActivationNpcLog(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.ActivateNPCFeedback(msg[1]);
    }



    #region Enemies

    public void EnemyRegistered(string[] msg)
    {
        int instanceId = int.Parse(msg[1]);
        int enemyId = int.Parse(msg[2]);
        int directionX = Int32.Parse(msg[3]);
        float posX = float.Parse(msg[4]);
        float posY = float.Parse(msg[5]);
        bool registered = false;

        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();

        //Función tablet control over enemies
        if (LocalPlayerHasControlOverEnemies())
        {
            registered = true;

            if (++registeredEnemies == enemies.Length)
            {
                Debug.Log("Start enemy patrolling");
                EnemiesStartPatrolling();
            }
        }

        //Función tablets sin control over enemies
        else
        {
            foreach (EnemyController enemy in enemies)
            {
                if (enemy)
                {

                    if (enemy.gameObject.GetInstanceID() == instanceId)
                    {
                        enemy.Initialize(enemyId, directionX, posX, posY);
                        registered = true;
                    }
                }
                else
                {

                    Debug.Log("Enemy is null mdfk");
                }
            }
        }

        if (!registered)
        {
            Debug.Log("Enemy with iID " + instanceId + " id " + enemyId + " NOT REGISTERED");
        }

    }

    public void EnemiesStartPatrolling()
    {
        string message = "EnemiesStartPatrolling/true";
        Client.instance.SendMessageToServer(message, true);
    }

    public void EnemiesRegisterOnRoom()
    {
        int enemyId = 0;

        // Agregar al enemigo local al networking
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();

        Debug.Log("Activating " + enemies.Length + " enemies");

        foreach (EnemyController enemy in enemies)
        {
            enemy.Register(enemyId++);
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

        if (!localPlayer)
        {
            Debug.Log("No local player at this point");
            return;
        }

        localPlayer.controlOverEnemies = true;
        client.StartFirstPlan();

    }

    private void EnemyStartPatrolling(string[] msg)
    {
        int enemyId = Int32.Parse(msg[1]);
        int directionX = Int32.Parse(msg[2]);
        float posX = float.Parse(msg[3]);
        float posY = float.Parse(msg[4]);
        float patrolX = float.Parse(msg[5]);
        float patrolY = float.Parse(msg[6]);

        EnemyController enemy = client.GetEnemy(enemyId);

        if (enemy)
        {
            enemy.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
            enemy.StartPatrolling();
        }
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

        EnemyController enemy = client.GetEnemy(enemyId);

        if (enemy)
        {
            enemy.SetPosition(directionX, posX, posY);
        }
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

        EnemyController enemy = client.GetEnemy(enemyId);

        if (enemy)
        {
            enemy.SetPatrollingPoint(directionX, posX, posY, patrolX, patrolY);
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

        EnemyController enemy = client.GetEnemy(enemyId);

        if (enemy)
        {
            enemy.Die();
        }
    }

    #endregion

    #region HUD

    private void HandleChangeHpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        HUDDisplay hpAndMp = GameObject.FindObjectOfType<LevelManager>().hpAndMp;
        hpAndMp.CurrentHPPercentage(float.Parse(msg[1]));
    }

    private void HandleChangeMpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        HUDDisplay hpAndMp = GameObject.FindObjectOfType<LevelManager>().hpAndMp;
        hpAndMp.CurrentMPPercentage(float.Parse(msg[1]));
    }

    private void StopChangeHPMPToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        HUDDisplay hpAndMp = GameObject.FindObjectOfType<LevelManager>().hpAndMp;
        hpAndMp.StopLocalParticles(); // Only stop local particles
    }

    private void HandleChangeExpHUDToClient(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        HUDDisplay hpAndMp = GameObject.FindObjectOfType<LevelManager>().hpAndMp;
        hpAndMp.CurrentExpPercentage(msg[1]);
    }

    #endregion

    #region Objects 

    #region Activables

    private void HandleActivateGearSystem(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.ActivateGearSystem(msg[1]);
    }

    private void HandleActivateRuneSystem(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.ActivateRuneSystem(msg[1]);
    }

    #endregion

    #region Switches

    private void HandleSwitchGroupReady(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        int groupId = Int32.Parse(msg[1]);

        SwitchManager manager = GameObject.FindObjectOfType<SwitchManager>();
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

        SwitchManager manager = GameObject.FindObjectOfType<SwitchManager>();
        Switch switchi = manager.GetSwitch(groupId, individualId);
        switchi.ReceiveDataFromServer(on);
    }

    #endregion

    #region GameObjects

    private void HandleChangeObjectPosition(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.MoveItemInGame(msg[1], msg[2], msg[3], msg[4]);
    }

    private void HandleInstantiateObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.InsantiateGameObject(msg);
    }

    private void HandleCreateGameObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        string spriteName = msg[1];
        int charId = Int32.Parse(msg[2]);

        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.CreateGameObject(spriteName, charId);
    }

    private void HandleDestroyObject(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        GameObject objectToDestroy = GameObject.Find(msg[1]);

        if (objectToDestroy)
        {
            levelManager.DestroyObjectInGame(objectToDestroy);
        }
    }

    private void HandleIgnoreCollisionBetweenObjects(string[] msg)
    {
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.IgnoreCollisionBetweenObjects(msg);
    }

    #endregion

    #region Movables

    private void HandleObjectMoved(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        string name = msg[1];
        float forceX = float.Parse(msg[2]);
        float forceY = float.Parse(msg[3]);

        Vector2 force = new Vector2(forceX, forceY);

        GameObject movableObject = GameObject.Find(name);

        if (!movableObject)
        {
            Debug.Log("Movable " + name + " does not exists");
            return;
        }

        MovableObject movableController = movableObject.GetComponent<MovableObject>();

        if (!movableController)
        {
            Debug.Log(name + " is not movable");
            return;
        }

        movableController.MoveMe(force, false);

    }

    #endregion

    #region Destroyables

    private void HandleObjectDestroyed(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene")
        {
            return;
        }

        string name = msg[1];

        GameObject destroyableObject = GameObject.Find(name);

        if (!destroyableObject)
        {
            Debug.Log("Destroyable " + name + " does not exists");
            return;
        }

        DestroyableObject destroyableController = destroyableObject.GetComponent<DestroyableObject>();

        if (!destroyableController)
        {
            Debug.Log(name + " is not destroyable");
            return;
        }

        destroyableController.DestroyMe(false);
    }

    #endregion

    #endregion

    #region Scene

    private void HandleChangeScene(string[] msg)
    {
        string scene = msg[1];
        Scene currentScene = SceneManager.GetActiveScene();

        if (!(currentScene.name == scene))
        {
            SceneManager.LoadScene(scene);
        }

    }

    #endregion

    #region Players

    private void HandlePlayerSetCharId(string[] msg)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ClientScene") // ??
        {
            return;
        }

        int charId = Int32.Parse(msg[1]);
        bool controlOverEnemies = bool.Parse(msg[2]);

        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.SetLocalPlayer(charId);

        PlayerController playerController = client.GetLocalPlayer();
        playerController.controlOverEnemies = controlOverEnemies;

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
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.ReloadLevel(array[1]);
    }

    #endregion

    #endregion

    #region Utils

    private bool LocalPlayerHasControlOverEnemies()
    {
        return client.GetLocalPlayer() && client.GetLocalPlayer().controlOverEnemies;
    }

    #endregion
}
