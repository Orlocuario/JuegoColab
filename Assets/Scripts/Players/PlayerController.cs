using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    protected Vector3 lastPosition;
    public PlannerPlayer playerObj;
    protected Transform transform;
    protected Rigidbody2D rb2d;
    public Collider2D collider;
    public GameObject parent;

    public Vector3 respawnPosition;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Animator animator;

    private LevelManager levelManager;
    private GlobalDisplayHUD hpAndMp;
    private SpriteRenderer sprite;

    public static float maxAcceleration = 1; //100% del speed
    public static float maxXSpeed = 3.5f;
    private static float maxYSpeed = 8f;

    public float acceleration = 0f;
    public bool canAccelerate = false; //Limita la aceleración a la mitad de los frames

    public float groundCheckRadius;
    public float actualSpeed;

    //Used to synchronize data from the server
    public bool remoteAttacking;
    public bool remoteJumping;
    public bool remoteRight;
    public bool remoteLeft;
    public bool remoteUp;

    public bool rightPressed;
    public bool leftPressed;
    public bool jumpPressed;
    public bool localPlayer;
    public bool isGrounded;
    public bool upPressed;

    public static string mpSpendRate = "-1"; // Cuanto mp se gasta cada vez
    public static int mpUpdateRate = 30; // Cada cuantos frames se actualiza el HP y MP display

    public bool controlOverEnemies;
    public int mpUpdateFrame;
    public bool gravity = true; // true = normal, false = invertida
    public int directionY = 1; // 1 = de pie, -1 = de cabeza
    public int directionX = 1;  // 1 = derecha, -1 = izquierda
    public int sortingOrder = 0;
    public int characterId;
    public bool isPowerOn;
    public bool mpDepleted;


    protected static int attackSpeed = 4;
    protected string currentAttackName;
    protected bool isAttacking;
    private bool conectado;
    private bool canMove;
    protected float speedX;
    protected float speedY;

    private int debuger;

    protected Dictionary<String, float> attackAnimLength;
    // protected string[] attackAnimNames;

    protected virtual void Start()
    {
        hpAndMp = GameObject.Find("Canvas").GetComponent<GlobalDisplayHUD>();

        attackAnimLength = new Dictionary<String, float>();
        levelManager = FindObjectOfType<LevelManager>();
        collider = GetComponent<Collider2D>();
        transform = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        respawnPosition = transform.position;

        controlOverEnemies = false;
        isAttacking = false;
        localPlayer = false;
        mpDepleted = false;
        isPowerOn = false;
        conectado = true;
        canMove = true;

        mpUpdateFrame = 0;
        debuger = 0;

        SetGravity(gravity);

        IgnoreCollisionBetweenPlayers();
        LoadAnimationLength();
    }

    protected virtual void LoadAnimationLength()
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        foreach (AnimationClip animationClip in ac.animationClips)
        {
            attackAnimLength.Add(animationClip.name, animationClip.length);
        }

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

    public void Conectar(bool valor)
    {
        conectado = valor;
    }

    public void IgnoreCollisionBetweenPlayers()
    {
        GameObject player1 = Client.instance.GetPlayerController(0).gameObject;
        GameObject player2 = Client.instance.GetPlayerController(1).gameObject;
        GameObject player3 = Client.instance.GetPlayerController(2).gameObject;
        Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>());
    }

    public void Activate(int charId)
    {
        localPlayer = true;
        this.characterId = charId;
        sprite = GetComponent<SpriteRenderer>();

        if (this.characterId == 0)
        {
            Chat.instance.EnterFunction("Mage: Ha Aparecido!");
        }
        if (this.characterId == 1)
        {
            Chat.instance.EnterFunction("Warrior: Ha Aparecido!");
        }
        else if (this.characterId == 2)
        {
            Chat.instance.EnterFunction("Engineer: Ha Aparecido!");
        }

        if (sprite)
        {
            sprite.sortingOrder = sortingOrder + 1;
        }

    }

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
        groundCheckRadius = collider.bounds.extents.x * .9f;
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
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsGrounded", true);
        animator.SetBool("IsAttacking", false);
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
            animator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
            animator.SetBool("IsGrounded", isGrounded);
        }

        rb2d.velocity = new Vector2(speedX, speedY);
        lastPosition = transform.position;

    }

    public bool UpdatePowerState()
    {

        if (localPlayer)
        {

            bool powerButtonPressed = CnInputManager.GetButtonDown("Power Button");
            float mpCurrentPercentage = hpAndMp.mpCurrentPercentage;

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
                    SetParticlesAnimationState(isPowerOn);
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
                    SetParticlesAnimationState(isPowerOn);
                }

                if (isPowerOn)
                {

                    if (mpUpdateFrame == mpUpdateRate)
                    {
                        SendMPDataToServer();
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

        if (force.x != 0 || force.y != 0)
        {
            rb2d.AddForce(force);

            string message = "PlayerTookDamage/" + characterId + "/" + force.x + "/" + force.y;
            Client.instance.SendMessageToServer(message);
        }

        if (damage != 0)
        {

            // Always send negative values tu HPHUD
            if (damage > 0)
            {
                damage *= -1;
            }

            string message = "ChangeHpHUDToRoom/" + damage;
            Client.instance.SendMessageToServer(message);
        }

    }

    protected virtual void SetParticlesAnimationState(bool activo)
    {

    }

    protected void OnTriggerEnter2D(Collider2D other)
    {

        if (!localPlayer)
        {
            return;
        }

        if (other.tag == "KillPlane")
        {
            string daño = other.gameObject.GetComponent<KillPlane>().killPlaneDamage;
            Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + daño);
            levelManager.Respawn();
        }

        if (other.tag == "KillPlaneSpike")
        {
            string daño = other.gameObject.GetComponent<KillPlane>().killPlaneDamage;
            Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + daño);
            levelManager.Respawn();
        }

        if (other.tag == "Checkpoint")
        {
            respawnPosition = other.transform.position;
        }
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Poi")
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

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            Debug.Log(other.gameObject.name);
            transform.parent = other.transform;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    public void SetPowerState(bool power)
    {
        SetParticlesAnimationState(power);
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

        animator.SetFloat("Speed", Mathf.Abs(speedX));
        animator.SetBool("IsGrounded", isGrounded);

        transform.position = new Vector3(positionX, positionY, transform.position.z);
        transform.localScale = new Vector3(directionX, directionY, 1f);
    }

    public virtual void SetAttack()
    {
        throw new NotImplementedException("Implement the remote set attack method in each player");
    }

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

        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendAttackDataToServer()
    {
        string message = "PlayerAttack/" + characterId;
        Client.instance.SendMessageToServer(message);
    }

    protected void SendPowerDataToServer()
    {
        string message = "PlayerPower/" + characterId + "/" + isPowerOn;
        Client.instance.SendMessageToServer(message);
    }

    public void SendMPDataToServer()
    {
        Client.instance.SendMessageToServer("ChangeMpHUDToRoom/" + mpSpendRate);
    }

    public virtual IEnumerator Attacking()
    {
        float animLength = attackAnimLength[currentAttackName];

        yield return new WaitForSeconds(animLength);

        isAttacking = false;

        animator.SetFloat("Speed", Mathf.Abs(speedX));
        animator.SetBool("IsAttacking", isAttacking);
    }

}
