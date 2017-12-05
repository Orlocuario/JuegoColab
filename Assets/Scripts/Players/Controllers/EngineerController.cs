using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController
{

    #region Attributes

    private bool jumpedInAir;

    #endregion

    #region Utils

    protected override AttackController GetAttack()
    {
        var attackType = new ProyectileController().GetType();
        string attackName = "Arrow";

        GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
        ProyectileController attackController = (ProyectileController)attackObject.GetComponent(attackType);

        return attackController;
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

