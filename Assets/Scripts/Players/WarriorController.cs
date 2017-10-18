using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class WarriorController : PlayerController {

    public int numHits = 0;
    private int prevNumHits = 0;
    bool par;
	int contadorPar;
	float damage;
    double force = 0;
    GameObject particulas;
    DisplayHUD hpAndMp;
    private int hits = 0;
    bool powerOn;


    protected override void Start()
	{
		base.Start();
		damage = 3;
        contadorPar = 0;
        particulas = GameObject.Find ("ParticulasWarrior");
		particulas.SetActive(false);
        hpAndMp = GameObject.Find("Canvas").GetComponent<DisplayHUD>();
    }

    protected override bool IsAttacking()
	{	
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if (buttonState && !remoteAttacking)
            {
				GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
				punch.GetComponent <Transform>().position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);
                remoteAttacking = true;
                numHits++;
                SendAttackDataToServer();
				if (SceneManager.GetActiveScene ().name == "Escena2") 
				{
					RemoveRockMass ();
				}
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    protected override void SetAnimVariables()
    {
        bool isAttacking = IsAttacking();
        myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        myAnim.SetBool("IsGrounded", isGrounded);

        if (prevNumHits >=numHits)
        {
            myAnim.SetBool("IsAttacking", false);
            myAnim.SetBool("IsAttacking2", false);

            return;
        }

        if (numHits % 2 == 0)
        {
            par = true;
        }
        else
        {
            par = false;
        }
        prevNumHits = numHits;


        if (par == false)
        {
            myAnim.SetBool("IsAttacking", isAttacking);

        }
        else
        {
            myAnim.SetBool("IsAttacking2", isAttacking);
        }
    }

    protected override void SetAnimacion(bool activo)
    {
        particulas.SetActive(activo);
	}
    
	public override void RemoteSetter(bool power)
	{
		SetAnimacion (power);
		remotePower = power;

	}

    private void CastOnePunchMan(string[] enemies, Vector3 position)
    {
		
		
    }


    protected override void SendAttackDataToServer()
    {
        string message = "AttackWarrior/" + characterId + "/" + remoteAttacking + "/" + numHits.ToString();
        Client.instance.SendMessageToServer(message);
    }

	public void RemoveRockMass()
	{
		Vector3 myPosition = this.GetComponent<Transform>().position;
		GameObject piedra = GameObject.FindGameObjectWithTag ("RocaGiganteAraña");
		Rigidbody2D rigidez = piedra.GetComponent<Rigidbody2D> ();
		Vector3 posicionPiedra = piedra.GetComponent<Transform> ().position;
		if ((myPosition - posicionPiedra).magnitude < 3f) 
		{
            hits++;
			if (hits > 7) {
				hits = 7;
			}
            force = Math.Pow(hits, damage)/50;
            rigidez.AddForce(Vector2.right * (float)force);
        }
	}
}
