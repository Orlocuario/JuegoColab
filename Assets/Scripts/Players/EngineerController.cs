using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerController : PlayerController {

    protected override bool IsAttacking()
    {
        if (localPlayer)
        {
            bool buttonState = CnInputManager.GetButtonDown("Attack Button");
            if (buttonState && !remoteAttacking)
            {
                remoteAttacking = true;
                SendAttackDataToServer();
                CastProyectile(this.direction);
            }
            else if (!buttonState && remoteAttacking)
            {
                remoteAttacking = false;
                SendAttackDataToServer();
            }
        }
        return remoteAttacking;
    }

    private void CastProyectile(int direction) //speed missing as parameter
    {
        Vector3 myPosition = transform.position;
        CastLocalProyectile(direction, speed, myPosition.x, myPosition.y, this);
        SendProyectileSignalToServer(direction, speed);
    }

    public void CastLocalProyectile(int direction, float speed, float x, float y, EngineerController caster)
    {
        GameObject proyectile = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/BolaM1")); //Encontrar el prefab de la honda
        ProyectileController controller = proyectile.GetComponent<ProyectileController>();
        controller.SetMovement(direction, speed, x, y, this);
    }

    private void SendProyectileSignalToServer(int direction, float speed)
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastProyectile/" + direction + "/" + speed + "/" + x + "/" + y);
    }
}
}
