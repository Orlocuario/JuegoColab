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


    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        myAnim = GetComponent<Animator>();
        respawnPosition = transform.position;
        theLevelManager = FindObjectOfType<LevelManager>();
        leftPressed = false;
        rightPressed = false;
        jumpPressed = false;
        localPlayer = false;
}

    public void Activate()
    {
        localPlayer = true;
        rb2d.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * Checks whether the character is going right.   
     */
    private bool isGoingRight()
    {
       if(!leftPressed && rightPressed)
        {
            return true;
        }
        return false;
    }

    /**
     * Checks whether the character is going left.   
     */
    private bool isGoingLeft()
    {
        if(!rightPressed && leftPressed)
        {
            return true;
        }
        return false;
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
        return jumpPressed && isGrounded;
    }

    /**
     * 
     */
    private void FixedUpdate()
    {
        if (localPlayer)
        {

            if (isGoingRight())
            {
                rb2d.velocity = new Vector3(moveSpeed, rb2d.velocity.y, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
                EnviarAccion(transform.position.x, transform.position.y);
            }
            else if (isGoingLeft())
            {
                rb2d.velocity = new Vector3(-moveSpeed, rb2d.velocity.y, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
                EnviarAccion(transform.position.x, transform.position.y);

            }
            else // it's not moving
            {
                rb2d.velocity = new Vector3(0f, rb2d.velocity.y, 0f);
            }

            isGrounded = isItGrounded();

            if (isJumping(isGrounded))
            {
                EnviarAccion(transform.position.x, transform.position.y);
                //rb2d.velocity = new Vector3(rb2d.velocity.x, jumpSpeed, 0f);
                rb2d.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
                jumpPressed = false;
            }

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

    public void EnviarAccion(float position_x, float position_y)
    {
        if (isGoingRight())
        {
            Debug.Log("ChangePosition/1/" + position_x + "/" + position_y + "/Derecha");
        }
        else if (isGoingLeft())
        {
            Debug.Log("ChangePosition/1/" + position_x + "/" + position_y + "/Izquierda");
        }
        else if (isJumping(isItGrounded()))
        {
            Debug.Log("ChangePosition/1/" + position_x + "/" + position_y + "/Salto");
        }

    }
}