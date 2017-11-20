using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController
{

    #region Attributes

    private bool jumpedInAir;

    #endregion

    #region Common

    public override void CastLocalAttack()
    {
        isAttacking = true;
        currentAttack = "Attacking";

        ProyectileController projectile = InstatiateAttack().GetComponent<ProyectileController>();
        projectile.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine(WaitAttacking());
        AnimateAttack();
    }

    #endregion

    #region Utils

    protected GameObject InstatiateAttack()
    {
        string attackName = "Arrow";
        return (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
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

    #endregion

}

