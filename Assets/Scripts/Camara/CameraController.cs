﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState{Normal,FixedX,FixedY,Zoomed, TargetZoom};

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
	private Camera laCamara; 


	private Vector3 saltitos;
	private float cameraRate;
	private int zoomIt;
	private float initialSize = 1.42f;
	float iteracionesTarget = 200; //Iteraciones hasta llegar al target
	float freezeTime = 202; //Tiempo en que espera volver
	public float startTime;



    // Use this for initialization
    void Start () {
		laCamara = this.gameObject.GetComponent<Camera> ();
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
			if (zoomIt < iteracionesTarget) {
				transform.position = new Vector3(transform.position.x + saltitos.x, transform.position.y + saltitos.y, transform.position.z);
				laCamara.orthographicSize = laCamara.orthographicSize + cameraRate;
				zoomIt++;
			} else if (zoomIt < iteracionesTarget + freezeTime) {
				zoomIt++;
			} else if (zoomIt < iteracionesTarget + freezeTime + iteracionesTarget) {
				transform.position = new Vector3(transform.position.x - saltitos.x, transform.position.y - saltitos.y, transform.position.z);
				laCamara.orthographicSize = laCamara.orthographicSize - cameraRate;
				zoomIt++;
			} else {
				Client.instance.GetLocalPlayer ().ResumeMoving ();
				SetDefaultValues ();
			}

			break;
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
		}
	}

	private void SetZoomedValues(float size, float x, float y){
		currentState = CameraState.Zoomed;
		//float i = (Time.time - startTime);
		laCamara.orthographicSize =  size;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void TargetedZoom(float size, float x, float y){
		Client.instance.GetLocalPlayer ().StopMoving ();
		currentState = CameraState.TargetZoom;
		Vector3 targetPosition = new Vector3 (x, y,0);
		float distance = (targetPosition - transform.position).magnitude;
		saltitos = (targetPosition - transform.position) / iteracionesTarget;
		cameraRate = (size - initialSize)/iteracionesTarget;
		zoomIt = 0;

	}

	private void SetFixedX(){
		currentState = CameraState.FixedX;
	}

	private void SetFixedY(){
		currentState = CameraState.FixedY;
	}

	private void SetDefaultValues ()
	{
		smoothCamera = 2;
		followAhead = 1;
		followUp = 0.8f;
		laCamara.orthographicSize = initialSize;
		currentState = CameraState.Normal;
	}
		
}