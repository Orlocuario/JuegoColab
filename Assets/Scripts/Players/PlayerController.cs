using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected Rigidbody2D rb2d;
    protected Vector3 previous_transform;
    protected Transform transform;

    public Transform groundCheck;
    public Animator myAnim;
    public Vector3 respawnPosition;
    public LayerMask whatIsGround;

    private SpriteRenderer sprite;
    private LevelManager theLevelManager;

    public float moveSpeed;
    public float jumpSpeed;
    public float speed; //For animation nonlocal purposes
    public float groundCheckRadius;

    public bool isGrounded;
    public bool leftPressed;
    public bool rightPressed;
    public bool upPressed;
    public bool jumpPressed;
    public bool localPlayer;
    public bool remoteRight; //Used to synchronize data from the server
    public bool remoteLeft;
    public bool remoteUp;
    public bool remoteJumping;
    public bool remoteAttacking;
    public bool remotePower;
    public bool controlOverEnemies;
    public int SortingOrder = 0;
    public int saltarDoble;
    public int direction;  //1 = derecha, -1 = izquierda
    public int characterId;
    
    protected virtual void Start()
    {
        remoteRight = false;
        remoteLeft = false;
        remoteJumping = false;
        remoteAttacking = false;
		remotePower = false;
        transform = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        respawnPosition = transform.position;
        theLevelManager = FindObjectOfType<LevelManager>();
        localPlayer = false;
        direction = 1;
        controlOverEnemies = false;
        saltarDoble = 0;
        IgnoreCollisionStar2puntoCero();
    }

    public void IgnoreCollisionStar2puntoCero()
    {
        GameObject player1 = Client.instance.GetPlayerController(0).gameObject;
        GameObject player2 = Client.instance.GetPlayerController(1).gameObject;
        GameObject player3 = Client.instance.GetPlayerController(2).gameObject;
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player1.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player2.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player3.GetComponent<Collider2D>());
    }


    public void Activate(int charId)
    {
        localPlayer = true;
        this.characterId = charId;
		sprite = GetComponent<SpriteRenderer>();
        if (this.characterId == 0)
        {
            Chat.instance.EnterFunction("Mage: Has Connected");
        }
        if (this.characterId == 1)
        {
            Chat.instance.EnterFunction("Warrior: Has Connected");
        }
        else if (this.characterId == 2)
        {
            Chat.instance.EnterFunction("Engineer: Has Connected");
        }

        if (sprite) 
		{
			sprite.sortingOrder = SortingOrder + 1;
		}
    }

    protected bool IsGoingRight()
    {
        if (localPlayer)
        {
            bool up;
            float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
            float axisVertical = CnInputManager.GetAxisRaw("Vertical");

            if (axisVertical > 0f)
            {
                up = true;
            }
            else
            {
                up = false;
            }
            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if ((!remoteRight && axisHorizontal > 0f) && ((up && axisHorizontal >= axisVertical) || (!up && axisHorizontal >= -axisVertical)))
            {
                remoteRight = true;
                remoteLeft = false;
                SendObjectDataToServer();
            }
            // si el wn esta apuntando hacia arriba/abajo con mayor inclinacion que hacia la derecha, stop moving
            else if ((up && axisHorizontal < axisVertical) || (!up && axisHorizontal < -axisVertical))
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
            return remoteRight;
        }
        return remoteRight;
    }

    protected bool IsGoingLeft()
    {
        if (localPlayer)
        {
            bool up;
            float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
            float axisVertical = CnInputManager.GetAxisRaw("Vertical");

            if (axisVertical > 0f)
            {
                up = true;
            }
            else
            {
                up = false;
            }
            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la izquierda, start moving
            if ((!remoteLeft && axisHorizontal < 0f) && ((up && -axisHorizontal >= axisVertical) || (!up && axisHorizontal <= axisVertical)))
            {
                remoteLeft = true;
                remoteRight = false;
                SendObjectDataToServer();
            }
            // si el wn esta apuntando hacia arriba/abajo con mayor inclinacion que hacia la izquierda, stop moving
            else if ((up && -axisHorizontal < axisVertical)  || (!up && axisHorizontal > axisVertical))
            {
                remoteLeft = false;
                remoteRight = false;
                SendObjectDataToServer();
            }
            // si no se esta apretando el joystick
            else if (remoteLeft && axisHorizontal == 0)
            {
                remoteLeft = false;
                SendObjectDataToServer();
            }
            return remoteLeft;
        }
        return remoteLeft;
    }

    public bool IsGoingUp()
    {
        if (localPlayer)
        {
            bool right;
            float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
            float axisVertical = CnInputManager.GetAxisRaw("Vertical");

            if (axisHorizontal > 0f)
            {
                right = true;
            }
            else
            {
                right = false;
            }
            // si el wn esta apuntando hacia izq/der con menor inclinacion que hacia arriba, return true
            if ((!remoteUp && axisVertical > 0f) && ((right && axisHorizontal <= axisVertical) || (!right && -axisHorizontal <= axisVertical)))
            {
                remoteUp = true;
                SendObjectDataToServer();
            }
            // si el wn esta apuntando hacia izq/der con mayor inclinacion que hacia arriba, o bien, esta apuntando hacia abajo, return false
            else if ((!remoteUp && axisVertical > 0f) && ((right && axisHorizontal > axisVertical) || (!right && -axisHorizontal > axisVertical)) || axisVertical <= 0f)
            {
                remoteUp = false;
                SendObjectDataToServer();
            }
            // si no se esta apretando el joystick
            else if (remoteUp && axisVertical <= 0.5)
            {
                remoteUp = false;
                SendObjectDataToServer();
            }
            return remoteUp;
        }
        return remoteUp;
    }

    protected bool IsItGrounded()
    {

        //bool ground = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        //return ground;
        float verticalSpeed = rb2d.velocity.y;
        return verticalSpeed == 0;
    }

    protected virtual bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            bool pressedJump = CnInputManager.GetButtonDown("Jump Button");
            bool saltando = pressedJump && isGrounded;
            if(saltando && !remoteJumping)
            {
                remoteJumping = true;
                SendObjectDataToServer();
            }
            else if(!saltando && remoteJumping)
            {
                remoteJumping = false;
                SendObjectDataToServer();
            }
            return saltando;
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
        if(previous_transform.x != newPosition.x)
        {
            return true;
        }
        if(previous_transform.y != newPosition.y)
        {
            return true;
        }
        return false;
    }

    protected void SynchronizeNonLocalPlayer()
    {
        if (!localPlayer)
        {
            if(direction == 1)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if(direction == -1)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            SetAnimVariables();
        }
			
    }

    protected int updateFrames = 0;

    protected virtual void Update()
    {
        if (IsGoingRight())
        {
            rb2d.velocity = new Vector3(moveSpeed, rb2d.velocity.y, 0f);
            transform.localScale = new Vector3(1f, 1f, 1f);
            direction = 1;
        }
        else if (IsGoingLeft())
        {
            rb2d.velocity = new Vector3(-moveSpeed, rb2d.velocity.y, 0f);
            transform.localScale = new Vector3(-1f, 1f, 1f);
            direction = -1;
        }
        else // it's not moving
        {
            rb2d.velocity = new Vector3(0f, rb2d.velocity.y, 0f);
        }

        isGrounded = IsItGrounded();
        if (IsJumping(isGrounded))
        {
            rb2d.velocity = new Vector2(0, 8f);
        }
        previous_transform = transform.position;
        SetAnimVariables();
		isPower ();
    }

	protected virtual bool isPower()
	{
		return false;
	}

    protected virtual void SetAnimVariables()
    {
        myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        myAnim.SetBool("IsGrounded", isGrounded);
        myAnim.SetBool("IsAttacking", IsAttacking());

    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "KillPlane")
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

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "MovingPlatform")
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
        this.isGrounded = isGrounded;
        this.speed = speed;
        this.direction = direction;
        this.remoteRight = remoteRight;
        this.remoteLeft = remoteLeft;
        this.remoteJumping = remoteJumping;
        SynchronizeNonLocalPlayer();
    }

    public void SendObjectDataToServer()
    {
        float position_x = transform.position.x;
        float position_y = transform.position.y;
        bool grounded = isGrounded;
        float speed = Mathf.Abs(rb2d.velocity.x);
        string message = "ChangePosition/" + characterId + "/" + position_x + "/" + position_y + "/" + isGrounded + "/" + speed + "/" + direction + "/" + remoteJumping + "/" + remoteLeft + "/" + remoteRight;
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
}