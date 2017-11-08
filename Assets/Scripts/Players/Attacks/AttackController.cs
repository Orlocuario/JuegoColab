using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    protected Collider2D gameObjectCollider;

    protected static float destroyDelayTime = .04f;

    protected float currentDistance;
    protected float maxDistance;
    protected int direction;
    protected float speed;
    protected int damage;
    protected bool moves;

    PlayerController caster;

    protected virtual void Start()
    {
        currentDistance = 0;
        damage = 5;

        gameObjectCollider = gameObject.GetComponent<Collider2D>();

        IgnoreCollisionWithObjects();
        IgnoreCollisionWithPlayers();
    }

    protected void IgnoreCollisionWithObjects()
    {
        if (GameObject.Find("RocaGiganteAraña") != null)
        {
            Physics2D.IgnoreCollision(gameObjectCollider, GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0]);
        }
    }

    protected void IgnoreCollisionWithPlayers()
    {
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetEngineer().GetComponent<Collider2D>());
    }

    public void SetMovement(int _direction, float _speed, float initialX, float initialY, PlayerController _caster)
    {
        direction = _direction;
        caster = _caster;
        speed = _speed;
        moves = true;

        transform.position = new Vector2(initialX + (direction * 0.0001f), initialY);

        if (direction == -1)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    protected virtual void Update()
    {
        if (moves)
        {
            Move();
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

    protected bool IsCasterLocal()
    {
        return caster.localPlayer;
    }

    protected bool CollidedWithEnemy(GameObject other)
    {
        return other.tag == "Enemy";
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {

        if (CollidedWithEnemy(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }

        Destroy(this.gameObject, destroyDelayTime);
    }

}
