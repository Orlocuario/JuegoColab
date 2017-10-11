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
                SendCharIdAndControl(connectionId);
                break;
            case "ChangePosition":
                SendUpdatedPosition(message, connectionId, arreglo);
                break;
            case "ChangeObjectPosition":
                SendUpdatedObjectPosition(message, connectionId);
                break;
            case "InstantiateObject":
                SendInstantiation(message, connectionId);
                break;
            case "NewChatMessage":
                SendNewChatMessage(message, connectionId);
                break;
            case "ChangeHpHUDToRoom":
                SendHpHUDToRoom(arreglo, connectionId);
                break;
            case "ChangeMpHUDToRoom":
                SendMpHUDToRoom(arreglo, connectionId);
                break;
            case "ChangeHpAndManaHUDToRoom": //Necessary coz' ChatZone changes both at the same rate
                SendHpHAndMpHUDToRoom(arreglo, connectionId);
                break;
            case "GainExp":
                SendExpToRoom(arreglo, connectionId);
                break;
            case "Attack":
                SendAttackState(message, connectionId, arreglo);
                break;
            case "AttackWarrior":
                SendAttackState(message, connectionId, arreglo);
                break;
            case "CastFireball":
                SendNewFireball(message, connectionId, arreglo);
                break;
            case "Power":
                SendPowerState(message, connectionId, arreglo);
                break;
            case "NewEnemyId":
                NewEnemy(arreglo, connectionId);
                break;
            case "EnemyHpChange":
                ReduceEnemyHp(message, arreglo, connectionId);
                break;
            case "EnemyChangePosition":
                EnemyChangePosition(message, arreglo, connectionId);
                break;
            case "CreateGameObject":
                SendNewGameObject(message, connectionId);
                break;
            case "DestroyObject":
                SendDestroyObject(message, connectionId);
                break;
            case "InventoryUpdate":
                SendInventoryUpdate(message, connectionId);
                break;
            case "ChangeSwitchStatus":
                SendChangeSwitchStatus(message, arreglo, connectionId);
                break;
            case "SwitchGroupReady":
                SendSwitchGroupAction(message,arreglo, connectionId);
                break;
            case "ActivateRuneDoor":
                SendActivationDoor(message, connectionId);
                break;
            case "ActivateMachine":
                SendActivationMachine(message, connectionId);
                break;
            case "ActivateNPCLog": // No se si es necesario o no, ya que puedes llamar el metodo desde afuera (start o script)
                SendActivationNPC(message, connectionId);
                break;
            default:
                break;
        }
    }

    public void SendActivationNPC(string message, int connectionId) // Manda un mensaje a un solo jugador
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        if (!message.Contains("ActivateNPCLog"))
        {
            message = "ActivateNPCLog/" + message;
        }
        server.NPCsLastMessage = message;
        room.SendMessageToPlayer(message, connectionId); // Message es el texto a mostrar en el NPC Log
    }

    private void SendActivationMachine(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendActivationDoor(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendSwitchGroupAction(string message, string[] arreglo,int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        int groupId = Int32.Parse(arreglo[1]);
        if (!room.activatedGroups.Contains(groupId))
        {
            room.activatedGroups.Add(groupId);
        }
    }

    private void SendChangeSwitchStatus(string message, string[] arreglo, int connectionId)
    {
        int groupId = Int32.Parse(arreglo[1]);
        int individualId = Int32.Parse(arreglo[2]);
        bool on = bool.Parse(arreglo[3]);
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SetSwitchOn(on, groupId, individualId);
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void EnemyChangePosition(string message, string[] arreglo, int connectionId)
    {
        int enemyId = Int32.Parse(arreglo[1]);
        float posX = float.Parse(arreglo[2]);
        float posY = float.Parse(arreglo[3]);
        Jugador player = server.GetPlayer(connectionId);
        Enemy enemy = player.room.GetEnemy(enemyId);
        enemy.posX = posX;
        enemy.posY = posY;
        player.room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void ReduceEnemyHp(string message, string[] arreglo, int connectionId)
    {
        int enemyId = Int32.Parse(arreglo[1]);
        float enemyHp = float.Parse(arreglo[2]);
        Jugador player = server.GetPlayer(connectionId);
        Enemy enemy = player.room.GetEnemy(enemyId);
        enemy.ReduceHp(enemyHp);
    }

    private void NewEnemy(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        int id = Int32.Parse(arreglo[1]);
        player.room.AddEnemy(id);
    }
    private void SendNewGameObject(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        int charId = player.charId;
        room.SendMessageToAllPlayers(message + "/" + charId.ToString());
    }

    private void SendInventoryUpdate(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        player.InventoryUpdate(message);
    }

    private void SendDestroyObject(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendHpHUDToRoom(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeHP(arreglo[1]);
    }

    private void SendMpHUDToRoom(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeMP(arreglo[1]);
    }

    private void SendHpHAndMpHUDToRoom(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.RecieveHpAndMpHUD(arreglo[1]);
    }

    private void SendExpToRoom(string[] arreglo, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeExp(arreglo[1]);
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
		bool isGrounded = bool.Parse (data [4]);
		float speed = float.Parse (data [5], CultureInfo.InvariantCulture);
		int direction = Int32.Parse (data [6]);
		bool pressingJump = bool.Parse (data [7]);
		bool pressingLeft = bool.Parse (data [8]);
		bool pressingRight = bool.Parse (data [9]);
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


    private void SendUpdatedObjectPosition(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void SendInstantiation(string message, int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendCharIdAndControl(int connectionId)
    {
        Jugador player = server.GetPlayer(connectionId);
        int charId = player.charId;
        string message = "SetCharId/" + charId + "/" + player.controlOverEnemies;
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
	public void SendPowerState(string message, int connectionId, string[] data)
	{
		Jugador player = server.GetPlayer (connectionId);
		Room room = player.room;
		player.power = bool.Parse (data [2]);
		room.SendMessageToAllPlayersExceptOne (message, connectionId);
	}
}
