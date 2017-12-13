using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCamera : MonoBehaviour
{

    #region Attributes

    public CameraState state;
    public GameObject target;

    public float ortographic_size;
    public bool hideChat;
    public bool playerCantMove;

    #endregion

    #region Common 

    private void OnEnter()
    {

        GameObject camera = GameObject.Find("MainCamera"); 
        CameraController cameraController = camera.GetComponent<CameraController>();

        cameraController.ChangeState(state, ortographic_size, target.transform.position.x, target.transform.position.y, hideChat, playerCantMove);
        if (state == CameraState.TargetZoom)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnExit()
    {

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera"); // TODO: Change this to obj name
        CameraController cameraController = camera.GetComponent<CameraController>();

        cameraController.ChangeState(CameraState.Normal, 0, 0, 0, false, false);

    }
    
    #endregion

    #region Events

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            OnEnter();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            OnExit();
        }
    }

    #endregion

    #region Utils

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    #endregion
}
