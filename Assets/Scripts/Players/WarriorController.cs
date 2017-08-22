using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarriorController : PlayerController {

    string changeMpRate;
    public int numHits = 0;
    private int prevNumHits = 0;
    bool par;
	int contadorPar;
    int contadorHpAndMp;
    int rate;
    GameObject particulas;
    DisplayHUD hpAndMp;

    protected override void Start()
	{
		base.Start();
        changeMpRate = "0";
        rate = 150;
        contadorPar = 0;
        contadorHpAndMp = 0;
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
            if (contadorHpAndMp < rate)
            {
                contadorHpAndMp++;
            }
            else if (float.Parse(hpAndMp.mpCurrentPercentage) > 0f)
            {
                Client.instance.SendMessageToServer("ChangeMpHUDToRoom/" + changeMpRate);
                contadorHpAndMp = 0;
            }
            bool primeraVez = false;
            bool buttonState = CnInputManager.GetButtonDown("Power Button");
            if (float.Parse(hpAndMp.mpCurrentPercentage) == 0f)
            {
                changeMpRate = "0";
                remotePower = false;
                contadorPar = 0;
                SendPowerDataToServer();
                SetAnimacion(remotePower);
            }
            else if (buttonState && !primeraVez) 
			{
				primeraVez = true;
				remotePower = contadorPar%2 == 0;
                if (remotePower)
                {
                    changeMpRate = "-50";
                }
                else
                {
                    changeMpRate = "0";
                }
                contadorPar++;
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
