using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private float hp;
    private float posX;
    private float posY;
    public int enemyId;
    public bool fromEditor;
    private LevelManager levelManager;

    protected void Start () {
        levelManager = FindObjectOfType<LevelManager>();
        PlayerController localPlayer = Client.instance.GetLocalPlayer();
        if (fromEditor && localPlayer.controlOverEnemies)
        {
            OnEditorStart();
        }
	}

    public void SetPosition(float x, float y)
    {
        this.posX = x;
        this.posY = y;
    }

    //Se llama si el objecto fue creado desde el editor (en vez de spawn desde el server)
    private void OnEditorStart()
    {
        SendIdToRegister();     
    }

    // Update is called once per frame
    protected void Update () {
		
	}

    public virtual void DealDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }

    public virtual void SendEnemyDataToServer()
    {
        SendHpDataToServer();
        SendPositionToServer();
    }

    protected virtual void SendIdToRegister()
    {
        string message = "NewEnemyId/" + enemyId.ToString();
        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendHpDataToServer()
    {
        string message = "EnemyHpChange/" + enemyId.ToString() + "/" +hp.ToString();
        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendPositionToServer()
    {
        string message = "EnemyChangePosition/" + enemyId.ToString()+"/" + posX + "/" + posY;
        Client.instance.SendMessageToServer(message);
    }
}
