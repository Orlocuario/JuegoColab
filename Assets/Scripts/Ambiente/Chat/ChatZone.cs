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
    public string HUDRate;
    int rate;
    int countTillRate;
    DisplayHUD displayHudScript;


    private void Start()
    {
        lockValue = false;
        HUDRate = "25";
        rate = 300;
        countTillRate = 0;
        chatButtonOn.SetActive(false);
        chatButtonOff.SetActive(false);
        displayHudScript = GameObject.Find("Canvas").GetComponent<DisplayHUD>();
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        displayHudScript = GameObject.Find("Canvas").GetComponent<DisplayHUD>();
    }

    private void Update()
    {
        PlayerController player = levelManager.thePlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance <= 2f)
        {
            lockValue = true;
            chatButtonOn.SetActive(true);
            chatButtonOff.SetActive(true);
            countTillRate ++;
            if (countTillRate == rate)
            {
                countTillRate = 0;
                if(!(Int32.Parse(displayHudScript.hpCurrentPercentage) == 1))
                {
                    Client.instance.SendMessageToServer("ChangeHpAndManaHUDToRoom/" + HUDRate);
                }
            }
            //Client.instance.SendMessageToServer("ActivateNPCLog/Pickle Rick! :D");
        }
        else
        {
            countTillRate = 0;
            if(lockValue)
            {
                chatButtonOn.SetActive(false);
                chatButtonOff.SetActive(false);
                lockValue = false;
            }
        }
    }
}