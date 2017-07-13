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
    int bufferSize = 15;

	void Start () {
        DontDestroyOnLoad(this);
        instance = this;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, 10);
        socketId = NetworkTransport.AddHost(topology, port);
    }

    public void Connect(string ip)
    {
        byte error;
        connectionId = NetworkTransport.Connect(socketId, ip, port, 0, out error);
    }

    public void SendMessageToServer(string message)
    {
        byte error;
        int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
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
                HandleMessage(message);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("disconnected from server");
                break;
        }
    }

    public void RequestCharIdToServer()
    {
        SendMessageToServer("RequestCharId");
    }

    private void HandleMessage(string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] arreglo = message.Split(separator);
        switch (arreglo[0])
        {
            case "ChangeScene":
                HandleChangeScene(arreglo);
                break;
            case "SetCharId":
                HandleSetCharId(arreglo);
                break;
            case "ChangePosition":
                HandleChangePosition(arreglo);
                break;
            default:
                break;
        }
    }

    private void HandleChangePosition(string[] data)
    {
        int charId = Int32.Parse(data[1]);
        float positionX = float.Parse(data[2], CultureInfo.InvariantCulture);
        float positionY = float.Parse(data[3], CultureInfo.InvariantCulture);
        bool isGrounded = bool.Parse(data[4]);
        float speed = float.Parse(data[5], CultureInfo.InvariantCulture);
        int direction = Int32.Parse(data[6]);
        GameObject player;
        switch (charId)
        {
            case 0:
                player = GameObject.FindGameObjectsWithTag("Player1")[0];
                break;
            case 1:
                player = GameObject.FindGameObjectsWithTag("Player2")[0];
                break;
            case 2:
                player = GameObject.FindGameObjectsWithTag("Player1")[0];
                break;
            default:
                player = null;
                break;
        }
        PlayerController script = player.GetComponent<PlayerController>();
        script.SetVariablesFromServer(positionX, positionY, isGrounded, speed, direction);

    }
    private void HandleChangeScene(string[] arreglo)
    {
        string scene = arreglo[1];
        SceneManager.LoadScene(scene);
    }

    private void HandleSetCharId(string[] arreglo)
    {
        string charId = arreglo[1];
        int charIdint = Convert.ToInt32(charId);
        LevelManager scriptLevel = GameObject.FindGameObjectsWithTag("LevelManager")[0].GetComponent<LevelManager>();
        scriptLevel.SetCharAsLocal(charIdint);
        
    }

}
