using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatZone : MonoBehaviour
{

    private HUDDisplay displayHudScript;
    public GameObject chatButtonOff;
    public GameObject chatButtonOn;

    private static string regenerationUnits = "1";
    private static int regenerationFrameRate = 30;
    private static float activationDistance = 2f;

    private int regenerationFrame;
    private bool activated;

    private void Start()
    {
        regenerationFrame = 0;
        activated = false;
        InitializeChatButtons();
        displayHudScript = GameObject.Find("Canvas").GetComponent<HUDDisplay>();
    }

    private void Update()
    {
        if (activated)
        {
            if (CanRegenerateHPorMP())
            {
                regenerationFrame++;

                if (regenerationFrame == regenerationFrameRate)
                {
                    regenerationFrame = 0;
                    Client.instance.SendMessageToServer("ChangeHpAndMpHUDToRoom/" + regenerationUnits);
                }
            }
        }
    }

    public void InitializeChatButtons()
    {
        chatButtonOn = GameObject.Find("ToggleChatOn");
        chatButtonOff = GameObject.Find("ToggleChatOff");

        if (chatButtonOn != null)
        {
            chatButtonOn.SetActive(false);
        }

        if (chatButtonOff != null)
        {
            chatButtonOff.SetActive(false);
        }

    }

    protected bool CanRegenerateHPorMP()
    {
        return displayHudScript.hpCurrentPercentage < 1f || displayHudScript.mpCurrentPercentage < 1f;
    }


    // Attack those who enter the alert zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            ToogleChatButtons(true);
            activated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            regenerationFrame = 0;
            ToogleChatButtons(false);
            activated = false;
        }
    }

    private void ToogleChatButtons(bool activate)
    {
        if (chatButtonOn != null && chatButtonOff != null)
        {
            chatButtonOn.SetActive(activate);
            chatButtonOff.SetActive(activate);
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

}