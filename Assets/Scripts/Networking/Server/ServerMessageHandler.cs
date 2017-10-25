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
        string[] msg = message.Split(separator);

        switch (msg[0])
        {
            case "RequestCharId":
                SendCharIdAndControl(connectionId);
                break;
            case "ChangePosition":
                SendUpdatedPosition(message, connectionId, msg);
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
                SendHpHUDToRoom(msg, connectionId);
                break;
            case "ChangeMpHUDToRoom":
                SendMpHUDToRoom(msg, connectionId);
                break;
			case "ChangeHpAndMpHUDToRoom": //Necessary coz' ChatZone changes both at the same rate
                SendHpHAndMpHUDToRoom(msg, connectionId);
                break;
            case "GainExp":
                SendExpToRoom(msg, connectionId);
                break;
            case "Attack":
                SendAttackState(message, connectionId, msg);
                break;
            case "AttackWarrior":
                SendAttackState(message, connectionId, msg);
                break;
            case "CastFireball":
                SendNewFireball(message, connectionId, msg);
                break;
            case "CastProyectile":
                SendNewProjectile(message, connectionId, msg);
                break;
            case "Power":
                SendPowerState(message, connectionId, msg);
                break;
            case "NewEnemyId":
                NewEnemy(msg, connectionId);
                break;
            case "EnemyHpChange":
                ReduceEnemyHp(message, msg, connectionId);
                break;
            case "EnemyChangePosition":
                EnemyChangePosition(message, msg, connectionId);
                break;
            case "CreateGameObject":
                SendNewGameObject(message, connectionId);
                break;
            case "DestroyObject":
                SendDestroyObject(message, connectionId);
                break;
			case "OthersDestroyObject":
				SendOthersDestroyObject (message, connectionId);
				break;
            case "InventoryUpdate":
                SendInventoryUpdate(message, connectionId);
                break;
            case "ChangeSwitchStatus":
                SendChangeSwitchStatus(message, msg, connectionId);
                break;
            case "SwitchGroupReady":
                SendSwitchGroupAction(message,msg, connectionId);
                break;
            case "ActivateRuneDoor":
                SendActivationDoor(message, connectionId);
                break;
            case "ActivateMachine":
                SendActivationMachine(message, connectionId);
                break;
            case "ActivateNPCLog": // No se si es necesario o no, ya que puedes llamar el metodo desde afuera (start o script)
			SendActivationNPC(msg, connectionId);
                break;
            case "IgnoreBoxCircleCollision":
                SendIgnoreBoxCircleCollision(message, connectionId);
                break;
            default:
                break;
        }
    }

    public void SendAllData(int connectionId, Room room)
    {

        foreach (NetworkPlayer player in room.players)
        {
            room.SendMessageToPlayer(player.GetReconnectData(), connectionId);
        }

        foreach(ServerSwitch switchi in room.switchs)
        {
            room.SendMessageToPlayer(switchi.GetReconnectData(), connectionId);
        }
    }


    private void SendIgnoreBoxCircleCollision(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    public void SendActivationNPC(string[] msg, int connectionId) // Manda un mensaje a un solo jugador
    {
		string message = msg [1];
		int playerId = int.Parse (msg [2]);
		int newConnectionId = 0;
		Room room = server.GetPlayer (connectionId).room;
		room.WriteFeedbackHistorial (message + "/" + playerId);
		foreach (NetworkPlayer jugador in room.players) {
			if (playerId == jugador.charId) {
				newConnectionId = jugador.connectionId;
				break;
			}
		}
        if (!message.Contains("ActivateNPCLog"))
        {
            message = "ActivateNPCLog/" + message;
        }
        server.NPCsLastMessage = message;
        room.SendMessageToPlayer(message, newConnectionId); // Message es el texto a mostrar en el NPC Log
    }

    private void SendActivationMachine(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void SendActivationDoor(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendSwitchGroupAction(string message, string[] msg,int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        int groupId = Int32.Parse(msg[1]);
        if (!room.activatedGroups.Contains(groupId))
        {
            room.activatedGroups.Add(groupId);
        }
    }

    private void SendChangeSwitchStatus(string message, string[] msg, int connectionId)
    {
        int groupId = Int32.Parse(msg[1]);
        int individualId = Int32.Parse(msg[2]);
        bool on = bool.Parse(msg[3]);
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SetSwitchOn(on, groupId, individualId);
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void EnemyChangePosition(string message, string[] msg, int connectionId)
    {
        int enemyId = Int32.Parse(msg[1]);
        float posX = float.Parse(msg[2]);
        float posY = float.Parse(msg[3]);
        NetworkPlayer player = server.GetPlayer(connectionId);
        NetworkEnemy enemy = player.room.GetEnemy(enemyId);
        enemy.posX = posX;
        enemy.posY = posY;
        player.room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void ReduceEnemyHp(string message, string[] msg, int connectionId)
    {
        int enemyId = Int32.Parse(msg[1]);
        float enemyHp = float.Parse(msg[2]);
        NetworkPlayer player = server.GetPlayer(connectionId);
        NetworkEnemy enemy = player.room.GetEnemy(enemyId);
        enemy.ReduceHp(enemyHp);
    }

    private void NewEnemy(string[] msg, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        int id = Int32.Parse(msg[1]);
        float hp = float.Parse(msg[2]);
        player.room.AddEnemy(id, hp);

      //  string message = "EnemyStartPatrolling/" + id;
      //  player.room.SendMessageToAllPlayers(message);
    }
    private void SendNewGameObject(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        int charId = player.charId;
        room.SendMessageToAllPlayers(message + "/" + charId.ToString());
    }

    private void SendInventoryUpdate(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        player.InventoryUpdate(message);
    }

    private void SendDestroyObject(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

	private void SendOthersDestroyObject (string message, int connectionId)
	{
		NetworkPlayer player = server.GetPlayer(connectionId);
		Room room = player.room;
		room.SendMessageToAllPlayersExceptOne (message, connectionId);
	}

    private void SendHpHUDToRoom(string[] msg, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeHP(msg[1]);
    }

    private void SendMpHUDToRoom(string[] msg, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeMP(msg[1]);
    }

    private void SendHpHAndMpHUDToRoom(string[] msg, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.RecieveHpAndMpHUD(msg[1]);
    }

    private void SendExpToRoom(string[] msg, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.hpManaGer.ChangeExp(msg[1]);
    }

    private void SendNewFireball(string message, int connectionId, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void SendNewProjectile(string message, int connectionId, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void SendNewChatMessage(string chatMessage, int connectionID)
    {
        NetworkPlayer player = server.GetPlayer(connectionID);
        Room room = player.room;
        room.SendMessageToAllPlayers(chatMessage);
    }

    private void SendUpdatedPosition(string message, int connectionID, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(connectionID);
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
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }

    private void SendInstantiation(string message, int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        room.SendMessageToAllPlayers(message);
    }

    private void SendCharIdAndControl(int connectionId)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        int charId = player.charId;
        string message = "SetCharId/" + charId + "/" + player.controlOverEnemies;
        server.SendMessageToClient(connectionId, message);
        SendAllData(connectionId, player.room);
    }

    public void SendChangeScene(string sceneName, Room room)
    {
        string command = "ChangeScene/" + sceneName;
        room.SendMessageToAllPlayers(command);
    }

    public void SendAttackState(string message, int connectionId, string[] data)
    {
        NetworkPlayer player = server.GetPlayer(connectionId);
        Room room = player.room;
        player.attacking = bool.Parse(data[2]);
        room.SendMessageToAllPlayersExceptOne(message, connectionId);
    }
	public void SendPowerState(string message, int connectionId, string[] data)
	{
		NetworkPlayer player = server.GetPlayer (connectionId);
		Room room = player.room;
		player.power = bool.Parse (data [2]);
		room.SendMessageToAllPlayersExceptOne (message, connectionId);
	}
}
