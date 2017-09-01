using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocaGigante : MonoBehaviour {

	GameObject createGameObject;
    Quaternion myQuaternion;
	Vector3 myPosition;
	Vector3 aux;

    private void Start()
    {
        aux = this.gameObject.GetComponent<Transform>().position;
    }

    private void Update()
	{
		myPosition = this.gameObject.GetComponent<Transform>().position;
        myQuaternion = this.gameObject.GetComponent<Transform>().rotation;
		if (myPosition != aux) 
		{
			aux = myPosition;
			Client.instance.SendMessageToServer ("ChangeItemPosition/" + this.gameObject.name + "/" + 
                myPosition.x.ToString() + "/" + myPosition.y.ToString() + "/" + myQuaternion.z.ToString());
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "TriggerRocaGigante") 
		{
			createGameObject = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/SueloRoca"));
            Client.instance.SendMessageToServer("DestroyItem/" + this.gameObject.name);
		}
	}
}
