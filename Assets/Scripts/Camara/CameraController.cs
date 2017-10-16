using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState{Normal,FixedX,FixedY,Zoomed, TargetZoom, NoFollowUp, NoFollowAhead};

public class CameraController : MonoBehaviour {

	private Vector3 targetPosition;
	public GameObject target;

	private float limitForUpperY = 100;
	public float limitForBottomY = 0;
	public float limitForleftX = -1;

	public float smoothCamera;
	public float followAhead;
	public float followUp;
	public CameraState currentState;
	private Camera holiwix; 


	private Vector3 saltitos;
	private float cameraRate;
	private int zoomIt;
	private float initialSize = 1.42f;
	float wea = 100; //Iteraciones hasta llegar al target
	float wea2 = 150; //Tiempo en que espera volver
	public float startTime;



	// Use this for initialization
	void Start () {
		holiwix = this.gameObject.GetComponent<Camera> ();
		ChangeState (CameraState.Normal, 10, 0, 0);
		Client.instance.GetLocalPlayer ().SetGravity (false);
	}

	// Update is called once per frame
	void Update() {

		/*float y;

        // Keep camera higher than bottom limit
        y = (target.transform.position.y >= limitForBottomY) ? target.transform.position.y : limitForBottomY;

        // Keep camera lower than upper limit
        y = (target.transform.position.y <= limitForUpperY) ? target.transform.position.y : limitForUpperY;

		float x;

		//Mantiene la cámera a la derecha

		x = (target.transform.position.x >= limitForleftX) ? target.transform.position.x : limitForleftX; 

		// Keep camera in the */

		targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

		// esto es para adelantar la cámara al personaje


		switch (currentState) {
		case CameraState.Normal:
			if (target.transform.localScale.x > 0f)
			{
				targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y + followUp, targetPosition.z);
			}
			else
			{
				targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y + followUp, targetPosition.z);
			}
			transform.position = Vector3.Lerp (transform.position, targetPosition, smoothCamera * Time.deltaTime);
			break;
		case CameraState.Zoomed:
			break;
		case CameraState.FixedX:
			transform.position = new Vector3(transform.position.x,targetPosition.y,targetPosition.z);
			break;
		case CameraState.FixedY:
			transform.position = new Vector3 (targetPosition.x, transform.position.y, targetPosition.z);
			break;
		case CameraState.TargetZoom:
			if (zoomIt < wea) {
				transform.position = new Vector3(transform.position.x + saltitos.x, transform.position.y + saltitos.y, transform.position.z);
				holiwix.orthographicSize = holiwix.orthographicSize + cameraRate;
				zoomIt++;
			} else if (zoomIt < wea + wea2) {
				zoomIt++;
			} else if (zoomIt < wea + wea2 + wea) {
				transform.position = new Vector3(transform.position.x - saltitos.x, transform.position.y - saltitos.y, transform.position.z);
				holiwix.orthographicSize = holiwix.orthographicSize - cameraRate;
				zoomIt++;
			} else {
				Client.instance.GetLocalPlayer ().ResumeMoving ();
				SetDefaultValues ();
			}

			break;
		case CameraState.NoFollowUp:
			{
				if (target.transform.localScale.x > 0f)
				{
					targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y, targetPosition.z);
				}
				else
				{
					targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y, targetPosition.z);
				}
				transform.position = Vector3.Lerp (transform.position, targetPosition, smoothCamera * Time.deltaTime);
				break;
			}
			break;
		case CameraState.NoFollowAhead:
			{
				if (target.transform.localScale.x > 0f) 
				{
					targetPosition = new Vector3 (targetPosition.x, targetPosition.y + followUp, targetPosition.z);
				} else 
				{
					targetPosition = new Vector3 (targetPosition.x, targetPosition.y + followUp, targetPosition.z);
				}
				transform.position = Vector3.Lerp (transform.position, targetPosition, smoothCamera * Time.deltaTime);
				break;
			}
		}
	}


	public void ChangeState(CameraState state, float ortographicsize, float x, float y){
		switch (state) {
		case CameraState.Normal:
			SetDefaultValues();
			break;
		case CameraState.Zoomed:
			SetZoomedValues (ortographicsize,x,y);
			break;
		case CameraState.FixedX:
			SetFixedX ();
			break;
		case CameraState.FixedY:
			SetFixedY ();
			break;
		case CameraState.TargetZoom:
			TargetedZoom (ortographicsize, x, y);
			break;
		case CameraState.NoFollowAhead:
			SetNoFollowAhead ();
			break;
		case CameraState.NoFollowUp:
			SetNofollowUp ();
			break;
		}
	}

	private void SetZoomedValues(float size, float x, float y){
		currentState = CameraState.Zoomed;
		//float i = (Time.time - startTime);
		holiwix.orthographicSize =  size;
		transform.position = new Vector3(x, y, transform.position.z);
	}

	private void TargetedZoom(float size, float x, float y){
		Client.instance.GetLocalPlayer ().StopMoving ();
		currentState = CameraState.TargetZoom;
		Vector3 targetPosition = new Vector3 (x, y,0);
		float distance = (targetPosition - transform.position).magnitude;
		saltitos = (targetPosition - transform.position) / wea;
		cameraRate = (size - initialSize)/wea;
		zoomIt = 0;

	}
	private void SetNofollowUp(){
		currentState = CameraState.NoFollowUp;
	}

	private void SetNoFollowAhead(){
		currentState = CameraState.NoFollowAhead;
		followAhead = 0.5f;
	}

	private void SetFixedX(){
		currentState = CameraState.FixedX;
	}

	private void SetFixedY(){
		currentState = CameraState.FixedY;
	}

	private void SetDefaultValues ()
	{
		smoothCamera = 3.9f;
		followAhead = 1f;
		followUp = 0.7f;
		holiwix.orthographicSize = initialSize;
		currentState = CameraState.Normal;
	}

}