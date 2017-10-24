using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Client : MonoBehaviour
{

    public static Client instance;
    ClientMessageHandler handler;

    private static int maxConnections = 12;

    int connectionId;
    int reliableChannelId;
    string serverIp;
    int unreliableChannelId;
    int socketId; // Host ID

    void Start()
    {

        DontDestroyOnLoad(this);
        instance = this;

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();

        unreliableChannelId = config.AddChannel(QosType.Unreliable);
        reliableChannelId = config.AddChannel(QosType.ReliableFragmented);

        HostTopology topology = new HostTopology(config, maxConnections);

        socketId = NetworkTransport.AddHost(topology, NetConsts.port);

        handler = new ClientMessageHandler();
    }

    public void Connect(string ip)
    {
        byte error;
        serverIp = ip;
        connectionId = NetworkTransport.Connect(socketId, ip, NetConsts.port, 0, out error);
    }

    public void Connect()
    {
        try
        {
            byte error;
            connectionId = NetworkTransport.Connect(socketId, serverIp, NetConsts.port, 0, out error);
        }
        catch
        {
            Debug.Log("Connection to server failed");
        }
    }

    public void SendMessageToServer(string message)
    {
        byte error;
        byte[] buffer = new byte[NetConsts.bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, connectionId, unreliableChannelId, buffer, NetConsts.bufferSize, out error);
    }

    public void SendMessageToPlanner(string message)
    {
        byte error;
        byte[] buffer = new byte[NetConsts.bigBufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, connectionId, reliableChannelId, buffer, NetConsts.bigBufferSize, out error);
    }

    void LateUpdate()
    {
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[NetConsts.bufferSize];
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetConsts.bufferSize, out dataSize, out error);
        NetworkError Error = (NetworkError)error;
        if (Error == NetworkError.MessageToLong)
        {
            //Trata de capturar el mensaje denuevo, pero asumiendo buffer más grande.
            recBuffer = new byte[NetConsts.bigBufferSize];
            recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetConsts.bigBufferSize, out dataSize, out error);
        }
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Scene currentScene = SceneManager.GetActiveScene();
                if (!(currentScene.name == "ClientScene"))
                {
                    if (GetLocalPlayer())
                    {
                        GetLocalPlayer().Conectar(true);
                        LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
                        lm.MostrarReconectando(false);
                    }
                }
                Debug.Log("Connection succesfull");
                break;

            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                if (recChannelId == unreliableChannelId)
                {
                    handler.HandleMessage(message);
                }
                if (recChannelId == reliableChannelId)
                {
                    ReceiveMessageFromPlanner(message, recConnectionId);
                }
                Debug.Log("From: " + connectionId +" Handling: " + message);
                break;

            case NetworkEventType.DisconnectEvent:
                if (connectionId == recConnectionId) //Detectamos que fuimos nosotros los que nos desconectamos
                {
                    currentScene = SceneManager.GetActiveScene();
                    if (!(currentScene.name == "ClientScene"))
                    {
                        GetLocalPlayer().Conectar(false);
                    }
                    Reconnect();
                }
                Debug.Log("Disconnected from server");
                break;
        }
    }

    private void Reconnect()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (!(currentScene.name == "ClientScene"))
        {
            //Asumo que si no estoy en la ClientScene, existe un LevelManager
            LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            lm.MostrarReconectando(true);
        }
        Connect();

    }

    private void ReceiveMessageFromPlanner(string message, int connectionId)
    {
        Planner planner = FindObjectOfType<Planner>();
        planner.SetPlanFromServer(message);
    }

    public void StartFirstPlan()
    {
        Planner planner = FindObjectOfType<Planner>();
        planner.FirstPlan();
    }

    public PlayerController GetPlayerController(int charId)
    {

        PlayerController script;
        GameObject player;

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

    public EnemyController GetEnemy(int enemyId)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController script = enemy.GetComponent<EnemyController>();
            if (script.enemyId == enemyId)
            {
                return script;
            }
        }
        return null;
    }

    public PlayerController GetLocalPlayer()
    {

        GameObject[] player1 = GameObject.FindGameObjectsWithTag("Player1");
        GameObject[] player2 = GameObject.FindGameObjectsWithTag("Player2");
        GameObject[] player3 = GameObject.FindGameObjectsWithTag("Player3");

        if (player1.Length > 0)
        {
            MageController player1Controller = player1[0].GetComponent<MageController>();
            if (player1Controller.localPlayer)
            {
                return player1Controller;
            }
        }

        if (player2.Length > 0)
        {
            WarriorController player2Controller = player2[0].GetComponent<WarriorController>();
            if (player2Controller.localPlayer)
            {
                return player2Controller;
            }
        }

        if (player3.Length > 0)
        {
            EngineerController player3Controller = player3[0].GetComponent<EngineerController>();
            if (player3Controller.localPlayer)
            {
                return player3Controller;
            }
        }

        return null;
    }
    public PlayerController GetById(int playerId)
    {
        if (playerId == 0)
        {
            return GetMage();
        }

        else if (playerId == 1)
        {
            return GetWarrior();
        }

        else
        {
            return GetEngineer();
        }
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

    public EngineerController GetEngineer()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player3")[0];
        EngineerController script = player.GetComponent<EngineerController>();
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
