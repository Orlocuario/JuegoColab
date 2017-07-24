﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;

public class Server : MonoBehaviour {

    public int maxConnections;
    int port = 6675;
    int socketId;
    int channelId;
    int timesScene1IsLoaded;
    public List<Room> rooms;
    public ServerMessageHandler messageHandler;
    public static Server instance;
    int bufferSize = 100;
    public int maxJugadores;

    // Use this for initialization
    void Start()
    {
        maxJugadores = 1;
        instance = this;
        timesScene1IsLoaded = 0;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        socketId = NetworkTransport.AddHost(topology, port);
        rooms = new List<Room>();
        messageHandler = new ServerMessageHandler(this);
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[bufferSize];
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                AddConnection(recConnectionId);
                Debug.Log("incoming connection event received " + recConnectionId);
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                messageHandler.HandleMessage(message, recConnectionId);
                Debug.Log("incoming message event received: " + message);
                break;
            case NetworkEventType.DisconnectEvent:
                DeleteConnection(recConnectionId);
                Debug.Log("remote client event disconnected " + recConnectionId);
                break;
        }
    }

    public void SendMessageToClient(int clientId, string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, clientId, channelId, buffer, bufferSize, out error);
    }

    private void AddConnection(int connectionId)
    {
        //Jugador existía y se reconecta.
        Jugador player = GetPlayer(connectionId);
        if (player != null)
        {
            player.connected = true;
            SendMessageToClient(connectionId, "ChangeScene/Escena1");
            timesScene1IsLoaded += 1;
            return;
        }

        //Jugador no existía y se crea uno nuevo.
        Room room = SearchRoom();
        if(room == null)
        {
            room = new Room(rooms.Count, this, messageHandler, maxJugadores);
            rooms.Add(room);
        }
        room.AddPlayer(connectionId);
    }

    private void DeleteConnection(int connectionId)
    {
        Jugador player = GetPlayer(connectionId);
        if (player != null)
        {
            player.connected = false;
            int charId = player.charId;
            string role;
            if (charId == 0)
            {
                role = "Mage: Has Disconnected";
            }
            else if (charId == 1)
            {
                role = "Warrior: Has Disconnected";
            }
            else
            {
                role = "Engineer: Has Disconnected";
            }
            player.room.SendMessageToAllPlayers("NewChatMessage/" + role);
        }
    }

    public Jugador GetPlayer(int connectionId)
    {
        foreach (Room room in rooms)
        {
            Jugador player = room.FindPlayerInRoom(connectionId);
            if (player != null)
            {
                return room.FindPlayerInRoom(connectionId);
            }
        }
        return null;
    }
    //Retorna la sala con la mayor cantidad de jugadores que no esté llena.
    private Room SearchRoom()
    {
        Room selectedRoom = null;
        int selectedMaxPlayers = 0;
        foreach(Room room in rooms)
        {
            if (!room.IsFull())
            {
                if(selectedMaxPlayers <= room.numJugadores)
                {
                    selectedRoom = room;
                    selectedMaxPlayers = room.numJugadores;
                }
            }
        }
        return selectedRoom;
    }
}
