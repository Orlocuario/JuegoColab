using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{

    CircleCollider2D collider2d;
    private static float maxColliderRadius = .25f;
	ParticleSystem particles;

    protected override void Start()
    {
		//particles = this.gameObject.GetComponent <ParticleSystem> ();
        base.Start();
        maxDistance = 5f;
        collider2d = GetComponent<CircleCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        collider2d.radius = (currentDistance / maxDistance) * maxColliderRadius;
		//particles.shape.radius = (currentDistance / maxDistance) * maxColliderRadius;
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

    private void OnCollisionEnter2D(Collision2D collision)
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
