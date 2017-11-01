using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{

    protected override void Start()
    {
        base.Start();   
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }

}
