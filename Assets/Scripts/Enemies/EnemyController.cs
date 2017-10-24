using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    protected LevelManager levelManager;
    protected Animator animator;

    protected float maxHp = 100f;

    protected float posX;
    protected float posY;
    protected float hp;

    public bool fromEditor;
    public int enemyId;

    protected virtual void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        levelManager = FindObjectOfType<LevelManager>();

        hp = maxHp;
    }

    public void SetPosition(float x, float y)
    {
        this.posX = x;
        this.posY = y;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public virtual void TakeDamage(float damage)
    {
        animator.SetBool("isDamaged", true);

        if (Client.instance != null)
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();

            if (localPlayer != null)
            {

                if (localPlayer.controlOverEnemies)
                {
                    Debug.Log(this.gameObject.name + " took " + damage);

                    //hp -= damage;
                    SendHpDataToServer(damage);
                }
            }
        }

    }

    public virtual void Die()
    {
        animator.SetBool("isDead", true);
    }

    public virtual void SendEnemyDataToServer()
    {
        SendHpDataToServer(0f);
        SendPositionToServer();
    }

    public void SendIdToRegister()
    {
        Debug.Log("Sending enemy " + enemyId + " to register");
        string message = "NewEnemyId/" + enemyId.ToString() + "/" + maxHp.ToString();
        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendHpDataToServer(float damage)
    {
        //string message = "EnemyHpChange/" + enemyId.ToString() + "/" + hp.ToString();
        string message = "EnemyHpChange/" + enemyId.ToString() + "/" + damage.ToString();
        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendPositionToServer()
    {
        string message = "EnemyChangePosition/" + enemyId.ToString() + "/" + posX + "/" + posY;
        Client.instance.SendMessageToServer(message);
    }

    public void OnDamageEnd(string s)
    {
        Debug.Log(s);
        animator.SetBool("isDamaged", false);
    }

    public void OnDiedEnd(string s)
    {
        Debug.Log(s);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
