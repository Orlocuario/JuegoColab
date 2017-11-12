using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    #region Attributes

    public PlannerObstacle obstacleObj = null;

    public string openningTrigger; // The trigger that makes dissapear the object
    public string openedPrefab; // How it looks when its opened

    protected Dictionary<string, float> animLengths;
    protected Rigidbody2D rgbd;
    protected Animator animator;

    #endregion

    #region Start & Update


    // Use this for initialization
    protected virtual void Start()
    {
        animLengths = new Dictionary<string, float>();
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody2D>();
        LoadAnimationLength();
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

            if (animator)
            {
                animator.SetBool("Moving", true);
                StartCoroutine("Moving");
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
        // Prevents weird collisions with other game objects.
        if (!collision.gameObject || !collision.rigidbody)
        {
            return;
        }

        // Counter the force of every other game object.
        if (!GameObjectIsPunch(collision.gameObject))
        {
            Vector2 counter = -collision.rigidbody.velocity;
            rgbd.AddForce(counter);
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


    protected virtual void LoadAnimationLength()
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        foreach (AnimationClip animationClip in ac.animationClips)
        {
            animLengths.Add(animationClip.name, animationClip.length);
        }

    }

    public IEnumerator Moving()
    {
        float animLength = animLengths["Moving"];

        yield return new WaitForSeconds(animLength);
        animator.SetBool("Moving", false);
    }

    #endregion

}