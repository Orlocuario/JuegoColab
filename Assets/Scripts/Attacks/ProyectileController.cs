using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileController : MonoBehaviour {

    private int direction;
    private float speed;
    float maxDistance;
    float currentDistance;
    EngineerController caster;
    // Use this for initialization
    void Start()
    {
        maxDistance = 3; //Get From Bag
        currentDistance = 0;
    }

    public void SetMovement(int direction, float speed, float x, float y, EngineerController caster)
    {
        this.caster = caster;
        this.direction = direction;
        this.speed = speed;
        transform.position = new Vector2(x, y - 0.02f);
        if (direction == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
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
