using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkDiscovery : NetworkDiscovery
{

    public int port;

    public void ServerInitialize()
    {
        Initialize();

        Server server = GetComponent<Server>();

        port = server.port;

        broadcastData = "port/" + server.port;

        InitializeBroadcast();
    }

    public void InitializeBroadcast()
    {
        if (StartAsServer())
        {
            Debug.Log("Server started broadcasting locally from port " + port);

        }
    }

    public void ResetServer()
    {
        StopBroadcast();
        StartAsServer();
    }
}