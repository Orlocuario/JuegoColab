using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    #region Attributes

    public PlannerObstacle obstacleObj = null;

    public string openningTrigger; // The trigger that makes dissapear the object
    public string openedPrefab; // How it looks when its opened

    protected Vector3 lastPosition;

    #endregion

    #region Start & Update


    // Use this for initialization
    protected virtual void Start()
    {
        lastPosition = transform.position;
    }

    protected void Update()
    {

        if (transform.position != lastPosition)
        {
            Client.instance.SendMessageToServer("ChangeObjectPosition/" +
                name + "/" +
                transform.position.x + "/" +
                transform.position.y + "/" +
                transform.position.z);
        }

        lastPosition = transform.position;
    }

    #endregion

    #region Common

    public virtual void MoveMe(Vector2 force)
    {
        Rigidbody2D rgbd = GetComponent<Rigidbody2D>();

        if (rgbd)
        {
            rgbd.AddForce(force);
        }
    }

    protected void TransitionToOpened(GameObject trigger)
    {
        if (obstacleObj != null)
        {
            obstacleObj.blocked = false;
            obstacleObj.open = true;
        }

        if (openedPrefab != null)
        {
            Client.instance.SendMessageToServer("InstantiateObject/Prefabs/" + openedPrefab);
            Client.instance.SendMessageToServer("DestroyObject/" + name);
        }
    }

    #endregion

    #region Events

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger)
        {
            if (TriggerIsOpener(other.gameObject))
            {
                TransitionToOpened(other.gameObject);
            }

        }
    }

    #endregion

    #region Utils

    protected bool TriggerIsOpener(GameObject trigger)
    {
        return trigger.name == openningTrigger;
    }

    #endregion

}