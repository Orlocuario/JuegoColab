using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarriorController : PlayerController {

    private int numHits = 0;
    bool par;
	int contador = 0;
	GameObject particulas;

	private void Start()
	{
		base.Start();
		particulas = GameObject.Find("ParticulasWarrior");
		particulas.SetActive (false);
	}

    protected override bool IsAttacking()
	{	return false;
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if (buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                SendAttackDataToServer();
                numHits++;
				SetAnimacion (remoteAttacking);
                //CastOnePunchMan(Client.instance.GetAllEnemies(), this.GetComponent<RectTransform>().position);
                //CastFireball(this.direction, 4);
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
				SetAnimacion (remoteAttacking);
            }
        }
        return remoteAttacking;
    }

	protected override bool isPower()
	{
		if (localPlayer) 
		{	
			bool primeraVez = false;
			bool buttonState = CnInputManager.GetButtonDown ("Power Button");
			if (buttonState && !primeraVez) 
			{
				primeraVez = true;
				remotePower = contador%2 == 0;
				contador++;
				SendPowerDataToServer();
				SetAnimacion (remotePower);
			}

			else if (!buttonState && primeraVez)
			{
				primeraVez = false;
			}
		}
		return remotePower;
	}

    protected override void SetAnimVariables()
    {
        if (numHits % 2 == 0)
        {
            par = true;
        }
        else
        {
            par = false;
        }

        myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        myAnim.SetBool("IsGrounded", isGrounded);
        if (par == false)
        {
            myAnim.SetBool("IsAttacking", IsAttacking());
        }
        else
        {
            myAnim.SetBool("IsAttacking2", IsAttacking());
        }

    }

	private void SetAnimacion(bool activo)
	{
		particulas.SetActive (activo);
	}
    
	public void RemoteSetter(bool power)
	{
		SetAnimacion (power);
		remotePower = power;

	}
}
