using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkDiscovery : NetworkDiscovery
{

    public void ServerInitialize()
    {
        Initialize();
        InitializeBroadcast();
    }

    public void InitializeBroadcast()
    {
        StartAsServer();
    }

    public void ResetServer()
    {
        StopBroadcast();
        StartAsServer();
    }
}