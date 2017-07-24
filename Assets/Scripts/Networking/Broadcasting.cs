using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Broadcasting : NetworkDiscovery
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
}
