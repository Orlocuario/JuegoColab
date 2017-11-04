using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public GameObject objectToMove;

    public GameObject startPoint;
    public GameObject endPoint;

    public float moveSpeed;

    private Vector3 currentTarget;


    // Use this for initialization
    void Start()
    {


            currentTarget = endPoint.transform.position;


    }

    // Update is called once per frame
    void Update()
    {
        if (objectToMove == null)
        {
            return;
        }

        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, currentTarget, moveSpeed * Time.deltaTime);


        if (objectToMove.transform.position == endPoint.transform.position)
        {
            currentTarget = startPoint.transform.position;
        }

        if (objectToMove.transform.position == startPoint.transform.position)
        {
            currentTarget = endPoint.transform.position;
        }

    }
}
