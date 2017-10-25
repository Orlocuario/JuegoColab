using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspirituController : EnemyController
{

    protected override void Start()
    {
        force = new Vector2(1500f, 500f);
        damage = 2;
        maxHp = 20f;
        base.Start();
    }

    protected override void Update()
    {
        if (patrolling)
        {
            Patroll();
        }
    }

}
