using UnityEngine;

/**
 * This class is for damaging objects that are destroyed on collision
 * and move such a an arrow
 */
public class MovingDamagingObject : DestroyableDamagingObject
{

    public Vector2 startPoint;
    public Vector2 endPoint;

    public float moveSpeed;

    private Vector3 currentTarget;

    // Use this for initialization
    protected virtual void Start()
    {

        if (endPoint != null)
        {
            currentTarget = endPoint;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null || startPoint == null || endPoint == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, endPoint) <= 0f)
        {
            currentTarget = startPoint;
        }

        if (Vector2.Distance(transform.position, startPoint) <= 0f)
        {
            currentTarget = endPoint;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

    }

}
