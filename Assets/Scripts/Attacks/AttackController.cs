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
        Physics2D.IgnoreCollision(gameObjectCollider, GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0]);
    }

    protected void IgnoreCollisionWithPlayers()
    {
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObjectCollider, Client.instance.GetEngineer().GetComponent<Collider2D>());
    }

    public void SetMovement(int direction, float speed, float x, float y, PlayerController caster)
    {
        this.direction = direction;
        this.caster = caster;
        this.speed = speed;

        transform.position = new Vector2(x + (direction * 0.0001f), y - 0.02f);

        if (direction == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    protected virtual void Update()
    {

        float distance = speed * direction * Time.deltaTime;

        transform.position = transform.position + Vector3.right * distance;

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
