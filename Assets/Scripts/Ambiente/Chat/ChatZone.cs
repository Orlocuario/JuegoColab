using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatZone : MonoBehaviour
{

    private DisplayHUD displayHudScript;
    public GameObject chatButtonOff;
    public GameObject chatButtonOn;
    private Vector2 myPosition;

    private static string regenerationUnits = "1";
    private static int regenerationFrameRate = 50;
    private static float activationDistance = 2f;

    private int regenerationFrame;
    private bool activated;

    private void Start()
    {
        activated = false;
        regenerationFrame = 0;

        chatButtonOn.SetActive(false);
        chatButtonOff.SetActive(false);

        myPosition = gameObject.transform.position;

        displayHudScript = GameObject.Find("Canvas").GetComponent<DisplayHUD>();
    }

    private void Update()
    {
        if(Client.instance == null || Client.instance.GetLocalPlayer() == null)
        {
            return;
        }

        PlayerController player = Client.instance.GetLocalPlayer();
        Vector2 playerPosition = player.gameObject.transform.position;
        float playerDistance = (playerPosition - myPosition).magnitude;

        if (playerDistance <= activationDistance)
        {
            if (activated)
            {
                if (displayHudScript.hpCurrentPercentage < 1f || displayHudScript.mpCurrentPercentage < 1f)
                {
                    regenerationFrame++;

                    if (regenerationFrame == regenerationFrameRate)
                    {
                        regenerationFrame = 0;
                        Client.instance.SendMessageToServer("ChangeHpAndMpHUDToRoom/" + regenerationUnits);
                    }
                } 
            }
            else
            {
                activated = true;
                ToogleChatButtons(true);
            }

        }
        else if (activated)
        {
            regenerationFrame = 0;
            ToogleChatButtons(false);
            activated = false;
        }

    }

    private void ToogleChatButtons(bool activate)
    {
        chatButtonOn.SetActive(activate);
        chatButtonOff.SetActive(activate);
    }
}