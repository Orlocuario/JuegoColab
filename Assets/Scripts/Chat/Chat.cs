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
    public Text textOriginalCanvas;
    public GameObject originalCanvas;
    public GameObject chatCanvas;

    string word;
    string entered;
    string historial;
    string numeroPartidas;

    int wordIndex = 0;
    int numMaxJugadores = 2;

    bool inicializador;
    bool mayus = false;

    public void Start()
    {
        instance = this;
        inicializador = true;
        originalCanvas = GameObject.FindGameObjectWithTag("OriginalCanvas");
        chatCanvas = GameObject.FindGameObjectWithTag("ChatCanvas");
        textOriginalCanvas = GameObject.FindGameObjectWithTag("OriginalTextCanvas").GetComponent<Text>();
        ToggleChatOff();
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
            EnterFunction(false, "");
        }
        else
            {
                return;
            }
    } // Lo que se escribe, manda y recibe

    public void EnterFunction(bool connection, string message)  {
        if (!connection)
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
            Client.instance.SendNewChatMessageToServer(message);
        }
        
    }

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

    public void UpdateChat(string message)
    {
        char[] separator = new char[1];
        separator[0] = ':';
        string[] arreglo = message.Split(separator);
        
        if (!inicializador)
        {
            if (arreglo[0] == "Mage")
            {
                message = "<color=#64b78e>" + message + "</color>";
            }
            else if (arreglo[0] == "Warrior")
            {
                message = "<color=#e67f84>" + message + "</color>";
            }
            else if (arreglo[0] == "Engineer")
            {
                message = "<color=#f9ca45>" + message + "</color>";
            }
            theirName.text += "\r\n" + message;
            textOriginalCanvas.text = theirName.text;
        }
        else
        {
            ChatInitializer(arreglo);
        }     
    }

    private void ChatInitializer(string[] arreglo)
    {
        numMaxJugadores -= 1;
        if (numMaxJugadores == 0)
        {
            inicializador = false;
        }

        string messageInit = null;
        if (arreglo[0] == "Mago")
        {
            messageInit = "<color=#64b78e>Mage Has Connected</color>";
        }
        else if (arreglo[0] == "Guerrero")
        {
            messageInit = "<color=#e67f84>Warrior Has Connected</color>";
        }
        else if (arreglo[0] == "Ingeniero")
        {
            messageInit = "<color=#f9ca45>Engineer Has Connected</color>";
        }
        textOriginalCanvas = GameObject.FindGameObjectWithTag("OriginalTextCanvas").GetComponent<Text>();
        theirName.text += "\r\n" + messageInit;
        textOriginalCanvas.text = theirName.text;
    }

    public void ToggleChatOff()
    {
        originalCanvas.SetActive(true);
        chatCanvas.SetActive(false);
    }

    public void ToggleChatOn()
    {
        chatCanvas.SetActive(true);
        originalCanvas.SetActive(false);
    }

    public int GetNumberOfGamesPlayed()
    {
        int numberOfGamesPlayed = Server.instance.NumberOfScenes1();
        return numberOfGamesPlayed;
    }

    public void CreateTextChat() //proviene de cuando se corta el juego
    {
        numeroPartidas = GetNumberOfGamesPlayed().ToString();
        string path = Directory.GetCurrentDirectory() + "/HistoricalChat.txt";

        if (!File.Exists(path))
        {
            using (var tw = new StreamWriter(File.Create(path)))
            {
                tw.WriteLine("Partida N°: " + numeroPartidas);
                tw.WriteLine(historial);
                tw.Close();
            }
        }
       else if (File.Exists(path))
       {
           using (var tw = new StreamWriter(path, true))
           {
               tw.WriteLine("\r\n" + "____________________________________");
               tw.WriteLine("Generando Nuevo Historial...");
               tw.WriteLine("Partida N°: " + numeroPartidas);
               tw.WriteLine(historial);
               tw.Close();
           }
       }
    }
}
