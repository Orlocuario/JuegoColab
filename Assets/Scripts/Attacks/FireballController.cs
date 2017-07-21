using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour {


    private int direction;
    private float speed;
    float maxDistance;
    float currentDistance;
	// Use this for initialization
	void Start () {
        maxDistance = 2;
        currentDistance = 0;
	}
	
    public void SetMovement(int direction, float speed, float x, float y)
    {
        this.direction = direction;
        this.speed = speed;
        transform.position = new Vector2(x,y-0.02f);
        if(direction == -1)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

	// Update is called once per frame
	void Update () {
        float distance = speed * direction * Time.deltaTime;
        transform.position = transform.position + Vector3.right * distance;
        currentDistance += System.Math.Abs(distance);
        if (maxDistance <= currentDistance)
        {
            Destroy(gameObject);
        }
	}
}
