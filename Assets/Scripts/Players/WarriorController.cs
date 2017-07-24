using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarriorController : PlayerController {

    private int numHits = 0;
    bool par;

    protected override bool IsAttacking()
    {
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if (buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                SendAttackDataToServer();
                numHits++;
                //CastOnePunchMan(Client.instance.GetAllEnemies(), this.GetComponent<RectTransform>().position);
                //CastFireball(this.direction, 4);
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    protected override void SetAnimVariables()
    {
        if (numHits % 2 == 0)
        {
            par = true;
        }
        else
        {
            par = false;
        }

        myAnim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        myAnim.SetBool("IsGrounded", isGrounded);
        if (par == false)
        {
            myAnim.SetBool("IsAttacking", IsAttacking());
        }
        else
        {
            myAnim.SetBool("IsAttacking2", IsAttacking());
        }

    }

    private void CastOnePunchMan(string[] enemies, Vector3 position)
    {
        
    }
}
