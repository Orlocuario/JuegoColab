using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

	GameObject particulas1;
	GameObject particulas2;

    protected static float shieldArea;

	protected override void Start()
	{
		base.Start();
        particulas1 = GameObject.Find ("ParticulasMage");
		particulas1.SetActive(false);
		particulas2 = GameObject.Find ("ParticulasMage2");
		particulas2.SetActive(false);

        shieldArea = particulas1.GetComponent<ParticleSystem>().shape.radius;
        Debug.Log("Mage particles radius is: " + shieldArea);
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

    public bool ProtectedByShield(GameObject player)
    {
        if (isPowerOn)
        {
            bool inShield = (player.transform.position - transform.position).magnitude <= shieldArea;
            Debug.Log("player position: " + player.transform.position.x + " mage position?: " + transform.position + "eval?: " + (player.transform.position - transform.position).magnitude + "inShield?: " + inShield);
            return inShield;
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
