using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCtrigger : MonoBehaviour
{

    private LevelManager theLevelManager;
    public string[] messages;
    public float readTime;


    // Use this for initialization
    void Start()
    {
        theLevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Debug.Log(other.name + " is in the npc trigger");
            OnEnter();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Debug.Log(other.name + " left npc trigger");
            OnExit();
        }
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        return playerController && playerController.localPlayer;
    }

    private void OnEnter()
    {
        theLevelManager.ActivateNPCLog(this);
    }

    private void OnExit()
    {
        Destroy(this.gameObject);
    }
}
