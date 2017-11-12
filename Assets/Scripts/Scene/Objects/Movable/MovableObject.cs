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

    #endregion

    #region Start & Update


    // Use this for initialization
    protected virtual void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Common

    public virtual void MoveMe(Vector2 force)
    {
        if (rgbd)
        {
            Debug.Log(name + " moved with force " + force);
            rgbd.AddForce(force);

            if (Client.instance)
            {

                Client.instance.SendMessageToServer("ObjectMoved/" +
                        name + "/" +
                        force.x + "/" +
                        force.y);
            }
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

        if (!collision.gameObject || !collision.rigidbody)
        {
            return;
        }

        if (!GameObjectIsPunch(collision.gameObject))
        {
            Vector2 counter = -collision.rigidbody.velocity;
            rgbd.AddForce(counter);
        }
        else
        {
            Debug.Log("Punch move me");
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