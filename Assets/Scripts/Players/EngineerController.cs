using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController {

    private float skillSpeed;
    GameObject particulas;
    bool jumpedInAir = false;

    protected override void Start()
	{
		base.Start();
		particulas = GameObject.FindGameObjectWithTag ("ParticulasEngin");
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

	protected override void SetAnimacion(bool activo)
	{
		particulas.SetActive (activo);
	}

	public override void RemoteSetter(bool power)
	{
		SetAnimacion (power);
		remotePower = power;

	}
}

