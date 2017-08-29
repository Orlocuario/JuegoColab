using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SujetaRoca : MonoBehaviour {


	public Animator sujetaRocaAnim;
	public bool isSwitch = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (isSwitch==true)
		{
			sujetaRocaAnim.SetBool ("isSwitch", isSwitch);	
		}
	}
}