using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageController : PlayerController
{

    #region Attributes

    protected static float shieldArea;

    #endregion

    #region Start & Update

    protected override void Start()
    {
        base.Start();
        shieldArea = 0;
        LoadShieldArea();
    }

    protected override void Update()
    {
        base.Update();
        DebugDrawDistance(shieldArea);
    }

    #endregion

    #region Common

    public override void CastLocalAttack()
    {
        isAttacking = true;
        currentAttack = "Attacking";

        GameObject fireball = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Fireball"));
        FireballController controller = fireball.GetComponent<FireballController>();
        controller.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine(WaitAttacking());
        AnimateAttack();
    }

    public bool ProtectedByShield(GameObject player)
    {
        if (isPowerOn)
        {
            return Vector2.Distance(player.transform.position, transform.position) <= shieldArea;
        }

        return false;
    }

    #endregion

    #region Utils

    protected void LoadShieldArea()
    {
        foreach (GameObject particle in particles)
        {
            float radius = particle.GetComponent<ParticleSystem>().shape.radius;
            if (shieldArea < radius)
            {
                shieldArea = radius;
            }
        }
    }

    #endregion

}
