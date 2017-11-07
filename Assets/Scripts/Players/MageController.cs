using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController
{

    GameObject particulas1;
    GameObject particulas2;

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

    public override void CastLocalAttack()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", isAttacking);
        currentAttackName = "MageAttack";

        GameObject fireball = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Fireball"));
        FireballController controller = fireball.GetComponent<FireballController>();
        controller.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine("Attacking");
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
        CastLocalAttack();
    }

}
