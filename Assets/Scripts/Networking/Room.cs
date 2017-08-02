using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Room
{
    List<Jugador> players;
    Server server;
    ServerMessageHandler sender;
    public HpAndManaHUD hpManaGer;
    public int numJugadores;
    public int maxJugadores;
    public int id;

    public bool started;
    string numeroPartidas;
    string historial;
    public List<Enemy> enemigos;
    //Inicialización
    public Room(int id, Server server, ServerMessageHandler sender, int maxJugadores)
    {
        numJugadores = 0;
        this.id = id;
        this.maxJugadores = maxJugadores;
        players = new List<Jugador>();
        this.server = server;
        this.sender = sender;
        started = false;
        historial = "";
        hpManaGer = new HpAndManaHUD(this);
        enemigos = new List<Enemy>();
    }

    public void AddEnemy(int enemyId)
    {
        enemigos.Add(new Enemy(enemyId));
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
        if(numJugadores == maxJugadores)
        {
            return true;
        }
        return false;
    }

    //Agrega a un jugador a la sala. Retorna true si lo consigue, false si está llena.
    public bool AddPlayer(int connectionId)
    {
        if (IsFull())
        {
            return false;
        }
        Jugador newPlayer = new Jugador(connectionId, GetCharId(numJugadores), this);
        players.Add(newPlayer);
        numJugadores++;
        
        if (IsFull())
        {
            Debug.Log("Full room");
            sender.SendChangeScene("Escena1", this);
            started = true;
            SendMessageToAllPlayers("Mago: Conectado");
            SendMessageToAllPlayers("Guerrero: Conectado");
            SendMessageToAllPlayers("Ingeniero: Conectado");
        }
        return true;
    }

    private int GetCharId(int numJugadores)
    {
        return numJugadores;
    }

    public Enemy GetEnemy(int id)
    {
        foreach(Enemy enemy in enemigos)
        {
            if(enemy.enemyId == id)
            {
                return enemy;
            }
        }
        return null;
    }

    public Jugador FindPlayerInRoom(int id)
    {
        foreach(Jugador player in players)
        {
            if (player.connectionId == id)
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
        string[] arreglo = message.Split(separator);
        if (arreglo[0] == "NewChatMessage")
        {
            historial += "\r\n" + arreglo[1] + HoraMinuto();
        }

        foreach (Jugador player in players)
        {
            if (player.connected)
            {
                server.SendMessageToClient(player.connectionId, message);
            }
        }
    }

    public void SendMessageToAllPlayersExceptOne(string message, int connectionId)
    {
        foreach (Jugador player in players)
        {
            if (player.connected && player.connectionId!=connectionId)
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
}
