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
        enemyId = Random.Range(0, 100);

        hp = maxHp;

        if (Client.instance != null)
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();

            if (localPlayer != null)
            {

                //if (fromEditor && localPlayer.controlOverEnemies)
                if (localPlayer.controlOverEnemies)
                {
                    OnEditorStart();
                }
            }

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
    protected virtual void Update()
    {

    }

    public virtual void TakeDamage(float damage)
    {
        animator.SetBool("isDamaged", true);

        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        animator.SetBool("isDead", true);
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
        string message = "EnemyHpChange/" + enemyId.ToString() + "/" + hp.ToString();
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
