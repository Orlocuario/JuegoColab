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
    public int damage;

    #endregion

    #region Start & Update

    protected void Start()
    {
        particles = GetComponent<ParticleSystem>();
        levelManager = FindObjectOfType<LevelManager>();

        if (particles)
        {
            SetActive(activated);
        }
    }

    #endregion

    #region Common

    public virtual void SetActive(bool active)
    {
  
        activated = active;

        if (!particles)
        {
            Debug.Log("This killing object does not have particles");
            return;
        }

        if (active)
        {
            particles.Play();
        }
        else
        {
            particles.Stop();
        }
    }

    protected virtual void Kill(GameObject player)
    {
        if (activated)
        {
            player.GetComponent<PlayerController>().TakeDamage(damage, new Vector2(0, 0));
            levelManager.Respawn();
        }
    }

    #endregion

    #region Events

    // Attack those who enter the alert zone
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Kill(other.gameObject);
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


