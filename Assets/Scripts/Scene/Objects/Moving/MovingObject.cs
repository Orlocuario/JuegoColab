using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    #region Attributes

    public Vector2 startPoint;
    public Vector2 endPoint;

    public bool dontCollideWithPlayers;
    public float moveSpeed;

    private Vector3 currentTarget;
    
    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        if (dontCollideWithPlayers)
        {
            IgnoreCollisionWithPlayers();   
        }

        if (endPoint != null)
        {
            currentTarget = endPoint;
        }

    }

    void Update()
    {
        if (currentTarget == null || startPoint == null || endPoint == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, endPoint) <= 0f)
        {
            currentTarget = startPoint;
        }

        if (Vector2.Distance(transform.position, startPoint) <= 0f)
        {
            currentTarget = endPoint;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

    }

    #endregion

    #region Utils

    private void IgnoreCollisionWithPlayers()
    {
        Collider2D collider = GetComponent<Collider2D>();

        GameObject player1 = GameObject.Find("Mage");
        GameObject player2 = GameObject.Find("Warrior");
        GameObject player3 = GameObject.Find("Engineer");
        Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>());
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        return other.GetComponent<PlayerController>();
    }

    #endregion

    #region Events

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            transform.parent = other.transform;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            transform.parent = null;
        }
    }

    #endregion

}
