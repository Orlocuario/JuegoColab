using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController
{
    private GameObject particulas;

    private bool jumpedInAir;

    protected override void Start()
    {
        base.Start();

        jumpedInAir = false;

        particulas = GameObject.Find("ParticulasEngin");
        particulas.SetActive(false);
    }

    public override void CastLocalAttack()
    {
        isAttacking = true;

        GameObject proyectile = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Arrow"));
        ProyectileController controller = proyectile.GetComponent<ProyectileController>();
        controller.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        if (!animControl)
        {
            Debug.Log("AnimatorControl not found in " + name);
            return;
        }

        StartCoroutine(animControl.StartAnimation("Attacking", this.gameObject));

    }

    protected override bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            
            if (!isPowerOn)
            {
                return base.IsJumping(isGrounded);
            }

            if (isGrounded)
            {
                jumpedInAir = false;
            }

            bool pressedJump = CnInputManager.GetButtonDown("Jump Button");

            if (pressedJump && isGrounded && !remoteJumping)
            {
                remoteJumping = true;
                SendPlayerDataToServer();
                return true;
            }

            if (pressedJump && !isGrounded && !jumpedInAir && !remoteJumping)
            {
                remoteJumping = true;
                jumpedInAir = true;
                SendPlayerDataToServer();
                return true;
            }

            if (remoteJumping)
            {
                remoteJumping = false;
                SendPlayerDataToServer();
            }

            return false;
        }

        return remoteJumping;
    }

    public override void SetAttack()
    {
        CastLocalAttack();
    }

    protected override void SetParticlesAnimationState(bool activo)
    {
        particulas.SetActive(activo);
    }

}

