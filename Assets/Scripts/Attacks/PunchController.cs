using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{

    CircleCollider2D collider2d;
    private static float maxColliderRadius = .25f;

    protected override void Start()
    {
        base.Start();
        maxDistance = 2.5f;
        collider2d = GetComponent<CircleCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        collider2d.radius = (currentDistance / maxDistance) * maxColliderRadius;
    }

    protected bool CollidedWithDestroyable(GameObject other)
    {
        return other.tag == "Destroyable";
    }

    protected void DestroyDestroyable(GameObject other)
    {
        DestroyableController destroyable = other.GetComponent<DestroyableController>();
        destroyable.DestroyMe();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);

        }

        else if (CollidedWithDestroyable(collision.gameObject))
        {
            DestroyDestroyable(collision.gameObject);
        }

        Destroy(this.gameObject, destroyDelayTime);
    }

}
