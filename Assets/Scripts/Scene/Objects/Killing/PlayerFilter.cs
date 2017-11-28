using UnityEngine;

/**
 * When a player touches this objects it loses health and respwans,
 * allows for some players to pass
 */
public class PlayerFilter : KillingObject
{
    #region Attributes

    public PlayerController[] playersCanPass;

    #endregion

    #region Events

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            if (!GameObjectCanPass(other.gameObject))
            {
                Kill(other.gameObject);
            }
        }
    }

    #endregion

    #region Utils

    protected bool GameObjectCanPass(GameObject other)
    {
        bool passes = false;

        foreach (PlayerController canPass in playersCanPass)
        {
            if (other.GetComponent<PlayerController>() == canPass)
            {
                passes = true;
                break;
            }
        }

        return passes;
    }

    #endregion

}


