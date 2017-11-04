using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Networking;

public class Room
{

    public ServerMessageHandler sender;
    public Server server;

    public GlobalHpMpHUD hpManaGer;

    public List<NetworkPlayer> players;
    public List<NetworkEnemy> enemies;
    public List<ServerSwitch> switchs;
    public List<int> activatedGroups; //guarda los numeros de los grupos de switchs activados

    public string sceneToLoad;
    public string actualChat;
    public int numJugadores;
    public int maxJugadores;
    public bool started;
    public int id;

    private string numeroPartidas;
    private string historial;
    public ServerDoorsManager doorManager;

    //Inicialización
    public Room(int id, Server server, ServerMessageHandler sender, int maxJugadores)
    {
        numJugadores = 0;
        this.doorManager = new ServerDoorsManager();
        this.maxJugadores = maxJugadores;
        this.id = id;

        hpManaGer = new GlobalHpMpHUD(this);
        switchs = new List<ServerSwitch>();
        players = new List<NetworkPlayer>();
        enemies = new List<NetworkEnemy>();
        activatedGroups = new List<int>();

        this.server = server;
        this.sender = sender;

        started = false;
        historial = "";

        sceneToLoad = Server.instance.sceneToLoad;
    }

    public void AddEnemy(int instanceId, int enemyId, float hp)
    {
        NetworkEnemy enemy = new NetworkEnemy(instanceId, enemyId, hp, this);
        enemies.Add(enemy);
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

    //Retorna true si no cabe más gente.
    public bool IsFull()
    {
        return numJugadores == maxJugadores;
    }

    //Agrega a un jugador a la sala. Retorna true si lo consigue, false si está llena.
    public bool AddPlayer(int connectionId, string address)
    {
        if (IsFull())
        {
            return false;
        }

        NetworkPlayer newPlayer = new NetworkPlayer(connectionId, GetCharId(numJugadores), this, address);
        players.Add(newPlayer);
        numJugadores++;
        SetControlEnemies(newPlayer);

        if (IsFull())
        {
            Debug.Log("Full room");
            sender.SendChangeScene(sceneToLoad, this);
            started = true;
            SendMessageToAllPlayers("Mago: Conectado");
            SendMessageToAllPlayers("Guerrero: Conectado");
            SendMessageToAllPlayers("Ingeniero: Conectado");
        }

        return true;
    }

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

                SendMessageToAllPlayers(message);
            }
        }
    }

    private int GetCharId(int numJugadores)
    {
        return numJugadores;
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

    public void SendMessageToAllPlayers(string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] msg = message.Split(separator);

        if (msg[0] == "NewChatMessage")
        {
            actualChat += msg[1];
            historial += "\r\n" + actualChat + HoraMinuto();
        }

        foreach (NetworkPlayer player in players)
        {
            if (player.connected)
            {
                server.SendMessageToClient(player.connectionId, message);
            }
        }
    }

    public void SendMessageToAllPlayersExceptOne(string message, int connectionId)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.connected && player.connectionId != connectionId)
            {
                server.SendMessageToClient(player.connectionId, message);
            }
        }
    }

    public void SendMessageToPlayer(string message, int connectionId)
    {
        foreach (NetworkPlayer player in players)
        {
            if (player.connected && player.connectionId == connectionId)
            {
                server.SendMessageToClient(player.connectionId, message);
            }
        }
    }

    public void CreateTextChat()
    {
        numeroPartidas = "Por Resolver";
        string path = Directory.GetCurrentDirectory() + "/ChatLogFromRoomN°" + id + ".txt";

        if (!File.Exists(path))
        {
            using (var tw = new StreamWriter(File.Create(path)))
            {
                tw.WriteLine("Partida N°: " + numeroPartidas);
                tw.WriteLine(historial);
                tw.Close();
            }
        }
        else if (File.Exists(path))
        {
            using (var tw = new StreamWriter(path, true))
            {
                tw.WriteLine("\r\n" + "____________________________________");
                tw.WriteLine("Generando Nuevo Historial...");
                tw.WriteLine("Partida N°: " + numeroPartidas);
                tw.WriteLine(historial);
                tw.Close();
            }
        }
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
                SendControlSignal(player);
            }
        }
    }

    private void SendControlSignal(NetworkPlayer player)
    {
        string message = "SetControlOverEnemies";
        Server.instance.SendMessageToClient(player, message);
    }

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

    public void WriteFeedbackHistorial(string message)
    {
        historial += "\r\n" + message + HoraMinuto();
    }
}
