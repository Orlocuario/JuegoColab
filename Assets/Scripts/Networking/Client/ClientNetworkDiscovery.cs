using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientNetworkDiscovery : NetworkDiscovery
{
    Client scriptClient;
    // Use this for initialization
    void Start()
    {
        Initialize();
        scriptClient = GetComponent<Client>();
    }

    public void InitializeListening()
    {

        if (StartAsClient())
        {
            Debug.Log("Client started listenting locally");
            GameObject.Find("ConnectText").GetComponent<Text>().text = "Conectando...";
        }

    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        char[] separator = new char[1] { '/' };
        string[] parsedData = data.Split(separator);

        if (parsedData.Length > 0 && parsedData[0] == "port")
        {
            Debug.Log("Client received broadcast from " + fromAddress + ":" + parsedData[1]);
        }

        bool connected = false;

        while (!connected)
        {
            connected = scriptClient.Connect(fromAddress, int.Parse(parsedData[1]));
        }

        GameObject.Find("ConnectText").GetComponent<Text>().text = "Esperando...";
        StopBroadcast();

    }

}
