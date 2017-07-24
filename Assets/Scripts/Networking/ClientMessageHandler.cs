using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientMessageHandler {

    Client client;
    public ClientMessageHandler()
    {
        client = Client.instance;
    }

    public void HandleMessage(string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] arreglo = message.Split(separator);
        switch (arreglo[0])
        {
            case "ChangeScene":
                HandleChangeScene(arreglo);
                break;
            case "SetCharId":
                HandleSetCharId(arreglo);
                break;
            case "ChangePosition":
                HandleChangePosition(arreglo);
                break;
            case "NewChatMessage":
                HandleNewChatMessage(arreglo);
                break;
            case "DisplayChangeHPToClient":
                HandleChangeHpHUDToClient(arreglo);
                break;
            case "DisplayChangeMPToClient":
                HandleChangeMpHUDToClient(arreglo);
                break;
            case "Attack":
                HandleUpdatedAttackState(arreglo);
                break;
            case "AttackWarrior":
                HandleUpdatedAttackStateWarrior(arreglo);
                break;
            case "CastFireball":
                HandleCastFireball(arreglo);
                break;
            case "CastProyectile":
                HandleCastProyectile(arreglo);
                break;
            case "PlayersAreDead":
                HandlePlayersAreDead();
                break;
            default:
                break;
        }
    }

    private void HandleChangeScene(string[] arreglo)
    {
        string scene = arreglo[1];
        SceneManager.LoadScene(scene);
        DisplayHUD.instance.CurrentHP("1");
        DisplayHUD.instance.CurrentMP("1");
        //falta settear su vida/mana real al 100%
    }

    private void HandleSetCharId(string[] arreglo)
    {
        string charId = arreglo[1];
        int charIdint = Convert.ToInt32(charId);
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.SetCharAsLocal(charIdint);
    }

    private void HandleChangePosition(string[] data)
    {
        int charId = Int32.Parse(data[1]);
        float positionX = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[3], CultureInfo.InvariantCulture);
        bool isGrounded = bool.Parse(data[4]);
        float speed = float.Parse(data[5], CultureInfo.InvariantCulture);
        int direction = Int32.Parse(data[6]);
        bool pressingJump = bool.Parse(data[7]);
        bool pressingLeft = bool.Parse(data[8]);
        bool pressingRight = bool.Parse(data[9]);
        PlayerController script = client.GetPlayerController(charId);
        script.SetVariablesFromServer(positionX, positionY, isGrounded, speed, direction, pressingRight, pressingLeft, pressingJump);
    }

    private void HandleNewChatMessage(string[] arreglo)
    {
        string chatMessage = arreglo[1];
        Chat.instance.UpdateChat(chatMessage);
    }

    private void HandleChangeHpHUDToClient(string[] arreglo)
    {
        DisplayHUD.instance.CurrentHP(arreglo[1]);
    }

    private void HandleChangeMpHUDToClient(string[] arreglo)
    {
        DisplayHUD.instance.CurrentMP(arreglo[1]);
    }

    private void HandleUpdatedAttackState(string[] arreglo)
    {
        int charId = Int32.Parse(arreglo[1]);
        bool state = bool.Parse(arreglo[2]);
        PlayerController script = client.GetPlayerController(charId);
        script.remoteAttacking = state;
    }

    private void HandleUpdatedAttackStateWarrior(string[] arreglo)
    {
        int charId = Int32.Parse(arreglo[1]);
        bool state = bool.Parse(arreglo[2]);
        int numHits = Int32.Parse(arreglo[3]);
        WarriorController script = client.GetWarrior();
        script.remoteAttacking = state;
        script.numHits = numHits;
    }

    private void HandleCastFireball(string[] data)
    {
        int direction = Int32.Parse(data[1]);
        float speed = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionX = float.Parse(data[3], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[4], CultureInfo.InvariantCulture);
        MageController script = client.GetMage();
        script.CastLocalFireball(direction, speed, positionX, positionY, script);
    }

    private void HandleCastProyectile(string[] arreglo)
    {
        int direction = Int32.Parse(arreglo[1]);
        float speed = float.Parse(arreglo[2], CultureInfo.InvariantCulture);
        float positionX = float.Parse(arreglo[3], CultureInfo.InvariantCulture);
        float positionY = float.Parse(arreglo[4], CultureInfo.InvariantCulture);
        EngineerController script = client.GetEngineer();
        script.CastLocalProyectile(direction, speed, positionX, positionY, script);
    }

    private void HandlePlayersAreDead()
    {
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ReloadLevel();
    }
}
