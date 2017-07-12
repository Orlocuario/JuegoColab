using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Vector3 targetPosition;
    public GameObject target;

    private float limitForUpperY = 100;
    public float limitForBottomY = 0;
	public float limitForleftX = -1;
	
	public float smoothCamera;
    public float followAhead;
	public float followUp;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        float y;

        // Keep camera higher than bottom limit
        y = (target.transform.position.y >= limitForBottomY) ? target.transform.position.y : limitForBottomY;

        // Keep camera lower than upper limit
        y = (target.transform.position.y <= limitForUpperY) ? target.transform.position.y : limitForUpperY;

		float x;

		//Mantiene la cámera a la derecha

		x = (target.transform.position.x >= limitForleftX) ? target.transform.position.x : limitForleftX; 

		// Keep camera in the 
        
		targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
       
        // esto es para adelantar la cámara al personaje

        if (target.transform.localScale.x > 0f)
        {
            targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y + followUp, targetPosition.z);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y + followUp, targetPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);

	}
}