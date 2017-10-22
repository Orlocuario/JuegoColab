using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspirituController : EnemyController
{

    // Use this for initialization
    protected override void Start () {
        this.maxHp = 20f;
        base.Start();
	}

}
