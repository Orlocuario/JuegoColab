using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    #region Attributes

    public PlannerPlayer playerObj;
    public Vector3 respawnPosition;
    public LayerMask whatIsGround;
    public GameObject[] particles;
    public Transform groundCheck;
    public GameObject parent;

    // Remote data
    public bool remoteAttacking;
    public bool remoteJumping;
    public bool remoteRight;
    public bool remoteLeft;
    public bool remoteUp;

    // Local data
    public bool rightPressed;
    public bool leftPressed;
    public bool jumpPressed;
    public bool localPlayer;
    public bool isGrounded;
    public bool upPressed;

    public bool controlOverEnemies;
    public float groundCheckRadius;
    public bool canAccelerate;
    public float acceleration;
    public float actualSpeed;
    public int mpUpdateFrame;
    public int sortingOrder;
    public int characterId;
    public bool mpDepleted;
    public bool isPowerOn;
    public int directionY; // 1 = de pie, -1 = de cabeza
    public int directionX;  // 1 = derecha, -1 = izquierda
    public bool gravity; // true = normal, false = invertida

    protected SceneAnimator sceneAnimator;
    protected LevelManager levelManager;
    protected SpriteRenderer sprite;
    protected Vector3 lastPosition;
    protected Rigidbody2D rb2d;

    // Statics
    protected static float maxAcceleration = 1;
    protected static float takeDamageRate = 1f;
    protected static float attackRate = .25f;
    protected static float maxXSpeed = 3.5f;
    protected static float maxYSpeed = 8f;

    protected static int mpUpdateFrameRate = 30;
    protected static int mpSpendRate = -1;
    protected static int attackSpeed = 4;

    protected string currentAttack;
    protected bool isTakingDamage;
    protected bool isAttacking;
    protected bool conectado;
    protected bool canMove;
    protected float speedX;
    protected float speedY;

    protected int debuger;

    #endregion

    #region Start & Update

    protected virtual void Start()
    {

        sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();

        if (!sceneAnimator)
        {
            Debug.Log(name + " did not found the SceneAnimator");
        }

        levelManager = FindObjectOfType<LevelManager>();
        rb2d = GetComponent<Rigidbody2D>();

        respawnPosition = transform.position;

        controlOverEnemies = false;
        canAccelerate = false;
        isAttacking = false;
        localPlayer = false;
        isGrounded = false;
        mpDepleted = false;
        isPowerOn = false;
        conectado = true;
        canMove = true;
        gravity = true;

        remoteAttacking = false;
        remoteJumping = false;
        remoteRight = false;
        remoteLeft = false;
        remoteUp = false;

        mpUpdateFrame = 0;
        acceleration = 0f;
        sortingOrder = 0;
        directionY = 1;
        directionX = 1;
        debuger = 0;

        SetGravity(gravity);
        InitializeParticles();
        IgnoreCollisionBetweenPlayers();
    }

    protected virtual void Update()
    {
        if (!conectado || !canMove)
        {
            return;
        }

        if (transform.parent != null)
        {
            parent = transform.parent.gameObject;
        }

        Move();
        Attack();
        UpdatePowerState();
    }

    #endregion

    #region Common

    public void Conectar(bool valor)
    {
        conectado = valor;
    }

    public void Activate(int charId)
    {
        localPlayer = true;
        this.characterId = charId;
        sprite = GetComponent<SpriteRenderer>();

        if (sprite)
        {
            sprite.sortingOrder = sortingOrder + 1;
        }

        if (Chat.instance)
        {
            Chat.instance.EnterFunction(name + ": Ha Aparecido!");
        }

    }

    protected void Attack()
    {

        if (!localPlayer || isAttacking)
        {
            return;
        }

        bool attackButtonPressed = CnInputManager.GetButtonDown("Attack Button");

        if (attackButtonPressed)
        {
            CastAttack();
        }

    }

    protected virtual void CastAttack()
    {
        CastLocalAttack();
        SendAttackDataToServer();
    }

    public virtual void CastLocalAttack()
    {
        throw new NotImplementedException("Every player must implement a public CastLocalAttack method");
    }

    public virtual void StopMoving()
    {
        canMove = false;
        isAttacking = false;

        if (sceneAnimator)
        {
            sceneAnimator.SetFloat("Speed", 0, this.gameObject);
            sceneAnimator.SetBool("IsGrounded", true, this.gameObject);
            sceneAnimator.SetBool("Attacking", false, this.gameObject);
        }
    }

    public virtual void ResumeMoving()
    {
        canMove = true;
    }

    public void SetGravity(bool normal)
    {

        rb2d.gravityScale = 2.5f;

        if (!normal)
        {
            directionY = -1;
            rb2d.gravityScale = -2.5f;
        }

    }

    protected void ResetDirectionX(int newDirectionX)
    {
        transform.localScale = new Vector3(newDirectionX, directionY, 1f);
        directionX = newDirectionX;
        acceleration = .1f;
    }

    protected void Accelerate()
    {
        if (canAccelerate)
        {
            acceleration += .1f;
            canAccelerate = false;
        }

        else
        {
            canAccelerate = true;
        }
    }

    protected void Move()
    {

        isGrounded = IsItGrounded();

        if (IsJumping(isGrounded))
        {
            speedY = maxYSpeed * directionY;
        }
        else
        {
            speedY = rb2d.velocity.y;
        }

        if (IsGoingRight())
        {
            // Si estaba yendo a la izquierda resetea la aceleración
            if (directionX == -1)
            {
                ResetDirectionX(1);
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
            {
                Accelerate();
            }

            actualSpeed = maxXSpeed * acceleration;
            speedX = actualSpeed;
        }

        else if (IsGoingLeft())
        {

            // Si estaba yendo a la derecha resetea la aceleración
            if (directionX == 1)
            {
                ResetDirectionX(-1);
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
            {
                Accelerate();
            }

            actualSpeed = maxXSpeed * acceleration;
            speedX = -actualSpeed;
        }

        else
        {
            speedX = 0f;
            acceleration = 0;
        }

        if (lastPosition != transform.position)
        {
            if (sceneAnimator)
            {
                sceneAnimator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x), this.gameObject);
                sceneAnimator.SetBool("IsGrounded", isGrounded, this.gameObject);
            }
        }

        rb2d.velocity = new Vector2(speedX, speedY);
        lastPosition = transform.position;

    }

    public bool UpdatePowerState()
    {

        if (localPlayer)
        {

            if (!levelManager.hpAndMp)
            {
                Debug.Log("Levelmanager HpAndMp is not set");
                return false;
            }

            bool powerButtonPressed = CnInputManager.GetButtonDown("Power Button");
            float mpCurrentPercentage = levelManager.hpAndMp.mpCurrentPercentage;

            // Se acabó el maná
            if (mpCurrentPercentage <= 0f)
            {
                // Si no he avisado que se acabó el maná, aviso
                if (!mpDepleted)
                {
                    mpUpdateFrame = 0;
                    isPowerOn = false;
                    mpDepleted = true;

                    SendPowerDataToServer();
                    ActivateParticles(isPowerOn);
                }
            }

            // Hay maná
            else
            {
                // Reseteo la variable para avisar que el maná se acabó
                if (mpDepleted)
                {
                    mpDepleted = false;
                }

                // Toogle power button
                if (powerButtonPressed)
                {
                    isPowerOn = !isPowerOn;

                    SendPowerDataToServer();
                    ActivateParticles(isPowerOn);
                }

                if (isPowerOn)
                {

                    if (mpUpdateFrame == mpUpdateFrameRate)
                    {
                        levelManager.hpAndMp.ChangeMP(mpSpendRate); // Change local
                        SendMPDataToServer(); // Change remote
                        mpUpdateFrame = 0;
                    }

                    mpUpdateFrame++;

                }

            }
        }

        return isPowerOn;
    }

    public void TakeDamage(int damage, Vector2 force)
    {
        if (isTakingDamage)
        {
            return;
        }

        isTakingDamage = true;

        if (force.x != 0 || force.y != 0)
        {
            rb2d.AddForce(force); // Take force local
            SendMessageToServer("PlayerTookDamage/" + characterId + "/" + force.x + "/" + force.y); // Take force remote
        }

        if (damage != 0)
        {

            // Always send negative values tu HPHUD
            if (damage > 0)
            {
                damage *= -1;
            }

            levelManager.hpAndMp.ChangeHP(damage); // Change local HP
            SendMessageToServer("ChangeHpHUDToRoom/" + damage); // Change remote HP

        }

        StartCoroutine(WaitTakingDamage());
        AnimateTakingDamage();

    }

    #endregion

    #region Utils

    // All related with debugging processes
    #region Debugging

    protected void DebugDrawDistance(float distance)
    {
        DebugDrawDistance(distance, Color.blue);
    }

    protected void DebugDrawDistance(float distance, Color color)
    {
        Vector2 left = new Vector3(transform.position.x - distance, transform.position.y, transform.position.z);
        Vector2 up = new Vector3(transform.position.x, transform.position.y + distance, transform.position.z);
        Vector2 right = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);
        Vector2 down = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);

        Debug.DrawLine(transform.position, left, color);
        Debug.DrawLine(transform.position, up, color);
        Debug.DrawLine(transform.position, right, color);
        Debug.DrawLine(transform.position, down, color);
    }

    #endregion
    
    // Set variables in their default state
    #region Initializers

    public void IgnoreCollisionBetweenPlayers()
    {
        Collider2D collider = GetComponent<Collider2D>();

        GameObject player1 = GameObject.Find("Mage");
        GameObject player2 = GameObject.Find("Warrior");
        GameObject player3 = GameObject.Find("Engineer");
        Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>());
    }

    #endregion

    // Validate for player conditions
    #region Validations


    protected bool IsGoingRight()
    {
        if (localPlayer)
        {

            bool buttonRightPressed = CnInputManager.GetAxisRaw("Horizontal") == 1;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if (buttonRightPressed && !remoteRight)
            {
                remoteRight = true;
                remoteLeft = false;
                SendPlayerDataToServer();
            }

            // si no se esta apretando el joystick
            else if (!buttonRightPressed && remoteRight)
            {
                remoteRight = false;
                SendPlayerDataToServer();
            }

        }

        return remoteRight;

    }

    protected bool IsGoingLeft()
    {
        if (localPlayer)
        {

            bool buttonLeftPressed = CnInputManager.GetAxisRaw("Horizontal") == -1f;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if (buttonLeftPressed && !remoteLeft)
            {
                remoteLeft = true;
                remoteRight = false;
                SendPlayerDataToServer();
            }

            // si no se esta apretando el joystick
            else if (!buttonLeftPressed && remoteLeft)
            {
                remoteLeft = false;
                SendPlayerDataToServer();
            }

        }

        return remoteLeft;
    }

    public bool IsGoingUp()
    {
        return false;
    }

    protected bool IsItGrounded()
    {
        // El radio del groundChecker debe ser menor a la medida del collider del player/2 para que no haga contactos laterales.
        groundCheckRadius = GetComponent<Collider2D>().bounds.extents.x * .9f;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    protected virtual bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            bool pressedJump = CnInputManager.GetButtonDown("Jump Button");
            bool isJumping = pressedJump && isGrounded;

            if (isJumping && !remoteJumping)
            {
                remoteJumping = true;
                SendPlayerDataToServer();
            }

            else if (!isJumping && remoteJumping)
            {
                remoteJumping = false;
                SendPlayerDataToServer();
            }

        }

        return remoteJumping;

    }

    protected bool GameObjectIsPOI(GameObject other)
    {
        return other.GetComponent<PlannerPoi>();
    }

    #endregion

    // Set player data from other classes
    #region Remote Setters

    public void SetPowerState(bool power)
    {
        ActivateParticles(power);
        isPowerOn = power;
    }

    public void SetDamageFromServer(Vector2 force)
    {
        rb2d.AddForce(force);
    }

    public void SetPlayerDataFromServer(float positionX, float positionY, int directionX, int directionY, float speedX, bool isGrounded, bool remoteJumping, bool remoteLeft, bool remoteRight)
    {

        this.remoteJumping = remoteJumping;
        this.remoteRight = remoteRight;
        this.remoteLeft = remoteLeft;
        this.isGrounded = isGrounded;
        this.directionX = directionX;
        this.directionY = directionY;
        this.speedX = speedX;

        if (sceneAnimator)
        {
            sceneAnimator.SetFloat("Speed", Mathf.Abs(speedX), this.gameObject);
            sceneAnimator.SetBool("IsGrounded", isGrounded, this.gameObject);
        }

        transform.position = new Vector3(positionX, positionY, transform.position.z);
        transform.localScale = new Vector3(directionX, directionY, 1f);
    }

    public virtual void SetAttack()
    {
        CastLocalAttack();
    }

    #endregion

    // Manage particles
    #region Particles

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = GetComponentsInChildren<ParticleSystem>();

        if (_particles == null || _particles.Length == 0)
        {
            return;
        }

        particles = new GameObject[_particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = _particles[i].gameObject;
        }

        ActivateParticles(false);

    }

    protected virtual void ActivateParticles(bool active)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(active);
            }
        }
    }

    #endregion

    // Manage animations
    #region Animations

    protected void AnimateAttack()
    {
        if (sceneAnimator && currentAttack != null)
        {
            StartCoroutine(sceneAnimator.StartAnimation(currentAttack, this.gameObject));
        }
    }

    protected void AnimateTakingDamage()
    {
        if (sceneAnimator)
        {
            StartCoroutine(sceneAnimator.StartAnimation("TakingDamage", this.gameObject));
        }
    }
    
    #endregion

    #endregion

    #region Events

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (GameObjectIsPOI(other.gameObject))
        {
            PlannerPoi newPoi = other.GetComponent<PlannerPoi>();
            if (!playerObj.playerAt.name.Equals(newPoi.name))
            {
                Debug.Log("Change OK: " + newPoi.name);
                playerObj.playerAt = newPoi;
                playerObj.luring = false;
                if (newPoi.araña != null && this.characterId == 0)
                {
                    playerObj.luring = true;
                    newPoi.araña.blocked = false;
                    newPoi.araña.open = true;
                }
                Planner planner = FindObjectOfType<Planner>();
                planner.Monitor();
            }
        }
    }

    #endregion

    #region Messaging

    public void SendPlayerDataToServer()
    {
        if (!localPlayer)
        {
            return;
        }

        string message = "PlayerChangePosition/" +
            characterId + "/" +
            transform.position.x + "/" +
            transform.position.y + "/" +
            directionX + "/" +
            directionY + "/" +
            Mathf.Abs(rb2d.velocity.x) + "/" +
            isGrounded + "/" +
            remoteJumping + "/" +
            remoteLeft + "/" +
            remoteRight;

        SendMessageToServer(message);
    }

    protected virtual void SendAttackDataToServer()
    {
        string message = "PlayerAttack/" + characterId;
        SendMessageToServer(message);
    }

    protected void SendPowerDataToServer()
    {
        string message = "PlayerPower/" + characterId + "/" + isPowerOn;
        SendMessageToServer(message);
    }

    public void SendMPDataToServer()
    {
        SendMessageToServer("ChangeMpHUDToRoom/" + mpSpendRate);
    }

    protected void SendMessageToServer(string message)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, false);
        }
    }

    #endregion

    #region Coroutines

    public IEnumerator WaitAttacking()
    {
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public IEnumerator WaitTakingDamage()
    {
        yield return new WaitForSeconds(takeDamageRate);
        isTakingDamage = false;
    }

    #endregion

}
