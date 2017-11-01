using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController
{

    GameObject particulas1;
    GameObject particulas2;

    private static int attackSpeed = 4;

    protected static float shieldArea;

    protected override void Start()
    {
        base.Start();
        particulas1 = GameObject.Find("ParticulasMage");
        particulas1.SetActive(false);
        particulas2 = GameObject.Find("ParticulasMage2");
        particulas2.SetActive(false);

        shieldArea = particulas1.GetComponent<ParticleSystem>().shape.radius;
    }

    protected override void Attack()
    {
        if (!localPlayer)
        {
            return;
        }

        isAttacking = false;

        bool attackButtonPressed = CnInputManager.GetButtonDown("Attack Button");
        if (attackButtonPressed)
        {
            SendAttackDataToServer();
            CastFireball();
        }

    }

    public bool ProtectedByShield(GameObject player)
    {
        if (isPowerOn)
        {
            return Vector2.Distance(player.transform.position, transform.position) <= shieldArea;
        }

        return false;
    }

    protected override void SetParticlesAnimationState(bool activo)
    {
        particulas1.SetActive(activo);
        particulas2.SetActive(activo);
    }

    public override void SetAttack()
    {
        CastLocalFireball();
    }

    private void CastFireball()
    {
        Vector3 myPosition = transform.position;
        CastLocalFireball();
        SendFireballSignalToServer();
    }

    public void CastLocalFireball()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);

        GameObject fireball = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/BolaM1"));
        FireballController controller = fireball.GetComponent<FireballController>();
        controller.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);
    }

    private void SendFireballSignalToServer()
    {
        string x = transform.position.x.ToString();
        string y = transform.position.y.ToString();
        Client.instance.SendMessageToServer("CastFireball/" + directionX + "/" + attackSpeed + "/" + transform.position.x + "/" + transform.position.y);
    }

}
