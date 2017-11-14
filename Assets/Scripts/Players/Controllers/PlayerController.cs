using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public PlannerPlayer playerObj;
    public Collider2D collider;
    public GameObject parent;

    public Vector3 respawnPosition;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Animator animator;

    private LevelManager levelManager;
    private HUDDisplay hpAndMp;
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

    protected SceneAnimator animControl;
    protected Vector3 lastPosition;
    protected Rigidbody2D rb2d;

    protected static int attackSpeed = 4;
    protected bool isAttacking;
    protected bool conectado;
    protected bool canMove;
    protected float speedX;
    protected float speedY;
	public static float attackRate = .25f;

    private int debuger;

    protected virtual void Start()
    {
        animControl = GameObject.FindObjectOfType<SceneAnimator>();
        hpAndMp = GameObject.Find("Canvas").GetComponent<HUDDisplay>();

        levelManager = FindObjectOfType<LevelManager>();
        collider = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        respawnPosition = transform.position;

        controlOverEnemies = false;
        isAttacking = false;
        localPlayer = false;
        isGrounded = false;
        mpDepleted = false;
        isPowerOn = false;
        conectado = true;
        canMove = true;

        remoteAttacking = false;
        remoteJumping = false;
        remoteRight = false;
        remoteLeft = false;
        remoteUp = false;

        mpUpdateFrame = 0;
        debuger = 0;

        SetGravity(gravity);

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
        isAttacking = false;
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsGrounded", true);
        animator.SetBool("Attacking", false);
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

        StartCoroutine(animControl.StartAnimation("TakingDamage", this.gameObject));

    }

    protected virtual void SetParticlesAnimationState(bool activo)
    {

    }

    protected bool GameObjectIsPOI(GameObject other)
    {
        return other.GetComponent<PlannerPoi>();
    }

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

	public IEnumerator WaitAttacking ()
	{
		yield return new WaitForSeconds (attackRate);
		isAttacking = false;
	}

}
