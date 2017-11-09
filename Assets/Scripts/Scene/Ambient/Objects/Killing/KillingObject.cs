using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * When a player touches this objects it loses health and respwans.
 */
public class KillingObject : MonoBehaviour
{
    #region Attributes

    protected LevelManager levelManager;
    public string damage;

    #endregion

    #region Start & Update

    protected void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    #endregion

    #region Common

    protected virtual void Kill()
    {
        Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + damage);
        levelManager.Respawn();
    }

    #endregion

    #region Events

    // Attack those who enter the alert zone
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Kill();
        }
    }

    #endregion

    #region Utils

    protected bool GameObjectIsPlayer(GameObject other)
    {

        if (other.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.localPlayer)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

}


