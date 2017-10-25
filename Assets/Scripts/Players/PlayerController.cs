using CnControls;
using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    protected Vector3 previousPosition;
    public PlannerPlayer playerObj;
    protected Transform transform;
    protected Rigidbody2D rb2d;
    public Collider2D collider;
    public GameObject parent;

    public Transform groundCheck;
    public Animator myAnim;
    public Vector3 respawnPosition;
    public LayerMask whatIsGround;

    private LevelManager theLevelManager;
    private SpriteRenderer sprite;

    public static float maxAcceleration = 1; //100% del speed
    private static float maxYSpeed = 8f;
    public static float maxXSpeed = 3.5f;
    public float acceleration = 0f;
    public bool canAccelerate = false; //Limita la aceleración a la mitad de los frames

    public float groundCheckRadius;
    public float actualSpeed;
    public float speed; //For animation nonlocal purposes

    public bool isGrounded;
    public bool leftPressed;
    public bool rightPressed;
    public bool upPressed;
    public bool jumpPressed;
    public bool localPlayer;

    //Used to synchronize data from the server
    public bool remoteAttacking;
    public bool remoteJumping;
    public bool remoteRight;
    public bool remotePower;
    public bool remoteLeft;
    public bool remoteUp;


    public static string mpSpendRate = "-1"; // Cuanto mp se gasta cada vez
    public static int mpUpdateRate = 30; // Cada cuantos frames se actualiza el HP y MP display
    public int mpUpdateFrame;
    public bool gravity = true; // true = normal, false = invertida
    private int directionY = 1; // 1 = de pie, -1 = de cabeza
    public int directionX = 1;  // 1 = derecha, -1 = izquierda
    public bool controlOverEnemies;
    public int sortingOrder = 0;
    public int saltarDoble;
    public int characterId;
    bool conectado = true;
    public bool isPowerOn;
    public bool mpDepleted;
    private bool canMove;

    DisplayHUD hpAndMp;

    protected virtual void Start()
    {
        remoteAttacking = false;
        remoteJumping = false;
        remotePower = false;
        remoteRight = false;
        remoteLeft = false;
        canMove = true;

        mpUpdateFrame = 0;
        mpDepleted = false;

        hpAndMp = GameObject.Find("Canvas").GetComponent<DisplayHUD>();

        theLevelManager = FindObjectOfType<LevelManager>();
        collider = GetComponent<Collider2D>();
        transform = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();

        isPowerOn = false;
        respawnPosition = transform.position;

        controlOverEnemies = false;
        localPlayer = false;
        saltarDoble = 0;

        this.SetGravity(gravity);

        IgnoreCollisionBetweenPlayers();
        SendObjectDataToServer();
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
            float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
            float axisVertical = CnInputManager.GetAxisRaw("Vertical");

            bool up = axisVertical > 0f;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if ((!remoteRight && axisHorizontal > 0f))
            {
                if ((up && axisHorizontal >= axisVertical) || (!up && axisHorizontal >= -axisVertical))
                {
                    remoteRight = true;
                    remoteLeft = false;
                    SendObjectDataToServer();
                }
            }

            // si el wn esta apuntando hacia arriba/abajo con mayor inclinacion que hacia la derecha, stop moving
            else if (Mathf.Abs(axisVertical) > Mathf.Abs(axisHorizontal) && (remoteRight || remoteLeft))
            {
                remoteRight = false;
                remoteLeft = false;
                SendObjectDataToServer();
            }

            // si no se esta apretando el joystick
            else if (remoteRight && axisHorizontal == 0)
            {
                remoteRight = false;
                SendObjectDataToServer();
            }

        }

        return remoteRight;
    }

    protected bool IsGoingLeft()
    {
        if (localPlayer)
        {
            float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
            float axisVertical = CnInputManager.GetAxisRaw("Vertical");

            bool up = axisVertical > 0f;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la izquierda, start moving
            if ((!remoteLeft && axisHorizontal < 0f))
            {
                if ((up && -axisHorizontal >= axisVertical) || (!up && axisHorizontal <= axisVertical))
                {
                    remoteLeft = true;
                    remoteRight = false;
                    SendObjectDataToServer();
                }
            }


            // si no se esta apretando el joystick
            else if (remoteLeft && axisHorizontal == 0)
            {
                remoteLeft = false;
                SendObjectDataToServer();
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

    protected void DrawGroundCheckerRadius()
    {
        Vector3 right = new Vector3(groundCheckRadius, 0, 0);
        Vector3 left = new Vector3(-groundCheckRadius, 0, 0);
        Vector3 up = new Vector3(0, groundCheckRadius, 0);
        Vector3 down = new Vector3(0, -groundCheckRadius, 0);

        Debug.DrawLine(groundCheck.position, groundCheck.position + right, Color.green);
        Debug.DrawLine(groundCheck.position, groundCheck.position + left, Color.green);
        Debug.DrawLine(groundCheck.position, groundCheck.position + up, Color.green);
        Debug.DrawLine(groundCheck.position, groundCheck.position + down, Color.green);
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
                SendObjectDataToServer();
            }

            else if (!isJumping && remoteJumping)
            {
                remoteJumping = false;
                SendObjectDataToServer();
            }

            return isJumping;
        }

        return remoteJumping;
    }

    protected virtual bool IsAttacking()
    {
        return false;
    }

    protected bool CheckIfSomethingChanged()
    {
        Vector3 newPosition = transform.position;

        if (previousPosition.x != newPosition.x)
        {
            return true;
        }

        if (previousPosition.y != newPosition.y)
        {
            return true;
        }

        return false;
    }

    protected void SynchronizeNonLocalPlayer()
    {
        if (!localPlayer)
        {

            transform.localScale = new Vector3(directionX * 1f, directionY, 1f);
            SetAnimVariables();
        }

    }

    public virtual void StopMoving()
    {
        canMove = false;
        myAnim.SetFloat("Speed", 0);
        myAnim.SetBool("IsGrounded", true);
        myAnim.SetBool("IsAttacking", false);
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

    protected virtual void Update()
    {
        if (!conectado)
        {
            return;
        }

        if (!canMove)
        {
            return;
        }

        if (transform.parent != null)
        {
            parent = transform.parent.gameObject;
        }

        isGrounded = IsItGrounded();

        float speedY = rb2d.velocity.y;
        float speedX;

        if (IsGoingRight())
        {
            // Si estaba yendo a la izquierda resetea la aceleración
            if (directionX == -1)
            {
                transform.localScale = new Vector3(1f, directionY, 1f);
                acceleration = .1f;
                directionX = 1;
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
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

            actualSpeed = maxXSpeed * acceleration;
            speedX = actualSpeed;
        }

        else if (IsGoingLeft())
        {

            // Si estaba yendo a la derecha resetea la aceleración
            if (directionX == 1)
            {
                transform.localScale = new Vector3(-1f, directionY, 1f);
                acceleration = .1f;
                directionX = -1;
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
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

            actualSpeed = maxXSpeed * acceleration;
            speedX = -actualSpeed;
        }

        else
        {
            speedX = 0f;
            acceleration = 0;
        }

        if (IsJumping(isGrounded))
        {
            speedY = maxYSpeed * directionY;
        }

        rb2d.velocity = new Vector2(speedX, speedY);

        previousPosition = transform.position;
        SetAnimVariables();
        UpdatePowerState();
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
                    remotePower = false;
                    isPowerOn = false;
                    mpDepleted = true;

                    SendPowerDataToServer();
                    SetAnimacion(remotePower);
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
                    remotePower = isPowerOn;

                    SendPowerDataToServer();
                    SetAnimacion(remotePower);
                }

                if (isPowerOn)
                {

                    if (mpUpdateFrame == mpUpdateRate)
                    {
                        SpendMP();
                        mpUpdateFrame = 0;
                    }

                    mpUpdateFrame++;

                }

            }

        }
        return remotePower;
    }

    protected virtual void SetAnimacion(bool activo)
    {

    }

    protected virtual void SetAnimVariables()
    {
        myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        myAnim.SetBool("IsGrounded", isGrounded);
        myAnim.SetBool("IsAttacking", IsAttacking());

    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "KillPlane")
        {
            if (localPlayer)
            {
                string daño = other.gameObject.GetComponent<KillPlane>().killPlaneDamage;
                Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + daño);
                theLevelManager.Respawn();
            }
        }
        if (other.tag == "KillPlaneSpike")
        {
            if (localPlayer)
            {
                string daño = other.gameObject.GetComponent<KillPlane>().killPlaneDamage;
                Client.instance.SendMessageToServer("ChangeHpHUDToRoom/" + daño);
                theLevelManager.Respawn();
            }
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
            Debug.Log(other.transform.position);
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

    public void SetVariablesFromServer(float positionX, float positionY, bool isGrounded, float speed, int direction, bool remoteRight, bool remoteLeft, bool remoteJumping)
    {
        if (localPlayer)
        {
            return;
        }

        transform.position = new Vector3(positionX, positionY);

        this.remoteJumping = remoteJumping;
        this.remoteRight = remoteRight;
        this.remoteLeft = remoteLeft;
        this.isGrounded = isGrounded;
        this.directionX = direction;
        this.speed = speed;

        SynchronizeNonLocalPlayer();
    }

    public void SendObjectDataToServer()
    {
        if (!localPlayer)
        {
            return;
        }

        float positionX = transform.position.x;
        float positionY = transform.position.y;
        float speed = Mathf.Abs(rb2d.velocity.x);
        string message = "ChangePosition/" + characterId + "/" + positionX + "/" + positionY + "/" + isGrounded + "/" + speed + "/" + directionX + "/" + remoteJumping + "/" + remoteLeft + "/" + remoteRight;
        Client.instance.SendMessageToServer(message);
    }

    protected virtual void SendAttackDataToServer()
    {
        string message = "Attack/" + characterId + "/" + remoteAttacking;
        Client.instance.SendMessageToServer(message);
    }

    protected void SendPowerDataToServer()
    {
        string message = "Power/" + characterId + "/" + remotePower;
        Client.instance.SendMessageToServer(message);
    }

    public virtual void RemoteSetter(bool power)
    {

    }

    public void SpendMP()
    {
        Client.instance.SendMessageToServer("ChangeMpHUDToRoom/" + mpSpendRate);
    }
}
