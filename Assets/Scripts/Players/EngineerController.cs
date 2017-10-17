using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController {

    private float skillSpeed;
    private bool salte;
    string changeMpRate;
    int contadorHpAndMp;
    int rate;
    int contadorPar;
    GameObject particulas;
    DisplayHUD hpAndMp;
    bool jumpedInAir = false;


    protected override void Start()
	{
		base.Start();
        changeMpRate = "0";
        rate = 150;
        contadorPar = 0;
        contadorHpAndMp = 0;
		particulas = GameObject.FindGameObjectWithTag ("ParticulasEngin");
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
                SendAttackDataToServer();
                CastProyectile(this.directionX, this.IsGoingUp());
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    private void CastProyectile(int direction, bool goingUp)
    {
        Vector3 myPosition = transform.position;
        CastLocalProyectile(direction, myPosition.x, myPosition.y, this);
        SendProyectileSignalToServer(direction);
    }

    public void CastLocalProyectile(int direction, float x, float y, EngineerController caster)
    {
        GameObject proyectile = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/FlechaE1")); //Encontrar el prefab de la roca
        ProyectileController controller = proyectile.GetComponent<ProyectileController>();
        controller.SetMovement(direction, SkillSpeed(1), x, y, this);
    }

    private void SendProyectileSignalToServer(int direction)
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastProyectile/" + direction + "/" + SkillSpeed(1).ToString() + "/" + x + "/" + y);
    }
    //change all 1's to GetLevel()

    public float SkillSpeed(int level)
    {
        if (level <= 1)
        {
            skillSpeed = 3.5f;
        }
        else if(level <= 3)
        {
            skillSpeed = 4f;
        }
        else if(level <= 5)
        {
            skillSpeed = 4.5f;
        }
        else
        {
            skillSpeed = 5f;
        }
        return skillSpeed;
    }

    protected override bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            if (isGrounded) {
                jumpedInAir = false;
            }

            bool pressedJump = CnInputManager.GetButtonDown("Jump Button");

            if (pressedJump && isGrounded && !remoteJumping)
            {
                remoteJumping = true;
                SendObjectDataToServer();
                return true;
            }

            if (pressedJump && !isGrounded && !jumpedInAir && !remoteJumping)
            {
                remoteJumping = true;
                jumpedInAir = true;
                SendObjectDataToServer();
                return true;
            }

             if(remoteJumping)
             {
                 remoteJumping = false;
                 SendObjectDataToServer();
             }

            return  false;
        }

        return remoteJumping;
    }

    /*public int GetLevel()
     {

     }*/
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
			bool buttonState = CnInputManager.GetButtonDown ("Power Button");
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
                    changeMpRate = "-5";
                }
                else
                {
                    changeMpRate = "0";
                }
                contadorPar++;
				SetAnimacion (remotePower);
				SendPowerDataToServer();
			}

			else if (!buttonState && primeraVez)
			{
				primeraVez = false;
			}
		}
		return remotePower;
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
}

