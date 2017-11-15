using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCtrigger : MonoBehaviour
{

    #region Attributes

    [System.Serializable]
    public struct NPCFeedback
    {
        public ParticleSystem particles;
        public string message;
    };

    public NPCFeedback[] feedbacks;

    public float feedbackTime;

    private NPCFeedback activeFeedback;
    private LevelManager levelManager;

    private int feedbackCount;

    #endregion

    #region Start 

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        feedbackCount = 0;
    }

    #endregion

    #region Common

    public void ReadNextFeedback()
    {
        // Always shut the last particles
        if (IsThereActiveFeedback() && activeFeedback.particles && activeFeedback.particles.isPlaying)
        {
            activeFeedback.particles.Stop();
        }

        // Exit if every feedback was read
        if (feedbackCount >= feedbacks.Length)
        {
            EndFeedback();
            return;
        }

        activeFeedback = feedbacks[feedbackCount];

        if (IsThereActiveFeedback())
        {
            // Activate particles
            if (activeFeedback.particles)
            {
                activeFeedback.particles.Play();
            }

            // Set feedback text
            if (activeFeedback.message != null)
            {
                levelManager.SetNPCText(activeFeedback.message);
            }
        }

        feedbackCount += 1;
    }

    #endregion

    #region Events

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            ReadNextFeedback();
        }
    }

    #endregion

    #region Utils

    protected void EndFeedback()
    {
        levelManager.ShutNPCFeedback();
        Destroy(this.gameObject);
    }

    protected bool IsThereActiveFeedback()
    {
        return !activeFeedback.Equals(default(NPCFeedback));
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    #endregion

    #region Coroutines

    private IEnumerator WaitToReadNPCMessage()
    {
        yield return new WaitForSeconds(feedbackTime);
        ReadNextFeedback();
    }

    #endregion

}
