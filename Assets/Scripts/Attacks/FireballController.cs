using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : AttackController
{

    protected override void Start()
    {
        base.Start();
        maxDistance = 12;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

}

