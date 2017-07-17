using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Hola Esto es un comentario

public class TouchController : MonoBehaviour {

    public void OnClick()
    {
        GameObject client = GameObject.Find("ClientObject");
        Listening listen = client.GetComponent<Listening>();
        GameObject.Find("ConnectText").GetComponent<Text>().text = "Conectando...";
        listen.InitializeListening();

    }
}
