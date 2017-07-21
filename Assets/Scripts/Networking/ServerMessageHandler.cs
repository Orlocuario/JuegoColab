using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

public class ServerMessageHandler
{
    Server server;
    
    public ServerMessageHandler(Server server)
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
            case "ChangePosition":
                SendUpdatedPosition(message, connectionId, arreglo);
                break;
            case "NewChatMessage":
                SendNewChatMessage(message, connectionId);
                break;
            case "RecoveryHUD":
                SendHUDToRoom(arreglo, connectionId);
                break;
            case "Attack":
                SendAttackState(message, connectionId,arreglo);
                break;
            case "CastFireball":
                SendNewFireball(message, connectionId, arreglo);
                break;
            default:
                break;
        }
    }

    private void SendHUDToRoom(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.RecieveHUD(arreglo[1]);
    }

    private void SendNewFireball(string message, int connectionId, string[] data)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

   private void SendNewChatMessage(string chatMessage, int connectionID)
    {
        Jugador player = server.GetPlayer(connectionID);
        Room room = player.room;
        room.SendMessageToAllPlayers(chatMessage);
    }

    private void SendUpdatedPosition(string message, int connectionID, string[] data)
    {
        Jugador player = server.GetPlayer(connectionID);
        Room room = player.room;
        int charId = Int32.Parse(data[1]);
        float positionX = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[3], CultureInfo.InvariantCulture);
        bool isGrounded = bool.Parse(data[4]);
        float speed = float.Parse(data[5], CultureInfo.InvariantCulture);
        int direction = Int32.Parse(data[6]);
        bool pressingJump = bool.Parse(data[7]);
        bool pressingLeft = bool.Parse(data[8]);
        bool pressingRight = bool.Parse(data[9]);
        player.positionX = positionX;
        player.positionY = positionY;
        player.isGrounded = isGrounded;
        player.speed = speed;
        player.direction = direction;
        player.pressingJump = pressingJump;
        player.pressingLeft = pressingLeft;
        player.pressingRight = pressingRight;
        room.SendMessageToAllPlayersExceptOne(message, connectionID);
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

    public void SendAttackState(string message, int connectionId, string[] data)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        player.attacking = bool.Parse(data[2]);
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }
}
