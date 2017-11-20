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

        if (attacks++ % 2 == 0)
        {
            currentAttack = "Attacking2";
        }
        else
        {
            currentAttack = "Attacking";
        }

        PunchController punch = InstatiateAttack().GetComponent<PunchController>();
        punch.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine(WaitAttacking());
        AnimateAttack();
    }

    #endregion

    #region Utils

    protected GameObject InstatiateAttack()
    {
        string attackName = (isPowerOn) ? "SuperPunch" : "Punch";
        return (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
    }

    #endregion

}
