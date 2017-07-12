using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prueba : MonoBehaviour {

	void Start ()
    {
        Client client = GameObject.Find("ClientObject").GetComponent<Client>();
        client.RequestCharIdToServer();
    }
	
}
