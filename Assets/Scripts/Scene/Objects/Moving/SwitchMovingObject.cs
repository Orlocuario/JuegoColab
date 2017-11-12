using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMovingObject : MovingObject
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        IgnoreSwitchMovingObjects();
    }

    private void IgnoreSwitchMovingObjects()
    {
        GameObject[] movingSwitchs = GameObject.FindGameObjectsWithTag("SwitchMovingObject");
        Collider2D collider = GetComponent<Collider2D>();

        foreach (GameObject movingSwitch in movingSwitchs)
        {
            Physics2D.IgnoreCollision(collider, movingSwitch.GetComponent<Collider2D>());
        }
    }

}
