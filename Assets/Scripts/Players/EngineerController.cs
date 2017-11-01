using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController
{

    private GameObject particulas;

    private float skillSpeed;
    private  bool jumpedInAir;

    protected override void Start()
    {
        base.Start();

        jumpedInAir = false;

        particulas = GameObject.FindGameObjectWithTag("ParticulasEngin");
        particulas.SetActive(false);
    }

    protected override void Attack()
    {
        isAttacking = false;

        bool attackButtonPressed = CnInputManager.GetButtonDown("Attack Button");
        if (attackButtonPressed)
        {
            SendAttackDataToServer();
            CastProyectile();
        }

    }

    public override void SetAttack()
    {
        CastLocalProyectile();
    }

    private void CastProyectile()
    {
        CastLocalProyectile();
        SendProyectileSignalToServer();
    }

    public void CastLocalProyectile()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);

        GameObject proyectile = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/FlechaE1")); 
        ProyectileController controller = proyectile.GetComponent<ProyectileController>();
        controller.SetMovement(directionX, SkillSpeed(1), transform.position.x, transform.position.y, this);
    }

    private void SendProyectileSignalToServer()
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastProyectile/" + directionX + "/" + SkillSpeed(1).ToString() + "/" + x + "/" + y);
    }
    //change all 1's to GetLevel()

    public float SkillSpeed(int level)
    {
        if (level <= 1)
        {
            skillSpeed = 3.5f;
        }
        else if (level <= 3)
        {
            skillSpeed = 4f;
        }
        else if (level <= 5)
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

        bool pressedJump = CnInputManager.GetButtonDown("Jump Button");

        if (isGrounded)
        {
            jumpedInAir = false;
        }

        if (pressedJump && isGrounded)
        {
            return true;
        }

        if (pressedJump && !isGrounded && !jumpedInAir)
        {
            jumpedInAir = true;
            return true;
        }

        return false;
    }

    protected override void SetParticlesAnimationState(bool activo)
    {
        particulas.SetActive(activo);
    }

}

