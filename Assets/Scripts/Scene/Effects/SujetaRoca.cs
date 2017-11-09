using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SujetaRoca : MonoBehaviour {


	public Animator sujetaRocaAnim;
	public bool isSwitch;


	// Use this for initialization
	void Start () {

		isSwitch = false;
		sujetaRocaAnim = this.gameObject.GetComponent <Animator> ();
		
	}
	
	// Update is called once per frame
	void Update () {

		if (isSwitch==true)
		{
			sujetaRocaAnim.SetBool ("isSwitch", true);	
		}
	}
}