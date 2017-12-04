using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class Room
{

    #region Attributes

    public List<int> activatedSwitchGroups; //guarda los numeros de los grupos de switchs activados
    public ServerDoorsManager doorManager;
    public ServerMessageHandler sender;
    public List<NetworkPlayer> players;
    public List<NetworkEnemy> enemies;
    public List<ServerSwitch> switchs;
    public RoomHpMp hpManaGer;
    public RoomLogger log;
    public Server server;

    public string sceneToLoad;
    public string actualChat;
    public int numPlayers;
    public int maxPlayers;
    public bool started;
    public int id;

    private string matchNumber;
    private string record;

    #endregion

    #region Constructor

    public Room(int _id, Server _server, ServerMessageHandler _sender, int _maxPlayers)
    {

        maxPlayers = _maxPlayers;
        numPlayers = 0;
        sender = _sender;
        server = _server;
        started = false;
        record = "";
        id = _id;

        activatedSwitchGroups = new List<int>();
        doorManager = new ServerDoorsManager();
        players = new List<NetworkPlayer>();
        switchs = new List<ServerSwitch>();
        enemies = new List<NetworkEnemy>();
        log = new RoomLogger(this.id);
        hpManaGer = new RoomHpMp(this);

        sceneToLoad = Server.instance.sceneToLoad;
    }

    #endregion

    #region Common

    #region Players

    public bool AddPlayer(int connectionId, string address)
    {
        if (IsFull())
        {
            return false;
        }

        NetworkPlayer newPlayer = new NetworkPlayer(connectionId, GetCharId(), this, address);
        players.Add(newPlayer);
        SetControlEnemies(newPlayer);

        if (IsFull())
        {
            Debug.Log("Full room");
            sender.SendChangeScene(sceneToLoad, this);
            started = true;
            SendMessageToAllPlayers("Mago: Conectado", false);
            SendMessageToAllPlayers("Guerrero: Conectado", false);
            SendMessageToAllPlayers("Ingeniero: Conectado", false);
        }

        return true;
    }

    public NetworkPlayer FindPlayerInRoom(int id)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.connectionId == id)
            {
                return player;
            }
        }
        return null;
    }

    public NetworkPlayer FindPlayerInRoom(string address)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.ipAddress == address)
            {
                return player;
            }
        }
        return null;
    }

    #endregion

    #region Enemies

    public void EnemiesStartPatrolling()
    {
        foreach (NetworkEnemy enemy in enemies)
        {
            if (enemy.patrollingPointX != default(float) & enemy.patrollingPointY != default(float))
            {
                string message = "EnemyStartPatrolling/" +
                    enemy.id + "/" +
                    enemy.directionX + "/" +
                    enemy.positionX + "/" +
                    enemy.positionY + "/" +
                    enemy.patrollingPointX + "/" +
                    enemy.patrollingPointY;

                SendMessageToAllPlayers(message, true);
            }
        }
    }

    public NetworkEnemy GetEnemy(int id)
    {
        foreach (NetworkEnemy enemy in enemies)
        {
            if (enemy.id == id)
            {
                return enemy;
            }
        }
        return null;
    }

    public NetworkEnemy AddEnemy(int instanceId, int enemyId, float hp)
    {
        NetworkEnemy enemy = new NetworkEnemy(instanceId, enemyId, hp, this);
        enemies.Add(enemy);

        return enemy;
    }

    public bool RemoveEnemy(NetworkEnemy enemy)
    {
        return enemies.Remove(enemy);
    }

    private void SetControlEnemies(NetworkPlayer targetPlayer)
    {
        bool check = false;
        foreach (NetworkPlayer player in players)
        {
            if (player.controlOverEnemies == true)
            {
                check = true;
            }
        }
        if (!check)
        {
            targetPlayer.controlOverEnemies = true;
        }
    }

    #endregion

    #region Chat

    public void CreateTextChat()
    {
        matchNumber = "Por Resolver";
        string path = Directory.GetCurrentDirectory() + "/ChatLogFromRoomN°" + id + ".txt";

        if (!File.Exists(path))
        {
            using (var tw = new StreamWriter(File.Create(path)))
            {
                tw.WriteLine("Partida N°: " + matchNumber);
                tw.WriteLine(record);
                tw.Close();
            }
        }
        else if (File.Exists(path))
        {
            using (var tw = new StreamWriter(path, true))
            {
                tw.WriteLine("\r\n" + "____________________________________");
                tw.WriteLine("Generando Nuevo Historial...");
                tw.WriteLine("Partida N°: " + matchNumber);
                tw.WriteLine(record);
                tw.Close();
            }
        }
    }

    //Set current controller to False, and find a new one that is connected
    public void ChangeControlEnemies()
    {

        foreach (NetworkPlayer player in players)
        {
            if (player.controlOverEnemies == true)
            {
                player.controlOverEnemies = false;
            }
        }
        foreach (NetworkPlayer player in players)
        {
            if (player.connected == true)
            {
                player.controlOverEnemies = true;
                SendControlEnemiesToClient(player);
                break;
            }
        }
    }

    private void SendControlEnemiesToClient(NetworkPlayer player)
    {
        string message = "SetControlOverEnemies";
        Server.instance.SendMessageToClient(player, message, true);
    }

    #endregion

    #region Switches

    public ServerSwitch AddSwitch(int groupId, int individualId)
    {
        foreach (ServerSwitch switchu in switchs)
        {
            if (switchu.groupId == groupId && switchu.individualId == individualId)
            {
                return switchu;
            }
        }
        ServerSwitch switchi = new ServerSwitch(groupId, individualId, this);
        switchs.Add(switchi);
        return switchi;
    }

    public ServerSwitch GetSwitch(int groupId, int individualId)
    {
        foreach (ServerSwitch switchi in switchs)
        {
            if (switchi.groupId == groupId && switchi.individualId == individualId)
            {
                return switchi;
            }
        }
        ServerSwitch switchis = AddSwitch(groupId, individualId);
        return switchis;
    }

    private bool CheckIfSwitchExist(int groupId, int individualId)
    {
        if (GetSwitch(groupId, individualId) == null)
        {
            return false;
        }
        return true;
    }

    public void SetSwitchOn(bool on, int groupId, int individualId)
    {
        ServerSwitch switchi = GetSwitch(groupId, individualId);
        switchi.on = on;
    }

    #endregion

    #region Record

    public void WriteFeedbackRecord(string message)
    {
        record += "\r\n" + message + HoraMinuto();
    }

    #endregion

    #endregion

    #region Utils

    private int GetCharId()
    {
        return numPlayers++;
    }

    public string HoraMinuto()
    {
        string hora = DateTime.Now.Hour.ToString();
        string minutos = DateTime.Now.Minute.ToString();

        if (minutos.Length == 1)
        {
            minutos = "0" + minutos;
        }

        string tiempo = " (" + hora + ":" + minutos + ")";
        return tiempo;
    }

    public bool IsFull()
    {
        return numPlayers == maxPlayers;
    }

    #endregion

    #region Messaging

    public void SendMessageToAllPlayers(string message, bool secure)
    {
        char[] separator = new char[1] { '/' };
        string[] msg = message.Split(separator);

        if (msg[0] == "NewChatMessage")
        {
            actualChat += msg[1];
            record += "\r\n" + actualChat + HoraMinuto();
        }

        foreach (NetworkPlayer player in players)
        {
            if (player.connected)
            {
                server.SendMessageToClient(player.connectionId, message, secure);
            }
        }
    }

    public void SendMessageToAllPlayersExceptOne(string message, int connectionId, bool secure)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.connected && player.connectionId != connectionId)
            {
                server.SendMessageToClient(player.connectionId, message, secure);
            }
        }
    }

    public void SendMessageToPlayer(string message, int connectionId, bool secure)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.connected && player.connectionId == connectionId)
            {
                server.SendMessageToClient(player.connectionId, message, secure);
            }
        }
    }

    #endregion

}
