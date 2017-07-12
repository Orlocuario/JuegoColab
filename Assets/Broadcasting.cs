using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Broadcasting : NetworkDiscovery
{

    // Use this for initialization
    void Start()
    {
        Initialize();
        InitializeBroadcast();
    }

    public void InitializeBroadcast()
    {
        StartAsServer();
    }
}
