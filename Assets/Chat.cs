using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    public Text myName = null; // Lo que se está escribiendo, se hace en "myName"
    public Text theirName = null;

    string word;
    string entered;
    string historial;

    int wordIndex = 0;
    int NumeroPartidas = 0;
    int timeMouseDown;

    bool tab = false;
    public static bool mouseDown;

    public void Start()
    {
        instance = this;
    }

    public string SetJugador()
    {
        PlayerController player1 = GameObject.FindGameObjectsWithTag("Player1")[0].GetComponent<PlayerController>();
        PlayerController player2 = GameObject.FindGameObjectsWithTag("Player2")[0].GetComponent<PlayerController>();
        PlayerController player3 = GameObject.FindGameObjectsWithTag("Player3")[0].GetComponent<PlayerController>();

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
                role = "Mago";
                break;
            case "1":
                role = "Guerrero";
                break;
            case "2":
                role = "";
                break;
            default:
                return null;
        }
        return role;
    }

    public string[] Update(string alphabet)
    {
        timeMouseDown += (int)Time.deltaTime;
        int largo1 = myName.text.Length;
        int largo2 = word.Length;
        largo1 = largo1 - 1 * timeMouseDown;
        largo2 = largo2 - 1 * timeMouseDown;
        if (largo1 < 0)
        {
            return null;
        }
        else
        {
            myName.text = myName.text.Substring(0, largo1);
            word = word.Substring(0, largo2);
            string[] myNameWord;
            myNameWord = new string[2] {myName.text, word};
            return myNameWord;
        }
    }
    
    public void AlphabetFunction(string alphabet)
    {
        bool delete = false;
        bool enter = false;

        if (alphabet == "tab")
        {
            alphabet = "";
            tab = !tab;
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

        if (tab)
        {
            string letra = TabFunction(alphabet);
            alphabet = letra;
        }
        else
        {
            alphabet = alphabet.ToLower();
        }

        if (delete)
        {
            string[] myNameWord = Update(alphabet);
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
                entered = word;
                word = "";
                myName.text = "";
                string texto = SetJugador() + ": " + entered;
                Client.instance.SendNewChatMessageToServer(texto);
                historial += "\r\n" + SetJugador() + ": " + entered + HoraMinuto();
            }
        else
            {
                return;
            }
    } // Lo que se escribe, manda y recibe

    void OnPointerDown()
    {
        mouseDown = true;
    }
    void OnPointerUp()
    {
        mouseDown = false;
        timeMouseDown = 0;
    }

    private string TabFunction(string alphabet)
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

    public string HoraMinuto()
    {
        string hora = DateTime.Now.Hour.ToString();
        string minutos = DateTime.Now.Minute.ToString();

        if (minutos.Length == 1)
        {
            minutos = "0" + minutos;
        }

        string tiempo = " (" + hora + ":" + minutos + ")";

        return tiempo;
    } 

    public void UpdateChat(string message)
    {
        theirName.text += message;
    }

    public void CreateTextChat(string exitGame) //proviene de cuando se corta el juego
    {
        if (exitGame == "true")
        {
            exitGame = "false";
            NumeroPartidas += 1;
            string path;
            path = Directory.GetCurrentDirectory() + "/HistoricalChat.txt";

            if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(File.Create(path)))
                {
                    tw.WriteLine("Primera Partida");
                    tw.WriteLine(historial);
                    tw.Close();
                }
            }
            else if (File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine("\r\n" + "____________________________________");
                    tw.WriteLine("\r\n" + "Se ha ejecutado una nueva partida...");
                    tw.WriteLine("\r\n" + "Partida N°: " + NumeroPartidas);
                    tw.WriteLine(historial);
                    tw.Close();
                }
            }
        }
    }
}
