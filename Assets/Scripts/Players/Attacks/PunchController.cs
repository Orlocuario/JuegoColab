using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchController : AttackController
{
    #region Attributes

    private static Vector2 attackForce = new Vector2(2500f, 100f);

    #endregion

    #region Start & Update

    protected override void Start()
    {

        base.Start();
        maxDistance = 3f;
    }

    protected override void Update()
    {
        base.Update();
        
    }

    #endregion

    #region Common

    protected void DestroyObject(GameObject other)
    {
        DestroyableObject destroyable = other.GetComponent<DestroyableObject>();

        if (destroyable.reinforced && !enhanced)
        {
            return;
        }

        destroyable.DestroyMe(true);
    }

    protected void MoveObject(GameObject other)
    {

        MovableObject movable = other.GetComponent<MovableObject>();
        Vector2 force = attackForce;

        if (enhanced)
        {
            force *= 150;
        }

        if (other.transform.position.x < transform.position.x)
        {
            force.x *= -1;
        }

        movable.MoveMe(force, true);
    }

    #endregion

    #region Events

    private new void OnCollisionEnter2D(Collision2D collision)
    {

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }

        if (caster.localPlayer)
        {
            if (CollidedWithDestroyable(collision.gameObject))
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

    #endregion

    #region Utils

    protected bool CollidedWithDestroyable(GameObject other)
    {
        return other.GetComponent<DestroyableObject>();
    }

    protected bool CollidedWithMovable(GameObject other)
    {
        return other.GetComponent<MovableObject>();
    }

    protected override int GetDamage()
    {
        if (enhanced)
        {
            return damage + 4;
        }
        else
        {
            return damage;
        }
    }

    #endregion

}
