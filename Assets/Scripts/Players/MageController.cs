using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

    string changeMpRate;
	int contadorPar;
    int contadorHpAndMp;
    int rate;
	GameObject particulas1;
	GameObject particulas2;
    DisplayHUD hpAndMp;

	protected override void Start()
	{
		base.Start();
        changeMpRate = "0";
        rate = 150;
        contadorPar = 0;
        contadorHpAndMp = 0;
        particulas1 = GameObject.Find ("ParticulasMage");
		particulas1.SetActive(false);
		particulas2 = GameObject.Find ("ParticulasMage2");
		particulas2.SetActive(false);
        hpAndMp = GameObject.Find("Canvas").GetComponent<DisplayHUD>();
	}

    protected override bool IsAttacking()
    {
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if(buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                SendAttackDataToServer();
                CastFireball(this.direction, 4);
            }
            else if(!buttonState && remoteAttacking)
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
			bool buttonState = CnInputManager.GetButtonDown ("Power Button");
            InShield();
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
                remotePower = contadorPar % 2 == 0;
                if (remotePower)
                {
                    changeMpRate = "-5";
                }
                else
                {
                    changeMpRate = "0";
                }
                contadorPar++;
                SendPowerDataToServer();
                SetAnimacion(remotePower);
            }
			else if (!buttonState && primeraVez)
			{
				primeraVez = false;
			}
		}
		return remotePower;
	}

    public bool[] InShield()
    {
        List<bool> shieldInPlayers = new List<bool>();
        GameObject[] players = GameObject.Find("LevelManager").GetComponent<LevelManager>().players;
        for (int i = 0; i < players.Length; i++)
        {
            Vector3 distance = (players[i].GetComponent<Transform>().position - this.gameObject.GetComponent<Transform>().position);
            if (distance.magnitude <= 0.77f)
            {
                shieldInPlayers.Add(true);
            }
            else
            {
                shieldInPlayers.Add(false);
            }
        }
        return shieldInPlayers.ToArray();
    }

	private void SetAnimacion(bool activo)
	{
		particulas1.SetActive (activo);
		particulas2.SetActive (activo);
	}

	public override void RemoteSetter(bool power)
	{
		SetAnimacion (power);
		remotePower = power;
	}

    private void CastFireball(int direction, float speed)
    {
        Vector3 myPosition = transform.position;
        CastLocalFireball(direction, speed,myPosition.x, myPosition.y, this);
        SendFireballSignalToServer(direction, speed);
    }

    public void CastLocalFireball(int direction, float speed, float x, float y, MageController caster)
    {
        GameObject fireball = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/BolaM1"));
        FireballController controller = fireball.GetComponent<FireballController>();
        controller.SetMovement(direction, speed, x, y, this);
    }

    private void SendFireballSignalToServer(int direction, float speed)
    { 
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastFireball/" + direction + "/" + speed + "/" + x + "/" + y);
    }
		
}
