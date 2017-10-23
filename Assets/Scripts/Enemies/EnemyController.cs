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

        /*while(Client.instance == null)
        {
            //
        }

        while (Client.instance.GetLocalPlayer() == null)
        {
            //
        }
        */

        Debug.Log("Enemy " + enemyId + " initializing...");

        if (Client.instance != null)
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();

            if (localPlayer != null)
            {
                Debug.Log("Enemy " + enemyId + " fue creado desde el editor: " + fromEditor + " y " + localPlayer.name + " tiene el control sobre los enemigos " + localPlayer.controlOverEnemies);

                if (fromEditor && localPlayer.controlOverEnemies)
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

        if (Client.instance != null)
        {
            PlayerController localPlayer = Client.instance.GetLocalPlayer();

            if (localPlayer != null)
            {

                if (localPlayer.controlOverEnemies)
                {
                    Debug.Log( this.gameObject.name + " took " + damage);

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

    protected virtual void SendIdToRegister()
    {
        string message = "NewEnemyId/" + enemyId.ToString();
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
