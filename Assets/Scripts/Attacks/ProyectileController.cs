using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileController : AttackController
{

    protected override void Start()
    {
        base.Start();   
        maxDistance = 12;
    }

}
