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


public class Client : MonoBehaviour {

    int port = 7777;
    int socketId; // Host ID
    int connectionId;
    int bigChannelId;
    int channelId;
    public static Client instance;
    int bufferSize = 100;
    int bigBufferSize = 64000;
    ClientMessageHandler handler;
    string serverIp;

	void Start () {
        DontDestroyOnLoad(this);
        instance = this;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Unreliable);
        bigChannelId = config.AddChannel(QosType.ReliableFragmented);
        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
        handler = new ClientMessageHandler();
    }

    public void Connect(string ip)
    {
        byte error;
        serverIp = ip;
        connectionId = NetworkTransport.Connect(socketId, ip, port, 0, out error);
    }

    public void Connect()
    {
        try
        {
            byte error;
            connectionId = NetworkTransport.Connect(socketId, serverIp, port, 0, out error);
        }
        catch
        {
            Debug.Log("Connection to server failed");
        }
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

    public void SendMessageToPlanner(string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[bigBufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, connectionId, bigChannelId, buffer, bigBufferSize, out error);
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
        NetworkError Error = (NetworkError)error;
        if (Error == NetworkError.MessageToLong)
        {
            //Trata de capturar el mensaje denuevo, pero asumiendo buffer más grande.
            recBuffer = new byte[bigBufferSize];
            recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, bigBufferSize, out dataSize, out error);
        }
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Scene currentScene = SceneManager.GetActiveScene();
               if (GetLocalPlayer() && !(currentScene.name == "ClientScene"))
                {
                    GetLocalPlayer().conectar(true);
                    LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
                    lm.MostrarReconectando(false);
                }
                Debug.Log("connection succesfull");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                if(recChannelId == channelId)
                {
                    handler.HandleMessage(message);
                }
                if(recChannelId == bigChannelId)
                {
                    ReceiveMessageFromPlanner(message, recConnectionId);
                }
                Debug.Log("incoming message event received: " + message);
                break;
            case NetworkEventType.DisconnectEvent:
                if(connectionId == recConnectionId) //Detectamos que fuimos nosotros los que nos desconectamos
                {
                    GetLocalPlayer().conectar(false);
                    Reconnect();
                }
                Debug.Log("disconnected from server");
                break;
        }
    }

    private void Reconnect()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if(! (currentScene.name == "ClientScene"))
        {
            //Asumo que si no estoy en la ClientScene, existe un LevelManager
            LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            lm.MostrarReconectando(true);
            Connect();
        }
    }

    private void ReceiveMessageFromPlanner(string message, int connectionId)
    {
		Planner planner = FindObjectOfType<Planner> ();
		planner.SetPlanFromServer (message);
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
        MageController player1 = GameObject.FindGameObjectsWithTag("Player1")[0].GetComponent<MageController>();
        WarriorController player2 = GameObject.FindGameObjectsWithTag("Player2")[0].GetComponent<WarriorController>();
        EngineerController player3 = GameObject.FindGameObjectsWithTag("Player3")[0].GetComponent<EngineerController>();
        if (player1.localPlayer)
        {
            return player1;
        }
        if (player2.localPlayer)
        {
            return player2;
        }
        if (player3.localPlayer)
        {
            return player3;
        }
        return null;
    }
	public PlayerController GetById(int playerId)
	{
		if (playerId == 0) 
		{
			return GetMage ();
		} 

		else if (playerId == 1) 
		{
			return GetWarrior ();
		}

		else 
		{
			return GetEngineer ();
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
        GameObject player = GameObject.FindGameObjectsWithTag("Player2")[0];
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
