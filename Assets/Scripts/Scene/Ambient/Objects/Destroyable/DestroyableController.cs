using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableController : MonoBehaviour
{

    protected static float destroyDelayTime = .04f;

    // Use this for initialization
    protected virtual void Start()
    {

    }

    public virtual void DestroyMe()
    {
        // Change sprites with smooth thing
        Destroy(this.gameObject, destroyDelayTime);

    }

}