using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatZone : MonoBehaviour {

    public GameObject chatButtonOn;
    public GameObject chatButtonOff;
    LevelManager levelManager;
    Vector2 myPosition;
    private bool lockValue;
    public string HUDRate = "5";

    private void Start()
    {
        lockValue = false;
        chatButtonOn.SetActive(false);
        chatButtonOff.SetActive(false);

        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    private void Update()
    {
        PlayerController player = levelManager.thePlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance <= 3)
        {
            chatButtonOn.SetActive(true);
            chatButtonOff.SetActive(true);
            Client.instance.SendMessageToServer("RecoveryHUD/" + HUDRate);
            lockValue = true;
        }
        else
        {
            if(lockValue)
            {
                chatButtonOn.SetActive(false);
                chatButtonOff.SetActive(false);
                lockValue = false;
            }
        }
    }
}
