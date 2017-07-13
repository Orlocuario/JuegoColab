using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;
    private Rigidbody2D rb2d;
    public float jumpSpeed;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public bool isGrounded;
    private Animator myAnim;
    public Vector3 respawnPosition;
    public LevelManager theLevelManager;

    public bool leftPressed;
    public bool rightPressed;
    public bool jumpPressed;
    public bool localPlayer;
    private int direction;  //1 = derecha, -1 = izquierda
    private Vector3 previous_transform;
    private Transform transform;
    public int characterId;
    public float speed; //For animation nonlocal purposes
    // Use this for initialization
    void Start()
    {
        transform = GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        myAnim = GetComponent<Animator>();
        respawnPosition = transform.position;
        theLevelManager = FindObjectOfType<LevelManager>();
        leftPressed = false;
        rightPressed = false;
        jumpPressed = false;
        localPlayer = false;
        direction = 1;
}

    public void Activate(int charId)
    {
        localPlayer = true;
        rb2d.isKinematic = false;
        GetComponent<BoxCollider2D>().enabled = true;
        this.characterId = charId;
    }


    /**
     * Checks whether the character is going right.   
     */
    private bool isGoingRight()
    {
        return CnInputManager.GetAxisRaw("Horizontal") > 0f;
    }

    /**
     * Checks whether the character is going left.   
     */
    private bool isGoingLeft()
    {
        return CnInputManager.GetAxisRaw("Horizontal") < 0f;
    }

    /**
     * Checks whether the character is going left.   
     */
    private bool isItGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    /**
     * Checks whether the character is jumping.   
     */
    private bool isJumping(bool isGrounded)
    {
        return CnInputManager.GetButtonDown("Jump Button") && isGrounded;
    }

    /**
     * 
     */
    
    private bool CheckIfSomethingChanged()
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


    private void SynchronizeNonLocalPlayer()
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
            myAnim.SetFloat("Speed", speed);
            myAnim.SetBool("Ground", isGrounded);

        }
    }

    private int updateFrames = 0;

    private void Update()
    {
        SynchronizeNonLocalPlayer();
        if (localPlayer)
        {

            if (isGoingRight())
            {
                rb2d.velocity = new Vector3(moveSpeed, rb2d.velocity.y, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
                direction = 1;
            }
            else if (isGoingLeft())
            {
                rb2d.velocity = new Vector3(-moveSpeed, rb2d.velocity.y, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
                direction = -1;
            }
            else // it's not moving
            {
                rb2d.velocity = new Vector3(0f, rb2d.velocity.y, 0f);
            }

            isGrounded = isItGrounded();

            if (isJumping(isGrounded))
            {
                //rb2d.velocity = new Vector3(rb2d.velocity.x, jumpSpeed, 0f);
                rb2d.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
                jumpPressed = false;
            }
            if (CheckIfSomethingChanged())
            {
                updateFrames++;
                if (updateFrames < 1)
                {
                    return;
                }
                updateFrames = 0;
                SendObjectDataToServer();
            }
            previous_transform = transform.position;
            myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
            myAnim.SetBool("Ground", isGrounded);
        }      
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "KillPlane")
        {
            //gameObject.SetActive(false); 
            //transform.position = respawnPosition;
            theLevelManager.Respawn();
        }

        if (other.tag == "Checkpoint")
        {
            respawnPosition = other.transform.position;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    public void SetVariablesFromServer(float positionX, float positionY, bool isGrounded, float speed, int direction)
    {
        if (localPlayer)
        {
            return;
        }
        transform.position = new Vector3(positionX, positionY);
        this.isGrounded = isGrounded;
        this.speed = speed;
        this.direction = direction;
    }
    public void SendObjectDataToServer()
    {
        float position_x = transform.position.x;
        float position_y = transform.position.y;
        bool grounded = isGrounded;
        float speed = Mathf.Abs(rb2d.velocity.x);
        string message = "ChangePosition/" + characterId + "/" + position_x + "/" + position_y + "/" + isGrounded + "/" + speed + "/" + direction;
        Client.instance.SendMessageToServer(message);
    }
}