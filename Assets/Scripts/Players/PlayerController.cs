using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected Vector3 previousTransform;
    public PlannerPlayer playerObj;
    protected Transform transform;
    protected Rigidbody2D rb2d;
	public GameObject parent;

    public Transform groundCheck;
    public Animator myAnim;
    public Vector3 respawnPosition;
    public LayerMask whatIsGround;

    private LevelManager theLevelManager;
    private SpriteRenderer sprite;

    private static float jumpSpeed = 8;
    public float moveSpeed;
    public float maxSpeed;
    public float speed; //For animation nonlocal purposes
    public float groundCheckRadius;

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

	private static int directionY = 1; //1 = de pie, -1 = de cabeza
    public bool controlOverEnemies;
    public int sortingOrder = 0;
    public int saltarDoble;
    public int characterId;
    public int direction;  //1 = derecha, -1 = izquierda
	private bool canMove;

    protected virtual void Start()
    {
        remoteAttacking = false;
        remoteJumping = false;
		remotePower = false;
        remoteRight = false;
        remoteLeft = false;
		canMove = true;

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

   //     if (localPlayer)
   //     {
   //         bool right;
   //         float axisHorizontal = CnInputManager.GetAxisRaw("Horizontal");
   //         float axisVertical = CnInputManager.GetAxisRaw("Vertical");

   //         if (axisHorizontal > 0f)
   //         {
   //             right = true;
   //         }
   //         else
   //         {
   //             right = false;
   //         }
   //         // si el wn esta apuntando hacia izq/der con menor inclinacion que hacia arriba, return true
			//if ((!remoteUp && axisVertical > 0f) && ((right && axisHorizontal <= axisVertical) || ( !right && -axisHorizontal <= axisVertical)))
   //         {
   //             remoteUp = true;
   //             SendObjectDataToServer();
   //         }
   //         // si el wn esta apuntando hacia izq/der con mayor inclinacion que hacia arriba, o bien, esta apuntando hacia abajo, return false
   //         else if ((remoteUp && axisVertical > 0f) && ((right && axisHorizontal > axisVertical) || (!right && -axisHorizontal > axisVertical)) || axisVertical <= 0f)
   //         {
   //             remoteUp = false;
   //             SendObjectDataToServer();
   //         }
   //         // si no se esta apretando el joystick
   //         else if (remoteUp && axisVertical <= 0.5)
   //         {
   //             remoteUp = false;
   //             SendObjectDataToServer();
   //         }
   //         return remoteUp;
   //     }
   //     return remoteUp;

    }

    protected bool IsItGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    protected virtual bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            bool pressedJump = CnInputManager.GetButtonDown("Jump Button");
            bool isJumping = pressedJump && isGrounded; 

            if(isJumping && !remoteJumping)
            {
                remoteJumping = true;
                SendObjectDataToServer();
            }

            else if(!isJumping && remoteJumping)
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

        if (previousTransform.x != newPosition.x)
        {
            return true;
        }

        if (previousTransform.y != newPosition.y)
        {
            return true;
        }

        return false;
    }

    protected void SynchronizeNonLocalPlayer()
    {
        if (!localPlayer)
        {

                transform.localScale = new Vector3(direction * 1f, directionY, 1f);
        
            SetAnimVariables();
        }

    }

	public virtual void StopMoving(){
		canMove = false;
		myAnim.SetFloat("Speed", 0);
		myAnim.SetBool("IsGrounded", true);
		myAnim.SetBool("IsAttacking", false);
	}

	public virtual void ResumeMoving(){
		canMove = true;
	}

	public void SetGravity(bool normal){

        rb2d.gravityScale = 2.5f;

        if (!normal) {
			directionY = -1;
			rb2d.gravityScale = -2.5f;
		}

    }

    protected virtual void Update()
    {

		Transform parentTransform = transform.parent;

		if (parentTransform != null) {
			parent = parentTransform.gameObject;		
		}

        if (rb2d.velocity.y > maxSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, maxSpeed);
        }

		if (!canMove) {
			return;
		}
        if (IsGoingRight())
        {
            rb2d.velocity = new Vector3(moveSpeed, rb2d.velocity.y, 0f);
			transform.localScale = new Vector3(1f, directionY, 1f);
            direction = 1;
        }
        else if (IsGoingLeft())
        {
            rb2d.velocity = new Vector3(-moveSpeed, rb2d.velocity.y, 0f);
			transform.localScale = new Vector3(-1f, directionY, 1f);
            direction = -1;
        }
        else // it's not moving
        {
            rb2d.velocity = new Vector3(0f, rb2d.velocity.y, 0f);
        }

        isGrounded = IsItGrounded();

        if (IsJumping(isGrounded))
        {
			rb2d.velocity = new Vector2(0, jumpSpeed * directionY);
        }

        previousTransform = transform.position;
        SetAnimVariables();
		IsPower();
    }

	protected virtual bool IsPower()
	{
		return false; // :O
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
		if(other.tag == "KillPlaneSpike")
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
		if(other.tag == "Poi")
		{
			PlannerPoi newPoi = other.GetComponent<PlannerPoi> ();
			if (!playerObj.playerAt.name.Equals (newPoi.name)) {
				Debug.Log ("Change OK: " + newPoi.name);
				playerObj.playerAt = newPoi;
				Planner planner = FindObjectOfType<Planner> ();
				planner.Monitor ();
			}
		}
	}

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "MovingPlatform")
		{
			Debug.Log (other.transform.position);
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

        this.remoteJumping = remoteJumping;
        this.remoteRight = remoteRight;
        this.remoteLeft = remoteLeft;
        this.isGrounded = isGrounded;
        this.direction = direction;
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
        string message = "ChangePosition/" + characterId + "/" + positionX + "/" + positionY + "/" + isGrounded + "/" + speed + "/" + direction + "/" + remoteJumping + "/" + remoteLeft + "/" + remoteRight;
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

	public virtual void RemoteSetter (bool power) {

	}

}
