using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController {


    protected override void Start()
    {
        base.Start();
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
                CastFireball(this.direction, 4);
            }
            else if(!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    public void CastFireball(int direction, float speed)
    {
        GameObject fireball = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/BolaM1"));
        FireballController controller = fireball.GetComponent<FireballController>();
        Vector2 myPosition = transform.position;
        controller.SetMovement(direction, speed, myPosition.x, myPosition.y);
    }

    private void SendFireballSignalToServer()
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastFireball/" + x + "/" +y);
    }

    protected override void Update()
    {
        base.Update();
    }
}
