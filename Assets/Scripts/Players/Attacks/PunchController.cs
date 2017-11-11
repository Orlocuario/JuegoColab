using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{

    CircleCollider2D collider2d;
    ParticleSystem particles;

    private static Vector2 attackForce = new Vector2(2000f, 100f);
    private static float maxColliderRadius = .25f;

    protected override void Start()
    {
        //particles = this.gameObject.GetComponent <ParticleSystem> ();
        base.Start();
        maxDistance = 3f;
        collider2d = GetComponent<CircleCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        //collider2d.radius = (currentDistance / maxDistance) * maxColliderRadius;
        //particles.shape.radius = (currentDistance / maxDistance) * maxColliderRadius;
    }

    protected bool CollidedWithDestroyable(GameObject other)
    {
        return other.GetComponent<DestroyableObject>();
    }

    protected bool CollidedWithMovable(GameObject other)
    {
        return other.GetComponent<MovableObject>();
    }

    protected void DestroyObject(GameObject other)
    {
        DestroyableObject destroyable = other.GetComponent<DestroyableObject>();
        destroyable.DestroyMe();
    }

    protected void MoveObject(GameObject other)
    {

        MovableObject movable = other.GetComponent<MovableObject>();
        Vector2 force = attackForce;

        if (caster.isPowerOn)
        {
            force *= 100;
        }

        if (other.transform.position.x < transform.position.x)
        {
            force.x *= -1;
        }

        movable.MoveMe(force);
    }

    private new void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Choqué con " + collision.gameObject.name + " " + collision.gameObject.GetComponent<MovableObject>());

        if (caster.localPlayer)
        {
            if (CollidedWithEnemy(collision.gameObject))
            {
                DealDamage(collision.gameObject);
            }

            else if (CollidedWithDestroyable(collision.gameObject))
            {
                DestroyObject(collision.gameObject);
            }

            else if (CollidedWithMovable(collision.gameObject))
            {
                MoveObject(collision.gameObject);
            }
        }

        Destroy(this.gameObject, destroyDelayTime);
    }

}
