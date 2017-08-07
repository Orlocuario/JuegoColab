﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour {

	public void ChatButton()
    {
        List<Room> rooms = Server.instance.rooms;
        foreach (Room room in rooms)
        {
            room.CreateTextChat();
        }
    }

    public void ServerButton()
    {
        Broadcasting script = Server.instance.gameObject.GetComponent<Broadcasting>();
        script.ServerInitialize();
    }

    public void MaxPlayerRoomButton()
    {
        Text inputText = GameObject.Find("InputPlayerText").GetComponent<Text>();
        int number = Int32.Parse(inputText.text);
        List<Room> rooms = Server.instance.rooms;
        foreach (Room room in rooms)
        {
            room.maxJugadores = number;
        }
        Server.instance.maxJugadores = number;
    }
}