using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private float hp;
    private float posX;
    private float posY;
    public int enemyId;
	// Use this for initialization
	protected void Start () {
		
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

    protected virtual void Die()
    {
        //RIP
    }

    public virtual void SendEnemyDataToServer()
    {
        SendHpDataToServer();
        SendPositionToServer();
    }

    protected void RequestEnemyId()
    {

    }

    public virtual void SendHpDataToServer()
    {
        string message = "EnemyHpChange/" + enemyId.ToString() + "/" +hp.ToString();
        Client.instance.SendMessageToServer(message);
    }

    public virtual void SendPositionToServer()
    {
        string message = "EnemyHpChange/" + enemyId.ToString()+"/" + posX + "/" + posY;
        Client.instance.SendMessageToServer(message);

    }
}
