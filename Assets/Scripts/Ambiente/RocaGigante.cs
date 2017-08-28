using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocaGigante : MonoBehaviour {

	GameObject createGameObject;
	Transform myTransform;
	Transform aux;

	private void Update()
	{
		myTransform = this.gameObject.GetComponent<Transform> ();
		if (myTransform != aux) 
		{
			aux = myTransform;
			Client.instance.SendMessageToServer ("ChangePosition/" + this.gameObject.name + "/" + 
				myTransform.position.x + "/" + myTransform.position.y + "/" + myTransform.rotation.x + "/" + myTransform.rotation.y);
			
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.name == "TriggerRocaGigante") 
		{
			createGameObject = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/SueloRoca"));
			Client.instance.SendMessageToServer("DestroyItem/" + this.gameObject.name)
		}
	}
}
