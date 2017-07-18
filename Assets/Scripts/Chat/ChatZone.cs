using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatZone : MonoBehaviour {

    public GameObject chatButtonOn;
    public GameObject chatButtonOff;
    Server server;
    LevelManager levelManager;
    Vector2 myPosition;
    private void Start()
    {
        myPosition = gameObject.transform.position;
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
        }
        else
        {
            chatButtonOn.SetActive(false);
            chatButtonOff.SetActive(false);
            Chat.instance.ToggleChatOff();
        }

    }
}
