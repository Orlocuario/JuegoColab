using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

	int contador = 0;
	GameObject particulasMago;

	protected override void Start()
	{
		base.Start();
		particulasMago = (GameObject)Instantiate(Resources.Load("Prefabs/Particulas/ParticulasMago"));
		particulasMago.SetActive (false);
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

	private void SetAnimacion(bool activo)
	{
		particulasMago.SetActive (activo);
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
