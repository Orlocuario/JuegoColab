﻿using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarriorController : PlayerController {

    public int numHits = 0;
    private int prevNumHits = 0;
    bool par;
	int contador = 0;
    GameObject particulas;

	protected override void Start()
	{
		base.Start();
		particulas = GameObject.Find ("ParticulasWarrior");
		particulas.SetActive(false);
	}

    protected override bool IsAttacking()
	{	
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if (buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                numHits++;
                SendAttackDataToServer();
                //CastOnePunchMan(Client.instance.GetAllEnemies(), this.GetComponent<RectTransform>().position);
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

	protected override bool IsPower()
	{
		if (localPlayer) 
		{	
			bool primeraVez = false;
			bool buttonState = CnInputManager.GetButtonDown ("Power Button");
			if (buttonState && !primeraVez) 
			{
                Client.instance.SendMessageToServer("ChangeMpHUDToRoom/-10");
				primeraVez = true;
				remotePower = contador%2 == 0;
				contador++;
                SetAnimacion(remotePower);
                SendPowerDataToServer();
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

	private void SetAnimacion(bool activo)
	{
		particulas.SetActive (activo);
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
}
