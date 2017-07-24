using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Client : MonoBehaviour {

    int port = 8888;
    int socketId; // Host ID
    int connectionId;
    int channelId;
    public static Client instance;
    int bufferSize = 100;
    ClientMessageHandler handler;

	void Start () {
        DontDestroyOnLoad(this);
        instance = this;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
        handler = new ClientMessageHandler();
    }

    public void Connect(string ip)
    {
        byte error;
        connectionId = NetworkTransport.Connect(socketId, ip, port, 0, out error);
    }

    public void SendMessageToServer(string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, connectionId, channelId, buffer, bufferSize, out error);
    }

    void LateUpdate()
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
                Debug.Log("connection succesfull");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                Debug.Log("incoming message event received: " + message);
                handler.HandleMessage(message);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("disconnected from server");
                break;
        }
    }

    public PlayerController GetPlayerController(int charId)
    {
        GameObject player;
        PlayerController script;
        switch (charId)
        {
            case 0:
                player = GameObject.FindGameObjectsWithTag("Player1")[0];
                script = player.GetComponent<MageController>();
                break;
            case 1:
                player = GameObject.FindGameObjectsWithTag("Player2")[0];
                script = player.GetComponent<WarriorController>();
                break;
            case 2:
                player = GameObject.FindGameObjectsWithTag("Player3")[0];
                script = player.GetComponent<EngineerController>();
                break;
            default:
                player = null;
                script = null;
                break;
        }
        return script;
    }

    public MageController GetMage()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player1")[0];
        MageController script = player.GetComponent<MageController>();
        return script;
    }

    public WarriorController GetWarrior()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player2")[0];
        WarriorController script = player.GetComponent<WarriorController>();
        return script;
    }

    public void SendNewChatMessageToServer(string newChatMessage)
    {
        SendMessageToServer("NewChatMessage/" + newChatMessage);
    }

    public void RequestCharIdToServer()
    {
        SendMessageToServer("RequestCharId");
    }
}
