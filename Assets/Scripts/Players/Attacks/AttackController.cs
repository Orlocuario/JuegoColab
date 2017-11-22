using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    #region Attributes

    protected Collider2D gameObjectCollider;
    protected PlayerController caster;

    protected static float destroyDelayTime = .04f;

    protected float currentDistance;
    protected float maxDistance;
    protected bool initialized;
    protected bool isMoving;
    protected int direction;
    protected bool enhanced;
    protected float speed;
    protected int damage;

    #endregion

    #region Start & Update

    protected virtual void Start()
    {
        currentDistance = 0;
        damage = 5;

        gameObjectCollider = gameObject.GetComponent<Collider2D>();

        IgnoreCollisionWithPlayers();
    }


    protected virtual void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    #endregion

    #region Common

    public void Initialize(PlayerController _caster)
    {
        enhanced = _caster.isPowerOn;
        caster = _caster;

        initialized = true;
    }

    public void SetMovement(int _direction, float _speed, Vector2 initialPosition)
    {
        if (!initialized)
        {
            Debug.Log("Initialize attacks before moving them");
            return;
        }

        direction = _direction;
        speed = _speed;
        isMoving = true;

        transform.position = new Vector2(initialPosition.x + (direction * 0.0001f), initialPosition.y);

        if (direction == -1)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    protected void Move()
    {
        float distance = speed * direction * Time.deltaTime;

        transform.position += Vector3.right * distance;

        currentDistance += System.Math.Abs(distance);

        if (maxDistance <= currentDistance)
        {
            Destroy(gameObject);
        }
    }

    //Hacer que reciba un enemigo
    protected void DealDamage(GameObject enemy)
    {

        float dealtDamage = GetDamage();

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.TakeDamage(dealtDamage);

    }

    // Este método debe ser sobreescrito para calcular el daño en cada caso
    protected virtual int GetDamage()
    {
        return damage;
    }

    #endregion

    #region Utils

    protected void IgnoreCollisionWithPlayers()
    {
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetEngineer().GetComponent<Collider2D>());
    }

    protected bool IsCasterLocal()
    {
        return caster.localPlayer;
    }

    protected bool CollidedWithEnemy(GameObject other)
    {
        return other.GetComponent<EnemyController>();
    }

    #endregion

    #region Events

    protected void OnCollisionEnter2D(Collision2D collision)
    {

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }

        Destroy(this.gameObject, destroyDelayTime);
    }

    #endregion
}
