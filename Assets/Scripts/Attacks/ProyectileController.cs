using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileController : AttackController
{
    EngineerController caster;

    protected override void Start()
    {
        base.Start();   
        maxDistance = 7;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

}
