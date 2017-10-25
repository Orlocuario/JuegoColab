using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspirituController : EnemyController
{

    protected override void Start () {
        this.maxHp = 20f;
        base.Start();
	}

    protected override void Update()
    {
        Patroll();
    }

}
