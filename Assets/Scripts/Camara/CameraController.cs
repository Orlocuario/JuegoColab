using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Normal,
    FixedX,
    FixedY,
    Zoomed,
    TargetZoom,
    NoFollowUp,
    NoFollowAhead
};

public class CameraController : MonoBehaviour
{

    public CameraState currentState;
    private Vector3 targetPosition;
    private GameObject target;
    private Vector3 saltitos;
    private Camera holiwix;

    private float limitForUpperY = 100;
    public float limitForBottomY = 0;
    public float limitForleftX = -1;

    public float smoothCamera;
    public float followAhead;
    public float followUp;

    private static float stepsToTarget = 100; //Iteraciones hasta llegar al target
    private static float initialSize = 1.50f;
    private static float freezeTime = 150; //Tiempo en que espera volver

    private float cameraRate;
    public float startTime;
    private int zoomIt;

    // Use this for initialization
    void Start()
    {
        holiwix = this.gameObject.GetComponent<Camera>();
        ChangeState(CameraState.Normal, 10, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        /*float y;

        // Keep camera higher than bottom limit
        y = (target.transform.position.y >= limitForBottomY) ? target.transform.position.y : limitForBottomY;

        // Keep camera lower than upper limit
        y = (target.transform.position.y <= limitForUpperY) ? target.transform.position.y : limitForUpperY;

		float x;

		//Mantiene la cámera a la derecha

		x = (target.transform.position.x >= limitForleftX) ? target.transform.position.x : limitForleftX;  */

        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

        // esto es para adelantar la cámara al personaje

        switch (currentState)
        {

            case CameraState.Normal:
                if (target.transform.localScale.x > 0f)
                {
                    targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y + followUp, targetPosition.z);
                }
                else
                {
                    targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y + followUp, targetPosition.z);
                }
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
                break;

            case CameraState.Zoomed:
                break;

            case CameraState.FixedX:
                transform.position = new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
                break;

            case CameraState.FixedY:
                transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
                break;

            case CameraState.TargetZoom:
                if (zoomIt < stepsToTarget)
                {
                    transform.position = new Vector3(transform.position.x + saltitos.x, transform.position.y + saltitos.y, transform.position.z);
                    holiwix.orthographicSize = holiwix.orthographicSize + cameraRate;
                    zoomIt++;
                }
                else if (zoomIt < stepsToTarget + freezeTime)
                {
                    zoomIt++;
                }
                else if (zoomIt < stepsToTarget + freezeTime + stepsToTarget)
                {
                    transform.position = new Vector3(transform.position.x - saltitos.x, transform.position.y - saltitos.y, transform.position.z);
                    holiwix.orthographicSize = holiwix.orthographicSize - cameraRate;
                    zoomIt++;
                }
                else
                {
                    Client.instance.GetLocalPlayer().ResumeMoving();
                    SetDefaultValues();
                }
                break;

            case CameraState.NoFollowUp:
                if (target.transform.localScale.x > 0f)
                {
                    targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y, targetPosition.z);
                }
                else
                {
                    targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y, targetPosition.z);
                }
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
                break;

            case CameraState.NoFollowAhead:
                if (target.transform.localScale.x > 0f)
                {
                    targetPosition = new Vector3(targetPosition.x, targetPosition.y + followUp, targetPosition.z);
                }
                else
                {
                    targetPosition = new Vector3(targetPosition.x, targetPosition.y + followUp, targetPosition.z);
                }
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
                break;

        }
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void ChangeState(CameraState state, float ortographicsize, float x, float y)
    {
        switch (state)
        {
            case CameraState.Normal:
                SetDefaultValues();
                break;
            case CameraState.Zoomed:
                SetZoomedValues(ortographicsize, x, y);
                break;
            case CameraState.FixedX:
                SetFixedX();
                break;
            case CameraState.FixedY:
                SetFixedY();
                break;
            case CameraState.TargetZoom:
                TargetedZoom(ortographicsize, x, y);
                break;
            case CameraState.NoFollowAhead:
                SetNoFollowAhead();
                break;
            case CameraState.NoFollowUp:
                SetNofollowUp();
                break;
        }
    }

    private void SetZoomedValues(float size, float x, float y)
    {
        currentState = CameraState.Zoomed;
        holiwix.orthographicSize = size;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void TargetedZoom(float size, float x, float y)
    {
        Client.instance.GetLocalPlayer().StopMoving();
        currentState = CameraState.TargetZoom;
        Vector3 targetPosition = new Vector3(x, y, 0);
        float distance = (targetPosition - transform.position).magnitude;
        saltitos = (targetPosition - transform.position) / stepsToTarget;
        cameraRate = (size - initialSize) / stepsToTarget;
        zoomIt = 0;

    }
    private void SetNofollowUp()
    {
        currentState = CameraState.NoFollowUp;
    }

    private void SetNoFollowAhead()
    {
        currentState = CameraState.NoFollowAhead;
        followAhead = .5f;
    }

    private void SetFixedX()
    {
        currentState = CameraState.FixedX;
    }

    private void SetFixedY()
    {
        currentState = CameraState.FixedY;
    }

    private void SetDefaultValues()
    {
        smoothCamera = 3.9f;
        followAhead = .9f;
        followUp = .3f;
        holiwix.orthographicSize = initialSize;
        currentState = CameraState.Normal;
    }

}