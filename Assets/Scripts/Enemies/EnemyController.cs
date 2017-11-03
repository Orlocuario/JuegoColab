using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Vector2[] patrollingPoints;
    public CircleCollider2D alertZone;

    public float patrollingSpeed;
    public bool patrolling;
    public bool fromEditor;
    public int directionX;  // 1 = derecha, -1 = izquierda

    protected Vector2 force = new Vector2(0, 0);
    protected PlayerController localPlayer;
    protected Vector2 currentPatrolPoint;
    protected LevelManager levelManager;
    protected GameObject attackTarget;
    protected Animator animator;
    protected Rigidbody2D rb2d;

    protected int currentPatrolPointCount;
    protected float maxHp = 100f;
    protected int debuger = 0;
    protected int damage = 0;
    protected int enemyId;
    protected float hp;

    protected static float alertDistanceFactor = 1.5f;
    protected static float maxYSpeed = 0f;
    protected static float maxXSpeed = .5f;

    protected virtual void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        levelManager = FindObjectOfType<LevelManager>();
        rb2d = GetComponent<Rigidbody2D>();

        currentPatrolPointCount = 0;
        patrollingSpeed = 0.005f;
        directionX = -1;
        hp = maxHp;

        if (levelManager != null)
        {
            localPlayer = levelManager.GetLocalPlayerController();
        }

        if (patrollingPoints != null && patrollingPoints.Length > 0)
        {
            NextPatrollingPoint();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        foreach (Vector2 patrollingPoint in patrollingPoints)
        {
            DebugDrawDistance(patrollingPoint);
        }
    }

    protected void Attack(GameObject player)
    {
        // Set the attacking property of the animator
        if (!animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", true);
            attackTarget = player;
        }
    }

    protected void DebugDrawDistance(Vector2 point)
    {
        Debug.DrawLine(transform.position, point, Color.green);
    }

    protected virtual void Patroll()
    {

        if (rb2d == null)
        {
            Debug.Log("If your are planning to move an enemy it should have a RigidBody2D");
            return;
        }

        if ((localPlayer != null && localPlayer.controlOverEnemies) || fromEditor)
        {
            if (Vector2.Distance(transform.position, currentPatrolPoint) < .1f)
            {
                NextPatrollingPoint();
            }

        }

        transform.position = Vector3.MoveTowards(transform.position, currentPatrolPoint, patrollingSpeed);
    }

    protected void TurnAroundIfNeccessary()
    {
        bool turnAround = false;

        if (currentPatrolPoint.x < transform.position.x)
        {
            if (directionX == 1)
            {
                turnAround = true;
            }

        }

        else if (currentPatrolPoint.x > transform.position.x)
        {
            if (directionX == -1)
            {
                turnAround = true;
            }

        }

        if (turnAround)
        {
            directionX *= -1;
            transform.localScale = new Vector3(-directionX, transform.localScale.y, transform.localScale.z);
        }

    }

    protected void NextPatrollingPoint()
    {

        if (patrollingPoints == null || patrollingPoints.Length == 0)
        {
            Debug.Log(name + " : " + enemyId + " has no patrolling points.");
            return;
        }

        currentPatrolPoint = patrollingPoints[currentPatrolPointCount];
        currentPatrolPointCount = (currentPatrolPointCount + 1) % patrollingPoints.Length;

        TurnAroundIfNeccessary();
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
                    Debug.Log(name + " took " + damage);
                    SendHpDataToServer(damage);
                }
            }
        }
    }

    public virtual void Die()
    {
        animator.SetBool("isDead", true);
    }

    public void SetPosition(int directionX, float positionX, float positionY)
    {
        this.directionX = directionX;
        transform.position = new Vector3(positionX, positionY, transform.position.z);
    }

    public void Register(int enemyId)
    {
        this.enemyId = enemyId;

        string message = "NewEnemyId/" +
            GetInstanceID() + "/" +
            enemyId + "/" +
            maxHp + "/" +
            directionX + "/" +
            transform.position.x + "/" +
            transform.position.y;

        SendMessageToServer(message);
    }

    protected virtual void SendHpDataToServer(float damage)
    {
        string message = "EnemyHpChange/" + enemyId + "/" + damage;
        SendMessageToServer(message);
    }

    protected virtual void SendPositionToServer()
    {
        string message = "EnemyChangePosition/" +
            enemyId + "/" +
            directionX + "/" +
            transform.position.x + "/" +
            transform.position.y;

        SendMessageToServer(message);
    }

    protected virtual void SendMessageToServer(string message)
    {
        Client.instance.SendMessageToServer(message);
    }

    public void StartPatrolling()
    {
        patrolling = true;
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {

        if (other.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.localPlayer)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void DealDamage(GameObject player)
    {

        PlayerController playerController = player.GetComponent<PlayerController>();
        MageController mage = Client.instance.GetMage();

        Vector2 playerPosition = player.transform.position;
        Vector2 attackForce = force;

        // Only hit local players
        if (!playerController.localPlayer)
        {
            return;
        }

        // Don't hit protected players
        if (mage.ProtectedByShield(player))
        {
            return;
        }

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }

        playerController.TakeDamage(damage, attackForce);

    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Attack(other.gameObject);
        }
    }


    // Attack those who enter the alert zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Debug.Log(other.gameObject.name + "  entered in " + gameObject.name + " alert zone");
            Attack(other.gameObject);
        }
    }

    // Attack those who collide with me
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            Debug.Log(other.gameObject.name + "  collided with " + gameObject.name);
            Attack(other.gameObject);
        }
    }

    public int GetEnemyId()
    {
        return enemyId;
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

    // This is called from the animator
    public virtual void OnAttackStarted(string s)
    {
        DealDamage(attackTarget);
        attackTarget = null;
    }

    public virtual void OnAttackEnd(string s)
    {
        animator.SetBool("isAttacking", false);
    }

    public void Initialize(int enemyId, int directionX, float posX, float posY)
    {
        this.enemyId = enemyId;
        SetPosition(directionX, posX, posY);
    }

}
