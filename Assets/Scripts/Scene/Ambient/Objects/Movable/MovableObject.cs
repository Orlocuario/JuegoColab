using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    #region Attributes

    public PlannerObstacle obstacleObj = null;
    protected Rigidbody2D rgbd;

    public string openningTrigger; // The trigger that makes dissapear the object
    public string openedPrefab; // How it looks when its opened

    protected Vector3 lastPosition;
    protected static int updateRate = 5;
    protected int updateFrame;

    #endregion

    #region Start & Update


    // Use this for initialization
    protected virtual void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        updateFrame = 0;
        lastPosition = transform.position;
    }

    protected void Update()
    {

        if (++updateFrame % updateRate == 0)
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
    }

    #endregion

    #region Common

    public virtual void MoveMe(Vector2 force)
    {
        if (rgbd)
        {
            Debug.Log(name + " moved with force " + force);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameObjectIsPunch(collision.gameObject))
        {
            rgbd.velocity = Vector3.zero;
        }
    }

    #endregion

    #region Utils

    protected bool TriggerIsOpener(GameObject trigger)
    {
        return trigger.name == openningTrigger;
    }

    protected bool GameObjectIsPunch(GameObject other)
    {
        return other.GetComponent<PunchController>();
    }

    #endregion

}