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

    private GameObject[] particles;
    private HUDDisplay hpAndMp;

    private static float regenerationUnits = 1;
    private static int regenerationFrameRate = 25;

    private int regenerationFrame;
    private bool activated;

    #endregion

    #region Start & Update

    private void Start()
    {
        regenerationFrame = 0;
        activated = false;

        InitializeChatButtons();
        InitializeParticles();
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
                    hpAndMp.ChangeHPAndMP(regenerationUnits);
                    SendMessageToServer("ChangeHpAndMpHUDToRoom/" + regenerationUnits);
                }
            }
        }
    }

    #endregion

    #region Utils

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        if (_particles.Length <= 0)
        {
            return;
        }

        particles = new GameObject[_particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = _particles[i].gameObject;
        }

        ToogleParticles(false);

    }

    protected void ToogleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

    protected void InitializeChatButtons()
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

    protected void ToogleChatButtons(bool activate)
    {
        if (chatButtonOn && chatButtonOff)
        {
            chatButtonOn.SetActive(activate);
            chatButtonOff.SetActive(activate);
        }
    }

    protected bool GameObjectIsPlayer(GameObject other, bool isLocal)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (isLocal)
        {
            return playerController && playerController.localPlayer;
        }
        else
        {
            return playerController;
        }
    }

    #endregion

    #region Events

    // Attack those who enter the alert zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject, true))
        {
            ToogleChatButtons(true);
            ToogleParticles(true);
            activated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject, true))
        {
            regenerationFrame = 0;
            ToogleChatButtons(false);
            ToogleParticles(false);
            activated = false;
            hpAndMp.StopParticles();
        }
    }

    private void SendMessageToServer(string message)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, false);
        }
    }

    #endregion

}