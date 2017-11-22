using System.Collections;
using UnityEngine;


public class WarriorController : PlayerController
{

    #region Attributes

    protected int attacks = 0;

    #endregion

    #region Utils

    protected override AttackController GetAttack()
    {

        attackAnimName = (attacks++ % 2 == 0) ? "Attacking2" : "Attacking";

        string attackName = (isPowerOn) ? "SuperPunch" : "Punch";
        var attackType = new PunchController().GetType();


        GameObject attackObject = (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
        PunchController attackController = (PunchController)attackObject.GetComponent(attackType);

        return attackController;
    }

    #endregion

}
