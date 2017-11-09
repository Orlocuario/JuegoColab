using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamera : MonoBehaviour {

	public float ortographic_size;
	public CameraState state;
	public float targetX;
	public float targetY;
    public GameObject target;
	public bool sinChat;

    // Use this for initialization
    void Start()
    {
        if (target != null)
        {
            targetX = target.transform.position.x;
            targetY = target.transform.position.y;          
        }
    }

	private void OnEnter(){
		GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
		camaraScript.ChangeState (state, ortographic_size,targetX,targetY,sinChat);
		if (state == CameraState.TargetZoom) {
			Destroy (this.gameObject);
		}
	}

	private void OnExit(){
		GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
		camaraScript.ChangeState (CameraState.Normal, 0,0,0, false);

	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (GameObjectIsPlayer(other.gameObject)) {
			OnEnter ();
		}
	}

	public void OnTriggerExit2D(Collider2D other){
		if (GameObjectIsPlayer(other.gameObject)) {
			OnExit ();
		}
	}

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
