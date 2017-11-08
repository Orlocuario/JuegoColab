using UnityEngine;

/**
 *  This class is for damaging objects that are destroyed on collision 
 *  but don't move such a bomb
 */
public class DestroyableDamagingObject : DamagingObject
{
    public float destroyDelayTime = .04f;

    #region Events

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        Destroy(this.gameObject, destroyDelayTime);
    }

    #endregion

}
