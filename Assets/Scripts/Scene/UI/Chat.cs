﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
using UnityEngine.SceneManagement;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    public Text myName = null; //Lo que se está escribiendo, se hace en "myName"
    public Text theirName = null; //Es el chat
    public Text textOriginalCanvas;
    public GameObject originalCanvas;
    public GameObject chatCanvas;
    public GameObject inventory;
    public GameObject displayPanel;

    string word;
    string entered;

    int wordIndex = 0;
    public int numMaxPlayers = 1;

    bool mayus = false;

    public void Start()
    {
        instance = this;
        originalCanvas = GameObject.FindGameObjectWithTag("OriginalCanvas");
        chatCanvas = GameObject.FindGameObjectWithTag("ChatCanvas");

        if (SceneManager.GetActiveScene().name != "ServerScene")
        {
            textOriginalCanvas = GameObject.FindGameObjectWithTag("OriginalTextCanvas").GetComponent<Text>();
            ToggleChatOff();
        }
    }

    public string SetJugador()
    {
        PlayerController player1 = GameObject.Find("Mage").GetComponent<PlayerController>();
        PlayerController player2 = GameObject.Find("Warrior").GetComponent<PlayerController>();
        PlayerController player3 = GameObject.Find("Engineer").GetComponent<PlayerController>();

        string role;
        string charId;

        if (player1.localPlayer)
        {
            charId = player1.characterId.ToString();
        }
        else if (player2.localPlayer)
        {
            charId = player2.characterId.ToString();
        }
        else
        {
            charId = player3.characterId.ToString();
        }

        switch (charId)
        {
            case "0":
                role = "Mage";
                break;
            case "1":
                role = "Warrior";
                break;
            case "2":
                role = "Engineer";
                break;
            default:
                return null;
        }
        return role;
    }

    public void AlphabetFunction(string alphabet)
    {
        bool delete = false;
        bool enter = false;

        if (alphabet == "mayus")
        {
            alphabet = "";
            mayus = !mayus;
        }
        else if (alphabet == "delete")
        {
            alphabet = "";
            delete = true;
        }
        else if (alphabet == "enter" && word != "")
        {
            alphabet = "";
            enter = true;
        }
        else if (alphabet == "enter" && word == "")
        {
            alphabet = "";
            return;
        }

        if (mayus)
        {
            string letra = MayusFunction(alphabet);
            alphabet = letra;
        }
        else
        {
            alphabet = alphabet.ToLower();
        }

        if (delete)
        {
            string[] myNameWord = ChatDelete(alphabet);
            myName.text = myNameWord[0];
            word = myNameWord[1];
        }
        else
        {
            wordIndex++;
            word += alphabet;
            myName.text = word;
        }

        if (enter)
        {
            EnterFunction("");
        }
        else
        {
            return;
        }
    } // Lo que se escribe, manda y recibe

    private string MayusFunction(string alphabet)
    {
        alphabet = alphabet.ToUpper();
        if (alphabet == "?")
        {
            alphabet = "/";
        }
        else if (alphabet == ",")
        {
            alphabet = "<";
        }
        else if (alphabet == ".")
        {
            alphabet = ">";
        }
        else if (alphabet == "1")
        {
            alphabet = "!";
        }
        else if (alphabet == "2")
        {
            alphabet = "'";
        }
        else if (alphabet == "3")
        {
            alphabet = "#";
        }
        else if (alphabet == "4")
        {
            alphabet = "$";
        }
        else if (alphabet == "5")
        {
            alphabet = "%";
        }
        else if (alphabet == "6")
        {
            alphabet = "&";
        }
        else if (alphabet == "7")
        {
            alphabet = "/";
        }
        else if (alphabet == "8")
        {
            alphabet = "(";
        }
        else if (alphabet == "9")
        {
            alphabet = ")";
        }
        else if (alphabet == "0")
        {
            alphabet = "=";
        }
        return alphabet;
    }

    public string[] ChatDelete(string alphabet)
    {
        int largo1 = myName.text.Length;
        int largo2 = word.Length;
        largo1 = largo1 - 1;
        largo2 = largo2 - 1;

        if (largo1 < 0)
        {
            return null;
        }
        else
        {
            myName.text = myName.text.Substring(0, largo1);
            word = word.Substring(0, largo2);
            string[] myNameWord;
            myNameWord = new string[2] { myName.text, word };
            return myNameWord;
        }
    }

    public void EnterFunction(string message)
    {
        if (message == "")
        {
            entered = word;
            word = "";
            myName.text = "";
            string texto = SetJugador() + ": " + entered;
            Client.instance.SendNewChatMessageToServer(texto);
        }
        else
        {
            Client.instance.SendNewChatMessageToServer(message);
        }
    }

    public void UpdateChat(string message)
    {
        char[] separator = new char[1];
        separator[0] = ':';
        string[] msg = message.Split(separator);

        if (msg[0] == "Mage")
        {
            message = "<color=#64b78e>" + message + "</color>";
        }
        else if (msg[0] == "Warrior")
        {
            message = "<color=#e67f84>" + message + "</color>";
        }
        else if (msg[0] == "Engineer")
        {
            message = "<color=#f9ca45>" + message + "</color>";
        }
        theirName.text += "\r\n" + message;
        textOriginalCanvas.text = theirName.text;
    }

    public void ToggleChatOff()
    {
        originalCanvas.SetActive(true);
        chatCanvas.SetActive(false);
        inventory.SetActive(true);
    }

    public void ToggleChatOn()
    {
        chatCanvas.SetActive(true);
        originalCanvas.SetActive(false);
        displayPanel.SetActive(false);
        inventory.SetActive(false);
    }
}
