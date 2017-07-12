using System;
using System.Collections;
using System.Collections.Generic;
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

	void Start () {
        DontDestroyOnLoad(this);
        instance = this;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);
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
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        int bufferSize = 1024;
        NetworkTransport.Send(socketId, connectionId, channelId, buffer, bufferSize, out error);
    }

    void LateUpdate()
    {
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
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
                GameObject.Find("ConnectText").GetComponent<Text>().text = message;
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
            default:
                break;
        }
    }

    private void HandleChangeScene(string[] arreglo)
    {
        string scene = arreglo[1];
        SceneManager.LoadScene(scene);
    }

    private void HandleSetCharId(string[] arreglo)
    {
        string charId = arreglo[1];
        GameObject[] lista = UnityEngine.Object.FindObjectsOfType<GameObject>();
        Text texto = GameObject.Find("ConnectText").GetComponent<Text>();
        texto.text = "BASUUUUUUURA TESTING" + charId;
    }

}
