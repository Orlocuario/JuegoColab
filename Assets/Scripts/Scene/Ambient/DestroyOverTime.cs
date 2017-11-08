using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour {

	public float lifeTime;

	// Use this for initialization
	void Start () {

		Destroy (this.gameObject, lifeTime);
		
	}
	
	// Update is called once per frame
	void Update () {


	}
}
