using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Room
{
    List<Jugador> players;
    public int numJugadores;
    public int maxJugadores;
    public int id;
    Server server;
    MessageHandler sender;

    //Inicialización
    public Room(int id, Server server, MessageHandler sender)
    {
        numJugadores = 0;
        this.id = id;
        this.maxJugadores = 1;
        players = new List<Jugador>();
        this.server = server;
        this.sender = sender;
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
            sender.SendChangeScene("Basura", this);
        }
        return true;
    }

    private int GetCharId(int numJugadores)
    {
        return numJugadores;
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
        foreach(Jugador player in players)
        {
            if (player.connected)
            {
                server.SendMessageToClient(player.connectionId, message);
            }
        }
    }

    
}
