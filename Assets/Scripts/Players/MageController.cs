using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {

    private int bolas;
    private int maxBolas;

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
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if(buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                SendAttackDataToServer();
                CastFireball();
            }
            else if(!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    private void CastFireball()
    {
        Client.instance.SendMessageToServer("FIREBALL");
    }

    protected override void Update()
    {
        base.Update();
    }
}
