﻿using System.Collections;
using UnityEngine;

public class MoveTowardsAndDie : MonoBehaviour {

    public Vector3 target;
    public float speed;

    private bool moving;
    private GameObject[] particles;

    void Start () {

        if (target.Equals(default(Vector3))) {
            Debug.LogWarning("No target for movetowardanddie");
        }

    }
	
	void Update () {

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed);
            if (transform.position == target)
            {
                ToogleParticles(false);
                Destroy(gameObject, .1f);
            }
        }
    }

    public void StartMoving(GameObject[] _particles)
    {
        moving = true;
        particles = _particles;
    }

    protected void ToogleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

}
