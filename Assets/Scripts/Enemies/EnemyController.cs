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
    protected static float alertDistanceFactor = 1.5f;
    private static float maxYSpeed = 0f;
    public static float maxXSpeed = .5f;

    public int directionX = -1;  // 1 = derecha, -1 = izquierda

    protected Vector2 force = new Vector2(0, 0);
    protected int damage = 0;

    protected float alertDistance;
    protected bool patrolling;
    protected bool attacking;

    public bool fromEditor;
    public int enemyId;

    protected virtual void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        rb2d = GetComponent<Rigidbody2D>();

        Collider2D collider = GetComponent<Collider2D>();

        initialPosition = transform.position;
        alertDistance = collider.bounds.size.x * alertDistanceFactor;

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
        Attack();
    }

    protected void Attack()
    {
        DrawAlertDistance(); //  Only works on editor mode

        foreach (GameObject player in levelManager.players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < alertDistance)
            {
                DealDamage(player);
                return;
            }
        }

    }

    protected void DrawAlertDistance()
    {
        Vector3 left = new Vector3(transform.position.x - alertDistance, transform.position.y, transform.position.z);
        Vector3 right = new Vector3(transform.position.x + alertDistance, transform.position.y, transform.position.z);

        Debug.DrawLine(left, transform.position, Color.blue);
        Debug.DrawLine(transform.position, right, Color.red);
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

        if (!animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", true);
        }

        if (!attacking)
        {
            return;
        }


        Vector2 playerPosition = player.transform.position;
        PlayerController playerController = player.GetComponent<PlayerController>();

        Vector2 attackForce = force;

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }

        playerController.TakeDamage(damage, attackForce);

        attacking = false;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollidedWithPlayer(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }
    }

    public void OnAttackEnd(string s)
    {
        animator.SetBool("isAttacking", false);
    }

    public void OnDamageEnd(string s)
    {
        animator.SetBool("isDamaged", false);
    }

    public void OnDiedEnd(string s)
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    public void OnAttackStarted(string s)
    {
        attacking = true;
    }


}
