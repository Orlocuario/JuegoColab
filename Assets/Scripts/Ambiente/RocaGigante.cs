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
		if (myPosition.ToString().Substring(0,4) != aux.ToString().Substring(0, 4) && Client.instance.GetWarrior().localPlayer == true) 
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
            Client.instance.SendMessageToServer("InstantiateObject/Prefabs/Ambientales/SueloRoca");
            Client.instance.SendMessageToServer("DestroyObject/" + this.gameObject.name);
		}
	}
}
