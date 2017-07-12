using UnityEngine;
using System.Collections;
using System;

public class MessageHandler
{
    Server server;
    
    public MessageHandler(Server server)
    {
        this.server = server;
    }

    public void HandleMessage(string message, int connectionId)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] arreglo = message.Split(separator);
        switch (arreglo[0])
        {
            case "RequestCharId":
                SendCharId(connectionId);
                break;
            default:
                break;
        }
    }

    private void SendCharId(int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        int charId = player.charId;
        string message = "SetCharId/" + charId;
        server.SendMessageToClient(connectionId, message);
    }

    public void SendChangeScene(string sceneName, Room room)
    {
        string command = "ChangeScene/" + sceneName;
        room.SendMessageToAllPlayers(command);
    }
}
