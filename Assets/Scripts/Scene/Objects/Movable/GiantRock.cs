using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantRock : MovableObject
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        openningTrigger = "TriggerRocaGigante";
        openedPrefab = "Ambientales/SueloRoca";
    }
}
