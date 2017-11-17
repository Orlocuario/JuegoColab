using System.Collections;
using UnityEngine;


public class WarriorController : PlayerController
{

    #region Attributes

    protected int attacks = 0;

    #endregion

    #region Common

    public override void CastLocalAttack()
    {
        isAttacking = true;

        GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
        PunchController punchController = punch.GetComponent<PunchController>();
        punchController.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine(WaitAttacking());

        if (attacks++ % 2 == 0)
        {
            currentAttack = "Attacking2";
        }
        else
        {
            currentAttack = "Attacking";
        }

        AnimateAttack();
    }
    
    #endregion

}
