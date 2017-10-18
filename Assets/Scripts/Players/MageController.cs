using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

	int contadorPar;
	GameObject particulas1;
	GameObject particulas2;
    DisplayHUD hpAndMp;
    bool powerOn;

	protected override void Start()
	{
		base.Start();
        powerOn = false;
        contadorPar = 0;
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
                CastFireball(this.directionX, 4);
            }
            else if(!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    public bool InShield(GameObject player)
    {
        if (powerOn)
        {
            float distance = Mathf.Abs(player.GetComponent<Transform>().position.magnitude - this.gameObject.GetComponent<Transform>().position.magnitude);
            return distance <= 0.77f;
        }
        return false;
        
    }

    protected override void SetAnimacion(bool activo)
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
