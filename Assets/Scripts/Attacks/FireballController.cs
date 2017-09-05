using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour {

    private int direction;
    private float speed;
    float maxDistance;
    float currentDistance;
    MageController caster;

    void Start() {
        maxDistance = 12;
        currentDistance = 0;
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0]);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetEngineer().GetComponent<Collider2D>());
    }

    public void SetMovement(int direction, float speed, float x, float y, MageController caster)
    { 
        this.caster = caster;
        this.direction = direction;
        this.speed = speed;
        transform.position = new Vector2(x + (direction*0.0001f), y - 0.02f);
        if (direction == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    void Update() {
        float distance = speed * direction * Time.deltaTime;
        transform.position = transform.position + Vector3.right * distance;
        currentDistance += System.Math.Abs(distance);
        if (maxDistance <= currentDistance)
        {
            Destroy(gameObject);
        }
    }

    //Hacer que reciba un enemigo
    public void DealDamage(){
        if (IsCasterLocal())
        {
            //Hace daño o algo
        }
        Destroy(gameObject);
    }

    private bool IsCasterLocal()
    {
        return caster.localPlayer;
    }
}
