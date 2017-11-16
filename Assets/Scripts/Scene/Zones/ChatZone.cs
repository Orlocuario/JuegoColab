using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatZone : MonoBehaviour
{

    #region Attributes

    public GameObject chatButtonOff;
    public GameObject chatButtonOn;

    private HUDDisplay hpAndMp;

    private static string regenerationUnits = "1";
    private static int regenerationFrameRate = 25;
    private static float activationDistance = 2f;

    private int regenerationFrame;
    private bool activated;

    #endregion

    #region Start & Update

    private void Start()
    {
        regenerationFrame = 0;
        activated = false;
        InitializeChatButtons();
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

    #endregion

    #region Utils

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
        if (!hpAndMp)
        {
            hpAndMp = GameObject.FindObjectOfType<LevelManager>().hpAndMp;
        }

        return hpAndMp.hpCurrentPercentage < 1f || hpAndMp.mpCurrentPercentage < 1f;
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

    #endregion

    #region Events

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

    #endregion

}