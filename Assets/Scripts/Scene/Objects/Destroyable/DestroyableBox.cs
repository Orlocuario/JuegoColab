using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBox : DestroyableObject
{
    #region Attributes

    public Sprite brokenBox;

    #endregion

    #region Start

    protected override void Start()
    {
        destroyDelayTime = 2f;
    }

    #endregion

    #region Common

    public override void DestroyMe(bool destroyedFromLocal)
    {

        Collider2D collider = GetComponent<Collider2D>();
        Destroy(collider);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brokenBox;

        base.DestroyMe(destroyedFromLocal);

    }

    #endregion

}