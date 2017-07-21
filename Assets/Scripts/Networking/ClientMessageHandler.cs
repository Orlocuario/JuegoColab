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
            case "PlayersAreDead":
                HandlePlayersAreDead();
                break;
            case "ChangeHpAndManaHUD":
                //HandleHUDToRoom(arreglo);
                break;
            case "Attack":
                HandleUpdatedAttackState(arreglo);
                break;
            case "CastFireball":
                HandleCastFireball(arreglo);
                break;
            default:
                break;
        }
    }

    /*private void HandleHUDToRoom(string[] arreglo)
    {
        
    }*/

    private void HandlePlayersAreDead()
    {
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.ReloadLevel();
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

    private void HandleUpdatedAttackState(string[] arreglo)
    {
        int charId = Int32.Parse(arreglo[1]);
        bool state = bool.Parse(arreglo[2]);
        PlayerController script = client.GetPlayerController(charId);
        script.remoteAttacking = state;
    }

    private void HandleChangeScene(string[] arreglo)
    {
        string scene = arreglo[1];
        SceneManager.LoadScene(scene);
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
}
