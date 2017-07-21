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
    public float maxHP = 250;
    public float maxMP = 250;
    public float currentHP = 1;
    public float currentMP = 1;
    Server server;
    ServerMessageHandler sender;
    public bool started;
    //Inicialización
    public Room(int id, Server server, ServerMessageHandler sender)
    {
        numJugadores = 0;
        this.id = id;
        this.maxJugadores = 3;
        players = new List<Jugador>();
        this.server = server;
        this.sender = sender;
        started = false;
        //currentHP = maxHP;
        //currentMP = maxMP;
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

    public void RecieveHUD(string recoveryRate)
    {
        ChangeMP(recoveryRate);
        ChangeHP(recoveryRate);
    }

    public void ChangeHP(string deltaHP)
    {
        float valueDeltaHP = float.Parse(deltaHP);
        if (valueDeltaHP <= 0) //Loose HP
        {
            currentHP -= valueDeltaHP;
            if (currentHP <= 0)
            {
                currentHP = 0;
                SendMessageToAllPlayers("PlayersAreDead/ReloadLevel");
            }
        }
        else // Gain HP
        {
            currentHP += valueDeltaHP;
            if (currentHP >= maxHP)
            {
                currentHP = maxHP;
            }
        }
    }

    public void ChangeMaxHP(string deltaMaxHP)
    {
        float valueMaxHP = float.Parse(deltaMaxHP);
        maxHP = valueMaxHP;
        ChangeHP(deltaMaxHP);
    }

    public void ChangeMP(string deltaMP)
    {
        float valueDeltaMP = float.Parse(deltaMP);
        if (valueDeltaMP <= 0)//Loose MP
        {
            currentMP -= valueDeltaMP;
            if (currentMP <= 0)
            {
                currentMP = 0;
            }
        }
        else //Gain MP
        {
            currentMP += valueDeltaMP;
            if (currentMP >= maxMP)
            {
                currentMP = maxMP;
            }
        }
    }

    public void ChangeMaxMP(string deltaMaxMP)
    {
        float valueMaxMP = float.Parse(deltaMaxMP);
        maxMP = valueMaxMP;
        ChangeMP(deltaMaxMP);
    }
}
