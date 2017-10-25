using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    protected LevelManager levelManager;
    protected Animator animator;
    protected Rigidbody2D rb2d;

    protected float maxHp = 100f;

    protected float posX;
    protected float posY;
    protected float hp;

    protected static Vector2 initialPosition;
    protected static float patrollingDistance = .75f;
    private static float maxYSpeed = 0f;
    public static float maxXSpeed = .5f;

    public int directionX = -1;  // 1 = derecha, -1 = izquierda

    protected Vector2 force = new Vector2(0,0);
    protected int damage = 0; 

    protected bool patrolling;
    public bool fromEditor;
    public int enemyId;

    protected virtual void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        rb2d = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;

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

    protected virtual void Patroll()
    {

        Debug.DrawLine(transform.position, initialPosition, Color.green); // See which point the enemy is patrolling

        if (rb2d == null)
        {
            Debug.Log("If your are planning to move an enemy it should have a RigidBody2D");
            return;
        }

        if (Mathf.Abs(Mathf.Abs(initialPosition.x) - Mathf.Abs(transform.position.x)) >= patrollingDistance)
        {
            directionX *= -1; // Turn the other direction and start walking
        }

        if (transform.localScale.x == directionX)
        {
            transform.localScale = new Vector3(-directionX, 1f, 1f);
        }


        float speedX = maxXSpeed * directionX;
        float speedY = maxYSpeed;

        rb2d.velocity = new Vector2(speedX, speedY);

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

    public void StartPatrolling()
    {
        patrolling = true;
    }

    protected bool CollidedWithPlayer(GameObject other)
    {
        return other.tag == "Player1" || other.tag == "Player2" || other.tag == "Player3"; // Refactor this to tag == "Player"
    }

    protected void DealDamage(GameObject player)
    {

        Vector2 playerPosition = player.transform.position;
        PlayerController playerController = player.GetComponent<PlayerController>();

        Vector2 attackForce = force;

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }

        playerController.TakeDamage(damage, attackForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollidedWithPlayer(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }
    }
}
