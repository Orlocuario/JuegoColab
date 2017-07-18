using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

    private int bolas;
    private int maxBolas;
    public bool pressedAttack;
    protected override void Start()
    {
        base.Start();
        bolas = 0;
        maxBolas = 2;
    }

    protected override bool isAttacking()
    {
        if (localPlayer)
        {
            pressedAttack = CnInputManager.GetButtonDown("Attack Button");
            if (pressedAttack && !remoteAttacking)
            {
                remoteAttacking = true;
                SendObjectDataToServer();
                CastFireball();
            }
            else if(!pressedAttack && remoteAttacking)
            {
                remoteAttacking = false;
                SendObjectDataToServer();
            }
            return remoteAttacking;
        }
        return remoteAttacking;
    }

    private void CastFireball()
    {

    }

    protected override void Update()
    {
        base.Update();
    }
}
