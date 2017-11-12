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

    public ParticleSystem particles;
    public bool activated;
    public string damage;

    #endregion

    #region Start & Update

    protected void Start()
    {
        particles = GetComponent<ParticleSystem>();
        levelManager = FindObjectOfType<LevelManager>();
        SetActive(activated);
    }

    #endregion

    #region Common

    public virtual void SetActive(bool active)
    {
        activated = active;

        if (active)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
    }

    protected virtual void Kill()
    {
        if (activated)
        {
            levelManager.Respawn();

            if (Client.instance)
            {
                Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + damage);
            }
        }
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
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    #endregion

}


