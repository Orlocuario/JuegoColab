using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamera : MonoBehaviour {

	public float ortographic_size;
	public CameraState state;
	public float targetX;
	public float targetY;

	// Use this for initialization
	void Start () {
		
	}

	private void OnEnter(){
		GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
		camaraScript.ChangeState (state, ortographic_size,targetX,targetY);
		if (state == CameraState.TargetZoom) {
			Destroy (this.gameObject);
		}
	}

	private void OnExit(){
		GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
		camaraScript.ChangeState (CameraState.Normal, 0,0,0);

	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (IsCollisionLocalPlayer (other)) {
			OnEnter ();
		}
	}

	public void OnTriggerExit2D(Collider2D other){
		if (IsCollisionLocalPlayer (other)) {
			OnExit ();
		}
	}

	private bool IsCollisionLocalPlayer(Collider2D collider)
	{
		string tag = collider.gameObject.tag;

		if(tag == "Player1" || tag == "Player2" || tag == "Player3"){
			PlayerController script = collider.gameObject.GetComponent<PlayerController>();
			if (script.localPlayer == true) {
				return true;
			}
		}
		return false;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
