﻿using CnControls;
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

        FireballController fireball = InstatiateAttack().GetComponent<FireballController>();
        fireball.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

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

    protected GameObject InstatiateAttack()
    {
        string attackName = "Fireball";
        return (GameObject)Instantiate(Resources.Load(attackPrefabName + attackName));
    }

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
