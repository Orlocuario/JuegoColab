using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileController : MonoBehaviour {

    private int direction;
    private float skillSpeed;
    bool goingUp;
    float maxDistance;
    float currentDistance;
    EngineerController caster;

    void Start()
    {
        maxDistance = 7;
        currentDistance = 0;
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0]);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetMage().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetWarrior().GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), Client.instance.GetEngineer().GetComponent<Collider2D>());
    }

    public void SetMovement(int direction, float skillSpeed, float x, float y, EngineerController caster)
    {
        this.caster = caster;
        this.goingUp = caster.IsGoingUp();
        this.direction = direction;
        this.skillSpeed = skillSpeed;
        transform.position = new Vector2(x, y - 0.02f);
        if (direction == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = skillSpeed * direction * Time.deltaTime;
        if (this.goingUp && direction > 0f)
        {
            transform.position = transform.position + Vector3.up * distance;
        }
        else if (this.goingUp && direction < 0f)
        {
            transform.position = transform.position + Vector3.up * (-distance);
        }
        else
        {
            transform.position = transform.position + Vector3.right * distance;
        }
        currentDistance += System.Math.Abs(distance);
        if (maxDistance <= currentDistance)
        {
            Destroy(gameObject);
        }
    }

    //Hacer que reciba un enemigo
    public void DealDamage()
    {
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
