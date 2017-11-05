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
        if (IsCollisionLocalPlayer(other))
        {
            Debug.Log(other.name + " is in the npc trigger");
            OnEnter();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (IsCollisionLocalPlayer(other))
        {
            Debug.Log(other.name + " left npc trigger");
            OnExit();
        }
    }

    private bool IsCollisionLocalPlayer(Collider2D collider)
    {
        string tag = collider.gameObject.tag;

        if (tag == "Player")
        {
            PlayerController script = collider.gameObject.GetComponent<PlayerController>();
            if (script.localPlayer == true)
            {
                return true;
            }
        }
        return false;
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
